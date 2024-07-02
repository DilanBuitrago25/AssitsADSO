using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Windows.Media.Imaging;
using ClaseDatos;
using iTextSharp.text.pdf;
using iTextSharp.text;
using static AssitADSOproyect.Controllers.LoginController;

namespace AssitADSOproyect.Controllers
{
    public class FichasController : Controller
    {
        private BDAssistsADSOv4Entities db = new BDAssistsADSOv4Entities();

        // GET: Fichas1
        [AutorizarTipoUsuario("Instructor", "InstructorAdmin")]
        public ActionResult Index(string estadoFiltro = "")
        {
            var fichasFiltradas = db.Ficha
                .Where(f => f.Estado_ficha == true) // Filtrar por Estado_Ficha = true
                .Where(f => estadoFiltro == "" || f.Estado_ficha.ToString() == estadoFiltro) // Filtrar por estado (opcional)
                .ToList();

            ViewBag.EstadoFiltro = estadoFiltro;

            return View(fichasFiltradas);
        }






        // ...

        public ActionResult GenerarReportePDF()
    {
        var fichas = db.Ficha.ToList(); // Obtén los datos de las fichas

        using (MemoryStream memoryStream = new MemoryStream())
        {
                Document document = new Document(PageSize.A4, 50, 50, 80, 50);
                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);

                writer.PageEvent = new HeaderFooterEvent(Server.MapPath("~/assets/images/Logo-remove.png")); // Pasar la ruta de la imagen

                document.Open();

                // Título
                Paragraph titulo = new Paragraph("Reporte de Fichas", new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD));
                titulo.Alignment = Element.ALIGN_CENTER;
                document.Add(titulo);

                document.Add(Chunk.NEWLINE);

                // Agregar contenido al PDF (tabla con datos de las fichas)
                PdfPTable table = new PdfPTable(8); // 4 columnas (ajusta según tus campos)
            table.WidthPercentage = 100;

                // Encabezados de la tabla
                Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                table.AddCell(new Phrase("Código Ficha", headerFont));
                table.AddCell(new Phrase("Jornada Ficha"));
                table.AddCell(new Phrase("Modalidad Ficha"));
                table.AddCell(new Phrase("Tipo Ficha"));
                table.AddCell(new Phrase("Fecha Inicio Ficha"));
                table.AddCell(new Phrase("Fecha Fin Ficha"));
                table.AddCell(new Phrase("Programa de Formacion"));
                table.AddCell(new Phrase("Estado Ficha"));
                // ... (agrega más encabezados según tus campos)

                // Datos de las fichas
                Font cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                foreach (var ficha in fichas)
                {
                table.AddCell(new Phrase(ficha.Codigo_ficha.ToString()));
                table.AddCell(new Phrase(ficha.Jornada_ficha));
                    table.AddCell(new Phrase(ficha.Modalidad_ficha));
                    table.AddCell(new Phrase(ficha.tipo_ficha));
                    table.AddCell(new Phrase(ficha.Fecha_inicio));
                    table.AddCell(new Phrase(ficha.Fecha_fin));
                    table.AddCell(new Phrase(ficha.Programa_formacion.Nombre_programa));
                    table.AddCell(new Phrase(ficha.Estado_ficha.ToString()));// Ajusta según tus relaciones
                                                                                        // ... (agrega más datos de las fichas)
                }

            document.Add(table);
            document.Close();

            // Devolver el PDF como archivo descargable
            return File(memoryStream.ToArray(), "application/pdf", "ReporteFichas.pdf");
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

        // GET: Fichas1/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ficha ficha = db.Ficha.Find(id);
            if (ficha == null)
            {
                return HttpNotFound();
            }
            return View(ficha);
        }

        // GET: Fichas1/Create
   
        public ActionResult Create()
        {
            ViewBag.Id_programa = new SelectList(db.Programa_formacion, "Id_programa", "Nombre_programa");
            return View();
        }

        // POST: Fichas1/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id_ficha,Codigo_ficha,Jornada_ficha,Modalidad_ficha,tipo_ficha,Id_programa,Fecha_inicio,Fecha_fin,Id_Instructor,Estado_ficha")] Ficha ficha)
        {
            if (ModelState.IsValid)
            {
                db.Ficha.Add(ficha);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Id_programa = new SelectList(db.Programa_formacion, "Id_programa", "Nombre_programa", ficha.Id_programa);
            return View(ficha);
        }

        // GET: Fichas1/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ficha ficha = db.Ficha.Find(id);
            if (ficha == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id_programa = new SelectList(db.Programa_formacion, "Id_programa", "Nombre_programa", ficha.Id_programa);
            return View(ficha);
        }

        // POST: Fichas1/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id_ficha,Codigo_ficha,Jornada_ficha,Modalidad_ficha,tipo_ficha,Id_programa,Fecha_inicio,Fecha_fin,Id_usuario,Estado_ficha")] Ficha ficha)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ficha).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Id_programa = new SelectList(db.Programa_formacion, "Id_programa", "Nombre_programa", ficha.Id_programa);
            return View(ficha);
        }

        // Foranea despues de aca
        public ActionResult Aprendices_Ficha(int id)
        {
            var aprendices = db.Ficha_has_Usuario
                .Where(fu => fu.Id_ficha == id && fu.TipoUsuario == "Aprendiz")
                .Select(fu => fu.Usuario)
                .ToList();

            ViewBag.FichaId = id; // Pasar el ID de la ficha a la vista

            return View(aprendices);
        }

        public ActionResult Asociar_Aprendiz_Ficha(int idFicha)
        {
            var aprendices = db.Usuario.Where(u => u.Tipo_usuario == "Aprendiz").ToList();
            ViewBag.Aprendices = aprendices;
            ViewBag.FichaId = idFicha;

            return View(new Ficha_has_Usuario { Id_ficha = idFicha }); // Inicializa el modelo con el ID de la ficha
        }

        [HttpPost]
        public ActionResult Asociar_Aprendiz_Ficha(Ficha_has_Usuario fichaHasUsuario)
        {
            if (ModelState.IsValid)
            {
                var aprendiz = db.Usuario.Find(fichaHasUsuario.Id_usuario);
                var ficha = db.Ficha.Find(fichaHasUsuario.Id_ficha); // Cargar la ficha

                if (aprendiz != null && aprendiz.Tipo_usuario == "Aprendiz" && ficha != null)
                {
                    fichaHasUsuario.TipoUsuario = aprendiz.Tipo_usuario;
                    db.Ficha_has_Usuario.Add(fichaHasUsuario);
                    db.SaveChanges();
                    return RedirectToAction("Asociar_Aprendiz_Ficha", new { idFicha = fichaHasUsuario.Id_ficha });
                }
                else
                {
                    ModelState.AddModelError("", "El usuario seleccionado no es un aprendiz válido o la ficha no existe.");
                }
            }

            // Si hay errores de validación, volver a mostrar el formulario
            ViewBag.Aprendices = db.Usuario.Where(u => u.Tipo_usuario == "Aprendiz").ToList();
            return View(fichaHasUsuario);
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
