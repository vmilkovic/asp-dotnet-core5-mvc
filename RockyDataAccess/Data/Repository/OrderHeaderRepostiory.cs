using RockyDataAccess.Data.Repository.IRepository;
using RockyModels.Models;

namespace RockyDataAccess.Data.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _db;

        public OrderHeaderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(OrderHeader orderDetail)
        {
            _db.OrderHeaders.Update(orderDetail);
        }
    }
}
