using System.Collections.Generic;

namespace Models.Dtos
{
    public class UserDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
    }
}