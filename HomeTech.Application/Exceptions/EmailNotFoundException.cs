using System.Net;

namespace HomeTech.Application.Exceptions
{
    public class EmailNotFoundException : BaseApplicationException
    {
        public EmailNotFoundException(string m)
            : base(m, HttpStatusCode.NotFound) { }
    }
}