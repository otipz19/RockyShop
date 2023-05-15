using RockyShop.DataAccess.Data;
using RockyShop.DataAccess.Repository.Interfaces;
using RockyShop.Model.Models;

namespace RockyShop.DataAccess.Repository
{
    public class ApplicationTypeRepository : Repository<ApplicationType>, IApplicationTypeRepository
    {
        public ApplicationTypeRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
