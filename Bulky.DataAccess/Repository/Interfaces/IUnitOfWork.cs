namespace BulkyBook.DataAccess.Repository.Interfaces;

public interface IUnitOfWork
{
    ICategoryRepository CategoryRepository { get; }

    Task SaveAsync();
}
