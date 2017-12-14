﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LandonApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;

namespace LandonApi.Services
{
    public class DefaultUserService : IUserService
    {
        private readonly UserManager<UserEntity> _userManager;

        public DefaultUserService(UserManager<UserEntity> userManager)
        {
            _userManager = userManager;
        }

        public async Task<(bool Succeeded, string Error)> CreateUserAsync(RegisterForm form)
        {
            var entity = new UserEntity
            {
                Email = form.Email,
                UserName = form.Email,
                FirstName = form.FirstName,
                LastName = form.LastName,
                CreatedAt = DateTimeOffset.UtcNow
            };

            var result = await _userManager.CreateAsync(entity, form.Password);
            if (!result.Succeeded)
            {
                var firstError = result.Errors.FirstOrDefault()?.Description;
                return (false, firstError);
            }

            return (true, null);
        }

        public async Task<PagedResults<User>> GetUsersAsync(
            PagingOptions pagingOptions,
            SortOptions<User, UserEntity> sortOptions,
            SearchOptions<User, UserEntity> searchOptions,
            CancellationToken ct)
        {
            IQueryable<UserEntity> query = _userManager.Users;
            query = searchOptions.Apply(query);
            query = sortOptions.Apply(query);

            var size = await query.CountAsync(ct);

            var items = await query
                .Skip(pagingOptions.Offset.Value)
                .Take(pagingOptions.Limit.Value)
                .ProjectTo<User>()
                .ToArrayAsync(ct);

            return new PagedResults<User>
            {
                Items = items,
                TotalSize = size
            };
        }
    }
}
