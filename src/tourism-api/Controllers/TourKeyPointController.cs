using Microsoft.AspNetCore.Mvc;
using tourism_api.Domain;
using tourism_api.Repositories;

namespace tourism_api.Controllers;

[Route("api/tour-key-points")]
[ApiController]
public class TourKeyPointController : ControllerBase
{
    private readonly TourRepository _tourRepo;
    private readonly TourKeyPointRepository _tourKeyPointRepo;

    public TourKeyPointController(IConfiguration configuration)
    {
        _tourRepo = new TourRepository(configuration);
        _tourKeyPointRepo = new TourKeyPointRepository(configuration);
    }

    [HttpPost]
    public ActionResult<TourKeyPoint> Create([FromBody] TourKeyPoint newKeyPoint)
    {
        if (!newKeyPoint.IsValid())
        {
            return BadRequest("Invalid key point data.");
        }

        try
        {
            Tour tour = _tourRepo.GetById(newKeyPoint.TourId);
            if (tour == null)
            {
                return NotFound($"Tour with ID {newKeyPoint.TourId} not found.");
            }

            TourKeyPoint createdKeyPoint = _tourKeyPointRepo.Create(newKeyPoint);
            return Ok(createdKeyPoint);
        }
        catch (Exception ex)
        {
            return Problem("An error occurred while creating the key point.");
        }
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        try
        {
            bool isDeleted = _tourKeyPointRepo.Delete(id);
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

