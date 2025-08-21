using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Util
{
    public class Parameter
    {
        
        public List<SqlParameter> SqlParameters = new List<SqlParameter>();

        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Value { get; set; }

        public Parameter()
        {

        }

        public void AddSqlParameter(string name, object? value)
        {
            SqlParameters.Add(new SqlParameter(name, value));
        }

        public void Initialize()
        {
            SqlParameters.Clear();
        }

    }
}
