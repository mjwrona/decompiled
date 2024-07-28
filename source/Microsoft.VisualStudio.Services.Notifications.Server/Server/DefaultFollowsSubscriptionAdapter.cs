// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.DefaultFollowsSubscriptionAdapter
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class DefaultFollowsSubscriptionAdapter : FollowsSubscriptionAdapter
  {
    public override string SubscriptionTypeName => "*";

    protected override string GetEditArtifactUrl(
      IVssRequestContext requestContext,
      Subscription subscription)
    {
      return (string) null;
    }

    protected override string GetArtifactUrlFromInfo(
      IVssRequestContext requestContext,
      string artifactType,
      string artifactId)
    {
      return (string) null;
    }

    protected override void GetArtifactInfoFromUrl(
      string artifactUri,
      ref string artifactType,
      ref string artifactId)
    {
    }

    protected override bool CheckCanFollowArtifact(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      string artifact)
    {
      return false;
    }
  }
}
