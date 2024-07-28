// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.Follows.IVssFollowsService
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Notifications.Server.Follows
{
  [DefaultServiceImplementation(typeof (VssFollowsService))]
  public interface IVssFollowsService : IVssFrameworkService
  {
    ArtifactSubscription GetArtifactSubscription(
      IVssRequestContext requestContext,
      string artifactType,
      string artifactId,
      Guid identityId);

    ArtifactSubscription GetArtifactSubscription(
      IVssRequestContext requestContext,
      int subscriptionId);

    ArtifactSubscription Follow(
      IVssRequestContext requestContext,
      ArtifactSubscription artifact,
      Guid identityId);

    ArtifactSubscription Unfollow(
      IVssRequestContext requestContext,
      int subscriptionId,
      Guid identityId);

    void Unfollow(IVssRequestContext requestContext, IEnumerable<int> subscriptionIds);

    IEnumerable<ArtifactSubscription> GetArtifactSubscriptions(
      IVssRequestContext requestContext,
      string artifactType,
      string[] artifactIds);
  }
}
