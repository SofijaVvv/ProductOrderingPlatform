namespace OrderService.Repository.Interface;

public interface IGenericRepository<T> where T : class
{
	Task<List<T>> GetAllAsync();
	Task<T?> GetByIdAsync(int id);
	void AddAsync(T entity);
	Task<bool> DeleteAsync(int id);
	void Update(T entity);
}
