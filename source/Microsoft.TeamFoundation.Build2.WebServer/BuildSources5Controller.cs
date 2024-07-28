// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.BuildSources5Controller
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "sources", ResourceVersion = 2)]
  public class BuildSources5Controller : BuildSourcesController
  {
    [ClientIgnore]
    [HttpGet]
    [PublicProjectRequestRestrictions]
    public override HttpResponseMessage GetSources(int buildId, string sourceVersion = null) => this.GetSources(this.BuildService.GetBuildById(this.TfsRequestContext, this.ProjectId, buildId, includeDeleted: true) ?? throw new BuildNotFoundException(Resources.BuildNotFound((object) buildId)).Expected("Build2"), sourceVersion);
  }
}
