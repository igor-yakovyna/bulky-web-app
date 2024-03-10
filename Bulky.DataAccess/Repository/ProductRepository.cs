using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.Interfaces;
using BulkyBook.Models;

namespace BulkyBook.DataAccess.Repository;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}
