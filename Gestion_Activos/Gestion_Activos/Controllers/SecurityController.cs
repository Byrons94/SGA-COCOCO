using Gestion_Activos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gestion_Activos.Models.Class;
using Gestion_Activos.Models.Commons;
using System.Web.Security;

namespace Gestion_Activos.Controllers
{
    public class SecurityController : Controller
    {
        //BMS_DATA_DENTEntities connection = new BMS_DATA_DENTEntities();

        // GET: Security
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Error404()
        {
            return View();
        }


        public ActionResult Login(FormCollection collection = null)
        {
            BDConnection.Get_Connection();
            if (collection != null)
            { 
                string userName = collection["nombre"];
                string password = collection["password"];
                string mjs = "mensaje";
                if (userName != null && password != null)
                {
                     mjs = Validate_User(userName, password);
                    if (mjs == "Succesful")
                    {
                        Start_Session(userName, password);
                        return RedirectToAction("Index", "Dashboard");
                    }
                    else
                    {
                        ViewBag.Error     = "Credenciales incorrectas";
                        ViewBag.Exception = mjs;
                    }
                }
            }
            return View();
        }
        
        private string Validate_User(string UserName = null, string Password = null)
        {
            string mjs = "";
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            if (UserName != null && Password != null)
            {
                try
                {
                    var objuser = connection.SGA_USUARIOS.FirstOrDefault(x => x.USER_LOGIN == UserName && x.CONTRASENA == Password && x.ACTIVO == true);
                    return (objuser != null) ? "Succesful" : "Error de validacion";
                }
                catch (Exception e)
                {
                    return mjs + e;
                }
            }
            return mjs;
        }

        private void Start_Session(string UserName = null, string Password = null)
        {
            UserController us = new UserController();
            User model = us.Get_User_By_User_Password(UserName, Password);
            if (model != null)
            { 
                SessionHelper.AddUserToSession(model.id.ToString());
                SessionHelper.AddNewSession("NAME_USER", (model.name + " " + model.last_name));
                SessionHelper.AddNewSession("ROL_ID", model.id_rol.ToString());
                SessionHelper.AddNewSession("ROL_USER", model.name_rol);
                SessionHelper.AddNewSession("MENU_PERMISSIONS", Menu_Permissions(model.id_rol));
            }
        }

        private void Set_Session(string key, string value)
        {
            Session[key] = value;
        }

        private string Menu_Permissions(int id_rol=0)
        {
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            var inq = new int[] {1, 5, 9, 10, 11, 12, 15, 18, 21, 22, 23 }; //permisos del menu
            string permissions_list = "";
            var model = (from r_permissions in connection.SGA_ROLES_PERMISOS
                         join permiso in connection.SGA_PERMISOS
                         on r_permissions.ID_PERMISO equals permiso.ID
                         where (id_rol <= 2 || (r_permissions.ID_ROL == id_rol && inq.Contains(permiso.IDENTIFICADOR)))
                         select permiso.IDENTIFICADOR).ToList();
            foreach (var i in model)
            {
                permissions_list +=  Convert.ToString(i)+",";
            }
            return permissions_list;
        }

        public ActionResult Log_Out()
        {
            SessionHelper.DestroyUserSession();
            return RedirectToAction("Login", "Security");
        }

        private void Generate_Exception(string detail = "")
        {
            Exceptions_Management ex = new Exceptions_Management();
            ex.Create_Exception(6, detail);
        }
    }
}