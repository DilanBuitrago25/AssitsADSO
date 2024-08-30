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
using System.Security.Cryptography;
using System.Text;

namespace AssitADSOproyect.Controllers
{
    public class UsuariosAprendizController : Controller
    {
        private BDAssistsADSOv4Entities db = new BDAssistsADSOv4Entities();
        [AutorizarTipoUsuario("InstructorAdmin")]
        // GET: UsuariosAprendiz
        public ActionResult Index(string estadoFiltro = "")
        {
            bool? estado = null; // Nullable bool for "Todos"

            if (estadoFiltro == "true")
                estado = true;
            else if (estadoFiltro == "false")
                estado = false;

            var AprendizFiltro = db.Usuario
                .Where(u => u.Tipo_usuario == "Aprendiz" && (!estado.HasValue || u.Estado_Usuario == estado))
                .ToList();

            ViewBag.EstadoFiltro = estadoFiltro;

            return View(AprendizFiltro);
        }

        public ActionResult GenerarReportePDF()
        {
            var AprendizFiltro = db.Usuario
            .Where(u => u.Tipo_usuario == "Aprendiz")
            .ToList();

            using (MemoryStream memoryStream = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 50, 50, 80, 50);
                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);

                writer.PageEvent = new HeaderFooterEvent(Server.MapPath("~/assets/images/Logo-remove.png"));

                document.Open();

                // Título
                Paragraph titulo = new Paragraph("Reporte de Aprendices", new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD));
                titulo.Alignment = Element.ALIGN_CENTER;
                document.Add(titulo);

                document.Add(Chunk.NEWLINE);


                PdfPTable table = new PdfPTable(7);
                table.WidthPercentage = 100;

                // Encabezados de la tabla
                Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                table.AddCell(new Phrase("Tipo Documento", headerFont));
                table.AddCell(new Phrase("Documento"));
                table.AddCell(new Phrase("Nombre"));
                table.AddCell(new Phrase("Apellido"));
                table.AddCell(new Phrase("Telefono"));
                table.AddCell(new Phrase("Correo"));
                table.AddCell(new Phrase("Estado Aprendiz"));




                Font cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                foreach (var aprendiz in AprendizFiltro)
                {
                    table.AddCell(new Phrase(aprendiz.Tipo_Documento_usuario));
                    table.AddCell(new Phrase(aprendiz.Documento_usuario.ToString()));
                    table.AddCell(new Phrase(aprendiz.Nombre_usuario));
                    table.AddCell(new Phrase(aprendiz.Apellido_usuario));
                    table.AddCell(new Phrase(aprendiz.Telefono_usuario.ToString()));
                    table.AddCell(new Phrase(aprendiz.Correo_usuario));
                    table.AddCell(new Phrase(aprendiz.Estado_Usuario.ToString()));

                }

                document.Add(table);
                document.Close();


