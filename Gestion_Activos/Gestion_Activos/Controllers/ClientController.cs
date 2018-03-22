using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Gestion_Activos.Models.Class;
using Gestion_Activos.Models;
using System.Data.SqlClient;
using System.Data;
using System.Web.Script.Serialization;
using Gestion_Activos.Models.Commons;

/// <summary>
/// this class manage all the conections (in the cliet entity) between the UI and the models, also manage the conections and querys to the DB
/// Byron Serrano 
/// v2.0
/// </summary>
namespace Gestion_Activos.Controllers
{
    [Autenticado]
    public class ClientController : Controller
    {
        #region views
        //this regios represent the views of the controller
        [Permission(Permiso = Permissions_Enum.Visualizar_afiliado)]
        public ActionResult Index()
        {
            List<Client> list = Get_List_Clients();
            return View(list);
        }

        public ActionResult Details_C(string id_client = "")
        {
            if (id_client == "")
            {
                RedirectToAction("Index"); 
            }
            if (Utilities.IsNumeric(id_client))
            {
                int id        = Convert.ToInt32(id_client);
                Client client = Get_Client_By_Id(id);

                //TicketController tc  = new TicketController();
                //ProductController pc = new ProductController();
                if (client != null)
                {
                    client.inventary = new ProductController().Products_Inventory_By_Client(client.membership_number);
                    client.tickets   = new TicketController().Get_Tickets_By_Client(client.membership_number);
                }
                return View(client);
            }

            return View();
        }
        #endregion


        #region Jsons
        //this region represent all teh JSON calls made by the UI
        [HttpPost]
        public JsonResult Change_Client_Status_Client(string id_client = null, string estatus = "")
        {
            bool status = (estatus == "A" || estatus == "R" || estatus == "L"  || estatus == "X") ? true : false;
            /* A = ACTIVO * I = INACTIVO * L=LEGAL * R=RETIRO * I = INACTIVO*/
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            string mjs = "";
            if (id_client != null)
            {
                var model = (from client in connection.SGA_AFILIADOS
                             where client.N_AFILIADO == id_client
                             select client);
                foreach (SGA_AFILIADOS client in model)
                {
                    client.ACTIVO             = status;
                    client.FECHA_INACTIVACION = DateTime.Now;
                    client.ESTATUS = estatus;
                }
                try
                {
                    connection.SaveChanges();
                    mjs = "Successful";
                }
                catch (Exception e)
                {
                    mjs = "Error " + e;
                    Generate_Exception(mjs);
                }
            }
            return Json(mjs);
        }
        

        [HttpPost]
        [Permission(Permiso = Permissions_Enum.Cambiar_Afiliado)]
        public JsonResult Change_Membership(string old_afiliado="", string n_afiliado="", string contacto="", string comercio="", string telefono="", string provincia="",
                                            string canton = "", string distrito="", string direccion="", string referencia="")
        {
            Change_Membership_Sp(old_afiliado, n_afiliado, contacto, comercio, telefono, provincia, canton, distrito, direccion, referencia);
            return Json("Successful");
        }   


        public JsonResult Get_Clients_Summary()
        {
            DateTime start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime now   = DateTime.Now;
             
            List<int> list = Get_Summary_Client_Movements_Dates(start, now);
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(list);
            return Json(json);
        }


        public JsonResult Get_Provincia_Canton_Distrito()
        {
            List<WEB_Provincia> lp = Get_Provincias();
            List<WEB_Canton> lc = Get_Cantones();
            List<WEB_Distrito> ld = Get_Distritos();
            Tuple<List<WEB_Provincia>, List<WEB_Canton>, List<WEB_Distrito>> tupple = new Tuple<List<WEB_Provincia>, List<WEB_Canton>, List<WEB_Distrito>>(lp, lc, ld);
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(tupple);
            return Json(json);
        }


        public JsonResult Get_Clients_By_Search(string busqueda = "", string tipo = "", string estatus = "")
        {
            string data = "";
            if (busqueda != "" && tipo != "" && estatus != "")
            {
                List<Client> list = Get_Client_By_Parametres(busqueda, tipo, estatus);
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                data = serializer.Serialize(list);
            }
            return Json(data);
        }

