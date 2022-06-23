namespace PleskEmailAliasManager.Enums
{
    internal enum ErrorCode
    {
        Success = 0,

        ExternalError = 100,
        SSLException = 110,
        InternalError = 200,
        RemoteClosedConnection = 201,

        FailedSerializePackage = 1000,
        RequestFailed = 2000,
    }
}
