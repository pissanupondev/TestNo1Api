using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

public interface IUserService
{
    Task<ServiceResult> CreateUserAsync(CreateUserDto dto);
}

public class ServiceResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Dictionary<string, List<string>>? Errors { get; set; }
    public object? Data { get; set; }
}

public class UserService : IUserService
{
    private readonly AppDbContext _db;
    private readonly IHubContext<NotificationHub> _hubContext;

    public UserService(AppDbContext db, IHubContext<NotificationHub> hubContext)
    {
        _db = db;
        _hubContext = hubContext;
    }

    public async Task<ServiceResult> CreateUserAsync(CreateUserDto dto)
    {
        var errors = new Dictionary<string, List<string>>();

        // 1. Validation Logic
        if (string.IsNullOrWhiteSpace(dto.FirstName))
            AddError(errors, "name", "Please provide a valid First Name");

        if (string.IsNullOrWhiteSpace(dto.LastName))
            AddError(errors, "lastname", "Please provide a valid Last Name");

        if (string.IsNullOrWhiteSpace(dto.Email) || !Regex.IsMatch(dto.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            AddError(errors, "email", "Please provide a valid Email");

        // Phone: 10 digits, starts with 06, 08, 09
        if (string.IsNullOrWhiteSpace(dto.Phone) || !Regex.IsMatch(dto.Phone, @"^(06|08|09)\d{8}$"))
            AddError(errors, "phone", "Please provide a valid Phone Number (10 digits starting with 06, 08, 09)");

        if (dto.BirthDay == default)
            AddError(errors, "BirthDay", "Please provide a valid Birth Day");

        if (string.IsNullOrWhiteSpace(dto.Occupation))
            AddError(errors, "occupation", "Please select an Occupation");

        if (string.IsNullOrWhiteSpace(dto.Sex) || (dto.Sex != "Male" && dto.Sex != "Female")){
            AddError(
                errors,
                "sex",
                "Please select a valid Sex"
            );
        }

        // Image Validation (Max 2 MB, formats: JPG, JPEG, PNG, WEBP)
        if (string.IsNullOrWhiteSpace(dto.Profile))
        {
            AddError(errors, "image", "Please provide a valid profile");
        }
        else
        {
            try
            {
                var base64Data = dto.Profile.Contains(",") ? dto.Profile.Split(',')[1] : dto.Profile;
                byte[] bytes = Convert.FromBase64String(base64Data);
                int fileSizeInBytes = bytes.Length;

                if (fileSizeInBytes > 2 * 1024 * 1024) // 2 MB
                    AddError(errors, "image", "Image size must not exceed 2 MB");

                var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/webp" };
                if (!allowedTypes.Contains(dto.ContentType.ToLower()))
                    AddError(errors, "image", "Only JPG, JPEG, PNG, or WEBP image formats are supported");
            }
            catch
            {
                AddError(errors, "image", "Invalid Base64 image data");
            }
        }

        if (errors.Count > 0)
        {
            return new ServiceResult
            {
                Success = false,
                Message = "Please check your input data",
                Errors = errors
            };
        }

        //Check Duplicate Email using EF Core
        bool emailExists = await _db.Users.AnyAsync(u => u.Email == dto.Email);
        if (emailExists)
        {
            return new ServiceResult
            {
                Success = false,
                Message = "This email is already in use"
            };
        }

        //Save to Database using EF Core
        var user = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            BirthDay = dto.BirthDay,
            Sex = dto.Sex,
            Occupation = dto.Occupation,
            CreatedAt = DateTime.UtcNow,
            UserProfile = new UserProfile
            {
                Profile = dto.Profile,
                ContentType = dto.ContentType,
                FileSize = Convert.FromBase64String(dto.Profile.Contains(",") ? dto.Profile.Split(',')[1] : dto.Profile).Length
            }
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        //Send SignalR Notification
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", $"New user created with ID: {user.Id}");

        //Return Success Result
        return new ServiceResult
        {
            Success = true,
            Message = "save data success",
            Data = new { id = user.Id }
        };
    }

    private static void AddError(Dictionary<string, List<string>> errors, string key, string message)
    {
        if (!errors.ContainsKey(key))
            errors[key] = new List<string>();
        errors[key].Add(message);
    }
}