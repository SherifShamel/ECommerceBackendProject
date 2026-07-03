using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Common
{
    public record Error(string code, string description, ErrorType type = ErrorType.Failure)
    {
        public static Error Failure(string code = "General.Failure", string description = "General Failure Desc") => new Error(code, description, ErrorType.Failure);
        public static Error Validation(string code = "General.Validation", string description = "General Validation Desc") => new Error(code, description, ErrorType.Validation);
        public static Error NotFound(string code = "General.NotFound", string description = "General NotFound Desc") => new Error(code, description, ErrorType.NotFound);
        public static Error Conflict(string code = "General.Conflict", string description = "General Conflict Desc") => new Error(code, description, ErrorType.Conflict);
        public static Error UnAuthorized(string code = "General.UnAuthorized", string description = "General UnAuthorized Desc") => new Error(code, description, ErrorType.UnAuthorized);
        public static Error Forbidden(string code = "General.Forbidden", string description = "General Forbidden Desc") => new Error(code, description, ErrorType.Forbidden);
        public static Error InvalidCredentials(string code = "General.InvalidCredintials", string description = "General InvalidCredintials Desc") => new Error(code, description, ErrorType.InvalidCredintials);
    }
}
