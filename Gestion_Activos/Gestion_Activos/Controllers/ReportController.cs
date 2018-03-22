using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Gestion_Activos.Models;
using Gestion_Activos.Models.Class;
using System.Globalization;
using Gestion_Activos.Models.Commons;
using System.Web.Script.Serialization;

namespace Gestion_Activos.Controllers
{
    [Autenticado]
    public class ReportController : Controller
    {
        #region views
        //GENERATES REPORT TO SHOW INSTALLED PRODUCT
        public ActionResult Get_Installations_By_Dates(string date_start = "", string date_end = "", string t_estatus = "")
        {
            DateTime F_INICIO = DateTime.Now;
            DateTime F_FIN    = DateTime.Now;

            if (date_start == "" && date_end == "")
            {
                F_INICIO = DateTime.Now.AddDays(-7);
            }
            else
            {
                string format = "MM/dd/yyyy";
                F_INICIO = DateTime.ParseExact(date_start, format, CultureInfo.InvariantCulture);
                F_FIN = DateTime.ParseExact(date_end, format, CultureInfo.InvariantCulture);
            }
            ClientController cc = new ClientController();
            ProductController pc = new ProductController();
            
            List<Client> listClientes = cc.Get_List_Installed(F_INICIO, F_FIN, t_estatus);
            List<Serial> listProducts = pc.Get_Serials_Installed(F_INICIO, F_FIN);
            List<string> listCategories = pc.Get_Product_Category();
            if (listClientes != null)
            {
                foreach (var client in listClientes)
                {
                    string cod_clie = client.membership_number;
                    client.products_intalled = Get_Products_By_Client(cod_clie, listProducts, listCategories);
                    client.categories_products = listCategories;
                }
            }
            TempData["f_inicio"]    = F_INICIO.ToString("MM/dd/yyyy");
            TempData["f_fin"]       = F_FIN.ToString("MM/dd/yyyy");
            TempData["cmb_estatus"] = t_estatus;
            return View(listClientes);
        }

        
        //metodo para descargar excel con todas las instalaciones, NO USAR NUNCA...
        public ActionResult Get_Total_Info_Test()
        {
            ClientController  cc = new ClientController();
            ProductController pc = new ProductController();

            List<Client> listClientes   = cc.Get_List_Clients_Active();
            List<string> listCategories = pc.Get_Product_Category();

            if (listClientes != null)
            {
                foreach (var client in listClientes)
                {
                    string cod_clie = client.membership_number;
                    client.inventary = pc.Products_Inventory_By_Client(cod_clie);
                    client.categories_products = listCategories;
                }
            }
            return View(listClientes);
        }

        public ActionResult Get_Report_Maintenance_Clients()
        {
            DateTime F_INICIO = DateTime.Now.AddDays(-7);
            DateTime F_FIN = DateTime.Now;
           
            ClientController cc  = new ClientController();
            TempData["f_inicio"] = F_INICIO.ToString("MM/dd/yyyy");
            TempData["f_fin"]    = F_FIN.ToString("MM/dd/yyyy");
            List<Client> list    = cc.Get_Clients_Maintenances(F_INICIO, F_FIN);
            return View(list);
        }


        //get all instaled inventary from clients.
        public ActionResult Get_All_Client_Inventary()
        {
            ClientController  cc = new ClientController();
            ProductController pc = new ProductController();

            List<Client> listClientes   = cc.Get_List_Clients_Active();
            List<string> listCategories = pc.Get_Product_Category();
            List<Serial> products       = pc.Get_Products_Inventary_Clients();

            if (listClientes != null)
            {
                foreach (var client in listClientes)
                {
                    string cod_clie             = client.membership_number;
                    client.products_intalled    = Get_Products_By_Client(cod_clie, products, listCategories);
                    client.categories_products  = listCategories;
                }
            }
            return View(listClientes);
        }


        //obtinene los tickets entre fechas para generar el reporte. 
        public ActionResult Get_Events_By_Dates()
        {
            DateTime F_INICIO   = DateTime.Now.AddDays(-7);
            DateTime F_FIN      = DateTime.Now;
            TicketController tc = new TicketController();
            List<Ticket> list   = tc.Get_Tickets_By_Dates(F_INICIO, F_FIN, "E", "C", "I", "T");
            TempData["f_inicio"] = F_INICIO.ToString("MM/dd/yyyy");
            TempData["f_fin"]     = F_FIN.ToString("MM/dd/yyyy");
            return View(list);
        }


