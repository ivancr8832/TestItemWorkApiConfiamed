using ItemWorks.Api.Domain.Repositories.Base;
using ItemWorks.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ItemWorks.Api.Infrastructure.RepositorieImpl.Base
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ItemWorkDbContext _itemWorkContext;

        public Repository(ItemWorkDbContext itemWorkContext)
        {
            _itemWorkContext = itemWorkContext;
        }

        public async Task<T> AddAsync(T entity)
        {
            await _itemWorkContext.Set<T>().AddAsync(entity);
            await _itemWorkContext.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(T entity)
        {
            var propertyInfo = typeof(T).GetProperty("Active");
            if (propertyInfo != null && propertyInfo.PropertyType == typeof(bool))
            {
                propertyInfo.SetValue(entity, false);
                _itemWorkContext.Set<T>().Update(entity);
                await _itemWorkContext.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException("The entity does not have an 'Active' property for logical deletion.");
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var propertyInfo = typeof(T).GetProperty("Active");

            if (propertyInfo != null && propertyInfo.PropertyType == typeof(bool))
            {
                var parameter = Expression.Parameter(typeof(T), "e");

                var propertyAccess = Expression.Property(parameter, propertyInfo);

                var condition = Expression.Equal(propertyAccess, Expression.Constant(true));

                var lambda = Expression.Lambda<Func<T, bool>>(condition, parameter);

                return await _itemWorkContext.Set<T>()
                    .Where(lambda)
                    .ToListAsync();
            }

            return await _itemWorkContext.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _itemWorkContext.Set<T>().FindAsync(id);
        }

        public async Task UpdateAsync(T entity)
        {
            _itemWorkContext.Set<T>().Update(entity);
            await _itemWorkContext.SaveChangesAsync();
        }
    }
}
