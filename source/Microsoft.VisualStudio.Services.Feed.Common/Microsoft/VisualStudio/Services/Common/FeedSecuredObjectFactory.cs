// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.FeedSecuredObjectFactory
// Assembly: Microsoft.VisualStudio.Services.Feed.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AAC6BCA4-7F6C-4DFE-8058-1CCDD886477F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Common.dll

using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Feed.WebApi.AzureArtifacts;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.VisualStudio.Services.Common
{
  public static class FeedSecuredObjectFactory
  {
    public static ISecuredObject CreateSecuredObjectReadOnly(FeedCore feed)
    {
      ArgumentUtility.CheckForNull<FeedCore>(feed, nameof (feed));
      string securityToken = FeedSecurityHelper.CalculateSecurityToken(feed);
      return (ISecuredObject) new ProjectScopedFeedSecuredObject(FeedConstants.FeedSecurityNamespaceId, 32, securityToken);
    }

    public static ISecuredObject CreateSecuredObjectReadOnly(ArtifactsFeed feed)
    {
      ArgumentUtility.CheckForNull<ArtifactsFeed>(feed, nameof (feed));
      string securityToken = FeedSecurityHelper.CalculateSecurityToken(feed);
      return (ISecuredObject) new ProjectScopedFeedSecuredObject(FeedConstants.FeedSecurityNamespaceId, 32, securityToken);
    }
  }
}
