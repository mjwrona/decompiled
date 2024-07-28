// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.StatusCodes
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

namespace Microsoft.Azure.Documents
{
  internal enum StatusCodes
  {
    Ok = 200, // 0x000000C8
    Created = 201, // 0x000000C9
    Accepted = 202, // 0x000000CA
    NoContent = 204, // 0x000000CC
    MultiStatus = 207, // 0x000000CF
    NotModified = 304, // 0x00000130
    BadRequest = 400, // 0x00000190
    StartingErrorCode = 400, // 0x00000190
    Unauthorized = 401, // 0x00000191
    Forbidden = 403, // 0x00000193
    NotFound = 404, // 0x00000194
    MethodNotAllowed = 405, // 0x00000195
    RequestTimeout = 408, // 0x00000198
    Conflict = 409, // 0x00000199
    Gone = 410, // 0x0000019A
    PreconditionFailed = 412, // 0x0000019C
    RequestEntityTooLarge = 413, // 0x0000019D
    Locked = 423, // 0x000001A7
    FailedDependency = 424, // 0x000001A8
    TooManyRequests = 429, // 0x000001AD
    RetryWith = 449, // 0x000001C1
    InternalServerError = 500, // 0x000001F4
    BadGateway = 502, // 0x000001F6
    ServiceUnavailable = 503, // 0x000001F7
    OperationPaused = 1200, // 0x000004B0
    OperationCancelled = 1201, // 0x000004B1
  }
}
