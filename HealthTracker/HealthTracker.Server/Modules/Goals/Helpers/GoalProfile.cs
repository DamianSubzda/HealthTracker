using AutoMapper;
using HealthTracker.Server.Modules.Goals.DTOs;
using HealthTracker.Server.Modules.Goals.Models;

namespace HealthTracker.Server.Modules.Goals.Helpers
{
    public class GoalProfile : Profile
    {
        public GoalProfile()
        {
            CreateMap<CreateGoalDTO, Goal>();
            CreateMap<Goal, GoalDTO>();

            CreateMap<CreateGoalTypeDTO, GoalType>();
            CreateMap<GoalType, GoalTypeDTO>();
        }
    }
}
