using Microsoft.AspNetCore.Mvc;
using tourism_api.Domain;
using tourism_api.Repositories;

namespace tourism_api.Controllers;

[Route("api/reservations")]
[ApiController]
public class ReservationController : Controller
{
    private readonly TourRepository _tourRepo;
    private readonly UserRepository _userRepo;
    private readonly ReservationRepository _reservationRepo;
    public ReservationController(IConfiguration configuration)
    {
        _tourRepo = new TourRepository(configuration);
        _userRepo = new UserRepository(configuration);
        _reservationRepo = new ReservationRepository(configuration);
    }

    [HttpPost]
    public ActionResult<List<Reservation>> Create([FromBody] Reservation newReservation, [FromQuery] int reservationAmount = 1)
    {
        try
        {
            List<Reservation> createdReservation = _reservationRepo.Create(newReservation, reservationAmount);
            return Ok(createdReservation);
        }
        catch (Exception ex)
        {
            return Problem("An error occurred while creating the tour.");
        }
    }

    [HttpDelete]
    public ActionResult Delete([FromQuery] int tourId)
    {
        try
        {
            bool isDeleted = _reservationRepo.Delete(tourId);
            if (isDeleted)
            {
                return NoContent();
            }
            return NotFound($"Reservation with ID {tourId} not found.");
        }
        catch (Exception ex)
        {
            return Problem("An error occurred while deleting the reservation.");
        }
    }
}
