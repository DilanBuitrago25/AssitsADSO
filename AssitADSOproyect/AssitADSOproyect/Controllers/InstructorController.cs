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
using static AssitADSOproyect.Controllers.LoginController;

namespace AssitADSOproyect.Controllers
{
    public class InstructorController : Controller
    {
        private BDAssistsADSOv4Entities db = new BDAssistsADSOv4Entities();

        // GET: Instructor
        [AutorizarTipoUsuario("Instructor")]
        public ActionResult Index()
        {
            int Total_Aprendices;
            string Conexion = "Data Source=LAPTOP-NC5UJ7OA;Initial Catalog=BDAssistsADSO;Integrated Security=True;trustservercertificate=True;";
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
