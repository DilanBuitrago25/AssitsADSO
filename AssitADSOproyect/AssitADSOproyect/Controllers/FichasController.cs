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
using System.Diagnostics;

namespace AssitADSOproyect.Controllers
{
    public class FichasController : Controller
    {
        private BDAssistsADSOv4Entities db = new BDAssistsADSOv4Entities();

   
        [AutorizarTipoUsuario("Instructor", "InstructorAdmin")]
        public ActionResult Index(string estadoFiltro = "")
        {
            bool? estado = null; 

            if (estadoFiltro == "true")
                estado = true;
            else if (estadoFiltro == "false")
                estado = false;

            var fichasFiltradas = db.Ficha
                .Where(f => !estado.HasValue || f.Estado_ficha == estado)
                .ToList();

            ViewBag.EstadoFiltro = estadoFiltro;

            return View(fichasFiltradas);
        }


        [AutorizarTipoUsuario("Instructor", "InstructorAdmin")]
        public ActionResult Fichas_Instructor(string estadoFiltro = "")
        {
            string idUsuario = Session["Idusuario"].ToString();


            var fichasFiltradas = db.Ficha
                .Where(f => f.Estado_ficha == true)
                .Where(f => estadoFiltro == "" || f.Estado_ficha.ToString() == estadoFiltro)
                .Where(f => f.Ficha_has_Usuario.Any(fh => fh.Id_usuario.ToString() == idUsuario))
                .ToList();

            ViewBag.EstadoFiltro = estadoFiltro;

            return View(fichasFiltradas);
        }



        public ActionResult GenerarReportePDF(string estadoFiltro = "")
        {
            var fichas = db.Ficha
                    .Where(f => f.Estado_ficha == true) 
                    .Where(f => estadoFiltro == "" || f.Estado_ficha.ToString() == estadoFiltro) 
                    .ToList(); 

            using (MemoryStream memoryStream = new MemoryStream())
            {
                Document document = new Document(PageSize.A4.Rotate(), 50, 50, 50, 35);
                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);

                writer.PageEvent = new HeaderFooterEvent(Server.MapPath("~/assets/images/Logo-remove.png")); 

                document.Open();

                
                Paragraph titulo = new Paragraph("Reporte de Fichas", new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD));
                titulo.Alignment = Element.ALIGN_CENTER;
                document.Add(titulo);

                document.Add(Chunk.NEWLINE);

                
                PdfPTable table = new PdfPTable(8); 
                table.WidthPercentage = 100;

            
                Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                table.AddCell(new Phrase("Código Ficha", headerFont));
                table.AddCell(new Phrase("Jornada Ficha"));
                table.AddCell(new Phrase("Modalidad Ficha"));
                table.AddCell(new Phrase("Tipo Ficha"));
                table.AddCell(new Phrase("Fecha Inicio Ficha"));
                table.AddCell(new Phrase("Fecha Fin Ficha"));
                table.AddCell(new Phrase("Programa de Formacion"));
                table.AddCell(new Phrase("Estado Ficha"));
       

      
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
                    table.AddCell(new Phrase(ficha.Estado_ficha.ToString()));
                                                                                        
                }