        public JsonResult Update_Client(string id_client = null)
        {
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            string mjs = "";
            if (id_client != null)
            {
                var model = (from client in connection.SGA_AFILIADOS
                             where client.N_AFILIADO == id_client
                             select client);
                foreach (SGA_AFILIADOS client in model)
                {
                    client.ACTUALIZADO = true;
                }
                try
                {
                    connection.SaveChanges();
                    mjs = "Successful";
                }
                catch (Exception e)
                {
                    mjs = "Error " + e;
                }
            }
            return Json(mjs);
        }
        #endregion


        #region querys
        //this regions is use to create the calls to the database
        //Get the last 10 register clients 
        public List<Client> Get_List_Clients()
        {
            int amount = 10;
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            try{
                var model = (from client in connection.SGA_AFILIADOS
                             join province in connection.WEB_Provincia
                                on client.N_COD_PROV equals province.idProvincia
                             join canton in connection.WEB_Canton
                                 on (client.N_COD_PROV+client.N_COD_CANT).Trim()  equals canton.idCanton
                             join distrito in connection.WEB_Distrito
                                 on (client.N_COD_PROV + client.N_COD_CANT + client.N_COD_DIST).Trim() equals distrito.idDistrito
                             orderby client.ID descending
                             select  new Client {
                                 id                 = client.ID,
                                 membership_number  = client.N_AFILIADO,
                                 local_name         = client.NOMBRE_NEGOCIO,
                                 contact            = client.NOMBRE,
                                 phone              = client.TELEFONO,
                                 province           = province.nombreProvincia,
                                 canton             = canton.nombreCanton,
                                 district           = distrito.nombreDistrito,
                                 address            = client.DIRECCION_1,
                                 updated            = client.ACTUALIZADO.Value,
                                 status             = client.ACTIVO.Value,
                                 status_varchar     = client.ESTATUS
                             }).Take(amount);
                return model.ToList();
            }
            catch (Exception e) {
                Generate_Exception("Error " + e);
                throw e;
            }
        }
        
        public List<Client> Get_List_Clients_Active()
        {
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            try {
                var model = (from client in connection.SGA_AFILIADOS
                             join province in connection.WEB_Provincia
                            on client.N_COD_PROV equals province.idProvincia into provusr
                             from province in provusr.DefaultIfEmpty()
                             join canton in connection.WEB_Canton
                                on (client.N_COD_PROV + client.N_COD_CANT).Trim() equals canton.idCanton into cantusr
                             from canton in cantusr.DefaultIfEmpty()
                             join distrito in connection.WEB_Distrito
                                on (client.N_COD_PROV + client.N_COD_CANT + client.N_COD_DIST).Trim() equals distrito.idDistrito into distusr
                             from distrito in distusr.DefaultIfEmpty()
                             where client.ACTIVO == true &&
                                   client.N_AFILIADO != "00093394" //client credomatic excluded
                             orderby client.ID descending
                             select  new Client
                             {
                                 id                 = client.ID,
                                 membership_number  = client.N_AFILIADO,
                                 local_name         = client.NOMBRE_NEGOCIO,
                                 contact            = client.NOMBRE,
                                 phone              = client.TELEFONO,
                                 province           = province.nombreProvincia  == null ? "N/A" : province.nombreProvincia,
                                 canton             = canton.nombreCanton       == null ? "N/A" : canton.nombreCanton,
                                 district           = distrito.nombreDistrito   == null ? "N/A" : distrito.nombreDistrito,
                                 address            = client.DIRECCION_1,
                                 updated            = client.ACTUALIZADO.Value,
                                 status             = client.ACTIVO.Value,
                                 status_varchar     = (client.ESTATUS == "A" ? "ACTIVO":
                                                       client.ESTATUS == "I" ? "INACTIVO" :
                                                       client.ESTATUS == "L" ? "LEGAL" :
                                                       client.ESTATUS == "R" ? "RETIRO" :
                                                       client.ESTATUS == "X" ? "IRRECUPERABLE" : "N/A") 
                                 /*
                                  log.TIPO_MOVIMIENTO == "I" ? "INSTALACION" :
                                               log.TIPO_MOVIMIENTO == "V" ? "VISITA" :
                                               log.TIPO_MOVIMIENTO == "R" ? "RETIRO" : "N/A",
                                  */
                             });
                return model.ToList();
            }
            catch (Exception e) {
                Generate_Exception("Error " + e);
                throw e;
            }
        }
           
