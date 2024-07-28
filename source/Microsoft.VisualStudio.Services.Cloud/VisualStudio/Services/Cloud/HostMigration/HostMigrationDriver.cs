// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostMigration.HostMigrationDriver
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Cloud.HostMigration
{
  public class HostMigrationDriver
  {
    private List<HostMigrationDriver.MigrationWrapper> m_children;
    private bool m_includeChildren;
    private bool m_virtualApplicationHost;
    private Guid m_hostId;
    private string m_sourceInstanceName;
    private string m_targetInstanceName;
    private Guid m_sourceInstanceId;
    private Guid m_targetInstanceId;
    private int m_targetDatabase;
    private int m_hostsAffectedByTheMove;
    private bool m_autoRollback;
    private bool m_validateDatabaseCopy;
    private HostMigrationOptions m_options;
    private Action<string> m_onInfo;
    private Action<string> m_onWarn;
    private Action<string> m_onError;
    private SourceHostMigration m_sourceMigration;
    private TargetHostMigration m_targetMigration;
    private TargetMigrationState m_targetMigrationInitialState;
    private static readonly string s_kustoFormat = "\r\nServicingStepDetail \r\n| where JobId == '{0}'\r\n| project PreciseTimeStamp, ScaleUnit, RoleInstance,  OperationName, Message\r\n| order by PreciseTimeStamp asc";
    internal const string ForceRollbackFeatureFlag = "Microsoft.VisualStudio.Services.Cloud.HostMigration.ForceRollback";
    internal static readonly string s_area = "HostMigration";
    internal static readonly string s_layer = nameof (HostMigrationDriver);

    public HostMigrationDriver(
      IVssRequestContext requestContext,
      Guid hostId,
      string sourceInstanceName,
      string targetInstanceName,
      int targetDatabase,
      int hostsAffectedByTheMove,
      HostMigrationOptions options,
      bool includeChildren,
      bool autoRollback,
      bool validateDatabaseCopy,
      Action<string> onInfo,
      Action<string> onWarn,
      Action<string> onError)
    {
      this.m_children = new List<HostMigrationDriver.MigrationWrapper>();
      this.m_includeChildren = includeChildren;
      this.m_autoRollback = autoRollback;
      Guid instanceType1;
      HostMigrationUtil.GetServiceInstance(requestContext, sourceInstanceName, out this.m_sourceInstanceId, out instanceType1);
      Guid instanceType2;
      HostMigrationUtil.GetServiceInstance(requestContext, targetInstanceName, out this.m_targetInstanceId, out instanceType2);
      if (instanceType1 != instanceType2)
        throw new TeamFoundationServiceException(string.Format("Source and target instances must be the same type of service. Source: '{0}' type: '{1}'.  Target: '{2}' type: '{3}'.", (object) sourceInstanceName, (object) instanceType1, (object) targetInstanceName, (object) instanceType2));
      if (instanceType1 != ServiceInstanceTypes.SPS)
        this.m_virtualApplicationHost = !requestContext.GetService<IInstanceManagementService>().GetServiceInstance(requestContext, this.m_sourceInstanceId).GetProperty<TeamFoundationHostType>("Microsoft.TeamFoundation.Location.PhysicalHostTypesSupported", TeamFoundationHostType.Unknown).HasFlag((Enum) TeamFoundationHostType.Application);
      this.m_hostId = hostId;
      this.m_sourceInstanceName = sourceInstanceName;
      this.m_targetInstanceName = targetInstanceName;
      this.m_targetDatabase = targetDatabase;
      this.m_hostsAffectedByTheMove = hostsAffectedByTheMove;
      this.m_validateDatabaseCopy = validateDatabaseCopy;
      this.m_options = options;
      this.m_onInfo = onInfo ?? new Action<string>(this.NoOpLog);
      this.m_onWarn = onWarn ?? new Action<string>(this.NoOpLog);
      this.m_onError = onError ?? new Action<string>(this.NoOpLog);
    }

    private bool CheckIfSynchronousMigrationsAreComplete(TargetHostMigration targetMigration)
    {
      if (targetMigration == null)
        return false;
      return targetMigration.State == TargetMigrationState.CompletePendingBlobs || targetMigration.State == TargetMigrationState.BeginComplete || targetMigration.State == TargetMigrationState.Complete;
    }

    private bool CheckIfMigrationIsComplete(SourceHostMigration sourceMigration) => sourceMigration != null && sourceMigration.State == SourceMigrationState.Complete;

    private bool Rollback(
      IVssRequestContext requestContext,
      SourceHostMigrationHttpClient sourceClient,
      TargetHostMigrationHttpClient targetClient,
      bool force = false)
    {
      requestContext.TraceEnter(15288086, HostMigrationDriver.s_area, HostMigrationDriver.s_layer, nameof (Rollback));
      try
      {
        this.EnsureAllResourceMigrationJobsAreStopped(requestContext, sourceClient, targetClient);
        if (this.m_options.HasFlag((Enum) HostMigrationOptions.OnlineBlobCopy))
        {
          if (force)
          {
            this.m_onInfo("Forcing rollback.");
          }
          else
          {
            this.m_onInfo("Checking if rollback is possible.");
            if (this.CheckIfSynchronousMigrationsAreComplete(this.m_targetMigration))
            {
              this.m_onWarn(string.Format("The target migration on host {0} is complete.  Cannot rollback after database migration completes.", (object) this.m_targetMigration.HostProperties.Id));
              return false;
            }
            if (this.CheckIfMigrationIsComplete(this.m_sourceMigration))
            {
              this.m_onWarn(string.Format("The source migration on host {0} is complete.  Cannot rollback from a fully completed migration.", (object) this.m_sourceMigration.HostProperties.Id));
              return false;
            }
            foreach (HostMigrationDriver.MigrationWrapper child in this.m_children)
            {
              this.m_onInfo(string.Format("Child {0}", (object) child));
              if (this.CheckIfSynchronousMigrationsAreComplete(child.TargetMigration))
              {
                this.m_onWarn(string.Format("The child target migration on host {0} is complete.  Cannot rollback after database migration completes.", (object) child.TargetMigration.HostProperties.Id));
                return false;
              }
              if (this.CheckIfMigrationIsComplete(child.SourceMigration))
              {
                this.m_onWarn(string.Format("The child source migration on host {0} is complete.  Cannot rollback from a fully completed migration.", (object) child.SourceMigration.HostProperties.Id));
                return false;
              }
            }
          }
        }
        else if (this.CheckIfMigrationIsComplete(this.m_sourceMigration) && !this.m_sourceMigration.StorageOnly)
        {
          this.m_onWarn(string.Format("The source migration on host {0} is complete.  Cannot rollback from a fully completed migration.", (object) this.m_sourceMigration.HostProperties.Id));
          return false;
        }
        if (this.m_targetMigration != null)
          this.m_targetMigration = this.ValidateTargetRollback(targetClient, this.m_targetMigration, force);
        foreach (HostMigrationDriver.MigrationWrapper child in this.m_children)
        {
          if (child.TargetMigration != null)
            child.TargetMigration = this.ValidateTargetRollback(targetClient, child.TargetMigration, force);
        }
        this.m_onInfo("Current state before rollback");
        this.PrintMigrationData();
        this.m_sourceMigration = this.RollbackSource(requestContext, sourceClient, this.m_sourceMigration);
        foreach (HostMigrationDriver.MigrationWrapper child in this.m_children)
          child.SourceMigration = this.RollbackSource(requestContext, sourceClient, child.SourceMigration);
        foreach (HostMigrationDriver.MigrationWrapper child in this.m_children)
          child.TargetMigration = this.RollbackTarget(requestContext, targetClient, child.TargetMigration);
        this.m_targetMigration = this.RollbackTarget(requestContext, targetClient, this.m_targetMigration);
        return true;
      }
      finally
      {
        requestContext.TraceLeave(15288087, HostMigrationDriver.s_area, HostMigrationDriver.s_layer, nameof (Rollback));
      }
    }

    private TargetHostMigration ValidateTargetRollback(
      TargetHostMigrationHttpClient targetClient,
      TargetHostMigration targetMigration,
      bool force)
    {
      targetMigration = targetClient.GetMigrationEntry(targetMigration.MigrationId);
      bool flag = targetMigration.Options.HasFlag((Enum) HostMigrationOptions.LiveHost);
      if (!force && !flag && !targetMigration.StorageOnly && targetMigration.HostProperties.HostCreated && !targetMigration.HostProperties.IsVirtual && targetMigration.HostProperties.Status != TeamFoundationServiceHostStatus.Stopped && !targetMigration.HostProperties.IsInReadOnlyMode)
        throw new InvalidOperationException(string.Format("Cannot rollback migration, host {0} has already been started on the target", (object) targetMigration.HostProperties.Id));
      return targetMigration;
    }

    private void AutoRollback(
      IVssRequestContext requestContext,
      SourceHostMigrationHttpClient sourceClient,
      TargetHostMigrationHttpClient targetClient)
    {
      if (!this.m_autoRollback)
        return;
      this.m_onInfo("Rolling back. Current migration state:");
      this.PrintMigrationData();
      this.Rollback(requestContext, sourceClient, targetClient);
    }

    private SourceHostMigration RollbackSource(
      IVssRequestContext requestContext,
      SourceHostMigrationHttpClient sourceClient,
      SourceHostMigration sourceMigration)
    {
      requestContext.TraceEnter(15288088, HostMigrationDriver.s_area, HostMigrationDriver.s_layer, nameof (RollbackSource));
      try
      {
        if (sourceClient != null && sourceMigration != null)
        {
          this.m_onWarn(string.Format("Rolling back source migration {0}", (object) sourceMigration.MigrationId));
          sourceMigration = sourceClient.DeleteSourceMigrationAsync(sourceMigration.MigrationId).SyncResult<SourceHostMigration>();
          if (sourceMigration.State != SourceMigrationState.RolledBack)
            sourceMigration = this.WaitForStateChange(requestContext, sourceClient, sourceMigration);
        }
        return sourceMigration;
      }
      finally
      {
        requestContext.TraceLeave(15288089, HostMigrationDriver.s_area, HostMigrationDriver.s_layer, nameof (RollbackSource));
      }
    }

    private TargetHostMigration RollbackTarget(
      IVssRequestContext requestContext,
      TargetHostMigrationHttpClient targetClient,
      TargetHostMigration targetMigration)
    {
      requestContext.TraceEnter(15288090, HostMigrationDriver.s_area, HostMigrationDriver.s_layer, nameof (RollbackTarget));
      try
      {
        if (targetClient != null && targetMigration != null)
        {
          this.m_onWarn(string.Format("Rolling back target migration {0}", (object) targetMigration.MigrationId));
          targetMigration = targetClient.DeleteTargetMigrationAsync(targetMigration.MigrationId).SyncResult<TargetHostMigration>();
          if (targetMigration.State != TargetMigrationState.RolledBack)
            targetMigration = this.WaitForStateChange(requestContext, targetClient, targetMigration);
        }
        return targetMigration;
      }
      finally
      {
        requestContext.TraceLeave(15288091, HostMigrationDriver.s_area, HostMigrationDriver.s_layer, nameof (RollbackTarget));
      }
    }

    public SourceHostMigration WaitForStateChange(
      IVssRequestContext requestContext,
      SourceHostMigrationHttpClient sourceClient,
      SourceHostMigration sourceRequest)
    {
      SourceMigrationState state = sourceRequest.State;
      Stopwatch stopwatch = Stopwatch.StartNew();
      requestContext.Trace(15288073, TraceLevel.Info, HostMigrationDriver.s_area, HostMigrationDriver.s_layer, string.Format("Waiting for state change with current state {0}.\nMigrationId: {1}", (object) state, (object) sourceRequest.MigrationId));
      while (sourceRequest.State == state)
      {
        this.Sleep(requestContext, TimeSpan.FromSeconds(5.0));
        string message = string.Format("Waiting for migration state change. CurrentState: {0}. Elapsed: {1}", (object) sourceRequest.State, (object) stopwatch.Elapsed);
        this.m_onInfo(message);
        requestContext.Trace(15288074, TraceLevel.Info, HostMigrationDriver.s_area, HostMigrationDriver.s_layer, message);
        SourceHostMigration sourceHostMigration = sourceClient.GetMigrationEntryAsync(sourceRequest.MigrationId).SyncResult<SourceHostMigration>();
        if (sourceHostMigration.HostId != Guid.Empty && sourceHostMigration.State != SourceMigrationState.BeginCreate)
          sourceRequest = sourceHostMigration;
      }
      requestContext.Trace(15288062, TraceLevel.Info, HostMigrationDriver.s_area, HostMigrationDriver.s_layer, string.Format("Driver detected state change from {0} to {1}.\n{2}", (object) state, (object) sourceRequest?.State, (object) sourceRequest));
      return sourceRequest;
    }

    public TargetHostMigration WaitForStateChange(
      IVssRequestContext requestContext,
      TargetHostMigrationHttpClient targetClient,
      TargetHostMigration targetMigration)
    {
      TargetMigrationState state = targetMigration.State;
      Stopwatch stopwatch = Stopwatch.StartNew();
      while (targetMigration.State == state)
      {
        this.Sleep(requestContext, TimeSpan.FromSeconds(5.0));
        string message = string.Format("Waiting for migration state change. CurrentState: {0}. Elapsed: {1}", (object) targetMigration.State, (object) stopwatch.Elapsed);
        this.m_onInfo(message);
        requestContext.Trace(15288075, TraceLevel.Info, HostMigrationDriver.s_area, HostMigrationDriver.s_layer, message);
        TargetHostMigration response = targetClient.GetMigrationEntryAsync(targetMigration.MigrationId).SyncResult<TargetHostMigration>();
        bool flag1 = response.HostId == Guid.Empty;
        bool flag2 = response.State == TargetMigrationState.Failed;
        if (flag1 & flag2)
        {
          Exception exception = this.CheckForExceptionFromJobResultStatusMessage(targetMigration, response);
          if (exception != null)
            throw exception;
          return response;
        }
        if (!flag1 && response.State != TargetMigrationState.Create)
          targetMigration = response;
      }
      return targetMigration;
    }

    private Exception CheckForExceptionFromJobResultStatusMessage(
      TargetHostMigration targetMigration,
      TargetHostMigration response)
    {
      if (string.IsNullOrEmpty(response.StatusMessage))
        return (Exception) null;
      return response.StatusMessage.Contains("TF400579") ? (Exception) new DatabaseNotFoundException(targetMigration.TargetDatabaseId) : (Exception) new MigrationFailedException(response.StatusMessage);
    }

    public SourceHostMigration SourceMigration => this.m_sourceMigration;

    public TargetHostMigration TargetMigration
    {
      get => this.m_targetMigration;
      set => this.m_targetMigration = value;
    }

    private void LoadPreviousMigrations(
      IVssRequestContext requestContext,
      SourceHostMigrationHttpClient sourceClient,
      TargetHostMigrationHttpClient targetClient)
    {
      requestContext.TraceEnter(15288092, HostMigrationDriver.s_area, HostMigrationDriver.s_layer, nameof (LoadPreviousMigrations));
      try
      {
        this.m_sourceMigration = sourceClient.GetMigrationEntry(Guid.Empty, this.m_hostId.ToString("D"));
        if (this.m_sourceMigration == null)
        {
          this.m_onError("There is no existing source migration.  Continue switch is not necessary.");
          throw new InvalidOperationException("There is no existing source migration.  Continue switch is not necessary.");
        }
        this.m_onInfo(string.Format("Found a source migration for host: {0}", (object) this.m_hostId));
        this.m_targetMigration = targetClient.GetMigrationEntry(this.m_sourceMigration.MigrationId);
        if (this.m_targetMigration != null)
        {
          if (this.m_targetMigration.HostId == Guid.Empty)
          {
            this.m_onError("In memory target migration. This does not represent a database TargetHostMigration entry and means the CreateTargetMigration did not complete all retry attempts");
            this.m_targetMigration = (TargetHostMigration) null;
          }
          else
          {
            this.m_onInfo(string.Format("Found a target migration for host: {0}", (object) this.m_hostId));
            this.m_targetMigrationInitialState = this.m_targetMigration.State;
          }
        }
        if (!this.m_includeChildren || this.m_sourceMigration.HostProperties.ChildrenIds == null)
          return;
        foreach (Guid childrenId in this.m_sourceMigration.HostProperties.ChildrenIds)
        {
          SourceHostMigration migrationEntry = sourceClient.GetMigrationEntry(Guid.Empty, childrenId.ToString("D"));
          if (migrationEntry != null)
          {
            this.m_onInfo(string.Format("Found a source migration for child host: {0}", (object) childrenId));
            if (this.m_sourceMigration.MigrationId != migrationEntry.ParentMigrationId)
            {
              this.m_onInfo(string.Format("The child source migration's parent does not match this migration ({0}) {1}", (object) this.m_sourceMigration.MigrationId, (object) migrationEntry));
              if (migrationEntry.State != SourceMigrationState.Complete && migrationEntry.State != SourceMigrationState.RolledBack)
              {
                Guid guid = migrationEntry.ParentMigrationId == Guid.Empty ? migrationEntry.MigrationId : migrationEntry.ParentMigrationId;
                this.m_onError(string.Format("The preexisting migration {0} from a different Migration is incomplete (State {1} is neither {2} or {3}) it must be completed or rolled back", (object) guid, (object) migrationEntry.State, (object) SourceMigrationState.Complete, (object) SourceMigrationState.RolledBack));
                throw new TeamFoundationServicingException(string.Format("Preexisting migration {0} must be rolled back or completed", (object) guid));
              }
              this.m_onInfo(string.Format("The child migration status {0} allows it to be ignored", (object) migrationEntry.State));
            }
            else
            {
              HostMigrationDriver.MigrationWrapper child = new HostMigrationDriver.MigrationWrapper();
              child.HostId = childrenId;
              child.SourceMigration = migrationEntry;
              this.AddChildMigration(child);
              child.TargetMigration = targetClient.GetMigrationEntry(migrationEntry.MigrationId);
              if (child.TargetMigration != null)
              {
                if (child.TargetMigration.HostId == Guid.Empty)
                {
                  child.TargetMigration = (TargetHostMigration) null;
                }
                else
                {
                  this.m_onInfo(string.Format("Found a target migration for child host: {0}", (object) childrenId));
                  child.TargetMigrationInitialState = child.TargetMigration.State;
                }
              }
            }
          }
        }
      }
      finally
      {
        requestContext.TraceLeave(15288093, HostMigrationDriver.s_area, HostMigrationDriver.s_layer, nameof (LoadPreviousMigrations));
      }
    }

    public bool ExecuteMigration(IVssRequestContext requestContext, MigrationOption migrationOption)
    {
      requestContext.TraceEnter(15288019, HostMigrationDriver.s_area, HostMigrationDriver.s_layer, nameof (ExecuteMigration));
      try
      {
        bool flag1 = true;
        bool flag2 = migrationOption == MigrationOption.Rollback || migrationOption == MigrationOption.ForceRollback;
        if (flag2)
          this.m_autoRollback = true;
        bool blobOnly = migrationOption == MigrationOption.StorageOnly || this.m_options.HasFlag((Enum) HostMigrationOptions.PermanentStorageOnly);
        this.ValidateMigrationSettings(requestContext, migrationOption);
        using (SourceHostMigrationHttpClient httpClient1 = HostMigrationUtil.CreateHttpClient<SourceHostMigrationHttpClient>(requestContext, this.m_sourceInstanceId))
        {
          using (TargetHostMigrationHttpClient httpClient2 = HostMigrationUtil.CreateHttpClient<TargetHostMigrationHttpClient>(requestContext, this.m_targetInstanceId))
          {
            if (this.m_options.HasFlag((Enum) HostMigrationOptions.Resume) | flag2)
            {
              this.LoadPreviousMigrations(requestContext, httpClient1, httpClient2);
              this.m_onInfo("Loaded migration status:");
              this.PrintMigrationData();
              if (this.m_targetMigration != null)
              {
                this.m_options = this.m_options & (HostMigrationOptions.Resume | HostMigrationOptions.LiveHost | HostMigrationOptions.UsePreConnectedDb | HostMigrationOptions.NoPartitionCopy | HostMigrationOptions.MigrationCertSigning | HostMigrationOptions.MigrateAdHocJobs | HostMigrationOptions.PermanentStorageOnly) | this.m_targetMigration.Options & HostMigrationOptions.OnlineBlobCopy;
                this.m_onInfo(string.Format("Using online blob copy setting from previous migration: {0}. MigrationId={1}", (object) this.m_options.HasFlag((Enum) HostMigrationOptions.OnlineBlobCopy), (object) this.m_targetMigration.MigrationId));
                foreach (HostMigrationDriver.MigrationWrapper child in this.m_children)
                {
                  if (child.TargetMigration != null && child.TargetMigration.Options.HasFlag((Enum) HostMigrationOptions.OnlineBlobCopy) != this.m_options.HasFlag((Enum) HostMigrationOptions.OnlineBlobCopy))
                  {
                    this.m_onError(string.Format("Child {0} has different OnlineBlobCopy setting than {1}. MigrationId={2}", (object) child.HostId, (object) this.m_targetMigration.HostId, (object) this.m_targetMigration.MigrationId));
                    return false;
                  }
                }
              }
              if (this.m_options.HasFlag((Enum) HostMigrationOptions.Resume) && this.m_options.HasFlag((Enum) HostMigrationOptions.OnlineBlobCopy))
              {
                this.m_onInfo("Resuming migration. Locating any failed target migrations and rolling back.");
                if (this.m_targetMigration != null && this.m_targetMigration.State >= TargetMigrationState.Failed)
                {
                  this.m_onInfo("Failed migration has children. Performing a full rollback.");
                  if (!this.Rollback(requestContext, httpClient1, httpClient2))
                  {
                    this.m_onError("Rollback failed. Unable to resume migration. See log for details why the rollback failed.");
                    return false;
                  }
                  this.m_children.Clear();
                  this.m_sourceMigration = this.CreateSourceMigration(requestContext, httpClient1, this.m_hostId, this.m_hostsAffectedByTheMove, blobOnly, Guid.Empty);
                  this.m_targetMigration = (TargetHostMigration) null;
                }
                foreach (HostMigrationDriver.MigrationWrapper child in this.m_children)
                {
                  if (child.TargetMigration != null && child.TargetMigration.State >= TargetMigrationState.Failed)
                  {
                    if (child.SourceMigration == null)
                    {
                      this.m_onError("Unable to locate child source migration. Can not resume.");
                      return false;
                    }
                    this.m_onInfo(string.Format("Found failed child migration, rolling back. HostId={0}, State={1}", (object) child.HostId, (object) child.TargetMigration.State));
                    child.SourceMigration = this.RollbackSource(requestContext, httpClient1, child.SourceMigration);
                    child.TargetMigration = this.RollbackTarget(requestContext, httpClient2, child.TargetMigration);
                    child.SourceMigration = this.CreateChildSourceMigration(requestContext, httpClient1, child.HostId, this.m_sourceMigration.StorageOnly, this.m_sourceMigration.MigrationId);
                    child.TargetMigration = (TargetHostMigration) null;
                  }
                }
              }
              if (this.m_sourceMigration.State == SourceMigrationState.Failed && !flag2)
              {
                this.m_onError("The source migration request failed.  Please rollback and start a new migration.");
                return false;
              }
              if (this.m_targetMigration != null && this.m_targetMigration.State == TargetMigrationState.Failed && !flag2)
              {
                this.m_onError("The target migration request failed.  Please rollback and start a new migration.");
                return false;
              }
              this.m_onInfo("Current migration status:");
              this.PrintMigrationData();
              if (flag2)
                return this.Rollback(requestContext, httpClient1, httpClient2, migrationOption == MigrationOption.ForceRollback);
            }
            else
            {
              this.m_sourceMigration = this.CreateSourceMigration(requestContext, httpClient1, this.m_hostId, this.m_hostsAffectedByTheMove, blobOnly, Guid.Empty);
              requestContext.Trace(15288021, TraceLevel.Info, HostMigrationDriver.s_area, HostMigrationDriver.s_layer, string.Format("Created source migration.\n{0}", (object) this.m_sourceMigration));
              if (this.m_sourceMigration == null)
              {
                this.m_onInfo(string.Format("Host {0} not found but an assigned host instance mapping was found.  The mapping has been removed.", (object) this.m_hostId));
                this.m_onInfo("No migration needed.");
                return true;
              }
              if (this.m_sourceMigration.State == SourceMigrationState.Failed)
              {
                this.m_onError("Fail to create Source Migration successfully: " + this.m_sourceMigration.StatusMessage);
                this.AutoRollback(requestContext, httpClient1, httpClient2);
                return false;
              }
            }
            try
            {
              this.m_onInfo(string.Format("Starting migration. StorageOnly={0}, OnlineBlobCopy={1}, AutoRollback={2}.", (object) blobOnly, (object) this.m_options.HasFlag((Enum) HostMigrationOptions.OnlineBlobCopy), (object) this.m_autoRollback));
              this.DriveSourceMigration(requestContext, httpClient1, ref this.m_sourceMigration);
              if (this.StopAfterSourceMigration)
              {
                this.m_onInfo("Test stopping the migration after the Source Migration has been created");
                return false;
              }
              this.DriveTargetMigration(requestContext, httpClient1, httpClient2, this.m_sourceMigration, this.m_targetDatabase, ref this.m_targetMigration);
              if (this.m_includeChildren && this.m_sourceMigration.HostProperties.ChildrenIds != null && !this.CreateChildMigrations(requestContext, httpClient1))
              {
                this.m_onError("Fail to create one or more Child Source Migration successfully");
                this.AutoRollback(requestContext, httpClient1, httpClient2);
                return false;
              }
              foreach (HostMigrationDriver.MigrationWrapper child in this.m_children)
              {
                this.m_onInfo(string.Format("Driving migration on child. HostId={0}", (object) child.HostId));
                this.DriveSourceMigration(requestContext, httpClient1, ref child.SourceMigration);
                int targetDatabase = this.m_targetMigration.HostProperties.IsVirtual ? this.m_targetDatabase : this.m_targetMigration.TargetDatabaseId;
                this.DriveTargetMigration(requestContext, httpClient1, httpClient2, child.SourceMigration, targetDatabase, ref child.TargetMigration);
              }
              this.m_onInfo(string.Format("Updating locations. HostId={0}", (object) this.m_sourceMigration.HostId));
              this.UpdateLocations(requestContext, httpClient1, httpClient2, ref this.m_sourceMigration);
              foreach (HostMigrationDriver.MigrationWrapper child in this.m_children)
              {
                this.m_onInfo(string.Format("Updating locations on child. HostId={0}", (object) child.HostId));
                this.UpdateLocations(requestContext, httpClient1, httpClient2, ref child.SourceMigration);
              }
              this.m_onInfo(string.Format("Finishing target migration. HostId={0}", (object) this.m_targetMigration.HostId));
              this.FinishTargetMigration(requestContext, httpClient1, httpClient2, ref this.m_targetMigration);
              foreach (HostMigrationDriver.MigrationWrapper child in this.m_children)
              {
                this.m_onInfo(string.Format("Finishing target migration on child. HostId={0}", (object) child.HostId));
                this.FinishTargetMigration(requestContext, httpClient1, httpClient2, ref child.TargetMigration);
              }
              if (this.m_options.HasFlag((Enum) HostMigrationOptions.LiveHost))
              {
                this.m_onWarn("Disabling auto rollback for livehost migration now host is running on the target while we wait for blobmigration to complete");
                this.m_autoRollback = false;
              }
              if (this.m_options.HasFlag((Enum) HostMigrationOptions.Resume) && this.m_options.HasFlag((Enum) HostMigrationOptions.OnlineBlobCopy))
              {
                if (this.m_targetMigrationInitialState == TargetMigrationState.CompletePendingBlobs)
                  this.ResumeBlobCopies(requestContext, httpClient1, httpClient2, ref this.m_targetMigration);
                foreach (HostMigrationDriver.MigrationWrapper child in this.m_children)
                {
                  if (child.TargetMigrationInitialState == TargetMigrationState.CompletePendingBlobs)
                    this.ResumeBlobCopies(requestContext, httpClient1, httpClient2, ref child.TargetMigration);
                }
              }
              if (HostMigrationUtil.GetTestRegistryFlag(requestContext, FrameworkServerConstants.FailBlobCopyForOneMigration, (Action<string>) (x => HostMigrationLogger.LogInfo(requestContext, this.m_sourceMigration.MigrationId, DateTime.UtcNow, nameof (ExecuteMigration), x))))
              {
                requestContext.GetService<IVssRegistryService>().SetValue(requestContext, FrameworkServerConstants.FailBlobCopyForOneMigration, (object) null);
                throw new Exception("Failing blob migration for one migration");
              }
              if (!this.m_targetMigration.StorageOnly && this.m_options.HasFlag((Enum) HostMigrationOptions.OnlineBlobCopy))
              {
                this.m_onInfo(string.Format("Waiting for blob storage migrations to finish. HostId={0}", (object) this.m_targetMigration.HostId));
                this.WaitUntilBlobStorageMigrationsFinish(requestContext, httpClient2, ref this.m_targetMigration);
                foreach (HostMigrationDriver.MigrationWrapper child in this.m_children)
                {
                  this.m_onInfo(string.Format("Waiting for blob storage migrations to finish on child. HostId={0}", (object) child.HostId));
                  this.WaitUntilBlobStorageMigrationsFinish(requestContext, httpClient2, ref child.TargetMigration);
                }
              }
              this.m_onInfo(string.Format("Cleaning up target migration. HostId={0}", (object) this.m_targetMigration.HostId));
              this.CleanupTargetMigration(requestContext, httpClient1, httpClient2, ref this.m_targetMigration);
              foreach (HostMigrationDriver.MigrationWrapper child in this.m_children)
              {
                this.m_onInfo(string.Format("Cleaning up target migration on child. HostId={0}", (object) child.HostId));
                this.CleanupTargetMigration(requestContext, httpClient1, httpClient2, ref child.TargetMigration);
              }
              foreach (HostMigrationDriver.MigrationWrapper child in this.m_children)
              {
                this.m_onInfo(string.Format("Finishing source migration on child. HostId={0}", (object) child.HostId));
                this.FinishSourceMigration(requestContext, httpClient1, httpClient2, ref child.SourceMigration);
              }
              this.m_onInfo(string.Format("Finishing source migration. HostId={0}", (object) this.m_sourceMigration.HostId));
              this.FinishSourceMigration(requestContext, httpClient1, httpClient2, ref this.m_sourceMigration);
              if (this.m_sourceMigration.StorageOnly)
              {
                if (this.m_sourceMigration.State != SourceMigrationState.Complete || this.m_targetMigration.State == TargetMigrationState.Failed)
                {
                  this.m_onError("Blob Migrations did not complete as expected.");
                  flag1 = false;
                }
                else
                  this.m_onInfo("Source Blob Migrations have succeeded, target blob migrations are still running.");
              }
              else if (this.m_sourceMigration.State != SourceMigrationState.Complete || this.m_targetMigration.State != TargetMigrationState.Complete)
              {
                this.m_onError("Migrations did not complete as expected.");
                this.AutoRollback(requestContext, httpClient1, httpClient2);
                flag1 = false;
              }
              else
                this.m_onInfo("Host Migrations have succeeded.");
            }
            catch (Exception ex)
            {
              this.m_onInfo("Unhandled exception occured during migration: " + ex.ToReadableStackTrace());
              if (this.m_autoRollback)
              {
                this.m_onInfo("Attempting to rollback after unhandled exception");
                this.AutoRollback(requestContext, httpClient1, httpClient2);
                flag1 = false;
              }
              else
                throw;
            }
          }
        }
        this.PrintMigrationData();
        return flag1;
      }
      finally
      {
        requestContext.TraceLeave(15288020, HostMigrationDriver.s_area, HostMigrationDriver.s_layer, nameof (ExecuteMigration));
      }
    }

    private void WaitUntilBlobStorageMigrationsFinish(
      IVssRequestContext requestContext,
      TargetHostMigrationHttpClient targetClient,
      ref TargetHostMigration targetMigration)
    {
      requestContext.TraceEnter(15288094, HostMigrationDriver.s_area, HostMigrationDriver.s_layer, nameof (WaitUntilBlobStorageMigrationsFinish));
      try
      {
        while (true)
        {
          int num1 = 0;
          int num2 = 0;
          int num3 = 0;
          targetMigration = targetClient.GetMigrationEntryAsync(targetMigration.MigrationId).SyncResult<TargetHostMigration>();
          foreach (StorageMigration blobContainer in targetMigration.GetBlobContainers())
          {
            if (blobContainer.Status < StorageMigrationStatus.Failed)
              ++num1;
            if (blobContainer.Status == StorageMigrationStatus.Failed)
              ++num2;
            ++num3;
          }
          HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) targetMigration, nameof (WaitUntilBlobStorageMigrationsFinish), string.Format("Checking status of blob migration jobs. inProgress={0}, failed={1}, total={2}", (object) num1, (object) num2, (object) num3));
          if (num2 <= 0)
          {
            if (targetMigration.State != TargetMigrationState.Failed)
            {
              if (num1 > 0)
              {
                string message = string.Format("Waiting for {0} blob migrations to finish.", (object) num1);
                HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) targetMigration, nameof (WaitUntilBlobStorageMigrationsFinish), message);
                this.Sleep(requestContext, TimeSpan.FromSeconds(30.0));
              }
              else
                goto label_16;
            }
            else
              goto label_13;
          }
          else
            break;
        }
        string message1 = string.Format("Blob migration job failed. Migration entry details: {0}", (object) targetMigration);
        this.m_onWarn(message1);
        throw new DataMigrationFailedToCompleteException(message1);
