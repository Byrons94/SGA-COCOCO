using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gestion_Activos.Models.Commons
{
    public class Permission_Validate
    {
        public static bool Have_Permission(Permissions_Enum permiso)
        {
            if (HttpContext.Current.Session["MENU_PERMISSIONS"] != null) {
                string list = HttpContext.Current.Session["MENU_PERMISSIONS"].ToString();
                var array = list.Split(',');
                foreach (var i in array)
                {
                    if (i.Trim() != "") { 
                        int value = (int)Enum.Parse(typeof(Permissions_Enum), Enum.GetName(typeof(Permissions_Enum), permiso));
                        if (value == Convert.ToInt32(i))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}