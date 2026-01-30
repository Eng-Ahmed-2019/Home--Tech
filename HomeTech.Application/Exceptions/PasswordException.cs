using System.Net;

namespace HomeTech.Application.Exceptions
{
    public class PasswordException : BaseApplicationException
    {
        public PasswordException(string m) :
            base(m, HttpStatusCode.NotFound)
        { }
    }
}