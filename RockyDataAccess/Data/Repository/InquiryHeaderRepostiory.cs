using RockyDataAccess.Data.Repository.IRepository;
using RockyModels.Models;

namespace RockyDataAccess.Data.Repository
{
    public class InquiryHeaderRepository : Repository<InquiryHeader>, IInquiryHeaderRepository
    {
        private readonly ApplicationDbContext _db;

        public InquiryHeaderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(InquiryHeader inquiryHeader)
        {
            _db.InquiryHeaders.Update(inquiryHeader);
        }
    }
}
