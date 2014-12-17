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
    }
}
