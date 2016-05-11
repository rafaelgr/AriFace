using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AriFaceLib;
using FaceWebApi.SSPP;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.IO;

namespace FaceWebApi
{
    public static class CntFaceApi
    {
        public static string SendEnvios(string certSN, MySqlConnection conn)
        {
            string mens = "RESULTADO DE ENVIO MULTIPLE:";
            string mensEmail = "Los siguientes envíos han sido procesados: <br/>";
            // obtener todos los envíos disponibles
            IList<Envio> envios = CntAriFaceLib.GetEnvios(conn);
            foreach (Envio e in envios)
            {
                mensEmail += SendEnvio(e.ClienteId, e.DepartamentoId, certSN, conn);
            }
            string asunto = "[ARIFACE] Envios procesados";
            string cuerpo = mensEmail;
            CntAriFaceLib.SendEmailAdministrador(asunto, cuerpo, new ArrayList());
            mens += "Todos los envíos han sido procesados. Recibirá en su correo un mensaje con los resultado";
            return mens;
        }

        public static string SendEnvio(int clienteId, int departamentoId, string certSN, MySqlConnection conn)
        {
            // obtenemos los datos del envío
            Envio e = CntAriFaceLib.GetEnvio(clienteId, departamentoId, conn);
            // la lista de facturas
            IList<Factura> lf = CntAriFaceLib.GetFacturasEnvio(clienteId, departamentoId, conn);
            // leemos la plantilla para envio por correo
            Plantilla p = CntAriFaceLib.GetPlantilla(1, conn);
            // leemos el cliente
            Cliente c = CntAriFaceLib.GetCliente(clienteId, conn);
            // este será el mensaje de vuelta
            string mens = String.Format("ENVIO--> Cliente: {0} Departamento: {1} Desde: {2} Hasta: {3} TOTAL: {4:0.00} <br/>", e.ClienteNombre, e.DepartamentoNombre, e.StrFechaInicial, e.StrFechaFinal, e.Total);
            if (!e.EsFace)
            {
                // NO FACE: Es una simple notificación por correo electrónico.
                // obtenemos el correo electrónico al que hay que mandar 
                if (c.Email != null && c.Email != "")
                {
                    int i = 0;
                    ArrayList adjuntos = new ArrayList();
                    string fichero = "";
                    string repositorio = "";
                    string detalleFacturas = "";
                    // Ahora preparamos las facturas
                    foreach (Factura f in lf)
                    {
                        i++;
                        // Montar los adjuntos
                        repositorio = CntAriFaceLib.GetRepositorio(conn);
                        fichero = repositorio + CntAriFaceLib.NombreFicheroFactura(f, c) + ".pdf";
                        DateTime fechaFactura = new DateTime(int.Parse(f.StrFecha.Substring(0, 4)),
                            int.Parse(f.StrFecha.Substring(4, 2)),
                            int.Parse(f.StrFecha.Substring(6, 2)));
                        detalleFacturas += String.Format("Serie:<strong>{0}</strong> Número:<strong>{1}</strong> Fecha:<strong>{2:dd/MM/yyyy}</strong> Importe: <strong>{3:0.00}</strong><br/>",
                            f.Serie, f.NumFactura, fechaFactura, f.Total);
                        adjuntos.Add(fichero);
                    }
                    // Montamos el correo electrónico.
                    string asunto = "[ARIFACE] Notificación de facturas electrónicas";
                    string cuerpo = String.Format(p.Contenido, c.Nombre, detalleFacturas);
                    try
                    {
                        CntAriFaceLib.SendEmailCliente(c.Email, asunto, cuerpo, adjuntos);
                    }
                    catch (Exception ex)
                    {
                        mens += String.Format("Error correo electrónico: {0}", ex.Message);
                        return mens;
                    }
                    // ahora marcamos las facturas como enviadas
                    foreach (Factura f in lf)
                    {
                        CntAriFaceLib.MarcarFacturaEnviada(f.FacturaId, conn);
                    }
                    mens += String.Format("({0}) {1} facturas CORRECTAS <br/>", c.Email, i);
                }
                else
                {
                    mens += String.Format("El cliente {0} no tiene un correo electrónico, no se le ha podido notificar <br/>", c.Nombre);
                }
            }
            else
            {
                // Envio Face
                mens += "FACE (Envio a administración pública) <br/>";
                string detalleFacturas = "";
                foreach (Factura f in lf)
                {
                    string repositorio = CntAriFaceLib.GetRepositorio(conn);
                    string fichero = repositorio + CntAriFaceLib.NombreFicheroFactura(f, c) + ".xsig";
                    string ficheroPdf = repositorio + CntAriFaceLib.NombreFicheroFactura(f, c) + ".pdf";
                    string dirNotificacion = ConfigurationSettings.AppSettings["dirNotificacion"];
                    string email = ConfigurationSettings.AppSettings["mail_address"];
                    try
                    {
                        //RespuestaFactura rs = EnviarFactura(certSN, fichero, dirNotificacion, email);
                        RespuestaFactura rs = EnviarFacturaAdjunto(certSN, fichero, dirNotificacion, email, ficheroPdf, f.SistemaGdes);
                        CntAriFaceLib.MarcarFacturaEnviadaFace(f.FacturaId, rs.CodigoRegistro, rs.StrFechaRecepcion, conn);
                        DateTime fechaFactura = new DateTime(int.Parse(f.StrFecha.Substring(0, 4)),
                            int.Parse(f.StrFecha.Substring(4, 2)),
                            int.Parse(f.StrFecha.Substring(6, 2)));
                        detalleFacturas += String.Format("Serie:<strong>{0}</strong> Número:<strong>{1}</strong> Fecha:<strong>{2:dd/MM/yyyy}</strong> Importe: <strong>{3:0.00}</strong> REGISTRO:{4} <br/>",
                            f.Serie, f.NumFactura, fechaFactura, f.Total, rs.CodigoRegistro);
                        mens += String.Format("PROCESADA --> Serie:<strong>{0}</strong> Número:<strong>{1}</strong> Fecha:<strong>{2:dd/MM/yyyy}</strong> Importe: <strong>{3:0.00}</strong> REGISTRO:{4} <br/>",
                            f.Serie, f.NumFactura, fechaFactura, f.Total, rs.CodigoRegistro);
                    }
                    catch (Exception ex)
                    {
                        mens += String.Format("Error procesando factura {0}{1}: {2}", f.Serie, f.NumFactura, ex.Message);
                        return mens;
                    }
                }
            }
            mens += "---------------------------- <br/>";
            return mens;
        }

