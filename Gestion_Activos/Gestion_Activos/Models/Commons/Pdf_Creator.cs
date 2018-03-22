using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using Gestion_Activos.Models.Class;

namespace Gestion_Activos.Models.Commons
{
    public class Pdf_Creator
    {
        private string T_HEADER = "HEADER.html";
        private string T_FOOTER = "FOOTER.html";

        public void Create_Pdf_From_Ticket(Ticket ticket = null)
        {
            FileStream fs       = new FileStream(@"C:\Windows\Temp\Boleta de servicio tecnico.pdf", FileMode.Create, FileAccess.Write, FileShare.None);
            Document   doc      = new Document(PageSize.LETTER, 50, 50, 30, 30);
            PdfWriter  writer   = PdfWriter.GetInstance(doc, fs);

            Set_Parameters_Doc(doc);
            doc.Open();

            string contents = "";
            //HEADER
            Add_Header_Logo(doc);
            contents = Get_Template_Section(T_HEADER);
            if (ticket != null)
            {
                contents = Set_Header_Info(contents, ticket);
            }
            Add_Section_To_Doc(doc, contents);

            //BODY
            if (ticket != null)
            {
                string template = Select_Template(ticket.type);
                contents        = Get_Template_Section(template);
                contents        = Set_Body_Info(contents, ticket);
                Add_Section_To_Doc(doc, contents);
            }

            //FOOTER
            contents = Get_Template_Section(T_FOOTER);
            if (ticket != null)
            {
                contents = Set_Footer_Info(contents, ticket);
            }
            Add_Section_To_Doc(doc, contents);
            Add_Footer_Logo(doc);

            //AÑADE LOS MOVIMIENTOS de series REALIZADOS
            Add_Inventary_Movement(doc, ticket);

            doc.Close();
        }

        //second pagee
        private void Add_Inventary_Movement(Document doc = null, Ticket ticket = null)
        {
            string contents = "";
            if (ticket.client.inventary != null)
            {
                if (ticket.client.inventary.Count() > 0)
                { 
                    doc.NewPage();
                    //header logo
                    Add_Header_Logo(doc);
                
                    //BODY TABLE
                    contents = Get_Template_Section("INVENTARY.html");
                    contents = Table_Ticket(ticket, contents);
                    Add_Section_To_Doc(doc, contents);                

                    //logo
                    Add_Footer_Logo(doc);
                }
            }
        }

        //CREATE TABLE BODY TO ADD
        private string Table_Ticket(Ticket ticket = null, string contents = "")
        {
            if (ticket != null && ticket.client.inventary != null)
            {
                if (ticket.type == "INSTALACION" || ticket.type == "RETIRO")
                {
                    var itemsTable = @"<table style=""font-size:10px;""><tr><th style=""font-weight: bold;"">Descripción</th><th style=""font-weight: bold"">Serie</th><th style=""font-weight: bold"">Estado</th></th><th style=""font-weight: bold"">Acción</th></tr>";
                    foreach (var prodcut in ticket.client.inventary)
                    {
                        itemsTable += string.Format("<tr><td>" + prodcut.description + "</td><td>" + prodcut.serial_number + "</td><td>" + prodcut.state + "</td><td>" + prodcut.action + "</td></tr>");
                    }
                    itemsTable += "</table>";
                    contents = contents.Replace("[ITEMS]", itemsTable);
                }
                else if (ticket.type == "VISITA")
                {
                    var itemsTable = @"<table style=""font-size:8px;""><tr><th style=""font-weight: bold;"">Descripción</th><th style=""font-weight: bold"">Serie retirada</th><th style=""font-weight: bold"">Estado</th><th style=""font-weight: bold"">Serie instalada</th><th style=""font-weight: bold"">Estado</th></th><th style=""font-weight: bold"">Acción</th></tr>";
                    foreach (var prodcut in ticket.client.inventary)
                    {
                        itemsTable += string.Format("<tr><td>" + prodcut.description + "</td><td>" + prodcut.serial_2 + "</td><td>" + prodcut.state + "</td><td>" + prodcut.serial_number + "</td><td>Optimo</td><td>Cambio</td></tr>");
                    }
                    itemsTable += "</table>";
                    contents = contents.Replace("[ITEMS]", itemsTable);
                } 
            }
            return contents;
        }

