using DataGenerator.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataGenerator.Infrastructure.Repositories
{
    public class DataGeneratorRepository : IDataGeneratorRepository
    {
        private readonly SqlDbContext _context;

        public DataGeneratorRepository(SqlDbContext context)
        {
            _context = context;
        }

        public List<string> GetTableNames()
        {
            // Retrieve all table names using the metadata
            return _context.Model.GetEntityTypes()
                .Select(entityType => entityType.GetTableName())
                .Where(name => !string.IsNullOrEmpty(name))
                .ToList();
        }

        public async Task<int> InsertMockData(string tableName, int recordCount)
        {
             var entityType = _context.Model.GetEntityTypes()
                .FirstOrDefault(t => t.GetTableName()?.Equals(tableName, StringComparison.OrdinalIgnoreCase) ?? false);

            if (entityType == null)
                throw new ArgumentException($"Table '{tableName}' does not exist.");

            var entityClrType = entityType.ClrType;

            // Generate mock data 
            var faker = new Faker();
            var mockData = Enumerable.Range(1, recordCount)
                .Select(_ =>
                {
                    var entity = Activator.CreateInstance(entityClrType);
                    PopulateMockData(entity, entityType, faker); 
                    return entity;
                })
                .ToList();

            _context.AddRange(mockData);
            return await _context.SaveChangesAsync();
        }

        private void PopulateMockData(object entity, IEntityType entityType, Faker faker)
        {
            foreach (var property in entityType.GetProperties())
            {

                //this code here for skipping the field if its identity in the table 
                if (property.ValueGenerated == ValueGenerated.OnAdd)
                    continue;



                // here i add data based on its type , ive added the main 4 or 5 types but if needed i can add more 
                 var propertyInfo = entity.GetType().GetProperty(property.Name);
                if (propertyInfo == null || !propertyInfo.CanWrite) continue;

                 if (propertyInfo.PropertyType == typeof(string))
                {
                    propertyInfo.SetValue(entity, faker.Lorem.Word());
                }
                else if (propertyInfo.PropertyType == typeof(int))
                {
                    propertyInfo.SetValue(entity, faker.Random.Int(1, 1000));
                }
                else if (propertyInfo.PropertyType == typeof(DateTime))
                {
                    propertyInfo.SetValue(entity, faker.Date.Past());
                }
                else if (propertyInfo.PropertyType == typeof(bool))
                {
                    propertyInfo.SetValue(entity, faker.Random.Bool());
                }
             }
        }
    }
}
