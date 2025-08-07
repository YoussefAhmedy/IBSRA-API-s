using System;
using System.Collections.Generic;

public class Course
{
    public int ID { get; set; }
    public string CourseName { get; set; }
    public string Category { get; set; }
    public string ImageUrl { get; set; }
    public string Description { get; set; }
    public string Instructor { get; set; }
    public string Duration { get; set; }
    public decimal? Rating { get; set; }
    public decimal? Price { get; set; }
    public bool IsRecommended { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CourseRecommendation
{
    public int ID { get; set; }
    public string CourseName { get; set; }
    public string Category { get; set; }
    public string ImageUrl { get; set; }
    public string Description { get; set; }
    public string Instructor { get; set; }
    public string Duration { get; set; }
    public decimal? Rating { get; set; }
    public decimal? Price { get; set; }
    public decimal RecommendationScore { get; set; }
}

public class CourseRecommendationsResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public List<CourseRecommendation> Recommendations { get; set; }
    public int TotalCount { get; set; }
}
