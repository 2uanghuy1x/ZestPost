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
            // Omitted for brevity: assuming AppInfo.Cache is correctly initialized
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
                // Omitted for brevity: caching logic
                using (var dbContext = new ZestPostContext())
                {
                    lstCate = dbContext.Categories.ToList();
                }
                return lstCate;
            }
            catch (Exception ex)
            {
                // Omitted for brevity: logging
                return [];
            }
        }
        // Omitted for brevity: Other existing methods like GetAllCategory(string typeCate), etc.


        public bool InsertCategory(Category cate)
        {
            try
            {
                if (!string.IsNullOrEmpty(cate.Name))
                {
                    using (ZestPostContext context = new ZestPostContext())
                    {
                        if (context.Categories.FirstOrDefault(c => c.Name == cate.Name) != null)
                        {
                            return false;
                        }
                        context.Categories.Add(cate);
                        context.SaveChanges();
                    }
                    return true;
                }
            }
            catch (Exception ex) { /* Omitted for brevity: logging */ }
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
                        context.SaveChanges();
                    }
                    return true;
                }
            }
            catch (Exception ex) { /* Omitted for brevity: logging */ }
            return false;
        }
        public bool DeleteCategory(Category cate)
        {
            try
            {
                using (ZestPostContext context = new ZestPostContext())
                {
                    // Find the category in the database first
                    var categoryToDelete = context.Categories.FirstOrDefault(c => c.Id == cate.Id);
                    if (categoryToDelete != null)
                    {
                        DeleteAccountByCategory(categoryToDelete);
                        DeleteArticleByCategory(categoryToDelete);

                        // Use Remove for a single entity
                        context.Categories.Remove(categoryToDelete);

                        // Omitted for brevity: cache invalidation
                        context.SaveChanges();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                // Omitted for brevity: logging
            }
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
            catch (Exception ex) { /* Omitted for brevity: logging */ }
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
            catch (Exception ex) { /* Omitted for brevity: logging */ }
            return false;
        }
        public bool DeleteAccount(AccountFB acc)
        {
            try
            {
                using (ZestPostContext context = new ZestPostContext())
                {
                    var accountToDelete = context.AccountFBs.Find(acc.Id);
                    if (accountToDelete != null)
                    {
                        DeleteHistoryOfAccount(accountToDelete);
                        context.AccountFBs.Remove(accountToDelete);
                        // Omitted for brevity: cache invalidation
                        context.SaveChanges();
                    }
                }
                return true;
            }
            catch (Exception ex) { /* Omitted for brevity: logging */ }
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
            catch (Exception ex) { /* Omitted for brevity: logging */ }
            return false;
        }
    }
}
