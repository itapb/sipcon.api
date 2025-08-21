
using System.Configuration;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.Reflection.Metadata;

namespace Util
{
    public static class Setting
    {

        private static IConfiguration? _Configuration { get; set; }
        public static string ConnectionString = "";
        public static bool Igtf = false;
        public static string Port = "";
        public static string PrinterName = "";
        public static bool Logger = false;
        public static string Logpath = "";
        public static string Logfile = "";
        public static string Url = "";
        public static string fullpath = "";
        public static TimeSpan TimeOut;
        public static TimeSpan TimeFrecuency;
        public static List<Parameter> Parameters = new List<Parameter>();
        public static string Environment = "";
        public static string ImagesUrl = "";
        public static string AttachmentUrl = "";

        private  static void GetSettingsFromJson()
        {
          
           
        inicio:


            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");

            try
            {

                _Configuration = builder.Build();

            }
            catch (Exception)
            {

                Console.WriteLine("Error: ConfigurationBuilder");
                System.Threading.Thread.Sleep(5000);
                goto inicio;
            }

            try
            {
                var _valor = _Configuration.GetSection("Custom:Environment").Value ?? "DEV";
                Environment = _valor;

            }
            catch (Exception)
            {

                Console.WriteLine("Error: Custom:Environment");
                System.Threading.Thread.Sleep(5000);
                goto inicio;
            }

            try
            {
                var _valor = _Configuration.GetSection("Custom:TimeOut").Value ?? "30";
                int _value = int.Parse(_valor);
                TimeOut = new TimeSpan(0, 0, _value);

            }
            catch (Exception)
            {

                Console.WriteLine("Error: Custom:TimeOut");
                System.Threading.Thread.Sleep(5000);
                goto inicio;
            }


            try
            {

                var _valor = _Configuration.GetSection("Custom:TimeFrecuency").Value ?? "15";
                int _value = int.Parse(_valor);
                TimeFrecuency = new TimeSpan(0,_value,0);

            }
            catch (Exception)
            {

                Console.WriteLine("Error: Custom:TimeFrecuency");
                System.Threading.Thread.Sleep(5000);
                goto inicio;
            }


            try
            {
                string _valor = "";
                if (Environment == "DEV")
                {
                    _valor = _Configuration.GetSection("Custom:ConnectionStrings:DEV").Value ?? "";
                    ConnectionString = _valor;
                }
                else
                {
                    _valor = _Configuration.GetSection("Custom:ConnectionStrings:PROD").Value ?? "";
                    ConnectionString = _valor;
                }

   
                if (ConnectionString == "")
                {
                    Console.WriteLine("Error: ConnectionString no definido en app.config");
                    System.Threading.Thread.Sleep(5000);
                    goto inicio;
                }

            }
            catch (Exception)
            {

                Console.WriteLine("Error: ConnectionString no definido en app.config");
                System.Threading.Thread.Sleep(5000);
                goto inicio;
            }


            try
            {
                Logpath = _Configuration.GetSection("Custom:Logpath").Value ?? "C:/Log";
                if (Logpath == "")
                {
                    Console.WriteLine("Error: Logpath no definido en app.config");
                    System.Threading.Thread.Sleep(5000);
                    goto inicio;
                }
            }
            catch (Exception)
            {

                Console.WriteLine("Error: Logpath no definido en app.config");
                System.Threading.Thread.Sleep(5000);
                goto inicio;
            }


            try
            {
                Logfile = _Configuration.GetSection("Custom:Logfile").Value ?? "log.txt";
                if (Logfile == "")
                {
                    Console.WriteLine("Error: Logfile no definido en app.config");
                    System.Threading.Thread.Sleep(5000);
                    goto inicio;
                }

                fullpath = Setting.Logpath + "/" + Setting.Logfile;
            }
            catch (Exception)
            {

                Console.WriteLine("Error: Logfile no definido en app.config");
                System.Threading.Thread.Sleep(5000);
                goto inicio;
            }

            try
            {
                ImagesUrl = _Configuration.GetSection("Custom:ImagesUrl").Value ?? "";
                if (ImagesUrl == "")
                {
                    Console.WriteLine("Error: ImagesUrl no definido en app.config");
                    System.Threading.Thread.Sleep(5000);
                    goto inicio;
                }
            }
            catch (Exception)
            {

                Console.WriteLine("Error: ImagesUrl no definido en app.config");
                System.Threading.Thread.Sleep(5000);
                goto inicio;
            }

            try
            {
                AttachmentUrl = _Configuration.GetSection("Custom:AttachmentUrl").Value ?? "";
                if (AttachmentUrl == "")
                {
                    Console.WriteLine("Error: ServicesUrl no definido en app.config");
                    System.Threading.Thread.Sleep(5000);
                    goto inicio;
                }
            }
            catch (Exception)
            {

                Console.WriteLine("Error: AttachmentUrl no definido en app.config");
                System.Threading.Thread.Sleep(5000);
                goto inicio;
            }


        }

