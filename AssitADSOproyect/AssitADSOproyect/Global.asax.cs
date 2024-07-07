using ClaseDatos;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Timers;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;
using WebGrease.Configuration;
using System.Data.SqlClient;
using AngleSharp.Common;

namespace AssitADSOproyect
{
    public class MvcApplication : System.Web.HttpApplication
    {

        private Timer timer;

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            HttpException httpEx = ex as HttpException;

            if (httpEx != null && httpEx.GetHttpCode() == 401)
            {
                // Lógica personalizada antes de redirigir (opcional)
                Server.ClearError(); // Limpia el error para evitar la página de error genérica
                Response.Redirect("~/Views/Home/Error401.cshtml");
            }
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            timer = new Timer(60000); // 60000 milisegundos = 1 minuto
            timer.Elapsed += VerificarAsistencias;
            timer.Start();
        }


        private void VerificarAsistencias(object sender, ElapsedEventArgs e)
        {
            using ( var db = new BDAssistsADSOv4Entities())
            {
                var fechaHoraActual = DateTime.Now;

                var todasLasAsistencias = db.Asistencia.ToList();

                // Filtra las asistencias pasadas en memoria
                var asistenciasPasadas = todasLasAsistencias
                    .Where(a =>
                    {
                        DateTime fechaFinAsistencia;
                        if (DateTime.TryParseExact(a.Fecha_fin_asistencia + " " + a.Hora_fin_asistencia, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaFinAsistencia))
                        {
                            return fechaFinAsistencia < fechaHoraActual;
                        }
                        return false; // Si el formato de fecha/hora es incorrecto, no se considera como pasada
                    })
                    .ToList();

                foreach (var asistencia in asistenciasPasadas)
                {
                    // Obtén los IDs de aprendices asociados a la ficha
                    var aprendicesEnFicha = db.Ficha_has_Usuario
                     .Where(fu => fu.Id_ficha == asistencia.Id_ficha && fu.TipoUsuario == "Aprendiz")
                     .Select(fu => (int?)fu.Id_usuario) // <-- Convertir a int?
                     .ToList();

                    var aprendicesConRegistro = db.RegistroAsistencia
                        .Where(ra => ra.Id_asistencia == asistencia.Id_asistencia)
                        .Select(ra => ra.Id_Aprendiz)
                        .ToList();

                    var aprendicesSinRegistro = aprendicesEnFicha.Except(aprendicesConRegistro).ToList();

                    foreach (var aprendizId in aprendicesSinRegistro)
                    {
                        var nuevoRegistro = new RegistroAsistencia
                        {
                            Fecha_registro = null,
                            Hora_registro = null,
                            Id_asistencia = asistencia.Id_asistencia,
                            Id_Aprendiz = aprendizId,
                            Asistio_registro = false
                        };

                        db.RegistroAsistencia.Add(nuevoRegistro);
                    }
                }

                db.SaveChanges();
            }
        }








    }
}
