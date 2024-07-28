// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Integration.Server.ProjectMaintenance
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using System;
using System.Web.Services;
using System.Web.Services.Protocols;

namespace Microsoft.TeamFoundation.Integration.Server
{
  [WebServiceBinding(Name = "DeleteProjectBinding", Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Admin/03")]
  [WebService(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Admin/03", Description = "DevOps Project Maintenance web service")]
  [ClientService(ServiceName = "IProjectMaintenance", CollectionServiceIdentifier = "855C71C3-4F2C-4FDE-A140-FB265F0FF0FA")]
  public class ProjectMaintenance : IntegrationWebService
  {
    [WebMethod]
    [SoapDocumentMethod(Binding = "DeleteProjectBinding", Action = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Admin/03/DeleteProject")]
    public bool DeleteProject(string projectUri)
    {
      if (this.RequestContext.IsFeatureEnabled("Project.Deletion.DisableLegacyDelete"))
        throw this.HandleException((Exception) new InvalidProjectDeleteException(WebApiResources.InvalidLegacyDeleteProjectError()));
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (DeleteProject), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (projectUri), (object) projectUri);
        this.EnterMethod(methodInformation);
        projectUri = ProjectInfo.NormalizeProjectUri(projectUri);
        ProjectInfo.GetProjectId(projectUri);
        this.RequestContext.GetService<IProjectService>().CheckProjectPermission(this.RequestContext, projectUri, TeamProjectPermissions.Delete);
        this.RequestContext.GetService<CommonStructureService>().HardDeleteProject(this.RequestContext.Elevate(), projectUri);
        return true;
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }
  }
}
