using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JustAProgrammer.AspNetCore.Auth.SslProxy.Pages
{
    public class IndexModel : PageModel
    {
        public Dictionary<string, string> Headers { get; set; }

        public IndexModel()
        {
        }

        public void OnGet()
        {
            Headers = new Dictionary<string, string>();
            foreach (var requestHeader in HttpContext.Request.Headers)
            {
                Headers.Add(requestHeader.Key, requestHeader.Value);
            }
        }


    }
}
