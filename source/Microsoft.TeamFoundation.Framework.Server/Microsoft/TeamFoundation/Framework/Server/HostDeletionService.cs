// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HostDeletionService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Partitioning;
using Microsoft.VisualStudio.Services.Partitioning.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class HostDeletionService : 
    IInternalHostDeletionService,
    IHostDeletionService,
    IVssFrameworkService
  {
    private const string s_stopHostTimeoutSeconds = "/Service/AccountDeletionService/StopHostTimeoutSeconds";
    private const string s_deleteHostRetries = "Service/AccountDeleteService/DeleteHostRetries";
    private const string s_area = "HostDeletionService";
    private const string s_layer = "IVssFrameworkService";

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext) => systemRequestContext.CheckDeploymentRequestContext();

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    void IHostDeletionService.DeleteHost(
      IVssRequestContext deploymentRequestContext,
      Guid hostId,
      DeleteHostResourceOptions hostDeletionOptions,
      HostDeletionReason hostDeletionReason,
      ITFLogger logger)
    {
      HostProperties hostProperties = deploymentRequestContext.GetService<ITeamFoundationHostManagementService>().QueryServiceHostPropertiesCached(deploymentRequestContext, hostId);
      this.ThrowIfAccessDenied(deploymentRequestContext);
      logger = logger ?? (ITFLogger) new ServerTraceLogger();
      if (hostProperties == null)
      {
        IVssRequestContext deploymentRequestContext1 = deploymentRequestContext;
        HostProperties hostProperties1 = new HostProperties();
        hostProperties1.Id = hostId;
        int hostDeletionOptions1 = (int) hostDeletionOptions;
        ITFLogger logger1 = logger;
        ((IInternalHostDeletionService) this).RemoveHostInstanceMapping(deploymentRequestContext1, hostProperties1, (DeleteHostResourceOptions) hostDeletionOptions1, logger1);
      }
      else
      {
        if (!hostProperties.IsVirtualServiceHost() && string.Equals(deploymentRequestContext.GetService<ITeamFoundationDatabaseManagementService>().GetDatabase(deploymentRequestContext, hostProperties.DatabaseId).PoolName, DatabaseManagementConstants.MigrationStagingPool, StringComparison.OrdinalIgnoreCase))
          throw new InvalidOperationException(string.Format("Host {0} can not be deleted because it resides in the {1}", (object) hostId, (object) DatabaseManagementConstants.MigrationStagingPool));
        RetryManager retryManager = new RetryManager(deploymentRequestContext.GetService<IVssRegistryService>().GetValue<int>(deploymentRequestContext, (RegistryQuery) "Service/AccountDeleteService/DeleteHostRetries", 1), (Action<Exception>) (e =>
        {
          if (!(e is HostStatusChangeException))
            throw e;
          logger.Info(e.Message);
        }));
        switch (hostProperties.HostType)
        {
          case TeamFoundationHostType.Application:
            retryManager.Invoke((Action) (() => this.RemoveApplication(deploymentRequestContext, hostProperties, hostDeletionOptions, hostDeletionReason, logger)));
            break;
          case TeamFoundationHostType.ProjectCollection:
            retryManager.Invoke((Action) (() => this.RemoveCollection(deploymentRequestContext, hostProperties, hostDeletionOptions, hostDeletionReason, !hostDeletionOptions.HasFlag((Enum) DeleteHostResourceOptions.SkipVirtualParentCleanup), logger)));
            break;
          default:
            throw new UnexpectedHostTypeException(hostProperties.HostType);
        }
      }
    }

    private void RemoveApplication(
      IVssRequestContext deploymentRequestContext,
      HostProperties hostProperties,
      DeleteHostResourceOptions hostDeletionOptions,
      HostDeletionReason hostDeletionReason,
      ITFLogger logger)
    {
      deploymentRequestContext.TraceEnter(54350083, nameof (HostDeletionService), "IVssFrameworkService", nameof (RemoveApplication));
      try
      {
        if (this.IsHostAlreadyDeleted(deploymentRequestContext, hostProperties.Id, logger))
          return;
        foreach (TeamFoundationServiceHostProperties childHost in this.GetChildHosts(deploymentRequestContext, hostProperties.Id))
        {
          this.RemoveCollection(deploymentRequestContext, (HostProperties) childHost, hostDeletionOptions, hostDeletionReason, false, logger);
          logger.Info("{0} collection removed.", (object) childHost.Id);
        }
        List<TeamFoundationServiceHostProperties> childHosts = this.GetChildHosts(deploymentRequestContext, hostProperties.Id);
        if (childHosts.Count > 0)
          throw new Exception(string.Format("{0} collections found when there should be no collections remaining at this point.", (object) childHosts.Count));
        IInternalHostDeletionService hostDeletionService = (IInternalHostDeletionService) this;
        try
        {
          hostDeletionService.StopHost(deploymentRequestContext, hostProperties, hostDeletionOptions);
          hostDeletionService.DeleteServiceHost(deploymentRequestContext, hostProperties, hostDeletionOptions, hostDeletionReason, logger);
          hostDeletionService.RemoveHostInstanceMapping(deploymentRequestContext, hostProperties, hostDeletionOptions, logger);
        }
        catch (HostDoesNotExistException ex)
        {
          deploymentRequestContext.TraceException(223676051, nameof (HostDeletionService), "IVssFrameworkService", (Exception) ex);
        }
        logger.Info("Application host and child hosts were removed.");
      }
      finally
      {
        deploymentRequestContext.TraceLeave(34521107, nameof (HostDeletionService), "IVssFrameworkService", nameof (RemoveApplication));
      }
    }

    private void RemoveCollection(
      IVssRequestContext deploymentRequestContext,
      HostProperties hostProperties,
      DeleteHostResourceOptions hostDeletionOptions,
      HostDeletionReason hostDeletionReason,
      bool attemptToDeleteVirtualParent,
      ITFLogger logger)
    {
      deploymentRequestContext.TraceEnter(552793102, nameof (HostDeletionService), "IVssFrameworkService", nameof (RemoveCollection));
      try
      {
        if (this.IsHostAlreadyDeleted(deploymentRequestContext, hostProperties.Id, logger))
          return;
        try
        {
          HostDeletionService hostDeletionService = this;
          bool skipPublishNotification = hostDeletionReason == HostDeletionReason.HostMigrated;
          ((IInternalHostDeletionService) hostDeletionService).StopHost(deploymentRequestContext, hostProperties, hostDeletionOptions, skipPublishNotification);
          ((IInternalHostDeletionService) hostDeletionService).DeleteServiceHost(deploymentRequestContext, hostProperties, hostDeletionOptions, hostDeletionReason, logger);
          ((IInternalHostDeletionService) hostDeletionService).RemoveHostInstanceMapping(deploymentRequestContext, hostProperties, hostDeletionOptions, logger);
        }
        catch (HostDoesNotExistException ex)
        {
          deploymentRequestContext.TraceException(179835379, nameof (HostDeletionService), "IVssFrameworkService", (Exception) ex);
        }
        if (!attemptToDeleteVirtualParent || this.GetChildHosts(deploymentRequestContext, hostProperties.ParentId).Count != 0)
          return;
        HostProperties hostProperties1 = (HostProperties) deploymentRequestContext.GetService<ITeamFoundationHostManagementService>().QueryServiceHostProperties(deploymentRequestContext, hostProperties.ParentId);
        if (hostProperties1 == null || !hostProperties1.IsVirtualServiceHost())
          return;
        this.RemoveApplication(deploymentRequestContext, hostProperties1, hostDeletionOptions, hostDeletionReason, logger);
      }
      finally
      {
        deploymentRequestContext.TraceLeave(576977596, nameof (HostDeletionService), "IVssFrameworkService", nameof (RemoveCollection));
      }
    }

    void IInternalHostDeletionService.RemoveHostInstanceMapping(
      IVssRequestContext deploymentRequestContext,
      HostProperties hostProperties,
      DeleteHostResourceOptions hostDeletionOptions,
      ITFLogger logger)
    {
      if (hostProperties == null)
        return;
      if (hostDeletionOptions.HasFlag((Enum) DeleteHostResourceOptions.SkipInstanceManagementCleanup))
        return;
      try
      {
        IPartitioningService service = deploymentRequestContext.GetService<IPartitioningService>();
        Partition partition = service.QueryPartition<Guid>(deploymentRequestContext, hostProperties.Id, ServiceInstanceTypes.SPS);
        if (partition != null)
        {
          if (deploymentRequestContext.ServiceInstanceType() == ServiceInstanceTypes.SPS)
          {
            if (!(partition.Container.ContainerId == deploymentRequestContext.ServiceHost.InstanceId))
              return;
            service.DeletePartition<Guid>(deploymentRequestContext, hostProperties.Id, ServiceInstanceTypes.SPS);
          }
          else if (hostProperties.IsVirtualServiceHost())
            deploymentRequestContext.GetService<IVirtualHostInstanceMappingRegistrationService>().UnRegisterVirtualHostInstanceMapping(deploymentRequestContext, hostProperties.Id);
          else
            deploymentRequestContext.GetService<IInstanceManagementService>().RemoveHostInstanceMapping(deploymentRequestContext, hostProperties.Id, overrideInstanceCheck: hostDeletionOptions.HasFlag((Enum) DeleteHostResourceOptions.OverrideInstanceCheck));
        }
        else
          deploymentRequestContext.TraceAlways(287568391, TraceLevel.Warning, nameof (HostDeletionService), "IVssFrameworkService", string.Format("Failed to find partition for service host {0}. Not removing host instance mappings because it has already been deleted from SPS.", (object) hostProperties.Id));
      }
      catch (Exception ex)
      {
        deploymentRequestContext.TraceException(527992149, nameof (HostDeletionService), "IVssFrameworkService", ex);
        logger.Warning(ex.ToReadableStackTrace());
        if (!hostDeletionOptions.HasFlag((Enum) DeleteHostResourceOptions.EnsureInstanceManagementCleanupSuccessful))
          return;
        throw;
      }
    }

    void IInternalHostDeletionService.StopHost(
      IVssRequestContext deploymentRequestContext,
      HostProperties hostProperties,
      DeleteHostResourceOptions deleteHostResourceOptions,
      bool skipPublishNotification)
    {
      deploymentRequestContext.TraceEnter(328765059, nameof (HostDeletionService), "IVssFrameworkService", "StopHost");
      try
      {
        if (hostProperties == null)
          return;
        if (!hostProperties.IsVirtualServiceHost() || !skipPublishNotification)
          this.PublishHostDeletedNotification(deploymentRequestContext, NotificationType.DecisionPoint, hostProperties);
        int num = deploymentRequestContext.GetService<IVssRegistryService>().GetValue<int>(deploymentRequestContext, (RegistryQuery) "/Service/AccountDeletionService/StopHostTimeoutSeconds", 960);
        ITeamFoundationHostManagementService service = deploymentRequestContext.GetService<ITeamFoundationHostManagementService>();
        string reason = hostProperties?.StatusReason ?? Microsoft.VisualStudio.Services.Account.AccountResources.AccountIsBeingDeleted();
        ServiceHostSubStatus subStatus = deleteHostResourceOptions.HasFlag((Enum) DeleteHostResourceOptions.SkipSubStatusUpdate) ? hostProperties.SubStatus : ServiceHostSubStatus.Deleting;
        if (!service.StopHost(deploymentRequestContext, hostProperties.Id, subStatus, reason, TimeSpan.FromSeconds((double) num)))
          throw new HostStatusChangeException(string.Format("Failed to stop host {0} in {1} seconds.", (object) hostProperties.Id.ToString(), (object) num));
        Guid id = hostProperties.Id;
        hostProperties = (HostProperties) service.QueryServiceHostProperties(deploymentRequestContext, hostProperties.Id, ServiceHostFilterFlags.IncludeChildren);
        if (hostProperties == null)
        {
          deploymentRequestContext.Trace(48044959, TraceLevel.Warning, nameof (HostDeletionService), "IVssFrameworkService", string.Format("Host {0} not found. The host may have already been deleted.", (object) id));
          throw new HostDoesNotExistException(id);
        }
        if (hostProperties.Status != TeamFoundationServiceHostStatus.Stopped)
          throw new HostStatusChangeException(string.Format("Host is not stopped as expected. ({0}) ({1})", (object) hostProperties.ToBriefString(), (object) hostProperties.Status));
      }
      finally
      {
        deploymentRequestContext.TraceLeave(176590779, nameof (HostDeletionService), "IVssFrameworkService", "StopHost");
      }
    }

    void IInternalHostDeletionService.DeleteServiceHost(
      IVssRequestContext deploymentRequestContext,
      HostProperties hostProperties,
      DeleteHostResourceOptions hostDeletionOptions,
      HostDeletionReason hostDeletionReason,
      ITFLogger logger)
    {
      deploymentRequestContext.TraceEnter(37232485, nameof (HostDeletionService), "IVssFrameworkService", "DeleteServiceHost");
      try
      {
        logger.Info(string.Format("Clearing job queue entries for {0}.", (object) hostProperties.Id));
        deploymentRequestContext.GetService<TeamFoundationJobService>().ClearJobQueueForOneHost(deploymentRequestContext, hostProperties.Id);
        logger.Warning(string.Format("Removing service host {0}.", (object) hostProperties.Id));
        deploymentRequestContext.TraceAlways(9365088, TraceLevel.Warning, nameof (HostDeletionService), "IVssFrameworkService", string.Format("{0}: Removing service host {1}. Full stack: {2}.", (object) "IHostDeletionService", (object) hostProperties.Id, (object) EnvironmentWrapper.ToReadableStackTrace()));
        deploymentRequestContext.GetService<ITeamFoundationHostManagementService>().DeleteServiceHost(deploymentRequestContext, hostProperties.Id, hostDeletionReason, hostDeletionOptions);
        if (!hostProperties.IsVirtualServiceHost())
          this.PublishHostDeletedNotification(deploymentRequestContext, NotificationType.Notification, hostProperties);
        deploymentRequestContext.ServiceHost.DeploymentServiceHost.ServiceHostInternal().FlushNotificationQueue(deploymentRequestContext);
      }
      finally
      {
        deploymentRequestContext.TraceLeave(33029007, nameof (HostDeletionService), "IVssFrameworkService", "DeleteServiceHost");
      }
    }

    private void ThrowIfAccessDenied(IVssRequestContext deploymentRequestContext)
    {
      IVssRequestContext vssRequestContext = deploymentRequestContext.To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, FrameworkSecurity.FrameworkNamespaceId).CheckPermission(vssRequestContext, FrameworkSecurity.FrameworkNamespaceToken, 2, false);
    }

    private bool IsHostAlreadyDeleted(
      IVssRequestContext deploymentRequestContext,
      Guid hostId,
      ITFLogger logger)
    {
      if (deploymentRequestContext.GetService<ITeamFoundationHostManagementService>().QueryServiceHostProperties(deploymentRequestContext, hostId) != null)
        return false;
      logger.Info(string.Format("{0} was already deleted.", (object) hostId));
      return true;
    }

    private List<TeamFoundationServiceHostProperties> GetChildHosts(
      IVssRequestContext deploymentRequestContext,
      Guid applicationHostId)
    {
      TeamFoundationServiceHostProperties serviceHostProperties = deploymentRequestContext.GetService<ITeamFoundationHostManagementService>().QueryServiceHostProperties(deploymentRequestContext, applicationHostId, ServiceHostFilterFlags.IncludeChildren);
      return serviceHostProperties != null && serviceHostProperties.Children != null ? serviceHostProperties.Children : new List<TeamFoundationServiceHostProperties>();
    }

    private void PublishHostDeletedNotification(
      IVssRequestContext deploymentRequestContext,
      NotificationType notificationType,
      HostProperties hostProperties)
    {
      HostModifiedMessage hostModifiedMessage = HostModifiedMessage.CreateHostModifiedMessage(hostProperties.HostType);
      hostModifiedMessage.HostId = hostProperties.Id;
      hostModifiedMessage.ModificationType = HostModificationType.Deleted;
      ITeamFoundationEventService service = deploymentRequestContext.GetService<ITeamFoundationEventService>();
      if (notificationType != NotificationType.DecisionPoint)
      {
        if (notificationType != NotificationType.Notification)
          return;
        service.PublishNotification(deploymentRequestContext, (object) hostModifiedMessage);
      }
      else
        service.PublishDecisionPoint(deploymentRequestContext, (object) hostModifiedMessage);
    }
  }
}
