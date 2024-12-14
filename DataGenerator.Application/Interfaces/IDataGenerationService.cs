using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGenerator.Application.Interfaces
{
    public interface IDataGenerationService
    {
        List<string> GetTableNames();
        Task<List<(string ColumnName, string DataType, bool IsIdentity)>> GetTableMetadataAsync(string tableName);
        Task<int> GenerateMockData(string tableName, int recordCount);
    }
}
