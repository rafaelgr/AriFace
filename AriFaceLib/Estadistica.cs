using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AriFaceLib
{
    public class Estadistica
    {
        private int numEnviadas;

        public int NumEnviadas
        {
            get { return numEnviadas; }
            set { numEnviadas = value; }
        }
        private int numPendientes;

        public int NumPendientes
        {
            get { return numPendientes; }
            set { numPendientes = value; }
        }
        private int numAparcadas;

        public int NumAparcadas
        {
            get { return numAparcadas; }
            set { numAparcadas = value; }
        }
    }
}
