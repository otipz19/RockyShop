using RockyShop.DataAccess.Data;
using RockyShop.DataAccess.Repository.Interfaces;
using RockyShop.Model.Models;

namespace RockyShop.DataAccess.Repository
{
    public class InquiryHeaderRepository : Repository<InquiryHeader>, IInquiryHeaderRepository
    {
        public InquiryHeaderRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
