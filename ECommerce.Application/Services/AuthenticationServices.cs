using ECommerce.Application.Common;
using ECommerce.Application.Contracts;
using ECommerce.Application.DTO_s.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Services
{
    public class AuthenticationServices : IAuthenticationServices
    {
        private readonly IIdentityServices identityServices;
        private readonly ITokenServices tokenServices;

        public AuthenticationServices(IIdentityServices identityServices, ITokenServices tokenServices)
        {
            this.identityServices = identityServices;
            this.tokenServices = tokenServices;
        }


        public async Task<Result<UserDto>> LoginAsync(LoginDto loginDto, CancellationToken ct = default)
        {
            var userResult = await identityServices.FindByEmailAsync(loginDto.Email, ct);
            if (!userResult.IsSuccess)
            {
                return Result<UserDto>.Fail(userResult.Errors);
            }

            var PasswordCheck = await identityServices.CheckPasswordAsync(loginDto.Email, loginDto.Password, ct);
            if (!PasswordCheck.IsSuccess)
            {
                return Result<UserDto>.Fail(Error.UnAuthorized("Invalid Email or Password"));
            }

            var Roles = await identityServices.GetRolesAsync(userResult.data.Email);
            var Token = tokenServices.CreateToken(userResult.data.Id, userResult.data.Email, userResult.data.UserName, Roles.data);
            return Result<UserDto>.Ok(new UserDto()
            {
                Email = userResult.data.Email,
                Displayname = userResult.data.Displayname,
                Token = Token
            });
        }

        public async Task<Result<UserDto>> RegisterAsync(RegisterDto registerDto, CancellationToken ct = default)
        {
            var Result = await identityServices.CreateUserAsync(registerDto, ct);
            if (!Result.IsSuccess || Result.data is null)
                return Result<UserDto>.Fail(Result.Errors);

            var Roles = await identityServices.GetRolesAsync(Result.data.Email);
            var Token = tokenServices.CreateToken(Result.data.Id, Result.data.Email, Result.data.UserName, Roles.data);
            return Result<UserDto>.Ok(new UserDto()
            {
                Email = Result.data.Email,
                Displayname = Result.data.Displayname,
                Token = Token
            });
        }

        public async Task<Result<bool>> CheckEmailAsync(string email, CancellationToken ct = default)
        {
            return await identityServices.EmailExistsAsync(email, ct);
        }
        public Task<Result<AddressDto>> UpdateUserAddressAsync(AddressDto addressDto, string email, CancellationToken ct = default)
        {
            return identityServices.UpdateAddressAsync(email, addressDto, ct);
        }

        public async Task<Result<AddressDto>> GetUserAddressAync(string email, CancellationToken ct = default)
        {
            var result = await identityServices.GetAddressByEmailAsync(email, ct);

            if (!result.IsSuccess)
                return Result<AddressDto>.Fail(result.Errors);

            return Result<AddressDto>.Ok(result.data);
        }
        public async Task<Result<UserDto>> GetCurrentuser(string email, CancellationToken ct = default)
        {
            var UserResult = await identityServices.FindByEmailAsync(email, ct);
            if (!UserResult.IsSuccess)
                Result<UserDto>.Fail(UserResult.Errors);

            var user = UserResult.data;
            var roleResult = await identityServices.GetRolesAsync(user.Email);

            if (!roleResult.IsSuccess)
                Result<UserDto>.Fail(roleResult.Errors);

            var token = tokenServices.CreateToken(user.Id, user.Email, user.UserName, roleResult.data);

            return Result<UserDto>.Ok(new UserDto()
            {
                Email = user.Email,
                Displayname = user.Displayname,
                Token = token
            });
        }
    }
}
