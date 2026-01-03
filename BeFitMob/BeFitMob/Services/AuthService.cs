using BeFitMob.Models;

namespace BeFitMob.Services;

public class AuthService
{
    private readonly List<User> _users = new()
    {
        new User { Id = 1, Username = "admin", Password = "admin", Role = UserRole.Administrator },
        new User { Id = 2, Username = "user",  Password = "user",  Role = UserRole.Uzytkownik }
    };

    public User? CurrentUser { get; private set; }

    public bool Login(string username, string password)
    {
        var user = _users.FirstOrDefault(u =>
            u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) &&
            u.Password == password);

        if (user == null)
            return false;

        CurrentUser = user;
        return true;
    }

    public void Logout()
    {
        CurrentUser = null;
    }

    public bool IsLoggedIn => CurrentUser != null;

    public bool IsAdmin => CurrentUser?.Role == UserRole.Administrator;

    public string? CurrentUserId => CurrentUser?.Username;
}