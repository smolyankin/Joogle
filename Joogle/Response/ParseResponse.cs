using System;
using System.ComponentModel.DataAnnotations;

namespace Joogle.Response
{
    /// <summary>
    /// результат парсинга
    /// </summary>
    public class ParseResponse
    {
        /// <summary>
        /// количество сайтов
        /// </summary>
        public long Sites { get; set; }

        /// <summary>
        /// затраченное время
        /// </summary>
        public TimeSpan Time { get; set; }

        /// <summary>
        /// флаг завершения
        /// </summary>
        public bool Finished { get; set; }

        /// <summary>
        /// количество не обработанных сайтов
        /// </summary>
        public long SitesNotParsed { get; set; }
    }
}