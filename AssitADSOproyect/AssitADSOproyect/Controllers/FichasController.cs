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
    public class FichasController : Controller
    {
        private BDAssistsADSOEntities db = new BDAssistsADSOEntities();
        [AutorizarTipoUsuario("Instructor")]
        // GET: Fichas1
        public ActionResult Index(string estadoFiltro = "")
        {
            string idUsuarioSesion = Session["Idusuario"].ToString();

            var FichasFiltradas = db.Ficha
                                         .Where(f => f.Id_Usuario.ToString() == idUsuarioSesion);

            if (estadoFiltro == "true")
            {
                FichasFiltradas = FichasFiltradas.Where(f => f.Estado_Ficha == true);
            }
            else if (estadoFiltro == "false")
            {
                FichasFiltradas = FichasFiltradas.Where(f => f.Estado_Ficha == false);
            }

            ViewBag.EstadoFiltro = estadoFiltro;

            return View(FichasFiltradas);
        }

        // GET: Fichas1/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ficha ficha = db.Ficha.Find(id);
            if (ficha == null)
            {
                return HttpNotFound();
            }
            return View(ficha);
        }

        // GET: Fichas1/Create
        public ActionResult Create()
        {
            ViewBag.Id_programa = new SelectList(db.Programa_formacion, "Id_programa", "Nombre_programa");
            return View();
        }

        // POST: Fichas1/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id_ficha,Codigo_ficha,Jornada_ficha,Modalidad_ficha,tipo_ficha,Id_programa,Fecha_inicio,Fecha_fin,Id_usuario")] Ficha ficha)
        {
            if (ModelState.IsValid)
            {
                db.Ficha.Add(ficha);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Id_programa = new SelectList(db.Programa_formacion, "Id_programa", "Nombre_programa", ficha.Id_programa);
            return View(ficha);
        }

        // GET: Fichas1/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ficha ficha = db.Ficha.Find(id);
            if (ficha == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id_programa = new SelectList(db.Programa_formacion, "Id_programa", "Nombre_programa", ficha.Id_programa);
            return View(ficha);
        }

        // POST: Fichas1/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id_ficha,Codigo_ficha,Jornada_ficha,Modalidad_ficha,tipo_ficha,Id_programa,Fecha_inicio,Fecha_fin,Id_usuario,Estado_ficha")] Ficha ficha)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ficha).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Id_programa = new SelectList(db.Programa_formacion, "Id_programa", "Nombre_programa", ficha.Id_programa);
            return View(ficha);
        }

        // GET: Fichas1/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ficha ficha = db.Ficha.Find(id);
            if (ficha == null)
            {
                return HttpNotFound();
            }
            return View(ficha);
        }

        // POST: Fichas1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ficha ficha = db.Ficha.Find(id);
            db.Ficha.Remove(ficha);
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
