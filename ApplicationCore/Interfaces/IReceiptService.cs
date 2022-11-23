using ApplicationCore.DTO;
using Infrastructure.Data.Models;

namespace ApplicationCore.Interfaces;

public interface IReceiptService
{
    void AddReceiptToTransaction(ReceiptUploadDto receiptUploadDto, Guid transactionId);
    ReceiptDto GetReceiptByTransactionId(Guid id);
}