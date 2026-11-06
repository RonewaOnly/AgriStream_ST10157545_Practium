using System.ComponentModel.DataAnnotations;

namespace AgriStream_ST10157545_Practium.Models
{
    public class Users
    {
        [Key]
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
