using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models;

namespace ecommerce_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private static readonly List<Payment> Payments = new List<Payment>();

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Payment>>> GetPayments()
        {
            return Ok(await Task.FromResult(Payments));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Payment>> GetPayment(int id)
        {
            var payment = Payments.FirstOrDefault(p => p.PaymentID == id);
            if (payment == null)
            {
                return NotFound();
            }
            return Ok(await Task.FromResult(payment));
        }

        [HttpPost]
        public async Task<ActionResult<Payment>> CreatePayment(Payment payment)
        {
            payment.PaymentID = Payments.Any() ? Payments.Max(p => p.PaymentID) + 1 : 1;
            Payments.Add(payment);
            return CreatedAtAction(nameof(GetPayment), new { id = payment.PaymentID }, await Task.FromResult(payment));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePayment(int id, Payment payment)
        {
            if (id != payment.PaymentID)
            {
                return BadRequest();
            }

            var existingPayment = Payments.FirstOrDefault(p => p.PaymentID == id);
            if (existingPayment == null)
            {
                return NotFound();
            }

            existingPayment.PaymentType = payment.PaymentType;
            existingPayment.Amount = payment.Amount;
            existingPayment.DateTime = payment.DateTime;
            existingPayment.Status = payment.Status;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var payment = Payments.FirstOrDefault(p => p.PaymentID == id);
            if (payment == null)
            {
                return NotFound();
            }

            Payments.Remove(payment);
            return NoContent();
        }
    }
}