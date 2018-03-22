using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Gestion_Activos.Models;
using Gestion_Activos.Models.Class;
using Gestion_Activos.Models.Commons;
using System.Web.Script.Serialization;
using System.Globalization;

//clase que gestiona todos los tiquets y las series
namespace Gestion_Activos.Controllers
{
    /* P = Pendientes * R = Revisados  * E = Resueltos */
    [Autenticado]
    public class TicketController : Controller
    {
        DateTime fecha = new DateTime(2016, 6, 01); //delete this variable
        
        #region views
        [Permission(Permiso = Permissions_Enum.Visualizar_tickets)]
        public ActionResult Index()
        {
            UserController uc = new UserController();
            User us =  uc.Get_User_By_Id(SessionHelper.GetUser());
            string cod_tec = "";
            bool filtrar = false;
            if (us.cod_tec != "")
            {
                cod_tec = us.cod_tec;
                filtrar = true;
            }
            List<Ticket> list = Get_Tickets_By_Type("P", cod_tec, filtrar);
            return View(list.ToList());
        }

        public ActionResult Index_Attended()
        {
            UserController uc = new UserController();
            User us = uc.Get_User_By_Id(SessionHelper.GetUser());
            string cod_tec = "";
            bool filtrar = false;
            if (us.cod_tec != "")
            {
                cod_tec = us.cod_tec;
                filtrar = true;
            }
            List<Ticket> list = Get_Tickets_By_Type("R", cod_tec, filtrar);
            return View(list.ToList());
        }


        public ActionResult Closed()
        {
            UserController uc = new UserController();
            User us = uc.Get_User_By_Id(SessionHelper.GetUser());
            string cod_tec = "";
            bool filtrar = false;
            if (us.cod_tec != "")
            {
                cod_tec = us.cod_tec;
                filtrar = true;
            }
            DateTime date_end  = DateTime.Now;
            DateTime date_start = date_end.AddDays(-7);
            List<Ticket> list = Get_Closed_Recently(cod_tec, filtrar, date_start, date_end);
            TempData["f_inicio"] = date_start.ToString("MM/dd/yyyy");
            TempData["f_fin"] = date_end.ToString("MM/dd/yyyy");
            return View(list.ToList());
        }


        [Permission(Permiso = Permissions_Enum.Consultar_tickets)]
        public ActionResult Get_List_Tickets()
        {
            List<Ticket> list = Get_Last_10();
            return View(list);
        }
        

        public ActionResult Details(string id_ticket = "")
        {
            if (id_ticket != "")
            {
                Ticket tic = Get_Details_Ticket(id_ticket);
                if (tic != null)
                {
                    ClientController CC              = new ClientController();
                    Inventory_MovementController IMC = new Inventory_MovementController();
                    ProductController PC             = new ProductController();
                    tic.client                       = CC.Get_Client_By_Membership_Number(tic.membership_number);
                    tic.advances                     = Get_Advance_By_Id_Ticket(tic.id);

                    TempData["damages"]              = Get_Damages_List();//se obtienen las categorias daños de los productos
                    if (tic.client != null)
                    { 
                        if (tic.type == "INSTALACION")
                        {
                            tic.movemetns = IMC.Get_Movements_By_Id_Ticket(Convert.ToString(tic.id));
                            return View(tic);
                        }
                        else if (tic.type == "VISITA")
                        {
                            tic.client.inventary = PC.Products_Inventory_By_Client(tic.client.membership_number);
                            TempData["myObject"] = tic;
                            return RedirectToAction("Details_V", "Ticket");
                        }
                        else if (tic.type == "RETIRO")
                        {
                            tic.client.inventary = PC.Products_Inventory_By_Client(tic.client.membership_number);
                            TempData["myObject"] = tic;
                            return RedirectToAction("Details_R", "Ticket");
                        }
                    }
                }
            }
            return Redirect_Index(); 
        }
        
        public ActionResult Details_V()
        {
            Ticket location = null;
            if (TempData["myObject"] != null)
            {
                location = (Ticket)TempData["myObject"];
                return View(location);
            }
            return Redirect_Index();
        }
        
        public ActionResult Details_R()
        {
            Ticket location = null;
            if (TempData["myObject"] != null)
            {
                location = (Ticket)TempData["myObject"];
                return View(location);
            }
            return Redirect_Index();
        }
        
