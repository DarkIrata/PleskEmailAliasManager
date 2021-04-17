using System;

namespace PleskEmailAliasManager.Data
{
    public class ErrorResult
    {
        public ErrorCode ErrorCode { get; }

        public string Message { get; }

        public Exception Exception { get; }

        public ErrorResult(ErrorCode errorCode, string message, Exception ex = null)
        {
            this.ErrorCode = errorCode;
            this.Message = message;
            this.Exception = ex;
        }

        public static ErrorResult Success() => new ErrorResult(ErrorCode.Success, null, null);
    }
}