        //select body template by kind of ticket
        private string Select_Template(string tip_documento ="")
        {
            string template_name = "";
            switch (tip_documento)
            {
                case "INSTALACION":
                    template_name = "T_INSTALACION.html";
                break;
                case "VISITA":
                    template_name = "T_VISITA.html";
                break;
                case "RETIRO":
                    template_name = "T_RETIRO.html";
                break;
                default:
                    template_name = "INSTALACION.html";
                 break;
            }
            return template_name;
        }

        //info document
        private void Set_Parameters_Doc(Document doc)
        {
            doc.AddTitle("Boleta de servicio COCOCO/CREDOMATIC");
            doc.AddCreator("S.G.A Credomatic");
            doc.AddAuthor("Byron Serrano");
        }

        //image header
        private void Add_Header_Logo(Document doc)
        {
            string fontpath = System.Web.HttpContext.Current.Server.MapPath("~/Content/dist/img/");
            iTextSharp.text.Image imagen = Image.GetInstance(fontpath + "logo_contraste.jpg");
            imagen.ScalePercent(30f);
            imagen.SetAbsolutePosition(((doc.PageSize.Width / 2) - (180)), (doc.PageSize.Height - 50));
            doc.Add(imagen);
        }

        //get template to add
        private string Get_Template_Section(string name)
        {
            string fontView = System.Web.HttpContext.Current.Server.MapPath("~/Views/Ticket/Template/");
            string contents = File.ReadAllText(fontView + name);
            return contents;
        }

        //set values of header
        private string Set_Header_Info(string contents="", Ticket ticket = null)
        {
            if (ticket != null)
            {
                contents = contents.Replace("[TIPO_BOLETA]",    ticket.type);
                contents = contents.Replace("[NUM_BOLETA]",     ticket.id);
                contents = contents.Replace("[N_AFILIADO]",     ticket.membership_number);
                contents = contents.Replace("[PROVINCIA]",      ticket.client.province);
                contents = contents.Replace("[TECNICO]",        ticket.technical);
                contents = contents.Replace("[COMERCIO]",       ticket.client.local_name);
                contents = contents.Replace("[CANTON]",         ticket.client.canton);
                contents = contents.Replace("[TIPO]",           ticket.type);
                contents = contents.Replace("[CONTACTO]",       ticket.client.contact);
                contents = contents.Replace("[DISTRITO]",       ticket.client.district);
                contents = contents.Replace("[DIA]",            ticket.log.date.ToString());
                contents = contents.Replace("[TELEFONO]",       ticket.client.phone);
                contents = contents.Replace("[DIRECCION]",      ticket.client.address);
            }
            return contents;
        }

        private string Set_Body_Info(string contents = "", Ticket ticket = null)
        {
            if (ticket != null)
            {
                if (ticket.type == "INSTALACION")
                {
                    contents = Set_Instalacion_Info(contents, ticket);
                }
                else if (ticket.type == "VISITA")
                {
                    contents = Set_Visita_Info(contents, ticket);
                }
                else if (ticket.type == "RETIRO")
                {
                    contents = Set_Retiro_Info(contents, ticket);
                }
                else
                {
                    contents = Set_Empty_Info(contents);
                }
            }
            return contents;
        }

        private string Set_Instalacion_Info(string contents = "", Ticket ticket = null)
        {
            if (ticket != null && ticket.log != null)
            {
                contents = contents.Replace("[CONEXION]",       Utilities_Ticket_Log.Conection(ticket.log.connetion.Split(',')));
                contents = contents.Replace("[D_INTALACION]",   Utilities_Ticket_Log.Detalle_Instalacion(ticket.log.instalation_detaill.Split(',')));
                contents = contents.Replace("[EXPOSICION]",     Utilities_Ticket_Log.Exposición_Equipo(ticket.log.equipment_exposition.Split(',')));
                contents = contents.Replace("[PRUEBAS]",        Utilities_Ticket_Log.Pruebas(ticket.log.others.Split(',')));
                contents = contents.Replace("[OBSERVACIONES]",  ticket.log.extra_1);
            }        
            return contents.Trim();
        }

