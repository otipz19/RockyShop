using RockyShop.DataAccess.Data;
using RockyShop.DataAccess.Repository.Interfaces;
using RockyShop.Model.Models;
using System.Linq.Expressions;

namespace RockyShop.DataAccess.Repository
{
    public class InquiryDetailsRepository : Repository<InquiryDetails>, IInquiryDetailsRepository
    {
        private const string includeProduct = $"{nameof(InquiryDetails.Product)}";

        public InquiryDetailsRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public IEnumerable<InquiryDetails> GetAllIncludeProduct(
            Expression<Func<InquiryDetails, bool>> filter = null,
            Func<IQueryable<InquiryDetails>, IOrderedQueryable<InquiryDetails>> orderBy = null,
            bool isTracking = true)
        {
            return base.GetAll(filter, orderBy, includeProduct, isTracking);
        }
    }
}
