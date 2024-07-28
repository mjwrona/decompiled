// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.Exceptions.HostDomainExceptionMapping
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Content.Common.MultiDomainExceptions;
using System;
using System.Collections.Generic;
using System.Net;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi.Exceptions
{
  public static class HostDomainExceptionMapping
  {
    private static readonly Lazy<Dictionary<string, Type>> translation = new Lazy<Dictionary<string, Type>>(new Func<Dictionary<string, Type>>(HostDomainExceptionMapping.InitClientTranslatedExceptions));
    private static readonly Lazy<Dictionary<Type, HttpStatusCode>> errorMap = new Lazy<Dictionary<Type, HttpStatusCode>>(new Func<Dictionary<Type, HttpStatusCode>>(HostDomainExceptionMapping.InitServerErrorMap));

    public static Dictionary<string, Type> ClientTranslatedExceptions => HostDomainExceptionMapping.translation.Value;

    public static Dictionary<Type, HttpStatusCode> ServerErrorMap => HostDomainExceptionMapping.errorMap.Value;

    private static Dictionary<string, Type> InitClientTranslatedExceptions() => new Dictionary<string, Type>()
    {
      {
        "InvalidDomainIdException",
        typeof (InvalidDomainIdException)
      },
      {
        "InvalidDomainShardListException",
        typeof (InvalidDomainShardListException)
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
        "InvalidProjectDomainIdException",
        typeof (InvalidProjectDomainIdException)
      },
      {
        "ProjectDomainIdAlreadyExistsException",
        typeof (ProjectDomainIdAlreadyExistsException)
      },
      {
        "DuplicateDomainDataException",
        typeof (DuplicateDomainDataException)
      },
      {
        "RestrictedAccessException",
        typeof (RestrictedAccessException)
      }
    };

    private static Dictionary<Type, HttpStatusCode> InitServerErrorMap() => new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (InvalidDomainIdException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidDomainShardListException),
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
        typeof (InvalidProjectDomainIdException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ProjectDomainIdAlreadyExistsException),
        HttpStatusCode.Conflict
      },
      {
        typeof (DuplicateDomainDataException),
        HttpStatusCode.InternalServerError
      },
      {
        typeof (RestrictedAccessException),
        HttpStatusCode.Forbidden
      }
    };
  }
}
