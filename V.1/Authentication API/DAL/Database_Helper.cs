using System;
using System.Configuration;
using System.Data.SqlClient;

public class AuthDatabaseHelper
{
    private readonly string connectionString;

    public AuthDatabaseHelper()
    {
        connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
    }

    public User AuthenticateUser(string email, string password)
    {
        string query = "SELECT ID, Name, Age, PhoneNumber, Email, Username, Password, CreatedAt FROM Users WHERE Email = @Email";
        
        using (var conn = new SqlConnection(connectionString))
        {
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Email", email);
                conn.Open();
                
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var user = new User
                        {
                            ID = reader.GetInt32("ID"),
                            Name = reader.GetString("Name"),
                            Age = reader.GetInt32("Age"),
                            PhoneNumber = reader.GetString("PhoneNumber"),
                            Email = reader.GetString("Email"),
                            Username = reader.GetString("Username"),
                            Password = reader.GetString("Password"),
                            CreatedAt = reader.GetDateTime("CreatedAt")
                        };

                        // Verify password
                        if (PasswordHelper.VerifyPassword(password, user.Password))
                        {
                            user.Password = null; // Don't return password
                            return user;
                        }
                    }
                }
            }
        }
        return null;
    }

    public User RegisterUser(string name, int age, string phoneNumber, string email, string username, string password)
    {
        // Check if user already exists
        if (UserExistsByEmail(email) || UserExistsByUsername(username))
            return null;

        string hashedPassword = PasswordHelper.HashPassword(password);
        
        string query = @"INSERT INTO Users (Name, Age, PhoneNumber, Email, Username, Password) 
                        OUTPUT INSERTED.ID, INSERTED.Name, INSERTED.Age, INSERTED.PhoneNumber, 
                               INSERTED.Email, INSERTED.Username, INSERTED.CreatedAt
                        VALUES (@Name, @Age, @PhoneNumber, @Email, @Username, @Password)";

        using (var conn = new SqlConnection(connectionString))
        {
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Age", age);
                cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", hashedPassword);
                
                conn.Open();
                
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            ID = reader.GetInt32("ID"),
                            Name = reader.GetString("Name"),
                            Age = reader.GetInt32("Age"),
                            PhoneNumber = reader.GetString("PhoneNumber"),
                            Email = reader.GetString("Email"),
                            Username = reader.GetString("Username"),
                            CreatedAt = reader.GetDateTime("CreatedAt")
                        };
                    }
                }
            }
        }
        return null;
    }

    public bool UserExistsByEmail(string email)
    {
        string query = "SELECT COUNT(*) FROM Users WHERE Email = @Email";
        
        using (var conn = new SqlConnection(connectionString))
        {
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Email", email);
                conn.Open();
                
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }
    }

    public bool UserExistsByUsername(string username)
    {
        string query = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
        
        using (var conn = new SqlConnection(connectionString))
        {
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Username", username);
                conn.Open();
                
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }
    }

    public bool SaveResetToken(string email, string resetToken)
    {
        string query = "UPDATE Users SET ResetToken = @ResetToken, ResetTokenExpiry = @Expiry WHERE Email = @Email";
        
        using (var conn = new SqlConnection(connectionString))
        {
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@ResetToken", resetToken);
                cmd.Parameters.AddWithValue("@Expiry", DateTime.UtcNow.AddHours(1)); // 1 hour expiry
                cmd.Parameters.AddWithValue("@Email", email);
                
                conn.Open();
                int result = cmd.ExecuteNonQuery();
                return result > 0;
            }
        }
    }

    public bool ResetPassword(string resetToken, string newPassword)
    {
        string query = @"UPDATE Users SET Password = @Password, ResetToken = NULL, ResetTokenExpiry = NULL 
                        WHERE ResetToken = @ResetToken AND ResetTokenExpiry > @Now";
        
        using (var conn = new SqlConnection(connectionString))
        {
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Password", PasswordHelper.HashPassword(newPassword));
                cmd.Parameters.AddWithValue("@ResetToken", resetToken);
                cmd.Parameters.AddWithValue("@Now", DateTime.UtcNow);
                
                conn.Open();
                int result = cmd.ExecuteNonQuery();
                return result > 0;
            }
        }
    }
}