label_13:
        string message2 = string.Format("Migration failed. Migration entry details: {0}", (object) targetMigration);
        this.m_onWarn(message2);
        throw new DataMigrationFailedToCompleteException(message2);
label_16:
        HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) targetMigration, nameof (WaitUntilBlobStorageMigrationsFinish), string.Format("Verified that blob migration has completed. MigrationId: {0}. HostId: {1}", (object) targetMigration.MigrationId, (object) targetMigration.HostId));
      }
      finally
      {
        requestContext.TraceLeave(15288095, HostMigrationDriver.s_area, HostMigrationDriver.s_layer, nameof (WaitUntilBlobStorageMigrationsFinish));
      }
    }

    private void FinishSourceMigration(
      IVssRequestContext requestContext,
      SourceHostMigrationHttpClient sourceClient,
      TargetHostMigrationHttpClient targetClient,
      ref SourceHostMigration sourceMigration)
    {
      requestContext.TraceEnter(15288096, HostMigrationDriver.s_area, HostMigrationDriver.s_layer, nameof (FinishSourceMigration));
      try
      {
        if (sourceMigration.State < SourceMigrationState.BeginComplete)
        {
          this.m_onInfo("Completing migrations on source");
          sourceMigration.State = SourceMigrationState.BeginComplete;
          sourceMigration = sourceClient.UpdateMigrationEntryAsync(sourceMigration).SyncResult<SourceHostMigration>();
        }
        if (sourceMigration.State == SourceMigrationState.BeginComplete)
          sourceMigration = this.WaitForStateChange(requestContext, sourceClient, sourceMigration);
        if (sourceMigration.State != SourceMigrationState.Complete)
        {
          string message = "Source host migration failed to complete.  Migration entry details: " + sourceMigration.ToString();
          this.m_onWarn(message);
          this.AutoRollback(requestContext, sourceClient, targetClient);
          throw new DataMigrationFailedToCompleteException(message);
        }
      }
      finally
      {
        requestContext.TraceLeave(15288097, HostMigrationDriver.s_area, HostMigrationDriver.s_layer, nameof (FinishSourceMigration));
      }
    }

    private void UpdateLocations(
      IVssRequestContext requestContext,
      SourceHostMigrationHttpClient sourceClient,
      TargetHostMigrationHttpClient targetClient,
      ref SourceHostMigration sourceMigration)
    {
      if (!sourceMigration.StorageOnly && sourceMigration.State < SourceMigrationState.BeginUpdateLocation)
      {
        this.m_onInfo(string.Format("Updating locations for host {0}", (object) this.m_hostId));
        sourceMigration.State = SourceMigrationState.BeginUpdateLocation;
        sourceMigration = sourceClient.UpdateMigrationEntryAsync(sourceMigration).SyncResult<SourceHostMigration>();
      }
      if (sourceMigration.StorageOnly || sourceMigration.State != SourceMigrationState.BeginUpdateLocation)
        return;
      sourceMigration = this.WaitForStateChange(requestContext, sourceClient, sourceMigration);
      if (!sourceMigration.StorageOnly && sourceMigration.State != SourceMigrationState.UpdatedLocation)
      {
        string message = string.Format("Source failed updating locations for migration id: {0}. Error: {1}", (object) sourceMigration.MigrationId, (object) sourceMigration.StatusMessage);
        this.m_onWarn(message);
        this.AutoRollback(requestContext, sourceClient, targetClient);
        throw new DataMigrationFailedToUpdateLocationException(message);
      }
    }

    private void DriveSourceMigration(
      IVssRequestContext deploymentContext,
      SourceHostMigrationHttpClient sourceClient,
      ref SourceHostMigration sourceMigration)
    {
      deploymentContext.TraceEnter(15288022, HostMigrationDriver.s_area, HostMigrationDriver.s_layer, nameof (DriveSourceMigration));
      try
      {
        if (!sourceMigration.StorageOnly && sourceMigration.State < SourceMigrationState.PrepareDatabase)
        {
          this.m_onInfo("Preparing host for migration, getting database information.");
          sourceMigration.State = SourceMigrationState.BeginPrepareDatabase;
          sourceMigration = sourceClient.UpdateMigrationEntryAsync(sourceMigration).SyncResult<SourceHostMigration>();
          sourceMigration = this.WaitForStateChange(deploymentContext, sourceClient, sourceMigration);
        }
        if (sourceMigration.State == SourceMigrationState.Created || sourceMigration.State == SourceMigrationState.PrepareDatabase)
        {
          this.m_onInfo("Preparing host for migration, getting blob storage information.");
          sourceMigration.State = SourceMigrationState.BeginPrepareBlobs;
          sourceMigration = sourceClient.UpdateMigrationEntryAsync(sourceMigration).SyncResult<SourceHostMigration>();
          sourceMigration = this.WaitForStateChange(deploymentContext, sourceClient, sourceMigration);
        }
        if (sourceMigration.State != SourceMigrationState.PrepareBlobs || sourceMigration.StorageMigrations != null && sourceMigration.ShardingInfo != null)
          return;
        this.m_onInfo("It appears as this migration was restarted as a result StorageMigrations and/or ShardInfo is not populated, requesting it now");
        sourceMigration.State = SourceMigrationState.BeginPrepareBlobs;
        sourceMigration = sourceClient.UpdateMigrationEntryAsync(sourceMigration).SyncResult<SourceHostMigration>();
        sourceMigration = this.WaitForStateChange(deploymentContext, sourceClient, sourceMigration);
      }
      finally
      {
        deploymentContext.TraceLeave(15288023, HostMigrationDriver.s_area, HostMigrationDriver.s_layer, nameof (DriveSourceMigration));
      }
    }

    private SourceHostMigration CreateChildSourceMigration(
      IVssRequestContext requestContext,
      SourceHostMigrationHttpClient sourceClient,
      Guid hostId,
      bool blobOnly,
      Guid parentMigrationId)
    {
      return this.CreateSourceMigration(requestContext, sourceClient, hostId, 1, blobOnly, parentMigrationId);
    }

    private SourceHostMigration CreateSourceMigration(
      IVssRequestContext requestContext,
      SourceHostMigrationHttpClient sourceClient,
      Guid hostId,
      int hostsAffectedByTheMove,
      bool blobOnly,
      Guid parentMigrationId)
    {
      requestContext.TraceEnter(15288024, HostMigrationDriver.s_area, HostMigrationDriver.s_layer, nameof (CreateSourceMigration));
      try
      {
        this.m_onInfo(string.Format("Creating source migration request for host {0}", (object) hostId));
        SourceHostMigration migrationRequest = new SourceHostMigration()
        {
          MigrationId = Guid.NewGuid(),
          TargetServiceInstanceId = this.m_targetInstanceId,
          HostProperties = new MigrationHostProperties()
          {
            Id = hostId
          },
          StorageOnly = blobOnly,
          ParentMigrationId = parentMigrationId,
          Options = this.m_options,
          HostsAffectedByTheMove = hostsAffectedByTheMove
        };
        using (HostMigrationQueueComponent component = requestContext.CreateComponent<HostMigrationQueueComponent>())
        {
          HostMigrationRequest queueRequest = component.GetQueueRequest(hostId);
          if (queueRequest != null)
          {
            if (queueRequest.MigrationId != Guid.Empty)
              throw new TeamFoundationServicingException("The migration is already being processed");
            this.m_onInfo("Found a request in Migration.tbl_HostMigrationRequest, updating the migration id.");
            component.SetQueueRequestMigrationId(hostId, migrationRequest.MigrationId);
          }
        }
        this.m_onInfo(string.Empty);
        this.m_onInfo("Use the following Kusto query to view migration details:");
        this.m_onInfo("Migration query:");
        this.m_onInfo(string.Format(HostMigrationDriver.s_kustoFormat, (object) migrationRequest.MigrationId.ToString("D").ToLower()));
        this.m_onInfo(string.Empty);
        SourceHostMigration sourceRequest = sourceClient.CreateMigrationEntryAsync(migrationRequest).SyncResult<SourceHostMigration>();
        SourceHostMigration sourceMigration = this.WaitForStateChange(requestContext, sourceClient, sourceRequest);
        requestContext.Trace(15288026, TraceLevel.Info, HostMigrationDriver.s_area, HostMigrationDriver.s_layer, string.Format("Created source migration async.\n{0}", (object) sourceMigration));
        return sourceMigration;
      }
      finally
      {
        requestContext.TraceLeave(15288025, HostMigrationDriver.s_area, HostMigrationDriver.s_layer, nameof (CreateSourceMigration));
      }
    }

    private void FinishTargetMigration(
      IVssRequestContext requestContext,
      SourceHostMigrationHttpClient sourceClient,
      TargetHostMigrationHttpClient targetClient,
      ref TargetHostMigration targetMigration)
    {
      requestContext.TraceEnter(15288098, HostMigrationDriver.s_area, HostMigrationDriver.s_layer, nameof (FinishTargetMigration));
      try
      {
        HostMigrationUtil.CheckForFaultInjection(requestContext, (IMigrationEntry) this.m_sourceMigration, nameof (HostMigrationDriver), nameof (FinishTargetMigration));
        if (!targetMigration.StorageOnly && targetMigration.State < TargetMigrationState.BeginCompletePendingBlobs)
        {
          this.m_onInfo("Completing migrations on target");
          targetMigration.State = TargetMigrationState.BeginCompletePendingBlobs;
          targetMigration = targetClient.UpdateMigrationEntryAsync(targetMigration).SyncResult<TargetHostMigration>();
        }
        if (!targetMigration.StorageOnly && targetMigration.State == TargetMigrationState.BeginCompletePendingBlobs)
          targetMigration = this.WaitForStateChange(requestContext, targetClient, targetMigration);
        if (!targetMigration.StorageOnly && (targetMigration.State < TargetMigrationState.CompletePendingBlobs || targetMigration.State >= TargetMigrationState.Failed))
        {
          string message = "Target host migration failed to complete.  Migration entry details: " + targetMigration.ToString();
          this.m_onWarn(message);
          this.AutoRollback(requestContext, sourceClient, targetClient);
          throw new DataMigrationFailedToCompleteException(message);
        }
      }
      finally
      {
        requestContext.TraceLeave(15288099, HostMigrationDriver.s_area, HostMigrationDriver.s_layer, nameof (FinishTargetMigration));
      }
    }

    private void CleanupTargetMigration(
      IVssRequestContext requestContext,
      SourceHostMigrationHttpClient sourceClient,
      TargetHostMigrationHttpClient targetClient,
      ref TargetHostMigration targetMigration)
    {
      requestContext.TraceEnter(15288100, HostMigrationDriver.s_area, HostMigrationDriver.s_layer, nameof (CleanupTargetMigration));
      try
      {
        if (!targetMigration.StorageOnly && targetMigration.State < TargetMigrationState.BeginComplete)
        {
          this.m_onInfo("Cleaning up after migrations on target");
          targetMigration.State = TargetMigrationState.BeginComplete;
          targetMigration = targetClient.UpdateMigrationEntryAsync(targetMigration).SyncResult<TargetHostMigration>();
        }
        if (!targetMigration.StorageOnly && targetMigration.State == TargetMigrationState.BeginComplete)
          targetMigration = this.WaitForStateChange(requestContext, targetClient, targetMigration);
        this.m_onInfo("----------" + targetMigration.State.ToString());
        this.m_onInfo(targetMigration?.ToString() ?? "");
        if (!targetMigration.StorageOnly && targetMigration.State != TargetMigrationState.Complete)
        {
          string message = "Target host cleanup failed to complete.  See log for manual cleanup instructions.  Migration entry details: " + targetMigration.ToString();
          this.m_onWarn(message);
          this.m_onInfo("All migrations were successful but the target host cleanup failed.  The following cleanup tasks must be performed manually:");
          this.m_onInfo(string.Format("1. Remove any entries in the strongbox drawer '{0}' with the migration id '{1}'.", (object) FrameworkServerConstants.HostMigrationSecretsDrawerName, (object) targetMigration.MigrationId));
          this.m_onInfo(string.Format("2. Change the service host substatus from '{0}' to '{1} for host '{2}'.", (object) ServiceHostSubStatus.Migrating, (object) ServiceHostSubStatus.None, (object) targetMigration.HostId));
          throw new DataMigrationFailedToCompleteException(message);
        }
      }
      finally
      {
        requestContext.TraceLeave(15288101, HostMigrationDriver.s_area, HostMigrationDriver.s_layer, nameof (CleanupTargetMigration));
      }
    }

    private void ResumeBlobCopies(
      IVssRequestContext requestContext,
      SourceHostMigrationHttpClient sourceClient,
      TargetHostMigrationHttpClient targetClient,
      ref TargetHostMigration targetMigration)
    {
      if (!targetMigration.StorageOnly && targetMigration.State == TargetMigrationState.CompletePendingBlobs)
      {
        this.m_onInfo("Resuming blob migrations on target.");
        targetMigration.State = TargetMigrationState.ResumeBlobCopy;
        targetMigration = targetClient.UpdateMigrationEntryAsync(targetMigration).SyncResult<TargetHostMigration>();
      }
      if (!targetMigration.StorageOnly && targetMigration.State == TargetMigrationState.ResumeBlobCopy)
        targetMigration = this.WaitForStateChange(requestContext, targetClient, targetMigration);
      if (!targetMigration.StorageOnly && (targetMigration.State < TargetMigrationState.CompletePendingBlobs || targetMigration.State >= TargetMigrationState.Failed))
      {
        string message = "Failed to resume blob copies.  Migration entry details: " + targetMigration.ToString();
        this.m_onWarn(message);
        throw new DataMigrationFailedToCompleteException(message);
      }
    }

    internal void AddChildMigration(HostMigrationDriver.MigrationWrapper child)
    {
      if (child == null)
        return;
      this.m_children.Add(child);
    }

    private bool CreateChildMigrations(
      IVssRequestContext requestContext,
      SourceHostMigrationHttpClient sourceClient)
    {
      requestContext.TraceEnter(15288059, HostMigrationDriver.s_area, HostMigrationDriver.s_layer, nameof (CreateChildMigrations));
      try
      {
        HashSet<Guid> guidSet = new HashSet<Guid>(this.m_children.Select<HostMigrationDriver.MigrationWrapper, Guid>((Func<HostMigrationDriver.MigrationWrapper, Guid>) (c => c.HostId)));
        foreach (Guid childrenId in this.m_sourceMigration.HostProperties.ChildrenIds)
        {
          if (!guidSet.Contains(childrenId))
          {
            this.m_onInfo(string.Format("Starting a migration for child host: {0}", (object) childrenId));
            HostMigrationDriver.MigrationWrapper child = new HostMigrationDriver.MigrationWrapper();
            child.HostId = childrenId;
            this.AddChildMigration(child);
            child.SourceMigration = this.CreateChildSourceMigration(requestContext, sourceClient, childrenId, this.m_sourceMigration.StorageOnly, this.m_sourceMigration.MigrationId);
            requestContext.Trace(15288027, TraceLevel.Info, HostMigrationDriver.s_area, HostMigrationDriver.s_layer, string.Format("Created child source migration.\n{0}", (object) child?.SourceMigration));
            if (child.SourceMigration != null && child.SourceMigration.State == SourceMigrationState.Failed)
            {
              this.m_onError(string.Format("Failed to create Migration for {0}: {1}", (object) childrenId, (object) child.SourceMigration.StatusMessage));
              return false;
            }
          }
        }
        return true;
      }
      finally
      {
        requestContext.TraceLeave(15288060, HostMigrationDriver.s_area, HostMigrationDriver.s_layer, nameof (CreateChildMigrations));
      }
    }

    protected void NoOpLog(string message)
    {
    }

    private void DriveTargetMigration(
      IVssRequestContext requestContext,
      SourceHostMigrationHttpClient sourceClient,
      TargetHostMigrationHttpClient targetClient,
      SourceHostMigration sourceMigration,
      int targetDatabase,
      ref TargetHostMigration targetMigration)
    {
      requestContext.TraceEnter(15288028, HostMigrationDriver.s_area, HostMigrationDriver.s_layer, nameof (DriveTargetMigration));
      try
      {
        if (targetMigration == null)
        {
          try
          {
            if (sourceMigration.State == SourceMigrationState.Failed)
              throw new TeamFoundationServicingException("Source host migration request creation failed");
            targetMigration = this.CreateTargetMigration(requestContext, sourceClient, targetClient, sourceMigration, targetDatabase);
            requestContext.Trace(15288030, TraceLevel.Info, HostMigrationDriver.s_area, HostMigrationDriver.s_layer, string.Format("Created target migration.\n{0}", (object) targetMigration));
          }
          catch (Exception ex)
          {
            requestContext.TraceException(15288011, TraceLevel.Error, HostMigrationDriver.s_area, HostMigrationDriver.s_layer, ex, string.Format("Migration={0},Exception={1}", (object) sourceMigration, (object) ex));
            this.m_onError(string.Format("Error creating target migration request for host {0}. Exception details: {1}", (object) sourceMigration.HostProperties.Id, (object) ex.ToReadableStackTrace()));
            this.AutoRollback(requestContext, sourceClient, targetClient);
            throw;
          }
          if (targetMigration.State == TargetMigrationState.Failed)
          {
            this.m_onWarn("Target migration was not created.");
            this.AutoRollback(requestContext, sourceClient, targetClient);
            throw new TeamFoundationServicingException("Target host migration request failed to create.");
          }
        }
        if (targetMigration.State < TargetMigrationState.BeginCopyJobs)
        {
          this.m_onInfo(string.Format("Invoking resource migrations for {0} host.", (object) targetMigration.HostProperties.HostType));
          targetMigration.State = TargetMigrationState.BeginCopyJobs;
          targetMigration = targetClient.UpdateMigrationEntryAsync(targetMigration).SyncResult<TargetHostMigration>();
        }
        if (!targetMigration.StorageOnly && targetMigration.State < TargetMigrationState.CopyJobsComplete)
        {
          Stopwatch stopwatch = Stopwatch.StartNew();
          TimeSpan timeout = TimeSpan.FromSeconds(5.0);
          while (targetMigration.State == TargetMigrationState.BeginCopyJobs)
          {
            Thread.Sleep(timeout);
            this.m_onInfo(string.Format("Waiting for migration state change for {0} host. CurrentState: {1}. Elapsed: {2}", (object) sourceMigration.HostProperties.HostType, (object) targetMigration.State, (object) stopwatch.Elapsed));
            targetMigration = targetClient.GetMigrationEntryAsync(targetMigration.MigrationId).SyncResult<TargetHostMigration>();
            requestContext.Trace(15288031, TraceLevel.Info, HostMigrationDriver.s_area, HostMigrationDriver.s_layer, string.Format("Got target migration entry.\n{0}", (object) targetMigration));
            if (stopwatch.Elapsed > TimeSpan.FromMinutes(1.0) && stopwatch.Elapsed < TimeSpan.FromMinutes(5.0))
              timeout = TimeSpan.FromSeconds(15.0);
            else if (stopwatch.Elapsed > TimeSpan.FromMinutes(10.0))
              timeout = TimeSpan.FromMinutes(1.0);
          }
        }
        if (targetMigration.State == TargetMigrationState.Failed)
        {
          this.m_onWarn("Target migration copy jobs did not complete.");
          this.m_onWarn(targetMigration.ToString());
          this.AutoRollback(requestContext, sourceClient, targetClient);
          throw new DataMigrationResourceMigrationJobsFailedException("Target resource migration jobs failed.");
        }
      }
      finally
      {
        requestContext.TraceLeave(15288029, HostMigrationDriver.s_area, HostMigrationDriver.s_layer, nameof (DriveTargetMigration));
      }
    }

    private TargetHostMigration CreateTargetMigration(
      IVssRequestContext requestContext,
      SourceHostMigrationHttpClient sourceClient,
      TargetHostMigrationHttpClient targetClient,
      SourceHostMigration sourceMigration,
      int targetDatabase)
    {
      requestContext.TraceEnter(15288032, HostMigrationDriver.s_area, HostMigrationDriver.s_layer, nameof (CreateTargetMigration));
      try
      {
        requestContext.Trace(15288061, TraceLevel.Info, HostMigrationDriver.s_area, HostMigrationDriver.s_layer, string.Format("Creating target from source with following information.\n{0}", (object) sourceMigration));
        this.m_onInfo(string.Format("Creating target migration request for {0} host with id: {1}", (object) sourceMigration.HostProperties.HostType, (object) sourceMigration.HostProperties.Id));
        TargetHostMigration migrationRequest = TargetHostMigration.FromSourceMigration(sourceMigration, this.m_sourceInstanceId);
        requestContext.Trace(15288016, TraceLevel.Info, HostMigrationDriver.s_area, HostMigrationDriver.s_layer, string.Format("Created target migration from source.\n{0}", (object) migrationRequest));
        migrationRequest.PerformDatabaseCopyValidation = this.m_validateDatabaseCopy;
        if (targetDatabase != DatabaseManagementConstants.InvalidDatabaseId)
        {
          this.m_onInfo(string.Format("TargetDatabaseId is set to: {0}", (object) targetDatabase));
          migrationRequest.TargetDatabaseId = targetDatabase;
        }
        int num = 3;
        while (true)
        {
          --num;
          try
          {
            TargetHostMigration targetMigration1 = targetClient.CreateMigrationEntryAsync(migrationRequest).SyncResult<TargetHostMigration>();
            TargetHostMigration targetMigration2 = this.WaitForStateChange(requestContext, targetClient, targetMigration1);
            requestContext.Trace(15288017, TraceLevel.Info, HostMigrationDriver.s_area, HostMigrationDriver.s_layer, string.Format("Target migration entry created.\n{0}", (object) targetMigration2));
            return targetMigration2;
          }
          catch (DataMigrationFailedToConnectToRemoteResourceException ex) when (num > 0)
          {
            this.m_onInfo(string.Format("Encountered an Exception while attempting to create Target Migration will attempt to update the source and try again. Remaining Attempts: {0}", (object) num));
            this.m_onInfo(string.Format("Exception {0}", (object) ex));
            this.Sleep(requestContext, TimeSpan.FromSeconds(20.0));
            Guid migrationId = sourceMigration.MigrationId;
            sourceMigration = sourceClient.GetMigrationEntryAsync(migrationId).SyncResult<SourceHostMigration>();
            requestContext.Trace(15288034, TraceLevel.Info, HostMigrationDriver.s_area, HostMigrationDriver.s_layer, string.Format("Target migration entry created async.\n{0}", (object) sourceMigration));
            migrationRequest.ConnectionInfo = sourceMigration != null ? sourceMigration.ConnectionInfo : throw new TeamFoundationServicingException(string.Format("Failed to find Source Migration with id {0}.", (object) migrationId));
          }
        }
      }
      finally
      {
        requestContext.TraceLeave(15288033, HostMigrationDriver.s_area, HostMigrationDriver.s_layer, nameof (CreateTargetMigration));
      }
    }

    private void PrintMigrationData()
    {
      if (this.SourceMigration != null)
      {
        this.m_onInfo(string.Empty);
        this.m_onInfo(string.Format("Source - Host ID {0}", (object) this.m_hostId));
        this.m_onInfo(this.SourceMigration.ToString());
      }
      if (this.TargetMigration != null)
      {
        this.m_onInfo(string.Empty);
        this.m_onInfo(string.Format("Target - Host ID {0}", (object) this.m_hostId));
        this.m_onInfo(this.TargetMigration.ToString());
      }
      foreach (HostMigrationDriver.MigrationWrapper child in this.Children)
      {
        if (child.SourceMigration != null)
        {
          this.m_onInfo(string.Empty);
          this.m_onInfo(string.Format("Child Source - Host ID {0}", (object) child.HostId));
          this.m_onInfo(child.SourceMigration.ToString());
        }
        if (child.TargetMigration != null)
        {
          this.m_onInfo(string.Empty);
          this.m_onInfo(string.Format("Child Target - Host ID {0}", (object) child.HostId));
          this.m_onInfo(child.TargetMigration.ToString());
        }
      }
    }

    internal virtual void Sleep(IVssRequestContext requestContext, TimeSpan sleepTime)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      this.m_onInfo(string.Format("Sleeping for {0}", (object) sleepTime));
      requestContext.CancellationToken.WaitHandle.WaitOne(sleepTime);
      if (requestContext.IsCanceled)
        throw new RequestCanceledException("The request context was cancelled");
    }

    internal virtual void ValidateMigrationSettings(
      IVssRequestContext requestContext,
      MigrationOption migrationOption)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (migrationOption == MigrationOption.ForceRollback && !requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Cloud.HostMigration.ForceRollback"))
      {
        this.m_onError("Attempting to use force rollback but this feature is not enabled.");
        this.m_onError("Please consult with the R3 team (rrr@microsoft.com) prior to using this feature.");
        throw new TeamFoundationServicingException("Attempting to use force rollback but this feature is not enabled.");
      }
      if (this.m_options.HasFlag((Enum) HostMigrationOptions.PermanentStorageOnly) && migrationOption != MigrationOption.Default)
        throw new TeamFoundationServicingException(string.Format("The {0} option cannot be used in conjunction with a {1} migration request", (object) migrationOption, (object) HostMigrationOptions.PermanentStorageOnly));
    }

    private void EnsureAllResourceMigrationJobsAreStopped(
      IVssRequestContext requestContext,
      SourceHostMigrationHttpClient sourceClient,
      TargetHostMigrationHttpClient targetClient)
    {
      requestContext.TraceEnter(15288102, HostMigrationDriver.s_area, HostMigrationDriver.s_layer, nameof (EnsureAllResourceMigrationJobsAreStopped));
      try
      {
        using (MigrationHttpClient httpClient1 = HostMigrationUtil.CreateHttpClient<MigrationHttpClient>(requestContext, this.m_sourceInstanceId))
        {
          using (MigrationHttpClient httpClient2 = HostMigrationUtil.CreateHttpClient<MigrationHttpClient>(requestContext, this.m_targetInstanceId))
          {
            this.StopAndWaitForResourceMigrationJobsToComplete(requestContext, httpClient1, (IMigrationEntry) this.m_sourceMigration);
            this.StopAndWaitForResourceMigrationJobsToComplete(requestContext, httpClient2, (IMigrationEntry) this.m_targetMigration);
            if (this.m_sourceMigration != null)
              this.m_sourceMigration = sourceClient.GetMigrationEntry(this.m_sourceMigration.MigrationId);
            if (this.m_targetMigration != null)
              this.m_targetMigration = targetClient.GetMigrationEntry(this.m_targetMigration.MigrationId);
            foreach (HostMigrationDriver.MigrationWrapper child in this.m_children)
            {
              if (child.SourceMigration != null)
              {
                this.StopAndWaitForResourceMigrationJobsToComplete(requestContext, httpClient1, (IMigrationEntry) child.SourceMigration);
                child.SourceMigration = sourceClient.GetMigrationEntry(child.SourceMigration.MigrationId);
              }
              if (child.TargetMigration != null)
              {
                this.StopAndWaitForResourceMigrationJobsToComplete(requestContext, httpClient2, (IMigrationEntry) child.TargetMigration);
                child.TargetMigration = targetClient.GetMigrationEntry(child.TargetMigration.MigrationId);
              }
            }
          }
        }
      }
      finally
      {
        requestContext.TraceLeave(15288103, HostMigrationDriver.s_area, HostMigrationDriver.s_layer, nameof (EnsureAllResourceMigrationJobsAreStopped));
      }
    }

    private void StopAndWaitForResourceMigrationJobsToComplete(
      IVssRequestContext requestContext,
      MigrationHttpClient httpClient,
      IMigrationEntry migrationEntry)
    {
      if (migrationEntry == null)
        return;
      this.m_onInfo(string.Format("Stopping background migration jobs for migration {0}", (object) migrationEntry.MigrationId));
      httpClient.StopMigrationJobsAsync(migrationEntry.MigrationId).SyncResult();
      this.m_onInfo(string.Format("Polling until background migration jobs for migration {0} have completed", (object) migrationEntry.MigrationId));
      while (httpClient.GetMigrationJobInformationAsync(migrationEntry.MigrationId).SyncResult<MigrationJobInformation>().HasRunningMigrationJobs)
      {
        this.m_onInfo(string.Format("Migration jobs are still running for migration {0}", (object) migrationEntry.MigrationId));
        this.Sleep(requestContext, TimeSpan.FromSeconds(5.0));
      }
      this.m_onInfo(string.Format("Background migration jobs for migration {0} have completed.", (object) migrationEntry.MigrationId));
    }

    internal List<HostMigrationDriver.MigrationWrapper> Children => this.m_children;

    protected virtual bool StopAfterSourceMigration => false;

    internal virtual TimeSpan MigrationWaitForRollback => TimeSpan.FromMinutes(10.0);

    internal class MigrationWrapper
    {
      internal SourceHostMigration SourceMigration;
      internal TargetHostMigration TargetMigration;
      internal TargetMigrationState TargetMigrationInitialState;
      internal Guid HostId;
    }
  }
}
