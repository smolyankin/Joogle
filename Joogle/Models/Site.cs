using System;
using System.Data.Linq.Mapping;
using System.ComponentModel.DataAnnotations;

namespace Joogle.Models
{
    /// <summary>
    /// сущность сайта
    /// </summary>
    [Table(Name = "Sites")]
    public class Site
    {
        /// <summary>
        /// ид
        /// </summary>
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public long Id { get; set; }

        /// <summary>
        /// адрес сайта
        /// </summary>
        [Column]
        public string Url { get; set; }

        /// <summary>
        /// флаг парсинга
        /// </summary>
        [Column]
        public bool IsParsed { get; set; } = false;

        /// <summary>
        /// флаг удален
        /// </summary>
        [Column]
        public bool IsDeleted { get; set; } = false;

        /// <summary>
        /// дата изменения
        /// </summary>
        [Column]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime DateModify { get; set; }
    }
}