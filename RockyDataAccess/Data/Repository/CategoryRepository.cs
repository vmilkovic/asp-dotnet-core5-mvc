using RockyDataAccess.Data.Repository.IRepository;
using RockyModels;
using System.Linq;

namespace RockyDataAccess.Data.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _db;

        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Category category)
        {
            var categoryFromDb = _db.Categories.FirstOrDefault(c => c.Id == category.Id);
            if (categoryFromDb != null)
            {
                categoryFromDb.Name = category.Name;
                categoryFromDb.DisplayOrder = category.DisplayOrder;
            }
        }
    }
}
