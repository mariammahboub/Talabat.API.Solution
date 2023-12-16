namespace Talabat.API.Errors
{
    public class ApiResponse
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public ApiResponse(int statusCode , string? message = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);
        }

        private string? GetDefaultMessageForStatusCode(int statusCode)
        {
            return StatusCode switch
            {
                400 => "Bad Request",
                404 => "Not Found",
                401 => "Un Authorize",
                500 => "Dark Error",
                _ => null,
            };
        }
    }
}
