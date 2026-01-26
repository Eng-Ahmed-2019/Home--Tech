using System.Net;

namespace HomeTech.Application.Exceptions
{
    public class ForbiddenException : BaseApplicationException
    {
        public ForbiddenException(string m) :
            base(m, HttpStatusCode.Forbidden)
        { }
    }
}