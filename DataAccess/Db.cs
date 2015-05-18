using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public static class Db
    {
        // Note: Static initializers are thread safe.
        // If this class had a static constructor then these static variables 
        // would need to be initialized there.
        private static readonly string dataProvider = ConfigurationManager.AppSettings.Get("DataProvider");
        private static readonly DbProviderFactory factory = DbProviderFactories.GetFactory(dataProvider);

        //private static readonly string connectionStringName = ConfigurationManager.AppSettings.Get("ConnectionStringName");
        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        #region Fast data readers
        public static T Read<T>(string sp) where T : class ,new()
        {
            List<T> list = ReadList<T>(sp);
            return list.Count() > 0 ? list[0] : null;
        }

        public static T Read<T>(string sp, T o) where T : class ,new()
        {
            List<T> list = ReadList<T>(sp, o);
            return list.Count() > 0 ? list[0] : null;
        }

        public static List<T> ReadList<T>(string sp) where T : class ,new()
        {
            List<T> list = new List<T>();
            using (var connection = factory.CreateConnection())
            {
                connection.ConnectionString = connectionString;

                using (var command = connection.CreateCommand())
                {
                    connection.Open();

                    command.Connection = connection;
                    command.CommandText = sp;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 15 * 60;

                    var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                    while (reader.Read())
                    {
                        T e = new T();
                        SetProperty(reader, e);
                        list.Add(e);
                    }

                    reader.Close();
                }
            }
            return list;
        }

        public static List<T> ReadList<T>(string sp, T o) where T : class ,new()
        {
            List<T> list = new List<T>();
            using (var connection = factory.CreateConnection())
            {
                connection.ConnectionString = connectionString;

                List<SqlParameter> parameters = GetParameters(o, sp, connection);

                using (var command = connection.CreateCommand())
                {
                    connection.Open();

                    command.Connection = connection;
                    command.CommandText = sp;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(parameters.ToArray());

                    var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                    while (reader.Read())
                    {
                        T e = new T();
                        SetProperty(reader, e);
                        list.Add(e);
                    }

                    reader.Close();

                    if (parameters != null)
                    {
                        var outParameters = parameters.Where(p => p.Direction == ParameterDirection.Output);
                        foreach (var param in outParameters)
                        {
                            SetProperty(command, o, param);
                        }
                    }
                }
            }
            return list;
        }

        public static List<T> ReadList<T, D>(string sp, D o) where T : class ,new()
        {
            List<T> list = new List<T>();
            using (var connection = factory.CreateConnection())
            {
                connection.ConnectionString = connectionString;

                List<SqlParameter> parameters = GetParameters(o, sp, connection);

                using (var command = connection.CreateCommand())
                {
                    connection.Open();

                    command.Connection = connection;
                    command.CommandText = sp;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(parameters.ToArray());

                    var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                    while (reader.Read())
                    {
                        T e = new T();
                        SetProperty(reader, e);
                        list.Add(e);
                    }

                    reader.Close();

                    if (parameters != null)
                    {
                        var outParameters = parameters.Where(p => p.Direction == ParameterDirection.Output);
                        foreach (var param in outParameters)
                        {
                            SetProperty(command, o, param);
                        }
                    }
                }
            }
            return list;
        }

        // Added by Keerati 02/18/2015 MM/dd/yyyy
        public static T Read<T>(string sp, string o) where T : class ,new()
        {
            T t = new T();
            using (var connection = factory.CreateConnection())
            {
                connection.ConnectionString = connectionString;

                using (var command = connection.CreateCommand())
                {
                    connection.Open();

                    command.Connection = connection;
                    command.CommandText = sp;
                    command.CommandType = CommandType.StoredProcedure;

                    DbParameter dbp = command.CreateParameter();
                    dbp.DbType = DbType.Xml;
                    dbp.Value = o;
                    dbp.ParameterName = "xml";
                    command.Parameters.Add(dbp);

                    command.CommandTimeout = 90 * 60;
                    o = string.Empty;
                    dbp = null;
                    GC.Collect();

                    var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                    if (reader.Read())
                    {
                        SetProperty(reader, t);
                    }

                    reader.Close();
                }
            }
            return t;
        }

        public static T Read<T>(string sp, string xml, string filename) where T : class ,new()
        {
            T t = new T();
            using (var connection = factory.CreateConnection())
            {
                connection.ConnectionString = connectionString;

                using (var command = connection.CreateCommand())
                {
                    connection.Open();

                    command.Connection = connection;
                    command.CommandText = sp;
                    command.CommandType = CommandType.StoredProcedure;

                    DbParameter dbp = command.CreateParameter();
                    dbp.DbType = DbType.Xml;
                    dbp.Value = xml;
                    dbp.ParameterName = "xml";
                    command.Parameters.Add(dbp);

                    DbParameter dbp2 = command.CreateParameter();
                    dbp2.DbType = DbType.String;
                    dbp2.Value = filename;
                    dbp2.ParameterName = "FILENAME";
                    command.Parameters.Add(dbp2);

                    command.CommandTimeout = 3 * 60;
                    xml = string.Empty;

                    var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                    dbp = null;
                    dbp2 = null;
                    GC.Collect();
                    if (reader.Read())
                    {
                        SetProperty(reader, t);
                    }

                    reader.Close();
                }
            }
            return t;
        }

        // Added by Keerati 02/18/2015 MM/dd/yyyy
        public static T Read<T>(string sp, string xml, string FILENAME, string branchID) where T : class ,new()
        {
            T t = new T();
            using (var connection = factory.CreateConnection())
            {
                connection.ConnectionString = connectionString;

                using (var command = connection.CreateCommand())
                {
                    connection.Open();

                    command.Connection = connection;
                    command.CommandText = sp;
                    command.CommandType = CommandType.StoredProcedure;

                    DbParameter dbp = command.CreateParameter();
                    dbp.DbType = DbType.Xml;
                    dbp.Value = xml;
                    dbp.ParameterName = "xml";
                    command.Parameters.Add(dbp);

                    DbParameter dbp2 = command.CreateParameter();
                    dbp2.DbType = DbType.String;
                    dbp2.Value = FILENAME;
                    dbp2.ParameterName = "FILENAME";
                    command.Parameters.Add(dbp2);

                    DbParameter dbp3 = command.CreateParameter();
                    dbp3.DbType = DbType.String;
                    dbp3.Value = branchID;
                    dbp3.ParameterName = "CONOWW";
                    command.Parameters.Add(dbp3);

                    command.CommandTimeout = 3 * 60;
                    xml = string.Empty;

                    var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                    dbp = null;
                    dbp2 = null;
                    GC.Collect();
                    if (reader.Read())
                    {
                        SetProperty(reader, t);
                    }

                    reader.Close();
                }
            }
            return t;
        }

        // Added by Keerati 02/18/2015 MM/dd/yyyy
        public static T Read<T, D>(string sp, D o) where T : class ,new()
        {
            T t = new T();
            using (var connection = factory.CreateConnection())
            {
                connection.ConnectionString = connectionString;
                List<SqlParameter> parameters = GetParameters(o, sp, connection);

                using (var command = connection.CreateCommand())
                {
                    connection.Open();

                    command.Connection = connection;
                    command.CommandText = sp;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(parameters.ToArray());

                    command.CommandTimeout = 90 * 60;

                    var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                    if (reader.Read())
                    {
                        SetProperty(reader, t);
                    }

                    reader.Close();
                }
            }
            return t;
        }

        public static string Readd<D>(string sp, D o)
        {
            string t = string.Empty;
            using (var connection = factory.CreateConnection())
            {
                connection.ConnectionString = connectionString;
                List<SqlParameter> parameters = GetParameters(o, sp, connection);

                using (var command = connection.CreateCommand())
                {
                    connection.Open();

                    command.Connection = connection;
                    command.CommandText = sp;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(parameters.ToArray());

                    command.CommandTimeout = 90 * 60;

                    var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                    if (reader.Read())
                    {
                        t = reader["ErrorMessage"].ToString();
                    }

                    reader.Close();
                }
            }
            return t;
        }

        public static T ReadList<T>(string sp, List<T> o, out string errMsg) where T : class ,new()
        {
            errMsg = string.Empty;
            T t = new T();
            int count = 0;
            DbTransaction tran = null;
            try
            {
                using (var connection = factory.CreateConnection())
                {
                    connection.ConnectionString = connectionString;
                    using (var command = connection.CreateCommand())
                    {
                        List<List<SqlParameter>> arr = GetParameters<T>(o, sp, connection);

                        connection.Open();
                        command.Connection = connection;
                        command.CommandText = sp;
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 5 * 60;

                        tran = connection.BeginTransaction();
                        command.Transaction = tran;
                        int listCount = o.Count;

                        while (arr.Count > 0)
                        {
                            t = o[count];
                            command.Parameters.Clear();
                            command.Parameters.AddRange(arr[0].ToArray());
                            var result = command.ExecuteNonQuery();
                            arr.RemoveAt(0);
                            t = null;
                            if ((count + 1) % 300 == 0)
                            {
                                GC.Collect();
                            }
                            count++;
                        }
                        tran.Commit();
                    }
                }
            }
            catch (OutOfMemoryException e)
            {
                t = o[count];
                errMsg = e.Message;
                if (tran != null)
                {
                    if (tran.Connection != null)
                    {
                        tran.Rollback();
                    }
                    tran.Dispose();
                }
                return t;
            }
            catch (Exception e)
            {
                t = o[count];
                errMsg = e.Message;
                if (tran != null)
                {
                    if (tran.Connection != null)
                    {
                        tran.Rollback();
                    }
                    tran.Dispose();
                }
                return t;
            }
            return null;
        }
        #endregion

        #region Data update sections

        public static void Insert<T>(string sp, T o) where T : class, new()
        {
            using (var connection = factory.CreateConnection() as SqlConnection)
            {
                connection.ConnectionString = connectionString;

                List<SqlParameter> parameters = GetParameters(o, sp, connection);

                connection.Open();

                SqlCommand command = connection.CreateCommand();
                SqlTransaction transaction = connection.BeginTransaction("tx_" + DateTime.Now.ToString("HH:mm:ss"));

                try
                {

                    command.Connection = connection;
                    command.Transaction = transaction;
                    command.CommandText = sp;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(parameters.ToArray());

                    var result = command.ExecuteNonQuery();

                    // Attempt to commit the transaction.
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }

            }
        }

        public static void Insert<T>(string sp, List<T> list) where T : class, new()
        {
            using (var connection = factory.CreateConnection() as SqlConnection)
            {
                connection.ConnectionString = connectionString;

                var command = connection.CreateCommand();
                SqlTransaction transaction = null;

                try
                {

                    List<List<SqlParameter>> arr = GetParameters<T>(list, sp, connection);

                    connection.Open();

                    command.Connection = connection;
                    command.CommandText = sp;
                    command.CommandType = CommandType.StoredProcedure;

                    transaction = connection.BeginTransaction("tx_" + DateTime.Now.ToString("HH:mm:ss"));
                    command.Transaction = transaction;

                    for (int i = 0; i < list.Count(); i++)
                    {
                        command.Parameters.Clear();
                        List<SqlParameter> paramter = arr[i];
                        command.Parameters.AddRange(paramter.ToArray());
                        var result = command.ExecuteNonQuery();
                    }

                    // Attempt to commit the transaction.
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public static void Update<T>(string sp, T o) where T : class, new()
        {
            using (var connection = factory.CreateConnection() as SqlConnection)
            {
                connection.ConnectionString = connectionString;

                List<SqlParameter> parameters = GetParameters(o, sp, connection);

                connection.Open();

                SqlCommand command = connection.CreateCommand();
                SqlTransaction transaction = connection.BeginTransaction("tx_" + DateTime.Now.ToString("HH:mm:ss"));

                try
                {

                    command.Connection = connection;
                    command.Transaction = transaction;
                    command.CommandText = sp;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(parameters.ToArray());

                    var result = command.ExecuteNonQuery();

                    // Attempt to commit the transaction.
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }

            }

        }

        public static void Delete<T>(string sp, T o) where T : class, new()
        {
            Update<T>(sp, o);
        }

        #endregion

        #region Support Methods
        private static void SetProperty(IDbCommand command, object e, SqlParameter param)
        {
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(e))
            {
                string name = property.Name;
                bool exists = param.ParameterName.Replace("@", "").Equals(name);
                if (exists)
                {
                    SetProperty(e, property, command.Parameters, param.ParameterName);
                }

            }
        }

        private static void SetProperty(object e, PropertyDescriptor property, IDataParameterCollection parameters, string name)
        {
            SqlParameter parameter = parameters[name] as SqlParameter;
            if (property.PropertyType.Equals(typeof(int)) && parameter.SqlDbType == SqlDbType.Int)
            {
                if (parameter.Value != DBNull.Value) property.SetValue(e, Convert.ToInt32(parameter.Value));
            }
            else if (property.PropertyType.Equals(typeof(decimal)) && parameter.SqlDbType == SqlDbType.Decimal)
            {
                if (parameter.Value != DBNull.Value) property.SetValue(e, Convert.ToDecimal(parameter.Value));
            }
            else if (property.PropertyType.Equals(typeof(float)) && parameter.SqlDbType == SqlDbType.Float)
            {
                if (parameter.Value != DBNull.Value) property.SetValue(e, Convert.ToDouble(parameter.Value));
            }
            else
            {
                if (parameter.Value != DBNull.Value)
                {
                    property.SetValue(e, parameter.Value);
                }
            }
        }

        private static void SetProperty(IDataReader reader, object e)
        {
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(e))
            {
                string name = property.Name;
                bool exists = Enumerable.Range(0, reader.FieldCount).Any(i => reader.GetName(i).ToUpper() == name.ToUpper());

                if (exists)
                {
                    object value = GetValueFromReader(reader, name);
                    Type t = property.PropertyType;
                    if (t.Equals(typeof(string)))
                    {
                        property.SetValue(e, value == DBNull.Value || value == null ? null : value.ToString());
                    }
                    else
                    {
                        property.SetValue(e, value == DBNull.Value ? null : value);
                    }
                }

            }
        }

        private static object GetValueFromReader(IDataReader reader, string name)
        {
            object value = null;

            int ordinal = reader.GetOrdinal(name);
            Type fieldType = reader.GetFieldType(ordinal);

            if (!reader.IsDBNull(ordinal))
            {
                if (fieldType.Equals(typeof(string)))
                {
                    value = reader.GetString(ordinal);
                }
                else if (fieldType.Equals(typeof(int)))
                {
                    value = reader.GetInt32(ordinal);
                }
                else if (fieldType.Equals(typeof(double)))
                {
                    value = reader.GetDouble(ordinal);
                }
                else if (fieldType.Equals(typeof(decimal)))
                {
                    value = reader.GetDecimal(ordinal);
                }
                else if (fieldType.Equals(typeof(float)))
                {
                    value = reader.GetFloat(ordinal);
                }
                else if (fieldType.Equals(typeof(DateTime)))
                {
                    value = reader.GetDateTime(ordinal);
                }
                else if (fieldType.Equals(typeof(bool)))
                {
                    value = reader.GetBoolean(ordinal);
                }
            }

            //return value == null? null : value.ToString();
            return value == null ? null : value;
        }

        private static object GetPropertyValue(string name, object e)
        {
            object value = DBNull.Value;

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(e))
            {
                if (name.Replace("@", "").ToUpper() == property.Name.ToUpper())
                {
                    value = property.GetValue(e);
                    return value ?? DBNull.Value;
                }
            }

            return value;
        }

        private static List<SqlParameter> GetParameters(object o, string sp, IDbConnection connection)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            #region Query

            string sql = @"
                        SELECT [SPECIFIC_CATALOG]
                              ,[SPECIFIC_SCHEMA]
                              ,[SPECIFIC_NAME]
                              ,[ORDINAL_POSITION]
                              ,[PARAMETER_MODE]
                              ,[IS_RESULT]
                              ,[AS_LOCATOR]
                              ,[PARAMETER_NAME]
                              ,[DATA_TYPE]
                              ,[CHARACTER_MAXIMUM_LENGTH]
                              ,[CHARACTER_OCTET_LENGTH]
                              ,[COLLATION_CATALOG]
                              ,[COLLATION_SCHEMA]
                              ,[COLLATION_NAME]
                              ,[CHARACTER_SET_CATALOG]
                              ,[CHARACTER_SET_SCHEMA]
                              ,[CHARACTER_SET_NAME]
                              ,[NUMERIC_PRECISION]
                              ,[NUMERIC_PRECISION_RADIX]
                              ,[NUMERIC_SCALE]
                              ,[DATETIME_PRECISION]
                              ,[INTERVAL_TYPE]
                              ,[INTERVAL_PRECISION]
                              ,[USER_DEFINED_TYPE_CATALOG]
                              ,[USER_DEFINED_TYPE_SCHEMA]
                              ,[USER_DEFINED_TYPE_NAME]
                              ,[SCOPE_CATALOG]
                              ,[SCOPE_SCHEMA]
                              ,[SCOPE_NAME]
                          FROM [INFORMATION_SCHEMA].[PARAMETERS]
                          WHERE UPPER(SPECIFIC_NAME) = UPPER('" + sp + @"')
                          ORDER BY ORDINAL_POSITION";
            #endregion
            using (var command = connection.CreateCommand())
            {
                command.Connection = connection;
                command.CommandText = sql;

                connection.Open();

                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    SqlParameter p = SetParameter(reader, o);
                    parameters.Add(p);
                }

                connection.Close();
            }

            return parameters;

        }

        private static List<List<SqlParameter>> GetParameters<T>(List<T> list, string sp, IDbConnection connection)
        {
            List<List<SqlParameter>> parameters = new List<List<SqlParameter>>();
            bool isFirstTime = true;

            #region Query

            string sql = @"
                        SELECT [SPECIFIC_CATALOG]
                              ,[SPECIFIC_SCHEMA]
                              ,[SPECIFIC_NAME]
                              ,[ORDINAL_POSITION]
                              ,[PARAMETER_MODE]
                              ,[IS_RESULT]
                              ,[AS_LOCATOR]
                              ,[PARAMETER_NAME]
                              ,[DATA_TYPE]
                              ,[CHARACTER_MAXIMUM_LENGTH]
                              ,[CHARACTER_OCTET_LENGTH]
                              ,[COLLATION_CATALOG]
                              ,[COLLATION_SCHEMA]
                              ,[COLLATION_NAME]
                              ,[CHARACTER_SET_CATALOG]
                              ,[CHARACTER_SET_SCHEMA]
                              ,[CHARACTER_SET_NAME]
                              ,[NUMERIC_PRECISION]
                              ,[NUMERIC_PRECISION_RADIX]
                              ,[NUMERIC_SCALE]
                              ,[DATETIME_PRECISION]
                              ,[INTERVAL_TYPE]
                              ,[INTERVAL_PRECISION]
                              ,[USER_DEFINED_TYPE_CATALOG]
                              ,[USER_DEFINED_TYPE_SCHEMA]
                              ,[USER_DEFINED_TYPE_NAME]
                              ,[SCOPE_CATALOG]
                              ,[SCOPE_SCHEMA]
                              ,[SCOPE_NAME]
                          FROM [INFORMATION_SCHEMA].[PARAMETERS]
                          WHERE UPPER(SPECIFIC_NAME) = UPPER('" + sp + @"')
                          ORDER BY ORDINAL_POSITION";
            #endregion

            using (var command = connection.CreateCommand())
            {
                command.Connection = connection;
                command.CommandText = sql;

                connection.Open();
                List<SqlParameter> pms;
                SqlParameter p;
                foreach (T o in list)
                {
                    pms = new List<SqlParameter>();

                    if (isFirstTime)
                    {
                        var reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            p = SetParameter(reader, o);
                            pms.Add(p);
                            GC.SuppressFinalize(p);
                        }

                        connection.Close();

                        isFirstTime = false;
                        parameters.Add(pms);
                        GC.SuppressFinalize(pms);
                    }
                    else
                    {
                        foreach (SqlParameter sqlp in parameters.First())
                        {
                            p = Util.Mapper.Map<SqlParameter, SqlParameter>(sqlp);
                            p.Value = GetPropertyValue(sqlp.ParameterName, o);
                            pms.Add(p);
                            GC.SuppressFinalize(p);
                        }

                        parameters.Add(pms);
                        GC.SuppressFinalize(pms);
                    }
                }

                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return parameters;

        }

        private static SqlParameter SetParameter(IDataReader reader, object o)
        {
            SqlParameter p = new SqlParameter();

            String name = reader.GetString(reader.GetOrdinal("PARAMETER_NAME"));
            String mode = reader.GetString(reader.GetOrdinal("PARAMETER_MODE"));
            String data_type = reader.GetString(reader.GetOrdinal("DATA_TYPE"));
            int? size = null;

            if (reader["CHARACTER_MAXIMUM_LENGTH"] != DBNull.Value)
            {
                size = reader.GetInt32(reader.GetOrdinal("CHARACTER_MAXIMUM_LENGTH"));
            }

            if (data_type.ToUpper() == SqlDbType.VarChar.ToString().ToUpper())
            {
                p.SqlDbType = SqlDbType.VarChar;
            }
            else if (data_type.ToUpper() == SqlDbType.NVarChar.ToString().ToUpper())
            {
                p.SqlDbType = SqlDbType.NVarChar;
            }
            else if (data_type.ToUpper() == SqlDbType.Int.ToString().ToUpper())
            {
                p.SqlDbType = SqlDbType.Int;
            }

            if (mode.ToUpper() == "IN")
            {
                p.Direction = ParameterDirection.Input;
                if (size.HasValue) p.Size = size.Value;
                p.Value = GetPropertyValue(name, o);
            }
            else
            {
                p.Direction = ParameterDirection.Output;
            }

            p.ParameterName = name;

            return p;
        }

        #endregion
    }
}
