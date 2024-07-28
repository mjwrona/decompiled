// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.Controllers.BuildChanges5Controller
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer.Controllers
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "changes", ResourceVersion = 2)]
  [ClientGroupByResource("builds")]
  public class BuildChanges5Controller : BuildChanges4Controller
  {
    [HttpGet]
    [ClientLocationId("54572C7B-BBD3-45D4-80DC-28BE08941620")]
    [ClientResponseType(typeof (IPagedList<Change>), null, null)]
    [PublicProjectRequestRestrictions]
    public override HttpResponseMessage GetBuildChanges(
      int buildId,
      string continuationToken = null,
      [FromUri(Name = "$top")] int top = 50,
      bool includeSourceChange = false)
    {
      return this.GetBuildChangesInternal(buildId, continuationToken, top, includeSourceChange, new Guid?(this.ProjectId));
    }

    [HttpGet]
    [PublicProjectRequestRestrictions(true, true, null)]
    public override List<Change> GetChangesBetweenBuilds(int? fromBuildId = null, int? toBuildId = null, [FromUri(Name = "$top")] int top = 50) => this.GetChangesBetweenBuildsInternal(fromBuildId, toBuildId, top, new Guid?(this.ProjectId));
  }
}