        [HttpPost] //cierra los tiquetes de instalaciones
        [Permission(Permiso = Permissions_Enum.Cerrar_ticket)]
        public ActionResult Close_Ticket_I(FormCollection collection = null)
        {
            if (collection != null)
            {
                string id_ticket            = Utilities.Elements_Form_Post(collection, "id_ticket");
                string tip_mov              = Utilities.Elements_Form_Post(collection, "tipo_mov");
                string chk_connection       = Utilities.Elements_Form_Post(collection, "chk_conecion");
                string chk_detalle_install  = Utilities.Elements_Form_Post(collection, "chk_detalle_install");
                string chk_exposicion_equip = Utilities.Elements_Form_Post(collection, "chk_exposicion_equip");
                string chk_otros            = Utilities.Elements_Form_Post(collection, "chk_otros");
                string observations         = Utilities.Elements_Form_Post(collection, "observaciones");
                string membershipNumber     = Utilities.Elements_Form_Post(collection, "membership_number");
                string local_name           = Utilities.Elements_Form_Post(collection, "local_name");
                string info_t               = membershipNumber + " " + local_name;
                //local_name
                string email                = Utilities.Elements_Form_Post(collection, "email");  
                DateTime now                = DateTime.Now;
                TicketLog ticketL = new TicketLog()
                {
                    id          = id_ticket,
                    type        = Get_Type_Tiquet(tip_mov),
                    connetion   = chk_connection,
                    instalation_detaill     = chk_detalle_install,
                    equipment_exposition    = chk_exposicion_equip,
                    others      = chk_otros,
                    extra_1     = observations,
                    date        = now
                };

                //actualiza el status del afiliado a actualizado
                ClientController cc = new ClientController();
                cc.Update_Client(membershipNumber);
                
                //acciones para cerrar el tiquet
                Insert_Log_Ticket(ticketL);
                Change_Status_Ticket(id_ticket, "R");
                Send_Email(id_ticket, email, info_t);
            }
            return Redirect_Index();
        }
        
        [HttpPost] //cierra los tiquetes de visitas
        [Permission(Permiso = Permissions_Enum.Cerrar_ticket)]
        public ActionResult Close_Ticket_V(FormCollection collection = null)
        {
            if (collection != null)
            {
                string id_ticket         = Utilities.Elements_Form_Post(collection, "id_ticket");
                string tip_mov           = Utilities.Elements_Form_Post(collection, "tipo_mov");
                string solucion          = Utilities.Elements_Form_Post(collection, "solucion");
                string diagnostico       = Utilities.Elements_Form_Post(collection, "diagnostico");
                string averia            = Utilities.Elements_Form_Post(collection, "averia");
                string chk_pruebas       = Utilities.Elements_Form_Post(collection, "chk_pruebas");
                string chk_mantenimiento = Utilities.Elements_Form_Post(collection, "chk_mantenimiento");
                string radio_polvo       = Utilities.Elements_Form_Post(collection, "radio_polvo");
                string radio_plaga       = Utilities.Elements_Form_Post(collection, "radio_plaga");
                string radio_voltaje     = Utilities.Elements_Form_Post(collection, "radio_voltaje");
                string radio_grasa       = Utilities.Elements_Form_Post(collection, "radio_grasa");
                string radio_humedad     = Utilities.Elements_Form_Post(collection, "radio_humedad");
                string radio_otra        = Utilities.Elements_Form_Post(collection, "radio_otra");
                string exposicion_otra   = Utilities.Elements_Form_Post(collection, "exposicion_otra");
                string e_faltante        = Utilities.Elements_Form_Post(collection, "e_faltante");

                string mantenimiento     = Utilities.Elements_Form_Post(collection, "chk_mantenimiento_general");
                string email             = Utilities.Elements_Form_Post(collection, "email");

                string exposicion = radio_polvo + ',' + radio_plaga + ',' + radio_voltaje + ',' + radio_grasa+','+ radio_humedad;
                exposicion = (radio_otra != "") ? (exposicion + ',' + radio_otra) : exposicion;

                string membershipNumber = Utilities.Elements_Form_Post(collection, "membership_number");
                string local_name = Utilities.Elements_Form_Post(collection, "local_name");
                string info_t = membershipNumber + " " + local_name;

                DateTime now = DateTime.Now;
                TicketLog ticketL = new TicketLog()
                {
                    id          = id_ticket,
                    type        = Get_Type_Tiquet( tip_mov),
                    damage      = averia,
                    diagnostic  = diagnostico,
                    solution    = solucion,
                    equipment_exposition    = exposicion,
                    equipment_exposition_2  = exposicion_otra,
                    mainmaintenance         = chk_mantenimiento,
                    tests       = chk_pruebas,
                    date        = now,
                    extra_2     = mantenimiento,
                    extra_3     = e_faltante
                };

                //acciones para cerrar el tiquet
                Insert_Log_Ticket(ticketL);
                Change_Status_Ticket(id_ticket, "R");
                Send_Email(id_ticket, email, info_t);
            }
            return Redirect_Index();
        }


