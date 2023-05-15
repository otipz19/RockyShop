using RockyShop.Model.Models;
using System.Linq.Expressions;

namespace RockyShop.DataAccess.Repository.Interfaces
{
    public interface IInquiryDetailsRepository : IRepository<InquiryDetails>
    {
        public IEnumerable<InquiryDetails> GetAllIncludeProduct(
            Expression<Func<InquiryDetails, bool>> filter = null,
            Func<IQueryable<InquiryDetails>, IOrderedQueryable<InquiryDetails>> orderBy = null,
            bool isTracking = true);
    }
}
