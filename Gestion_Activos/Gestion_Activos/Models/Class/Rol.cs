using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Gestion_Activos.Models.Class
{
    public class Rol
    {
        public int id { get; set; }
        [DisplayName("Nombre")]
        public string name { get; set; }
        [DisplayName("Permanente")]
        public bool permanet { get; set; }
        public List<Permission> list_permissions;
    }
}