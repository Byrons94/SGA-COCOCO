using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Gestion_Activos.Models.Class
{
    public class Changed_Client
    {
        [DisplayName("Antiguo Afiliado")]
        public string old_membership { get; set; }
        [DisplayName("Antiguo Nombre Local")]
        public string old_local_name { get; set; }
        [DisplayName("Nuevo Afiliado")]
        public string new_membership { get; set; }
        [DisplayName("Nuevo Nombre Local")]
        public string new_local_name { get; set; }
        [DisplayName("Direccion")]
        public string address { get; set; }
        [DisplayName("Referencia")]
        public string reference { get; set; }
        [DisplayName("Fecha")]
        public DateTime date { get; set; }
    }
}