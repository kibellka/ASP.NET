using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
namespace PromoCodeFactory.DataAccess.Repositories
{
    public class InMemoryRepository<T>: IRepository<T> where T: BaseEntity
    {
        protected IEnumerable<T> Data { get; set; }

        public InMemoryRepository(IEnumerable<T> data)
        {
            Data = data;
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult(Data);
        }

        public Task<T> GetByIdAsync(Guid id)
        {
            return Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
        }

        public async Task<T> AddAsync(T entity)
        {
            var existedEntity = await GetByIdAsync(entity.Id);

            if (existedEntity == null)
            {
                Data = Data.Append(entity);
                return entity;
            }

            return null;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            var existedEntity = await GetByIdAsync(entity.Id);

            if (existedEntity != null)
            {
                var data = Data.ToList();
                var index = data.IndexOf(existedEntity);
                data[index] = entity;
                Data = data;
                return entity;
            }

            return null;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var existedEntity = await GetByIdAsync(id);

            if (existedEntity != null)
            {
                var data = Data.ToList();
                data.Remove(existedEntity);
                Data = data;
                return true;
            }

            return false;
        }
    }
}