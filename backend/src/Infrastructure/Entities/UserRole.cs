
namespace MexyApp.Models;

// Entidad de relaci¢n 1..N entre User y RoleName.
// EF Core la usa para persistir roles por usuario.
public class UserRole
{
    public int UserId { get; set; }             // FK al usuario
    public RoleName Role { get; set; }          // Rol asignado
    public User? User { get; set; }             // Navegaci¢n opcional

    public UserRole() { }                       // Constructor vac¡o para EF
}

