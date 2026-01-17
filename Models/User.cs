using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.Models;

public enum UserRole
{
    Administrator,
    Manager,
    Staff
}

public class User
{
    public int Id { get; set; }
    
    [Required]
    [Display(Name = "Username")]
    public string UserName { get; set; } = string.Empty;
    
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;
    
    [Required]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [Display(Name = "Role")]
    public UserRole Role { get; set; }
    
    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

