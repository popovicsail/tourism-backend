using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tourism_api.Repositories;
using tourism_api.Service;
using tourism_api.Domain;

namespace tourism_api.Controllers
{
    [Route("api/restaurants/statistics")]
    [ApiController]
    public class RestaurantStatistcsController : ControllerBase
    {
        private readonly RestaurantStatisticsService restaurantStatisticsService;

        public RestaurantStatistcsController(IConfiguration configuration)
        {
            restaurantStatisticsService = new RestaurantStatisticsService(configuration);
        }

        [HttpGet("total-reservations")]

        public ActionResult<List<RestoranStatistics>> GetTotalReservationsForYear([FromQuery] int ownerId)
        {
            if (ownerId <= 0)
            {
                return BadRequest("Invalid owner ID.");
            }
            try
            {
                List<RestoranStatistics> statistics = restaurantStatisticsService.GetTotalResservationsForYear(ownerId);
                if (statistics == null || !statistics.Any())
                {
                    return NotFound("No reservations found for the specified owner.");
                }
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return Problem("An unexpected error occurred while processing the request: " + ex.Message);
            }
        }

        [HttpGet("occupancy-by-month/{restaurantId}")]
        public ActionResult<MonthlyOccupancyStatistic> GetOccupancyByMonth(int restaurantId)
        {
            if (restaurantId <= 0)
            {
                return BadRequest("Invalid restaurant ID.");
            }
            try
            {
                MonthlyOccupancyStatistic occupancyStatistic = restaurantStatisticsService.GetOccupancyByMonth(restaurantId);
                if (occupancyStatistic == null)
                {
                    return NotFound("No occupancy data found for the specified restaurant.");
                }
                return Ok(occupancyStatistic);
            }
            catch (Exception ex)
            {
                return Problem("An unexpected error occurred while processing the request: " + ex.Message);
            }
        }
    }
}
