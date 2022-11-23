using ApplicationCore.DTO;

namespace ApplicationCore.Interfaces;

public interface IPaymentService
{
    public PaymentDto AddPayment(PaymentDto paymentDto);
}