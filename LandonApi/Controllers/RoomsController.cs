using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LandonApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LandonApi.Controllers
{
    [Produces("application/json")]
    [Route("/[controller]")]
    public class RoomsController : Controller
    {
        private readonly HotelApiContext _context;

        public RoomsController(HotelApiContext context)
        {
            _context = context;
        }
        [HttpGet(Name =nameof(GetRooms))]
        public IActionResult GetRooms()
        {
            throw new NotImplementedException();
        }

        // /rooms/{roomId}
        [HttpGet("{roomId}", Name =nameof(GetRoomByIdAsync))]
        public async Task<IActionResult>GetRoomByIdAsync(Guid roomId, CancellationToken ct)
        {
            var entity = await _context.Rooms.SingleOrDefaultAsync(r => r.Id == roomId, ct);
            if (entity == null) return NotFound();
            var resource=new Room
            {
                Href =Url.Link(nameof(GetRoomByIdAsync), new { roomId=entity.Id}),
                Name=entity.Name,
                Rate=entity.Rate/100.0m
            };
            return Ok(resource);
        }
    }
}