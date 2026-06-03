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
            _connectionString = Util.Setting.OracleDbConnection;
        }

        public OracleConnection GetConnection()
        {
            return new OracleConnection(_connectionString);
        }

        public async Task<DataTable> GetDataTable(string Query, List<OracleParameter>? Params = null)
        {
            var dataTable = new DataTable();

            using (var connection = GetConnection())
            {
                await connection.OpenAsync();

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