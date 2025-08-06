using System;
using System.Web.Http;
using System.Web.Http.Cors;

[EnableCors(origins: "*", headers: "*", methods: "*")]
[RoutePrefix("api/auth")]
public class AuthController : ApiController
{
    private readonly AuthDatabaseHelper _db;
    private readonly JwtHelper _jwtHelper;

    public AuthController()
    {
        _db = new AuthDatabaseHelper();
        _jwtHelper = new JwtHelper();
    }

    [HttpPost]
    [Route("login")]
    public IHttpActionResult Login([FromBody] LoginRequest request)
    {
        try
        {
            // Validate input
            if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return Ok(new LoginResponse
                {
                    Success = false,
                    Message = "Email and password are required"
                });
            }

            // Authenticate user
            User user = _db.AuthenticateUser(request.Email, request.Password);
            
            if (user != null)
            {
                // Generate JWT token
                string token = _jwtHelper.GenerateToken(user);
                
                return Ok(new LoginResponse
                {
                    Success = true,
                    Message = "Login successful",
                    Token = token,
                    User = new UserInfo
                    {
                        ID = user.ID,
                        Name = user.Name,
                        Username = user.Username,
                        Email = user.Email,
                        WelcomeMessage = $"Welcome, {user.Name}"
                    }
                });
            }
            else
            {
                return Ok(new LoginResponse
                {
                    Success = false,
                    Message = "Invalid email or password"
                });
            }
        }
        catch (Exception ex)
        {
            return Ok(new LoginResponse
            {
                Success = false,
                Message = "An error occurred during login"
            });
        }
    }

    [HttpPost]
    [Route("register")]
    public IHttpActionResult Register([FromBody] RegisterRequest request)
    {
        try
        {
            // Validate input
            if (request == null || string.IsNullOrEmpty(request.Name) || 
                string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Username) ||
                string.IsNullOrEmpty(request.Password))
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = "All fields are required"
                });
            }

            if (request.Age <= 0)
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = "Please provide a valid age"
                });
            }

            // Register user
            User newUser = _db.RegisterUser(request.Name, request.Age, request.PhoneNumber, 
                                          request.Email, request.Username, request.Password);
            
            if (newUser != null)
            {
                // Generate JWT token for immediate login
                string token = _jwtHelper.GenerateToken(newUser);
                
                return Ok(new LoginResponse
                {
                    Success = true,
                    Message = "Registration successful",
                    Token = token,
                    User = new UserInfo
                    {
                        ID = newUser.ID,
                        Name = newUser.Name,
                        Username = newUser.Username,
                        Email = newUser.Email,
                        WelcomeMessage = $"Welcome, {newUser.Name}"
                    }
                });
            }
            else
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = "User with this email or username already exists"
                });
            }
        }
        catch (Exception ex)
        {
            return Ok(new ApiResponse
            {
                Success = false,
                Message = "An error occurred during registration"
            });
        }
    }

    [HttpPost]
    [Route("forgot-password")]
    public IHttpActionResult ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        try
        {
            if (request == null || string.IsNullOrEmpty(request.Email))
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = "Email is required"
                });
            }

            // Check if user exists
            if (!_db.UserExistsByEmail(request.Email))
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = "No user found with this email address"
                });
            }

            // Generate reset token
            string resetToken = PasswordHelper.GenerateResetToken();
            
            // Save reset token
            if (_db.SaveResetToken(request.Email, resetToken))
            {
                // In a real application, you would send this token via email
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Password reset token generated successfully",
                    Data = new { ResetToken = resetToken } // Don't do this in production!
                });
            }
            else
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = "Failed to generate reset token"
                });
            }
        }
        catch (Exception ex)
        {
            return Ok(new ApiResponse
            {
                Success = false,
                Message = "An error occurred during password reset request"
            });
        }
    }

    [HttpPost]
    [Route("reset-password")]
    public IHttpActionResult ResetPassword([FromBody] ResetPasswordRequest request)
    {
        try
        {
            if (request == null || string.IsNullOrEmpty(request.ResetToken) || 
                string.IsNullOrEmpty(request.NewPassword))
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = "Reset token and new password are required"
                });
            }

            if (request.NewPassword.Length < 6)
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = "Password must be at least 6 characters long"
                });
            }

            // Reset password
            if (_db.ResetPassword(request.ResetToken, request.NewPassword))
            {
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Password reset successful"
                });
            }
            else
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = "Invalid or expired reset token"
                });
            }
        }
        catch (Exception ex)
        {
            return Ok(new ApiResponse
            {
                Success = false,
                Message = "An error occurred during password reset"
            });
        }
    }
}
