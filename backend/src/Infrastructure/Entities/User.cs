
using System;
using System.Collections.Generic;
using System.Linq;

namespace MexyApp.Models;

// Clase dominio User: reglas simples + relación con roles
public class User
{
    // === DATOS PERSISTIDOS ====================================================
    public int Id { get; private set; }                         // PK
    public string Username { get; private set; } = default!;    // Nombre visible
    public string Email { get; private set; } = default!;       // Email normalizado
    public string PasswordHash { get; private set; } = default!;// Hash de contraseña
    public UserStatus Status { get; private set; } = UserStatus.Activo; // Activo por defecto

    // === ROLES (BACKING FIELD 1..N) ===========================================
    private readonly List<UserRole> _userRoles = new();         // Fuente de verdad
    public IReadOnlyCollection<RoleName> Roles =>               // Proyección solo lectura
        _userRoles.Select(ur => ur.Role).ToArray();

    // === CONSTRUCTORES ========================================================
    private User() { }                                          // Requerido por EF
    public User(string username, string email, string passwordHash)
    {
        Username     = NormalizeUsername(username);
        Email        = NormalizeEmail(email);
        PasswordHash = RequireNotEmpty(passwordHash);
        EnsureBaseRole();                                       // KEY: rol base "Comprador"
    }

    // === FUNCIONES DE ROL =====================================================
    public bool HasRole(RoleName role) => _userRoles.Any(r => r.Role == role); // Ya tiene rol?
    public void AddRole(RoleName role)
    {
        if (HasRole(role)) return;                              // Evita duplicado
        _userRoles.Add(new UserRole { Role = role });           // Inserta
    }
    public void RemoveRole(RoleName role)
    {
        var link = _userRoles.FirstOrDefault(r => r.Role == role); // Busca
        if (link is not null) _userRoles.Remove(link);             // Quita si existe
    }

    // === ESTADO ===============================================================
    public void Block()   => Status = UserStatus.Bloqueado;     // Bloquear
    public void Unblock() => Status = UserStatus.Activo;        // Desbloquear

    // === MUTACIONES ===========================================================
    public void ChangeEmail(string newEmail)       => Email        = NormalizeEmail(newEmail); // Unicidad: DB
    public void ChangePasswordHash(string newHash) => PasswordHash = RequireNotEmpty(newHash); // No vacío

    // === HELPERS (PLEGABLES) =================================================
    #region HELPERS
    private void EnsureBaseRole()
    {
        if (!HasRole(RoleName.Comprador))
            _userRoles.Add(new UserRole { Role = RoleName.Comprador });
    }
    private static string NormalizeUsername(string username)
    {
        var v = (username ?? string.Empty).Trim();
        if (v.Length == 0) throw new ArgumentException("Username es obligatorio.", nameof(username));
        return v;
    }
    private static string NormalizeEmail(string email)
    {
        var v = (email ?? string.Empty).Trim().ToLowerInvariant();
        if (v.Length == 0) throw new ArgumentException("Email es obligatorio.", nameof(email));
        return v;
    }
    private static string RequireNotEmpty(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("El valor no puede ser vacío.", nameof(value));
        return value;
    }
    #endregion
}
