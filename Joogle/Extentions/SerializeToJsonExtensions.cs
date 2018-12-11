using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Joogle.Utils
{
    /// <summary>
    /// Сериализация объекта в JSON
    /// </summary>
    public static class SerializeToJsonExtensions
    {
        /// <summary>
        /// Сериализовать в JSON
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string SerializeToJson(this object obj)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            };
            settings.Converters.Add(new UnixTimestampConvertor());
            return JsonConvert.SerializeObject(obj, settings);
        }

        /// <summary>
        /// Экранировать Json строку
        /// </summary>
        /// <param name="str">Json строка</param>
        /// <returns></returns>
        public static string EscapeJsonString(this string str)
        {
            const char backSlash = '\\';
            const char slash = '/';
            const char dblQuote = '"';

            var output = new StringBuilder(str.Length);
            foreach (char c in str)
            {
                switch (c)
                {
                    case slash:
                        output.AppendFormat("{0}{1}", backSlash, slash);
                        break;

                    case backSlash:
                        output.AppendFormat("{0}{0}", backSlash);
                        break;

                    case dblQuote:
                        output.AppendFormat("{0}{1}", backSlash, dblQuote);
                        break;

                    default:
                        output.Append(c);
                        break;
                }
            }

            return output.ToString();
        }
    }
}
