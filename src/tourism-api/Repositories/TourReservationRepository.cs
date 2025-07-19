using Microsoft.Data.Sqlite;
using tourism_api.Domain;

namespace tourism_api.Repositories
{
    public class TourReservationRepository
    {
        private readonly string _connectionString;

        public TourReservationRepository(IConfiguration configuration)
        {
            _connectionString = configuration["ConnectionString:SQLiteConnection"];
        }

        public List<TourReservation> Create(TourReservation reservation, int reservationAmount)
        {
            var reservations = new List<TourReservation>();

            try
            {
                using SqliteConnection connection = new SqliteConnection(_connectionString);
                connection.Open();

                for (int i = 0; i < reservationAmount; i++)
                {
                    string query = @"
                        INSERT INTO TourReservations (UserId, TourId)
                        VALUES (@UserId, @TourId);
                        SELECT last_insert_rowid();";

                    using SqliteCommand command = new SqliteCommand(query, connection);
                    command.Parameters.AddWithValue("@UserId", reservation.UserId);
                    command.Parameters.AddWithValue("@TourId", reservation.TourId);

                    int newId = Convert.ToInt32(command.ExecuteScalar());

                    reservations.Add(new TourReservation
                    {
                        Id = (int)newId,
                        UserId = reservation.UserId,
                        TourId = reservation.TourId
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
        public bool Delete(int reservationId)
        {
            try
            {
                using SqliteConnection connection = new SqliteConnection(_connectionString);
                connection.Open();

                string query = "DELETE FROM TourReservations WHERE Id = @Id";
                using SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@Id", reservationId);

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
        public List<TourReservation> GetByUserId(int userId)
        {
            List<TourReservation> tourReservations = new List<TourReservation>();

            try
            {
                using SqliteConnection connection = new SqliteConnection(_connectionString);
                connection.Open();

                string query = @$"
                    SELECT t.Id, t.UserId, t.TourId
                    FROM TourReservations t
                    WHERE t.UserId = {userId}";
                using SqliteCommand command = new SqliteCommand(query, connection);

                using SqliteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    tourReservations.Add(new TourReservation
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        TourId = Convert.ToInt32(reader["TourId"]),
                        UserId = Convert.ToInt32(reader["UserId"]),
                    });
                }

                return tourReservations;
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


        public List<TourReservation> GetByTourId(int tourId)
        {
            List<TourReservation> tourReservations = new List<TourReservation>();

            try
            {
                using SqliteConnection connection = new SqliteConnection(_connectionString);
                connection.Open();

                string query = @$"
                    SELECT t.Id, t.TourId, t.UserId
                    FROM TourReservations t
                    WHERE t.TourId = {tourId}";
                using SqliteCommand command = new SqliteCommand(query, connection);

                using SqliteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    tourReservations.Add(new TourReservation
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        TourId = Convert.ToInt32(reader["TourId"]),
                        UserId = Convert.ToInt32(reader["UserId"])
                    });
                }

                return tourReservations;
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
