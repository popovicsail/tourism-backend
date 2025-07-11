namespace tourism_api.Domain
{
    public class TourRating
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TourId { get; set; }
        public string? Comment { get; set; }
        public int Rating { get; set; }
        public DateTime? RatingDate { get; set; }
        public string? Username { get; set; }
    }
}
