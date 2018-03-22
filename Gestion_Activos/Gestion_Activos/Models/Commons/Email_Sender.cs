using Gestion_Activos.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Web;

namespace Gestion_Activos.Models.Commons
{
    public class Email_Sender
    {
        private string email    = "informes@grupo3c.com";
        private string password = "cococo@3c.";
        //private string password = "Wazu6649";

        private string from_nam = "Cococo Informa";
        private string host     = "smtp.office365.com"; 
        private int    port     = 587;

        public string Send_Email(string address = "", string name="", string subject = "")
        {
            string mjs = "";
            try
            {
                var fromAddress     = new MailAddress(email, from_nam);
                string fromPassword = password;
                var toAddress  = new MailAddress("soportePOS@grupo3c.com", "Soporte Credomatic");
                var toAddress2 = new MailAddress("rjarac@credomatic.com", "Soporte Credomatic");

                var body = "<html>" +
                               "<body>" +
                                    "<h2>Estimado: " + name + "</h2>" +
                                    "<br/>• En este correo electrónico encontrará una copia de la boleta del servicio brindado por uno de nuestros técnicos. " +
                                    "<br/>• Además, como parte del proyecto de Punto de Venta, a Credomatic le interesa conocer su opinión con relación al servicio brindado por COCOCO. " +
                                    "<br/><br/><hr/>" +
                                    "• Agradecemos la oportunidad que nos brinda de servirle mejor por eso le invitamos a completar la siguiente encuesta:  " +
                                    "<br/><br/>" +
                                    "<a href='https://es.surveymonkey.com/r/35G2WGV'>" +
                                        "<img src='http://i1236.photobucket.com/albums/ff446/CococoCR/encuesta1_zpsxvthdnnu.jpg'/>"+
                                     "</a>" +
                                    "<br/><br/><hr/>" +
                                    "<h3>Equipo de Punto de Venta Credomatic.</h3>" +
                                    "• En caso de no estar conforme con la información adjunta, cuenta a partir de este momento con " +
                                    "<span style='font-weight:bold;'>3 días hábiles</span> para realizar el reclamo respectivo, ya sea" +
                                    "< br/> respondiendo este correo electrónico, o bien escribiendonos directamente al correo: soporte @grupo3c.com" +
                                    "<br/><br/>" +
                                    "<img src='http://i1236.photobucket.com/albums/ff446/CococoCR/soporte_zpsojxfyikh.png'>" +
                                "</body>"+
                           "</html>";
                var smtp = new SmtpClient
                {
                    Host = host,
                    Port = port,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword) 
                };

                Thread T1 = new Thread(delegate ()
                {
                    using (var message = new MailMessage(fromAddress, toAddress)
                    {
                        Subject = subject,
                        Body    = body
                    })
                    {
                        if (address != "")
                        {
                            message.To.Add(new MailAddress(address, name)); //correo del cliente agregado por el tecnico
                            message.To.Add(toAddress2); // correo de richard jara, cambiar en caso de que sea necesario
                        }
                        message.IsBodyHtml = true;
                        message.Attachments.Add(new Attachment(@"C:\Windows\Temp\Boleta de servicio tecnico.pdf"));
                        smtp.Send(message);
                    }
                });
                T1.Start();
                mjs = "Success";
            }
            catch (Exception e)
            {
                mjs = "Error al enviar email " + e;
                Generate_Exception(mjs);
            }
            return mjs;
        }

        private object RenderViewToString(Email_Sender email_Sender, string v, object p)
        {
            throw new NotImplementedException();
        }

        private void Generate_Exception(string detail = "")
        {
            Exceptions_Management ex = new Exceptions_Management();
            ex.Create_Exception(8, detail);
        }
    }
}