        [HttpPost] //cierra los tiquetes de retiros
        [Permission(Permiso = Permissions_Enum.Cerrar_ticket)]
        public ActionResult Close_Ticket_R(FormCollection collection = null)
        {
            if (collection != null)
            {
                string id_ticket            = Utilities.Elements_Form_Post(collection, "id_ticket");
                string tip_mov              = Utilities.Elements_Form_Post(collection, "tipo_mov");
                string chk_exposicion_equip = Utilities.Elements_Form_Post(collection, "chk_exposicion_equip");
                string observations         = Utilities.Elements_Form_Post(collection, "observaciones");
                string email                = Utilities.Elements_Form_Post(collection, "email");
                string e_faltante           = Utilities.Elements_Form_Post(collection, "e_faltante");
                DateTime now                = DateTime.Now;

                string membershipNumber = Utilities.Elements_Form_Post(collection, "membership_number");
                string local_name = Utilities.Elements_Form_Post(collection, "local_name");
                string info_t = membershipNumber + " " + local_name;

                TicketLog ticketL = new TicketLog()
                {
                    id      = id_ticket,
                    type    = Get_Type_Tiquet(tip_mov),
                    equipment_exposition = chk_exposicion_equip,
                    extra_1 = observations,
                    date    = now,
                    extra_3 = e_faltante
                };

                //acciones para cerrar el tiquet
                Insert_Log_Ticket(ticketL);
                Change_Status_Ticket(id_ticket, "R");
                Send_Email(id_ticket, email, info_t);
            }
            return Redirect_Index();
        }

        private ActionResult Redirect_Index()
        {
            return RedirectToAction("Index");
        }


        public ActionResult Get_Log_Ticket(string id_ticket="")
        {
            Ticket tic = null;
            if (id_ticket != "")
            {
                tic = Get_Details_Ticket(id_ticket);
                if (tic != null)
                {
                    ClientController CC  = new ClientController();
                    ProductController PC = new ProductController();
                    tic.client           = CC.Get_Client_By_Membership_Number(tic.membership_number);
                    tic.advances        = Get_Advance_By_Id_Ticket(tic.id);
                    if (tic.client != null)
                    { 
                        tic.client.inventary = PC.Products_Log_Ticket(id_ticket);
                        tic.log              = Get_Log_Ticket_By_Id(id_ticket);
                    }
                }
            }
            return View(tic);
        }

        public Ticket Get_Ticket_Log_By_Id(string id_ticket="")
        {
            Ticket tic = null;
            if (id_ticket != "")
            {
                tic = Get_Details_Ticket(id_ticket);
                if (tic != null)
                {
                    ClientController CC = new ClientController();
                    ProductController PC = new ProductController();
                    tic.client = CC.Get_Client_By_Membership_Number(tic.membership_number);
                    if (tic.client != null)
                    {
                        tic.client.inventary = PC.Products_Log_Ticket(id_ticket);
                        tic.log = Get_Log_Ticket_By_Id(id_ticket);
                    }
                }
            }
            return tic;
        }

        public Ticket Get_Ticket_Log(string id_ticket = "")
        {
            Ticket tic = null;
            if (id_ticket != "")
            {
                tic = Get_Details_Ticket(id_ticket);
                if (tic != null)
                {
                    ClientController CC = new ClientController();
                    ProductController PC = new ProductController();
                    tic.client = CC.Get_Client_By_Membership_Number(tic.membership_number);
                    if (tic.client != null)
                    {
                        tic.client.inventary = PC.Products_Log_Ticket(id_ticket);
                        tic.log = Get_Log_Ticket_By_Id(id_ticket);
                    }
                }
            }
            return tic;
        }
        #endregion


        #region JSONS
        [HttpPost]
        [Permission(Permiso = Permissions_Enum.Cerrar_ticket)]
        public JsonResult Close_Tiquet(string id_tiquete="", string type="", string comentario="")
        {
            string msj = "Error";
            if (id_tiquete != "")
            {
                msj = Change_Status_Ticket(id_tiquete, "E");
                Save_Ticket_Advance(id_tiquete, type, comentario);
            }
            return Json(msj);
        }


        [HttpPost]
        [Permission(Permiso = Permissions_Enum.Cerrar_ticket)]
        //agrega un avance al tiquete en caso de que el mismo no pueda ser cerrado aún
        public JsonResult Add_Ticket_Progress(string id_tiquete = "", string type = "", string comentario = "")
        {
            if (id_tiquete != "")
            {
                Save_Ticket_Advance(id_tiquete, type, comentario);
            }
            TicketAdvance ta = Get_Advance_By_Id_Ticket(id_tiquete).Last();
            string data      = Utilities.convertDataTJson(ta);
            return Json(ta);
        }

        [HttpPost]
        [Permission(Permiso = Permissions_Enum.Consultar_tickets)]
        public JsonResult Get_Tickets_By_Search(string busqueda="", string tipo="", string estatus="")
        {
            string data = "";
            if (busqueda != "" && tipo != "" && estatus != "")
            {
                List<Ticket> list = Get_List_Tickets_By_Search(busqueda, tipo, estatus);
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                data = serializer.Serialize(list);
            }
            return Json(data);
        }

