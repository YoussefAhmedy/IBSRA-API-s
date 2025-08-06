// Course Recommendations Controller
using System;
using System.Linq;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Cors;

[EnableCors(origins: "*", headers: "*", methods: "*")]
[RoutePrefix("api/courses")]
public class CourseRecommendationsController : ApiController
{
    private readonly CourseDatabaseHelper _db;

    public CourseRecommendationsController()
    {
        _db = new CourseDatabaseHelper();
    }

    [HttpGet]
    [Route("recommendations")]
    public IHttpActionResult GetCourseRecommendations(
        [FromUri] string category = null,
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
                Category = category,
                Page = page,
                PageSize = pageSize,
                SortBy = sortBy,
                SortOrder = sortOrder
            };

            var recommendations = _db.GetCourseRecommendations(request);
            var totalCount = _db.GetCourseRecommendationsCount(request);

            return Ok(new CourseRecommendationsResponse
            {
                Success = true,
                Message = "Course recommendations retrieved successfully",
                Recommendations = recommendations,
                TotalCount = totalCount
            });
        }
        catch (Exception ex)
        {
            return Ok(new CourseRecommendationsResponse
            {
                Success = false,
                Message = "An error occurred while retrieving course recommendations"
            });
        }
    }

    [HttpGet]
    [Route("recommendations/personalized")]
    [JwtAuthorization]
    public IHttpActionResult GetPersonalizedRecommendations([FromUri] int count = 5)
    {
        try
        {
            // Get user ID from JWT token
            var userIdClaim = ((ClaimsPrincipal)RequestContext.Principal)
                .FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Ok(new CourseRecommendationsResponse
                {
                    Success = false,
                    Message = "User not found in token"
                });
            }

            int userId = int.Parse(userIdClaim.Value);

            // Validate count parameter
            if (count <= 0 || count > 20) count = 5;

            var recommendations = _db.GetPersonalizedRecommendations(userId, count);

            return Ok(new CourseRecommendationsResponse
            {
                Success = true,
                Message = "Personalized recommendations retrieved successfully",
                Recommendations = recommendations,
                TotalCount = recommendations.Count
            });
        }
        catch (Exception ex)
        {
            return Ok(new CourseRecommendationsResponse
            {
                Success = false,
                Message = "An error occurred while retrieving personalized recommendations"
            });
        }
    }

    [HttpGet]
    [Route("{id:int}")]
    public IHttpActionResult GetCourseDetails(int id)
    {
        try
        {
            var course = _db.GetCourseById(id);

            if (course != null)
            {
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Course details retrieved successfully",
                    Data = course
                });
            }
            else
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = "Course not found"
                });
            }
        }
        catch (Exception ex)
        {
            return Ok(new ApiResponse
            {
                Success = false,
                Message = "An error occurred while retrieving course details"
            });
        }
    }

    [HttpGet]
    [Route("trending")]
    public IHttpActionResult GetTrendingCourses([FromUri] int count = 10)
    {
        try
        {
            // Get top-rated courses as trending
            var request = new CourseRecommendationRequest
            {
                Page = 1,
                PageSize = count,
                SortBy = "rating",
                SortOrder = "desc"
            };

            var trendingCourses = _db.GetCourseRecommendations(request);

            return Ok(new CourseRecommendationsResponse
            {
                Success = true,
                Message = "Trending courses retrieved successfully",
                Recommendations = trendingCourses,
                TotalCount = trendingCourses.Count
            });
        }
        catch (Exception ex)
        {
            return Ok(new CourseRecommendationsResponse
            {
                Success = false,
                Message = "An error occurred while retrieving trending courses"
            });
        }
    }

    [HttpPost]
    [Route("search")]
    public IHttpActionResult SearchCourses([FromBody] CourseSearchRequest request)
    {
        try
        {
            if (request == null)
            {
                return Ok(new CourseRecommendationsResponse
                {
                    Success = false,
                    Message = "Search request is required"
                });
            }

            var searchResults = _db.SearchCourses(request);
            var totalCount = _db.GetSearchResultsCount(request);

            return Ok(new CourseRecommendationsResponse
            {
                Success = true,
                Message = "Search completed successfully",
                Recommendations = searchResults,
                TotalCount = totalCount
            });
        }
        catch (Exception ex)
        {
            return Ok(new CourseRecommendationsResponse
            {
                Success = false,
                Message = "An error occurred during search"
            });
        }
    }
}
