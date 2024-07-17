using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration.Internal;
using System.Web.Mvc;
using ClaseDatos;
using Ganss.Xss;
using System.Web.Hosting;
using static AssitADSOproyect.Controllers.LoginController;
using Antlr.Runtime.Misc;
using System.Data.Entity.Infrastructure;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Diagnostics;

namespace AssitADSOproyect.Controllers
{
    public class JustificacionInasistenciaController : Controller
    {
        private BDAssistsADSOv4Entities db = new BDAssistsADSOv4Entities();
        [AutorizarTipoUsuario("Instructor", "InstructorAdmin", "Aprendizs")]
        // GET: JustificacionInasistencia
        public ActionResult Index()
        {
            int instructorId = (int)Session["Idusuario"];

            var soportes = db.Soporte.Include(s => s.Asistencia)
                                    .Where(s => s.Id_Instructor == instructorId && s.Validacion_Instructor == null);

            return View(soportes.ToList());


        }

        public ActionResult FiltrarSoportes(string filtro)
{
    int instructorId = (int)Session["Idusuario"];
    var soportes = db.Soporte.Include(s => s.Asistencia).Where(s => s.Id_Instructor == instructorId);

    switch (filtro)
    {
        case "BandejaDeEntrada":
            break; // No se aplica ningún filtro adicional
        case "NoLeidos":
            soportes = soportes.Where(s => s.Validacion_Instructor == null);
            break;
        case "Leidos":
            soportes = soportes.Where(s => s.Validacion_Instructor != null);
            break;
        case "Aceptados":
            soportes = soportes.Where(s => s.Validacion_Instructor == true);
            break;
        case "Rechazados":
            soportes = soportes.Where(s => s.Validacion_Instructor == false);
            break;
    }

    return PartialView("_SoportePartial", soportes.ToList()); // Devuelve una vista parcial
}





