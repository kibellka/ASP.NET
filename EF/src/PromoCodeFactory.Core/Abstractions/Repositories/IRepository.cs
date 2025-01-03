﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.Core.Abstractions.Repositories
{
	public interface IRepository<T>
        where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAllAsync();

        Task<IEnumerable<T>> GetByIdsAsync(IEnumerable<Guid> ids);

        Task<T> GetByIdAsync(Guid id);

        Task<T> AddAsync(T entity);

        Task<T> UpdateAsync(T entity);

        Task<bool> DeleteAsync(Guid id);

        Task SaveChangesAsync();
    }
}