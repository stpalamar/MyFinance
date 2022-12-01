using ApplicationCore.DTO;

namespace ApplicationCore.Interfaces;

public interface ITransactionService
{
    List<TransactionDto> GetTransactions();
    TransactionDto GetTransactionById(Guid id);
    List<TransactionDto> GetTransactionsByAccountId(Guid accountId);
    TransactionDto AddTransaction(TransactionDto transaction);

    TransactionDto UpdateTransaction(TransactionDto transaction);

    void DeleteTransaction(Guid id);
    TransactionDto DuplicateTransaction(Guid id);
}