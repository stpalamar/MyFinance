using ApplicationCore.DTO;

namespace ApplicationCore.Interfaces;

public interface IPaymentService
{
    public List<PaymentDto> GetPayments();
    public PaymentDto GetPaymentById(Guid id);
    public PaymentDto AddPayment(PaymentDto paymentDto);
    public PaymentDto UpdatePayment(PaymentDto paymentDto);
    public void DeletePayment(Guid id);
}