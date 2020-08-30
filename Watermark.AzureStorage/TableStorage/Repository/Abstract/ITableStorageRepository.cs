using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Watermark.AzureStorage.TableStorage.Repository.Abstract
{
    public interface ITableStorageRepository<TEntity> where TEntity : TableEntity, new()
    {
        Task<TEntity> AddAsync(TEntity entity);
        Task<TEntity> GetAsync(string partitionKey, string rowkey);
        Task<TEntity> UpdateAsync(TEntity entity);
        Task<TEntity> DeleteAsync(string partitionKey, string rowkey);
        IQueryable<TEntity> QueryAsync(Expression<Func<TEntity, bool>> expression);
        IQueryable<TEntity> QueryAsync();
    }
}
