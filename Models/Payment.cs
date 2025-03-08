// create a Payment class with all the necessary properties such as Payment ID, Payment type, Amount, DateTime, Status

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Payment
    {
        public int PaymentID { get; set; }
        public string PaymentType { get; set; }
        public double Amount { get; set; }
        public DateTime DateTime { get; set; }
        public string Status { get; set; }
    }
}