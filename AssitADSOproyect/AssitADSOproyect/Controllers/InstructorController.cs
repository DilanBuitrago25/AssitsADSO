using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ClaseDatos;

namespace AssitADSOproyect.Controllers
{
    public class InstructorController : Controller
    {
        private BDAssistsADSOEntities db = new BDAssistsADSOEntities();

        // GET: Instructor
        public ActionResult Index()
        {
            int Total_Aprendices;
            string Conexion = "Data Source=DESKTOP-020021\\SQLEXPRESS;Initial Catalog=BDAssistsADSO;Integrated Security=True;trustservercertificate=True;";
            using (SqlConnection connection = new SqlConnection(Conexion))
            {
                string query = "(select count(*) from Usuario where Tipo_usuario = 'Aprendiz')";

                SqlCommand comando = new SqlCommand(query, connection);
                connection.Open();
                Total_Aprendices = (int)comando.ExecuteScalar();
            }
            ViewBag.Total_Aprendices = Total_Aprendices;
            int Total_Competencias;

            using (SqlConnection connection = new SqlConnection(Conexion))
            {
                string query = "(select count(*) from Competencia)";

                SqlCommand comando = new SqlCommand(query, connection);
                connection.Open();
                Total_Competencias = (int)comando.ExecuteScalar();
            }
            ViewBag.Total_Competencias = Total_Competencias;

            int Total_Asistencias;

            using (SqlConnection connection = new SqlConnection(Conexion))
            {
                string query = "(select count(*) from Asistencia)";

                SqlCommand comando = new SqlCommand(query, connection);
                connection.Open();
                Total_Asistencias = (int)comando.ExecuteScalar();
            }
            ViewBag.Total_Asistencias = Total_Asistencias;

            int Total_Fichas;

            using (SqlConnection connection = new SqlConnection(Conexion))
            {
                string query = "(select count(*) from Ficha)";

                SqlCommand comando = new SqlCommand(query, connection);
                connection.Open();
                Total_Fichas = (int)comando.ExecuteScalar();
            }
            ViewBag.Total_Fichas = Total_Fichas;

            var usuario = db.Usuario.Include(u => u.Ficha);
            return View(usuario.ToList());
        }

        // GET: Instructor/Details/5
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

        // GET: Instructor/Create
        public ActionResult Create()
        {
            ViewBag.Id_ficha = new SelectList(db.Ficha, "Id_ficha", "Jornada_ficha");
            return View();
        }

        // POST: Instructor/Create
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

        // GET: Instructor/Edit/5
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

        // POST: Instructor/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id_usuario,Documento_usuario,Tipo_usuario,Tipo_Documento_usuario,Nombre_usuario,Apellido_usuario,Telefono_usuario,Correo_usuario,Tipo_instructor,Id_ficha,Esinstructormaster_usuario,Contrasena_usuario")] Usuario usuario)
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

        // GET: Instructor/Delete/5
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

        // POST: Instructor/Delete/5
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
