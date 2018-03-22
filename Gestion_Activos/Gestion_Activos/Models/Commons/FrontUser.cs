using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gestion_Activos.Models.Class;

namespace Gestion_Activos.Models.Commons
{
    public class FrontUser
    {
        static BMS_DATA_DENTEntities connection = null;
        public static bool Have_Permission(Permissions_Enum permiso)
        {
            Get_Conection();
            var rol_id    = FrontUser.Get_User_Rol();
            int val = (int)Enum.Parse(typeof(Permissions_Enum), Enum.GetName(typeof(Permissions_Enum), permiso));
            var model = (from permission in connection.SGA_PERMISOS
                         join rp in connection.SGA_ROLES_PERMISOS
                         on permission.ID equals rp.ID_PERMISO
                         where (rp.ID_ROL == rol_id && permission.IDENTIFICADOR == val) || rol_id <= 2
                         select permission);
             return model.Any();
        }

        public static int Get_User_Rol()
        {
            return SessionHelper.GetRol();
        }

        public static void Get_Conection()
        {
            if (FrontUser.connection == null)
            {
                FrontUser.connection = new BMS_DATA_DENTEntities();
            }
        }
    }
}