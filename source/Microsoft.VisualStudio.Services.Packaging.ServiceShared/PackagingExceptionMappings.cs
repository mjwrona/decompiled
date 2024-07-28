// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackagingExceptionMappings
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Net;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public static class PackagingExceptionMappings
  {
    public static readonly Dictionary<Type, HttpStatusCode> HttpExceptionMapping = new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (BatchRequestLengthOutOfRangeException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (RequiredArgumentMissingException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ViewOperationException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidPackageException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidPackagesException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidVersionException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidPublishRequestException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidUserRequestException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (PotentiallyDangerousRequestException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (VssPropertyValidationException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (PackageLimitExceededException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (TooManyVersionsException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (PutBlobUsingHttpException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (IngestionProhibitedException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (CannotMixFilesFromDifferentUpstreamsException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ArtifactBillingException),
        HttpStatusCode.PaymentRequired
      },
      {
        typeof (FeedNeedsPermissionsException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (UnauthorizedAccessException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (FeedIdNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (Microsoft.VisualStudio.Services.Feed.WebApi.PackageNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (PackageDeletedException),
        HttpStatusCode.NotFound
      },
      {
        typeof (FeedViewNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (FeatureDisabledException),
        HttpStatusCode.NotFound
      },
      {
        typeof (FeatureOffException),
        HttpStatusCode.NotFound
      },
      {
        typeof (PackageVersionNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (UpstreamSourceNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (InvalidUpstreamSourceDuplicateVersionsException),
        HttpStatusCode.NotFound
      },
      {
        typeof (ProvenanceSessionNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (UnknownPackagingProtocolException),
        HttpStatusCode.NotFound
      },
      {
        typeof (HttpMethodNotAllowedException),
        HttpStatusCode.MethodNotAllowed
      },
      {
        typeof (ReadOnlyFeedOperationException),
        HttpStatusCode.MethodNotAllowed
      },
      {
        typeof (ReadOnlyViewOperationException),
        HttpStatusCode.MethodNotAllowed
      },
      {
        typeof (ServiceCanceledDueToClientException),
        HttpStatusCode.RequestTimeout
      },
      {
        typeof (PackageNotDeletedException),
        HttpStatusCode.Conflict
      },
      {
        typeof (PackagePermanentlyDeletedException),
        HttpStatusCode.Conflict
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
        typeof (PackageBlockedPublishException),
        HttpStatusCode.Conflict
      },
      {
        typeof (VssDeprecatedApiException),
        HttpStatusCode.Gone
      },
      {
        typeof (PackageBlockedException),
        HttpStatusCode.Gone
      },
      {
        typeof (UnsupportedMediaTypeException),
        HttpStatusCode.UnsupportedMediaType
      },
      {
        typeof (ArgumentException),
        HttpStatusCode.InternalServerError
      },
      {
        typeof (ArgumentNullException),
        HttpStatusCode.InternalServerError
      },
      {
        typeof (ArgumentOutOfRangeException),
        HttpStatusCode.InternalServerError
      },
      {
        typeof (FormatException),
        HttpStatusCode.InternalServerError
      },
      {
        typeof (NotSupportedException),
        HttpStatusCode.InternalServerError
      },
      {
        typeof (UpstreamUnexpectedPackageDataException),
        HttpStatusCode.BadGateway
      },
      {
        typeof (UpstreamProjectDoesNotExistException),
        HttpStatusCode.BadGateway
      },
      {
        typeof (UpstreamBlockedException),
        HttpStatusCode.BadGateway
      },
      {
        typeof (FeatureReadOnlyException),
        HttpStatusCode.ServiceUnavailable
      },
      {
        typeof (RequestCanceledException),
        (HttpStatusCode) 0
      }
    };

    public static Dictionary<Type, HttpStatusCode> WithOverrides(
      IDictionary<Type, HttpStatusCode> overrides)
    {
      Dictionary<Type, HttpStatusCode> dictionary = new Dictionary<Type, HttpStatusCode>((IDictionary<Type, HttpStatusCode>) PackagingExceptionMappings.HttpExceptionMapping);
      foreach (KeyValuePair<Type, HttpStatusCode> keyValuePair in (IEnumerable<KeyValuePair<Type, HttpStatusCode>>) overrides)
        dictionary[keyValuePair.Key] = keyValuePair.Value;
      return dictionary;
    }
  }
}
