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

namespace AssitADSOproyect.Controllers
{
    public class JustificacionInasistenciaController : Controller
    {
        private BDAssistsADSOv4Entities db = new BDAssistsADSOv4Entities();
        [AutorizarTipoUsuario("Instructor", "InstructorAdmin", "Aprendizs")]
        // GET: JustificacionInasistencia
        public ActionResult Index()
        {
            var soporte = db.Soporte.Include(s => s.Asistencia);
            return View(soporte.ToList());
            
        }

        public ActionResult MisSoportes()
        {
            if (Session["Idusuario"] != null)
            {
                string idUsuarioSesion = Session["Idusuario"].ToString();

                var Soportesfiltrados = db.Soporte
                                             .Where(r => r.Id_Aprendiz.ToString() == idUsuarioSesion);
               
                return View(Soportesfiltrados);
            }
            else
            {
                return RedirectToAction("Login");
            }
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

        [HttpGet]
        public JsonResult GetAsistenciasPorInstructor(int instructorId)
        {
            var asistencias = db.Asistencia
                .Where(a => a.Id_Instructor == instructorId)
                .Select(a => new { a.Id_asistencia, a.Fecha_inicio_asistencia }) // O el campo que desees mostrar
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
                ViewBag.Fecha_inicio_asistencia = asistencia.Fecha_inicio_asistencia;// Formato de fecha
            }
            else
            {
                // Manejar el caso donde la asistencia no existe
                ViewBag.Fecha_inicio_asistencia = "Fecha no encontrada"; // O un mensaje de error
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
                    ModelState.AddModelError("archivo", "Debe seleccionar un archivo.");
                }
                var asistencia = db.Asistencia.Find(soporte.Id_asistencia);
                if (asistencia != null)
                {
                    soporte.Id_Instructor = asistencia.Id_Instructor; // Asignar el ID del instructor
                }
                db.Soporte.Add(soporte);
                db.SaveChanges();

                return RedirectToAction("MisSoportes");
            }
                ViewBag.Fecha_asistencia = new SelectList(db.Asistencia, "Id_asistencia", "Fecha_inicio_asistencia", soporte.Id_asistencia);
                ViewBag.Id_Instructor = new SelectList(db.Usuario.Where(u => u.Tipo_usuario == "Instructor"), "Id_Usuario", "Nombre_Usuario", soporte.Id_Instructor);
            
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
            ViewBag.RutaArchivo = soporte.Archivo_soporte;
            return View(soporte);
        }

        //// POST: JustificacionInasistencia/Edit/5
        //// Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        //// más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id_soporte, Validacion_Instructor, Nota_Instructor")] Soporte soporte, string accion)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var soporteExistente = db.Soporte.Find(soporte.Id_soporte);

        //            if (soporteExistente != null)
        //            {
        //                if (accion == "rechazar")
        //                {
        //                    soporte.Validacion_Instructor = false;
        //                }
        //                else if (accion == "validar")
        //                {
        //                    soporte.Validacion_instructor = true;
        //                }
        //                db.Entry(soporte).State = EntityState.Modified;
        //                db.SaveChanges();
        //                return RedirectToAction("Index");
        //            }

        //            return RedirectToAction("Details", new { id = soporte.Id_soporte });
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            return RedirectToAction("Index");
        //        }
        //        catch (Exception ex)
        //        {
        //            ModelState.AddModelError("", "Error al guardar los cambios: " + ex.Message);
        //        }
        //    }

        //    ViewBag.Id_asistencia = new SelectList(db.Asistencia, "Id_asistencia", "Tipo_asistencia", soporte.Id_asistencia);
        //    return View(soporte);
        //}
        // GET: JustificacionInasistencia/Delete/5
        //    public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Soporte soporte = db.Soporte.Find(id);
        //    if (soporte == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(soporte);
        //}

        //// POST: JustificacionInasistencia/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Soporte soporte = db.Soporte.Find(id);
        //    db.Soporte.Remove(soporte);
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
