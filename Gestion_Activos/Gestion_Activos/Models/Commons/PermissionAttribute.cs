using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Gestion_Activos.Models.Commons
{
    public class PermissionAttribute : ActionFilterAttribute
    {
        public Permissions_Enum Permiso { get; set; }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (!FrontUser.Have_Permission(this.Permiso))
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Dashboard",
                    action = "Index"
                }));
            }
        }
    }
}