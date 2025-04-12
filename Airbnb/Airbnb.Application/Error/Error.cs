namespace Airbnb.Application.Errors
{
    public class Error
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public int StatusCode { get; }
        public Error(string code, string desc, int status)
        {
            Code = code;
            Description = desc;
            StatusCode = status;
        }
    }
}
