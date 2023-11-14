using Microsoft.AspNetCore.Identity;

namespace HandiPapi.DataAccess
{
    public class ApiUser: IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
