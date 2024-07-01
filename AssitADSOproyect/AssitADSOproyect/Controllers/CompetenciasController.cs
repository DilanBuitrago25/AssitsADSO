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

                
                PdfPTable table = new PdfPTable(6); 
                table.WidthPercentage = 100;

                // Encabezados de la tabla
                Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                table.AddCell(new Phrase("Id de Competencia", headerFont));
                table.AddCell(new Phrase("Nombre Competencia"));
                table.AddCell(new Phrase("Tipo Competencia"));
                table.AddCell(new Phrase("Codigo Ficha"));
                table.AddCell(new Phrase("Programa Formacion"));
                table.AddCell(new Phrase("Estado Competencia"));
                

               
                Font cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                foreach (var competencia in Competencias)
                {
                    table.AddCell(new Phrase(competencia.Id_competencia.ToString()));
                    table.AddCell(new Phrase(competencia.Nombre_competencia));
                    table.AddCell(new Phrase(competencia.tipo_competencia));
                    //table.AddCell(new Phrase(competencia.Ficha.Codigo_ficha.ToString()));
                    table.AddCell(new Phrase(competencia.Programa_formacion.ToString()));
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

            public override void OnEndPage(PdfWriter writer, Document document)
            {
                // Encabezado (imagen alineada a la izquierda)
                _logo.SetAbsolutePosition(document.LeftMargin, document.PageSize.Height - document.TopMargin - _logo.ScaledHeight);
                document.Add(_logo);

                // Pie de página (texto centrado)
                Font footerFont = FontFactory.GetFont(FontFactory.HELVETICA, 8);
                Phrase footerText = new Phrase("Generado el " + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + " © AssisdsdsstADSO. Todos los derechos reservados.", footerFont);
                float textWidth = footerFont.GetCalculatedBaseFont(false).GetWidthPoint(footerText.Content, footerFont.Size);
                float xPosition = (document.PageSize.Width - textWidth) / 2;

                ColumnText.ShowTextAligned(writer.DirectContent, Element.ALIGN_CENTER, footerText, xPosition, document.BottomMargin, 0);
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
                db.Competencia.Add(competencia);
                db.SaveChanges();
                return RedirectToAction("Index");
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

        public ActionResult CrearProgramaCompetencia(int competenciaId)
        {
            ViewBag.CompetenciaId = competenciaId;
            ViewBag.ProgramasDisponibles = db.Programa_formacion.ToList(); // Obtén todos los programas disponibles

            return View();
        }

        [HttpPost]
        public ActionResult GuardarRelacionProgramaCompetencia(int competenciaId, int programaId)
        {
            // Obtener la competencia y el programa desde la base de datos
            var competencia = db.Competencia.Find(competenciaId);
            var programa = db.Programa_formacion.Find(programaId);

            if (competencia != null && programa != null)
            {
                // Verificar si la relación ya existe (opcional)
                if (!competencia.Programa_formacion.Contains(programa))
                {
                    // Agregar el programa a la colección de navegación de la competencia
                    competencia.Programa_formacion.Add(programa);

                    // Guardar los cambios en la base de datos
                    db.SaveChanges();
                }
            }
            //else
            //{
            //    // Manejar el caso en que la competencia o el programa no existen
            //}

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
