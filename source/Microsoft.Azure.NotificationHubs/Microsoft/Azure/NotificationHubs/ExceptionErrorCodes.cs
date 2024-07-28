// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.ExceptionErrorCodes
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

namespace Microsoft.Azure.NotificationHubs
{
  public enum ExceptionErrorCodes
  {
    BadRequest = 40000, // 0x00009C40
    UnauthorizedGeneric = 40100, // 0x00009CA4
    NoTransportSecurity = 40101, // 0x00009CA5
    MissingToken = 40102, // 0x00009CA6
    InvalidSignature = 40103, // 0x00009CA7
    InvalidAudience = 40104, // 0x00009CA8
    MalformedToken = 40105, // 0x00009CA9
    ExpiredToken = 40106, // 0x00009CAA
    AudienceNotFound = 40107, // 0x00009CAB
    ExpiresOnNotFound = 40108, // 0x00009CAC
    IssuerNotFound = 40109, // 0x00009CAD
    SignatureNotFound = 40110, // 0x00009CAE
    ForbiddenGeneric = 40300, // 0x00009D6C
    EndpointNotFound = 40400, // 0x00009DD0
    InvalidDestination = 40401, // 0x00009DD1
    NamespaceNotFound = 40402, // 0x00009DD2
    StoreLockLost = 40500, // 0x00009E34
    SqlFiltersExceeded = 40501, // 0x00009E35
    CorrelationFiltersExceeded = 40502, // 0x00009E36
    SubscriptionsExceeded = 40503, // 0x00009E37
    UpdateConflict = 40504, // 0x00009E38
    EventHubAtFullCapacity = 40505, // 0x00009E39
    ConflictGeneric = 40900, // 0x00009FC4
    ConflictOperationInProgress = 40901, // 0x00009FC5
    EntityGone = 41000, // 0x0000A028
    UnspecifiedInternalError = 50000, // 0x0000C350
    DataCommunicationError = 50001, // 0x0000C351
    InternalFailure = 50002, // 0x0000C352
    ProviderUnreachable = 50003, // 0x0000C353
    ServerBusy = 50004, // 0x0000C354
    BadGatewayFailure = 50200, // 0x0000C418
    GatewayTimeoutFailure = 50400, // 0x0000C4E0
    UnknownExceptionDetail = 60000, // 0x0000EA60
  }
}
