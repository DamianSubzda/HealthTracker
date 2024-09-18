using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthTracker.Server.Core.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize(Policy = "AdminOnly")]
    public class DashboardController
    {

    }
}
