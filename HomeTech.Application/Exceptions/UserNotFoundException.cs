using System.Net;

namespace HomeTech.Application.Exceptions
{
    public class UserNotFoundException : BaseApplicationException
    {
        public UserNotFoundException(string m)
            : base(m, HttpStatusCode.NotFound)
        { }
    }
}