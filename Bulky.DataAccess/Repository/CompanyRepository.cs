using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.Interfaces;
using BulkyBook.Models;

namespace BulkyBook.DataAccess.Repository;

public class CompanyRepository : Repository<Company>, ICompanyRepository
{
    public CompanyRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}
