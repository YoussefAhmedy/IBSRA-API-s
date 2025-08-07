using System;
using System.Collections.Generic;

public class CourseRecommendationRequest
{
    public int? UserID { get; set; }
    public string Category { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SortBy { get; set; } = "rating"; // rating, price, name
    public string SortOrder { get; set; } = "desc"; // asc, desc
}
