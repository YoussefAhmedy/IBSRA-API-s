using System;
using System.Collections.Generic;

public class Category
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string IconUrl { get; set; }
    public string Color { get; set; }
    public int CourseCount { get; set; }
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CategorySummary
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string IconUrl { get; set; }
    public string Color { get; set; }
    public int CourseCount { get; set; }
}

public class CategoriesResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public List<CategorySummary> Categories { get; set; }
    public int TotalCount { get; set; }
}

public class CategoryDetailsResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public Category Category { get; set; }
    public List<CourseRecommendation> PopularCourses { get; set; }
}
