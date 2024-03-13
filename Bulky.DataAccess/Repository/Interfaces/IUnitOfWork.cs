namespace BulkyBook.DataAccess.Repository.Interfaces;

public interface IUnitOfWork
{
    ICategoryRepository CategoryRepository { get; }

    IProductRepository ProductRepository { get; }

    ICompanyRepository CompanyRepository { get; }

    Task SaveAsync();
}
