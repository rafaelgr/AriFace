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


namespace FaceWebCli
{
    public partial class VersionApi : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        #region WebMethods
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string GetVersion()
        {
            string v = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(); 
            return String.Format("VRS {0}",v);
        }
        #endregion
    }
}