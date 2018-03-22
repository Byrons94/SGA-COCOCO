using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gestion_Activos.Models.Class
{
    public class Inventory_Movement
    {
        public int id { get; set; }
        public string type { get; set; }
        public string num_move { get; set; }
        public List<Product> products_list { get; set; }
    }
}