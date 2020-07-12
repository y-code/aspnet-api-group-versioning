using System;
namespace CodeSample.Models
{
    public class ResponseModel
    {
        public string Message { get; set; }
        public ResponseModel(string message)
            => Message = message;
    }
}