            document.Add(table);
            document.Close();

           
            return File(memoryStream.ToArray(), "application/pdf", "ReporteFichas.pdf");
            }
        }

        public ActionResult GenerarReportePDFInstructorFicha(string estadoFiltro = "")
        {
            string idUsuario = Session["Idusuario"].ToString();

            var fichas = db.Ficha
                .Where(f => f.Estado_ficha == true)
                .Where(f => estadoFiltro == "" || f.Estado_ficha.ToString() == estadoFiltro)
                .Where(f => f.Ficha_has_Usuario.Any(fh => fh.Id_usuario.ToString() == idUsuario))
                .ToList();

            using (MemoryStream memoryStream = new MemoryStream())
            {
                Document document = new Document(PageSize.A4.Rotate(), 50, 50, 50, 35);
                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);

                writer.PageEvent = new HeaderFooterEvent(Server.MapPath("~/assets/images/Logo-remove.png")); 

                document.Open();

                // Título
                Paragraph titulo = new Paragraph("Reporte de Fichas", new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD));
                titulo.Alignment = Element.ALIGN_CENTER;
                document.Add(titulo);

                document.Add(Chunk.NEWLINE);

              
                PdfPTable table = new PdfPTable(8); 
                table.WidthPercentage = 100;

         
                Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                table.AddCell(new Phrase("Código Ficha", headerFont));
                table.AddCell(new Phrase("Jornada Ficha"));
                table.AddCell(new Phrase("Modalidad Ficha"));
                table.AddCell(new Phrase("Tipo Ficha"));
                table.AddCell(new Phrase("Fecha Inicio Ficha"));
                table.AddCell(new Phrase("Fecha Fin Ficha"));
                table.AddCell(new Phrase("Programa de Formacion"));
                table.AddCell(new Phrase("Estado Ficha"));
              

          
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
                    table.AddCell(new Phrase(ficha.Estado_ficha.ToString()));
                                                                             
                }

                document.Add(table);
                document.Close();

                // Devolver el PDF como archivo descargable
                return File(memoryStream.ToArray(), "application/pdf", "ReporteFichas.pdf");
            }
        }

        [HttpPost]
        public ActionResult GenerarReportePDFAprendiz(int id)
        {
            try
            {
                var aprendices = db.Ficha_has_Usuario
            .Where(fu => fu.Id_ficha == id && fu.TipoUsuario == "Aprendiz")
            .Select(fu => fu.Usuario) // Accede a la información del usuario
            .ToList();

                var ficha = db.Ficha.Find(id); // Busca la ficha por su ID
                string codigoFicha = ficha?.Codigo_ficha.ToString(); // Obtiene el código de la ficha

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    Document document = new Document(PageSize.A4.Rotate(), 50, 50, 50, 35);
                    PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);

                    writer.PageEvent = new HeaderFooterEvent(Server.MapPath("~/assets/images/Logo-remove.png")); // Pasar la ruta de la imagen

                    document.Open();

                    // Título
                    Paragraph titulo = new Paragraph("Reporte de Aprendices de la Ficha " + codigoFicha, new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD));
                    titulo.Alignment = Element.ALIGN_CENTER;
                    document.Add(titulo);

                    document.Add(Chunk.NEWLINE);

                    // Agregar contenido al PDF (tabla con datos de las fichas)
                    PdfPTable table = new PdfPTable(6); // 4 columnas (ajusta según tus campos)
                    table.WidthPercentage = 100;

                    // Encabezados de la tabla
                    Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                    table.AddCell(new Phrase("Documento", headerFont));
                    table.AddCell(new Phrase("Nombres", headerFont));
                    table.AddCell(new Phrase("Apellidos", headerFont));
                    table.AddCell(new Phrase("Correo", headerFont));
                    table.AddCell(new Phrase("Teléfono", headerFont));
                    table.AddCell(new Phrase("Estado", headerFont));

                    // Datos de las fichas
                    Font cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                    foreach (var aprendiz in aprendices)
                    {
                        table.AddCell(new Phrase(aprendiz.Tipo_Documento_usuario.ToString() + aprendiz.Documento_usuario.ToString()));
                        table.AddCell(new Phrase(aprendiz.Nombre_usuario));
                        table.AddCell(new Phrase(aprendiz.Apellido_usuario));
                        table.AddCell(new Phrase(aprendiz.Correo_usuario));
                        table.AddCell(new Phrase(aprendiz.Telefono_usuario.ToString()));
                        table.AddCell(new Phrase(aprendiz.Estado_Usuario.ToString()));

                    }

                    document.Add(table);
                    document.Close();

                    return File(memoryStream.ToArray(), "application/pdf", "ReporteAprendicesFicha" + codigoFicha + ".pdf");

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error al generar el PDF: " + ex.Message); // Registro
                return new HttpStatusCodeResult(500, "Error interno del servidor al generar el PDF.");
            }
        }

        [HttpPost]
        public ActionResult GenerarReportePDFInstructor(int id)
        {
            try
            {
                var instructorId = db.Ficha_has_Usuario
                .Where(fu => fu.Id_ficha == id && fu.TipoUsuario == "Instructor" || fu.TipoUsuario == "InstructorAdmin")
                .Select(fu => fu.Usuario)
                .ToList();

                var ficha = db.Ficha.Find(id); // Busca la ficha por su ID
                string codigoFicha = ficha?.Codigo_ficha.ToString(); // Obtiene el código de la ficha

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    Document document = new Document(PageSize.A4.Rotate(), 50, 50, 50, 35);
                    PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);

                    writer.PageEvent = new HeaderFooterEvent(Server.MapPath("~/assets/images/Logo-remove.png")); // Pasar la ruta de la imagen

                    document.Open();

                    // Título
                    Paragraph titulo = new Paragraph("Reporte de instructores de la Ficha " + codigoFicha, new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD));
                    titulo.Alignment = Element.ALIGN_CENTER;
                    document.Add(titulo);

                    document.Add(Chunk.NEWLINE);

                    // Agregar contenido al PDF (tabla con datos de las fichas)
                    PdfPTable table = new PdfPTable(7); 
                    table.WidthPercentage = 100;

                    // Encabezados de la tabla
                    Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                    table.AddCell(new Phrase("Documento", headerFont));
                    table.AddCell(new Phrase("Nombres", headerFont));
                    table.AddCell(new Phrase("Apellidos", headerFont));
                    table.AddCell(new Phrase("Correo", headerFont));
                    table.AddCell(new Phrase("Teléfono", headerFont));
                    table.AddCell(new Phrase("Tipo Instructor", headerFont));
                    table.AddCell(new Phrase("Estado", headerFont));

                    // Datos de las fichas
                    Font cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                    foreach (var instructores in instructorId)
                    {
                        table.AddCell(new Phrase(instructores.Tipo_Documento_usuario.ToString() + instructores.Documento_usuario.ToString()));
                        table.AddCell(new Phrase(instructores.Nombre_usuario));
                        table.AddCell(new Phrase(instructores.Apellido_usuario));
                        table.AddCell(new Phrase(instructores.Correo_usuario));
                        table.AddCell(new Phrase(instructores.Telefono_usuario.ToString()));
                        table.AddCell(new Phrase(instructores.Tipo_usuario));
                        table.AddCell(new Phrase(instructores.Estado_Usuario.ToString()));

                    }

                    document.Add(table);
                    document.Close();

                    return File(memoryStream.ToArray(), "application/pdf", "ReporteInstructoresFicha" + codigoFicha + ".pdf");

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

        // GET: Fichas1/Details/5
        [AutorizarTipoUsuario("Instructor", "InstructorAdmin")]
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
        [AutorizarTipoUsuario("Instructor", "InstructorAdmin")]
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
        [AutorizarTipoUsuario("Instructor", "InstructorAdmin")]
        public ActionResult Create([Bind(Include = "Id_ficha,Codigo_ficha,Jornada_ficha,Modalidad_ficha,tipo_ficha,Id_programa,Fecha_inicio,Fecha_fin,Estado_ficha")] Ficha ficha)
        {
            if (ModelState.IsValid)
            {
                // Verificar si ya existe una ficha con el mismo código
                bool existeFicha = db.Ficha.Any(f => f.Codigo_ficha == ficha.Codigo_ficha);

                if (existeFicha)
                {
                    ModelState.AddModelError("Codigo_ficha", "Ya existe una ficha con este código.");
                }
                else
                {
                    db.Ficha.Add(ficha);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            ViewBag.Id_programa = new SelectList(db.Programa_formacion, "Id_programa", "Nombre_programa", ficha.Id_programa);
            return View(ficha);
        }


        // GET: Fichas1/Edit/5
        [AutorizarTipoUsuario("Instructor", "InstructorAdmin")]
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
        [AutorizarTipoUsuario("Instructor", "InstructorAdmin")]
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
        [AutorizarTipoUsuario("Instructor", "InstructorAdmin")]
        public ActionResult Aprendices_Ficha(int id)
        {
            var aprendizId = db.Ficha_has_Usuario
                .Where(fu => fu.Id_ficha == id && fu.TipoUsuario == "Aprendiz")
                .Select(fu => fu.Usuario)
                .ToList();

            ViewBag.FichaId = id;
            ViewBag.CodigoFicha = db.Ficha.Find(id)?.Codigo_ficha; 
            return View(aprendizId);
        }

        [AutorizarTipoUsuario("Instructor", "InstructorAdmin")]
        public ActionResult Instructores_Ficha(int id)
        {
            var instructorId = db.Ficha_has_Usuario
                .Where(fu => fu.Id_ficha == id && fu.TipoUsuario == "Instructor" || fu.TipoUsuario == "InstructorAdmin")
                .Select(fu => fu.Usuario)
                .ToList();

            ViewBag.FichaId = id;
            ViewBag.CodigoFicha = db.Ficha.Find(id)?.Codigo_ficha;
            return View(instructorId);
        }

        [AutorizarTipoUsuario("Instructor", "InstructorAdmin")]
        public ActionResult Asociar_Aprendiz_Ficha(int idFicha)
        {
            var aprendizId = db.Usuario.Where(u => u.Tipo_usuario == "Aprendiz").ToList();
            ViewBag.Aprendices = aprendizId;
            ViewBag.FichaId = idFicha;
            ViewBag.CodigoFicha = db.Ficha.Find(idFicha)?.Codigo_ficha;
            return View();
        }

        [AutorizarTipoUsuario("Instructor", "InstructorAdmin")]
        public ActionResult Asociar_Instructor_Ficha(int idFicha)
        {
            var instructorId = db.Usuario.Where(u => u.Tipo_usuario == "Instructor" || u.Tipo_usuario == "InstructorAdmin").ToList();
            ViewBag.Instructores = instructorId;
            ViewBag.FichaId = idFicha;
            ViewBag.CodigoFicha = db.Ficha.Find(idFicha)?.Codigo_ficha;
            return View();
        }

        [HttpPost]
        public ActionResult Asociar_Aprendiz_Ficha(int idFicha, int aprendizId) 
        {
            if (ModelState.IsValid)
            {
                // Obtén el ID del aprendiz seleccionado del formulario
                if (!int.TryParse(Request.Form["aprendizId"], out aprendizId)) 
                {
                    ModelState.AddModelError("", "Error al seleccionar el aprendiz.");
                    return View(); 
                }

                var aprendiz = db.Usuario.Find(aprendizId);
                var ficha = db.Ficha.Find(idFicha);

                if (aprendiz != null && aprendiz.Tipo_usuario == "Aprendiz" && ficha != null)
                {
                    // Verificar si el aprendiz ya está asociado a la ficha
                    if (!db.Ficha_has_Usuario.Any(fhu => fhu.Id_ficha == idFicha && fhu.Id_usuario == aprendizId))
                    {
                        // Crea el objeto Ficha_has_Usuario aquí
                        var fichaHasUsuario = new Ficha_has_Usuario
                        {
                            Id_ficha = idFicha,
                            Id_usuario = aprendizId,
                            TipoUsuario = aprendiz.Tipo_usuario
                        };

                        db.Ficha_has_Usuario.Add(fichaHasUsuario);
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("", "El aprendiz ya está asociado a esta ficha.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "El usuario seleccionado no es un aprendiz válido o la ficha no existe.");
                }
            }

            
            ViewBag.Aprendices = db.Usuario.Where(u => u.Tipo_usuario == "Aprendiz").ToList();
            ViewBag.FichaId = idFicha;
            return RedirectToAction("Aprendices_Ficha", new { id = idFicha });
        }

        [HttpPost]
        public ActionResult Asociar_Instructor_Ficha(int idFicha, int instructorId) 
        {
            if (ModelState.IsValid)
            {
                // Obtén el ID del instructor seleccionado del formulario
                if (!int.TryParse(Request.Form["instructorId"], out instructorId))
                {
                    ModelState.AddModelError("", "Error al seleccionar el Instructor.");
                    return View();
                }

                var instructor = db.Usuario.Find(instructorId);
                var ficha = db.Ficha.Find(idFicha);

                if (instructor != null && instructor.Tipo_usuario == "Instructor" && instructor.Tipo_usuario == "InstructorAdmin" && ficha != null)
                {
                    // Verificar si el instructor ya está asociado a la ficha
                    if (!db.Ficha_has_Usuario.Any(fhu => fhu.Id_ficha == idFicha && fhu.Id_usuario == instructorId))
                    {
                        // Crea el objeto Ficha_has_Usuario
                        var fichaHasUsuario = new Ficha_has_Usuario
                        {
                            Id_ficha = idFicha,
                            Id_usuario = instructorId,
                            TipoUsuario = instructor.Tipo_usuario
                        };

                        db.Ficha_has_Usuario.Add(fichaHasUsuario);
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("", "El instructor ya está asociado a esta ficha.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "El usuario seleccionado no es un instructor válido o la ficha no existe.");
                }
            }

  
            ViewBag.Instructores = db.Usuario.Where(u => u.Tipo_usuario == "Instructor" || u.Tipo_usuario == "InstructorAdmin").ToList();
            ViewBag.FichaId = idFicha;
            return RedirectToAction("Instructores_Ficha", new { id = idFicha });
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
