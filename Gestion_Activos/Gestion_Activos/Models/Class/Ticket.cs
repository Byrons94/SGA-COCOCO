using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Gestion_Activos.Models.Class;

namespace Gestion_Activos.Models
{
    public class Ticket
    {

        public HttpPostedFileBase MyFile { get; set; }

        //general information
        [Key]
        //[System.Web.Mvc.HiddenInput(DisplayValue =false)]
        [DisplayName("Id")]
        public string id { get; set; }
        [DisplayName("Tipo")]
        public string type { get; set; }
        [DisplayName("Fecha Creado")]
        public DateTime date { get; set; }

        //administrative information
        [DisplayName("Usuario")]
        public string user { get; set; }
        [DisplayName("Estatus")]
        public string status { get; set; }
        [DisplayName("Técnico")]
        public string technical { get; set; }

        //client information
        [DisplayName("Afiliado")]
        public string membership_number {get; set;}
        [DisplayName("Contacto")]
        public string contact { get; set; }
        [DisplayName("Local")]
        public string local_name { get; set; }
        [DisplayName("Teléfono")]
        public string phone { get; set; }

        //ticket information
        [DisplayName("Asunto")]
        public string problem { get; set; }
        [DisplayName("Visita")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime visit_date { get; set; }
        [DisplayName("Provincia")]
        public string province { get; set; }
        [DisplayName("Cantón")]
        public string canton { get; set; }
        [DisplayName("Distrito")]
        public string district { get; set; }
        [DisplayName("Dirección")]
        public string address { get; set; }


        [DisplayName("Ejecutivo")]
        public string ejecutive { get; set; }
        [DisplayName("Coordinador")]
        public string coordinator { get; set; }
        [DisplayName("Hora programada")]
        public string hour_programmed { get; set; }
        [DisplayName("Detalle")]
        public string details { get; set; }
        [DisplayName("Mueble")]
        public string desktop { get; set; }
        [DisplayName("Internet")]
        public string internet { get; set; }
        [DisplayName("Electricidad")]
        public string electricity { get; set; }


        /*atributos para campos extra*/
        public string ticket_cred{ get; set; }
        public string extra_1 { get; set; }
        public string extra_2 { get; set; }


        //relationships with other objects
        public Client client { get; set; }
        public List<Inventory_Movement> movemetns { get; set; }
        public TicketLog log { get; set; }
        public List<TicketAdvance> advances { get; set; }

        public int Get_Days()
        {
            return (DateTime.Now - date).Days;
        }
    }
}