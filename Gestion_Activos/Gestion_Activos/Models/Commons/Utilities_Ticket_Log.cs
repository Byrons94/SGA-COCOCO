using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gestion_Activos.Models.Commons
{
    public static class Utilities_Ticket_Log
    {
        public static string Conection(string[] array = null)
        {
            string data = "";
            foreach (var value in array)
            {
                switch (value)
                {
                    case "1":
                        data += " •Internet";
                        break;
                    case "2":
                        data += " •Inalámbrica";
                        break;
                    case "3":
                        data += " •Modem";
                        break;
                    case "4":
                        data += " •Cable";
                        break;
                    case "5":
                        data += " •Datacard";
                        break;
                }
                data += "\n";
            }
            return validate(data);
        }

        public static string Detalle_Instalacion(string[] array = null)
        {
            string data = "";
            foreach (var value in array)
            {
                switch (value)
                {
                    case "1":
                        data += " •Lugar instalación";
                        break;
                    case "2":
                        data += " •Espacio caja dinero";
                        break;
                    case "3":
                        data += " •Espacio impresora";
                        break;
                    case "4":
                        data += " •Espacio CPU";
                        break;
                    case "5":
                        data += " •Espacio Monitor";
                        break;
                }
                data += "\n";
            }
            return validate(data);
        }

        public static string Exposición_Equipo(string[] array = null)
        {
            string data = "";
            foreach (var value in array)
            {
                switch (value)
                {
                    case "0":
                        data += " •Humedad";
                        break;
                    case "1":
                        data += " •Grasa";
                        break;
                    case "2":
                        data += " •Sobre voltaje";
                        break;
                    case "3":
                        data += " •Plagas";
                        break;
                    case "4":
                        data += " •Polvo";
                        break;
                    case "5":
                        data += " •Oxidacion";
                        break;
                    case "6":
                        data += " •Desgaste";
                        break;
                }
                data += "\n";
            }
            return validate(data);
        }

        public static string Pruebas(string[] array = null)
        {
            string data = "";
            foreach (var value in array)
            {
                switch (value)
                {
                    case "1":
                        data += " •Se realizan pruebas de lectura/escritura";
                        break;
                    case "2":
                        data += " •Pruebas de encendido y apagado";
                        break;
                    case "3":
                        data += " •Pruebas de impresión";
                        break;
                    case "4":
                        data += " •Se indica como agregar el rollo de papel";
                        break;
                    case "5":
                        data += " •Se ingresa al acceso directo del punto de venta";
                        break;
                    case "6":
                        data += " •Se hacen recomendaciones";
                        break;
                }
                data += "\n";
            }
            return validate(data);
        }

        public static string Pruebas_Visita(string[] array = null)
        {
            string data = "";
            foreach (var value in array)
            {
                switch (value)
                {
                    case "1":
                        data += " •Se verifica Backup de UPS";
                        break;
                    case "2":
                        data += " •Se realiza test de HDD";
                        break;
                    case "3":
                        data += " •Se realizan pruebas de impresión";
                        break;
                    case "4":
                        data += " •Se prueban lectores";
                        break;
                    case "5":
                        data += " •Se verifican Drivers";
                        break;
                    case "6":
                        data += " •Limpieza de Sistema Operativo";
                        break;
                }
                data += "\n";
            }
            return validate(data);
        }

        public static string Mantenimiento(string[] array = null)
        {
            string data = "";
            foreach (var value in array)
            {
                switch (value)
                {
                    case "1":
                        data += " •Limpieza de puertos y conexiones";
                        break;
                    case "2":
                        data += " •Limpieza interna del CPU";
                        break;
                    case "3":
                        data += " •Limpieza interna de la impresora";
                        break;
                    case "4":
                        data += " •Limpieza de periféricos";
                        break;
                    case "5":
                        data += " •Estado del cableado";
                        break;
                    case "6":
                        data += " •Pruebas de encendido y apagado";
                        break;
                }
                data += "\n";
            }
            return validate(data);
        }

        private static string validate(string value="")
        {
            value = (value.Trim() == "") ? "No especificado" : value;
            return value;
        }
    }
}