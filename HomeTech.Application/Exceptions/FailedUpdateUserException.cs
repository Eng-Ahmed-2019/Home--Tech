using System.Net;

namespace HomeTech.Application.Exceptions
{
    public class FailedUpdateUserException : BaseApplicationException
    {
        public FailedUpdateUserException(string m) :
            base(m, HttpStatusCode.NotModified)
        { }
    }
}