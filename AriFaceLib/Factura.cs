using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AriFaceLib
{
    public class Factura
    {
        private int facturaId;

        public int FacturaId
        {
            get { return facturaId; }
            set { facturaId = value; }
        }
        private string sistema;

        public string Sistema
        {
            get { return sistema; }
            set { sistema = value; }
        }
        private string serie;

        public string Serie
        {
            get { return serie; }
            set { serie = value; }
        }
        private int numFactura;

        public int NumFactura
        {
            get { return numFactura; }
            set { numFactura = value; }
        }
        private string strFecha;

        public string StrFecha
        {
            get { return strFecha; }
            set { strFecha = value; }
        }
        private bool esDeCliente;

        public bool EsDeCliente
        {
            get { return esDeCliente; }
            set { esDeCliente = value; }
        }
        private string letraProveedor;

        public string LetraProveedor
        {
            get { return letraProveedor; }
            set { letraProveedor = value; }
        }
        private string codTipom;

        public string CodTipom
        {
            get { return codTipom; }
            set { codTipom = value; }
        }
        private int clienteId;

        public int ClienteId
        {
            get { return clienteId; }
            set { clienteId = value; }
        }
        private string clienteNombre;

        public string ClienteNombre
        {
            get { return clienteNombre; }
            set { clienteNombre = value; }
        }
        private decimal baseIva;

        public decimal BaseIva
        {
            get { return baseIva; }
            set { baseIva = value; }
        }
        private decimal cuotaIva;

        public decimal CuotaIva
        {
            get { return cuotaIva; }
            set { cuotaIva = value; }
        }
        private decimal retencion;

        public decimal Retencion
        {
            get { return retencion; }
            set { retencion = value; }
        }
        private decimal total;

        public decimal Total
        {
            get { return total; }
            set { total = value; }
        }
        private decimal aportacion;

        public decimal Aportacion
        {
            get { return aportacion; }
            set { aportacion = value; }
        }

        private bool nueva;

        public bool Nueva
        {
            get { return nueva; }
            set { nueva = value; }
        }
    }
}
