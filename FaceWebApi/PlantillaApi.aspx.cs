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
    public partial class PlantillaApi : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        #region WebMethods

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static Plantilla GetPlantilla(int plantillaId)
        {
            Plantilla p = null;
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                p = CntAriFaceLib.GetPlantilla(plantillaId, conn);
                conn.Close();
            }
            return p;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static IList<Plantilla> GetPlantillas()
        {
            IList<Plantilla> lp = new List<Plantilla>();
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                lp = CntAriFaceLib.GetPlantillas(conn);
                conn.Close();
            }
            return lp;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static Plantilla SetPlantilla(Plantilla plantilla)
        {
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                plantilla = CntAriFaceLib.SetPlantilla(plantilla, conn);
                conn.Close();
            }
            return plantilla;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static Plantilla DeletePlantilla(int plantillaId)
        {
            Plantilla p = null;
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                CntAriFaceLib.DeletePlantilla(plantillaId, conn);
                conn.Close();
            }
            return p;
        }    

        #endregion
    }
}