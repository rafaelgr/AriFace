using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AriFaceLib
{
    public class Cliente
    {
        private int clienteId;

        public int ClienteId
        {
            get { return clienteId; }
            set { clienteId = value; }
        }
        private string nombre;

        public string Nombre
        {
            get { return nombre; }
            set { nombre = value; }
        }
        private string cif;

        public string Cif
        {
            get { return cif; }
            set { cif = value; }
        }
        private string email;

        public string Email
        {
            get { return email; }
            set { email = value; }
        }
        private string codOrganoGestor;

        public string CodOrganoGestor
        {
            get { return codOrganoGestor; }
            set { codOrganoGestor = value; }
        }
        private string codUnidadTramitadora;

        public string CodUnidadTramitadora
        {
            get { return codUnidadTramitadora; }
            set { codUnidadTramitadora = value; }
        }
        private string codOficinaContable;

        public string CodOficinaContable
        {
            get { return codOficinaContable; }
            set { codOficinaContable = value; }
        }

        private int codSocioAriagro;

        public int CodSocioAriagro
        {
            get { return codSocioAriagro; }
            set { codSocioAriagro = value; }
        }
        private int codSocioAritaxi;

        public int CodSocioAritaxi
        {
            get { return codSocioAritaxi; }
            set { codSocioAritaxi = value; }
        }

        private int codClienAriges;

        public int CodClienAriges
        {
            get { return codClienAriges; }
            set { codClienAriges = value; }
        }

        private string iban;

        public string Iban
        {
            get { return iban; }
            set { iban = value; }
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
