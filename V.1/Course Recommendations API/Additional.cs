using System;
using System.Linq;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Cors;

public class CourseSearchRequest
{
    public string SearchTerm { get; set; }
    public string Category { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public decimal? MinRating { get; set; }
    public string Instructor { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SortBy { get; set; } = "rating";
    public string SortOrder { get; set; } = "desc";
}
