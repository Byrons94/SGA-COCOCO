using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Gestion_Activos.Models.Class
{
    public class Serial
    {
        [DisplayName("ID")]
        public string id     { get; set; }
        [DisplayName("Serie")]
        public string serial { get; set; }
        [DisplayName("Último movimiento")]
        public string state  { get; set; }
        [DisplayName("Fecha movimiento")]
        public DateTime date { get; set; }
        [DisplayName("Responsable")]
        public string other  { get; set; }
        [DisplayName("Producto")]
        public string product_name { get; set; }
        [DisplayName("Estado físico")]
        public string physical_state { get; set; }
        [DisplayName("Referencia")]
        public string reference { get; set; }
        [DisplayName("Categoria")]
        public string category { get; set; }

        public List<Serial_Log> serial_log {  get;    set;}
    }
}