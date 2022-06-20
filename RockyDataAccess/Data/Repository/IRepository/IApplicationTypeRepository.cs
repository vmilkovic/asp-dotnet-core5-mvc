using RockyModels;

namespace RockyDataAccess.Data.Repository.IRepository
{
    public interface IApplicationTypeRepository : IRepository<ApplicationType>
    {
        void Update(ApplicationType applicationType);
    }
}
