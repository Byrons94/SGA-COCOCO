using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gestion_Activos.Models.Class;
using Gestion_Activos.Models.Commons;
using Gestion_Activos.Models;

namespace Gestion_Activos.Controllers
{
    public class Exceptions_Management
    {
        public void Create_Exception(int module = 0, string detaill = "")
        {
            DateTime date   = Get_Date();
            string user     = Get_User_Name();
            string module_n = Get_Module_Name(module);
            Save_Exception(module_n, user, detaill, date);
        }

        private void Save_Exception(string module="", string user="", string detaill="", DateTime? date = null)
        {
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            try
            {
                if (module != "" && user != "" && detaill != "")
                {
                    SGA_EXEPCIONES exeption = new SGA_EXEPCIONES()
                    {
                        USUARIO = user,
                        DETALLE = detaill,
                        MODULO  = module,
                        FECHA   = date.Value
                    };
                    connection.SGA_EXEPCIONES.Add(exeption);
                    connection.SaveChanges();
                }
            }
            catch
            {
                string mjs = "una excepcion en una excepcion sería el colmo";
            }
        }

        private string Get_Module_Name(int id_module=0)
        {
            string name = "";
            switch(id_module){
                case 1:
                    name = "Clientes";
                break;
                case 2:
                    name = "Dashboard";
                break;
                case 3:
                    name = "Inventarios";
               break;
               case 4:
                    name = "Productos";
               break;
               case 5:
                    name = "Roles y permisos";
               break;
               case 6:
                    name = "Seguridad";
               break;
               case 7:
                    name = "Usuarios";
               break;
               case 8:
                    name = "Tickets";
                    break;
               case 9:
                    name = "Excepciones :V";
                    break;
                default:
                    name = "No definido";
               break;
            }
            return name;
        }

        private string Get_User_Name()
        {
            UserController uc = new UserController();
            User user         = uc.Get_User_By_Id(SessionHelper.GetUser());
            string name       = user.name + " " + user.last_name;
            return name;
        }

        private DateTime Get_Date()
        {
            return DateTime.Now;
        }
    }
}