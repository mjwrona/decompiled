// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedApiController
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.Server.Provenance;
using Microsoft.VisualStudio.Services.Feed.Server.Upstream;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Net;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class FeedApiController : TfsProjectApiController
  {
    public static readonly string Area = "Feed";
    public static readonly Dictionary<Type, HttpStatusCode> HttpExceptionMapping = new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (FormatException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidFeedNameException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidFeedDescriptionException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (NotSupportedException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ViewInvalidRoleException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (PermissionInvalidRoleException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ProtocolNotSupportedException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidPackageVersionException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidAliasException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidPackageIdException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (PackageLimitExceededException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidUserInputException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (PackageVersionDeletedException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ArgumentOutOfRangeException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidFeedViewNameException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ArgumentException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (IncompatibleQueryParametersException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidUpstreamSourceException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (VssPropertyValidationException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ServiceConnectionNotSupportedException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (UnauthorizedAccessException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (FeedNeedsPermissionsException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (UnallowedUserIdException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (FeatureDisabledException),
        HttpStatusCode.NotFound
      },
      {
        typeof (FeedIdNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (PackageNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (PackageVersionNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (ProvenanceNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (FeedViewNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (IdentityNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (FeedIsReadOnlyException),
        HttpStatusCode.MethodNotAllowed
      },
      {
        typeof (FeedNameAlreadyExistsException),
        HttpStatusCode.Conflict
      },
      {
        typeof (FeedNotReleasedException),
        HttpStatusCode.Conflict
      },
      {
        typeof (FeedViewNameAlreadyExistsException),
        HttpStatusCode.Conflict
      },
      {
        typeof (FeedViewNotReleasedException),
        HttpStatusCode.Conflict
      }
    };

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) FeedApiController.HttpExceptionMapping;

    public override string TraceArea => FeedApiController.Area;

    public override string ActivityLogArea => FeedApiController.Area;

    protected IFeedViewService ViewService => this.TfsRequestContext.GetService<IFeedViewService>();

    protected IFeedService FeedService => this.TfsRequestContext.GetService<IFeedService>();

    protected IFeedIndexService FeedIndexService => this.TfsRequestContext.GetService<IFeedIndexService>();

    protected IPackageRecycleBinService PackageRecycleBinService => this.TfsRequestContext.GetService<IPackageRecycleBinService>();

    protected IFeedMetricsService FeedMetricsService => this.TfsRequestContext.GetService<IFeedMetricsService>();

    protected IFeedChangeService FeedChangeService => this.TfsRequestContext.GetService<IFeedChangeService>();

    protected IProvenanceService ProvenanceService => this.TfsRequestContext.GetService<IProvenanceService>();

    protected IFeedRecycleBinService FeedRecycleBinService => this.TfsRequestContext.GetService<IFeedRecycleBinService>();

    protected IFeedUpstreamService FeedUpstreamService => new FeedUpstreamServiceFactory(this.TfsRequestContext).Get();

    protected ITeamFoundationJobService JobService => this.TfsRequestContext.GetService<ITeamFoundationJobService>();
  }
}
