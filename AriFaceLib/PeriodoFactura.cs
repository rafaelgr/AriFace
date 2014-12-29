using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AriFaceLib
{
    public class PeriodoFactura
    {
        private decimal sumBase;

        public decimal SumBase
        {
            get { return sumBase; }
            set { sumBase = value; }
        }
        private decimal sumCuota;

        public decimal SumCuota
        {
            get { return sumCuota; }
            set { sumCuota = value; }
        }
        private decimal sumRetencion;

        public decimal SumRetencion
        {
            get { return sumRetencion; }
            set { sumRetencion = value; }
        }
        private decimal sumTotal;

        public decimal SumTotal
        {
            get { return sumTotal; }
            set { sumTotal = value; }
        }
        private string strMinFecha;

        public string StrMinFecha
        {
            get { return strMinFecha; }
            set { strMinFecha = value; }
        }
        private string strMaxFecha;

        public string StrMaxFecha
        {
            get { return strMaxFecha; }
            set { strMaxFecha = value; }
        }
        private IList<Factura> facturas;

        public IList<Factura> Facturas
        {
            get { return facturas; }
            set { facturas = value; }
        }

        public PeriodoFactura()
        {
            // incializamos la colección dependiente
            facturas = new List<Factura>();
        }

        private decimal sumAportacion;

        public decimal SumAportacion
        {
            get { return sumAportacion; }
            set { sumAportacion = value; }
        }
    }
}
