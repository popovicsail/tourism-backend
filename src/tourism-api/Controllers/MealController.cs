using Microsoft.AspNetCore.Mvc;
using tourism_api.Domain;
using tourism_api.Repositories;

namespace tourism_api.Controllers;

[Route("api/restaurants/{restaurantId}/meals")]
[ApiController]
public class MealController : ControllerBase
{
    private readonly RestaurantRepository restaurantRepo;
    private readonly MealRepository mealRepo;

    public MealController(IConfiguration configuration)
    {
        restaurantRepo = new RestaurantRepository(configuration);
        mealRepo = new MealRepository(configuration);
    }

    [HttpPost]
    public ActionResult<Meal> Create(int restaurantId, [FromBody] Meal newMeal)
    {
        if (!newMeal.IsValid())
        {
            return BadRequest("Invalid meal data.");
        }

        try
        {
            Restaurant restaurant = restaurantRepo.GetById(restaurantId);
            if (restaurant == null)
            {
                return NotFound($"Restaurant with ID {restaurantId} not found.");
            }

            newMeal.RestaurantId = restaurantId;
            Meal createdMeal = mealRepo.Create(newMeal);
            return Ok(createdMeal);
        }
        catch (Exception ex)
        {
            return Problem("An error occurred while creating the meal.");
        }
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int restaurantId, int id)
    {
        try
        {
            Restaurant restaurant = restaurantRepo.GetById(restaurantId);
            if (restaurant == null)
            {
                return NotFound($"Restaurant with ID {restaurantId} not found.");
            }

            bool isDeleted = mealRepo.Delete(id);
            if (isDeleted)
            {
                return NoContent();
            }
            return NotFound($"Meal with ID {id} not found.");
        }
        catch (Exception ex)
        {
            return Problem("An error occurred while deleting the meal.");
        }
    }
}
