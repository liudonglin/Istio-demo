using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DevOne.Security.Cryptography.BCrypt;
using Microsoft.AspNetCore.Mvc;

namespace stress_test.Controllers
{
    [Route("stress/[controller]")]
    [ApiController]
    public class CpuTestController : ControllerBase
    {
        // GET stress/cputest
        [HttpGet]
        public ActionResult<string> Get()
        {
            string myPassword = "Test the cpu stress use BCrypt ";
            string mySalt = BCryptHelper.GenerateSalt();

            BCryptHelper.HashPassword(myPassword, mySalt);
            
            return BCryptHelper.HashPassword(myPassword, mySalt);
        }

        static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
