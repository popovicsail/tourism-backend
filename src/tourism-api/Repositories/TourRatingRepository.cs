using Microsoft.Data.Sqlite;
using tourism_api.Domain;

namespace tourism_api.Repositories
{
    public class TourRatingRepository
    {
        private readonly string _connectionString;

        public TourRatingRepository(IConfiguration configuration)
        {
            _connectionString = configuration["ConnectionString:SQLiteConnection"];
        }

        public TourRating Create(TourRating tourRating)
        {
            try
            {
                using SqliteConnection connection = new SqliteConnection(_connectionString);
                connection.Open();
                {
                    string query = @"
                        INSERT INTO TourRatings (UserId, TourId, RatingDate, Rating, Comment)
                        VALUES (@UserId, @TourId, @RatingDate, @Rating, @Comment);
                        SELECT last_insert_rowid();";

                    using SqliteCommand command = new SqliteCommand(query, connection);
                    command.Parameters.AddWithValue("@UserId", tourRating.UserId);
                    command.Parameters.AddWithValue("@TourId", tourRating.TourId);
                    command.Parameters.AddWithValue("@RatingDate", tourRating.RatingDate);
                    command.Parameters.AddWithValue("@Rating", tourRating.Rating);
                    command.Parameters.AddWithValue("@Comment", tourRating.Comment);

                    int newId = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"Greška pri konekciji ili izvršavanju SQL upita: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Neočekivana greška: {ex.Message}");
                throw;
            }

            return tourRating;
        }

        public TourRating GetById(int id)
        {
            TourRating tourRating = null;

            try
            {
                using SqliteConnection connection = new SqliteConnection(_connectionString);
                connection.Open();

                string query = @$"
                    SELECT t.Id, t.TourId, t.UserId, t.RatingDate, t.Rating, t.Comment, u.Username
                    FROM TourRatings t 
                    INNER JOIN Users u ON t.UserId = u.Id";

                using SqliteCommand command = new SqliteCommand(query, connection);

                using SqliteDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    tourRating = new TourRating
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        TourId = Convert.ToInt32(reader["TourId"]),
                        UserId = Convert.ToInt32(reader["UserId"]),
                        Comment = reader["Comment"].ToString(),
                        Rating = Convert.ToInt32(reader["Rating"]),
                        Username = reader["Username"].ToString()
                    };
                    return tourRating;
                }
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
            return tourRating;
        }

        public List<TourRating> GetAll()
        {
            List<TourRating> tourRatings = new List<TourRating>();

            try
            {
                using SqliteConnection connection = new SqliteConnection(_connectionString);
                connection.Open();

                string query = @$"
                    SELECT t.Id, t.TourId, t.UserId, t.RatingDate, t.Rating, t.Comment
                    FROM TourRatings t";
                using SqliteCommand command = new SqliteCommand(query, connection);

                using SqliteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    tourRatings.Add(new TourRating
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        TourId = Convert.ToInt32(reader["TourId"]),
                        UserId = Convert.ToInt32(reader["UserId"]),
                        Comment = reader["Comment"].ToString(),
                        Rating = Convert.ToInt32(reader["Rating"]),
                    });
                }

                return tourRatings;
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
    }
}
