using ApplicationCore.DTO;

namespace ApplicationCore.Interfaces;

public interface ITransactionService
{
    List<TransactionDto> GetTransactions();
    TransactionDto GetTransactionById(Guid id);
    
    TransactionDto AddTransaction(TransactionDto transaction);
    
    TransactionDto UpdateTransaction(TransactionDto transaction);
    
    void DeleteTransaction(Guid id);
}