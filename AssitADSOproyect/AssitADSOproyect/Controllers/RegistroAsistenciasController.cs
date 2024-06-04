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
    public class RegistroAsistenciasController : Controller
    {
        private BDAssistsADSOEntities db = new BDAssistsADSOEntities();

        // GET: RegistroAsistencias
        public ActionResult Index()
        {
            //var registroAsistencia = db.RegistroAsistencia.Include(r => r.Asistencia).Include(r => r.Usuario);
            //return View(registroAsistencia.ToList());
            string idUsuarioSesion = Session["Idusuario"].ToString();

            // Filtrar las fichas por Id_Usuario
            var RegistrosFiltrados = db.RegistroAsistencia.Where(f => f.Id_usuario.ToString() == idUsuarioSesion);
            return View(RegistrosFiltrados);
        }

        // GET: RegistroAsistencias/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RegistroAsistencia registroAsistencia = db.RegistroAsistencia.Find(id);
            if (registroAsistencia == null)
            {
                return HttpNotFound();
            }
            return View(registroAsistencia);
        }

        // GET: RegistroAsistencias/Create
        public ActionResult Create(int? Id_Asistencia)
        {
            if (Session["IdUsuario"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (Id_Asistencia == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var asistencia = db.Asistencia.Find(Id_Asistencia); // Obtener la asistencia completa
            if (asistencia == null)
            {
                return HttpNotFound();
            }
            var registroAsistencia = new RegistroAsistencia { 
                Id_asistencia = Id_Asistencia.Value,
                Id_usuario = (int)Session["IdUsuario"]
            };
            ViewBag.CodigoFicha = asistencia.Ficha.Codigo_ficha; // Pasar el código de ficha a la vista
            ViewBag.Nombre_competencia = asistencia.Competencia.Nombre_competencia; // Pasar el nombre de la competencia a la vista
            ViewBag.Nombre_Aprendiz = asistencia.Usuario.Nombre_usuario; 
            ViewBag.Id_asistencia = new SelectList(db.Asistencia, "Id_asistencia", "Fecha_inicio_asistencia");
            ViewBag.Id_usuario = new SelectList(db.Usuario, "Id_usuario", "Tipo_Documento_usuario");
            return View(registroAsistencia);
        }

        // POST: RegistroAsistencias/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id_Registroasistencia,Fecha_registro,Hora_registro,Id_asistencia,Id_usuario,Asistio_registro")] RegistroAsistencia registroAsistencia)
        {
            if (ModelState.IsValid)
            {
                db.RegistroAsistencia.Add(registroAsistencia);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Id_asistencia = new SelectList(db.Asistencia, "Id_asistencia", "Fecha_inicio_asistencia", registroAsistencia.Id_asistencia);
            ViewBag.Id_usuario = new SelectList(db.Usuario, "Id_usuario", "Tipo_Documento_usuario", registroAsistencia.Id_usuario);
            return View(registroAsistencia);
        }

        // GET: RegistroAsistencias/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RegistroAsistencia registroAsistencia = db.RegistroAsistencia.Find(id);
            if (registroAsistencia == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id_asistencia = new SelectList(db.Asistencia, "Id_asistencia", "Fecha_inicio_asistencia", registroAsistencia.Id_asistencia);
            ViewBag.Id_usuario = new SelectList(db.Usuario, "Id_usuario", "Tipo_Documento_usuario", registroAsistencia.Id_usuario);
            return View(registroAsistencia);
        }

        // POST: RegistroAsistencias/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id_Registroasistencia,Fecha_registro,Hora_registro,Id_asistencia,Id_usuario")] RegistroAsistencia registroAsistencia)
        {
            if (ModelState.IsValid)
            {
                db.Entry(registroAsistencia).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Id_asistencia = new SelectList(db.Asistencia, "Id_asistencia", "Fecha_inicio_asistencia", registroAsistencia.Id_asistencia);
            ViewBag.Id_usuario = new SelectList(db.Usuario, "Id_usuario", "Tipo_Documento_usuario", registroAsistencia.Id_usuario);
            return View(registroAsistencia);
        }

        // GET: RegistroAsistencias/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RegistroAsistencia registroAsistencia = db.RegistroAsistencia.Find(id);
            if (registroAsistencia == null)
            {
                return HttpNotFound();
            }
            return View(registroAsistencia);
        }

        // POST: RegistroAsistencias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RegistroAsistencia registroAsistencia = db.RegistroAsistencia.Find(id);
            db.RegistroAsistencia.Remove(registroAsistencia);
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
