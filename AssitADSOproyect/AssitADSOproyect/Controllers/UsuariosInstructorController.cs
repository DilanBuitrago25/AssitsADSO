using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ClaseDatos;
using static AssitADSOproyect.Controllers.LoginController;

namespace AssitADSOproyect.Controllers
{
    public class UsuariosInstructorController : Controller
    {
        private BDAssistsADSOEntities db = new BDAssistsADSOEntities();
        [AutorizarTipoUsuario("Instructor")]
        // GET: UsuariosInstructor
        public ActionResult Index()
        {
            var InstructorFiltro = db.Usuario
            .Where(u => u.Tipo_usuario == "Instructor" && u.Estado_Usuario == true)
            .ToList();

            return View(InstructorFiltro);
        }

        // GET: UsuariosInstructor/Details/5
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
        public ActionResult Create([Bind(Include = "Id_usuario,Documento_usuario,Tipo_usuario,Tipo_Documento_usuario,Nombre_usuario,Apellido_usuario,Telefono_usuario,Correo_usuario,Tipo_instructor,Id_ficha,Esinstructormaster_usuario,Contrasena_usuario")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                db.Usuario.Add(usuario);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Id_ficha = new SelectList(db.Ficha, "Id_ficha", "Jornada_ficha", usuario.Id_ficha);
            return View(usuario);
        }

        // GET: UsuariosInstructor/Edit/5
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
            ViewBag.Id_ficha = new SelectList(db.Ficha, "Id_ficha", "Jornada_ficha", usuario.Id_ficha);
            return View(usuario);
        }

        // POST: UsuariosInstructor/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id_usuario,Documento_usuario,Tipo_Documento_usuario,Nombre_usuario,Apellido_usuario,Telefono_usuario,Correo_usuario,Contrasena_usuario,Tipo_instructor,Tipo_usuario,Id_ficha,Esinstructormaster_usuario,Estado_usuario")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                db.Entry(usuario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Id_ficha = new SelectList(db.Ficha, "Id_ficha", "Jornada_ficha", usuario.Id_ficha);
            return View(usuario);
        }

        // GET: UsuariosInstructor/Delete/5
        public ActionResult Delete(int? id)
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

        // POST: UsuariosInstructor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Usuario usuario = db.Usuario.Find(id);
            db.Usuario.Remove(usuario);
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
    }
}
