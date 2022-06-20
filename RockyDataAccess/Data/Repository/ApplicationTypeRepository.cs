using RockyDataAccess.Data.Repository.IRepository;
using RockyModels;
using System.Linq;

namespace RockyDataAccess.Data.Repository
{
    public class ApplicationTypeRepository : Repository<ApplicationType>, IApplicationTypeRepository
    {
        private readonly ApplicationDbContext _db;

        public ApplicationTypeRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(ApplicationType applicationType)
        {
            var applicationTypeFromDb = _db.ApplicationTypes.FirstOrDefault(at => at.Id == applicationType.Id);
            if (applicationTypeFromDb != null)
            {
                applicationTypeFromDb.Name = applicationType.Name;
            }
        }
    }
}
