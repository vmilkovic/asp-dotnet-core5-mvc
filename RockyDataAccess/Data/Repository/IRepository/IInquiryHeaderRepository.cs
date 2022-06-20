using RockyModels.Models;

namespace RockyDataAccess.Data.Repository.IRepository
{
    public interface IInquiryHeaderRepository : IRepository<InquiryHeader>
    {
        void Update(InquiryHeader inquiryHeader);
    }
}
