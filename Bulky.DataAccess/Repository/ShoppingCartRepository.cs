using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.Interfaces;
using BulkyBook.Models;

namespace BulkyBook.DataAccess.Repository;

public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
{
    public ShoppingCartRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}
