// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SocialServer.Server.SocialEngController
// Assembly: Microsoft.VisualStudio.Services.Social.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6878458A-724A-4C44-954E-B2170F10219E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Social.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Social.WebApi;
using System.ComponentModel;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.SocialServer.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [VersionedApiControllerCustomName("Social", "SocialEngagement", 1)]
  public class SocialEngController : TfsApiController
  {
    public override string TraceArea => nameof (SocialEngController);

    [HttpGet]
    public SocialEngagementRecord GetSocialEngagement(
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
      return this.TfsRequestContext.GetService<ISocialEngService>().GetSocialEngagementRecord(this.TfsRequestContext, socialEngagementCreateParameter);
    }

    [HttpPost]
    public SocialEngagementRecord CreateSocialEngagement(
      SocialEngagementCreateParameter socialEngagementCreateParameter)
    {
      ArgumentUtility.CheckForNull<SocialEngagementCreateParameter>(socialEngagementCreateParameter, nameof (socialEngagementCreateParameter));
      ArgumentUtility.CheckStringLength(socialEngagementCreateParameter.ArtifactId, "ArtifactId", 256);
      return this.TfsRequestContext.GetService<ISocialEngService>().CreateSocialEngagementRecord(this.TfsRequestContext, socialEngagementCreateParameter);
    }

    [HttpDelete]
    public SocialEngagementRecord DeleteSocialEngagement(
      [ClientQueryParameter] string artifactType,
      [ClientQueryParameter] string artifactId,
      [ClientQueryParameter] SocialEngagementType engagementType,
      [ClientQueryParameter] string artifactScopeType,
      [ClientQueryParameter] string artifactScopeId = null)
    {
      ISocialEngService service = this.TfsRequestContext.GetService<ISocialEngService>();
      SocialEngagementCreateParameter engagementCreateParameter = new SocialEngagementCreateParameter()
      {
        ArtifactType = artifactType,
        ArtifactId = artifactId,
        EngagementType = engagementType,
        ArtifactScope = new ArtifactScope(artifactScopeType, artifactScopeId, string.Empty)
      };
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      SocialEngagementCreateParameter socialEngagementCreateParameter = engagementCreateParameter;
      return service.DeleteSocialEngagementRecord(tfsRequestContext, socialEngagementCreateParameter);
    }
  }
}
