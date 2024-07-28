// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SocialEngagement.Server.ISocialSdkSocialEngagementProvider
// Assembly: Microsoft.VisualStudio.Services.Social.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6878458A-724A-4C44-954E-B2170F10219E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Social.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Social.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.Services.SocialEngagement.Server
{
  [InheritedExport]
  public interface ISocialSdkSocialEngagementProvider
  {
    IDictionary<SocialEngagementType, string> GetSupportedSocialEngagement(
      IVssRequestContext requestContext);

    void ValidateArtifactId(
      IVssRequestContext requestContext,
      string artifactId,
      ArtifactScope artifactScope,
      out ISecuredObject securedObject);

    AggregationType EnableMetricsAggregation();

    int HoursToRetainAggregation();
  }
}
