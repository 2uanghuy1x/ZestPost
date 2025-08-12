using Microsoft.Extensions.Caching.Memory;

namespace ZestPost.Controller
{
    public class CategoryController
    {
        private static CategoryController _instance;
        private readonly string CACHE_CATEGORY = "__list_category__";
        private readonly IMemoryCache _cache;

        public CategoryController()
        {
            _cache = AppInfo.Cache;
        }

        public static CategoryController Instance()
        {
            if (_instance == null)
            {
                _instance = new CategoryController();
            }
            return _instance;
        }

        public List<Category> GetAllCategory()
        {
            List<Category> lstCate = new List<Category>();
            try
            {
                if (!_cache.TryGetValue(CACHE_CATEGORY, out lstCate))
                {
                    using (var dbContext = new ZestPostContext())
                    {
                        lstCate = dbContext.Categories.ToList();
                    }
                    _cache.Set(CACHE_CATEGORY, lstCate);
                }
                return lstCate;
            }
            catch (Exception ex)
            {
                Log4NetSyncController.LogException(ex, "");
                return null;
            }
        }
        public List<Category> GetAllCategory(string typeCate)
        {
            List<Category> lstCate = GetAllCategory();
            try
            {
                lstCate = lstCate.Where(category => category.Type == typeCate).ToList();
            }
            catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
            return lstCate;
        }
        public List<Category> GetAllCategoryByName(string name)
        {
            List<Category> lst_category = GetAllCategory();
            try
            {
                lst_category = lst_category.Where(category => category.Name == name).ToList();
            }
            catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
            return lst_category;
        }
        public List<ComboBoxItem<Guid>> GetAllCategoryCheckBox(string typeCate)
        {
            List<Category> lstCate = GetAllCategory();
            try
            {
                List<ComboBoxItem<Guid>> lstCmb = (from cate in lstCate
                                                   where cate.Type == typeCate
                                                   select new ComboBoxItem<Guid>(cate.Id, cate.Name)).ToList();
                return lstCmb;
            }
            catch (Exception ex)
            {
                Log4NetSyncController.LogException(ex, "");
                return null;
            }
        }
        public List<Article> GetArticleByListId(List<string> lst_id, string type = "")
        {
            List<Article> lst_art = new List<Article>();
            try
            {
                using (ZestPostContext context = new ZestPostContext())
                {
                    foreach (string id in lst_id)
                    {
                        Article art = context.Articles.FirstOrDefault(a => a.Id.ToString() == id);
                        if (type == "post" && art.Type == "status") continue;
                        lst_art.Add(art);
                    }
                }
            }
            catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
            return lst_art;
        }

        public bool InsertCategory(Category cate)
        {
            try
            {
                if (!string.IsNullOrEmpty(cate.Name))
                {
                    using (ZestPostContext context = new ZestPostContext())
                    {
                        if (context.Categories.FirstOrDefault(c => c.Name == cate.Name) == null)
                        {
                            context.Categories.Add(cate);
                            _cache.Remove(CACHE_CATEGORY);

                            context.SaveChanges();
                        }
                    }
                    return true;
                }
            }
            catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
            return false;
        }
        public bool UpdateCategory(Category cate)
        {
            try
            {
                if (!string.IsNullOrEmpty(cate.Name))
                {
                    using (ZestPostContext context = new ZestPostContext())
                    {
                        context.Categories.Update(cate);
                        _cache.Remove(CACHE_CATEGORY);

                        context.SaveChanges();
                    }
                    return true;
                }
            }
            catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
            return false;
        }
        public bool DeleteCategory(Category cate)
        {
            try
            {
                if (!string.IsNullOrEmpty(cate.Name))
                {
                    using (ZestPostContext context = new ZestPostContext())
                    {
                        DeleteAccountByCategory(cate);
                        DeleteArticleByCategory(cate);
                        context.Categories.RemoveRange(cate);
                        _cache.Remove(CACHE_CATEGORY);
                        context.SaveChanges();
                    }
                    return true;
                }
            }
            catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
            return false;
        }
        public bool DeleteArticleByCategory(Category cate)
        {
            try
            {
                using (ZestPostContext context = new ZestPostContext())
                {
                    context.Articles.RemoveRange(context.Articles.Where(a => a.IdCategory == cate.Id));
                    context.SaveChanges();
                }
                return true;
            }
            catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
            return false;
        }
        public bool DeleteAccountByCategory(Category cate)
        {
            try
            {
                using (ZestPostContext context = new ZestPostContext())
                {
                    foreach (AccountFB acc in context.AccountFBs.Where(a => a.IdCategory == cate.Id))
                    {
                        DeleteAccount(acc);
                    }
                    context.SaveChanges();
                }
                return true;
            }
            catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
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
                        _cache.Remove(CACHE_CATEGORY);
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
