using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LandonApi.Controllers
{
    [Produces("application/json")]
    //[Route("api/Root")]
    [Route("/")]
    public class RootController : Controller
    {
        [HttpGet(Name =nameof(GetRoot))]
        public IActionResult GetRoot()
        {
            var response = new
            {
                href=Url.Link(nameof(GetRoot),null)
            };
            return Ok(response);
        }
    }
}