using System.Net;

namespace HomeTech.Application.Exceptions
{
    public class SizeFileException : BaseApplicationException
    {
        public SizeFileException(string m)
            : base(m, HttpStatusCode.BadRequest) { }
    }
}