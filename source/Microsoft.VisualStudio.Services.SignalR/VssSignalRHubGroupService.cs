// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SignalR.VssSignalRHubGroupService
// Assembly: Microsoft.VisualStudio.Services.SignalR, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BD148864-3B8A-4D7D-BD16-EF04E9549DC9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SignalR.dll

using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Transports;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.SignalR.DataAccess;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.SignalR
{
  internal class VssSignalRHubGroupService : IVssSignalRHubGroupService, IVssFrameworkService
  {
    private DateTime m_lastHeartbeatTime;
    private TimeSpan m_heartbeatInterval;
    private TeamFoundationTask m_heartbeatTask;
    private ITransportHeartbeat m_transportHeartbeat;
    private static readonly RegistryQuery s_settingsRootQuery = (RegistryQuery) "/Service/SignalR/Settings/...";
    private static readonly RegistryQuery s_heartbeatFeaturePath = (RegistryQuery) "/FeatureAvailability/Entries/VisualStudio.Services.SignalR.DisconnectOnlyIfKeepAliveIsLost/...";

    public void AddConnectionToGroup(
      IVssRequestContext requestContext,
      string hubName,
      string groupName,
      string connectionId)
    {
      using (new MethodScope(requestContext, nameof (VssSignalRHubGroupService), nameof (AddConnectionToGroup)))
        requestContext.GetService<VssSignalRHubGroupCache>().AddConnections(requestContext, new VssSignalRHubGroupId(hubName, groupName), connectionId, new Func<IVssRequestContext, VssSignalRHubGroupId, string, VssSignalRHubGroupConnection>(this.AddConnectionToDatabase));
    }

    public void QueueConnectionCleanUpJob(IVssRequestContext requestContext)
    {
      using (new MethodScope(requestContext, nameof (VssSignalRHubGroupService), nameof (QueueConnectionCleanUpJob)))
      {
        int timeoutForMonitoring = requestContext.GetService<IVssSignalRConfigurationService>().GetConnectionCleanupTimeoutForMonitoring(requestContext);
        ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
        using (requestContext.AllowAnonymousOrPublicUserWrites())
          service.QueueDelayedJobs(requestContext, (IEnumerable<Guid>) new Guid[1]
          {
            SignalRConstants.VssSignalRConnectionCleanupJobId
          }, timeoutForMonitoring);
      }
    }

    public void CleanupExpiredConnections(IVssRequestContext requestContext)
    {
      using (new MethodScope(requestContext, nameof (VssSignalRHubGroupService), nameof (CleanupExpiredConnections)))
      {
        IVssSignalRConfigurationService service = requestContext.GetService<IVssSignalRConfigurationService>();
        int timeoutForMonitoring1 = service.GetConnectionCleanupTimeoutForMonitoring(requestContext);
        int timeoutForMonitoring2 = service.GetGroupCleanupTimeoutForMonitoring(requestContext);
        using (VssSignalRHubComponent component = requestContext.CreateComponent<VssSignalRHubComponent>())
        {
          IList<VssSignalRHubGroupConnection> expiredConnections = component.CleanupConnections(TimeSpan.FromSeconds((double) timeoutForMonitoring2), TimeSpan.FromSeconds((double) timeoutForMonitoring1));
          requestContext.TraceDataConditionally(10017096, TraceLevel.Verbose, "SignalR", nameof (VssSignalRHubGroupService), "Removed TrackedConnections", (Func<object>) (() => (object) new
          {
            expiredConnections = expiredConnections
          }), nameof (CleanupExpiredConnections));
        }
      }
    }

    public VssSignalRHubGroup GetGroup(
      IVssRequestContext requestContext,
      string hubName,
      string groupName)
    {
      using (new MethodScope(requestContext, nameof (VssSignalRHubGroupService), nameof (GetGroup)))
        return requestContext.GetService<VssSignalRHubGroupCache>().GetGroup(requestContext, hubName, groupName, new Func<IVssRequestContext, VssSignalRHubGroupId, VssSignalRHubGroup>(this.ReadGroupFromDatabase));
    }

    public void RemoveConnectionFromGroup(
      IVssRequestContext requestContext,
      string hubName,
      string groupName,
      string connectionId)
    {
      using (new MethodScope(requestContext, nameof (VssSignalRHubGroupService), nameof (RemoveConnectionFromGroup)))
      {
        ArgumentUtility.CheckStringForNullOrEmpty(hubName, nameof (hubName));
        ArgumentUtility.CheckStringForNullOrEmpty(groupName, nameof (groupName));
        ArgumentUtility.CheckStringForNullOrEmpty(connectionId, nameof (connectionId));
        requestContext.GetService<VssSignalRHubGroupCache>().RemoveConnection(requestContext, new VssSignalRHubGroupId(hubName, groupName), connectionId, new Func<IVssRequestContext, VssSignalRHubGroupId, string, IList<VssSignalRHubGroupConnection>>(this.RemoveConnectionFromDatabase));
      }
    }

    public void RemoveConnectionFromAllGroups(
      IVssRequestContext requestContext,
      string hubName,
      string connectionId)
    {
      using (new MethodScope(requestContext, nameof (VssSignalRHubGroupService), nameof (RemoveConnectionFromAllGroups)))
      {
        ArgumentUtility.CheckStringForNullOrEmpty(hubName, nameof (hubName));
        ArgumentUtility.CheckStringForNullOrEmpty(connectionId, nameof (connectionId));
        requestContext.GetService<VssSignalRHubGroupCache>().RemoveConnection(requestContext, new VssSignalRHubGroupId(hubName, (string) null), connectionId, new Func<IVssRequestContext, VssSignalRHubGroupId, string, IList<VssSignalRHubGroupConnection>>(this.RemoveConnectionFromDatabase));
      }
    }

    private VssSignalRHubGroup ReadGroupFromDatabase(
      IVssRequestContext requestContext,
      VssSignalRHubGroupId groupId)
    {
      using (new MethodScope(requestContext, nameof (VssSignalRHubGroupService), nameof (ReadGroupFromDatabase)))
      {
        using (VssSignalRHubComponent component = requestContext.CreateComponent<VssSignalRHubComponent>())
        {
          VssSignalRHubGroup group = component.GetGroup(groupId.HubName, groupId.GroupName);
          if (component.Version < 2)
            return group;
          VssSignalRHubGroup vssSignalRhubGroup = group;
          if (vssSignalRhubGroup == null)
            vssSignalRhubGroup = new VssSignalRHubGroup()
            {
              GroupId = groupId
            };
          return vssSignalRhubGroup;
        }
      }
    }

    private VssSignalRHubGroupConnection AddConnectionToDatabase(
      IVssRequestContext requestContext,
      VssSignalRHubGroupId groupId,
      string connectionId)
    {
      using (new MethodScope(requestContext, nameof (VssSignalRHubGroupService), nameof (AddConnectionToDatabase)))
      {
        using (requestContext.AllowAnonymousOrPublicUserWrites())
        {
          using (VssSignalRHubComponent component = requestContext.CreateComponent<VssSignalRHubComponent>())
          {
            VssSignalRHubGroupConnection addedConnection = component.AddConnectionToGroup(groupId.HubName, groupId.GroupName, connectionId, requestContext.GetUserId());
            requestContext.TraceConditionally(10017097, TraceLevel.Verbose, "SignalR", nameof (VssSignalRHubGroupService), (Func<string>) (() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Added TrackedConnection GroupId: {0}.{1} ConnectionId: {2} UserId: {3}", (object) addedConnection.GroupId.HubName, (object) addedConnection.GroupId.GroupName, (object) addedConnection.ConnectionId, (object) addedConnection.UserId)));
            return addedConnection;
          }
        }
      }
    }

    private IList<VssSignalRHubGroupConnection> RemoveConnectionFromDatabase(
      IVssRequestContext requestContext,
      VssSignalRHubGroupId groupId,
      string connectionId)
    {
      using (new MethodScope(requestContext, nameof (VssSignalRHubGroupService), nameof (RemoveConnectionFromDatabase)))
      {
        using (requestContext.AllowAnonymousOrPublicUserWrites())
        {
          using (VssSignalRHubComponent component = requestContext.CreateComponent<VssSignalRHubComponent>())
          {
            IList<VssSignalRHubGroupConnection> removedConnections = component.RemoveConnectionFromGroup(groupId.HubName, groupId.GroupName, connectionId);
            requestContext.TraceDataConditionally(10017099, TraceLevel.Verbose, "SignalR", nameof (VssSignalRHubGroupService), "Removed TrackedConnections", (Func<object>) (() => (object) new
            {
              removedConnections = removedConnections
            }), nameof (RemoveConnectionFromDatabase));
            return removedConnections;
          }
        }
      }
    }

    private void CheckConnections(IVssRequestContext requestContext, object taskArgs)
    {
      using (new MethodScope(requestContext, nameof (VssSignalRHubGroupService), nameof (CheckConnections)))
      {
        HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.Ordinal);
        using (VssSignalRHubComponent component = requestContext.CreateComponent<VssSignalRHubComponent>())
          stringSet.UnionWith((IEnumerable<string>) component.GetConnectionIds());
        if (stringSet.Count == 0)
          return;
        HashSet<string> activeConnectionIds = new HashSet<string>();
        foreach (ITrackingConnection trackingConnection in this.m_transportHeartbeat.GetConnections().Where<ITrackingConnection>((Func<ITrackingConnection, bool>) (x => x.IsAlive)))
        {
          if (stringSet.Contains(trackingConnection.ConnectionId))
            activeConnectionIds.Add(trackingConnection.ConnectionId);
        }
        if (activeConnectionIds.Count > 0)
        {
          using (VssSignalRHubComponent component = requestContext.CreateComponent<VssSignalRHubComponent>())
            component.UpdateConnections((IEnumerable<string>) activeConnectionIds);
          requestContext.TraceDataConditionally(10017098, TraceLevel.Verbose, "SignalR", nameof (VssSignalRHubGroupService), "Updated TrackedConnections Heartbeat", (Func<object>) (() => (object) new
          {
            activeConnectionIds = activeConnectionIds
          }), nameof (CheckConnections));
        }
        this.m_lastHeartbeatTime = DateTime.UtcNow;
      }
    }

    private void SettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      TimeSpan heartbeatInterval = this.GetHeartbeatInterval(requestContext);
      if (heartbeatInterval == this.m_heartbeatInterval)
        return;
      DateTime startTime = DateTime.UtcNow;
      TimeSpan timeSpan = DateTime.UtcNow - this.m_lastHeartbeatTime;
      if (timeSpan < heartbeatInterval)
        startTime = startTime.Add(heartbeatInterval - timeSpan);
      this.m_heartbeatInterval = heartbeatInterval;
      this.m_heartbeatTask = new TeamFoundationTask(new TeamFoundationTaskCallback(this.CheckConnections), (object) null, startTime, Convert.ToInt32(heartbeatInterval.TotalMilliseconds));
      requestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>().AddTask(requestContext.ServiceHost.InstanceId, this.m_heartbeatTask);
    }

    private TimeSpan GetHeartbeatInterval(IVssRequestContext requestContext) => TimeSpan.FromSeconds((double) requestContext.GetService<IVssSignalRConfigurationService>().GetHeartbeatIntervalForMonitoringKeepAlive(requestContext));

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext)
    {
      requestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>().RemoveTask(requestContext.ServiceHost.InstanceId, this.m_heartbeatTask);
      requestContext.GetService<IVssRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.SettingsChanged));
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext)
    {
      this.m_transportHeartbeat = GlobalHost.DependencyResolver.Resolve<ITransportHeartbeat>();
      requestContext.GetService<IVssRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.SettingsChanged), true, (IEnumerable<RegistryQuery>) new RegistryQuery[2]
      {
        VssSignalRHubGroupService.s_settingsRootQuery,
        VssSignalRHubGroupService.s_heartbeatFeaturePath
      });
      this.m_heartbeatInterval = this.GetHeartbeatInterval(requestContext);
      this.m_heartbeatTask = new TeamFoundationTask(new TeamFoundationTaskCallback(this.CheckConnections), (object) null, Convert.ToInt32(this.m_heartbeatInterval.TotalMilliseconds));
      requestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>().AddTask(requestContext.ServiceHost.InstanceId, this.m_heartbeatTask);
    }
  }
}
