using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Security.Cryptography.X509Certificates;

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
            if (!rdr.IsDBNull(rdr.GetOrdinal("certsn")))
                a.Certsn = rdr.GetString("certsn");
            return a;
        }
        public static Administrador GetAdministradorLogin(string login, string password, MySqlConnection conn)
        {
            // Se entiene que la conexión se recibe abierta y es responsabilidad del llamante
            // cerrarla y destruirla
            Administrador a = null;
            MySqlCommand cmd = conn.CreateCommand();
            string sql = @"SELECT *
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
        public static Administrador GetAdministrador(int id, MySqlConnection conn)
        {
            Administrador a = null;
            MySqlCommand cmd = conn.CreateCommand();
            string sql = @"SELECT * FROM administrador WHERE administrador_id = {0}";
            sql = String.Format(sql, id);
            cmd.CommandText = sql;
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                a = GetAdministrador(rdr);
            }
            return a;
        }
        public static IList<Administrador> GetAdministradores(MySqlConnection conn)
        {
            IList<Administrador> la = new List<Administrador>();
            MySqlCommand cmd = conn.CreateCommand();
            string sql = "SELECT * FROM administrador";
            cmd.CommandText = sql;
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    Administrador a = GetAdministrador(rdr);
                    la.Add(a);
                }
            }
            return la;
        }
        public static IList<Administrador> GetAdministradores(string parNom, MySqlConnection conn)
        {
            IList<Administrador> la = new List<Administrador>();
            MySqlCommand cmd = conn.CreateCommand();
            string sql = "SELECT * FROM administrador WHERE nombre LIKE '%{0}%';";
            sql = String.Format(sql, parNom);
            cmd.CommandText = sql;
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    Administrador a = GetAdministrador(rdr);
                    la.Add(a);
                }
            }
            return la;
        }
        public static Administrador SetAdministrador(Administrador a, MySqlConnection conn)
        {
            if (a == null) return null;
            // si el id es 0 se crea el objeto, si no se actualiza.
            MySqlCommand cmd = conn.CreateCommand();
            string sql = @"
                    INSERT INTO administrador
                    (administrador_id, login, password, nombre, email, certsn)
                    VALUES ({0},'{1}','{2}','{3}','{4}','{5}')
                    ON DUPLICATE KEY UPDATE
                    administrador_id={0},
                    login='{1}',
                    password='{2}',
                    nombre='{3}',
                    email='{4}',
                    certsn='{5}';
            ";
            cmd.CommandText = sql;
            sql = String.Format(sql, a.AdministradorId, a.Login, a.Password, a.Nombre, a.Email, a.Certsn);
            cmd.ExecuteNonQuery();
            // y vamos rápidamente a por la recién creada
            sql = @"SELECT LAST_INSERT_ID() as ultid FROM administrador;";
            cmd.CommandText = sql;
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                a = GetAdministrador(rdr);
            }
            return a;
        }
        public static void DeleteAdministrador(Administrador a, MySqlConnection conn)
        {
            MySqlCommand cmd = conn.CreateCommand();
            string sql = String.Format("DELETE FROM administrador WHERE administrador_id={0}", a.AdministradorId);
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }
        #endregion

        #region Certificados
        public static IList<Certificado> GetCertificados()
        {
            IList<Certificado> lc = new List<Certificado>();
            X509Store store = new X509Store("My", StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);
            for (int i = 0; i < store.Certificates.Count; i++)
            {
                Certificado c = new Certificado();
                c.SerialNumber = store.Certificates[i].SerialNumber;
                c.FriendlyName = store.Certificates[i].FriendlyName;
                c.ExpirationDateString = store.Certificates[i].GetExpirationDateString();
                lc.Add(c);
            }
            return lc;
        }
        #endregion
    }
}
