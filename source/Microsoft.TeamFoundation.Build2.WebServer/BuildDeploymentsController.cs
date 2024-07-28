// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.BuildDeploymentsController
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Xaml;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ControllerApiVersion(2.0)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "deployments", ResourceVersion = 2)]
  public sealed class BuildDeploymentsController : BuildApiController
  {
    [HttpGet]
    public List<Deployment> GetBuildDeployments(int buildId)
    {
      try
      {
        return this.TfsRequestContext.GetService<IXamlBuildProvider>().GetBuildDeployments(this.TfsRequestContext, this.ProjectInfo, buildId).ToList<Deployment>();
      }
      catch (BuildNotFoundException ex)
      {
        if (this.InternalBuildService.GetBuildById(this.TfsRequestContext, buildId) == null)
          throw new BuildNotFoundException(Resources.BuildNotFound((object) buildId));
        return new List<Deployment>();
      }
    }
  }
}
