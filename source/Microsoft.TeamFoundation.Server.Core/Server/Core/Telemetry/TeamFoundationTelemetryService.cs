// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Telemetry.TeamFoundationTelemetryService
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.Configuration;
using Microsoft.VisualStudio.Telemetry;
using System;
using System.Net;

namespace Microsoft.TeamFoundation.Server.Core.Telemetry
{
  public sealed class TeamFoundationTelemetryService : IVssFrameworkService
  {
    private TelemetrySession m_telemetrySession;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      CachedRegistryService registryService = systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) ? systemRequestContext.GetService<CachedRegistryService>() : throw new NotSupportedException("Only supported for deployment hosts");
      registryService.RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnOptInSettingsChanged), FrameworkServerConstants.CollectTeamFoundationServerSqmData + "/...");
      this.m_telemetrySession = TelemetryService.DefaultSession;
      this.m_telemetrySession.Start();
      this.m_telemetrySession.IsOptedIn = this.IsCustomerOptedIn(systemRequestContext);
      this.m_telemetrySession.SetSharedProperty(TelemetryPropertyName.OSEdition, (object) OSDetails.Edition);
      this.m_telemetrySession.SetSharedProperty(TelemetryPropertyName.IsHosted, (object) systemRequestContext.ExecutionEnvironment.IsHostedDeployment);
      this.m_telemetrySession.SetSharedProperty(TelemetryPropertyName.ServerId, (object) systemRequestContext.ServiceHost.DeploymentServiceHost.InstanceId);
      this.m_telemetrySession.SetSharedProperty(TelemetryPropertyName.ServerVersion, (object) systemRequestContext.ServiceHost.DeploymentServiceHost.DatabaseProperties.ServiceLevel);
      bool propertyValue1 = TeamFoundationTelemetryService.IsOnMicrosoftNetwork();
      this.m_telemetrySession.SetSharedProperty(TelemetryPropertyName.IsInternal, (object) propertyValue1);
      if (propertyValue1)
      {
        this.m_telemetrySession.SetSharedProperty(TelemetryPropertyName.InternalUserName, (object) Environment.UserName);
        this.m_telemetrySession.SetSharedProperty(TelemetryPropertyName.InternalUserDomainName, (object) Environment.UserDomainName);
        this.m_telemetrySession.SetSharedProperty(TelemetryPropertyName.InternalMachineName, (object) Environment.MachineName);
      }
      string propertyValue2 = registryService.GetValue(systemRequestContext, (RegistryQuery) FrameworkServerConstants.InstalledUICulture, false, (string) null);
      if (string.IsNullOrEmpty(propertyValue2))
        return;
      this.m_telemetrySession.SetSharedProperty(TelemetryPropertyName.BuildLocale, (object) propertyValue2);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext) => this.m_telemetrySession = (TelemetrySession) null;

    public void PostEvent(TelemetryEvent telemetryEvent)
    {
      if (!this.IsOptedIn && !telemetryEvent.IsOptOutFriendly)
        return;
      this.m_telemetrySession.PostEvent(telemetryEvent);
    }

    public bool IsOptedIn
    {
      get
      {
        TelemetrySession telemetrySession = this.m_telemetrySession;
        return telemetrySession != null && telemetrySession.IsOptedIn;
      }
    }

    internal static bool IsOnMicrosoftNetwork()
    {
      bool flag = false;
      try
      {
        if (EnvironmentHandler.IsMachineInWorkgroup())
        {
          DeviceJoinInfomation deviceJoinInfomation;
          if (OSDetails.IsMachineAadJoined(out deviceJoinInfomation))
          {
            if (string.Equals(deviceJoinInfomation.TenantId, "72f988bf-86f1-41af-91ab-2d7cd011db47", StringComparison.OrdinalIgnoreCase))
              flag = true;
          }
        }
        else
        {
          IPHostEntry hostEntry = Dns.GetHostEntry(Environment.MachineName);
          if (hostEntry != null)
          {
            string hostName = hostEntry.HostName;
            flag = !string.IsNullOrEmpty(hostName) && VssStringComparer.DomainName.Contains(hostName, ".microsoft.com");
          }
        }
      }
      catch (Exception ex)
      {
        flag = false;
      }
      return flag;
    }

    private void OnOptInSettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.m_telemetrySession.IsOptedIn = this.IsCustomerOptedIn(requestContext);
    }

    private bool IsCustomerOptedIn(IVssRequestContext requestContext)
    {
      try
      {
        return requestContext.GetService<CachedRegistryService>().GetValue<bool>(requestContext, (RegistryQuery) FrameworkServerConstants.CollectTeamFoundationServerSqmData, false);
      }
      catch
      {
      }
      return false;
    }
  }
}
