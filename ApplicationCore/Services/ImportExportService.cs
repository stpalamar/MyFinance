using System.Security.Claims;
using ApplicationCore.DTO;
using ApplicationCore.Exceptions;
using ApplicationCore.Interfaces;
using ClosedXML.Excel;
using Infrastructure;
using Infrastructure.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Type = Infrastructure.Data.Models.Type;

namespace ApplicationCore.Services;

public class ImportExportService : IImportExportService
{
    private ApplicationDbContext _context;
    private IAccountService _accountService;
    private readonly User _user;

    public ImportExportService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, 
        IAccountService accountService)
    {
        _context = context;
        _accountService = accountService;
        var email = httpContextAccessor.HttpContext.User.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        _user = _context.Users.First(u => u.Email == email);
    }

    public AccountDto ImportTransactions(IFormFile excelFile)
    {
        using var workbook = new XLWorkbook(excelFile.OpenReadStream(), XLEventTracking.Disabled);
        var worksheet = workbook.Worksheet(1);
        var account = _context.Accounts.FirstOrDefault(a => a.User.Id == _user.Id && a.Name == worksheet.Name);
        if (account == null)
        {
            account = new Account
            {
                Name = worksheet.Name,
                User = _user
            };
            _context.Accounts.Add(account);
            _context.SaveChanges();
        }
        foreach (var row in worksheet.RowsUsed().Skip(1))
        {
            var transaction = new Transaction
            {
                Description = row.Cell(1).GetValue<string>(),
                Date = row.Cell(2).GetValue<DateTime>(),
                Type = Enum.Parse<Type>(row.Cell(3).GetValue<string>()),
                Amount = double.Parse(row.Cell(4).GetValue<string>().Replace(',' , '.')),
                Account = account
            };
            _context.Transactions.Add(transaction);
            _context.SaveChanges();
        }
        _accountService.CalculateAccountAmount(_context, _user);
        return (AccountDto)account;
    }

    public byte[] ExportTransactions()
    {
        var transactions = _context.Transactions
            .Include("Account")
            .Where(t => t.Account.User.Id == _user.Id)
            .ToList();
        return CreateExcel("Transactions", transactions, null);
    }
    
    public byte[] ExportTransactionsByAccountId(Guid id)
    {
        if (_context.Accounts
            .Where(a => a.User.Id == _user.Id && a.Id == id).IsNullOrEmpty())
        {
            throw new NotFoundAccountException();
        }

        var account = _context.Accounts
            .First(a => a.User.Id == _user.Id && a.Id == id);

        var transactions = _context.Transactions
            .Include("Account")
            .Where(t => t.Account.User.Id == _user.Id && t.Account.Id == id)
            .ToList();
        return CreateExcel(account.Name, transactions, account);
    }
    
    private byte[] CreateExcel(string worksheetName, List<Transaction> transactions, Account? account)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add(worksheetName);
        var currentRow = 1;
        worksheet.Cell(currentRow, 1).Value = "Description";
        worksheet.Cell(currentRow, 2).Value = "Date";
        worksheet.Cell(currentRow, 3).Value = "Type";
        worksheet.Cell(currentRow, 4).Value = "Amount";
        if (account == null) worksheet.Cell(currentRow, 5).Value = "AccountName";
        foreach (var transaction in transactions)
        {
            currentRow++;
            worksheet.Cell(currentRow, 1).Value = transaction.Description;
            worksheet.Cell(currentRow, 2).Value = transaction.Date;
            worksheet.Cell(currentRow, 3).Value = transaction.Type;
            worksheet.Cell(currentRow, 4).Value = transaction.Amount;
            if (account == null) worksheet.Cell(currentRow, 5).Value = transaction.Account.Name;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        var content = stream.ToArray();
        return content;
    }
}