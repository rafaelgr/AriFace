using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AriFaceLib
{
    public class RespuestaFactura
    {
        private string codigoRegistro;

        public string CodigoRegistro
        {
            get { return codigoRegistro; }
            set { codigoRegistro = value; }
        }
        private string strFechaRecepcion;

        public string StrFechaRecepcion
        {
            get { return strFechaRecepcion; }
            set { strFechaRecepcion = value; }
        }
        private string identificadorEmisor;

        public string IdentificadorEmisor
        {
            get { return identificadorEmisor; }
            set { identificadorEmisor = value; }
        }
        private string numeroFactura;

        public string NumeroFactura
        {
            get { return numeroFactura; }
            set { numeroFactura = value; }
        }
        private string codOficinaContable;

        public string CodOficinaContable
        {
            get { return codOficinaContable; }
            set { codOficinaContable = value; }
        }
        private string codOrganoGestor;

        public string CodOrganoGestor
        {
            get { return codOrganoGestor; }
            set { codOrganoGestor = value; }
        }
        private string pdfJustificante;

        public string PdfJustificante
        {
            get { return pdfJustificante; }
            set { pdfJustificante = value; }
        }
        private string seriefactura;

        public string Seriefactura
        {
            get { return seriefactura; }
            set { seriefactura = value; }
        }
        private string codUnidadTramitadora;

        public string CodUnidadTramitadora
        {
            get { return codUnidadTramitadora; }
            set { codUnidadTramitadora = value; }
        }
    }
}
