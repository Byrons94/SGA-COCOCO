using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gestion_Activos.Models.Class
{
    public class Changed_Serial
    {
        public string old_serial { get; set; }
        public string new_serial { get; set; }
        public int consecutive { get; set; }
        public string category { get; set; }
    }
}