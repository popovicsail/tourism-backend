using Microsoft.Data.Sqlite;
using tourism_api.Domain;

namespace tourism_api.Repositories
{
    public class RestoranReviewRepository
    {
        private readonly string _connectionString;

        public RestoranReviewRepository(IConfiguration configuration)
        {
            _connectionString = configuration["ConnectionString:SQLiteConnection"];
        }

        //Get review by ID
        public RestoranReview GetById(int reviewId)
        {
            try
            {
                using var connection = new SqliteConnection(_connectionString);
                connection.Open();
                string query = @"SELECT Id, RestoranId, UserId, ReviewText, Rating
                             FROM RestoranReview
                             WHERE Id = @Id";
                using SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@Id", reviewId);
                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return new RestoranReview
                    {
                        Id = reader.GetInt32(0),
                        RestoranId = reader.GetInt32(1),
                        UserId = reader.GetInt32(2),
                        ReviewText = reader.GetString(3),
                        Rating = reader.GetInt32(4)
                    };
                }
                return null; // No review found with the given ID
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

        // Get reviews by restaurant ID
        public List<RestoranReview> GetByRestaurantId(int restaurantId)
        {
            try
            {
                List<RestoranReview> reviews = new List<RestoranReview>();
                using var connection = new SqliteConnection(_connectionString);
                connection.Open();
                string query = @"SELECT Id, RestoranId, UserId, ReviewText, Rating
                             FROM RestoranReview
                             WHERE RestoranId = @RestoranId";
                using SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@RestoranId", restaurantId);
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    reviews.Add(new RestoranReview
                    {
                        Id = reader.GetInt32(0),
                        RestoranId = reader.GetInt32(1),
                        UserId = reader.GetInt32(2),
                        ReviewText = reader.GetString(3),
                        Rating = reader.GetInt32(4)
                    });
                }
                return reviews;
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

        // Add a new review
        public bool Add(RestoranReview review)
        {
            try
            {
                if (!review.IsValid())
                {
                    return false; // Invalid review
                }
                using SqliteConnection connection = new SqliteConnection(_connectionString);
                connection.Open();
                string query = @"INSERT INTO RestoranReview (RestoranId, UserId, ReviewText, Rating)
                             VALUES (@RestoranId, @UserId, @ReviewText, @Rating)";
                using SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@RestoranId", review.RestoranId);
                command.Parameters.AddWithValue("@UserId", review.UserId);
                command.Parameters.AddWithValue("@ReviewText", review.ReviewText);
                command.Parameters.AddWithValue("@Rating", review.Rating);


                return command.ExecuteNonQuery() > 0; // Returns true if the insert was successful
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

        // Delete a review by ID
        public bool Delete(int reviewId)
        {
            try
            {
                using SqliteConnection connection = new SqliteConnection(_connectionString);
                connection.Open();
                string query = @"DELETE FROM RestoranReview WHERE Id = @Id";
                using SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@Id", reviewId);
                return command.ExecuteNonQuery() > 0; // Returns true if the delete was successful
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

        public double GetAverageRatingForRestaurant(int restaurantId)
        {
            var reviews = GetByRestaurantId(restaurantId);
            if (reviews == null || reviews.Count == 0)
                return 0;

            return Math.Round(reviews.Average(r => r.Rating), 2);
        }

    }
}
