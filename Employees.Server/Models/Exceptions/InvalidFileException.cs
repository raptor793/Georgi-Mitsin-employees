namespace Employees.Server.Models.Exceptions
{
    public class InvalidFileException : Exception
    {
        public int StatusCode { get; set; }

        public InvalidFileException()
        {
        }

        public InvalidFileException(int statusCode, string message)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public InvalidFileException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