        //sent ticket email
        [HttpPost]
        public JsonResult Send_Email(string n_doc = "", string email = "", string other="")
        {
            Ticket ti = Get_Ticket_Log_By_Id(n_doc);
            if (ti != null && ti.log != null)
            {
                Pdf_Creator k = new Pdf_Creator();
                k.Create_Pdf_From_Ticket(ti);

                string msj = ti.client.contact;
                string encabezado = "Confirmación de servicio técnico POS, " + other;
                Email_Sender es = new Email_Sender();
                es.Send_Email(email, msj, encabezado);
            }
            return Json("");
        }

        [HttpPost]
        public JsonResult Get_Closed(string date_start = "", string date_end = "")
        {
            string data = "";
            UserController uc = new UserController();
            User us = uc.Get_User_By_Id(SessionHelper.GetUser());
            string cod_tec = "";
            bool filtrar = false;
            DateTime F_INICIO = DateTime.Now;
            DateTime F_FIN = DateTime.Now;
            if (us.cod_tec != "")
            {
                cod_tec = us.cod_tec;
                filtrar = true;
            }
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
            List<Ticket> list = Get_Closed_Recently(cod_tec, filtrar, F_INICIO, F_FIN);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            data = serializer.Serialize(list);
            return Json(data);
        }
        #endregion

        //logica
        #region logic
        private char Get_Type_Tiquet(string name="")
        {
            char value = ' ';
            switch (name)
            {
                case "INSTALACION":
                    value = 'I';
                break;
                case "VISITA":
                    value = 'V';
                break;
                case "RETIRO":
                    value = 'R';
                break;
                default:
                    value = ' ';
                break;
            }
            return value;
        }
        #endregion

        //querys
        #region querys
        public List<Ticket> Get_Tickets_By_Type(string type = "", string cod_tec = "", bool filtrar = false)
        {
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            var model = (from ticket in connection.INV_GARA
                         join tecnico in connection.INV_TECN
                            on new {x="003", y=ticket.TECNICO } equals new {x= tecnico.CIA, y=tecnico.COD_TECN } into ntenico
                            from tecnico in ntenico.DefaultIfEmpty()
                         where ticket.CIA == "003" &&
                               ticket.FECHA_INGRESO >= fecha && //eliminar esta linea
                               ticket.STATUS == type &&
                               ticket.N_FECHA_P != null &&
                               ( (filtrar == true && ticket.TECNICO == cod_tec) || filtrar == false )
                         select new Ticket
                         {
                             id         =  ticket.NUM_DOC_GARANTIA,
                             date       =  ticket.FECHA_INGRESO,
                             ticket_cred = ticket.PROBLEMA_2,
                             user       =  ticket.USUARIO,
                             status     =  ticket.STATUS,
                             technical  =  tecnico.DES_TECN == null ? "No Asignado": tecnico.DES_TECN,
                             membership_number = ticket.N_AFILIADO,
                             contact    =  ticket.NOTA_1,    
                             local_name =  ticket.CONTACTO,
                             phone      =  ticket.TELEFONO,
                             problem    =  ticket.PROBLEMA_1,
                             province   =  ticket.N_COD_PROV,
                             canton     =  ticket.N_COD_CANT,
                             district   =  ticket.N_COD_DIST,
                             address    =  ticket.N_DIRECCION,
                             visit_date =  ticket.N_FECHA_P.Value,
                             type       =  ticket.TIPO_EVENTO == "I" ? "INSTALACION" :
                                           ticket.TIPO_EVENTO == "V" ? "VISITA" :
                                           ticket.TIPO_EVENTO == "R" ? "RETIRO" : "N/A"
                         });
            return model.ToList();
        }

        public List<Ticket> Get_Closed_Recently(string cod_tec = "", bool filtrar = false, DateTime? date_start = null, DateTime? date_end = null)
        {
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();

            var model = (from ticket in connection.INV_GARA
                         join log in connection.SGA_BITACORA_TICKET
                        on new { x = ticket.CIA, y = ticket.NUM_DOC_GARANTIA.Trim() } equals new { x = "003", y = log.ID_TICKET.Trim() }
                        where ticket.CIA == "003" &&
                        (ticket.STATUS == "R" || ticket.STATUS == "E") &&
                        (log.FECHA_DIG >= date_start && log.FECHA_DIG <= date_end) &&
                        ((filtrar == true && ticket.TECNICO == cod_tec) || filtrar == false)
                        orderby log.FECHA_DIG descending
                        select new Ticket
                        {
                             id = ticket.NUM_DOC_GARANTIA,
                             date = ticket.FECHA_INGRESO,
                             status = ticket.STATUS,
                             membership_number = ticket.N_AFILIADO,
                             contact = ticket.NOTA_1,
                             local_name = ticket.CONTACTO,
                             visit_date = log.FECHA_DIG.Value,
                             type = ticket.TIPO_EVENTO == "I" ? "INSTALACION" :
                                           ticket.TIPO_EVENTO == "V" ? "VISITA" :
                                           ticket.TIPO_EVENTO == "R" ? "RETIRO" : "N/A"
                        });
            return model.ToList();
        }

