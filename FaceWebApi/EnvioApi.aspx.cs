using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Web.Script.Services;
using AriFaceLib;
using MySql.Data.MySqlClient;

namespace FaceWebApi
{
    public partial class EnvioApi : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        #region WebMethods
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static IList<Envio> GetEnvios()
        {
            IList<Envio> le = new List<Envio>();
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                le = CntAriFaceLib.GetEnvios(conn);
                conn.Close();
            }
            return le;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static IList<Factura> GetFacturasEnvio(int clienteId, int departamentoId)
        {
            IList<Factura> lf = new List<Factura>();
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                lf = CntAriFaceLib.GetFacturasEnvio(clienteId, departamentoId, conn);
                conn.Close();
            }
            return lf;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static Envio GetEnvio(int clienteId, int departamentoId)
        {
            Envio e = null;
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                e = CntAriFaceLib.GetEnvio(clienteId, departamentoId, conn);
                conn.Close();
            }
            return e;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static void EliminarFacturaDeEnvio(int facturaId)
        {
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                CntAriFaceLib.EliminarFacturaDeEnvio(facturaId, conn);
                conn.Close();
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static void RecuperarFacturaDeEnvio(int facturaId)
        {
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                CntAriFaceLib.RecuperarFacturaDeEnvio(facturaId, conn);
                conn.Close();
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string SendEnvio(int clienteId, int departamentoId, string certSn)
        {
            string r = "";
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                // el directorio de notificaciones hay que pasarlo
                r = CntFaceApi.SendEnvio(clienteId, departamentoId, certSn, conn);
                conn.Close();
            }
            return r;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string SendEnvios(string certSn)
        {
            string r = "";
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                // el directorio de notificaciones hay que pasarlo
                r = CntFaceApi.SendEnvios(certSn, conn);
                conn.Close();
            }
            return r;
        }

        #endregion
    }
}