using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gestion_Activos.Models.Class
{
    public class Product
    {
        public int id { get; set; }
        public string cod_prod { get; set; }
        public string description { get; set; }
        public string serial_number { get; set; }
        public string state { get; set; }
        public string action { get; set; }
        public string category { get; set; }
        public string serial_2 { get; set; }
    }
}