        private void Save_Ticket_Advance(string id_tiquete = "", string type = "", string comentario = "")
        {
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            UserController uc = new UserController();
            string userName   = uc.Get_User_By_Id(SessionHelper.GetUser()).login_name;
            DateTime now      = DateTime.Now;
            string hour       = DateTime.Now.ToString("HH:mm");
            try
            {
                INV_GARV iv = new INV_GARV()
                {
                    CIA              = "003",
                    NUM_DOC_GARANTIA = id_tiquete,
                    FECHA            = now,
                    HORA             = hour,
                    DETALLE1         = type,
                    DETALLE2         = "Realizado desde el sistema S.G.A",
                    DETALLE3         = userName,
                    CONFIDENCIAL     = comentario
                };
                connection.INV_GARV.Add(iv);
                connection.SaveChanges();
            }
            catch (Exception e)
            {
                Generate_Exception("Error " + e);
            }
        }

        //obtiene todos los avances agregados a los tiquetes
        private List<TicketAdvance> Get_Advance_By_Id_Ticket(string idTicket = "")
        {
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            var model = (from advace in connection.INV_GARV
                         where advace.CIA == "003" &&
                               advace.NUM_DOC_GARANTIA == idTicket
                         select new TicketAdvance
                         {
                             id = advace.SECUENCIA.ToString(),
                             user = advace.DETALLE3,
                             detail = advace.CONFIDENCIAL,
                             date = advace.FECHA.Value
                         }).ToList();
            if (model.Count() > 0)
            {
                return model;
            }
            return null;
        }
        
        public List<History_Tickets_Dashboard> Get_CLoset_Tickets_By_Date(string cod_tec = "", int diasH = 0, string parametro="")
        {
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            var model = (from day in connection.F_OBTENER_RESUMEN_TICKETS(cod_tec, diasH, parametro)
                         select new History_Tickets_Dashboard
                         {
                             name_day  = day.DIA,
                             installed = day.INSTALADO.Value,
                             visited   = day.VISITADO.Value,
                             retired   = day.RETIRADO.Value,
                             total     = day.TOTAL.Value
                         });
            return model.ToList();
        }

        //se obtiene el desgloce del tiquete a partir del id
        private Ticket Get_Details_Ticket(string id_ticket="0")
        {
             DateTime dateError = new DateTime();
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            var model = (from ticket in connection.INV_GARA
                         join tecnico in connection.INV_TECN
                            on new { x = "003", y = ticket.TECNICO } equals new { x = tecnico.CIA, y = tecnico.COD_TECN } into ntenico
                         from tecnico in ntenico.DefaultIfEmpty()
                         where ticket.CIA == "003" &&
                               ticket.NUM_DOC_GARANTIA == id_ticket.Trim() 
                         select new Ticket
                         {
                             id         = ticket.NUM_DOC_GARANTIA,
                             ticket_cred= ticket.PROBLEMA_2,
                             type       = (ticket.TIPO_EVENTO == "I" ? "INSTALACION" :
                                            ticket.TIPO_EVENTO == "V" ? "VISITA" :
                                            ticket.TIPO_EVENTO == "R" ? "RETIRO" : "N/A"),

                             date       = ticket.FECHA_INGRESO,
                             user       = ticket.USUARIO,

                             status =   (ticket.STATUS == "E" ? "CERRADO":
                                         ticket.STATUS == "P" ? "PENDIENTE":
                                         ticket.STATUS == "R" ? "ATENDIDO" : "N/A"),

                             technical  = tecnico.DES_TECN == null ? "No Asignado" : tecnico.DES_TECN,
                             membership_number = ticket.N_AFILIADO,
                             problem    = ticket.PROBLEMA_1,
                             address    = ticket.N_DIRECCION,
                             visit_date = ticket.N_FECHA_P != null ? ticket.N_FECHA_P.Value : dateError,
                             hour_programmed = ticket.N_HORA_P
                         }).FirstOrDefault();
            return model;
        }
        
