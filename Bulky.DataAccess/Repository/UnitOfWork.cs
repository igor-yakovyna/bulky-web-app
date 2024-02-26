using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.Interfaces;

namespace BulkyBook.DataAccess.Repository;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;
    private ICategoryRepository? _categoryRepository;

    public UnitOfWork(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public ICategoryRepository CategoryRepository => _categoryRepository ??= new CategoryRepository(_dbContext);

    public async Task SaveAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}
