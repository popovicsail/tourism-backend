﻿using Microsoft.AspNetCore.Mvc;
using tourism_api.Domain;
using tourism_api.Repositories;

namespace tourism_api.Controllers;

[Route("api/users")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly TourReservationRepository _tourReservationRepo;
    private readonly TourRatingRepository _tourRatingRepo;
    private readonly UserRepository _userRepo;

    public UserController(IConfiguration configuration)
    {
        _tourReservationRepo = new TourReservationRepository(configuration);
        _tourRatingRepo = new TourRatingRepository(configuration);
        _userRepo = new UserRepository(configuration);
    }

    [HttpPost("login")]
    public ActionResult<User> Login([FromBody] User credentials)
    {
        if (!credentials.IsValid())
        {
            return BadRequest("Invalid data.");
        }
        try
        {
            User user = _userRepo.Get(credentials.Username, credentials.Password);
            if (user == null)
            {
                return NotFound("Invalid username or password.");
            }
            return Ok(user);
        }
        catch (Exception ex)
        {
            return Problem("An unexpected error occurred while processing login request.");
        }
    }

    [HttpGet("{userId}/tour-reservations")]
    public ActionResult<List<TourReservation>> GetTourReservationsByUserId(int userId)
    {
        try
        {
            List<TourReservation> tourReservations = _tourReservationRepo.GetByUserId(userId);
            return Ok(tourReservations);
        }
        catch (Exception e)
        {
            return Problem("An unexpected error occurred while processing login request.");
        }
    }

    [HttpGet("{userId}/tour-ratings")]
    public ActionResult<List<TourRating>> GetTourRatingsByUserId(int userId)
    {
        try
        {
            List<TourRating> tourRatings = _tourRatingRepo.GetByUserId(userId);
            return Ok(tourRatings);
        }
        catch (Exception ex)
        {
            return Problem("An unexpected error occurred while processing login request.");
        }
    }
}
