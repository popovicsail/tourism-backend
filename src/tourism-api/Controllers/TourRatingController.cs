using Microsoft.AspNetCore.Mvc;
using tourism_api.Domain;
using tourism_api.Repositories;

namespace tourism_api.Controllers;

[Route("api/tour-ratings")]
[ApiController]
public class TourRatingController : ControllerBase
{
    private readonly TourRatingRepository _tourRatingRepo;

    public TourRatingController(IConfiguration configuration)
    {
        _tourRatingRepo = new TourRatingRepository(configuration);
    }

    [HttpGet]
    public ActionResult<List<TourRating>> GetAll()
    {
        try
        {
            List<TourRating> tourRatings = _tourRatingRepo.GetAll();
            return Ok(tourRatings);
        }
        catch (Exception ex)
        {
            return Problem("An error occurred while fetching the ratings.");
        }
    }

    [HttpGet("{id}")]
    public ActionResult<TourRating> GetById(int id)
    {
        try
        {
            TourRating tourRating = _tourRatingRepo.GetById(id);
            return Ok(tourRating);
        }
        catch (Exception ex)
        {
            return Problem("An error occurred while fetching the rating.");
        }
    }


    [HttpPost]
    public ActionResult<TourRating> Create([FromBody] TourRating tourRating)
    {
        if (tourRating.Comment == null)
        {
            tourRating.Comment = "";
        }

        try
        {
            TourRating createdTourRating = _tourRatingRepo.Create(tourRating);
            return Ok(createdTourRating);
        }
        catch (Exception ex)
        {
            return Problem("An error occurred while creating the comment.");
        }
    }
}

