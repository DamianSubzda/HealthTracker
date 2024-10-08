﻿using HealthTracker.Server.Core.Models;
using HealthTracker.Server.Modules.PhysicalActivity.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HealthTracker.Server.Modules.Goals.DTOs
{
    public class GoalTypeDTO
    {
        public int Id { get; set; }
        [MaxLength(255, ErrorMessage = "Must be 255 characters or less!")]
        public string Name { get; set; }

    }
}
