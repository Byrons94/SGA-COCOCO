using Gestion_Activos.Models;
using Gestion_Activos.Models.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gestion_Activos.Models.Class;

namespace Gestion_Activos.Controllers
{
    public class  ConfigurationController : Controller
    {
        // GET: Configuration
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Get_Products_Categories()
        {
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            var model = (from categoria in connection.SGA_CATEGORY_PRODUCT
                         select new Product_Category {
                             id = categoria.ID,
                             description = categoria.NOMBRE_CATEGORIA
                         }).ToList();
            return View(model);
        }

        [HttpPost]
        public JsonResult Create_Category(string desc = "")
        {
            string mjs = "";
            if (desc.Trim() != ""){
                mjs = Save_Category(desc);
            }
            return Json(mjs);
        }

        [HttpPost]
        public JsonResult Update_Category(string id = "", string desc = "")
        {
            string mjs = "";
            if (desc.Trim() != "" && id.Trim() != "")
            {
                mjs = Modify_Category(id, desc);
            }
            return Json(mjs);
        }

        [HttpPost]
        public JsonResult Delete_Category(string id = "")
        {
            string mjs = "";
            if (id.Trim() != "")
            {
                int idC = Convert.ToInt32(id);
                mjs = Delete_Category_Q(idC);
            }
            return Json(mjs);
        }

        //querys
        private string Save_Category(string desc="")
        {
            string mjs = "Successful";
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            SGA_CATEGORY_PRODUCT cat = new SGA_CATEGORY_PRODUCT()
            {
                NOMBRE_CATEGORIA = desc
            };
            connection.SGA_CATEGORY_PRODUCT.Add(cat);
            try
            {
                connection.SaveChanges();
            }
            catch (Exception e)
            {
                mjs = "Error: " + e;
            }
            return mjs;
        }

        private string Modify_Category(string id="", string desc = "")
        {
            string mjs = "Successful";
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            int value = Convert.ToInt32(id);
            var query = (from categoria in connection.SGA_CATEGORY_PRODUCT
                         where categoria.ID == value
                         select categoria);
            foreach (SGA_CATEGORY_PRODUCT category in query)
            {
                category.NOMBRE_CATEGORIA = desc;
            }
            try
            {
                connection.SaveChanges();
            }
            catch (Exception e)
            {
                mjs = "Error: " + e;
            }
            return mjs;
        }

        private string Delete_Category_Q(int id = 0)
        {
            string mjs = "Successful";
            BMS_DATA_DENTEntities connection = BDConnection.Get_Connection();
            var remove = from category in connection.SGA_CATEGORY_PRODUCT
                         where category.ID == id
                         select category;
            foreach (var category in remove)
            {
                connection.SGA_CATEGORY_PRODUCT.Remove(category);
            }
            try
            {
                connection.SaveChanges();
            }
            catch (Exception e)
            {
                mjs = "Error: " + e;
            }
            return mjs;
        }



    }
}