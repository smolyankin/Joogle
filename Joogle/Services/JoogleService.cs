using System;
using System.Linq;
using System.Threading.Tasks;
using Joogle.Context;
using System.Data;
using System.Text;
using System.Threading;
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
        public async Task<TextsResponse> Search(string search)
        {
            var texts = db.Texts.Where(x => x.Title.Contains(search));
            var model = new TextsResponse
            {
                Search = search,
                Texts = texts.ToList()
            };

            return model;
        }

        public async Task StartParseAllSites()
        {
            var sites = db.Sites.Where(x => !x.IsDeleted && !x.IsParsed);
            foreach (var site in sites)
            {
                ThreadPool.QueueUserWorkItem(SiteParse, site);
            }
        }

        /// <summary>
        /// парсинг сайта
        /// </summary>
        /// <param name="site">сайт</param>
        /// <returns></returns>
        async void SiteParse(object obj)
        {
            var site = (Site)obj;
            var result = new StringBuilder();
            var parser = new HtmlParser();
            var html = await parser.ParseAsync(site.Url);
            var hrefs = html.QuerySelectorAll("a").OfType<IHtmlAnchorElement>();
            var divs = html.QuerySelectorAll("div");
            var ps = html.QuerySelectorAll("p");
            foreach (var div in divs)
            {
                result.Append(div.Text());
                result.Append("");
            }
            foreach (var p in ps)
            {
                result.Append(p.Text());
                result.Append("");
            }

            if (hrefs != null)
            {
                foreach (var href in hrefs)
                {
                    await CreateSite(new CreateSiteRequest { Url = href.Text });
                }
            }

            db.Texts.Add(new Text
            {
                SiteId = site.Id,
                Url = site.Url,
                Title = result.ToString(),
                DateModify = DateTime.UtcNow
            });
            
            site.IsParsed = true;
            await db.SaveChangesAsync();
        }
    }
    /*
    public interface IJoogleService
    {
        Task CreateApp(CreateSiteRequest request);
    }*/
}