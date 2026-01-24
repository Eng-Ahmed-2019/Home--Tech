using System.Net;

namespace HomeTech.Application.Exceptions
{
    public abstract class BaseApplicationException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }

        protected BaseApplicationException(string m, HttpStatusCode statuscode)
            : base(m)
        { this.StatusCode = statuscode; }
    }
}