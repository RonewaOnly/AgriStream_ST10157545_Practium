using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgriStream_ST10157545_Practium.Models
{
    public static class AppRoles
    {
        public const string Farmer = "Farmer";
        public const string Agronomist = "Agronomist";
        public const string Admin = "Admin";


        public const string AllRoles = $"{Farmer},{Agronomist},{Admin}";
    }


    public class Role
    {
        public int RoleId { get; set; }


        [MaxLength(30)]
        public string RoleName { get; set; } = string.Empty;
    }


    public class User
    {
        public int UserId { get; set; }


        [MaxLength(60)]
        public string Username { get; set; } = string.Empty;


        [MaxLength(256)]
        public string PasswordHash { get; set; } = string.Empty;


        public int RoleId { get; set; }


        public Role? Role { get; set; }
    }


    [Table("DeviceRegistry")]
    public class Device
    {
        [Key]
        [MaxLength(50)]
        public string DeviceId { get; set; } = string.Empty;


        [MaxLength(50)]
        public string FieldId { get; set; } = string.Empty;


        [MaxLength(30)]
        public string OperationalStatus { get; set; } = "Idle";


        [MaxLength(50)]
        public string? LastCommand { get; set; }


        public DateTime LastUpdatedUtc { get; set; } = DateTime.UtcNow;
    }


}
