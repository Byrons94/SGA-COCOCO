//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Gestion_Activos.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class SGA_CAMBIO_AFILIADO
    {
        public int ID { get; set; }
        public string ANTIGUO_AFILIADO { get; set; }
        public string NUEVO_AFILIADO { get; set; }
        public string DETALLE { get; set; }
        public System.DateTime FECHA { get; set; }
        public string AFILIADO_INICIAL { get; set; }
    }
}