using System.Collections.Generic;
using System.Linq;
using ZestPost.DbService;
using ZestPost.DbService.Entity;

namespace ZestPost.Controller
{
    public class ArticleController
    {
        private readonly ZestPostContext _context;

        public ArticleController(ZestPostContext context)
        {
            _context = context;
        }

        public List<Article> GetAll()
        {
            return _context.Articles.ToList();
        }

        public void Add(Article article)
        {
            if (article != null)
            {
                _context.Articles.Add(article);
                _context.SaveChanges();
            }
        }

        public void Update(Article article)
        {
            var existingArticle = _context.Articles.Find(article.Id);
            if (existingArticle != null)
            {
                existingArticle.Title = article.Title;
                existingArticle.Content = article.Content;
                _context.SaveChanges();
            }
        }

        public void Delete(int articleId)
        {
            var article = _context.Articles.Find(articleId);
            if (article != null)
            {
                _context.Articles.Remove(article);
                _context.SaveChanges();
            }
        }
    }
}
