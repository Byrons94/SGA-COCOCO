using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace Gestion_Activos.Models.Commons
{
    public class External_Services
    {
        private string pc_track_service_url = "http://pctrack.net/cococo/validacion.php";
        private string key_pc_track        = "20161031";

        #region pc_track
        //afiliadoAntiguo
        //afiliado viene el nuevo
        //comunicacion con servicios de pctrack.com para validar instalaciones de cases
        public string Call_Service(string tipo = "", string serie = "", string afiliado = "")
        {
            var postData =  "key="    + key_pc_track;
                postData += "&serie=" + serie;
                postData += "&tipo="  + tipo;
                postData += "&afiliado=" + afiliado;

            var data = Encoding.ASCII.GetBytes(postData);
            try
            {
                HttpWebRequest request = WebRequest.Create(pc_track_service_url) as HttpWebRequest;
                request.Method          = "Post";
                request.ContentType     = "application/x-www-form-urlencoded";
                request.ContentLength   = data.Length;
                using (var stream   = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                return responseString.ToString();
            }
            catch (Exception e)
            {
                return "Exception " + e;
            }
        }
        #endregion

    }
}