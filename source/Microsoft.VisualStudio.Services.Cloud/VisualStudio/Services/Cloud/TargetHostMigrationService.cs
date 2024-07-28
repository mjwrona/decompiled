// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.TargetHostMigrationService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Azure.Storage.Blobs.Models;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Hosting;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.HostMigration;
using Microsoft.VisualStudio.Services.Cloud.HostMigration;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class TargetHostMigrationService : IVssFrameworkService
  {
    internal static readonly string s_area = "TargetHostMigration";
    internal static readonly string s_layer = "IVssFrameworkService";
    internal static readonly string s_blobMigrationJobId = "D5CA9F39-3531-4835-A5A6-C0883D594E77";
    internal static readonly string s_databasePartitionCopyJobName = "Database Partition Copy Job";
    private static readonly string s_blobContainerCopyJobName = "Blob Container Copy Job";
    private static readonly string s_blobContainerPreMigrationJobName = "Blob Container Pre-Migration Copy Job";
    private static readonly string s_blobContainerPreMigrationCoordinatingJobName = "Blob Container Pre-Migration Copy Coordinating Job";
    private static readonly string s_blobContainerDowntimeMigrationCoordinatingJobName = "Blob Container Downtime Migration Copy Coordinating Job";
    private const string c_parallelTfsBlobMigrationCoordinatorJobExtensionName = "Microsoft.VisualStudio.Services.Cloud.JobAgentPlugins.ParallelTfsBlobMigrationCoordinatorJob";
    private const string c_parallelTfsBlobMigrationCoordinatorJobId = "8EE44A94-905D-4165-9E71-75BF28AF4521";
    private const string c_parallelTfsBlobContainerCoordinatorCopyJobName = "Parallel Blob Container Coordinator Copy Job";
    private const string c_parallelTfsBlobContainerCoordinatorPreMigrationJobName = "Parallel Blob Container Coordinator Pre-Migration Copy Job";
    private const int c_migrationJobRetries = 5;
    private const string c_bulkMigrationDatabasePrefix = "BulkMigrationDatabase";
    private static readonly string s_AdHocJobMigrationJobName = "Microsoft.TeamFoundation.JobService.Extensions.Hosting.AdHocJobMigrationJob";
    private static readonly string s_NoReenableJobsFF = "Microsoft.AzureDevOps.HostMigration.NoReenableJobsAfterMigration";
    private const string c_CreateTargetMigrationJobExtensionName = "Microsoft.TeamFoundation.JobService.Extensions.Hosting.CreateTargetMigrationJob";
    private const string c_CreateTargetMigrationJobName = "Create Target Migration Job";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      systemRequestContext.CheckSystemRequestContext();
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public TargetHostMigration GetMigrationEntry(
      IVssRequestContext requestContext,
      Guid migrationId,
      bool checkRunningJobs = false,
      bool decryptSasTokens = false,
      bool rawDatabaseEntry = false)
    {
      requestContext.TraceEnter(15288035, TargetHostMigrationService.s_area, TargetHostMigrationService.s_layer, nameof (GetMigrationEntry));
      try
      {
        TargetHostMigration migrationEntry;
        using (HostMigrationComponent component = requestContext.CreateComponent<HostMigrationComponent>())
          migrationEntry = component.GetTargetMigration(migrationId);
        if (migrationEntry == null || migrationEntry.State == TargetMigrationState.Create)
        {
          if (migrationEntry == null)
            requestContext.Trace(15288037, TraceLevel.Error, TargetHostMigrationService.s_area, TargetHostMigrationService.s_layer, string.Format("Migration entry not found.\nMigrationId: {0}", (object) migrationId));
          else
            requestContext.Trace(15288037, TraceLevel.Warning, TargetHostMigrationService.s_area, TargetHostMigrationService.s_layer, string.Format("Migration entry is in Create state waiting for completion to Created.\nMigrationId: {0}", (object) migrationId));
          migrationEntry = this.CheckCreateTargetMigrationJob(requestContext, migrationId, migrationEntry);
          return migrationEntry;
        }
        if (rawDatabaseEntry)
          return migrationEntry;
        HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (GetMigrationEntry), "Querying for a migration entry.");
        bool flag = TargetHostMigrationService.CheckForDedicatedBlobJobOnStorageMove(requestContext);
        if (checkRunningJobs && migrationEntry.StorageOnly && !flag)
        {
          HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (GetMigrationEntry), "Disabling checkRunningJobs, this is a blob only migration request.");
          checkRunningJobs = false;
        }
        if (checkRunningJobs && (migrationEntry.State == TargetMigrationState.BeginCopyJobs || migrationEntry.State == TargetMigrationState.CopyJobsComplete || migrationEntry.State == TargetMigrationState.BeginCompletePendingBlobs || migrationEntry.State == TargetMigrationState.CompletePendingBlobs))
        {
          HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (GetMigrationEntry), "Checking for running Resource Migration Jobs");
          bool onlineBlobCopy = migrationEntry.OnlineBlobCopy;
          if (onlineBlobCopy && migrationEntry.GetBlobContainers().Any<StorageMigration>((Func<StorageMigration, bool>) (sm => sm.IsSharded)))
          {
            HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (GetMigrationEntry), "Online blob copy enabled but found sharded container(s). Blocking until all containers copy.");
            onlineBlobCopy = false;
          }
          this.CheckResourceMigrationJobs(requestContext, migrationId, migrationEntry.StorageOnly, onlineBlobCopy);
          using (HostMigrationComponent component = requestContext.CreateComponent<HostMigrationComponent>())
            migrationEntry = component.GetTargetMigration(migrationId);
        }
        migrationEntry = TargetHostMigrationService.CheckBackgroundMigrationJobs(requestContext, migrationEntry);
        TeamFoundationServiceHostProperties hostProperties = requestContext.GetService<ITeamFoundationHostManagementService>().QueryServiceHostProperties(requestContext, migrationEntry.HostId, ServiceHostFilterFlags.IncludeChildren);
        if (hostProperties == null)
        {
          if (migrationEntry.StorageOnly)
            HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (GetMigrationEntry), string.Format("Host {0} does not exist in host management because this is a blob-only request.", (object) migrationEntry.HostId));
          else
            HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (GetMigrationEntry), string.Format("Host {0} does not exist in host management.  The migration was likely corrupted during creation and it should be rolled back and deleted.", (object) migrationEntry.HostId));
          migrationEntry.HostProperties = new MigrationHostProperties()
          {
            Id = migrationEntry.HostId,
            HostType = migrationEntry.HostType
          };
        }
        else
        {
          migrationEntry.HostProperties = MigrationHostProperties.FromServiceHostProperties(hostProperties);
          if (!migrationEntry.HostProperties.IsVirtual)
          {
            try
            {
              if (!string.Equals(requestContext.GetService<ITeamFoundationDatabaseManagementService>().GetDatabase(requestContext, hostProperties.DatabaseId).PoolName, DatabaseManagementConstants.MigrationStagingPool, StringComparison.OrdinalIgnoreCase))
                migrationEntry.HostProperties.IsInReadOnlyMode = HostMigrationUtil.IsHostInReadOnlyMode(requestContext, hostProperties.Id);
            }
            catch (DatabaseNotFoundException ex)
            {
              HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (GetMigrationEntry), string.Format("Database {0} does not exist, skipping check for read-only mode", (object) hostProperties.DatabaseId));
              migrationEntry.HostProperties.IsInReadOnlyMode = false;
            }
          }
        }
        if (decryptSasTokens)
        {
          foreach (StorageMigration storageResource in migrationEntry.StorageResources)
          {
            HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (GetMigrationEntry), "Decrypting SAS token for container: " + storageResource.Id + ".");
            storageResource.SasToken = HostMigrationUtil.GetSasToken(requestContext, migrationEntry, storageResource, (Action<string>) (x => HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (GetMigrationEntry), x)));
          }
        }
        requestContext.Trace(15288038, TraceLevel.Info, TargetHostMigrationService.s_area, TargetHostMigrationService.s_layer, string.Format("Got migration entry on target.\n{0}", (object) migrationEntry));
        return migrationEntry;
      }
      finally
      {
        requestContext.TraceLeave(15288036, TargetHostMigrationService.s_area, TargetHostMigrationService.s_layer, nameof (GetMigrationEntry));
      }
    }

    private TargetHostMigration CheckCreateTargetMigrationJob(
      IVssRequestContext requestContext,
      Guid migrationId,
      TargetHostMigration migrationEntry)
    {
      requestContext.Trace(15288081, TraceLevel.Info, TargetHostMigrationService.s_area, TargetHostMigrationService.s_layer, string.Format("Checking for a background job that is creating the target migration entry.\nMigrationId: {0}", (object) migrationId));
      List<ResourceMigrationJob> source;
      using (HostMigrationComponent component = requestContext.CreateComponent<HostMigrationComponent>())
        source = component.QueryMigrationJobs(migrationId);
      List<ResourceMigrationJob> list = source.Where<ResourceMigrationJob>((Func<ResourceMigrationJob, bool>) (x => x.JobStage == MigrationJobStage.Target_CreateTargetMigration)).ToList<ResourceMigrationJob>();
      TargetHostMigration targetMigrationJob = (TargetHostMigration) null;
      if (migrationEntry == null)
      {
        targetMigrationJob = new TargetHostMigration();
        targetMigrationJob.HostId = Guid.Empty;
        targetMigrationJob.MigrationId = migrationId;
        targetMigrationJob.State = TargetMigrationState.Create;
        targetMigrationJob.HostProperties = new MigrationHostProperties()
        {
          HostCreated = true,
          IsVirtual = false,
          Status = TeamFoundationServiceHostStatus.Stopped,
          IsInReadOnlyMode = false
        };
      }
      else if (migrationEntry != null && list.Count > 0)
        targetMigrationJob = migrationEntry;
      foreach (ResourceMigrationJob resourceMigrationJob in list)
      {
        ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
        if (service.QueryJobQueue(requestContext, resourceMigrationJob.JobId) == null)
        {
          TeamFoundationJobHistoryEntry foundationJobHistoryEntry = service.QueryLatestJobHistory(requestContext, resourceMigrationJob.JobId);
          if (foundationJobHistoryEntry != null)
          {
            if (foundationJobHistoryEntry.Result != TeamFoundationJobResult.Succeeded)
            {
              requestContext.TraceAlways(6349085, TraceLevel.Info, TargetHostMigrationService.s_area, TargetHostMigrationService.s_layer, string.Format("Job {0} corresponding to migration {1} failed with {2} retries remaining: {3}", (object) resourceMigrationJob.JobId, (object) resourceMigrationJob.MigrationId, (object) resourceMigrationJob.RetriesRemaining, (object) foundationJobHistoryEntry.ToString()));
              if (resourceMigrationJob.RetriesRemaining > 0)
              {
                --resourceMigrationJob.RetriesRemaining;
                resourceMigrationJob.Status = ResourceMigrationState.Queued;
                service.QueueDelayedJobs(requestContext, (IEnumerable<Guid>) new Guid[1]
                {
                  resourceMigrationJob.JobId
                }, 3);
              }
              else
              {
                resourceMigrationJob.Status = ResourceMigrationState.Failed;
                if (targetMigrationJob.State == TargetMigrationState.Create)
                {
                  targetMigrationJob.State = TargetMigrationState.Failed;
                  targetMigrationJob.StatusMessage = foundationJobHistoryEntry.ResultMessage;
                }
                if (migrationEntry != null)
                {
                  using (HostMigrationComponent component = requestContext.CreateComponent<HostMigrationComponent>())
                    component.UpdateTargetMigration(targetMigrationJob);
                }
                else
                {
                  using (HostMigrationComponent component = requestContext.CreateComponent<HostMigrationComponent>())
                    targetMigrationJob = component.CreateTargetMigration(targetMigrationJob);
                }
              }
            }
            else
            {
              resourceMigrationJob.Status = ResourceMigrationState.Complete;
              using (HostMigrationComponent component = requestContext.CreateComponent<HostMigrationComponent>())
                targetMigrationJob = component.GetTargetMigration(migrationId);
            }
          }
          else
          {
            resourceMigrationJob.Status = ResourceMigrationState.Failed;
            targetMigrationJob.State = TargetMigrationState.Failed;
          }
        }
        using (HostMigrationComponent component = requestContext.CreateComponent<HostMigrationComponent>())
          component.UpdateResourceMigrationJob(resourceMigrationJob);
      }
      return targetMigrationJob;
    }

    private static TargetHostMigration CheckBackgroundMigrationJobs(
      IVssRequestContext requestContext,
      TargetHostMigration migrationEntry)
    {
      requestContext.TraceEnter(15288116, TargetHostMigrationService.s_area, TargetHostMigrationService.s_layer, nameof (CheckBackgroundMigrationJobs));
      try
      {
        TargetMigrationState? nextStageToAutoAdvance = new TargetMigrationState?();
        MigrationJobStage? expectedJobStage = new MigrationJobStage?();
        switch (migrationEntry.State)
        {
          case TargetMigrationState.BeginCompletePendingBlobs:
            expectedJobStage = new MigrationJobStage?(MigrationJobStage.Target_FinalizeMigrationOnTarget);
            nextStageToAutoAdvance = new TargetMigrationState?(TargetMigrationState.CompletePendingBlobs);
            break;
          case TargetMigrationState.BeginComplete:
            expectedJobStage = new MigrationJobStage?(MigrationJobStage.Target_CleanupMigrationOnTarget);
            nextStageToAutoAdvance = new TargetMigrationState?(TargetMigrationState.Complete);
            break;
          case TargetMigrationState.BeginRollback:
            expectedJobStage = new MigrationJobStage?(MigrationJobStage.Target_FinalizeMigrationOnTarget);
            nextStageToAutoAdvance = new TargetMigrationState?(TargetMigrationState.RolledBack);
            break;
        }
        requestContext.Trace(15288066, TraceLevel.Info, TargetHostMigrationService.s_area, TargetHostMigrationService.s_layer, string.Format("Checking background jobs for {0} from {1} to advance to {2}.\nMigrationId: {3}", (object) migrationEntry.State, (object) expectedJobStage, (object) nextStageToAutoAdvance, (object) migrationEntry.MigrationId));
        requestContext.GetService<IHostMigrationBackgroundJobService>().CheckBackgroundMigrationJobs(requestContext, migrationEntry.MigrationId, expectedJobStage, (Action<Guid, string>) ((migrationId, reason) =>
        {
          using (HostMigrationComponent component = requestContext.CreateComponent<HostMigrationComponent>())
            component.UpdateTargetMigration(migrationId, TargetMigrationState.Failed, reason);
        }), (Action<Guid>) (migrationId =>
        {
          if (!nextStageToAutoAdvance.HasValue)
            return;
          using (HostMigrationComponent component = requestContext.CreateComponent<HostMigrationComponent>())
            component.UpdateTargetMigration(migrationId, nextStageToAutoAdvance.Value, string.Empty);
        }));
        using (HostMigrationComponent component = requestContext.CreateComponent<HostMigrationComponent>())
          migrationEntry = component.GetTargetMigration(migrationEntry.MigrationId);
        return migrationEntry;
      }
      finally
      {
        requestContext.TraceLeave(15288117, TargetHostMigrationService.s_area, TargetHostMigrationService.s_layer, nameof (CheckBackgroundMigrationJobs));
      }
    }

    public TargetHostMigration StartCreateTargetMigrationJob(
      IVssRequestContext deploymentContext,
      TargetHostMigration migrationEntry)
    {
      deploymentContext.TraceEnter(15288082, TargetHostMigrationService.s_area, TargetHostMigrationService.s_layer, nameof (StartCreateTargetMigrationJob));
      try
      {
        HostMigrationLogger.LogInfo(deploymentContext, (IMigrationEntry) migrationEntry, nameof (StartCreateTargetMigrationJob), "Starting job to create the target migration");
        IHostMigrationBackgroundJobService service1 = deploymentContext.GetService<IHostMigrationBackgroundJobService>();
        TeamFoundationJobService service2 = deploymentContext.GetService<TeamFoundationJobService>();
        Guid jobId = Guid.Empty;
        try
        {
          XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) migrationEntry);
          jobId = service2.QueueOneTimeJob(deploymentContext, "Create Target Migration Job", "Microsoft.TeamFoundation.JobService.Extensions.Hosting.CreateTargetMigrationJob", xml, JobPriorityLevel.Normal);
          service1.RegisterResourceMigrationJob(deploymentContext, (IMigrationEntry) migrationEntry, jobId, "Create Target Migration Job", MigrationJobStage.Target_CreateTargetMigration, 5);
          migrationEntry.State = TargetMigrationState.Create;
        }
        catch (Exception ex)
        {
          if (jobId != Guid.Empty)
            service2.StopJob(deploymentContext, jobId);
          HostMigrationLogger.LogInfo(deploymentContext, (IMigrationEntry) migrationEntry, nameof (StartCreateTargetMigrationJob), "Error while queuing and registering the CreateTargetMigrationJob. Exception: " + ex.Message);
          migrationEntry.State = TargetMigrationState.Failed;
          throw;
        }
        HostMigrationLogger.LogInfo(deploymentContext, (IMigrationEntry) migrationEntry, nameof (StartCreateTargetMigrationJob), "Create target migration job submitted with job id: " + jobId.ToString());
      }
      finally
      {
        deploymentContext.TraceLeave(15288083, TargetHostMigrationService.s_area, TargetHostMigrationService.s_layer, nameof (StartCreateTargetMigrationJob));
      }
      return migrationEntry;
    }

    public void CreateTargetMigration(
      IVssRequestContext requestContext,
      TargetHostMigration migrationRequest)
    {
      requestContext.TraceEnter(15288039, TargetHostMigrationService.s_area, TargetHostMigrationService.s_layer, nameof (CreateTargetMigration));
      try
      {
        ArgumentUtility.CheckForNull<TargetHostMigration>(migrationRequest, nameof (migrationRequest));
        ArgumentUtility.CheckForNull<MigrationHostProperties>(migrationRequest.HostProperties, "migrationRequest.HostProperties");
        ArgumentUtility.CheckForNull<StorageMigration[]>(migrationRequest.StorageResources, "migrationRequest.StorageResources");
        HostMigrationUtil.CheckMigrationEnabled(requestContext, migrationRequest.StorageOnly);
        if (migrationRequest.OnlineBlobCopy)
          HostMigrationUtil.CheckOnlineBlobCopyEnabled(requestContext);
        if (!migrationRequest.StorageOnly && !migrationRequest.HostProperties.IsVirtual && migrationRequest.TargetDatabaseId == 0)
        {
          if (migrationRequest.ConnectionInfo == null)
            requestContext.Trace(15288018, TraceLevel.Error, TargetHostMigrationService.s_area, TargetHostMigrationService.s_layer, string.Format("Null-check failed for ConnectionInfo.\n{0}", (object) migrationRequest));
          ArgumentUtility.CheckForNull<SqlConnectionInfoWrapper>(migrationRequest.ConnectionInfo, "migrationRequest.ConnectionInfo");
        }
        if (migrationRequest.HostProperties.Id == requestContext.ServiceHost.DeploymentServiceHost.InstanceId)
          throw new ArgumentException(string.Format("Cannot migrate the deployment host. Host id provided in migration request matches deployment host id: {0}", (object) migrationRequest.HostProperties));
        HostMigrationLogger.CreateServicingJobLogInfo(requestContext, migrationRequest);
        TeamFoundationDatabaseManagementService service1 = requestContext.GetService<TeamFoundationDatabaseManagementService>();
        TeamFoundationHostManagementService service2 = requestContext.GetService<TeamFoundationHostManagementService>();
        bool flag1 = false;
        bool flag2 = false;
        TargetHostMigration migrationEntry = (TargetHostMigration) null;
        ITeamFoundationDatabaseProperties databaseProperties = (ITeamFoundationDatabaseProperties) null;
        CreateHostOptions createOptions = CreateHostOptions.None;
        TeamFoundationServiceHostProperties hostProperties = service2.QueryServiceHostProperties(requestContext, migrationRequest.HostId, ServiceHostFilterFlags.IncludeChildren);
        if (hostProperties != null && hostProperties.HostType == TeamFoundationHostType.Application)
        {
          TeamFoundationServiceHostProperties serviceHostProperties = hostProperties.Children.Where<TeamFoundationServiceHostProperties>((Func<TeamFoundationServiceHostProperties, bool>) (x => x.Status == TeamFoundationServiceHostStatus.Started)).FirstOrDefault<TeamFoundationServiceHostProperties>();
          if (serviceHostProperties != null)
          {
            string message = string.Format("Can't migrate Organization hosts that have one or more active children hosts in the target.  Found {0} {1}.  You can target the children", (object) serviceHostProperties.Name, (object) serviceHostProperties.Id);
            HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationRequest, nameof (CreateTargetMigration), message);
            throw new InvalidOperationException(message);
          }
        }
        string str = (string) null;
        TargetHostMigration targetHostMigration = (TargetHostMigration) null;
        using (HostMigrationComponent component = requestContext.CreateComponent<HostMigrationComponent>())
          targetHostMigration = component.GetLastPreMigration(migrationRequest.HostProperties.Id);
        int storageAccountId;
        if (targetHostMigration != null)
        {
          HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationRequest, nameof (CreateTargetMigration), "There was a pre-migration attempt.  Picking up the storage account ID.");
          if (targetHostMigration.State >= TargetMigrationState.Complete)
          {
            storageAccountId = targetHostMigration.StorageAccountId;
            migrationRequest.StorageAccountId = storageAccountId;
          }
          else
          {
            HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationRequest, nameof (CreateTargetMigration), "Pre-migration blob copying is still in progress.  Please wait until that is finished successfully before continueing with host migration.");
            throw new TeamFoundationServicingException(string.Format("Pre-migration blob copying is still in progress. Pre-migration status: {0}", (object) targetHostMigration.State));
          }
        }
        else
        {
          HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationRequest, nameof (CreateTargetMigration), "Selecting a storage account id for this new migration request.");
          if (hostProperties == null)
          {
            TeamFoundationServiceHostProperties serviceHostProperties = new TeamFoundationServiceHostProperties();
            serviceHostProperties.Id = migrationRequest.HostId;
            serviceHostProperties.HostType = TeamFoundationHostType.Unknown;
            hostProperties = serviceHostProperties;
            service2.CalculateStorageAccountId(requestContext, ref hostProperties);
          }
          storageAccountId = hostProperties.StorageAccountId;
          migrationRequest.StorageAccountId = storageAccountId;
          HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationRequest, nameof (CreateTargetMigration), string.Format("Storage account id: {0}", (object) storageAccountId));
        }
        if (!migrationRequest.StorageOnly)
        {
          HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationRequest, nameof (CreateTargetMigration), string.Format("Target database id was set to {0}", (object) migrationRequest.TargetDatabaseId));
          if (migrationRequest.TargetDatabaseId > 0)
          {
            ITeamFoundationDatabaseProperties database = service1.GetDatabase(requestContext, migrationRequest.TargetDatabaseId, true);
            if (database == null)
            {
              string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The target database with Id {0} does not exist.", (object) migrationRequest.TargetDatabaseId);
              HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationRequest, nameof (CreateTargetMigration), message);
              throw new ArgumentException(message);
            }
            TeamFoundationDatabasePool databasePool = service1.GetDatabasePool(requestContext, database.PoolName);
            if (databasePool.DatabaseType != TeamFoundationDatabaseType.Partition)
            {
              string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The target database with Id {0} in Pool {1} is not in a valid database pool. The target database must be in a Partition database pool.", (object) migrationRequest.TargetDatabaseId, (object) databasePool.PoolName);
              HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationRequest, nameof (CreateTargetMigration), message);
              throw new ArgumentException(message);
            }
          }
          else
            HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationRequest, nameof (CreateTargetMigration), "A target database was not provided, one will be automatically selected from available partitions in the DefaultPartitionPool.");
        }
        ISqlConnectionInfo sqlConnectionInfo = (ISqlConnectionInfo) null;
        if (!migrationRequest.StorageOnly && !migrationRequest.HostProperties.IsVirtual && migrationRequest.ConnectionInfo != null)
        {
          requestContext.Trace(15288041, TraceLevel.Info, TargetHostMigrationService.s_area, TargetHostMigrationService.s_layer, string.Format("Decrypting credentials.\n{0}", (object) migrationRequest));
          try
          {
            HostMigrationUtil.CheckForFaultInjection(requestContext, (IMigrationEntry) migrationRequest, "HostMigrationUtil", nameof (CreateTargetMigration), true);
            HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationRequest, nameof (CreateTargetMigration), "Getting the connection string for the source database.");
            sqlConnectionInfo = HostMigrationUtil.DecryptConnection(requestContext, migrationRequest);
            using (ExtendedAttributeComponent componentRaw = sqlConnectionInfo.CreateComponentRaw<ExtendedAttributeComponent>())
              str = componentRaw.ReadServiceLevelStamp();
          }
          catch (Exception ex)
          {
            HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationRequest, nameof (CreateTargetMigration), string.Format("Exception: {0}", (object) ex));
            throw new DataMigrationFailedToConnectToRemoteResourceException("Encounter an Exception while trying to access the Source Database " + sqlConnectionInfo?.DataSource + " " + sqlConnectionInfo?.InitialCatalog + ". Messages " + ex.Message, ex);
          }
        }
        try
        {
          HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationRequest, nameof (CreateTargetMigration), "Getting SAS tokens for containers in the migration request and storing them in strongbox.");
          HostMigrationUtil.SetSasTokens(requestContext, migrationRequest, ((IEnumerable<StorageMigration>) migrationRequest.StorageResources).ToList<StorageMigration>(), (Action<string>) (x => HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationRequest, "CreateTargetMigration.SetSasTokens", x)));
          HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationRequest, nameof (CreateTargetMigration), "Creating the target host migration entry");
          using (HostMigrationComponent component = requestContext.CreateComponent<HostMigrationComponent>())
            migrationEntry = component.CreateTargetMigration(migrationRequest);
          if (!migrationRequest.StorageOnly)
          {
            IHostSyncService service3 = requestContext.GetService<IHostSyncService>();
            if (!migrationRequest.HostProperties.IsVirtual)
            {
              if (migrationRequest.ConnectionInfo != null)
              {
                SqlConnectionInfoWrapper connectionInfoWrapper = new SqlConnectionInfoWrapper(requestContext, sqlConnectionInfo);
                HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationRequest, nameof (CreateTargetMigration), "Verifying service levels of source database is compatible with local database.");
                string serviceLevel;
                if (migrationRequest.TargetDatabaseId == DatabaseManagementConstants.InvalidDatabaseId)
                {
                  HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationRequest, nameof (CreateTargetMigration), "No target database was requested, using config db's service level for compat check.");
                  serviceLevel = requestContext.ServiceHost.DeploymentServiceHost.DatabaseProperties.ServiceLevel;
                }
                else
                {
                  HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationRequest, nameof (CreateTargetMigration), string.Format("Target database {0} was requested, using config db's service level for compat check.", (object) migrationRequest.TargetDatabaseId));
                  serviceLevel = service1.GetDatabase(requestContext, migrationRequest.TargetDatabaseId, true).ServiceLevel;
                }
                if (!string.Equals(str, serviceLevel, StringComparison.Ordinal))
                {
                  string message = "Cannot migrate when databases are at different servicing levels or during servicing upgrade.  Source database ServiceLevel: " + str + ".  Target deployment ServiceLevel: " + requestContext.ServiceHost.DeploymentServiceHost.DatabaseProperties.ServiceLevel;
                  HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationRequest, nameof (CreateTargetMigration), message);
                  throw new TeamFoundationServicingException(message);
                }
                HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationRequest, nameof (CreateTargetMigration), "Verifying execution permissions against source database.");
                using (DatabasePartitionComponent componentRaw = sqlConnectionInfo.CreateComponentRaw<DatabasePartitionComponent>())
                {
                  if (componentRaw.QueryPartition(migrationRequest.HostProperties.Id) == null)
                  {
                    HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationRequest, nameof (CreateTargetMigration), "Error: Unable to query for partition in the source database.");
                    throw new DatabasePartitionNotFoundException(migrationRequest.HostProperties.Id);
                  }
                }
                string connectionString = sqlConnectionInfo.ConnectionString;
                string databaseName = TeamFoundationDatabaseManagementService.ParseUniqueConnectionStringFields(connectionString) + ";" + migrationEntry.MigrationId.ToString("D");
                databaseProperties = this.GetBulkMigrationDatabase(requestContext, service1, connectionString);
                if (databaseProperties == null)
                {
                  HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationRequest, nameof (CreateTargetMigration), "Registering the source database in tbl_Database in the local instance.");
                  databaseProperties = service1.RegisterDatabase(requestContext.Elevate(), connectionString, databaseName, str, DatabaseManagementConstants.MigrationStagingPool, 0, 1, TeamFoundationDatabaseStatus.Servicing, new DateTime?(DateTime.UtcNow), "Mounting the foreign database", new DateTime?(), true, connectionInfoWrapper.UserId, Convert.FromBase64String(connectionInfoWrapper.PasswordEncrypted), TeamFoundationDatabaseFlags.None);
                  flag1 = true;
                }
                else
                {
                  TeamFoundationDatabaseCredential databaseCredential = service1.GetCredentialsForDatabase(requestContext, databaseProperties.DatabaseId, DatabaseCredentialNames.DefaultCredential).SingleOrDefault<TeamFoundationDatabaseCredential>();
                  if (databaseCredential == null)
                    throw new TeamFoundationServicingException("Could not find credential for bulk migration DB " + databaseProperties.DatabaseName);
                  if (databaseCredential.UserId != connectionInfoWrapper.UserId)
                    throw new TeamFoundationServicingException("Bulk migration DB was registered, but the credential in the migration request does not match.  Registered user id: " + connectionInfoWrapper.UserId + ".  Id from migration request: " + databaseCredential.UserId);
                  HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationRequest, nameof (CreateTargetMigration), "Found a pre-registered bulk migration database.  Will use that for the migration.");
                }
                migrationEntry.SourceDatabaseId = databaseProperties.DatabaseId;
                HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationRequest, nameof (CreateTargetMigration), string.Format("Registered the source database.  DatabaseId = {0}", (object) databaseProperties.DatabaseId));
              }
              else
              {
                requestContext.Trace(15288042, TraceLevel.Info, TargetHostMigrationService.s_area, TargetHostMigrationService.s_layer, string.Format("No connection info provided.\n{0}", (object) migrationRequest));
                HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationRequest, nameof (CreateTargetMigration), "No connection info was provided and a target database was provided.  Assuming this is a migration with partition copy disabled (sql failover approach).");
                if (migrationRequest.TargetDatabaseId == 0)
                {
                  string message = "No source database connection info was provided. This signifies a migration with sql failover instead of partition copy, however no target database was provided.";
                  HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationRequest, nameof (CreateTargetMigration), message);
                  throw new TeamFoundationServicingException(message);
                }
                migrationEntry.SourceDatabaseId = migrationRequest.TargetDatabaseId;
                databaseProperties = service1.GetDatabase(requestContext, migrationRequest.TargetDatabaseId, true);
                sqlConnectionInfo = databaseProperties.DboConnectionInfo;
                HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationRequest, nameof (CreateTargetMigration), string.Format("Setting the source database to the target database id: {0} (assuming sql failover mode).", (object) migrationRequest.TargetDatabaseId));
              }
              migrationEntry.TargetDatabaseId = migrationRequest.TargetDatabaseId;
              using (HostMigrationComponent component = requestContext.CreateComponent<HostMigrationComponent>())
                component.UpdateTargetMigration(migrationEntry);
              requestContext.GetService<IVssRegistryService>().SetValue<string>(requestContext, string.Format(FrameworkServerConstants.DisableDatabaseDownsizeDuringMigrationsUntil, (object) migrationRequest.TargetDatabaseId), DateTime.UtcNow.AddHours(24.0).ToString("yyyy-MM-ddTHH:mm:ss"));
            }
            HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationRequest, nameof (CreateTargetMigration), string.Format("Registering the service host {0}", (object) migrationEntry.HostId));
            hostProperties = migrationRequest.HostProperties.ToServiceHostProperties(requestContext);
            if (migrationRequest.Options.HasFlag((Enum) HostMigrationOptions.LiveHost))
              hostProperties.SubStatus = ServiceHostSubStatus.Idle;
            if (migrationRequest.HostProperties.IsVirtual)
            {
              hostProperties.DatabaseId = -2;
              hostProperties.StorageAccountId = -2;
            }
            else
            {
              hostProperties.DatabaseId = databaseProperties.DatabaseId;
              hostProperties.StorageAccountId = storageAccountId;
            }
            HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationRequest, nameof (CreateTargetMigration), string.Format("HostType: {0}. IsVirtualServiceHost: {1}", (object) hostProperties.HostType, (object) hostProperties.IsVirtualServiceHost()));
            if (hostProperties.HostType == TeamFoundationHostType.ProjectCollection && service2.QueryServiceHostProperties(requestContext, hostProperties.ParentId) == null)
            {
              if (((TeamFoundationHostType) (!(requestContext.ServiceInstanceType() == ServiceInstanceTypes.SPS) ? (int) requestContext.GetService<IInstanceManagementService>().GetServiceInstance(requestContext, requestContext.ServiceHost.InstanceId).GetProperty<TeamFoundationHostType>("Microsoft.TeamFoundation.Location.PhysicalHostTypesSupported", TeamFoundationHostType.All) : 7)).HasFlag((Enum) TeamFoundationHostType.Application))
              {
                string message = "Error: This Service does not support virtual hosts, and a migration was queued only at the ProjectCollection level.Please queue the migration request starting with the Organization host.";
                HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationRequest, nameof (CreateTargetMigration), message);
                throw new InvalidOperationException(message);
              }
              if (!service3.EnsureHostUpdated(requestContext, hostProperties.ParentId))
              {
                HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationRequest, nameof (CreateTargetMigration), "Error: The parent virtual host was not found locally and could not be created.");
                throw new HostDoesNotExistException(hostProperties.ParentId);
              }
            }
            if (hostProperties.HostType == TeamFoundationHostType.Application && hostProperties.IsVirtualServiceHost())
            {
              service3.EnsureHostUpdated(requestContext, hostProperties.Id);
            }
            else
            {
              service2.CreateServiceHost(requestContext, hostProperties, sqlConnectionInfo, createOptions);
              if (migrationRequest.Options.HasFlag((Enum) HostMigrationOptions.LiveHost) && !migrationEntry.StorageOnly)
              {
                requestContext.GetService<IVssRegistryService>().SetValue<bool>(requestContext, FrameworkServerConstants.Migration_TagNewAzureBlobProviderBaseBlobsPath + "/" + hostProperties.Id.ToString(), true);
                if (HostMigrationUtil.GetTestRegistryFlag(requestContext, BlobCopyUtil.s_testCreateFileServiceTaggedFilesOnTargetHost, (Action<string>) (message => HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationRequest, nameof (CreateTargetMigration), message))))
                {
                  HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationRequest, nameof (CreateTargetMigration), "Creating tagged blobs in the file service for the target servicehost for reverse blob migration test.");
                  IVssRequestContext context = service2.BeginRequest(requestContext, hostProperties.Id, RequestContextType.ServicingContext, true, false);
                  for (int index = 0; index < 10; ++index)
                  {
                    TeamFoundationFileService service4 = context.GetService<TeamFoundationFileService>();
                    byte[] bytes = Encoding.ASCII.GetBytes("This is a file! File number: " + index.ToString());
                    IVssRequestContext requestContext1 = context;
                    byte[] content = bytes;
                    service4.UploadFile(requestContext1, content);
                  }
                  context.Dispose();
                }
              }
            }
            HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationRequest, nameof (CreateTargetMigration), "Service host has been registered. Host properties: " + hostProperties.ToString());
            flag2 = true;
          }
          migrationEntry.State = TargetMigrationState.Created;
          using (HostMigrationComponent component = requestContext.CreateComponent<HostMigrationComponent>())
            component.UpdateTargetMigration(migrationEntry);
        }
        catch (Exception ex1)
        {
          HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationRequest, nameof (CreateTargetMigration), string.Format("Exception: {0}", (object) ex1));
          requestContext.TraceException(0, TargetHostMigrationService.s_area, TargetHostMigrationService.s_layer, ex1);
          if (flag2)
          {
            if (hostProperties != null)
            {
              try
              {
                service2.DeleteServiceHost(requestContext, hostProperties.Id, HostDeletionReason.HostMigrated, DeleteHostResourceOptions.None);
              }
              catch (Exception ex2)
              {
                requestContext.TraceException(0, TargetHostMigrationService.s_area, TargetHostMigrationService.s_layer, ex2);
              }
            }
          }
          if (flag1)
          {
            if (databaseProperties != null)
            {
              try
              {
                service1.RemoveDatabase(requestContext.Elevate(), databaseProperties.DatabaseId);
              }
              catch (Exception ex3)
              {
                requestContext.TraceException(0, TargetHostMigrationService.s_area, TargetHostMigrationService.s_layer, ex3);
              }
            }
          }
          throw;
        }
      }
      finally
      {
        requestContext.TraceLeave(15288040, TargetHostMigrationService.s_area, TargetHostMigrationService.s_layer, nameof (CreateTargetMigration));
      }
    }

    public void ResourceMigrationJobCompleted(
      IVssRequestContext requestContext,
      TargetHostMigration migration,
      Guid jobId)
    {
      if (!migration.StorageOnly)
        return;
      List<ResourceMigrationJob> resourceMigrationJobList;
      using (HostMigrationComponent component = requestContext.CreateComponent<HostMigrationComponent>())
        resourceMigrationJobList = component.QueryResourceMigrationJobs(migration.MigrationId);
      if (resourceMigrationJobList.All<ResourceMigrationJob>((Func<ResourceMigrationJob, bool>) (j => j.Status == ResourceMigrationState.Complete)))
      {
        this.CleanupSasTokens(requestContext, migration);
        using (HostMigrationComponent component = requestContext.CreateComponent<HostMigrationComponent>())
          component.UpdateTargetMigration(migration.MigrationId, TargetMigrationState.Complete, "StorageOnly migration is completed.");
        HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migration, nameof (ResourceMigrationJobCompleted), string.Format("Marked Migration Job ({0}) {1}.", (object) jobId, (object) TargetMigrationState.Complete));
      }
      else if (resourceMigrationJobList.Any<ResourceMigrationJob>((Func<ResourceMigrationJob, bool>) (j => j.Status == ResourceMigrationState.Failed)))
      {
        using (HostMigrationComponent component = requestContext.CreateComponent<HostMigrationComponent>())
          component.UpdateTargetMigration(migration.MigrationId, TargetMigrationState.Failed, "StorageOnly migration has failed. One or more resource migration jobs failed.");
        HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migration, nameof (ResourceMigrationJobCompleted), string.Format("Marked Migration Job ({0}) {1}. {2}", (object) jobId, (object) TargetMigrationState.Failed, (object) string.Join<ResourceMigrationJob>(", ", (IEnumerable<ResourceMigrationJob>) resourceMigrationJobList)));
      }
      else
        HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migration, nameof (ResourceMigrationJobCompleted), string.Format("Not Marking Migration Job ({0}). {1}", (object) jobId, (object) string.Join<ResourceMigrationJob>(", ", (IEnumerable<ResourceMigrationJob>) resourceMigrationJobList)));
    }

    public TargetHostMigration StartCopyJobs(
      IVssRequestContext requestContext,
      TargetHostMigration migrationEntry)
    {
      if (migrationEntry.StorageOnly)
      {
        if (TargetHostMigrationService.CheckForDedicatedBlobJobOnStorageMove(requestContext))
        {
          HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (StartCopyJobs), "Creating dedicated blob migration job for storage move");
          this.QueueBlobCopy(requestContext, migrationEntry);
        }
        else
          this.QueueBlobPreMigration(requestContext, migrationEntry);
      }
      else if (!migrationEntry.HostProperties.IsVirtual)
      {
        this.QueueDatabaseMigration(requestContext, migrationEntry);
        this.QueueBlobCopy(requestContext, migrationEntry);
        if (migrationEntry.Options.HasFlag((Enum) HostMigrationOptions.MigrateAdHocJobs))
          this.QueueAdHocJobCopy(requestContext, migrationEntry);
      }
      using (HostMigrationComponent component = requestContext.CreateComponent<HostMigrationComponent>())
      {
        migrationEntry.State = TargetMigrationState.BeginCopyJobs;
        component.UpdateTargetMigration(migrationEntry);
      }
      using (IDisposableReadOnlyList<IHostMigrationResourceJobExtension> extensions = requestContext.GetExtensions<IHostMigrationResourceJobExtension>())
      {
        foreach (IHostMigrationResourceJobExtension resourceJobExtension in (IEnumerable<IHostMigrationResourceJobExtension>) extensions)
        {
          HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (StartCopyJobs), "Loading extension: " + resourceJobExtension.GetType().FullName);
          ResourceMigrationJob migrationJobInfo = resourceJobExtension.GetMigrationJobInfo(requestContext, migrationEntry);
          if (migrationJobInfo != null)
          {
            HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (StartCopyJobs), string.Format("Registering a ResourceMigrationJob.  JobId: {0}.  Name: {1}", (object) migrationJobInfo.JobId, (object) migrationJobInfo.Name));
            using (HostMigrationComponent component = requestContext.CreateComponent<HostMigrationComponent>())
              component.CreateResourceMigrationJob(migrationJobInfo);
            HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (StartCopyJobs), string.Format("Queueing a ResourceMigrationJob.  JobId: {0}.  Name: {1}", (object) migrationJobInfo.JobId, (object) migrationJobInfo.Name));
            resourceJobExtension.QueueMigrationJob(requestContext, migrationEntry);
          }
        }
      }
      return migrationEntry;
    }

    public void QueueBlobPreMigration(
      IVssRequestContext requestContext,
      TargetHostMigration migrationEntry)
    {
      IHostMigrationBackgroundJobService service1 = requestContext.GetService<IHostMigrationBackgroundJobService>();
      TeamFoundationJobService service2 = requestContext.GetService<TeamFoundationJobService>();
      if (requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, (RegistryQuery) FrameworkServerConstants.UseTfsBlobMigrationStorageOnly, false))
      {
        service1.RegisterResourceMigrationJob(requestContext, (IMigrationEntry) migrationEntry, Guid.Parse("8EE44A94-905D-4165-9E71-75BF28AF4521"), "Parallel Blob Container Coordinator Pre-Migration Copy Job", MigrationJobStage.Target_Resources, 0);
        service2.QueueDelayedJobs(requestContext, (IEnumerable<Guid>) new Guid[1]
        {
          Guid.Parse("8EE44A94-905D-4165-9E71-75BF28AF4521")
        }, 3);
      }
      else
      {
        int levelParallelism = this.GetJobLevelParallelism(requestContext, migrationEntry);
        if (levelParallelism <= 1)
        {
          service1.RegisterResourceMigrationJob(requestContext, (IMigrationEntry) migrationEntry, Guid.Parse(TargetHostMigrationService.s_blobMigrationJobId), TargetHostMigrationService.s_blobContainerPreMigrationJobName, MigrationJobStage.Target_Resources, 5);
          service2.QueueDelayedJobs(requestContext, (IEnumerable<Guid>) new Guid[1]
          {
            Guid.Parse(TargetHostMigrationService.s_blobMigrationJobId)
          }, 3);
        }
        else
        {
          Guid jobId = Guid.NewGuid();
          this.QueueCoordinatingJob(requestContext, migrationEntry, jobId, TargetHostMigrationService.s_blobContainerPreMigrationCoordinatingJobName, levelParallelism, service1, service2, true);
        }
      }
    }

    private void QueueCoordinatingJob(
      IVssRequestContext requestContext,
      TargetHostMigration migrationEntry,
      Guid jobId,
      string jobName,
      int parallelJobs,
      IHostMigrationBackgroundJobService migJobService,
      TeamFoundationJobService jobService,
      bool register)
    {
      if (register)
        migJobService.RegisterResourceMigrationJob(requestContext, (IMigrationEntry) migrationEntry, jobId, jobName, MigrationJobStage.Target_Resources, 0);
      ParallelBlobMigrationCoordinatingJobData coordinatingJobData1 = new ParallelBlobMigrationCoordinatingJobData();
      coordinatingJobData1.StorageType = StorageType.Blob;
      coordinatingJobData1.TotalGroups = parallelJobs;
      ParallelBlobMigrationCoordinatingJobData coordinatingJobData2 = coordinatingJobData1;
      coordinatingJobData2.PopulateFromMigration(migrationEntry);
      ParallelMigrationUtil.QueueCustomizedMigrationJob<ParallelBlobMigrationCoordinatingJobData>((ITeamFoundationJobService) jobService, requestContext, "Microsoft.TeamFoundation.JobService.Extensions.Hosting.ParallelBlobMigrationCoordinatingJob", jobId, TargetHostMigrationService.s_blobContainerPreMigrationCoordinatingJobName, coordinatingJobData2, TimeSpan.FromSeconds(3.0));
    }

    private int GetJobLevelParallelism(
      IVssRequestContext requestContext,
      TargetHostMigration migrationEntry)
    {
      Guid hostId = migrationEntry.HostId;
      int levelParallelism = 1;
      if (migrationEntry.HostType == TeamFoundationHostType.ProjectCollection)
      {
        levelParallelism = requestContext.GetPerHostRegistry<int>(hostId, FrameworkServerConstants.ParallelMigrationRoot, FrameworkServerConstants.ParallelMigration_TotalStorageAccountGroups, false, 1);
        bool perHostRegistry = requestContext.GetPerHostRegistry<bool>(hostId, FrameworkServerConstants.ParallelMigrationRoot, FrameworkServerConstants.ParallelMigration_EnableBlobMigrationParallelism, false, false);
        if (levelParallelism < 1 || !perHostRegistry)
          levelParallelism = 1;
      }
      return levelParallelism;
    }

    public void UpdateTargetMigration(
      IVssRequestContext requestContext,
      TargetHostMigration migrationEntry)
    {
      using (HostMigrationComponent component = requestContext.CreateComponent<HostMigrationComponent>())
        component.UpdateTargetMigration(migrationEntry);
    }

    public TargetHostMigration QueueCleanupMigrationOnTarget(
      IVssRequestContext requestContext,
      TargetHostMigration migrationEntry)
    {
      HostMigrationLogger.CreateServicingJobLogInfo(requestContext, migrationEntry);
      HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (QueueCleanupMigrationOnTarget), "Cleanup Migration.");
      migrationEntry.State = TargetMigrationState.BeginComplete;
      HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (QueueCleanupMigrationOnTarget), string.Format("Setting migration status to {0}.  Message: {1}", (object) migrationEntry.State, (object) migrationEntry.StatusMessage));
      this.UpdateTargetMigration(requestContext, migrationEntry);
      requestContext.GetService<IHostMigrationBackgroundJobService>().QueueBackgroundMigrationJob(requestContext, new string[1]
      {
        ServicingOperationConstants.CleanupMigrationOnTarget
      }, (IMigrationEntry) migrationEntry, false, MigrationJobStage.Target_CleanupMigrationOnTarget);
      return migrationEntry;
    }

    public TargetHostMigration QueueFinalizeMigrationOnTarget(
      IVssRequestContext requestContext,
      TargetHostMigration migrationEntry,
      bool rollback = false)
    {
      HostMigrationLogger.CreateServicingJobLogInfo(requestContext, migrationEntry);
      HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (QueueFinalizeMigrationOnTarget), "Finalizing Migration.");
      migrationEntry.State = rollback ? TargetMigrationState.BeginRollback : TargetMigrationState.BeginCompletePendingBlobs;
      HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (QueueFinalizeMigrationOnTarget), string.Format("Setting migration status to {0}.  Message: {1}", (object) migrationEntry.State, (object) migrationEntry.StatusMessage));
      this.UpdateTargetMigration(requestContext, migrationEntry);
      requestContext.GetService<IHostMigrationBackgroundJobService>().QueueBackgroundMigrationJob(requestContext, new string[1]
      {
        ServicingOperationConstants.FinalizeMigrationOnTarget
      }, (IMigrationEntry) migrationEntry, (rollback ? 1 : 0) != 0, MigrationJobStage.Target_FinalizeMigrationOnTarget);
      return migrationEntry;
    }

    public TargetHostMigration ResumeBlobCopyOnTarget(
      IVssRequestContext requestContext,
      TargetHostMigration migrationEntry)
    {
      requestContext.TraceEnter(15288118, TargetHostMigrationService.s_area, TargetHostMigrationService.s_layer, nameof (ResumeBlobCopyOnTarget));
      try
      {
        HostMigrationLogger.CreateServicingJobLogInfo(requestContext, migrationEntry);
        HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (ResumeBlobCopyOnTarget), "Resuming any failed or cancelled blob resource migration jobs.");
        if (!migrationEntry.StorageOnly && !migrationEntry.HostProperties.IsVirtual)
        {
          foreach (StorageMigration blobContainer in migrationEntry.GetBlobContainers())
          {
            if (blobContainer.Status == StorageMigrationStatus.Failed)
            {
              HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (ResumeBlobCopyOnTarget), string.Format("Resetting status of failed blob container.  Id={0}, MigrationId={1}", (object) blobContainer.Id, (object) blobContainer.MigrationId));
              using (HostMigrationComponent component = requestContext.CreateComponent<HostMigrationComponent>())
                component.UpdateContainerMigrationStatus(blobContainer, StorageMigrationStatus.Created, "Resetting status of failed blob container.");
            }
          }
          TeamFoundationJobService service = requestContext.GetService<TeamFoundationJobService>();
          List<ResourceMigrationJob> resourceMigrationJobList;
          using (HostMigrationComponent component = requestContext.CreateComponent<HostMigrationComponent>())
            resourceMigrationJobList = component.QueryResourceMigrationJobs(migrationEntry.MigrationId);
          foreach (ResourceMigrationJob resourceMigrationJob in resourceMigrationJobList)
          {
            HostMigrationLogger.LogInfo(requestContext, resourceMigrationJob.MigrationId, DateTime.UtcNow, nameof (ResumeBlobCopyOnTarget), string.Format("Found job. MigrationId={0}; JobName={1}; Status={2}", (object) resourceMigrationJob.MigrationId, (object) resourceMigrationJob.Name, (object) resourceMigrationJob.Status));
            if ((resourceMigrationJob.Status == ResourceMigrationState.Failed || resourceMigrationJob.Status == ResourceMigrationState.Canceled) && string.Equals(resourceMigrationJob.Name, TargetHostMigrationService.s_blobContainerCopyJobName, StringComparison.OrdinalIgnoreCase))
            {
              HostMigrationLogger.LogInfo(requestContext, resourceMigrationJob.MigrationId, DateTime.UtcNow, nameof (ResumeBlobCopyOnTarget), string.Format("Requeueing job. MigrationId={0}; JobName={1}; Status={2}", (object) resourceMigrationJob.MigrationId, (object) resourceMigrationJob.Name, (object) resourceMigrationJob.Status));
              service.QueueDelayedJobs(requestContext, (IEnumerable<Guid>) new Guid[1]
              {
                resourceMigrationJob.JobId
              }, 3);
              resourceMigrationJob.Status = ResourceMigrationState.Queued;
              resourceMigrationJob.RetriesRemaining = 5;
              using (HostMigrationComponent component = requestContext.CreateComponent<HostMigrationComponent>())
                component.UpdateResourceMigrationJob(resourceMigrationJob);
            }
          }
          migrationEntry.State = TargetMigrationState.CompletePendingBlobs;
          migrationEntry.StatusMessage = "Resuming any failed or cancelled blob resource migration jobs.";
          HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (ResumeBlobCopyOnTarget), string.Format("Setting migration status to {0}.  Message: {1}", (object) migrationEntry.State, (object) migrationEntry.StatusMessage));
          this.UpdateTargetMigration(requestContext, migrationEntry);
        }
        else
          HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (ResumeBlobCopyOnTarget), string.Format("Unexpected state! Unable to resume blob copy on a storage-only migration or a virtual host. MigrationId={0}", (object) migrationEntry.MigrationId), ServicingStepLogEntryKind.Warning);
        return migrationEntry;
      }
      finally
      {
        requestContext.TraceLeave(15288119, TargetHostMigrationService.s_area, TargetHostMigrationService.s_layer, nameof (ResumeBlobCopyOnTarget));
      }
    }

    private void QueueDatabaseMigration(
      IVssRequestContext requestContext,
      TargetHostMigration migrationEntry)
    {
      if (migrationEntry.SourceDatabaseId == migrationEntry.TargetDatabaseId)
      {
        HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (QueueDatabaseMigration), string.Format("Source and target database are the same value, this disables the partition copy job from running. Database id: {0}", (object) migrationEntry.SourceDatabaseId));
      }
      else
      {
        int num = HostMigrationInjectionUtil.CheckInjection(requestContext, FrameworkServerConstants.DisableBlobCopyDuringMigrations) ? 1 : 0;
        List<string> values = new List<string>();
        values.Add("dbo.tbl_ProcessTemplateUsage");
        values.Add("dbo.WorkItemLinksDestroyed");
        if (num != 0)
        {
          values.Add("dbo.tbl_FileReference");
          values.Add("dbo.tbl_FileMetadata");
          values.Add("dbo.tbl_FileContentValidationMetadata");
          values.Add("dbo.tbl_FileContentValidationWatermark");
          values.Add("dbo.tbl_Content");
          values.Add("dbo.tbl_FilePendingUpload");
          values.Add("dbo.tbl_PendingDelta");
          values.Add("dbo.tbl_ChangedFiles");
        }
        Guid jobId = Guid.NewGuid();
        requestContext.GetService<IHostMigrationBackgroundJobService>().RegisterResourceMigrationJob(requestContext, (IMigrationEntry) migrationEntry, jobId, TargetHostMigrationService.s_databasePartitionCopyJobName, MigrationJobStage.Target_Resources, 5);
        string[] strArray;
        string invariant;
        if (migrationEntry.HostProperties.HostType == TeamFoundationHostType.ProjectCollection)
        {
          strArray = new string[1]
          {
            ServicingOperationConstants.MigrateCollectionTargetDatabase
          };
          invariant = TargetHostMigrationService.ToInvariant(FormattableStringFactory.Create("Migrate collection partition for host {0} on target.", (object) migrationEntry.HostProperties.Id));
        }
        else
        {
          strArray = new string[1]
          {
            ServicingOperationConstants.MigrateAccountTargetDatabase
          };
          invariant = TargetHostMigrationService.ToInvariant(FormattableStringFactory.Create("Migrate account partition for host {0} on target.", (object) migrationEntry.HostProperties.Id));
        }
        ServicingJobData servicingJobData1 = new ServicingJobData(strArray);
        servicingJobData1.ServicingHostId = migrationEntry.HostProperties.Id;
        servicingJobData1.OperationClass = "MigrateAccount";
        servicingJobData1.JobTitle = invariant;
        servicingJobData1.ServicingOptions = ServicingFlags.RequiresStoppedHost;
        servicingJobData1.ServicingLocks = new TeamFoundationLockInfo[1]
        {
          new TeamFoundationLockInfo()
          {
            LockMode = TeamFoundationLockMode.Shared,
            LockName = "Servicing-" + migrationEntry.HostProperties.Id.ToString(),
            LockTimeout = -1
          }
        };
        ServicingJobData servicingJobData2 = servicingJobData1;
        MoveHostOptions moveHostOptions = MoveHostOptions.IgnoreDatabaseType | MoveHostOptions.CopyCommonTablesOnly | MoveHostOptions.SourceReadOnly;
        if (migrationEntry.PerformDatabaseCopyValidation)
          moveHostOptions |= MoveHostOptions.PerformValidation;
        SqlConnectionInfoWrapper connectionInfoWrapper = this.GetConnectionInfoWrapper(requestContext, migrationEntry.SourceDatabaseId);
        servicingJobData2.ServicingItems[ServicingItemConstants.ConnectionInfo] = (object) connectionInfoWrapper;
        servicingJobData2.ServicingTokens.Add(ServicingTokenConstants.SourceDatabaseId, migrationEntry.SourceDatabaseId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        servicingJobData2.ServicingTokens.Add(ServicingTokenConstants.SourceHostId, migrationEntry.HostProperties.Id.ToString("D"));
        IDictionary<string, string> servicingTokens1 = servicingJobData2.ServicingTokens;
        string instanceId = ServicingTokenConstants.InstanceId;
        Guid guid = migrationEntry.HostProperties.Id;
        string str1 = guid.ToString("D");
        servicingTokens1.Add(instanceId, str1);
        servicingJobData2.ServicingTokens.Add(ServicingTokenConstants.TableFilter, string.Join(";", (IEnumerable<string>) values));
        servicingJobData2.ServicingTokens.Add(ServicingTokenConstants.MoveHostOptions, ((int) moveHostOptions).ToString());
        IDictionary<string, string> servicingTokens2 = servicingJobData2.ServicingTokens;
        string migrationId = ServicingTokenConstants.MigrationId;
        guid = migrationEntry.MigrationId;
        string str2 = guid.ToString("D");
        servicingTokens2.Add(migrationId, str2);
        requestContext.GetService<TeamFoundationServicingService>().QueueServicingJob(requestContext, servicingJobData2, JobPriorityClass.AboveNormal, JobPriorityLevel.Normal, new Guid?(jobId));
      }
    }

    private bool IsSynchronousResourceMigrationJob(
      ResourceMigrationJob migrationJob,
      bool storageOnly,
      bool onlineBlobCopy)
    {
      return storageOnly || !onlineBlobCopy || !string.Equals(migrationJob.Name, TargetHostMigrationService.s_blobContainerCopyJobName, StringComparison.OrdinalIgnoreCase);
    }

    private void CheckResourceMigrationJobs(
      IVssRequestContext requestContext,
      Guid migrationId,
      bool storageOnly,
      bool onlineBlobCopy)
    {
      requestContext.TraceEnter(15288120, TargetHostMigrationService.s_area, TargetHostMigrationService.s_layer, nameof (CheckResourceMigrationJobs));
      try
      {
        TeamFoundationJobService service = requestContext.GetService<TeamFoundationJobService>();
        List<ResourceMigrationJob> collection = FetchResourceJobsForMigration();
        Dictionary<Guid, TeamFoundationJobQueueEntry> dictionary = new Dictionary<Guid, TeamFoundationJobQueueEntry>();
        foreach (ResourceMigrationJob resourceMigrationJob in collection)
        {
          TeamFoundationJobQueueEntry foundationJobQueueEntry = service.QueryJobQueue(requestContext, resourceMigrationJob.JobId);
          dictionary.Add(resourceMigrationJob.JobId, foundationJobQueueEntry);
        }
        List<ResourceMigrationJob> resourceMigrationJobList1 = FetchResourceJobsForMigration();
        int num1 = 0;
        int num2 = 0;
        List<ResourceMigrationJob> resourceMigrationJobList2 = new List<ResourceMigrationJob>();
        ISet<ResourceMigrationJob> resourceMigrationJobSet = (ISet<ResourceMigrationJob>) new HashSet<ResourceMigrationJob>((IEnumerable<ResourceMigrationJob>) collection);
        foreach (ResourceMigrationJob resourceMigrationJob in resourceMigrationJobList1)
        {
          if (resourceMigrationJobSet.Contains(resourceMigrationJob))
            resourceMigrationJobList2.Add(resourceMigrationJob);
          else
            ++num2;
        }
        List<ResourceMigrationJob> resourceMigrationJobList3 = resourceMigrationJobList2;
        bool testRegistryFlag = HostMigrationUtil.GetTestRegistryFlag(requestContext, BlobCopyUtil.s_testFailingBlobCopy, (Action<string>) (x => HostMigrationLogger.LogInfo(requestContext, migrationId, DateTime.UtcNow, nameof (CheckResourceMigrationJobs), x)));
        string message = (string) null;
        foreach (ResourceMigrationJob resourceMigrationJob in resourceMigrationJobList3)
        {
          TeamFoundationJobQueueEntry foundationJobQueueEntry = dictionary[resourceMigrationJob.JobId];
          HostMigrationLogger.LogInfo(requestContext, migrationId, DateTime.UtcNow, nameof (CheckResourceMigrationJobs), string.Format("Checking Resource Migration Job.  MigrationId={0}; JobName={1}; JobId={2} Status={3}", (object) migrationId, (object) resourceMigrationJob.Name, (object) resourceMigrationJob.JobId, (object) resourceMigrationJob.Status));
          if (resourceMigrationJob.Status != ResourceMigrationState.Complete)
          {
            if (resourceMigrationJob.Status == ResourceMigrationState.Canceled)
              ++num1;
            else if (foundationJobQueueEntry == null)
            {
              if (resourceMigrationJob.RetriesRemaining > 0 && !testRegistryFlag)
              {
                HostMigrationLogger.LogInfo(requestContext, migrationId, DateTime.UtcNow, nameof (CheckResourceMigrationJobs), string.Format("Requeueing job.  MigrationId={0}; JobName={1}; Status={2}", (object) migrationId, (object) resourceMigrationJob.Name, (object) resourceMigrationJob.Status));
                service.QueueDelayedJobs(requestContext, (IEnumerable<Guid>) new Guid[1]
                {
                  resourceMigrationJob.JobId
                }, 3);
                using (HostMigrationComponent component = requestContext.CreateComponent<HostMigrationComponent>())
                {
                  resourceMigrationJob.Status = ResourceMigrationState.Queued;
                  --resourceMigrationJob.RetriesRemaining;
                  component.UpdateResourceMigrationJob(resourceMigrationJob);
                }
                if (this.IsSynchronousResourceMigrationJob(resourceMigrationJob, storageOnly, onlineBlobCopy))
                  ++num2;
              }
              else
              {
                ++num1;
                message = !testRegistryFlag ? message + string.Format("Resource Migration Job has exhausted retries. JobId={0}. ", (object) resourceMigrationJob.JobId) : message + string.Format("Resource Migration Job intentionally failed to test online blob copy. JobId={0}. ", (object) resourceMigrationJob.JobId);
                using (HostMigrationComponent component = requestContext.CreateComponent<HostMigrationComponent>())
                {
                  resourceMigrationJob.Status = ResourceMigrationState.Failed;
                  component.UpdateResourceMigrationJob(resourceMigrationJob);
                }
              }
            }
            else if (this.IsSynchronousResourceMigrationJob(resourceMigrationJob, storageOnly, onlineBlobCopy))
              ++num2;
          }
        }
        HostMigrationLogger.LogInfo(requestContext, migrationId, DateTime.UtcNow, nameof (CheckResourceMigrationJobs), string.Format("Checking Resource Migration Jobs. Failed={0}; InProgress={1}", (object) num1, (object) num2));
        if (num1 > 0)
        {
          this.UpdateMigrationStateIfNotCopyJobsComplete(requestContext, migrationId, TargetMigrationState.Failed, message);
        }
        else
        {
          if (num2 != 0)
            return;
          if (TargetHostMigrationService.CheckForDedicatedBlobJobOnStorageMove(requestContext))
            this.UpdateMigrationStateIfNotCopyJobsComplete(requestContext, migrationId, TargetMigrationState.Complete, "All synchronous resource migration jobs have finished");
          else
            this.UpdateMigrationStateIfNotCopyJobsComplete(requestContext, migrationId, TargetMigrationState.CopyJobsComplete, "All synchronous resource migration jobs have finished");
        }
      }
      finally
      {
        requestContext.TraceLeave(15288121, TargetHostMigrationService.s_area, TargetHostMigrationService.s_layer, nameof (CheckResourceMigrationJobs));
      }

      List<ResourceMigrationJob> FetchResourceJobsForMigration()
      {
        List<ResourceMigrationJob> source;
        using (HostMigrationComponent component = requestContext.CreateComponent<HostMigrationComponent>())
          source = component.QueryResourceMigrationJobs(migrationId);
        return source.Where<ResourceMigrationJob>((Func<ResourceMigrationJob, bool>) (j => j.JobStage == MigrationJobStage.Target_Resources)).ToList<ResourceMigrationJob>();
      }
    }

    private static bool CheckForDedicatedBlobJobOnStorageMove(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, (RegistryQuery) FrameworkServerConstants.UseDedicatedBlobJobForStorageMove, false);

    private void UpdateMigrationStateIfNotCopyJobsComplete(
      IVssRequestContext requestContext,
      Guid migrationId,
      TargetMigrationState state,
      string message)
    {
      bool flag = false;
      TargetMigrationState state1;
      using (HostMigrationComponent component = requestContext.CreateComponent<HostMigrationComponent>())
      {
        state1 = component.GetTargetMigration(migrationId).State;
        if (state1 < TargetMigrationState.CopyJobsComplete)
        {
          component.UpdateTargetMigration(migrationId, state, message);
          flag = true;
        }
      }
      HostMigrationLogger.LogInfo(requestContext, migrationId, DateTime.UtcNow, "CheckResourceMigrationJobs", flag ? string.Format("Setting target migration state to {0}.", (object) state) : string.Format("Skipping setting the target migration state to {0} since the current state is {1}.", (object) state, (object) state1));
    }

    public void CleanupSasTokens(
      IVssRequestContext requestContext,
      TargetHostMigration migrationEntry)
    {
      TeamFoundationStrongBoxService service = requestContext.GetService<TeamFoundationStrongBoxService>();
      string secretsDrawerName = FrameworkServerConstants.HostMigrationSecretsDrawerName;
      HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (CleanupSasTokens), string.Format("Attempting to clean up SAS tokens in drawer '{0}' with migration id '{1}'", (object) secretsDrawerName, (object) migrationEntry.MigrationId));
      Guid drawerId = service.UnlockDrawer(requestContext, secretsDrawerName, false);
      if (!(drawerId != Guid.Empty))
        return;
      foreach (StrongBoxItemInfo drawerContent in service.GetDrawerContents(requestContext, drawerId))
      {
        Guid migrationId;
        if (HostMigrationStrongBoxUtil.GetMigrationIdFromLookupKey(drawerContent.LookupKey, out migrationId) && migrationEntry.MigrationId.Equals(migrationId))
        {
          service.DeleteItem(requestContext, drawerId, drawerContent.LookupKey);
          HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (CleanupSasTokens), "Deleted entry with lookup key " + drawerContent.LookupKey);
        }
      }
    }

    public string FinalizationCheck(
      IVssRequestContext requestContext,
      FinalizationCheckRequest request)
    {
      ArgumentUtility.CheckForNull<FinalizationCheckRequest>(request, nameof (request));
      ArgumentUtility.CheckForNull<SourceHostMigration>(request.SourceMigration, "SourceMigration");
      ArgumentUtility.CheckForNull<Guid[]>(request.DatabaseBackedHostIds, "DatabaseBackedHostIds");
      ArgumentUtility.CheckForNull<Guid[]>(request.VirtualHostIds, "VirtualHostIds");
      HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) request.SourceMigration, nameof (FinalizationCheck), "Starting Finalization Check");
      ITeamFoundationHostManagementService hms = requestContext.GetService<ITeamFoundationHostManagementService>();
      StringBuilder result = new StringBuilder();
      ((IEnumerable<Guid>) request.DatabaseBackedHostIds).ForEach<Guid>((Action<Guid>) (hostId => CheckHost(hostId, false)));
      ((IEnumerable<Guid>) request.VirtualHostIds).ForEach<Guid>((Action<Guid>) (hostId => CheckHost(hostId, true)));
      return result.ToString();

      void CheckHost(Guid hostId, bool expectVirutal)
      {
        HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) request.SourceMigration, "FinalizationCheck", string.Format("Starting Request context for Host Id {0}", (object) hostId));
        try
        {
          using (IVssRequestContext vssRequestContext = hms.BeginRequest(requestContext, hostId, RequestContextType.UserContext))
          {
            if (vssRequestContext.IsVirtualServiceHost())
            {
              string message;
              if (expectVirutal)
              {
                message = string.Format("Host {0} was found to be virtual in both the source and the target", (object) hostId);
              }
              else
              {
                message = string.Format("Host {0} was found to be virtual in the target, but back by a database partition in the source", (object) hostId);
                result.AppendLine(message);
              }
              HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) request.SourceMigration, "FinalizationCheck", message);
            }
            else
            {
              if (expectVirutal)
              {
                string message = string.Format("Host {0} is backed by a database partition in the target but was expected to be virtual in the source", (object) hostId);
                result.AppendLine(message);
                HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) request.SourceMigration, "FinalizationCheck", message);
              }
              IVssRegistryService service = vssRequestContext.GetService<IVssRegistryService>();
              string str1 = string.Format(FrameworkServerConstants.AccountMigrationCheckKey, (object) request.SourceMigration.MigrationId);
              IVssRequestContext requestContext = vssRequestContext;
              // ISSUE: explicit reference operation
              ref RegistryQuery local = @(RegistryQuery) str1;
              string empty = string.Empty;
              string str2 = service.GetValue(requestContext, in local, empty);
              HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) request.SourceMigration, "FinalizationCheck", string.Format("Registry Key {0} for {1} set to {2}", (object) str1, (object) hostId, (object) str2));
              if (!string.IsNullOrEmpty(str2))
                return;
              requestContext.TraceAlways(HostMigrationTrace.RegistryKeyNotSetTracepoint, TraceLevel.Info, TargetHostMigrationService.s_area, TargetHostMigrationService.s_layer, "Registry Key " + str1 + " not set");
            }
          }
        }
        catch (Exception ex)
        {
          string message = string.Format("Exception found while validating host {0}. {1}", (object) hostId, (object) ex);
          result.Append(message);
          HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) request.SourceMigration, "FinalizationCheck", message);
        }
      }
    }

    public void CleanupMigrationOnTarget(
      IVssRequestContext requestContext,
      TargetHostMigration migrationEntry)
    {
      HostMigrationUtil.CheckForFaultInjection(requestContext, (IMigrationEntry) migrationEntry, nameof (TargetHostMigrationService), nameof (CleanupMigrationOnTarget));
      if (!migrationEntry.HostProperties.IsVirtual && HostMigrationUtil.IsHostInReadOnlyMode(requestContext, migrationEntry.HostId))
        HostMigrationUtil.DisableReadOnlyMode(requestContext, migrationEntry.HostId, (Action<string>) (m => HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (CleanupMigrationOnTarget), m)));
      this.CleanupSasTokens(requestContext, migrationEntry);
      IInternalTeamFoundationHostManagementService service = requestContext.GetService<IInternalTeamFoundationHostManagementService>();
      TeamFoundationServiceHostProperties hostProperties = service.QueryServiceHostProperties(requestContext, migrationEntry.HostId);
      hostProperties.SubStatus = ServiceHostSubStatus.None;
      service.UpdateServiceHost(requestContext, hostProperties);
      HostMigrationLogger.FinishServicingJob(requestContext, migrationEntry, ServicingJobStatus.Complete, ServicingJobResult.Succeeded);
    }

    public void FinalizeMigrationOnTarget(
      IVssRequestContext requestContext,
      TargetHostMigration migrationEntry,
      bool rollback)
    {
      requestContext.TraceEnter(15288122, TargetHostMigrationService.s_area, TargetHostMigrationService.s_layer, nameof (FinalizeMigrationOnTarget));
      try
      {
        if (!rollback)
          HostMigrationUtil.CheckForFaultInjection(requestContext, (IMigrationEntry) migrationEntry, nameof (TargetHostMigrationService), nameof (FinalizeMigrationOnTarget));
        if (migrationEntry.StorageOnly & rollback)
          this.CleanupSasTokens(requestContext, migrationEntry);
        if (!migrationEntry.StorageOnly)
        {
          HostMigrationUtil.InvokeExtensionCallouts(requestContext, (Action<string, ServicingStepLogEntryKind>) ((msg, level) => HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnTarget), msg, level)), "FinalizeTarget", (Action<IHostMigrationExtension>) (extension => extension.FinalizeTarget(requestContext, migrationEntry, rollback)), false);
          TeamFoundationHostManagementService service1 = requestContext.GetService<TeamFoundationHostManagementService>();
          if (rollback)
          {
            HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnTarget), "Rolling back the migration.");
            TeamFoundationJobService service2 = requestContext.GetService<TeamFoundationJobService>();
            List<ResourceMigrationJob> resourceMigrationJobList;
            using (HostMigrationComponent component = requestContext.CreateComponent<HostMigrationComponent>())
              resourceMigrationJobList = component.QueryResourceMigrationJobs(migrationEntry.MigrationId);
            foreach (ResourceMigrationJob resourceMigrationJob in resourceMigrationJobList)
            {
              if (resourceMigrationJob.Status == ResourceMigrationState.Queued || resourceMigrationJob.Status == ResourceMigrationState.Started || resourceMigrationJob.Status == ResourceMigrationState.Verifing)
              {
                HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnTarget), string.Format("Stopping resource migration job: {0}", (object) resourceMigrationJob.JobId));
                try
                {
                  if (service2.StopJob(requestContext, resourceMigrationJob.JobId))
                  {
                    resourceMigrationJob.Status = ResourceMigrationState.Canceled;
                    using (HostMigrationComponent component = requestContext.CreateComponent<HostMigrationComponent>())
                      component.UpdateResourceMigrationJob(resourceMigrationJob);
                  }
                }
                catch (Exception ex)
                {
                  HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnTarget), string.Format("Error stopping resource migration job: {0}. Exception Details: {1}", (object) resourceMigrationJob.JobId, (object) ex.ToReadableStackTrace()));
                }
              }
              else
                HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnTarget), string.Format("Resource migration job already finished: {0}, status: {1}", (object) resourceMigrationJob.JobId, (object) resourceMigrationJob.Status));
            }
            HostProperties hostProperties = (HostProperties) service1.QueryServiceHostProperties(requestContext, migrationEntry.HostId);
            if (hostProperties != null)
            {
              HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnTarget), string.Format("Deleting service host: {0}", (object) migrationEntry.HostId));
              DeleteHostResourceOptions deleteHostResourceOptions;
              if (hostProperties.DatabaseId == migrationEntry.TargetDatabaseId && migrationEntry.TargetDatabaseId != DatabaseManagementConstants.InvalidDatabaseId)
              {
                HostMigrationUtil.CheckInstanceMappingNotLocal(requestContext, migrationEntry.HostId);
                deleteHostResourceOptions = DeleteHostResourceOptions.SkipMarkingBlobsForDeletion;
                if (!migrationEntry.Options.HasFlag((Enum) HostMigrationOptions.LiveHost))
                  deleteHostResourceOptions |= DeleteHostResourceOptions.MarkForDeletion;
                HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnTarget), string.Format("Using deletion option: {0}.  The host was moved into the target database so we will clean up that partition.", (object) deleteHostResourceOptions));
              }
              else
              {
                deleteHostResourceOptions = DeleteHostResourceOptions.None;
                HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnTarget), string.Format("Using deletion option: {0}.  The host is not in the target database yet, we only need to delete the host row in tbl_ServiceHost.", (object) deleteHostResourceOptions));
              }
              if (hostProperties.Status != TeamFoundationServiceHostStatus.Stopped)
                service1.StopHost(requestContext, hostProperties.Id, ServiceHostSubStatus.Migrating, "The host has been stopped for Migration rollback", TimeSpan.FromMinutes(3.0));
              if (migrationEntry.Options.HasFlag((Enum) HostMigrationOptions.LiveHost))
              {
                if (requestContext.IsFeatureEnabled(FrameworkServerConstants.Migration_RollbackCopyTaggedTargetBlobsToSourceFeatureName))
                {
                  HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnTarget), "Migration has LiveHost flag and feature flag Migration_RollbackCopyTaggedTargetBlobsToSourceFeatureName is enabled, rolling back tagged target blobs");
                  this.CopyTargetBlobsForRollback(requestContext, migrationEntry);
                }
                else
                  HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnTarget), "Migration has LiveHost flag and feature flag Migration_RollbackCopyTaggedTargetBlobsToSourceFeatureName is disabled, skipping rolling back tagged target blobs");
              }
              else
                HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnTarget), "Migration does not have LiveHost flag, skipping rolling back tagged target blobs");
              service1.DeleteServiceHost(requestContext, migrationEntry.HostProperties.Id, HostDeletionReason.HostMigrated, deleteHostResourceOptions);
            }
            else
              HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnTarget), string.Format("Skipping host deletion, the host {0} does not exist in tbl_ServiceHost in this instance.", (object) migrationEntry.HostId));
            HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnTarget), "Cleaning up SAS tokens on rollback");
            this.CleanupSasTokens(requestContext, migrationEntry);
            if (requestContext.IsFeatureEnabled(FrameworkServerConstants.Migration_RemoveRegisteredDBOnRollback))
              this.UnregisterSourceDatabase(requestContext, migrationEntry);
            HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnTarget), "Finished rolling back");
          }
          else
          {
            HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnTarget), "Completing the migration.");
            using (IDisposableReadOnlyList<IHostMigrationValidationExtension> extensions = requestContext.GetExtensions<IHostMigrationValidationExtension>())
            {
              HostMigrationUtil.CheckForFaultInjection(requestContext, (IMigrationEntry) migrationEntry, nameof (TargetHostMigrationService), "PreStartHostValidation");
              HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnTarget), string.Format("Found {0} classes implementing {1}.", (object) extensions.Count, (object) "IHostMigrationValidationExtension"));
              foreach (IHostMigrationValidationExtension validationExtension in (IEnumerable<IHostMigrationValidationExtension>) extensions)
              {
                HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnTarget), "Executing " + validationExtension.GetType().FullName + ".PreStartHostValidation.");
                string reason;
                if (!validationExtension.PreStartHostValidation(requestContext, migrationEntry.HostProperties.Id, out reason))
                {
                  if (string.IsNullOrEmpty(reason))
                    reason = "Validation failed for an unspecified reason.";
                  HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnTarget), validationExtension.GetType().FullName + " failed in PreStartHostValidation: " + reason);
                  throw new DataMigrationValidationFailedException(reason);
                }
              }
            }
            using (IVssRequestContext vssRequestContext = service1.BeginRequest(requestContext, migrationEntry.HostProperties.Id, RequestContextType.ServicingContext, true, true))
            {
              if (!vssRequestContext.IsVirtualServiceHost())
              {
                if (!migrationEntry.Options.HasFlag((Enum) HostMigrationOptions.LiveHost))
                {
                  TeamFoundationJobService service3 = vssRequestContext.GetService<TeamFoundationJobService>();
                  HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnTarget), "Repairing the job queue for standard or georeplication migration.");
                  IVssRequestContext requestContext1 = vssRequestContext;
                  service3.RepairQueue(requestContext1, (ITFLogger) null);
                }
              }
            }
            if (HostMigrationUtil.UseReadOnlyMode(requestContext))
              HostMigrationUtil.EnableReadOnlyMode(requestContext, migrationEntry.HostId, (Action<string>) (m => HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnTarget), m)));
            HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnTarget), string.Format("Starting the service host {0}.", (object) migrationEntry.HostProperties.Id));
            IInternalTeamFoundationHostManagementService service4 = requestContext.GetService<IInternalTeamFoundationHostManagementService>();
            bool reenableJobs = !requestContext.IsFeatureEnabled(TargetHostMigrationService.s_NoReenableJobsFF);
            if (!migrationEntry.Options.HasFlag((Enum) HostMigrationOptions.LiveHost))
            {
              service4.StartHostInternal(requestContext, migrationEntry.HostProperties.Id, ServiceHostSubStatus.Migrating, reenableJobs);
              HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnTarget), string.Format("Host {0} started with substatus {1}", (object) migrationEntry.HostProperties.Id, (object) ServiceHostSubStatus.Migrating));
              TargetHostMigrationService.LoadJobsFromStrongBox(requestContext, migrationEntry);
            }
            else
            {
              service4.StartHostInternal(requestContext, migrationEntry.HostProperties.Id, reenableJobs: reenableJobs);
              HostMigrationUtil.EnableMigratingMode(requestContext, migrationEntry.HostProperties.Id);
              HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnTarget), string.Format("LiveHost {0} started with substatus {1}", (object) migrationEntry.HostProperties.Id, (object) ServiceHostSubStatus.Migrating));
              using (IVssRequestContext requestContext2 = service1.BeginRequest(requestContext, migrationEntry.HostProperties.Id, RequestContextType.ServicingContext, true, true))
              {
                if (!requestContext2.IsVirtualServiceHost())
                {
                  HostMigrationUtil.SetDatabaseIdForHostDataspacesToUnspecified(requestContext, (ITeamFoundationHostManagementService) service1, migrationEntry.HostProperties.Id);
                  HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnTarget), string.Format("LiveHost {0} repairing dataspaces for host", (object) migrationEntry.HostProperties.Id));
                  this.FixQueueAndIncorrectJobPriorities(requestContext, migrationEntry, service1);
                }
              }
            }
            requestContext.GetService<IInternalLocationService>().OnLocationDataChanged(requestContext, LocationDataKind.All);
            using (IDisposableReadOnlyList<IHostMigrationValidationExtension> extensions = requestContext.GetExtensions<IHostMigrationValidationExtension>())
            {
              HostMigrationUtil.CheckForFaultInjection(requestContext, (IMigrationEntry) migrationEntry, nameof (TargetHostMigrationService), "PostStartHostValidation");
              HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnTarget), string.Format("Found {0} classes implementing {1}.", (object) extensions.Count, (object) "IHostMigrationValidationExtension"));
              foreach (IHostMigrationValidationExtension validationExtension in (IEnumerable<IHostMigrationValidationExtension>) extensions)
              {
                HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnTarget), "Executing " + validationExtension.GetType().FullName + ".PostStartHostValidation.");
                string reason;
                if (!validationExtension.PostStartHostValidation(requestContext, migrationEntry.HostProperties.Id, out reason))
                {
                  if (string.IsNullOrEmpty(reason))
                    reason = "Validation failed for an unspecified reason.";
                  HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnTarget), validationExtension.GetType().FullName + " failed in PostStartHostValidation: " + reason);
                  throw new DataMigrationValidationFailedException(reason);
                }
              }
            }
          }
          this.UnregisterSourceDatabase(requestContext, migrationEntry);
          HostMigrationUtil.InvokeExtensionCallouts(requestContext, (Action<string, ServicingStepLogEntryKind>) ((msg, level) => HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnTarget), msg, level)), "CleanupTarget", (Action<IHostMigrationExtension>) (extension => extension.CleanupTarget(requestContext, migrationEntry, rollback)), true);
        }
        if (migrationEntry.Options.HasFlag((Enum) HostMigrationOptions.LiveHost) && !migrationEntry.StorageOnly)
          requestContext.GetService<IVssRegistryService>().DeleteEntries(requestContext, FrameworkServerConstants.Migration_TagNewAzureBlobProviderBaseBlobsPath + "/" + migrationEntry.HostId.ToString());
        HostMigrationLogger.FinishServicingJob(requestContext, migrationEntry, ServicingJobStatus.Complete, ServicingJobResult.Succeeded);
      }
      catch (Exception ex)
      {
        HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnTarget), "An error occurred: " + ex.ToReadableStackTrace());
        if (rollback)
        {
          HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnTarget), "Cleaning up SAS tokens after error on rollback");
          this.CleanupSasTokens(requestContext, migrationEntry);
        }
        HostMigrationLogger.FinishServicingJob(requestContext, migrationEntry, ServicingJobStatus.Failed, ServicingJobResult.Failed);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(15288123, TargetHostMigrationService.s_area, TargetHostMigrationService.s_layer, nameof (FinalizeMigrationOnTarget));
      }
    }

    private void UnregisterSourceDatabase(
      IVssRequestContext requestContext,
      TargetHostMigration migrationEntry)
    {
      if (migrationEntry.SourceDatabaseId != DatabaseManagementConstants.InvalidDatabaseId && migrationEntry.SourceDatabaseId != -2)
      {
        HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, "FinalizeMigrationOnTarget", string.Format("Unregistering the source database with id {0}.  This will also unregister credentials.", (object) migrationEntry.SourceDatabaseId));
        TeamFoundationDatabaseManagementService service = requestContext.GetService<TeamFoundationDatabaseManagementService>();
        try
        {
          ITeamFoundationDatabaseProperties database = service.GetDatabase(requestContext, migrationEntry.SourceDatabaseId, true);
          if (string.Equals(DatabaseManagementConstants.MigrationStagingPool, database.PoolName, StringComparison.OrdinalIgnoreCase))
          {
            if (this.IsBulkMigrationDatabase(database))
              HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, "FinalizeMigrationOnTarget", string.Format("Skipping unmounting the foreign database because it is a bulk migration DB. DatabaseId: {0}", (object) migrationEntry.SourceDatabaseId));
            else if (migrationEntry.Options.HasFlag((Enum) HostMigrationOptions.UsePreConnectedDb))
            {
              HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, "FinalizeMigrationOnTarget", string.Format("Skipping unmounting the foreign database because it was connected outside the host migration process. DatabaseId: {0}", (object) migrationEntry.SourceDatabaseId));
            }
            else
            {
              HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, "FinalizeMigrationOnTarget", string.Format("Unmounting the foreign database. DatabaseId: {0}", (object) migrationEntry.SourceDatabaseId));
              service.RemoveDatabase(requestContext, migrationEntry.SourceDatabaseId);
            }
          }
          else
            HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, "FinalizeMigrationOnTarget", string.Format("WARNING: Database {0} is not in the migration pool.  Not removing it.  Current pool: {1}", (object) database.DatabaseId, (object) database.PoolName));
        }
        catch (DatabaseNotFoundException ex)
        {
        }
      }
      else
        HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, "FinalizeMigrationOnTarget", "Source database was never set, no database to unregister.");
    }

    private void FixQueueAndIncorrectJobPriorities(
      IVssRequestContext requestContext,
      TargetHostMigration migrationEntry,
      TeamFoundationHostManagementService hms)
    {
      using (IVssRequestContext vssRequestContext = hms.BeginRequest(requestContext, migrationEntry.HostProperties.Id, RequestContextType.ServicingContext, true, true))
      {
        if (!vssRequestContext.IsVirtualServiceHost())
        {
          if (migrationEntry.Options.HasFlag((Enum) HostMigrationOptions.LiveHost))
          {
            TeamFoundationJobService service = vssRequestContext.GetService<TeamFoundationJobService>();
            HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, "FinalizeMigrationOnTarget", "Repairing the job queue for livehost migration.");
            IVssRequestContext requestContext1 = vssRequestContext;
            service.RepairQueue(requestContext1, (ITFLogger) null);
          }
        }
      }
      try
      {
        TargetHostMigrationService.LoadJobsFromStrongBox(requestContext, migrationEntry);
        HostMigrationUtil.FixJobPriorities(requestContext, (IMigrationEntry) migrationEntry);
      }
      catch (Exception ex)
      {
        HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, "FinalizeMigrationOnTarget", "Failed to fix job priorities: " + ex.Message + " " + ex.StackTrace);
      }
    }

    private static void LoadJobsFromStrongBox(
      IVssRequestContext requestContext,
      TargetHostMigration migrationEntry)
    {
      string entryName = string.Format(FrameworkServerConstants.HostMigrationJobQueueStrongBoxItem, (object) migrationEntry.MigrationId);
      string entryCountName = string.Format(FrameworkServerConstants.HostMigrationJobQueueEntryCountStrongBoxItem, (object) migrationEntry.MigrationId);
      try
      {
        AdHocJobInfo[] adHocJobInfoArray = JsonUtilities.Deserialize<AdHocJobInfo[]>(HostMigrationStrongBoxUtil.ReadEntriesFromStrongBox(requestContext, entryName, entryCountName, FrameworkServerConstants.HostMigrationJobQueueStrongBoxDrawer));
        List<AdHocJobInfo> source1 = new List<AdHocJobInfo>();
        foreach (AdHocJobInfo adHocJobInfo in adHocJobInfoArray)
        {
          HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, "FixQueueAndIncorrectJobPriorities", string.Format("Adding JobId: {0}, QueueTime: {1}, JobPriority: {2}", (object) adHocJobInfo.JobId, (object) adHocJobInfo.QueueTime, (object) adHocJobInfo.JobPriority));
          if (adHocJobInfo.JobPriority >= 240)
          {
            adHocJobInfo.JobPriority = adHocJobInfo.JobPriority * -1 + 256;
            HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, "FixQueueAndIncorrectJobPriorities", string.Format("Corrected JobId: {0}, JobPriority: {1}", (object) adHocJobInfo.JobId, (object) adHocJobInfo.JobPriority));
          }
          source1.Add(adHocJobInfo);
        }
        using (JobQueueComponent component = requestContext.CreateComponent<JobQueueComponent>())
        {
          IEnumerable<AdHocJobInfo> source2 = source1.Where<AdHocJobInfo>((Func<AdHocJobInfo, bool>) (job => !job.IsDormant));
          IEnumerable<AdHocJobInfo> source3 = source1.Where<AdHocJobInfo>((Func<AdHocJobInfo, bool>) (job => job.IsDormant));
          component.QueueJobs(migrationEntry.HostId, source2.Select<AdHocJobInfo, Tuple<Guid, int>>((Func<AdHocJobInfo, Tuple<Guid, int>>) (job => new Tuple<Guid, int>(job.JobId, job.JobPriority))), requestContext.ActivityId, requestContext.GetUserId(), JobPriorityLevel.Normal, 0, false);
          component.QueueJobs(migrationEntry.HostId, source3.Select<AdHocJobInfo, Tuple<Guid, int>>((Func<AdHocJobInfo, Tuple<Guid, int>>) (job => new Tuple<Guid, int>(job.JobId, job.JobPriority))), requestContext.ActivityId, requestContext.GetUserId(), JobPriorityLevel.Normal, 0, true);
        }
      }
      catch (StrongBoxDrawerNotFoundException ex)
      {
        HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, "FixQueueAndIncorrectJobPriorities", "Did not find drawer for " + FrameworkServerConstants.HostMigrationJobQueueStrongBoxDrawer);
      }
      catch (StrongBoxItemNotFoundException ex)
      {
        HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, "FixQueueAndIncorrectJobPriorities", string.Format("Did not find strongbox item for migration {0} : {1}", (object) migrationEntry.MigrationId, (object) ex.Message));
      }
    }

    internal List<AccountSasTokenInfo> GetTargetBlobProviderSasTokensForTaggedBlobs(
      IVssRequestContext requestContext,
      TargetHostMigration migrationEntry)
    {
      requestContext.GetService<IAzureBlobProviderService>();
      ISasTokenRequestService service = requestContext.GetService<ISasTokenRequestService>();
      Dictionary<string, AzureProvider> dictionary = HostMigrationUtil.LoadBlobProviders(requestContext, migrationEntry.StorageAccountId);
      List<AccountSasTokenInfo> tokensForTaggedBlobs = new List<AccountSasTokenInfo>();
      foreach (KeyValuePair<string, AzureProvider> keyValuePair in dictionary)
      {
        AzureProvider azureProvider = keyValuePair.Value;
        if (azureProvider.FindBlobsByTags((IDictionary<string, string>) new Dictionary<string, string>()
        {
          {
            "HostId",
            migrationEntry.HostProperties.Id.ToString("N")
          }
        }).Values.Any<TaggedBlobItem>())
        {
          TimeSpan expiration = TimeSpan.FromDays(60.0);
          string sasToken = service.GetSasToken(requestContext, azureProvider.Uri, SasRequestPermissions.Read | SasRequestPermissions.List | SasRequestPermissions.Filter, expiration, SasTokenVersion.V2020_08_04);
          tokensForTaggedBlobs.Add(new AccountSasTokenInfo()
          {
            VsoArea = keyValuePair.Key,
            ResourceUri = azureProvider.Uri.ToString(),
            AccountName = ((IEnumerable<string>) azureProvider.Uri.Host.Split('.')).First<string>(),
            SasTokenS2SEncrypted = sasToken
          });
        }
      }
      return tokensForTaggedBlobs;
    }

    private void CopyTargetBlobsForRollback(
      IVssRequestContext requestContext,
      TargetHostMigration migrationEntry)
    {
      if (migrationEntry.HostProperties.HostType != TeamFoundationHostType.ProjectCollection)
        return;
      List<AccountSasTokenInfo> tokensForTaggedBlobs = this.GetTargetBlobProviderSasTokensForTaggedBlobs(requestContext, migrationEntry);
      if (tokensForTaggedBlobs.Count <= 0)
        return;
      using (MigrationHttpClient httpClient = HostMigrationUtil.CreateHttpClient<MigrationHttpClient>(requestContext, migrationEntry.SourceServiceInstanceId))
      {
        MigrationHttpClient migrationHttpClient = httpClient;
        CopyNewTargetBlobsToSourceForRollbackRequest copyNewTargetBlobsToSourceForRollbackRequest = new CopyNewTargetBlobsToSourceForRollbackRequest();
        copyNewTargetBlobsToSourceForRollbackRequest.BlobProviderSasTokens = tokensForTaggedBlobs;
        Guid migrationId = migrationEntry.MigrationId;
        CancellationToken cancellationToken = new CancellationToken();
        migrationHttpClient.CopyNewTargetBlobsToSourceForRollbackAsync(copyNewTargetBlobsToSourceForRollbackRequest, migrationId, cancellationToken: cancellationToken).SyncResult();
      }
    }

    internal (ITeamFoundationDatabaseProperties Database, TeamFoundationDatabaseCredential[] Credentials) RegisterLiveHostMigrationDatabase(
      IVssRequestContext deploymentContext,
      ITFLogger logger,
      DatabaseRegistrationInfo registrationInfo,
      string dataTierName = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(deploymentContext, nameof (deploymentContext));
      ArgumentUtility.CheckForNull<ITFLogger>(logger, nameof (logger));
      ArgumentUtility.CheckForNull<DatabaseRegistrationInfo>(registrationInfo, nameof (registrationInfo));
      string serviceLevel1 = deploymentContext.ServiceHost.DeploymentServiceHost.DatabaseProperties.ServiceLevel;
      string b = registrationInfo.ServiceLevel;
      if (HostMigrationInjectionUtil.CheckInjection(deploymentContext, FrameworkServerConstants.SourceServiceLevelHigherThanTargetInjection))
        b = new ServiceLevel(serviceLevel1.Split(ServiceLevel.ServiceLevelSeparator)[0]).MajorVersion + ".M999;" + b;
      if (!string.Equals(serviceLevel1, b, StringComparison.Ordinal))
      {
        ServiceLevel serviceLevel2 = new ServiceLevel(serviceLevel1.Split(ServiceLevel.ServiceLevelSeparator)[0]);
        ServiceLevel serviceLevel3 = new ServiceLevel(b.Split(ServiceLevel.ServiceLevelSeparator)[0]);
        if (serviceLevel3 > serviceLevel2)
          throw new TeamFoundationServicingException("The source database is on a later milestone than the target. Source: " + serviceLevel3.ToString() + ", Target: " + serviceLevel2.ToString());
      }
      SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(registrationInfo.ConnectionString);
      TeamFoundationDatabaseManagementService service1 = deploymentContext.GetService<TeamFoundationDatabaseManagementService>();
      ITeamFoundationDatabaseProperties databaseProperties = service1.RegisterDatabase(deploymentContext, registrationInfo.ConnectionString, connectionStringBuilder.DataSource + ";" + connectionStringBuilder.InitialCatalog, registrationInfo.ServiceLevel, DatabaseManagementConstants.NoUpgradePartitionPool, registrationInfo.Tenants, 0, TeamFoundationDatabaseStatus.Servicing, new DateTime?(DateTime.UtcNow), "Scale Unit Split: External Database", new DateTime?(), false, (string) null, (byte[]) null, registrationInfo.Flags);
      ITeamFoundationSigningService service2 = deploymentContext.GetService<ITeamFoundationSigningService>();
      Guid databaseSigningKey = service2.GetDatabaseSigningKey(deploymentContext);
      byte[][] numArray = new byte[registrationInfo.Logins.Length][];
      for (int index = 0; index < registrationInfo.Logins.Length; ++index)
        numArray[index] = service2.Encrypt(deploymentContext, databaseSigningKey, Encoding.UTF8.GetBytes(registrationInfo.Logins[index].Password), SigningAlgorithm.SHA256);
      TeamFoundationDatabaseCredential[] databaseCredentialArray = new TeamFoundationDatabaseCredential[registrationInfo.Logins.Length];
      using (DatabaseCredentialsComponent component = deploymentContext.CreateComponent<DatabaseCredentialsComponent>())
      {
        for (int index = 0; index < registrationInfo.Logins.Length; ++index)
        {
          DatabaseLoginInfo login = registrationInfo.Logins[index];
          databaseCredentialArray[index] = component.RegisterDatabaseCredential(databaseProperties.DatabaseId, login.UserId, numArray[index], databaseSigningKey, false, login.CredentialName, (string) null);
          databaseCredentialArray[index].CredentialStatus = TeamFoundationDatabaseCredentialStatus.InUse;
          if (component is DatabaseCredentialsComponent5 credentialsComponent5)
            credentialsComponent5.UpdateDatabaseCredential(databaseCredentialArray[index]);
          else
            component.UpdateDatabaseCredential(databaseCredentialArray[index], false);
        }
      }
      if (!string.IsNullOrEmpty(dataTierName))
      {
        ISqlConnectionInfo connectionInfo = deploymentContext.GetService<TeamFoundationDataTierService>().GetDataTiers(deploymentContext, false).SingleOrDefault<DataTierInfo>((Func<DataTierInfo, bool>) (dt => dt.DataSource == dataTierName)).ConnectionInfo;
        foreach (DatabaseLoginInfo login in registrationInfo.Logins)
          service1.CreateSqlLogin(deploymentContext, connectionInfo, login.UserId, login.Password, login.CredentialName == DatabaseCredentialNames.DbOwnerCredential, logger, login.Sid);
      }
      return (databaseProperties, databaseCredentialArray);
    }

    public ITeamFoundationDatabaseProperties[] RegisterBulkMigrationDatabases(
      IVssRequestContext deploymentContext,
      DatabaseRegistrationInfo[] sourceDatabaseInfo,
      ITFLogger logger)
    {
      deploymentContext.CheckDeploymentRequestContext();
      ITeamFoundationDatabaseProperties[] databasePropertiesArray = new ITeamFoundationDatabaseProperties[sourceDatabaseInfo.Length];
      for (int index = 0; index < sourceDatabaseInfo.Length; ++index)
        databasePropertiesArray[index] = this.RegisterBulkMigrationSourceDatabase(deploymentContext, sourceDatabaseInfo[index], logger);
      return databasePropertiesArray;
    }

    public void CleanupBulkMigrationRegistrations(
      IVssRequestContext deploymentContext,
      ITFLogger logger)
    {
      deploymentContext.CheckDeploymentRequestContext();
      TeamFoundationDatabaseManagementService service = deploymentContext.GetService<TeamFoundationDatabaseManagementService>();
      foreach (ITeamFoundationDatabaseProperties queryDatabase in service.QueryDatabases(deploymentContext, DatabaseManagementConstants.MigrationStagingPool, true))
      {
        if (this.IsBulkMigrationDatabase(queryDatabase))
        {
          logger.Info("Deleting database " + queryDatabase.DatabaseName);
          service.RemoveDatabase(deploymentContext, queryDatabase.DatabaseId);
        }
        else
          logger.Info("Database " + queryDatabase.DatabaseName + " is not a bulk migration database");
      }
    }

    private ITeamFoundationDatabaseProperties RegisterBulkMigrationSourceDatabase(
      IVssRequestContext requestContext,
      DatabaseRegistrationInfo dbInfo,
      ITFLogger logger)
    {
      ArgumentUtility.CheckForNull<DatabaseRegistrationInfo>(dbInfo, nameof (dbInfo));
      ArgumentUtility.CheckCollectionForOutOfRange<DatabaseLoginInfo>((ICollection<DatabaseLoginInfo>) dbInfo.Logins, "Logins", 1, 1);
      string databaseName = this.BuildBulkMigrationDatabaseName(dbInfo.ConnectionString);
      TeamFoundationDatabaseManagementService service = requestContext.GetService<TeamFoundationDatabaseManagementService>();
      ITeamFoundationDatabaseProperties databaseProperties = service.QueryDatabases(requestContext, DatabaseManagementConstants.MigrationStagingPool, true).FirstOrDefault<ITeamFoundationDatabaseProperties>((Func<ITeamFoundationDatabaseProperties, bool>) (x => x.DatabaseName == databaseName));
      if (databaseProperties == null)
      {
        logger.Info("Registering database " + databaseName);
        databaseProperties = service.RegisterDatabase(requestContext, dbInfo.ConnectionString, databaseName, dbInfo.ServiceLevel, DatabaseManagementConstants.MigrationStagingPool, 0, 1, TeamFoundationDatabaseStatus.Servicing, new DateTime?(DateTime.UtcNow), "Mounting the foreign database", new DateTime?(), true, dbInfo.Logins[0].UserId, this.GetEncryptedPassword(requestContext, dbInfo.Logins[0].Password), dbInfo.Flags);
        logger.Info("Database registered");
      }
      else
        logger.Info("Database " + databaseName + " has already been registered");
      return databaseProperties;
    }

    private ITeamFoundationDatabaseProperties GetBulkMigrationDatabase(
      IVssRequestContext requestContext,
      TeamFoundationDatabaseManagementService dbms,
      string connectionString)
    {
      string bulkDatabaseName = this.BuildBulkMigrationDatabaseName(connectionString);
      return dbms.QueryDatabases(requestContext, DatabaseManagementConstants.MigrationStagingPool, true).SingleOrDefault<ITeamFoundationDatabaseProperties>((Func<ITeamFoundationDatabaseProperties, bool>) (x => x.DatabaseName.Equals(bulkDatabaseName, StringComparison.OrdinalIgnoreCase)));
    }

    private string BuildBulkMigrationDatabaseName(string connectionString) => "BulkMigrationDatabase;" + TeamFoundationDatabaseManagementService.ParseUniqueConnectionStringFields(connectionString);

    private byte[] GetEncryptedPassword(IVssRequestContext requestContext, string password)
    {
      ITeamFoundationSigningService service = requestContext.GetService<ITeamFoundationSigningService>();
      Guid databaseSigningKey = service.GetDatabaseSigningKey(requestContext);
      return service.Encrypt(requestContext, databaseSigningKey, Encoding.UTF8.GetBytes(password), SigningAlgorithm.SHA256);
    }

    private bool IsBulkMigrationDatabase(
      ITeamFoundationDatabaseProperties databaseProperties)
    {
      return databaseProperties.DatabaseName.StartsWith("BulkMigrationDatabase");
    }

    private void QueueBlobCopy(
      IVssRequestContext requestContext,
      TargetHostMigration migrationEntry)
    {
      requestContext.TraceEnter(15288124, TargetHostMigrationService.s_area, TargetHostMigrationService.s_layer, nameof (QueueBlobCopy));
      try
      {
        HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (QueueBlobCopy), "Configuring blob copy job.");
        bool flag1 = HostMigrationInjectionUtil.CheckInjection(requestContext, FrameworkServerConstants.DisableBlobCopyDuringMigrations);
        if (migrationEntry.OnlineBlobCopy & flag1)
          throw new TeamFoundationServicingException("Migration has online blob copy enabled and has disabled blob copy. Both of these features can't be enabled.");
        if (migrationEntry.OnlineBlobCopy | flag1)
        {
          List<StorageMigration> blobContainers = migrationEntry.GetBlobContainers();
          Dictionary<string, AzureProvider> dictionary = HostMigrationUtil.LoadBlobProviders(requestContext, migrationEntry.StorageAccountId);
          foreach (StorageMigration storageMigration in blobContainers)
          {
            if (storageMigration.IsSharded)
            {
              if (flag1)
              {
                HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (QueueBlobCopy), "Encountered a sharded container with blob copy disabled. Container: " + storageMigration.Id);
                throw new TeamFoundationServicingException("Migrations with blob copy disabled are not supported with sharded containers");
              }
            }
            else
            {
              HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (QueueBlobCopy), "Creating non-sharded container on target.  Container: " + storageMigration.Id);
              dictionary[storageMigration.VsoArea].GetCloudBlobContainer(requestContext, storageMigration.Id, true);
            }
          }
        }
        if (flag1)
        {
          HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (QueueBlobCopy), FrameworkServerConstants.DisableBlobCopyDuringMigrations + " is set, skipping blob copy.");
        }
        else
        {
          string str1;
          switch (migrationEntry.HostProperties.HostType)
          {
            case TeamFoundationHostType.Application:
              str1 = ServicingOperationConstants.MigrateAccountTargetBlobs;
              break;
            case TeamFoundationHostType.ProjectCollection:
              str1 = ServicingOperationConstants.MigrateCollectionTargetBlobs;
              break;
            default:
              throw new InvalidOperationException(string.Format("Only support host types are Account and ProjectCollection. {0} is not supported.", (object) migrationEntry.HostProperties.HostType));
          }
          ServicingFlags servicingFlags = ServicingFlags.HostMustExist;
          bool flag2 = TargetHostMigrationService.CheckForDedicatedBlobJobOnStorageMove(requestContext);
          if (migrationEntry.OnlineBlobCopy || flag2 && migrationEntry.StorageOnly)
            servicingFlags |= ServicingFlags.NotAcquiringServicingLock;
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
          TeamFoundationServicingService service1 = vssRequestContext.GetService<TeamFoundationServicingService>();
          IHostMigrationBackgroundJobService service2 = requestContext.GetService<IHostMigrationBackgroundJobService>();
          if (requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, (RegistryQuery) FrameworkServerConstants.UseTfsBlobMigration, false))
          {
            Guid jobId = Guid.NewGuid();
            bool flag3 = false;
            while (!flag3)
            {
              byte[] byteArray = jobId.ToByteArray();
              if (BitConverter.ToInt16(new byte[2]
              {
                byteArray[4],
                byteArray[5]
              }, 0) > (short) 256)
                flag3 = true;
              else
                jobId = Guid.NewGuid();
            }
            BlobMigrationJobData migrationJobData = new BlobMigrationJobData();
            migrationJobData.MigrationId = migrationEntry.MigrationId;
            BlobMigrationJobData objectToSerialize = migrationJobData;
            service2.RegisterResourceMigrationJob(requestContext, (IMigrationEntry) migrationEntry, jobId, "Parallel Blob Container Coordinator Copy Job", MigrationJobStage.Target_Resources, 0);
            TeamFoundationJobService service3 = requestContext.GetService<TeamFoundationJobService>();
            JobPriorityClass priorityClass = JobPriorityClass.None;
            JobPriorityLevel priorityLevel = JobPriorityLevel.Normal;
            TeamFoundationJobDefinition foundationJobDefinition = new TeamFoundationJobDefinition(jobId, "Parallel Blob Container Coordinator Copy Job", "Microsoft.VisualStudio.Services.Cloud.JobAgentPlugins.ParallelTfsBlobMigrationCoordinatorJob", TeamFoundationSerializationUtility.SerializeToXml((object) objectToSerialize), TeamFoundationJobEnabledState.Enabled, true, false, priorityClass);
            service3.UpdateJobDefinitions(requestContext, (IEnumerable<Guid>) null, (IEnumerable<TeamFoundationJobDefinition>) new TeamFoundationJobDefinition[1]
            {
              foundationJobDefinition
            });
            IEnumerable<TeamFoundationJobReference> jobReferences = (IEnumerable<TeamFoundationJobReference>) new TeamFoundationJobReference[1]
            {
              foundationJobDefinition.ToJobReference()
            };
            service3.QueueJobs(requestContext, jobReferences, priorityLevel, 3, false);
          }
          else
          {
            int levelParallelism = this.GetJobLevelParallelism(requestContext, migrationEntry);
            if (levelParallelism <= 1)
            {
              ServicingJobData servicingJobData = new ServicingJobData(new string[1]
              {
                str1
              })
              {
                ServicingHostId = requestContext.ServiceHost.DeploymentServiceHost.InstanceId,
                OperationClass = "MigrateAccount",
                JobTitle = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Migrate Blob Containers"),
                ServicingOptions = servicingFlags
              };
              Guid jobId = Guid.NewGuid();
              string containerCopyJobName = TargetHostMigrationService.s_blobContainerCopyJobName;
              IDictionary<string, string> servicingTokens1 = servicingJobData.ServicingTokens;
              string instanceId = ServicingTokenConstants.InstanceId;
              Guid guid = requestContext.ServiceHost.DeploymentServiceHost.InstanceId;
              string str2 = guid.ToString("D");
              servicingTokens1.Add(instanceId, str2);
              IDictionary<string, string> servicingTokens2 = servicingJobData.ServicingTokens;
              string migrationId = ServicingTokenConstants.MigrationId;
              guid = migrationEntry.MigrationId;
              string str3 = guid.ToString("D");
              servicingTokens2.Add(migrationId, str3);
              servicingJobData.ServicingTokens.Add(ServicingTokenConstants.MigrationContainerGroupIndex, 0.ToString());
              servicingJobData.ServicingTokens.Add(ServicingTokenConstants.MigrationJobLevelParallelism, 1.ToString());
              servicingJobData.ServicingTokens.Add(ServicingTokenConstants.MigrationInContainerParallelism, 1.ToString());
              IDictionary<string, string> servicingTokens3 = servicingJobData.ServicingTokens;
              string hostId = ServicingTokenConstants.HostId;
              guid = migrationEntry.HostProperties.Id;
              string str4 = guid.ToString("D");
              servicingTokens3.Add(hostId, str4);
              IDictionary<string, string> servicingTokens4 = servicingJobData.ServicingTokens;
              string concurrentJobsPerJobAgent = ServicingTokenConstants.MigrationMaxConcurrentJobsPerJobAgent;
              int num = 0;
              string str5 = num.ToString();
              servicingTokens4.Add(concurrentJobsPerJobAgent, str5);
              IDictionary<string, string> servicingTokens5 = servicingJobData.ServicingTokens;
              string intervalInSecond = ServicingTokenConstants.MigrationJobRescheduleIntervalInSecond;
              num = 0;
              string str6 = num.ToString();
              servicingTokens5.Add(intervalInSecond, str6);
              servicingJobData.ServicingTokens.Add(ServicingTokenConstants.MigrationJobRescheduleEndingFileTimeUtc, DateTime.UtcNow.ToFileTimeUtc().ToString());
              servicingJobData.ServicingTokens.Add(ServicingTokenConstants.JobName, containerCopyJobName);
              servicingJobData.ServicingTokens.Add(ServicingTokenConstants.JobId, jobId.ToString("D"));
              service2.RegisterResourceMigrationJob(vssRequestContext, (IMigrationEntry) migrationEntry, jobId, containerCopyJobName, MigrationJobStage.Target_Resources, 5);
              HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (QueueBlobCopy), string.Format("Queuing the blob copy job {0}.", (object) jobId));
              ServicingJobDetail servicingJobDetail = service1.QueueServicingJob(vssRequestContext, servicingJobData, JobPriorityClass.AboveNormal, JobPriorityLevel.Normal, new Guid?(jobId));
              HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (QueueBlobCopy), string.Format("Queued a job to setup the blob copies.  Job Id = {0}", (object) servicingJobDetail.JobId));
            }
            else
            {
              Guid jobId = Guid.NewGuid();
              string containerCopyJobName = TargetHostMigrationService.s_blobContainerCopyJobName;
              service2.RegisterResourceMigrationJob(vssRequestContext, (IMigrationEntry) migrationEntry, jobId, containerCopyJobName, MigrationJobStage.Target_Resources, 1);
              HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (QueueBlobCopy), string.Format("Queuing the blob copy coordinating job {0}.", (object) jobId));
              TeamFoundationJobService service4 = requestContext.GetService<TeamFoundationJobService>();
              this.QueueCoordinatingJob(requestContext, migrationEntry, jobId, TargetHostMigrationService.s_blobContainerDowntimeMigrationCoordinatingJobName, levelParallelism, service2, service4, false);
              HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (QueueBlobCopy), string.Format("Queued a coordinating job to launch blob copy jobs.  Job Id = {0}", (object) jobId));
            }
          }
        }
      }
      finally
      {
        requestContext.TraceLeave(15288125, TargetHostMigrationService.s_area, TargetHostMigrationService.s_layer, nameof (QueueBlobCopy));
      }
    }

    private void QueueAdHocJobCopy(
      IVssRequestContext requestContext,
      TargetHostMigration migrationEntry)
    {
      if (migrationEntry.HostType.HasFlag((Enum) TeamFoundationHostType.Deployment))
        return;
      Guid jobId = Guid.NewGuid();
      requestContext.GetService<IHostMigrationBackgroundJobService>().RegisterResourceMigrationJob(requestContext, (IMigrationEntry) migrationEntry, jobId, "Copy AdHoc Jobs", MigrationJobStage.Target_Resources, 5);
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      TeamFoundationJobDefinition foundationJobDefinition = new TeamFoundationJobDefinition(jobId, "Copy Ad-Hoc Jobs", TargetHostMigrationService.s_AdHocJobMigrationJobName, TeamFoundationSerializationUtility.SerializeToXml((object) migrationEntry.MigrationId), TeamFoundationJobEnabledState.Enabled, true, false, JobPriorityClass.Normal);
      service.UpdateJobDefinitions(requestContext, (IEnumerable<Guid>) null, (IEnumerable<TeamFoundationJobDefinition>) new TeamFoundationJobDefinition[1]
      {
        foundationJobDefinition
      });
      IEnumerable<TeamFoundationJobReference> jobReferences = (IEnumerable<TeamFoundationJobReference>) new TeamFoundationJobReference[1]
      {
        foundationJobDefinition.ToJobReference()
      };
      service.QueueJobs(requestContext, jobReferences, JobPriorityLevel.Normal, 0, false);
    }

    private SqlConnectionInfoWrapper GetConnectionInfoWrapper(
      IVssRequestContext requestContext,
      int databaseId)
    {
      TeamFoundationDatabaseManagementService service = requestContext.GetService<TeamFoundationDatabaseManagementService>();
      ITeamFoundationDatabaseProperties database = service.GetDatabase(requestContext, databaseId, true);
      TeamFoundationDatabaseCredential databaseCredential = service.GetCredentialsForDatabase(requestContext, database.DatabaseId).SingleOrDefault<TeamFoundationDatabaseCredential>();
      string stringVar = databaseCredential != null ? Convert.ToBase64String(databaseCredential.PasswordEncrypted) : throw new DatabaseCredentialNotFoundException();
      ArgumentUtility.CheckStringForNullOrEmpty(databaseCredential.UserId, "UserId");
      ArgumentUtility.CheckStringForNullOrEmpty(stringVar, "passwordEncrypted");
      return new SqlConnectionInfoWrapper()
      {
        ConnectionString = database.ConnectionInfoWrapper.ConnectionString,
        UserId = databaseCredential.UserId,
        PasswordEncrypted = stringVar,
        SigningKeyId = databaseCredential.SigningKeyId
      };
    }

    private static string ToInvariant(FormattableString formattable) => formattable.ToString((IFormatProvider) CultureInfo.InvariantCulture);
  }
}
