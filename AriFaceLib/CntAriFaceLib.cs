using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace AriFaceLib
{
    public static class CntAriFaceLib
    {
        #region Conexión a MySql
        public static MySqlConnection GetConnection(string connectionString)
        {
            // crear la conexion y devolverla.
            MySqlConnection conn = new MySqlConnection(connectionString);
            return conn;
        }
        #endregion

        #region Administrador
        public static Administrador GetAdministrador(MySqlDataReader rdr)
        {
            if (rdr.IsDBNull(rdr.GetOrdinal("administrador_id"))) return null;
            Administrador a = new Administrador();
            a.AdministradorId = rdr.GetInt32("administrador_id");
            a.Login = rdr.GetString("login");
            a.Password = rdr.GetString("password");
            a.Nombre = rdr.GetString("nombre");
            if (!rdr.IsDBNull(rdr.GetOrdinal("email")))
                a.Email = rdr.GetString("email");
            return a;
        }
        public static Administrador GetAdministradorLogin(string login, string password, MySqlConnection conn)
        {
            // Se entiene que la conexión se recibe abierta y es responsabilidad del llamante
            // cerrarla y destruirla
            Administrador a = null;
            MySqlCommand cmd = conn.CreateCommand();
            string sql = @"SELECT 
                    *
                    FROM administrador
                    WHERE login = '{0}'
                    AND password = '{1}'";
            sql = String.Format(sql, login, password);
            cmd.CommandText = sql;
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                a = GetAdministrador(rdr);
            }
            return a;
        }
        #endregion
    }
}
