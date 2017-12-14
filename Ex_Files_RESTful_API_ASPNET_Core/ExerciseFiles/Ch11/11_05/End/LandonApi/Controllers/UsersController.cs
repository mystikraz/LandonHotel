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
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly PagingOptions _defaultPagingOptions;

        public UsersController(
            IUserService userService,
            IOptions<PagingOptions> defaultPagingOptions)
        {
            _userService = userService;
            _defaultPagingOptions = defaultPagingOptions.Value;
        }

        [HttpGet(Name = nameof(GetVisibleUsersAsync))]
        public async Task<IActionResult> GetVisibleUsersAsync(
            [FromQuery] PagingOptions pagingOptions,
            [FromQuery] SortOptions<User, UserEntity> sortOptions,
            [FromQuery] SearchOptions<User, UserEntity> searchOptions,
            CancellationToken ct)
        {
            if (!ModelState.IsValid) return BadRequest(new ApiError(ModelState));

            pagingOptions.Offset = pagingOptions.Offset ?? _defaultPagingOptions.Offset;
            pagingOptions.Limit = pagingOptions.Limit ?? _defaultPagingOptions.Limit;

            // TODO: Authorization check. Is the user an admin?

            // TODO: Return a collection of visible users
            var users = await _userService.GetUsersAsync(
                pagingOptions, sortOptions, searchOptions, ct);

            var collection = PagedCollection<User>.Create(
                Link.To(nameof(GetVisibleUsersAsync)),
                users.Items.ToArray(),
                users.TotalSize,
                pagingOptions);

            return Ok(collection);
        }

        [Authorize]
        [HttpGet("{userId}", Name = nameof(GetUserByIdAsync))]
        public Task<IActionResult> GetUserByIdAsync(Guid userId, CancellationToken ct)
        {
            // TODO is userId the current user's ID?
            // If so, return myself.
            // If not, only Admin roles should be able to view arbitrary users.
            throw new NotImplementedException();
        }
    }
}
