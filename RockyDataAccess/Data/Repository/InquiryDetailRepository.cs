using RockyDataAccess.Data.Repository.IRepository;
using RockyModels.Models;

namespace RockyDataAccess.Data.Repository
{
    public class InquiryDetailRepository : Repository<InquiryDetail>, IInquiryDetailRepository
    {
        private readonly ApplicationDbContext _db;

        public InquiryDetailRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(InquiryDetail inquiryDetail)
        {
            _db.InquiryDetails.Update(inquiryDetail);
        }
    }
}
