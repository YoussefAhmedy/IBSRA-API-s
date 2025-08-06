using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

public class CourseDatabaseHelper
{
    private readonly string connectionString;

    public CourseDatabaseHelper()
    {
        connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
    }

    public List<CourseRecommendation> GetCourseRecommendations(CourseRecommendationRequest request)
    {
        var courses = new List<CourseRecommendation>();
        
        // Build dynamic query based on filters
        var query = @"
            SELECT c.ID, c.CourseName, c.Category, c.ImageUrl, c.Description, 
                   c.Instructor, c.Duration, c.Rating, c.Price,
                   CASE 
                       WHEN c.IsRecommended = 1 THEN c.Rating * 1.2
                       ELSE c.Rating 
                   END as RecommendationScore
            FROM Courses c
            WHERE 1=1";

        var parameters = new List<SqlParameter>();

        // Add category filter if specified
        if (!string.IsNullOrEmpty(request.Category))
        {
            query += " AND c.Category = @Category";
            parameters.Add(new SqlParameter("@Category", request.Category));
        }

        // Add sorting
        switch (request.SortBy?.ToLower())
        {
            case "price":
                query += $" ORDER BY c.Price {request.SortOrder}";
                break;
            case "name":
                query += $" ORDER BY c.CourseName {request.SortOrder}";
                break;
            case "rating":
            default:
                query += $" ORDER BY RecommendationScore {request.SortOrder}";
                break;
        }

        // Add pagination
        int offset = (request.Page - 1) * request.PageSize;
        query += $" OFFSET {offset} ROWS FETCH NEXT {request.PageSize} ROWS ONLY";

        using (var conn = new SqlConnection(connectionString))
        {
            using (var cmd = new SqlCommand(query, conn))
            {
                // Add parameters
                foreach (var param in parameters)
                {
                    cmd.Parameters.Add(param);
                }

                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        courses.Add(new CourseRecommendation
                        {
                            ID = reader.GetInt32("ID"),
                            CourseName = reader.GetString("CourseName"),
                            Category = reader.GetString("Category"),
                            ImageUrl = reader.IsDBNull("ImageUrl") ? null : reader.GetString("ImageUrl"),
                            Description = reader.IsDBNull("Description") ? null : reader.GetString("Description"),
                            Instructor = reader.IsDBNull("Instructor") ? null : reader.GetString("Instructor"),
                            Duration = reader.IsDBNull("Duration") ? null : reader.GetString("Duration"),
                            Rating = reader.IsDBNull("Rating") ? null : (decimal?)reader.GetDecimal("Rating"),
                            Price = reader.IsDBNull("Price") ? null : (decimal?)reader.GetDecimal("Price"),
                            RecommendationScore = reader.IsDBNull("RecommendationScore") ? 0 : reader.GetDecimal("RecommendationScore")
                        });
                    }
                }
            }
        }

        return courses;
    }

    public int GetCourseRecommendationsCount(CourseRecommendationRequest request)
    {
        var query = "SELECT COUNT(*) FROM Courses c WHERE 1=1";
        var parameters = new List<SqlParameter>();

        // Add category filter if specified
        if (!string.IsNullOrEmpty(request.Category))
        {
            query += " AND c.Category = @Category";
            parameters.Add(new SqlParameter("@Category", request.Category));
        }

        using (var conn = new SqlConnection(connectionString))
        {
            using (var cmd = new SqlCommand(query, conn))
            {
                // Add parameters
                foreach (var param in parameters)
                {
                    cmd.Parameters.Add(param);
                }

                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }
    }

    public List<CourseRecommendation> GetPersonalizedRecommendations(int userID, int count = 5)
    {
        var courses = new List<CourseRecommendation>();

        // This is a simplified recommendation algorithm
        // In a real system, you might use machine learning or collaborative filtering
        var query = @"
            SELECT TOP (@Count) c.ID, c.CourseName, c.Category, c.ImageUrl, c.Description,
                   c.Instructor, c.Duration, c.Rating, c.Price,
                   (c.Rating * 1.5 + 
                    CASE WHEN c.IsRecommended = 1 THEN 2.0 ELSE 0 END) as RecommendationScore
            FROM Courses c
            LEFT JOIN UserCourseRecommendations ucr ON c.ID = ucr.CourseID AND ucr.UserID = @UserID
            WHERE ucr.CourseID IS NULL  -- Exclude courses already recommended to user
            ORDER BY RecommendationScore DESC, c.Rating DESC";

        using (var conn = new SqlConnection(connectionString))
        {
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Count", count);
                cmd.Parameters.AddWithValue("@UserID", userID);
                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        courses.Add(new CourseRecommendation
                        {
                            ID = reader.GetInt32("ID"),
                            CourseName = reader.GetString("CourseName"),
                            Category = reader.GetString("Category"),
                            ImageUrl = reader.IsDBNull("ImageUrl") ? null : reader.GetString("ImageUrl"),
                            Description = reader.IsDBNull("Description") ? null : reader.GetString("Description"),
                            Instructor = reader.IsDBNull("Instructor") ? null : reader.GetString("Instructor"),
                            Duration = reader.IsDBNull("Duration") ? null : reader.GetString("Duration"),
                            Rating = reader.IsDBNull("Rating") ? null : (decimal?)reader.GetDecimal("Rating"),
                            Price = reader.IsDBNull("Price") ? null : (decimal?)reader.GetDecimal("Price"),
                            RecommendationScore = reader.IsDBNull("RecommendationScore") ? 0 : reader.GetDecimal("RecommendationScore")
                        });
                    }
                }
            }
        }

        return courses;
    }

    public Course GetCourseById(int courseId)
    {
        string query = @"SELECT ID, CourseName, Category, ImageUrl, Description, 
                        Instructor, Duration, Rating, Price, IsRecommended, CreatedAt 
                        FROM Courses WHERE ID = @CourseID";

        using (var conn = new SqlConnection(connectionString))
        {
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@CourseID", courseId);
                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Course
                        {
                            ID = reader.GetInt32("ID"),
                            CourseName = reader.GetString("CourseName"),
                            Category = reader.GetString("Category"),
                            ImageUrl = reader.IsDBNull("ImageUrl") ? null : reader.GetString("ImageUrl"),
                            Description = reader.IsDBNull("Description") ? null : reader.GetString("Description"),
                            Instructor = reader.IsDBNull("Instructor") ? null : reader.GetString("Instructor"),
                            Duration = reader.IsDBNull("Duration") ? null : reader.GetString("Duration"),
                            Rating = reader.IsDBNull("Rating") ? null : (decimal?)reader.GetDecimal("Rating"),
                            Price = reader.IsDBNull("Price") ? null : (decimal?)reader.GetDecimal("Price"),
                            IsRecommended = reader.GetBoolean("IsRecommended"),
                            CreatedAt = reader.GetDateTime("CreatedAt")
                        };
                    }
                }
            }
        }
        return null;
    }
}
