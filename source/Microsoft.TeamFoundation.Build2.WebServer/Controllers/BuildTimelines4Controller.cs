// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.Controllers.BuildTimelines4Controller
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using System;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer.Controllers
{
  [ControllerApiVersion(4.1)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "Timeline", ResourceVersion = 2)]
  public class BuildTimelines4Controller : BuildTimelinesController
  {
    [HttpGet]
    [ClientResponseType(typeof (Timeline), null, null)]
    [PublicProjectRequestRestrictions]
    public override HttpResponseMessage GetBuildTimeline(
      int buildId,
      Guid? timelineId = null,
      int? changeId = null,
      Guid? planId = null)
    {
      return base.GetBuildTimeline(buildId, timelineId, changeId, planId);
    }
  }
}
