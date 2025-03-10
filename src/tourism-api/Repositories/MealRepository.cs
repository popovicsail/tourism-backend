using Microsoft.Data.Sqlite;
using tourism_api.Domain;

namespace tourism_api.Repositories;

public class MealRepository
{
    private readonly string connectionString;

    public MealRepository(IConfiguration configuration)
    {
        connectionString = configuration["ConnectionString:SQLiteConnection"];
    }

    public Meal Create(Meal meal)
    {
        try
        {
            using SqliteConnection connection = new SqliteConnection(connectionString);
            connection.Open();

            string query = @"
                    INSERT INTO Meals (OrderPosition, Name, Price, Ingredients, ImageUrl, RestaurantId)
                    VALUES (@Order, @Name, @Price, @Ingredients, @ImageUrl, @RestaurantId);
                    SELECT LAST_INSERT_ROWID();";
            using SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@Order", meal.Order);
            command.Parameters.AddWithValue("@Name", meal.Name);
            command.Parameters.AddWithValue("@Price", meal.Price);
            command.Parameters.AddWithValue("@Ingredients", meal.Ingredients);
            command.Parameters.AddWithValue("@ImageUrl", meal.ImageUrl ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@RestaurantId", meal.RestaurantId);

            meal.Id = Convert.ToInt32(command.ExecuteScalar());

            return meal;
        }
        catch (SqliteException ex)
        {
            Console.WriteLine($"Greška pri konekciji ili izvršavanju neispravnih SQL upita: {ex.Message}");
            throw;
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"Greška u konverziji podataka iz baze: {ex.Message}");
            throw;
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Konekcija nije otvorena ili je otvorena više puta: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Neočekivana greška: {ex.Message}");
            throw;
        }
    }

    public bool Delete(int id)
    {
        try
        {
            using SqliteConnection connection = new SqliteConnection(connectionString);
            connection.Open();

            string query = "DELETE FROM Meals WHERE Id = @Id";
            using SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            int rowsAffected = command.ExecuteNonQuery();

            return rowsAffected > 0;
        }
        catch (SqliteException ex)
        {
            Console.WriteLine($"Greška pri konekciji ili izvršavanju neispravnih SQL upita: {ex.Message}");
            throw;
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Konekcija nije otvorena ili je otvorena više puta: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Neočekivana greška: {ex.Message}");
            throw;
        }
    }
}
