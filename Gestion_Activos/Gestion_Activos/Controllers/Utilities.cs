using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Gestion_Activos.Controllers
{
    public static class Utilities
    {
        public static bool IsNumeric(object Expression)
        {
            double retNum;
            bool isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }

        //extract the values if exits, if not return white text
        public static string Elements_Form_Post(FormCollection form, string keyValue)
        {
            string values = (form.AllKeys.Contains(keyValue)) ? (form[keyValue]) : "";
            return values;
        }

        //tipos: 1=compra, 2=instalacion, 3=cambio, 4=retiro, 5=otro
        public static string Msj_Log_Serial(int type = 0, string serial1="", string serial2="")
        {
            string mjs = "";
            if (type == 1)
            {
                mjs = "Compra";
            }
            else if (type == 2)
            {
                mjs = "Instalación";
            }
            else if (type == 3)
            {
                mjs = "Cambio de serie: " + serial1.Trim() + " por: " + serial2.Trim();
            }
            else if (type == 4)
            {
                mjs = "Retiro de serie: " + serial1;
            }
            else
            {
                mjs = "No definido";
            }
            return mjs.Trim();
        }

        public static string convertDataTJson(object o) {
            string data = "";
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            data = serializer.Serialize(o);
            return data;
        }

    }
}