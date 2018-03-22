using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gestion_Activos.Models.Class
{
    public class Dashboard
    {
        public int install { get; set; }
        public int visite  { get; set; }
        public int retire  { get; set; }

        public List<Ticket> open { get; set; }
        public List<Ticket> closest { get; set; }

    }
}