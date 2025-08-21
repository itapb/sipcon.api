
using System.Data.Common;
using System.Reflection;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Reflection.PortableExecutable;

namespace Util
{
    public sealed class Data
    {
        // test util
        private string _stringConnection = string.Empty;
        private static Data? _miData = null ;

        private Data()
        {
            
        }

        public static Data GetInstance()
        {
            if (_miData == null)
            {
                _miData = new Data();
                _miData._stringConnection = Util.Setting.ConnectionString;
            }

            return _miData;
        }

        #region "ExecuteNonQueryAsync"
        private async Task<Boolean> ExecuteNonQueryAsync(string _storedProcedure, List<SqlParameter>? _sqlParameters = null)
        {

            bool result = false;
            try
            {
                using (var _connection = new SqlConnection(_stringConnection))
                {
                    await _connection.OpenAsync();
                    using (SqlCommand _command = new SqlCommand(_storedProcedure, _connection))
                    {
                        _command.CommandTimeout = Setting.TimeOut.Seconds;
                        _command.CommandType = CommandType.StoredProcedure;

                        if (_sqlParameters != null && _sqlParameters.Count() > 0)
                            foreach (SqlParameter parametro in _sqlParameters)
                            {
                                _command.Parameters.Add(parametro);
                            }

                        int _resul = await _command.ExecuteNonQueryAsync();
                        result = true;
                    }
                }
            }
            catch (SqlException e)
            {
                result = false;
                Util.Log.Error(e);
                throw;
            }
            catch (Exception e)
            {
                result = false;
                Util.Log.Error(e);
                throw;
            }

            return result;

        }
        public async Task<Boolean> ExecuteNonQueryAsync(string _storedProcedure, Parameter _parameter)
        {

            return await ExecuteNonQueryAsync(_storedProcedure, _parameter.SqlParameters);

        }

        #endregion




        #region "Privados"

        private Boolean ExecuteNonQuery(string _storedProcedure, List<SqlParameter>? _sqlParameters = null)
        {
            bool result = false;
            try
            {
                using (var _connection = new SqlConnection(_stringConnection))
                {
                    _connection.Open();
                    using (SqlCommand _command = new SqlCommand(_storedProcedure, _connection))
                    {
                        _command.CommandTimeout = Setting.TimeOut.Seconds;
                        _command.CommandType = CommandType.StoredProcedure;

                        if (_sqlParameters != null && _sqlParameters.Count() > 0)
                            foreach (SqlParameter parametro in _sqlParameters)
                            {
                                _command.Parameters.Add(parametro);
                            }

                        int _resul = _command.ExecuteNonQuery();
                        result = true;
                    }
                }
            }
            catch (SqlException e)
            {
                result = false;
                Util.Log.Error(e);
                throw;
            }
            catch (Exception e)
            {
                result = false;
                Util.Log.Error(e);
                throw;
            }

            return result;
        }

        private Boolean ExecuteNonQuery(string _storedProcedure, Parameter _parameter)
        {
            return ExecuteNonQuery(_storedProcedure, _parameter.SqlParameters);
        }

        private List<T> ExecuteReader<T>(string _storedProcedure, Mapping _mapping, Parameter _parameter)
        {
            return ExecuteReader<T>(_storedProcedure, _mapping.Items, _parameter.SqlParameters);
        }

        private List<T> ExecuteReader<T>(string _storedProcedure, Dictionary<string, string> _mapping, List<SqlParameter>? _sqlParameters = null)
        {
            Type type = typeof(List<T>);
            List<T> items = (List<T>)Activator.CreateInstance(type);

            try
            {
                using (var _connection = new SqlConnection(_stringConnection))
                {
                    _connection.Open();
                    using (SqlCommand _command = new SqlCommand(_storedProcedure, _connection))
                    {
                        _command.CommandTimeout = Setting.TimeOut.Seconds;
                        _command.CommandType = CommandType.StoredProcedure;

                        if (_sqlParameters != null && _sqlParameters.Count() > 0)
                            foreach (SqlParameter parametro in _sqlParameters)
                            {
                                _command.Parameters.Add(parametro);
                            }

                        using (SqlDataReader _reader = _command.ExecuteReader())
                        {
                            while (_reader.Read())
                            {
                                items.Add(FillData<T>(_mapping, _reader));
                            }
                        }

                    }
                }
            }
            catch (SqlException e)
            {
                Util.Log.Error(e);
                throw;
            }
            catch (Exception e)
            {
                Util.Log.Error(e);
                throw;
            }

            return items;
        }

