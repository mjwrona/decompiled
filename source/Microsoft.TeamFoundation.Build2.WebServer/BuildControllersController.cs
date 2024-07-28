// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.BuildControllersController
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
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "Controllers", ResourceVersion = 2)]
  public sealed class BuildControllersController : BuildApiController
  {
    [HttpGet]
    public BuildController GetBuildController(int controllerId)
    {
      BuildController controller = this.TfsRequestContext.GetService<IXamlControllerProvider>().GetController(this.TfsRequestContext, controllerId);
      if (controller == null)
        throw new BuildControllerNotFoundException(Resources.BuildControllerNotFound((object) controllerId));
      controller.AddLinks(this.TfsRequestContext);
      return controller;
    }

    [HttpGet]
    [ClientResponseType(typeof (List<BuildController>), null, null)]
    public List<BuildController> GetBuildControllers(string name = "*") => this.TfsRequestContext.GetService<IXamlControllerProvider>().GetControllers(this.TfsRequestContext, name).ToList<BuildController>();
  }
}
