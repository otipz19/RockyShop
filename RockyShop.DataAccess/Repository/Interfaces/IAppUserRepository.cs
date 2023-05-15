using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using RockyShop.Model.Models;

namespace RockyShop.DataAccess.Repository.Interfaces
{
    public interface IAppUserRepository : IRepository<AppUser>
    {
        public AppUser Find(string id);
    }
}
