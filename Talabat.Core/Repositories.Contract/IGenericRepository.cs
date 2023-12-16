using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Core.Repositories.Contract
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<T?> GetAsync(int id);// Return one item in Model that not contain any navigation prop
        Task<IReadOnlyList<T>> GetAllAsync();// Return all items in Model that not contain any navigation prop

        Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifications<T> spec);// Return all items in Model that contain navigation prop
        Task<T?> GetWithSpecAsync(ISpecifications<T> spec);// Return one item in Model that contain navigation prop
        Task<int> GetCountAsync(ISpecifications<T> spec);// Return Count all items 

        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);

    }
}
