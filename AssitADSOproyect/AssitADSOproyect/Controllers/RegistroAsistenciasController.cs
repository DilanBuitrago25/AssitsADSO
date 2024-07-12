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
    public class RegistroAsistenciasController : Controller
    {
        private BDAssistsADSOv4Entities db = new BDAssistsADSOv4Entities();
        [AutorizarTipoUsuario("Aprendiz")]
        // GET: RegistroAsistencias
        public ActionResult Index(string estadoFiltro = "")
        {
            string idUsuarioSesion = Session["Idusuario"].ToString();

            var RegistrosFiltrados = db.RegistroAsistencia
                                         .Where(r => r.Id_Aprendiz.ToString() == idUsuarioSesion);

            if (estadoFiltro == "true")
            {
                RegistrosFiltrados = RegistrosFiltrados.Where(c => c.Estado_RegistroAsitencia == true);
            }
            else if (estadoFiltro == "false")
            {
                RegistrosFiltrados = RegistrosFiltrados.Where(c => c.Estado_RegistroAsitencia == false);
            }

            ViewBag.EstadoFiltro = estadoFiltro;

            return View(RegistrosFiltrados);
        }

        public ActionResult GenerarReportePDF()
        {
            string idUsuarioSesion = Session["Idusuario"].ToString();

            var RegistrosFiltrados = db.RegistroAsistencia
                                         .Where(r => r.Id_Aprendiz.ToString() == idUsuarioSesion);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 50, 50, 80, 50);
                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);

                writer.PageEvent = new HeaderFooterEvent(Server.MapPath("~/assets/images/Logo-remove.png"));

                document.Open();

                // Título
                Paragraph titulo = new Paragraph("Reporte de Registros de Asistencias", new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD));
                titulo.Alignment = Element.ALIGN_CENTER;
                document.Add(titulo);

                document.Add(Chunk.NEWLINE);


                PdfPTable table = new PdfPTable(7);
                table.WidthPercentage = 100;

                // Encabezados de la tabla
                Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                table.AddCell(new Phrase("Id de Registro", headerFont));
                table.AddCell(new Phrase("Fecha Registro"));
                table.AddCell(new Phrase("Hora Registro"));
                table.AddCell(new Phrase("Fecha Asistencia"));
                table.AddCell(new Phrase("Aprendiz"));
                table.AddCell(new Phrase("Estado Asistencia"));
                table.AddCell(new Phrase("Estado Registro"));




                Font cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                foreach (var registros in RegistrosFiltrados)
                {
                    table.AddCell(new Phrase(registros.Id_Registroasistencia.ToString()));
                    table.AddCell(new Phrase(registros.Fecha_registro));
                    table.AddCell(new Phrase(registros.Hora_registro));
                    table.AddCell(new Phrase(registros.Asistencia.Fecha_asistencia));
                    table.AddCell(new Phrase(registros.Usuario.Nombre_usuario));
                    table.AddCell(new Phrase(registros.Asistio_registro.ToString()));
                    table.AddCell(new Phrase(registros.Estado_RegistroAsitencia.ToString()));

                }

                document.Add(table);
                document.Close();


                return File(memoryStream.ToArray(), "application/pdf", "ReporteRegistrosAsistencias.pdf");
            }
        }

        class HeaderFooterEvent : PdfPageEventHelper
        {
            private readonly Image _logo;

            public HeaderFooterEvent(string imagePath)
            {
                _logo = Image.GetInstance(imagePath);
                _logo.ScaleToFit(80f, 50f); // Ajustar tamaño de la imagen
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

        // GET: RegistroAsistencias/Details/5
        [AutorizarTipoUsuario("Aprendiz")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RegistroAsistencia registroAsistencia = db.RegistroAsistencia.Find(id);
            if (registroAsistencia == null)
            {
                return HttpNotFound();
            }
            return View(registroAsistencia);
        }

        // GET: RegistroAsistencias/Create
        [AllowAnonymous]
        public ActionResult Create(int? Id_Asistencia, string fechaFin)
        {
            if (Session["IdUsuario"] == null)
            {
                TempData["ReturnUrl"] = Url.Action("Create", "RegistroAsistencias", new { id_Asistencia = Id_Asistencia });

                return RedirectToAction("Index", "Login");
            }
            if (Session["TipoUsuario"].ToString() != "Aprendiz")
            {
                return RedirectToAction("Error401", "Home");
            }
            if (Id_Asistencia == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var asistencia = db.Asistencia.Find(Id_Asistencia);// Obtener la asistencia completa
            if (asistencia == null)
            {
                return HttpNotFound();
            }
            var registroAsistencia = new RegistroAsistencia { 
                Id_asistencia = Id_Asistencia.Value,
                Id_Aprendiz = (int)Session["IdUsuario"]
            };
            ViewBag.CodigoFicha = asistencia.Ficha.Codigo_ficha; // Pasar el código de ficha a la vista
            ViewBag.Nombre_competencia = asistencia.Competencia.Nombre_competencia; // Pasar el nombre de la competencia a la vista
            ViewBag.Nombre_Aprendiz = asistencia.Usuario.Nombre_usuario;
            ViewBag.Id_asistencia = new SelectList(db.Asistencia, "Id_asistencia", "Fecha_inicio_asistencia");
            ViewBag.Id_usuario = new SelectList(db.Usuario, "Id_usuario", "Tipo_Documento_usuario");
            return View(registroAsistencia);
        }

        // POST: RegistroAsistencias/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AutorizarTipoUsuario("Aprendiz")]
        public ActionResult Create([Bind(Include = "Id_Registroasistencia,Fecha_registro,Hora_registro,Id_asistencia,Id_Aprendiz,Asistio_registro,Estado_RegistroAsistencia")] RegistroAsistencia registroAsistencia)
        {
            if (ModelState.IsValid)
            {
                // Obtener la asistencia correspondiente al Id_asistencia
                var asistencia = db.Asistencia.Find(registroAsistencia.Id_asistencia);

                if (asistencia != null)
                {
                    // Parsear las fechas y horas (asumiendo que están en formato "yyyy-MM-dd" y "HH:mm")
                    if (DateTime.TryParse(asistencia.Fecha_asistencia, out DateTime fechaFin) &&
                        DateTime.TryParse(asistencia.Hora_fin_asistencia, out DateTime horaFin))
                    {
                        // Combinar la fecha y hora de fin de la asistencia
                        DateTime fechaHoraFinAsistencia = fechaFin.Date + horaFin.TimeOfDay;

                        // Verificar si la fecha y hora de fin ya pasaron
                        if (fechaHoraFinAsistencia < DateTime.Now)
                        {
                            ModelState.AddModelError("Id_asistencia", "La asistencia seleccionada ya ha finalizado. No se puede registrar.");

                            // Recargar el ViewBag para el dropdown list de asistencias
                            ViewBag.Id_asistencia = new SelectList(db.Asistencia, "Id_asistencia", "Fecha_inicio_asistencia", registroAsistencia.Id_asistencia);

                            return View(registroAsistencia); // Volver a la vista con el mensaje de error
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Error al analizar la fecha y hora de fin de la asistencia.");
                        return View(registroAsistencia);
                    }
                }

                db.RegistroAsistencia.Add(registroAsistencia);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Id_asistencia = new SelectList(db.Asistencia, "Id_asistencia", "Fecha_inicio_asistencia", registroAsistencia.Id_asistencia);
            ViewBag.Id_usuario = new SelectList(db.Usuario, "Id_usuario", "Tipo_Documento_usuario", registroAsistencia.Id_Aprendiz);
            ViewBag.CodigoFicha = new SelectList(db.Usuario, "Id_ficha", "Codigo_ficha", registroAsistencia.Asistencia.Ficha.Codigo_ficha); // Pasar el código de ficha a la vista
            ViewBag.Nombre_competencia = new SelectList(db.Usuario, "Id_competencia", "Nombre_competencia", registroAsistencia.Asistencia.Competencia.Nombre_competencia); // Pasar el nombre de la competencia a la vista
            return View(registroAsistencia);
        }

        // GET: RegistroAsistencias/Edit/5
        [AutorizarTipoUsuario("Aprendiz")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RegistroAsistencia registroAsistencia = db.RegistroAsistencia.Find(id);
            if (registroAsistencia == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id_asistencia = new SelectList(db.Asistencia, "Id_asistencia", "Fecha_inicio_asistencia", registroAsistencia.Id_asistencia);
            ViewBag.Id_usuario = new SelectList(db.Usuario, "Id_usuario", "Tipo_Documento_usuario", registroAsistencia.Id_Aprendiz);
            return View(registroAsistencia);
        }

        // POST: RegistroAsistencias/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id_Registroasistencia,Fecha_registro,Hora_registro,Id_asistencia,Id_usuario")] RegistroAsistencia registroAsistencia)
        {
            if (ModelState.IsValid)
            {
                db.Entry(registroAsistencia).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Id_asistencia = new SelectList(db.Asistencia, "Id_asistencia", "Fecha_inicio_asistencia", registroAsistencia.Id_asistencia);
            ViewBag.Id_usuario = new SelectList(db.Usuario, "Id_usuario", "Tipo_Documento_usuario", registroAsistencia.Id_Aprendiz);
            return View(registroAsistencia);
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
