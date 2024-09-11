using AutoMapper;
using AutoMapper.QueryableExtensions;
using HealthTracker.Server.Core.DTOs;
using HealthTracker.Server.Core.Exceptions;
using HealthTracker.Server.Core.Models;
using HealthTracker.Server.Infrastructure.Data;
using HealthTracker.Server.Infrastructure.Services;
using HealthTracker.Server.Modules.Community.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
namespace HealthTracker.Server.Core.Repositories
{
    public interface IUserRepository
    {
        Task<UserDTO> GetUser(int id);
        Task<List<UserSerachDTO>> GetUsers(int id, string input);
        Task<string> SetPhotoUser(int id, IFormFile photo);
    }
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;
        public UserRepository(ApplicationDbContext context, IMapper mapper, IWebHostEnvironment environment)
        {
            _context = context;
            _mapper = mapper;
            _environment = environment;
        }

        public async Task<UserDTO> GetUser(int id)
        {
            var userDTO = await _context.User
                .Where(u => u.Id == id)
                .ProjectTo<UserDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync() ?? throw new UserNotFoundException(id);

            return userDTO;
        }

        public async Task<string> SetPhotoUser(int id, IFormFile photo)
        {
            var user = await _context.User
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                throw new UserNotFoundException(id);
            }

            var fileName = $"user_{id}.png";
            var folderPath = Path.Combine("Core/Assets/ProfilePictures", fileName);
            var fullFilePath = Path.Combine(Directory.GetCurrentDirectory(), folderPath);

            using (var fileStream = new FileStream(fullFilePath, FileMode.Create))
            {
                await photo.CopyToAsync(fileStream);
            }

            user.ProfilePicture = folderPath;

            await _context.SaveChangesAsync();

            return folderPath;
        }

        public async Task<List<UserSerachDTO>> GetUsers(int id, string input)
        {
            var userDTO = await _context.User
                .Where(u => u.Id != id && (u.FirstName.ToLower().Contains(input.ToLower()) || u.LastName.ToLower().Contains(input.ToLower())))
                .Take(10)
                .ProjectTo<UserSerachDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return userDTO;
        }

    }
}
