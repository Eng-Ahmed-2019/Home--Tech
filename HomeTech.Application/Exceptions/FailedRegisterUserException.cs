using System.Net;

namespace HomeTech.Application.Exceptions
{
    public class FailedRegisterUserException : BaseApplicationException
    {
        public FailedRegisterUserException(string message)
            : base(message, HttpStatusCode.BadRequest)
        { }
    }
}