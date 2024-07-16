using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ClaseDatos;
using iTextSharp.text.pdf;
using iTextSharp.text;
using static AssitADSOproyect.Controllers.LoginController;
using System.Diagnostics;

namespace AssitADSOproyect.Controllers
{
    public class CompetenciasController : Controller
    {
        private BDAssistsADSOv4Entities db = new BDAssistsADSOv4Entities();
        [AutorizarTipoUsuario("Instructor", "InstructorAdmin")]
        // GET: Competencias
        public ActionResult Index(string estadoFiltro = "")
        {
            var CompetenciasFiltradas = db.Competencia.AsQueryable(); // Comienza con todas las competencias

            if (estadoFiltro == "true")
            {
                CompetenciasFiltradas = CompetenciasFiltradas.Where(c => c.Estado_Competencia == true);
            }
            else if (estadoFiltro == "false")
            {
                CompetenciasFiltradas = CompetenciasFiltradas.Where(c => c.Estado_Competencia == false);
            }

            ViewBag.EstadoFiltro = estadoFiltro;

            return View(CompetenciasFiltradas);
        }


        public ActionResult GenerarReportePDF()
        {
            var Competencias = db.Competencia.ToList(); 

            using (MemoryStream memoryStream = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 50, 50, 80, 50);
                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);

                writer.PageEvent = new HeaderFooterEvent(Server.MapPath("~/assets/images/Logo-remove.png")); 

                document.Open();

                // Título
                Paragraph titulo = new Paragraph("Reporte de Competencias", new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD));
                titulo.Alignment = Element.ALIGN_CENTER;
                document.Add(titulo);

                document.Add(Chunk.NEWLINE);

                
                PdfPTable table = new PdfPTable(4); 
                table.WidthPercentage = 100;

                // Encabezados de la tabla
                Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                table.AddCell(new Phrase("Id de Competencia", headerFont));
                table.AddCell(new Phrase("Nombre Competencia"));
                table.AddCell(new Phrase("Tipo Competencia"));
                table.AddCell(new Phrase("Estado Competencia"));
                

               
                Font cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                foreach (var competencia in Competencias)
                {
                    table.AddCell(new Phrase(competencia.Id_competencia.ToString()));
                    table.AddCell(new Phrase(competencia.Nombre_competencia));
                    table.AddCell(new Phrase(competencia.tipo_competencia));
                    table.AddCell(new Phrase(competencia.Estado_Competencia.ToString()));
                    
                }