        private string Set_Visita_Info(string contents = "", Ticket ticket = null)
        {
            if (ticket != null && ticket.log != null)
            {
                contents  = contents.Replace("[AVERIA]",         ticket.log.damage.Trim());
                contents  = contents.Replace("[DIAGNOSTICO]",    ticket.log.diagnostic.Trim());
                contents  = contents.Replace("[SOLUCION]",       ticket.log.solution.Trim());
                contents  = contents.Replace("[EXPOSICION]",     ""); //pendiente
                contents  = contents.Replace("[MANTENIMIENTO]",  Utilities_Ticket_Log.Mantenimiento(ticket.log.mainmaintenance.Split(',')));
                contents  = contents.Replace("[PRUEBAS]",        Utilities_Ticket_Log.Pruebas_Visita(ticket.log.tests.Split(',')));
                string mg = (ticket.log.extra_2 == "1") ? "•Se ha realizado un mantenimiento general del equipo. ✓" : "";
                contents  = contents.Replace("[MANTENIMIENTO_GENERAL]", mg);
                string ef = (ticket.log.extra_3 != "") ? "•"+ticket.log.extra_3 : "";
                contents = contents.Replace("[EQUIPO_FALTANTE]", ef);
            }
            return contents.Trim();
        }

        private string Set_Retiro_Info(string contents = "", Ticket ticket = null)
        {
            if (ticket != null && ticket.log != null)
            {
                contents = contents.Replace("[EXPOSICION]",    Utilities_Ticket_Log.Exposición_Equipo(ticket.log.equipment_exposition.Split(',')));
                contents = contents.Replace("[OBSERVACIONES]", ticket.log.extra_1);
                string ef = (ticket.log.extra_3 != "") ? "•" + ticket.log.extra_3 : "";
                contents = contents.Replace("[EQUIPO_FALTANTE]", ef);
            }
            return contents.Trim();
        }

        private string Set_Empty_Info(string contents = "")
        { 
            contents = contents.Replace("[CONEXION]", "");
            contents = contents.Replace("[D_INTALACION]", "");
            contents = contents.Replace("[EXPOSICION]", "");
            contents = contents.Replace("[PRUEBAS]", "");
            contents = contents.Replace("[OBSERVACIONES]", "");
            contents = contents.Replace("[AVERIA]", "");
            contents = contents.Replace("[DIAGNOSTICO]", "");
            contents = contents.Replace("[SOLUCION]", "");
            contents = contents.Replace("[MANTENIMIENTO]", "");
            contents = contents.Replace("[AVERIA]", "");
            return contents;
        }

        //set info footer
        private string Set_Footer_Info(string contents = "", Ticket ticket = null)
        {
            if (ticket != null)
            {
                contents = contents.Replace("[AFILIADO]",    ticket.client.membership_number);
                contents = contents.Replace("[TECNICO]",     ticket.technical);
                contents = contents.Replace("[CONTACTO]",    ticket.client.contact);
                contents = contents.Replace("[RESPONSABLE]", ticket.user);
            }
            return contents;
        }

        private string Get_Type_Tiquet(char name = ' ')
        {
            string value = "";
            switch (name)
            {
                case 'I':
                    value = "INSTALACIÓN";
                    break;
                case 'V':
                    value = "VISITA";
                    break;
                case 'R':
                    value = "RETIRO";
                    break;
                default:
                    value = "N/A";
                    break;
            }
            return value;
        }

        private void Add_Section_To_Doc(Document doc, string contents)
        {
            var parsedHtmlElements = HTMLWorker.ParseToList(new StringReader(contents), null);
            foreach (var htmlElement in parsedHtmlElements)
                doc.Add(htmlElement as IElement);
        }

        private void Add_Footer_Logo(Document doc)
        {
            string fontpath = System.Web.HttpContext.Current.Server.MapPath("~/Content/dist/img/");
            iTextSharp.text.Image imagen = Image.GetInstance(fontpath + "footer_template.jpg");
            imagen.ScalePercent(62f);
            imagen.SetAbsolutePosition(0, 0);
            doc.Add(imagen);
        }
    }
}