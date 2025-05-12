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

    // Modified to be generic
    public class CommandResponse : Response
    {
        public CommandResponse() : base() { }

        public CommandResponse(bool isSuccessful, string message) : base(isSuccessful, message) { }
    }

    // Generic version of CommandResponse
    public class CommandResponse<T> : Response<T>
    {
        public CommandResponse() : base() { }

        public CommandResponse(bool isSuccessful, string message) : base(isSuccessful, message) { }

        public CommandResponse(bool isSuccessful, string message, T data) : base(isSuccessful, message, data) { }
    }

    // Modified to be generic 
    public class QueryResponse : Response
    {
        public QueryResponse() : base() { }

        public QueryResponse(bool isSuccessful, string message) : base(isSuccessful, message) { }
    }

    // Generic version of QueryResponse
    public class QueryResponse<T> : Response<T>
    {
        public QueryResponse() : base() { }

        public QueryResponse(bool isSuccessful, string message) : base(isSuccessful, message) { }

        public QueryResponse(bool isSuccessful, string message, T data) : base(isSuccessful, message, data) { }
    }
}