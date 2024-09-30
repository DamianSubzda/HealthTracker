using AutoMapper;
using HealthTracker.Server.Core.DTOs;
using HealthTracker.Server.Core.Exceptions;
using HealthTracker.Server.Core.Models;
using HealthTracker.Server.Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;

namespace HealthTracker.Server.Core.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(IUserRepository userRepository, ILogger<ProfileController> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        [HttpGet("users/{id}")]
        public async Task<ActionResult> GetUser(int id)
        {
            try
            {
                var result = await _userRepository.GetUser(id);
                return Ok(result);
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during the register process for user {id}.", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("users/{id}/search")]
        public async Task<ActionResult<List<UserSearchDTO>>> GetUsers(int id, [FromQuery] string query)
        {
            try
            {
                var result = await _userRepository.GetUsers(id, query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during the register process for user {id}.", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("users/{id}/photo")]
        public async Task<ActionResult> SetUserPhoto(int id, IFormFile photo)
        {
            try
            {
                var result = await _userRepository.SetPhotoUser(id, photo);
                return Ok(result);
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during the set photo process for user {id}.", id);
                return StatusCode(500, "Internal server error");
            }
        }


    }
}
