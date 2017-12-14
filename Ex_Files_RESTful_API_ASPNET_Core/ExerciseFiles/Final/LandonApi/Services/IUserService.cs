using LandonApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Claims;

namespace LandonApi.Services
{
    public interface IUserService
    {
        Task<PagedResults<User>> GetUsersAsync(
            PagingOptions pagingOptions,
            SortOptions<User, UserEntity> sortOptions,
            SearchOptions<User, UserEntity> searchOptions,
            CancellationToken ct);

        Task<(bool Succeeded, string Error)> CreateUserAsync(RegisterForm form);

        Task<Guid?> GetUserIdAsync(ClaimsPrincipal principal);

        Task<User> GetUserByIdAsync(Guid userId, CancellationToken ct);

        Task<User> GetUserAsync(ClaimsPrincipal user);
    }
}
