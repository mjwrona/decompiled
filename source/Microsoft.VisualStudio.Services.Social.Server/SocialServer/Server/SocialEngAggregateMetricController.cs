// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SocialServer.Server.SocialEngAggregateMetricController
// Assembly: Microsoft.VisualStudio.Services.Social.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6878458A-724A-4C44-954E-B2170F10219E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Social.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Social.WebApi;
using System.ComponentModel;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.SocialServer.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [VersionedApiControllerCustomName("Social", "SocialEngagementAggregateMetric", 1)]
  public class SocialEngAggregateMetricController : TfsApiController
  {
    public override string TraceArea => nameof (SocialEngAggregateMetricController);

    [HttpGet]
    public SocialEngagementAggregateMetric GetSocialEngagementAggregateMetric(
      [ClientQueryParameter] string artifactType,
      [ClientQueryParameter] string artifactId,
      [ClientQueryParameter] SocialEngagementType engagementType,
      [ClientQueryParameter] string artifactScopeType,
      [ClientQueryParameter] string artifactScopeId = null)
    {
      SocialEngagementCreateParameter socialEngagementCreateParameter = new SocialEngagementCreateParameter()
      {
        ArtifactType = artifactType,
        ArtifactId = artifactId,
        EngagementType = engagementType,
        ArtifactScope = new ArtifactScope(artifactScopeType, artifactScopeId, string.Empty)
      };
      return this.TfsRequestContext.GetService<ISocialEngService>().GetSocialEngagementAggregateMetric(this.TfsRequestContext, socialEngagementCreateParameter);
    }
  }
}
