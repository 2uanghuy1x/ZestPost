using Microsoft.Extensions.Caching.Memory;

namespace ZestPost.Controller
{
    public class AccountController
    {
        private readonly string CACHE_ACCOUNT = "__list_account__";
        private readonly IMemoryCache _cache;
        private static AccountController _instance;
        public AccountController()
        {
            _cache = AppInfo.Cache;
        }
        public static AccountController Instance()
        {
            if (_instance == null)
            {
                _instance = new AccountController();
            }
            return _instance;
        }

        public List<AccountFB> GetListAccount()
        {
            List<AccountFB> lst_acc = new List<AccountFB>();
            try
            {
                if (!_cache.TryGetValue(CACHE_ACCOUNT, out lst_acc))
                {
                    using (var dbContext = new ZestPostContext())
                    {
                        lst_acc = dbContext.AccountFBs.ToList();
                    }
                    _cache.Set(CACHE_ACCOUNT, lst_acc);
                }
                return lst_acc;
            }
            catch (Exception ex)
            {
                Log4NetSyncController.LogException(ex, "");
                return [];
            }
        }
        public List<AccountFB> GetListAccount(List<Guid> lstId)
        {
            List<AccountFB> lst_acc = GetListAccount();
            try
            {
                lst_acc = lst_acc.Where(acc => lstId.Contains(acc.IdCategory)).ToList();
                return lst_acc;
            }
            catch (Exception ex)
            {
                Log4NetSyncController.LogException(ex, "");
            }
            return [];
        }
        public List<AccountFB> GetListAccount(List<Guid> lstId, string status = "")
        {
            List<AccountFB> lst_acc = GetListAccount();
            try
            {
                lst_acc = lst_acc.Where(acc => lstId.Contains(acc.IdCategory) && acc.Status.Contains(status)).ToList();
                return lst_acc;
            }
            catch (Exception ex)
            {
                Log4NetSyncController.LogException(ex, "");
                return null;
            }
        }
        public List<AccountFB> GetListAccount(Guid id_danmhmuc)
        {
            List<AccountFB> lst_acc = GetListAccount();
            try
            {
                lst_acc = lst_acc.Where(acc => acc.IdCategory == id_danmhmuc).ToList();
                return lst_acc;
            }
            catch (Exception ex)
            {
                Log4NetSyncController.LogException(ex, "");
                return null;
            }
        }
        public List<AccountFB> GetAccountsBySearch(string type_search, string search)
        {
            List<AccountFB> lst_acc = new List<AccountFB>();
            try
            {
                using (var dbContext = new ZestPostContext())
                {
                    switch (type_search)
                    {
                        case "uid":
                            lst_acc = dbContext.AccountFBs.Where(acc => search.Contains(acc.Uid)).ToList();
                            break;
                        case "email":
                            lst_acc = dbContext.AccountFBs.Where(acc => search.Contains(acc.Email)).ToList();
                            break;
                        case "name":
                            lst_acc = dbContext.AccountFBs.Where(acc => search.Contains(acc.Name)).ToList();
                            break;
                    }
                }
            }
            catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
            return lst_acc;
        }
        public List<AccountFB> GetAccount(string[] lst_column = null, string type = "username", bool is_Live = true)
        {
            List<AccountFB> lst_acc = GetListAccount();
            try
            {
                switch (type)
                {
                    case "uid":
                        lst_acc = lst_acc.Where(a => lst_column.Contains(a.Uid)).ToList();
                        break;

                    case "email":
                        lst_acc = lst_acc.Where(a => lst_column.Contains(a.Email)).ToList();
                        break;

                    case "status":
                        if (is_Live)
                        {
                            lst_acc = lst_acc.Where(a => a.Status == "Live").ToList();
                        }
                        else
                        {
                            lst_acc = lst_acc.Where(a => a.Status == "Die").ToList();
                        }
                        break;
                }
            }
            catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
            return lst_acc;
        }
        public bool InsertAccount(AccountFB acc)
        {
            try
            {
                if (string.IsNullOrEmpty(acc.Uid))
                {
                    return false;
                }
                using (ZestPostContext context = new ZestPostContext())
                {
                    if (context.AccountFBs.FirstOrDefault(c => c.Uid == acc.Uid) == null)
                    {
                        context.AccountFBs.Add(acc);
                        _cache.Remove(CACHE_ACCOUNT);
                        context.SaveChanges();
                        return true;
                    }
                }
            }

            catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
            return false;
        }
        public async Task<bool> UpdateAccountAsync(AccountFB acc)
        {
            try
            {
                if (!string.IsNullOrEmpty(acc.Uid))
                {
                    using (var context = new ZestPostContext())
                    {
                        context.AccountFBs.Update(acc);
                        _cache.Remove(CACHE_ACCOUNT);
                        await context.SaveChangesAsync();
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log4NetSyncController.LogException(ex, "");
            }
            return false;
        }
        public bool DeleteAccount(AccountFB acc)
        {
            try
            {
                using (ZestPostContext context = new ZestPostContext())
                {
                    if (context.AccountFBs.Find(acc.Id) != null)
                    {
                        DeleteHistoryOfAccount(acc);
                        AccountFB account = context.AccountFBs.First(a => a.Id == acc.Id);
                        context.AccountFBs.RemoveRange(account);
                        _cache.Remove(CACHE_ACCOUNT);
                        context.SaveChanges();
                    }
                }
                return true;
            }
            catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
            return false;
        }
        public bool DeleteHistoryOfAccount(AccountFB acc)
        {
            try
            {
                using (ZestPostContext context = new ZestPostContext())
                {
                    context.HistoryAccounts.RemoveRange(context.HistoryAccounts.Where(a => a.Uid == acc.Uid));
                    context.SaveChanges();
                }
                return true;
            }
            catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
            return false;
        }

    }
}
