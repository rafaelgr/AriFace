using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AriFaceLib;
using FaceWebApi.SPP2;

namespace FaceWebApi
{
    public static class CntFaceApi
    {
        public static RespuestaFactura EnviarFactura(string certSn, string fE, string dA, string email)
        {
            RespuestaFactura rf = null;
            try
            {
                SenderFace sf = new SenderFace(certSn);
                SSPPResultadoEnviarFactura res = sf.EnviarFactura(fE, dA, email);
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
    }
}