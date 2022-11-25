using ApplicationCore.DTO;
using Microsoft.AspNetCore.Http;

namespace ApplicationCore.Interfaces;

public interface IImportExportService
{
    AccountDto ImportTransactions(IFormFile excelFile);
    byte[] ExportTransactions();
    byte[] ExportTransactionsByAccountId(Guid id);
}