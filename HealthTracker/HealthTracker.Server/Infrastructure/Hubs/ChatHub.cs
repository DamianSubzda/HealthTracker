using AutoMapper;
using HealthTracker.Server.Core.Models;
using HealthTracker.Server.Infrastructure.Data;
using HealthTracker.Server.Modules.Community.Controllers;
using HealthTracker.Server.Modules.Community.DTOs;
using HealthTracker.Server.Modules.Community.Models;
using HealthTracker.Server.Modules.Community.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthTracker.Server.Infrastructure.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatRepository _chatRepository;
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(IChatRepository chatRepository, ILogger<ChatHub> logger)
        {
            _chatRepository = chatRepository;
            _logger = logger;
        }
        public async Task SendMessageToUser(int userFrom, int userTo, string messageText)
        {
            var message = new CreateMessageDTO { Text = messageText, UserIdFrom = userFrom, UserIdTo = userTo };
            try
            {
                var result = await _chatRepository.CreateMessage(message);
                await Clients.User(userFrom.ToString()).SendAsync("ReceiveMessage", result.Id, userFrom, userTo, messageText);
                await Clients.User(userTo.ToString()).SendAsync("ReceiveMessage", result.Id, userFrom, userTo, messageText);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending a message. {message}", message);
            }

        }

    }


}
