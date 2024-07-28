// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Xaml.XamlControllerExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Xaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 48A241AC-D20F-49E0-A581-C219E1ED7760
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Xaml.dll

using Microsoft.TeamFoundation.Build2.Routes;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.TeamFoundation.Build2.Xaml
{
  public static class XamlControllerExtensions
  {
    public static Microsoft.TeamFoundation.Build.WebApi.BuildController ToDataContract(
      this Microsoft.TeamFoundation.Build.Server.BuildController controller,
      IVssRequestContext requestContext)
    {
      IXamlBuildRouteService service = requestContext.GetService<IXamlBuildRouteService>();
      int controllerId = int.Parse(LinkingUtilities.DecodeUri(controller.Uri).ToolSpecificId);
      Microsoft.TeamFoundation.Build.WebApi.BuildController dataContract = new Microsoft.TeamFoundation.Build.WebApi.BuildController();
      dataContract.Id = controllerId;
      dataContract.Name = controller.Name;
      dataContract.Description = controller.Description;
      dataContract.CreatedDate = controller.DateCreated;
      dataContract.UpdatedDate = controller.DateUpdated;
      dataContract.Enabled = controller.Enabled;
      dataContract.Status = controller.Status.ToControllerStatus();
      dataContract.Uri = new Uri(controller.Uri);
      dataContract.Url = service.GetControllerRestUrl(requestContext, controllerId);
      return dataContract;
    }

    public static Microsoft.TeamFoundation.Build.WebApi.ControllerStatus ToControllerStatus(
      this Microsoft.TeamFoundation.Build.Server.ControllerStatus controllerStatus)
    {
      switch (controllerStatus)
      {
        case Microsoft.TeamFoundation.Build.Server.ControllerStatus.Available:
          return Microsoft.TeamFoundation.Build.WebApi.ControllerStatus.Available;
        case Microsoft.TeamFoundation.Build.Server.ControllerStatus.Offline:
          return Microsoft.TeamFoundation.Build.WebApi.ControllerStatus.Offline;
        default:
          return Microsoft.TeamFoundation.Build.WebApi.ControllerStatus.Unavailable;
      }
    }

    public static void AddLinks(
      this Microsoft.TeamFoundation.Build.WebApi.BuildController buildController,
      IVssRequestContext requestContext)
    {
      if (buildController == null || string.IsNullOrEmpty(buildController.Url))
        return;
      buildController.Links.TryAddLink("self", (ISecuredObject) buildController, buildController.Url);
    }
  }
}
