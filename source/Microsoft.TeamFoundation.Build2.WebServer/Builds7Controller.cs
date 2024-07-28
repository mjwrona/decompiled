// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.Builds7Controller
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ControllerApiVersion(6.1)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "builds", ResourceVersion = 7)]
  [CheckWellFormedProject(Required = true)]
  [ClientIgnoreRouteScopes(ClientRouteScopes.Collection)]
  public class Builds7Controller : Builds6r6Controller
  {
    [HttpPatch]
    [ClientResponseType(typeof (Microsoft.TeamFoundation.Build.WebApi.Build), null, null)]
    [ClientMethodAccessModifier(ClientMethodAccessModifierAttribute.AccessModifier.PrivateProtected)]
    public override async Task<HttpResponseMessage> UpdateBuild(
      int buildId,
      Microsoft.TeamFoundation.Build.WebApi.Build build,
      [FromUri] bool retry = false)
    {
      Builds7Controller builds7Controller = this;
      // ISSUE: reference to a compiler-generated method
      return build != null && build.KeepForever.HasValue ? builds7Controller.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Resources.KeepForeverObsolete()) : await builds7Controller.\u003C\u003En__0(buildId, build, retry);
    }

    [HttpPatch]
    [ClientResponseType(typeof (List<Microsoft.TeamFoundation.Build.WebApi.Build>), null, null)]
    public override async Task<HttpResponseMessage> UpdateBuilds([FromBody] IEnumerable<Microsoft.TeamFoundation.Build.WebApi.Build> builds)
    {
      Builds7Controller builds7Controller = this;
      // ISSUE: reference to a compiler-generated method
      return builds.Any<Microsoft.TeamFoundation.Build.WebApi.Build>((Func<Microsoft.TeamFoundation.Build.WebApi.Build, bool>) (b => b.KeepForever.HasValue)) ? builds7Controller.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Resources.KeepForeverObsolete()) : await builds7Controller.\u003C\u003En__1(builds);
    }
  }
}
