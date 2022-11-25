using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class ImportExportController : ControllerBase
{
    private readonly IImportExportService _importExportService;

    public ImportExportController(IImportExportService importExportService)
    {
        _importExportService = importExportService;
    }

    [HttpPost]
    public IActionResult ImportTransactions([FromForm] IFormFile excelFile)
    {
        return Ok(_importExportService.ImportTransactions(excelFile));
    }
    
    [HttpGet]
    public IActionResult ExportTransactions()
    {
        var content = _importExportService.ExportTransactions();
        return File(
            content,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Transactions {DateTime.Now:dd/MM/yyyy HH:mm:ss}.xlsx");
    }

    [HttpGet("{id}")]
    public IActionResult ExportTransactions(Guid id)
    {
        var content = _importExportService.ExportTransactionsByAccountId(id);
        return File(
            content,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Transactions {DateTime.Now:dd/MM/yyyy HH:mm:ss}.xlsx");
    }
}