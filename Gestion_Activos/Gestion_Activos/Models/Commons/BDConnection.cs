using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gestion_Activos.Models.Commons
{
    public class BDConnection
    {
        private static BMS_DATA_DENTEntities connection = null;

        private BDConnection() { }

        public static BMS_DATA_DENTEntities Get_Connection()
        {
            return (connection == null) ? new BMS_DATA_DENTEntities() : connection;
        }
    }
}