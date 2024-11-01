using System.Net;

namespace ItemWorks.Api.Shared.Uitls
{
    public class ResponseApi<T>
    {
        public T Data { get; set; }
        public string Message { get; set; }
        public HttpStatusCode Code { get; set; }

        public ResponseApi(T data, string message, HttpStatusCode code)
        {
            Data = data;
            Message = message;
            Code = code;
        }

        public static ResponseApi<T> Success(T data)
        {
            return new ResponseApi<T>(data, string.Empty, HttpStatusCode.OK);
        }

        public static ResponseApi<T> Fail(string message, HttpStatusCode code)
        {
            return new ResponseApi<T>(default, message, code);
        }
    }
}
