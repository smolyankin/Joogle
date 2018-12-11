using System;
using System.Data.Linq.Mapping;
using System.ComponentModel.DataAnnotations;

namespace Joogle.Models
{
    /// <summary>
    /// сущность текста
    /// </summary>
    [Table(Name = "Texts")]
    public class Text
    {
        /// <summary>
        /// ид
        /// </summary>
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public long Id { get; set; }

        /// <summary>
        /// ид сайта
        /// </summary>
        [Column]
        public long? SiteId { get; set; }

        /// <summary>
        /// адрес сайта
        /// </summary>
        [Column]
        public string Url { get; set; }

        /// <summary>
        /// текст
        /// </summary>
        [Column]
        public string Title { get; set; }

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