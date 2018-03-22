using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gestion_Activos.Models.Class;
using Gestion_Activos.Models;
using Gestion_Activos.Models.Commons;
using System.Web.Script.Serialization;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Helpers;

namespace Gestion_Activos.Controllers
{
    [Autenticado]
    public class Inventory_MovementController : Controller
    {
      
        ProductController pcontroller = null;
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Get_Inventory()
        {
            DateTime fecha = DateTime.Now;
            string mes     = (fecha.Month < 10) ? ("0"+ fecha.Month.ToString()): (fecha.Month.ToString());
            string year    = fecha.Year.ToString();
            List <Product_Inventory> inventory = Get_Inventory_By_Search(mes, year, "", "1"); 
            return View(inventory);
        }

        [HttpPost]
        public JsonResult Search_Inventory_By_Parameter(string filtro="", string tipo = "")
        {
            DateTime fecha = DateTime.Now;
            string mes = (fecha.Month < 10) ? ("0" + fecha.Month.ToString()) : (fecha.Month.ToString());
            string year = fecha.Year.ToString();
            string data = "";
            List<Product_Inventory> inventory = Get_Inventory_By_Search(mes, year, filtro, tipo);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            data = serializer.Serialize(inventory);
            return Json(data);
        }

        public List<Inventory_Movement> Get_Movements_By_Id_Ticket(string Id_Ticket = "0")
        {
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            Inicialice_Controllers();
            List<Inventory_Movement> list_movements = null;
           //en caso de que sea un campo diferente cambiar le adi8
            var model = (from movi in connection.INV_MOV1
                         where movi.CIA == "003"
                          &&   movi.ADI7 == Id_Ticket
                          &&   movi.NULO != "S"
                         select movi).ToList();
            if (model.Count > 0)
            {
                list_movements = new List<Inventory_Movement>();
                foreach (var movement in model)
                {
                    Inventory_Movement Inv_mov = new Inventory_Movement
                    {
                        id          = (int)movement.CONSECUTIVO,
                        num_move    = movement.NUM_DOC,
                        type        = movement.COD_TIPO
                    };
                    Inv_mov.products_list = pcontroller.Get_Products_By_Mov(Inv_mov.num_move);
                    list_movements.Add(Inv_mov);
                }
            }
            return list_movements;
        }

        private List<Product_Inventory> Get_Inventory_By_Search(string MES = "", string ANO = "", string PARAMETRO="", string P_CONSULTA="")
        {
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            var model = (from product in connection.F_INVENTARIO_CREDOMATIC(MES, ANO, PARAMETRO, P_CONSULTA)
                         select new Product_Inventory
                         {
                             cod_prod      = product.COD_PROD,
                             description   = product.DESCRIPCION,
                             balance_start = product.SALDO_INI.Value,
                             balance_end   = product.SALDO_FIN.Value,
                             bought        = product.COMPRADOS.Value,
                             retired       = product.RETIRADOS.Value,
                             reconditioned = product.REACONDI != null? product.REACONDI.Value : 0,
                             installed     = product.INSTALADOS.Value,
                             discarded     = product.DESECHADO.Value
                         }).ToList();
            return model;
        } 

        private void Inicialice_Controllers()
        {
            if (pcontroller == null)
            {
                pcontroller = new ProductController();
            }
        }

        private void Generate_Exception(string detail = "")
        {
            Exceptions_Management ex = new Exceptions_Management();
            ex.Create_Exception(3, detail);
        }
    }
}