using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gestion_Activos.Models;
using Gestion_Activos.Models.Class;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;
using Gestion_Activos.Models.Commons;

namespace Gestion_Activos.Controllers
{
    [Autenticado]
    public class Rol_PermissionsController : Controller
    {
        BMS_DATA_DENTEntities connection = new BMS_DATA_DENTEntities();
        #region views
        [Permission(Permiso = Permissions_Enum.Visualizar_Rol)]
        public ActionResult Index()
        {
            return View();
        }

        [Permission(Permiso = Permissions_Enum.Visualizar_Rol)]
        public ActionResult List_Roles()
        {
            var model = (from p in connection.SGA_ROLES
                         where p.ID > 1
                         orderby p.ID
                         select new Rol
                         {
                             id         = p.ID,
                             name       = p.NOMBRE_ROL,
                             permanet   = p.PERMANENTE.Value
                         });
            return View(model.ToList());
        }

        public ActionResult List_Permissions()
        {
            var model = Get_Permission_By_Id();
            return View(model.ToList());
        }

        [Permission(Permiso = Permissions_Enum.Registrar_Rol)]
        public ActionResult Create_Rol()
        {
            var model = Get_Permission_By_Id();
            return View(model.ToList());
        }

        [Permission(Permiso = Permissions_Enum.Editar_Rol)]
        public ActionResult Edit_Rol(int id_Rol=1)
        {
            Rol rol_edit    = Get_Rol_By_Id(id_Rol);
            var permisions  = Get_Permission_By_Id(id_Rol);
            rol_edit.list_permissions = permisions;
            return View(rol_edit);
        }

        [HttpPost]
        public ActionResult Save_Created_Rol(string name_Rol, string[] permisos)
        {
            int id_inserted = Save_Rol(name_Rol);
            string result   = Insert_Rol_Permissions(id_inserted, permisos);
            return RedirectToAction("List_Roles");
        }
        #endregion

        #region ajax calls
        [HttpPost]
        public JsonResult Save_Edited_Rol(string id_rol, string name_rol, string[] permisos)
        {
            string value = "";
            int id_int   = Convert.ToInt32(id_rol);
            Rol old_rol  = Get_Rol_By_Id(id_int);
            if (Utilities.IsNumeric(id_rol) && !old_rol.permanet)
            {
                string successful = Update_Rol(old_rol, id_int, name_rol);
                if (successful == "0" )
                {
                   string successful_D = Delete_Rol_Permissions(id_int);
                   string successful_I = Insert_Rol_Permissions(id_int, permisos);
                    if (successful_D == "0" && successful_I == "0")
                    {
                        value = "Update realizado con éxito";
                    }
                    else
                    {
                        value = successful_I + " " + successful_D;
                    }
                }
            }
            else
            {
                value = "Error al actualizar";
            }
            return Json(value);
        }

        [Permission(Permiso = Permissions_Enum.Eliminar_Rol)]
        public JsonResult Delete_Rol_Id(string id_rol)
        {
            string value="";
            if (Utilities.IsNumeric(id_rol))
            {
                int id_int      = Convert.ToInt32(id_rol);
                int cantUserRol = Get_Count_Users_Rol(id_int);
                if (cantUserRol == 0)
                {
                    value = Delete_Rol(id_int);
                    value = Delete_Rol_Permissions(id_int);
                    value = (value == "0") ? "Eliminado con éxito." : value;
                }
                else
                {
                    value = "No se puede borrar! Existen usuarios con el rol seleciconado.";
                }
            }
            else
            {
                value = "El rol es inválido";
            }
            return Json(value);
        }

        public JsonResult Validate_Name_Rol(string name_rol)
        {
            bool exist = false;
            Rol rol = Get_Rol_By_Name(name_rol);
            exist   = (rol == null)? false : true;
            return Json(exist);
        }

        [HttpPost]
        public JsonResult Get_Rols()
        {   
            List<Rol> rol_list = Get_Rol_List();
            Dictionary<string, string> list = new Dictionary<string, string>();
            foreach (var rol in rol_list)
            {
                list.Add(Convert.ToString(rol.id), rol.name);
            }
            return Json(list);
        }
        #endregion
        

        #region querys
        private List<Rol> Get_Rol_List()
        {
            var model = (from p in connection.SGA_ROLES
                         where p.ID != 1
                         select new Rol
                         {
                             id         = p.ID,
                             name       = p.NOMBRE_ROL,
                             permanet   = p.PERMANENTE.Value
                         });
            return model.ToList();
        }

