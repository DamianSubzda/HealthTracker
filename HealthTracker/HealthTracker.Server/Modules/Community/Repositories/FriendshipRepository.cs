using HealthTracker.Server.Modules.Community.DTOs;
using HealthTracker.Server.Modules.Community.Models;
using Microsoft.EntityFrameworkCore;
using HealthTracker.Server.Infrastructure.Data;
using HealthTracker.Server.Core.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HealthTracker.Server.Core.Exceptions.Community;
using HealthTracker.Server.Core.Exceptions;

namespace HealthTracker.Server.Modules.Community.Repositories
{
    /// <summary>
    /// Status: Name
    /// RequestSend
    /// Accepted / Declined
    /// </summary>
    public interface IFriendRepository
    {
        Task<FriendshipDTO> CreateFriendshipRequest(CreateFriendshipDTO createFriendshipDTO);
        Task<List<FriendDTO>> GetFriendList(int userId);
        Task<List<FriendDTO>> GetFriendshipRequestsForUser(int userId);
        Task<FriendshipDTO> GetFriendship(int friendshipId);
        Task<FriendshipDTO> GetFriendshipByUsersId(int userId, int friendId);
        Task AcceptFriendship(int userId, int friendId);
        Task DeclineFriendship(int userId, int friendId);
        Task DeleteFriendship(int userId, int friendId);
    }

    public class FriendshipRepository : IFriendRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public FriendshipRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<FriendshipDTO> CreateFriendshipRequest(CreateFriendshipDTO createFriendshipDTO)
        {
            var user = await _context.User.AnyAsync(line => line.Id == createFriendshipDTO.UserId);
            var friend = await _context.User.AnyAsync(line => line.Id == createFriendshipDTO.FriendId);

            if (!user || !friend)
            {
                throw new UserNotFoundException(user ? createFriendshipDTO.UserId : createFriendshipDTO.FriendId);
            }

            bool friendshipExists = await _context.Friendship.AnyAsync(f =>
                (f.UserId == createFriendshipDTO.UserId && f.FriendId == createFriendshipDTO.FriendId) ||
                (f.UserId == createFriendshipDTO.FriendId && f.FriendId == createFriendshipDTO.UserId));

            if (friendshipExists)
            {
                throw new FriendshipAlreadyExistsException(); //Może rozdzielić na to aby wysyłać odpowiednie komunikaty. Żeby nie można było wysłać zaproszenia osobie która wcześniej cie zaprosiła
            }

            var friendship = _mapper.Map<Friendship>(createFriendshipDTO);
            friendship.Status = Status.Requested;

            await _context.Friendship.AddAsync(friendship);
            await _context.SaveChangesAsync();

            return _mapper.Map<FriendshipDTO>(friendship);
        }

        public async Task<FriendshipDTO> GetFriendship(int friendshipId)
        {
            var friendship = await _context.Friendship
                .Where(line => line.Id == friendshipId)
                .ProjectTo<FriendshipDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return friendship ?? throw new FriendshipNotFoundException(friendshipId);
        }

        public async Task<FriendshipDTO> GetFriendshipByUsersId(int userId, int friendId)
        {
            var user = await _context.User.AnyAsync(line => line.Id == userId);

            if (!user)
            {
                throw new UserNotFoundException(userId);
            }

            var friend = await _context.User.AnyAsync(line => line.Id == friendId);

            if (!friend)
            {
                throw new UserNotFoundException(friendId);
            }

            var friendship = await _context.Friendship
                .Where(f => (f.UserId == userId && f.FriendId == friendId) || (f.FriendId == userId && f.UserId == friendId))
                .ProjectTo<FriendshipDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return friendship ?? throw new FriendshipNotFoundException();
        }

        public async Task<List<FriendDTO>> GetFriendList(int userId)
        {
            var user = await _context.User.AnyAsync(line => line.Id == userId);

            if (!user)
            {
                throw new UserNotFoundException(userId);
            }

            var friends = await _context.Friendship
                .Where(f => f.UserId == userId)
                .Where(f => f.Status == Status.Accepted)
                .Select(u => new FriendDTO
                {
                    UserId = u.Friend.Id,
                    FirstName = u.Friend.FirstName,
                    LastName = u.Friend.LastName
                })
                .ToListAsync();

            return friends;
        }

        public async Task<List<FriendDTO>> GetFriendshipRequestsForUser(int userId)
        {
            var user = await _context.User.AnyAsync(u => u.Id == userId);

            if (!user)
            {
                throw new UserNotFoundException(userId);
            }

            var friendshipRequests = await _context.Friendship
                .Where(f => f.FriendId == userId && f.Status == Status.Requested)
                .Select(u => new FriendDTO
                {
                    UserId = u.User.Id,
                    FirstName = u.User.FirstName,
                    LastName = u.User.LastName
                })
                .ToListAsync();

            return friendshipRequests;

        }

        public async Task AcceptFriendship(int userId, int friendId)
        {
            var user = await _context.User.AnyAsync(line => line.Id == userId);
            var friend = await _context.User.AnyAsync(line => line.Id == friendId);

            if (!user || !friend)
            {
                throw new UserNotFoundException(user ? userId : friendId);
            }

            var friendship = await _context.Friendship
                .Where(f => (f.UserId == friendId && f.FriendId == userId)) //Chyba trzeba kolejnością zamienić i że 
                .Where(f => f.Status == Status.Requested)
                .FirstOrDefaultAsync();

            if (friendship == null)
            {
                throw new FriendshipNotFoundException();
            }

            friendship.Status = Status.Accepted;
            friendship.UpdatedAt = DateTime.UtcNow;

            var symmetricalFriendship = new Friendship
            {
                UserId = userId,    //UserId potwierdza requesta,
                FriendId = friendId,
                Status = Status.Accepted,
                CreatedAt = friendship.CreatedAt,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.AddAsync(symmetricalFriendship);
            await _context.SaveChangesAsync();
        }

        public async Task DeclineFriendship(int userId, int friendId)
        {
            var user = await _context.User.AnyAsync(line => line.Id == userId);
            var friend = await _context.User.AnyAsync(line => line.Id == friendId);

            if (!user || !friend)
            {
                throw new UserNotFoundException(user ? userId : friendId);
            }

            var friendship = await _context.Friendship
                .Where(f => (f.UserId == friendId && f.FriendId == userId))
                .Where(f => f.Status == Status.Requested)
                .FirstOrDefaultAsync();

            if (friendship == null)
            {
                throw new FriendshipNotFoundException();
            }

            friendship.Status = Status.Declined;
            friendship.UpdatedAt = DateTime.UtcNow;

            var symmetricalFriendship = new Friendship
            {
                UserId = userId,    //UserId potwierdza requesta,
                FriendId = friendId,
                Status = Status.Declined,
                CreatedAt = friendship.CreatedAt,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.AddAsync(symmetricalFriendship);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteFriendship(int userId, int friendId)
        {
            var friendships = await _context.Friendship
                .Where(f => (f.UserId == userId && f.FriendId == friendId) || (f.FriendId == userId && f.UserId == friendId))
                .ToListAsync();

            if (!friendships.Any())
            {
                throw new FriendshipNotFoundException();
            }

            _context.Friendship.RemoveRange(friendships);
            await _context.SaveChangesAsync();
        }

    }
}
