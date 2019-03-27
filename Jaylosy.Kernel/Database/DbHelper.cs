using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using System.Reflection;
using Jaylosy.Kernel.Attribute;
using System.IO;

namespace Jaylosy.Kernel.DataBase
{
    public enum DbProvider
    {
        MSSQL,
        ORACLE,
        ManagedORACLE,

    }
    public sealed class DbHelper
    {
        string sqlPath = "C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319\\System.Data.dll";
        string oraclePath = "C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319\\System.Data.OracleClient.dll";
        string oracleManagerPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Oracle.ManagedDataAccess.dll");
        string conStr;
        IDbConnection _con;
        IDbConnection con
        {
            get
            {
                if (_con == null)
                {
                    throw new Exception("数据库连接未打开不允许操作");
                }

                if (!IsDbOpened())
                {
                    OpenConnection();
                }

                return _con;
            }
        }
        DbProvider DbProvider;
        public bool IsDbOpened()
        {
            if (_con == null)
            {
                return false;
            }
            if (_con.State == ConnectionState.Broken || _con.State == ConnectionState.Closed|| _con.State == ConnectionState.Connecting)
            {
                return false;
            }
            return true;
        }
        IDbConnection CreateConnection(string conStr)
        {
            if (DbProvider == DbProvider.MSSQL)
            {
                var con = CreateInstance<IDbConnection>(sqlPath, "System.Data.SqlClient.SqlConnection");
                con.ConnectionString = conStr;
                return con;
            }
            else if (DbProvider == DbProvider.ORACLE)
            {
                var con = CreateInstance<IDbConnection>(oraclePath, "System.Data.OracleClient.OracleConnection");
                con.ConnectionString = conStr;
                return con;
            }
            else if (DbProvider == DbProvider.ManagedORACLE)
            {
                var con = CreateInstance<IDbConnection>(oracleManagerPath, "Oracle.ManagedDataAccess.Client.OracleConnection");
                con.ConnectionString = conStr;
                return con;
            }
            else
            {
                throw UnkownProviderNameException();
            }
        }

        Exception UnkownProviderNameException()
        {
            return new Exception("不支持的数据库类型");
        }

        public void OpenConnection()
        {
            _con = CreateConnection(conStr);
            _con.Open();
        }
        public IDbTransaction BeginTransaction()
        {
            return con.BeginTransaction();
        }

        public IDbDataParameter CreateDbParameter()
        {
            if(DbProvider== DbProvider.MSSQL)
            {
                var parameter = CreateInstance<IDbDataParameter>(sqlPath, "System.Data.SqlClient.SqlParameter");
                return parameter;
            }
            else if (DbProvider == DbProvider.ORACLE)
            {
                var parameter = CreateInstance<IDbDataParameter>(oraclePath, "System.Data.OracleClient.OracleParameter");
                return parameter;
            }
            else if (DbProvider == DbProvider.ManagedORACLE)
            {
                var parameter = CreateInstance<IDbDataParameter>(sqlPath, "Oracle.ManagedDataAccess.Client.OracleParameter");
                return parameter;
            }
            else
            {
                throw UnkownProviderNameException();
            }
        }

        public void CloseConnection()
        {
            if (IsDbOpened())
            {
                _con.Close();
            }
            _con = null;
        }
        IDbDataAdapter CreateDataAdapter(IDbCommand cmd)
        {
            if (DbProvider == DbProvider.MSSQL)
            {
                var dapt = CreateInstance<IDbDataAdapter>(sqlPath, "System.Data.SqlClient.SqlDataAdapter");
                dapt.SelectCommand = cmd;
                return dapt;
            }
            else if (DbProvider == DbProvider.ORACLE)
            {
                var dapt = CreateInstance<IDbDataAdapter>(oraclePath, "System.Data.OracleClient.OracleDataAdapter");
                dapt.SelectCommand = cmd;
                return dapt;
            }
            else if(DbProvider == DbProvider.ManagedORACLE)
            {
                var dapt = CreateInstance<IDbDataAdapter>(oracleManagerPath, "Oracle.ManagedDataAccess.Client.OracleDataAdapter");
                dapt.SelectCommand = cmd;
                return dapt;
            }
            else
            {
                throw UnkownProviderNameException();
            }
        }

