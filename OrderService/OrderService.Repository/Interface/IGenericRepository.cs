namespace OrderService.Repository.Interface;

public interface IGenericRepository<T> where T : class
{
	Task<List<T>> GetAllAsync();
	Task<T?> GetByIdAsync(int id);
	void Add(T entity);
	void DeleteAsync(T entity);
	void Update(T entity);
}
