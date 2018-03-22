using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Gestion_Activos.Models.Class
{
    public class Client
    {
        public int id { get; set; }

        [DisplayName("# Afiliado")]
        public string membership_number { get; set; }

        [DisplayName("Nombre")]
        public string name { get; set; }

        [DisplayName("Contacto")]
        public string contact { get; set; }

        [DisplayName("Comercio")]
        public string local_name { get; set; }

        [DisplayName("Teléfono")]
        public string phone { get; set; }

        [DisplayName("Provincia")]
        public string province { get; set; }

        [DisplayName("Cantón")]
        public string canton { get; set; }

        [DisplayName("Distrito")]
        public string district { get; set; }

        [DisplayName("Dirección")]
        public string address { get; set; }

        [DisplayName("Actualizado")]
        public bool updated { get; set; }

        [DisplayName("Estatus")]
        public bool status { get; set; }

        [DisplayName("Estatus")]
        public string status_varchar { get; set; }

        [DisplayName("Antiguo # Afiliado")]
        public string last_membership { get; set; }

        [DisplayName("Cambiado el:")]
        public DateTime changed { get; set; }

        [DisplayName("Cambio al # afiliado")]
        public string new_menbership { get; set; }

        public List<Product> inventary { get; set; }

        public List<Ticket> tickets { get; set; }

        [DisplayName("Email")]
        public string email { get; set; }

        [DisplayName("Coordinador")]
        public string area { get; set; }

        [DisplayName("Ejecutivo")]
        public string ejecutive { get; set; }

        [DisplayName("Software")]
        public string software { get; set; }

        [DisplayName("Tipo industria")]
        public string industry { get; set; }

        [DisplayName("Licencia")]
        public string license { get; set; }

        [DisplayName("Fecha instalacion")]
        public DateTime installation { get; set; }

        [DisplayName("Fecha compra")]
        public DateTime buy_date { get; set; }

        [DisplayName("Pago mensual")]
        public string pago { get; set; }

        [DisplayName("Motivo retiro")]
        public string retire_comentary { get; set; }

        [DisplayName("Fecha retiro")]
        public DateTime retired_date { get; set; }

        [DisplayName("Ultimo mantenimiento")]
        public string last_mainteinance { get; set; }

        public Dictionary<string, List<string>> products_intalled { get; set; }
        public List<string> categories_products { get; set; }

        //comodines
        public string other { get; set; }
        public string other_2 { get; set; }
        public string other_3 { get; set; }

    }

  }