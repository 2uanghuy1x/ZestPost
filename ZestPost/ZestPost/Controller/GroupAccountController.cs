using ZestPost.Service;

namespace ZestPost.Controller
{
    public class GroupAccountController
    {
        private readonly ZestPostContext _context;
        private readonly CachingService _cache;
        private const string CacheKey = "groupAccounts";

        public GroupAccountController(ZestPostContext context, CachingService cache)
        {
            _context = context;
            _cache = cache;
        }

        public List<GroupAccount> GetAll()
        {
            var cachedGroupAccounts = _cache.Get<List<GroupAccount>>(CacheKey);
            if (cachedGroupAccounts != null)
            {
                return cachedGroupAccounts;
            }

            var groupAccounts = _context.GroupAccounts.ToList();
            _cache.Set(CacheKey, groupAccounts);
            return groupAccounts;
        }

        public void Add(GroupAccount groupAccount)
        {
            if (groupAccount != null)
            {
                _context.GroupAccounts.Add(groupAccount);
                _context.SaveChanges();
                _cache.Remove(CacheKey); // Invalidate cache
            }
        }

        public void Delete(int groupAccountId)
        {
            var groupAccount = _context.GroupAccounts.Find(groupAccountId);
            if (groupAccount != null)
            {
                _context.GroupAccounts.Remove(groupAccount);
                _context.SaveChanges();
                _cache.Remove(CacheKey); // Invalidate cache
            }
        }
    }
}
