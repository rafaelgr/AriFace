using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AriFaceLib
{
    public class Envio
    {
        private string nif;

        public string Nif
        {
            get { return nif; }
            set { nif = value; }
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
        private int departamentoId;

        public int DepartamentoId
        {
            get { return departamentoId; }
            set { departamentoId = value; }
        }
        private string departamentoNombre;

        public string DepartamentoNombre
        {
            get { return departamentoNombre; }
            set { departamentoNombre = value; }
        }
        private string strFechaInicial;

        public string StrFechaInicial
        {
            get { return strFechaInicial; }
            set { strFechaInicial = value; }
        }
        private string strFechaFinal;

        public string StrFechaFinal
        {
            get { return strFechaFinal; }
            set { strFechaFinal = value; }
        }
        private decimal total;

        public decimal Total
        {
            get { return total; }
            set { total = value; }
        }
        private decimal bases;

        public decimal Bases
        {
            get { return bases; }
            set { bases = value; }
        }
        private decimal cuotas;

        public decimal Cuotas
        {
            get { return cuotas; }
            set { cuotas = value; }
        }
        private decimal retencion;

        public decimal Retencion
        {
            get { return retencion; }
            set { retencion = value; }
        }
        private bool comoCliente;

        public bool ComoCliente
        {
            get { return comoCliente; }
            set { comoCliente = value; }
        }

        private bool esFace;

        public bool EsFace
        {
            get { return esFace; }
            set { esFace = value; }
        }
    }
}
