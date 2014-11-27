using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AriFaceLib
{
    public class RespuestaConsultaFactura
    {
        private string anulacionCodigo;

        public string AnulacionCodigo
        {
            get { return anulacionCodigo; }
            set { anulacionCodigo = value; }
        }
        private string anulacionDescripcion;

        public string AnulacionDescripcion
        {
            get { return anulacionDescripcion; }
            set { anulacionDescripcion = value; }
        }
        private string anulacionMotivo;

        public string AnulacionMotivo
        {
            get { return anulacionMotivo; }
            set { anulacionMotivo = value; }
        }
        private string numeroRegistro;

        public string NumeroRegistro
        {
            get { return numeroRegistro; }
            set { numeroRegistro = value; }
        }
        private string tramitacionCodigo;

        public string TramitacionCodigo
        {
            get { return tramitacionCodigo; }
            set { tramitacionCodigo = value; }
        }
        private string tramitacionDescripcion;

        public string TramitacionDescripcion
        {
            get { return tramitacionDescripcion; }
            set { tramitacionDescripcion = value; }
        }
        private string tramitacionMotivo;

        public string TramitacionMotivo
        {
            get { return tramitacionMotivo; }
            set { tramitacionMotivo = value; }
        }
    }
}