        //se obtienen los tiquetes por codigo de tecnico
        public List<Ticket> Get_Tickets_By_Technical(string code="")
        {
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            DateTime dateError = new DateTime();
            var model = (from ticket in connection.INV_GARA
                         join tecnico in connection.INV_TECN
                            on new { x = "003", y = ticket.TECNICO } equals new { x = tecnico.CIA, y = tecnico.COD_TECN } into ntenico
                         from tecnico in ntenico.DefaultIfEmpty()
                         where ticket.CIA     == "003" && 
                               ticket.TECNICO == code &&
                               ticket.FECHA_INGRESO >= fecha 
                         select new Ticket
                         {
                             date       = ticket.FECHA_INGRESO,
                             ticket_cred = ticket.PROBLEMA_2,
                             user       = ticket.USUARIO,
                             status     = ticket.STATUS,
                             technical  =  tecnico.DES_TECN == null ? "No Asignado" : tecnico.DES_TECN,
                             membership_number = ticket.N_AFILIADO,
                             contact    = ticket.NOTA_1,
                             local_name = ticket.CONTACTO,
                             phone      = new String(ticket.TELEFONO.Take(18).ToArray()),
                             problem    = ticket.PROBLEMA_1,
                             province   = ticket.N_COD_PROV,
                             canton     = ticket.N_COD_CANT,
                             district   = ticket.N_COD_DIST,
                             address    = ticket.N_DIRECCION,
                             visit_date = ticket.N_FECHA_P != null ? ticket.N_FECHA_P.Value : dateError,
                             type       = ticket.TIPO_EVENTO == "I" ? "INSTALACION" :
                                          ticket.TIPO_EVENTO == "V" ? "VISITA" :
                                          ticket.TIPO_EVENTO == "R" ? "RETIRO" : "N/A"
                         });
            return model.ToList();
        }

        //obtiene los tiquetes en un rango de fechas determinado
        public List<Ticket> Get_Tickets_By_Dates(DateTime? f_inicio=null, DateTime? f_fin=null, string filtro= "", string t_fecha = "", string t_movimiento = "", string t_estatus="")
        {
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            try
            {
                var model = (from ticket in connection.INV_GARA
                             join tecnico in connection.INV_TECN
                                on new { x = "003", y = ticket.TECNICO } equals new { x = tecnico.CIA, y = tecnico.COD_TECN } into ntenico
                                from tecnico in ntenico.DefaultIfEmpty()
                             join province in connection.WEB_Provincia
                                on ticket.N_COD_PROV equals province.idProvincia into provusr
                                from province in provusr.DefaultIfEmpty()
                             join canton in connection.WEB_Canton
                                on (ticket.N_COD_PROV + ticket.N_COD_CANT).Trim() equals canton.idCanton into cantusr
                                from canton in cantusr.DefaultIfEmpty()
                             join distrito in connection.WEB_Distrito
                                on (ticket.N_COD_PROV + ticket.N_COD_CANT + ticket.N_COD_DIST).Trim() equals distrito.idDistrito into distusr
                                from distrito in distusr.DefaultIfEmpty()
                             join afiliado in connection.SGA_AFILIADOS
                                on ticket.N_AFILIADO  equals afiliado.N_AFILIADO into afi_ticket
                                from afiliado in afi_ticket.DefaultIfEmpty()
                             where ticket.CIA == "003" &&
                                   (
                                     (filtro == "P" && ticket.STATUS == "P") || (filtro=="E" && ticket.STATUS == "E") || (filtro == "T")
                                   ) &&
                                   (
                                     ((t_fecha == "P" || t_fecha == "") && ticket.N_FECHA_P >= f_inicio && ticket.N_FECHA_P <= f_fin) ||
                                     (t_fecha == "C" && ticket.FECHA_INGRESO >= f_inicio && ticket.FECHA_INGRESO <= f_fin) ||
                                     (t_fecha == "E" && ticket.FECHA_ENTREGA >= f_inicio && ticket.FECHA_ENTREGA <= f_fin) 
                                   ) &&
                                    (t_movimiento == "" || ticket.TIPO_EVENTO == t_movimiento) &&
                                   (
                                     (t_estatus == "T" || afiliado.ESTATUS == t_estatus || t_estatus == "") 
                                   )
                             select new Ticket
                             {
                                 date = (t_fecha == "P" || t_fecha == "" ? ticket.N_FECHA_P :
                                         t_fecha == "C" ? ticket.FECHA_INGRESO: 
                                         t_fecha == "E" ? ticket.FECHA_ENTREGA : f_fin).Value,
                                 type = ticket.TIPO_EVENTO == "I" ? "INSTALACION" :
                                              ticket.TIPO_EVENTO == "V" ? "VISITA" :
                                              ticket.TIPO_EVENTO == "R" ? "RETIRO" : "N/A",
                                 id = ticket.NUM_DOC_GARANTIA,
                                 local_name = ticket.CONTACTO,
                                 membership_number = ticket.N_AFILIADO,
                                 contact    = ticket.NOTA_1,
                                 phone      = ticket.TELEFONO,
                                 technical  = tecnico.DES_TECN == null ? "No Asignado" : tecnico.DES_TECN,
                                 province   = province.nombreProvincia != null ? province.nombreProvincia : "N/A",
                                 canton     = canton.nombreCanton != null ? canton.nombreCanton : "N/A",
                                 district   = distrito.nombreDistrito != null ? distrito.nombreDistrito : "N/A",
                                 status     =   ticket.STATUS == "E" ? "CERRADO":
                                                ticket.STATUS == "P" ? "PENDIENTE":
                                                ticket.STATUS == "R" ? "ATENDIDO": "N/A",
                                 ticket_cred =  ticket.PROBLEMA_2,
                                 extra_1     =  ticket.N_TIPO,
                                 extra_2     =  ticket.N_CANTIDAD.ToString() !="" ? ticket.N_CANTIDAD.ToString() :"0.00",
                                 address     =  ticket.SOLUCION_1,
                                 ejecutive   =  ticket.N_EJECUTIVO,
                                 hour_programmed = ticket.N_HORA_P,
                                 coordinator = ticket.N_COORDINA,
                                 details     = ticket.PROBLEMA_1,
                                 internet    = ticket.N_INTERNET,
                                 desktop     = ticket.N_MUEBLE,
                                 electricity = ticket.N_ELECTRICIDAD
                             }).ToList();
                if (model.Count() > 0){
                    return model;
                }
                else{
                    return new List<Ticket>();
                }
            }
            catch (Exception e)
            {
                string mjs = "Error " + e;
            }
            return null;
        }

