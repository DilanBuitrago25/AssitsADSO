﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ClaseDatos
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class BDAssistsADSOEntities : DbContext
    {
        public BDAssistsADSOEntities()
            : base("name=BDAssistsADSOEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Asistencia> Asistencia { get; set; }
        public virtual DbSet<Competencia> Competencia { get; set; }
        public virtual DbSet<Ficha> Ficha { get; set; }
        public virtual DbSet<Programa_formacion> Programa_formacion { get; set; }
        public virtual DbSet<Reporte> Reporte { get; set; }
        public virtual DbSet<Soporte> Soporte { get; set; }
        public virtual DbSet<Usuario> Usuario { get; set; }
    
        public virtual ObjectResult<string> ValidarUsuarios(string correo, string contraseña)
        {
            var correoParameter = correo != null ?
                new ObjectParameter("Correo", correo) :
                new ObjectParameter("Correo", typeof(string));
    
            var contraseñaParameter = contraseña != null ?
                new ObjectParameter("Contraseña", contraseña) :
                new ObjectParameter("Contraseña", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<string>("ValidarUsuarios", correoParameter, contraseñaParameter);
        }
    
        public virtual ObjectResult<string> ValidarUsuarios1(string correo_usuario, string contrasena_usuario)
        {
            var correo_usuarioParameter = correo_usuario != null ?
                new ObjectParameter("Correo_usuario", correo_usuario) :
                new ObjectParameter("Correo_usuario", typeof(string));
    
            var contrasena_usuarioParameter = contrasena_usuario != null ?
                new ObjectParameter("Contrasena_usuario", contrasena_usuario) :
                new ObjectParameter("Contrasena_usuario", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<string>("ValidarUsuarios1", correo_usuarioParameter, contrasena_usuarioParameter);
        }
    }
}
