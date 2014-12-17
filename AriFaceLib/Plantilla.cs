using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AriFaceLib
{
    public class Plantilla
    {
        private int plantillaId;

        public int PlantillaId
        {
            get { return plantillaId; }
            set { plantillaId = value; }
        }
        private string nombre;

        public string Nombre
        {
            get { return nombre; }
            set { nombre = value; }
        }
        private string contenido;

        public string Contenido
        {
            get { return contenido; }
            set { contenido = value; }
        }

        private string observaciones;

        public string Observaciones
        {
            get { return observaciones; }
            set { observaciones = value; }
        }
    }
}
