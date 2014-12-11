using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Web.Script.Services;
using AriFaceLib;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace FaceWebApi
{
    public partial class UnidadApi : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        #region WebMethods

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static Unidad GetUnidad(string organoGestorCodigo, string unidadTramitadoraCodigo, string oficinaContableCodigo)
        {
            Unidad u = null;
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                u = CntAriFaceLib.GetUnidad(organoGestorCodigo, unidadTramitadoraCodigo, oficinaContableCodigo, conn);
                conn.Close();
            }
            return u;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static IList<Unidad> GetUnidades()
        {
            IList<Unidad> lu = new List<Unidad>();
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                lu = CntAriFaceLib.GetUnidades(conn);
                conn.Close();
            }
            return lu;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static Unidad SetUnidad(Unidad unidad)
        {
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                unidad = CntAriFaceLib.SetUnidad(unidad, conn);
                conn.Close();
            }
            return unidad;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static void SetUnidades(IList<Unidad> unidades)
        {
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                CntAriFaceLib.SetUnidades(unidades, conn);
                conn.Close();
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static Unidad DeleteUnidad(string organoGestorCodigo, string unidadTramitadoraCodigo, string oficinaContableCodigo)
        {
            Unidad u = null;
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                CntAriFaceLib.DeleteUnidad(organoGestorCodigo, unidadTramitadoraCodigo, oficinaContableCodigo, conn);
                conn.Close();
            }
            return u;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static IList<MiniUnidad> GetUtOfOg(string organoGestorCodigo)
        {
            IList<MiniUnidad> lmu = new List<MiniUnidad>();
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                lmu = CntAriFaceLib.GetUtOfOg(organoGestorCodigo, conn);
                conn.Close();
            }
            return lmu;
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static IList<MiniUnidad> GetOcOfUt(string organoGestorCodigo, string unidadTramitadoraCodigo)
        {
            IList<MiniUnidad> lmu = new List<MiniUnidad>();
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                lmu = CntAriFaceLib.GetOcOfUt(organoGestorCodigo, unidadTramitadoraCodigo, conn);
                conn.Close();
            }
            return lmu;
        }

        #endregion
    }
}