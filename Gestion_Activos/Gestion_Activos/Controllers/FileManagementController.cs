using System;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Gestion_Activos.Models;
using Gestion_Activos.Models.Commons;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using Gestion_Activos.Models.Class;

namespace Gestion_Activos.Controllers
{
    public class FileManagementController : Controller
    {

        
        [HttpPost]
        public JsonResult UploadFile()
        {
            String path = "~/Uploads";
            HttpPostedFileBase file = Request.Files["files[]"];
            string idTicket = Request.Form["idTicket"].Replace(" ", "");
            string membershipCode = Request.Form["membershipCode"].Replace(" ", "");
            
            bool isUploaded = false;
            string message = "File upload failed";

            if (file != null && file.ContentLength != 0)
            {
                string fileName = file.FileName.Replace(" ", "");
                string pathForSaving = Server.MapPath(path + "/" + membershipCode + "/" + idTicket);
                if (this.CreateFolderIfNeeded(pathForSaving))
                {
                    try
                    {
                        string filePath = Path.Combine(pathForSaving, fileName);
                        bool save = saveFileInformation(file, filePath, membershipCode, idTicket);
                        message += filePath;
                        if (save) { 
                            file.SaveAs(filePath);
                            isUploaded = true;
                            message += "File uploaded successfully!";
                        }
                    }
                    catch (Exception ex)
                    {
                        message = string.Format("File upload failed: {0}", ex.Message);
                    }
                }
            }
            return Json(new { isUploaded = isUploaded, message = message }, "text/html");
        }


        public ActionResult DownloadFile(string idFile)
        {
            if (idFile != "") {
                int id = Int32.Parse(idFile);
                Ticket_Evidence te = GetEvidenceById(id);
                string filePath = te.getPath();
                string fileName = te.name + te.extension;
                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                Response.Headers.Add("content-disposition", "attachment; filename="+fileName);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            return null;
        }


        private bool CreateFolderIfNeeded(string path)
        {
            bool result = true;
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception)
                {
                    /*TODO: You must process this exception.*/
                    result = false;
                }
            }
            return result;
        }


        #region Json
        [HttpPost]
        public JsonResult GetEvidencesByTicketId(string tickedId) {
            string data = "";
            if (tickedId.Trim() != "") {
                List<Ticket_Evidence> list = GetEvidenceByTicket(tickedId);
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                data = serializer.Serialize(list);
            }
            return Json(data);
        }

        [HttpPost]
        public JsonResult FileExists(string idFile)
        {
            bool exists = false;
            if (idFile != "") {
                exists = FileExistsQuery(idFile); 
            }
            return Json(exists);
        }
        #endregion


        private bool saveFileInformation(HttpPostedFileBase file, string path, string membershipId, string ticket) {
            bool success = false;
            string fileName = file.FileName.Replace(" ", "");
            var ext = Path.GetExtension(fileName);
            var name = Path.GetFileNameWithoutExtension(fileName);
            DateTime now = DateTime.Now;

            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            SGA_EVIDENCIA_TICKET evidence = new SGA_EVIDENCIA_TICKET()
            {
                   NAME = name,
                   EXTENSION = ext,
                   PATH = path,
                   MEMBERSHIP_ID = membershipId,
                   ID_TICKET = ticket,
                   DATE = now,
                   ACTIVE = true,
                   UPDATED = null,
                   DESCRIPTION = null
            };
            try
            {
                connection.SGA_EVIDENCIA_TICKET.Add(evidence);
                connection.SaveChanges();
                success = true;
            }
            catch (Exception) {
                success = false;
            }
            return success;
        }

        private List<Ticket_Evidence> GetEvidenceByTicket(string ticketId) {
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            var data = (from evidence in connection.SGA_EVIDENCIA_TICKET
                        where evidence.ID_TICKET == ticketId
                        select new Ticket_Evidence
                        {
                           id = evidence.ID,
                           name = evidence.NAME,
                           extension = evidence.EXTENSION,
                           dateUploaded = evidence.DATE.Value
                        }).ToList();
            if (data.Count > 0)
            {
                return data;
            }
            return null;
        }

        private Ticket_Evidence GetEvidenceById(int ticketId)
        {
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            var data = (from evidence in connection.SGA_EVIDENCIA_TICKET
                        where evidence.ID == ticketId
                        select new Ticket_Evidence
                        {
                            id = evidence.ID,
                            name = evidence.NAME,
                            extension = evidence.EXTENSION,
                            dateUploaded = evidence.DATE.Value,
                            path = evidence.PATH

                        }).FirstOrDefault();
            return data;
        }

        private bool FileExistsQuery(string idFile) {
            bool exists = false;
            try {
                BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
                int id = Int32.Parse(idFile);

                var data = (from evidence in connection.SGA_EVIDENCIA_TICKET
                            where evidence.ID == id
                            select new Ticket_Evidence
                            {
                                id = evidence.ID
                            }).ToList();
                if (data.Count > 0)
                {
                    exists = true;
                }
            }
            catch (Exception) { /*nothing to do*/ }
           
            return exists;
        }

    }
}