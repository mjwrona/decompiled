// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.WebServices.ProcessConfigurationService4
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Web.Services;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.WebServices
{
  [WebService(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/ProcessConfiguration/02", Description = "DevOps Process Configuration web service V4.0")]
  [ClientService(ServerConfiguration = ServerConfiguration.TfsTeamProjectCollection, ComponentName = "vstfs", ServiceName = "ProcessConfigurationService4", CollectionServiceIdentifier = "AF5CD66B-9F1D-4eb2-B3FD-CDEE9B082B7A")]
  [ProxyParentClass("ProcessConfigurationService", IgnoreInheritedMethods = true)]
  public class ProcessConfigurationService4 : ProcessConfigurationService3
  {
    [WebMethod]
    public override ProjectProcessConfiguration GetProjectProcessConfiguration(string projectUri)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (GetProjectProcessConfiguration), MethodType.Admin, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (projectUri), (object) projectUri);
        this.EnterMethod(methodInformation);
        this.RequestContext.TraceEnter(220461, "Agile", TfsTraceLayers.BusinessLogic, nameof (GetProjectProcessConfiguration));
        ArgumentUtility.CheckStringForNullOrEmpty(projectUri, nameof (projectUri));
        this.CheckProjectPermission(this.RequestContext, projectUri, "GENERIC_READ");
        return this.GetProjectProcessConfiguration2(projectUri);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.RequestContext.TraceLeave(220469, "Agile", TfsTraceLayers.BusinessLogic, nameof (GetProjectProcessConfiguration));
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public override void SetProjectProcessConfiguration(
      string projectUri,
      ProjectProcessConfiguration processConfiguration)
    {
      WorkItemTrackingFeatureFlags.CheckLegacyProcessUpdateInCustomizationModeEnabled(this.RequestContext);
      try
      {
        WebServiceHelpers.CheckWITProvisionPermission(this.RequestContext, projectUri);
        MethodInformation methodInformation = new MethodInformation(nameof (SetProjectProcessConfiguration), MethodType.Admin, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (projectUri), (object) projectUri);
        methodInformation.AddParameter(nameof (processConfiguration), (object) processConfiguration);
        this.EnterMethod(methodInformation);
        this.RequestContext.TraceEnter(220471, "Agile", TfsTraceLayers.BusinessLogic, nameof (SetProjectProcessConfiguration));
        ArgumentUtility.CheckStringForNullOrEmpty(projectUri, nameof (projectUri));
        ArgumentUtility.CheckForNull<ProjectProcessConfiguration>(processConfiguration, nameof (processConfiguration));
        IVssRequestContext requestContext = this.RequestContext;
        this.CheckProjectPermission(requestContext, projectUri, "GENERIC_WRITE");
        requestContext.GetService<IProjectConfigurationService>().SetProcessSettings(requestContext, projectUri, processConfiguration);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.RequestContext.TraceLeave(220479, "Agile", TfsTraceLayers.BusinessLogic, nameof (SetProjectProcessConfiguration));
        this.LeaveMethod();
      }
    }
  }
}
