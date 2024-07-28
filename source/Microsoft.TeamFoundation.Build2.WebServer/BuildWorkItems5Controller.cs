// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.BuildWorkItems5Controller
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "workitems", ResourceVersion = 2)]
  [ClientGroupByResource("builds")]
  public class BuildWorkItems5Controller : BuildWorkItems4Controller
  {
    [HttpGet]
    [ClientLocationId("5A21F5D2-5642-47E4-A0BD-1356E6731BEE")]
    [CustomerIntelligence("Build", "AssociatedWorkItems")]
    [PublicProjectRequestRestrictions]
    public override IList<ResourceRef> GetBuildWorkItemsRefs(int buildId, [FromUri(Name = "$top")] int top = 50) => this.GetBuildWorkItemsRefsInternal(buildId, top, new Guid?(this.ProjectId));

    [HttpGet]
    [CustomerIntelligence("Build", "AssociatedWorkItems")]
    [PublicProjectRequestRestrictions(true, true, null)]
    public override IList<ResourceRef> GetWorkItemsBetweenBuilds(
      int? fromBuildId,
      int? toBuildId,
      [FromUri(Name = "$top")] int top = 50)
    {
      return this.GetWorkItemsBetweenBuildsInternal(fromBuildId, toBuildId, top, new Guid?(this.ProjectId));
    }
  }
}
