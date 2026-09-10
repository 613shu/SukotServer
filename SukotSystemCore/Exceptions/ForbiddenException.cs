using System;

namespace SukotSystemCore.Exceptions
{
    // 403: the caller is authenticated and holds a valid role, but is not allowed to act on
    // this specific resource (e.g. a Rabbi trying to complete an Order claimed by a different
    // Rabbi). Distinct from UnauthorizedAccessException (401 - missing/invalid credentials) -
    // before this exception type existed, nothing in the project could produce a 403 at all,
    // even though the REST checklist (requirement 1) explicitly lists it as a required status
    // code.
    public class ForbiddenException : Exception
    {
        public ForbiddenException(string message) : base(message) { }
    }
}
