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
using Ganss.Xss;
using static AssitADSOproyect.Controllers.LoginController;

namespace AssitADSOproyect.Controllers
{
    public class JustificacionInasistenciaController : Controller
    {
        private BDAssistsADSOEntities db = new BDAssistsADSOEntities();

        // GET: JustificacionInasistencia
        public ActionResult Index()
        {
            var soporte = db.Soporte.Include(s => s.Asistencia);
            return View(soporte.ToList());
        }
        
        public ActionResult IndexAprendiz()
        {
            var soporte = db.Soporte.Include(s => s.Asistencia);
            return View(soporte.ToList());
        }

        // GET: JustificacionInasistencia/Details/5
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
            return View(soporte);
        }

        // GET: JustificacionInasistencia/Create
        [AutorizarTipoUsuario("Aprendiz")]
        public ActionResult Create()
        {
            ViewBag.Id_asistencia = new SelectList(db.Asistencia, "Id_asistencia", "Tipo_asistencia");
            ViewBag.Id_Usuario = new SelectList(db.Usuario, "Id_Usuario", "Nombre_Usuario");
            ViewBag.Fecha_Asistencia = new SelectList(db.Asistencia, "Id_Asistencia", "Fecha_inicio_asistencia");
            ViewBag.Id_Competencia = new SelectList(db.Competencia, "Id_Competencia", "Nombre_Competencia");
            return View();
        }

        public ActionResult CreateAprendiz()
        {
            ViewBag.Id_asistencia = new SelectList(db.Asistencia, "Id_asistencia", "Tipo_asistencia");
            return View();
        }

        // POST: JustificacionInasistencia/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
[ValidateAntiForgeryToken]
[AutorizarTipoUsuario("Aprendiz")]
public ActionResult Create([Bind(Include = "Id_soporte,Nombre_soporte,Descripcion_soporte,Tipo_soporte,Id_asistencia")] Soporte soporte, HttpPostedFileBase archivoPDF)
{
            if (ModelState.IsValid)
            {
                var sanitizer = new HtmlSanitizer();
                soporte.Descripcion_soporte = sanitizer.Sanitize(soporte.Descripcion_soporte);
                //if (Path.GetExtension(soporte.Tipo_soporte).ToLower() == ".pdf")
                //{
                //archivoPDF = soporte.Tipo_soporte;

                    // Guardar el archivo y obtener la ruta
                    var rutaCarpeta = Server.MapPath("~/ArchivosSoportes/");
                    var nombreArchivo = Path.GetFileName(archivoPDF.FileName);
                    var rutaCompleta = Path.Combine(rutaCarpeta, nombreArchivo);
                    archivoPDF.SaveAs(rutaCompleta);

                    // Asignar la ruta al modelo (usando la propiedad correcta)
                    soporte.Tipo_soporte = "~/ArchivosSoportes/" + nombreArchivo; // Ruta relativa


                    db.Soporte.Add(soporte);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                //}
                //else
                //{
                //    ModelState.AddModelError("Tipo_soporte", "Solo se permiten archivos PDF.");
                //}
            }
            ViewBag.Fecha_Asistencia = new SelectList(db.Asistencia, "Id_asistencia", "Fecha_inicio_asistencia", soporte.Id_asistencia);
            ViewBag.Id_Usuario = new SelectList(db.Usuario, "Id_Usuario", "Nombre_Usuario", soporte.Id_Instructor);
            return View(soporte);




        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAprendiz([Bind(Include = "Id_soporte,Nombre_soporte,Descripcion_soporte,Tipo_soporte,Id_asistencia")] Soporte soporte)
        {
            if (ModelState.IsValid)
            {
                db.Soporte.Add(soporte);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Id_asistencia = new SelectList(db.Asistencia, "Id_asistencia", "Tipo_asistencia", soporte.Id_asistencia);
            return View(soporte);
        }

        // GET: JustificacionInasistencia/Edit/5
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
            ViewBag.Id_asistencia = new SelectList(db.Asistencia, "Id_asistencia", "Tipo_asistencia", soporte.Id_asistencia);
            return View(soporte);
        }

        // POST: JustificacionInasistencia/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id_soporte,Nombre_soporte,Descripcion_soporte,Tipo_soporte,Id_asistencia")] Soporte soporte)
        {
            if (ModelState.IsValid)
            {
                db.Entry(soporte).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Id_asistencia = new SelectList(db.Asistencia, "Id_asistencia", "Tipo_asistencia", soporte.Id_asistencia);
            return View(soporte);
        }

        // GET: JustificacionInasistencia/Delete/5
        public ActionResult Delete(int? id)
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
            return View(soporte);
        }

        // POST: JustificacionInasistencia/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Soporte soporte = db.Soporte.Find(id);
            db.Soporte.Remove(soporte);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult Upload()
        {
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    // Validación del tipo de archivo
                    if (file.ContentType != "application/pdf")
                    {
                        return Json(new { error = "Solo se permiten archivos PDF" });
                    }

                    // Generar un nombre de archivo único y seguro
                    var fileName = Guid.NewGuid().ToString() + ".pdf";
                    var path = Path.Combine(Server.MapPath("~/uploads"), fileName);

                    // Guardar el archivo
                    file.SaveAs(path);

                    // Actualizar la base de datos usando Entity Framework
                    
                        //var soporte = new Soporte { Tipo_soporte = "~/uploads/" + fileName }; // Ruta relativa
                        //db.Soporte.Add(soporte);
                        //db.SaveChanges();
                    

                    return Json(new { success = true, filePath = "~/uploads/" + fileName });
                }
            }

            return Json(new { error = "No se seleccionó ningún archivo" });
        }
    }
}
