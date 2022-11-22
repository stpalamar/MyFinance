using ApplicationCore.DTO;
using Infrastructure.Data.Models;

namespace ApplicationCore.Interfaces;

public interface IReceiptService
{
    void AddReceiptToTransaction(ReceiptUploadDto receiptDto, Guid transactionId);
    Receipt GetReceiptByTransactionId(Guid id);
}