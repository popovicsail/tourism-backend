using Microsoft.AspNetCore.Mvc;
using tourism_api.Domain;
using tourism_api.Repositories;

namespace tourism_api.Controllers;

[Route("api/comments")]
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
            return Problem("An error occurred while fetching the comment.");
        }
    }

    [HttpGet("{id}")]
    public ActionResult<List<TourRating>> GetById(int id, [FromQuery] string idType)
    {
        try
        {
            List<TourRating> tourRatings = _tourRatingRepo.GetById(id, idType);
            return Ok(tourRatings);
        }
        catch (Exception ex)
        {
            return Problem("An error occurred while fetching the comment.");
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

