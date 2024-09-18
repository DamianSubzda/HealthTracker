using AutoMapper;
using HealthTracker.Server.Core.DTOs;
using HealthTracker.Server.Core.Exceptions;
using HealthTracker.Server.Core.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HealthTracker.Server.Core.Repositories
{
    public interface IAuthRepository
    {
        Task<AuthenticationProperties> ConfigureExternalAuthenticationProperties(string provider, string redirectUrl);
        Task<SuccessLoginDto> HandleGoogleLogin();
        Task<IdentityResult> RegisterUserAsync(RegisterDTO userDto);
        Task<IdentityResult> LoginAsync(LoginDto loginDto);
        Task<string> GenerateJwtToken(string emailUserName);
    }
    public class AuthRepository : IAuthRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public AuthRepository(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<AuthenticationProperties> ConfigureExternalAuthenticationProperties(string provider, string redirectUrl)
        {
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return properties;
        }

        public async Task<SuccessLoginDto> HandleGoogleLogin()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                throw new Exception("External server error.");
            }

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(info.Principal.FindFirstValue(ClaimTypes.Email) ?? "");
                var userDTO = _mapper.Map<SuccessLoginDto>(user);
                userDTO.Token = await GenerateJwtToken(user.Email);
                return userDTO;
            }
            else
            {
                var user = new User
                {
                    Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                    UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                    FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName) ?? "",
                    LastName = info.Principal.FindFirstValue(ClaimTypes.Surname) ?? "",
                    PhoneNumber = info.Principal.FindFirstValue(ClaimTypes.MobilePhone),
                    DateOfCreate = DateTime.UtcNow
                };

                if (DateTime.TryParse(info.Principal.FindFirstValue(ClaimTypes.DateOfBirth), out DateTime dob))
                {
                    user.DateOfBirth = dob;
                }

                var createUserResult = await _userManager.CreateAsync(user);
                if (createUserResult.Succeeded)
                {
                    await _userManager.AddLoginAsync(user, info);
                    await _userManager.AddToRoleAsync(user, "User");
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    var userDTO = _mapper.Map<SuccessLoginDto>(user);
                    userDTO.Token = await GenerateJwtToken(user.Email);

                    return userDTO;
                }
                else
                {
                    throw new Exception("Failed to create a user with external login.");
                }
            }
        }

        public async Task<IdentityResult> RegisterUserAsync(RegisterDTO userDto)
        {
            var userExists = await _userManager.FindByEmailAsync(userDto.Email);
            if (userExists != null)
            {
                return IdentityResult.Failed(new IdentityError { Code = "400", Description = $"E-mail '{userDto.Email}' is already taken." });
            }

            var user = new User
            {
                UserName = userDto.UserName,
                Email = userDto.Email,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                PhoneNumber = userDto.PhoneNumber,
                DateOfBirth = userDto.DateOfBirth,
                DateOfCreate = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, userDto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
            }

            return result;
        }

        public async Task<IdentityResult> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.EmailUserName) != null ?
                await _userManager.FindByNameAsync(loginDto.EmailUserName) : await _userManager.FindByEmailAsync(loginDto.EmailUserName);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Code = "404", Description = $"Username or Email doesn\'t exists." });
            }
            else if (!user.EmailConfirmed)
            {
                return IdentityResult.Failed(new IdentityError { Code = "405", Description = $"Email is not confirmed. Please confirm before login." });
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, loginDto.Password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return IdentityResult.Success;
            }
            else
            {
                return IdentityResult.Failed(new IdentityError { Code = "406", Description = $"User wrong credentials" });
            }
        }

        public async Task<string> GenerateJwtToken(string emailUserName)
        {
            var user = await _userManager.FindByNameAsync(emailUserName) ?? await _userManager.FindByEmailAsync(emailUserName);
            if (user == null)
            {
                throw new UserNotFoundException("User not found.");
            }

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim("name", $"{user.FirstName} {user.LastName}"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}
