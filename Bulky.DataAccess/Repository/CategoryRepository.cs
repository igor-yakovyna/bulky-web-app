using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.Interfaces;
using BulkyBook.Models;

namespace BulkyBook.DataAccess.Repository;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}
