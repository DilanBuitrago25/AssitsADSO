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
    public class Programa_formacionController : Controller
    {
        private BDAssistsADSOv4Entities db = new BDAssistsADSOv4Entities();
        [AutorizarTipoUsuario("Instructor", "InstructorAdmin")]
        // GET: Programa_formacion
        public ActionResult Index(string estadoFiltro = "")
        {
            var ProgramasFiltrados = db.Programa_formacion.AsQueryable(); // Comienza con todas las competencias

            if (estadoFiltro == "true")
            {
                ProgramasFiltrados = ProgramasFiltrados.Where(c => c.Estado_Programa_formacion == true);
            }
            else if (estadoFiltro == "false")
            {
                ProgramasFiltrados = ProgramasFiltrados.Where(c => c.Estado_Programa_formacion == false);
            }

            ViewBag.EstadoFiltro = estadoFiltro;

            return View(ProgramasFiltrados);
        }


        public ActionResult GenerarReportePDF()
        {
            var programa_formacion = db.Programa_formacion.ToList();

            using (MemoryStream memoryStream = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 50, 50, 80, 50);
                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);

                writer.PageEvent = new HeaderFooterEvent(Server.MapPath("~/assets/images/Logo-remove.png"));

                document.Open();

                // Título
                Paragraph titulo = new Paragraph("Reporte de Programas de Formacion", new Font(Font.FontFamily.HELVETICA, 15, Font.BOLD));
                titulo.Alignment = Element.ALIGN_CENTER;
                document.Add(titulo);

                document.Add(Chunk.NEWLINE);


                PdfPTable table = new PdfPTable(5);
                table.WidthPercentage = 100;

                // Encabezados de la tabla
                Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                table.AddCell(new Phrase("Id de Programa", headerFont));
                table.AddCell(new Phrase("Nombre Programa"));
                table.AddCell(new Phrase("Tipo de Programa"));
                table.AddCell(new Phrase("Duracion Programa"));
                table.AddCell(new Phrase("Estado Programa"));



                Font cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                foreach (var programa in programa_formacion)
                {
                    table.AddCell(new Phrase(programa.Id_programa.ToString()));
                    table.AddCell(new Phrase(programa.Nombre_programa));
                    table.AddCell(new Phrase(programa.Tipo_programa));
                    table.AddCell(new Phrase(programa.Duracion_programa));                 
                    table.AddCell(new Phrase(programa.Estado_Programa_formacion.ToString()));

                }

                document.Add(table);
                document.Close();


                return File(memoryStream.ToArray(), "application/pdf", "ReporteProgramasFormacion.pdf");
            }
        }

        [HttpPost]
        public ActionResult GenerarReportePDFProgramasCompetencias(int id)
        {
            try
            {
                var competencias = db.Programa_formacion
                .Where(pf => pf.Id_programa == id)
                .SelectMany(pf => pf.Competencia)
                .ToList();

                var programa = db.Programa_formacion.Find(id); // Busca la ficha por su ID
                string nombrePrograma = programa?.Nombre_programa; // Obtiene el código de la ficha

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    Document document = new Document(PageSize.A4.Rotate(), 50, 50, 50, 35);
                    PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);

                    writer.PageEvent = new HeaderFooterEvent(Server.MapPath("~/assets/images/Logo-remove.png")); // Pasar la ruta de la imagen

                    document.Open();

                    // Título
                    Paragraph titulo = new Paragraph("Reporte de Competencias relacionados con el programa " + nombrePrograma, new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD));
                    titulo.Alignment = Element.ALIGN_CENTER;
                    document.Add(titulo);

                    document.Add(Chunk.NEWLINE);

                    // Agregar contenido al PDF (tabla con datos de las fichas)
                    PdfPTable table = new PdfPTable(4);
                    table.WidthPercentage = 100;

                    // Encabezados de la tabla
                    Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                    table.AddCell(new Phrase("id", headerFont));
                    table.AddCell(new Phrase("Nombre", headerFont));
                    table.AddCell(new Phrase("Tipo", headerFont));
                    table.AddCell(new Phrase("Estado", headerFont));

                    // Datos de las fichas
                    Font cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                    foreach (var competenciaspro in competencias)
                    {
                        table.AddCell(new Phrase(competenciaspro.Id_competencia.ToString()));
                        table.AddCell(new Phrase(competenciaspro.Nombre_competencia));
                        table.AddCell(new Phrase(competenciaspro.tipo_competencia));
                        table.AddCell(new Phrase(competenciaspro.Estado_Competencia.ToString()));
                    }

                    document.Add(table);
                    document.Close();

                    return File(memoryStream.ToArray(), "application/pdf", "ReporteCompetenciasde" + nombrePrograma + ".pdf");

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error al generar el PDF: " + ex.Message); // Registro
                return new HttpStatusCodeResult(500, "Error interno del servidor al generar el PDF.");
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

            public override void OnEndPage(PdfWriter writer, Document document)
            {
                // Encabezado (imagen alineada a la izquierda)
                _logo.SetAbsolutePosition(document.LeftMargin, document.PageSize.Height - document.TopMargin - _logo.ScaledHeight);
                document.Add(_logo);

                // Pie de página (texto centrado)
                Font footerFont = FontFactory.GetFont(FontFactory.HELVETICA, 8);
                Phrase footerText = new Phrase("Generado el " + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + " © AssistADSO. Todos los derechos reservados.", footerFont);
                float textWidth = footerFont.GetCalculatedBaseFont(false).GetWidthPoint(footerText.Content, footerFont.Size);
                float xPosition = (document.PageSize.Width - textWidth) / 2;

                ColumnText.ShowTextAligned(writer.DirectContent, Element.ALIGN_CENTER, footerText, xPosition, document.BottomMargin, 0);
            }
        }

        // GET: Programa_formacion/Details/5
        [AutorizarTipoUsuario("Instructor", "InstructorAdmin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Programa_formacion programa_formacion = db.Programa_formacion.Find(id);
            if (programa_formacion == null)
            {
                return HttpNotFound();
            }
            return View(programa_formacion);
        }

        // GET: Programa_formacion/Create
        [AutorizarTipoUsuario("Instructor", "InstructorAdmin")]
        public ActionResult Create()
        {
            
            return View();
        }

        // POST: Programa_formacion/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id_programa,Nombre_programa,Tipo_programa,Duracion_programa,Estado_Programa_formacion")] Programa_formacion programa_formacion)
        {
            bool existePrograma = db.Programa_formacion.Any(p => p.Nombre_programa == programa_formacion.Nombre_programa);

            if (existePrograma)
            {
                ModelState.AddModelError("Nombre_programa", "Ya existe un programa con este nombre.");
            }
            else
            {
                db.Programa_formacion.Add(programa_formacion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(programa_formacion);
        }

        // GET: Programa_formacion/Edit/5
        [AutorizarTipoUsuario("Instructor", "InstructorAdmin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Programa_formacion programa_formacion = db.Programa_formacion.Find(id);
            if (programa_formacion == null)
            {
                return HttpNotFound();
            }
            return View(programa_formacion);
        }

        // POST: Programa_formacion/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id_programa,Nombre_programa,Tipo_programa,Duracion_programa")] Programa_formacion programa_formacion) 
        {
            if (ModelState.IsValid)
            {
                db.Entry(programa_formacion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            return View(programa_formacion);
        }


        //--- Despues de aqui relaciones foraneas
        [AutorizarTipoUsuario("Instructor", "InstructorAdmin")]
        public ActionResult Programas_Competencias(int id)
        {
            var competencias = db.Programa_formacion
                .Where(pf => pf.Id_programa == id)
                .SelectMany(pf => pf.Competencia) 
                .ToList();

            ViewBag.NombrePrograma = db.Programa_formacion.Find(id).Nombre_programa; 
            ViewBag.ProgramaId = id;
            return View(competencias);
        }


        // GET: Programa_formacion_competencia/Asociación
        [AutorizarTipoUsuario("Instructor", "InstructorAdmin")]
        public ActionResult Programas_Competencias_Asociar(int programaId, [Bind(Include = "Id_programa,Competencia")] Programa_formacion programa_formacion)
        {


            ViewBag.ProgramaId = programaId;
            

            var competenciasAsociados = db.Programa_formacion
            .Where(c => c.Id_programa == programaId)
            .SelectMany(c => c.Competencia) // Accede a los programas asociados a través de la propiedad de navegación
            .Select(p => p.Id_competencia)
            .ToList();

            ViewBag.CompetenciasDisponibles = db.Competencia
               .Where(p => !competenciasAsociados.Contains(p.Id_competencia))
               .ToList();

            ViewBag.NombrePrograma = db.Programa_formacion.Find(programaId).Nombre_programa;

            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [AutorizarTipoUsuario("Instructor", "InstructorAdmin")]
        public ActionResult Programas_Competencias_Asociar(int programaId, int competenciaId)
        {
            var programa = db.Programa_formacion.Find(programaId);
            var competencia = db.Competencia.Find(competenciaId);

            if (programa != null && competencia != null)
            {
                if (!programa.Competencia.Contains(competencia))
                {
                    programa.Competencia.Add(competencia);
                    db.SaveChanges();
                }
            }
            else
            {
                // Manejar el caso en que la competencia o el programa no existen
            }
            ViewBag.NombrePrograma = db.Programa_formacion.Find(programaId).Nombre_programa;// Obtenemos el nombre de la competencia para mostrar en la vista
            ViewBag.ProgramaId = programaId;
            return RedirectToAction("Programas_Competencias", new { id = programaId });
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
