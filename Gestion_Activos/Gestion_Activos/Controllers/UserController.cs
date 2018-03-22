using Gestion_Activos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gestion_Activos.Models.Class;
using Gestion_Activos.Models.Commons;

namespace Gestion_Activos.Controllers
{
    [Autenticado]
    public class UserController : Controller
    {
        BMS_DATA_DENTEntities connection = new BMS_DATA_DENTEntities();

        #region views
        public ActionResult Index()
        {
            List<User> users_list = Get_All_Users();
            return View(users_list.ToList());
        }

        // POST: User/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                string name_f      = collection["nombre"];
                string last_name_f = collection["apellido"];
                string login_f     = collection["login"];
                string password_f  = collection["password"];
                string rol_f       = collection["rol"];
                string cod_tec_r   = collection["cod_tec"];
                User user = null;
                if (name_f != "" && last_name_f != "" && login_f != "" && password_f != "" && rol_f != "" && Utilities.IsNumeric(rol_f)) {
                    user = new User {
                        name       = name_f,
                        last_name  = last_name_f,
                        login_name = login_f,
                        password   = password_f,
                        id_rol     = Convert.ToUInt16(rol_f),
                        cod_tec    = cod_tec_r
                    };
                }
                string value = Save_User(user);
                if (value == "0")
                {
                    ViewData["succefull"] = value;
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: User/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: User/Edit/5
        [HttpPost]
        public ActionResult Edit(FormCollection collection)
        {
            try
            {
                string name_f       = collection["nombre"];
                string last_name_f  = collection["apellido"];
                string login_f      = collection["login"];
                string password_f   = collection["password"];
                string rol_f        = collection["rol"];
                bool status         = (collection["estatus"] == "true") ? true : false;
                string cod_tec_r    = collection["cod_tec"];
                int id_user         = Convert.ToInt16(collection["id_user"]);
                User user = null;
                if (name_f != "" && last_name_f != "" && login_f != "" && password_f != "" && rol_f != "" && Utilities.IsNumeric(rol_f))
                {
                    user = new User
                    {
                        id          = id_user,
                        name        = name_f,
                        last_name   = last_name_f,
                        login_name  = login_f,
                        password    = password_f,
                        id_rol      = Convert.ToUInt16(rol_f),
                        active      = status,
                        cod_tec     = cod_tec_r
                    };
                }
                var value = Update_User(user);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: User/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: User/Delete/5
        [HttpPost]
        public ActionResult Delete(FormCollection collection)
        {
            try
            {
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        #endregion


        #region ajax_calls
        [HttpPost]
        public JsonResult Get_Json_User_Id(string id_user)
        {
            User user = null;
            if (Utilities.IsNumeric(id_user)) {
                user = Get_User_By_Id(Convert.ToInt16(id_user));
            }
            if (user != null) {
                var items = new[] {
                    new {
                        id          = user.id,
                        name        = user.name,
                        last_name   = user.last_name,
                        login       = user.login_name,
                        password    = user.password,
                        rol         = user.id_rol,
                        status      = user.active,
                        cod_tec     = user.cod_tec
                        }
                    };
                return Json(items);
            }
            return Json("Error");
        }

        [HttpPost]
        public JsonResult Delete_User_Id(string id_user)
        {
            string value = "";
            if (Utilities.IsNumeric(id_user))
            {
                int id_int  = Convert.ToInt32(id_user);
                value       = Delete_User(id_int);
            }
            else
            {
                value = "El usuario es inválido";
            }
            return Json(value);
        }
        #endregion


        #region scripts
        private List<User> Get_All_Users()
        {
            var model = (from user in connection.SGA_USUARIOS
                         join rol in connection.SGA_ROLES
                            on user.ROL equals rol.ID
                         where user.ROL != 1
                         select new User
                         {
                             id         = user.ID,
                             name       = user.NOMBRE,
                             last_name  = user.APELLIDO,
                             login_name = user.USER_LOGIN,
                             name_rol   = rol.NOMBRE_ROL,
                             creation_date = user.FECHA_REGISTRO,
                             active     = user.ACTIVO,
                             cod_tec    = user.COD_TECNICO
                         });
            return model.ToList();
        }

        public User Get_User_By_Id(int id_user = 1)
        {
            var model = (from user in connection.SGA_USUARIOS
                         join rol in connection.SGA_ROLES
                         on user.ROL equals rol.ID
                         where user.ID == id_user
                         select new User
                         {
                             id         = user.ID,
                             name       = user.NOMBRE,
                             last_name  = user.APELLIDO,
                             login_name = user.USER_LOGIN,
                             password   = user.CONTRASENA,
                             id_rol     = user.ROL,
                             name_rol   = rol.NOMBRE_ROL,
                             creation_date = user.FECHA_REGISTRO,
                             active     = user.ACTIVO,
                             cod_tec    = user.COD_TECNICO
                         }).First();
            return model;
        }

        public User Get_User_By_User_Password(string UserName = null, string Password = null)
        {
            var model = (from user in connection.SGA_USUARIOS
                         join rol in connection.SGA_ROLES
                         on user.ROL equals rol.ID
                         where user.USER_LOGIN == UserName && user.CONTRASENA == Password
                         select new User
                         {
                             id = user.ID,
                             name = user.NOMBRE,
                             last_name = user.APELLIDO,
                             name_rol = rol.NOMBRE_ROL,
                             id_rol = rol.ID
                         }).FirstOrDefault();
            return model;
        }


        private string Save_User(User new_user =null)
        {
            string value = "0";
            if (new_user != null) {
                DateTime now = DateTime.Now;
                SGA_USUARIOS user = new SGA_USUARIOS()
                {
                    NOMBRE      = new_user.name,
                    APELLIDO    = new_user.last_name,
                    USER_LOGIN  = new_user.login_name,
                    CONTRASENA  = new_user.password,
                    FECHA_REGISTRO = now,
                    ROL         = new_user.id_rol,
                    ACTIVO      = true,
                    COD_TECNICO = new_user.cod_tec
                };
                connection.SGA_USUARIOS.Add(user);
                try
                {
                    connection.SaveChanges();
                }
                catch (Exception e)
                {
                    value = "Error: " + e;
                }
            }
            return value;
        }

        private string Update_User(User edited_user)
        {
            string value = "0";
                var query = (from user in connection.SGA_USUARIOS
                             where user.ID == edited_user.id
                             select user);
                foreach (SGA_USUARIOS user in query) {
                    user.NOMBRE      = edited_user.name;
                    user.APELLIDO    = edited_user.last_name;
                    user.USER_LOGIN  = edited_user.login_name;
                    user.CONTRASENA  = edited_user.password;
                    user.ROL         = edited_user.id_rol;
                    user.ACTIVO      = edited_user.active;
                    user.COD_TECNICO = edited_user.cod_tec;
                }
                try
                {
                    connection.SaveChanges();
                }
                catch (Exception e)
                {
                    value = "Error " +e;
                }
            return value;
        }
        
        private string Delete_User(int id_user = 0)
        {
            string value = "0";
            var remove   = from   user in connection.SGA_USUARIOS
                           where  user.ID == id_user
                           select user;
            foreach (var user_r in remove)
            {
                connection.SGA_USUARIOS.Remove(user_r);
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
            ex.Create_Exception(7, detail);
        }
    }
}
