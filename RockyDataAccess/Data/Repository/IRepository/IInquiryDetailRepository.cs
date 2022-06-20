using RockyModels.Models;

namespace RockyDataAccess.Data.Repository.IRepository
{
    public interface IInquiryDetailRepository : IRepository<InquiryDetail>
    {
        void Update(InquiryDetail inquiryDetail);
    }
}
