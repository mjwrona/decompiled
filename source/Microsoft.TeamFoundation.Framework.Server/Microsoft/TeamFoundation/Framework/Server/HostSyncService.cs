// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HostSyncService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.HostManagement;
using Microsoft.VisualStudio.Services.HostManagement.Server;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class HostSyncService : IInternalHostSyncService, IHostSyncService, IVssFrameworkService
  {
    internal const string AccountHostCreationStatus = "Creating Account Host";
    internal const string CollectionHostCreationStatus = "Creating Collection Host";
    internal const int FailedRetryTracePoint = 60456;
    private const int c_hostCreationMinTimeToDumpSqlStaticsMilliseconds = 30000;
    internal static readonly string s_area = "HostManagement";
    internal static readonly string s_layer = nameof (HostSyncService);

    public void ServiceStart(IVssRequestContext systemRequestContext) => systemRequestContext.CheckDeploymentRequestContext();

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public virtual bool EnsureHostUpdated(IVssRequestContext requestContext, Guid hostId) => this.EnsureHostUpdated(requestContext, hostId, false, (IDictionary<string, string>) null);

    internal virtual bool EnsureHostUpdated(
      IVssRequestContext requestContext,
      Guid hostId,
      bool syncExistingHostsOnly,
      IDictionary<string, string> servicingTokens)
    {
      if (!requestContext.ServiceHost.IsProduction && requestContext.IsTracing(5604611, TraceLevel.Info, HostSyncService.s_area, HostSyncService.s_layer))
      {
        string str = EnvironmentWrapper.ToReadableStackTrace().ToString();
        requestContext.Trace(5604611, TraceLevel.Info, HostSyncService.s_area, HostSyncService.s_layer, "stack trace for collection rename: {0} ", (object) str);
      }
      ArgumentUtility.CheckForEmptyGuid(hostId, nameof (hostId));
      requestContext.CheckHostedDeployment();
      requestContext.CheckDeploymentRequestContext();
      requestContext = requestContext.Elevate();
      if (requestContext.ServiceInstanceType() == ServiceInstanceTypes.SPS)
        return false;
      IHostManagementService service1 = requestContext.GetService<IHostManagementService>();
      ServiceHostProperties serviceHostProperties = service1.GetServiceHostProperties(requestContext, hostId);
      if (serviceHostProperties == null)
      {
        requestContext.Trace(5002518, TraceLevel.Info, HostSyncService.s_area, HostSyncService.s_layer, "The remote properties for host {0} are null.", (object) hostId);
        return false;
      }
      IInstanceManagementService service2 = requestContext.GetService<IInstanceManagementService>();
      if (serviceHostProperties.ParentHostId != Guid.Empty)
      {
        TeamFoundationServiceHostProperties localProperties1;
        if (!this.LocalHostExistsAndWellFormed(requestContext, serviceHostProperties.ParentHostId, out localProperties1))
        {
          requestContext.Trace(5002060, TraceLevel.Info, HostSyncService.s_area, HostSyncService.s_layer, "The host {0} has a parent which doesn't exist locally or is partially synced. Attempting to sync the parent host inline", (object) serviceHostProperties.HostId, (object) serviceHostProperties.ParentHostId);
          bool flag = this.EnsureHostUpdated(requestContext, serviceHostProperties.ParentHostId, syncExistingHostsOnly, servicingTokens);
          if (!flag)
          {
            IHostSyncServiceExtension extension = requestContext.GetExtension<IHostSyncServiceExtension>();
            if (extension != null)
              flag = extension.HandleErrorInParentHostFaultIn(requestContext, serviceHostProperties);
          }
          if (!flag)
          {
            requestContext.Trace(5002525, TraceLevel.Info, HostSyncService.s_area, HostSyncService.s_layer, "Not faulting in host {0} because parent host {1} fault in failed. Check servicing logs.", (object) serviceHostProperties.HostId, (object) serviceHostProperties.ParentHostId);
            return false;
          }
        }
        else
        {
          TeamFoundationServiceHostProperties localProperties2;
          if (localProperties1.IsVirtualServiceHost() && this.LocalHostExistsAndWellFormed(requestContext, serviceHostProperties.HostId, out localProperties2) && !StringComparer.OrdinalIgnoreCase.Equals(localProperties2.Name, serviceHostProperties.Name))
            this.EnsureHostUpdated(requestContext, serviceHostProperties.ParentHostId, syncExistingHostsOnly, servicingTokens);
        }
      }
      IDisposable disposable = (IDisposable) null;
      try
      {
        if (serviceHostProperties.ParentHostId != Guid.Empty)
          disposable = HostLeaseHelper.AcquireHostLease(requestContext, serviceHostProperties.ParentHostId);
        using (HostLeaseHelper.AcquireHostLease(requestContext, serviceHostProperties.HostId))
        {
          TeamFoundationServiceHostProperties localProperties;
          if (this.LocalHostExistsAndWellFormed(requestContext, serviceHostProperties.HostId, out localProperties))
          {
            if (!StringComparer.OrdinalIgnoreCase.Equals(localProperties.Name, serviceHostProperties.Name))
            {
              serviceHostProperties = service1.GetServiceHostProperties(requestContext, hostId);
              if (serviceHostProperties == null)
              {
                requestContext.Trace(5002519, TraceLevel.Info, HostSyncService.s_area, HostSyncService.s_layer, "The remote properties for host {0} are null.", (object) hostId);
                return false;
              }
              if (!StringComparer.OrdinalIgnoreCase.Equals(localProperties.Name, serviceHostProperties.Name))
              {
                requestContext.Trace(5002520, TraceLevel.Info, HostSyncService.s_area, HostSyncService.s_layer, "Renaming host {0}.", (object) hostId);
                this.RenameHost(requestContext, serviceHostProperties);
              }
            }
            if (requestContext.IsFeatureEnabled(HostManagementConstants.HostStatusSyncFeatureFlag) && serviceHostProperties.SubStatus == ServiceHostSubStatus.Propagate && localProperties.Status != serviceHostProperties.State)
            {
              requestContext.TraceAlways(5002523, TraceLevel.Info, HostSyncService.s_area, HostSyncService.s_layer, "Host status for host '{0}' propagated from SPS.\r\nLocal state: {1} - '{2}'\r\nRemote state: {3} - '{4}'", (object) localProperties.Id, (object) localProperties.Status, (object) localProperties.StatusReason, (object) serviceHostProperties.State, (object) serviceHostProperties.StatusReason);
              IVssRequestContext vssRequestContext = requestContext.Elevate();
              ITeamFoundationHostManagementService service3 = vssRequestContext.GetService<ITeamFoundationHostManagementService>();
              if (serviceHostProperties.State == TeamFoundationServiceHostStatus.Started)
                service3.StartHost(vssRequestContext, localProperties.Id, ServiceHostSubStatus.Propagate);
              else if (serviceHostProperties.State == TeamFoundationServiceHostStatus.Stopped)
                service3.StopHost(vssRequestContext, localProperties.Id, ServiceHostSubStatus.Propagate, serviceHostProperties.StatusReason, TimeSpan.FromMinutes(3.0));
            }
            requestContext.Trace(5002521, TraceLevel.Info, HostSyncService.s_area, HostSyncService.s_layer, "Host {0} existed and was well formed.", (object) hostId);
            return true;
          }
          if (syncExistingHostsOnly && localProperties == null)
          {
            requestContext.Trace(5002071, TraceLevel.Info, HostSyncService.s_area, HostSyncService.s_layer, "Host {0} will not be synchronized since {1} setting was set to true and the host doesn't exist locally.", (object) serviceHostProperties.HostId, (object) FrameworkServerConstants.SyncExistingHostsOnly);
            return false;
          }
          this.DeletePartiallyCreatedHost(requestContext, (HostProperties) localProperties);
          ServiceInstance serviceInstance = service2.GetServiceInstance(requestContext, requestContext.ServiceHost.InstanceId);
          if (serviceHostProperties.HostType == ServiceHostType.Application && !serviceInstance.SupportsPhysicalHostsForHostType(TeamFoundationHostType.Application))
          {
            requestContext.Trace(5002522, TraceLevel.Info, HostSyncService.s_area, HostSyncService.s_layer, "Creating host {0}.", (object) hostId);
            this.CreateHost(requestContext, serviceHostProperties, true, servicingTokens);
            return true;
          }
          HostInstanceMapping hostInstanceMapping = service2.GetHostInstanceMapping(requestContext, serviceHostProperties.HostId);
          if (hostInstanceMapping == null || hostInstanceMapping.ServiceInstance.InstanceId != requestContext.ServiceHost.InstanceId)
          {
            requestContext.Trace(5002524, TraceLevel.Info, HostSyncService.s_area, HostSyncService.s_layer, "Not faulting in host {0} because non-Org hosts and instance mapping was pointing to {1}.", (object) hostId, (object) (hostInstanceMapping?.ServiceInstance.InstanceId.ToString() ?? "Null"));
            return false;
          }
          if (hostInstanceMapping.Status != ServiceStatus.Assigned)
            requestContext.TraceAlways(5002072, TraceLevel.Warning, HostSyncService.s_area, HostSyncService.s_layer, string.Format("Unexpected host instance mapping status: {0}.", (object) hostInstanceMapping.Status));
          service2.UpdateHostInstanceMappingStatus(requestContext, hostId, ServiceStatus.Assigned);
          requestContext.Trace(5002525, TraceLevel.Info, HostSyncService.s_area, HostSyncService.s_layer, "Creating host {0}.", (object) hostId);
          this.CreateHost(requestContext, serviceHostProperties, servicingTokens);
          return true;
        }
      }
      finally
      {
        disposable?.Dispose();
      }
    }

    public virtual bool EnsureHostDeleted(IVssRequestContext requestContext, Guid hostId)
    {
      requestContext.Trace(5002013, TraceLevel.Info, HostSyncService.s_area, HostSyncService.s_layer, "Deleting service host {0}", (object) hostId);
      requestContext.CheckDeploymentRequestContext();
      requestContext.GetService<ITeamFoundationHostManagementService>().QueryServiceHostProperties(requestContext, hostId);
      requestContext.TraceAlways(5002053, TraceLevel.Info, HostSyncService.s_area, HostSyncService.s_layer, "Deleting host {0}", (object) hostId);
      requestContext.GetService<IHostDeletionService>().DeleteHost(requestContext, hostId, DeleteHostResourceOptions.MarkForDeletion);
      requestContext.TraceAlways(5002054, TraceLevel.Info, HostSyncService.s_area, HostSyncService.s_layer, "Host deleted {0}", (object) hostId);
      return true;
    }

    public bool RetryFailedSync(IVssRequestContext requestContext, HostProperties localProperties)
    {
      if (this.HostIsPartiallyCreated(requestContext, localProperties))
      {
        try
        {
          return this.EnsureHostUpdated(requestContext, localProperties.Id);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(60456, HostSyncService.s_area, HostSyncService.s_layer, ex);
        }
      }
      return false;
    }

    internal virtual void CreateHost(
      IVssRequestContext requestContext,
      ServiceHostProperties hostProperties,
      IDictionary<string, string> servicingTokens)
    {
      this.CreateHost(requestContext, hostProperties, false, servicingTokens);
    }

    internal virtual void CreateHost(
      IVssRequestContext requestContext,
      ServiceHostProperties hostProperties,
      bool isVirtual,
      IDictionary<string, string> servicingTokens)
    {
      IHostCreator service;
      if (hostProperties.HostType == ServiceHostType.Application)
      {
        service = (IHostCreator) requestContext.GetService<OrganizationHostCreationService>();
      }
      else
      {
        if (hostProperties.HostType != ServiceHostType.Collection)
          throw new InvalidOperationException("The operation only supports Application and Collection host types.");
        service = (IHostCreator) requestContext.GetService<CollectionHostManagementService>();
      }
      if (isVirtual)
      {
        service.CreateVirtualHost(requestContext, hostProperties);
      }
      else
      {
        SqlStatistics sqlStatistics = (SqlStatistics) null;
        Stopwatch stopwatch = Stopwatch.StartNew();
        try
        {
          if (!SqlStatisticsContext.CollectingStatistics)
            sqlStatistics = new SqlStatistics();
          service.CreateHost(requestContext, hostProperties, servicingTokens);
        }
        finally
        {
          stopwatch.Stop();
          if (stopwatch.ElapsedMilliseconds > 30000L)
            requestContext.TraceAlways(1837732, TraceLevel.Warning, HostSyncService.s_area, HostSyncService.s_layer, "{0} took longer than expected: {1}", (object) nameof (CreateHost), (object) (sqlStatistics?.ToString() ?? "check tracepoint 1277."));
          sqlStatistics?.Dispose();
        }
      }
    }

    internal virtual void RenameHost(
      IVssRequestContext requestContext,
      ServiceHostProperties hostProperties)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      ITeamFoundationHostManagementService service = vssRequestContext.GetService<ITeamFoundationHostManagementService>();
      TeamFoundationServiceHostProperties hostProperties1 = service.QueryServiceHostProperties(vssRequestContext, hostProperties.HostId);
      hostProperties1.Name = hostProperties.Name;
      service.UpdateServiceHost(vssRequestContext, hostProperties1);
    }

    public bool HostIsPartiallyCreated(
      IVssRequestContext requestContext,
      HostProperties localProperties)
    {
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment || localProperties == null || localProperties.Status != TeamFoundationServiceHostStatus.Stopped)
        return false;
      return localProperties.SubStatus == ServiceHostSubStatus.Creating || localProperties.SubStatus == ServiceHostSubStatus.Deleting;
    }

    public bool LocalHostExistsAndWellFormed(IVssRequestContext requestContext, Guid hostId) => this.LocalHostExistsAndWellFormed(requestContext, hostId, out TeamFoundationServiceHostProperties _);

    public bool LocalHostExistsAndWellFormed(
      IVssRequestContext requestContext,
      Guid hostId,
      out TeamFoundationServiceHostProperties localProperties)
    {
      ITeamFoundationHostManagementService service = requestContext.GetService<ITeamFoundationHostManagementService>();
      localProperties = service.QueryServiceHostProperties(requestContext, hostId);
      return localProperties != null && !this.HostIsPartiallyCreated(requestContext, (HostProperties) localProperties);
    }

    bool IInternalHostSyncService.EnsureHostUpdated(
      IVssRequestContext requestContext,
      Guid hostId,
      bool syncExistingHostsOnly,
      IDictionary<string, string> servicingTokens)
    {
      return this.EnsureHostUpdated(requestContext, hostId, syncExistingHostsOnly, servicingTokens);
    }

    private void DeletePartiallyCreatedHost(
      IVssRequestContext requestContext,
      HostProperties localProperties)
    {
      if (!this.HostIsPartiallyCreated(requestContext, localProperties))
        return;
      requestContext.TraceAlways(326661723, TraceLevel.Info, HostSyncService.s_area, HostSyncService.s_layer, "Deleting partially created host {0}", (object) localProperties.Id);
      requestContext.GetService<IInternalHostDeletionService>().DeleteHost(requestContext, localProperties.Id, DeleteHostResourceOptions.DeleteImmediately | DeleteHostResourceOptions.SkipInstanceManagementCleanup | DeleteHostResourceOptions.SkipVirtualParentCleanup);
      if (!requestContext.ServiceHost.ServiceHostInternal().FlushNotificationQueue(requestContext))
      {
        string message = string.Format("Flush notification queue to flush {0} out of the host management cache timed out.", (object) localProperties.Id);
        requestContext.Trace(1198898176, TraceLevel.Error, HostSyncService.s_area, HostSyncService.s_layer, message);
        throw new TimeoutException(message);
      }
    }
  }
}
