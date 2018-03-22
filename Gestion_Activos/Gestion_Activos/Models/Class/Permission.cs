using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Gestion_Activos.Models.Class
{
    public class Permission
    {
        [System.Web.Mvc.HiddenInput(DisplayValue = false)]
        public int ID { get; set; }

        [System.Web.Mvc.HiddenInput(DisplayValue = false)]
        public int Category { get; set; }

        [DisplayName("Nombre")]
        public string Description { get; set; }

        [System.Web.Mvc.HiddenInput(DisplayValue = false)]
        public bool Active { get; set; }
    }
}