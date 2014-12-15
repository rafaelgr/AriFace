using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AriFaceLib
{
    public class Usuario
    {
        private int usuarioId;

        public int UsuarioId
        {
            get { return usuarioId; }
            set { usuarioId = value; }
        }
        private string nombre;

        public string Nombre
        {
            get { return nombre; }
            set { nombre = value; }
        }
        private string login;

        public string Login
        {
            get { return login; }
            set { login = value; }
        }
        private string email;

        public string Email
        {
            get { return email; }
            set { email = value; }
        }
        private string password;

        public string Password
        {
            get { return password; }
            set { password = value; }
        }
        private string nif;

        public string Nif
        {
            get { return nif; }
            set { nif = value; }
        }
        private int clienteId;

        public int ClienteId
        {
            get { return clienteId; }
            set { clienteId = value; }
        }
        private int departamentoId;

        public int DepartamentoId
        {
            get { return departamentoId; }
            set { departamentoId = value; }
        }

        private string nifNombre;

        public string NifNombre
        {
            get { return nifNombre; }
            set { nifNombre = value; }
        }
        private string departamentoNombre;

        public string DepartamentoNombre
        {
            get { return departamentoNombre; }
            set { departamentoNombre = value; }
        }
        private string clienteNombre;

        public string ClienteNombre
        {
            get { return clienteNombre; }
            set { clienteNombre = value; }
        }
    }
}
