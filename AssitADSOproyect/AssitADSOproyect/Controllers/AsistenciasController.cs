using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ClaseDatos;
using QRCoder;

namespace AssitADSOproyect.Controllers
{
    public class AsistenciasController : Controller
    {
        private BDAssistsADSOEntities db = new BDAssistsADSOEntities();

        // GET: Asistencias
        public ActionResult Index()
        {
            var asistencia = db.Asistencia.Include(a => a.Usuario);
            return View(asistencia.ToList());
        }

        // GET: Asistencias/Details/5
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

        // GET: Asistencias/Create
        public ActionResult Create()
        {
            ViewBag.Id_usuario = new SelectList(db.Usuario, "Id_usuario", "Documento_usuario");
            ViewBag.Codigo_ficha = new SelectList(db.Ficha, "Id_ficha", "Codigo_ficha");
            ViewBag.Nombre_competencia = new SelectList(db.Competencia, "Id_competencia", "Nombre_competencia");
            return View();
        }


        // POST: Asistencias/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id_asistencia,Fecha_inicio_asistencia,Hora_inicio_asistencia,Fecha_fin_asistencia,Hora_fin_asistencia,Detalles_asistencia,Id_usuario,Id_ficha,Id_competencia")] Asistencia asistencia)
        {
            if (ModelState.IsValid)
            {
                db.Asistencia.Add(asistencia);
                db.SaveChanges();
                var editUrl = Url.Action("Edit", "Asistencias", new { id = asistencia.Id_asistencia }, Request.Url.Scheme); // Asegurar esquema (http/https)
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(editUrl, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);

                var qrCodePath = Path.Combine(Server.MapPath("~/QRCodes"), $"{asistencia.Id_asistencia}.png");
                using (var bitmap = qrCode.GetGraphic(20)) // Ajusta la escala según necesites
                {
                    bitmap.Save(qrCodePath, ImageFormat.Png);
                }
                return RedirectToAction("Index");
            }

            ViewBag.Id_usuario = new SelectList(db.Usuario, "Id_usuario", "Documento_usuario", asistencia.Id_usuario);
            ViewBag.Nombre_competencia = new SelectList(db.Competencia, "Id_competencia", "Nombre_competencia", asistencia.Id_competencia);
            ViewBag.Id_ficha = new SelectList(db.Ficha, "Id_ficha", "Codigo_ficha", asistencia.Id_ficha);

            

            //return View("Confirmacion", asistencia);

            return View(asistencia);
        }

        // GET: Asistencias/Edit/5
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
            ViewBag.Id_usuario = new SelectList(db.Usuario, "Id_usuario", "Documento_usuario", asistencia.Id_usuario);
            ViewBag.Id_competencia = new SelectList(db.Competencia, "Id_competencia", "Nombre_competencia", asistencia.Id_competencia);
            ViewBag.Id_ficha = new SelectList(db.Ficha, "Id_ficha", "Codigo_ficha", asistencia.Id_ficha);
            return View(asistencia);
        }

        // POST: Asistencias/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id_asistencia,Id_ficha,Id_competencia,Fecha_inicio_asistencia,Hora_inicio_asistencia,Fecha_fin_asistencia,Hora_fin_asistencia,Detalles_asistencia,Id_usuario")] Asistencia asistencia)
        {
            if (ModelState.IsValid)
            {
                db.Entry(asistencia).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Id_usuario = new SelectList(db.Usuario, "Id_usuario", "Documento_usuario", asistencia.Id_usuario);
            ViewBag.Id_competencia = new SelectList(db.Competencia, "Id_competencia", "Nombre_competencia", asistencia.Id_competencia);
            ViewBag.Id_ficha = new SelectList(db.Ficha, "Id_ficha", "Codigo_ficha", asistencia.Id_ficha);
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
