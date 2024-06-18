﻿using System;
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
    public class CompetenciasController : Controller
    {
        private BDAssistsADSOEntities db = new BDAssistsADSOEntities();
        [AutorizarTipoUsuario("Instructor")]
        // GET: Competencias
        public ActionResult Index(string estadoFiltro = "")
        {
            string idUsuarioSesion = Session["Idusuario"].ToString();

            var CompetenciasFiltradas = db.Competencia
                                         .Where(c => c.Id_Usuario.ToString() == idUsuarioSesion);

            if (estadoFiltro == "true")
            {
                CompetenciasFiltradas = CompetenciasFiltradas.Where(c => c.Estado_Competencia == true);
            }
            else if (estadoFiltro == "false")
            {
                CompetenciasFiltradas = CompetenciasFiltradas.Where(c => c.Estado_Competencia == false);
            }

            ViewBag.EstadoFiltro = estadoFiltro; 

            return View(CompetenciasFiltradas);
        }




        // GET: Competencias/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Competencia competencia = db.Competencia.Find(id);
            if (competencia == null)
            {
                return HttpNotFound();
            }
            return View(competencia);
        }

        // GET: Competencias/Create
        public ActionResult Create()
        {
            ViewBag.Id_programa = new SelectList(db.Programa_formacion, "Id_programa", "Nombre_programa");
            ViewBag.Numero_ficha = new SelectList(db.Ficha, "Id_ficha", "Codigo_ficha");
            return View();
        }

        // POST: Competencias/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id_competencia,tipo_competencia,Numero_ficha,Id_programa,Nombre_competencia,Id_usuario")] Competencia competencia)
        {
            if (ModelState.IsValid)
            {
                db.Competencia.Add(competencia);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Id_programa = new SelectList(db.Programa_formacion, "Id_programa", "Nombre_programa", competencia.Id_programa);
            ViewBag.Numero_ficha = new SelectList(db.Ficha, "Id_ficha", "Codigo_ficha", competencia.Numero_ficha);
            return View(competencia);
        }

        // GET: Competencias/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Competencia competencia = db.Competencia.Find(id);
            if (competencia == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id_programa = new SelectList(db.Programa_formacion, "Id_programa", "Nombre_programa", competencia.Id_programa);
            ViewBag.Numero_ficha = new SelectList(db.Ficha, "Id_ficha", "Codigo_ficha", competencia.Numero_ficha);
            return View(competencia);
        }

        // POST: Competencias/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id_competencia,tipo_competencia,Numero_ficha,Id_programa,Nombre_competencia,Id_usuario,Estado_competencia")] Competencia competencia)
        {
            if (ModelState.IsValid)
            {
                db.Entry(competencia).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Id_programa = new SelectList(db.Programa_formacion, "Id_programa", "Nombre_programa", competencia.Id_programa);
            ViewBag.Numero_ficha = new SelectList(db.Ficha, "Id_ficha", "Codigo_ficha", competencia.Numero_ficha);
            return View(competencia);
        }

        // GET: Competencias/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Competencia competencia = db.Competencia.Find(id);
            if (competencia == null)
            {
                return HttpNotFound();
            }
            return View(competencia);
        }

        // POST: Competencias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Competencia competencia = db.Competencia.Find(id);
            db.Competencia.Remove(competencia);
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
