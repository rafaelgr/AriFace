using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using FaceWebApi.SSPP;
using AriFaceLib;
using System.Xml;
using System.Diagnostics;
using System.IO;
using MySql.Data.MySqlClient;
using System.Threading;
using System.Security.Permissions;

namespace FaceWebApi
{
    public partial class FaceApi : System.Web.UI.Page
    {
       
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        #region WebForms Methods
        /// <summary>
        /// Consultar las unidades (servicio web)
        /// </summary>
        /// <param name="certSn">
        /// Número de serie del certificado con el que se quiere firmar
        /// </param>
        /// <returns>
        /// Lista con los objetos unidad enviados
        /// </returns>
        //[WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public static IList<Unidad> GetUnidades(string certSn)
        //{
        //    IList<Unidad> lu = new List<Unidad>();
        //    SenderFace sf = new SenderFace(certSn);
        //    SSPPResultadoConsultarUnidades resUnidades = sf.ConsultarUnidades();
        //    if (resUnidades != null)
        //    {
        //        foreach (SSPPOrganoGestorUnidadTramitadora unidad in resUnidades.unidades)
        //        {
        //            SSPPUnidadDir dirOrganoGestor = unidad.organo_gestor;
        //            SSPPUnidadDir dirUnidadTramitadora = unidad.unidad_tramitadora;
        //            SSPPUnidadDir dirOficinaContable = unidad.oficina_contable;
        //            Unidad u = new Unidad();
        //            u.OrganoGestorCodigo = dirOrganoGestor.codigo_dir;
        //            u.OrganoGestorNombre = dirOrganoGestor.nombre;
        //            u.UnidadTramitadoraCodigo = dirUnidadTramitadora.codigo_dir;
        //            u.UnidadTramitadoraNombre = dirUnidadTramitadora.nombre;
        //            u.OficinaContableCodigo = dirOficinaContable.codigo_dir;
        //            u.OficinaContableNombre = dirOficinaContable.nombre;
        //            lu.Add(u);
        //        }
        //    }
        //    return lu;
        //}

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static IList<Estado> GetEstados(string certSn)
        {
            StreamWriter w = File.AppendText("C:\\Intercambio\\log.txt");
            IList<Estado> lu = new List<Estado>();
            try
            {
                w.WriteLine("GetEstados-----------{0:dd/MM/yyyy hh:mm:ss}", DateTime.Now);
                string sistema = ConfigurationSettings.AppSettings["gdes_sistema"];
                SenderFace sf = new SenderFace(certSn);
                ConsultarEstadosResponse resEstados = sf.ConsultarEstados(sistema);
                if (resEstados != null)
                {
                    if (resEstados.resultado.codigo != "0")
                    {
                        Exception ex = new Exception(String.Format("RESPUESTA FACe: ({0}) {1}", resEstados.resultado.codigo, resEstados.resultado.descripcion));
                        throw ex;
                    }
                    Estado[] estados = resEstados.estados;
                    for(int i = 0; i < estados.Length; i++){
                        Estado e = estados[i];
                        //e.Codigo = sf.LeerXmlElemento("codigo",xel.InnerXml);
                        //e.Nombre = sf.LeerXmlElemento("nombre",xel.InnerXml);
                        //e.Descripcion = sf.LeerXmlElemento("descripcion",xel.InnerXml);
                        lu.Add(e);
                    }
                }
            }
            catch (Exception ex)
            {
                w.WriteLine("[{0:dd/MM/yyyy hh:mm:ss}] (EXCEPCION) {1}", DateTime.Now, ex.ToString());
                w.Close();
                throw ex;
            }
            w.Close();
            return lu;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static IList<Unidad> GetUnidades(string certSn)
        {
            StreamWriter w = File.AppendText("C:\\Intercambio\\log.txt");
            IList<Unidad> lu = new List<Unidad>();
            try
            {
                w.WriteLine("GetUnidades-----------{0:dd/MM/yyyy hh:mm:ss}", DateTime.Now);
                string sistema = ConfigurationSettings.AppSettings["gdes_sistema"];
                SenderFace sf = new SenderFace(certSn);
                ConsultarRelacionesResponse resUnidades = sf.ConsultarUnidades(sistema);
                if (resUnidades != null)
                {
                    if (resUnidades.resultado.codigo != "0")
                    {
                        Exception ex = new Exception(String.Format("RESPUESTA FACe: ({0}) {1}", resUnidades.resultado.codigo, resUnidades.resultado.descripcion));
                        throw ex;
                    }
                    OGUTOC[] unidades = resUnidades.relaciones;
                    w.WriteLine("GetUnidades (unidades obtenidas) -----------{0:dd/MM/yyyy hh:mm:ss}", DateTime.Now);
                    for (int i = 0; i < unidades.Length; i++)
                    {
                        OGUTOC u1 = unidades[i];
                        Unidad u = new Unidad();
                        u.OrganoGestorCodigo = u1.organoGestor.codigo;
                        u.OrganoGestorNombre = u1.organoGestor.nombre;
                        u.UnidadTramitadoraCodigo = u1.unidadTramitadora.codigo;
                        u.UnidadTramitadoraNombre = u1.unidadTramitadora.nombre;
                        u.OficinaContableCodigo = u1.oficinaContable.codigo;
                        u.OficinaContableNombre = u1.oficinaContable.nombre;
                        lu.Add(u);
                    }
                    // leer la cadena de conexión de los parámetros
                    string connectionString = ConfigurationManager.ConnectionStrings["FacElec"].ConnectionString;
                    using (MySqlConnection conn = CntAriFaceLib.GetConnection(connectionString))
                    {
                        conn.Open();
                        try
                        {
                            CntAriFaceLib.SetUnidades(lu, conn);
                        }
                        catch (ThreadAbortException ex)
                        {
                            Thread.ResetAbort();
                        }
                        conn.Close();
                    }
                    w.WriteLine("GetUnidades (unidades guardadas) -----------{0:dd/MM/yyyy hh:mm:ss}", DateTime.Now);
                }
            }
            catch (Exception ex)
            {
                w.WriteLine("[{0:dd/MM/yyyy hh:mm:ss}] (EXCEPCION) {1}", DateTime.Now, ex.ToString());
                w.Close();
                throw ex;
            }
            w.Close();
            return lu;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static RespuestaFactura EnviarFactura(string certSn, string fE, string dA, string email, string sistema)
        {
            RespuestaFactura rf = null;
            try
            {
                SenderFace sf = new SenderFace(certSn);
                EnviarFacturaResponse res = sf.EnviarFactura(fE, dA, email, sistema);
                if (res.resultado.codigo != "0")
                {
                    Exception ex = new Exception(String.Format("RESPUESTA FACe: ({0}) {1}", res.resultado.codigo, res.resultado.descripcion));
                    throw ex;
                }
                rf = new RespuestaFactura();
                rf.CodigoRegistro = res.factura.numeroRegistro;
                rf.StrFechaRecepcion = res.factura.fechaRecepcion;
                rf.IdentificadorEmisor = res.factura.identificadorEmisor;
                rf.NumeroFactura = res.factura.numeroFactura;
                rf.CodOficinaContable = res.factura.oficinaContable;
                rf.CodOrganoGestor = res.factura.organoGestor;
                //rf.PdfJustificante = res.factura.?;
                rf.Seriefactura = res.factura.serieFactura;
                rf.CodUnidadTramitadora = res.factura.unidadTramitadora;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return rf;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static RespuestaConsultaFactura ConsultaFactura(string certSn, string codRegistro, string sistema)
        {
            RespuestaConsultaFactura rcf = new RespuestaConsultaFactura();
            SenderFace sf = new SenderFace(certSn);
            ConsultarFacturaResponse res = sf.ConsultarFactura(codRegistro, sistema);
            if (res != null)
            {
                if (res.resultado.codigo != "0")
                {
                    Exception ex = new Exception(String.Format("RESPUESTA FACe: ({0}) {1}", res.resultado.codigo, res.resultado.descripcion));
                    throw ex;
                }
                rcf.NumeroRegistro = res.factura.numeroRegistro;
                rcf.AnulacionCodigo = res.factura.anulacion.codigo;
                rcf.AnulacionDescripcion = res.factura.anulacion.descripcion;
                rcf.AnulacionMotivo = res.factura.anulacion.motivo;
                rcf.TramitacionCodigo = res.factura.tramitacion.codigo;
                rcf.TramitacionDescripcion = res.factura.tramitacion.descripcion;
                rcf.TramitacionMotivo = res.factura.tramitacion.motivo;
            }
            AnularFactura ra;
            return rcf;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static RespuestaAnularFactura AnularFactura(string certSn, string codRegistro, string motivo, string sistema)
        {
            RespuestaAnularFactura raf = new RespuestaAnularFactura();
            SenderFace sf = new SenderFace(certSn);
            AnularFacturaResponse res = sf.AnularFactura(codRegistro, motivo, sistema);
            if (res != null)
            {
                if (res.resultado.codigo != "0")
                {
                    Exception ex = new Exception(String.Format("RESPUESTA FACe: ({0}) {1}", res.resultado.codigo, res.resultado.descripcion));
                    throw ex;
                }
                raf = new RespuestaAnularFactura();
                raf.NumRegistro = res.factura.numeroRegistro;
                raf.Mensaje = res.factura.mensaje;
            }
            return raf;
        }
        

        #endregion 

    }
}