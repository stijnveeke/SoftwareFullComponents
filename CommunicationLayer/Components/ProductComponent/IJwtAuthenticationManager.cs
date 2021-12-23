using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductComponent
{
    public interface IJwtAuthenticationManager
    {
        public string Authenticate(HttpRequest request, string accessName);
    }
}
