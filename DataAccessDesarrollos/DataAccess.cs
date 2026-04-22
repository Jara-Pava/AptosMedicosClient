using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace DataAccessDesarrollos
{
    public class DataAccess : IDisposable
    {
        private List<StoreProcedure> storedProcedureCollection;

        private SqlConnection Connection = null;

        private readonly object ObjLock = new object();

        private readonly string connectionStringName = "StringConnection";

        public DataAccess()
        {
            storedProcedureCollection = new List<StoreProcedure>();
            try
            {
                var connStr = System.Configuration.ConfigurationManager.ConnectionStrings[connectionStringName]?.ConnectionString;
                this.Connection = new SqlConnection(connStr ?? string.Empty);

                this.Connection.Open();

                var config = (DataAccessDesarrollos.QASConfig)System.Configuration.ConfigurationManager.GetSection("QASConfig");

                if (config?.StoreProcedures != null)
                {
                    for (var i = 0; i < config.StoreProcedures.Count; i++)
                    {
                        var spConfig = config.StoreProcedures[i];
                        Trace.TraceInformation("Add SP mapping: Code={0} Name={1}", spConfig.Code, spConfig.Name);
                        storedProcedureCollection.Add(new StoreProcedure { Code = spConfig.Code, Name = spConfig.Name });
                    }
                }
                else
                {
                    Trace.TraceWarning("QASConfig.StoreProcedures is null or empty. Check web.config section and type name.");
                }

                Trace.TraceInformation("Stored procedures loaded: {0}", storedProcedureCollection?.Count ?? 0);
                if (storedProcedureCollection != null)
                {
                    foreach (var sp in storedProcedureCollection)
                        Trace.TraceInformation("Mapped SP -> Code: {0}, Name: {1}", sp.Code, sp.Name);
                }
            }
            catch (Exception exc)
            {
                Trace.TraceError("Error occurred while initializing DataAccess class. Message == > {0}", exc.ToString());
                if (this.Connection != null)
                {
                    try { this.Connection.Dispose(); } catch { }
                    this.Connection = null;
                }
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.storedProcedureCollection != null)
                {
                    this.storedProcedureCollection.Clear();
                    this.storedProcedureCollection = null;
                }
            }
            if (Connection != null)
            {
                try { this.Connection.Dispose(); } catch { }
                this.Connection = null;
            }
        }

        public string GetStoredProcedureName(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || storedProcedureCollection == null) return null;
            var sp = storedProcedureCollection.Find(s => string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase));
            return sp?.Name;
        }
        public List<T> ExecuteReaderByCode<T>(string spCode, Func<SqlDataReader, T> mapper, Action<SqlCommand> parameterize = null)
        {
            var list = new List<T>();
            if (mapper == null) return list;

            string spName = GetStoredProcedureName(spCode);
            if (string.IsNullOrWhiteSpace(spName))
            {
                Trace.TraceError("Stored procedure for code '{0}' not found.", spCode);
                return list;
            }

            if (Connection == null)
            {
                Trace.TraceError("Database connection is not available.");
                return list;
            }

            lock (ObjLock)
            {
                using (var cmd = Connection.CreateCommand())
                {
                    cmd.CommandText = spName;
                    cmd.CommandType = CommandType.StoredProcedure;
                    parameterize?.Invoke(cmd);

                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            try
                            {
                                var item = mapper(rdr);
                                list.Add(item);
                            }
                            catch (Exception ex)
                            {
                                Trace.TraceError("Mapper exception for SP '{0}': {1}", spName, ex);
                            }
                        }
                    }
                }
            }
            return list;
        }

        public int ExecuteNonQueryByCode(string spCode, Action<SqlCommand> parameterize = null)
        {
            string spName = GetStoredProcedureName(spCode);
            if (string.IsNullOrWhiteSpace(spName))
            {
                Trace.TraceError("Stored procedure for code '{0}' not found.", spCode);
                return -1;
            }

            if (Connection == null)
            {
                Trace.TraceError("Database connection is not available.");
                return -1;
            }

            lock (ObjLock)
            {
                using (var cmd = Connection.CreateCommand())
                {
                    cmd.CommandText = spName;
                    cmd.CommandType = CommandType.StoredProcedure;
                    parameterize?.Invoke(cmd);
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public object ExecuteScalarByCode(string spCode, Action<SqlCommand> parameterize = null)
        {
            string spName = GetStoredProcedureName(spCode);
            if (string.IsNullOrWhiteSpace(spName))
            {
                Trace.TraceError("Stored procedure for code '{0}' not found.", spCode);
                return null;
            }

            if (Connection == null)
            {
                Trace.TraceError("Database connection is not available.");
                return null;
            }

            lock (ObjLock)
            {
                using (var cmd = Connection.CreateCommand())
                {
                    cmd.CommandText = spName;
                    cmd.CommandType = CommandType.StoredProcedure;
                    parameterize?.Invoke(cmd);
                    return cmd.ExecuteScalar();
                }
            }
        }
    }
}