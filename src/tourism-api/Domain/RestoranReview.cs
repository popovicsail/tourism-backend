namespace tourism_api.Domain
{
    public class RestoranReview
    {
        public int? Id { get; set; }
        public int RestoranId { get; set; }
        public int UserId { get; set; }
        public string? ReviewText { get; set; } // Nullable to allow for empty reviews, but can be validated later
        public int Rating { get; set; } // Assuming rating is an integer from 1 to 5
        public bool IsValid()
        {
            return RestoranId > 0 && UserId > 0 && Rating >= 1 && Rating <= 5;
        }
    }
}
