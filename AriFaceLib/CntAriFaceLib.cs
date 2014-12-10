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
            string sql = "SELECT * FROM administrador WHERE nombre LIKE '%{0}%' ORDER BY nombre;";
            if (parNom == "*")
                sql = "SELECT * FROM administrador ORDER BY nombre;";
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
            bool alta = false;
            if (a == null) return null;
            if (a.AdministradorId == 0)
            {
                alta = true;
            }
            // si el id es 0 se crea el objeto, si no se actualiza.
            MySqlCommand cmd = conn.CreateCommand();
            string sql = "";
            if (alta)
            {
                sql = @"
                    INSERT INTO administrador
                    (login, password, nombre, email, certsn)
                    VALUES ('{1}','{2}','{3}','{4}','{5}');
                ";
            }
            else
            {
                sql = @"
                    UPDATE administrador
                    SET
                    login='{1}',
                    password='{2}',
                    nombre='{3}',
                    email='{4}',
                    certsn='{5}'
                    WHERE administrador_id={0};
                ";
            }
            if (a.Password == "" && !alta)
            {
                sql = @"
                    UPDATE administrador
                    SET
                    login='{1}',
                    nombre='{3}',
                    email='{4}',
                    certsn='{5}'
                    WHERE administrador_id={0};
            ";
            }
            sql = String.Format(sql, a.AdministradorId, a.Login, a.Password, a.Nombre, a.Email, a.Certsn);
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
            // y vamos rápidamente a por la recién creada
            if (alta)
            {
                sql = @"SELECT LAST_INSERT_ID() as ultid FROM administrador;";
                cmd.CommandText = sql;
                MySqlDataReader rdr2 = cmd.ExecuteReader();
                if (rdr2.HasRows)
                {
                    rdr2.Read();
                    a.AdministradorId = rdr2.GetInt32("ultid");
                }
                rdr2.Close();
            }
            sql = @"SELECT * FROM administrador WHERE administrador_id={0};";
            sql = String.Format(sql, a.AdministradorId);
            cmd.CommandText = sql;
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                a = GetAdministrador(rdr);
            }
            return a;
        }

        public static void DeleteAdministrador(int id, MySqlConnection conn)
        {
            MySqlCommand cmd = conn.CreateCommand();
            string sql = String.Format("DELETE FROM administrador WHERE administrador_id={0}", id);
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }

        #endregion

        #region Estados
        public static Estado GetEstado(MySqlDataReader rdr)
        {
            if (rdr.IsDBNull(rdr.GetOrdinal("codigo"))) return null;
            Estado e = new Estado();
            e.Codigo = rdr.GetString("codigo");
            if (!rdr.IsDBNull(rdr.GetOrdinal("nombre")))
                e.Nombre = rdr.GetString("nombre");
            if (!rdr.IsDBNull(rdr.GetOrdinal("descripcion")))
                e.Descripcion = rdr.GetString("descripcion");
            return e;
        }

        public static Estado GetEstado(string codigo, MySqlConnection conn)
        {
            Estado e = null;
            MySqlCommand cmd = conn.CreateCommand();
            string sql = @"SELECT * FROM estado WHERE codigo = '{0}'";
            sql = String.Format(sql, codigo);
            cmd.CommandText = sql;
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                e = GetEstado(rdr);
            }
            return e;
        }

        public static IList<Estado> GetEstados(MySqlConnection conn)
        {
            IList<Estado> le = new List<Estado>();
            MySqlCommand cmd = conn.CreateCommand();
            string sql = "SELECT * FROM estado";
            cmd.CommandText = sql;
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    Estado e = GetEstado(rdr);
                    le.Add(e);
                }
            }
            return le;
        }

        public static Estado SetEstado(Estado e, MySqlConnection conn)
        {
            // si el id es 0 se crea el objeto, si no se actualiza.
            MySqlCommand cmd = conn.CreateCommand();
            string sql = @"
                    INSERT INTO estado
                    (codigo, nombre, descripcion)
                    VALUES ({0},'{1}','{2}')
                    ON DUPLICATE KEY UPDATE
                    codigo='{0}',
                    nombre='{1}',
                    descripcion='{2}'
            ";
            sql = String.Format(sql, e.Codigo, e.Nombre, e.Descripcion);
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
            sql = @"SELECT * FROM estado WHERE codigo='{0}';";
            sql = String.Format(sql, e.Codigo);
            cmd.CommandText = sql;
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                e = GetEstado(rdr);
            }
            rdr.Close();
            return e;
        }

        public static void SetEstados(IList<Estado> le, MySqlConnection conn)
        {
            foreach (Estado e in le)
            {
                SetEstado(e, conn);
            }
        }

        public static void DeleteEstado(string codigo, MySqlConnection conn)
        {
            MySqlCommand cmd = conn.CreateCommand();
            string sql = String.Format("DELETE FROM estado WHERE codigo='{0}'", codigo);
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }

        #endregion

        #region Unidades
        public static Unidad GetUnidad(MySqlDataReader rdr)
        {
            if (rdr.IsDBNull(rdr.GetOrdinal("organoGestorCodigo"))) return null;
            Unidad u = new Unidad();
            u.OrganoGestorCodigo = rdr.GetString("organoGestorCodigo");
            u.OrganoGestorNombre = rdr.GetString("organoGestorNombre");
            u.UnidadTramitadoraCodigo = rdr.GetString("unidadTramitadoraCodigo");
            u.UnidadTramitadoraNombre = rdr.GetString("unidadTramitadoraNombre");
            u.OficinaContableCodigo = rdr.GetString("oficinaContableCodigo");
            u.OficinaContableNombre = rdr.GetString("oficinaContableNombre");
            return u;
        }

        public static Unidad GetUnidad(string organoGestorCodigo, string unidadTramitadoraCodigo, string oficinaContableCodigo, MySqlConnection conn)
        {
            Unidad u = null;
            MySqlCommand cmd = conn.CreateCommand();
            string sql = @"SELECT * FROM unidad WHERE organoGestorCodigo = '{0}' AND unidadTramitadoraCodigo = '{1}' AND oficinaContableCodigo = '{2}'";
            sql = String.Format(sql, organoGestorCodigo, unidadTramitadoraCodigo, oficinaContableCodigo);
            cmd.CommandText = sql;
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                u = GetUnidad(rdr);
            }
            return u;
        }

        public static IList<Unidad> GetUnidades(MySqlConnection conn)
        {
            IList<Unidad> lu = new List<Unidad>();
            MySqlCommand cmd = conn.CreateCommand();
            string sql = "SELECT * FROM unidad";
            cmd.CommandText = sql;
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    Unidad u = GetUnidad(rdr);
                    lu.Add(u);
                }
            }
            return lu;
        }

        public static IList<Unidad> GetUnidades(string organoGestorCodigo, MySqlConnection conn)
        {
            IList<Unidad> lu = new List<Unidad>();
            MySqlCommand cmd = conn.CreateCommand();
            string sql = "SELECT * FROM unidad WHERE organoGestorCodigo='{0}'";
            sql = String.Format(sql, organoGestorCodigo);
            cmd.CommandText = sql;
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    Unidad u = GetUnidad(rdr);
                    lu.Add(u);
                }
            }
            return lu;
        }

        public static IList<Unidad> GetUnidades(string organoGestorCodigo, string unidadTramitadoraCodigo, MySqlConnection conn)
        {
            IList<Unidad> lu = new List<Unidad>();
            MySqlCommand cmd = conn.CreateCommand();
            string sql = "SELECT * FROM unidad WHERE organoGestorCodigo='{0}' AND unidadTramitadoraCodigo='{1}'";
            sql = String.Format(sql, organoGestorCodigo, unidadTramitadoraCodigo);
            cmd.CommandText = sql;
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    Unidad u = GetUnidad(rdr);
                    lu.Add(u);
                }
            }
            return lu;
        }

        public static Unidad SetUnidad(Unidad u, MySqlConnection conn)
        {
            // si el id es 0 se crea el objeto, si no se actualiza.
            MySqlCommand cmd = conn.CreateCommand();
            string sql = @"
                    INSERT INTO unidad
                    (organoGestorCodigo, organoGestorNombre, unidadTramitadoraCodigo, unidadTramitadoraNombre, oficinaContableCodigo, oficinaContableNombre)
                    VALUES ('{0}','{1}','{2}','{3}','{4}','{5}')
                    ON DUPLICATE KEY UPDATE
                    organoGestorCodigo='{0}',
                    organoGestorNombre='{1}',
                    unidadTramitadoraCodigo='{2}',
                    unidadTramitadoraNombre='{3}',
                    oficinaContableCodigo='{4}',
                    oficinaContableNombre='{5}'
            ";
            sql = String.Format(sql, u.OrganoGestorCodigo, u.OrganoGestorNombre, u.UnidadTramitadoraCodigo, u.UnidadTramitadoraNombre, u.OficinaContableCodigo, u.OficinaContableNombre);
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
            sql = @"SELECT * FROM unidad WHERE organoGestorCodigo = '{0}' AND unidadTramitadoraCodigo = '{1}' AND oficinaContableCodigo = '{2}'";
            sql = String.Format(sql,u.OrganoGestorCodigo, u.UnidadTramitadoraCodigo, u.OficinaContableCodigo);
            cmd.CommandText = sql;
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                u = GetUnidad(rdr);
            }
            rdr.Close();
            return u;
        }

        public static void SetUnidades(IList<Unidad> lu, MySqlConnection conn)
        {
            foreach (Unidad u in lu)
            {
                SetUnidad(u, conn);
            }
        }

        public static void DeleteUnidad(string organoGestorCodigo, string unidadTramitadoraCodigo, string oficinaContableCodigo, MySqlConnection conn)
        {
            MySqlCommand cmd = conn.CreateCommand();
            string sql = String.Format("DELETE FROM estado  WHERE organoGestorCodigo = '{0}' AND unidadTramitadoraCodigo = '{1}' AND oficinaContableCodigo = '{2}'", 
                organoGestorCodigo, unidadTramitadoraCodigo, oficinaContableCodigo);
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

        #region Cliente
        public static Cliente GetCliente(MySqlDataReader rdr)
        {
            if (rdr.IsDBNull(rdr.GetOrdinal("i_d"))) return null;
            Cliente c = new Cliente();
            c.ClienteId = rdr.GetInt32("i_d");
            c.Nombre = rdr.GetString("nombre");
            if (!rdr.IsDBNull(rdr.GetOrdinal("email")))
                c.Email = rdr.GetString("email");
            if (!rdr.IsDBNull(rdr.GetOrdinal("organoGestorCodigo")))
                c.CodOrganoGestor = rdr.GetString("organoGestorCodigo");
            if (!rdr.IsDBNull(rdr.GetOrdinal("unidadTramitadoraCodigo")))
                c.CodUnidadTramitadora = rdr.GetString("unidadTramitadoraCodigo");
            if (!rdr.IsDBNull(rdr.GetOrdinal("oficinaContableCodigo")))
                c.CodOficinaContable = rdr.GetString("oficinaContableCodigo");
            return c;
        }

        public static Cliente GetCliente(int id, MySqlConnection conn)
        {
            Cliente c = null;
            MySqlCommand cmd = conn.CreateCommand();
            string sql = @"SELECT * FROM cliente WHERE i_d = {0}";
            sql = String.Format(sql, id);
            cmd.CommandText = sql;
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                c = GetCliente(rdr);
            }
            return c;
        }

        public static IList<Cliente> GetClientes(MySqlConnection conn)
        {
            IList<Cliente> lc = new List<Cliente>();
            MySqlCommand cmd = conn.CreateCommand();
            string sql = "SELECT * FROM cliente";
            cmd.CommandText = sql;
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    Cliente c = GetCliente(rdr);
                    lc.Add(c);
                }
            }
            return lc;
        }

        public static IList<Cliente> GetClientes(string parNom, MySqlConnection conn)
        {
            IList<Cliente> lc = new List<Cliente>();
            MySqlCommand cmd = conn.CreateCommand();
            string sql = "SELECT * FROM cliente WHERE nombre LIKE '%{0}%' ORDER BY nombre;";
            if (parNom == "*")
                sql = "SELECT * FROM cliente ORDER BY nombre;";
            sql = String.Format(sql, parNom);
            cmd.CommandText = sql;
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    Cliente c = GetCliente(rdr);
                    lc.Add(c);
                }
            }
            return lc;
        }

        public static Cliente SetCliente(Cliente c, MySqlConnection conn)
        {
            bool alta = false;
            if (c == null) return null;
            if (c.ClienteId == 0)
            {
                alta = true;
            }
            // si el id es 0 se crea el objeto, si no se actualiza.
            MySqlCommand cmd = conn.CreateCommand();
            string sql = "";
            if (alta)
            {
                sql = @"
                    INSERT INTO cliente
                    (nombre, cif, email, organoGestorCodigo, unidadTramitadoraCodigo, oficinaContableCodigo)
                    VALUES ('{1}','{2}','{3}','{4}','{5}','{6}');
                ";
            }
            else
            {
                sql = @"
                    UPDATE cliente
                    SET
                    nombre='{1}',
                    cif='{2}',
                    email='{3}',
                    organoGestorCodigo='{4}',
                    unidadTramitadora='{5}',
                    oficinaContableCodigo='{6}'
                    WHERE i_d={0};
                ";
            }
            sql = String.Format(sql,c.ClienteId, c.Nombre, c.Cif, c.Email, c.CodOrganoGestor, c.CodUnidadTramitadora, c.CodOficinaContable);
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
            // y vamos rápidamente a por la recién creada
            if (alta)
            {
                sql = @"SELECT LAST_INSERT_ID() as ultid FROM cliente;";
                cmd.CommandText = sql;
                MySqlDataReader rdr2 = cmd.ExecuteReader();
                if (rdr2.HasRows)
                {
                    rdr2.Read();
                    c.ClienteId = rdr2.GetInt32("ultid");
                }
                rdr2.Close();
            }
            sql = @"SELECT * FROM cliente WHERE i_d={0};";
            sql = String.Format(sql, c.ClienteId);
            cmd.CommandText = sql;
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                c = GetCliente(rdr);
            }
            return c;
        }

        public static void DeleteCliente(int id, MySqlConnection conn)
        {
            MySqlCommand cmd = conn.CreateCommand();
            string sql = String.Format("DELETE FROM cliente WHERE i_d={0}", id);
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }
        #endregion
    }
}
