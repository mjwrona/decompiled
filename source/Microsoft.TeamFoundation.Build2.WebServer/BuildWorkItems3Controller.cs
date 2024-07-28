// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.BuildWorkItems3Controller
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ControllerApiVersion(3.0)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "workitems", ResourceVersion = 2)]
  [ClientGroupByResource("builds")]
  public class BuildWorkItems3Controller : BuildApiController
  {
    protected const int DefaultMaxItems = 50;

    [HttpGet]
    [ClientLocationId("5A21F5D2-5642-47E4-A0BD-1356E6731BEE")]
    [CustomerIntelligence("Build", "AssociatedWorkItems")]
    public virtual IList<ResourceRef> GetBuildWorkItemsRefs(int buildId, [FromUri(Name = "$top")] int top = 50)
    {
      this.TfsRequestContext.AddCIEntry("BuildId", (object) buildId);
      return this.InternalBuildService.GetBuildWorkItemRefs(this.TfsRequestContext, buildId, (IEnumerable<string>) null, top);
    }

    [HttpGet]
    [CustomerIntelligence("Build", "AssociatedWorkItems")]
    public virtual IList<ResourceRef> GetWorkItemsBetweenBuilds(
      int? fromBuildId,
      int? toBuildId,
      [FromUri(Name = "$top")] int top = 50)
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
          return this.InternalBuildService.GetWorkItemsBetweenBuilds(this.TfsRequestContext, fromBuildId.Value, toBuildId.Value, (IEnumerable<string>) null, top);
        }
      }
      throw new InvalidBuildQueryException(Resources.InvalidInputForGetChangesAndWorkItemsBetweenBuilds());
    }

    [HttpPost]
    [ClientLocationId("5A21F5D2-5642-47E4-A0BD-1356E6731BEE")]
    public virtual IList<ResourceRef> GetBuildWorkItemsRefsFromCommits(
      int buildId,
      IEnumerable<string> commitIds,
      [FromUri(Name = "$top")] int top = 50)
    {
      return (IList<ResourceRef>) this.InternalBuildService.GetBuildWorkItemRefs(this.TfsRequestContext, buildId, commitIds, top).ToList<ResourceRef>();
    }
  }
}
