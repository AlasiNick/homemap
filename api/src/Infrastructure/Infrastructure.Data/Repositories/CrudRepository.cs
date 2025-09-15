using Homemap.ApplicationCore.Interfaces.Repositories;
using Homemap.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Homemap.Infrastructure.Data.Repositories
{
    internal class CrudRepository<T> : Repository, ICrudRepository<T> where T : class
    {
        private readonly DbSet<T> _entities;

        public CrudRepository(ApplicationDbContext context) : base(context)
        {
            _entities = context.Set<T>();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _entities.AddAsync(entity);
            return entity;
        }

        public async Task<T?> FindByIdAsync(int id)
        {
            return await _entities.FindAsync(id);
        }

        public async Task<IReadOnlyList<T>> FindAllAsync()
        {
            return await _entities.ToListAsync();
        }

        public void Update(T entity)
        {
            _entities.Update(entity);
        }

        public void Remove(T entity)
        {
            _entities.Remove(entity);
        }
        
    }
}
