using AutoMapper;
using HealthTracker.Server.Core.DTOs;
using HealthTracker.Server.Core.Models;
using HealthTracker.Server.Core.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;


namespace HealthTracker.Server.Core.Controllers
{
    [Route("")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthRepository authRepository, UserManager<User> userManager, SignInManager<User> signInManager, IMapper mapper, ILogger<AuthController> logger)
        {
            _authRepository = authRepository;
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var result = await _authRepository.LoginAsync(loginDto);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(loginDto.EmailUserName) ?? await _userManager.FindByEmailAsync(loginDto.EmailUserName);
                    var userDTO = _mapper.Map<SuccessLoginDto>(user);
                    userDTO.Token = await _authRepository.GenerateJwtToken(loginDto.EmailUserName);
                    return Ok(userDTO);
                }

                return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during the login process for user {EmailUserName}.", loginDto.EmailUserName);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerUserDto)
        {
            try
            {
                var result = await _authRepository.RegisterUserAsync(registerUserDto);

                if (result.Succeeded)
                {
                    return Ok(new { Message = "User registered successfully" });
                }

                return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during the register process for user {UserName}.", registerUserDto.UserName);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("login-google")]
        public async Task<IActionResult> LoginWithGoogleAsync()
        {
            string? redirectUrl = Url.Action(nameof(HandleGoogleLogin), "Auth");
            var properties = await _authRepository.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return new ChallengeResult("Google", properties);
        }

        [HttpGet("handle-google-login")]
        public async Task<IActionResult> HandleGoogleLogin()
        {
            try
            {
                var userDTO = await _authRepository.HandleGoogleLogin();

                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };

                var userJson = JsonConvert.SerializeObject(userDTO, settings);
                var frontendUrl = $"https://localhost:5174/login-success?user={Uri.EscapeDataString(userJson)}";
                return Redirect(frontendUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "Error occurred during the external google login process.");
            }
        }


    }
}
