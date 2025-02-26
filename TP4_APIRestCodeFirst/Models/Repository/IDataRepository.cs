using Microsoft.AspNetCore.Mvc;
using TP4_APIRestCodeFirst.Models.EntityFramework;

namespace TP4_APIRestCodeFirst.Models.Repository
{
    public interface IDataRepository
    {
        public interface IDataRepository<TEntity>
        {
            Task<ActionResult<IEnumerable<TEntity>>> GetAllAsync();
            Task<ActionResult<TEntity>> GetByIdAsync(int id);
            Task<ActionResult<TEntity>> GetByStringAsync(string str);
            Task AddAsync(Utilisateur entity);
            Task UpdateAsync(TEntity entityToUpdate, TEntity entity);
            Task DeleteAsync(TEntity entity);
        }
    }
}
