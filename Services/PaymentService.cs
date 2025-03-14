using Models;

namespace ecommerce_api.Services
{
    public class PaymentService
    {
        private readonly List<Payment> _payments;

        public PaymentService()
        {
            _payments = new List<Payment>();
        }

        // Retrieve all payments
        public IEnumerable<Payment> GetPayments()
        {
            return _payments;
        }

        // Retrieve a payment by ID
        public Payment GetPaymentById(int id)
        {
            return _payments.FirstOrDefault(p => p.PaymentID == id);
        }

        // Create a new payment
        public Payment CreatePayment(Payment payment)
        {
            if (payment == null)
            {
                throw new ArgumentNullException(nameof(payment));
            }

            payment.PaymentID = _payments.Count > 0 ? _payments.Max(p => p.PaymentID) + 1 : 1;
            _payments.Add(payment);
            return payment;
        }

        // Update an existing payment
        public Payment UpdatePayment(int id, Payment updatedPayment)
        {
            var payment = _payments.FirstOrDefault(p => p.PaymentID == id);
            if (payment == null)
            {
                return null;
            }

            payment.PaymentType = updatedPayment.PaymentType;
            payment.Amount = updatedPayment.Amount;
            payment.DateTime = updatedPayment.DateTime;
            payment.Status = updatedPayment.Status;

            return payment;
        }

        // Delete a payment by ID
        public bool DeletePayment(int id)
        {
            var payment = _payments.FirstOrDefault(p => p.PaymentID == id);
            if (payment == null)
            {
                return false;
            }

            _payments.Remove(payment);
            return true;
        }
    }
}