        //se obtienens los tiquetes por numero de afiliado
        public List<Ticket> Get_Tickets_By_Client(string N_AFILIADO2 = "")
        {
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            var model = (from ticket in connection.F_TICKETS_AFILIADO(N_AFILIADO2)
                         orderby ticket.FECHA
                         select new Ticket
                         {
                             id         = ticket.ID,
                             date       = ticket.FECHA.Value,
                             user       = ticket.USUARIO,   
                             status     = ticket.ESTATUS,
                             technical  = ticket.TECNICO,
                             membership_number = ticket.N_AFILIADO,
                             contact    = ticket.CONTACTO,
                             local_name = ticket.LOCAL_N,
                             phone      = ticket.TELEFONO,
                             problem    = ticket.PROBLEMA,
                             province   = ticket.PROVINCIA,
                             canton     = ticket.CANTON,
                             district   = ticket.DISTRITO,
                             address    = ticket.DIRECCION,
                             visit_date = ticket.FECHA.Value,
                             type       = ticket.TIPO
                         }).ToList();
            if (model.Count > 0)
            { 
                return model;
            }
            return null;
        }

        //funcion que obtiene los tiquetes segun la busqueda y parametros
        /*
         * busqueda = string a buscar
         * parametro = 0 = # tiquete, 1= #afiliado, 2=nombre contacto, 3=nombre negocio
         * estatus = 0 = activo, 1 = inactivo
         */
        private List<Ticket> Get_List_Tickets_By_Search(string BUSQUEDA="", string PARAMETRO="", string ESTATUS ="")
        {
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            var model = (from f_ticket in connection.F_BUSQUEDA_TICKET(BUSQUEDA, PARAMETRO, ESTATUS)
                         select new Ticket
                         {
                             id                 = f_ticket.ID,
                             type               = f_ticket.TIPO,
                             date               =  f_ticket.FECHA.Value,
                             user               = f_ticket.USUARIO,
                             technical          = f_ticket.TECNICO,
                             membership_number  = f_ticket.N_AFILIADO,
                             contact            = f_ticket.CONTACTO,
                             local_name         = f_ticket.LOCAL_N,
                             phone              = f_ticket.TELEFONO,
                             problem            = f_ticket.PROBLEMA,
                             visit_date         = f_ticket.DIA_VISITA.Value
                         });
            return model.ToList();
        }

        //agrega una bitacora al tiquete cerrado
        private string Insert_Log_Ticket(TicketLog log)
        {
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            string mjs = "";
            SGA_BITACORA_TICKET logT = new SGA_BITACORA_TICKET()
            {
                ID_TICKET           = log.id,
                TIPO_MOVIMIENTO     = log.type.ToString(),
                AVERIA_REPORTADA    = log.damage     == null ? "" : log.damage.Trim(), 
                DIAGNOSTICO         = log.diagnostic == null ? "" : log.diagnostic.Trim(),
                SOLUCION            = log.solution   == null ? "" : log.solution.Trim(),
                EXPOSICION_EQUIPO   = log.equipment_exposition,
                EXPOSICION_EQUIPO_2 = log.equipment_exposition_2,
                MANTENIMIENTO       = log.mainmaintenance,
                PRUEBAS             = log.tests,
                TIPO_CONEXION       = log.connetion,
                DETALLE_INSTALACION = log.instalation_detaill,
                EXPOSICION_EQUIPO_VISITA = log.visit_equipment_exposition,
                OTROS               = log.others,
                FECHA_DIG           = log.date,
                EXTRA_1             = log.extra_1,
                EXTRA_2             = log.extra_2,
                EXTRA_3             = log.extra_3
            };
            connection.SGA_BITACORA_TICKET.Add(logT);
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
            return mjs;
        }
        
