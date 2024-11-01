using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using ServicesForShelfSwap.Data;
using ServicesForShelfSwap.Models;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Security.Cryptography;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly BookExchangeContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(BookExchangeContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        // Check if the email is already registered
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (existingUser != null)
        {
            return BadRequest(new { message = "Email already in use." });
        }

        // Hash the password
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // Create a new User entity
        var newUser = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = passwordHash,
            FavouriteGenre = request.FavouriteGenre,
            ReadingPreferences = request.ReadingPreference,
            CreatedAt = DateTime.Now,         // Set CreatedAt timestamp
            UpdatedAt = DateTime.Now          // Set UpdatedAt timestamp
        };

        try
        {
            // Add the new user to the database
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User registered successfully.", userId = newUser.UserId });
        }
        catch (Exception ex)
        {
            // Handle errors
            return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
        }
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] ServicesForShelfSwap.Models.LoginRequest request)
    {
        // Check if the user exists by email
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        // Verify the password
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        // Generate JWT token
        var token = GenerateJwtToken(user);

        //if (string.IsNullOrEmpty(user.ResetToken))
        //{
        //    return BadRequest(new { message = "User data is incomplete." });
        //}

        // Return response with JWT and user information
        return Ok(new
        {
            token = token,
            userId = user.UserId,
            name = user.Name
        });
    }

    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email)
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = _configuration["JwtSettings:Issuer"],
            Audience = _configuration["JwtSettings:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ServicesForShelfSwap.Models.ResetPasswordRequest request)
    {
        // Check if the user exists in the database
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null)
        {
            return BadRequest(new { message = "User with the provided email does not exist." });
        }

        // Generate a password reset token (This can be a unique token stored in a separate table or a JWT)
        string resetToken = Guid.NewGuid().ToString(); // Simple example, consider using a more secure approach

        // Store the reset token and send it via email
        string resetLink = $"{_configuration["AppSettings:ClientUrl"]}/reset-password?token={resetToken}";
        await SendPasswordResetEmail(request.Email, resetLink);

        return Ok(new { message = "Password reset link sent to the registered email." });
    }

    private async Task SendPasswordResetEmail(string email, string resetLink)
    {
        try
        {
            var smtpClient = new SmtpClient(_configuration["SMTP:Host"])
            {
                Port = int.Parse(_configuration["SMTP:Port"]),
                Credentials = new System.Net.NetworkCredential(_configuration["SMTP:Username"], _configuration["SMTP:Password"]),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["SMTP:FromEmail"]),
                Subject = "Password Reset",
                Body = $"Click on the link to reset your password: {resetLink}",
                IsBodyHtml = true,
            };
            mailMessage.To.Add(email);

            await smtpClient.SendMailAsync(mailMessage);
        }
        catch (Exception ex)
        {
            // Log exception or handle error
        }
    }
    [HttpPut("update-password")]
    public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequest request)
    {
        // Retrieve the user associated with the reset token from the database
        var user = await _context.Users.FirstOrDefaultAsync(u => u.ResetToken == request.Token && u.ResetTokenExpiry > DateTime.UtcNow);
        if (user == null)
        {
            return BadRequest(new { message = "Invalid or expired reset token." });
        }

        // Update the password after hashing it
        user.PasswordHash = HashPassword(request.NewPassword);
        user.ResetToken = null; // Clear the reset token after successful password reset
        user.ResetTokenExpiry = null;

        // Update the user's password in the database
        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Password updated successfully." });
    }

    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }    
}
