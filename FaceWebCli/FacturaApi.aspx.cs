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
    public partial class FacturaApi : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        #region WebMethods
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static IList<Factura> GetFacturas()
        {
            IList<Factura> lf = new List<Factura>();
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                lf = CntAriFaceLib.GetFacturas(conn);
                conn.Close();
            }
            return lf;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static IList<Factura> GetFacturasNoEnviadas()
        {
            IList<Factura> lf = new List<Factura>();
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                lf = CntAriFaceLib.GetFacturasNoEnviadas(conn);
                conn.Close();
            }
            return lf;
        }
        
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static IList<Factura> GetFacturasCliente(int clienteId)
        {
            IList<Factura> lf = new List<Factura>();
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                lf = CntAriFaceLib.GetFacturasCliente(clienteId, conn);
                conn.Close();
            }
            return lf;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static IList<Factura> GetFacturasUsuario(int usuarioId)
        {
            IList<Factura> lf = new List<Factura>();
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                // primero comprobamos qué permisos tiene el usuario
                // fijándonos en a qué empresa raiz, cliente y departamento pertenece
                Usuario u = CntAriFaceLib.GetUsuario(usuarioId, conn);
                if (u.ClienteId != 0)
                {
                    if (u.DepartamentoId != 0)
                    {
                        // departamento
                        lf = CntAriFaceLib.GetFacturasDepartamento(u.DepartamentoId, conn);
                    }
                    else
                    {
                        // cliente
                        lf = CntAriFaceLib.GetFacturasCliente(u.ClienteId, conn);
                    }
                }
                else
                {
                    // empresa raiz
                    lf = CntAriFaceLib.GetFacturasEmpresaRaiz(u.Nif, conn);
                }
                conn.Close();
            }
            return lf;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static PeriodoFactura GetFacturasUsuarioPeriodo(int usuarioId, int a, int q, int m, bool esCliente)
        {
            PeriodoFactura pf = new PeriodoFactura();
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                // primero comprobamos qué permisos tiene el usuario
                // fijándonos en a qué empresa raiz, cliente y departamento pertenece
                Usuario u = CntAriFaceLib.GetUsuario(usuarioId, conn);
                if (u.ClienteId != 0)
                {
                    if (u.DepartamentoId != 0)
                    {
                        // departamento
                        pf = CntAriFaceLib.GetFacturasDepartamentoPeriodo(a,q,m,u.DepartamentoId,esCliente,conn);
                    }
                    else
                    {
                        // cliente
                        pf = CntAriFaceLib.GetFacturasClientePeriodo(a,q,m,u.ClienteId,esCliente, conn);
                    }
                }
                else
                {
                    // empresa raiz
                    pf = CntAriFaceLib.GetFacturasEmpresaRaizPeriodo(a,q,m,u.Nif,esCliente, conn);
                }
                conn.Close();
            }
            return pf;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static Factura GetFacturaById(string id)
        {
            Factura f = null;
            int mId = int.Parse(id);
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                f = CntAriFaceLib.GetFactura(mId, conn);
                conn.Close();
            }
            return f;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string VerPdf(int facturaId, int usuarioId)
        {
            string fichero = "";
            string localPath = HostingEnvironment.ApplicationPhysicalPath;
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                fichero = CntAriFaceLib.ficheroPdfDownloadCli(usuarioId,facturaId,localPath,conn);
                conn.Close();
            }
            return fichero;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string VerXml(int facturaId, int usuarioId)
        {
            string fichero = "";
            string localPath = HostingEnvironment.ApplicationPhysicalPath;
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                fichero = CntAriFaceLib.ficheroXmlDownloadCli(usuarioId, facturaId, localPath, conn);
                conn.Close();
            }
            return fichero;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static IList<MiniUnidad> GetAnosFacturados(int usuarioId)
        {
            IList<MiniUnidad> lu = new List<MiniUnidad>();
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                // primero comprobamos qué permisos tiene el usuario
                // fijándonos en a qué empresa raiz, cliente y departamento pertenece
                Usuario u = CntAriFaceLib.GetUsuario(usuarioId, conn);
                lu = CntAriFaceLib.GetAnosFacturados(u.Nif, u.ClienteId, u.DepartamentoId, conn);
                conn.Close();
            }
            return lu;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static IList<MiniUnidad> GetTrimestres()
        {
            IList<MiniUnidad> lu = new List<MiniUnidad>();
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                // primero comprobamos qué permisos tiene el usuario
                // fijándonos en a qué empresa raiz, cliente y departamento pertenece
                lu = CntAriFaceLib.GetListaTrimestres();
                conn.Close();
            }
            return lu;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static IList<MiniUnidad> GetMeses(int t)
        {
            IList<MiniUnidad> lu = new List<MiniUnidad>();
            // leer la cadena de conexión de los parámetros
            string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
            using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
            {
                conn.Open();
                // primero comprobamos qué permisos tiene el usuario
                // fijándonos en a qué empresa raiz, cliente y departamento pertenece
                lu = CntAriFaceLib.GetListaMeses(t);
                conn.Close();
            }
            return lu;
        }


        #endregion
    }
}