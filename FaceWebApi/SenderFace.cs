using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FaceWebApi.SPP2;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Web.Services3;
using Microsoft.Web.Services3.Security;
using Microsoft.Web.Services3.Security.Tokens;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

namespace FaceWebApi
{
    /// <summary>
    /// Conector con los servicios Face
    /// </summary>
    public class SenderFace
    {
        private SSPPWebServiceProxyService objSender;
        private string strNumeroSerie;
        private X509Certificate2 objX509;

        public SenderFace(string numSerie)
        {
            strNumeroSerie = numSerie;
            objSender = new SSPPWebServiceProxyService();
            objSender.RequireMtom = false;
        }

        #region Funciones auxiliares
        private X509SecurityToken GetSecurityToken()
        {
            X509SecurityToken securityToken = null;
            X509Store store = new X509Store("My", StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            this.objX509 = null;

            try
            {
                for (int i = 0; i < store.Certificates.Count; i++)
                {
                    if (store.Certificates[i].SerialNumber.Equals(this.strNumeroSerie))
                    {
                        this.objX509 = (X509Certificate2)store.Certificates[i];
                    }
                }
                if (this.objX509 == null)
                {
                    throw new Exception("No se ha econtrado el certificado " + this.strNumeroSerie);
                }
                else
                {
                    securityToken = new X509SecurityToken(this.objX509);
                }
            }
            finally
            {
                if (store != null) store.Close();
            }
            return securityToken;
        }

        private void FirmarEnvio()
        {
            MessageSignature sig;
            X509SecurityToken sigToken = GetSecurityToken();
            SoapContext rqContext = objSender.RequestSoapContext;

            objSender.RequestSoapContext.Security.Tokens.Add(sigToken);
            sig = new MessageSignature(sigToken);
            sig.SignatureOptions = SignatureOptions.IncludeSoapBody | SignatureOptions.IncludeTimestamp;
            objSender.RequestSoapContext.Security.Elements.Add(sig);
        }
        #endregion

        #region Mensajes a FACE
        public SSPPEstados ConsultarEstados()
        {
            SSPPEstados res = null;
            try
            {
                FirmarEnvio();
                res = objSender.consultarEstados();
                //TODO: Hay que añadir algo a correcto?
            }
            catch (Exception ex)
            {
                throw ex;
                res = null;
                //TODO: tratar el error
            }
            return res;
        }

        public SSPPResultadoConsultarUnidades ConsultarUnidades()
        {
            SSPPResultadoConsultarUnidades res = null;
            try
            {
                FirmarEnvio();
                res = objSender.consultarUnidades();
            }
            catch (Exception ex)
            {
                res = null;
            }
            return res;
        }
        public SSPPResultadoEnviarFactura EnviarFactura(string pathFacturae, string carpetaAcuseRecibo, string correo)
        {
            SSPPFactura req;
            SSPPResultadoEnviarFactura res;
            ClsEncoder64 encoder64;
            string expediente = string.Empty;
            bool blnTry = true;
            if (System.IO.File.Exists(pathFacturae) == false) blnTry = false;
            if (System.IO.Directory.Exists(carpetaAcuseRecibo) == false) blnTry = false;
            if (!blnTry)
            {
                // O no existe en fichero o la carpeta en la que se encuentra
                throw new Exception(String.Format("El fichero {0} o la carpeta {1} no existen.",pathFacturae,carpetaAcuseRecibo));
                return null;
            }
            encoder64 = new ClsEncoder64();
            req = new SSPPFactura();
            req.fichero_factura = new SSPPFicheroFactura();
            req.fichero_factura.factura = encoder64.EncodeTo64File(pathFacturae);
            req.fichero_factura.mime = "application/xml";
            req.fichero_factura.nombre = "signed.xml";
            req.correo = correo;
            try
            {
                FirmarEnvio();
                res = objSender.enviarFactura(req);
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public SSPPResultadoEnviarFactura EnviarFacturaAdjunto(string pathFacturae, string carpetaAcuseRecibo, string correo, string pathPDF)
        {
            SSPPFactura req;
            SSPPResultadoEnviarFactura res;
            ClsEncoder64 encoder64;
            string expediente = string.Empty;
            bool blnTry = true;
            if (System.IO.File.Exists(pathFacturae) == false) blnTry = false;
            if (System.IO.File.Exists(pathPDF) == false) blnTry = false;
            if (System.IO.Directory.Exists(carpetaAcuseRecibo) == false) blnTry = false;
            if (!blnTry)
            {
                // O no existe en fichero o la carpeta en la que se encuentra
                throw new Exception(String.Format("El fichero {0} o la carpeta {1} no existen.", pathFacturae, carpetaAcuseRecibo));
                return null;
            }
            encoder64 = new ClsEncoder64();
            req = new SSPPFactura();
            req.fichero_factura = new SSPPFicheroFactura();
            req.fichero_factura.factura = encoder64.EncodeTo64File(pathFacturae);
            req.fichero_factura.mime = "application/xml";
            req.fichero_factura.nombre = "signed.xml";
            req.correo = correo;
            // manejo de anexos
            AriFaceLib.item item = new AriFaceLib.item();
            //byte[] pdfBytes = File.ReadAllBytes(pathPDF);
            item.anexo = encoder64.EncodeTo64File(pathPDF);
            //item.anexo = pdfBytes.ToString();
            item.nombre = pathPDF.Substring(pathPDF.LastIndexOf("\\") + 1);
            item.mime = "application/pdf";
            //
            XmlDocument doc = new XmlDocument();
            using (XmlWriter writer = doc.CreateNavigator().AppendChild())
            {
                new XmlSerializer(item.GetType()).Serialize(writer, item);
            }
            XmlElement ele = doc.DocumentElement;
            req.ficheros_anexos = new ArrayOfSSPPFicheroAnexo();
            req.ficheros_anexos.Any = new XmlElement[1];
            req.ficheros_anexos.Any[0] = ele;
            //
            try
            {
                FirmarEnvio();
                res = objSender.enviarFactura(req);
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public SSPPResultadoConsultarFactura ConsultarFactura(string numRegistro)
        {
            SSPPResultadoConsultarFactura res = null;
            try
            {
                FirmarEnvio();
                res = objSender.consultarFactura(numRegistro);
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public SSPPResultadoAnularFactura AnularFactura(string numRegistro, string motivo)
        {
            SSPPResultadoAnularFactura res = null;
            try
            {
                FirmarEnvio();
                res = objSender.anularFactura(numRegistro, motivo);
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        public string LeerXmlElemento(string tg, string xml)
        {
            string iniTg = String.Format("<{0}>", tg);
            string endTg = String.Format("</{0}>", tg);
            int pos1 = xml.IndexOf(iniTg);
            int pos2 = xml.IndexOf(endTg);
            string valor = xml.Substring(pos1 + iniTg.Length, pos2 - (pos1 + iniTg.Length));
            return valor;
        }

    }
}