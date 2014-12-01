using System.Security.Cryptography.X509Certificates;
using AriFaceLib;
using FaceWebApi.SSPP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FaceWebApi
{
    public partial class CertificadoApi : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static IList<Certificado> GetCertificados()
        {
            IList<Certificado> lu = new List<Certificado>();
            X509Store store = new X509Store("My", StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);
            for (int i = 0; i < store.Certificates.Count; i++)
            {
                Certificado c = new Certificado();
                c.SerialNumber = store.Certificates[i].SerialNumber;
                c.FriendlyName = store.Certificates[i].FriendlyName;
                c.ExpirationDateString = store.Certificates[i].GetExpirationDateString();
                lu.Add(c);
            }
            return lu;
        }
    }
}