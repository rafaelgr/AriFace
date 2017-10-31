using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AriFaceLib
{
    public class Punto
    {
        private int codclien;

        public int CodClien
        {
            get { return codclien; }
            set { codclien = value; }
        }
        private string nomClien;

        public string NomClien
        {
            get { return nomClien; }
            set { nomClien = value; }
        }
        private int numero;

        public int Numero
        {
            get { return numero; }
            set { numero = value; }
        }
        private string codtipom;

        public string CodTipom
        {
            get { return codtipom; }
            set { codtipom = value; }
        }
        private string fechaalb;

        public string FechaAlb
        {
            get { return fechaalb; }
            set { fechaalb = value; }
        }
        private string concepto;

        public string Concepto
        {
            get { return concepto; }
            set { concepto = value; }
        }

        public Decimal puntos;
        public Decimal Puntos
        {
            get { return puntos; }
            set { puntos = value; }
        }

        public string fecMov;
        public string FecMov
        {
            get { return fecMov; }
            set { fecMov = value; }
        }

        public string observaciones;
        public string Observaciones
        {
            get { return observaciones; }
            set { observaciones = value; }
        }

    }
}
