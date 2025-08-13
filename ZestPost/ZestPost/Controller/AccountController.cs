using System.Collections.Generic;
using System.Linq;
using ZestPost.DbService;
using ZestPost.DbService.Entity;
using ZestPost.Service;

namespace ZestPost.Controller
{
    public class AccountController
    {
        private readonly ZestPostContext _context;
        private readonly CachingService _cache;
        private const string CacheKey = "accounts";

        public AccountController(ZestPostContext context, CachingService cache)
        {
            _context = context;
            _cache = cache;
        }

        public List<AccountFB> GetAll()
        {
            var cachedAccounts = _cache.Get<List<AccountFB>>(CacheKey);
            if (cachedAccounts != null)
            {
                return cachedAccounts;
            }

            var accounts = _context.AccountFBs.ToList();
            _cache.Set(CacheKey, accounts);
            return accounts;
        }

        public void Add(AccountFB account)
        {
            if (account != null)
            {
                _context.AccountFBs.Add(account);
                _context.SaveChanges();
                _cache.Remove(CacheKey); // Invalidate cache
            }
        }

        public void Update(AccountFB account)
        {
            var existingAccount = _context.AccountFBs.Find(account.Id);
            if (existingAccount != null)
            {
                existingAccount.Name = account.Name;
                existingAccount.Username = account.Username;
                if (!string.IsNullOrEmpty(account.Password)) // Only update password if provided
                {
                    existingAccount.Password = account.Password;
                }
                existingAccount.CategoryId = account.CategoryId;
                _context.SaveChanges();
                _cache.Remove(CacheKey); // Invalidate cache
            }
        }

        public void Delete(int accountId)
        {
            var account = _context.AccountFBs.Find(accountId);
            if (account != null)
            {
                _context.AccountFBs.Remove(account);
                _context.SaveChanges();
                _cache.Remove(CacheKey); // Invalidate cache
            }
        }
    }
}
