// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Exceptions.NuGetExceptionMappings
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.Data.OData;
using Microsoft.VisualStudio.Services.NuGet.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Exceptions
{
  public static class NuGetExceptionMappings
  {
    public static Dictionary<Type, HttpStatusCode> Mappings { get; } = PackagingExceptionMappings.WithOverrides((IDictionary<Type, HttpStatusCode>) new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (ArgumentFormatException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidPackageException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidPushRequestException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (NuGetApiKeyRequiredException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (PackageContentBlobNotFoundException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (RequiredArgumentMissingException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ODataException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ViewOperationException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (TooManyVersionsException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (NupkgIngestionNotAllowedException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (UnsupportedODataFilterException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (UnsupportedODataOrderByException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (PackageNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (PackageSubresourceNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (PathPartMissingException),
        HttpStatusCode.NotFound
      },
      {
        typeof (ReadOnlyViewOperationException),
        HttpStatusCode.MethodNotAllowed
      },
      {
        typeof (NoNupkgForDropBackedPackageException),
        HttpStatusCode.NotAcceptable
      },
      {
        typeof (ServiceCanceledDueToClientException),
        HttpStatusCode.RequestTimeout
      },
      {
        typeof (PackageAlreadyExistsException),
        HttpStatusCode.Conflict
      },
      {
        typeof (PackageExistsAsDeletedException),
        HttpStatusCode.Conflict
      },
      {
        typeof (TooLargePackagePushException),
        HttpStatusCode.RequestEntityTooLarge
      },
      {
        typeof (UnsupportedMediaTypeException),
        HttpStatusCode.UnsupportedMediaType
      }
    }).ToDictionary<KeyValuePair<Type, HttpStatusCode>, Type, HttpStatusCode>((Func<KeyValuePair<Type, HttpStatusCode>, Type>) (kv => kv.Key), (Func<KeyValuePair<Type, HttpStatusCode>, HttpStatusCode>) (kv => kv.Value));
  }
}
