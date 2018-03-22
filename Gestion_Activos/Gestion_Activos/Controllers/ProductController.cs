using Gestion_Activos.Models;
using Gestion_Activos.Models.Class;
using Gestion_Activos.Models.Commons;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Gestion_Activos.Controllers
{
    [Autenticado]
    public class ProductController : Controller
    {
        // GET: Product
        public ActionResult Index()
        {
            return View();
        }

        public List<Product> Get_Products_By_Mov(string Id_Move = "0")
        {
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            var model = (from seri in connection.INV_MOVS
                         join prod in connection.INV_PROD
                          on new { x = seri.CIA, y = seri.COD_PROD } equals new { x = prod.CIA, y = prod.COD_PROD }
                          into UnionValues
                         from product in UnionValues.DefaultIfEmpty()
                         where seri.NUM_DOC == Id_Move &&
                              seri.CIA == "003"
                         select new Product {
                             id = seri.CONSECUTIVO,
                             cod_prod = seri.COD_PROD,
                             description = product.NOMBRE_1,
                             serial_number = seri.SERIE,
                             category      = product.NOTAS
                         }).ToList();

            return model;
        }

        public ActionResult Get_Serial()
        {
            return View(Get_Last_10());
        }

        public ActionResult Get_Serial_Details(string serial = "")
        {
            Serial serie = null;
            if (serial != "")
            {
                serie = Get_Serial_By_Id(serial.Trim());
                if (serie != null)
                {
                    serie.serial_log = Get_Log_By_Serial(serie.serial.Trim());
                }
            }
            if (serie != null)
            {
                return View(serie);
            }
            return View();
        }

        public ActionResult Get_Serial_Changed(string date_start = "", string date_end = "")
        {
            List<Serial_Log> list_log = null;
            if (date_start != "" && date_end != "")
            {
                DateTime start = DateTime.Parse(date_start);
                DateTime end   = DateTime.Parse(date_end);
                list_log       = Get_Log_Serial_Changed_Dates(start, end);
            }
            else
            {
                DateTime now = DateTime.Now;
                var start    = new DateTime(now.Year, now.Month, 1);
                list_log     = Get_Log_Serial_Changed_Dates(start, now);
            }
            return View(list_log);
        }


        //call External_Services class to validate some serials and return Json data
        public JsonResult Call_External_Service(string tipo="", string series="", string afiliado="")
        {
            var serializer = new JavaScriptSerializer();
            External_Services es = new External_Services();
            Dictionary<String, String> list_call_service = new Dictionary<string, string>();

            if (tipo == "1") //serial pc track installation
            {
                List<Product> products = serializer.Deserialize<List<Product>>(series);
                foreach (var product in products)
                {
                    var serial = product.serial_number;
                    list_call_service.Add(serial, es.Call_Service(tipo, serial, afiliado));
                }
            }
            else if (tipo == "2") //serial pc track change
            {
                List<Changed_Serial> products = serializer.Deserialize<List<Changed_Serial>>(series);
                foreach (var product in products)
                {
                    if (product.category == "CPU"  || product.category == "BRIX")
                    {
                        string old_serial = product.old_serial;
                        string new_serial = product.new_serial;
                        list_call_service.Add(new_serial, es.Call_Service("1", new_serial, afiliado)); //instalacion por cambio de equipo
                        es.Call_Service("3", old_serial, afiliado);                                    //retiro de equipo por cambio
                    }
                }
            }
            else if (tipo == "3") //serial pc track retire
            {
                List<Product> products = serializer.Deserialize<List<Product>>(series);
                foreach (var product in products)
                {
                    if (product.category == "CPU" || product.category == "BRIX")
                    {
                        var serial = product.serial_number;
                        es.Call_Service(tipo, serial, afiliado);
                    }
                }
            }
            return Json(list_call_service);
        }
        
        #region jsons
        [HttpPost]
        public JsonResult Update_Product_Info(string id_boleta = "", string codigos = null, string series_c = null)
        {
            bool retorno = Change_Serials(series_c);
            var mjs      = "";
            if (Utilities.IsNumeric(id_boleta) && codigos != null)
            {
                var serializer = new JavaScriptSerializer();
                List<Product> products = serializer.Deserialize<List<Product>>(codigos);
                mjs = Insert_State_Products(id_boleta, products);
            }
            else
            {
                mjs = "Error"; //redireccionar a pagina de error
            }
            return Json(mjs);
        }


        [HttpPost]
        public JsonResult Update_Serials(string series_c = null)
        {
            string mjs = "Successful";
            bool retorno = false;
            if (series_c != null)
            {
                retorno = Change_Serials(series_c);
            }
            mjs = (retorno) ? mjs : "Error";
            return Json(mjs);
        }


        private bool Change_Serials(string series_c = null)
        {
            bool retorno = true;
            var serializer = new JavaScriptSerializer();
            List<Changed_Serial> products = serializer.Deserialize<List<Changed_Serial>>(series_c);
            if (products.Count > 0)
            {
                retorno = Change_Serial_Query(products);
            }
            return retorno;
        }

        [HttpPost]
        public JsonResult Validate_Serial(string n_serie_verificar = "", string n_boleta = "")
        {
            string mjs = "Error al cambiar series";
            //if (n_serie_verificar == "" || n_boleta == "")
            if (n_serie_verificar == "")
            {
                mjs = "Series en blanco no estan permitidas";
            }
            else
            {
                SqlConnection conn = Get_Connection();
                using (SqlCommand cmd = new SqlCommand("SELECT dbo.SGA_F_STATUS_SERIES(@serie_validar,@num_ticket)", conn))
                {
                    SqlParameter parm1 = new SqlParameter("@serie_validar", SqlDbType.VarChar);
                    SqlParameter parm2 = new SqlParameter("@num_ticket", SqlDbType.VarChar);
                    parm1.Value = n_serie_verificar;
                    parm2.Value = n_boleta;
                    cmd.Parameters.Add(parm1);
                    cmd.Parameters.Add(parm2);
                    conn.Open();
                    var result = cmd.ExecuteScalar();
                    mjs = result.ToString();
                    conn.Close();
                }
            }
            return Json(mjs);
        }

        [HttpPost]
        public JsonResult Delete_From_Inventary(string series_c = null, string cod_afi = null)
        {
            string mjs = "Successful";
            if (series_c != null && cod_afi != null)
            {
                UserController uc = new UserController();
                string userName = uc.Get_User_By_Id(SessionHelper.GetUser()).login_name;
                string[] codigos = JsonConvert.DeserializeObject<string[]>(series_c);
                foreach (var code in codigos)
                {
                    Change_Status_Serial_Client(cod_afi, code);
                    Write_Serial_Log(4, code, "", "", userName);
                }
            }
            return Json(mjs);
        }

        [HttpPost]
        public JsonResult Get_Serial_By_Search(string search = "")
        {
            string data = "";
            if (search != "")
            {
                List<Serial> list = Get_Serial_By_Search_Query(search);
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                data = serializer.Serialize(list);
            }
            return Json(data);
        }

        [HttpPost]
        public JsonResult Get_Serial_Changed_J(string date_start = "", string date_end = "")
        {
            List<Serial_Log> list_log = null;
            if (date_start != "" && date_end != "")
            {
                DateTime start = DateTime.ParseExact(date_start, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                DateTime end   = DateTime.ParseExact(date_end, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                list_log = Get_Log_Serial_Changed_Dates(start, end);
            }
            else
            {
                DateTime now = DateTime.Now;
                var start = new DateTime(now.Year, now.Month, 1);
                list_log = Get_Log_Serial_Changed_Dates(start, now);
            }
            string data = "";
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            data = serializer.Serialize(list_log);
            return Json(data);
        }
        #endregion


        #region querys 
        private string Insert_State_Products(string id_boleta, List<Product> product_list)
        {
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            string result = "Success";
            DateTime now = DateTime.Now;
            foreach (var product in product_list)
            {
                SGA_ESTADO_PRODUCTOS estado = new SGA_ESTADO_PRODUCTOS()
                {
                    COD_PROD            = product.cod_prod,
                    SERIE_CONSECUTIVO   = product.id,
                    ESTADO              = product.state,
                    ID_BOLETA           = id_boleta,
                    FECHA               = now,
                    ACCION              = product.action
                };
                connection.SGA_ESTADO_PRODUCTOS.Add(estado);
            }
            try
            {
                connection.SaveChanges();
                result = "Success";
            }
            catch (Exception e)
            {
                result = "Error " + e;
                Generate_Exception(result);
            }
            return result;
        }
        

        private bool Change_Serial_Query(List<Changed_Serial> list = null)
        {
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            bool success = true;
            UserController uc = new UserController();
            string userName = uc.Get_User_By_Id(SessionHelper.GetUser()).login_name;
            if (list != null)
            {
                foreach (var serial in list)
                {
                    Write_Serial_Log(3, serial.old_serial, serial.new_serial, "", userName);
                    int consecutive = serial.consecutive;
                    var query = (from seri in connection.INV_MOVS
                                 where seri.CONSECUTIVO == consecutive
                                 select seri);
                    foreach (INV_MOVS movi in query)
                    {
                        movi.NOTAS = serial.old_serial.Trim();
                        movi.SERIE = serial.new_serial.Trim();
                    }
                    try
                    {
                        connection.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        success = false;
                        string result = "Error " + e;
                        Generate_Exception(result);
                    }
                }
            }
            return success;
        }

        public List<Product> Products_Inventory_By_Client(string id_client = "")
        {
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            if (id_client != "")
            {
                var data = (from product in connection.F_INVENTARIO_AFILIADO(id_client)
                            select new Product
                            {
                                id              = product.ID.Value,
                                cod_prod        = product.COD_PROD,
                                description     = product.DESCRIPCION,
                                serial_number   = product.SERIE,
                                category        = (product.CATEGORIA != "") ? product.CATEGORIA : "ADICIONALES"
                            }).ToList();
                if (data.Count > 0)
                {
                    return data;
                }
            }
            return null;
        }

        public List<Product> Products_Log_Ticket(string id_ticket = "")
        {
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            if (id_ticket != "")
            {
                var data = (from state in connection.SGA_ESTADO_PRODUCTOS
                            join prod in connection.INV_PROD
                            on new { x = "003", y = state.COD_PROD } equals new { x = prod.CIA, y = prod.COD_PROD }
                            join seri in connection.INV_MOVS
                            on state.SERIE_CONSECUTIVO equals seri.CONSECUTIVO
                            where state.ID_BOLETA == id_ticket
                            select new Product
                            {
                                id = state.ID,
                                cod_prod = state.COD_PROD,
                                serial_number = seri.SERIE,
                                description = prod.NOMBRE_1,
                                state = state.ESTADO,
                                action = state.ACCION != null ? state.ACCION: "N/A",
                                category = prod.NOTAS,
                                serial_2 = seri.NOTAS
                            });
                if (data != null || data.Count() > 0)
                {
                    return data.ToList();
                }
            }
            return new List<Product>();
        }

        private void Change_Status_Serial_Client(string num_afiliado = "", string serie_consecutivo = "")
        {
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            if (num_afiliado != "" && serie_consecutivo != "")
            {
                connection.SGA_ACTUALIZAR_ESTATUS_PRODUCTO_AFILIADO(num_afiliado, serie_consecutivo);
                connection.SaveChanges();
            }
        }

        private void Write_Serial_Log(int type = 0, string serial1 = "", string serial2 = "", string num_ticket = "", string responsable = "")
        {
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            if (serial1 != "")
            {
                string detail = Utilities.Msj_Log_Serial(type, serial1, serial2);
                DateTime now = DateTime.Now;
                SGA_BITACORA_SERIE log = new SGA_BITACORA_SERIE()
                {
                    SERIAL = serial1,
                    DETALLE = detail,
                    FECHA = now,
                    NUM_TICKET = num_ticket,
                    RESPONSABLE = responsable
                };
                connection.SGA_BITACORA_SERIE.Add(log);
            }
            try
            {
                connection.SaveChanges();
            }
            catch (Exception e)
            {
                Generate_Exception("Error " + e);
            }
        }

        private List<Serial> Get_Last_10()
        {
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            var model = (from serial in connection.INV_MOVS
                         join mov in connection.INV_MOV1
                          on new { x = serial.CIA, y = serial.NUM_DOC, z = serial.COD_TIPO } equals new { x = mov.CIA, y = mov.NUM_DOC, z = mov.COD_TIPO }
                         where serial.CIA == "003" && serial.SERIE.Trim() != "---SIN SERIE---" && serial.SERIE.Trim() != ""
                         orderby serial.CONSECUTIVO descending
                         select new Serial {
                             id = serial.CONSECUTIVO.ToString(),
                             serial = serial.SERIE,
                             state = serial.COD_TIPO == "01" ? "Compra" :
                                          serial.COD_TIPO == "02" ? "Salida para instalación" :
                                          serial.COD_TIPO == "03" ? "Entrada por retiro" :
                                          serial.COD_TIPO == "04" ? "Salida para repuesto" :
                                          serial.COD_TIPO == "05" ? "Entrada equipo de repuesto" :
                                          serial.COD_TIPO == "06" ? "Salida a desecho" : "No dispobible",
                             date = mov.FECHA_DIG,
                             other = mov.USUARIO
                         }).Take(10);
            return model.ToList();
        }

        private List<Serial> Get_Serial_By_Search_Query(string BUSQUEDA = "")
        {
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            var data = (from serial in connection.F_BUSQUEDA_SERIE(BUSQUEDA)
                        select new Serial
                        {
                            id = serial.CONSECUTIVO,
                            serial = serial.SERIE,
                            state = serial.ESTADO,
                            date = serial.FECHA.Value,
                            other = serial.USUARIO
                        }).ToList();
            return data;
        }

        private Serial Get_Serial_By_Id(string serial_p = "")
        {
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            var model = (from serial in connection.INV_MOVS
                         join mov in connection.INV_MOV1
                         on new { x = serial.CIA, y = serial.NUM_DOC, z = serial.COD_TIPO } equals new { x = mov.CIA, y = mov.NUM_DOC, z = mov.COD_TIPO }
                         join product in connection.INV_PROD
                         on new { x = serial.CIA, y = serial.COD_PROD } equals new { x = product.CIA, y = product.COD_PROD }
                         where serial.CIA == "003" && serial.SERIE.Trim() == serial_p
                         orderby serial.CONSECUTIVO descending
                         select new Serial
                         {
                             id = serial.CONSECUTIVO.ToString(),
                             serial = serial.SERIE,
                             state = serial.COD_TIPO == "01" ? "Compra" :
                                          serial.COD_TIPO == "02" ? "Salida para instalación" :
                                          serial.COD_TIPO == "03" ? "Entrada por retiro" :
                                          serial.COD_TIPO == "04" ? "Salida para repuesto" :
                                          serial.COD_TIPO == "05" ? "Entrada equipo de repuesto" :
                                          serial.COD_TIPO == "06" ? "Salida a desecho" : "No dispobible",
                             date = mov.FECHA_DIG,
                             other = mov.USUARIO,
                             product_name = product.NOMBRE_1,
                             physical_state = serial.COD_TIPO.Contains("01") ? "Nueva" : "Usada"
                         }).Take(1);
            return model.FirstOrDefault();
        }


        private List<Serial_Log> Get_Log_By_Serial(string SERIAL = "")
        {
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            var model = (from log in connection.F_HISTORIAL_SERIE(SERIAL)
                         orderby log.FECHA ascending
                         select new Serial_Log
                         {
                             movement = log.MOVIMIENTO,
                             date = log.FECHA.Value,
                             user = log.RESPONSABLE,
                             details = log.DETALLE,
                             ticket = log.TIQUETE,
                             note = log.NOTAS
                         }).ToList();
            return model;
        }   


        private List<Serial_Log> Get_Log_Serial_Changed_Dates(DateTime? dates_start = null, DateTime? date_end = null)
        {
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            var model = (from log in connection.SGA_BITACORA_SERIE
                         where log.FECHA >= dates_start && log.FECHA <= date_end
                                && log.DETALLE.Contains("Cambio")
                         orderby log.FECHA ascending
                         select new Serial_Log
                         {
                             serial  = log.SERIAL,
                             date    = log.FECHA,
                             user    = log.RESPONSABLE,
                             details = log.DETALLE,
                             ticket  = log.NUM_TICKET
                         }).ToList();
            return model;
        }

        public List<string> Get_Product_Category()
        {
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            var model = (from category in connection.SGA_CATEGORY_PRODUCT
                         select category.NOMBRE_CATEGORIA).ToList();
            return model;   
        }


        public List<Serial> Get_Serials_Installed(DateTime? F_INICIO = null, DateTime? F_FINAL = null)
        {
            if (F_INICIO != null && F_FINAL != null)
            {
                BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
                try
                {
                    var model = (from serial in connection.F_PRODUCTOS_INSTALADOS(F_INICIO, F_FINAL)
                                 select new Serial
                                 {
                                     reference = serial.REFERENCIA,
                                     serial = serial.SERIE,
                                     category = (serial.CATEGORIA != "") ? serial.CATEGORIA : "ADICIONALES"
                                 }).ToList();
                    return model;
                }
                catch (Exception e)
                {
                    string mjs = "Error " + e;
                    Generate_Exception(mjs);
                }
            }
            return new List<Serial>();
        }


        public List<Serial> Get_Products_Inventary_Clients()
        {
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            List<Product> list = new List<Product>();
            var model = (from producto in connection.F_SGA_PRODUCTOS_POR_CLIENTE()
                         select new Serial {
                             category  = producto.CATEGORIA,
                             serial    = producto.N_SERIE,
                             reference = producto.COD_CLIE 
                        });
            return model.ToList();
        }

        public List<Product_Movement> Get_Product_Movements(DateTime ? F_INICIO = null, DateTime ? F_FIN = null, string CATEGORIA = "", string MOVEMENTS="")
        {
            string CIA = "003";
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            var model = (from movement in connection.F_MOVIMIENTOS_POR_PRODUCTOS(CIA, F_INICIO, F_FIN, CATEGORIA, MOVEMENTS)
                         select movement).ToList();
            List<Product_Movement> list = new List<Product_Movement>();
            foreach (var movement in model)
            {
                Product product = new Product
                {
                    cod_prod      = movement.COD_PROD,
                    category      = movement.CATEGORIA,
                    description   = movement.PRODUCTO,
                    serial_number = movement.SERIE
                };

                Product_Movement pv = new Product_Movement
                {
                    date_movement       = movement.FECHA_MOVI.Value,
                    local_name          = movement.NEGOCIO,
                    membership_number   = movement.AFILIADO,
                    num_doc             = movement.NUM_DOC,
                    type_movement       = movement.TIPO,
                    product             = product
                };
                list.Add(pv);
            }
            return list;
        }  

        private SqlConnection Get_Connection()
        {
            SqlConnection conn = new SqlConnection("Data Source=10.3.9.2;Initial Catalog=BMS_DATA_DENT;Persist Security Info=True;User ID=web2;Password=CoCoCoWeb2;MultipleActiveResultSets=True;Application Name=EntityFramework");
            return conn;
        }
        #endregion

        private void Generate_Exception(string detail = "")
        {
            Exceptions_Management ex = new Exceptions_Management();
            ex.Create_Exception(4, detail);
        }

    }
}