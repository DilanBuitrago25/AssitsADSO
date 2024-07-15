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
    public class UsuariosInstructorController : Controller
    {
        private BDAssistsADSOv4Entities db = new BDAssistsADSOv4Entities();
        [AutorizarTipoUsuario("InstructorAdmin")]
        // GET: UsuariosInstructor
        public ActionResult Index(string estadoFiltro = "")
        {
            var InstructoresFiltrados = db.Usuario
                .Where(u => (u.Tipo_usuario == "Instructor" || u.Tipo_usuario == "InstructorAdmin") && u.Estado_Usuario == true)
                .ToList();



            ViewBag.EstadoFiltro = estadoFiltro;

            return View(InstructoresFiltrados);
        }

        public ActionResult GenerarReportePDF()
        {
            var InstructoresFiltrados = db.Usuario
                .Where(u => (u.Tipo_usuario == "Instructor" || u.Tipo_usuario == "InstructorAdmin") && u.Estado_Usuario == true)
                .ToList();

            using (MemoryStream memoryStream = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 50, 50, 80, 50);
                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);

                writer.PageEvent = new HeaderFooterEvent(Server.MapPath("~/assets/images/Logo-remove.png"));

                document.Open();

                // Título
                Paragraph titulo = new Paragraph("Reporte de Instructores", new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD));
                titulo.Alignment = Element.ALIGN_CENTER;
                document.Add(titulo);

                document.Add(Chunk.NEWLINE);


                PdfPTable table = new PdfPTable(8);
                table.WidthPercentage = 100;

                // Encabezados de la tabla
                Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                table.AddCell(new Phrase("Tipo Documento", headerFont));
                table.AddCell(new Phrase("Documento"));
                table.AddCell(new Phrase("Nombre"));
                table.AddCell(new Phrase("Apellido"));
                table.AddCell(new Phrase("Telefono"));
                table.AddCell(new Phrase("Correo"));
                table.AddCell(new Phrase("Tipo Instructor"));
                table.AddCell(new Phrase("Estado Instructor"));




                Font cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                foreach (var instructor in InstructoresFiltrados)
                {
                    table.AddCell(new Phrase(instructor.Tipo_Documento_usuario));
                    table.AddCell(new Phrase(instructor.Documento_usuario.ToString()));
                    table.AddCell(new Phrase(instructor.Nombre_usuario));
                    table.AddCell(new Phrase(instructor.Apellido_usuario));
                    table.AddCell(new Phrase(instructor.Telefono_usuario.ToString()));
                    table.AddCell(new Phrase(instructor.Correo_usuario));
                    table.AddCell(new Phrase(instructor.Tipo_usuario));
                    table.AddCell(new Phrase(instructor.Estado_Usuario.ToString()));

                }

                document.Add(table);
                document.Close();


                return File(memoryStream.ToArray(), "application/pdf", "ReporteInstructores.pdf");
            }
        }

        [HttpPost]
        public ActionResult GenerarReportePDFInstructorFicha(int id)
        {
            try
            {
                var fichasInstructor = db.Ficha_has_Usuario
                .Where(fu => fu.Id_usuario == id && (fu.TipoUsuario == "Instructor" || fu.TipoUsuario == "InstructorAdmin"))
                .Select(fu => fu.Ficha)
                .ToList();

                var instructor = db.Usuario.Find(id); // Busca la ficha por su ID
                string nombreInstructor = instructor?.Nombre_usuario + instructor.Apellido_usuario; // Obtiene el código de la ficha

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    Document document = new Document(PageSize.A4.Rotate(), 50, 50, 50, 35);
                    PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);

                    writer.PageEvent = new HeaderFooterEvent(Server.MapPath("~/assets/images/Logo-remove.png")); // Pasar la ruta de la imagen

                    document.Open();

                    // Título
                    Paragraph titulo = new Paragraph("Reporte de Fichas del Instructor " + nombreInstructor, new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD));
                    titulo.Alignment = Element.ALIGN_CENTER;
                    document.Add(titulo);

                    document.Add(Chunk.NEWLINE);

                    // Agregar contenido al PDF (tabla con datos de las fichas)
                    PdfPTable table = new PdfPTable(5);
                    table.WidthPercentage = 100;

                    // Encabezados de la tabla
                    Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                    table.AddCell(new Phrase("Codigo", headerFont));
                    table.AddCell(new Phrase("Jornada", headerFont));
                    table.AddCell(new Phrase("Modalidad", headerFont));
                    table.AddCell(new Phrase("Tipo", headerFont));
                    table.AddCell(new Phrase("Programa de Formación", headerFont));

                    // Datos de las fichas
                    Font cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                    foreach (var fichasinst in fichasInstructor)
                    {
                        table.AddCell(new Phrase(fichasinst.Codigo_ficha.ToString()));
                        table.AddCell(new Phrase(fichasinst.Jornada_ficha));
                        table.AddCell(new Phrase(fichasinst.Modalidad_ficha));
                        table.AddCell(new Phrase(fichasinst.tipo_ficha));
                        table.AddCell(new Phrase(fichasinst.Programa_formacion.Nombre_programa));
                    }

                    document.Add(table);
                    document.Close();

                    return File(memoryStream.ToArray(), "application/pdf", "Reportefichasde" + nombreInstructor + ".pdf");

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

        // GET: UsuariosInstructor/Details/5
        [AutorizarTipoUsuario("InstructorAdmin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuario.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // GET: UsuariosInstructor/Create
        [AutorizarTipoUsuario("InstructorAdmin")]
        public ActionResult Create()
        {
            ViewBag.Id_ficha = new SelectList(db.Ficha, "Id_ficha", "Jornada_ficha");
            return View();
        }

        // POST: UsuariosInstructor/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id_usuario,Documento_usuario,Tipo_usuario,Tipo_Documento_usuario,Nombre_usuario,Apellido_usuario,Telefono_usuario,Correo_usuario,Tipo_Usuario,Id_ficha,Estado_usuario,Contrasena_usuario")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                // Verificar si ya existe un usuario con el mismo Documento_usuario
                bool existeDocumento = db.Usuario.Any(u => u.Documento_usuario == usuario.Documento_usuario);

                // Verificar si ya existe un usuario con el mismo Correo_usuario
                bool existeCorreo = db.Usuario.Any(u => u.Correo_usuario == usuario.Correo_usuario);

                if (existeDocumento)
                {
                    ModelState.AddModelError("Documento_usuario", "Ya existe un usuario con este número de documento.");
                }

                if (existeCorreo)
                {
                    ModelState.AddModelError("Correo_usuario", "Ya existe un usuario con este correo electrónico.");
                }

                // Si no hay errores, guardar el usuario
                if (ModelState.IsValid)
                {
                    db.Usuario.Add(usuario);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            ////ViewBag.Id_ficha = new SelectList(db.Ficha, "Id_ficha", "Jornada_ficha", usuario.Id_ficha);
            return View(usuario);
        }

        // GET: UsuariosInstructor/Edit/5
        [AutorizarTipoUsuario("InstructorAdmin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuario.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            //ViewBag.Id_ficha = new SelectList(db.Ficha, "Id_ficha", "Jornada_ficha", usuario.Id_ficha);
            return View(usuario);
        }

        // POST: UsuariosInstructor/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id_usuario,Documento_usuario,Tipo_Documento_usuario,Nombre_usuario,Apellido_usuario,Telefono_usuario,Correo_usuario,Contrasena_usuario,Tipo_usuario,Estado_usuario")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                db.Entry(usuario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.Id_ficha = new SelectList(db.Ficha, "Id_ficha", "Jornada_ficha", usuario.Id_ficha);
            return View(usuario);
        }

        //desde aca tablas foranea
        [AutorizarTipoUsuario("InstructorAdmin")]
        public ActionResult Fichas_Instructor(int id)
        {
            // Obtener fichas relacionadas al instructor
            var fichasInstructor = db.Ficha_has_Usuario
                .Where(fu => fu.Id_usuario == id && (fu.TipoUsuario == "Instructor" || fu.TipoUsuario == "InstructorAdmin"))
                .Select(fu => fu.Ficha)
                .ToList();

            ViewBag.InstructorId = id; // Pasar el ID del aprendiz a la vista
            ViewBag.Nombre_Instructor = db.Usuario.Find(id)?.Nombre_usuario;
            ViewBag.Apellido_Instructor = db.Usuario.Find(id)?.Apellido_usuario;

            return View(fichasInstructor);
        }

        [AutorizarTipoUsuario("InstructorAdmin")]
        public ActionResult Asociar_Ficha_Instructor(int idInstructor)
        {
            // Obtener las fichas ya asociadas al instructor
            var fichasAsociadas = db.Ficha_has_Usuario
                .Where(fu => fu.Id_usuario == idInstructor)
                .Select(fu => fu.Id_ficha)
                .ToList();

            // Obtener todas las fichas y excluir las ya asociadas
            var fichasDisponibles = db.Ficha
                .Where(f => !fichasAsociadas.Contains(f.Id_ficha))
                .ToList();

            ViewBag.Fichas = fichasDisponibles;
            ViewBag.InstructorId = idInstructor;
            ViewBag.Nombre_Instructor = db.Usuario.Find(idInstructor)?.Nombre_usuario;
            ViewBag.Apellido_Instructor = db.Usuario.Find(idInstructor)?.Apellido_usuario;
            return View();
        }

        [HttpPost]
        public ActionResult Asociar_Ficha_Instructor(int idInstructor, int fichaId)
        {
            if (ModelState.IsValid)
            {
                // Obtén el ID de la ficha seleccionado del formulario
                if (!int.TryParse(Request.Form["fichaId"], out fichaId))
                {
                    ModelState.AddModelError("", "Error al seleccionar la ficha.");
                    return View(); // O redirige a otra acción si lo prefieres
                }

                var ficha = db.Ficha.Find(fichaId);
                var instructor = db.Usuario.Find(idInstructor);

                if (instructor != null && (instructor.Tipo_usuario == "Instructor" || instructor.Tipo_usuario == "InstructorAdmin") && ficha != null)
                {
                    // Verificar si el instructor ya está asociado a la ficha
                    if (!db.Ficha_has_Usuario.Any(fhu => fhu.Id_ficha == fichaId && fhu.Id_usuario == idInstructor))
                    {
                        // Crea el objeto Ficha_has_Usuario aquí
                        var fichaHasUsuario = new Ficha_has_Usuario
                        {
                            Id_ficha = fichaId,
                            Id_usuario = idInstructor,
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

            // Si hay errores de validación, volver a mostrar el formulario con los datos previos
            // Obtener las fichas ya asociadas al instructor
            var fichasAsociadas = db.Ficha_has_Usuario
                .Where(fu => fu.Id_usuario == idInstructor)
                .Select(fu => fu.Id_ficha)
                .ToList();

            // Obtener todas las fichas y excluir las ya asociadas
            var fichasDisponibles = db.Ficha
                .Where(f => !fichasAsociadas.Contains(f.Id_ficha))
                .ToList();

            ViewBag.Fichas = fichasDisponibles;
            ViewBag.InstructorId = idInstructor;
            return RedirectToAction("Fichas_Instructor", new { id = idInstructor });
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
