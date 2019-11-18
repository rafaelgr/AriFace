using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using FaceWebApi.SSPP;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Web.Services3;
using Microsoft.Web.Services3.Security;
using Microsoft.Web.Services3.Security.Tokens;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Security.Cryptography;

namespace FaceWebApi
{
    /// <summary>
    /// Conector con los servicios Face
    /// </summary>
    public class SenderFace
    {
        private Service objSender;
        private string strNumeroSerie;
        private X509Certificate2 objX509;
        // desencriptador
        static readonly string PasswordHash = "P@@Sw0rd";
        static readonly string SaltKey = "S@LT&KEY";
        static readonly string VIKey = "@1B2c3D4e5F6g7H8";

        public SenderFace(string numSerie)
        {
            strNumeroSerie = numSerie;
            objSender = new Service();
            // objSender.RequireMtom = false;
        }

        #region Funciones auxiliares
        private X509SecurityToken GetSecurityToken2()
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

        private X509SecurityToken GetSecurityToken()
        {
            X509Certificate2 cer2 = new X509Certificate2();
            string cert_file = ConfigurationSettings.AppSettings["cert_file"];
            string cert_pass = ConfigurationSettings.AppSettings["cert_pass"];
            cer2.Import(cert_file,cert_pass,X509KeyStorageFlags.MachineKeySet);
            //cer2.Import(cert_file, cert_pass, X509KeyStorageFlags.DefaultKeySet);
            this.objX509 = cer2;
            X509SecurityToken securityToken = null;
            securityToken = new X509SecurityToken(this.objX509);
            return securityToken;
        }

        private X509SecurityToken GetSecurityTokenMulti(string nif)
        {
            X509Certificate2 cer2 = new X509Certificate2();
            string cert_file = ConfigurationSettings.AppSettings["cert_file"];
            string cert_pass = ConfigurationSettings.AppSettings["cert_pass"];
            string cert2_file = ConfigurationSettings.AppSettings[nif + "_file"];
            string cert2_pass = ConfigurationSettings.AppSettings[nif + "_pass"];
            if (cert2_file != null && cert2_pass != null)
            {
                cert_file = cert2_file;
                cert_pass = cert2_pass;
            }
            cer2.Import(cert_file, cert_pass, X509KeyStorageFlags.MachineKeySet);
            //cer2.Import(cert_file, cert_pass, X509KeyStorageFlags.DefaultKeySet);
            this.objX509 = cer2;
            X509SecurityToken securityToken = null;
            securityToken = new X509SecurityToken(this.objX509);
            return securityToken;
        }


        private X509SecurityToken GetSecurityTokenGdes(string sistemaGdes)
        {
            X509Certificate2 cer2 = new X509Certificate2();
            string cert_file = String.Format(ConfigurationSettings.AppSettings["cert_file"], sistemaGdes);
            string cert_pass = Decrypt(ConfigurationSettings.AppSettings["cert_pass"]);
            cer2.Import(cert_file, cert_pass, X509KeyStorageFlags.MachineKeySet);
            //cer2.Import(cert_file, cert_pass, X509KeyStorageFlags.DefaultKeySet);
            this.objX509 = cer2;
            X509SecurityToken securityToken = null;
            securityToken = new X509SecurityToken(this.objX509);
            return securityToken;
        }

        private void FirmarEnvio(string sistema)
        {
            MessageSignature sig;
            X509SecurityToken sigToken;
            if (sistema == "")
            {
                sigToken = GetSecurityToken();
            }
            else
            {
                sigToken = GetSecurityTokenGdes(sistema);
            }

            SoapContext rqContext = objSender.RequestSoapContext;
            

            objSender.RequestSoapContext.Security.Tokens.Add(sigToken);
            sig = new MessageSignature(sigToken);
            sig.SignatureOptions = SignatureOptions.IncludeSoapBody | SignatureOptions.IncludeTimestamp;
            objSender.RequestSoapContext.Security.Elements.Add(sig);
        }

        private void FirmarEnvioMulti(string sistema, string nif)
        {
            MessageSignature sig;
            X509SecurityToken sigToken;
            if (sistema == "")
            {
                sigToken = GetSecurityTokenMulti(nif);
            }
            else
            {
                sigToken = GetSecurityTokenGdes(sistema);
            }

            SoapContext rqContext = objSender.RequestSoapContext;


            objSender.RequestSoapContext.Security.Tokens.Add(sigToken);
            sig = new MessageSignature(sigToken);
            sig.SignatureOptions = SignatureOptions.IncludeSoapBody | SignatureOptions.IncludeTimestamp;
            objSender.RequestSoapContext.Security.Elements.Add(sig);
        }
        #endregion

