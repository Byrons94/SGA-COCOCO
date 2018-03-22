using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Gestion_Activos.Models.Class
{
    public class Product_Movement
    {
        [DisplayName("Fecha")]
        public DateTime date_movement       { get; set; }
        [DisplayName("Negocio")]
        public string   local_name          { get; set; }
        [DisplayName("Afiliado")]
        public string   membership_number   { get; set; }
        [DisplayName("Movimiento")]
        public string   type_movement       { get; set; }

        [DisplayName("Documento")]
        public string num_doc { get; set; }

        public Product  product             { get; set; }
    }
}