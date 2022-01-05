using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PleskEmailAliasManager.Enums
{
    internal enum ErrorCode
    {
        Success = 0,

        ExternalError = 100,
        InternalError = 200,
        RemoteClosedConnection = 201,

        FailedSerializePackage = 1000,
        RequestFailed = 2000,
    }
}
