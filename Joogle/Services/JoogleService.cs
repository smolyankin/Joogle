using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Joogle.Context;
using System.Data;
using System.Net.Http;
using System.Text;
using System.Threading;
using AngleSharp;
using AngleSharp.Dom.Html;
using AngleSharp.Extensions;
using AngleSharp.Parser.Html;
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
        private JoogleContext db = new JoogleContext();

        /// <summary>
        /// получить список сайтов
        /// </summary>
        /// <param name="model">модель списка сайтов</param>
        public async Task<SitesResponse> GetAllSites(SitesResponse model)
        {
            var sites = db.Sites;
            model.Sites = sites.OrderByDescending(x => x.DateModify).ToList();

            return model;
        }

        /// <summary>
        /// создать сайт
        /// </summary>
        /// <param name="request">модель создания сайта</param>
        public async Task CreateSite(CreateSiteRequest request)
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

        /// <summary>
        /// информация о сайте
        /// </summary>
        /// <param name="request">сайт</param>
        public async Task<Site> GetDetailSite(Site request)
        {
            return db.Sites.FirstOrDefault(x => x.Id == request.Id);
        }

        /// <summary>
        /// изменить сайт
        /// </summary>
        /// <param name="request">сайт</param>
        public async Task EditSite(Site site)
        {
            var exist = db.Sites.FirstOrDefault(x => x.Id == site.Id);
            if (site.IsDeleted != site.IsDeleted)
            {
                var texts = db.Texts.Where(x => x.SiteId == site.Id);
                foreach (var text in texts)
                    text.IsDeleted = site.IsDeleted;
            }
            if (exist.IsParsed)
            {
                var texts = db.Texts.Where(x => x.SiteId == site.Id);
                foreach (var text in texts)
                    text.SiteId = null;
            }
            exist = site.ShallowCopy();
            db.SaveChanges();
        }

        /// <summary>
        /// удалить сайт и все связанные текста
        /// </summary>
        /// <param name="id">ид сайта</param>
        public async Task DeleteSite(long id)
        {
            var site = db.Sites.FirstOrDefault(x => x.Id == id);
            var texts = db.Texts.Where(x => x.SiteId == site.Id);
            db.Texts.RemoveRange(texts);
            db.Sites.Remove(site);
            db.SaveChanges();
        }

        /// <summary>
        /// получить список текстов
        /// </summary>
        /// <param name="search">поисковый запрос</param>
        public async Task<TextsResponse> Search(TextsResponse model, PageInfo pageInfo)
        {
            var texts = db.Texts.Where(x => x.Title.Contains(model.Search)).OrderByDescending(x => x.DateModify).Skip((pageInfo.PageNumber - 1) * pageInfo.PageSize).Take(pageInfo.PageSize).ToList();
            var countPages = db.Texts.Where(x => x.Title.Contains(model.Search)).Count();
            model.Search = model.Search;
            model.Texts = texts;
            var oldTotalItems = model.PageInfo.TotalItems;
            model.PageInfo.TotalItems = countPages;
            if (model.SearchOld != model.Search)
                model.PageInfo.PageNumber = 1;
            //model.PageInfo.PageNumber = model.SearchOld == model.Search ? model.PageInfo.PageNumber : 1;

            return model;
        }

        public async Task StartParseAllSites()
        {
            var sites = db.Sites.Where(x => !x.IsDeleted && !x.IsParsed).ToList();
            foreach (var site in sites)
            {
                await SiteParse(site);
            }
        }

        /// <summary>
        /// парсинг сайта
        /// </summary>
        /// <param name="site">сайт</param>
        /// <returns></returns>
        public async Task SiteParse(Site site)
        {
            try
            {
                using (JoogleContext data = new JoogleContext())
                {
                    //var site = (Site)obj;
                    var result = new StringBuilder();
                    /*var parser = new HtmlParser();
                    var cancellationToken = new CancellationTokenSource();
                    var httpClient = new HttpClient();
                    var request = await httpClient.GetAsync(site.Url);
                    cancellationToken.Token.ThrowIfCancellationRequested();
                    var response = await request.Content.ReadAsStreamAsync();
                    cancellationToken.Token.ThrowIfCancellationRequested();

                    var html = await parser.ParseAsync(response);*/
                    var config = Configuration.Default.WithDefaultLoader();
                    var task = BrowsingContext.New(config).OpenAsync(site.Url); //используем чтобы не было проблем с кодировкой
                    var html = task.Result;

                    var hrefs = html.QuerySelectorAll("a")
                        .Where(x => x.Attributes["href"] != null)
                        .Select(x => x.Attributes["href"].Value)
                        .Distinct()
                        .ToList();
                    //var divs = html.QuerySelectorAll("div");
                    var ps = html.QuerySelectorAll("p");
                    /*foreach (var div in divs)
                    {
                        result.Append(div.TextContent);
                        result.Append(" ");
                    }*/
                    foreach (var p in ps)
                    {
                        result.Append(p.TextContent);
                        result.Append(" ");
                    }

                    var newSites = new List<Site>();
                    if (hrefs.Any())
                    {
                        hrefs.RemoveAll(x => !x.StartsWith("http"));
                        hrefs.RemoveAll(x => x == site.Url);
                        var existSites = db.Sites.ToList();
                        foreach (var href in hrefs)
                        {
                            var url = href.Last() == '/' ? href.Remove(href.Length - 1).ToLower() : href.ToLower();
                            //await CreateSite(new CreateSiteRequest { Url = href });
                            if (existSites.Count(x => x.Url == url) > 0)
                                continue;
                            newSites.Add(new Site
                            {
                                Url = url,
                                DateModify = DateTime.UtcNow
                            });
                        }
                    }

                    db.Sites.AddRange(newSites);
                    db.Texts.Add(new Text
                    {
                        SiteId = site.Id,
                        Url = site.Url,
                        Title = result.ToString(),
                        DateModify = DateTime.UtcNow
                    });

                    site.IsParsed = true;
                    db.SaveChanges();
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                //db.Dispose();
            }
        }
    }
}