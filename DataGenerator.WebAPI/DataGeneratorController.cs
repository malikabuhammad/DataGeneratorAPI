using Microsoft.AspNetCore.Mvc;
using DataGenerator.Domain.Interfaces;
using DataGenerator.Application.Interfaces;

namespace DataGenerator.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataGeneratorController : ControllerBase
    {
        private readonly IDataGenerationService _dataGenerationService;

        public DataGeneratorController(IDataGenerationService dataGenerationService)
        {
            _dataGenerationService = dataGenerationService;
        }

        [HttpGet("tables")]
        public IActionResult GetTables()
        {
            var tables = _dataGenerationService.GetTableNames();
            return Ok(tables);
        }

        [HttpPost("generate/{tableName}/{recordCount}")]
        public async Task<IActionResult> GenerateMockData(string tableName, int recordCount)
        {
            try
            {
                var rowsAdded = await _dataGenerationService.GenerateMockData(tableName, recordCount);
                return Ok($"{rowsAdded} records added to '{tableName}'.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
