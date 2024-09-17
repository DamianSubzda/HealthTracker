using HealthTracker.Server.Core.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthTracker.Server.Modules.Community.Models
{
    public class Friendship
    {
        public Friendship() 
        { 
            CreatedAt = DateTime.UtcNow;
        }

        public int Id { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public int UserId { get; set; }
        [ForeignKey("FriendId")]
        public User Friend { get; set; }
        public int FriendId { get; set; }
        public Status Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public enum Status
    {
        Requested,  
        Accepted,   
        Declined
    }
}
