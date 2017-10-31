using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AriFaceLib
{
    public class ClientePuntos
    {
        private int codclien;

        public int CodClien
        {
            get { return codclien; }
            set { codclien = value; }
        }
        private string nomclien;

        public string NomClien
        {
            get { return nomclien; }
            set { nomclien = value; }
        }

        private int tienePuntos = 0;

        public int TienePuntos
        {
            get { return tienePuntos; }
            set { tienePuntos = value; }

        }

        private decimal puntos = 0;

        public decimal Puntos
        {
            get { return puntos; }
            set { puntos = value; }

        }
    }
}
