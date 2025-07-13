﻿namespace tourism_api.Domain;

public class Restaurant
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Capacity { get; set; }
    public string ImageUrl { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Status { get; set; } = "u pripremi";
    public User? Owner { get; set; }
    public int OwnerId { get; set; }
    public double AverageRating { get; set; }
    public List<Meal> Meals { get; set; } = new List<Meal>();
    public List<RestoranReview> Reviews { get; set; } = new List<RestoranReview>();

    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Description)
            && !string.IsNullOrWhiteSpace(ImageUrl) && Capacity > 0;
    }
}
