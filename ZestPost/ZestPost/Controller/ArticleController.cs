using ZestPost.Service;

namespace ZestPost.Controller
{
    public class ArticleController
    {
        private readonly ZestPostContext _context;
        private readonly CachingService _cache;
        private const string CacheKey = "articles";

        public ArticleController(ZestPostContext context, CachingService cache)
        {
            _context = context;
            _cache = cache;
        }

        public List<Article> GetAll()
        {
            var cachedArticles = _cache.Get<List<Article>>(CacheKey);
            if (cachedArticles != null)
            {
                return cachedArticles;
            }

            var articles = _context.Articles.ToList();
            _cache.Set(CacheKey, articles);
            return articles;
        }

        public void Add(Article article)
        {
            if (article != null)
            {
                _context.Articles.Add(article);
                _context.SaveChanges();
                _cache.Remove(CacheKey); // Invalidate cache
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
                _cache.Remove(CacheKey); // Invalidate cache
            }
        }

        public void Delete(int articleId)
        {
            var article = _context.Articles.Find(articleId);
            if (article != null)
            {
                _context.Articles.Remove(article);
                _context.SaveChanges();
                _cache.Remove(CacheKey); // Invalidate cache
            }
        }
    }
}
