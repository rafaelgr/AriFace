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
    public partial class EmpresaRaizApi : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        #region WebMethods
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static IList<EmpresaRaiz> GetEmpresasRaiz()
        {
            IList<EmpresaRaiz> ler = new List<EmpresaRaiz>();
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                ler = CntAriFaceLib.GetEmpresasRaiz(conn);
                conn.Close();
            }
            return ler;
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static IList<EmpresaRaiz> BuscarEmpresaRaiz(string aBuscar)
        {
            IList<EmpresaRaiz> lc = new List<EmpresaRaiz>();
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                lc = CntAriFaceLib.GetEmpresasRaiz(aBuscar, conn);
                conn.Close();
            }
            return lc;
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static EmpresaRaiz GetEmpresaRaizById(string nif)
        {
            EmpresaRaiz c = null;
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                c = CntAriFaceLib.GetEmpresaRaiz(nif, conn);
                conn.Close();
            }
            return c;
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static EmpresaRaiz DeleteEmpresaRaiz(string nif)
        {
            EmpresaRaiz c = null;
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                CntAriFaceLib.DeleteEmpresaRaiz(nif, conn);
                conn.Close();
            }
            return c;
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static EmpresaRaiz SetEmpresaRaiz(EmpresaRaiz empresa)
        {
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                empresa = CntAriFaceLib.SetEmpresaRaiz(empresa, conn);
                conn.Close();
            }
            return empresa;
        }
        #endregion
    }
}