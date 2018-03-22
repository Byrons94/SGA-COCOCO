using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Gestion_Activos.Models.Class
{
    public class Product_Inventory
    {
        [DisplayName("Cod_Prod")]
        public string cod_prod { get; set; }
        [DisplayName("Descripción")]
        public string description { get; set; }
        [DisplayName("Inv. Inicial")]
        public int balance_start { get; set; }
        [DisplayName("Inv. Actual")]
        public int balance_end { get; set; }
        [DisplayName("Comprado+")]
        public int bought { get; set; }
        [DisplayName("Instalado-")]
        public int installed { get; set; }
        [DisplayName("Retirado+")]
        public int retired { get; set; }
        [DisplayName("Reacond")]
        public int reconditioned { get; set; }
        [DisplayName("Desechado-")]
        public int discarded { get; set; }

        public int get_total()
        {
            int total = 0;
            total = balance_end;
            return total;
        }
    }
}