namespace OrderService.Repository.Interface;

public interface IUnitOfWork
{
	IGenericRepository<T> GetRepository<T>() where T : class;

	Task SaveAsync();
}
