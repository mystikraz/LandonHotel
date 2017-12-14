using LandonApi.Models;
using LandonApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LandonApi.Controllers
{
    [Route("/[controller]")]
    public class BookingsController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly IUserService _userService;
        private readonly IAuthorizationService _authzService;
        private readonly PagingOptions _defaultPagingOptions;

        public BookingsController(
            IBookingService bookingService,
            IUserService userService,
            IAuthorizationService authzService,
            IOptions<PagingOptions> defaultPagingOptionsAccessor)
        {
            _bookingService = bookingService;
            _userService = userService;
            _authzService = authzService;
            _defaultPagingOptions = defaultPagingOptionsAccessor.Value;
        }

        [Authorize]
        [HttpGet(Name = nameof(GetVisibleBookings))]
        public async Task<IActionResult> GetVisibleBookings(
            PagingOptions pagingOptions,
            SortOptions<Booking, BookingEntity> sortOptions,
            SearchOptions<Booking, BookingEntity> searchOptions,
            CancellationToken ct)
        {
            if (!ModelState.IsValid) return BadRequest(new ApiError(ModelState));

            pagingOptions.Offset = pagingOptions.Offset ?? _defaultPagingOptions.Offset;
            pagingOptions.Limit = pagingOptions.Limit ?? _defaultPagingOptions.Limit;

            var bookings = new PagedResults<Booking>();

            if (User.Identity.IsAuthenticated)
            {
                var userCanSeeAllBookings = await _authzService.AuthorizeAsync(User, "ViewAllBookingsPolicy");
                if (userCanSeeAllBookings)
                {
                    bookings = await _bookingService.GetBookingsAsync(
                        pagingOptions, sortOptions, searchOptions, ct);
                }
                else
                {
                    var userId = await _userService.GetUserIdAsync(User);
                    if (userId != null)
                    {
                        bookings = await _bookingService.GetBookingsForUserIdAsync(
                            userId.Value, pagingOptions, sortOptions, searchOptions, ct);
                    }
                }
            }

            var collectionLink = Link.ToCollection(nameof(GetVisibleBookings));
            var collection = PagedCollection<Booking>.Create(
                collectionLink,
                bookings.Items.ToArray(),
                bookings.TotalSize,
                pagingOptions);

            return Ok(collection);
        }

        [Authorize]
        [HttpGet("{bookingId}", Name = nameof(GetBookingByIdAsync))]
        public async Task<IActionResult> GetBookingByIdAsync(
            Guid bookingId,
            CancellationToken ct)
        {
            var userId = await _userService.GetUserIdAsync(User);
            if (userId == null) return NotFound();

            Booking booking = null;

            if (await _authzService.AuthorizeAsync(User, "ViewAllBookingsPolicy"))
            {
                booking = await _bookingService.GetBookingAsync(bookingId, ct);
            }
            else
            {
                booking = await _bookingService.GetBookingForUserIdAsync(bookingId, userId.Value, ct);
            }

            if (booking == null) return NotFound();

            return Ok(booking);

        }

        [Authorize]
        [HttpDelete("{bookingId}", Name = nameof(DeleteBookingByIdAsync))]
        public async Task<IActionResult> DeleteBookingByIdAsync(
            Guid bookingId,
            CancellationToken ct)
        {
            var userId = await _userService.GetUserIdAsync(User);
            if (userId == null) return NotFound();

            var booking = await _bookingService.GetBookingForUserIdAsync(bookingId, userId.Value, ct);
            if (booking != null)
            {
                await _bookingService.DeleteBookingAsync(bookingId, ct);
                return NoContent();
            }

            if (!await _authzService.AuthorizeAsync(User, "ViewAllBookingsPolicy"))
            {
                return NotFound();
            }

            booking = await _bookingService.GetBookingAsync(bookingId, ct);
            if (booking == null) return NotFound();

            await _bookingService.DeleteBookingAsync(bookingId, ct);
            return NoContent();
        }
    }
}