        T CreateInstance<T>(string assemblyString, string typeName) where T : class
        {
            var obj = Assembly.LoadFrom(assemblyString).CreateInstance(typeName);
            if(obj is T)
            {
                return (obj as T);
            }
            else
            {
                throw new Exception("实例化对象出错");
            }
        }
        public DbHelper(string dbName)
        {
            var conStrs = ConfigurationManager.ConnectionStrings[dbName];
            conStr = conStrs.ConnectionString;
            if (conStrs.ProviderName == "System.Data.SqlClient")
            {
                DbProvider = DbProvider.MSSQL;
            }
            else if (conStrs.ProviderName == "System.Data.OracleClient")
            {
                DbProvider = DbProvider.ORACLE;
            }
            else if (conStrs.ProviderName == "Oracle.ManagedDataAccess.Client")
            {
                DbProvider = DbProvider.ManagedORACLE;
            }
            else
            {
                throw UnkownProviderNameException();
            }
        }
        public DataTable QueryDatatable(string sql)
        {
            return QueryDatatable(sql, null);
        }
        public DataTable QueryDatatable(string sql,IDbDataParameter[] parameters)
        {
            var cmd = con.CreateCommand();
            cmd.CommandText = sql;
            if (parameters != null)
            {
                foreach(var parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
            }
            IDbDataAdapter dapt = CreateDataAdapter(cmd);
            DataSet ds = new DataSet();
            dapt.Fill(ds);
            if (ds.Tables.Count > 0)
            {
                return ds.Tables[0];
            }
            return null ;
        }
        public int ExecuteNonQuery(string sql)
        {
            return ExecuteNonQuery(sql, null);
        }
        public int ExecuteNonQuery(string sql, IDbDataParameter[] parameters)
        {
            var cmd = con.CreateCommand();
            cmd.CommandText = sql;
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
            }
            int ret = cmd.ExecuteNonQuery();
            return ret;
        }

        public int ExecuteNonQuery(IDbTransaction trans,string sql)
        {
            return ExecuteNonQuery(trans, sql, null);
        }
        public int ExecuteNonQuery(IDbTransaction trans, string sql,IDbDataParameter[] parameters)
        {
            var cmd = con.CreateCommand();
            cmd.CommandText = sql;
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
            }
            cmd.Transaction = trans;
            int ret = cmd.ExecuteNonQuery();
            return ret;
        }
        public object ExecuteScalar(string sql)
        {
            return ExecuteScalar(sql, null);
        }
        public object ExecuteScalar(string sql, IDbDataParameter[] parameters)
        {
            var cmd = con.CreateCommand();
            cmd.CommandText = sql;
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
            }
            object ret = cmd.ExecuteScalar();
            return ret;
        }
        public List<T> QueryDataList<T>(string sql) where T:class,new()
        {
            return QueryDataList<T>(sql, null);
        }
        public List<T> QueryDataList<T>(string sql, IDbDataParameter[] parameters) where T : class, new()
        {
            List<T> dataList = new List<T>();
            var cmd = con.CreateCommand();
            cmd.CommandText = sql;
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
            }
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                T t = new T();
                foreach (PropertyInfo p in t.GetType().GetProperties())
                {
                    var attributes = p.GetCustomAttributes(false);
                    if (attributes.Length > 0)
                    {
                        bool isContinue = false;
                        foreach (var attribute in attributes)
                        {
                            if (attribute.GetType() == typeof(IgnoreAttribute))
                            {
                                isContinue = true;
                                break;
                            }
                        }

                        if (isContinue)
                        {
                            continue;
                        }
                    }
                    if (p.PropertyType == typeof(string))
                    {
                        p.SetValue(t, reader[p.Name].AsString(), null);
                    }
                    if (p.PropertyType == typeof(int))
                    {
                        p.SetValue(t, reader[p.Name].AsInt(), null);
                    }
                    if (p.PropertyType == typeof(long))
                    {
                        p.SetValue(t, reader[p.Name].AsLong(), null);
                    }
                    if (p.PropertyType == typeof(double))
                    {
                        p.SetValue(t, reader[p.Name].AsDouble(), null);
                    }
                    if (p.PropertyType == typeof(decimal))
                    {
                        p.SetValue(t, reader[p.Name].AsDecimal(), null);
                    }
                    if (p.PropertyType == typeof(DateTime))
                    {
                        p.SetValue(t, reader[p.Name].AsDateTime(), null);
                    }
                    if (p.PropertyType == typeof(bool))
                    {
                        p.SetValue(t, reader[p.Name].AsBoolean(), null);
                    }
                }
                dataList.Add(t);
            }
            return dataList;
        }
    }
}
