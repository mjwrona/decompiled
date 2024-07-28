// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.WebServices.ProcessConfigurationService3
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Settings.Legacy;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Web.Services;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.WebServices
{
  [WebService(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/ProcessConfiguration/02", Description = "DevOps Process Configuration web service V3.0")]
  [ClientService(ServerConfiguration = ServerConfiguration.TfsTeamProjectCollection, ComponentName = "vstfs", ServiceName = "ProcessConfigurationService3", CollectionServiceIdentifier = "BE3EB50B-7FDD-40FF-979F-B5B77F43F662")]
  [ProxyParentClass("ProcessConfigurationService", IgnoreInheritedMethods = true)]
  public class ProcessConfigurationService3 : ProcessConfigurationService2
  {
    [WebMethod]
    public override void SetAgileConfiguration(
      string projectUri,
      AgileProjectConfiguration agileConfiguration)
    {
      WorkItemTrackingFeatureFlags.CheckLegacyProcessUpdateInCustomizationModeEnabled(this.RequestContext);
      try
      {
        WebServiceHelpers.CheckWITProvisionPermission(this.RequestContext, projectUri);
        MethodInformation methodInformation = new MethodInformation(nameof (SetAgileConfiguration), MethodType.Admin, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (projectUri), (object) projectUri);
        methodInformation.AddParameter(nameof (agileConfiguration), (object) agileConfiguration);
        this.EnterMethod(methodInformation);
        this.RequestContext.TraceEnter(220441, "Agile", TfsTraceLayers.BusinessLogic, "SetAgileConfiguration3");
        ArgumentUtility.CheckForNull<string>(projectUri, nameof (projectUri));
        IVssRequestContext requestContext = this.RequestContext;
        this.CheckProjectPermission(requestContext, projectUri, "GENERIC_WRITE");
        LegacySettingsConverter.SetAgileSettings(requestContext, projectUri, agileConfiguration);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.RequestContext.TraceLeave(220449, "Agile", TfsTraceLayers.BusinessLogic, "SetAgileConfiguration3");
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public override void SetAgileConfiguration2(
      string projectUri,
      AgileProjectConfiguration agileConfiguration)
    {
      WorkItemTrackingFeatureFlags.CheckLegacyProcessUpdateInCustomizationModeEnabled(this.RequestContext);
      this.SetAgileConfiguration(projectUri, agileConfiguration);
    }

    [WebMethod]
    public override void SetCommonConfiguration(
      string projectUri,
      CommonProjectConfiguration commonConfiguration)
    {
      WorkItemTrackingFeatureFlags.CheckLegacyProcessUpdateInCustomizationModeEnabled(this.RequestContext);
      try
      {
        WebServiceHelpers.CheckWITProvisionPermission(this.RequestContext, projectUri);
        MethodInformation methodInformation = new MethodInformation(nameof (SetCommonConfiguration), MethodType.Admin, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (projectUri), (object) projectUri);
        methodInformation.AddParameter(nameof (commonConfiguration), (object) commonConfiguration);
        this.EnterMethod(methodInformation);
        this.RequestContext.TraceEnter(220411, "Agile", TfsTraceLayers.BusinessLogic, "SetCommonConfiguration3");
        ArgumentUtility.CheckForNull<string>(projectUri, nameof (projectUri));
        IVssRequestContext requestContext = this.RequestContext;
        this.CheckProjectPermission(requestContext, projectUri, "GENERIC_WRITE");
        LegacySettingsConverter.SetCommonSettings(requestContext, projectUri, commonConfiguration);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.RequestContext.TraceLeave(220419, "Agile", TfsTraceLayers.BusinessLogic, "SetCommonConfiguration3");
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public override void SetCommonConfiguration2(
      string projectUri,
      CommonProjectConfiguration commonConfiguration)
    {
      WorkItemTrackingFeatureFlags.CheckLegacyProcessUpdateInCustomizationModeEnabled(this.RequestContext);
      this.SetCommonConfiguration(projectUri, commonConfiguration);
    }

    [WebMethod]
    public virtual ProjectProcessConfiguration GetProjectProcessConfiguration(string projectUri)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (GetProjectProcessConfiguration), MethodType.Admin, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (projectUri), (object) projectUri);
        this.EnterMethod(methodInformation);
        this.RequestContext.TraceEnter(220461, "Agile", TfsTraceLayers.BusinessLogic, nameof (GetProjectProcessConfiguration));
        ArgumentUtility.CheckStringForNullOrEmpty(projectUri, nameof (projectUri));
        this.CheckProjectPermission(this.RequestContext, projectUri, "GENERIC_READ");
        return ProjectConfigurationCompatibilityConverter.ConvertToV3(this.GetProjectProcessConfiguration2(projectUri));
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
    protected ProjectProcessConfiguration GetProjectProcessConfiguration2(string projectUri)
    {
      try
      {
        IVssRequestContext requestContext = this.RequestContext;
        IProjectConfigurationService service = requestContext.GetService<IProjectConfigurationService>();
        try
        {
          return service.GetProcessSettings(requestContext, projectUri, true);
        }
        catch (ProjectSettingsException ex)
        {
          return service.GetProcessSettings(requestContext, projectUri, false);
        }
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
    }

    [WebMethod]
    public virtual void SetProjectProcessConfiguration(
      string projectUri,
      ProjectProcessConfiguration processConfiguration)
    {
      WorkItemTrackingFeatureFlags.CheckLegacyProcessUpdateInCustomizationModeEnabled(this.RequestContext);
      try
      {
        if (this.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          throw new NotSupportedException(Resources.CannotSetProjectProcessConfigurationFromOlderClient);
        ProjectProcessConfiguration processConfiguration2 = this.GetProjectProcessConfiguration2(projectUri);
        if (processConfiguration2 != null && processConfiguration2.BugsBehavior != BugsBehavior.Off)
          throw new NotSupportedException(Resources.CannotSetProjectProcessConfigurationFromOlderClient);
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

    [WebMethod]
    public void ValidateProjectProcessConfiguration(
      string projectUri,
      ProjectProcessConfiguration processConfiguration)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (ValidateProjectProcessConfiguration), MethodType.Admin, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (projectUri), (object) projectUri);
        methodInformation.AddParameter(nameof (processConfiguration), (object) processConfiguration);
        this.EnterMethod(methodInformation);
        this.RequestContext.TraceEnter(220481, "Agile", TfsTraceLayers.BusinessLogic, nameof (ValidateProjectProcessConfiguration));
        if (processConfiguration == null)
          return;
        IVssRequestContext requestContext = this.RequestContext;
        this.CheckProjectPermission(requestContext, projectUri, "GENERIC_READ");
        if (!string.IsNullOrEmpty(projectUri))
          ProcessSettingsValidator.Validate(requestContext, processConfiguration, projectUri, false);
        else
          ProcessSettingsValidator.ValidateStructure(requestContext, processConfiguration);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.RequestContext.TraceLeave(220489, "Agile", TfsTraceLayers.BusinessLogic, nameof (ValidateProjectProcessConfiguration));
        this.LeaveMethod();
      }
    }
  }
}
