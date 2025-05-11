using CORE.APP.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CORE.APP.Features
{
    public class Response
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }

        public Response()
        {
            IsSuccessful = true;
            Message = string.Empty;
        }

        public Response(bool isSuccessful, string message)
        {
            IsSuccessful = isSuccessful;
            Message = message;
        }
    }

    public class Response<T> : Response
    {
        public T Data { get; set; }

        public Response() : base()
        {
            Data = default;
        }

        public Response(bool isSuccessful, string message) : base(isSuccessful, message)
        {
            Data = default;
        }

        public Response(bool isSuccessful, string message, T data) : base(isSuccessful, message)
        {
            Data = data;
        }

    
    }

    public class CommandResponse : Entity
    {
        public bool IsSuccessful { get; }
        public string Message { get; }

        public CommandResponse(bool isSuccessful, string message = "", int id = 0) : base(id)
        {
            IsSuccessful = isSuccessful;
            Message = message;
        }
    }

    public class QueryResponse : Entity
    { }


}