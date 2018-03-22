using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Gestion_Activos.Models.Class
{
    public class User
    {
        [Key]
        [System.Web.Mvc.HiddenInput(DisplayValue = false)]
        public int id { get; set; }

        [DisplayName("Nombre")]
        public string name { get; set; }

        [DisplayName("Apellido")]
        public string last_name { get; set; }

        [DisplayName("Login")]
        public string login_name { get; set; }

        [DisplayName("Contraseña")]
        public string password { get; set; }

        [DisplayName("Fecha registro")]
        public DateTime creation_date { get; set; }

        [DisplayName("Estatus")]
        public bool active { get; set; }

        [DisplayName("Código Técnico")]
        public string cod_tec { get; set; }



        [System.Web.Mvc.HiddenInput(DisplayValue = false)]
        public int id_rol { get; set; }

        [DisplayName("Rol")]
        public string name_rol { get; set; }

        public Rol rol { get; set; }
    }
}