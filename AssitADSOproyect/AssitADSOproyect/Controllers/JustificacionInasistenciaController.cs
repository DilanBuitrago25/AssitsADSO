﻿using System;
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
    public class JustificacionInasistenciaController : Controller
    {
        private BDAssistsADSOEntities db = new BDAssistsADSOEntities();

        // GET: JustificacionInasistencia
        public ActionResult Index()
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
        public ActionResult Create()
        {
            ViewBag.Id_asistencia = new SelectList(db.Asistencia, "Id_asistencia", "Tipo_asistencia");
            return View();
        }

        // POST: JustificacionInasistencia/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id_soporte,Nombre_soporte,Descripcion_soporte,Tipo_soporte,Id_asistencia")] Soporte soporte)
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
    }
}