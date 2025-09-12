using ZestPost.Service;

namespace ZestPost.Controller
{
    public class CategoryController
    {
        private readonly ZestPostContext _context;
        private readonly CachingService _cache;
        private const string CacheKey = "categories";

        public CategoryController(ZestPostContext context, CachingService cache)
        {
            _context = context;
            _cache = cache;
        }

        public List<Category> GetAll()
        {
            var cachedCategories = _cache.Get<List<Category>>(CacheKey);
            if (cachedCategories != null)
            {
                return cachedCategories;
            }

            var categories = _context.Categories.Where(x => x.IsDelete == false).ToList();
            _cache.Set(CacheKey, categories);
            return categories;
        }

        public void Add(Category category)
        {
            if (category != null)
            {
                category.IsDelete = false;
                _context.Categories.Add(category);
                _context.SaveChanges();
                _cache.Remove(CacheKey); // Invalidate cache
            }
        }

        public void Update(Category category)
        {
            var existingCategory = _context.Categories.Find(category.Id);
            if (existingCategory != null)
            {
                existingCategory.Name = category.Name;
                existingCategory.Type = category.Type;
                _context.SaveChanges();
                _cache.Remove(CacheKey); // Invalidate cache
            }
        }

        public void Delete(int categoryId)
        {
            var category = _context.Categories.Find(categoryId);
            if (category != null)
            {
                category.DeletedAt = DateTime.UtcNow;
                category.IsDelete = true;
                _context.SaveChanges();
                _cache.Remove(CacheKey); // Invalidate cache
            }
        }
    }
}