        //cambia el estatus del tiquetem puede cambiar a R = revisado o E = cerrado
        private string Change_Status_Ticket(string id_ticket="", string status="")
        {
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            string mjs = "";
            if (id_ticket != "")
            {
                var model = (from  ticket in connection.INV_GARA
                             where ticket.NUM_DOC_GARANTIA == id_ticket &&
                                   ticket.CIA == "003"
                             select ticket);
                foreach (INV_GARA tickets in model)
                {
                    tickets.STATUS = status;
                }
                try
                {
                    connection.SaveChanges();
                    mjs = "Succesfull";
                }
                catch (Exception e)
                {
                    mjs = "Error " + e;
                    Generate_Exception(mjs);
                }
            }
            return mjs;
        }


        //ultimos 10 revisados o cerrados
        private List<Ticket> Get_Last_10()
        {
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            DateTime dateError = new DateTime();
            var model = (from ticket in connection.INV_GARA
                         where ticket.CIA == "003" && 
                         (ticket.STATUS == "E" || ticket.STATUS == "R")
                         orderby ticket.NUM_DOC_GARANTIA descending
                         select new Ticket {
                             id         = ticket.NUM_DOC_GARANTIA,
                             date       = ticket.FECHA_INGRESO,
                             user       = ticket.USUARIO,
                             status     = ticket.STATUS,
                             technical  = ticket.TECNICO,
                             ticket_cred = ticket.PROBLEMA_2,
                             membership_number = ticket.N_AFILIADO,
                             contact    = ticket.NOTA_1,
                             local_name = ticket.CONTACTO,
                             phone      = ticket.TELEFONO,
                             problem    = ticket.PROBLEMA_1,
                             province   = ticket.N_COD_PROV,
                             canton     = ticket.N_COD_CANT,
                             district   = ticket.N_COD_DIST,
                             address    = ticket.N_DIRECCION,
                             visit_date = ticket.N_FECHA_P != null ? ticket.N_FECHA_P.Value : dateError,
                             type       = ticket.TIPO_EVENTO == "I" ? "INSTALACION" :
                                          ticket.TIPO_EVENTO == "V" ? "VISITA" :
                                          ticket.TIPO_EVENTO == "R" ? "RETIRO" : "N/A"
                         }).Take(10);
            return model.ToList();
        } //revisar excepciones


        public List<String> Get_Damages_List()
        {
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            var model = (from damage in connection.INV_DANOS
                         where damage.CIA == "003"
                         orderby damage.Descripcion
                         select damage.Descripcion).ToList();
            return model;
        }
        #endregion
        

        #region bitacora
        public TicketLog Get_Log_Ticket_By_Id(string id_ticket = "")
        {
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            if (id_ticket != "")
            {
                var model = (from log in connection.SGA_BITACORA_TICKET
                             where log.ID_TICKET == id_ticket
                             select new TicketLog
                             {
                                 id          = log.ID_TICKET,
                                 date        = log.FECHA_DIG.Value,
                                 type_string = log.TIPO_MOVIMIENTO == "I" ? "INSTALACION" :
                                               log.TIPO_MOVIMIENTO == "V" ? "VISITA" :
                                               log.TIPO_MOVIMIENTO == "R" ? "RETIRO" : "N/A",
                                 damage      = log.AVERIA_REPORTADA.Replace("\r\n", string.Empty),
                                 diagnostic  = log.DIAGNOSTICO.Replace("\r\n", string.Empty),
                                 solution    = log.SOLUCION.Replace("\r\n", string.Empty),
                                 others      = log.OTROS,
                                 extra_1     = log.EXTRA_1,
                                 extra_2     = log.EXTRA_2,
                                 extra_3     = log.EXTRA_3,
                                 equipment_exposition   = log.EXPOSICION_EQUIPO,
                                 equipment_exposition_2 = log.EXPOSICION_EQUIPO_2,
                                 mainmaintenance        = log.MANTENIMIENTO,
                                 tests       = log.PRUEBAS,
                                 connetion   = log.TIPO_CONEXION,
                                 instalation_detaill        = log.DETALLE_INSTALACION,
                                 visit_equipment_exposition = log.EXPOSICION_EQUIPO_VISITA
                             }).FirstOrDefault();
                return model;
            }
            return null;
        }

        public String Get_Last_Mainteinance_By_Client(string n_afiliado="")
        {
            DateTime fecha = DateTime.Now;
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            var model = (from bitacora in connection.SGA_BITACORA_TICKET
                         join ticket in connection.INV_GARA
                         on new {x = bitacora.ID_TICKET, y = "003"} equals new {x = ticket.NUM_DOC_GARANTIA, y = ticket.CIA }
                         where bitacora.EXTRA_2 == "1" && ticket.N_AFILIADO == n_afiliado
                         orderby bitacora.FECHA_DIG descending
                         select bitacora.FECHA_DIG.ToString()).FirstOrDefault();
            return (model != null) ? model : "N/A";
        }
        #endregion

        private void Generate_Exception(string detail = "")
        {
            Exceptions_Management ex = new Exceptions_Management();
            ex.Create_Exception(8, detail);
        }
    
    }
}