        [AutorizarTipoUsuario("Aprendiz")]
        public ActionResult MisSoportes()
        {
            if (Session["Idusuario"] != null)
            {
                string idUsuarioSesion = Session["Idusuario"].ToString();

                var Soportesfiltrados = db.Soporte
                                             .Where(r => r.Id_Aprendiz.ToString() == idUsuarioSesion);
               ViewBag.aprendizId = idUsuarioSesion;
                return View(Soportesfiltrados);
                
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        // GET: JustificacionInasistencia/Details/5
        [AutorizarTipoUsuario("Aprendiz")]
        public ActionResult Details(int? id)
        {
            if (id == null) 
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Soporte soporte = db.Soporte.Find(id);
            if (soporte == null)
            {
                return HttpNotFound();
            }
            ViewBag.RutaArchivo = soporte.Archivo_soporte;
            return View(soporte);
        }

        [HttpGet]
        public JsonResult GetAsistenciasPorInstructor(int instructorId)
        {
            var asistencias = db.Asistencia
                .Where(a => a.Id_Instructor == instructorId)
                .Select(a => new { a.Id_asistencia, a.Fecha_asistencia })
                .ToList();

            return Json(asistencias, JsonRequestBehavior.AllowGet);
        }



        // GET: JustificacionInasistencia/Create
        [AutorizarTipoUsuario("Aprendiz")]
        public ActionResult Create(int asistenciaId) // Recibe el asistenciaId
        {
            var soporte = new Soporte
            {
                Id_asistencia = asistenciaId
            };

            // Obtener la fecha de inicio de la asistencia
            var asistencia = db.Asistencia.Find(asistenciaId);
            var instructor = db.Usuario.Find(asistencia.Id_Instructor);
            if (instructor != null)
            {
                ViewBag.Nombre_instructor = instructor.Nombre_usuario + " " + instructor.Apellido_usuario;
                soporte.Id_Instructor = asistencia.Id_Instructor; // Asignar el ID al modelo
            }
            if (asistencia != null)
            {
                ViewBag.Fecha_asistencia = asistencia.Fecha_asistencia;// Formato de fecha
            }
            else
            {
                // Manejar el caso donde la asistencia no existe
                ViewBag.Fecha_asistencia = "Fecha no encontrada"; // O un mensaje de error
            }

            //ViewBag.Id_Instructor = new SelectList(db.Usuario.Where(u => u.Tipo_usuario == "Instructor" || u.Tipo_usuario == "InstructorAdmin"), "Id_Usuario", "Nombre_Usuario", "Apellido_Usuario");
            ViewBag.Id_Competencia = new SelectList(db.Competencia, "Id_Competencia", "Nombre_Competencia");

            return View(soporte); // Pasa el modelo prellenado a la vista
        }




        // POST: JustificacionInasistencia/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AutorizarTipoUsuario("Aprendiz")]
        public ActionResult Create([Bind(Include = "Id_soporte,Nombre_soporte,Descripcion_soporte,Fecha_registro,Hora_registro,Id_Aprendiz,Id_asistencia,Id_Instructor,Archivo_soporte,Estado_soporte")] Soporte soporte, HttpPostedFileBase archivo)
        {
            if (string.IsNullOrWhiteSpace(soporte.Descripcion_soporte))
            {
                soporte.Descripcion_soporte = "N/A";
            }
            if (ModelState.IsValid)
            {
                if (archivo != null && archivo.ContentLength > 0)
                {
                    string extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
                    if (extension != ".pdf")
                    {
                        ViewBag.ErrorArchivo = "El archivo debe ser un PDF.";
                    }
                    else
                    {
                        // 1. Generar nombre de archivo único (para evitar conflictos)
                        string nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(archivo.FileName);

                        // 2. Ruta completa para guardar el archivo
                        string rutaArchivo = Path.Combine(Server.MapPath("~/ArchivosSoportes"), nombreArchivo);

                        // 3. Guardar el archivo en la carpeta
                        archivo.SaveAs(rutaArchivo);

                        // 4. Almacenar la ruta relativa en la base de datos
                        soporte.Archivo_soporte = "~/ArchivosSoportes/" + nombreArchivo;
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Debe seleccionar un archivo.");
                }
                var asistencia = db.Asistencia.Find(soporte.Id_asistencia);
                if (asistencia != null)
                {
                    soporte.Id_Instructor = asistencia.Id_Instructor; 
                }
                db.Soporte.Add(soporte);
                db.SaveChanges();

                return RedirectToAction("MisSoportes");
            }
                ViewBag.Fecha_asistencia = new SelectList(db.Asistencia, "Id_asistencia", "Fecha_asistencia", soporte.Id_asistencia);
                ViewBag.Id_Instructor = new SelectList(db.Usuario.Where(u => u.Tipo_usuario == "Instructor"), "Id_Usuario", "Nombre_Usuario", soporte.Id_Instructor);
                ViewBag.Nombre_instructor = new SelectList(db.Usuario, "Id_Instructor", "Nombre_usuario", soporte.Id_Instructor);

            return View(soporte);
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
        public ActionResult GenerarReportePDFSoportesAprendiz(int id)
        {
            try
            {
                string idUsuarioSesion = Session["Idusuario"].ToString();

                var Soportesfiltrados = db.Soporte
                                             .Where(r => r.Id_Aprendiz.ToString() == idUsuarioSesion);


                var aprendiz = db.Usuario.Find(id); // Busca la ficha por su ID
                string nombreAprendiz = aprendiz?.Nombre_usuario + aprendiz.Apellido_usuario; // Obtiene el código de la ficha

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    Document document = new Document(PageSize.A4.Rotate(), 50, 50, 50, 35);
                    PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);

                    writer.PageEvent = new HeaderFooterEvent(Server.MapPath("~/assets/images/Logo-remove.png")); // Pasar la ruta de la imagen

                    document.Open();

                    // Título
                    Paragraph titulo = new Paragraph("Reporte Justificaciones Asistencia del Aprendiz " + nombreAprendiz, new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD));
                    titulo.Alignment = Element.ALIGN_CENTER;
                    document.Add(titulo);

                    document.Add(Chunk.NEWLINE);

                    // Agregar contenido al PDF (tabla con datos de las fichas)
                    PdfPTable table = new PdfPTable(6);
                    table.WidthPercentage = 100;

                    // Encabezados de la tabla
                    Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                    table.AddCell(new Phrase("ID", headerFont));
                    table.AddCell(new Phrase("Asunto", headerFont));
                    table.AddCell(new Phrase("Fecha Registro", headerFont));
                    table.AddCell(new Phrase("Fecha de la Inasistencia", headerFont));
                    table.AddCell(new Phrase("Ficha", headerFont));
                    table.AddCell(new Phrase("Validación del Instructor", headerFont));

                    // Datos de las fichas
                    Font cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                    foreach (var Justificaciones in Soportesfiltrados)
                    {
                        table.AddCell(new Phrase(Justificaciones.Id_soporte.ToString()));
                        table.AddCell(new Phrase(Justificaciones.Nombre_soporte.ToString()));
                        table.AddCell(new Phrase(Justificaciones.Fecha_registro));
                        table.AddCell(new Phrase(Justificaciones.Asistencia.Fecha_asistencia));
                        table.AddCell(new Phrase(Justificaciones.Asistencia.Ficha.Codigo_ficha.ToString()));
                        table.AddCell(new Phrase(Justificaciones.Validacion_Instructor.ToString()));
                    }

                    document.Add(table);
                    document.Close();

                    return File(memoryStream.ToArray(), "application/pdf", "ReporteJustificacionesAprendiz" + nombreAprendiz + ".pdf");

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error al generar el PDF: " + ex.Message); // Registro
                return new HttpStatusCodeResult(500, "Error interno del servidor al generar el PDF.");
            }
        }


        // GET: JustificacionInasistencia/Edit/5
        [AutorizarTipoUsuario("Instructor", "InstructorAdmin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Soporte soporte = db.Soporte.Find(id);
            if (soporte == null)
            {
                return HttpNotFound();
            }
            if (soporte.Id_Instructor != (int)Session["Idusuario"])
            {
                return RedirectToAction("Index");
            }
            ViewBag.Id_asistencia = new SelectList(db.Asistencia, "Id_asistencia", "Tipo_asistencia", soporte.Id_asistencia);
            ViewBag.RutaArchivo = soporte.Archivo_soporte;
            return View(soporte);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AutorizarTipoUsuario("Instructor", "InstructorAdmin")]
        public ActionResult Edit(int id, Soporte soportes, string accion)
        {
            using (var db = new BDAssistsADSOv4Entities())
            {
                if (string.IsNullOrWhiteSpace(soportes.Nota_Instructor))
                {
                    soportes.Nota_Instructor = "N/A";
                }
                if (ModelState.IsValid)
                {
                    var soporte = db.Soporte.Find(id);

                    // Asigna Validacion_Instructor según el botón presionado
                    if (accion == "rechazar")
                    {
                        soporte.Validacion_Instructor = false;
                    }
                    else if (accion == "validar")
                    {
                        soporte.Validacion_Instructor = true;
                    }

                    // Actualiza otros campos (Nota_Instructor)
                    soporte.Nota_Instructor = soportes.Nota_Instructor;

                    db.Entry(soporte).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(soportes);
            }
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
