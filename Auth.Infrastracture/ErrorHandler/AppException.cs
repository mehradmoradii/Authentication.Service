namespace Auth.Infrastracture.ErrorHandler
{
    public class AppException : Exception
    {
        public int StatusCode { get; set; }
        public string ErrorMessage { get; set; }

        public AppException(string message = "An error occurred")
            : base(message)
        {
            StatusCode = 400;
            ErrorMessage = message;
        }
    }
}
