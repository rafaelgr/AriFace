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
using System.Web.Hosting;

namespace FaceWebApi
{
    public partial class FacturaApi : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        #region WebMethods
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static IList<Factura> GetFacturas()
        {
            IList<Factura> lf = new List<Factura>();
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                lf = CntAriFaceLib.GetFacturas(conn);
                conn.Close();
            }
            return lf;
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static IList<Factura> GetFacturasNoEnviadas()
        {
            IList<Factura> lf = new List<Factura>();
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                lf = CntAriFaceLib.GetFacturasNoEnviadas(conn);
                conn.Close();
            }
            return lf;
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static IList<Factura> GetFacturasCliente(int clienteId)
        {
            IList<Factura> lf = new List<Factura>();
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                lf = CntAriFaceLib.GetFacturasCliente(clienteId, conn);
                conn.Close();
            }
            return lf;
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static Factura GetFacturaById(string id)
        {
            Factura f = null;
            int mId = int.Parse(id);
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                f = CntAriFaceLib.GetFactura(mId, conn);
                conn.Close();
            }
            return f;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string VerPdf(int facturaId, int administradorId)
        {
            string fichero = "";
            string localPath = HostingEnvironment.ApplicationPhysicalPath;
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                fichero = CntAriFaceLib.ficheroPdfDownloadAdm(administradorId,facturaId,localPath,conn);
                conn.Close();
            }
            return fichero;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string VerXml(int facturaId, int administradorId)
        {
            string fichero = "";
            string localPath = HostingEnvironment.ApplicationPhysicalPath;
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                fichero = CntAriFaceLib.ficheroXmlDownloadAdm(administradorId, facturaId, localPath, conn);
                conn.Close();
            }
            return fichero;
        }

        #endregion
    }
}