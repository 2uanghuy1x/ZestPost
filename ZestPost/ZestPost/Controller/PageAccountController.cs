using ZestPost.Service;
using ZestPost.DbService.Entity;

namespace ZestPost.Controller
{
    public class PageAccountController
    {
        private readonly ZestPostContext _context;
        private readonly CachingService _cache;
        private const string CacheKey = "pageAccounts";

        public PageAccountController(ZestPostContext context, CachingService cache)
        {
            _context = context;
            _cache = cache;
        }

        public List<PageAccount> GetAll()
        {
            var cachedPageAccounts = _cache.Get<List<PageAccount>>(CacheKey);
            if (cachedPageAccounts != null)
            {
                return cachedPageAccounts;
            }

            var pageAccounts = _context.PageAccounts.ToList();
            _cache.Set(CacheKey, pageAccounts);
            return pageAccounts;
        }

        public void Add(PageAccount pageAccount)
        {
            if (pageAccount != null)
            {
                _context.PageAccounts.Add(pageAccount);
                _context.SaveChanges();
                _cache.Remove(CacheKey); // Invalidate cache
            }
        }

        public void Update(PageAccount pageAccount)
        {
            var existingPageAccount = _context.PageAccounts.Find(pageAccount.Id);
            if (existingPageAccount != null)
            {
                existingPageAccount.Name = pageAccount.Name;
                existingPageAccount.PageId = pageAccount.PageId;
                existingPageAccount.AccessToken = pageAccount.AccessToken;
                existingPageAccount.CategoryId = pageAccount.CategoryId;
                _context.SaveChanges();
                _cache.Remove(CacheKey); // Invalidate cache
            }
        }

        public void Delete(int pageAccountId)
        {
            var pageAccount = _context.PageAccounts.Find(pageAccountId);
            if (pageAccount != null)
            {
                _context.PageAccounts.Remove(pageAccount);
                _context.SaveChanges();
                _cache.Remove(CacheKey); // Invalidate cache
            }
        }
    }
}
