using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Web.Script.Services;
using AriFaceLib;
using MySql.Data.MySqlClient;

namespace FaceWebApi
{
    public partial class AdministradorApi : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        #region WebMethods
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static Administrador GetAdministradorLogin(string login, string password)
        {
            try
            {
                Administrador a = null;
                // leer la cadena de conexión de los parámetros
                string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
                using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
                {
                    conn.Open();
                    a = CntAriFaceLib.GetAdministradorLogin(login, password, conn);
                    if (a != null)
                    {
                        string localPath = HostingEnvironment.ApplicationPhysicalPath;
                        CntAriFaceLib.PrepararDirectorio(a.AdministradorId, localPath);
                    }
                    conn.Close();
                }
                return a;
            }
            catch (Exception ex)
            {
                Administrador a2 = new Administrador();
                a2.Nombre = ex.Message;
                return a2;
            }
            
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static IList<Administrador> GetAdministradores()
        {
            IList<Administrador> la = new List<Administrador>();
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                la = CntAriFaceLib.GetAdministradores(conn);
                conn.Close();
            }
            return la;
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static IList<Administrador> BuscarAdministradores(string aBuscar)
        {
            IList<Administrador> la = new List<Administrador>();
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                la = CntAriFaceLib.GetAdministradores(aBuscar, conn);
                conn.Close();
            }
            return la;
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static Administrador GetAdministradorById(string id)
        {
            Administrador a = null;
            int mId = int.Parse(id);
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                a = CntAriFaceLib.GetAdministrador(mId, conn);
                conn.Close();
            }
            return a;
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static Administrador DeleteAdministrador(string id)
        {
            Administrador a = null;
            int mId = int.Parse(id);
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                CntAriFaceLib.DeleteAdministrador(mId, conn);
                conn.Close();
            }
            return a;
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static Administrador SetAdministrador(Administrador administrador)
        {
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                administrador = CntAriFaceLib.SetAdministrador(administrador, conn);
                conn.Close();
            }
            return administrador;
        }
        #endregion
    }
}