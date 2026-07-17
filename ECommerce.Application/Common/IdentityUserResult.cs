using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Common
{
    public class IdentityUserResult
    {
        public IdentityUserResult(string id, string email, string userName, string displayname)
        {
            Id = id;
            Email = email;
            UserName = userName;
            Displayname = displayname;
        }

        public string Id { get; set; } = default!;

        public string Email { get; set; } = default!;

        public string UserName { get; set; } = default!;

        public string Displayname { get; set; } = default!;
    }
}
