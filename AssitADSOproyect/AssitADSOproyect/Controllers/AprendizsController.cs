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
using System.Web.Services;

namespace AssitADSOproyect.Controllers
{
    public class AprendizsController : Controller
    {
        private BDAssistsADSOv4Entities db = new BDAssistsADSOv4Entities();


        //[WebService(Namespace = "http://tempuri.org/")]
        //[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
        //public class ServicioQR : System.Web.Services.WebService
        //{
        //    [WebMethod]
        //    public string EscanearQR(string qrData)
        //    {
        //        // Aquí procesas la información del código QR (qrData)
        //        // ... (lógica de tu aplicación)
        //        return "QR escaneado: " + qrData; // Ejemplo de respuesta
        //    }
        //}

        // GET: Aprendizs
        [AutorizarTipoUsuario("Aprendiz")]
        public ActionResult Index()
        {
            var usuario = db.Usuario.Include(u => u.Ficha);
            return View(usuario.ToList());
        }

        // GET: Aprendizs/Details/5
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
