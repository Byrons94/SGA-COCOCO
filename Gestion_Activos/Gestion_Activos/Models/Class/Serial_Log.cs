using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Gestion_Activos.Models.Class
{
    public class Serial_Log
    {

        [DisplayName("Serie")]
        public string serial        { get; set; }
        [DisplayName("Movimiento")]
        public string   movement    { get; set; }
        [DisplayName("Fecha")]
        public DateTime date        { get; set; }
        [DisplayName("Usuario")]
        public string   user        { get; set; }
        [DisplayName("Detalle")]
        public string   details     { get; set; }
        [DisplayName("Tickete")]
        public string   ticket      { get; set; }
        [DisplayName("Notas")]
        public string   note        { get; set; } 
    }
}