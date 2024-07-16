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
    using System.ComponentModel.DataAnnotations;

    public partial class Programa_formacion
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Programa_formacion()
        {
            this.Ficha = new HashSet<Ficha>();
            this.Competencia = new HashSet<Competencia>();
        }

        public int Id_programa { get; set; }
        [Required(ErrorMessage = "Por favor de ingresar el nombre del programa")]
        public string Nombre_programa { get; set; }
        [Required(ErrorMessage = "Por favor de ingresar el tipo de programa")]
        public string Tipo_programa { get; set; }
        [Required(ErrorMessage = "Por favor de ingresar la duración del programa")]
        public string Duracion_programa { get; set; }
        public Nullable<bool> Estado_Programa_formacion { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Ficha> Ficha { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Competencia> Competencia { get; set; }
    }
}
