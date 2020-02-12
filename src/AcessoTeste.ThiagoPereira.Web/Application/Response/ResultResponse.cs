using Newtonsoft.Json;

namespace AcessoTeste.ThiagoPereira.Web.Application.Response
{
    public class ResultResponse
    {
        [JsonProperty("Status")]
        public string Status { get; set; }
        [JsonProperty("Message")]
        public string Message { get; set; }

        public static ResultResponse CreateError(string message)
        {
            return new ResultResponse
            {
                Status = "Error",
                Message = message
            };
        }
    }
}
