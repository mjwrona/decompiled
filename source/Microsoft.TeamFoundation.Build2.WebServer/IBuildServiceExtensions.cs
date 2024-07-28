// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.IBuildServiceExtensions
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  internal static class IBuildServiceExtensions
  {
    internal static IList<ResourceRef> GetBuildWorkItemRefs(
      this IBuildServiceInternal buildService,
      IVssRequestContext requestContext,
      int buildId,
      IEnumerable<string> commitIds,
      int maxItems)
    {
      using (requestContext.TraceScope(nameof (IBuildServiceExtensions), nameof (GetBuildWorkItemRefs)))
      {
        BuildData build = buildService.GetBuildById(requestContext, buildId);
        if (build == null)
          throw new BuildNotFoundException(Resources.BuildNotFound((object) buildId));
        return (IList<ResourceRef>) buildService.GetBuildWorkItemRefs(requestContext, (IReadOnlyBuildData) build, commitIds, maxItems).Select<ResourceRef, ResourceRef>((Func<ResourceRef, ResourceRef>) (r => new ResourceRef(build.ToSecuredObject())
        {
          Id = r.Id,
          Url = r.Url
        })).ToList<ResourceRef>();
      }
    }

    internal static IList<ResourceRef> GetWorkItemsBetweenBuilds(
      this IBuildServiceInternal buildService,
      IVssRequestContext requestContext,
      int fromBuildId,
      int toBuildId,
      int maxItems)
    {
      using (requestContext.TraceScope(nameof (IBuildServiceExtensions), nameof (GetWorkItemsBetweenBuilds)))
      {
        IList<ResourceRef> itemsBetweenBuilds = buildService.GetWorkItemsBetweenBuilds(requestContext, fromBuildId, toBuildId, maxItems);
        Func<ResourceRef, ResourceRef> selector = (Func<ResourceRef, ResourceRef>) (r => new ResourceRef((buildService.GetBuildById(requestContext, fromBuildId) ?? throw new BuildNotFoundException(Resources.BuildNotFound((object) fromBuildId))).ToSecuredObject())
        {
          Id = r.Id,
          Url = r.Url
        });
        return (IList<ResourceRef>) itemsBetweenBuilds.Select<ResourceRef, ResourceRef>(selector).ToList<ResourceRef>();
      }
    }
  }
}
