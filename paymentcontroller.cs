using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace project123.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentgatewayController : Controller
    {
        private readonly ApplicationDbContext _context;
        string merchantSecret = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX"; // Replace with your merchant secret

        public PaymentgatewayController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        // Endpoint to calculate hash for a payment request
        [HttpGet]
        public IActionResult CalculateHash(double price)
        {
            string merchantId = "123456"; // Replace with your merchant ID

            // Generate unique order ID
            string orderId = GenerateOrderId();

            double amount = price;
            string currency = "USD";
            string amountFormatted = amount.ToString("####0.00");

            string hashedSecret = ComputeMD5(merchantSecret);

            // Create hash using merchant details and payment info
            string hash = ComputeMD5(merchantId + orderId + amountFormatted + currency + hashedSecret);

            var response = new
            {
                OrderId = orderId,
                Hash = hash,
                Amount = amount
            };

            return Ok(response);
        }

        // Endpoint to handle payment notification from PayHere
        [HttpPost("notify")] 
        public IActionResult Notify([FromForm] PaymentNotification notification)
        {
            // Calculate the local md5 signature
            string localMd5sig = GenerateMd5Sig(
                notification.merchant_id,
                notification.order_id,
                notification.payhere_amount,
                notification.payhere_currency,
                notification.status_code,
                merchantSecret
            );

            if (int.TryParse(notification.status_code, out int statusCode))
            {
                // Validate the md5 signature and status code
                if (localMd5sig.Equals(notification.md5sig, StringComparison.OrdinalIgnoreCase) && notification.status_code == "2")
                {
                    var payment = _context.paymentgateway.FirstOrDefault(p => p.orderid == notification.order_id);
                    if (payment != null)
                    {
                        payment.status = statusCode;
                        _context.SaveChanges(); // Save changes to the database
                    }

                    return Ok();
                }
            }
            return BadRequest();
        }

      
        // Generate a unique order ID based on current date and time
        private string GenerateOrderId()
        {
            string orderId = DateTime.UtcNow.ToString("ffmmHHssddMMyy");
            return orderId;
        }

        // Compute MD5 hash of a string
        private string ComputeMD5(string s)
        {
            StringBuilder sb = new StringBuilder();
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashValue = md5.ComputeHash(Encoding.UTF8.GetBytes(s));
                foreach (byte b in hashValue)
                {
                    sb.Append($"{b:X2}");
                }
            }
            return sb.ToString();
        }

        // Generate MD5 signature for validation
        private string GenerateMd5Sig(string merchantId, string orderId, string amount, string currency, string statusCode, string merchantSecret)
        {
            string secretHash = GetMd5Hash(merchantSecret).ToUpper();
            string combinedString = merchantId + orderId + amount + currency + statusCode + secretHash;
            return GetMd5Hash(combinedString).ToUpper();
        }

        // Get MD5 hash of a string
        private string GetMd5Hash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }

                return sb.ToString();
            }
        }
    }

    // Model class to represent payment notification
    public class PaymentNotification
    {
        public string merchant_id { get; set; }
        public string order_id { get; set; }
        public string payhere_amount { get; set; }
        public string payhere_currency { get; set; }
        public string status_code { get; set; }
        public string md5sig { get; set; }
    }
}

