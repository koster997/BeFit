using System.ComponentModel.DataAnnotations;

namespace BeFitMob.Models;

public enum UserRole
{
    Uzytkownik = 0,
    Administrator = 1
}

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Display(Name = "Nazwa użytkownika")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Nazwa użytkownika musi mieć od 3 do 50 znaków.")]
    public string Username { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Hasło")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Hasło musi mieć co najmniej 3 znaki.")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Rola")]
    public UserRole Role { get; set; }
}