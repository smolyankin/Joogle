using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Joogle.Context;
using System.Data;
using System.Text;
using System.Threading;
using AngleSharp;
using AngleSharp.Extensions;
using Joogle.Extensions;
using Joogle.Models;
using Joogle.Request;
using Joogle.Response;

namespace Joogle.Services
{
    /// <summary>
    /// сервис joogle
    /// </summary>
    public class JoogleService
    {
        /// <summary>
        /// получить список сайтов
        /// </summary>
        /// <param name="model">модель списка сайтов</param>
        /// <param name="pageInfo">информация о странице</param>
        /// <returns>модель списка сайтов</returns>
        public async Task<SitesResponse> GetAllSites(SitesResponse model, PageInfo pageInfo)
        {
            using (var db = new JoogleContext())
            {
                var sites = db.Sites.OrderByDescending(x => x.DateModify).Skip((pageInfo.PageNumber - 1) * pageInfo.PageSize).Take(pageInfo.PageSize).ToList();
                var countSites = db.Sites.Count();
                model.Sites = sites;
                model.PageInfo.TotalItems = countSites;

                return model;
            }
        }

        /// <summary>
        /// создать сайт
        /// </summary>
        /// <param name="request">модель создания сайта</param>
        public async Task CreateSite(CreateSiteRequest request)
        {
            using (var db = new JoogleContext())
            {
                var url = request.Url.Last() == '/' ? request.Url.Remove(request.Url.Length - 1) : request.Url;
                var exist = db.Sites.FirstOrDefault(x => x.Url == url.ToLower());
                if (exist != null)
                    return;
                var site = new Site
                {
                    Url = url.ToLower(),
                    DateModify = DateTime.UtcNow
                };
                db.Sites.Add(site);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// информация о сайте
        /// </summary>
        /// <param name="id">ид сайта</param>
        public async Task<Site> GetDetailSite(long id)
        {
            using (var db = new JoogleContext())
            {
                return db.Sites.FirstOrDefault(x => x.Id == id);
            }
        }

        /// <summary>
        /// изменить сайт
        /// </summary>
        /// <param name="site">сайт</param>
        public async Task EditSite(Site site)
        {
            using (var db = new JoogleContext())
            {
                var exist = db.Sites.FirstOrDefault(x => x.Id == site.Id);
                if (site.IsParsed)
                {
                    var texts = db.Texts.Where(x => x.SiteId == site.Id).ToList();
                    texts.ForEach(t => t.IsDeleted = site.IsDeleted);
                }
                exist.IsDeleted = site.IsDeleted;
                db.SaveChanges();
            }
            
        }

        /// <summary>
        /// удалить сайт и все связанные текста
        /// </summary>
        /// <param name="site">сайт</param>
        public async Task DeleteSite(Site site)
        {
            try
            {
                using (var db = new JoogleContext())
                {
                    var exist = db.Sites.FirstOrDefault(x => x.Id == site.Id);
                    var text = db.Texts.FirstOrDefault(x => x.SiteId == exist.Id);
                    if (text != null)
                        db.Texts.Remove(text);
                    if (exist != null)
                        db.Sites.Remove(exist);
                    db.SaveChanges();
                }
            }
            catch { }
        }

        /// <summary>
        /// получить список текстов
        /// </summary>
        /// <param name="model">модель результата поиска</param>
        /// /// <param name="pageInfo">информация о странице</param>
        public async Task<TextsResponse> Search(TextsResponse model, PageInfo pageInfo)
        {
            using (var db = new JoogleContext())
            {
                var texts = db.Texts.Where(x => x.Title.Contains(model.Search)).OrderByDescending(x => x.DateModify).Skip((pageInfo.PageNumber - 1) * pageInfo.PageSize).Take(pageInfo.PageSize).ToList();
                var countTexts = db.Texts.Where(x => x.Title.Contains(model.Search)).Count();
                model.Search = model.Search;
                model.Texts = texts;
                model.PageInfo.TotalItems = countTexts;
                SubstringTexts(model);

                return model;
            }
        }

        /// <summary>
        /// урезание текста и выделение запроса
        /// </summary>
        /// <param name="model"></param>
        private void SubstringTexts(TextsResponse model)
        {
            var search = model.Search;
            var searchLength = search.Length;
            var maxLength = 400;
            
            foreach (var text in model.Texts)
            {
                StringBuilder newTitle = new StringBuilder(string.Empty);
                var result = new List<char>();
                var title = text.Title;
                var startIndex = title.ToLower().IndexOf(search);
                if (startIndex < 0)
                    startIndex++;
                var endIndex = startIndex + search.Length - 1;
                var searchFromTitle = title.Substring(startIndex, searchLength);
                
                var before = startIndex;
                var after = endIndex;

                var start = "<span style=\"color:red\";>";
                var end = "</span>";
                result.AddRange(start);
                result.AddRange(searchFromTitle);
                result.AddRange(end);
                var reverse = true;
                while (result.Count < maxLength)
                {
                    if (result.Count == title.Length)
                        break;
                    if (reverse)
                    {
                        if (before > 0)
                        {
                            before--;
                            result.Reverse();
                            result.Add(title[before]);
                            result.Reverse();
                        }
                    }
                    else
                    {
                        if (after < title.Length - 1)
                        {
                            after++;
                            result.Add(title[after]);
                        }
                    }

                    reverse = !reverse;
                }
                result.ForEach(x => newTitle.Append(x));
                text.Title = newTitle.ToString();
            }
        }

        /// <summary>
        /// запуск парсера
        /// </summary>
        /// <param name="model">модель парсера</param>
        /// <returns></returns>
        public async Task<ParseResponse> StartParseAllSites(ParseResponse model)
        {
            using (var db = new JoogleContext())
            {
                var startTime = DateTime.UtcNow;
                var sites = db.Sites.Where(x => !x.IsDeleted && !x.IsParsed).ToList();
                List<Thread> threads = new List<Thread>();
                foreach (var site in sites)
                    threads.Add(new Thread(() => SiteParse(site)));
                threads.ForEach(x => x.Start());
                threads.WaitAll();
                
                var endTime = DateTime.UtcNow;
                model.Sites = sites.Count;
                model.Time = endTime - startTime;
                model.Finished = true;

                return model;
            }
        }

        /// <summary>
        /// парсинг сайта
        /// </summary>
        /// <param name="obj">сайт</param>
        /// <returns></returns>
        private async Task SiteParse(object obj) //void
        {
            try
            {
                using (var db = new JoogleContext())
                {

                    var site = (Site)obj;
                    var exist = db.Sites.FirstOrDefault(x => x.Id == site.Id);
                    var result = new StringBuilder();
                    var config = Configuration.Default.WithDefaultLoader();
                    var task = BrowsingContext.New(config).OpenAsync(site.Url);
                    var html = task.Result;

                    var hrefs = html.QuerySelectorAll("a")
                        .Where(x => x.Attributes["href"] != null)
                        .Select(x => x.Attributes["href"].Value)
                        .Distinct()
                        .ToList();

                    var selectors = html.QuerySelectorAll("h1, h2, h3, h4, p");
                    foreach (var selector in selectors)
                    {
                        result.Append(" ");
                        result.Append(selector.TextContent);
                        result.Append(" ");
                    }

                    var newSites = new List<Site>();
                    if (hrefs.Any())
                    {
                        hrefs.RemoveAll(x => !x.StartsWith("http"));
                        foreach (var href in hrefs)
                        {
                            var url = href.Last() == '/' ? href.Remove(href.Length - 1).ToLower() : href.ToLower();
                            var existUrl = db.Sites.FirstOrDefault(x => x.Url == url);
                            if (existUrl != null || url == exist.Url)
                                continue;
                            newSites.Add(new Site
                            {
                                Url = url,
                                DateModify = DateTime.UtcNow
                            });
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(result.ToString()))
                    {
                        db.Texts.Add(new Text
                        {
                            SiteId = exist.Id,
                            Url = site.Url,
                            Title = result.ToString(),
                            DateModify = DateTime.UtcNow
                        });
                    }

                    exist.IsParsed = true;
                    db.Sites.AddRange(newSites);
                    db.SaveChanges();
                }
            }
            catch { }
        }

        /// <summary>
        /// очистка базы данных
        /// </summary>
        /// <returns></returns>
        public async Task ClearDatabase()
        {
            try
            {
                using (var db = new JoogleContext())
                {
                    db.Database.ExecuteSqlCommand("TRUNCATE TABLE [Sites]");
                    db.Database.ExecuteSqlCommand("TRUNCATE TABLE [Texts]");
                }
            }
            catch { }
        }
    }
}