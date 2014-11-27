using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AriFaceLib
{
    public class RespuestaAnularFactura
    {
        private string numRegistro;

        public string NumRegistro
        {
            get { return numRegistro; }
            set { numRegistro = value; }
        }
        private string mensaje;

        public string Mensaje
        {
            get { return mensaje; }
            set { mensaje = value; }
        }
    }
}
