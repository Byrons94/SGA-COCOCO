using Gestion_Activos.Models.Commons;
using Gestion_Activos.Models.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gestion_Activos.Models;
using System.Web.Script.Serialization;

namespace Gestion_Activos.Controllers
{
    [Autenticado]
    public class DashboardController : Controller
    {
        // GET: Dashboard
        public ActionResult Index()
        {
            return View(Get_Info_Dashboard());
        }

        private Dashboard Get_Info_Dashboard()
        {
            TicketController tc = new TicketController();
            UserController uc = new UserController();
            User us = uc.Get_User_By_Id(SessionHelper.GetUser());
            string cod_tec = "";
            bool filtrar = false;
            if (us.cod_tec != "")
            {
                cod_tec = us.cod_tec;
                filtrar = true;
            }
            List<Ticket> listP = tc.Get_Tickets_By_Type("P", cod_tec, filtrar);
            Dashboard dash = new Dashboard();
            dash.install  = Get_Tycket_By_Type(listP, "INSTALACION");
            dash.visite   = Get_Tycket_By_Type(listP, "VISITA");
            dash.retire   = Get_Tycket_By_Type(listP, "RETIRO");
            dash.open     = listP;
          
            return dash;
        }

        [HttpPost]
        public JsonResult Get_History_Tickets()
        {
            DateTime finish     = Get_Today();
            DateTime start      = Get_Today_Less_7_Days();
            UserController uc   = new UserController();
            TicketController tc = new TicketController();
            User us = uc.Get_User_By_Id(SessionHelper.GetUser());
            string cod_tec = "";
            if (us.cod_tec != "")
            {
                cod_tec = us.cod_tec;
            }
            List<History_Tickets_Dashboard> ls = tc.Get_CLoset_Tickets_By_Date(cod_tec, 7, "ENTREGA");
            List<History_Tickets_Dashboard> lc = tc.Get_CLoset_Tickets_By_Date(cod_tec, 7, "CREADO");
            List<List<History_Tickets_Dashboard>> array = new List<List<History_Tickets_Dashboard>>();
            array.Add(ls);
            array.Add(lc);
            string data = "";
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            data = serializer.Serialize(array);
            return Json(data);
        }
        
        private int Get_Tycket_By_Type(List<Ticket> list = null, string type = "")
        {
            int total = 0;
            List<Ticket> litsR = new List<Ticket>();
            if (list != null && type != "")
            {
                foreach (var ticket in list)
                {
                    if (ticket.type.Trim() == type)
                    {
                        total++;
                    }
                }
            }
            return total;
        }

        private DateTime Get_Today()
        {
            return DateTime.Now;
        }

        private DateTime Get_Today_Less_7_Days()
        {
            return DateTime.Today.AddDays(-7);
        }

        private void Generate_Exception(string detail = "")
        {
            Exceptions_Management ex = new Exceptions_Management();
            ex.Create_Exception(2, detail);
        }
    }
}