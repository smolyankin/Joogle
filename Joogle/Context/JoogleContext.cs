using System.Data.Entity;
using Joogle.Models;

namespace Joogle.Context
{
    /// <summary>
    /// подключение к базе данных
    /// </summary>
    public class JoogleContext : DbContext
    {
        /// <summary>
        /// путь по умолчанию
        /// </summary>
        public JoogleContext() : base("DefaultConnection")
        {

        }

        /// <summary>
        /// таблица сайтов
        /// </summary>
        public DbSet<Site> Sites { get; set; }

        /// <summary>
        /// таблица текстов
        /// </summary>
        public DbSet<Text> Texts { get; set; }
    }
}