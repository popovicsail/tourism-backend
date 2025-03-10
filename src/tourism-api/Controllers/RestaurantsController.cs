using Microsoft.AspNetCore.Mvc;
using tourism_api.Domain;
using tourism_api.Repositories;

namespace tourism_api.Controllers;

[Route("api/restaurants")]
[ApiController]
public class RestaurantController : ControllerBase
{
    private readonly RestaurantRepository restaurantRepo;

    public RestaurantController(IConfiguration configuration)
    {
        restaurantRepo = new RestaurantRepository(configuration);
    }

    [HttpGet]
    public ActionResult<List<Restaurant>> GetPaged([FromQuery] int ownerId = 0, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string orderBy = "Name", [FromQuery] string orderDirection = "ASC")
    {
        if(ownerId > 0)
        {
            return Ok(restaurantRepo.GetByOwner(ownerId));
        }

        // Validacija za orderBy i orderDirection
        List<string> validOrderByColumns = new List<string> { "Name", "Description", "Capacity" }; // Lista dozvoljenih kolona za sortiranje
        if (!validOrderByColumns.Contains(orderBy))
        {
            orderBy = "Name"; // Default vrednost
        }

        List<string> validOrderDirections = new List<string> { "ASC", "DESC" }; // Lista dozvoljenih smerova
        if (!validOrderDirections.Contains(orderDirection))
        {
            orderDirection = "ASC"; // Default vrednost
        }

        try
        {
            List<Restaurant> restaurants = restaurantRepo.GetPaged(page, pageSize, orderBy, orderDirection);
            int totalCount = restaurantRepo.CountAll();
            Object result = new
            {
                Data = restaurants,
                TotalCount = totalCount
            };
            return Ok(result);
        }
        catch (Exception ex)
        {
            return Problem("An error occurred while fetching the restaurants.");
        }
    }

    [HttpGet("{id}")]
    public ActionResult<Restaurant> GetById(int id)
    {
        try
        {
            Restaurant restaurant = restaurantRepo.GetById(id);
            if (restaurant == null)
            {
                return NotFound($"Restaurant with ID {id} not found.");
            }
            return Ok(restaurant);
        }
        catch (Exception ex)
        {
            return Problem("An error occurred while fetching the restaurant.");
        }
    }

    [HttpPost]
    public ActionResult<Restaurant> Create([FromBody] Restaurant newRestaurant)
    {
        if (!newRestaurant.IsValid())
        {
            return BadRequest("Invalid restaurant data.");
        }

        try
        {
            Restaurant createdRestaurant = restaurantRepo.Create(newRestaurant);
            return Ok(createdRestaurant);
        }
        catch (Exception ex)
        {
            return Problem("An error occurred while creating the restaurant.");
        }
    }

    [HttpPut("{id}")]
    public ActionResult<Restaurant> Update(int id, [FromBody] Restaurant restaurant)
    {
        if (!restaurant.IsValid())
        {
            return BadRequest("Invalid restaurant data.");
        }

        try
        {
            restaurant.Id = id;
            Restaurant updatedRestaurant = restaurantRepo.Update(restaurant);
            if (updatedRestaurant == null)
            {
                return NotFound($"Restaurant with ID {id} not found.");
            }
            return Ok(updatedRestaurant);
        }
        catch (Exception ex)
        {
            return Problem("An error occurred while updating the restaurant.");
        }
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        try
        {
            bool isDeleted = restaurantRepo.Delete(id);
            if (isDeleted)
            {
                return NoContent();
            }
            return NotFound($"Restaurant with ID {id} not found.");
        }
        catch (Exception ex)
        {
            return Problem("An error occurred while deleting the restaurant.");
        }
    }
}

