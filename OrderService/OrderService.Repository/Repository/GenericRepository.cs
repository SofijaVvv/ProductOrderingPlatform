using Microsoft.EntityFrameworkCore;
using OrderService.Repository.Context;
using OrderService.Repository.Interface;

namespace OrderService.Repository.Repository;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
	private readonly DbSet<T> _dbSet;

	public GenericRepository(ApplicationDbContext context)
	{
		_dbSet = context.Set<T>();
	}


	public virtual async Task<List<T>> GetAllAsync()
	{
		return await _dbSet.ToListAsync();
	}

	public virtual async Task<T?> GetByIdAsync(int id)
	{
		return await _dbSet.FindAsync(id);
	}

	public virtual void AddAsync(T entity)
	{
		_dbSet.Add(entity);
	}

	public virtual async Task<bool> DeleteAsync(int id)
	{
		var entity = await GetByIdAsync(id);
		if (entity == null) return false;

		_dbSet.Remove(entity);
		return true;
	}

	public virtual void Update(T entity)
	{
		_dbSet.Update(entity);
	}
}
