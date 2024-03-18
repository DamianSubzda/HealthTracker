﻿using HealthTracker.Server.Core.Models;
using HealthTracker.Server.Modules.Health.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthTracker.Server.Modules.Community.Models
{
    public class Message
    {
        public Message()
        {
            SendTime = DateTime.UtcNow;
        }
        public int Id { get; set; }
        [ForeignKey("UserIdTo")]
        public User UserTo { get; set; }
        [ForeignKey("UserIdFrom")]
        public User UserFrom { get; set; }
        public int UserIdTo { get; set; }
        public int UserIdFrom { get; set; }
        public string Text { get; set; }
        public DateTime SendTime { get; set; }
    }
}