        private Rol Get_Rol_By_Id(int id_Rol=1)
        {
            var model = (from p in connection.SGA_ROLES
                         where p.ID == id_Rol
                         select new Rol
                         {
                             id         = p.ID,
                             name       = p.NOMBRE_ROL,
                             permanet   = p.PERMANENTE.Value
                         }).FirstOrDefault();
            return model;
        }

        private Rol Get_Rol_By_Name(string name = "")
        {
            try {
                    var model = (from p in connection.SGA_ROLES
                        where p.NOMBRE_ROL == name
                        select new Rol
                        {
                            id       = p.ID,
                            name     = p.NOMBRE_ROL,
                            permanet = p.PERMANENTE.Value
                        }).FirstOrDefault();
                return model;
            } catch (Exception) {
                return null;
            }
        }

        private int Get_Last_Id_Rol_Inserted()
        {
            var id = (from p in connection.SGA_ROLES
                         select p.ID
                         ).Max();
            return id;
        }

        private int Get_Count_Users_Rol(int id_rol = 0)
        {
            var count = (from p in connection.SGA_USUARIOS
                         where p.ID == id_rol
                         select p).Count();
            return count;
        }

        private List<Permission> Get_Permission_By_Id(int id_rol=1)
        {
            var list = (from permission in connection.SGA_PERMISOS
                        join rolU in connection.SGA_ROLES_PERMISOS
                        on new { x = permission.ID, y = id_rol } equals new { x = rolU.ID_PERMISO, y = rolU.ID_ROL }
                        into UnionValues
                        from rolU in UnionValues.DefaultIfEmpty()
                        select new Permission
                        {
                            ID          = permission.ID,
                            Category    = permission.CATEGORIA.Value,
                            Description = permission.NOMBRE,
                            Active = (rolU == null ? false : true)
                        });
            return list.ToList();
        }

        //inserts- creates
        private string Insert_Rol_Permissions(int id_rol, string[] permissions)
        {
            string value = "0";
            foreach (var permission in permissions)
            {
                if (Utilities.IsNumeric(permission))
                {
                    SGA_ROLES_PERMISOS n_permi = new SGA_ROLES_PERMISOS()
                    {
                        ID_PERMISO = Convert.ToInt32(permission),
                        ID_ROL     = id_rol
                    };
                    connection.SGA_ROLES_PERMISOS.Add(n_permi);
                }
                try
                {
                    connection.SaveChanges();
                }
                catch (Exception e)
                {
                    value = "Error " + e;
                }
            } 
            return value;
        }

        private int Save_Rol(string name)
        {
            int id = 0;
            if (name != "" && name != null)
            {
                SGA_ROLES rol = new SGA_ROLES()
                {
                    NOMBRE_ROL = name,
                    PERMANENTE = false
                };
                connection.SGA_ROLES.Add(rol);
                try
                {
                    connection.SaveChanges();
                    id = Get_Last_Id_Rol_Inserted();
                }
                catch (Exception)
                {
                    return 0;
                }
            }
            return id;
        }

        
        //updates
        private string Update_Rol(Rol old_rol, int id_rol, string name_rol)
        {
            string value = "0";
            if (old_rol.name != name_rol) {
                var query = (from rols in connection.SGA_ROLES
                             where rols.ID == id_rol &&
                                   rols.PERMANENTE != true
                             select rols);
                foreach (SGA_ROLES rol in query)
                {
                    rol.NOMBRE_ROL = name_rol;
                }
                try
                {
                    connection.SaveChanges();
                }
                catch (Exception e)
                {
                    value = "Error " +e;
                }
            }
            return value;
        }

        //deletes
        private string Delete_Rol_Permissions(int id_rol)
        {
            string value = "0";
            var remove = from permission in connection.SGA_ROLES_PERMISOS
                         where permission.ID_ROL == id_rol
                         select permission;
            foreach (var detail in remove)
            {
                connection.SGA_ROLES_PERMISOS.Remove(detail);
            }
            try
            {
                connection.SaveChanges();
            }
            catch (Exception e)
            {
                value = "Error " + e;
            }
            return value;
        }

        private string Delete_Rol(int id_rol)
        {
            string value = "0";
            var remove = from rols in connection.SGA_ROLES
                         where rols.ID == id_rol &&
                               rols.PERMANENTE == false
                         select rols;
            foreach (var rol in remove)
            {
                connection.SGA_ROLES.Remove(rol);
            }
            try
            {
                connection.SaveChanges();
            }
            catch (Exception e)
            {
                value = "Error " + e;
            }
            return value;
        }
        #endregion

        private void Generate_Exception(string detail = "")
        {
            Exceptions_Management ex = new Exceptions_Management();
            ex.Create_Exception(5, detail);
        }
    }
}