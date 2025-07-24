using System.Globalization;
using tourism_api.Domain;
using tourism_api.Repositories;

namespace tourism_api.Service
{
    public class RestaurantStatisticsService
    {
        private readonly RestaurantRepository _restaurantRepository;
        private readonly RestaurantReservationRepository restaurantReservationRepository;

        public RestaurantStatisticsService(IConfiguration configuration)
        {
            _restaurantRepository = new RestaurantRepository(configuration);
            restaurantReservationRepository = new RestaurantReservationRepository(configuration);
        }

        // Metoda koja vraća sortiranu listu statistike po restoranima za tekuću godinu
        public List<RestoranStatistics> GetTotalResservationsForYear(int ownerId)
        {
            // Uzimamo tekuću godinu
            int currentYear = DateTime.Now.Year;

            // Dobavljamo sve restorane koje poseduje konkretan vlasnik
            List<Restaurant> restaurants = _restaurantRepository.GetByOwner(ownerId);

            // Lista za skladištenje statistike po restoranima
            List<RestoranStatistics> statistics = new List<RestoranStatistics>();

            // Za svaki restoran računamo broj rezervacija za trenutnu godinu
            foreach (var restaurant in restaurants)
            {
                // Dobavljamo sve rezervacije za dati restoran u tekućoj godini
                List<restaurantReservation> reservations = restaurantReservationRepository
                    .GetByRestaurantDate(restaurant.Id, currentYear);

                // Prebrojavamo sve rezervacije
                int totalBookings = reservations.Count();

                // Dodajemo podatke u statistiku
                statistics.Add(new RestoranStatistics
                {
                    RestoranId = restaurant.Id,
                    RestoranName = restaurant.Name,
                    TotalBookings = totalBookings
                });
            }

            // Sortiramo statistiku opadajuće po broju rezervacija
            List<RestoranStatistics> sortedStats = statistics
                .OrderByDescending(stat => stat.TotalBookings)
                .ToList();

            // Vraćamo sortiranu listu statistike
            return sortedStats;
        }


        // Metoda koja vraća statistiku mesečne popunjenosti za određeni restoran
        public MonthlyOccupancyStatistic GetOccupancyByMonth(int restaurantId)
        {
            // Uzimanje trenutne godine
            int currentYear = DateTime.Now.Year;

            // Dobavljanje podataka o restoranu iz baze
            Restaurant restaurant = _restaurantRepository.GetById(restaurantId);

            // Dobavljanje svih rezervacija za restoran u tekućoj godini
            List<restaurantReservation> allReservations = restaurantReservationRepository
                .GetByRestaurantDate(restaurantId, currentYear);

            // Generisanje liste popunjenosti po mesecima korišćenjem LINQ
            List<MonthlyOccupancy> monthlyData = Enumerable.Range(1, 12) // meseci od 1 do 12
                .Select(month =>
                {
                    // Filtriranje rezervacija po mesecu
                    var monthlyReservations = allReservations
                        .Where(r => r.Date.Month == month);

                    // Ukupan broj ljudi koji su rezervisali u tom mesecu
                    int totalPeople = monthlyReservations.Sum(r => r.NumberOfPeople);

                    // Broj dana u mesecu
                    int daysInMonth = DateTime.DaysInMonth(currentYear, month);

                    // Maksimalni kapacitet za ceo mesec (pretpostavka: kapacitet važi po danu)
                    int maxCapacity = restaurant.Capacity * daysInMonth;

                    // Izračunavanje procenta popunjenosti
                    double occupancyRate = maxCapacity > 0
                        ? (double)totalPeople / maxCapacity * 100
                        : 0;

                    // Kreiranje objekta sa statistikom za taj mesec
                    return new MonthlyOccupancy
                    {
                        Month = DateTimeFormatInfo.CurrentInfo.GetMonthName(month),
                        OccupancyRate = Math.Round(occupancyRate, 2)
                    };
                })
                .ToList(); // Pretvaranje rezultata u listu

            // Vraćanje finalnog objekta koji sadrži ime restorana i listu mesečnih popunjenosti
            return new MonthlyOccupancyStatistic
            {
                RestaurantName = restaurant.Name,
                MonthlyOccupancy = monthlyData
            };
        }


    }
}