        // get the cliets by parameters
        private List<Client> Get_Client_By_Parametres(string BUSQUEDA = "", string PARAMETRO = "", string ESTATUS = "")
        {
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            var model = (from client in connection.F_BUSQUEDA_AFILIADO(BUSQUEDA, PARAMETRO, ESTATUS)
                         select new Client {
                             id                 = client.ID.Value,
                             membership_number  = client.N_AFILIADO,
                             local_name         = client.NOMBRE_NEGOCIO,
                             contact            = client.NOMBRE,
                             phone              = client.TELEFONO,
                             address            = client.DIRECCION_1,
                             province           = client.N_COD_PROV == null ? "": client.N_COD_PROV,
                             canton             = client.N_COD_CANT == null ? "": client.N_COD_CANT,
                             district           = client.N_COD_DIST == null ? "": client.N_COD_DIST,
                             updated            = client.ACTUALIZADO.Value,
                             status             = client.ACTIVO.Value,
                             status_varchar     = client.ACTIVO_V
                         }).DefaultIfEmpty();
            if (model != null){
                return model.ToList();
            }
            return null; 
        }
        
        //get client by specific id
        private Client Get_Client_By_Id(int id = 0)
        {
            DateTime now = DateTime.Now;
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            var model = (from client in connection.SGA_AFILIADOS
                         join province in connection.WEB_Provincia
                            on client.N_COD_PROV equals province.idProvincia into provusr
                         from province in provusr.DefaultIfEmpty()
                         join canton in connection.WEB_Canton
                            on (client.N_COD_PROV + client.N_COD_CANT).Trim() equals canton.idCanton into cantusr
                         from canton in cantusr.DefaultIfEmpty()
                         join distrito in connection.WEB_Distrito
                            on (client.N_COD_PROV + client.N_COD_CANT + client.N_COD_DIST).Trim() equals distrito.idDistrito into distusr
                         from distrito in distusr.DefaultIfEmpty()
                         where client.ID == id
                         select new Client
                         {
                             id                 = client.ID,
                             membership_number  = client.N_AFILIADO,
                             local_name         = client.NOMBRE_NEGOCIO,
                             contact            = client.NOMBRE,
                             phone              = client.TELEFONO,
                             address            = client.DIRECCION_1,
                             province           = province.nombreProvincia   == null ? "N/A" : province.nombreProvincia,
                             canton             = canton.nombreCanton        == null ? "N/A" : canton.nombreCanton,
                             district           = distrito.nombreDistrito    == null ? "N/A" : distrito.nombreDistrito,
                             updated            = client.ACTUALIZADO.Value,
                             status             = client.ACTIVO.Value,
                             last_membership    = client.ANTIGUO_N_AFILIADO  == null ? "N/A" : client.ANTIGUO_N_AFILIADO,
                             changed            = client.FECHA_CREACION      == null ? now   : client.FECHA_CREACION.Value,
                             new_menbership     = client.EXTRA_1             == null ? "N/A" : client.EXTRA_1,
                             other              = client.EXTRA_2,
                             status_varchar = client.ESTATUS
                         });
            Client clientF = null;
            if (model.Count() > 0) {
                TicketController tc = new TicketController();
                clientF = model.FirstOrDefault();
                string fecha = tc.Get_Last_Mainteinance_By_Client(clientF.membership_number);
                clientF.last_mainteinance = fecha;
            }   
            return clientF;
        }
        
        
        public Client Get_Client_By_Membership_Number(string ID_Number="0")
        {
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            DateTime now = DateTime.Now;
            var model = (from client in connection.SGA_AFILIADOS
                         join province in connection.WEB_Provincia
                            on client.N_COD_PROV equals province.idProvincia into provusr
                         from province in provusr.DefaultIfEmpty()
                         join canton in connection.WEB_Canton
                            on (client.N_COD_PROV + client.N_COD_CANT).Trim() equals canton.idCanton into cantusr
                         from canton in cantusr.DefaultIfEmpty() 
                         join distrito in connection.WEB_Distrito
                            on (client.N_COD_PROV + client.N_COD_CANT + client.N_COD_DIST).Trim() equals distrito.idDistrito into distusr
                            from distrito in distusr.DefaultIfEmpty()
                         where client.N_AFILIADO == ID_Number
                         select new Client {
                             id = client.ID,
                             membership_number  = client.N_AFILIADO,
                             local_name = client.NOMBRE_NEGOCIO,
                             contact = client.NOMBRE,
                             phone = client.TELEFONO,
                             address = client.DIRECCION_1,
                             province = province.nombreProvincia  != null ? province.nombreProvincia: "N/A",
                             canton = canton.nombreCanton       != null ? canton.nombreCanton: "N/A",
                             district = distrito.nombreDistrito   != null ? distrito.nombreDistrito: "N/A",
                             updated = client.ACTUALIZADO.Value,
                             status = client.ACTIVO.Value,
                             last_membership = client.ANTIGUO_N_AFILIADO == null ? "N/A" : client.ANTIGUO_N_AFILIADO,
                             changed = client.FECHA_CREACION == null ? now : client.FECHA_CREACION.Value,
                             new_menbership = client.EXTRA_1 == null ? "N/A" : client.EXTRA_1,
                             status_varchar = client.ESTATUS
                         }).FirstOrDefault();
            return model;
        }


