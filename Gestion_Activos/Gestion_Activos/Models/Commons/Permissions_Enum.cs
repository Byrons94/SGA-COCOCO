using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gestion_Activos.Models.Commons
{
    //permissions of the app to validate with the sql permisions
    public enum Permissions_Enum
    {
       #region usuarios
        Visualizar_Usuario   = 1,
        Registrar_Usuario    = 2,
        Editar_Usuario       = 3,
        Eliminar_Usuario     = 4,
       #endregion

       #region roles
        Visualizar_Rol       = 5,
        Registrar_Rol        = 6,
        Editar_Rol           = 7,
        Eliminar_Rol         = 8,
       #endregion

       #region tickets
        Visualizar_tickets   = 9,
        Visualizar_en_ruta   = 10,
        Visualizar_atendidos = 11,
        Consultar_tickets    = 12,
        Gestionar_ticket     = 13,
        Cerrar_ticket        = 14,
       #endregion

       #region Afiliados
        Visualizar_afiliado  = 15,
        Gestionar_afiliado   = 16,
        Actualizar_afiliado  = 17,
       #endregion

       Visualizar_Mantenimientos = 18,
       Procesar_Tiquetes         = 19,
       Cambiar_Afiliado          = 20,
       Consultar_Inventario      = 21,
       Consultar_Series          = 22,
       Visualizar_Reportes       = 23,
       Manejar_Evidencias        = 24
    }
}