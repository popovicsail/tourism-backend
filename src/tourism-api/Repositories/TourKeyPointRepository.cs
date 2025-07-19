using Microsoft.Data.Sqlite;
using tourism_api.Domain;

namespace tourism_api.Repositories;

public class TourKeyPointRepository
{
    private readonly string _connectionString;

    public TourKeyPointRepository(IConfiguration configuration)
    {
        _connectionString = configuration["ConnectionString:SQLiteConnection"];
    }

    public TourKeyPoint Create(TourKeyPoint keyPoint)
    {
        try
        {
            using SqliteConnection connection = new SqliteConnection(_connectionString);
            connection.Open();

            string query = @"
                    INSERT INTO TourKeyPoints ([Order], Name, Description, ImageUrl, Latitude, Longitude, TourId)
                    VALUES (@Order, @Name, @Description, @ImageUrl, @Latitude, @Latitude, @TourId);
                    SELECT LAST_INSERT_ROWID();";
            using SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@Order", keyPoint.Order);
            command.Parameters.AddWithValue("@Name", keyPoint.Name);
            command.Parameters.AddWithValue("@Description", keyPoint.Description);
            command.Parameters.AddWithValue("@ImageUrl", keyPoint.ImageUrl);
            command.Parameters.AddWithValue("@Latitude", keyPoint.Latitude);
            command.Parameters.AddWithValue("@Longitude", keyPoint.Longitude);
            command.Parameters.AddWithValue("@TourId", keyPoint.TourId);

            keyPoint.Id = Convert.ToInt32(command.ExecuteScalar());

            return keyPoint;
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

    public List<TourKeyPoint> GetByTourId(int tourId)
    {
        List<TourKeyPoint> tourKeyPoints = new List<TourKeyPoint>();
        try
        {
            using SqliteConnection connection = new SqliteConnection(_connectionString);
            connection.Open();

            string query = @$"
                    SELECT t.Id, t.[Order], t.Name, t.Description, t.ImageUrl, t.Latitude, t.Longitude, t.TourId
                    FROM TourKeyPoints t
                    WHERE t.TourId = {tourId}";
            using SqliteCommand command = new SqliteCommand(query, connection);

            using SqliteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                tourKeyPoints.Add(new TourKeyPoint
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Order = Convert.ToInt32(reader["Order"]),
                    Name = reader["Name"].ToString(),
                    Description = reader["Description"].ToString(),
                    ImageUrl = reader["ImageUrl"].ToString(),
                    Latitude = Convert.ToDouble(reader["Latitude"]),
                    Longitude = Convert.ToDouble(reader["Longitude"]),
                    TourId = Convert.ToInt32(reader["TourId"]),
                });
            }

            return tourKeyPoints;
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

            string query = "DELETE FROM TourKeyPoints WHERE Id = @Id";
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