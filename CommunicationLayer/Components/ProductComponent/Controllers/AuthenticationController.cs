using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductComponent.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IJwtAuthenticationManager authenticationManager;
        public AuthenticationController(IJwtAuthenticationManager authenticationManager)
        {
            this.authenticationManager = authenticationManager;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] string accessName)
        {
            var token = this.authenticationManager.Authenticate(Request, accessName);
            if (token == null)
                return Unauthorized();
            return Ok(token);
        }
    }
}
