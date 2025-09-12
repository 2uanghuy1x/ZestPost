using ZestPost.Service;

namespace ZestPost.Controller
{
    public class GroupAccountController
    {
        private readonly ZestPostContext _context;
        private readonly CachingService _cache;
        private const string CacheKey = "groupAccounts";
        private const string CacheKeyPage = "pageAccounts";

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
                groupAccount.IsDelete = false;
                _context.GroupAccounts.Add(groupAccount);
                _context.SaveChanges();
                _cache.Remove(CacheKey);
            }
        }

        public List<GroupAccount> GetAllGroupPage()
        {
            var cachedGroupAccounts = _cache.Get<List<GroupAccount>>(CacheKeyPage);
            if (cachedGroupAccounts != null)
            {
                return cachedGroupAccounts;
            }

            var dataJoin = (from pagea in _context.PageAccounts
                            join groupAcc in _context.GroupAccounts on pagea.Uid equals groupAcc.UidFb into pageGroups
                            from groupAcc in pageGroups.DefaultIfEmpty()
                            where pagea.IsDelete == false && groupAcc.IsDelete == false
                            select new GroupAccount
                            {
                                Id = groupAcc.Id,
                                Name = groupAcc.Name,
                                UidFb = groupAcc.UidFb,
                                Note = groupAcc.Note
                            })
                    .Where(g => g.Id != null)
                    .ToList();
            _cache.Set(CacheKeyPage, dataJoin);
            return dataJoin;
        }

        public void Delete(int groupAccountId)
        {
            var groupAccount = _context.GroupAccounts.Find(groupAccountId);
            if (groupAccount != null)
            {
                groupAccount.DeletedAt = DateTime.UtcNow;
                groupAccount.IsDelete = true;
                _context.GroupAccounts.Remove(groupAccount);
                _context.SaveChanges();
                _cache.Remove(CacheKey); // Invalidate cache
            }
        }
    }
}