        private List<T> ExecuteReader<T>(string _storedProcedure, Mapping _mapping, List<SqlParameter>? _sqlParameters = null)
        {
            return ExecuteReader<T>(_storedProcedure, _mapping.Items, _sqlParameters);
        }


        private T FillData<T>(Dictionary<string, string> schema, SqlDataReader _reader)
        {
            try
            {
                Type type = typeof(T);
                T _objeto = (T)Activator.CreateInstance(type);

                if (_objeto != null)
                {
                    foreach (var column in schema)
                    {
                        string propertyName = column.Key;
                        object value = _reader[column.Value];

                        if (value.GetType().ToString() != "System.DBNull")
                        {
                            SetPropertyValue(_objeto, propertyName, value);
                        }
                    }
                }

                return _objeto;
            }
            catch (Exception ex)
            {
                Util.Log.Error(ex);
                throw;
            }

        }

        private T FillData<T>(Dictionary<string, string> schema, DataRow row)
        {
            try
            {
                Type type = typeof(T);
                T _objeto = (T)Activator.CreateInstance(type);

                if (_objeto != null)
                {
                    foreach (var column in schema)
                    {
                        string propertyName = column.Key;
                        object value = row[column.Value];

                        if (value.GetType().ToString() != "System.DBNull")
                        {
                            SetPropertyValue(_objeto, propertyName, value);
                        }
                    }
                }

                return _objeto;
            }
            catch (Exception ex)
            {
                Util.Log.Error(ex);
                throw;
            }

        }

        private void SetPropertyValue(object obj, string propertyName, object value)
        {
            // Si la propiedad contiene punto, significa que es una propiedad anidada
            if (propertyName.Contains("."))
            {
                // Dividir la cadena en partes
                int index = propertyName.IndexOf(".");
                string parentProperty = propertyName.Substring(0, index);
                string remainingProperty = propertyName.Substring(index + 1);

                // Obtener la propiedad padre
                PropertyInfo parentProp = obj.GetType().GetProperty(parentProperty);

                // Si existe y es un objeto
                if (parentProp != null && parentProp.PropertyType.IsClass)
                {
                    object parentValue = parentProp.GetValue(obj);

                    // Crear instancia si no existe
                    if (parentValue == null)
                    {
                        parentValue = Activator.CreateInstance(parentProp.PropertyType);
                        parentProp.SetValue(obj, parentValue);
                    }

                    // Recursivamente establecer el valor en la propiedad hija
                    SetPropertyValue(parentValue, remainingProperty, value);
                }
            }
            else
            {
                // Caso normal: propiedad directa
                PropertyInfo property = obj.GetType().GetProperty(propertyName);
                if (property != null)
                {
                    try
                    {

                        //property.SetValue(obj, Convert.ChangeType(value, property.PropertyType));
                        if (property.GetGetMethod().ReturnType.FullName.Contains("System.Nullable`1[[System.Boolean"))
                        {
                            property.SetValue(obj, value);
                        }
                        else if (property.GetGetMethod().ReturnType.FullName.Contains("System.Nullable`1[[System.Int32"))
                        {
                            property.SetValue(obj, value);
                        }
                        else if (property.GetGetMethod().ReturnType.FullName.Contains("System.Nullable`1[[System.DateTime"))
                        {
                            property.SetValue(obj, value);
                        }
                        else if (property.GetGetMethod().ReturnType.FullName.Contains("System.Nullable`1[[System.Decimal"))
                        {
                            property.SetValue(obj, value);
                        }
                        else if (property.GetGetMethod().ReturnType.FullName.Contains("System.String"))
                        {
                            property.SetValue(obj, Convert.ChangeType(value.ToString().ToUpper(), property.PropertyType));
                        }
                        else
                        {
                            property.SetValue(obj, Convert.ChangeType(value, property.PropertyType));
                        }

                    }
                    catch (Exception ex)
                    {
                        Util.Log.Error(ex);
                        throw;
                    }

                }
            }
        }


