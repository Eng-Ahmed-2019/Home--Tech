using System.Net;

namespace HomeTech.Application.Exceptions
{
    public class unAuthorizedException : BaseApplicationException
    {
        public unAuthorizedException(string m) :
            base(m, HttpStatusCode.Unauthorized)
        { }
    }
}