        public List<Client> Get_List_Installed(DateTime? F_INICIO = null, DateTime? F_FINAL = null, string STATUS = "")
        {
            if(F_INICIO != null && F_FINAL != null){
                try{
                    BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
                    var model = (from client in connection.F_AFILIADOS_INSTALACIONES(F_INICIO, F_FINAL, STATUS)
                                 select new Client
                                 {
                                     local_name = client.COMERCIO,
                                     membership_number = client.AFILIADO,
                                     contact = client.CONTACTO,
                                     phone = client.TELEFONO,
                                     email = client.EMAIL,
                                     province = client.PROVINCIA,
                                     area = client.AREA,
                                     ejecutive = client.EJECUTIVO,
                                     software = client.TIPO_SOF,
                                     industry = client.TIPO_INDU,
                                     license = client.LICENCIA,
                                     installation = client.FEC_INSTA.Value,
                                     buy_date = client.FEC_COMP.Value,
                                     pago = client.PAGO_MEN,
                                     retired_date = client.FEC_RETIRO.Value,
                                     retire_comentary = client.MOT_RETIRO,
                                     address           = client.DIRECCION
                                 }).ToList();
                    return model;
                }
                catch (Exception e){
                    string mjs = "Error " + e;
                    Generate_Exception(mjs);
                }
            }
            return new List<Client>();
        }

        //retorna una lista con los clientes a los que se les ha realizado algun tipo de mantenimiento de equipo
        //llamese mantenimiento haber tenido una visita y haber seleccionado el boton de "mantenimiento"
        public List<Client> Get_Clients_Maintenances(DateTime? F_INICIO = null, DateTime? F_FIN = null)
        {
            if (F_INICIO != null && F_FIN != null)
            {
                BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
                var model = (from client in connection.F_REPORTE_MANTENIMIENTO_AFILIADOS(F_INICIO, F_FIN)
                             select new Client
                             {
                                 membership_number = client.N_AFILIADO,
                                 local_name = client.NOMBRE_NEGOCIO,
                                 address = client.DIRECCION,
                                 buy_date = client.FECHA.Value,
                                 other = client.ID_TICKET,
                                 other_2 = client.T_SERVICE,
                                 other_3 = client.TECNICO
                             });
                return model.ToList();
            }
            return new List<Client>();
        }


        public List<int> Get_Summary_Client_Movements_Dates(DateTime? f_inicio = null, DateTime? f_fin  = null)
        {
            if (f_inicio != null && f_fin != null)
            {
                try
                {
                    BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
                    List<int> movements = new List<int>();
                    movements.Add(connection.SGA_AFILIADOS.Where(a => a.FECHA_CREACION >= f_inicio && a.FECHA_CREACION <= f_fin).Count()); //activados
                    movements.Add(connection.SGA_AFILIADOS.Where(a => a.FECHA_INACTIVACION >= f_inicio && a.FECHA_INACTIVACION <= f_fin).Count()); //inactivads
                    movements.Add(connection.SGA_AFILIADOS.Count(a => a.ACTIVO == true)); //total activos
                    return movements;
                }
                catch (Exception e)
                {
                    string mjs = "Error " + e;
                    Generate_Exception(mjs);
                }
            }
            return null;
        }