        private async Task<List<T>> ExecuteReaderAsync<T>(string _storedProcedure, Dictionary<string, string> _mapping, List<SqlParameter>? _sqlParameters = null)
        {

            Type type = typeof(List<T>);
            List<T> items = (List<T>)Activator.CreateInstance(type);

            try
            {
                using (var _connection = new SqlConnection(_stringConnection))
                {
                    await _connection.OpenAsync();
                    using (SqlCommand _command = new SqlCommand(_storedProcedure, _connection))
                    {
                        _command.CommandTimeout = Setting.TimeOut.Seconds;
                        _command.CommandType = CommandType.StoredProcedure;

                        if (_sqlParameters != null && _sqlParameters.Count() > 0)
                            foreach (SqlParameter parametro in _sqlParameters)
                            {
                                _command.Parameters.Add(parametro);
                            }

                        using (SqlDataReader _reader = await _command.ExecuteReaderAsync())
                        {
                            while (await _reader.ReadAsync())
                            {
                                items.Add(FillData<T>(_mapping, _reader));
                            }
                        }

                    }
                }
            }
            catch (SqlException e)
            {
                Util.Log.Error(e);
                throw;
            }
            catch (Exception e)
            {
                Util.Log.Error(e);
                throw;
            }

            return items;
        }

        private async Task<List<T>> ExecuteReaderAsync<T>(string _storedProcedure, Mapping _mapping, List<SqlParameter>? _sqlParameters = null)
        {
            return await ExecuteReaderAsync<T>(_storedProcedure, _mapping.Items, _sqlParameters);
        }

        private async Task<T> _ExecuteReaderAsyncTop<T>(string _storedProcedure, Dictionary<string, string> _mapping, List<SqlParameter>? _sqlParameters = null)
        {

            Type type = typeof(T);
            T _item = (T)Activator.CreateInstance(type);

            try
            {
                using (var _connection = new SqlConnection(_stringConnection))
                {
                    await _connection.OpenAsync();
                    using (SqlCommand _command = new SqlCommand(_storedProcedure, _connection))
                    {
                        _command.CommandTimeout = Setting.TimeOut.Seconds;
                        _command.CommandType = CommandType.StoredProcedure;

                        if (_sqlParameters != null && _sqlParameters.Count() > 0)
                            foreach (SqlParameter parametro in _sqlParameters)
                            {
                                _command.Parameters.Add(parametro);
                            }

                        using (SqlDataReader _reader = await _command.ExecuteReaderAsync())
                        {
                            if (await _reader.ReadAsync())
                            {
                                _item = FillData<T>(_mapping, _reader);
                            }
                        }

                    }
                }
            }
            catch (SqlException e)
            {
                Util.Log.Error(e);
                throw;
            }
            catch (Exception e)
            {
                Util.Log.Error(e);
                throw;
            }

            return _item;
        }



        #endregion

        #region "Publicos"

        public List<T> GetList<T>(Mapping _mapping, DataTable _dataTable)
        {

            Type type = typeof(List<T>);
            List<T> _list = (List<T>)Activator.CreateInstance(type);

            try
            {

                foreach (DataRow row in _dataTable.Rows)
                {
                    _list.Add(GetItem<T>(_mapping, row));
                }

            }
            catch (SqlException e)
            {
                Util.Log.Error(e);
                throw;
            }
            catch (Exception e)
            {
                Util.Log.Error(e);
                throw;
            }

            return _list;
        }