        public ActionResult Get_Products_Movements()
        {
            DateTime F_INICIO    = DateTime.Now.AddDays(-7);
            DateTime F_FIN       = DateTime.Now;
            ProductController pc = new ProductController();
            
            List<Product_Movement> products = pc.Get_Product_Movements(F_INICIO, F_FIN, "", "");
            TempData["categories"]          = pc.Get_Product_Category();
            TempData["f_inicio"]            = F_INICIO.ToString("MM/dd/yyyy");
            TempData["f_fin"]               = F_FIN.ToString("MM/dd/yyyy");
            return View(products);
        }


        public ActionResult Get_Report_Client_Changed()
        {
            DateTime F_INICIO = DateTime.Now.AddDays(-7);
            DateTime F_FIN    = DateTime.Now;

            TempData["f_inicio"] = F_INICIO.ToString("MM/dd/yyyy");
            TempData["f_fin"]    = F_FIN.ToString("MM/dd/yyyy");
            ClientController cc  = new ClientController();
            List<Changed_Client> list = cc.Get_List_Change_Client(F_INICIO, F_FIN);
            return View(list);
        }
        #endregion views
        

        #region Jsons
        [HttpPost]
        public JsonResult Get_Installations_By_Dates_Json(string date_start = "", string date_end = "")
        {
            DateTime F_INICIO = DateTime.Now;
            DateTime F_FIN    = DateTime.Now;
            if (date_start == "" && date_end == "")
            {
                F_INICIO = DateTime.Now.AddDays(-7);
                F_FIN = DateTime.Now;
            }
            else
            {
                string format = "MM/dd/yyyy";
                F_INICIO = DateTime.ParseExact(date_start, format,  CultureInfo.InvariantCulture);
                F_FIN = DateTime.ParseExact(date_end, format, CultureInfo.InvariantCulture);
            }
            ClientController cc = new ClientController();
            ProductController pc = new ProductController();

            List<Client> listClientes = cc.Get_List_Installed(F_INICIO, F_FIN);
            List<Serial> listProducts = pc.Get_Serials_Installed(F_INICIO, F_FIN);
            List<string> listCategories = pc.Get_Product_Category();
            if (listClientes != null)
            {
                foreach (var client in listClientes)
                {
                    string cod_clie = client.membership_number;
                    client.products_intalled = Get_Products_By_Client(cod_clie, listProducts, listCategories);
                    client.categories_products = listCategories;
                }
            }
            return Json(listClientes);
        }

        //se extraen todos los clientes a los cuales se les ha realizado una visita de mantenimiento para el reporte
        [HttpPost]
        public JsonResult Get_Report_Maintenance_Clients_Json(string date_start = "", string date_end = "")
        {
            DateTime F_INICIO = DateTime.Now;
            DateTime F_FIN    = DateTime.Now;
            if (date_start == "" && date_end == "")
            {
                F_INICIO = DateTime.Now.AddDays(-7);
                F_FIN = DateTime.Now;
            }
            else
            {
                string format = "MM/dd/yyyy";
                F_INICIO = DateTime.ParseExact(date_start, format, CultureInfo.InvariantCulture);
                F_FIN = DateTime.ParseExact(date_end, format, CultureInfo.InvariantCulture);
            }
            ClientController cc = new ClientController();
            List<Client> list = cc.Get_Clients_Maintenances(F_INICIO, F_FIN);
            return Json(list);
        }

        [HttpPost] // return json with those clients without a recently mainteinance 
        public ActionResult Get_Report_Not_Maintenance_Clients_Json(string meses="")
        {
            ClientController cc = new ClientController();
            List<Client> list   = cc.Get_Membership_Without_Mainteinance(meses);
            var serializer = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
            var result = new ContentResult
            {
                Content = serializer.Serialize(list),
                ContentType = "application/json"
            };
            return result;
        }


