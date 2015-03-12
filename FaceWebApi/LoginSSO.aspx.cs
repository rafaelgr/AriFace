using System.Configuration;
using System.Web.Hosting;
using AriFaceLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;

namespace FaceWebApi
{
    public partial class LoginSSO : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // comprobar el usuario logado
            //WindowsIdentity wi = System.Security.Principal.WindowsIdentity.GetCurrent();
            WindowsIdentity wi = (WindowsIdentity)Page.User.Identity;
            if (wi.User == null)
            {
                Response.Redirect("login.html");
            }
            else
            {
                Administrador a = null;
                // leer la cadena de conexión de los parámetros
                string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
                string login = wi.User.Value;
                string password = wi.User.Value;
                using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
                {
                    conn.Open();
                    a = CntAriFaceLib.GetAdministradorLogin(login, password, conn);
                    if (a != null)
                    {
                        string localPath = HostingEnvironment.ApplicationPhysicalPath;
                        CntAriFaceLib.PrepararDirectorio(a.AdministradorId, localPath);
                        // Grabar la cookie para que funcione
                        string js = String.Format("adminCookie('{0}', '{1}', '{2}');",a.AdministradorId, a.Nombre, a.Login);
                        ScriptManager.RegisterStartupScript(this, GetType(), "adminCookie", js, true);
                        //Response.Redirect("Index.html");
                    }
                    else
                    {
                        Response.Redirect("login.html");
                    }
                    conn.Close();
                }
            }
        }
    }
}