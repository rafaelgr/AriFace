﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Security.Cryptography.X509Certificates;
using System.Net.Mail;
using System.Configuration;
using System.Net;


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

        public static MiniUnidad GetMiniUnidad(MySqlDataReader rdr)
        {
            if (rdr.IsDBNull(rdr.GetOrdinal("codigo"))) return null;
            MiniUnidad mu = new MiniUnidad();
            mu.Codigo = rdr.GetString("codigo");
            mu.Nombre = rdr.GetString("nombre");
            return mu;
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

        public static IList<MiniUnidad> GetOg(MySqlConnection conn)
        {
            IList<MiniUnidad> lmu = new List<MiniUnidad>();
            MySqlCommand cmd = conn.CreateCommand();
            string sql = "SELECT DISTINCT organoGestorCodigo AS codigo, organoGestorNombre AS nombre FROM unidad ORDER BY organoGestorNombre";
            cmd.CommandText = sql;
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    MiniUnidad mu = GetMiniUnidad(rdr);
                    lmu.Add(mu);
                }
            }
            return lmu;
        }


        /// <summary>
        /// Sirve para los desplegables de los formularios
        /// Devuelve las Unidades Administradoras posibles dado un órgano gestor
        /// </summary>
        /// <param name="organoGestorCodigo"></param>
        /// <param name="conn"></param>
        /// <returns>MiniUnidad: un objeto con código y nombre sólo</returns>
        public static IList<MiniUnidad> GetUtOfOg(string organoGestorCodigo, MySqlConnection conn)
        {
            IList<MiniUnidad> lmu = new List<MiniUnidad>();
            MySqlCommand cmd = conn.CreateCommand();
            string sql = "SELECT DISTINCT unidadTramitadoraCodigo AS codigo, unidadTramitadoraNombre AS nombre FROM unidad WHERE organoGestorCodigo ='{0}' ORDER BY unidadTramitadoraNombre";
            sql = String.Format(sql, organoGestorCodigo);
            cmd.CommandText = sql;
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    MiniUnidad mu = GetMiniUnidad(rdr);
                    lmu.Add(mu);
                }
            }
            return lmu;
        }

        /// <summary>
        /// Sirve para los desplegables de los formularios
        /// Devuelve las Oficinas contables posibles dado una unidad tramitadora
        /// </summary>
        /// <param name="organoGestorCodigo"></param>
        /// <param name="unidadAdministradoraCodigo"></param>
        /// <param name="conn"></param>
        /// <returns>MiniUnidad: un objeto con código y nombre sólo</returns>
        public static IList<MiniUnidad> GetOcOfUt(string organoGestorCodigo, string unidadTramitadoraCodigo, MySqlConnection conn)
        {
            IList<MiniUnidad> lmu = new List<MiniUnidad>();
            MySqlCommand cmd = conn.CreateCommand();
            string sql = "SELECT DISTINCT oficinaContableCodigo AS codigo, oficinaContableNombre AS nombre FROM unidad WHERE organoGestorCodigo ='{0}' AND unidadTramitadoraCodigo ='{1}' ORDER BY oficinaContableNombre";
            sql = String.Format(sql, organoGestorCodigo, unidadTramitadoraCodigo);
            cmd.CommandText = sql;
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    MiniUnidad mu = GetMiniUnidad(rdr);
                    lmu.Add(mu);
                }
            }
            return lmu;
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
            if (!rdr.IsDBNull(rdr.GetOrdinal("cif")))
                c.Cif = rdr.GetString("cif");
            if (!rdr.IsDBNull(rdr.GetOrdinal("organoGestorCodigo")))
                c.CodOrganoGestor = rdr.GetString("organoGestorCodigo");
            if (!rdr.IsDBNull(rdr.GetOrdinal("unidadTramitadoraCodigo")))
                c.CodUnidadTramitadora = rdr.GetString("unidadTramitadoraCodigo");
            if (!rdr.IsDBNull(rdr.GetOrdinal("oficinaContableCodigo")))
                c.CodOficinaContable = rdr.GetString("oficinaContableCodigo");
            if (!rdr.IsDBNull(rdr.GetOrdinal("cod_socio_ariagro")))
                c.CodSocioAriagro = rdr.GetInt32("cod_socio_ariagro");
            if (!rdr.IsDBNull(rdr.GetOrdinal("cod_socio_aritaxi")))
                c.CodSocioAritaxi = rdr.GetInt32("cod_socio_aritaxi");
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
            rdr.Close();
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
                    unidadTramitadoraCodigo='{5}',
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

        #region Factura
        public static Factura GetFactura(MySqlDataReader rdr)
        {
            if (rdr.IsDBNull(rdr.GetOrdinal("FACTURA_ID"))) return null;
            Factura f = new Factura();
            f.Aportacion = rdr.GetDecimal("APORTACION");
            f.BaseIva = rdr.GetDecimal("BASE_IVA");
            f.ClienteId = rdr.GetInt32("CLIENTE_ID");
            f.ClienteNombre = rdr.GetString("CLIENTE_NOMBRE");
            f.CodTipom = rdr.GetString("CODTIPOM");
            f.CuotaIva = rdr.GetDecimal("CUOTA_IVA");
            if (rdr.GetInt16("ES_DE_CLIENTE") == 1) f.EsDeCliente = true;
            f.FacturaId = rdr.GetInt32("FACTURA_ID");
            f.StrFecha = String.Format("{0:yyyyMMdd}",rdr.GetDateTime("FECHA"));
            f.LetraProveedor = rdr.GetString("LETRA_PROVEEDOR");
            f.NumFactura = rdr.GetInt32("NUMFACTURA");
            f.Retencion = rdr.GetDecimal("RETENCION");
            f.Serie = rdr.GetString("SERIE");
            f.Sistema = rdr.GetString("SISTEMA");
            f.Total = rdr.GetDecimal("TOTAL");
            if (rdr.GetInt16("NUEVA") == 1) f.Nueva = true;
            f.Estado = rdr.GetInt16("NUEVA");
            if (!rdr.IsDBNull(rdr.GetOrdinal("CODDIREC"))) f.CodDirec = rdr.GetInt32("CODDIREC");
            if (!rdr.IsDBNull(rdr.GetOrdinal("DEPARTAMENTO"))) f.Departamento= rdr.GetString("DEPARTAMENTO");
            f.RegistroFace = rdr.GetString("REGISTRO_FACE");
            f.MotivoFace = rdr.GetString("MOTIVO_FACE");
            return f;
        }

        public static Factura GetFactura(int id, MySqlConnection conn)
        {
            Factura f = null;
            MySqlCommand cmd = conn.CreateCommand();
            string sql = @"SELECT 
                f.`imp_gastos_a_fo` AS APORTACION,
                f.`base_total` AS BASE_IVA,
                f.`id_cliente` AS CLIENTE_ID,
                c.nombre AS CLIENTE_NOMBRE,
                COALESCE(f.v_codtipom1,"") AS CODTIPOM,
                f.cuota_total AS CUOTA_IVA,
                f.es_fra_cliente AS ES_DE_CLIENTE,
                f.id_factura AS FACTURA_ID,
                f.strFecha AS FECHA,
                f.letra_id_fra_prove AS LETRA_PROVEEDOR,
                f.num_factura AS NUMFACTURA,
                f.imp_retencion AS RETENCION,
                f.num_serie AS SERIE,
                s.descripcion AS SISTEMA,
                f.ttal AS TOTAL,
                f.nueva AS NUEVA,
                d.coddirec AS CODDIREC,
                d.nombre AS DEPARTAMENTO,
                COALESCE(f.registroFace,'') AS REGISTRO_FACE,
                COALESCE(f.motivoFace,'') AS MOTIVO_FACE
                FROM factura AS f
                LEFT JOIN sistema AS s ON s.sistema_id = f.sistema_id
                LEFT JOIN cliente AS c ON c.i_d = f.id_cliente
                LEFT JOIN departamento AS d ON d.codclien = c.codclien_ariges AND d.coddirec = f.coddirec_ariges
                WHERE f.id_factura = {0}";
            sql = String.Format(sql, id);
            cmd.CommandText = sql;
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                f = GetFactura(rdr);
            }
            return f;
        }

        public static IList<Factura> GetFacturas(MySqlConnection conn)
        {
            IList<Factura> lf = new List<Factura>();
            MySqlCommand cmd = conn.CreateCommand();
            string sql = @"SELECT 
                f.`imp_gastos_a_fo` AS APORTACION,
                f.`base_total` AS BASE_IVA,
                f.`id_cliente` AS CLIENTE_ID,
                c.nombre AS CLIENTE_NOMBRE,
                COALESCE(f.v_codtipom1,'') AS CODTIPOM,
                f.cuota_total AS CUOTA_IVA,
                f.es_fra_cliente AS ES_DE_CLIENTE,
                f.id_factura AS FACTURA_ID,
                f.Fecha AS FECHA,
                f.letra_id_fra_prove AS LETRA_PROVEEDOR,
                f.num_factura AS NUMFACTURA,
                f.imp_retencion AS RETENCION,
                f.num_serie AS SERIE,
                s.descripcion AS SISTEMA,
                f.ttal AS TOTAL,
                f.nueva AS NUEVA,
                d.coddirec AS CODDIREC,
                d.nombre AS DEPARTAMENTO,
                COALESCE(f.registroFace,'') AS REGISTRO_FACE,
                COALESCE(f.motivoFace,'') AS MOTIVO_FACE
                FROM factura AS f
                LEFT JOIN sistema AS s ON s.sistema_id = f.sistema_id
                LEFT JOIN cliente AS c ON c.i_d = f.id_cliente
                LEFT JOIN departamento AS d ON d.codclien = c.codclien_ariges AND d.coddirec = f.coddirec_ariges";
            cmd.CommandText = sql;
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    Factura f = GetFactura(rdr);
                    lf.Add(f);
                }
            }
            return lf;
        }

        public static IList<Factura> GetFacturasCliente(int idCliente, MySqlConnection conn)
        {
            IList<Factura> lf = new List<Factura>();
            MySqlCommand cmd = conn.CreateCommand();
            string sql = @"SELECT 
                f.`imp_gastos_a_fo` AS APORTACION,
                f.`base_total` AS BASE_IVA,
                f.`id_cliente` AS CLIENTE_ID,
                c.nombre AS CLIENTE_NOMBRE,
                COALESCE(f.v_codtipom1,"") AS CODTIPOM,
                f.cuota_total AS CUOTA_IVA,
                f.es_fra_cliente AS ES_DE_CLIENTE,
                f.id_factura AS FACTURA_ID,
                f.strFecha AS FECHA,
                f.letra_id_fra_prove AS LETRA_PROVEEDOR,
                f.num_factura AS NUMFACTURA,
                f.imp_retencion AS RETENCION,
                f.num_serie AS SERIE,
                s.descripcion AS SISTEMA,
                f.ttal AS TOTAL,
                f.nueva AS NUEVA,
                d.coddirec AS CODDIREC,
                d.nombre AS DEPARTAMENTO,
                COALESCE(f.motivoFace,'') AS MOTIVO_FACE
                FROM factura AS f
                LEFT JOIN sistema AS s ON s.sistema_id = f.sistema_id
                LEFT JOIN cliente AS c ON c.i_d = f.id_cliente
                LEFT JOIN departamento AS d ON d.codclien = c.codclien_ariges AND d.coddirec = f.coddirec_ariges
                WHERE f.id_cliente = {0}";
            sql = String.Format(sql,idCliente);
            cmd.CommandText = sql;
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    Factura f = GetFactura(rdr);
                    lf.Add(f);
                }
            }
            return lf;
        }

        public static IList<Factura> GetFacturasNoEnviadas(MySqlConnection conn)
        {
            IList<Factura> lf = new List<Factura>();
            MySqlCommand cmd = conn.CreateCommand();
            string sql = @"SELECT 
                f.`imp_gastos_a_fo` AS APORTACION,
                f.`base_total` AS BASE_IVA,
                f.`id_cliente` AS CLIENTE_ID,
                c.nombre AS CLIENTE_NOMBRE,
                COALESCE(f.v_codtipom1,'') AS CODTIPOM,
                f.cuota_total AS CUOTA_IVA,
                f.es_fra_cliente AS ES_DE_CLIENTE,
                f.id_factura AS FACTURA_ID,
                f.Fecha AS FECHA,
                f.letra_id_fra_prove AS LETRA_PROVEEDOR,
                f.num_factura AS NUMFACTURA,
                f.imp_retencion AS RETENCION,
                f.num_serie AS SERIE,
                s.descripcion AS SISTEMA,
                f.ttal AS TOTAL,
                f.nueva AS NUEVA,
                d.coddirec AS CODDIREC,
                d.nombre AS DEPARTAMENTO,
                COALESCE(f.registroFace,'') AS REGISTRO_FACE,
                COALESCE(f.motivoFace,'') AS MOTIVO_FACE
                FROM factura AS f
                LEFT JOIN sistema AS s ON s.sistema_id = f.sistema_id
                LEFT JOIN cliente AS c ON c.i_d = f.id_cliente
                LEFT JOIN departamento AS d ON d.codclien = c.codclien_ariges AND d.coddirec = f.coddirec_ariges
                WHERE f.nueva < 2";
            cmd.CommandText = sql;
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    Factura f = GetFactura(rdr);
                    lf.Add(f);
                }
            }
            return lf;
        }

        
        #endregion

        #region Envios
        public static Envio GetEnvio(MySqlDataReader rdr)
        {
            if (rdr.IsDBNull(rdr.GetOrdinal("NIF"))) return null;
            Envio e = new Envio();
            e.Nif = rdr.GetString("NIF");
            e.ClienteId = rdr.GetInt32("CLIENTE_ID");
            e.ClienteNombre = rdr.GetString("CLIENTE_NOMBRE");
            e.DepartamentoId = rdr.GetInt32("DEPARTAMENTO_ID");
            e.DepartamentoNombre = rdr.GetString("DEPARTAMENTO_NOMBRE");
            e.StrFechaInicial = String.Format("{0:yyyyMMdd}", rdr.GetDateTime("FECHA_INICIAL"));
            e.StrFechaFinal = String.Format("{0:yyyyMMdd}", rdr.GetDateTime("FECHA_FINAL"));
            e.Bases = rdr.GetDecimal("BASES");
            e.Cuotas = rdr.GetDecimal("CUOTAS");
            e.Retencion = rdr.GetDecimal("RETENCIONES");
            e.Total = rdr.GetDecimal("TOTAL");
            if (rdr.GetString("FACE") != "") e.EsFace = true;
            return e;
        }

        public static Envio GetEnvio(int clienteId, int departamentoId, MySqlConnection conn)
        {
            Envio e = null;
            MySqlCommand cmd = conn.CreateCommand();
            string sql = @"SELECT
                        c.cif AS NIF,
                        c.i_d AS CLIENTE_ID,
                        c.nombre AS CLIENTE_NOMBRE,
                        COALESCE(d.departamento_id,0) AS DEPARTAMENTO_ID,
                        COALESCE(d.nombre, '') AS DEPARTAMENTO_NOMBRE,
                        MIN(f.fecha) AS FECHA_INICIAL,
                        MAX(f.fecha) AS FECHA_FINAL,
                        SUM(f.base_total) AS BASES,
                        SUM(f.cuota_total) AS CUOTAS,
                        SUM(f.imp_retencion) AS RETENCIONES,
                        SUM(f.ttal) AS TOTAL,
                        COALESCE(c.organoGestorCodigo, '') AS FACE
                        FROM factura AS f
                        LEFT JOIN cliente AS c ON c.i_d = f.id_cliente
                        LEFT JOIN departamento AS d ON d.codclien = c.codclien_ariges AND d.coddirec = f.coddirec_ariges
                        WHERE f.nueva = 1
                        AND f.id_cliente = {0} AND d.departamento_id = {1}
                        GROUP BY c.cif, c.i_d, d.departamento_id;";
            sql = String.Format(sql, clienteId, departamentoId);
            if (departamentoId == 0)
            {
                // hay que modificar el sql porque en realidad equivale a un nulo
                sql = @"SELECT
                        c.cif AS NIF,
                        c.i_d AS CLIENTE_ID,
                        c.nombre AS CLIENTE_NOMBRE,
                        COALESCE(d.departamento_id,0) AS DEPARTAMENTO_ID,
                        COALESCE(d.nombre, '') AS DEPARTAMENTO_NOMBRE,
                        MIN(f.fecha) AS FECHA_INICIAL,
                        MAX(f.fecha) AS FECHA_FINAL,
                        SUM(f.base_total) AS BASES,
                        SUM(f.cuota_total) AS CUOTAS,
                        SUM(f.imp_retencion) AS RETENCIONES,
                        SUM(f.ttal) AS TOTAL,
                        COALESCE(c.organoGestorCodigo, '') AS FACE
                        FROM factura AS f
                        LEFT JOIN cliente AS c ON c.i_d = f.id_cliente
                        LEFT JOIN departamento AS d ON d.codclien = c.codclien_ariges AND d.coddirec = f.coddirec_ariges
                        WHERE f.nueva = 1
                        AND f.id_cliente = {0} AND d.departamento_id IS NULL
                        GROUP BY c.cif, c.i_d, d.departamento_id;";
                sql = String.Format(sql, clienteId);
            }
            cmd.CommandText = sql;
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                e = GetEnvio(rdr);
            }
            rdr.Close();
            return e;
        }

        public static IList<Envio> GetEnvios(MySqlConnection conn)
        {
            IList<Envio> le = new List<Envio>();
            MySqlCommand cmd = conn.CreateCommand();
            string sql = @"SELECT
                        c.cif AS NIF,
                        c.i_d AS CLIENTE_ID,
                        c.nombre AS CLIENTE_NOMBRE,
                        COALESCE(d.departamento_id,0) AS DEPARTAMENTO_ID,
                        COALESCE(d.nombre, '') AS DEPARTAMENTO_NOMBRE,
                        MIN(f.fecha) AS FECHA_INICIAL,
                        MAX(f.fecha) AS FECHA_FINAL,
                        SUM(f.base_total) AS BASES,
                        SUM(f.cuota_total) AS CUOTAS,
                        SUM(f.imp_retencion) AS RETENCIONES,
                        SUM(f.ttal) AS TOTAL,
                        COALESCE(c.organoGestorCodigo, '') AS FACE
                        FROM factura AS f
                        LEFT JOIN cliente AS c ON c.i_d = f.id_cliente
                        LEFT JOIN departamento AS d ON d.codclien = c.codclien_ariges AND d.coddirec = f.coddirec_ariges
                        WHERE f.nueva = 1
                        GROUP BY c.cif, c.i_d, d.departamento_id;";
            cmd.CommandText = sql;
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    Envio e = GetEnvio(rdr);
                    le.Add(e);
                }
            }
            return le;
        }

        public static IList<Factura> GetFacturasEnvio(int idCliente, int idDepartamento, MySqlConnection conn)
        {
            IList<Factura> lf = new List<Factura>();
            MySqlCommand cmd = conn.CreateCommand();
            string sql = @"SELECT 
                f.`imp_gastos_a_fo` AS APORTACION,
                f.`base_total` AS BASE_IVA,
                f.`id_cliente` AS CLIENTE_ID,
                c.nombre AS CLIENTE_NOMBRE,
                COALESCE(f.v_codtipom1,'') AS CODTIPOM,
                f.cuota_total AS CUOTA_IVA,
                f.es_fra_cliente AS ES_DE_CLIENTE,
                f.id_factura AS FACTURA_ID,
                f.fecha AS FECHA,
                f.letra_id_fra_prove AS LETRA_PROVEEDOR,
                f.num_factura AS NUMFACTURA,
                f.imp_retencion AS RETENCION,
                f.num_serie AS SERIE,
                s.descripcion AS SISTEMA,
                f.ttal AS TOTAL,
                f.nueva AS NUEVA,
                d.coddirec AS CODDIREC,
                d.nombre AS DEPARTAMENTO,
                COALESCE(f.registroFace,'') AS REGISTRO_FACE,
                COALESCE(f.motivoFace,'') AS MOTIVO_FACE
                FROM factura AS f
                LEFT JOIN sistema AS s ON s.sistema_id = f.sistema_id
                LEFT JOIN cliente AS c ON c.i_d = f.id_cliente
                LEFT JOIN departamento AS d ON d.codclien = c.codclien_ariges AND d.coddirec = f.coddirec_ariges
                WHERE f.id_cliente = {0} AND d.departamento_id = {1} AND f.nueva=1";
            sql = String.Format(sql, idCliente, idDepartamento);
            if (idDepartamento == 0)
            {
                sql = @"SELECT 
                f.`imp_gastos_a_fo` AS APORTACION,
                f.`base_total` AS BASE_IVA,
                f.`id_cliente` AS CLIENTE_ID,
                c.nombre AS CLIENTE_NOMBRE,
                COALESCE(f.v_codtipom1,'') AS CODTIPOM,
                f.cuota_total AS CUOTA_IVA,
                f.es_fra_cliente AS ES_DE_CLIENTE,
                f.id_factura AS FACTURA_ID,
                f.fecha AS FECHA,
                f.letra_id_fra_prove AS LETRA_PROVEEDOR,
                f.num_factura AS NUMFACTURA,
                f.imp_retencion AS RETENCION,
                f.num_serie AS SERIE,
                s.descripcion AS SISTEMA,
                f.ttal AS TOTAL,
                f.nueva AS NUEVA,
                d.coddirec AS CODDIREC,
                d.nombre AS DEPARTAMENTO,
                COALESCE(f.registroFace,'') AS REGISTRO_FACE,
                COALESCE(f.motivoFace,'') AS MOTIVO_FACE
                FROM factura AS f
                LEFT JOIN sistema AS s ON s.sistema_id = f.sistema_id
                LEFT JOIN cliente AS c ON c.i_d = f.id_cliente
                LEFT JOIN departamento AS d ON d.codclien = c.codclien_ariges AND d.coddirec = f.coddirec_ariges
                WHERE f.id_cliente = {0} AND d.departamento_id IS NULL AND f.nueva=1";
                sql = String.Format(sql, idCliente);
            }
            cmd.CommandText = sql;
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    Factura f = GetFactura(rdr);
                    lf.Add(f);
                }
            }
            rdr.Close();
            return lf;
        }

        public static void EliminarFacturaDeEnvio(int facturaId, MySqlConnection conn)
        {
            MySqlCommand cmd = conn.CreateCommand();
            string sql = @"UPDATE factura SET nueva = 0 WHERE id_factura = {0}";
            sql = String.Format(sql, facturaId);
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }

        public static void RecuperarFacturaDeEnvio(int facturaId, MySqlConnection conn)
        {
            MySqlCommand cmd = conn.CreateCommand();
            string sql = @"UPDATE factura SET nueva = 1 WHERE id_factura = {0}";
            sql = String.Format(sql, facturaId);
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }

        public static void MarcarFacturaEnviada(int facturaId, MySqlConnection conn)
        {
            MySqlCommand cmd = conn.CreateCommand();
            string sql = @"UPDATE factura SET nueva = 2 WHERE id_factura = {0}";
            sql = String.Format(sql, facturaId);
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }

        public static string SendEnvio(int clienteId, int departamentoId, string certSN, MySqlConnection conn)
        {
            // obtenemos los datos del envío
            Envio e = GetEnvio(clienteId, departamentoId, conn);
            // la lista de facturas
            IList<Factura> lf = GetFacturasEnvio(clienteId, departamentoId, conn);
            // leemos la plantilla para envio por correo
            Plantilla p = GetPlantilla(1, conn);
            // leemos el cliente
            Cliente c = GetCliente(clienteId, conn);
            // este será el mensaje de vuelta
            string mens = String.Format("ENVIO--> Cliente: {0} Departamento: {1} Desde: {2} Hasta: {3} TOTAL: {4:0.00} <br/>",e.ClienteNombre, e.DepartamentoNombre, e.StrFechaInicial, e.StrFechaFinal, e.Total);
            if (!e.EsFace)
            {
                // NO FACE: Es una simple notificación por correo electrónico.
                // obtenemos el correo electrónico al que hay que mandar 
                if (c.Email != null && c.Email != "")
                {
                    int i=0;
                    ArrayList adjuntos = new ArrayList();
                    string fichero = "";
                    string repositorio = "";
                    string detalleFacturas = "";
                    // Ahora preparamos las facturas
                    foreach (Factura f in lf)
                    {
                        i++;
                        // Montar los adjuntos
                        repositorio = GetRepositorio(conn);
                        fichero = repositorio + NombreFicheroFactura(f, c) + ".pdf";
                        DateTime fechaFactura = new DateTime(int.Parse(f.StrFecha.Substring(0, 4)),
                            int.Parse(f.StrFecha.Substring(4, 2)), 
                            int.Parse(f.StrFecha.Substring(6, 2)));
                        detalleFacturas += String.Format("<strong>Serie:</strong>{0} <strong>Número:</strong>{1} <strong>Fecha:</strong>{2:dd/MM/yyyy} <strong>Importe (con IVA):</strong>{3:0.00} <br/>",
                            f.Serie, f.NumFactura, fechaFactura, f.Total);
                        adjuntos.Add(fichero);
                    }
                    // Montamos el correo electrónico.
                    string asunto = "[ARIFACE] Notificación de facturas electrónicas";
                    string cuerpo = String.Format(p.Contenido, c.Nombre, detalleFacturas);
                    try
                    {
                        CntAriFaceLib.SendEmailCliente(c.Email, asunto, cuerpo, adjuntos);
                    }
                    catch (Exception ex)
                    {
                        mens += String.Format("Error correo electrónico: {0}", ex.Message);
                        return mens;
                    }
                    // ahora marcamos las facturas como enviadas
                    foreach (Factura f in lf)
                    {
                        MarcarFacturaEnviada(f.FacturaId, conn);
                    }
                    mens += String.Format("({0}) {1} facturas CORRECTAS <br/>", c.Email, i);
                }
                else
                {
                    mens += String.Format("El cliente {0} no tiene un correo electrónico, no se le ha podido notificar <br/>", c.Nombre);
                }

            }
            mens += "---------------------------- <br/>";
            return mens;
        }

        public static string NombreFicheroFactura(Factura f, Cliente c)
        {
            string n = "";
            string mes = f.StrFecha.Substring(4, 2);
            string ano = f.StrFecha.Substring(0, 4);
            if (f.EsDeCliente)
            {
                n = String.Format("{0}{1}_{2:00}{3:0000}{4}", f.Serie, f.NumFactura, mes, ano, f.LetraProveedor);
            }
            else
            {
                // es de proveedor (sumiendo que es socio ariagro)
                string serie = String.Format(@"{0:000000}{1}", c.CodSocioAriagro, f.Serie);
                if (c.CodSocioAritaxi != 0)
                {
                    serie = String.Format(@"{0:000000}{1}", c.CodSocioAritaxi, f.Serie);
                }
                n = String.Format("{0}{1}_{2:00}{3:0000}{4}", serie, f.NumFactura, mes, ano, f.LetraProveedor);
            }
            return n;
        }
               

        #endregion

        #region Empresa raiz
        public static EmpresaRaiz GetEmpresaRaiz(MySqlDataReader rdr)
        {
            if (rdr.IsDBNull(rdr.GetOrdinal("nif"))) return null;
            EmpresaRaiz er = new EmpresaRaiz();
            er.Nif = rdr.GetString("nif");
            er.Nombre = rdr.GetString("nombre");
            return er;
        }

        public static EmpresaRaiz GetEmpresaRaiz(string nif, MySqlConnection conn)
        {
            EmpresaRaiz er = null;
            MySqlCommand cmd = conn.CreateCommand();
            string sql = @"SELECT * FROM nifbase WHERE nif = '{0}'";
            sql = String.Format(sql, nif);
            cmd.CommandText = sql;
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                er = GetEmpresaRaiz(rdr);
            }
            return er;
        }

        public static IList<EmpresaRaiz> GetEmpresasRaiz(MySqlConnection conn)
        {
            IList<EmpresaRaiz> ler = new List<EmpresaRaiz>();
            MySqlCommand cmd = conn.CreateCommand();
            string sql = "SELECT * FROM nifbase";
            cmd.CommandText = sql;
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    EmpresaRaiz er = GetEmpresaRaiz(rdr);
                    ler.Add(er);
                }
            }
            return ler;
        }

        public static IList<EmpresaRaiz> GetEmpresasRaiz(string parNom, MySqlConnection conn)
        {
            IList<EmpresaRaiz> ler = new List<EmpresaRaiz>();
            MySqlCommand cmd = conn.CreateCommand();
            string sql = "SELECT * FROM nifbase WHERE nombre LIKE '%{0}%' ORDER BY nombre;";
            if (parNom == "*")
                sql = "SELECT * FROM nifbase ORDER BY nombre;";
            sql = String.Format(sql, parNom);
            cmd.CommandText = sql;
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    EmpresaRaiz er = GetEmpresaRaiz(rdr);
                    ler.Add(er);
                }
            }
            return ler;
        }

        public static EmpresaRaiz SetEmpresaRaiz(EmpresaRaiz er, MySqlConnection conn)
        {
            bool alta = false;
            if (er == null) return null;
            if (er.Nif == "")
            {
                alta = true;
            }
            // si el id es 0 se crea el objeto, si no se actualiza.
            MySqlCommand cmd = conn.CreateCommand();
            string sql = "";
            if (alta)
            {
                sql = @"
                    INSERT INTO nifbase
                    (nif, nombre)
                    VALUES ('{0}','{1}');
                ";
            }
            else
            {
                sql = @"
                    UPDATE nifbase
                    SET
                    nombre='{1}'
                    WHERE nif='{0}';
                ";
            }
            sql = String.Format(sql, er.Nif, er.Nombre);
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
            // y vamos rápidamente a por la recién creada
            sql = @"SELECT * FROM nifbase WHERE nif='{0}';";
            sql = String.Format(sql, er.Nif);
            cmd.CommandText = sql;
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                er = GetEmpresaRaiz(rdr);
            }
            return er;
        }

        public static void DeleteEmpresaRaiz(string nif, MySqlConnection conn)
        {
            MySqlCommand cmd = conn.CreateCommand();
            string sql = String.Format("DELETE FROM nifbase WHERE nif='{0}'", nif);
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }
        #endregion

        #region Plantillas
        public static Plantilla GetPlantilla(MySqlDataReader rdr)
        {
            if (rdr.IsDBNull(rdr.GetOrdinal("plantilla_id"))) return null;
            Plantilla p = new Plantilla();
            p.PlantillaId = rdr.GetInt32("plantilla_id");
            if (!rdr.IsDBNull(rdr.GetOrdinal("nombre")))
                p.Nombre = rdr.GetString("nombre");
            if (!rdr.IsDBNull(rdr.GetOrdinal("contenido")))
                p.Contenido = rdr.GetString("contenido");
            if (!rdr.IsDBNull(rdr.GetOrdinal("observaciones")))
                p.Observaciones = rdr.GetString("observaciones");
            return p;
        }

        public static Plantilla GetPlantilla(int plantillaId, MySqlConnection conn)
        {
            Plantilla p = null;
            MySqlCommand cmd = conn.CreateCommand();
            string sql = @"SELECT * FROM plantilla WHERE plantilla_id = '{0}'";
            sql = String.Format(sql, plantillaId);
            cmd.CommandText = sql;
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                p = GetPlantilla(rdr);
            }
            rdr.Close();
            return p;
        }

        public static IList<Plantilla> GetPlantillas(MySqlConnection conn)
        {
            IList<Plantilla> lp = new List<Plantilla>();
            MySqlCommand cmd = conn.CreateCommand();
            string sql = "SELECT * FROM plantilla";
            cmd.CommandText = sql;
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    Plantilla p = GetPlantilla(rdr);
                    lp.Add(p);
                }
            }
            return lp;
        }

        public static Plantilla SetPlantilla(Plantilla p, MySqlConnection conn)
        {
            // si el id es 0 se crea el objeto, si no se actualiza.
            MySqlCommand cmd = conn.CreateCommand();
            string sql = @"
                    INSERT INTO plantilla
                    (plantilla_id, nombre, contenido)
                    VALUES ({0},'{1}','{2}')
                    ON DUPLICATE KEY UPDATE
                    nombre='{1}',
                    contenido='{2}'
            ";
            sql = String.Format(sql, p.PlantillaId, p.Nombre, p.Contenido);
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
            sql = @"SELECT * FROM plantilla WHERE plantilla_id={0};";
            sql = String.Format(sql, p.PlantillaId);
            cmd.CommandText = sql;
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                p = GetPlantilla(rdr);
            }
            rdr.Close();
            return p;
        }

        public static void DeletePlantilla(int plantillaId, MySqlConnection conn)
        {
            MySqlCommand cmd = conn.CreateCommand();
            string sql = String.Format("DELETE FROM plantilla WHERE plantilla_id={0}", plantillaId);
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }

        #endregion

        #region Repositorio
        public static string GetRepositorio(MySqlConnection conn)
        {
            string r = "";
            MySqlCommand cmd = conn.CreateCommand();
            string sql = "SELECT * FROM repositorio";
            cmd.CommandText = sql;
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                r = rdr.GetString("path");
            }
            rdr.Close();
            return r;
        }
        #endregion

        #region Manejo de correos electrónicos
        public static SmtpClient GetClienteSmtp()
        {
            SmtpClient smtp;
            smtp = new SmtpClient(ConfigurationSettings.AppSettings["mail_server"]);
            smtp.Credentials = new NetworkCredential(ConfigurationSettings.AppSettings["mail_usr"], ConfigurationSettings.AppSettings["mail_pass"]);
            smtp.Port = int.Parse(ConfigurationSettings.AppSettings["mail_port"]);

            smtp.EnableSsl = bool.Parse(ConfigurationSettings.AppSettings["mail_ssl"]);
            return smtp;
        }

        public static void SendEmailCliente(string emailCliente, string asunto, string cuerpo, ArrayList adjuntos)
        {
            SmtpClient smtp = GetClienteSmtp();
            MailMessage correo = new MailMessage();
            correo.From = new MailAddress(ConfigurationSettings.AppSettings["mail_address"]);
            correo.To.Add(emailCliente);
            // siempre copia oculta a la dirección base.
            correo.Bcc.Add(ConfigurationSettings.AppSettings["mail_address"]);
            if (adjuntos.Count > 0)
            {
                foreach (string fileName in adjuntos)
                {
                    Attachment data = new Attachment(fileName);
                    // Add time stamp information for the file.
                    // Add the file attachment to this e-mail message.
                    correo.Attachments.Add(data);
                }
            }
            correo.Subject = asunto;
            correo.Body = cuerpo;
            correo.IsBodyHtml = true;
            correo.Priority = System.Net.Mail.MailPriority.Normal;
            smtp.Send(correo);
        }
        #endregion
    }
}
