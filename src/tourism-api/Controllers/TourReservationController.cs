using Microsoft.AspNetCore.Mvc;
using tourism_api.Domain;
using tourism_api.Repositories;

namespace tourism_api.Controllers;

[Route("api/tour-reservations")]
[ApiController]
public class TourReservationController : Controller
{
    private readonly TourReservationRepository _reservationRepo;
    public TourReservationController(IConfiguration configuration)
    {
        _reservationRepo = new TourReservationRepository(configuration);
    }

    [HttpPost]
    public ActionResult<List<TourReservation>> Create([FromBody] TourReservation newReservation, [FromQuery] int reservationAmount = 1)
    {
        try
        {
            List<TourReservation> createdReservation = _reservationRepo.Create(newReservation, reservationAmount);
            return Ok(createdReservation);
        }
        catch (Exception ex)
        {
            return Problem("An error occurred while creating the tour.");
        }
    }

    [HttpDelete]
    public ActionResult Delete([FromQuery] int reservationId)
    {
        try
        {
            bool isDeleted = _reservationRepo.Delete(reservationId);
            if (isDeleted)
            {
                return NoContent();
            }
            return NotFound($"Reservation with ID {reservationId} not found.");
        }
        catch (Exception ex)
        {
            return Problem("An error occurred while deleting the reservation.");
        }
    }
}
