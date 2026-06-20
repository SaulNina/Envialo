using System.ComponentModel.DataAnnotations;

namespace Envialo.Domain.DTOs.Auth;

public sealed record LoginDto(
    [Required, EmailAddress] string Email,
    [Required]               string Password
);