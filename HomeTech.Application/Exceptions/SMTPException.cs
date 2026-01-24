using System.Net;

namespace HomeTech.Application.Exceptions
{
    public class SMTPException : BaseApplicationException
    {
        public SMTPException(string m)
            : base(m, HttpStatusCode.ServiceUnavailable)
        { }
    }
}