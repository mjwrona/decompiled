// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.BlobExceptionMapping
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using Microsoft.VisualStudio.Services.BlobStore.Common.Exceptions;
using Microsoft.VisualStudio.Services.BlobStore.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Common.MultiDomainExceptions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  public static class BlobExceptionMapping
  {
    private static readonly Lazy<Dictionary<string, Type>> translation = new Lazy<Dictionary<string, Type>>(new Func<Dictionary<string, Type>>(BlobExceptionMapping.InitClientTranslatedExceptions));
    private static readonly Lazy<Dictionary<Type, HttpStatusCode>> errorMap = new Lazy<Dictionary<Type, HttpStatusCode>>(new Func<Dictionary<Type, HttpStatusCode>>(BlobExceptionMapping.InitServerErrorMap));

    public static Dictionary<string, Type> ClientTranslatedExceptions => BlobExceptionMapping.translation.Value;

    public static Dictionary<Type, HttpStatusCode> ServerErrorMap => BlobExceptionMapping.errorMap.Value;

    private static Dictionary<string, Type> InitClientTranslatedExceptions() => new Dictionary<string, Type>()
    {
      {
        "BlobNotFoundException",
        typeof (BlobNotFoundException)
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
        "ClientToolNotFoundException",
        typeof (ClientToolNotFoundException)
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
        "InvalidDomainIdException",
        typeof (InvalidDomainIdException)
      },
      {
        "DomainNotFoundException",
        typeof (DomainNotFoundException)
      },
      {
        "DomainIdDeserializationException",
        typeof (DomainIdDeserializationException)
      },
      {
        "UnauthorizedBlobstoreRequestException",
        typeof (UnauthorizedBlobstoreRequestException)
      },
      {
        "InvalidSessionOperationException",
        typeof (InvalidSessionOperationException)
      },
      {
        "SessionOrPartNotFoundException",
        typeof (SessionOrPartNotFoundException)
      },
      {
        "InvalidPartsRangeException",
        typeof (InvalidPartsRangeException)
      }
    };

    private static Dictionary<Type, HttpStatusCode> InitServerErrorMap() => new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (BlobNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (ArgumentException),
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
        typeof (ClientToolNotFoundException),
        HttpStatusCode.NotFound
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
        typeof (InvalidDomainIdException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (DomainNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (DomainIdDeserializationException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidBlobTypeException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ContentLengthTooLargeException),
        HttpStatusCode.RequestEntityTooLarge
      },
      {
        typeof (ContentLengthMissingException),
        HttpStatusCode.LengthRequired
      },
      {
        typeof (ContentMissingException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (UnauthorizedBlobstoreRequestException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (InvalidSessionOperationException),
        HttpStatusCode.Conflict
      },
      {
        typeof (SessionOrPartNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (InvalidPartsRangeException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (MaxRequestContentLengthException),
        HttpStatusCode.BadRequest
      }
    };
  }
}
