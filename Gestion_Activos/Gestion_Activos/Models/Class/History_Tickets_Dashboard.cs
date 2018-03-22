using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gestion_Activos.Models.Class
{
    public class History_Tickets_Dashboard
    {
        public string name_day { get; set; }
        public int    installed  { get; set; }
        public int    visited  { get; set; }
        public int    retired  { get; set; }
        public int    total    { get; set; }
    }
}