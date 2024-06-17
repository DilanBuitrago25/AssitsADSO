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
    public class Programa_formacionController : Controller
    {
        private BDAssistsADSOEntities db = new BDAssistsADSOEntities();
        [AutorizarTipoUsuario("Instructor")]
        // GET: Programa_formacion
        public ActionResult Index()
        {
            //return View(db.Programa_formacion.ToList());
            string idUsuarioSesion = Session["Idusuario"].ToString();

            // Filtrar las fichas por Id_Usuario
            var ProgramasFiltrados = db.Programa_formacion.Where(f => f.Id_Usuario.ToString() == idUsuarioSesion);
            return View(ProgramasFiltrados);
        }

        // GET: Programa_formacion/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Programa_formacion programa_formacion = db.Programa_formacion.Find(id);
            if (programa_formacion == null)
            {
                return HttpNotFound();
            }
            return View(programa_formacion);
        }

        // GET: Programa_formacion/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Programa_formacion/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id_programa,Nombre_programa,Tipo_programa,Duracion_programa,Id_usuario")] Programa_formacion programa_formacion)
        {
            if (ModelState.IsValid)
            {
                db.Programa_formacion.Add(programa_formacion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(programa_formacion);
        }

        // GET: Programa_formacion/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Programa_formacion programa_formacion = db.Programa_formacion.Find(id);
            if (programa_formacion == null)
            {
                return HttpNotFound();
            }
            return View(programa_formacion);
        }

        // POST: Programa_formacion/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id_programa,Nombre_programa,Tipo_programa,Duracion_programa")] Programa_formacion programa_formacion) /* posibilidad de implementar el id usaurio con el session*/
        {
            if (ModelState.IsValid)
            {
                db.Entry(programa_formacion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(programa_formacion);
        }

        // GET: Programa_formacion/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Programa_formacion programa_formacion = db.Programa_formacion.Find(id);
            if (programa_formacion == null)
            {
                return HttpNotFound();
            }
            return View(programa_formacion);
        }

        // POST: Programa_formacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Programa_formacion programa_formacion = db.Programa_formacion.Find(id);
            db.Programa_formacion.Remove(programa_formacion);
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
