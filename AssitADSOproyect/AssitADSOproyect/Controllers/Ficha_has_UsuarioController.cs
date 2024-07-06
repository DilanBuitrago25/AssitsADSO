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
    public class Ficha_has_UsuarioController : Controller
    {
        private BDAssistsADSOv4Entities db = new BDAssistsADSOv4Entities();

        // GET: Ficha_has_Usuario
        public ActionResult Index()
        {
            var ficha_has_Usuario = db.Ficha_has_Usuario.Include(f => f.Ficha).Include(f => f.Usuario);
            return View(ficha_has_Usuario.ToList());
        }

        // GET: Ficha_has_Usuario/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ficha_has_Usuario ficha_has_Usuario = db.Ficha_has_Usuario.Find(id);
            if (ficha_has_Usuario == null)
            {
                return HttpNotFound();
            }
            return View(ficha_has_Usuario);
        }

        // GET: Ficha_has_Usuario/Create
        public ActionResult Create()
        {
            ViewBag.Id_ficha = new SelectList(db.Ficha, "Id_ficha", "Jornada_ficha");
            ViewBag.Id_usuario = new SelectList(db.Usuario, "Id_usuario", "Tipo_Documento_usuario");
            return View();
        }

        // POST: Ficha_has_Usuario/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id_usuario,Id_ficha,TipoUsuario")] Ficha_has_Usuario ficha_has_Usuario)
        {
            if (ModelState.IsValid)
            {
                db.Ficha_has_Usuario.Add(ficha_has_Usuario);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Id_ficha = new SelectList(db.Ficha, "Id_ficha", "Jornada_ficha", ficha_has_Usuario.Id_ficha);
            ViewBag.Id_usuario = new SelectList(db.Usuario, "Id_usuario", "Tipo_Documento_usuario", ficha_has_Usuario.Id_usuario);
            return View(ficha_has_Usuario);
        }

        // GET: Ficha_has_Usuario/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ficha_has_Usuario ficha_has_Usuario = db.Ficha_has_Usuario.Find(id);
            if (ficha_has_Usuario == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id_ficha = new SelectList(db.Ficha, "Id_ficha", "Jornada_ficha", ficha_has_Usuario.Id_ficha);
            ViewBag.Id_usuario = new SelectList(db.Usuario, "Id_usuario", "Tipo_Documento_usuario", ficha_has_Usuario.Id_usuario);
            return View(ficha_has_Usuario);
        }

        // POST: Ficha_has_Usuario/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id_usuario,Id_ficha,TipoUsuario")] Ficha_has_Usuario ficha_has_Usuario)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ficha_has_Usuario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Id_ficha = new SelectList(db.Ficha, "Id_ficha", "Jornada_ficha", ficha_has_Usuario.Id_ficha);
            ViewBag.Id_usuario = new SelectList(db.Usuario, "Id_usuario", "Tipo_Documento_usuario", ficha_has_Usuario.Id_usuario);
            return View(ficha_has_Usuario);
        }

        // GET: Ficha_has_Usuario/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ficha_has_Usuario ficha_has_Usuario = db.Ficha_has_Usuario.Find(id);
            if (ficha_has_Usuario == null)
            {
                return HttpNotFound();
            }
            return View(ficha_has_Usuario);
        }

        // POST: Ficha_has_Usuario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ficha_has_Usuario ficha_has_Usuario = db.Ficha_has_Usuario.Find(id);
            db.Ficha_has_Usuario.Remove(ficha_has_Usuario);
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
