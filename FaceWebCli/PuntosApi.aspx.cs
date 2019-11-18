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

namespace FaceWebCli
{
    public partial class PuntosApi : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        #region WebMethods
        
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static ClientePuntos GetClientePuntos(int codclien)
        {
            
            ClientePuntos cp = new ClientePuntos();
            string usaPuntos = ConfigurationManager.AppSettings["usaPuntos"];
            if (usaPuntos != "true") return cp;

            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["ArigesPuntos"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                cp = CntAriFaceLib.GetClientePuntos(codclien, conn);
                conn.Close();
            }
            return cp;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static IList<Punto> GetPuntosCliente(int codclien)
        {
            IList<Punto> lp = new List<Punto>();
            string usaPuntos = ConfigurationManager.AppSettings["usaPuntos"];
            if (usaPuntos != "true") return lp;

            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["ArigesPuntos"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                lp = CntAriFaceLib.GetPuntosCliente(codclien, conn);
                conn.Close();
            }
            return lp;
        }

        #endregion
    }
}