                document.Add(table);
                document.Close();

               
                return File(memoryStream.ToArray(), "application/pdf", "ReporteCompetencias.pdf");
            }
        }

        class HeaderFooterEvent : PdfPageEventHelper
        {
            private readonly Image _logo;

            public HeaderFooterEvent(string imagePath)
            {
                _logo = Image.GetInstance(imagePath);
                _logo.ScaleToFit(100f, 50f); // Ajustar tamaño de la imagen
            }

            public override void OnOpenDocument(PdfWriter writer, Document document)
            {
                // Calcular la posición del logo para que no se superponga con el contenido
                float logoY = document.PageSize.Height - document.TopMargin - _logo.ScaledHeight;
                _logo.SetAbsolutePosition(document.LeftMargin, logoY);
                document.Add(_logo); // Agregar el logo al inicio del documento
            }

            public override void OnEndPage(PdfWriter writer, Document document)
            {
                // Pie de página (texto centrado)
                Font footerFont = FontFactory.GetFont(FontFactory.HELVETICA, 8);
                Phrase footerText = new Phrase("Generado el " + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + " © AssistADSO. Todos los derechos reservados.", footerFont);
                float textWidth = footerFont.GetCalculatedBaseFont(false).GetWidthPoint(footerText.Content, footerFont.Size);
                float xPosition = (document.PageSize.Width - textWidth) / 2;

                ColumnText.ShowTextAligned(writer.DirectContent, Element.ALIGN_CENTER, footerText, xPosition, document.BottomMargin, 0);
            }
        }

        [HttpPost]
        public ActionResult GenerarReportePDFCompetenciasProgramas(int id)
        {
            try
            {
                var programas = db.Competencia
                .Where(c => c.Id_competencia == id)
                .SelectMany(c => c.Programa_formacion) // Accedemos a los programas a través de la colección de navegación
                .ToList();

                var competencia = db.Competencia.Find(id); // Busca la ficha por su ID
                string nombreCompetencia = competencia?.Nombre_competencia; // Obtiene el código de la ficha

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    Document document = new Document(PageSize.A4.Rotate(), 50, 50, 50, 35);
                    PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);

                    writer.PageEvent = new HeaderFooterEvent(Server.MapPath("~/assets/images/Logo-remove.png")); // Pasar la ruta de la imagen

                    document.Open();

                    // Título
                    Paragraph titulo = new Paragraph("Reporte de Programas relacionados con la competencia " + nombreCompetencia, new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD));
                    titulo.Alignment = Element.ALIGN_CENTER;
                    document.Add(titulo);

                    document.Add(Chunk.NEWLINE);

                    // Agregar contenido al PDF (tabla con datos de las fichas)
                    PdfPTable table = new PdfPTable(5);
                    table.WidthPercentage = 100;

                    // Encabezados de la tabla
                    Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                    table.AddCell(new Phrase("id", headerFont));
                    table.AddCell(new Phrase("Nombre", headerFont));
                    table.AddCell(new Phrase("Tipo", headerFont));
                    table.AddCell(new Phrase("Duracion", headerFont));
                    table.AddCell(new Phrase("Estado", headerFont));

                    // Datos de las fichas
                    Font cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                    foreach (var programascom in programas)
                    {
                        table.AddCell(new Phrase(programascom.Id_programa.ToString()));
                        table.AddCell(new Phrase(programascom.Nombre_programa));
                        table.AddCell(new Phrase(programascom.Tipo_programa));
                        table.AddCell(new Phrase(programascom.Duracion_programa));
                        table.AddCell(new Phrase(programascom.Estado_Programa_formacion.ToString()));
                    }

                    document.Add(table);
                    document.Close();

                    return File(memoryStream.ToArray(), "application/pdf", "ReporteProgramasde" + nombreCompetencia + ".pdf");

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error al generar el PDF: " + ex.Message); // Registro
                return new HttpStatusCodeResult(500, "Error interno del servidor al generar el PDF.");
            }
        }



        // GET: Competencias/Details/5
        [AutorizarTipoUsuario("Instructor", "InstructorAdmin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Competencia competencia = db.Competencia.Find(id);
            if (competencia == null)
            {
                return HttpNotFound();
            }
            return View(competencia);
        }

        // GET: Competencias/Create
        [AutorizarTipoUsuario("Instructor", "InstructorAdmin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Competencias/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AutorizarTipoUsuario("Instructor", "InstructorAdmin")]
        public ActionResult Create([Bind(Include = "Id_competencia,tipo_competencia,Nombre_competencia,Estado_Competencia")] Competencia competencia)
        {
            if (ModelState.IsValid)
            {
                // Verificar si ya existe un programa con el mismo nombre
                bool existeCompetencia = db.Competencia.Any(c => c.Nombre_competencia == competencia.Nombre_competencia);

                if (existeCompetencia)
                {
                    ModelState.AddModelError("Nombre_competencia", "Ya existe una competencia con este nombre.");
                }
                else
                {
                    db.Competencia.Add(competencia);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(competencia);
        }

        // GET: Competencias/Edit/5
        [AutorizarTipoUsuario("Instructor", "InstructorAdmin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Competencia competencia = db.Competencia.Find(id);
            if (competencia == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id_programa = new SelectList(db.Programa_formacion, "Id_programa", "Nombre_programa", competencia.Programa_formacion);
            //ViewBag.Numero_ficha = new SelectList(db.Ficha, "Id_ficha", "Codigo_ficha", competencia.Programa_formacion);
            return View(competencia);
        }

        // POST: Competencias/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AutorizarTipoUsuario("Instructor", "InstructorAdmin")]
        public ActionResult Edit([Bind(Include = "Id_competencia,tipo_competencia,Numero_ficha,Id_programa,Nombre_competencia,Id_usuario,Estado_competencia")] Competencia competencia)
        {
            if (ModelState.IsValid)
            {
                db.Entry(competencia).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Id_programa = new SelectList(db.Programa_formacion, "Id_programa", "Nombre_programa", competencia.Programa_formacion);
            //ViewBag.Numero_ficha = new SelectList(db.Ficha, "Id_ficha", "Codigo_ficha", competencia.Id_ficha);
            return View(competencia);
        }

        public ActionResult CompetenciaProgramas(int id)
        {
            var programas = db.Competencia
                .Where(c => c.Id_competencia == id)
                .SelectMany(c => c.Programa_formacion) // Accedemos a los programas a través de la colección de navegación
                .ToList();

            ViewBag.NombreCompetencia = db.Competencia.Find(id)?.Nombre_competencia; // Obtenemos el nombre de la competencia para mostrar en la vista
            ViewBag.CompetenciaId = id;
            return View(programas);
        }

        public ActionResult CrearProgramaCompetencia(int competenciaId, [Bind(Include = "Id_competencia,Programa_formacion")] Competencia competencia)
        {
            ViewBag.CompetenciaId = competenciaId;

            // Obtener los programas ya asociados a la competencia
            var programasAsociados = db.Competencia
            .Where(c => c.Id_competencia == competenciaId)
            .SelectMany(c => c.Programa_formacion) // Accede a los programas asociados a través de la propiedad de navegación
            .Select(p => p.Id_programa)
            .ToList();

            // Obtener todos los programas y excluir los ya asociados
            ViewBag.ProgramasDisponibles = db.Programa_formacion
                .Where(p => !programasAsociados.Contains(p.Id_programa))
                .ToList();

            ViewBag.NombreCompetencia = db.Competencia.Find(competenciaId)?.Nombre_competencia;
            return View();
        }


        [HttpPost]
        [AutorizarTipoUsuario("Instructor", "InstructorAdmin")]
        public ActionResult CrearProgramaCompetencia(int competenciaId, int programaId)
        {
            var competencia = db.Competencia.Find(competenciaId);
            var programa = db.Programa_formacion.Find(programaId);

            if (competencia != null && programa != null)
            {
                if (!competencia.Programa_formacion.Contains(programa))
                {
                    competencia.Programa_formacion.Add(programa);
                    db.SaveChanges();
                }
            }
            else
            {
                // Manejar el caso en que la competencia o el programa no existen
            }
            ViewBag.NombreCompetencia = db.Competencia.Find(competenciaId)?.Nombre_competencia; // Obtenemos el nombre de la competencia para mostrar en la vista
            ViewBag.CompetenciaId = competenciaId;
            return RedirectToAction("CompetenciaProgramas", new { id = competenciaId });
        }





        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
