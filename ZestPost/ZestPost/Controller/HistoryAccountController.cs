using ZestPost.Service;

namespace ZestPost.Controller
{
    public class HistoryAccountController
    {
        private readonly ZestPostContext _context;
        private readonly CachingService _cache;
        private const string CacheKey = "historyAccounts";

        public HistoryAccountController(ZestPostContext context, CachingService cache)
        {
            _context = context;
            _cache = cache;
        }

        public List<HistoryAccount> GetAll()
        {
            var cachedHistoryAccounts = _cache.Get<List<HistoryAccount>>(CacheKey);
            if (cachedHistoryAccounts != null)
            {
                return cachedHistoryAccounts;
            }

            var historyAccounts = _context.HistoryAccounts.ToList();
            _cache.Set(CacheKey, historyAccounts);
            return historyAccounts;
        }

        public void Add(HistoryAccount historyAccount)
        {
            if (historyAccount != null)
            {
                _context.HistoryAccounts.Add(historyAccount);
                _context.SaveChanges();
                _cache.Remove(CacheKey); // Invalidate cache
            }
        }

        public void Delete(int historyAccountId)
        {
            var historyAccount = _context.HistoryAccounts.Find(historyAccountId);
            if (historyAccount != null)
            {
                _context.HistoryAccounts.Remove(historyAccount);
                _context.SaveChanges();
                _cache.Remove(CacheKey); // Invalidate cache
            }
        }
    }
}
