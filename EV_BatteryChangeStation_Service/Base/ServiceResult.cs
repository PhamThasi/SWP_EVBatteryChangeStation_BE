using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Service.Base
{
    public class ServiceResult : IServiceResult
    {
        public int Status { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? Errors { get; set; }
        public Enum? ErrorCode { get; set; }

        public ServiceResult()
        {
            Status = -1;
            Message = "Action Fail";
        }

        public ServiceResult(int status, string message)
        {
            Status = status;
            Message = message;
        }

        public ServiceResult(int status, string message, object data)
        {
            Status = status;
            Message = message;
            Data = data;
        }

        public ServiceResult(int status, string message, List<string> errors)
        {
            Status = status;
            Message = message;
            Errors = errors;
        }
        public ServiceResult(int status, string message, object data, Enum errorCode)
        {
            Status = status;
            Message = message;
            Data = data;
            ErrorCode = errorCode;
        }

    }
}
