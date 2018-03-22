using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Gestion_Activos.Models.Class
{
    public class Ticket_Evidence
    {
        public int id { get; set; }

        [DisplayName("Nombre")]
        public string name { get; set; }

        [DisplayName("Extension")]
        public string extension { get; set; }

        [DisplayName("Fecha subido")]
        public DateTime dateUploaded { get; set; }

        public string path { private get; set; }

        public string getPath() {
            return path;
        }
    }
}