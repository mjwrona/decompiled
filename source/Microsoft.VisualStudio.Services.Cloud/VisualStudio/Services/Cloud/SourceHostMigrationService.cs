// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SourceHostMigrationService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Storage.Blob;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Hosting;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.HostMigration;
using Microsoft.TeamFoundation.Framework.Server.Internal;
using Microsoft.VisualStudio.Services.Cloud.HostMigration;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Client;
using Microsoft.VisualStudio.Services.Partitioning;
using Microsoft.VisualStudio.Services.Partitioning.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class SourceHostMigrationService : IVssFrameworkService
  {
    private const int c_migrationJobRetries = 5;
    internal static readonly string s_area = "SourceHostMigration";
    internal static readonly string s_layer = "IVssFrameworkService";
    private const string c_bulkMigrationFirewallRulePrefix = "BulkMigration-TargetInstance-";
    private const string c_PrepareSourceDatabaseMigrationJobExtensionName = "Microsoft.TeamFoundation.JobService.Extensions.Hosting.PrepareSourceDatabaseMigrationJob";
    private const string c_PrepareSourceDatabaseMigrationJobName = "Prepare Source Database Job";
    private const string c_PrepareBlobsMigrationJobExtensionName = "Microsoft.TeamFoundation.JobService.Extensions.Hosting.PrepareBlobsMigrationJob";
    private const string c_PrepareBlobsMigrationJobName = "Prepare Blobs Job";
    private const string c_CreateSourceMigrationJobExtensionName = "Microsoft.TeamFoundation.JobService.Extensions.Hosting.CreateSourceMigrationJob";
    private const string c_CreateSourceMigrationJobName = "Create Source Migration Job";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      systemRequestContext.CheckSystemRequestContext();
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    private bool TryGetConnectionInfoWrapper(
      IVssRequestContext requestContext,
      int databaseId,
      int credentialId,
      out SqlConnectionInfoWrapper connectionInfoWrapper)
    {
      connectionInfoWrapper = (SqlConnectionInfoWrapper) null;
      TeamFoundationDatabaseManagementService service = requestContext.GetService<TeamFoundationDatabaseManagementService>();
      ITeamFoundationDatabaseProperties database = service.GetDatabase(requestContext, databaseId, true);
      IEnumerable<TeamFoundationDatabaseCredential> source = service.GetCredentialsForDatabase(requestContext, database.DatabaseId).Where<TeamFoundationDatabaseCredential>((Func<TeamFoundationDatabaseCredential, bool>) (cred => cred.Id == credentialId));
      if (source.Any<TeamFoundationDatabaseCredential>())
      {
        TeamFoundationDatabaseCredential databaseCredential = source.First<TeamFoundationDatabaseCredential>();
        string base64String = Convert.ToBase64String(databaseCredential.PasswordEncrypted);
        ArgumentUtility.CheckStringForNullOrEmpty(databaseCredential.UserId, "UserId");
        ArgumentUtility.CheckStringForNullOrEmpty(base64String, "passwordEncrypted");
        connectionInfoWrapper = new SqlConnectionInfoWrapper()
        {
          ConnectionString = database.ConnectionInfoWrapper.ConnectionString,
          UserId = databaseCredential.UserId,
          PasswordEncrypted = base64String,
          SigningKeyId = databaseCredential.SigningKeyId
        };
      }
      return connectionInfoWrapper != null;
    }

    public SourceHostMigration GetLatestMigrationEntry(
      IVssRequestContext requestContext,
      Guid hostId)
    {
      requestContext.TraceEnter(15288043, SourceHostMigrationService.s_area, SourceHostMigrationService.s_layer, nameof (GetLatestMigrationEntry));
      try
      {
        SourceHostMigration sourceMigration;
        using (HostMigrationComponent component = requestContext.CreateComponent<HostMigrationComponent>())
          sourceMigration = component.GetSourceMigrationByHostId(hostId).FirstOrDefault<SourceHostMigration>();
        if (sourceMigration != null)
          this.SetMigrationProperties(requestContext, sourceMigration);
        requestContext.Trace(15288045, TraceLevel.Info, SourceHostMigrationService.s_area, SourceHostMigrationService.s_layer, string.Format("Got latest migration entry on source.\n{0}", (object) sourceMigration));
        return sourceMigration;
      }
      finally
      {
        requestContext.TraceLeave(15288044, SourceHostMigrationService.s_area, SourceHostMigrationService.s_layer, nameof (GetLatestMigrationEntry));
      }
    }

    public SourceHostMigration GetMigrationEntry(
      IVssRequestContext requestContext,
      Guid migrationId)
    {
      requestContext.TraceEnter(15288046, SourceHostMigrationService.s_area, SourceHostMigrationService.s_layer, nameof (GetMigrationEntry));
      try
      {
        SourceHostMigration sourceMigration1;
        using (HostMigrationComponent component = requestContext.CreateComponent<HostMigrationComponent>())
          sourceMigration1 = component.GetSourceMigration(migrationId);
        SourceHostMigration sourceMigration2;
        if (sourceMigration1 != null)
        {
          sourceMigration2 = SourceHostMigrationService.CheckBackgroundMigrationJobs(requestContext, sourceMigration1);
          if (sourceMigration2.State == SourceMigrationState.PrepareBlobs)
          {
            string entryName1 = string.Format(FrameworkServerConstants.HostMigrationStorageMigrationStrongBoxItem, (object) sourceMigration2.MigrationId);
            string entryName2 = string.Format(FrameworkServerConstants.HostMigrationShardingInfoStrongBoxItem, (object) sourceMigration2.MigrationId);
            string entryCountName1 = string.Format(FrameworkServerConstants.HostMigrationStorageMigrationEntryCountStrongBoxItem, (object) sourceMigration2.MigrationId);
            string entryCountName2 = string.Format(FrameworkServerConstants.HostMigrationShardingInfoEntryCountStrongBoxItem, (object) sourceMigration2.MigrationId);
            try
            {
              string json1 = HostMigrationStrongBoxUtil.ReadEntriesFromStrongBox(requestContext, entryName1, entryCountName1, FrameworkServerConstants.HostMigrationStorageStrongBoxDrawer);
              string json2 = HostMigrationStrongBoxUtil.ReadEntriesFromStrongBox(requestContext, entryName2, entryCountName2, FrameworkServerConstants.HostMigrationStorageStrongBoxDrawer);
              StorageMigration[] storageMigrationArray = JsonUtilities.Deserialize<StorageMigration[]>(json1);
              ShardingInfo[] shardingInfoArray = JsonUtilities.Deserialize<ShardingInfo[]>(json2);
              sourceMigration2.StorageMigrations = storageMigrationArray;
              sourceMigration2.ShardingInfo = shardingInfoArray;
            }
            catch (StrongBoxDrawerNotFoundException ex)
            {
              HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) sourceMigration2, nameof (GetMigrationEntry), "Did not find drawer for " + FrameworkServerConstants.HostMigrationStorageStrongBoxDrawer);
            }
            catch (StrongBoxItemNotFoundException ex)
            {
              HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) sourceMigration2, nameof (GetMigrationEntry), string.Format("Did not find strongbox item for migration {0} : {1}", (object) sourceMigration2.MigrationId, (object) ex.Message));
            }
          }
          this.SetMigrationProperties(requestContext, sourceMigration2);
          requestContext.Trace(15288048, TraceLevel.Info, SourceHostMigrationService.s_area, SourceHostMigrationService.s_layer, string.Format("Got migration entry on source.\n{0}", (object) sourceMigration2));
        }
        else
          sourceMigration2 = this.CheckCreateSourceMigrationJob(requestContext, migrationId);
        return sourceMigration2;
      }
      finally
      {
        requestContext.TraceLeave(15288047, SourceHostMigrationService.s_area, SourceHostMigrationService.s_layer, nameof (GetMigrationEntry));
      }
    }

    private SourceHostMigration CheckCreateSourceMigrationJob(
      IVssRequestContext requestContext,
      Guid migrationId)
    {
      requestContext.Trace(15288076, TraceLevel.Info, SourceHostMigrationService.s_area, SourceHostMigrationService.s_layer, string.Format("Checking for a background job that is creating the source migration entry.\nMigrationId: {0}", (object) migrationId));
      SourceHostMigration sourceMigrationJob = (SourceHostMigration) null;
      List<ResourceMigrationJob> source;
      using (HostMigrationComponent component = requestContext.CreateComponent<HostMigrationComponent>())
        source = component.QueryMigrationJobs(migrationId);
      List<ResourceMigrationJob> list = source.Where<ResourceMigrationJob>((Func<ResourceMigrationJob, bool>) (x => x.JobStage == MigrationJobStage.Source_CreateSourceMigration)).ToList<ResourceMigrationJob>();
      if (list.Count > 0)
      {
        sourceMigrationJob = new SourceHostMigration();
        sourceMigrationJob.HostId = Guid.Empty;
        sourceMigrationJob.MigrationId = migrationId;
        sourceMigrationJob.State = SourceMigrationState.BeginCreate;
      }
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
              requestContext.TraceAlways(6349084, TraceLevel.Info, SourceHostMigrationService.s_area, SourceHostMigrationService.s_layer, string.Format("Job {0} corresponding to migration {1} failed with {2} retries remaining: {3}", (object) resourceMigrationJob.JobId, (object) resourceMigrationJob.MigrationId, (object) resourceMigrationJob.RetriesRemaining, (object) foundationJobHistoryEntry.ToString()));
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
                sourceMigrationJob.State = SourceMigrationState.Failed;
              }
            }
            else
              resourceMigrationJob.Status = ResourceMigrationState.Complete;
          }
          else
          {
            resourceMigrationJob.Status = ResourceMigrationState.Failed;
            sourceMigrationJob.State = SourceMigrationState.Failed;
          }
        }
        using (HostMigrationComponent component = requestContext.CreateComponent<HostMigrationComponent>())
          component.UpdateResourceMigrationJob(resourceMigrationJob);
      }
      return sourceMigrationJob;
    }

    private void SetMigrationProperties(
      IVssRequestContext requestContext,
      SourceHostMigration sourceMigration)
    {
      requestContext.TraceEnter(15288049, SourceHostMigrationService.s_area, SourceHostMigrationService.s_layer, nameof (SetMigrationProperties));
      try
      {
        if (sourceMigration == null)
          return;
        TeamFoundationServiceHostProperties hostProperties = requestContext.GetService<ITeamFoundationHostManagementService>().QueryServiceHostProperties(requestContext, sourceMigration.HostId, ServiceHostFilterFlags.IncludeChildren);
        if (hostProperties != null)
        {
          sourceMigration.HostProperties = MigrationHostProperties.FromServiceHostProperties(hostProperties);
          sourceMigration.HostProperties.IsLocal = true;
          if (!sourceMigration.HostProperties.IsVirtual)
          {
            try
            {
              sourceMigration.HostProperties.IsInReadOnlyMode = HostMigrationUtil.IsHostInReadOnlyMode(requestContext, hostProperties.Id);
            }
            catch (Exception ex) when (ex is HostDoesNotExistException || ex is DatabasePartitionNotFoundException)
            {
              requestContext.Trace(HostMigrationTrace.HostMissingDuringReadOnlyCheckTracepoint, TraceLevel.Info, SourceHostMigrationService.s_area, SourceHostMigrationService.s_layer, "Failed when checking if the host is read-only. This can occur if the host was recently deleted. Exception: {0}", (object) ex);
              sourceMigration.HostProperties.IsInReadOnlyMode = false;
            }
          }
          if (sourceMigration.CredentialId != 0)
          {
            SqlConnectionInfoWrapper connectionInfoWrapper1;
            if (this.TryGetConnectionInfoWrapper(requestContext, hostProperties.DatabaseId, sourceMigration.CredentialId, out connectionInfoWrapper1))
            {
              ISqlConnectionInfo sqlConnectionInfo = connectionInfoWrapper1.ToSqlConnectionInfo(requestContext);
              SqlConnectionInfoWrapper connectionInfoWrapper2 = HostMigrationUtil.EncryptConnectionString(requestContext, sqlConnectionInfo);
              if (connectionInfoWrapper2 == null)
              {
                SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(sqlConnectionInfo.ConnectionString);
                connectionStringBuilder.Remove("Password");
                requestContext.Trace(15288009, TraceLevel.Error, SourceHostMigrationService.s_area, SourceHostMigrationService.s_layer, "Credentials failed to encrypt: ConnectionString: " + connectionStringBuilder.ConnectionString);
              }
              sourceMigration.ConnectionInfo = connectionInfoWrapper2;
              requestContext.Trace(15288051, TraceLevel.Info, SourceHostMigrationService.s_area, SourceHostMigrationService.s_layer, string.Format("ConnectionInfo set on source.\n{0}", (object) sourceMigration));
            }
            else
              requestContext.Trace(15288010, TraceLevel.Info, SourceHostMigrationService.s_area, SourceHostMigrationService.s_layer, string.Format("ConnectionInfo not set on source.\n{0}", (object) sourceMigration));
          }
          else
            requestContext.Trace(15288058, TraceLevel.Info, SourceHostMigrationService.s_area, SourceHostMigrationService.s_layer, string.Format("Credential ID not set.\n{0}", (object) sourceMigration));
        }
        else
          sourceMigration.HostProperties = new MigrationHostProperties()
          {
            Id = sourceMigration.HostId,
            HostType = sourceMigration.HostType
          };
      }
      finally
      {
        requestContext.TraceLeave(15288050, SourceHostMigrationService.s_area, SourceHostMigrationService.s_layer, nameof (SetMigrationProperties));
      }
    }

    public SourceHostMigration StartCreateSourceMigrationJob(
      IVssRequestContext deploymentContext,
      SourceHostMigration migrationEntry)
    {
      deploymentContext.TraceEnter(15288077, SourceHostMigrationService.s_area, SourceHostMigrationService.s_layer, nameof (StartCreateSourceMigrationJob));
      try
      {
        HostMigrationLogger.LogInfo(deploymentContext, (IMigrationEntry) migrationEntry, nameof (StartCreateSourceMigrationJob), "Starting job To create the source migration");
        IHostMigrationBackgroundJobService service1 = deploymentContext.GetService<IHostMigrationBackgroundJobService>();
        TeamFoundationJobService service2 = deploymentContext.GetService<TeamFoundationJobService>();
        Guid jobId = Guid.Empty;
        try
        {
          XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) migrationEntry);
          jobId = service2.QueueOneTimeJob(deploymentContext, "Create Source Migration Job", "Microsoft.TeamFoundation.JobService.Extensions.Hosting.CreateSourceMigrationJob", xml, JobPriorityLevel.Normal);
          service1.RegisterResourceMigrationJob(deploymentContext, (IMigrationEntry) migrationEntry, jobId, "Create Source Migration Job", MigrationJobStage.Source_CreateSourceMigration, 5);
          migrationEntry.State = SourceMigrationState.BeginCreate;
        }
        catch (Exception ex)
        {
          if (jobId != Guid.Empty)
            service2.StopJob(deploymentContext, jobId);
          HostMigrationLogger.LogInfo(deploymentContext, (IMigrationEntry) migrationEntry, nameof (StartCreateSourceMigrationJob), "Error while queuing and registering the CreateSourceMigrationJob. Exception: " + ex.Message);
          throw;
        }
        HostMigrationLogger.LogInfo(deploymentContext, (IMigrationEntry) migrationEntry, nameof (StartCreateSourceMigrationJob), "Create source migration job submitted with job id: " + jobId.ToString());
        return migrationEntry;
      }
      finally
      {
        deploymentContext.TraceLeave(15288078, SourceHostMigrationService.s_area, SourceHostMigrationService.s_layer, nameof (StartCreateSourceMigrationJob));
      }
    }

    public SourceHostMigration CreateSourceMigration(
      IVssRequestContext requestContext,
      SourceHostMigration migrationRequest)
    {
      // ISSUE: unable to decompile the method.
    }

    private void CheckForExistingMigrations(IVssRequestContext requestContext, Guid hostId)
    {
      List<SourceHostMigration> list;
      using (HostMigrationComponent component = requestContext.CreateComponent<HostMigrationComponent>())
        list = component.GetSourceMigrationByHostId(hostId).OrderByDescending<SourceHostMigration, DateTime>((Func<SourceHostMigration, DateTime>) (x => x.StatusChangedDate)).ToList<SourceHostMigration>();
      SourceHostMigration sourceHostMigration = list.FirstOrDefault<SourceHostMigration>((Func<SourceHostMigration, bool>) (p => p.State != SourceMigrationState.Complete && p.State != SourceMigrationState.RolledBack));
      if (sourceHostMigration != null)
      {
        requestContext.Trace(HostMigrationTrace.ExistingMigrationOnSourceTracepoint, TraceLevel.Error, SourceHostMigrationService.s_area, SourceHostMigrationService.s_layer, "Existing Migrations: " + string.Join<SourceHostMigration>(", ", (IEnumerable<SourceHostMigration>) list));
        throw new DataMigrationEntryAlreadyExistsException(hostId, sourceHostMigration.ToString());
      }
      SourceHostMigration sourceMigration = list.FirstOrDefault<SourceHostMigration>();
      if (sourceMigration == null || !sourceMigration.StorageOnly || sourceMigration.State != SourceMigrationState.Complete)
        return;
      TargetHostMigration targetMigration = this.GetTargetMigration(requestContext, sourceMigration);
      if (targetMigration != null && targetMigration.State < TargetMigrationState.Complete)
      {
        requestContext.Trace(HostMigrationTrace.ExistingMigrationOnTargetTracepoint, TraceLevel.Error, SourceHostMigrationService.s_area, SourceHostMigrationService.s_layer, string.Format("Existing Migrations: {0} {1}", (object) string.Join<SourceHostMigration>(", ", (IEnumerable<SourceHostMigration>) list), (object) targetMigration));
        throw new DataMigrationEntryAlreadyExistsException(hostId, targetMigration.ToString());
      }
    }

    public SourceHostMigration StartPrepareSourceDatabaseJob(
      IVssRequestContext deploymentContext,
      SourceHostMigration migrationEntry)
    {
      deploymentContext.TraceEnter(15288063, SourceHostMigrationService.s_area, SourceHostMigrationService.s_layer, nameof (StartPrepareSourceDatabaseJob));
      try
      {
        HostMigrationLogger.LogInfo(deploymentContext, (IMigrationEntry) migrationEntry, nameof (StartPrepareSourceDatabaseJob), "Starting job To prepare source database");
        IHostMigrationBackgroundJobService service1 = deploymentContext.GetService<IHostMigrationBackgroundJobService>();
        TeamFoundationJobService service2 = deploymentContext.GetService<TeamFoundationJobService>();
        Guid jobId = Guid.Empty;
        try
        {
          XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) migrationEntry);
          jobId = service2.QueueOneTimeJob(deploymentContext, "Prepare Source Database Job", "Microsoft.TeamFoundation.JobService.Extensions.Hosting.PrepareSourceDatabaseMigrationJob", xml, JobPriorityLevel.Normal);
          service1.RegisterResourceMigrationJob(deploymentContext, (IMigrationEntry) migrationEntry, jobId, "Prepare Source Database Job", MigrationJobStage.Source_PrepareSourceDatabase, 5);
          using (HostMigrationComponent component = deploymentContext.CreateComponent<HostMigrationComponent>())
            component.UpdateSourceMigration(migrationEntry);
        }
        catch (Exception ex)
        {
          if (jobId != Guid.Empty)
            service2.StopJob(deploymentContext, jobId);
          HostMigrationLogger.LogInfo(deploymentContext, (IMigrationEntry) migrationEntry, "PrepareSourceDatabase", "Error while queuing and registering the PrepareSourceDatabaseMigrationJob. Exception: " + ex.Message);
          throw;
        }
        HostMigrationLogger.LogInfo(deploymentContext, (IMigrationEntry) migrationEntry, nameof (StartPrepareSourceDatabaseJob), "Prepare source database job submitted with job id: " + jobId.ToString());
        return migrationEntry;
      }
      finally
      {
        deploymentContext.TraceLeave(15288064, SourceHostMigrationService.s_area, SourceHostMigrationService.s_layer, nameof (StartPrepareSourceDatabaseJob));
      }
    }

    public SourceHostMigration PrepareSourceDatabase(
      IVssRequestContext deploymentContext,
      SourceHostMigration migrationEntry)
    {
      deploymentContext.TraceEnter(15288055, SourceHostMigrationService.s_area, SourceHostMigrationService.s_layer, nameof (PrepareSourceDatabase));
      try
      {
        ITeamFoundationHostManagementService service = deploymentContext.GetService<ITeamFoundationHostManagementService>();
        TeamFoundationServiceHostProperties hostProperties = service.QueryServiceHostProperties(deploymentContext, migrationEntry.HostProperties.Id);
        if (!migrationEntry.Options.HasFlag((System.Enum) HostMigrationOptions.UsePreConnectedDb) && migrationEntry.CredentialId == 0)
        {
          HostMigrationLogger.LogInfo(deploymentContext, (IMigrationEntry) migrationEntry, nameof (PrepareSourceDatabase), "Migration does not have a database credential assigned yet. Creating a new one.");
          Guid migrationId = migrationEntry.MigrationId;
          TeamFoundationDatabaseCredential credential = (TeamFoundationDatabaseCredential) null;
          if (hostProperties.IsVirtualServiceHost())
          {
            HostMigrationLogger.LogInfo(deploymentContext, (IMigrationEntry) migrationEntry, nameof (PrepareSourceDatabase), "The host is virtual, no need to create an alternate database credential.");
          }
          else
          {
            bool flag = migrationEntry.Options.HasFlag((System.Enum) HostMigrationOptions.NoPartitionCopy);
            if (!flag)
            {
              string query = string.Format((IFormatProvider) CultureInfo.InvariantCulture, FrameworkServerConstants.MigrationNoPartitionCopy, (object) migrationEntry.HostProperties.Id.ToString("D"));
              flag = deploymentContext.GetService<IVssRegistryService>().GetValue<bool>(deploymentContext, (RegistryQuery) query, false);
            }
            HostMigrationLogger.LogInfo(deploymentContext, (IMigrationEntry) migrationEntry, nameof (PrepareSourceDatabase), string.Format("Is partition database copy disabled? {0}", (object) flag));
            if (flag)
            {
              migrationEntry.StatusMessage = "Disable Partition Copy was set.  No database data will be added to the request.";
            }
            else
            {
              credential = this.GetBulkMigrationCredential(deploymentContext, hostProperties.DatabaseId);
              if (credential == null)
              {
                credential = this.CreateReadOnlyMigrationDatabaseCredential(deploymentContext, hostProperties.DatabaseId, DatabaseCredentialNames.MigrationReadOnlyCredential, (ITFLogger) new HostMigrationTFLogger(deploymentContext, (IMigrationEntry) migrationEntry, nameof (PrepareSourceDatabase)));
                HostMigrationLogger.LogInfo(deploymentContext, (IMigrationEntry) migrationEntry, nameof (PrepareSourceDatabase), string.Format("Alternate login credential successfully created with id {0}", (object) credential.Id));
                deploymentContext.Trace(15288012, TraceLevel.Info, SourceHostMigrationService.s_area, SourceHostMigrationService.s_layer, string.Format("Ad-hoc credentials created.\n{0}", (object) migrationEntry));
              }
              else
              {
                HostMigrationLogger.LogInfo(deploymentContext, (IMigrationEntry) migrationEntry, nameof (PrepareSourceDatabase), string.Format("Found an existing bulk migration credential {0}.  Will use this one for the migration", (object) credential.Id));
                deploymentContext.Trace(15288013, TraceLevel.Info, SourceHostMigrationService.s_area, SourceHostMigrationService.s_layer, string.Format("Existing credentials found.\n{0}", (object) migrationEntry));
              }
              migrationEntry.CredentialId = credential.Id;
              migrationEntry.StatusMessage = "The alternate database credential was successfully created";
            }
          }
          try
          {
            if (HostMigrationInjectionUtil.CheckInjection(deploymentContext, FrameworkServerConstants.FailSqlFirewallRuleInjection))
              throw new InvalidOperationException("Injection to simulate firewall rule creation exception");
            if (credential != null)
              this.CreateFirewallRules(deploymentContext, migrationEntry, credential);
          }
          catch (Exception ex)
          {
            HostMigrationLogger.LogInfo(deploymentContext, (IMigrationEntry) migrationEntry, nameof (PrepareSourceDatabase), "An error occurred while creating firewall rules. Exception Details: " + ex.ToReadableStackTrace(), ServicingStepLogEntryKind.Warning);
          }
        }
        else
          HostMigrationLogger.LogInfo(deploymentContext, (IMigrationEntry) migrationEntry, nameof (PrepareSourceDatabase), "Migration already had a database credential assigned. Returning existing one.");
        if (migrationEntry.Options.HasFlag((System.Enum) HostMigrationOptions.LiveHost) && !hostProperties.IsVirtualServiceHost())
          HostMigrationUtil.SetDatabaseIdForHostDataspacesToUnspecified(deploymentContext, service, migrationEntry.HostProperties.Id);
        HostMigrationUtil.InvokeExtensionCallouts(deploymentContext, (Action<string, ServicingStepLogEntryKind>) ((msg, level) => HostMigrationLogger.LogInfo(deploymentContext, (IMigrationEntry) migrationEntry, nameof (PrepareSourceDatabase), msg, level)), "PrepareSource", (Action<IHostMigrationExtension>) (extension => extension.PrepareSource(deploymentContext, migrationEntry)), false);
        using (HostMigrationComponent component = deploymentContext.CreateComponent<HostMigrationComponent>())
          component.UpdateSourceMigration(migrationEntry);
        HostMigrationLogger.LogInfo(deploymentContext, (IMigrationEntry) migrationEntry, nameof (PrepareSourceDatabase), "Successfully setup source migration");
        return this.GetMigrationEntry(deploymentContext, migrationEntry.MigrationId);
      }
      finally
      {
        deploymentContext.TraceLeave(15288056, SourceHostMigrationService.s_area, SourceHostMigrationService.s_layer, nameof (PrepareSourceDatabase));
      }
    }

    public TeamFoundationDatabaseCredential GetBulkMigrationCredential(
      IVssRequestContext deploymentContext,
      int databaseId)
    {
      List<TeamFoundationDatabaseCredential> credentialsForDatabase = deploymentContext.GetService<TeamFoundationDatabaseManagementService>().GetCredentialsForDatabase(deploymentContext, databaseId);
      return credentialsForDatabase.SingleOrDefault<TeamFoundationDatabaseCredential>((Func<TeamFoundationDatabaseCredential, bool>) (c => c.Name == DatabaseCredentialNames.BulkMigrationReadWriteCredential)) ?? credentialsForDatabase.SingleOrDefault<TeamFoundationDatabaseCredential>((Func<TeamFoundationDatabaseCredential, bool>) (c => c.Name == DatabaseCredentialNames.BulkMigrationReadOnlyCredential));
    }

    public TeamFoundationDatabaseCredential CreateReadOnlyMigrationDatabaseCredential(
      IVssRequestContext deploymentContext,
      int databaseId,
      string credentialName,
      ITFLogger logger)
    {
      ITeamFoundationDatabaseProperties database = deploymentContext.GetService<TeamFoundationDatabaseManagementService>().GetDatabase(deploymentContext, databaseId, true);
      return this.CreateReadOnlyMigrationDatabaseCredential(deploymentContext, database, credentialName, logger);
    }

    public TeamFoundationDatabaseCredential CreateReadOnlyMigrationDatabaseCredential(
      IVssRequestContext deploymentContext,
      ITeamFoundationDatabaseProperties databaseProperties,
      string credentialName,
      ITFLogger logger)
    {
      TeamFoundationDatabaseManagementService service = deploymentContext.GetService<TeamFoundationDatabaseManagementService>();
      logger.Info(string.Format("Attempting to create an alternate read-only credential for databaseId = {0}", (object) databaseProperties.DatabaseId));
      IVssRequestContext requestContext = deploymentContext;
      ITeamFoundationDatabaseProperties databaseProperties1 = databaseProperties;
      ITFLogger logger1 = logger;
      string credentialName1 = credentialName;
      string tfsReaderRole = DatabaseRoles.TfsReaderRole;
      TeamFoundationDatabaseCredential credential;
      ref TeamFoundationDatabaseCredential local = ref credential;
      service.AddDatabaseAlternateLogin(requestContext, databaseProperties1, TeamFoundationDatabaseType.Partition, logger1, credentialName1, tfsReaderRole, out local);
      using (TeamFoundationSqlSecurityComponent componentRaw = databaseProperties.DboConnectionInfo.CreateComponentRaw<TeamFoundationSqlSecurityComponent>())
      {
        componentRaw.GrantExecutePermission(credential.UserId, "prc_AddServicingStepGroupHistory");
        componentRaw.GrantExecutePermission(credential.UserId, "DIAGNOSTIC.prc_QueryEmptyTables");
      }
      this.ApplyPrepareSourceDatabaseExtensions(deploymentContext, databaseProperties, credential, logger);
      return credential;
    }

    public TeamFoundationDatabaseCredential CreateReadWriteMigrationDatabaseCredential(
      IVssRequestContext deploymentContext,
      ITeamFoundationDatabaseProperties databaseProperties,
      string credentialName,
      ITFLogger logger)
    {
      TeamFoundationDatabaseManagementService service = deploymentContext.GetService<TeamFoundationDatabaseManagementService>();
      logger.Info(string.Format("Attempting to create an alternate read/write credential for databaseId = {0}", (object) databaseProperties.DatabaseId));
      IVssRequestContext requestContext = deploymentContext;
      ITeamFoundationDatabaseProperties databaseProperties1 = databaseProperties;
      ITFLogger logger1 = logger;
      string credentialName1 = credentialName;
      string tfsExecRole = DatabaseRoles.TfsExecRole;
      TeamFoundationDatabaseCredential credential;
      ref TeamFoundationDatabaseCredential local = ref credential;
      service.AddDatabaseAlternateLogin(requestContext, databaseProperties1, TeamFoundationDatabaseType.Partition, logger1, credentialName1, tfsExecRole, out local);
      this.ApplyPrepareSourceDatabaseExtensions(deploymentContext, databaseProperties, credential, logger);
      return credential;
    }

    private void ApplyPrepareSourceDatabaseExtensions(
      IVssRequestContext deploymentContext,
      ITeamFoundationDatabaseProperties databaseProperties,
      TeamFoundationDatabaseCredential credential,
      ITFLogger logger)
    {
      Action<string, ServicingStepLogEntryKind> log = (Action<string, ServicingStepLogEntryKind>) ((msg, level) =>
      {
        if (level != ServicingStepLogEntryKind.Warning)
        {
          if (level == ServicingStepLogEntryKind.Error)
            logger.Error(msg);
          else
            logger.Info(msg);
        }
        else
          logger.Warning(msg);
      });
      HostMigrationUtil.InvokeExtensionCallouts(deploymentContext, log, "PrepareSourceDatabase", (Action<IHostMigrationExtension>) (extension => extension.PrepareSourceDatabase(deploymentContext, databaseProperties, credential, logger)), false);
    }

    public SourceHostMigration StartPrepareBlobsMigrationJob(
      IVssRequestContext deploymentContext,
      SourceHostMigration migrationEntry)
    {
      deploymentContext.TraceEnter(15288079, SourceHostMigrationService.s_area, SourceHostMigrationService.s_layer, nameof (StartPrepareBlobsMigrationJob));
      try
      {
        HostMigrationLogger.LogInfo(deploymentContext, (IMigrationEntry) migrationEntry, nameof (StartPrepareBlobsMigrationJob), "Starting job To prepare source database");
        IHostMigrationBackgroundJobService service1 = deploymentContext.GetService<IHostMigrationBackgroundJobService>();
        TeamFoundationJobService service2 = deploymentContext.GetService<TeamFoundationJobService>();
        Guid jobId = Guid.Empty;
        try
        {
          XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) migrationEntry);
          jobId = service2.QueueOneTimeJob(deploymentContext, "Prepare Blobs Job", "Microsoft.TeamFoundation.JobService.Extensions.Hosting.PrepareBlobsMigrationJob", xml, JobPriorityLevel.Normal);
          service1.RegisterResourceMigrationJob(deploymentContext, (IMigrationEntry) migrationEntry, jobId, "Prepare Blobs Job", MigrationJobStage.Source_PrepareBlobs, 5);
          using (HostMigrationComponent component = deploymentContext.CreateComponent<HostMigrationComponent>())
            component.UpdateSourceMigration(migrationEntry);
        }
        catch (Exception ex)
        {
          if (jobId != Guid.Empty)
            service2.StopJob(deploymentContext, jobId);
          HostMigrationLogger.LogInfo(deploymentContext, (IMigrationEntry) migrationEntry, nameof (StartPrepareBlobsMigrationJob), "Error while queuing and registering the PrepareBlobsMigrationJob. Exception: " + ex.Message);
          throw;
        }
        HostMigrationLogger.LogInfo(deploymentContext, (IMigrationEntry) migrationEntry, nameof (StartPrepareBlobsMigrationJob), "Prepare blobs job submitted with job id: " + jobId.ToString());
        return migrationEntry;
      }
      finally
      {
        deploymentContext.TraceLeave(15288080, SourceHostMigrationService.s_area, SourceHostMigrationService.s_layer, nameof (StartPrepareBlobsMigrationJob));
      }
    }

    public void PrepareBlobContainersForMigration(
      IVssRequestContext requestContext,
      SourceHostMigration migrationEntry)
    {
      requestContext.TraceEnter(15288104, SourceHostMigrationService.s_area, SourceHostMigrationService.s_layer, nameof (PrepareBlobContainersForMigration));
      try
      {
        HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (PrepareBlobContainersForMigration), "Starting preparation of blob containers by setting up SAS tokens.");
        bool testRegistryFlag = HostMigrationUtil.GetTestRegistryFlag(requestContext, BlobCopyUtil.s_testInvalidateSasTokens, (Action<string>) (x => HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (PrepareBlobContainersForMigration), x)));
        HostMigrationUtil.CheckForFaultInjection(requestContext, (IMigrationEntry) migrationEntry, nameof (SourceHostMigrationService), nameof (PrepareBlobContainersForMigration));
        this.PopulateStorageData(requestContext, migrationEntry, testRegistryFlag);
        migrationEntry.State = SourceMigrationState.PrepareBlobs;
        HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (PrepareBlobContainersForMigration), string.Format("Updating the state to {0}", (object) migrationEntry.State));
        using (HostMigrationComponent component = requestContext.CreateComponent<HostMigrationComponent>())
          component.UpdateSourceMigration(migrationEntry);
        HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (PrepareBlobContainersForMigration), "Successfully setup blobs for migration.");
      }
      catch (Exception ex)
      {
        migrationEntry.State = SourceMigrationState.Failed;
        HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (PrepareBlobContainersForMigration), string.Format("Updating the state to {0}", (object) migrationEntry.State));
        using (HostMigrationComponent component = requestContext.CreateComponent<HostMigrationComponent>())
          component.UpdateSourceMigration(migrationEntry);
        HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (PrepareBlobContainersForMigration), "Failed to setup blobs for migration. Error: " + ex.Message + ", " + ex.StackTrace);
      }
      finally
      {
        requestContext.TraceLeave(15288105, SourceHostMigrationService.s_area, SourceHostMigrationService.s_layer, nameof (PrepareBlobContainersForMigration));
      }
    }

    public void StartCopyNewTargetBlobsToSourceForRollbackJob(
      IVssRequestContext elevatedContext,
      Guid migrationId,
      CopyNewTargetBlobsToSourceForRollbackRequest copyNewTargetBlobsToSourceForRollbackRequest)
    {
      ITeamFoundationJobService service = elevatedContext.GetService<ITeamFoundationJobService>();
      XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) new ReverseBlobMigrationJobData()
      {
        MigrationId = migrationId,
        RollbackRequest = copyNewTargetBlobsToSourceForRollbackRequest
      });
      IVssRequestContext requestContext = elevatedContext;
      XmlNode jobData = xml;
      service.QueueOneTimeJob(requestContext, "Copy Tagged Target Blobs To Source", "Microsoft.VisualStudio.Services.Cloud.JobAgentPlugins.ReverseBlobMigrationJob", jobData, true);
    }

    public SourceHostMigration QueueUpdateLocation(
      IVssRequestContext requestContext,
      SourceHostMigration migrationEntry)
    {
      HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (QueueUpdateLocation), "Updating instance mappings.");
      if (migrationEntry.StorageOnly)
        throw new InvalidOperationException("It is invalid to update locations for a blob only migration.");
      migrationEntry.State = SourceMigrationState.BeginUpdateLocation;
      migrationEntry.StatusMessage = "Updating location data";
      using (HostMigrationComponent component = requestContext.CreateComponent<HostMigrationComponent>())
        component.UpdateSourceMigration(migrationEntry);
      requestContext.GetService<IHostMigrationBackgroundJobService>().QueueBackgroundMigrationJob(requestContext, new string[1]
      {
        ServicingOperationConstants.UpdateHostInstanceMapping
      }, (IMigrationEntry) migrationEntry, false, MigrationJobStage.Source_UpdateLocation);
      return migrationEntry;
    }

    public SourceHostMigration QueueFinalizeMigrationOnSource(
      IVssRequestContext requestContext,
      SourceHostMigration migrationEntry,
      bool rollback = false)
    {
      requestContext.TraceEnter(15288106, SourceHostMigrationService.s_area, SourceHostMigrationService.s_layer, nameof (QueueFinalizeMigrationOnSource));
      try
      {
        if (rollback)
        {
          HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (QueueFinalizeMigrationOnSource), "Verifying rollback-ability.");
          if (migrationEntry.HostProperties == null)
            throw new InvalidOperationException(string.Format("{0} failed. Unable to read the host properties. CreateSourceMigrationJob has most likely failed. MigrationId: {1} HostId: {2}", (object) nameof (QueueFinalizeMigrationOnSource), (object) migrationEntry.MigrationId, (object) migrationEntry.HostId));
          SourceHostMigration sourceHostMigration;
          if (migrationEntry.HostProperties.HostType == TeamFoundationHostType.ProjectCollection && migrationEntry.ParentMigrationId != Guid.Empty)
          {
            HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (QueueFinalizeMigrationOnSource), "The host type is a collection, verifying if there is a parent migration to make sure all hosts are still available for rollback.");
            sourceHostMigration = this.GetMigrationEntry(requestContext, migrationEntry.ParentMigrationId);
          }
          else
          {
            HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (QueueFinalizeMigrationOnSource), "The host type is an application, using the current migration request to check for host existence.");
            sourceHostMigration = migrationEntry;
          }
          if (sourceHostMigration.HostProperties.IsLocal)
          {
            HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (QueueFinalizeMigrationOnSource), "Hosts are still available. Rollback can continue.");
          }
          else
          {
            HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (QueueFinalizeMigrationOnSource), "Hosts are no longer local. Rollback can NOT continue.", ServicingStepLogEntryKind.Warning);
            throw new HostDoesNotExistException(sourceHostMigration.HostProperties.Id);
          }
        }
        else
          HostMigrationUtil.CheckForFaultInjection(requestContext, (IMigrationEntry) migrationEntry, nameof (SourceHostMigrationService), nameof (QueueFinalizeMigrationOnSource));
        migrationEntry.State = rollback ? SourceMigrationState.BeginRollback : SourceMigrationState.BeginComplete;
        migrationEntry.StatusMessage = string.Empty;
        HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (QueueFinalizeMigrationOnSource), string.Format("Setting migration state to {0}. Message: {1}", (object) migrationEntry.State, (object) migrationEntry.StatusMessage));
        using (HostMigrationComponent component = requestContext.CreateComponent<HostMigrationComponent>())
          component.UpdateSourceMigration(migrationEntry);
        requestContext.GetService<IHostMigrationBackgroundJobService>().QueueBackgroundMigrationJob(requestContext, new string[1]
        {
          ServicingOperationConstants.FinalizeMigrationOnSource
        }, (IMigrationEntry) migrationEntry, (rollback ? 1 : 0) != 0, (MigrationJobStage) (rollback ? 130 : 120), 5);
        return migrationEntry;
      }
      finally
      {
        requestContext.TraceLeave(15288107, SourceHostMigrationService.s_area, SourceHostMigrationService.s_layer, nameof (QueueFinalizeMigrationOnSource));
      }
    }

    private void PopulateStorageData(
      IVssRequestContext requestContext,
      SourceHostMigration migrationEntry,
      bool invalidateSasTokens)
    {
      requestContext.TraceEnter(15288108, SourceHostMigrationService.s_area, SourceHostMigrationService.s_layer, nameof (PopulateStorageData));
      try
      {
        TeamFoundationServiceHostProperties hostProperties = requestContext.GetService<TeamFoundationHostManagementService>().QueryServiceHostProperties(requestContext, migrationEntry.HostProperties.Id);
        Dictionary<string, AzureProvider> dictionary = HostMigrationUtil.LoadBlobProviders(requestContext, hostProperties.StorageAccountId);
        List<StorageMigration> storageMigrationList1 = new List<StorageMigration>();
        List<ShardingInfo> shardingInfoList = new List<ShardingInfo>();
        HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (PopulateStorageData), "Loading IBlobMigrationExtensions to retrieve the list of containers to migrate.");
        using (IDisposableReadOnlyList<BlobMigrationExtension> extensions = requestContext.GetExtensions<BlobMigrationExtension>())
        {
          foreach (BlobMigrationExtension migrationExtension in (IEnumerable<BlobMigrationExtension>) extensions)
          {
            HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (PopulateStorageData), "Loaded extension: " + migrationExtension.GetType().Name);
            List<StorageMigration> containerLists = migrationExtension.GetContainerLists(requestContext, hostProperties, (Action<string>) (x => HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (PopulateStorageData), x)));
            if (containerLists == null || containerLists.Count == 0)
            {
              HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (PopulateStorageData), "No blob containers were listed for migration from extension: " + migrationExtension.GetType().Name + ".");
            }
            else
            {
              HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (PopulateStorageData), string.Format("Adding {0} potential storage resources for migration. (We will check for existence first).", (object) containerLists.Count));
              storageMigrationList1.AddRange((IEnumerable<StorageMigration>) containerLists);
            }
            HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (PopulateStorageData), "Attempting to load ShardingInfo");
            List<ShardingInfo> shardingInfo = migrationExtension.GetShardingInfo(requestContext, hostProperties, (Action<string>) (x => HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (PopulateStorageData), x)));
            if (shardingInfo == null || shardingInfo.Count == 0)
            {
              HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (PopulateStorageData), "No sharding data were listed for migration from extension: " + migrationExtension.GetType().Name + ".");
            }
            else
            {
              HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (PopulateStorageData), string.Format("Adding {0} sharding metadata entries to the migration request.", (object) shardingInfo.Count));
              shardingInfoList.AddRange((IEnumerable<ShardingInfo>) shardingInfo);
            }
          }
        }
        if (!hostProperties.IsVirtualServiceHost())
        {
          HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (PopulateStorageData), "Adding SDK storage container to migration list.");
          storageMigrationList1.Add(new StorageMigration()
          {
            Id = migrationEntry.HostProperties.Id.ToString("n"),
            VsoArea = "FileService",
            StorageType = StorageType.Blob
          });
        }
        else
          HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (PopulateStorageData), "The host is virtual, not adding SDK storage container.");
        HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (PopulateStorageData), "Setting up SAS tokens on source containers.");
        List<StorageMigration> storageMigrationList2 = new List<StorageMigration>(storageMigrationList1.Count);
        AzureProvider azureProvider;
        DateTime utcNow;
        foreach (StorageMigration storageMigration in storageMigrationList1)
        {
          if (storageMigration.StorageType == StorageType.Blob)
          {
            if (dictionary.TryGetValue(storageMigration.VsoArea, out azureProvider))
            {
              CloudBlobContainer cloudBlobContainer = azureProvider.GetCloudBlobContainer(requestContext, storageMigration.Id, false);
              if (cloudBlobContainer != null && cloudBlobContainer.Exists())
              {
                SharedAccessBlobPolicy accessBlobPolicy1 = new SharedAccessBlobPolicy();
                accessBlobPolicy1.Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.List;
                utcNow = DateTime.UtcNow;
                accessBlobPolicy1.SharedAccessExpiryTime = new DateTimeOffset?((DateTimeOffset) utcNow.AddDays((double) HostMigrationUtil.DefaultSasTokenExpirationDays));
                SharedAccessBlobPolicy policy = accessBlobPolicy1;
                if (invalidateSasTokens)
                {
                  HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (PopulateStorageData), "Injecting expired SAS Token.  This should not occur in production, only when running tests.");
                  SharedAccessBlobPolicy accessBlobPolicy2 = policy;
                  utcNow = DateTime.UtcNow;
                  DateTimeOffset? nullable = new DateTimeOffset?((DateTimeOffset) utcNow.AddDays(-1.0));
                  accessBlobPolicy2.SharedAccessExpiryTime = nullable;
                }
                string sharedAccessSignature = cloudBlobContainer.GetSharedAccessSignature(policy);
                storageMigration.Uri = cloudBlobContainer.Uri.AbsoluteUri;
                storageMigration.SasToken = sharedAccessSignature;
                storageMigration.MigrationId = migrationEntry.MigrationId;
                storageMigrationList2.Add(storageMigration);
              }
            }
          }
          else if (storageMigration.StorageType == StorageType.Table)
          {
            if (dictionary.TryGetValue(storageMigration.VsoArea, out azureProvider))
            {
              CloudTable cloudTableReference = azureProvider.GetCloudTableReference(requestContext, storageMigration.Id, false);
              if (cloudTableReference != null && cloudTableReference.Exists())
              {
                CloudTable cloudTable = cloudTableReference;
                SharedAccessTablePolicy policy = new SharedAccessTablePolicy();
                policy.Permissions = SharedAccessTablePermissions.Query;
                utcNow = DateTime.UtcNow;
                policy.SharedAccessExpiryTime = new DateTimeOffset?((DateTimeOffset) utcNow.AddDays((double) HostMigrationUtil.DefaultSasTokenExpirationDays));
                string filterKey1 = storageMigration.FilterKey;
                string filterKey2 = storageMigration.FilterKey;
                string sharedAccessSignature = cloudTable.GetSharedAccessSignature(policy, (string) null, filterKey1, (string) null, filterKey2, (string) null);
                storageMigration.Uri = cloudTableReference.Uri.AbsoluteUri;
                storageMigration.SasToken = sharedAccessSignature;
                storageMigration.MigrationId = migrationEntry.MigrationId;
                storageMigrationList2.Add(storageMigration);
              }
            }
          }
          else if (storageMigration.StorageType == StorageType.CosmosDB)
            storageMigrationList2.Add(storageMigration);
        }
        StorageMigration[] array1 = storageMigrationList2.ToArray();
        ShardingInfo[] array2 = shardingInfoList.ToArray();
        string stringContent1 = array1.Serialize<StorageMigration[]>();
        string stringContent2 = array2.Serialize<ShardingInfo[]>();
        if (requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, (RegistryQuery) FrameworkServerConstants.PopulateBlobContainerLargeStrongBoxInjection, false, false))
        {
          HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, "PrepareBlobContainersForMigration", "Testing entry is set padding");
          int totalWidth = 1048576;
          stringContent1 = stringContent1.PadRight(totalWidth, ' ');
          stringContent2 = stringContent2.PadRight(totalWidth, ' ');
        }
        string entryName1 = string.Format(FrameworkServerConstants.HostMigrationStorageMigrationStrongBoxItem, (object) migrationEntry.MigrationId);
        string entryName2 = string.Format(FrameworkServerConstants.HostMigrationShardingInfoStrongBoxItem, (object) migrationEntry.MigrationId);
        string entryCountName1 = string.Format(FrameworkServerConstants.HostMigrationStorageMigrationEntryCountStrongBoxItem, (object) migrationEntry.MigrationId);
        string entryCountName2 = string.Format(FrameworkServerConstants.HostMigrationShardingInfoEntryCountStrongBoxItem, (object) migrationEntry.MigrationId);
        int strongBox1 = HostMigrationStrongBoxUtil.AddEntriesToStrongBox(requestContext, stringContent1, entryName1, entryCountName1, FrameworkServerConstants.HostMigrationStorageStrongBoxDrawer);
        int strongBox2 = HostMigrationStrongBoxUtil.AddEntriesToStrongBox(requestContext, stringContent2, entryName2, entryCountName2, FrameworkServerConstants.HostMigrationStorageStrongBoxDrawer);
        HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, "PrepareBlobContainersForMigration", string.Format("StorageMigrations in strongbox {0} with {1} entries", (object) entryName1, (object) strongBox1));
        HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, "PrepareBlobContainersForMigration", string.Format("Sharding Info in strongbox {0} with {1} entries", (object) entryName2, (object) strongBox2));
        HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, "PrepareBlobContainersForMigration", "Completed.");
      }
      finally
      {
        requestContext.TraceLeave(15288109, SourceHostMigrationService.s_area, SourceHostMigrationService.s_layer, "PrepareBlobContainersForMigration");
      }
    }

    public void FinalizeMigrationOnSource(
      IVssRequestContext requestContext,
      SourceHostMigration migrationEntry,
      bool rollback)
    {
      requestContext.TraceEnter(15288110, SourceHostMigrationService.s_area, SourceHostMigrationService.s_layer, nameof (FinalizeMigrationOnSource));
      try
      {
        HostMigrationUtil.CheckForFaultInjection(requestContext, (IMigrationEntry) migrationEntry, nameof (SourceHostMigrationService), nameof (FinalizeMigrationOnSource));
        if (migrationEntry.StorageOnly)
        {
          HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnSource), "Finalizing blob only source migration.");
        }
        else
        {
          HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnSource), "Finalizing source migration, cleaning up source resources");
          HostMigrationUtil.InvokeExtensionCallouts(requestContext, (Action<string, ServicingStepLogEntryKind>) ((msg, level) => HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnSource), msg, level)), "FinalizeSource", (Action<IHostMigrationExtension>) (extension => extension.FinalizeSource(requestContext, migrationEntry, rollback)), false);
          Guid id1 = migrationEntry.HostProperties.Id;
          TeamFoundationHostManagementService service1 = requestContext.GetService<TeamFoundationHostManagementService>();
          TeamFoundationServiceHostProperties hostProperties = service1.QueryServiceHostProperties(requestContext, id1, ServiceHostFilterFlags.IncludeChildren);
          if (!migrationEntry.Options.HasFlag((System.Enum) HostMigrationOptions.UsePreConnectedDb))
          {
            if (hostProperties != null && !hostProperties.IsVirtualServiceHost())
            {
              TeamFoundationDatabaseManagementService service2 = requestContext.GetService<TeamFoundationDatabaseManagementService>();
              TeamFoundationDatabaseCredential credential = service2.GetCredentialsForDatabase(requestContext, hostProperties.DatabaseId).Where<TeamFoundationDatabaseCredential>((Func<TeamFoundationDatabaseCredential, bool>) (cred => cred.Id == migrationEntry.CredentialId)).FirstOrDefault<TeamFoundationDatabaseCredential>();
              HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnSource), string.Format("Cleaning up database credentials and firewall rules. Credential id: {0}", (object) migrationEntry.CredentialId));
              if (credential != null)
              {
                if (!credential.IsPrimaryCredential)
                {
                  if (!this.IsBulkMigrationCredential(credential))
                  {
                    try
                    {
                      this.DeleteFirewallRules(requestContext, migrationEntry, credential);
                    }
                    catch (Exception ex)
                    {
                      HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnSource), "An error occurred while deleting firewall rules. Exception Details: " + ex.ToReadableStackTrace(), ServicingStepLogEntryKind.Warning);
                    }
                    HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnSource), string.Format("Dropping user and login for migration credential. Credential id: {0}", (object) migrationEntry.CredentialId));
                    requestContext.Trace(15288014, TraceLevel.Info, SourceHostMigrationService.s_area, SourceHostMigrationService.s_layer, string.Format("Credentials being dropped.\nMigrationId: {0}\nCredentialId: {1}", (object) migrationEntry.MigrationId, (object) migrationEntry.CredentialId));
                    credential.CredentialStatus = TeamFoundationDatabaseCredentialStatus.PendingDelete;
                    service2.UpdateCredential(requestContext, credential);
                    service2.DropDatabaseUserAndLogin(requestContext, credential, (ITFLogger) new NullLogger());
                  }
                  else
                  {
                    HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnSource), string.Format("Bulk credential was used for the migration, will not drop credential or firewall rules. Credential id: {0}", (object) migrationEntry.CredentialId));
                    requestContext.Trace(15288015, TraceLevel.Info, SourceHostMigrationService.s_area, SourceHostMigrationService.s_layer, string.Format("Bulk credentials are not being dropped.\nMigrationId: {0}\nCredentialId: {1}", (object) migrationEntry.MigrationId, (object) migrationEntry.CredentialId));
                  }
                }
              }
              else
                HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnSource), string.Format("Could not find a credential with id: {0}.", (object) migrationEntry.CredentialId));
            }
            else
              HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnSource), "Host has already been cleaned up (or is virtual). Skipping deletion of temporary db credentials.");
          }
          else
            HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnSource), "Pre-created credentials were used for this migration. No temporary db credentials to delete.");
          if (rollback)
          {
            if (hostProperties == null)
              throw new HostDoesNotExistException(id1);
            try
            {
              if (migrationEntry.Options.HasFlag((System.Enum) HostMigrationOptions.LiveHost))
              {
                if (!hostProperties.IsVirtualServiceHost())
                {
                  HostMigrationUtil.SetDatabaseIdForHostDataspacesToUnspecified(requestContext, (ITeamFoundationHostManagementService) service1, migrationEntry.HostProperties.Id);
                  HostMigrationUtil.FixJobPriorities(requestContext, (IMigrationEntry) migrationEntry);
                }
              }
            }
            catch
            {
              HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnSource), string.Format("Failed fixing job priorities on host {0}.", (object) hostProperties.Id));
            }
            HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnSource), "Rolling back Location updates.");
            this.UpdateHostInstanceMapping(requestContext, migrationEntry, hostProperties, true);
            HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnSource), string.Format("Starting host {0}.", (object) hostProperties.Id));
            IInternalTeamFoundationHostManagementService service3 = requestContext.GetService<IInternalTeamFoundationHostManagementService>();
            if (hostProperties.Status == TeamFoundationServiceHostStatus.Started)
            {
              if (!hostProperties.IsVirtualServiceHost() && HostMigrationUtil.IsHostInReadOnlyMode(requestContext, hostProperties.Id))
              {
                HostMigrationUtil.DisableReadOnlyMode(requestContext, hostProperties.Id, (Action<string>) (m => HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnSource), m)));
                TeamFoundationServiceHostProperties hostProperties1 = service3.QueryServiceHostProperties(requestContext, migrationEntry.HostId);
                hostProperties1.SubStatus = ServiceHostSubStatus.None;
                service3.UpdateServiceHost(requestContext, hostProperties1);
              }
              else if (HostMigrationUtil.IsServiceHostIdle(requestContext, hostProperties.Id))
                HostMigrationUtil.DisableIdleMode(requestContext, hostProperties.Id);
            }
            else
              service3.StartHostInternal(requestContext, hostProperties.Id);
          }
          else
          {
            if (requestContext.ServiceInstanceType() == ServiceInstanceTypes.SPS)
              this.UpdateHostInstanceMapping(requestContext, migrationEntry, hostProperties, false);
            if (!hostProperties.IsVirtualServiceHost())
              HostMigrationUtil.CheckInstanceMappingNotLocal(requestContext, hostProperties.Id);
            int attempt = 0;
            new RetryManager(10, TimeSpan.FromSeconds(5.0), (Action<Exception>) (e => HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnSource), string.Format("Failed attempting to verify that the target migration had completed. Attempt: {0}. HostId: {1}.  Exception details: {2}", (object) attempt, (object) hostProperties.Id, (object) e.ToReadableStackTrace())))).Invoke((Action) (() =>
            {
              ++attempt;
              TargetHostMigration targetMigration = this.GetTargetMigration(requestContext, migrationEntry);
              if (targetMigration.State != TargetMigrationState.Complete && targetMigration.State != TargetMigrationState.CompletePendingBlobs)
                throw new InvalidOperationException("Cannot finalize the source migration if the target migration is not complete.");
              if (targetMigration.HostProperties.Status != TeamFoundationServiceHostStatus.Started)
                throw new InvalidOperationException("Cannot finalize the source migration if the target migration's host is not Started.");
              HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnSource), string.Format("Verified that the target host migration has completed and the host is Started. MigrationId: {0}.  HostId: {1}", (object) targetMigration.MigrationId, (object) targetMigration.HostProperties.Id));
            }));
            if (migrationEntry.ParentMigrationId == Guid.Empty)
            {
              List<Guid> databaseBackedHostIds = new List<Guid>();
              List<Guid> virtualHostIds = new List<Guid>();
              this.AddHostToVirtualBasedList(requestContext, migrationEntry, id1, databaseBackedHostIds, virtualHostIds);
              foreach (TeamFoundationServiceHostProperties child in hostProperties.Children)
              {
                this.AddHostToVirtualBasedList(requestContext, migrationEntry, child.Id, databaseBackedHostIds, virtualHostIds);
                SourceHostMigration latestMigrationEntry = this.GetLatestMigrationEntry(requestContext, child.Id);
                if (latestMigrationEntry == null)
                {
                  string message = string.Format("A child host {0} still exists under host {1}, and does NOT have a migration Id, cannot continue with SOURCE host deletion!", (object) child.Id, (object) id1);
                  HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnSource), message);
                  throw new InvalidOperationException(message);
                }
                if (latestMigrationEntry.ParentMigrationId != migrationEntry.MigrationId)
                {
                  string message = string.Format("A child host {0} has a migration {1} who's parent is not this migration {2}", (object) child.Id, (object) latestMigrationEntry.MigrationId, (object) migrationEntry.MigrationId);
                  HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnSource), message);
                  throw new InvalidOperationException(message);
                }
                if (latestMigrationEntry.State != SourceMigrationState.Complete)
                {
                  string message = string.Format("A child host {0} still exists under host {1}, and does NOT have a successful migration.  Child host's migration id: {2}, Status: {3}", (object) child.Id, (object) id1, (object) latestMigrationEntry.MigrationId, (object) latestMigrationEntry.State);
                  HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnSource), message);
                  throw new InvalidOperationException(message);
                }
              }
              using (TargetHostMigrationHttpClient httpClient = HostMigrationUtil.CreateHttpClient<TargetHostMigrationHttpClient>(requestContext, migrationEntry.TargetServiceInstanceId))
              {
                string message = httpClient.FinalizationCheck(migrationEntry, (IEnumerable<Guid>) databaseBackedHostIds, (IEnumerable<Guid>) virtualHostIds);
                if (!string.IsNullOrEmpty(message))
                {
                  HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnSource), message);
                  throw new InvalidOperationException(message);
                }
              }
              IHostDeletionService service4 = requestContext.GetService<IHostDeletionService>();
              DeleteHostResourceOptions hostResourceOptions = DeleteHostResourceOptions.SkipSubStatusUpdate;
              if (!migrationEntry.Options.HasFlag((System.Enum) HostMigrationOptions.LiveHost))
                hostResourceOptions |= DeleteHostResourceOptions.MarkForDeletion;
              IVssRequestContext deploymentRequestContext = requestContext;
              Guid id2 = hostProperties.Id;
              int hostDeletionOptions = (int) hostResourceOptions;
              HostMigrationTFLogger logger = new HostMigrationTFLogger(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnSource));
              service4.DeleteHost(deploymentRequestContext, id2, (DeleteHostResourceOptions) hostDeletionOptions, HostDeletionReason.HostMigrated, (ITFLogger) logger);
            }
          }
          HostMigrationUtil.InvokeExtensionCallouts(requestContext, (Action<string, ServicingStepLogEntryKind>) ((msg, level) => HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnSource), msg, level)), "CleanupSource", (Action<IHostMigrationExtension>) (extension => extension.CleanupSource(requestContext, migrationEntry, rollback)), true);
        }
        string entryName1 = string.Format(FrameworkServerConstants.HostMigrationStorageMigrationStrongBoxItem, (object) migrationEntry.MigrationId);
        string entryName2 = string.Format(FrameworkServerConstants.HostMigrationShardingInfoStrongBoxItem, (object) migrationEntry.MigrationId);
        string entryCountName1 = string.Format(FrameworkServerConstants.HostMigrationStorageMigrationEntryCountStrongBoxItem, (object) migrationEntry.MigrationId);
        string entryCountName2 = string.Format(FrameworkServerConstants.HostMigrationShardingInfoEntryCountStrongBoxItem, (object) migrationEntry.MigrationId);
        requestContext.GetService<ITeamFoundationStrongBoxService>();
        try
        {
          HostMigrationStrongBoxUtil.DeleteStrongBoxEntries(requestContext, entryName1, entryCountName1);
          HostMigrationStrongBoxUtil.DeleteStrongBoxEntries(requestContext, entryName2, entryCountName2);
        }
        catch (StrongBoxDrawerNotFoundException ex)
        {
          HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (FinalizeMigrationOnSource), "Did not find drawer for " + FrameworkServerConstants.HostMigrationStorageStrongBoxDrawer);
        }
        HostMigrationLogger.FinishServicingJob(requestContext, migrationEntry, ServicingJobStatus.Complete, ServicingJobResult.Succeeded);
      }
      finally
      {
        requestContext.TraceLeave(15288111, SourceHostMigrationService.s_area, SourceHostMigrationService.s_layer, nameof (FinalizeMigrationOnSource));
      }
    }

    private void AddHostToVirtualBasedList(
      IVssRequestContext requestContext,
      SourceHostMigration sourceMigration,
      Guid hostId,
      List<Guid> databaseBackedHostIds,
      List<Guid> virtualHostIds)
    {
      HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) sourceMigration, nameof (AddHostToVirtualBasedList), string.Format("Starting Request context for Host Id {0}", (object) hostId));
      TeamFoundationServiceHostProperties hostProperties = requestContext.GetService<ITeamFoundationHostManagementService>().QueryServiceHostProperties(requestContext, hostId);
      if (hostProperties == null)
        throw new HostDoesNotExistException(hostId);
      (hostProperties.IsVirtualServiceHost() ? virtualHostIds : databaseBackedHostIds).Add(hostId);
    }

    public void UpdateHostInstanceMapping(
      IVssRequestContext requestContext,
      SourceHostMigration migrationEntry,
      TeamFoundationServiceHostProperties hostProperties,
      bool rollback)
    {
      requestContext.TraceEnter(15288112, SourceHostMigrationService.s_area, SourceHostMigrationService.s_layer, nameof (UpdateHostInstanceMapping));
      try
      {
        HostMigrationUtil.CheckForFaultInjection(requestContext, (IMigrationEntry) migrationEntry, nameof (SourceHostMigrationService), nameof (UpdateHostInstanceMapping));
        HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (UpdateHostInstanceMapping), string.Format("Attempting to update the host instance mappings for host: {0}. Host type: {1}", (object) hostProperties.Id, (object) hostProperties.HostType));
        IInternalInstanceManagementService ims = requestContext.GetService<IInternalInstanceManagementService>();
        ServiceInstance sourceInstance = (ServiceInstance) null;
        ServiceInstance targetInstance = (ServiceInstance) null;
        if (rollback)
        {
          ims.FlushLocationServiceData(requestContext, hostProperties.Id);
          HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (UpdateHostInstanceMapping), string.Format("Attempting to ROLLBACK the host instance mappings to the current instance id: {0}.", (object) requestContext.ServiceHost.InstanceId));
          targetInstance = new ServiceInstance()
          {
            InstanceId = requestContext.ServiceInstanceId(),
            InstanceType = requestContext.ServiceInstanceType()
          };
          sourceInstance = ims.GetServiceInstance(requestContext, migrationEntry.TargetServiceInstanceId);
        }
        else
        {
          HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (UpdateHostInstanceMapping), string.Format("Attempting to UPDATE the host instance mappings to instance to the target instance id: {0}.", (object) migrationEntry.TargetServiceInstanceId));
          targetInstance = ims.GetServiceInstance(requestContext, migrationEntry.TargetServiceInstanceId);
          sourceInstance = new ServiceInstance()
          {
            InstanceId = requestContext.ServiceInstanceId(),
            InstanceType = requestContext.ServiceInstanceType()
          };
        }
        if (migrationEntry.HostProperties.IsVirtual)
          HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (UpdateHostInstanceMapping), "The host is virtual, no need to update locations.");
        else if (requestContext.ServiceInstanceType() != ServiceInstanceTypes.SPS)
        {
          HostInstanceMapping actualTargetInstance = ims.GetHostInstanceMapping(requestContext, hostProperties.Id);
          if (actualTargetInstance.ServiceInstance.InstanceId == targetInstance.InstanceId)
          {
            HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (UpdateHostInstanceMapping), string.Format("Host {0} is already configured to target the service instance {1}", (object) migrationEntry.HostProperties.Id, (object) targetInstance.InstanceId));
          }
          else
          {
            if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Cloud.HostMigration.RobustLocationUpdate"))
            {
              int retryCount = 30;
              RetryManager retryManager = new RetryManager(retryCount, TimeSpan.FromSeconds(10.0));
              int attempt = 0;
              Action action = (Action) (() =>
              {
                ++attempt;
                if (attempt % 6 == 1)
                {
                  HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (UpdateHostInstanceMapping), string.Format("Updating host instance mapping. Attempt {0} of {1}.", (object) attempt, (object) retryCount));
                  ims.UpdateHostInstanceMappingStatus(requestContext, hostProperties.Id, ServiceStatus.Moving, sourceInstance);
                  ims.SetHostInstanceMapping(requestContext, hostProperties.Id, targetInstance);
                  Thread.Sleep(TimeSpan.FromSeconds(5.0));
                }
                HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (UpdateHostInstanceMapping), string.Format("Verify updated instance mapping. Attempt {0} of {1}.", (object) attempt, (object) retryCount));
                actualTargetInstance = ims.GetHostInstanceMapping(requestContext, hostProperties.Id);
                if (targetInstance.InstanceId != actualTargetInstance.ServiceInstance.InstanceId)
                {
                  string message = string.Format("Setting the assigned instance for the tenant in SPS did not yet update the assigned instance.  This would leave the target unreachable. Attempt {0} of {1}.", (object) attempt, (object) retryCount);
                  HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (UpdateHostInstanceMapping), message);
                  ims.FlushLocationServiceData(requestContext, hostProperties.Id);
                  throw new TeamFoundationServicingException(message);
                }
              });
              retryManager.Invoke(action);
            }
            else
            {
              ims.UpdateHostInstanceMappingStatus(requestContext, hostProperties.Id, ServiceStatus.Moving, sourceInstance);
              ims.SetHostInstanceMapping(requestContext, hostProperties.Id, targetInstance);
              Thread.Sleep(TimeSpan.FromSeconds(5.0));
              int retryCount = 30;
              RetryManager retryManager = new RetryManager(retryCount, TimeSpan.FromSeconds(10.0));
              int attempt = 0;
              Action action = (Action) (() =>
              {
                ++attempt;
                HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (UpdateHostInstanceMapping), string.Format("Verify updated instance mapping. Attempt {0} of {1}", (object) attempt, (object) retryCount));
                actualTargetInstance = ims.GetHostInstanceMapping(requestContext, hostProperties.Id);
                if (targetInstance.InstanceId != actualTargetInstance.ServiceInstance.InstanceId)
                {
                  actualTargetInstance = ims.GetHostInstanceMapping(requestContext, hostProperties.Id);
                  string message = string.Format("Setting the assigned instance for the tenant in SPS did not yet update the assigned instance.  This would leave the target unreachable. Attempt {0} of {1}.", (object) attempt, (object) retryCount);
                  HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (UpdateHostInstanceMapping), message);
                  throw new TeamFoundationServicingException(message);
                }
              });
              retryManager.Invoke(action);
            }
            HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (UpdateHostInstanceMapping), string.Format("Host instance mapping have been updated for host with id: {0}. Host Type: {1}", (object) hostProperties.Id, (object) hostProperties.HostType));
            if (!migrationEntry.Options.HasFlag((System.Enum) HostMigrationOptions.LiveHost))
              return;
            this.StopHost(requestContext, migrationEntry);
          }
        }
        else
        {
          IPartitioningService service = requestContext.GetService<IPartitioningService>();
          Guid guid;
          Guid targetInstanceId;
          if (rollback)
          {
            targetInstanceId = requestContext.ServiceHost.DeploymentServiceHost.InstanceId;
            guid = migrationEntry.TargetServiceInstanceId;
            HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (UpdateHostInstanceMapping), string.Format("Rolling back locations. Setting SPS target instance for MPS to local instance. Instance id: {0}", (object) targetInstanceId));
          }
          else
          {
            targetInstanceId = migrationEntry.TargetServiceInstanceId;
            guid = requestContext.ServiceHost.DeploymentServiceHost.InstanceId;
            HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (UpdateHostInstanceMapping), string.Format("Setting SPS target instance for MPS to remote instance. Instance id: {0}", (object) targetInstanceId));
          }
          PartitionContainer container = service.QueryPartitionContainers(requestContext, ServiceInstanceTypes.SPS).Where<PartitionContainer>((Func<PartitionContainer, bool>) (c => c.ContainerId == targetInstanceId)).FirstOrDefault<PartitionContainer>();
          if (container == null)
          {
            string message = string.Format("Could not locate the PartitionContainer in MPS for the SPS instance id: {0}", (object) targetInstanceId);
            HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (UpdateHostInstanceMapping), message);
            throw new TeamFoundationServicingException(message);
          }
          Partition partition = service.QueryPartition<Guid>(requestContext, migrationEntry.HostProperties.Id, ServiceInstanceTypes.SPS);
          if (partition == null || partition.Container.ContainerId == guid)
          {
            HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (UpdateHostInstanceMapping), "Deleting the old partition mapping for MPS.");
            service.DeletePartition<Guid>(requestContext, migrationEntry.HostProperties.Id, ServiceInstanceTypes.SPS);
            HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (UpdateHostInstanceMapping), "Creating the new partition mapping for MPS.");
            service.CreatePartition<Guid>(requestContext, migrationEntry.HostProperties.Id, container);
          }
          Thread.Sleep(TimeSpan.FromSeconds(5.0));
          foreach (HostInstanceMapping hostInstanceMapping in (IEnumerable<HostInstanceMapping>) ims.GetHostInstanceMappings(requestContext, migrationEntry.HostProperties.Id))
          {
            HostInstanceMapping mapping = hostInstanceMapping;
            if (mapping.Status != ServiceStatus.Active)
            {
              HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (UpdateHostInstanceMapping), string.Format("Skipping mapping {0} because non-Active status: {1}", (object) mapping.Uri, (object) mapping.Status));
            }
            else
            {
              int retryCount = 30;
              int attempt = 0;
              new RetryManager(retryCount, TimeSpan.FromSeconds(5.0), (Action<Exception>) (e => HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (UpdateHostInstanceMapping), string.Format("Exception while updating SPS location for host at: {0}. Attempt {1} of {2}. Exception details: {3}", (object) mapping.Uri, (object) attempt, (object) retryCount, (object) e.ToReadableStackTrace())))).Invoke((Action) (() =>
              {
                ++attempt;
                HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (UpdateHostInstanceMapping), string.Format("Creating location client for instanceId: {0}. (type: {1})", (object) mapping.ServiceInstance.InstanceId, (object) mapping.ServiceInstance.InstanceType));
                using (LocationHttpClient httpClient = HostMigrationUtil.CreateHttpClient<LocationHttpClient>(requestContext, mapping.ServiceInstance.InstanceId))
                {
                  HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (UpdateHostInstanceMapping), string.Format("Updating SPS location for host at: {0}. Attempt {1} of {2}.", (object) mapping.Uri, (object) attempt, (object) retryCount));
                  TaskExtensions.SyncResult(httpClient.FlushSpsServiceDefinitionAsync(migrationEntry.HostProperties.Id, requestContext.CancellationToken));
                }
              }));
            }
          }
        }
      }
      finally
      {
        requestContext.TraceLeave(15288113, SourceHostMigrationService.s_area, SourceHostMigrationService.s_layer, nameof (UpdateHostInstanceMapping));
      }
    }

    public void StopHost(IVssRequestContext deploymentContext, SourceHostMigration migration)
    {
      TimeSpan timeout = deploymentContext.GetService<IVssRegistryService>().GetValue<TimeSpan>(deploymentContext, (RegistryQuery) FrameworkServerConstants.AccountMigrateWaitForStop, TimeSpan.FromMinutes(20.0));
      HostMigrationLogger.LogInfo(deploymentContext, (IMigrationEntry) migration, "CreateSourceMigration", string.Format("[Job] Stopping host {0}, going to wait {1}.", (object) migration.HostProperties.Id, (object) timeout));
      if (deploymentContext.GetService<TeamFoundationHostManagementService>().StopHost(deploymentContext, migration.HostProperties.Id, ServiceHostSubStatus.Migrating, "Stopping the host for an account migration", timeout))
      {
        HostMigrationLogger.LogInfo(deploymentContext, (IMigrationEntry) migration, "CreateSourceMigration", string.Format("[Job] Host {0} has been stopped.", (object) migration.HostProperties.Id));
        HostMigrationUtil.CheckForFaultInjection(deploymentContext, (IMigrationEntry) migration, nameof (SourceHostMigrationService), nameof (StopHost));
      }
      else
      {
        HostMigrationLogger.LogInfo(deploymentContext, (IMigrationEntry) migration, "CreateSourceMigration", string.Format("[Job] Error, unable to stop host {0}.", (object) migration.HostProperties.Id));
        throw new TeamFoundationServicingException("Failed to stop the host");
      }
    }

    public void SwitchHostToReadOnlyMode(
      IVssRequestContext deploymentContext,
      SourceHostMigration migration)
    {
      HostMigrationUtil.EnableReadOnlyMode(deploymentContext, migration.HostProperties.Id, (Action<string>) (m => HostMigrationLogger.LogInfo(deploymentContext, (IMigrationEntry) migration, "CreateSourceMigration", "[Job] " + m)));
      ITeamFoundationHostManagementService service = deploymentContext.GetService<ITeamFoundationHostManagementService>();
      TeamFoundationServiceHostProperties hostProperties = service.QueryServiceHostProperties(deploymentContext, migration.HostProperties.Id);
      hostProperties.SubStatus = ServiceHostSubStatus.Migrating;
      service.UpdateServiceHost(deploymentContext, hostProperties);
      HostMigrationUtil.CheckForFaultInjection(deploymentContext, (IMigrationEntry) migration, nameof (SourceHostMigrationService), nameof (SwitchHostToReadOnlyMode));
    }

    public void SwitchHostToIdleMode(
      IVssRequestContext deploymentContext,
      SourceHostMigration migration)
    {
      HostMigrationUtil.EnableIdleMode(deploymentContext, migration.HostProperties.Id);
      HostMigrationUtil.CheckForFaultInjection(deploymentContext, (IMigrationEntry) migration, nameof (SourceHostMigrationService), nameof (SwitchHostToIdleMode));
    }

    public DatabaseRegistrationInfo[] EnableBulkMigrations(
      IVssRequestContext deploymentContext,
      string[] targetInstances,
      IEnumerable<int> sourceDatabaseIds,
      bool writeAccess,
      ITFLogger logger)
    {
      deploymentContext.CheckDeploymentRequestContext();
      TeamFoundationDatabaseManagementService service = deploymentContext.GetService<TeamFoundationDatabaseManagementService>();
      IEnumerable<ITeamFoundationDatabaseProperties> databases = this.FilterDatabases((IEnumerable<ITeamFoundationDatabaseProperties>) service.QueryDatabases(deploymentContext, DatabaseManagementConstants.DefaultPartitionPoolName, true), sourceDatabaseIds);
      this.CreateBulkMigrationFirewallRules(deploymentContext, service, databases, targetInstances, logger);
      return this.CreateBulkMigrationDatabaseCredentials(deploymentContext, service, databases, writeAccess, logger);
    }

    public DatabaseRegistrationInfo EnableLiveHostMigrations(
      IVssRequestContext deploymentContext,
      ITFLogger logger,
      string targetInstance,
      int databaseId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(deploymentContext, nameof (deploymentContext));
      ArgumentUtility.CheckForNull<ITFLogger>(logger, nameof (logger));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(targetInstance, nameof (targetInstance));
      TeamFoundationDatabaseManagementService service = deploymentContext.GetService<TeamFoundationDatabaseManagementService>();
      ITeamFoundationDatabaseProperties database = service.QueryDatabases(deploymentContext, TeamFoundationDatabaseType.Partition).SingleOrDefault<ITeamFoundationDatabaseProperties>((Func<ITeamFoundationDatabaseProperties, bool>) (d => d.DatabaseId == databaseId));
      if (database == null)
        throw new ArgumentException(string.Format("Specified partition database ({0}) does not exist", (object) databaseId), nameof (databaseId));
      service.UpdateDatabaseProperties(deploymentContext, database.DatabaseId, (Action<TeamFoundationDatabaseProperties>) (editableProperties => editableProperties.PoolName = DatabaseManagementConstants.NoUpgradePartitionPool));
      this.CreateStandardFirewallRuleForInstance(deploymentContext, service, database, targetInstance, logger);
      return this.CreatePartitionMoveDatabaseLogins(deploymentContext, logger, service, database);
    }

    public void DeleteFirewallRuleForInstance(
      IVssRequestContext deploymentContext,
      ITFLogger logger,
      string targetInstance,
      int databaseId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(deploymentContext, nameof (deploymentContext));
      ArgumentUtility.CheckForNull<ITFLogger>(logger, nameof (logger));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(targetInstance, nameof (targetInstance));
      if (!deploymentContext.ExecutionEnvironment.IsCloudDeployment)
      {
        logger.Info("No need to delete firewall rule, since this is not a cloud deployment.");
      }
      else
      {
        logger.Info("Cleaning up firewall rule for instance '" + targetInstance + "'");
        TeamFoundationDatabaseManagementService service = deploymentContext.GetService<TeamFoundationDatabaseManagementService>();
        ITeamFoundationDatabaseProperties databaseProperties = service.QueryDatabases(deploymentContext, TeamFoundationDatabaseType.Partition).SingleOrDefault<ITeamFoundationDatabaseProperties>((Func<ITeamFoundationDatabaseProperties, bool>) (d => d.DatabaseId == databaseId));
        if (databaseProperties == null)
          throw new ArgumentException(string.Format("Specified partition database ({0}) does not exist", (object) databaseId), nameof (databaseId));
        ServiceInstance serviceInstance = HostMigrationUtil.GetServiceInstance(deploymentContext, targetInstance);
        logger.Info(string.Format("Resolved TargetInstance {0} to id {1}", (object) targetInstance, (object) serviceInstance.InstanceId));
        string name = serviceInstance.Name;
        DeploymentInformation deploymentInformation = HostMigrationUtil.CreateHttpClient<MigrationHttpClient>(deploymentContext, serviceInstance.InstanceId).GetDeploymentInformationAsync().SyncResult<DeploymentInformation>();
        if (deploymentInformation?.OutboundVIPs == null || deploymentInformation.OutboundVIPs.Length == 0)
          return;
        logger.Info(string.Format("Removing firewall rules for instance {0} on database {1}", (object) serviceInstance.InstanceId, (object) databaseProperties.DatabaseName));
        using (SqlAzureFirewallComponent componentRaw = service.GetDboConnectionInfo(deploymentContext, databaseProperties.DatabaseId).CreateComponentRaw<SqlAzureFirewallComponent>())
        {
          List<SqlAzureFirewallRule> dataseFirewallRules = componentRaw.GetDataseFirewallRules();
          for (int index = 0; index < dataseFirewallRules.Count; ++index)
          {
            SqlAzureFirewallRule azureFirewallRule = dataseFirewallRules[index];
            if (((IEnumerable<string>) deploymentInformation.OutboundVIPs).Contains<string>(azureFirewallRule.StartIpAddress))
            {
              componentRaw.DeleteDatabaseFirewallRule(azureFirewallRule.Name);
              logger.Info("Rule '" + azureFirewallRule.Name + "' has been deleted.");
            }
          }
        }
      }
    }

    private DatabaseRegistrationInfo CreatePartitionMoveDatabaseLogins(
      IVssRequestContext deploymentContext,
      ITFLogger logger,
      TeamFoundationDatabaseManagementService databaseManagementService,
      ITeamFoundationDatabaseProperties database)
    {
      DatabaseLoginInfo[] logins = new DatabaseLoginInfo[2];
      string loginName;
      string loginPassword;
      byte[] sid;
      databaseManagementService.AddDatabaseAlternateLoginNoCredential(deploymentContext, logger, database, TeamFoundationDatabaseType.Partition, DatabaseCredentialNames.DbOwnerCredential, DatabaseRoles.DbOwner, out loginName, out loginPassword, out sid);
      logins[0] = new DatabaseLoginInfo(loginName, loginPassword, DatabaseCredentialNames.DbOwnerCredential, sid);
      databaseManagementService.AddDatabaseAlternateLoginNoCredential(deploymentContext, logger, database, TeamFoundationDatabaseType.Partition, DatabaseCredentialNames.DefaultCredential, DatabaseRoles.TfsExecRole, out loginName, out loginPassword, out sid);
      logins[1] = new DatabaseLoginInfo(loginName, loginPassword, DatabaseCredentialNames.DefaultCredential, sid);
      return new DatabaseRegistrationInfo(database, logins);
    }

    private IEnumerable<ITeamFoundationDatabaseProperties> FilterDatabases(
      IEnumerable<ITeamFoundationDatabaseProperties> databases,
      IEnumerable<int> sourceDatabaseIds)
    {
      if (sourceDatabaseIds == null)
        return databases;
      List<ITeamFoundationDatabaseProperties> databasePropertiesList = new List<ITeamFoundationDatabaseProperties>();
      HashSet<int> source = new HashSet<int>(sourceDatabaseIds);
      foreach (ITeamFoundationDatabaseProperties database in databases)
      {
        if (source.Contains(database.DatabaseId))
        {
          source.Remove(database.DatabaseId);
          databasePropertiesList.Add(database);
        }
      }
      if (source.Count > 0)
        throw new ArgumentException("Specified partition database(s) do not exist: " + string.Join<int>(",", (IEnumerable<int>) source.ToArray<int>()), nameof (sourceDatabaseIds));
      return (IEnumerable<ITeamFoundationDatabaseProperties>) databasePropertiesList;
    }

    public void CleanupBulkMigrations(IVssRequestContext requestContext, ITFLogger logger)
    {
      requestContext.CheckDeploymentRequestContext();
      this.DeleteBulkMigrationFirewallRules(requestContext, logger);
      this.DeleteBulkMigrationDatabaseCredentials(requestContext, logger);
    }

    private bool IsBulkMigrationCredential(TeamFoundationDatabaseCredential credential) => credential.Name == DatabaseCredentialNames.BulkMigrationReadOnlyCredential || credential.Name == DatabaseCredentialNames.BulkMigrationReadWriteCredential;

    private DatabaseRegistrationInfo[] CreateBulkMigrationDatabaseCredentials(
      IVssRequestContext requestContext,
      TeamFoundationDatabaseManagementService databaseManagementService,
      IEnumerable<ITeamFoundationDatabaseProperties> databases,
      bool writeAccess,
      ITFLogger logger)
    {
      ITeamFoundationSigningService service = requestContext.GetService<ITeamFoundationSigningService>();
      List<TeamFoundationDatabaseCredential> source = databaseManagementService.QueryDatabaseCredentials(requestContext);
      List<DatabaseRegistrationInfo> registrationInfoList = new List<DatabaseRegistrationInfo>();
      logger.Info("Creating a migration credential for each partition DB");
      string credentialName = writeAccess ? DatabaseCredentialNames.BulkMigrationReadWriteCredential : DatabaseCredentialNames.BulkMigrationReadOnlyCredential;
      foreach (ITeamFoundationDatabaseProperties database in databases)
      {
        ITeamFoundationDatabaseProperties databaseProperties = database;
        TeamFoundationDatabaseCredential databaseCredential = source.SingleOrDefault<TeamFoundationDatabaseCredential>((Func<TeamFoundationDatabaseCredential, bool>) (x => x.DatabaseId == databaseProperties.DatabaseId && x.Name == credentialName));
        if (databaseCredential == null)
        {
          logger.Info(string.Format("Creating migration credential for database {0} ID: {1}", (object) databaseProperties.DatabaseName, (object) databaseProperties.DatabaseId));
          databaseCredential = writeAccess ? this.CreateReadWriteMigrationDatabaseCredential(requestContext, databaseProperties, credentialName, logger) : this.CreateReadOnlyMigrationDatabaseCredential(requestContext, databaseProperties, credentialName, logger);
          logger.Info("Credential " + databaseCredential.Name + " created successfully");
        }
        else
          logger.Info("Credential already exists for database " + databaseProperties.DatabaseName + ", will not create a new one");
        registrationInfoList.Add(new DatabaseRegistrationInfo(databaseProperties, databaseCredential.UserId, Encoding.UTF8.GetString(service.Decrypt(requestContext, databaseCredential.SigningKeyId, databaseCredential.PasswordEncrypted, SigningAlgorithm.SHA256)), databaseCredential.Name));
      }
      logger.Info("Credentials created successfully");
      return registrationInfoList.ToArray();
    }

    private void DeleteBulkMigrationDatabaseCredentials(
      IVssRequestContext requestContext,
      ITFLogger logger)
    {
      TeamFoundationDatabaseManagementService service = requestContext.GetService<TeamFoundationDatabaseManagementService>();
      List<TeamFoundationDatabaseCredential> databaseCredentialList = service.QueryDatabaseCredentials(requestContext);
      logger.Info(string.Format("Deleting bulk migration credentials. Processing {0} credentials", (object) databaseCredentialList.Count));
      foreach (TeamFoundationDatabaseCredential credential in databaseCredentialList)
      {
        if (this.IsBulkMigrationCredential(credential))
        {
          logger.Info("Deleting credential " + credential.Name);
          credential.CredentialStatus = TeamFoundationDatabaseCredentialStatus.PendingDelete;
          service.UpdateCredential(requestContext, credential);
          service.DropDatabaseUserAndLogin(requestContext, credential, logger);
        }
      }
    }

    private void SetStatusToMigrating(
      IVssDeploymentServiceHost deploymentHost,
      SourceHostMigration migration)
    {
      using (IVssRequestContext systemContext = deploymentHost.CreateSystemContext())
      {
        HostMigrationLogger.LogInfo(systemContext, (IMigrationEntry) migration, "CreateSourceMigration", string.Format("[Job] Updating host {0} to be in a Migrating state", (object) migration.HostProperties.Id));
        ITeamFoundationHostManagementService service = systemContext.GetService<ITeamFoundationHostManagementService>();
        TeamFoundationServiceHostProperties hostProperties = service.QueryServiceHostProperties(systemContext, migration.HostProperties.Id);
        if (hostProperties.Status != TeamFoundationServiceHostStatus.Stopped)
          throw new TeamFoundationServicingException(string.Format("Cannot update host {0} substatus to migrating because it is not stopped", (object) hostProperties.Id));
        hostProperties.SubStatus = ServiceHostSubStatus.Migrating;
        service.UpdateServiceHost(systemContext, hostProperties);
      }
    }

    internal virtual TargetHostMigration GetTargetMigration(
      IVssRequestContext requestContext,
      SourceHostMigration sourceMigration)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<SourceHostMigration>(sourceMigration, nameof (sourceMigration));
      using (TargetHostMigrationHttpClient httpClient = HostMigrationUtil.CreateHttpClient<TargetHostMigrationHttpClient>(requestContext, sourceMigration.TargetServiceInstanceId))
        return httpClient.GetMigrationEntry(sourceMigration.MigrationId);
    }

    private static SourceHostMigration CheckBackgroundMigrationJobs(
      IVssRequestContext requestContext,
      SourceHostMigration migrationEntry)
    {
      requestContext.TraceEnter(15288114, SourceHostMigrationService.s_area, SourceHostMigrationService.s_layer, nameof (CheckBackgroundMigrationJobs));
      try
      {
        SourceMigrationState? nextStageToAutoAdvance = new SourceMigrationState?();
        MigrationJobStage? jobStageToCheck = new MigrationJobStage?();
        switch (migrationEntry.State)
        {
          case SourceMigrationState.BeginCreate:
            jobStageToCheck = new MigrationJobStage?(MigrationJobStage.Source_CreateSourceMigration);
            nextStageToAutoAdvance = new SourceMigrationState?(SourceMigrationState.Created);
            break;
          case SourceMigrationState.BeginPrepareDatabase:
            jobStageToCheck = new MigrationJobStage?(MigrationJobStage.Source_PrepareSourceDatabase);
            nextStageToAutoAdvance = new SourceMigrationState?(SourceMigrationState.PrepareDatabase);
            break;
          case SourceMigrationState.BeginPrepareBlobs:
            jobStageToCheck = new MigrationJobStage?(MigrationJobStage.Source_PrepareBlobs);
            nextStageToAutoAdvance = new SourceMigrationState?(SourceMigrationState.PrepareBlobs);
            break;
          case SourceMigrationState.BeginUpdateLocation:
            jobStageToCheck = new MigrationJobStage?(MigrationJobStage.Source_UpdateLocation);
            nextStageToAutoAdvance = new SourceMigrationState?(SourceMigrationState.UpdatedLocation);
            break;
          case SourceMigrationState.BeginComplete:
            jobStageToCheck = new MigrationJobStage?(MigrationJobStage.Source_FinalizeMigrationOnSource);
            nextStageToAutoAdvance = new SourceMigrationState?(SourceMigrationState.Complete);
            break;
          case SourceMigrationState.BeginRollback:
            jobStageToCheck = new MigrationJobStage?(MigrationJobStage.Source_FinalizeMigrationOnSource_Rollback);
            nextStageToAutoAdvance = new SourceMigrationState?(SourceMigrationState.RolledBack);
            break;
        }
        requestContext.Trace(15288065, TraceLevel.Info, SourceHostMigrationService.s_area, SourceHostMigrationService.s_layer, string.Format("Checking background jobs for {0} from {1} to advance to {2}.\nMigrationId: {3}", (object) migrationEntry.State, (object) jobStageToCheck, (object) nextStageToAutoAdvance, (object) migrationEntry.MigrationId));
        requestContext.GetService<IHostMigrationBackgroundJobService>().CheckBackgroundMigrationJobs(requestContext, migrationEntry.MigrationId, jobStageToCheck, (Action<Guid, string>) ((migrationId, reason) =>
        {
          using (HostMigrationComponent component = requestContext.CreateComponent<HostMigrationComponent>())
            component.UpdateSourceMigration(migrationId, SourceMigrationState.Failed, reason);
        }), (Action<Guid>) (migrationId =>
        {
          if (!nextStageToAutoAdvance.HasValue)
            return;
          requestContext.Trace(15288057, TraceLevel.Info, SourceHostMigrationService.s_area, SourceHostMigrationService.s_layer, string.Format("Jobs completed successfully for {0} to advance to {1}.\nMigrationId: {2}", (object) jobStageToCheck, (object) nextStageToAutoAdvance, (object) migrationId));
          using (HostMigrationComponent component = requestContext.CreateComponent<HostMigrationComponent>())
            component.UpdateSourceMigration(migrationId, nextStageToAutoAdvance.Value, string.Empty);
        }));
        using (HostMigrationComponent component = requestContext.CreateComponent<HostMigrationComponent>())
          migrationEntry = component.GetSourceMigration(migrationEntry.MigrationId);
        return migrationEntry;
      }
      finally
      {
        requestContext.TraceLeave(15288115, SourceHostMigrationService.s_area, SourceHostMigrationService.s_layer, nameof (CheckBackgroundMigrationJobs));
      }
    }

    private static string CreateFirewallRuleName(string loginName, string targetHost) => string.Format("Data migration. Login: {0}. Target scale unit: {1}.", (object) loginName, (object) targetHost);

    private void CreateFirewallRules(
      IVssRequestContext requestContext,
      SourceHostMigration migrationEntry,
      TeamFoundationDatabaseCredential credential)
    {
      string operation = nameof (CreateFirewallRules);
      HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, operation, "Creating a firewall rule(s).");
      if (!requestContext.ExecutionEnvironment.IsCloudDeployment)
        HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, operation, "No need to create firewall rules, since this is not a cloud deployment.");
      else if (this.IsBulkMigrationCredential(credential))
      {
        HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, operation, "No need to create firewall rules, this is a bulk migration and the firewall rules have already been created.");
      }
      else
      {
        Guid serviceInstanceId = migrationEntry.TargetServiceInstanceId;
        string host = HostMigrationUtil.CreateHttpClient<SourceHostMigrationHttpClient>(requestContext, serviceInstanceId).BaseAddress.Host;
        HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, operation, "Target host: " + host);
        string name1 = credential.Name;
        HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, operation, "Login name: " + name1);
        string firewallRuleName = SourceHostMigrationService.CreateFirewallRuleName(name1, host);
        HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, operation, "Base rule name: " + firewallRuleName);
        IPAddress[] forFirewallRules = SourceHostMigrationService.GetIpAddressesForFirewallRules(requestContext, migrationEntry, operation, host, serviceInstanceId);
        using (SqlAzureFirewallComponent componentRaw = requestContext.GetService<TeamFoundationDatabaseManagementService>().GetDboConnectionInfo(requestContext, credential.DatabaseId).CreateComponentRaw<SqlAzureFirewallComponent>())
        {
          for (int index = 0; index < forFirewallRules.Length; ++index)
          {
            string str = forFirewallRules[index].ToString();
            string name2 = firewallRuleName;
            if (index > 0)
              name2 += index.ToString((IFormatProvider) CultureInfo.InvariantCulture);
            HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, operation, "Creating firewall rule. Rule name: " + name2 + ", IP address: " + str + ", database name: " + componentRaw.DataSource);
            componentRaw.SetDatabaseFirewallRule(name2, str, str);
          }
        }
      }
    }

    public static IPAddress[] GetIpAddressesForFirewallRules(
      IVssRequestContext requestContext,
      SourceHostMigration migrationEntry,
      string operation,
      string targetHost,
      Guid targetInstanceId)
    {
      MigrationHttpClient httpClient = HostMigrationUtil.CreateHttpClient<MigrationHttpClient>(requestContext, targetInstanceId);
      DeploymentInformation deploymentInformation = (DeploymentInformation) null;
      try
      {
        deploymentInformation = httpClient.GetDeploymentInformationAsync().SyncResult<DeploymentInformation>();
      }
      catch (VssResourceNotFoundException ex)
      {
      }
      if (deploymentInformation != null && deploymentInformation.OutboundVIPs != null && deploymentInformation.OutboundVIPs.Length != 0)
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        return ((IEnumerable<string>) deploymentInformation.OutboundVIPs).Select<string, IPAddress>(SourceHostMigrationService.\u003C\u003EO.\u003C0\u003E__Parse ?? (SourceHostMigrationService.\u003C\u003EO.\u003C0\u003E__Parse = new Func<string, IPAddress>(IPAddress.Parse))).ToArray<IPAddress>();
      }
      HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, operation, "Could not retrieve data from DeploymentInformation endpoint, falling back to DNS lookup");
      return Dns.GetHostAddresses(targetHost);
    }

    private void DeleteFirewallRules(
      IVssRequestContext requestContext,
      SourceHostMigration migrationEntry,
      TeamFoundationDatabaseCredential credential)
    {
      string operation = nameof (DeleteFirewallRules);
      HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, operation, "Deleting firewall rule(s).");
      if (!requestContext.ExecutionEnvironment.IsCloudDeployment)
      {
        HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, operation, "No need to delete firewall rules, since this is not a cloud deployment.");
      }
      else
      {
        Guid serviceInstanceId = migrationEntry.TargetServiceInstanceId;
        string name = credential.Name;
        HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, operation, "Login name: " + name);
        string host = HostMigrationUtil.CreateHttpClient<SourceHostMigrationHttpClient>(requestContext, serviceInstanceId).BaseAddress.Host;
        HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, operation, "Target host: " + host);
        string firewallRuleName = SourceHostMigrationService.CreateFirewallRuleName(name, host);
        HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, operation, "Base rule name: " + firewallRuleName);
        using (SqlAzureFirewallComponent componentRaw = requestContext.GetService<TeamFoundationDatabaseManagementService>().GetDboConnectionInfo(requestContext, credential.DatabaseId).CreateComponentRaw<SqlAzureFirewallComponent>())
        {
          HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, operation, "Getting database firewall rules");
          List<SqlAzureFirewallRule> dataseFirewallRules = componentRaw.GetDataseFirewallRules();
          HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, operation, string.Format("Found {0} rule(s).", (object) dataseFirewallRules.Count));
          for (int index = 0; index < dataseFirewallRules.Count; ++index)
          {
            SqlAzureFirewallRule azureFirewallRule = dataseFirewallRules[index];
            HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, operation, string.Format("Processing rule: {0}", (object) azureFirewallRule));
            if (azureFirewallRule.Name.StartsWith(firewallRuleName, StringComparison.Ordinal))
            {
              componentRaw.DeleteDatabaseFirewallRule(azureFirewallRule.Name);
              HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, operation, "Rule '" + azureFirewallRule.Name + "' has been deleted.");
            }
          }
        }
      }
    }

    private void CreateStandardFirewallRuleForInstance(
      IVssRequestContext requestContext,
      TeamFoundationDatabaseManagementService databaseManagementService,
      ITeamFoundationDatabaseProperties database,
      string targetInstance,
      ITFLogger logger)
    {
      if (!requestContext.ExecutionEnvironment.IsCloudDeployment)
      {
        logger.Info("No need to create firewall rule, since this is not a cloud deployment.");
      }
      else
      {
        ServiceInstance serviceInstance = HostMigrationUtil.GetServiceInstance(requestContext, targetInstance);
        logger.Info(string.Format("Resolved TargetInstance {0} to id {1}", (object) targetInstance, (object) serviceInstance.InstanceId));
        string name1 = serviceInstance.Name;
        DeploymentInformation deploymentInformation = HostMigrationUtil.CreateHttpClient<MigrationHttpClient>(requestContext, serviceInstance.InstanceId).GetDeploymentInformationAsync().SyncResult<DeploymentInformation>();
        if (deploymentInformation?.OutboundVIPs != null && deploymentInformation.OutboundVIPs.Length != 0)
        {
          logger.Info(string.Format("Creating firewall rules for instance {0} on database {1}", (object) serviceInstance.InstanceId, (object) database.DatabaseName));
          using (SqlAzureFirewallComponent componentRaw = databaseManagementService.GetDboConnectionInfo(requestContext, database.DatabaseId).CreateComponentRaw<SqlAzureFirewallComponent>())
          {
            for (int index = 0; index < deploymentInformation.OutboundVIPs.Length; ++index)
            {
              string name2 = string.Format("{0}ip{1}", (object) name1, (object) (index + 1));
              string outboundViP = deploymentInformation.OutboundVIPs[index];
              logger.Info("Creating rule " + name2 + " for ip address " + outboundViP);
              componentRaw.SetDatabaseFirewallRule(name2, outboundViP, outboundViP);
            }
          }
          logger.Info("Filewall rules created");
        }
        else
          logger.Error(string.Format("Could not retrieve OutboundVIPS for instance {0}", (object) serviceInstance.InstanceId));
      }
    }

    private void CreateBulkMigrationFirewallRules(
      IVssRequestContext requestContext,
      TeamFoundationDatabaseManagementService databaseManagementService,
      IEnumerable<ITeamFoundationDatabaseProperties> databases,
      string[] targetInstances,
      ITFLogger logger)
    {
      if (!requestContext.ExecutionEnvironment.IsCloudDeployment)
      {
        logger.Info("No need to create firewall rules, since this is not a cloud deployment.");
      }
      else
      {
        foreach (string targetInstance in targetInstances)
        {
          Guid instanceId;
          HostMigrationUtil.GetServiceInstance(requestContext, targetInstance, out instanceId, out Guid _);
          logger.Info(string.Format("Resolved TargetInstance {0} to id {1}", (object) targetInstance, (object) instanceId));
          this.CreateBulkMigrationFirewallRulesForInstance(requestContext, databaseManagementService, databases, instanceId, logger);
        }
      }
    }

    private void CreateBulkMigrationFirewallRulesForInstance(
      IVssRequestContext requestContext,
      TeamFoundationDatabaseManagementService databaseManagementService,
      IEnumerable<ITeamFoundationDatabaseProperties> databases,
      Guid instanceId,
      ITFLogger logger)
    {
      string str = string.Format("{0}{1}", (object) "BulkMigration-TargetInstance-", (object) instanceId);
      DeploymentInformation deploymentInformation = HostMigrationUtil.CreateHttpClient<MigrationHttpClient>(requestContext, instanceId).GetDeploymentInformationAsync().SyncResult<DeploymentInformation>();
      if (deploymentInformation?.OutboundVIPs != null && deploymentInformation.OutboundVIPs.Length != 0)
      {
        foreach (ITeamFoundationDatabaseProperties database in databases)
        {
          logger.Info(string.Format("Creating firewall rules for instance {0} on database {1}", (object) instanceId, (object) database.DatabaseName));
          using (SqlAzureFirewallComponent componentRaw = databaseManagementService.GetDboConnectionInfo(requestContext, database.DatabaseId).CreateComponentRaw<SqlAzureFirewallComponent>())
          {
            for (int index = 0; index < deploymentInformation.OutboundVIPs.Length; ++index)
            {
              string name = string.Format("{0}-{1}", (object) str, (object) index);
              string outboundViP = deploymentInformation.OutboundVIPs[index];
              logger.Info("Creating rule " + name + " for ip address " + outboundViP);
              componentRaw.SetDatabaseFirewallRule(name, outboundViP, outboundViP);
            }
          }
        }
        logger.Info("Filewall rules created");
      }
      else
        logger.Error(string.Format("Could not retrieve OutboundVIPS for instance {0}", (object) instanceId));
    }

    private void DeleteBulkMigrationFirewallRules(
      IVssRequestContext requestContext,
      ITFLogger logger)
    {
      if (!requestContext.ExecutionEnvironment.IsCloudDeployment)
      {
        logger.Info("No need to delete firewall rules, since this is not a cloud deployment.");
      }
      else
      {
        logger.Info("Cleaning up bulk migration firewall rules");
        TeamFoundationDatabaseManagementService service = requestContext.GetService<TeamFoundationDatabaseManagementService>();
        foreach (ITeamFoundationDatabaseProperties queryDatabase in service.QueryDatabases(requestContext, DatabaseManagementConstants.DefaultPartitionPoolName, true))
        {
          logger.Info("Cleaning up rules on database " + queryDatabase.DatabaseName);
          using (SqlAzureFirewallComponent componentRaw = service.GetDboConnectionInfo(requestContext, queryDatabase.DatabaseId).CreateComponentRaw<SqlAzureFirewallComponent>())
          {
            List<SqlAzureFirewallRule> dataseFirewallRules = componentRaw.GetDataseFirewallRules();
            logger.Info(string.Format("Found {0} rule(s).", (object) dataseFirewallRules.Count));
            for (int index = 0; index < dataseFirewallRules.Count; ++index)
            {
              SqlAzureFirewallRule azureFirewallRule = dataseFirewallRules[index];
              logger.Info(string.Format("Processing rule: {0}", (object) azureFirewallRule));
              if (azureFirewallRule.Name.StartsWith("BulkMigration-TargetInstance-", StringComparison.Ordinal))
              {
                componentRaw.DeleteDatabaseFirewallRule(azureFirewallRule.Name);
                logger.Info("Rule '" + azureFirewallRule.Name + "' has been deleted.");
              }
              else
                logger.Info("Leaving '" + azureFirewallRule.Name + "', it was not created for bulk migrations");
            }
          }
        }
      }
    }
  }
}
