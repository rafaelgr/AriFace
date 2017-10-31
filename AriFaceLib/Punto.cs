using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AriFaceLib
{
    public class Punto
    {
        private string organoGestorCodigo;

        public string OrganoGestorCodigo
        {
            get { return organoGestorCodigo; }
            set { organoGestorCodigo = value; }
        }
        private string organoGestorNombre;

        public string OrganoGestorNombre
        {
            get { return organoGestorNombre; }
            set { organoGestorNombre = value; }
        }
        private string unidadTramitadoraCodigo;

        public string UnidadTramitadoraCodigo
        {
            get { return unidadTramitadoraCodigo; }
            set { unidadTramitadoraCodigo = value; }
        }
        private string unidadTramitadoraNombre;

        public string UnidadTramitadoraNombre
        {
            get { return unidadTramitadoraNombre; }
            set { unidadTramitadoraNombre = value; }
        }
        private string oficinaContableCodigo;

        public string OficinaContableCodigo
        {
            get { return oficinaContableCodigo; }
            set { oficinaContableCodigo = value; }
        }
        private string oficinaContableNombre;

        public string OficinaContableNombre
        {
            get { return oficinaContableNombre; }
            set { oficinaContableNombre = value; }
        }
    }
}
