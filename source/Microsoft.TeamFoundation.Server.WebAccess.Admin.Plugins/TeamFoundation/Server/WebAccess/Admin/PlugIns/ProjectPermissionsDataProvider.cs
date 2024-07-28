// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns.ProjectPermissionsDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WebPlatform.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns
{
  public class ProjectPermissionsDataProvider : IExtensionDataProvider
  {
    public string Name => "Admin.ProjectPermissions";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      requestContext.GetService<IClientLocationProviderService>().AddLocation(requestContext, providerContext.SharedData, ServiceInstanceTypes.SPS);
      ProjectPermissionsData data;
      try
      {
        ProjectInfo project = requestContext.GetService<IRequestProjectService>().GetProject(requestContext);
        project.PopulateProcessTemplateProperties(requestContext);
        TeamProjectModel teamProjectModel = new TeamProjectModel(requestContext, project, false, requestContext.ServiceHost.Name, false);
        data = new ProjectPermissionsData()
        {
          ProjectId = teamProjectModel.ProjectId
        };
      }
      catch (Exception ex)
      {
        data = (ProjectPermissionsData) null;
        requestContext.TraceException(10050072, "ProjectPermissions", "DataProvider", ex);
      }
      return (object) data;
    }
  }
}
