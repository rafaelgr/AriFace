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
    public partial class ClienteApi : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        #region WebMethods
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static IList<Cliente> GetClientes()
        {
            IList<Cliente> lc = new List<Cliente>();
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                lc = CntAriFaceLib.GetClientes(conn);
                conn.Close();
            }
            return lc;
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static IList<Cliente> BuscarCliente(string aBuscar)
        {
            IList<Cliente> lc = new List<Cliente>();
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                lc = CntAriFaceLib.GetClientes(aBuscar, conn);
                conn.Close();
            }
            return lc;
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static Cliente GetClienteById(string id)
        {
            Cliente c = null;
            int mId = int.Parse(id);
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                c = CntAriFaceLib.GetCliente(mId, conn);
                conn.Close();
            }
            return c;
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static Cliente DeleteCliente(string id)
        {
            Cliente c = null;
            int mId = int.Parse(id);
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                CntAriFaceLib.DeleteCliente(mId, conn);
                conn.Close();
            }
            return c;
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static Cliente SetCliente(Cliente cliente)
        {
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                cliente = CntAriFaceLib.SetCliente(cliente, conn);
                conn.Close();
            }
            return cliente;
        }
        #endregion
    }
}