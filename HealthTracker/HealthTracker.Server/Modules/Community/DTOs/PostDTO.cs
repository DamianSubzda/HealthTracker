﻿namespace HealthTracker.Server.Modules.Community.DTOs
{
    public class PostDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public required string Content { get; set; }
        public DateTime? DateOfCreate { get; set; }
    }
}
