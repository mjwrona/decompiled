// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Exceptions.NpmExceptionMappings
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Microsoft.VisualStudio.Services.Npm.Server.Exceptions
{
  public static class NpmExceptionMappings
  {
    public static readonly Dictionary<Type, HttpStatusCode> HttpExceptionMapping = PackagingExceptionMappings.WithOverrides((IDictionary<Type, HttpStatusCode>) new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (ArgumentFormatException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions.InvalidPackageException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidPackageTarballException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidPublishException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidDownloadRequestException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidPackageJsonException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidRequestException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidDistTagException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (DistTagNotFoundException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions.TooManyVersionsException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (PackageVersionAlreadyExistsException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (PackageAlreadyExistsException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (PackageExistsAsDeletedException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (PackageNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (FileNotFoundInPackageException),
        HttpStatusCode.NotFound
      },
      {
        typeof (PublicUpstreamTgzNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions.ReadOnlyViewOperationException),
        HttpStatusCode.MethodNotAllowed
      },
      {
        typeof (InvalidFeedOperationException),
        HttpStatusCode.MethodNotAllowed
      },
      {
        typeof (Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions.ServiceCanceledDueToClientException),
        HttpStatusCode.RequestTimeout
      },
      {
        typeof (RevisionMismatchException),
        HttpStatusCode.Conflict
      },
      {
        typeof (MultipleVersionsUnpublishedException),
        HttpStatusCode.Conflict
      },
      {
        typeof (PackageTooLargeException),
        HttpStatusCode.RequestEntityTooLarge
      },
      {
        typeof (Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions.UnsupportedMediaTypeException),
        HttpStatusCode.UnsupportedMediaType
      },
      {
        typeof (ArgumentNullException),
        HttpStatusCode.InternalServerError
      },
      {
        typeof (InvalidUpstreamPackage),
        HttpStatusCode.InternalServerError
      },
      {
        typeof (UpstreamUnavailableException),
        HttpStatusCode.InternalServerError
      },
      {
        typeof (DeletePackageWithMultipleVersionsNotSupportedException),
        HttpStatusCode.NotImplemented
      },
      {
        typeof (PublicUpstreamFailureException),
        HttpStatusCode.BadGateway
      },
      {
        typeof (UnknownUpstreamErrorException),
        HttpStatusCode.BadGateway
      }
    }).ToDictionary<KeyValuePair<Type, HttpStatusCode>, Type, HttpStatusCode>((Func<KeyValuePair<Type, HttpStatusCode>, Type>) (kv => kv.Key), (Func<KeyValuePair<Type, HttpStatusCode>, HttpStatusCode>) (kv => kv.Value));
  }
}
