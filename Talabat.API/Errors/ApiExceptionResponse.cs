namespace Talabat.API.Errors
{
    public class ApiExceptionResponse : ApiResponse
    {
        public string? Details { get; set; }
        public ApiExceptionResponse(int statusCode , string? messsage = null , string? details = null):base(statusCode,messsage)  
        {
            Details = details;
        }
    }
}
