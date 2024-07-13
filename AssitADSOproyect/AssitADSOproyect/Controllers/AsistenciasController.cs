using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ClaseDatos;
using iTextSharp.text.pdf;
using iTextSharp.text;
using QRCoder;
using static AssitADSOproyect.Controllers.LoginController;
using System.Globalization;

namespace AssitADSOproyect.Controllers
{
    public class AsistenciasController : Controller
    {
        private BDAssistsADSOv4Entities db = new BDAssistsADSOv4Entities();
        [AutorizarTipoUsuario("Instructor", "InstructorAdmin")]
        //GET: Asistencias
        public ActionResult Index(int? pagina,string fechaFiltro = "", int? fichaFiltro = null, int? competenciaFiltro = null)
        {
            string idUsuarioSesion = Session["Idusuario"].ToString();

            // Consulta base con Include para cargar relaciones necesarias
            var query = db.Asistencia
                .Include(a => a.Ficha)
                .Include(a => a.Ficha.Programa_formacion)
                .Include(a => a.Ficha.Programa_formacion.Competencia)
                .Where(a => a.Id_Instructor.ToString() == idUsuarioSesion);

            if (!string.IsNullOrEmpty(fechaFiltro))
            {
                var fechas = fechaFiltro.Split('-');
                if (fechas.Length == 2)
                {
                    DateTime fechaInicioParsed, fechaFinParsed;
                    if (DateTime.TryParseExact(fechas[0].Trim(), "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaInicioParsed) &&
                        DateTime.TryParseExact(fechas[1].Trim(), "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaFinParsed))
                    {
                        // Realizar la conversión de fecha fuera de la consulta LINQ to Entities
                        var asistenciasFiltradasLista = query.ToList(); // Convertir a lista para usar LINQ to Objects

                        asistenciasFiltradasLista = asistenciasFiltradasLista.Where(a =>
                            DateTime.ParseExact(a.Fecha_asistencia, "yyyy-MM-dd", CultureInfo.InvariantCulture) >= fechaInicioParsed &&
                            DateTime.ParseExact(a.Fecha_asistencia, "yyyy-MM-dd", CultureInfo.InvariantCulture) <= fechaFinParsed).ToList();

                        // Volver a asignar el resultado filtrado
                        query = asistenciasFiltradasLista.AsQueryable();
                    }
                }
            }

            // Aplicar filtros de ficha y competencia ANTES de materializar la consulta
            if (fichaFiltro.HasValue)
            {
                query = query.Where(a => a.Id_ficha == fichaFiltro.Value);
            }

            if (competenciaFiltro.HasValue)
            {
                query = query.Where(a => a.Id_competencia == competenciaFiltro.Value);
            }

            ViewBag.Id_ficha = new SelectList(db.Ficha, "Id_ficha", "Codigo_ficha", fichaFiltro);
            ViewBag.Id_Instructor = new SelectList(db.Usuario, "Id_usuario", "Documento_usuario");

            // Cargar competencias de la ficha seleccionada
            if (fichaFiltro.HasValue)
            {
                var competencias = db.Ficha
                    .Where(f => f.Id_ficha == fichaFiltro.Value)
                    .SelectMany(f => f.Programa_formacion.Competencia)
                    .Select(c => new { Value = c.Id_competencia, Text = c.Nombre_competencia })
                    .ToList();

                ViewBag.Id_competencia = new SelectList(competencias, "Value", "Text", competenciaFiltro); // Mantener la competencia seleccionada
            }
            else
            {
                ViewBag.Id_competencia = new SelectList(new List<object>(), "Value", "Text"); // Lista vacía si no hay ficha seleccionada
            }

            ViewBag.fechaFiltro = fechaFiltro; // Para mantener el valor en el campo

            int registrosPorPagina = 9;
            int numeroPagina = (pagina ?? 1); // Si 'pagina' es nulo, usa la página 1

            var asistenciasFiltradasporusuario = db.Asistencia.Where(f => f.Id_Instructor.ToString() == idUsuarioSesion &&
                                                     f.Estado_Asistencia == true);

            var asistenciasFiltradas = db.Asistencia
                .OrderByDescending(a => a.Fecha_asistencia)
                .Skip((numeroPagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .ToList();


            

            ViewBag.PaginaActual = numeroPagina;
            ViewBag.TotalRegistros = db.Asistencia.Count();

            return View(asistenciasFiltradasporusuario);
        }





        public ActionResult GenerarReportePDF()
        {
            string idUsuarioSesion = Session["Idusuario"].ToString();

            // Consulta base con Include para cargar relaciones necesarias
            var asistencias = db.Asistencia
                .Include(a => a.Ficha)
                .Include(a => a.Ficha.Programa_formacion)
                .Include(a => a.Ficha.Programa_formacion.Competencia)
                .Where(a => a.Id_Instructor.ToString() == idUsuarioSesion);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 50, 50, 80, 50);
                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);

                writer.PageEvent = new HeaderFooterEvent(Server.MapPath("~/assets/images/Logo-remove.png"));

                document.Open();

                // Título
                Paragraph titulo = new Paragraph("Reporte de Asistencias", new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD));
                titulo.Alignment = Element.ALIGN_CENTER;
                document.Add(titulo);

                document.Add(Chunk.NEWLINE);


                PdfPTable table = new PdfPTable(8);
                table.WidthPercentage = 100;

                // Encabezados de la tabla
                Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                table.AddCell(new Phrase("Id de Asistencia", headerFont));
                table.AddCell(new Phrase("Fecha"));
                table.AddCell(new Phrase("Hora Inicio"));
                table.AddCell(new Phrase("Hora Fin"));
                table.AddCell(new Phrase("Detalles Asistencia"));
                table.AddCell(new Phrase("Codigo Ficha"));
                table.AddCell(new Phrase("Competencia"));
                table.AddCell(new Phrase("Estado Asistencia"));



                Font cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                foreach (var asistencia in asistencias)
                {
                    table.AddCell(new Phrase(asistencia.Id_asistencia.ToString()));
                    table.AddCell(new Phrase(asistencia.Fecha_asistencia));
                    table.AddCell(new Phrase(asistencia.Hora_inicio_asistencia));
                    table.AddCell(new Phrase(asistencia.Hora_fin_asistencia));
                    table.AddCell(new Phrase(asistencia.Detalles_asistencia));
                    table.AddCell(new Phrase(asistencia.Ficha.Codigo_ficha.ToString()));
                    table.AddCell(new Phrase(asistencia.Competencia.Nombre_competencia));
                    table.AddCell(new Phrase(asistencia.Estado_Asistencia.ToString()));

                }

                document.Add(table);
                document.Close();


                return File(memoryStream.ToArray(), "application/pdf", "ReporteAsistencias.pdf");
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

        // GET: Asistencias/Details/5
        [AutorizarTipoUsuario("Instructor", "InstructorAdmin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Asistencia asistencia = db.Asistencia.Find(id);
            if (asistencia == null)
            {
                return HttpNotFound();
            }
            return View(asistencia);
        }


        public ActionResult ObtenerCompetenciasPorFicha(int idFicha)
        {
            var competencias = db.Ficha
                .Where(f => f.Id_ficha == idFicha)
                .Select(f => new
                {
                    Competencias = f.Programa_formacion.Competencia.Select(c => new { Value = c.Id_competencia, Text = c.Nombre_competencia })
                })
                .FirstOrDefault()?
                .Competencias;

            return Json(competencias, JsonRequestBehavior.AllowGet);
        }


        // GET: Asistencias/Create
        [AutorizarTipoUsuario("Instructor", "InstructorAdmin")]
        public ActionResult Create()
        {
            ViewBag.Id_ficha = new SelectList(db.Ficha, "Id_ficha", "Codigo_ficha");
            ViewBag.Id_Instructor = new SelectList(db.Usuario, "Id_usuario", "Documento_usuario");

            // Cargar competencias de la primera ficha (ajusta la lógica si es necesario)
            var primeraFicha = db.Ficha.FirstOrDefault();

            if (primeraFicha != null && primeraFicha.Id_programa != null)
            {
                var competencias = db.Programa_formacion
                    .Where(pf => pf.Id_programa == primeraFicha.Id_programa)
                    .SelectMany(pf => pf.Competencia)
                    .Select(c => new { Value = c.Id_competencia, Text = c.Nombre_competencia })
                    .ToList();

                ViewBag.Id_competencia = new SelectList(competencias, "Value", "Text");
            }
            else
            {
                ViewBag.Id_competencia = new SelectList(new List<object>(), "Value", "Text"); // Lista vacía si no hay competencias
            }

            return View();
        }



        // POST: Asistencias/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AutorizarTipoUsuario("Instructor", "InstructorAdmin")]
        public ActionResult Create([Bind(Include = "Id_asistencia,Hora_inicio_asistencia,Fecha_asistencia,Hora_fin_asistencia,Detalles_asistencia,Id_Instructor,Id_ficha,Id_competencia,Estado_asistencia")] Asistencia asistencia)
        {
            if (string.IsNullOrWhiteSpace(asistencia.Detalles_asistencia))
            {
                asistencia.Detalles_asistencia = "N/A";
            }
            if (ModelState.IsValid)
            {
                db.Asistencia.Add(asistencia);
                db.SaveChanges(); // Guardar la asistencia para obtener el Id_asistencia generado

                // Generar el código QR después de guardar la asistencia
                var createRegistroUrl = Url.Action("Create", "RegistroAsistencias", new { id_Asistencia = asistencia.Id_asistencia }, Request.Url.Scheme);
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(createRegistroUrl, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);

                var qrCodePath = Path.Combine(Server.MapPath("~/QRCodes"), $"{asistencia.Id_asistencia}.png");
                using (var bitmap = qrCode.GetGraphic(20))
                {
                    bitmap.Save(qrCodePath, ImageFormat.Png);
                }

                asistencia.QrCode = $"~/QRCodes/{asistencia.Id_asistencia}.png"; // Asignar la ruta del QR
                db.SaveChanges(); // Guardar la actualización de la asistencia

                return RedirectToAction("Index");
            }

            ViewBag.Id_Instructor = new SelectList(db.Usuario, "Id_usuario", "Documento_usuario", asistencia.Id_Instructor);
            //ViewBag.Id_competencia = new SelectList(db.Competencia, "Id_competencia", "Nombre_competencia", asistencia.Id_competencia);
            ViewBag.Id_ficha = new SelectList(db.Ficha, "Id_ficha", "Codigo_ficha", asistencia.Id_ficha);

            

            //return View("Confirmacion", asistencia);

            return View(asistencia);
        }

        // GET: Asistencias/Edit/5
        [AutorizarTipoUsuario("Instructor", "InstructorAdmin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Asistencia asistencia = db.Asistencia.Find(id);
            if (asistencia == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id_Instructor = new SelectList(db.Usuario, "Id_usuario", "Documento_usuario", asistencia.Id_Instructor);
            //ViewBag.Id_competencia = new SelectList(db.Competencia, "Id_competencia", "Nombre_competencia", asistencia.Id_competencia);
            ViewBag.Id_ficha = new SelectList(db.Ficha, "Id_ficha", "Codigo_ficha", asistencia.Id_ficha);
            return View(asistencia);
        }

        // POST: Asistencias/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AutorizarTipoUsuario("Instructor", "InstructorAdmin")]
        public ActionResult Edit([Bind(Include = "Id_asistencia,Id_ficha,Id_competencia,Fecha_inicio_asistencia,Hora_inicio_asistencia,Fecha_fin_asistencia,Hora_fin_asistencia,Detalles_asistencia,Id_usuario")] Asistencia asistencia)
        {
            if (ModelState.IsValid)
            {
                db.Entry(asistencia).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Id_Instructor = new SelectList(db.Usuario, "Id_usuario", "Documento_usuario", asistencia.Id_Instructor  );
            //ViewBag.Id_competencia = new SelectList(db.Competencia, "Id_competencia", "Nombre_competencia", asistencia.Id_competencia);
            ViewBag.Id_ficha = new SelectList(db.Ficha, "Id_ficha", "Codigo_ficha", asistencia.Id_ficha);
            return View(asistencia);
        }

        [AutorizarTipoUsuario("Instructor", "InstructorAdmin")]
        public ActionResult Asistencia_Aprendices(int fichaId, int asistenciaId)
        {
            // 1. Obtener la "Ficha" usando el fichaId proporcionado
            var ficha = db.Ficha.Find(fichaId); // Asumiendo que tienes un DbSet<Ficha> en tu DbContext

            if (ficha == null)
            {
                return HttpNotFound(); // O maneja el caso donde la ficha no existe
            }

            // 2. Obtener los usuarios "Aprendiz" y sus registros de asistencia
            var aprendicesConAsistencia = db.Usuario
             .Where(u => u.Tipo_usuario == "Aprendiz" &&
                         u.Ficha_has_Usuario.Any(fhu => fhu.Id_ficha == fichaId))
             .Select(u => new UsuarioConAsistenciaViewModel
             {
                 Usuario = u,
                 Asistio_registro = u.RegistroAsistencia
                                      .Any(ra => ra.Id_asistencia == asistenciaId && ra.Asistio_registro == true) // Verifica si existe y es true
             })
             .ToList();


            // 3. Pasar los datos a la vista
            ViewBag.Ficha = ficha;
            return View(aprendicesConAsistencia); // Pasamos la lista del ViewModel
        }



        //// GET: Asistencias1/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Asistencia asistencia = db.Asistencia.Find(id);
        //    if (asistencia == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(asistencia);
        //}

        //// POST: Asistencias1/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Asistencia asistencia = db.Asistencia.Find(id);
        //    db.Asistencia.Remove(asistencia);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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
