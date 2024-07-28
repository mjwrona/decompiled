// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ConfigurationSettingsService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 929F0284-16B2-4277-9F4A-B615689A77D1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Web.Services;
using System.Web.Services.Protocols;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [WebService(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/configurationSettingsService/03", Description = "Team Foundation WorkItemTracking ConfigurationSettings web service")]
  [ClientService(ServiceName = "ConfigurationSettingsUrl", CollectionServiceIdentifier = "1e9d1b48-775a-49c8-af8f-d41a06e0cdb0")]
  public class ConfigurationSettingsService : WorkItemTrackingWebService
  {
    [WebMethod]
    [SoapHeader("requestHeader", Direction = SoapHeaderDirection.In)]
    public string GetWorkitemTrackingVersion()
    {
      this.RequestContext.TraceEnter(900382, "WebServices", nameof (ConfigurationSettingsService), nameof (GetWorkitemTrackingVersion));
      string empty = string.Empty;
      try
      {
        this.EnterMethod(new MethodInformation(nameof (GetWorkitemTrackingVersion), MethodType.LightWeight, EstimatedMethodCost.Low));
        if (!this.User.Identity.IsAuthenticated)
          throw new SoapException(ResourceStrings.Get("BadServerConfig"), Soap12FaultCodes.ReceiverFaultCode, WorkItemTrackingFaultCodes.BadServerConfig);
        empty = Assembly.GetCallingAssembly().GetName().Version.ToString();
        this.RequestContext.Trace(900385, TraceLevel.Info, "WebServices", nameof (ConfigurationSettingsService), "Workitem Tracking DataServices Assembly Version: {0}", (object) empty);
      }
      catch (Exception ex)
      {
        this.HandleException(ex);
        throw;
      }
      finally
      {
        this.LeaveMethod();
        this.RequestContext.TraceLeave(900598, "WebServices", nameof (ConfigurationSettingsService), nameof (GetWorkitemTrackingVersion));
      }
      return empty;
    }

    [WebMethod]
    [SoapHeader("requestHeader", Direction = SoapHeaderDirection.In)]
    public long GetMaxAttachmentSize()
    {
      this.RequestContext.TraceEnter(900383, "WebServices", nameof (ConfigurationSettingsService), nameof (GetMaxAttachmentSize));
      try
      {
        this.EnterMethod(new MethodInformation(nameof (GetMaxAttachmentSize), MethodType.Normal, EstimatedMethodCost.Low));
        if (!this.User.Identity.IsAuthenticated)
          throw new SoapException(ResourceStrings.Get("BadServerConfig"), Soap12FaultCodes.ReceiverFaultCode, WorkItemTrackingFaultCodes.BadServerConfig);
        return this.RequestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(this.RequestContext).MaxAttachmentSize;
      }
      catch (Exception ex)
      {
        this.HandleException(ex);
        throw;
      }
      finally
      {
        this.LeaveMethod();
        this.RequestContext.TraceLeave(900599, "WebServices", nameof (ConfigurationSettingsService), nameof (GetMaxAttachmentSize));
      }
    }

    [WebMethod]
    [SoapHeader("requestHeader", Direction = SoapHeaderDirection.In)]
    public void SetMaxAttachmentSize(long maxSize)
    {
      this.RequestContext.TraceEnter(900384, "WebServices", nameof (ConfigurationSettingsService), nameof (SetMaxAttachmentSize));
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (SetMaxAttachmentSize), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (maxSize), (object) maxSize);
        this.EnterMethod(methodInformation);
        if (this.RequestContext.ExecutionEnvironment.IsHostedDeployment)
        {
          this.Context.Response.StatusCode = 404;
          this.Context.Response.End();
        }
        else
        {
          if (!this.User.Identity.IsAuthenticated)
            throw new SoapException(ResourceStrings.Get("BadServerConfig"), Soap12FaultCodes.ReceiverFaultCode, WorkItemTrackingFaultCodes.BadServerConfig);
          IdentityService service = this.RequestContext.GetService<IdentityService>();
          if (!service.IsMember(this.RequestContext, GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup, this.RequestContext.UserContext) && !service.IsMember(this.RequestContext, GroupWellKnownIdentityDescriptors.ServiceUsersGroup, this.RequestContext.UserContext))
            throw new UnauthorizedAccessException();
          if (maxSize > 2000000000L)
            throw new SoapException(ResourceStrings.Format("MaxAttachmentSizeExceedsMaximum", (object) 2000000000), Soap12FaultCodes.ReceiverFaultCode, WorkItemTrackingFaultCodes.MaxAttachmentsSizeExceeded);
          this.RequestContext.GetService<WorkItemTrackingConfigurationSettingService>().SetMaxAttachmentSize(this.RequestContext, maxSize);
        }
      }
      catch (Exception ex)
      {
        this.HandleException(ex);
        throw;
      }
      finally
      {
        this.LeaveMethod();
        this.RequestContext.TraceLeave(900656, "WebServices", nameof (ConfigurationSettingsService), nameof (SetMaxAttachmentSize));
      }
    }

    [WebMethod]
    [SoapHeader("requestHeader", Direction = SoapHeaderDirection.In)]
    public bool GetInProcBuildCompletionNotificationAvailability()
    {
      this.RequestContext.TraceEnter(900451, "WebServices", nameof (ConfigurationSettingsService), nameof (GetInProcBuildCompletionNotificationAvailability));
      try
      {
        this.EnterMethod(new MethodInformation(nameof (GetInProcBuildCompletionNotificationAvailability), MethodType.Normal, EstimatedMethodCost.Low));
        if (!this.User.Identity.IsAuthenticated)
          throw new SoapException(ResourceStrings.Get("BadServerConfig"), Soap12FaultCodes.ReceiverFaultCode, WorkItemTrackingFaultCodes.BadServerConfig);
        return this.RequestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(this.RequestContext).IsInProcBuildCompletionNotificationEnabled;
      }
      catch (Exception ex)
      {
        this.HandleException(ex);
        throw;
      }
      finally
      {
        this.LeaveMethod();
        this.RequestContext.TraceLeave(900600, "WebServices", nameof (ConfigurationSettingsService), nameof (GetInProcBuildCompletionNotificationAvailability));
      }
    }

    [WebMethod]
    [SoapHeader("requestHeader", Direction = SoapHeaderDirection.In)]
    public void SetInProcBuildCompletionNotificationAvailability(bool isEnabled)
    {
      this.RequestContext.TraceEnter(900411, "WebServices", nameof (ConfigurationSettingsService), nameof (SetInProcBuildCompletionNotificationAvailability));
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (SetInProcBuildCompletionNotificationAvailability), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (SetInProcBuildCompletionNotificationAvailability), (object) isEnabled);
        this.EnterMethod(methodInformation);
        if (this.RequestContext.ExecutionEnvironment.IsHostedDeployment)
        {
          this.Context.Response.StatusCode = 404;
          this.Context.Response.End();
        }
        else
        {
          if (!this.User.Identity.IsAuthenticated)
            throw new SoapException(ResourceStrings.Get("BadServerConfig"), Soap12FaultCodes.ReceiverFaultCode, WorkItemTrackingFaultCodes.BadServerConfig);
          IdentityService service = this.RequestContext.GetService<IdentityService>();
          if (!service.IsMember(this.RequestContext, GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup, this.RequestContext.UserContext) && !service.IsMember(this.RequestContext, GroupWellKnownIdentityDescriptors.ServiceUsersGroup, this.RequestContext.UserContext))
            throw new UnauthorizedAccessException();
          this.RequestContext.GetService<WorkItemTrackingConfigurationSettingService>().SetInProcBuildCompletionNotificationAvailability(this.RequestContext, isEnabled);
        }
      }
      catch (Exception ex)
      {
        this.HandleException(ex);
        throw;
      }
      finally
      {
        this.LeaveMethod();
        this.RequestContext.TraceLeave(900657, "WebServices", nameof (ConfigurationSettingsService), nameof (SetInProcBuildCompletionNotificationAvailability));
      }
    }

    [WebMethod]
    [SoapHeader("requestHeader", Direction = SoapHeaderDirection.In)]
    public int GetMaxBuildListSize()
    {
      this.RequestContext.TraceEnter(900452, "WebServices", nameof (ConfigurationSettingsService), nameof (GetMaxBuildListSize));
      try
      {
        this.EnterMethod(new MethodInformation(nameof (GetMaxBuildListSize), MethodType.Normal, EstimatedMethodCost.Low));
        if (!this.User.Identity.IsAuthenticated)
          throw new SoapException(ResourceStrings.Get("BadServerConfig"), Soap12FaultCodes.ReceiverFaultCode, WorkItemTrackingFaultCodes.BadServerConfig);
        return this.RequestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(this.RequestContext).MaxBuildListSize;
      }
      catch (Exception ex)
      {
        this.HandleException(ex);
        throw;
      }
      finally
      {
        this.LeaveMethod();
        this.RequestContext.TraceLeave(900602, "WebServices", nameof (ConfigurationSettingsService), nameof (GetMaxBuildListSize));
      }
    }

    [WebMethod]
    [SoapHeader("requestHeader", Direction = SoapHeaderDirection.In)]
    public void SetMaxBuildListSize(int maxBuildListSize)
    {
      this.RequestContext.TraceEnter(900411, "WebServices", nameof (ConfigurationSettingsService), nameof (SetMaxBuildListSize));
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (SetMaxBuildListSize), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (maxBuildListSize), (object) maxBuildListSize);
        this.EnterMethod(methodInformation);
        if (this.RequestContext.ExecutionEnvironment.IsHostedDeployment)
        {
          this.Context.Response.StatusCode = 404;
          this.Context.Response.End();
        }
        else
        {
          if (!this.User.Identity.IsAuthenticated)
            throw new SoapException(ResourceStrings.Get("BadServerConfig"), Soap12FaultCodes.ReceiverFaultCode, WorkItemTrackingFaultCodes.BadServerConfig);
          IdentityService service = this.RequestContext.GetService<IdentityService>();
          if (!service.IsMember(this.RequestContext, GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup, this.RequestContext.UserContext) && !service.IsMember(this.RequestContext, GroupWellKnownIdentityDescriptors.ServiceUsersGroup, this.RequestContext.UserContext))
            throw new UnauthorizedAccessException();
          this.RequestContext.GetService<WorkItemTrackingConfigurationSettingService>().SetMaxBuildListSize(this.RequestContext, maxBuildListSize);
        }
      }
      catch (Exception ex)
      {
        this.HandleException(ex);
        throw;
      }
      finally
      {
        this.LeaveMethod();
        this.RequestContext.TraceLeave(900658, "WebServices", nameof (ConfigurationSettingsService), nameof (SetMaxBuildListSize));
      }
    }

    [WebMethod]
    [SoapHeader("requestHeader", Direction = SoapHeaderDirection.In)]
    public int GetWorkItemQueryTimeout()
    {
      this.RequestContext.TraceEnter(900410, "WebServices", nameof (ConfigurationSettingsService), nameof (GetWorkItemQueryTimeout));
      try
      {
        this.EnterMethod(new MethodInformation(nameof (GetWorkItemQueryTimeout), MethodType.Normal, EstimatedMethodCost.Low));
        if (!this.User.Identity.IsAuthenticated)
          throw new SoapException(ResourceStrings.Get("BadServerConfig"), Soap12FaultCodes.ReceiverFaultCode, WorkItemTrackingFaultCodes.BadServerConfig);
        return this.RequestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(this.RequestContext).WorkItemQueryTimeoutInSecond;
      }
      catch (Exception ex)
      {
        this.HandleException(ex);
        throw;
      }
      finally
      {
        this.LeaveMethod();
        this.RequestContext.TraceLeave(900659, "WebServices", nameof (ConfigurationSettingsService), nameof (GetWorkItemQueryTimeout));
      }
    }

    [WebMethod]
    [SoapHeader("requestHeader", Direction = SoapHeaderDirection.In)]
    public void SetWorkItemQueryTimeout(int workItemQueryTimeout)
    {
      this.RequestContext.TraceEnter(900411, "WebServices", nameof (ConfigurationSettingsService), nameof (SetWorkItemQueryTimeout));
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (SetWorkItemQueryTimeout), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (workItemQueryTimeout), (object) workItemQueryTimeout);
        this.EnterMethod(methodInformation);
        if (this.RequestContext.ExecutionEnvironment.IsHostedDeployment)
        {
          this.Context.Response.StatusCode = 404;
          this.Context.Response.End();
        }
        else
        {
          if (!this.User.Identity.IsAuthenticated)
            throw new SoapException(ResourceStrings.Get("BadServerConfig"), Soap12FaultCodes.ReceiverFaultCode, WorkItemTrackingFaultCodes.BadServerConfig);
          IdentityService service = this.RequestContext.GetService<IdentityService>();
          if (!service.IsMember(this.RequestContext, GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup, this.RequestContext.UserContext) && !service.IsMember(this.RequestContext, GroupWellKnownIdentityDescriptors.ServiceUsersGroup, this.RequestContext.UserContext))
            throw new UnauthorizedAccessException();
          this.RequestContext.GetService<WorkItemTrackingConfigurationSettingService>().SetWorkItemQueryTimeout(this.RequestContext, workItemQueryTimeout);
        }
      }
      catch (Exception ex)
      {
        this.HandleException(ex);
        throw;
      }
      finally
      {
        this.LeaveMethod();
        this.RequestContext.TraceLeave(900660, "WebServices", nameof (ConfigurationSettingsService), nameof (SetWorkItemQueryTimeout));
      }
    }
  }
}
