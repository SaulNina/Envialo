using System.ComponentModel.DataAnnotations;

namespace Envialo.Domain.DTOs.Auth;

public sealed record RegisterDto(
    [Required, MaxLength(100)] string FullName,
    [Required, EmailAddress]   string Email,
    [Required, MinLength(8)]   string Password,
    [Required, Phone]          string Phone,
    [Required]                 string Role   
);