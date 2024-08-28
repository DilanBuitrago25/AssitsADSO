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
            var CompetenciasFiltradas = db.Competencia.AsQueryable(); 

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
                _logo.ScaleToFit(100f, 50f); 
            }

            public override void OnOpenDocument(PdfWriter writer, Document document)
            {

                float logoY = document.PageSize.Height - document.TopMargin - _logo.ScaledHeight;
                _logo.SetAbsolutePosition(document.LeftMargin, logoY);
                document.Add(_logo); 
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
                .SelectMany(c => c.Programa_formacion) 
                .ToList();

                var competencia = db.Competencia.Find(id); 
                string nombreCompetencia = competencia?.Nombre_competencia; 

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    Document document = new Document(PageSize.A4.Rotate(), 50, 50, 50, 35);
                    PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);

                    writer.PageEvent = new HeaderFooterEvent(Server.MapPath("~/assets/images/Logo-remove.png")); 

                    document.Open();

                 
                    Paragraph titulo = new Paragraph("Reporte de Programas relacionados con la competencia " + nombreCompetencia, new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD));
                    titulo.Alignment = Element.ALIGN_CENTER;
                    document.Add(titulo);

                    document.Add(Chunk.NEWLINE);


                    PdfPTable table = new PdfPTable(5);
                    table.WidthPercentage = 100;

   
                    Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                    table.AddCell(new Phrase("id", headerFont));
                    table.AddCell(new Phrase("Nombre", headerFont));
                    table.AddCell(new Phrase("Tipo", headerFont));
                    table.AddCell(new Phrase("Duracion", headerFont));
                    table.AddCell(new Phrase("Estado", headerFont));


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
                Debug.WriteLine("Error al generar el PDF: " + ex.Message); 
                return new HttpStatusCodeResult(500, "Error interno del servidor al generar el PDF.");
            }
        }




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

   
        [AutorizarTipoUsuario("Instructor", "InstructorAdmin")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AutorizarTipoUsuario("Instructor", "InstructorAdmin")]
        public ActionResult Create([Bind(Include = "Id_competencia,tipo_competencia,Nombre_competencia,Estado_Competencia")] Competencia competencia)
        {
            if (ModelState.IsValid)
            {
              
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
            
            return View(competencia);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AutorizarTipoUsuario("Instructor", "InstructorAdmin")]
        public ActionResult Edit([Bind(Include = "Id_competencia,tipo_competencia,Nombre_competencia,Estado_competencia")] Competencia competencia)
        {
            if (ModelState.IsValid)
            {
                db.Entry(competencia).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Id_programa = new SelectList(db.Programa_formacion, "Id_programa", "Nombre_programa", competencia.Programa_formacion);
          
            return View(competencia);
        }

        public ActionResult CompetenciaProgramas(int id)
        {
            var programas = db.Competencia
                .Where(c => c.Id_competencia == id)
                .SelectMany(c => c.Programa_formacion) 
                .ToList();

            ViewBag.NombreCompetencia = db.Competencia.Find(id)?.Nombre_competencia; 
            ViewBag.CompetenciaId = id;
            return View(programas);
        }

        public ActionResult CrearProgramaCompetencia(int competenciaId, [Bind(Include = "Id_competencia,Programa_formacion")] Competencia competencia)
        {
            ViewBag.CompetenciaId = competenciaId;

 
            var programasAsociados = db.Competencia
            .Where(c => c.Id_competencia == competenciaId)
            .SelectMany(c => c.Programa_formacion) 
            .Select(p => p.Id_programa)
            .ToList();

          
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
           
            ViewBag.NombreCompetencia = db.Competencia.Find(competenciaId)?.Nombre_competencia; 
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
