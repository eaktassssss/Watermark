namespace Watermark.AzureStorage.TableStorage.Repository.Concrete
{
    using Microsoft.Azure.Cosmos.Table;
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Watermark.AzureStorage.ClientConnetion;
    using Watermark.AzureStorage.TableStorage.Repository.Abstract;

    public class TableStorageBaseRepository<Tentity> :ITableStorageRepository<Tentity>
        where Tentity : TableEntity, new()
    {
        internal CloudStorageAccount cloudStorageAccount;

        internal CloudTable _cloudTable;

        internal CloudTableClient _cloudTableClient;

        public TableStorageBaseRepository()
        {
            cloudStorageAccount = CloudStorageAccount.Parse(StorageConnection.ConnectionString);
            _cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
            _cloudTable = _cloudTableClient.GetTableReference(typeof(Tentity).Name);
            _cloudTable.CreateIfNotExistsAsync();
        }

        public IQueryable<Tentity> QueryAsync(Expression<Func<Tentity, bool>> expression)
        {
            return _cloudTable.CreateQuery<Tentity>().Where(expression).AsQueryable();
        }

        public async Task<Tentity> AddAsync(Tentity entity)
        {
            try
            {
                var operation = TableOperation.InsertOrMerge(entity);
                var execute = await _cloudTable.ExecuteAsync(operation);
                return execute.Result as Tentity;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public async Task<Tentity> DeleteAsync(string partitionKey, string rowkey)
        {
            try
            {
                var entity = await GetAsync(partitionKey, rowkey);
                var operation = TableOperation.Delete(entity);
                var execute = _cloudTable.Execute(operation);
                return execute.Result as Tentity;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public IQueryable<Tentity> QueryAsync()
        {
            return _cloudTable.CreateQuery<Tentity>().AsQueryable();
        }

        public async Task<Tentity> GetAsync(string partitionKey, string rowkey)
        {
            try
            {
                var operation = TableOperation.Retrieve<Tentity>(partitionKey, rowkey);
                var execute = await _cloudTable.ExecuteAsync(operation);
                return execute.Result as Tentity;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public async Task<Tentity> UpdateAsync(Tentity entity)
        {
            try
            {
                var operation = TableOperation.Replace(entity);
                var execute = await _cloudTable.ExecuteAsync(operation);
                return execute.Result as Tentity;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
    }
}
