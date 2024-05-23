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
    public class Asistencias1Controller : Controller
    {
        private BDAssistsADSOEntities db = new BDAssistsADSOEntities();

        // GET: Asistencias1
        public ActionResult Index()
        {
            var asistencia = db.Asistencia.Include(a => a.Usuario).Include(a => a.Competencia).Include(a => a.Ficha);
            return View(asistencia.ToList());
        }

        // GET: Asistencias1/Details/5
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

        // GET: Asistencias1/Create
        public ActionResult Create()
        {
            ViewBag.Id_usuario = new SelectList(db.Usuario, "Id_usuario", "Tipo_Documento_usuario");
            ViewBag.Id_competencia = new SelectList(db.Competencia, "Id_competencia", "Nombre_competencia");
            ViewBag.Id_ficha = new SelectList(db.Ficha, "Id_ficha", "Jornada_ficha");
            return View();
        }

        // POST: Asistencias1/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id_asistencia,Fecha_inicio_asistencia,Fecha_fin_asistencia,Hora_inicio_asistencia,Hora_fin_asistencia,Detalles_asistencia,Id_usuario,Id_ficha,Id_competencia")] Asistencia asistencia)
        {
            if (ModelState.IsValid)
            {
                db.Asistencia.Add(asistencia);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Id_usuario = new SelectList(db.Usuario, "Id_usuario", "Tipo_Documento_usuario", asistencia.Id_usuario);
            ViewBag.Id_competencia = new SelectList(db.Competencia, "Id_competencia", "Nombre_competencia", asistencia.Id_competencia);
            ViewBag.Id_ficha = new SelectList(db.Ficha, "Id_ficha", "Jornada_ficha", asistencia.Id_ficha);
            return View(asistencia);
        }

        // GET: Asistencias1/Edit/5
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
            ViewBag.Id_usuario = new SelectList(db.Usuario, "Id_usuario", "Tipo_Documento_usuario", asistencia.Id_usuario);
            ViewBag.Id_competencia = new SelectList(db.Competencia, "Id_competencia", "Nombre_competencia", asistencia.Id_competencia);
            ViewBag.Id_ficha = new SelectList(db.Ficha, "Id_ficha", "Jornada_ficha", asistencia.Id_ficha);
            return View(asistencia);
        }

        // POST: Asistencias1/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id_asistencia,Fecha_inicio_asistencia,Fecha_fin_asistencia,Hora_inicio_asistencia,Hora_fin_asistencia,Detalles_asistencia,Id_usuario,Id_ficha,Id_competencia")] Asistencia asistencia)
        {
            if (ModelState.IsValid)
            {
                db.Entry(asistencia).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Id_usuario = new SelectList(db.Usuario, "Id_usuario", "Tipo_Documento_usuario", asistencia.Id_usuario);
            ViewBag.Id_competencia = new SelectList(db.Competencia, "Id_competencia", "Nombre_competencia", asistencia.Id_competencia);
            ViewBag.Id_ficha = new SelectList(db.Ficha, "Id_ficha", "Jornada_ficha", asistencia.Id_ficha);
            return View(asistencia);
        }

        // GET: Asistencias1/Delete/5
        public ActionResult Delete(int? id)
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

        // POST: Asistencias1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Asistencia asistencia = db.Asistencia.Find(id);
            db.Asistencia.Remove(asistencia);
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
