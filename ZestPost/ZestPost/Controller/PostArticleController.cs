using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;
using System;
using ZestPost.DbService.DbContext;
using ZestPost.DbService.Entity;
using System.Threading.Tasks;
using ZestPost.Base.Model;

namespace ZestPost.Controller
{
    public class PostArticleController
    {
        private readonly string CACHE_ARTICLE = "__list_article__";
        private readonly IMemoryCache _cache;
        private static PostArticleController _instance;
        public PostArticleController()
        {
            _cache = AppInfo.Cache;
        }
        public static PostArticleController Instance()
        {
            if (_instance == null)
            {
                _instance = new PostArticleController();
            }
            return _instance;
        }

        public List<Article> GetListArticle()
        {
            List<Article> lst_article = new List<Article>();
            try
            {
                if (!_cache.TryGetValue(CACHE_ARTICLE, out lst_article))
                {
                    using (var dbContext = new ZestPostContext())
                    {
                        lst_article = dbContext.Articles.ToList();
                    }
                    _cache.Set(CACHE_ARTICLE, lst_article);
                }
                return lst_article;
            }
            catch (Exception ex)
            {
                Log4NetSyncController.LogException(ex, "");
                return [];
            }
        }
        
        public bool InsertArticle(Article article)
        {
            try
            {
                using (ZestPostContext context = new ZestPostContext())
                {
                    context.Articles.Add(article);
                    _cache.Remove(CACHE_ARTICLE);
                    context.SaveChanges();
                    return true;
                }
            }

            catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
            return false;
        }
        public async Task<bool> UpdateArticleAsync(Article article)
        {
            try
            {
                using (var context = new ZestPostContext())
                {
                    context.Articles.Update(article);
                    _cache.Remove(CACHE_ARTICLE);
                    await context.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                Log4NetSyncController.LogException(ex, "");
            }
            return false;
        }
        public bool DeleteArticle(Article article)
        {
            try
            {
                using (ZestPostContext context = new ZestPostContext())
                {
                    if (context.Articles.Find(article.Id) != null)
                    {
                        Article articleToDelete = context.Articles.First(a => a.Id == article.Id);
                        context.Articles.RemoveRange(articleToDelete);
                        _cache.Remove(CACHE_ARTICLE);
                        context.SaveChanges();
                    }
                }
                return true;
            }
            catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
            return false;
        }
    }
}
