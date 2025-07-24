namespace tourism_api.Domain
{
    public class MonthlyOccupancyStatistic
    {
        public string RestaurantName { get; set; }
        public List<MonthlyOccupancy> MonthlyOccupancy { get; set; }
    }
}
