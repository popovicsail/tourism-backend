using Microsoft.AspNetCore.Mvc;
using tourism_api.Domain;
using tourism_api.Repositories;

namespace tourism_api.Controllers;

[Route("api/tours/{tourId}/key-points")]
[ApiController]
public class KeyPointController : ControllerBase
{
    private readonly TourRepository _tourRepo;
    private readonly KeyPointRepository _keyPointRepo;

    public KeyPointController(IConfiguration configuration)
    {
        _tourRepo = new TourRepository(configuration);
        _keyPointRepo = new KeyPointRepository(configuration);
    }

    [HttpPost]
    public ActionResult<KeyPoint> Create(int tourId, [FromBody] KeyPoint newKeyPoint)
    {
        if (!newKeyPoint.IsValid())
        {
            return BadRequest("Invalid key point data.");
        }

        try
        {
            Tour tour = _tourRepo.GetById(tourId);
            if (tour == null)
            {
                return NotFound($"Tour with ID {tourId} not found.");
            }

            newKeyPoint.TourId = tourId;
            KeyPoint createdKeyPoint = _keyPointRepo.Create(newKeyPoint);
            return Ok(createdKeyPoint);
        }
        catch (Exception ex)
        {
            return Problem("An error occurred while creating the key point.");
        }
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int tourId, int id)
    {
        try
        {
            Tour tour = _tourRepo.GetById(tourId);
            if (tour == null)
            {
                return NotFound($"Tour with ID {tourId} not found.");
            }

            bool isDeleted = _keyPointRepo.Delete(id);
            if (isDeleted)
            {
                return NoContent();
            }
            return NotFound($"Key point with ID {id} not found.");
        }
        catch (Exception ex)
        {
            return Problem("An error occurred while deleting the key point.");
        }
    }
}

