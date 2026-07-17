using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.DTO_s.Identity
{
    public class UserDto
    {
        public string Displayname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
