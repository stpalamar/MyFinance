using ApplicationCore.DTO;

namespace ApplicationCore.Interfaces;

public interface IReceiptService
{
    void AddReceiptToTransaction(ReceiptDto receiptDto, Guid transactionId);
    // ReceiptDto GetReceiptByTransactionId(Guid id);
}