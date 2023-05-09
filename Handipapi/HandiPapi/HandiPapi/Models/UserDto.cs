using System.ComponentModel.DataAnnotations;

namespace HandiPapi.Models
{
    public class UserDto: LoginDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }

        public ICollection<string> Roles { get; set; }
    }

    public class LoginDto 
    { 
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [StringLength(15, ErrorMessage = "Your Password is Limited to {2} to {1} characters", MinimumLength = 6)]

        public string Password { get; set; }
    }


}
