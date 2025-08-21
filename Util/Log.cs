using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util
{
    public static class Log
    {

        public static void Info(string _messege)
        {
            Console.WriteLine(_messege);
            RegistrarExcepcion(_messege, false);
        }

        public static void Error(string _messege)
        {
            Console.WriteLine(_messege);
            RegistrarExcepcion(_messege);
        }

        public static void Error(Exception ex)
        {

            Console.WriteLine(ex.Message + " StackTrace: " + ex.StackTrace);
            RegistrarExcepcion(ex.Message + " StackTrace: " + ex.StackTrace);
        }

        private static void CrearDirectorio()
        {
            if (!Directory.Exists(Setting.Logpath))
            {
                Directory.CreateDirectory(Setting.Logpath);
            }
        }
        private static void CrearArchivo(string _logfile)
        {

            if (!File.Exists(_logfile))
            {
                FileStream archivo;
                archivo = File.Create(_logfile);
                archivo.Close();
            }
        }
        private static void GestionarDirectorio()
        {
         
            CrearDirectorio();
            CrearArchivo(Setting.fullpath);

            FileInfo fInfo = new FileInfo(Setting.fullpath);
            if (fInfo.Length > 10485760)
            {
                fInfo.MoveTo(Setting.fullpath + "_" + DateTime.Now.Day.ToString("D2") +
                                                DateTime.Now.Month.ToString("D2") +
                                                DateTime.Now.Year.ToString("D4") +
                                                DateTime.Now.Hour.ToString("D2") +
                                                DateTime.Now.Minute.ToString("D2") +
                                                DateTime.Now.Second.ToString("D2"));
                CrearArchivo(Setting.fullpath);
            }

        }
        private static void RegistrarExcepcion(string sMensaje, bool isError = true)
        {
            try
            {
                GestionarDirectorio();

                string str;

                using (StreamReader sreader = new StreamReader(Setting.fullpath))
                {
                    str = sreader.ReadToEnd();
                }

                using (StreamWriter writer = new StreamWriter(Setting.fullpath, false))
                {
                    writer.WriteLine("Fecha     = " + DateTime.Now);
                    if (isError)
                    {
                        writer.WriteLine("ERROR     = " + sMensaje);
                    }
                    else
                    {
                        writer.WriteLine("INFO     = " + sMensaje);
                    }
                    writer.WriteLine("===========================================================================================================================");
                    writer.WriteLine(str);
                }

            }
            catch
            {
            }

        }


    }
}
