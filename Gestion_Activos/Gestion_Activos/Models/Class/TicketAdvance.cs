using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Gestion_Activos.Models.Class
{
    public class TicketAdvance
    {
        [DisplayName("Id")]
        public string   id { set; get; }

        [DisplayName("Responsable")]
        public string   user { set; get; }

        [DisplayName("Detalle")]
        public string   detail { set; get; }

        [DisplayName("Hora")]
        public DateTime date { set; get; }

    }
}