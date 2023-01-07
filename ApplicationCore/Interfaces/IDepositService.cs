using ApplicationCore.DTO;

namespace ApplicationCore.Interfaces;

public interface IDepositService
{
    List<DepositDto> GetDeposits();
    DepositDto GetDepositById(Guid id);
    DepositDto AddDeposit(DepositDto deposit);
    DepositDto UpdateDeposit(DepositDto deposit);
    void DeleteDeposit(Guid id);
}