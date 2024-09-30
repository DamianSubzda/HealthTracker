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
        Task<UserDTO> GetUser(int userId);
        Task<List<UserSearchDTO>> GetUsers(int userId, string query);
        Task<string> SetPhotoUser(int userId, IFormFile photo);
    }
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;
        public UserRepository(ApplicationDbContext context, IMapper mapper, IFileService fileService)
        {
            _context = context;
            _mapper = mapper;
            _fileService = fileService;
        }

        public async Task<UserDTO> GetUser(int userId)
        {
            var userDTO = await _context.User
                .Where(u => u.Id == userId)
                .ProjectTo<UserDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync() ?? throw new UserNotFoundException(userId);

            return userDTO;
        }

        public async Task<string> SetPhotoUser(int userId, IFormFile photo)
        {
            var user = await _context.User
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new UserNotFoundException(userId);
            }

            if (user.ProfilePicture != null)
            {
                _fileService.DeleteFile(user.ProfilePicture);
            }

            user.ProfilePicture = _fileService.SaveFile(photo, "Core\\Assets\\ProfilePictures");

            await _context.SaveChangesAsync();

            return user.ProfilePicture;
        }

        public async Task<List<UserSearchDTO>> GetUsers(int userId, string query)
        {
            var queryParts = query.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(q => q.ToLower())
                .ToList();

            var userDTOQuery = _context.Users
                .Where(u => u.Id != userId && queryParts.Any(qp => u.FirstName.ToLower().Contains(qp)
                                                               || u.LastName.ToLower().Contains(qp)));

            var userDTO = await userDTOQuery
                .OrderBy(u => u.FirstName.ToLower().StartsWith(queryParts[0]) ? 0 : 1)
                .ThenBy(u => u.LastName.ToLower().StartsWith(queryParts[0]) ? 0 : 1)
                .ThenBy(u => u.FirstName.ToLower())
                .Take(10)
                .ProjectTo<UserSearchDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return userDTO;
        }


    }
}
