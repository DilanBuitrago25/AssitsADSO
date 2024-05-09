using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ClaseDatos;

namespace AssitADSOproyect.Controllers
{
    public class LoginController : Controller
    {
        private BDAssistsADSOEntities db = new BDAssistsADSOEntities();
        string Conexion = "Data Source=DESKTOP-057421\\SQLEXPRESS;Initial Catalog=BDTIENDAMVC;Integrated Security=True; multipleactiveresultsets=True;";
        // GET: Login
        public ActionResult Index()
        {
            var usuario = db.Usuario.Include(u => u.Ficha);
            return View(usuario.ToList());
        }

        // GET: Login/Details/5
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

        // GET: Login/Create
        public ActionResult Create()
        {
            ViewBag.Id_ficha = new SelectList(db.Ficha, "Id_ficha", "Jornada_ficha");
            return View();
        }

        // POST: Login/Create
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

        // GET: Login/Edit/5
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

        // POST: Login/Edit/5
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

        // GET: Login/Delete/5
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

        // POST: Login/Delete/5
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

        [HttpPost]
        public ActionResult Index(Usuario oUsuario)
        {
            oUsuario.Contrasena_usuario = oUsuario.Contrasena_usuario;
            using (SqlConnection cn = new SqlConnection(Conexion))
            {
                SqlCommand login = new SqlCommand("ValidarUsuarios", cn);
                login.Parameters.AddWithValue("Correo", oUsuario.Correo_usuario);
                login.Parameters.AddWithValue("Contraseña", oUsuario.Contrasena_usuario);
                login.CommandType = CommandType.StoredProcedure;

                cn.Open();
                oUsuario.Tipo_usuario = Convert.ToString(login.ExecuteScalar().ToString());

                if (oUsuario.Tipo_usuario == "Instructor")
                {
                    FormsAuthentication.SetAuthCookie(oUsuario.Correo_usuario, false);
                    TempData["IdUsuarios"] = oUsuario.Documento_usuario;
                    Session["Instructor"] = oUsuario;
                    return RedirectToAction("Index", "Instructor");
                }
                else if (oUsuario.Tipo_usuario == "Aprendiz")
                {
                    FormsAuthentication.SetAuthCookie(oUsuario.Correo_usuario, false);
                    TempData["IdUsuarios"] = oUsuario.Documento_usuario;
                    Session["Aprendiz"] = oUsuario.Tipo_usuario = "Aprendiz";
                    return RedirectToAction("Index", "Aprendiz");
                }
                else
                {

                    ViewData["Mensaje"] = "Usuario no encontrado";

                    return View();


                }
            }

        }

    }
}
