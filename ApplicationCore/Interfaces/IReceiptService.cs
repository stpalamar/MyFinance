using ApplicationCore.DTO;
using Infrastructure.Data.Models;

namespace ApplicationCore.Interfaces;

public interface IReceiptService
{
    ReceiptDto GetReceiptByTransactionId(Guid id);
    void AddReceiptToTransaction(ReceiptUploadDto receiptUploadDto, Guid transactionId);
    void DeleteReceiptFromTransaction(Guid transactionId);
}