                return File(memoryStream.ToArray(), "application/pdf", "ReporteAprendices.pdf");
            }
        }

        [HttpPost]
        public ActionResult GenerarReportePDFAprendizFicha(int id)
        {
            try
            {
                var fichasAprendiz = db.Ficha_has_Usuario
               .Where(fu => fu.Id_usuario == id && fu.TipoUsuario == "Aprendiz")
               .Select(fu => fu.Ficha)
               .ToList();

                var aprendiz = db.Usuario.Find(id); // Busca la ficha por su ID
                string nombreAprendiz = aprendiz?.Nombre_usuario + aprendiz.Apellido_usuario; // Obtiene el código de la ficha

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    Document document = new Document(PageSize.A4.Rotate(), 50, 50, 50, 35);
                    PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);

                    writer.PageEvent = new HeaderFooterEvent(Server.MapPath("~/assets/images/Logo-remove.png")); // Pasar la ruta de la imagen

                    document.Open();

                    // Título
                    Paragraph titulo = new Paragraph("Reporte de Fichas del Aprendiz " + nombreAprendiz, new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD));
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
                    foreach (var fichasapre in fichasAprendiz)
                    {
                        table.AddCell(new Phrase(fichasapre.Codigo_ficha.ToString()));
                        table.AddCell(new Phrase(fichasapre.Jornada_ficha));
                        table.AddCell(new Phrase(fichasapre.Modalidad_ficha));
                        table.AddCell(new Phrase(fichasapre.tipo_ficha));
                        table.AddCell(new Phrase(fichasapre.Programa_formacion.Nombre_programa));
                    }

                    document.Add(table);
                    document.Close();

                    return File(memoryStream.ToArray(), "application/pdf", "Reportefichasde" + nombreAprendiz + ".pdf");

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

        // GET: UsuariosAprendiz/Details/5
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

        // GET: UsuariosAprendiz/Create
        [AutorizarTipoUsuario("InstructorAdmin")]
        public ActionResult Create()
        {
            ViewBag.Id_ficha = new SelectList(db.Ficha, "Id_ficha","Codigo_ficha" /*"Jornada_ficha"*/);
            return View();
        }

        // POST: UsuariosAprendiz/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id_usuario,Documento_usuario,Tipo_usuario,Tipo_Documento_usuario,Nombre_usuario,Apellido_usuario,Telefono_usuario,Correo_usuario,Contrasena_usuario,Estado_usuario")] Usuario usuario)
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
                    using (var sha256 = SHA256.Create())
                    {
                        byte[] passwordBytes = Encoding.UTF8.GetBytes(usuario.Contrasena_usuario);

                        byte[] hashBytes = sha256.ComputeHash(passwordBytes);

                        usuario.Contrasena_usuario = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                    }
                    db.Usuario.Add(usuario);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            // Si hay errores, volver a la vista con los mensajes de error
            ViewBag.Id_ficha = new SelectList(db.Ficha, "Id_ficha", "Codigo_ficha", usuario.Ficha_has_Usuario);
            return View(usuario);
        }

        // GET: UsuariosAprendiz/Edit/5
        [AutorizarTipoUsuario("InstructorAdmin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuario.Find(id);
            //Ficha_has_Usuario ficha_Has_Usuario = db.Ficha_has_Usuario.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            //ViewBag.Id_ficha = new SelectList(db.Ficha, "Id_ficha", "Codigo_ficha" /*"Jornada_ficha"*/, ficha_Has_Usuario.Id_ficha);
            return View(usuario);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id_usuario,Tipo_Documento_usuario,Documento_usuario,Nombre_usuario,Apellido_usuario,Telefono_usuario,Correo_usuario,Tipo_usuario,Tipo_instructor,Id_ficha,Estado_usuario")] Usuario usuario, Ficha_has_Usuario ficha_Has_Usuario)
        {
            if (ModelState.IsValid)
            {
                db.Entry(usuario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Id_ficha = new SelectList(db.Ficha, "Id_ficha", "Codigo_ficha", ficha_Has_Usuario.Id_ficha);
            return View(usuario);
        }

        //desde aca tablas foranea
        [AutorizarTipoUsuario("InstructorAdmin")]
        public ActionResult Fichas_Aprendiz(int id)
        {
           
            var fichasAprendiz = db.Ficha_has_Usuario
                .Where(fu => fu.Id_usuario == id && fu.TipoUsuario == "Aprendiz")
                .Select(fu => fu.Ficha)
                .ToList();

            ViewBag.AprendizId = id; 
            ViewBag.Nombre_Aprendiz = db.Usuario.Find(id)?.Nombre_usuario;
            ViewBag.Apellido_Aprendiz = db.Usuario.Find(id)?.Apellido_usuario;

            return View(fichasAprendiz);
        }

        [AutorizarTipoUsuario("InstructorAdmin")]
        public ActionResult Asociar_Ficha_Aprendiz(int idAprendiz)
        {
            // Obtener las fichas ya asociadas al aprendiz
            var fichasAsociadas = db.Ficha_has_Usuario
                .Where(fu => fu.Id_usuario == idAprendiz)
                .Select(fu => fu.Id_ficha)
                .ToList();

            // Obtener todas las fichas y excluir las ya asociadas
            var fichasDisponibles = db.Ficha
                .Where(f => !fichasAsociadas.Contains(f.Id_ficha))
                .ToList();

            ViewBag.Fichas = fichasDisponibles;
            ViewBag.AprendizId = idAprendiz;
            ViewBag.Nombre_Aprendiz = db.Usuario.Find(idAprendiz)?.Nombre_usuario;
            ViewBag.Apellido_Aprendiz = db.Usuario.Find(idAprendiz)?.Apellido_usuario;
            return View();
        }


        [HttpPost]
        public ActionResult Asociar_Ficha_Aprendiz(int idAprendiz, int fichaId)
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
                var aprendiz = db.Usuario.Find(idAprendiz);

                if (aprendiz != null && aprendiz.Tipo_usuario == "Aprendiz" && ficha != null)
                {
                    // Verificar si el aprendiz ya está asociado a la ficha
                    if (!db.Ficha_has_Usuario.Any(fhu => fhu.Id_ficha == fichaId && fhu.Id_usuario == idAprendiz))
                    {
                        // Crea el objeto Ficha_has_Usuario aquí
                        var fichaHasUsuario = new Ficha_has_Usuario
                        {
                            Id_ficha = fichaId,
                            Id_usuario = idAprendiz,
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

            // Si hay errores de validación, volver a mostrar el formulario con los datos previos
            // Obtener las fichas ya asociadas al aprendiz
            var fichasAsociadas = db.Ficha_has_Usuario
                .Where(fu => fu.Id_usuario == idAprendiz)
                .Select(fu => fu.Id_ficha)
                .ToList();

            // Obtener todas las fichas y excluir las ya asociadas
            var fichasDisponibles = db.Ficha
                .Where(f => !fichasAsociadas.Contains(f.Id_ficha))
                .ToList();

            ViewBag.Fichas = fichasDisponibles;
            ViewBag.AprendizId = idAprendiz;
            return RedirectToAction("Fichas_Aprendiz", new { id = idAprendiz });
        }

        public ActionResult GenerarNuevaContrasena(int idUsuario)
        {
            // Obtener el usuario de la base de datos
            var usuario = db.Usuario.Find(idUsuario);

            if (usuario == null)
            {
                return HttpNotFound(); // O manejar el caso de usuario no encontrado de otra forma
            }

            // Generar una nueva contraseña aleatoria (puedes personalizar la longitud y complejidad)
            string nuevaContrasena = GenerarContrasenaAleatoria(10); // Ajusta la longitud según tus necesidades

            // Aplicar el mismo algoritmo de hashing que usas al crear usuarios
            using (var sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(nuevaContrasena);
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);
                usuario.Contrasena_usuario = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }

            // Guardar los cambios en la base de datos
            db.Entry(usuario).State = EntityState.Modified;
            db.SaveChanges();

            // Pasar la nueva contraseña (en texto plano) a la vista parcial
            return PartialView("_MostrarNuevaContrasena", nuevaContrasena);
        }

        // Método auxiliar para generar contraseñas aleatorias
        private string GenerarContrasenaAleatoria(int longitud)
        {
            const string caracteresValidos = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_-+=[{]};:>|./?";
            var random = new Random();
            var contrasena = new char[longitud];

            for (int i = 0; i < longitud; i++)
            {
                contrasena[i] = caracteresValidos[random.Next(caracteresValidos.Length)];
            }

            return new string(contrasena);
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
