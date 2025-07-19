using Microsoft.AspNetCore.Mvc;
using tourism_api.Domain;
using tourism_api.Repositories;

namespace tourism_api.Controllers;

[Route("api/tours")]
[ApiController]
public class TourController : ControllerBase
{
    private readonly TourReservationRepository _tourReservationRepo;
    private readonly TourRatingRepository _tourRatingRepo;
    private readonly TourRepository _tourRepo;
    private readonly UserRepository _userRepo;
    private readonly TourKeyPointRepository _keyPointRepo;

    public TourController(IConfiguration configuration)
    {
        _tourReservationRepo = new TourReservationRepository(configuration);
        _tourRatingRepo = new TourRatingRepository(configuration);
        _keyPointRepo = new TourKeyPointRepository(configuration);
        _tourRepo = new TourRepository(configuration);
        _userRepo = new UserRepository(configuration);
    }

    [HttpGet]
    public ActionResult GetPaged([FromQuery] int guideId = 0, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string orderBy = "Name", [FromQuery] string orderDirection = "ASC", [FromQuery] string tourStatus = null)
    {
        if (guideId > 0)
        {
            return Ok(_tourRepo.GetByGuideId(guideId));
        }

        List<string> validOrderByColumns = new List<string> { "Name", "Description", "DateTime", "MaxGuests" }; // Lista dozvoljenih kolona za sortiranje
        if (!validOrderByColumns.Contains(orderBy))
        {
            orderBy = "Name";
        }

        List<string> validOrderDirections = new List<string> { "ASC", "DESC" };
        if (!validOrderDirections.Contains(orderDirection))
        {
            orderDirection = "ASC";
        }

        List<string> validTourStatus = new List<string> { "Published", "Ready", "Not Ready" };
        if (!validTourStatus.Contains(tourStatus))
        {
            tourStatus = null;
        }

        try
        {
            List<Tour> tours = _tourRepo.GetPaged(page, pageSize, orderBy, orderDirection, tourStatus);
            int totalCount = _tourRepo.CountAll(tourStatus);
            Object result = new
            {
                data = tours,
                totalCount = totalCount
            };
            return Ok(result);
        }
        catch (Exception ex)
        {
            return Problem("An error occurred while fetching tours.");
        }
    }

    [HttpGet("{id}")]
    public ActionResult<Tour> GetTourById(int id)
    {
        try
        {
            Tour tour = _tourRepo.GetById(id);
            if (tour == null)
            {
                return NotFound($"Tour with ID {id} not found.");
            }
            return Ok(tour);
        }
        catch (Exception ex)
        {
            return Problem("An error occurred while fetching the tour.");
        }
    }

    [HttpGet("{tourId}/tour-reservations")]
    public ActionResult<List<TourReservation>> GetTourReservationsByTourId(int tourId)
    {
        try
        {
            List<TourReservation> tourReservations = _tourReservationRepo.GetByTourId(tourId);
            return tourReservations;
        }
        catch (Exception e)
        {
            return Problem("An unexpected error occurred while processing login request.");
        }
    }

    [HttpGet("{tourId}/tour-ratings")]
    public ActionResult<List<TourRating>> GetTourRatingsByTourId(int tourId)
    {
        try
        {
            ActionResult<List<TourRating>> tourRatings = _tourRatingRepo.GetByTourId(tourId);
            return tourRatings;
        }
        catch (Exception ex)
        {
            return Problem("An unexpected error occurred while processing login request.");
        }
    }

    [HttpGet("{tourId}/tour-key-points")]
    public ActionResult<List<TourKeyPoint>> GetTourKeyPointsByTourId(int tourId)
    {
        try
        {
            List<TourKeyPoint> tourKeyPoints = _keyPointRepo.GetByTourId(tourId);
            return tourKeyPoints;
        }
        catch (Exception ex)
        {
            return Problem("An unexpected error occurred while processing login request.");
        }
    }

    [HttpPost]
    public ActionResult<Tour> Create([FromBody] Tour newTour)
    {
        if (!newTour.IsValid())
        {
            return BadRequest("Invalid tour data.");
        }

        try
        {
            User user = _userRepo.GetById(newTour.GuideId);
            if (user == null)
            {
                return NotFound($"User with ID {newTour.GuideId} not found.");
            }

            Tour createdTour = _tourRepo.Create(newTour);
            return Ok(createdTour);
        }
        catch (Exception ex)
        {
            return Problem("An error occurred while creating the tour.");
        }
    }

    [HttpPut("{id}")]
    public ActionResult<Tour> Update(int id, [FromBody] Tour tour)
    {
        if (!tour.IsValid())
        {
            return BadRequest("Invalid tour data.");
        }

        try
        {
            tour.Id = id;
            Tour updatedTour = _tourRepo.Update(tour);
            if (updatedTour == null)
            {
                return NotFound();
            }
            return Ok(updatedTour);
        }
        catch (Exception ex)
        {
            return Problem("An error occurred while updating the tour.");
        }
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        try
        {
            bool isDeleted = _tourRepo.Delete(id);
            if (isDeleted)
            {
                return NoContent();
            }
            return NotFound($"Tour with ID {id} not found.");
        }
        catch (Exception ex)
        {
            return Problem("An error occurred while deleting the tour.");
        }
    }
}
