using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using tourism_api.Domain;
using tourism_api.Repositories;

namespace tourism_api.Controllers
{
    [ApiController] // Add this attribute to enable controller features like BadRequest
    [Route("api/restaurantReservetion")] // Define the route for the controller
    public class RestaurantReservationController : ControllerBase // Inherit from ControllerBase to use BadRequest
    {
        private readonly RestaurantReservationRepository _RestaurantReservationRepo;
        private readonly RestaurantRepository _restaurantRepo;

        public RestaurantReservationController(IConfiguration configuration)
        {
            _restaurantRepo = new RestaurantRepository(configuration);
            _RestaurantReservationRepo = new RestaurantReservationRepository(configuration);
        }

        [HttpPost("{restaurantId}")]
        public ActionResult<Reservation> CreateReservation(int restaurantId, [FromBody] restaurantReservation reservation)
        {
            // Validate input
            if (reservation == null || reservation.NumberOfPeople <= 0 || string.IsNullOrEmpty(reservation.Meal) || reservation.Date == default)
                return BadRequest("Invalid reservation data.");

            // Load restaurant and check capacity
            var restaurant = _restaurantRepo.GetById(restaurantId);
            if (restaurant == null)
                return NotFound("Restaurant not found.");

            var existing = _RestaurantReservationRepo.GetByRestaurantDateMeal(restaurantId, reservation.Date.Date, reservation.Meal);
            int reserved = existing.Sum(r => r.NumberOfPeople);

            int available = restaurant.Capacity - reserved;
            if (reservation.NumberOfPeople > available)
                return BadRequest($"Only {available} seats available for the selected date and meal.");

            // Save reservation
            reservation.RestaurantId = restaurantId;
            var created = _RestaurantReservationRepo.Create(reservation);
            return Ok(created);
        }

        [HttpGet]
        public ActionResult<List<restaurantReservation>> GetReservations([FromQuery] int touristId)
        {
            var reservations = _RestaurantReservationRepo.GetByTourist(touristId);
            return Ok(reservations);
        }

        [HttpDelete("{reservationId}")]
        public ActionResult CancelReservation(int reservationId)
        {
            var reservation = _RestaurantReservationRepo.GetById(reservationId);
            if (reservation == null)
                return NotFound("Reservation not found.");

            // Izračunaj tačno vreme obroka
            DateTime reservationTime = reservation.Date.Date;
            switch (reservation.Meal)
            {
                case "Doručak": reservationTime = reservationTime.AddHours(8); break;
                case "Ručak": reservationTime = reservationTime.AddHours(13); break;
                case "Večera": reservationTime = reservationTime.AddHours(18); break;
                default: return BadRequest("Invalid meal type.");
            }

            var now = DateTime.Now;

            // Ako je rezervacija već prošla, dozvoli brisanje bez ograničenja
            if (now > reservationTime)
            {
                _RestaurantReservationRepo.Delete(reservationId);
                return Ok("Reservation deleted (past event).");
            }

            // Inače primeni pravila otkazivanja
            var diff = reservationTime - now;
            if (reservation.Meal == "Doručak" && diff.TotalHours < 12)
                return BadRequest("Breakfast reservations can only be canceled at least 12 hours in advance.");
            if ((reservation.Meal == "Ručak" || reservation.Meal == "Večera") && diff.TotalHours < 4)
                return BadRequest("Lunch and dinner reservations can only be canceled at least 4 hours in advance.");

            _RestaurantReservationRepo.Delete(reservationId);
            return Ok("Reservation canceled.");
        }

    }
}
