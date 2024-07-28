// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SocialServer.Server.SocialEngProviderController
// Assembly: Microsoft.VisualStudio.Services.Social.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6878458A-724A-4C44-954E-B2170F10219E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Social.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Social.WebApi;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.SocialServer.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [VersionedApiControllerCustomName("Social", "SocialEngagementProviders", 1)]
  public class SocialEngProviderController : TfsApiController
  {
    public override string TraceArea => nameof (SocialEngProviderController);

    [HttpGet]
    public List<KeyValuePair<SocialEngagementType, string>> GetSocialEngagementProviders() => this.TfsRequestContext.GetService<ISocialEngService>().GetSocialEngagementProviders(this.TfsRequestContext);
  }
}
