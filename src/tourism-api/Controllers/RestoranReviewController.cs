using Microsoft.AspNetCore.Mvc;
using tourism_api.Domain;
using tourism_api.Repositories;
using Microsoft.Extensions.Configuration;

namespace tourism_api.Controllers
{
    [Route("api/restaurants/review")]
    [ApiController]
    // This controller handles restaurant reviews, including fetching reviews by restaurant ID
    public class RestoranReviewController : ControllerBase
    {
        private readonly RestaurantRepository _restaurantRepo;
        private readonly UserRepository _userRepo;
        private readonly RestoranReviewRepository _reviewRepo;
        private readonly RestaurantReservationRepository _reservationRepo;

        public RestoranReviewController(IConfiguration configuration)
        {
            _restaurantRepo = new RestaurantRepository(configuration);
            _userRepo = new UserRepository(configuration);
            _reviewRepo = new RestoranReviewRepository(configuration);
            _reservationRepo = new RestaurantReservationRepository(configuration);
        }

        // Get reviews by restaurant ID
        [HttpGet("{restaurantId}")]
        public ActionResult<List<RestoranReview>> GetByRestaurantId(int restaurantId)
        {
            try
            {
                var reviews = _reviewRepo.GetByRestaurantId(restaurantId);
                if (reviews == null || reviews.Count == 0)
                {
                    return NotFound("No reviews found for this restaurant.");
                }
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return Problem("An error occurred while fetching the reviews: " + ex.Message);
            }
        }

        // Add a new review
        [HttpPost]
        public ActionResult<RestoranReview> CreateReview([FromBody] RestoranReview review)
        {
            try
            {
                if (review == null || !review.IsValid())
                {
                    return BadRequest("Invalid review data. Please check rating, text, and IDs.");
                }

                if (!CanRateBasedOnLastReservation(review.RestoranId, review.UserId))
                {
                    return StatusCode(StatusCodes.Status403Forbidden,
                        "Restoran možete oceniti najranije sat vremena nakon poslednje rezervacije i najkasnije tri dana nakon posete.");
                }

                var createdReview = _reviewRepo.Add(review);

                return Ok(createdReview);
            }
            catch (Exception ex)
            {
                return Problem("An error occurred while submitting the review.");
            }
        }

        // Delete a review by ID
        [HttpDelete("{reviewId}")]
        public ActionResult DeleteReview(int reviewId)
        {
            try
            {
                RestoranReview review = _reviewRepo.GetById(reviewId);
                if (review == null)
                {
                    return NotFound("Review not found.");
                }
                _reviewRepo.Delete(reviewId);
                return NoContent(); // 204 No Content
            }
            catch (Exception ex)
            {
                return Problem("An error occurred while deleting the review: " + ex.Message);
            }
        }


        private bool CanRateBasedOnLastReservation(int restaurantId, int touristId)
        {
            List<restaurantReservation> reservations = _reservationRepo.GetByTourist(touristId);

            restaurantReservation lastReservation = reservations
                .Where(r => r.RestaurantId == restaurantId)
                .OrderByDescending(r => r.Date)
                .FirstOrDefault();

            if (lastReservation == null)
                return false;

            DateTime now = DateTime.UtcNow;
            DateTime oneHourAfter = lastReservation.Date.AddHours(1);
            DateTime threeDaysAfter = lastReservation.Date.AddDays(3);

            return now >= oneHourAfter && now <= threeDaysAfter;
        }


    }
}
