using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace stress_test.Controllers
{
    [Route("stress/[controller]")]
    [ApiController]
    public class MemoryTestController : ControllerBase
    {
        // GET stress/memorytest
        [HttpGet]
        public ActionResult<string> Get()
        {
            string myPassword = "Test the cpu stress use BCrypt ";

            for (int i = 0; i < 20; i++)
            {
                myPassword += myPassword + Guid.NewGuid().ToString();
            }

            return "Memory Test";
        }
    }
}