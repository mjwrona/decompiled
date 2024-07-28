// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Symbol.WebApi.SymbolExceptionMapping
// Assembly: Microsoft.VisualStudio.Services.Symbol.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52CDDA61-EF2D-4AD8-A25B-09A8F04FE8C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Symbol.WebApi.dll

using Microsoft.VisualStudio.Services.BlobStore.Common.Exceptions;
using Microsoft.VisualStudio.Services.Content.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Symbol.WebApi
{
  public static class SymbolExceptionMapping
  {
    private static Lazy<Dictionary<string, Type>> translation = new Lazy<Dictionary<string, Type>>(new Func<Dictionary<string, Type>>(SymbolExceptionMapping.InitClientTranslatedExceptions));
    private static Lazy<Dictionary<Type, HttpStatusCode>> errorMap = new Lazy<Dictionary<Type, HttpStatusCode>>(new Func<Dictionary<Type, HttpStatusCode>>(SymbolExceptionMapping.InitServerErrorMap));

    public static Dictionary<string, Type> ClientTranslatedExceptions => SymbolExceptionMapping.translation.Value;

    public static Dictionary<Type, HttpStatusCode> ServerErrorMap => SymbolExceptionMapping.errorMap.Value;

    private static Dictionary<string, Type> InitClientTranslatedExceptions() => new Dictionary<string, Type>()
    {
      {
        "ClientNotFoundException",
        typeof (ClientNotFoundException)
      },
      {
        "ClientTypeInvalidException",
        typeof (ClientTypeInvalidException)
      },
      {
        "CollectionNotFoundException",
        typeof (CollectionNotFoundException)
      },
      {
        "DebugEntryExistsException",
        typeof (DebugEntryExistsException)
      },
      {
        "DebugEntryNotChunkedException",
        typeof (DebugEntryNotChunkedException)
      },
      {
        "DebugEntryNotCreatedException",
        typeof (DebugEntryNotCreatedException)
      },
      {
        "DebugEntryNotDeletedException",
        typeof (DebugEntryNotDeletedException)
      },
      {
        "DebugEntryNotFoundException",
        typeof (DebugEntryNotFoundException)
      },
      {
        "InvalidEndpointParameterException",
        typeof (InvalidEndpointParameterException)
      },
      {
        "NotAuthorizedException",
        typeof (NotAuthorizedException)
      },
      {
        "QueryParameterIsNotNumericException",
        typeof (QueryParameterIsNotNumericException)
      },
      {
        "QueryParameterMustNotBeNegativeException",
        typeof (QueryParameterMustNotBeNegativeException)
      },
      {
        "RequestExistsException",
        typeof (RequestExistsException)
      },
      {
        "RequestNameRequiredException",
        typeof (RequestNameRequiredException)
      },
      {
        "RequestNotDeletedException",
        typeof (RequestNotDeletedException)
      },
      {
        "RequestNotFoundException",
        typeof (RequestNotFoundException)
      },
      {
        "RequestSealedException",
        typeof (RequestSealedException)
      },
      {
        "RequestUpdateConflictException",
        typeof (RequestUpdateConflictException)
      },
      {
        "RequestUpdateInvalidException",
        typeof (RequestUpdateInvalidException)
      },
      {
        "PackageExtensionNotInstalledException",
        typeof (PackageExtensionNotInstalledException)
      },
      {
        "ArtifactBillingException",
        typeof (ArtifactBillingException)
      },
      {
        "PutBlobUsingHttpException",
        typeof (PutBlobUsingHttpException)
      },
      {
        "RequestListInvalidQueryParameterValuesException",
        typeof (RequestListInvalidQueryParameterValuesException)
      },
      {
        "InvalidCastException",
        typeof (InvalidCastException)
      },
      {
        "SerializationException",
        typeof (SerializationException)
      },
      {
        "ContentMissingException",
        typeof (ContentMissingException)
      }
    };

    private static Dictionary<Type, HttpStatusCode> InitServerErrorMap() => new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (ClientNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (ClientTypeInvalidException),
        HttpStatusCode.NotFound
      },
      {
        typeof (CollectionNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (DebugEntryExistsException),
        HttpStatusCode.Conflict
      },
      {
        typeof (DebugEntryNotChunkedException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (DebugEntryNotCreatedException),
        HttpStatusCode.InternalServerError
      },
      {
        typeof (DebugEntryNotDeletedException),
        HttpStatusCode.Conflict
      },
      {
        typeof (DebugEntryNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (InvalidEndpointParameterException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (NotAuthorizedException),
        HttpStatusCode.Unauthorized
      },
      {
        typeof (QueryParameterIsNotNumericException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (QueryParameterMustNotBeNegativeException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (RequestExistsException),
        HttpStatusCode.Conflict
      },
      {
        typeof (RequestNameRequiredException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (RequestNotDeletedException),
        HttpStatusCode.Conflict
      },
      {
        typeof (RequestNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (RequestSealedException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (RequestUpdateConflictException),
        HttpStatusCode.Conflict
      },
      {
        typeof (RequestUpdateInvalidException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (PackageExtensionNotInstalledException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (ArtifactBillingException),
        HttpStatusCode.PaymentRequired
      },
      {
        typeof (PutBlobUsingHttpException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (RequestListInvalidQueryParameterValuesException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidCastException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (SerializationException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ContentMissingException),
        HttpStatusCode.BadRequest
      }
    };
  }
}
