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

    public partial class Soporte
    {
        public int Id_soporte { get; set; }
        [Required(ErrorMessage = "Por favor ingresar el Asunto de la justificación")]
        public string Nombre_soporte { get; set; }
        [Required(ErrorMessage = "Por favor ingresar el Mensaje de la justificación")]
        public string Descripcion_soporte { get; set; }
        [Required(ErrorMessage = "Por favor insertar los Archivos de la justificacion Tipo PDF solamente")]
        public string Tipo_soporte { get; set; }
        public Nullable<int> Id_asistencia { get; set; }
        public Nullable<int> Id_usuario { get; set; }
        public string Fecha_registro { get; set; }
        public string Hora_registro { get; set; }
        public Nullable<int> Id_Instructor { get; set; }
    
        public virtual Asistencia Asistencia { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}
