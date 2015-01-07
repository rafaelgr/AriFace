using System.Configuration;
using AriFaceLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;

namespace FaceWebApi
{
    public partial class EstadisticaApi : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static Estadistica GetEstadistica()
        {
            Estadistica e = null;
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                e = CntAriFaceLib.GetEstadistica(conn);
                conn.Close();
            }
            return e;
        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static IList<EstFacMes> GetNumFacMes(int anyo)
        {
            IList<EstFacMes> le = new List<EstFacMes>();
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                le = CntAriFaceLib.GetNumFacMes(anyo, conn);
                conn.Close();
            }
            return le;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static IList<MiniUnidad> GetAnosFacturados()
        {
            IList<MiniUnidad> lu = new List<MiniUnidad>();
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                // primero comprobamos qué permisos tiene el usuario
                // fijándonos en a qué empresa raiz, cliente y departamento pertenece
                lu = CntAriFaceLib.GetAnosFacturados(conn);
                conn.Close();
            }
            return lu;
        }
    }
}