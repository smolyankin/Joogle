using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Joogle.Utils
{
    /// <summary>
    /// Конвертор DateTme-UnixTimestamp для JSON.NET
    /// </summary>
    public class UnixTimestampConvertor : DateTimeConverterBase
    {
        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader"/> to read from.</param><param name="objectType">Type of the object.</param><param name="existingValue">The existing value of object being read.</param><param name="serializer">The calling serializer.</param>
        /// <returns>
        /// The object value.
        /// </returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
                return null;

            long time;
            if (long.TryParse(reader.Value.ToString(), out time))
                return DateTimeExtensions.UnixTimestampMillisecondsToDateTime(time);
            return DateTime.Parse(reader.Value.ToString());//попробуем десериализовать по умолчанию
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter"/> to write to.</param><param name="value">The value.</param><param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((DateTime)value).ToUnixTimestampMilliseconds());
        }
    }
}