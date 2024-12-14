using DataGenerator.Application.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;
 using DataGenerator.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace DataGenerator.Application.Services
{
    public class DataGenerationService : IDataGenerationService
    {
        private readonly IDataGeneratorRepository _repository;

        public DataGenerationService(IDataGeneratorRepository repository)
        {
            _repository = repository;
        }

        public List<string> GetTableNames()
        {
            return _repository.GetTableNames();
        }
       

        public async Task<int> GenerateMockData(string tableName, int recordCount)
        {
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentException("Table name cannot be empty.");

            return await _repository.InsertMockData(tableName, recordCount);
        }


        public async Task<List<(string ColumnName, string DataType, bool IsIdentity)>> GetTableMetadataAsync(string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentException("Table name cannot be empty.");

            return await _repository.GetTableMetadataAsync(tableName);
        }

    }
}