        #region Mensajes a FACE
        public ConsultarEstadosResponse ConsultarEstados(string sistema)
        {
            ConsultarEstadosResponse res = null;
            try
            {
                FirmarEnvio(sistema);
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

        public ConsultarEstadosResponse ConsultarEstadosMulti(string sistema, string nif)
        {
            ConsultarEstadosResponse res = null;
            try
            {
                FirmarEnvioMulti(sistema, nif);
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

        public ConsultarRelacionesResponse ConsultarUnidades(string sistema)
        {
            ConsultarRelacionesResponse res = null;
            try
            {
                FirmarEnvio(sistema);
                res = objSender.consultarUnidades();
            }
            catch (Exception ex)
            {
                res = null;
            }
            return res;
        }

        public ConsultarRelacionesResponse ConsultarUnidadesMulti(string sistema, string nif)
        {
            ConsultarRelacionesResponse res = null;
            try
            {
                FirmarEnvioMulti(sistema, nif);
                res = objSender.consultarUnidades();
            }
            catch (Exception ex)
            {
                res = null;
            }
            return res;
        }

        public EnviarFacturaResponse EnviarFactura(string pathFacturae, string carpetaAcuseRecibo, string correo, string sistema)
        {
            EnviarFacturaRequest req;
            EnviarFacturaResponse res;
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
            req = new EnviarFacturaRequest();
            req.factura = new FacturaFile();
            req.factura.factura = encoder64.EncodeTo64File(pathFacturae);
            req.factura.mime = "application/xml";
            req.factura.nombre = "signed.xml";
            req.correo = correo;
            try
            {
                FirmarEnvio(sistema);
                res = objSender.enviarFactura(req);
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public EnviarFacturaResponse EnviarFacturaMulti(string pathFacturae, string carpetaAcuseRecibo, string correo, string sistema, string nif)
        {
            EnviarFacturaRequest req;
            EnviarFacturaResponse res;
            ClsEncoder64 encoder64;
            string expediente = string.Empty;
            bool blnTry = true;
            if (System.IO.File.Exists(pathFacturae) == false) blnTry = false;
            if (System.IO.Directory.Exists(carpetaAcuseRecibo) == false) blnTry = false;
            if (!blnTry)
            {
                // O no existe en fichero o la carpeta en la que se encuentra
                throw new Exception(String.Format("El fichero {0} o la carpeta {1} no existen.", pathFacturae, carpetaAcuseRecibo));
                return null;
            }
            encoder64 = new ClsEncoder64();
            req = new EnviarFacturaRequest();
            req.factura = new FacturaFile();
            req.factura.factura = encoder64.EncodeTo64File(pathFacturae);
            req.factura.mime = "application/xml";
            req.factura.nombre = "signed.xml";
            req.correo = correo;
            try
            {
                FirmarEnvioMulti(sistema, nif);
                res = objSender.enviarFactura(req);
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public EnviarFacturaResponse EnviarFacturaAdjunto(string pathFacturae, string carpetaAcuseRecibo, string correo, string pathPDF, string sistema)
        {
            EnviarFacturaRequest req;
            EnviarFacturaResponse res;
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
            req = new EnviarFacturaRequest();
            req.factura = new FacturaFile();
            req.factura.factura = encoder64.EncodeTo64File(pathFacturae);
            req.factura.mime = "application/xml";
            req.factura.nombre = "signed.xml";
            req.correo = correo;
            // manejo de anexos
            AnexoFile item = new AnexoFile();
            item.anexo = encoder64.EncodeTo64File(pathPDF);
            item.nombre = pathPDF.Substring(pathPDF.LastIndexOf("\\") + 1);
            item.mime = "application/pdf";
            //
            XmlDocument doc = new XmlDocument();
            using (XmlWriter writer = doc.CreateNavigator().AppendChild())
            {
                new XmlSerializer(item.GetType()).Serialize(writer, item);
            }
            XmlElement ele = doc.DocumentElement;
            AnexoFile af = new AnexoFile();
            
            req.anexos = new AnexoFile[1];
            req.anexos[0] = item;
            //
            try
            {
                FirmarEnvio(sistema);
                res = objSender.enviarFactura(req);
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public EnviarFacturaResponse EnviarFacturaAdjuntoMulti(string pathFacturae, string carpetaAcuseRecibo, string correo, string pathPDF, string sistema, string nif)
        {
            EnviarFacturaRequest req;
            EnviarFacturaResponse res;
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
            req = new EnviarFacturaRequest();
            req.factura = new FacturaFile();
            req.factura.factura = encoder64.EncodeTo64File(pathFacturae);
            req.factura.mime = "application/xml";
            req.factura.nombre = "signed.xml";
            req.correo = correo;
            // manejo de anexos
            AnexoFile item = new AnexoFile();
            item.anexo = encoder64.EncodeTo64File(pathPDF);
            item.nombre = pathPDF.Substring(pathPDF.LastIndexOf("\\") + 1);
            item.mime = "application/pdf";
            //
            XmlDocument doc = new XmlDocument();
            using (XmlWriter writer = doc.CreateNavigator().AppendChild())
            {
                new XmlSerializer(item.GetType()).Serialize(writer, item);
            }
            XmlElement ele = doc.DocumentElement;
            AnexoFile af = new AnexoFile();

            req.anexos = new AnexoFile[1];
            req.anexos[0] = item;
            //
            try
            {
                FirmarEnvioMulti(sistema, nif);
                res = objSender.enviarFactura(req);
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ConsultarFacturaResponse ConsultarFactura(string numRegistro, string sistema)
        {
            ConsultarFacturaResponse res = null;
            try
            {
                FirmarEnvio(sistema);
                res = objSender.consultarFactura(numRegistro);
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ConsultarFacturaResponse ConsultarFacturaMulti(string numRegistro, string sistema, string nif)
        {
            ConsultarFacturaResponse res = null;
            try
            {
                FirmarEnvioMulti(sistema, nif);
                res = objSender.consultarFactura(numRegistro);
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public AnularFacturaResponse AnularFactura(string numRegistro, string motivo, string sistema)
        {
            AnularFacturaResponse res = null;
            try
            {
                FirmarEnvio(sistema);
                res = objSender.anularFactura(numRegistro, motivo);
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public AnularFacturaResponse AnularFacturaMulti(string numRegistro, string motivo, string sistema, string nif)
        {
            AnularFacturaResponse res = null;
            try
            {
                FirmarEnvioMulti(sistema, nif);
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

        public static string Decrypt(string encryptedText)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

            var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
        }

    }
}