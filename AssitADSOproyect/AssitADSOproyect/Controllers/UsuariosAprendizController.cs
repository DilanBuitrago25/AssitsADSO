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
    public class UsuariosAprendizController : Controller
    {
        private BDAssistsADSOv4Entities db = new BDAssistsADSOv4Entities();
        [AutorizarTipoUsuario("InstructorAdmin")]
        // GET: UsuariosAprendiz
        public ActionResult Index()
        {
            var AprendizFiltro = db.Usuario
            .Where(u => u.Tipo_usuario == "Aprendiz" && u.Estado_Usuario == true)
            .ToList();

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
                table.AddCell(new Phrase("Codigo Ficha"));
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
                    //table.AddCell(new Phrase(aprendiz.Ficha2.Codigo_ficha.ToString()));
                    table.AddCell(new Phrase(aprendiz.Estado_Usuario.ToString()));

                }

                document.Add(table);
                document.Close();


                return File(memoryStream.ToArray(), "application/pdf", "ReporteAprendices.pdf");
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
                db.Usuario.Add(usuario);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Id_ficha = new SelectList(db.Ficha, "Id_ficha", "Codigo_ficha" /*"Jornada_ficha"*/, usuario.Ficha_has_Usuario);
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
        public ActionResult Edit([Bind(Include = "Id_usuario,Tipo_Documento_usuario,Documento_usuario,Nombre_usuario,Apellido_usuario,Telefono_usuario,Correo_usuario,Contrasena_usuario,Tipo_usuario,Tipo_instructor,Id_ficha,Estado_usuario")] Usuario usuario, Ficha_has_Usuario ficha_Has_Usuario)
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
            // Obtener fichas relacionadas al aprendiz
            var fichasAprendiz = db.Ficha_has_Usuario
                .Where(fu => fu.Id_usuario == id && fu.TipoUsuario == "Aprendiz")
                .Select(fu => fu.Ficha)
                .ToList();

            ViewBag.AprendizId = id; // Pasar el ID del aprendiz a la vista
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
