using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Web.Security;

namespace Gestion_Activos.Models.Commons
{
    public class SessionHelper
    {
        public static bool ExistUserInSession()
        {
            return (HttpContext.Current.Session["usuario"] != null)? true : false;
        }

        public static void DestroyUserSession()
        {
            HttpContext.Current.Session.Abandon();
            HttpContext.Current.Session.Clear();
        }

        public static int GetUser()
        {
            int user_id = 0;
            if (HttpContext.Current.Session["usuario"] != null)
            {
                user_id = Convert.ToInt32(HttpContext.Current.Session["usuario"]);
                ResetSession();
            }
            return user_id;
        }

        public static int GetRol()
        {
            int user_rol = 0;
            if (HttpContext.Current.Session["usuario"] != null)
            {
                user_rol = Convert.ToInt32(HttpContext.Current.Session["ROL_ID"]);
            }
            return user_rol;
        }

        public static void AddUserToSession(string id)
        {
            HttpContext.Current.Session["usuario"] = id;
        }

        public static void AddNewSession(string key, string value)
        {
            HttpContext.Current.Session[key] = value;
        }

        public static void ResetSession()
        {
            HttpContext.Current.Session["usuario"] = HttpContext.Current.Session["usuario"];
        }
    }
}
