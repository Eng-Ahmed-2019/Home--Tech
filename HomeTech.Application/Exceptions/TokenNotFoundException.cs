using System.Net;

namespace HomeTech.Application.Exceptions
{
    public class TokenNotFoundException : BaseApplicationException
    {
        public TokenNotFoundException(string m) :
            base(m, HttpStatusCode.NotFound)
        { }
    }
}