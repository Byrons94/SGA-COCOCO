using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Gestion_Activos.Models.Class
{
    //this class represent the log of any ticket
    public class TicketLog
    {
        public string id { get; set; }
        [DisplayName("Dia/Hora Evento")]
        public DateTime date { get; set; }
        public char type { get; set; }
        public string type_string { get; set; }


        #region textbox
        public string damage { get; set; }
        public string diagnostic { get; set; }
        public string solution { get; set; }
        public string others { get; set; }
        public string extra_1 { get; set; }
        public string extra_2 { get; set; }
        public string extra_3 { get; set; }
        #endregion


        #region radiobts
        public string equipment_exposition { get; set; }
        public string equipment_exposition_2 { get; set; }
        public string mainmaintenance { get; set; }
        public string tests { get; set; }
        public string connetion { get; set; }
        public string instalation_detaill { get; set; }
        public string visit_equipment_exposition { get; set; }
        #endregion
    }
}