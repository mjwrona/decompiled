// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.ErrorHandling.ErrorCodes
// Assembly: Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7AAC33C3-FEBB-470D-AB81-20ABE7F3618F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.ActiveDirectory.GraphClient.ErrorHandling
{
  public static class ErrorCodes
  {
    private static Dictionary<string, Type> exceptionCodeMap = new Dictionary<string, Type>()
    {
      {
        "Authentication_MissingOrMalformed",
        typeof (AuthenticationException)
      },
      {
        "Authentication_Unauthorized",
        typeof (AuthenticationException)
      },
      {
        "Authentication_UnsupportedTokenType",
        typeof (AuthenticationException)
      },
      {
        "Authentication_ExpiredToken",
        typeof (ExpiredTokenException)
      },
      {
        "Authorization_IdentityDisabled",
        typeof (AuthorizationException)
      },
      {
        "Authorization_IdentityNotFound",
        typeof (AuthorizationException)
      },
      {
        "Authorization_RequestDenied",
        typeof (AuthorizationException)
      },
      {
        "Request_ResourceNotFound",
        typeof (ObjectNotFoundException)
      },
      {
        "Directory_ObjectNotFound",
        typeof (ObjectNotFoundException)
      },
      {
        "Directory_ExpiredPageToken",
        typeof (PageNotAvailableException)
      },
      {
        "Directory_ReplicaUnavailable",
        typeof (ServiceUnavailableException)
      },
      {
        "Request_DataContractVersionMissing",
        typeof (InvalidApiVersionException)
      },
      {
        "Request_InvalidDataContractVersion",
        typeof (InvalidApiVersionException)
      },
      {
        "Request_InvalidBackfillMultiTenant",
        typeof (InvalidServicePrincipalTypeException)
      },
      {
        "Request_NoApplicationAssociated",
        typeof (NoApplicationAssociatedException)
      },
      {
        "Headers_HeaderNotSupported",
        typeof (InvalidHeaderException)
      },
      {
        "Request_BadRequest",
        typeof (BadRequestException)
      },
      {
        "Request_InvalidRequestUrl",
        typeof (BadRequestException)
      },
      {
        "Request_UnsupportedQuery",
        typeof (UnsupportedQueryException)
      },
      {
        "Request_MultipleObjectsWithSameKeyValue",
        typeof (DuplicateObjectException)
      },
      {
        "Service_InternalServerError",
        typeof (InternalServerErrorException)
      },
      {
        "Directory_ConcurrencyViolation",
        typeof (ServiceUnavailableException)
      },
      {
        "Directory_QuotaExceeded",
        typeof (QuotaExceededException)
      },
      {
        "Request_ThrottledPermanently",
        typeof (RequestThrottledException)
      },
      {
        "Request_ThrottledTemporarily",
        typeof (RequestThrottledException)
      }
    };

    public static Dictionary<string, Type> ExceptionErrorCodeMap => ErrorCodes.exceptionCodeMap;
  }
}
