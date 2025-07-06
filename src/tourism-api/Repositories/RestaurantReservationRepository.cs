using Microsoft.Data.Sqlite;
using tourism_api.Domain;

namespace tourism_api.Repositories
{
    public class RestaurantReservationRepository
    {
        private readonly string _connectionString;

        public RestaurantReservationRepository(IConfiguration configuration)
        {
            _connectionString = configuration["ConnectionString:SQLiteConnection"];
        }

        // Get reservations by restaurant, date, and meal
        public List<restaurantReservation> GetByRestaurantDateMeal(int restaurantId, DateTime date, string meal)
        {
            var reservations = new List<restaurantReservation>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            string query = @"SELECT Id, RestaurantId, TouristId, Date, Meal, NumberOfPeople
                             FROM RestaurantReservation
                             WHERE RestaurantId = @RestaurantId AND Date = @Date AND Meal = @Meal";
            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@RestaurantId", restaurantId);
            command.Parameters.AddWithValue("@Date", date.Date);
            command.Parameters.AddWithValue("@Meal", meal);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                reservations.Add(new restaurantReservation
                {
                    Id = reader.GetInt32(0),
                    RestaurantId = reader.GetInt32(1),
                    TouristId = reader.GetInt32(2),
                    Date = reader.GetDateTime(3),
                    Meal = reader.GetString(4),
                    NumberOfPeople = reader.GetInt32(5)
                });
            }
            return reservations;
        }


        // Get reservations by tourist
        public List<restaurantReservation> GetByTourist(int touristId)
        {
            var reservations = new List<restaurantReservation>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            string query = @"SELECT Id, RestaurantId, TouristId, Date, Meal, NumberOfPeople
                             FROM RestaurantReservation
                             WHERE TouristId = @TouristId";
            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@TouristId", touristId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                reservations.Add(new restaurantReservation
                {
                    Id = reader.GetInt32(0),
                    RestaurantId = reader.GetInt32(1),
                    TouristId = reader.GetInt32(2),
                    Date = reader.GetDateTime(3),
                    Meal = reader.GetString(4),
                    NumberOfPeople = reader.GetInt32(5)
                });
            }
            return reservations;
        }

        // Get reservation by ID
        public restaurantReservation? GetById(int reservationId)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            string query = @"SELECT Id, RestaurantId, TouristId, Date, Meal, NumberOfPeople
                             FROM RestaurantReservation
                             WHERE Id = @Id";
            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@Id", reservationId);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new restaurantReservation
                {
                    Id = reader.GetInt32(0),
                    RestaurantId = reader.GetInt32(1),
                    TouristId = reader.GetInt32(2),
                    Date = reader.GetDateTime(3),
                    Meal = reader.GetString(4),
                    NumberOfPeople = reader.GetInt32(5)
                };
            }
            return null;
        }

        // Create reservation
        public restaurantReservation Create(restaurantReservation reservation)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            string query = @"INSERT INTO RestaurantReservation
                             (RestaurantId, TouristId, Date, Meal, NumberOfPeople)
                             VALUES (@RestaurantId, @TouristId, @Date, @Meal, @NumberOfPeople);
                             SELECT last_insert_rowid();";
            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@RestaurantId", reservation.RestaurantId);
            command.Parameters.AddWithValue("@TouristId", reservation.TouristId);
            command.Parameters.AddWithValue("@Date", reservation.Date.Date);
            command.Parameters.AddWithValue("@Meal", reservation.Meal);
            command.Parameters.AddWithValue("@NumberOfPeople", reservation.NumberOfPeople);

            reservation.Id = Convert.ToInt32(command.ExecuteScalar());
            return reservation;
        }


        // Delete reservation
        public bool Delete(int reservationId)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            string query = @"DELETE FROM RestaurantReservation WHERE Id = @Id";
            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@Id", reservationId);

            int rowsAffected = command.ExecuteNonQuery();
            return rowsAffected > 0;
        }
    }
}
