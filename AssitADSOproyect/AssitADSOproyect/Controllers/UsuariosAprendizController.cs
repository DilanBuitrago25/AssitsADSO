using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ClaseDatos;

namespace AssitADSOproyect.Controllers
{
    public class UsuariosAprendizController : Controller
    {
        private BDAssistsADSOEntities db = new BDAssistsADSOEntities();

        // GET: UsuariosAprendiz
        public ActionResult Index()
        {
            var usuario = db.Usuario.Include(u => u.Ficha);
            return View(usuario.ToList());
        }

        // GET: UsuariosAprendiz/Details/5
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
        public ActionResult Create([Bind(Include = "Id_usuario,Documento_usuario,Tipo_usuario,Tipo_Documento_usuario,Nombre_usuario,Apellido_usuario,Telefono_usuario,Correo_usuario,Contrasena_usuario,Id_ficha")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                db.Usuario.Add(usuario);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Id_ficha = new SelectList(db.Ficha, "Id_ficha", "Codigo_ficha" /*"Jornada_ficha"*/, usuario.Id_ficha);
            return View(usuario);
        }

        // GET: UsuariosAprendiz/Edit/5
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
            ViewBag.Id_ficha = new SelectList(db.Ficha, "Id_ficha", "Codigo_ficha" /*"Jornada_ficha"*/, usuario.Id_ficha);
            return View(usuario);
        }

        // POST: UsuariosAprendiz/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id_usuario,Documento_usuario,Tipo_usuario,Tipo_Documento_usuario,Nombre_usuario,Apellido_usuario,Telefono_usuario,Correo_usuario,Contrasena_usuario,Id_ficha")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                db.Entry(usuario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Id_ficha = new SelectList(db.Ficha, "Id_ficha", "Codigo_ficha" /*"Jornada_ficha"*/, usuario.Id_ficha);
            return View(usuario);
        }

        // GET: UsuariosAprendiz/Delete/5
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

        // POST: UsuariosAprendiz/Delete/5
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
