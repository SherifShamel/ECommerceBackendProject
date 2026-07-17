using ECommerce.Application.Common;
using ECommerce.Application.Contracts;
using ECommerce.Application.DTO_s.Identity;
using ECommerce.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Text;

namespace ECommerce.Application.Services
{
    public class IdentityServices : IIdentityServices
    {
        private readonly UserManager<ApplicationUser> userManager;

        public IdentityServices(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<Result<IdentityUserResult>> FindByEmailAsync(string email, CancellationToken ct = default)
        {
            var User = await userManager.FindByEmailAsync(email);
            if (User is null)
            {
                return Result<IdentityUserResult>.Fail(Error.NotFound("User Not Found"));
            }
            else
            {
                return Result<IdentityUserResult>.Ok(new IdentityUserResult(User.Id, User.Email, User.UserName, User.DisplayName));
            }
        }

        public async Task<Result<bool>> CheckPasswordAsync(string email, string password, CancellationToken ct = default)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user is null)
                return Result<bool>.Fail(Error.NotFound("User Not Found"));

            var isValid = await userManager.CheckPasswordAsync(user, password);
            return Result<bool>.Ok(isValid);
        }

        public async Task<Result<IdentityUserResult>> CreateUserAsync(RegisterDto registerDto, CancellationToken ct = default)
        {
            var user = new ApplicationUser()
            {
                Email = registerDto.Email,
                UserName = registerDto.UserName,
                PhoneNumber = registerDto.PhoneNumber,
                DisplayName = registerDto.DisplayName
            };

            var Result = await userManager.CreateAsync(user, registerDto.password);

            if (!Result.Succeeded)
            {
                var Errors = Result.Errors.Select(e => new Error(e.Code, e.Description)).ToList();
                return Result<IdentityUserResult>.Fail(Errors);
            }
            return Result<IdentityUserResult>.Ok(new IdentityUserResult(user.Id, user.Email, user.UserName, user.DisplayName));


        }

        public async Task<Result<IEnumerable<string>>> GetRolesAsync(string email)
        {
            var User = await userManager.FindByEmailAsync(email);
            if (User is null)
                return Result<IEnumerable<string>>.Fail(Error.NotFound("User Not Found"));

            var Roles = await userManager.GetRolesAsync(User);

            return Result<IEnumerable<string>>.Ok(Roles);
        }

        public async Task<Result<AddressDto>> GetAddressByEmailAsync(string email, CancellationToken ct = default)
        {
            var user = await userManager.Users.Include(u => u.Address).FirstOrDefaultAsync(u => u.Email == email, ct);

            if (user is null)
                return Result<AddressDto>.Fail(Error.NotFound("User is not found"));

            if (user.Address is null) return Result<AddressDto>.Fail(Error.NotFound("Address is not found"));

            return Result<AddressDto>.Ok(new AddressDto()
            {
                FirstName = user.Address.FirstName,
                LastName = user.Address.LastName,
                Street = user.Address.Street,
                City = user.Address.City,
                Country = user.Address.Country
            });

        }

        public async Task<Result<AddressDto>> UpdateAddressAsync(string email, AddressDto addressDto, CancellationToken ct = default)
        {
            var user = await userManager.Users.Include(u => u.Address).FirstOrDefaultAsync(u => u.Email == email, ct);
            if (user is null)
                return Result<AddressDto>.Fail(Error.NotFound("User is not found"));
            if (user.Address is null)
            {
                user.Address = new Address()
                {
                    FirstName = addressDto.FirstName,
                    LastName = addressDto.LastName,
                    Street = addressDto.Street,
                    Country = addressDto.Country,
                    City = addressDto.City
                };
            }
            else
            {
                user.Address.FirstName = addressDto.FirstName;
                user.Address.LastName = addressDto.LastName;
                user.Address.Street = addressDto.Street;
                user.Address.Country = addressDto.Country;
                user.Address.City = addressDto.City;
            }

            var Result = await userManager.UpdateAsync(user);
            if (!Result.Succeeded)
                return Result<AddressDto>.Fail(Error.Failure("Could not update address"));

            return Result<AddressDto>.Ok(addressDto);
        }

        public async Task<Result<bool>> EmailExistsAsync(string email, CancellationToken ct = default)
        {
            return await userManager.FindByEmailAsync(email) is not null;
        }
    }
}
