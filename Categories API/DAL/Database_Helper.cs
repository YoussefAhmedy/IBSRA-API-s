using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

public class CategoryDatabaseHelper
{
    private readonly string connectionString;

    public CategoryDatabaseHelper()
    {
        connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
    }

    public List<CategorySummary> GetAllCategories(bool activeOnly = true)
    {
        var categories = new List<CategorySummary>();
        
        string query = @"
            SELECT ID, Name, Description, IconUrl, Color, CourseCount 
            FROM Categories 
            WHERE (@ActiveOnly = 0 OR IsActive = 1)
            ORDER BY DisplayOrder, Name";

        using (var conn = new SqlConnection(connectionString))
        {
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@ActiveOnly", activeOnly ? 1 : 0);
                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        categories.Add(new CategorySummary
                        {
                            ID = reader.GetInt32("ID"),
                            Name = reader.GetString("Name"),
                            Description = reader.IsDBNull("Description") ? null : reader.GetString("Description"),
                            IconUrl = reader.IsDBNull("IconUrl") ? null : reader.GetString("IconUrl"),
                            Color = reader.IsDBNull("Color") ? "#007bff" : reader.GetString("Color"),
                            CourseCount = reader.GetInt32("CourseCount")
                        });
                    }
                }
            }
        }

        return categories;
    }

    public Category GetCategoryById(int categoryId)
    {
        string query = @"
            SELECT ID, Name, Description, IconUrl, Color, CourseCount, 
                   IsActive, DisplayOrder, CreatedAt, UpdatedAt 
            FROM Categories 
            WHERE ID = @CategoryID";

        using (var conn = new SqlConnection(connectionString))
        {
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@CategoryID", categoryId);
                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Category
                        {
                            ID = reader.GetInt32("ID"),
                            Name = reader.GetString("Name"),
                            Description = reader.IsDBNull("Description") ? null : reader.GetString("Description"),
                            IconUrl = reader.IsDBNull("IconUrl") ? null : reader.GetString("IconUrl"),
                            Color = reader.IsDBNull("Color") ? "#007bff" : reader.GetString("Color"),
                            CourseCount = reader.GetInt32("CourseCount"),
                            IsActive = reader.GetBoolean("IsActive"),
                            DisplayOrder = reader.GetInt32("DisplayOrder"),
                            CreatedAt = reader.GetDateTime("CreatedAt"),
                            UpdatedAt = reader.GetDateTime("UpdatedAt")
                        };
                    }
                }
            }
        }
        return null;
    }

    public Category GetCategoryByName(string categoryName)
    {
        string query = @"
            SELECT ID, Name, Description, IconUrl, Color, CourseCount, 
                   IsActive, DisplayOrder, CreatedAt, UpdatedAt 
            FROM Categories 
            WHERE Name = @CategoryName";

        using (var conn = new SqlConnection(connectionString))
        {
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@CategoryName", categoryName);
                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Category
                        {
                            ID = reader.GetInt32("ID"),
                            Name = reader.GetString("Name"),
                            Description = reader.IsDBNull("Description") ? null : reader.GetString("Description"),
                            IconUrl = reader.IsDBNull("IconUrl") ? null : reader.GetString("IconUrl"),
                            Color = reader.IsDBNull("Color") ? "#007bff" : reader.GetString("Color"),
                            CourseCount = reader.GetInt32("CourseCount"),
                            IsActive = reader.GetBoolean("IsActive"),
                            DisplayOrder = reader.GetInt32("DisplayOrder"),
                            CreatedAt = reader.GetDateTime("CreatedAt"),
                            UpdatedAt = reader.GetDateTime("UpdatedAt")
                        };
                    }
                }
            }
        }
        return null;
    }

    public List<CategorySummary> GetPopularCategories(int count = 5)
    {
        var categories = new List<CategorySummary>();
        
        string query = @"
            SELECT TOP (@Count) ID, Name, Description, IconUrl, Color, CourseCount 
            FROM Categories 
            WHERE IsActive = 1 AND CourseCount > 0
            ORDER BY CourseCount DESC, Name";

        using (var conn = new SqlConnection(connectionString))
        {
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Count", count);
                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        categories.Add(new CategorySummary
                        {
                            ID = reader.GetInt32("ID"),
                            Name = reader.GetString("Name"),
                            Description = reader.IsDBNull("Description") ? null : reader.GetString("Description"),
                            IconUrl = reader.IsDBNull("IconUrl") ? null : reader.GetString("IconUrl"),
                            Color = reader.IsDBNull("Color") ? "#007bff" : reader.GetString("Color"),
                            CourseCount = reader.GetInt32("CourseCount")
                        });
                    }
                }
            }
        }

        return categories;
    }

    public List<CourseRecommendation> GetPopularCoursesByCategory(string categoryName, int count = 5)
    {
        var courses = new List<CourseRecommendation>();
        
        string query = @"
            SELECT TOP (@Count) ID, CourseName, Category, ImageUrl, Description,
                   Instructor, Duration, Rating, Price,
                   Rating as RecommendationScore
            FROM Courses 
            WHERE Category = @Category 
            ORDER BY Rating DESC, CourseName";

        using (var conn = new SqlConnection(connectionString))
        {
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Count", count);
                cmd.Parameters.AddWithValue("@Category", categoryName);
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

    public int GetTotalCategoriesCount(bool activeOnly = true)
    {
        string query = "SELECT COUNT(*) FROM Categories WHERE (@ActiveOnly = 0 OR IsActive = 1)";
        
        using (var conn = new SqlConnection(connectionString))
        {
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@ActiveOnly", activeOnly ? 1 : 0);
                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }
    }

    public void RefreshCategoryCourseCounts()
    {
        string query = @"
            UPDATE Categories 
            SET CourseCount = (
                SELECT COUNT(*) 
                FROM Courses 
                WHERE Category = Categories.Name
            ),
            UpdatedAt = GETDATE()";

        using (var conn = new SqlConnection(connectionString))
        {
            using (var cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
