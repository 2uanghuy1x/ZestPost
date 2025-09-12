using ZestPost.Service;

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
                pageAccount.IsDelete = false;
                _context.PageAccounts.Add(pageAccount);
                _context.SaveChanges();
                _cache.Remove(CacheKey); // Invalidate cache
            }
        }
        public void Delete(int pageAccountId)
        {
            var pageAccount = _context.PageAccounts.Find(pageAccountId);
            if (pageAccount != null)
            {
                pageAccount.DeletedAt = DateTime.UtcNow;
                pageAccount.IsDelete = true;
                _context.PageAccounts.Remove(pageAccount);
                _context.SaveChanges();
                _cache.Remove(CacheKey); // Invalidate cache
            }
        }
    }
}
