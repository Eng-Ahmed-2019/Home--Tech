using System.Net;

namespace HomeTech.Application.Exceptions
{
    public class ContentTypeException : BaseApplicationException
    {
        public ContentTypeException(string m)
            : base(m, HttpStatusCode.BadRequest) { }
    }
}