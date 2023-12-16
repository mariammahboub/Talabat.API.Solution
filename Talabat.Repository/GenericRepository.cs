using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Specifications;
using Talabat.Repository.Data;

namespace Talabat.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly StoreDbContext _dbContext;

        public GenericRepository(StoreDbContext dbContext) 
        {
            _dbContext = dbContext;
        }
        public async Task<IReadOnlyList<T>> GetAllAsync() // Return all items in Model that not contain any navigation prop
        {
            //if(typeof(T) == typeof(Product))
            //    return (IEnumerable<T>) await _dbContext.Products.Include(P => P.Brand).Include(P => P.Category).ToListAsync();
            return await _dbContext.Set<T>().ToListAsync();
        }
        public async Task<T?> GetAsync(int id) // Return one item in Model that not contain any navigation prop
        {
            //if (typeof(T) == typeof(Product))
            //    return await _dbContext.Products.Where(P => P.Id == id).Include(P => P.Brand).Include(P => P.Category).FirstOrDefaultAsync() as T;

            return await _dbContext.Set<T>().FindAsync(id);
        }

        public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifications<T> spec)
        {
            return await AddSpecifications(spec).ToListAsync();
        }

        public async Task<T?> GetWithSpecAsync(ISpecifications<T> spec)
        {
            return await AddSpecifications(spec).FirstOrDefaultAsync();
        }
        public async Task<int> GetCountAsync(ISpecifications<T> spec)
        {
            return await AddSpecifications(spec).CountAsync();
        }

        private IQueryable<T> AddSpecifications(ISpecifications<T> spec)
        {
            return SpecificationsEvaluator<T>.GetQuery(_dbContext.Set<T>(), spec);
        }

        public async Task AddAsync(T entity)
            => await _dbContext.AddAsync(entity);

        public void Update(T entity)
            => _dbContext.Update(entity);

        public void Delete(T entity)
            => _dbContext.Remove(entity);
    }
}