        public static string SendEnvioFactura(int facturaId, string certSN, MySqlConnection conn)
        {
            // obtenemos los datos del envío
            // la lista de facturas
            Factura f = CntAriFaceLib.GetFactura(facturaId, conn);
            // leemos la plantilla para envio por correo
            Plantilla p = CntAriFaceLib.GetPlantilla(1, conn);
            // leemos el cliente
            Cliente c = CntAriFaceLib.GetCliente(f.ClienteId, conn);
            // este será el mensaje de vuelta
            string mens = String.Format("ENVIO--> Cliente: {0} Departamento: {1} Fecha: {2} TOTAL: {3:0.00} <br/>", f.ClienteNombre, f.Departamento, f.StrFecha, f.Total);
            if (c.CodOficinaContable == null || c.CodOficinaContable == "")
            {
                // NO FACE: Es una simple notificación por correo electrónico.
                // obtenemos el correo electrónico al que hay que mandar 
                if (c.Email != null && c.Email != "")
                {
                    int i = 0;
                    ArrayList adjuntos = new ArrayList();
                    string fichero = "";
                    string repositorio = "";
                    string detalleFacturas = "";
                    // Ahora preparamos las facturas
                    i++;
                    // Montar los adjuntos
                    repositorio = CntAriFaceLib.GetRepositorio(conn);
                    fichero = repositorio + CntAriFaceLib.NombreFicheroFactura(f, c) + ".pdf";
                    DateTime fechaFactura = new DateTime(int.Parse(f.StrFecha.Substring(0, 4)),
                        int.Parse(f.StrFecha.Substring(4, 2)),
                        int.Parse(f.StrFecha.Substring(6, 2)));
                    detalleFacturas += String.Format("Serie:<strong>{0}</strong> Número:<strong>{1}</strong> Fecha:<strong>{2:dd/MM/yyyy}</strong> Importe: <strong>{3:0.00}</strong><br/>",
                        f.Serie, f.NumFactura, fechaFactura, f.Total);
                    adjuntos.Add(fichero);
                    // Montamos el correo electrónico.
                    string asunto = "[ARIFACE] Notificación de facturas electrónicas";
                    string cuerpo = String.Format(p.Contenido, c.Nombre, detalleFacturas);
                    try
                    {
                        CntAriFaceLib.SendEmailCliente(c.Email, asunto, cuerpo, adjuntos);
                    }
                    catch (Exception ex)
                    {
                        mens += String.Format("Error correo electrónico: {0}", ex.Message);
                        return mens;
                    }
                    // ahora marcamos las facturas como enviadas
                    CntAriFaceLib.MarcarFacturaEnviada(f.FacturaId, conn);
                    mens += String.Format("({0}) {1} facturas CORRECTAS <br/>", c.Email, i);
                }
                else
                {
                    mens += String.Format("El cliente {0} no tiene un correo electrónico, no se le ha podido notificar <br/>", c.Nombre);
                }
            }
            else
            {
                // Envio Face
                mens += "FACE (Envio a administración pública) <br/>";
                string detalleFacturas = "";
                string repositorio = CntAriFaceLib.GetRepositorio(conn);
                string fichero = repositorio + CntAriFaceLib.NombreFicheroFactura(f, c) + ".xsig";
                string ficheroPdf = repositorio + CntAriFaceLib.NombreFicheroFactura(f, c) + ".pdf";
                string dirNotificacion = ConfigurationSettings.AppSettings["dirNotificacion"];
                string email = ConfigurationSettings.AppSettings["mail_address"];
                try
                {
                    //RespuestaFactura rs = EnviarFactura(certSN, fichero, dirNotificacion, email);
                    RespuestaFactura rs = EnviarFacturaAdjunto(certSN, fichero, dirNotificacion, email, ficheroPdf, f.SistemaGdes);
                    CntAriFaceLib.MarcarFacturaEnviadaFace(f.FacturaId, rs.CodigoRegistro, rs.StrFechaRecepcion, conn);
                    DateTime fechaFactura = new DateTime(int.Parse(f.StrFecha.Substring(0, 4)),
                        int.Parse(f.StrFecha.Substring(4, 2)),
                        int.Parse(f.StrFecha.Substring(6, 2)));
                    detalleFacturas += String.Format("Serie:<strong>{0}</strong> Número:<strong>{1}</strong> Fecha:<strong>{2:dd/MM/yyyy}</strong> Importe: <strong>{3:0.00}</strong> REGISTRO:{4} <br/>",
                        f.Serie, f.NumFactura, fechaFactura, f.Total, rs.CodigoRegistro);
                    mens += String.Format("PROCESADA --> Serie:<strong>{0}</strong> Número:<strong>{1}</strong> Fecha:<strong>{2:dd/MM/yyyy}</strong> Importe: <strong>{3:0.00}</strong> REGISTRO:{4} <br/>",
                        f.Serie, f.NumFactura, fechaFactura, f.Total, rs.CodigoRegistro);
                }
                catch (Exception ex)
                {
                    mens += String.Format("Error procesando factura {0}{1}: {2}", f.Serie, f.NumFactura, ex.Message);
                    return mens;
                }
            }
            mens += "---------------------------- <br/>";
            return mens;
        }

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
                //rf.PdfJustificante = res.?;
                rf.Seriefactura = res.factura.serieFactura;
                rf.CodUnidadTramitadora = res.factura.unidadTramitadora;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return rf;
        }

        public static RespuestaFactura EnviarFacturaAdjunto(string certSn, string fE, string dA, string email, string ficheroPdf, string sistema)
        {
            RespuestaFactura rf = null;
            try
            {
                SenderFace sf = new SenderFace(certSn);
                //SSPPResultadoEnviarFactura res = sf.EnviarFactura(fE, dA, email);
                EnviarFacturaResponse res = sf.EnviarFacturaAdjunto(fE, dA, email, ficheroPdf, sistema);
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
                //rf.PdfJustificante = res.?;
                rf.Seriefactura = res.factura.serieFactura;
                rf.CodUnidadTramitadora = res.factura.unidadTramitadora;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return rf;
        }
    }
}