
namespace dplo_shop.Models
{


  public class ApiResponseModel
  {
    public string Status { get; set; }
    public virtual AuthResponse AuthResponse { get; set; }
    public object data { get; set; }
    public ErrorResponse error { get; set; }
  }

  public class AuthResponse
  {
    public string AccessToken { get; set; }
    public int Expiresin { get; set; }
    public string SignedRequest { get; set; }
    public string UserId { get; set; }
  }

    public class ErrorResponse
    {
        public ErrorResponse(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}
