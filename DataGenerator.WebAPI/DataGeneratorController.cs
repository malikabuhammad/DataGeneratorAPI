using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using DataGenerator.Application.Interfaces;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using DataGenerator.Domain.TemplateData;
using System.Xml;
using Newtonsoft.Json;
namespace DataGenerator.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataGeneratorController : Controller
    {
        private readonly IDataGenerationService _dataGenerationService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DataGeneratorController(IDataGenerationService dataGenerationService, IWebHostEnvironment webHostEnvironment)
        {
            _dataGenerationService = dataGenerationService;
            _webHostEnvironment = webHostEnvironment;

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


        #region Old Generating Code 

        //[HttpPost("generate-files/{tableName}")]
        //public async Task<IActionResult> GenerateFiles(string tableName)
        //{
        //    var tableMetaData = await _dataGenerationService.GetTableMetadataAsync(tableName);

        //    if (tableMetaData == null || !tableMetaData.Any())
        //    {
        //        return BadRequest($"No metadata found for the table {tableName}.");
        //    }

        //    string outputPath = Path.Combine(Path.GetTempPath(), "GeneratedFiles", tableName);
        //    Directory.CreateDirectory(outputPath);

        //    try
        //    {
        //        // Generate Controller
        //        string templatePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Templates");
        //        var controllerTemplate = System.IO.File.ReadAllText(Path.Combine(templatePath, "ControllerTemplate.txt"));
        //        var controllerContent = ReplacePlaceholders(controllerTemplate, tableName, tableMetaData);
        //        string controllerPath = Path.Combine(outputPath, $"{tableName}Controller.cs");
        //        System.IO.File.WriteAllText(controllerPath, controllerContent);

        //        // Generate Models
        //        var tableModelTemplate = System.IO.File.ReadAllText(Path.Combine(templatePath, "ModelsTemplate.txt"));

        //        var ModelContent = ReplacePlaceholders(tableModelTemplate, tableName, tableMetaData);

        //        string ModelPath = Path.Combine(outputPath, $"{tableName}AddModel.cs");
        //         System.IO.File.WriteAllText(ModelPath, ModelContent);

        //        // Generate Views
        //        var indexViewTemplate = System.IO.File.ReadAllText(Path.Combine(templatePath, "IndexViewTemplate.cshtml"));
        //        var addViewTemplate = System.IO.File.ReadAllText(Path.Combine(templatePath, "AddViewTemplate.cshtml"));
        //        var editViewTemplate = System.IO.File.ReadAllText(Path.Combine(templatePath, "EditViewTemplate.cshtml"));

        //        var indexViewContent = ReplacePlaceholders(indexViewTemplate, tableName, tableMetaData);
        //        var addViewContent = ReplacePlaceholders(addViewTemplate, tableName, tableMetaData);
        //        var editViewContent = ReplacePlaceholders(editViewTemplate, tableName, tableMetaData);

        //        string viewsPath = Path.Combine(outputPath, "Views", tableName);
        //        Directory.CreateDirectory(viewsPath);

        //        System.IO.File.WriteAllText(Path.Combine(viewsPath, "Index.cshtml"), indexViewContent);
        //        System.IO.File.WriteAllText(Path.Combine(viewsPath, "Add.cshtml"), addViewContent);
        //        System.IO.File.WriteAllText(Path.Combine(viewsPath, "Edit.cshtml"), editViewContent);

        //        // Package all files into a zip
        //        string zipPath = Path.Combine(Path.GetTempPath(), $"{tableName}_Files.zip");
        //        if (System.IO.File.Exists(zipPath)) System.IO.File.Delete(zipPath);

        //        System.IO.Compression.ZipFile.CreateFromDirectory(outputPath, zipPath);

        //        // Return the zip file as response
        //        var bytes = await System.IO.File.ReadAllBytesAsync(zipPath);
        //        return File(bytes, "application/zip", $"{tableName}_Files.zip");
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest($"Error generating files for table '{tableName}': {ex.Message}, {ex.InnerException}");
        //    }
        //}


        //old code 
        //private string ReplacePlaceholders(
        //string template,
        //string tableName,
        //List<(string ColumnName, string DataType, bool IsIdentity)> metadata)
        //    {
        //        var properties = string.Join(Environment.NewLine,
        //            metadata.Select(m => $"public {MapToCSharpType(m.DataType)} {m.ColumnName} {{ get; set; }}"));

        //        var headers = string.Join(Environment.NewLine, metadata.Select(m => $"<th>{m.ColumnName}</th>"));
        //        var values = string.Join(Environment.NewLine, metadata.Select(m => $"<td>@item.{m.ColumnName}</td>"));

        //        return template
        //            .Replace("{TableName}", tableName)
        //            .Replace("{Properties}", properties)
        //            .Replace("{Headers}", headers)
        //            .Replace("{Values}", values);
        //    }
        #endregion


        [HttpPost("generate-files/{tableName}")]
        public async Task<IActionResult> GenerateFiles(string tableName,string NameSpace , int PermIndex)
        {
            var tableMetaData = await _dataGenerationService.GetTableMetadataAsync(tableName);

            if (tableMetaData == null || !tableMetaData.Any())
                return BadRequest($"No metadata found for the table {tableName}.");

            string outputPath = Path.Combine(Path.GetTempPath(), "GeneratedFiles", tableName);
            Directory.CreateDirectory(outputPath);

            try
            {
                // Create template data
                var templateData = new TemplateData
                {
                    TableName = tableName,
                    Namespace = NameSpace,
                    EntityName = tableName, 
                    PermIndex=PermIndex,
                    Columns = tableMetaData.Select(c => new ColumnMeta
                    {
                        column_name = c.ColumnName,
                        data_type = MapToCSharpType( c.DataType),
                        IsIdentity = c.IsIdentity
                    })
                };
                string templatePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Templates");
                Console.WriteLine(JsonConvert.SerializeObject(templateData.Columns, Newtonsoft.Json.Formatting.Indented));

                // Generate Controller
                string controllerContent = RenderTemplate(Path.Combine(templatePath, "ControllerTemplate.scriban"), templateData);
                WriteFile(outputPath, $"{tableName}Controller.cs", controllerContent);

                // Generate Models
                string modelContent = RenderTemplate(Path.Combine(templatePath, "ModelsTemplate.scriban"), templateData);
                WriteFile(outputPath, $"{tableName}Models.cs", modelContent);

                // Generate Views
                string indexViewContent = RenderTemplate(Path.Combine(templatePath, "IndexViewTemplate.scriban"), templateData);
                WriteFile(Path.Combine(outputPath, "Views", tableName), "Index.cshtml", indexViewContent);

                string addViewContent = RenderTemplate(Path.Combine(templatePath, "AddViewTemplate.scriban"), templateData);
                WriteFile(Path.Combine(outputPath, "Views", tableName), "Add.cshtml", addViewContent);

                string editViewContent = RenderTemplate(Path.Combine(templatePath, "EditViewTemplate.scriban"), templateData);
                WriteFile(Path.Combine(outputPath, "Views", tableName), "Edit.cshtml", editViewContent);

                // Package all files into a zip
                string zipPath = Path.Combine(Path.GetTempPath(), $"{tableName}_Files.zip");
                if (System.IO.File.Exists(zipPath)) System.IO.File.Delete(zipPath);

                System.IO.Compression.ZipFile.CreateFromDirectory(outputPath, zipPath);

                // Return the zip file as response
                var bytes = await System.IO.File.ReadAllBytesAsync(zipPath);
                return File(bytes, "application/zip", $"{tableName}_Files.zip");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error generating files for table '{tableName}': {ex.Message}");
            }
        }



        private string RenderTemplate(string templatePath, object data)
        {
            var template = System.IO.File.ReadAllText(templatePath);
            var parsedTemplate = Scriban.Template.Parse(template);
            return parsedTemplate.Render(data);
        }

        private void WriteFile(string outputPath, string fileName, string content)
        {
            Directory.CreateDirectory(outputPath);
            System.IO.File.WriteAllText(Path.Combine(outputPath, fileName), content);
        }


        private string MapToCSharpType(string sqlType)
        {
            return sqlType.ToLower() switch
            {
                "int" or "bigint" => "int",
                "nvarchar" or "varchar" or "text" => "string",
                "datetime" or "date" => "DateTime",
                "bit" => "bool",
                "float" => "float",
                "decimal" => "decimal",
                "uniqueidentifier" => "Guid",
                _ => "string"
            };
        }


    }
}
