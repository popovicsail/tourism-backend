using Microsoft.Data.Sqlite;
using tourism_api.Domain;

namespace tourism_api.Repositories;

public class TourRepository
{
    private readonly string _connectionString;
    public TourRepository(IConfiguration configuration)
    {
        _connectionString = configuration["ConnectionString:SQLiteConnection"];
    }

    public List<Tour> GetPaged(int page, int pageSize, string orderBy, string orderDirection, string tourStatus)
    {
        List<Tour> tours = new List<Tour>();
        string sqlStatusFilter = "";

        if (tourStatus != null)
        {
            sqlStatusFilter = $"WHERE t.Status = @tourStatus";
        }

        try
        {
            using SqliteConnection connection = new SqliteConnection(_connectionString);
            connection.Open();

            string query = @$"
                    SELECT t.Id, t.Name, t.Description, t.DateTime, t.MaxGuests, t.Status, t.GuideId
                    FROM Tours t 
                    {sqlStatusFilter}
                    ORDER BY {orderBy} {orderDirection} LIMIT @PageSize OFFSET @Offset";
            using SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@PageSize", pageSize);
            command.Parameters.AddWithValue("@Offset", pageSize * (page - 1));

            if (tourStatus != null)
            {
                command.Parameters.AddWithValue("@tourStatus", tourStatus);
            }

            using SqliteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                tours.Add(new Tour
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Name = reader["Name"].ToString(),
                    Description = reader["Description"].ToString(),
                    DateTime = Convert.ToDateTime(reader["DateTime"]),
                    MaxGuests = Convert.ToInt32(reader["MaxGuests"]),
                    Status = reader["Status"].ToString(),
                    GuideId = Convert.ToInt32(reader["GuideId"]),
                });
            }

            return tours;
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

    public int CountAll(string tourStatus)
    {
        string sqlStatusFilter = "";

        if (tourStatus != null)
        {
            sqlStatusFilter = $"WHERE Status = @tourStatus";
        }
        try
        {
            using SqliteConnection connection = new SqliteConnection(_connectionString);
            connection.Open();

            string query = @$"SELECT COUNT(*) FROM Tours
                            {sqlStatusFilter}";
            ;

            using SqliteCommand command = new SqliteCommand(query, connection);
            if (tourStatus != null)
            {
                command.Parameters.AddWithValue("@tourStatus", tourStatus);
            }

            return Convert.ToInt32(command.ExecuteScalar());
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

    public List<Tour> GetByGuideId(int guideId)
    {
        List<Tour> tours = new List<Tour>();

        try
        {
            using SqliteConnection connection = new SqliteConnection(_connectionString);
            connection.Open();

            string query = @$"
                    SELECT t.Id, t.Name, t.Description, t.DateTime, t.MaxGuests, t.Status,
                           u.Id AS GuideId, u.Username 
                    FROM Tours t 
                    INNER JOIN Users u ON t.GuideId = u.Id
                    WHERE t.GuideId = @GuideId";
            using SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@GuideId", guideId);

            using SqliteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                tours.Add(new Tour
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Name = reader["Name"].ToString(),
                    Description = reader["Description"].ToString(),
                    DateTime = Convert.ToDateTime(reader["DateTime"]),
                    MaxGuests = Convert.ToInt32(reader["MaxGuests"]),
                    Status = reader["Status"].ToString(),
                    GuideId = Convert.ToInt32(reader["GuideId"])
                });
            }

            return tours;
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

    public Tour GetById(int id)
    {
        Tour tour = null;

        try
        {
            using SqliteConnection connection = new SqliteConnection(_connectionString);
            connection.Open();

            string query = @"
                    SELECT t.Id, t.Name, t.Description, t.DateTime, t.MaxGuests, t.Status
                    FROM Tours t             
                    WHERE t.Id = @Id";
            using SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            using SqliteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                if (tour == null)
                {
                    tour = new Tour
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["Name"].ToString(),
                        Description = reader["Description"].ToString(),
                        DateTime = Convert.ToDateTime(reader["DateTime"]),
                        MaxGuests = Convert.ToInt32(reader["MaxGuests"]),
                        Status = reader["Status"].ToString(),
                    };
                }
            }

            return tour;
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

    public Tour Create(Tour tour)
    {
        try
        {
            using SqliteConnection connection = new SqliteConnection(_connectionString);
            connection.Open();

            string query = @"
                    INSERT INTO Tours (Name, Description, DateTime, MaxGuests, Status, GuideId)
                    VALUES (@Name, @Description, @DateTime, @MaxGuests, @Status, @GuideId);
                    SELECT LAST_INSERT_ROWID();";
            using SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@Name", tour.Name);
            command.Parameters.AddWithValue("@Description", tour.Description);
            command.Parameters.AddWithValue("@DateTime", tour.DateTime);
            command.Parameters.AddWithValue("@MaxGuests", tour.MaxGuests);
            command.Parameters.AddWithValue("@Status", tour.Status);
            command.Parameters.AddWithValue("@GuideId", tour.GuideId);

            tour.Id = Convert.ToInt32(command.ExecuteScalar());

            return tour;
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

    public Tour Update(Tour tour)
    {
        try
        {
            using SqliteConnection connection = new SqliteConnection(_connectionString);
            connection.Open();

            string query = @"
                    UPDATE Tours 
                    SET Name = @Name, Description = @Description, DateTime = @DateTime, 
                        MaxGuests = @MaxGuests, Status = @Status
                    WHERE Id = @Id";
            using SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@Id", tour.Id);
            command.Parameters.AddWithValue("@Name", tour.Name);
            command.Parameters.AddWithValue("@Description", tour.Description);
            command.Parameters.AddWithValue("@DateTime", tour.DateTime);
            command.Parameters.AddWithValue("@MaxGuests", tour.MaxGuests);
            command.Parameters.AddWithValue("@Status", tour.Status);

            int affectedRows = command.ExecuteNonQuery();
            return affectedRows > 0 ? tour : null;
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

    public bool Delete(int id)
    {
        try
        {
            using SqliteConnection connection = new SqliteConnection(_connectionString);
            connection.Open();

            string query = "DELETE FROM Tours WHERE Id = @Id";
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
