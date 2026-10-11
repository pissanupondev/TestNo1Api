public class UserProfile
{
    public int UserId { get; set; }
    public string Profile { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public int FileSize { get; set; }

    public User User { get; set; } = null!;
}