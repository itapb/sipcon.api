using System.Data;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using Util;

namespace Data
{
    public class OracleDB
    {
        private readonly string _connectionString;

        public OracleDB(IConfiguration config)
        {
            Util.Setting.GetSettings(true);
            _connectionString = Util.Setting.OracleDbConnection;
            Util.Log.Info($"OracleDB - Connection String: {_connectionString?.Substring(0, 50)}...");
        }

        public OracleConnection GetConnection()
        {
            return new OracleConnection(_connectionString);
        }

        public async Task<DataTable> GetDataTable(string Query, List<OracleParameter>? Params = null, int? empId = null, int? userId = null)
        {
            var dataTable = new DataTable();

            using (var connection = GetConnection())
            {
                await connection.OpenAsync();

                //  Ejecutar el Stored Procedure de Contexto en la misma sesión física de Oracle
                using (var contextCmd = new OracleCommand("SQLFIGO.P_SETEAR_CONTEXTO", connection))
                {
                    contextCmd.CommandType = CommandType.StoredProcedure;
                    contextCmd.CommandTimeout = Setting.TimeOut.Seconds;
                    contextCmd.BindByName = true;

                    // Si no se envían los valores desde C#, no se añaden los parámetros.
                    // Esto obliga a Oracle a tomar los valores "DEFAULT" definidos en el procedimiento almacenado.
                    if (empId.HasValue)
                    {
                        contextCmd.Parameters.Add(new OracleParameter("p_emp", OracleDbType.Int32) { Value = empId.Value });
                    }
                    if (userId.HasValue)
                    {
                        contextCmd.Parameters.Add(new OracleParameter("p_user", OracleDbType.Int32) { Value = userId.Value });
                    }

                    await contextCmd.ExecuteNonQueryAsync();
                }

                // Ejecutar el Query de selección manteniendo la conexión abierta para no perder el contexto
                using (var command = new OracleCommand(Query, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = Setting.TimeOut.Seconds;
                    command.BindByName = true;

                    if (Params != null && Params.Count > 0)
                    {
                        command.Parameters.AddRange(Params.ToArray());
                    }

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        dataTable.Load(reader);
                    }
                }

                await connection.CloseAsync();
            }

            return dataTable;
        }
    }
}