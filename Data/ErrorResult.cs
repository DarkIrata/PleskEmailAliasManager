using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PleskEmailAliasManager.Enums;

namespace PleskEmailAliasManager.Data
{
    internal class ErrorResult
    {
        public ErrorCode ErrorCode { get; }

        public string? Message { get; }

        public Exception? Exception { get; }

        public bool Successfully => this.ErrorCode == ErrorCode.Success;

        public ErrorResult(ErrorCode errorCode, string? message, Exception? ex = null)
        {
            this.ErrorCode = errorCode;
            this.Message = message;
            this.Exception = ex;
        }

        public ErrorResult(ErrorCode errorCode, Exception? ex = null)
            : this(errorCode, ex?.Message, ex)
        {
        }

        public static ErrorResult Success => new(ErrorCode.Success, null, null);
    }
}