        [HttpPost] 
        public JsonResult Get_Report_Client_Changed_Json(string date_start = "", string date_end = "")
        {
            DateTime F_INICIO = DateTime.Now;
            DateTime F_FIN    = DateTime.Now;
            if (date_start == "" && date_end == "")
            {
                F_INICIO = DateTime.Now.AddDays(-7);
                F_FIN = DateTime.Now;
            }
            else
            {
                string format = "MM/dd/yyyy";
                F_INICIO = DateTime.ParseExact(date_start, format, CultureInfo.InvariantCulture);
                F_FIN = DateTime.ParseExact(date_end, format, CultureInfo.InvariantCulture);
            }
            ClientController cc = new ClientController();
            List<Changed_Client> list = cc.Get_List_Change_Client(F_INICIO, F_FIN);
            return Json(list);
        }


        //se extraen todos los tickets entre fechas que han sido cerrados para el reporte
        [HttpPost]
        public JsonResult Get_Events_By_Dates_Json(string date_start = "", string date_end = "", string filter = "", string t_fecha = "", string t_movimiento="", string t_estatus="")
        {
            DateTime F_INICIO = DateTime.Now;
            DateTime F_FIN    = DateTime.Now;
            if (date_start == "" && date_end == "")
            {
                F_INICIO = DateTime.Now.AddDays(-7);
                F_FIN = DateTime.Now;
            }
            else
            {
                string format = "MM/dd/yyyy";
                F_INICIO = DateTime.ParseExact(date_start, format, CultureInfo.InvariantCulture);
                F_FIN = DateTime.ParseExact(date_end, format, CultureInfo.InvariantCulture);
            }
            TicketController tc = new TicketController();
            List<Ticket> list = tc.Get_Tickets_By_Dates(F_INICIO, F_FIN, filter, t_fecha, t_movimiento, t_estatus);
            return Json(list);
        }


        public JsonResult Get_Products_Movements_Json(string date_start = "", string date_end = "", string categorias = "", string movements="")
        {
            DateTime F_INICIO = DateTime.Now;
            DateTime F_FIN    = DateTime.Now;
            if (date_start == "" && date_end == "")
            {
                F_INICIO = DateTime.Now.AddDays(-7);
                F_FIN    = DateTime.Now;
            }
            else
            {
                string format = "MM/dd/yyyy";
                F_INICIO      = DateTime.ParseExact(date_start, format, CultureInfo.InvariantCulture);
                F_FIN         = DateTime.ParseExact(date_end, format, CultureInfo.InvariantCulture);
            }
            var serializer = new JavaScriptSerializer();

            List<string> categories = serializer.Deserialize<List<string>>(categorias);
            categorias = (categories != null) ? string.Join(",", categories) : "";
            
            List<string> movementList = serializer.Deserialize<List<string>>(movements);
            movements = (movementList != null) ? string.Join(",", movementList) : "";
            
            ProductController pc = new ProductController();
            List<Product_Movement> products = pc.Get_Product_Movements(F_INICIO, F_FIN, categorias, movements);
            return Json(products);
        }
        #endregion

        #region logic
        private Dictionary<string, List<string>> Get_Products_By_Client(string cod_clie ="", List<Serial> listSerial = null, List<string> categories = null)
        {
            bool existe = false;
            Dictionary<string, List<string>> categorias = Categories_List(categories);
            foreach (var serial in listSerial)
            {
                string reference = serial.reference;
                if (cod_clie.Trim() == reference.Trim())
                {
                    if (serial.category != null) { 
                        string serial_cat = serial.category.Trim();
                        foreach (var categoria in categorias)
                        {
                            if (categoria.Key.Trim() == serial_cat.Trim())
                            {
                                categoria.Value.Add(serial.serial);
                                existe = true;
                                //listSerial.Remove(serial);
                            }
                        }
                    }
                }
            }
            return (existe) ? categorias : new Dictionary<string, List<string>>();             
        }

        //generate array products to client
        private Dictionary<string, List<string>> Categories_List(List<string> categories = null)
        {
            Dictionary<string, List<string>> list_client =  new  Dictionary<string, List<string>>();
            foreach (var category in categories)
            {
                list_client.Add(category, new List<string>());
            }
            return list_client;
        }
        #endregion
    }
}