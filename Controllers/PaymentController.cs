using Microsoft.AspNetCore.Mvc;
using Models;

namespace ecommerce_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private static List<Payment> payments = new List<Payment>();

        // GET: api/Payment
        [HttpGet]
        public IActionResult GetPayments()
        {
            return Ok(payments);
        }

        // GET: api/Payment/{id}
        /// <summary>
        /// Retrieves a payment by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the payment.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the payment details if found, 
        /// or a 404 Not Found response with an error message if the payment does not exist.
        /// </returns>
        [HttpGet("{id}")]
        public IActionResult GetPaymentById(int id)
        {
            var payment = payments.FirstOrDefault(p => p.PaymentID == id);
            if (payment == null)
            {
                return NotFound(new { Message = "Payment not found" });
            }
            return Ok(payment);
        }

        // POST: api/Payment
        [HttpPost]
        public IActionResult CreatePayment([FromBody] Payment payment)
        {
            if (payment == null)
            {
                return BadRequest(new { Message = "Invalid payment data" });
            }

            payment.PaymentID = payments.Count > 0 ? payments.Max(p => p.PaymentID) + 1 : 1;
            payments.Add(payment);
            return CreatedAtAction(nameof(GetPaymentById), new { id = payment.PaymentID }, payment);
        }

        // PUT: api/Payment/{id}
        [HttpPut("{id}")]
        public IActionResult UpdatePayment(int id, [FromBody] Payment updatedPayment)
        {
            var payment = payments.FirstOrDefault(p => p.PaymentID == id);
            if (payment == null)
            {
                return NotFound(new { Message = "Payment not found" });
            }

            payment.PaymentType = updatedPayment.PaymentType;
            payment.Amount = updatedPayment.Amount;
            payment.DateTime = updatedPayment.DateTime;
            payment.Status = updatedPayment.Status;

            return Ok(payment);
        }

        //add delete method to delete the record
        // DELETE: api/Payment/{id}
        [HttpDelete("{id}")]
        public IActionResult DeletePayment(int id)
        {
            var payment = payments.FirstOrDefault(p => p.PaymentID == id);
            if (payment == null)
            {
                return NotFound(new { Message = "Payment not found" });
            }

            payments.Remove(payment);
            return NoContent();
        }

    }
}