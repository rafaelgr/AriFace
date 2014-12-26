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

namespace FaceWebCli
{
    public partial class ParametrosApi : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        #region WebMethods
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static Parametros GetParametros()
        {
            Parametros p = new Parametros();
            // obtener el nombre de aplicación
            p.TituloApp = ConfigurationSettings.AppSettings["titulo_app"];
            return p;
        }
        #endregion
    }
}