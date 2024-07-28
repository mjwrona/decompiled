// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Xaml.XamlControllerProvider
// Assembly: Microsoft.TeamFoundation.Build2.Xaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 48A241AC-D20F-49E0-A581-C219E1ED7760
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Xaml.dll

using Microsoft.TeamFoundation.Build.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Xaml
{
  public class XamlControllerProvider : IXamlControllerProvider, IVssFrameworkService
  {
    public Microsoft.TeamFoundation.Build.WebApi.BuildController GetController(
      IVssRequestContext requestContext,
      int controllerId)
    {
      string str = Microsoft.TeamFoundation.Build2.Server.UriHelper.CreateArtifactUri("Controller", controllerId.ToString((IFormatProvider) CultureInfo.InvariantCulture)).ToString();
      Microsoft.TeamFoundation.Build.Server.BuildController controller = requestContext.GetService<TeamFoundationBuildResourceService>().QueryBuildControllersByUri(requestContext, (IList<string>) new string[1]
      {
        str
      }, (IList<string>) null, false).Controllers.FirstOrDefault<Microsoft.TeamFoundation.Build.Server.BuildController>();
      return controller != null ? controller.ToDataContract(requestContext) : (Microsoft.TeamFoundation.Build.WebApi.BuildController) null;
    }

    public IEnumerable<Microsoft.TeamFoundation.Build.WebApi.BuildController> GetControllers(
      IVssRequestContext requestContext,
      string name)
    {
      TeamFoundationBuildResourceService service = requestContext.GetService<TeamFoundationBuildResourceService>();
      BuildControllerSpec buildControllerSpec = new BuildControllerSpec();
      buildControllerSpec.IncludeAgents = false;
      buildControllerSpec.ServiceHostName = "*";
      buildControllerSpec.Name = name;
      IVssRequestContext requestContext1 = requestContext;
      BuildControllerSpec controllerSpec = buildControllerSpec;
      return service.QueryBuildControllers(requestContext1, controllerSpec).Controllers.Select<Microsoft.TeamFoundation.Build.Server.BuildController, Microsoft.TeamFoundation.Build.WebApi.BuildController>((Func<Microsoft.TeamFoundation.Build.Server.BuildController, Microsoft.TeamFoundation.Build.WebApi.BuildController>) (controller => controller.ToDataContract(requestContext)));
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }
  }
}
