using System;
using NodaTime;

namespace Joogle.Utils
{
    /// <summary>
    /// Расширения для DateTime
    /// </summary>
    public static class DateTimeExtensions
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Преобразование даты в UNIX формат (миллисекунды)
        /// </summary>
        /// <param name="dateTime">Дата</param>
        /// <returns>UNIX формат времени</returns>
        public static long ToUnixTimestampMilliseconds(this DateTime dateTime)
        {
            return (long)(dateTime - UnixEpoch).TotalMilliseconds;
        }

        /// <summary>
        /// Преобразование даты в UNIX формат (секунды)
        /// </summary>
        /// <param name="dateTime">Дата</param>
        /// <returns>UNIX формат времени</returns>
        public static long ToUnixTimestampSeconds(this DateTime dateTime)
        {
            return (long)(dateTime - UnixEpoch).TotalSeconds;
        }

        /// <summary>
        /// Преобразование даты в DateTime из UNIX формат (миллисекунды)
        /// </summary>
        /// <param name="unixTimeStamp">Дата в формате UNIX (миллисекунды)</param>
        /// <returns>DateTime формат времени</returns>
        public static DateTime UnixTimestampMillisecondsToDateTime(long unixTimeStamp)
        {
            return UnixEpoch.AddMilliseconds(unixTimeStamp);
        }

        /// <summary>
        /// Получить дату эры UNIX
        /// </summary>
        /// <returns>Дата эры UNIX</returns>
        public static DateTime DefaultUnixDateTime()
        {
            return UnixEpoch;
        }

        /// <summary>
        /// Сравнить две даты
        /// </summary>
        /// <param name="date0">Дата 1</param>
        /// <param name="date1">Дата 2</param>
        /// <returns>0 - равны, положит.  - date0 больше date1, отриц. - date0 меньше date1 </returns>
        public static int CompareDateTime(this DateTime? date0, DateTime? date1)
        {
            DateTime d0 = date0 == null ? DateTime.UtcNow : date0.Value;
            var d1 = date1 == null ? DateTime.UtcNow : date1.Value;
            return DateTime.Compare(d0, d1);
        }

        /// <summary>
        /// Перевести UTC-время в заданный часовой пояс
        /// </summary>
        /// <param name="time">Дата и время UTC</param>
        /// <param name="timeZone">Часовой пояс в формате IANA Time Zone Database (zoneinfo database, Olson database)</param>
        /// <returns>Дата и время в заданном часовом поясе</returns>
        public static DateTime ConvertUtcToTimeZone(this DateTime time, string timeZone)
        {
            if (string.IsNullOrWhiteSpace(timeZone))
                return time;

            var easternTimeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(timeZone);
            if (easternTimeZone != null)
                return Instant.FromDateTimeUtc(time)
                              .InZone(easternTimeZone)
                              .ToDateTimeUnspecified();
            return time;
        }
    }
}