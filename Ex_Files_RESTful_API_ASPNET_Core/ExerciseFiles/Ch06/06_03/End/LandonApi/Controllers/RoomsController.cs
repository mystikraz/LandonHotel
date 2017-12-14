using LandonApi.Models;
using LandonApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LandonApi.Controllers
{
    [Route("/[controller]")]
    public class RoomsController : Controller
    {
        private readonly IRoomService _roomService;
        private readonly IOpeningService _openingService;

        public RoomsController(
            IRoomService roomService,
            IOpeningService openingService)
        {
            _roomService = roomService;
            _openingService = openingService;
        }

        [HttpGet(Name = nameof(GetRoomsAsync))]
        public async Task<IActionResult> GetRoomsAsync(CancellationToken ct)
        {
            var rooms = await _roomService.GetRoomsAsync(ct);

            var collection = new Collection<Room>
            {
                Self = Link.ToCollection(nameof(GetRoomsAsync)),
                Value = rooms.ToArray()
            };

            return Ok(collection);
        }

        // GET /rooms/openings
        [HttpGet("openings", Name = nameof(GetAllRoomOpeningsAsync))]
        public async Task<IActionResult> GetAllRoomOpeningsAsync(
            [FromQuery] PagingOptions pagingOptions,
            CancellationToken ct)
        {
            var openings = await _openingService.GetOpeningsAsync(pagingOptions, ct);

            var collection = new PagedCollection<Opening>()
            {
                Self = Link.ToCollection(nameof(GetAllRoomOpeningsAsync)),
                Value = openings.Items.ToArray(),
                Size = openings.TotalSize,
                Offset = pagingOptions.Offset.Value,
                Limit = pagingOptions.Limit.Value
            };

            return Ok(collection);
        }


        // GET /rooms/{roomId}
        [HttpGet("{roomId}", Name = nameof(GetRoomByIdAsync))]
        public async Task<IActionResult> GetRoomByIdAsync(Guid roomId, CancellationToken ct)
        {
            var room = await _roomService.GetRoomAsync(roomId, ct);
            if (room == null) return NotFound();

            return Ok(room);
        }
    }
}