        public List<Changed_Client> Get_List_Change_Client(DateTime? f_inifio = null, DateTime? f_fin = null)
        {
            if (f_inifio != null && f_fin != null)
            {
                try
                {
                    BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
                    var model = (from cambio in connection.SGA_CAMBIO_AFILIADO
                                 join old_client in connection.SGA_AFILIADOS
                                    on cambio.ANTIGUO_AFILIADO equals old_client.N_AFILIADO into old_change
                                 from old_client in old_change.DefaultIfEmpty()
                                 join new_client in connection.SGA_AFILIADOS 
                                    on cambio.NUEVO_AFILIADO equals new_client.N_AFILIADO into new_change
                                 from new_client in new_change.DefaultIfEmpty()
                                where cambio.FECHA >= f_inifio && cambio.FECHA <= f_fin
                                select new Changed_Client {
                                    old_membership = old_client.N_AFILIADO,
                                    old_local_name = old_client.NOMBRE_NEGOCIO,
                                    new_membership = new_client.N_AFILIADO,
                                    new_local_name = new_client.NOMBRE_NEGOCIO,
                                    address        = new_client.DIRECCION_1,
                                    reference      = new_client.EXTRA_2,
                                    date           = cambio.FECHA
                                }).ToList();
                    return model;
                }
                catch (Exception e)
                {
                    string mjs = "Error " + e;
                    Generate_Exception(mjs);
                }
            }
            return new List<Changed_Client>();
        }

        public List<Client> Get_Membership_Without_Mainteinance(string MESES = "")
        {
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            try
            {
                var model = (from client in connection.F_SGA_AFILIADOS_SIN_MANTENIMIENTO(MESES)
                             select new Client
                             {
                                 membership_number = client.N_AFILIADO,
                                 name              = client.NOMBRE,
                                 address           = client.DIRECCION,
                                 local_name        = client.NEGOCIO,
                                 phone             = client.TELEFONO,
                                 province          = client.PROVINCIA,
                                 canton            = client.CANTON,
                                 district          = client.DISTRITO
                             }).ToList();
                return model;
            }
            catch (Exception e) {
                Generate_Exception("Error " + e);
            }
            return new List<Client>();
        }

        //execute the store procedure that create and change an old membership to new one
        private void Change_Membership_Sp(string OLD_AFILIADO = "", string N_AFILIADO = "", string CONTACTO = "", string COMERCIO = "", string TELEFONO = "", string PROVINCIA = "",
                                            string CANTON = "", string DISTRITO = "", string DIRECCION = "", string REFERENCIA = "")
        {
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            connection.SGA_CAMBIAR_AFILIADO(OLD_AFILIADO, N_AFILIADO, CONTACTO, COMERCIO, TELEFONO, PROVINCIA, CANTON, DISTRITO, DIRECCION, REFERENCIA);
            connection.SaveChanges();
        }

        //cantones, distritos y provincias del pais
        private List<WEB_Provincia> Get_Provincias()
        {
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            var model = (from provincia in connection.WEB_Provincia
                         orderby provincia.nombreProvincia
                         select provincia).ToList();
            return model;
        }

        private List<WEB_Canton> Get_Cantones()
        {
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            var model = (from canton in connection.WEB_Canton
                         orderby canton.nombreCanton
                         select canton).ToList();
            return model;
        }

        private List<WEB_Distrito> Get_Distritos()
        {
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            var model = (from distrito in connection.WEB_Distrito
                         orderby distrito.nombreDistrito
                         select distrito).ToList();
            return model;
        }
        #endregion

        #region extra
        private SqlConnection Get_Connection()
        {
            SqlConnection conn = new SqlConnection("Data Source=10.3.9.2;Initial Catalog=BMS_DATA_DENT;Persist Security Info=True;User ID=web2;Password=CoCoCoWeb2;MultipleActiveResultSets=True;Application Name=EntityFramework");
            return conn;
        }

        private void Generate_Exception(string detail="")
        {
            Exceptions_Management ex = new Exceptions_Management();
            ex.Create_Exception(1, detail);
        }
        #endregion
    }
}