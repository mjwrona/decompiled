// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream.FeedViewVisibilityValidatingHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream
{
  public class FeedViewVisibilityValidatingHandler : IFeedViewVisibilityValidator
  {
    private readonly IExecutionEnvironment executionEnvironment;
    private readonly IVssRequestContext requestContext;
    private readonly IFeedService feedService;
    private readonly IUpstreamVerificationHelper upstreamVerification;

    public FeedViewVisibilityValidatingHandler(
      IExecutionEnvironment executionEnvironment,
      IVssRequestContext requestContext,
      IFeedService feedService,
      IUpstreamVerificationHelper upstreamVerification)
    {
      this.executionEnvironment = executionEnvironment;
      this.requestContext = requestContext;
      this.feedService = feedService;
      this.upstreamVerification = upstreamVerification;
    }

    public static IFeedViewVisibilityValidator Bootstrap(IVssRequestContext requestContext)
    {
      IExecutionEnvironment environmentFacade = requestContext.GetExecutionEnvironmentFacade();
      IFeatureFlagService featureFlagFacade = requestContext.GetFeatureFlagFacade();
      return (IFeedViewVisibilityValidator) new FeedViewVisibilityValidatingHandler(environmentFacade, requestContext, (IFeedService) new FeedServiceFacade(requestContext.Elevate()), (IUpstreamVerificationHelper) new UpstreamVerificationHelper(environmentFacade, featureFlagFacade));
    }

    public async Task Validate(IFeedRequest request, UpstreamSource upstreamSource)
    {
      Guid? nullable = upstreamSource.InternalUpstreamCollectionId;
      Guid hostId = this.executionEnvironment.HostId;
      if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != hostId ? 1 : 0) : 0) : 1) != 0)
        return;
      nullable = upstreamSource.InternalUpstreamCollectionId;
      Guid collectionId = nullable ?? Guid.Empty;
      nullable = upstreamSource.GetProjectId();
      this.upstreamVerification.ThrowIfFeedIsNotWidelyVisible(this.requestContext, await this.feedService.GetFeedAsync(nullable ?? Guid.Empty, upstreamSource.GetFullyQualifiedFeedId()), this.executionEnvironment.GetOrganizationAadTenantId(), collectionId);
    }
  }
}
