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
    public class Programa_formacionController : Controller
    {
        private BDAssistsADSOv2Entities db = new BDAssistsADSOv2Entities();
        [AutorizarTipoUsuario("Instructor", "InstructorAdmin")]
        // GET: Programa_formacion
        public ActionResult Index(string estadoFiltro = "")
        {
            string idUsuarioSesion = Session["Idusuario"].ToString();

            var ProgramasFiltradas = db.Programa_formacion
                                         .Where(f => f.Id_Usuario.ToString() == idUsuarioSesion);

            if (estadoFiltro == "true")
            {
                ProgramasFiltradas = ProgramasFiltradas.Where(f => f.Estado_Programa_formacion == true);
            }
            else if (estadoFiltro == "false")
            {
                ProgramasFiltradas = ProgramasFiltradas.Where(f => f.Estado_Programa_formacion == false);
            }

            ViewBag.EstadoFiltro = estadoFiltro;

            return View(ProgramasFiltradas);
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
                Paragraph titulo = new Paragraph("Reporte de Programas de Formacion", new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD));
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

        // GET: Programa_formacion/Details/5
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
        public ActionResult Create()
        {
            
            return View();
        }

        // POST: Programa_formacion/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id_programa,Nombre_programa,Tipo_programa,Duracion_programa,Id_usuario")] Programa_formacion programa_formacion)
        {
            if (ModelState.IsValid)
            {
                db.Programa_formacion.Add(programa_formacion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(programa_formacion);
        }

        // GET: Programa_formacion/Edit/5
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
        public ActionResult Edit([Bind(Include = "Id_programa,Nombre_programa,Tipo_programa,Duracion_programa")] Programa_formacion programa_formacion) /* posibilidad de implementar el id usaurio con el session*/
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

        public ActionResult Programas_Competencias(int id)
        {
            var competencias = db.Programa_formacion
                .Where(pf => pf.Id_programa == id)
                .SelectMany(pf => pf.Competencia1) // Obtener todas las competencias asociadas
                .ToList();

            ViewBag.NombrePrograma = db.Programa_formacion.Find(id).Nombre_programa; // Obtener el nombre del programa

            return View(competencias);
        }

        // GET: Programa_formacion_competencia/Asociación
        public ActionResult Programas_Competencias_Asociar(int id)
        {
            //ViewBag.NombrePrograma = new SelectList(db.Programa_formacion, "id_programa", "Nombre_programa");
            ViewBag.NombrePrograma = db.Programa_formacion.Find(id).Nombre_programa;
            ViewBag.Competencias = new SelectList(db.Competencia, "id_competencia", "Nombre_competencia");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Programas_Competencias_Asociar([Bind(Include = "Id_programa,Competencia1")] Programa_formacion programa_formacion, Competencia competencia)
        {
            if (ModelState.IsValid)
            {
                db.Programa_formacion.Add(programa_formacion);
                db.Competencia.Add(competencia);
                db.SaveChanges();
                return RedirectToAction("Programas_Competencias");
            }
            ViewBag.Competencias = new SelectList(db.Competencia, "id_competencia", "Nombre_competencia", programa_formacion.Competencia1);
            ViewBag.NombrePrograma = new SelectList(db.Programa_formacion, "id_competencia", "Nombre_programa", competencia.Programa_formacion1);
            return View(programa_formacion);
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
