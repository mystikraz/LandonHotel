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
    [ApiVersion("1.0")]
    public class RootController : Controller
    {
        [HttpGet(Name =nameof(GetRoot))]
        public IActionResult GetRoot()
        {
            var response = new
            {
                href=Url.Link(nameof(GetRoot),null),
                rooms=new {href=Url.Link(nameof(RoomsController.GetRooms),null)},
                info=new {href=Url.Link(nameof(InfoController.GetInfo),null)}
            };
            return Ok(response);
        }
    }
}