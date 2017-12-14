using LandonApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace LandonApi.Services
{
    public interface IBookingService
    {
        Task<Booking> GetBookingAsync(Guid bookingId, CancellationToken ct);

        Task<Guid> CreateBookingAsync(
            Guid userId,
            Guid roomId,
            DateTimeOffset startAt,
            DateTimeOffset endAt,
            CancellationToken ct);

        Task DeleteBookingAsync(Guid bookingId, CancellationToken ct);

        Task<PagedResults<Booking>> GetBookingsAsync(
            PagingOptions pagingOptions,
            SortOptions<Booking, BookingEntity> sortOptions,
            SearchOptions<Booking, BookingEntity> searchOptions,
            CancellationToken ct);

        Task<Booking> GetBookingForUserIdAsync(
            Guid bookingId,
            Guid userId,
            CancellationToken ct);

        Task<PagedResults<Booking>> GetBookingsForUserIdAsync(
            Guid userId,
            PagingOptions pagingOptions,
            SortOptions<Booking, BookingEntity> sortOptions,
            SearchOptions<Booking, BookingEntity> searchOptions,
            CancellationToken ct);
    }
}
