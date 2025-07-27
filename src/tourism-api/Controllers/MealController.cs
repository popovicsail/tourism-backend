using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using tourism_api.Domain;
using tourism_api.Repositories;

namespace tourism_api.Controllers;

[Route("api/restaurants/{restaurantId}/meals")]
[ApiController]
public class MealController : ControllerBase
{
    private readonly RestaurantRepository _restaurantRepo;
    private readonly MealRepository _mealRepo;

    public MealController(IConfiguration configuration)
    {
        _restaurantRepo = new RestaurantRepository(configuration);
        _mealRepo = new MealRepository(configuration);
    }

    [HttpGet()]
    public ActionResult<List<Meal>> GetMealsByStatus(int restaurantId, [FromQuery] string status = "u ponudi")
    {
        try
        {
            var restaurant = _restaurantRepo.GetById(restaurantId);
            if (restaurant == null)
                return NotFound($"Restoran {restaurantId} nije pronađen.");

            var meals = _mealRepo.GetByRestaurantIdAndStatus(restaurantId, status);
            return Ok(meals);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Greška pri dohvatu jela: {ex.Message}");
            return Problem("Greška na serveru prilikom dohvatanja jela.");
        }
    }


    [HttpPut()]
    public IActionResult ReplaceMenu(int restaurantId, [FromBody] List<Meal> newMenu)
    {
        if (newMenu == null || newMenu.Count == 0)
            return BadRequest("Novi jelovnik je prazan.");

        try
        {
            var restaurant = _restaurantRepo.GetById(restaurantId);
            if (restaurant == null)
                return NotFound($"Restoran {restaurantId} nije pronađen.");

            _mealRepo.ReplaceMenu(restaurantId, newMenu);
            return Ok("Jelovnik je uspešno zamenjen.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Greška u zameni jelovnika: {ex.Message}");
            return Problem("Greška na serveru pri zameni jelovnika.");
        }
    }


    [HttpPost]
    [Route("bulk")]
    public IActionResult CreateMany([FromBody] List<Meal> meals)
    {
        if (meals == null || meals.Count == 0)
            return BadRequest("Lista jela je prazna ili nije prosleđena.");

        try
        {
            _mealRepo.CreateMany(meals); // Poziv na bulk insert metodu iz repozitorijuma
            return Ok($"{meals.Count} jela uspešno dodato.");
        }
        catch (SqliteException ex)
        {
            Console.WriteLine($"Greška u SQLite unosu: {ex.Message}");
            return StatusCode(500, "Došlo je do greške prilikom čuvanja jela.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Neočekivana greška: {ex.Message}");
            return StatusCode(500, "Neočekivana greška na serveru.");
        }
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
            Restaurant restaurant = _restaurantRepo.GetById(restaurantId);
            if (restaurant == null)
            {
                return NotFound($"Restaurant with ID {restaurantId} not found.");
            }

            newMeal.RestaurantId = restaurantId;
            Meal createdMeal = _mealRepo.Create(newMeal);
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
            Restaurant restaurant = _restaurantRepo.GetById(restaurantId);
            if (restaurant == null)
            {
                return NotFound($"Restaurant with ID {restaurantId} not found.");
            }

            bool isDeleted = _mealRepo.Delete(id);
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

    [HttpDelete("DeleteActiveInRestaurant/{id}")]
    public ActionResult DeleteActiveInRestaurant(int restaurantId, int id)
    {
        try
        {
            Restaurant restaurant = _restaurantRepo.GetById(restaurantId);
            if (restaurant == null)
            {
                return NotFound($"Restaurant with ID {restaurantId} not found.");
            }

            bool isDeleted = _mealRepo.DeleteActiveInRestaurant(id);
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
