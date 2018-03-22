using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Gestion_Activos.Models.Class
{
    public class Product_Category
    {
        [DisplayName("Identificador")]
        public int id { get; set; }

        [DisplayName("Descripción")]
        public string description { get; set; }
    }
}