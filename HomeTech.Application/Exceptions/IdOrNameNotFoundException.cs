using System.Net;

namespace HomeTech.Application.Exceptions
{
    public class IdOrNameNotFoundException : BaseApplicationException
    {
        public IdOrNameNotFoundException(string message)
            : base(message, HttpStatusCode.NotFound)
        {
        }
    }
}