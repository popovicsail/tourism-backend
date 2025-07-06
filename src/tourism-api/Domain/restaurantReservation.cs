namespace tourism_api.Domain
{
    public class restaurantReservation
    {
        public int Id { get; set; }
        public int RestaurantId { get; set; }
        public int TouristId { get; set; }
        public DateTime Date { get; set; }
        public string Meal { get; set; } // "Doručak", "Ručak", "Večera"
        public int NumberOfPeople { get; set; }
    }
}
