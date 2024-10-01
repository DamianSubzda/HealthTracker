using AutoMapper;
using HealthTracker.Server.Core.DTOs;
using HealthTracker.Server.Core.Models;
using HealthTracker.Server.Core.Repositories;
using HealthTracker.Server.Infrastructure.Data;
using HealthTracker.Server.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace HealthTracker.Server.Tests
{
    public class BaseRepositoryTests : IDisposable
    {
        internal ApplicationDbContext _context;
        internal IMapper? _mapper;

        public BaseRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

            _context = new ApplicationDbContext(options);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}