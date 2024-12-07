using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGenerator.Domain.Interfaces
{
    public interface IDataGeneratorRepository
    {
        List<string> GetTableNames();
        Task<int> InsertMockData(string tableName, int recordCount);
    }
}
