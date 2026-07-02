using System.Data;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using Util;

namespace Data
{
    public class OracleDB
    {
        private readonly string _connA;
        private readonly string _connB;

        public OracleDB(IConfiguration config)
        {
            Util.Setting.GetSettings(true);
            _connA = Util.Setting.OracleDbConnection_A;
            _connB = Util.Setting.OracleDbConnection_B;
        }

        private string GetConnectionString(string tipo) => (tipo == "B") ? _connB : _connA;

        public OracleConnection GetConnection(string tipo)
        {
            return new OracleConnection(GetConnectionString(tipo));
        }

        public async Task<DataTable> GetDataTable(string Query, int supplierId, string tipoConn = "A", List<OracleParameter>? Params = null, int? empId = null, int? userId = null)
        {
            var dataTable = new DataTable();

            using (var connection = GetConnection(tipoConn))
            {
                await connection.OpenAsync();

                if (supplierId == 4069)
                {
                    // Sin variables
                    using (var cmd = new OracleCommand("BEGIN SQLFIGO.P_SETEAR_CONTEXTO; END;", connection))
                    {
                        cmd.CommandType = CommandType.Text;
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                else if (supplierId == 4076)
                {
                    using (var cmd = new OracleCommand("BEGIN SQLFIGO.P_SETEAR_CONTEXTO(:p_id); END;", connection))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add("p_id", OracleDbType.Int32).Value = 6569364;
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                else
                {
                    using (var cmd = new OracleCommand("SQLFIGO.P_SETEAR_CONTEXTO", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.BindByName = true;
                        if (empId.HasValue) cmd.Parameters.Add("p_emp", OracleDbType.Int32).Value = empId.Value;
                        if (userId.HasValue) cmd.Parameters.Add("p_user", OracleDbType.Int32).Value = userId.Value;
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                // --- EJECUCIÓN DEL QUERY ---
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