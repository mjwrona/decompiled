// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.BuildWorkItems4Controller
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ControllerApiVersion(4.1)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "workitems", ResourceVersion = 2)]
  [ClientGroupByResource("builds")]
  public class BuildWorkItems4Controller : BuildWorkItems3Controller
  {
    [HttpGet]
    [ClientLocationId("5A21F5D2-5642-47E4-A0BD-1356E6731BEE")]
    [CustomerIntelligence("Build", "AssociatedWorkItems")]
    [PublicProjectRequestRestrictions]
    public override IList<ResourceRef> GetBuildWorkItemsRefs(int buildId, [FromUri(Name = "$top")] int top = 50) => this.GetBuildWorkItemsRefsInternal(buildId, top);

    internal IList<ResourceRef> GetBuildWorkItemsRefsInternal(
      int buildId,
      int top = 50,
      Guid? projectId = null)
    {
      this.TfsRequestContext.AddCIEntry("BuildId", (object) buildId);
      return !projectId.HasValue ? this.InternalBuildService.GetBuildWorkItemRefs(this.TfsRequestContext, buildId, (IEnumerable<string>) null, top) : this.BuildService.GetBuildWorkItemRefs(this.TfsRequestContext, projectId.Value, buildId, (IEnumerable<string>) null, top);
    }

    [HttpGet]
    [CustomerIntelligence("Build", "AssociatedWorkItems")]
    [PublicProjectRequestRestrictions(true, true, null)]
    public override IList<ResourceRef> GetWorkItemsBetweenBuilds(
      int? fromBuildId,
      int? toBuildId,
      [FromUri(Name = "$top")] int top = 50)
    {
      return this.GetWorkItemsBetweenBuildsInternal(fromBuildId, toBuildId, top);
    }

    internal virtual IList<ResourceRef> GetWorkItemsBetweenBuildsInternal(
      int? fromBuildId,
      int? toBuildId,
      int top = 50,
      Guid? projectId = null)
    {
      int? nullable1 = fromBuildId;
      int num1 = 0;
      if (nullable1.GetValueOrDefault() > num1 & nullable1.HasValue)
      {
        int? nullable2 = toBuildId;
        int num2 = 0;
        if (nullable2.GetValueOrDefault() > num2 & nullable2.HasValue)
        {
          this.TfsRequestContext.AddCIEntry("BuildId", (object) toBuildId.Value);
          return !projectId.HasValue ? this.InternalBuildService.GetWorkItemsBetweenBuilds(this.TfsRequestContext, fromBuildId.Value, toBuildId.Value, (IEnumerable<string>) null, top) : this.BuildService.GetWorkItemsBetweenBuilds(this.TfsRequestContext, projectId.Value, fromBuildId.Value, toBuildId.Value, (IEnumerable<string>) null, top);
        }
      }
      throw new InvalidBuildQueryException(Resources.InvalidInputForGetChangesAndWorkItemsBetweenBuilds());
    }

    [HttpPost]
    [ClientLocationId("5A21F5D2-5642-47E4-A0BD-1356E6731BEE")]
    [PublicProjectRequestRestrictions]
    public override IList<ResourceRef> GetBuildWorkItemsRefsFromCommits(
      int buildId,
      IEnumerable<string> commitIds,
      [FromUri(Name = "$top")] int top = 50)
    {
      return base.GetBuildWorkItemsRefsFromCommits(buildId, commitIds);
    }
  }
}
