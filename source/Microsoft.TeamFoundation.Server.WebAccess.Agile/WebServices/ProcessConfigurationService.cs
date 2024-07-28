// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.WebServices.ProcessConfigurationService
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Integration.Server;
using Microsoft.TeamFoundation.Server.Core.WebServices;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Settings.Legacy;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Web.Services;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.WebServices
{
  [WebService(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/ProcessConfiguration/01", Description = "DevOps Process Configuration web service")]
  [ClientService(ServerConfiguration = ServerConfiguration.TfsTeamProjectCollection, ComponentName = "vstfs", ServiceName = "ProcessConfigurationService", CollectionServiceIdentifier = "856874df-9942-4f5f-9c42-797a47e7db4f")]
  public class ProcessConfigurationService : TeamFoundationWebService
  {
    [WebMethod]
    public CommonProjectConfiguration GetCommonConfiguration(string projectUri)
    {
      try
      {
        return ProjectConfigurationCompatibilityConverter.ConvertToV1(this.GetConfiguration(projectUri));
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.RequestContext.TraceLeave(220409, "Agile", TfsTraceLayers.BusinessLogic, nameof (GetCommonConfiguration));
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public virtual void SetCommonConfiguration(
      string projectUri,
      CommonProjectConfiguration commonConfiguration)
    {
      WorkItemTrackingFeatureFlags.CheckLegacyProcessUpdateInCustomizationModeEnabled(this.RequestContext);
      try
      {
        throw new NotSupportedException(Resources.CannotSetProcessConfigurationFromOlderClient);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.RequestContext.TraceLeave(220419, "Agile", TfsTraceLayers.BusinessLogic, nameof (SetCommonConfiguration));
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void ValidateCommonConfiguration(
      string projectUri,
      CommonProjectConfiguration commonConfiguration)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (ValidateCommonConfiguration), MethodType.Admin, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (projectUri), (object) projectUri);
        methodInformation.AddParameter(nameof (commonConfiguration), (object) commonConfiguration);
        this.EnterMethod(methodInformation);
        this.RequestContext.TraceEnter(220421, "Agile", TfsTraceLayers.BusinessLogic, nameof (ValidateCommonConfiguration));
        if (commonConfiguration == null)
          return;
        IVssRequestContext requestContext = this.RequestContext;
        this.CheckProjectPermission(requestContext, projectUri, "GENERIC_READ");
        if (!string.IsNullOrEmpty(projectUri))
          commonConfiguration.Validate(requestContext, projectUri, false);
        else
          commonConfiguration.ValidateStructure();
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.RequestContext.TraceLeave(220429, "Agile", TfsTraceLayers.BusinessLogic, nameof (ValidateCommonConfiguration));
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public AgileProjectConfiguration GetAgileConfiguration(string projectUri)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (GetAgileConfiguration), MethodType.Admin, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (projectUri), (object) projectUri);
        this.EnterMethod(methodInformation);
        this.RequestContext.TraceEnter(220431, "Agile", TfsTraceLayers.BusinessLogic, nameof (GetAgileConfiguration));
        ArgumentUtility.CheckForNull<string>(projectUri, nameof (projectUri));
        IVssRequestContext requestContext = this.RequestContext;
        this.CheckProjectPermission(requestContext, projectUri, "GENERIC_READ");
        return LegacySettingsConverter.GetAgileSettings(requestContext, projectUri, false);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.RequestContext.TraceLeave(220439, "Agile", TfsTraceLayers.BusinessLogic, nameof (GetAgileConfiguration));
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public virtual void SetAgileConfiguration(
      string projectUri,
      AgileProjectConfiguration agileConfiguration)
    {
      WorkItemTrackingFeatureFlags.CheckLegacyProcessUpdateInCustomizationModeEnabled(this.RequestContext);
      try
      {
        throw new NotSupportedException(Resources.CannotSetProcessConfigurationFromOlderClient);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.RequestContext.TraceLeave(220449, "Agile", TfsTraceLayers.BusinessLogic, nameof (SetAgileConfiguration));
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void ValidateAgileConfiguration(
      string projectUri,
      AgileProjectConfiguration agileConfiguration)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (ValidateAgileConfiguration), MethodType.Admin, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (projectUri), (object) projectUri);
        methodInformation.AddParameter(nameof (agileConfiguration), (object) agileConfiguration);
        this.EnterMethod(methodInformation);
        this.RequestContext.TraceEnter(220451, "Agile", TfsTraceLayers.BusinessLogic, nameof (ValidateAgileConfiguration));
        if (agileConfiguration == null)
          return;
        IVssRequestContext requestContext = this.RequestContext;
        this.CheckProjectPermission(requestContext, projectUri, "GENERIC_READ");
        if (!string.IsNullOrEmpty(projectUri))
          agileConfiguration.Validate(requestContext, projectUri, false);
        else
          agileConfiguration.ValidateStructure();
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.RequestContext.TraceLeave(220459, "Agile", TfsTraceLayers.BusinessLogic, nameof (ValidateAgileConfiguration));
        this.LeaveMethod();
      }
    }

    protected CommonProjectConfiguration GetConfiguration(string projectUri)
    {
      MethodInformation methodInformation = new MethodInformation("GetCommonConfiguration", MethodType.Admin, EstimatedMethodCost.Low);
      methodInformation.AddParameter(nameof (projectUri), (object) projectUri);
      this.EnterMethod(methodInformation);
      this.RequestContext.TraceEnter(220401, "Agile", TfsTraceLayers.BusinessLogic, "GetCommonConfiguration");
      ArgumentUtility.CheckForNull<string>(projectUri, nameof (projectUri));
      IVssRequestContext requestContext = this.RequestContext;
      this.CheckProjectPermission(requestContext, projectUri, "GENERIC_READ");
      return LegacySettingsConverter.GetCommonSettings(requestContext, projectUri, false);
    }

    protected void CheckProjectPermission(
      IVssRequestContext requestContext,
      string projectUri,
      string actionId)
    {
      IntegrationSecurityManager service = requestContext.GetService<IntegrationSecurityManager>();
      if (!string.IsNullOrEmpty(projectUri))
        service.CheckProjectPermission(requestContext, projectUri, actionId);
      else
        service.CheckGlobalPermission(requestContext, actionId);
    }

    protected CommonStructureProjectInfo ProjectFromProjectUri(
      IVssRequestContext requestContext,
      string projectUri)
    {
      return requestContext.GetService<CommonStructureService>().GetProject(requestContext, projectUri);
    }
  }
}
