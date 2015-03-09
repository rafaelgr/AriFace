﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using FaceWebApi.SPP2;
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
                SSPPEstados resEstados = sf.ConsultarEstados(sistema);
                if (resEstados != null)
                {
                    XmlElement xel;
                    ArrayOfSSPPEstado estados = resEstados.estados;
                    for(int i = 0; i < estados.Any.Length; i++){
                        xel = estados.Any[i];
                        Estado e = new Estado();
                        e.Codigo = sf.LeerXmlElemento("codigo",xel.InnerXml);
                        e.Nombre = sf.LeerXmlElemento("nombre",xel.InnerXml);
                        e.Descripcion = sf.LeerXmlElemento("descripcion",xel.InnerXml);
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
                SSPPResultadoConsultarUnidades resUnidades = sf.ConsultarUnidades(sistema);
                if (resUnidades != null)
                {
                    XmlElement xel;
                    ArrayOfSSPPOrganoGestorUnidadTramitadora unidades = resUnidades.unidades;
                    w.WriteLine("GetUnidades (unidades obtenidas) -----------{0:dd/MM/yyyy hh:mm:ss}", DateTime.Now);
                    for (int i = 0; i < unidades.Any.Length; i++)
                    {
                        xel = unidades.Any[i];
                        Unidad u = new Unidad();
                        u.OrganoGestorCodigo = sf.LeerXmlElemento("codigo_dir", sf.LeerXmlElemento("organo_gestor", xel.InnerXml));
                        u.OrganoGestorNombre = sf.LeerXmlElemento("nombre", sf.LeerXmlElemento("organo_gestor", xel.InnerXml));
                        u.UnidadTramitadoraCodigo = sf.LeerXmlElemento("codigo_dir", sf.LeerXmlElemento("unidad_tramitadora", xel.InnerXml));
                        u.UnidadTramitadoraNombre = sf.LeerXmlElemento("nombre", sf.LeerXmlElemento("unidad_tramitadora", xel.InnerXml));
                        u.OficinaContableCodigo = sf.LeerXmlElemento("codigo_dir", sf.LeerXmlElemento("oficina_contable", xel.InnerXml));
                        u.OficinaContableNombre = sf.LeerXmlElemento("nombre", sf.LeerXmlElemento("oficina_contable", xel.InnerXml));
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
                SSPPResultadoEnviarFactura res = sf.EnviarFactura(fE, dA, email, sistema);
                rf = new RespuestaFactura();
                rf.CodigoRegistro = res.codigo_registro;
                rf.StrFechaRecepcion = res.fecha_recepcion;
                rf.IdentificadorEmisor = res.identificador_emisor;
                rf.NumeroFactura = res.numero_factura;
                rf.CodOficinaContable = res.oficina_contable;
                rf.CodOrganoGestor = res.organo_gestor;
                rf.PdfJustificante = res.pdf_justificante;
                rf.Seriefactura = res.serie_factura;
                rf.CodUnidadTramitadora = res.unidad_tramitadora;
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
            SSPPResultadoConsultarFactura res = sf.ConsultarFactura(codRegistro, sistema);
            if (res != null)
            {
                rcf.NumeroRegistro = res.numero_registro;
                rcf.AnulacionCodigo = res.anulacion.codigo_estado;
                rcf.AnulacionDescripcion = res.anulacion.descripcion_estado;
                rcf.AnulacionMotivo = res.anulacion.motivo_estado;
                rcf.TramitacionCodigo = res.tramitacion.codigo_estado;
                rcf.TramitacionDescripcion = res.tramitacion.descripcion_estado;
                rcf.TramitacionMotivo = res.tramitacion.motivo_estado;
            }
            SSPPResultadoAnularFactura ra;
            return rcf;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static RespuestaAnularFactura AnularFactura(string certSn, string codRegistro, string motivo, string sistema)
        {
            RespuestaAnularFactura raf = new RespuestaAnularFactura();
            SenderFace sf = new SenderFace(certSn);
            SSPPResultadoAnularFactura res = sf.AnularFactura(codRegistro, motivo, sistema);
            if (res != null)
            {
                raf = new RespuestaAnularFactura();
                raf.NumRegistro = res.numero_registro;
                raf.Mensaje = res.mensaje;
            }
            return raf;
        }
        

        #endregion 

    }
}