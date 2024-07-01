//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class Ficha
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Ficha()
        {
            this.Asistencia = new HashSet<Asistencia>();
            this.Usuario1 = new HashSet<Usuario>();
            this.Usuario2 = new HashSet<Usuario>();
        }
    
        public int Id_ficha { get; set; }
        public int Codigo_ficha { get; set; }
        public string Jornada_ficha { get; set; }
        public string Modalidad_ficha { get; set; }
        public string tipo_ficha { get; set; }
        public string Fecha_inicio { get; set; }
        public string Fecha_fin { get; set; }
        public Nullable<bool> Estado_ficha { get; set; }
        public Nullable<int> Id_programa { get; set; }
        public Nullable<int> Id_Instructor { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Asistencia> Asistencia { get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual Programa_formacion Programa_formacion { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Usuario> Usuario1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Usuario> Usuario2 { get; set; }
    }
}
