// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.WebServices.ProcessManagementIntegrationService
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core.WebServices;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Web.Services;
using System.Web.Services.Protocols;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.WebServices
{
  [WebService(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Admin/03", Description = "DevOps Process Management Integration web service")]
  [WebServiceBinding(Name = "IProjectMaintenanceBinding", Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Admin/03")]
  public class ProcessManagementIntegrationService : TeamFoundationWebService
  {
    [WebMethod]
    [SoapDocumentMethod(Binding = "IProjectMaintenanceBinding", Action = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Admin/03/DeleteProject")]
    public bool DeleteProject(string projectUri)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (DeleteProject), MethodType.Tool, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (projectUri), (object) projectUri);
        this.EnterMethod(methodInformation);
        this.RequestContext.TraceEnter(220601, "Agile", TfsTraceLayers.BusinessLogic, nameof (DeleteProject));
        ArgumentUtility.CheckStringForNullOrEmpty(projectUri, nameof (projectUri));
        this.EnsureDeletePermission(projectUri);
        return AgileSettingsUtils.DeleteSettingsForProject(this.RequestContext.Elevate(), projectUri);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.RequestContext.TraceLeave(220609, "Agile", TfsTraceLayers.BusinessLogic, nameof (DeleteProject));
        this.LeaveMethod();
      }
    }

    private void EnsureDeletePermission(string projectUri)
    {
      IVssSecurityNamespace securityNamespace = this.RequestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(this.RequestContext, FrameworkSecurity.TeamProjectNamespaceId);
      string token = securityNamespace.NamespaceExtension.HandleIncomingToken(this.RequestContext, securityNamespace, projectUri);
      securityNamespace.CheckPermission(this.RequestContext, token, TeamProjectPermissions.Delete);
    }
  }
}
