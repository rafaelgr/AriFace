using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AriFaceLib
{
    public class item
    {
        private string _anexo;

        public string anexo
        {
            get { return _anexo; }
            set { _anexo = value; }
        }
        private string _nombre;

        public string nombre
        {
            get { return _nombre; }
            set { _nombre = value; }
        }
        private string _mime;

        public string mime
        {
            get { return _mime; }
            set { _mime = value; }
        }
    }
}
