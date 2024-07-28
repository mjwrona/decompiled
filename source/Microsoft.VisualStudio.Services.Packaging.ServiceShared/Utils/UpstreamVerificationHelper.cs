// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils.UpstreamVerificationHelper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils
{
  public class UpstreamVerificationHelper : IUpstreamVerificationHelper
  {
    private readonly IExecutionEnvironment executionEnvironment;
    private readonly IFeatureFlagService featureFlagService;

    public UpstreamVerificationHelper(
      IExecutionEnvironment executionEnvironment,
      IFeatureFlagService featureFlagService)
    {
      this.executionEnvironment = executionEnvironment;
      this.featureFlagService = featureFlagService;
    }

    public void ThrowIfFeedIsNotWidelyVisible(
      IVssRequestContext requestContext,
      FeedCore feed,
      Guid downstreamAadTenantId,
      Guid downstreamCollectionId = default (Guid))
    {
      if (feed.View == null)
        throw new FeedNeedsPermissionsException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_InternalUpstreamToBaseFeedNotPermitted());
      if (this.executionEnvironment.IsHosted() && feed.IsPublicFeed())
        return;
      FeedVisibility? visibility1 = feed.View.Visibility;
      FeedVisibility feedVisibility1 = FeedVisibility.Organization;
      if (visibility1.GetValueOrDefault() == feedVisibility1 & visibility1.HasValue && this.executionEnvironment.IsOrganizationAadBacked() && this.executionEnvironment.GetOrganizationAadTenantId() == downstreamAadTenantId)
        return;
      FeedVisibility? visibility2 = feed.View.Visibility;
      FeedVisibility feedVisibility2 = FeedVisibility.Collection;
      if (visibility2.GetValueOrDefault() == feedVisibility2 & visibility2.HasValue && this.executionEnvironment.HostId == downstreamCollectionId)
        return;
      bool flag = FeedSecurityHelper.HasModifyIndexPermissions(requestContext, feed);
      FeedVisibility? visibility3 = feed.View.Visibility;
      FeedVisibility feedVisibility3 = FeedVisibility.Collection;
      if (!(visibility3.GetValueOrDefault() == feedVisibility3 & visibility3.HasValue) || !this.executionEnvironment.IsOrganizationAadBacked() || !(this.executionEnvironment.GetOrganizationAadTenantId() == downstreamAadTenantId) || flag || !FeedSecurityHelper.HasReadFeedPermissions(requestContext, feed))
        throw new FeedNeedsPermissionsException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_ViewNotWidelyVisible((object) feed.FullyQualifiedId, (object) feed.View.Visibility.GetValueOrDefault()));
    }
  }
}
