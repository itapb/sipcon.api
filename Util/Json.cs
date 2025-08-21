using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Reflection;


namespace Util
{
    public static class Json
    {
        public static string RemoveAtribute(string _jsonString, string atribute)
        {
            string valor = "";
            try
            {

                JArray jsonArray = JArray.Parse(_jsonString);

                foreach (JObject item in jsonArray)
                {
                    item.Remove(atribute);
                }

                valor = jsonArray.ToString();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"RemoveAtribute : {ex.Message}");
                Log.Error(ex);
            }

            return valor;

        }

        public static string CustomFormat(string _jsonString)
        {


            string patronCadenas = "\"[^\"]*\"";
            string jsonTemporal = Regex.Replace(_jsonString, patronCadenas,
                match => match.Value.Replace("\n", "\\n").Replace("\r", "\\r"));

            // Patrón mejorado que preserva la estructura JSON
            string patronSaltos = @"[\r\n]\s*";
            string jsonLimpio = Regex.Replace(jsonTemporal, patronSaltos, "");

            // Validación adicional para asegurar que las cadenas estén correctamente cerradas
            jsonLimpio = jsonLimpio.Replace("\\n", "\n").Replace("\\r", "\r");

            // Aseguramos que el JSON esté bien formateado
            jsonLimpio = jsonLimpio.Trim();
            if (!jsonLimpio.StartsWith("{") && !jsonLimpio.StartsWith("["))
                jsonLimpio = "[" + jsonLimpio + "]";

            string jsonCorregido = Regex.Replace(jsonLimpio, @"(""[^""]*"")\s*:\s*(\d{2}/\d{2}/\d{4})", "$1: \"$2\"");
            jsonCorregido = jsonCorregido.Replace("\n", "").Replace("\r", "");

            // formatear valores numericos decimales sin parte entera, anteponiendo el 0.
            jsonCorregido = System.Text.RegularExpressions.Regex.Replace(jsonCorregido, @"(?<![\d.])\.(\d+)(?![\d.])", match => $"0.{match.Groups[1].Value}", System.Text.RegularExpressions.RegexOptions.Singleline);

            return jsonCorregido;


        }

        public static JsonSerializerSettings GetJsonSerializerSettings()
        {
            var settings = new JsonSerializerSettings
            {
                DateParseHandling = DateParseHandling.DateTimeOffset,
                Converters = new Newtonsoft.Json.JsonConverter[]
                     {
                        new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                     },
                Error = (sender, args) =>
                {
                    Console.WriteLine($"Error en la propiedad: {args.ErrorContext.Path}");
                    Console.WriteLine($"Mensaje de error: {args.ErrorContext.Error.Message}");
                    args.ErrorContext.Handled = true;
                }
            };
            return settings;
        }

        public static string ConvertToJsonString(Object _objet)
        {

            string _jsonstring = "[]";

            try
            {

                _jsonstring = JsonConvert.SerializeObject(_objet, Newtonsoft.Json.Formatting.Indented);

            }
            catch (Exception ex)
            {
                Util.Log.Error(ex);
            }

            return _jsonstring;

        }
    }
}