        private static void GetSettingsFromConfig()
        {
        inicio:

            try
            {
                
                Environment = System.Configuration.ConfigurationManager.AppSettings["Environment"]?.ToString() ?? "DEV";

            }
            catch (Exception)
            {
                Console.WriteLine("Error: Environment no definido en app.config");
                System.Threading.Thread.Sleep(5000);
                goto inicio;
            }

            try
            {
                string _value = System.Configuration.ConfigurationManager.AppSettings["Igtf"]?.ToString() ?? "0";
                if (_value == "1")
                    Igtf = true;
            }
            catch (Exception)
            {
                Console.WriteLine("Error: Igtf no definido en app.config");
                System.Threading.Thread.Sleep(5000);
                goto inicio;
            }

            try
            {
                var _value = System.Configuration.ConfigurationManager.AppSettings["Logger"]?.ToString() ?? "0";
                if (_value  == "1")
                    Logger = true;
            }
            catch (Exception)
            {
                Console.WriteLine("Error: Logger no definido en app.config");
                System.Threading.Thread.Sleep(5000);
                goto inicio;
            }

            try
            {

                if (Environment == "DEV")
                {
                    ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DEV"].ConnectionString;
                }
                else
                {
                    ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PROD"].ConnectionString;
                }

                if (ConnectionString == "")
                {
                    Console.WriteLine("Error: ConnectionString no definido en app.config");
                    System.Threading.Thread.Sleep(5000);
                    goto inicio;
                }
            }
            catch (Exception)
            {

                Console.WriteLine("Error: ConnectionString no definido en app.config");
                System.Threading.Thread.Sleep(5000);
                goto inicio;
            }

            try
            {
                Port = System.Configuration.ConfigurationManager.AppSettings["Puerto"]?.ToString() ?? "";

            }
            catch (Exception)
            {
                Console.WriteLine("Error: Port no definido en app.config");
                System.Threading.Thread.Sleep(5000);
                goto inicio;
            }

            try
            {
                PrinterName = System.Configuration.ConfigurationManager.AppSettings["PrinterName"]?.ToString() ?? "";

            }
            catch (Exception)
            {
                Console.WriteLine("Error: PrinterName no definido en app.config");
                System.Threading.Thread.Sleep(5000);
                goto inicio;
            }

            try
            {
                Logpath = System.Configuration.ConfigurationManager.AppSettings["Logpath"]?.ToString() ?? "C:/Log";
                if (Logpath == "")
                {
                    Console.WriteLine("Error: Logpath no definido en app.config");
                    System.Threading.Thread.Sleep(5000);
                    goto inicio;
                }
            }
            catch (Exception)
            {

                Console.WriteLine("Error: Logpath no definido en app.config");
                System.Threading.Thread.Sleep(5000);
                goto inicio;
            }

            try
            {
                Logfile = System.Configuration.ConfigurationManager.AppSettings["Logfile"]?.ToString() ?? "log.txt";
                if (Logfile == "")
                {
                    Console.WriteLine("Error: Logfile no definido en app.config");
                    System.Threading.Thread.Sleep(5000);
                    goto inicio;
                }

                fullpath = Setting.Logpath + "/" + Setting.Logfile;
            }
            catch (Exception)
            {

                Console.WriteLine("Error: Logfile no definido en app.config");
                System.Threading.Thread.Sleep(5000);
                goto inicio;
            }




            try
            {
                Url = System.Configuration.ConfigurationManager.AppSettings["Url"]?.ToString() ?? "";
                if (Url == "")
                {
                    Console.WriteLine("Error: Url no definido en app.config");
                    System.Threading.Thread.Sleep(5000);
                    goto inicio;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error: Url no definido en app.config");
                System.Threading.Thread.Sleep(5000);
                goto inicio;
            }

            try
            {
                int _value = int.Parse(System.Configuration.ConfigurationManager.AppSettings["TimeOut"]?.ToString() ?? "30");
                TimeOut = new TimeSpan(0, 0, _value);

            }
            catch (Exception)
            {

                Console.WriteLine("Error: Custom:TimeOut");
                System.Threading.Thread.Sleep(5000);
                goto inicio;
            }


            try
            {

                int _value = int.Parse(System.Configuration.ConfigurationManager.AppSettings["TimeFrecuency"]?.ToString() ?? "15");
                TimeFrecuency = new TimeSpan(0, _value, 0);

            }
            catch (Exception)
            {

                Console.WriteLine("Error: Custom:TimeFrecuency");
                System.Threading.Thread.Sleep(5000);
                goto inicio;
            }


        }


        public static void GetSettings(bool isFromAppsettingsjson = false)
        {

            if (isFromAppsettingsjson)
            {
                GetSettingsFromJson();
            }
            else
            {
                GetSettingsFromConfig();
            }

        }

        public static async void GetParameters()
        {

            try
            {

                Parameters.Clear();
           
                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Code", "CODE");
                _mapping.AddItem("Name", "DESCRIPTION");
                _mapping.AddItem("Value", "VALUE");

                Util.Data _data = Util.Data.GetInstance();
                Parameters =  await _data.ExecuteReaderAsync<Parameter>("USP_GETPARAMETERS",_mapping);
                
            }
            catch (Exception ex)
            {
                Util.Log.Error(ex);
            }


        }

        private static void GetParameters2()
        {

            try
            {
                Parameters.Clear();
                using (SqlConnection connection = new SqlConnection(Util.Setting.ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand comando = new SqlCommand("USP_GETPARAMETERS"))
                    {
                        comando.Connection = connection;
                        comando.CommandType = CommandType.StoredProcedure;
                        using (SqlDataReader reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Parameter _item = new Parameter();
                                _item.Id = reader.GetInt32(reader.GetOrdinal("ID"));
                                _item.Code = reader.GetString(reader.GetOrdinal("CODE"));
                                _item.Name = reader.GetString(reader.GetOrdinal("DESCRIPTION"));
                                _item.Value = reader.GetString(reader.GetOrdinal("VALUE"));

                                Parameters.Add(_item);

                            }
                            reader.Close();
                        }
                        comando.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Util.Log.Error(ex);
            }

            //BaseAddress = "http://10.23.212.12:8095/apb/";
            //User = "usuarioweb";
            //Password = "@pb_142536?*-";

        }

        public static  string GetParameterValue(string _code)
        {

            return Parameters.Find(x => x.Code == _code)?.Value ?? "";

        }




    }

}
