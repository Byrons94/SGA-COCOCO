using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gestion_Activos.Models.Commons
{
    public static class Categories_List
    {
        public static Dictionary<int, string> permisions_categories = new Dictionary<int, string>() {
            {1, "Mantenimientos"},
            {2, "Tickets" },
            {3, "Afiliados" },
            {4, "Inventario y series" },
            {5, "Reportes" }
        };
        
    }
}