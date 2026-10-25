using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Application.DTOs
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string? Email { get; set; }
        public string? AvatarUrl { get; set; }
    }
}
