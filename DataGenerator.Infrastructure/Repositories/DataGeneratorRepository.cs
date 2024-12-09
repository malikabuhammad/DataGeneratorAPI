using DataGenerator.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.Data.SqlClient;

namespace DataGenerator.Infrastructure.Repositories
{
    public class DataGeneratorRepository : IDataGeneratorRepository
    {
        private readonly SqlDbContext _context;

        private readonly IConfiguration _configuration;
      
        
       
        public DataGeneratorRepository(SqlDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

        }

        public List<string> GetTableNames()
        {


            const string query = @"
select table_name  from Information_Schema.tables
where table_type='Base table' and table_schema = 'dbo'


                                ";
            var tablenames = new List<string>();
            using (var connection= new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {

                connection.Open();
                using (var command= connection.CreateCommand()) 
                {
                    command.CommandText = query;
                    using (var reader= command.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            tablenames.Add(reader.GetString(0));
                        }
                    }
                 
                
                }
            }
            return tablenames;
        }


        //returning the table columns 
         private async Task<List<(string ColumnName, string DataType, bool IsIdentity)>> GetTableMetadataAsync(string tableName)
        {
            var result = new List<(string ColumnName, string DataType, bool IsIdentity)>();

            //this query return the table columns and if is the column is identity 
            const string query = @"
                select column_name,Data_type,columnProperty(OBJECT_ID(TABLE_NAME), COLUMN_NAME,'IsIdentity') as IsIdentity
                from INFORMATION_SCHEMA.COLUMNS
                where  table_name = @TableName";

            using (var connection = _context.Database.GetDbConnection())
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;

                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "@TableName";
                    parameter.Value = tableName;
                    command.Parameters.Add(parameter);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var columnName = reader.GetString(0);
                            var dataType = reader.GetString(1);
                            var isIdentity = !reader.IsDBNull(2) && reader.GetInt32(2) == 1;

                            result.Add((columnName, dataType, isIdentity));
                        }
                    }
                }
            }

            return result;
        }

      
        public async Task<int> InsertMockData(string tableName, int recordCount)
        {
            // Get table metadata
            var metadata = await GetTableMetadataAsync(tableName);
            if (!metadata.Any())
                throw new ArgumentException($"Table '{tableName}' does not exist or has no columns.");

            var columns = metadata.Where(m => !m.IsIdentity).ToList(); // Skip identity columns
            if (!columns.Any())
                throw new ArgumentException($"Table '{tableName}' has only identity columns, no data can be inserted.");

            var faker = new Faker();

            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {


                    await connection.OpenAsync();


                    using (var transaction = connection.BeginTransaction())
                    {
                        for (int i = 0; i < recordCount; i++)
                        {
                   
                            var columnNames = string.Join(", ", columns.Select(c => c.ColumnName));
                            var parameterNames = string.Join(", ", columns.Select((_, index) => $"@param{index}"));

                            string insertQuery = $@"
                            INSERT INTO {tableName} ({columnNames})
                            VALUES ({parameterNames})";

                            using (var command = connection.CreateCommand())
                            {
                                command.Transaction = transaction;
                                command.CommandText = insertQuery;

                                for (int j = 0; j < columns.Count; j++)
                                {
                                    var column = columns[j];
                                    var parameter = command.CreateParameter();
                                    parameter.ParameterName = $"@param{j}";
                                    parameter.Value = GenerateMockValue(column.DataType, faker);
                                    command.Parameters.Add(parameter);
                                }

                                await command.ExecuteNonQueryAsync();
                            }
                        }

                        await transaction.CommitAsync();
                    }
                }
            }
            catch( Exception ex )
            {
                 
            }

            return recordCount;
        }

        // Generate mock value based on column type
        private object GenerateMockValue(string dataType, Faker faker)
        {
            return dataType.ToLower() switch
            {
                "int" or "bigint" => faker.Random.Int(1, 1000),
                "nvarchar" or "varchar" or "text" => faker.Lorem.Word(),
                "datetime" or "date" => faker.Date.Past(),
                "bit" => faker.Random.Bool(),
                "float" => faker.Random.Double(1, 1000),
                "decimal" => faker.Random.Decimal(1, 1000),
                "uniqueidentifier" => Guid.NewGuid(),
                _ => DBNull.Value  
            };
        }
    }
        

    }
 
