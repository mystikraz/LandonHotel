using LandonApi.Infrastructure;
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
        private readonly IAuthorizationService _authzService;
        private readonly PagingOptions _defaultPagingOptions;

        public UsersController(
            IUserService userService,
            IAuthorizationService authzService,
            IOptions<PagingOptions> defaultPagingOptionsAccessor)
        {
            _userService = userService;
            _authzService = authzService;
            _defaultPagingOptions = defaultPagingOptionsAccessor.Value;
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

            var users = new PagedResults<User>();

            if (User.Identity.IsAuthenticated)
            {
                var canSeeEveryone = await _authzService
                    .AuthorizeAsync(User, "ViewAllUsersPolicy");
                if (canSeeEveryone)
                {
                    users = await _userService.GetUsersAsync(
                        pagingOptions, sortOptions, searchOptions, ct);
                }
                else
                {
                    var myself = await _userService.GetUserAsync(User);
                    users.Items = new[] { myself };
                    users.TotalSize = 1;
                }
            }

            var collection = PagedCollection<User>.Create<UsersResponse>(
                Link.To(nameof(GetVisibleUsersAsync)),
                users.Items?.ToArray() ?? new User[0],
                users.TotalSize,
                pagingOptions);

            collection.Me = Link.To(nameof(GetMeAsync));

            collection.Register = FormMetadata.FromModel(
                new RegisterForm(),
                Link.ToForm(nameof(RegisterUserAsync), relations: Form.CreateRelation));

            return Ok(collection);
        }

        [Authorize]
        [HttpGet("me", Name = nameof(GetMeAsync))]
        public async Task<IActionResult> GetMeAsync(CancellationToken ct)
        {
            if (User == null) return BadRequest();

            var user = await _userService.GetUserAsync(User);
            if (user == null) return NotFound();

            return Ok(user);
        }

        [HttpPost(Name = nameof(RegisterUserAsync))]
        public async Task<IActionResult> RegisterUserAsync(
            [FromBody] RegisterForm form,
            CancellationToken ct)
        {
            if (!ModelState.IsValid) return BadRequest(new ApiError(ModelState));

            var (succeeded, error) = await _userService.CreateUserAsync(form);
            if (succeeded) return Created(Url.Link(nameof(GetMeAsync), null), null);

            return BadRequest(new ApiError
            {
                Message = "Registration failed.",
                Detail = error
            });
        }

        [Authorize]
        [HttpGet("{userId}", Name = nameof(GetUserByIdAsync))]
        public async Task<IActionResult> GetUserByIdAsync(Guid userId, CancellationToken ct)
        {
            var currentUserId = await _userService.GetUserIdAsync(User);
            if (currentUserId == null) return NotFound();

            if (currentUserId == userId)
            {
                var myself = await _userService.GetUserAsync(User);
                return Ok(myself);
            }

            var canSeeEveryone = await _authzService.AuthorizeAsync(User, "ViewAllUsersPolicy");
            if (!canSeeEveryone) return NotFound();

            var user = await _userService.GetUserByIdAsync(userId, ct);
            if (user == null) return NotFound();

            return Ok(user);
        }
    }
}