        public T GetItem<T>(Mapping _mapping, DataRow row)
        {
            Type type = typeof(T);
            T _item = (T)Activator.CreateInstance(type);

            try
            {

                _item = FillData<T>(_mapping.Items, row);

            }
            catch (SqlException e)
            {
                Util.Log.Error(e);
                throw;
            }
            catch (Exception e)
            {
                Util.Log.Error(e);
                throw;
            }

            return _item;
        }

        public T GetItem<T>(Mapping _mapping, DataTable _dataTable)
        {
            Type type = typeof(T);
            T _item = (T)Activator.CreateInstance(type);

            try
            {
                if (_dataTable.Rows.Count > 0) {
                    _item = FillData<T>(_mapping.Items, _dataTable.Rows[0]);
                }         

            }
            catch (SqlException e)
            {
                Util.Log.Error(e);
                throw;
            }
            catch (Exception e)
            {
                Util.Log.Error(e);
                throw;
            }

            return _item;
        }

        public object GetValue(string _culumnName, DataRow row)
        {
            try
            {
                object _value = new object();
                _value = row[_culumnName];
                return _value;
            }
            catch (Exception ex)
            {
                Util.Log.Error(ex);
                throw;
            }

        }

        public int GetTotal(DataTable _dataTable)
        {
            object _value = new object();
            int _total = 0;
            try
            {
                _value = _dataTable.Rows[0]["TOTAL"];
                return (int)_value;
            }
            catch (Exception ex)
            {
                Util.Log.Error(ex);
                return _total;
            }

        }

        public async Task<DataTable> GetDataTable(string _storedProcedure, Parameter? _parameter = null)
        {

            DataTable _dataTable = new DataTable();

            try
            {
                using (var _connection = new SqlConnection(_stringConnection))
                {
                    await _connection.OpenAsync();
                    using (SqlCommand _command = new SqlCommand(_storedProcedure, _connection))
                    {
                        _command.CommandTimeout = Setting.TimeOut.Seconds;
                        _command.CommandType = CommandType.StoredProcedure;

                        if (_parameter != null) {
                            if (_parameter.SqlParameters != null && _parameter.SqlParameters.Count() > 0)
                                foreach (SqlParameter parametro in _parameter.SqlParameters)
                                {
                                    _command.Parameters.Add(parametro);
                                }
                        }

                        using (SqlDataReader reader = await _command.ExecuteReaderAsync())
                        {
                            _dataTable.Load(reader); // Carga los datos del reader en el DataTable
                        }


                    }
                }
            }
            catch (SqlException e)
            {
                
                Util.Log.Error(e);
                throw;
            }
            catch (Exception e)
            {
                Util.Log.Error(e);
                throw;
            }

            return _dataTable;
        }

        public async Task<List<T>> ExecuteReaderAsync<T>(string _storedProcedure, Mapping _mapping,Parameter _parameter)
        {
            return await ExecuteReaderAsync<T>(_storedProcedure, _mapping.Items, _parameter.SqlParameters);
        }

        public async Task<List<T>> ExecuteReaderAsync<T>(string _storedProcedure, Mapping _mapping)
        {
            return await ExecuteReaderAsync<T>(_storedProcedure, _mapping.Items);
        }

        public async Task<T> ExecuteReaderAsyncTop<T>(string _storedProcedure, Mapping _mapping, Parameter _parameter)
        {
            return await _ExecuteReaderAsyncTop<T>(_storedProcedure, _mapping.Items, _parameter.SqlParameters);
        }

        public async Task<T> ExecuteReaderAsyncTop<T>(string _storedProcedure, Mapping _mapping)
        {
            return await _ExecuteReaderAsyncTop<T>(_storedProcedure, _mapping.Items);
        }


        #endregion





    }
}
