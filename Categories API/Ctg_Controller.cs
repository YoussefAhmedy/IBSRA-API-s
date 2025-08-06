// Categories Controller
using System;
using System.Web.Http;
using System.Web.Http.Cors;

[EnableCors(origins: "*", headers: "*", methods: "*")]
[RoutePrefix("api/categories")]
public class CategoriesController : ApiController
{
    private readonly CategoryDatabaseHelper _db;
    private readonly CourseDatabaseHelper _courseDb;

    public CategoriesController()
    {
        _db = new CategoryDatabaseHelper();
        _courseDb = new CourseDatabaseHelper();
    }

    [HttpGet]
    [Route("")]
    public IHttpActionResult GetAllCategories([FromUri] bool includeInactive = false)
    {
        try
        {
            var categories = _db.GetAllCategories(activeOnly: !includeInactive);
            var totalCount = _db.GetTotalCategoriesCount(activeOnly: !includeInactive);

            return Ok(new CategoriesResponse
            {
                Success = true,
                Message = "Categories retrieved successfully",
                Categories = categories,
                TotalCount = totalCount
            });
        }
        catch (Exception ex)
        {
            return Ok(new CategoriesResponse
            {
                Success = false,
                Message = "An error occurred while retrieving categories"
            });
        }
    }

    [HttpGet]
    [Route("popular")]
    public IHttpActionResult GetPopularCategories([FromUri] int count = 5)
    {
        try
        {
            // Validate count parameter
            if (count <= 0 || count > 20) count = 5;

            var categories = _db.GetPopularCategories(count);

            return Ok(new CategoriesResponse
            {
                Success = true,
                Message = "Popular categories retrieved successfully",
                Categories = categories,
                TotalCount = categories.Count
            });
        }
        catch (Exception ex)
        {
            return Ok(new CategoriesResponse
            {
                Success = false,
                Message = "An error occurred while retrieving popular categories"
            });
        }
    }

    [HttpGet]
    [Route("{id:int}")]
    public IHttpActionResult GetCategoryById(int id)
    {
        try
        {
            var category = _db.GetCategoryById(id);

            if (category != null)
            {
                // Get popular courses in this category
                var popularCourses = _db.GetPopularCoursesByCategory(category.Name, 5);

                return Ok(new CategoryDetailsResponse
                {
                    Success = true,
                    Message = "Category details retrieved successfully",
                    Category = category,
                    PopularCourses = popularCourses
                });
            }
            else
            {
                return Ok(new CategoryDetailsResponse
                {
                    Success = false,
                    Message = "Category not found"
                });
            }
        }
        catch (Exception ex)
        {
            return Ok(new CategoryDetailsResponse
            {
                Success = false,
                Message = "An error occurred while retrieving category details"
            });
        }
    }

    [HttpGet]
    [Route("{name}")]
    public IHttpActionResult GetCategoryByName(string name)
    {
        try
        {
            var category = _db.GetCategoryByName(name);

            if (category != null)
            {
                // Get popular courses in this category
                var popularCourses = _db.GetPopularCoursesByCategory(category.Name, 5);

                return Ok(new CategoryDetailsResponse
                {
                    Success = true,
                    Message = "Category details retrieved successfully",
                    Category = category,
                    PopularCourses = popularCourses
                });
            }
            else
            {
                return Ok(new CategoryDetailsResponse
                {
                    Success = false,
                    Message = "Category not found"
                });
            }
        }
        catch (Exception ex)
        {
            return Ok(new CategoryDetailsResponse
            {
                Success = false,
                Message = "An error occurred while retrieving category details"
            });
        }
    }

    [HttpGet]
    [Route("{categoryName}/courses")]
    public IHttpActionResult GetCoursesByCategory(string categoryName, 
        [FromUri] int page = 1, 
        [FromUri] int pageSize = 10,
        [FromUri] string sortBy = "rating",
        [FromUri] string sortOrder = "desc")
    {
        try
        {
            // Validate parameters
            if (page <= 0) page = 1;
            if (pageSize <= 0 || pageSize > 50) pageSize = 10;

            var request = new CourseRecommendationRequest
            {
                Category = categoryName,
                Page = page,
                PageSize = pageSize,
                SortBy = sortBy,
                SortOrder = sortOrder
            };

            var courses = _courseDb.GetCourseRecommendations(request);
            var totalCount = _courseDb.GetCourseRecommendationsCount(request);

            return Ok(new CourseRecommendationsResponse
            {
                Success = true,
                Message = $"Courses in '{categoryName}' category retrieved successfully",
                Recommendations = courses,
                TotalCount = totalCount
            });
        }
        catch (Exception ex)
        {
            return Ok(new CourseRecommendationsResponse
            {
                Success = false,
                Message = "An error occurred while retrieving courses"
            });
        }
    }

    [HttpPost]
    [Route("refresh-counts")]
    [JwtAuthorization] // Only authenticated users can refresh counts
    public IHttpActionResult RefreshCategoryCourseCounts()
    {
        try
        {
            _db.RefreshCategoryCourseCounts();

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Category course counts refreshed successfully"
            });
        }
        catch (Exception ex)
        {
            return Ok(new ApiResponse
            {
                Success = false,
                Message = "An error occurred while refreshing category counts"
            });
        }
    }

    [HttpGet]
    [Route("stats")]
    public IHttpActionResult GetCategoryStats()
    {
        try
        {
            var categories = _db.GetAllCategories(activeOnly: true);
            
            var stats = new
            {
                TotalCategories = categories.Count,
                TotalCourses = categories.Sum(c => c.CourseCount),
                AverageCoursesPerCategory = categories.Count > 0 ? (double)categories.Sum(c => c.CourseCount) / categories.Count : 0,
                MostPopularCategory = categories.OrderByDescending(c => c.CourseCount).FirstOrDefault()?.Name,
                CategoriesWithCourses = categories.Count(c => c.CourseCount > 0),
                EmptyCategories = categories.Count(c => c.CourseCount == 0)
            };

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Category statistics retrieved successfully",
                Data = stats
            });
        }
        catch (Exception ex)
        {
            return Ok(new ApiResponse
            {
                Success = false,
                Message = "An error occurred while retrieving category statistics"
            });
        }
    }
}
