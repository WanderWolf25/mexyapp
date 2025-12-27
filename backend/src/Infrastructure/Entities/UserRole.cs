
namespace MexyApp.Models;

public class UserRole
{
    public int UserId { get; set; }
    public RoleName Role { get; set; }
    public User? User { get; set; } // navegación opcional

    public UserRole() { } // constructor vacío para EF
}
