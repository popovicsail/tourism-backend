using Microsoft.Data.Sqlite;
using tourism_api.Domain;

namespace tourism_api.Repositories;

public class MealRepository
{
    private readonly string _connectionString;

    public MealRepository(IConfiguration configuration)
    {
        _connectionString = configuration["ConnectionString:SQLiteConnection"];
    }
    public void ReplaceMenu(int restaurantId, List<Meal> newMenu)
    {
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();

            // 1️ Brisanje starih jela iz jelovnika
            string deleteQuery = @"
            DELETE FROM Meals
            WHERE RestaurantId = @RestaurantId AND Status = 'u ponudi';";
            using var deleteCmd = new SqliteCommand(deleteQuery, connection, transaction);
            deleteCmd.Parameters.AddWithValue("@RestaurantId", restaurantId);
            deleteCmd.ExecuteNonQuery();

            // 2️ Ubacivanje novih jela
            string insertQuery = @"
            INSERT INTO Meals (OrderPosition, Name, Price, Ingredients, ImageUrl, Status, RestaurantId)
            VALUES (@Order, @Name, @Price, @Ingredients, @ImageUrl, @Status, @RestaurantId);";

            using var insertCmd = new SqliteCommand(insertQuery, connection, transaction);
            foreach (var meal in newMenu)
            {
                insertCmd.Parameters.Clear();
                insertCmd.Parameters.AddWithValue("@Order", meal.Order);
                insertCmd.Parameters.AddWithValue("@Name", meal.Name);
                insertCmd.Parameters.AddWithValue("@Price", meal.Price);
                insertCmd.Parameters.AddWithValue("@Ingredients", meal.Ingredients);
                insertCmd.Parameters.AddWithValue("@ImageUrl", meal.ImageUrl ?? (object)DBNull.Value);
                insertCmd.Parameters.AddWithValue("@Status", meal.Status ?? "u ponudi");
                insertCmd.Parameters.AddWithValue("@RestaurantId", restaurantId);

                insertCmd.ExecuteNonQuery();
            }

            transaction.Commit();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Greška u ReplaceMenu: {ex.Message}");
            throw;
        }
    }


    public List<Meal> GetByRestaurantIdAndStatus(int restaurantId, string status)
    {
        var meals = new List<Meal>();

        try
        {
            using SqliteConnection connection = new SqliteConnection(_connectionString);
            connection.Open();

            string query = @"
            SELECT Id, OrderPosition, Name, Price, Ingredients, ImageUrl, RestaurantId, Status
            FROM Meals
            WHERE RestaurantId = @RestaurantId AND Status = @Status;";

            using SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@RestaurantId", restaurantId);
            command.Parameters.AddWithValue("@Status", status);

            using SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                var meal = new Meal
                {
                    Id = reader.GetInt32(0),
                    Order = reader.GetInt32(1),
                    Name = reader.GetString(2),
                    Price = reader.GetDecimal(3),
                    Ingredients = reader.GetString(4),
                    ImageUrl = reader.IsDBNull(5) ? null : reader.GetString(5),
                    RestaurantId = reader.GetInt32(6),
                    Status = reader.GetString(7)
                };
                meals.Add(meal);
            }

            return meals;
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
    public List<Meal> GetByRestaurantId(int restaurantId)
    {
        var meals = new List<Meal>();

        try
        {
            using SqliteConnection connection = new SqliteConnection(_connectionString);
            connection.Open();

            string query = @"
            SELECT Id, OrderPosition, Name, Price, Ingredients, ImageUrl, RestaurantId, Status
            FROM Meals
            WHERE RestaurantId = @RestaurantId;";

            using SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@RestaurantId", restaurantId);

            using SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                var meal = new Meal
                {
                    Id = reader.GetInt32(0),
                    Order = reader.GetInt32(1),
                    Name = reader.GetString(2),
                    Price = reader.GetDecimal(3),
                    Ingredients = reader.GetString(4),
                    ImageUrl = reader.IsDBNull(5) ? null : reader.GetString(5),
                    RestaurantId = reader.GetInt32(6),
                    Status = reader.GetString(7)
                };
                meals.Add(meal);
            }

            return meals;
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

    public bool Update(Meal meal)
    {
        try
        {
            using SqliteConnection connection = new SqliteConnection(_connectionString);
            connection.Open();

            string query = @"
            UPDATE Meals
            SET OrderPosition = @Order,
                Name = @Name,
                Price = @Price,
                Ingredients = @Ingredients,
                ImageUrl = @ImageUrl,
                RestaurantId = @RestaurantId,
                Status = @Status
            WHERE Id = @Id;";

            using SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@Order", meal.Order);
            command.Parameters.AddWithValue("@Name", meal.Name);
            command.Parameters.AddWithValue("@Price", meal.Price);
            command.Parameters.AddWithValue("@Ingredients", meal.Ingredients);
            command.Parameters.AddWithValue("@ImageUrl", meal.ImageUrl ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@RestaurantId", meal.RestaurantId);
            command.Parameters.AddWithValue("@Status", meal.Status);
            command.Parameters.AddWithValue("@Id", meal.Id);

            int affectedRows = command.ExecuteNonQuery();
            return affectedRows > 0;
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

    public List<Meal> GetAvailableMeals()
    {
        List<Meal> meals = new List<Meal>();

        try
        {
            using SqliteConnection connection = new SqliteConnection(_connectionString);
            connection.Open();

            string query = @"
            SELECT Id, OrderPosition, Name, Price, Ingredients, ImageUrl, RestaurantId
            FROM Meals
            WHERE Status = 'U ponudi';";

            using SqliteCommand command = new SqliteCommand(query, connection);
            using SqliteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                var meal = new Meal
                {
                    Id = reader.GetInt32(0),
                    Order = reader.GetInt32(1),
                    Name = reader.GetString(2),
                    Price = reader.GetDecimal(3),
                    Ingredients = reader.GetString(4),
                    ImageUrl = reader.IsDBNull(5) ? null : reader.GetString(5),
                    RestaurantId = reader.GetInt32(6)
                };
                meals.Add(meal);
            }

            return meals;
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


    public Meal Create(Meal meal)
    {
        try
        {
            using SqliteConnection connection = new SqliteConnection(_connectionString);
            connection.Open();

            string query = @"
                    INSERT INTO Meals (OrderPosition, Name, Price, Ingredients, ImageUrl, RestaurantId , Status)
                    VALUES (@Order, @Name, @Price, @Ingredients, @ImageUrl, @RestaurantId ,@Status);
                    SELECT LAST_INSERT_ROWID();";
            using SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@Order", meal.Order);
            command.Parameters.AddWithValue("@Name", meal.Name);
            command.Parameters.AddWithValue("@Price", meal.Price);
            command.Parameters.AddWithValue("@Ingredients", meal.Ingredients);
            command.Parameters.AddWithValue("@ImageUrl", meal.ImageUrl ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@RestaurantId", meal.RestaurantId);
            command.Parameters.AddWithValue("@Status", meal.Status);

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
            using SqliteConnection connection = new SqliteConnection(_connectionString);
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

    public bool DeleteActiveInRestaurant(int id)
    {
        try
        {
            using SqliteConnection connection = new SqliteConnection(_connectionString);
            connection.Open();

            string query = "DELETE FROM Meals WHERE RestaurantId = @RestaurantId AND Status = 'u ponudi'";
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
