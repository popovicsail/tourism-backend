using Microsoft.Data.Sqlite;
using tourism_api.Domain;

namespace tourism_api.Repositories
{
    public class ReservationRepository
    {
        private readonly string _connectionString;

        public ReservationRepository(IConfiguration configuration)
        {
            _connectionString = configuration["ConnectionString:SQLiteConnection"];
        }

        public List<Reservation> Create(Reservation reservation, int reservationAmount)
        {
            var reservations = new List<Reservation>();

            try
            {
                using SqliteConnection connection = new SqliteConnection(_connectionString);
                connection.Open();

                for (int i = 0; i < reservationAmount; i++)
                {
                    string query = @"
                        INSERT INTO Reservations (UserId, TourId)
                        VALUES (@UserId, @TourId);
                        SELECT last_insert_rowid();";

                    using SqliteCommand command = new SqliteCommand(query, connection);
                    command.Parameters.AddWithValue("@UserId", reservation.userId);
                    command.Parameters.AddWithValue("@TourId", reservation.tourId);

                    int newId = Convert.ToInt32(command.ExecuteScalar());

                    reservations.Add(new Reservation
                    {
                        Id = (int)newId,
                        userId = reservation.userId,
                        tourId = reservation.tourId
                    });
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

            return reservations;
        }
        public bool Delete(int tourId)
        {
            try
            {
                using SqliteConnection connection = new SqliteConnection(_connectionString);
                connection.Open();

                string query = "DELETE FROM Reservations WHERE TourId = @TourId";
                using SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@TourId", tourId);

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
}
