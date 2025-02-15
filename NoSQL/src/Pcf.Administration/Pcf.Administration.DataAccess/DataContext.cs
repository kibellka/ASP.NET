using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using Pcf.Administration.Core.Domain.Administration;
using Pcf.Administration.DataAccess.Data;

namespace Pcf.Administration.DataAccess
{
    public class DataContext (IMongoDatabase database)
    {
        IMongoDatabase _db = database;

        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            await InitializeRolesAsync("roles");
            await InitializeEmployees("employees");
        }

        private Task InitializeRolesAsync(string collectionName)
        {
            ReCreateCollection(collectionName);

            var collection = _db.GetCollection<Role>(collectionName);
            var roles = FakeDataFactory.Roles;

            return collection.InsertManyAsync(roles);
        }

        private Task InitializeEmployees(string collectionName)
        {
            ReCreateCollection(collectionName);

            var collection = _db.GetCollection<Employee>(collectionName);
            var employees = FakeDataFactory.Employees;

            return collection.InsertManyAsync(employees);
        }

        private void ReCreateCollection(string collectionName)
        {
            _db.DropCollection(collectionName);
            _db.CreateCollection(collectionName);
        }
    }
}
