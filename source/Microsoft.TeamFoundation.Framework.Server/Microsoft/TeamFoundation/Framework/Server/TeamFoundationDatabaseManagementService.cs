// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationDatabaseManagementService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server.DatabaseReplication;
using Microsoft.VisualStudio.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class TeamFoundationDatabaseManagementService : 
    VssBaseService,
    ITeamFoundationDatabaseManagementService,
    IVssFrameworkService
  {
    private VssRefreshCache<TeamFoundationDatabaseManagementService.TeamFoundationDatabaseThrottleConfiguration> m_throttleConfigurationCache = new VssRefreshCache<TeamFoundationDatabaseManagementService.TeamFoundationDatabaseThrottleConfiguration>(TimeSpan.FromSeconds(DatabaseManagementConstants.PerfCacheRefreshInterval), (Func<IVssRequestContext, TeamFoundationDatabaseManagementService.TeamFoundationDatabaseThrottleConfiguration>) (deploymentContext => new TeamFoundationDatabaseManagementService.TeamFoundationDatabaseThrottleConfiguration(deploymentContext)), true);
    internal const string LogSizeKpiNameFormat = "{0}LogSizeInMb";
    internal const string LogPercentKpiNameFormat = "{0}LogSpaceUsedPercent";
    internal const int LogDetailErrorTracePoint = 314989846;
    private const string c_DefaultPartitionDatabaseCapacityLinkTemplate = "http://vsoreports/ReportServer?/vso.sqlAzure/vso.SqlAzure.PartitionDatabases&serviceInstance={0}";
    private static readonly TimeSpan s_defaultMinDisabledTime = new TimeSpan(7, 0, 0, 0);
    private static readonly TimeSpan s_defaultMinReplacedTime = new TimeSpan(24, 0, 0);
    private static readonly int s_PoolManagementJobDelay = 60;
    private static readonly string s_OnDemandPartitionLockName = "AcquireDatabasePartitionLock_";
    private static readonly string s_BeginCopyStatusReasonFormatString = "Database is being created as a copy of {0}.{1}";
    private static readonly string s_FailedBeginCopyStatusReasonFormatString = "Failed to create database as a copy of {0}.{1}: {2}";
    private static readonly string s_CopyCompleteStatusReason = "Database is a full copy of another database and requires servicing before being brought online.";
    private static readonly string s_stalePropertiesTrace = "DatabasePropertiesStaleException was encountered when updating database properties. Database Id = {0}. Cached properties version = {1}. Exception Details = {2}.";
    private const string c_throttleFormat = "{0} = {1}. Threshold = {2}. ";
    private const string c_dataStaleSeconds = "Age (in seconds) of db perf data = {0}. Threshold = {1}. DateTime.UtcNow = {2}; dbResourceStats.PerfTime = {3}. ";
    private const string c_avgCpuPercent = "Database AvgCpuPercent";
    private const string c_avgDataIOPercent = "Database AvgDataIOPercent";
    private const string c_avgLogWritePercent = "Database AvgLogWritePercent";
    private const string c_avgMemoryUsagePercent = "Database AvgMemoryUsagePercent";
    private const string c_maxWorkerPercent = "Database MaxWorkerPercent";
    private const string c_pageLatchAvgWaitTimeMS = "Page latch average wait time (ms)";
    private static readonly HashSet<string> s_databasePoolsThatShouldNotPublishKpi = new HashSet<string>((IEnumerable<string>) new string[4]
    {
      DatabaseManagementConstants.CollectionExportPool,
      DatabaseManagementConstants.CollectionImportPool,
      DatabaseManagementConstants.CollectionImportSourcePool,
      DatabaseManagementConstants.CollectionImportDacPacPool
    });
    private readonly ConcurrentDictionary<int, InternalDatabaseProperties> m_databasePropertiesCache = new ConcurrentDictionary<int, InternalDatabaseProperties>();
    internal ILockName m_cacheLockName;
    private TeamFoundationTask m_cacheRefreshTask;
    private INotificationRegistration m_propertiesRegistration;
    private INotificationRegistration m_registeredNotification;
    private INotificationRegistration m_removedRegistration;
    private INotificationRegistration m_flushCacheRegistration;
    private const string c_UserID = "User ID";
    private const string c_Password = "Password";
    private static readonly string s_Area = "DatabaseManagement";
    private static readonly string s_Layer = "BusinessLogic";
    private const int c_waitForDBReplicaInSeconds = 30;

    internal TeamFoundationDatabaseManagementService()
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      TeamFoundationTracingService.TraceEnterRaw(99000, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, "ServiceStart", new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      try
      {
        systemRequestContext.CheckDeploymentRequestContext();
        this.m_cacheLockName = this.CreateLockName(systemRequestContext, "dbmsCacheLock");
        this.SetCacheEntry(systemRequestContext, systemRequestContext.ServiceHost.DeploymentServiceHost.DatabaseProperties);
        ITeamFoundationSqlNotificationService service1 = systemRequestContext.GetService<ITeamFoundationSqlNotificationService>();
        this.m_propertiesRegistration = service1.CreateRegistration(systemRequestContext, "Default", SqlNotificationEventClasses.DatabasePropertiesChanged, new SqlNotificationCallback(this.NotificationCallback), false, false);
        this.m_registeredNotification = service1.CreateRegistration(systemRequestContext, "Default", SqlNotificationEventClasses.DatabaseRegistered, new SqlNotificationCallback(this.NotificationCallback), false, false);
        this.m_removedRegistration = service1.CreateRegistration(systemRequestContext, "Default", SqlNotificationEventClasses.DatabaseRemoved, new SqlNotificationCallback(this.NotificationCallback), false, false);
        this.m_flushCacheRegistration = service1.CreateRegistration(systemRequestContext, "Default", SqlNotificationEventClasses.FlushDatabaseCache, new SqlNotificationCallback(this.NotificationCallback), false, false);
        List<InternalDatabaseProperties> items;
        using (DatabaseManagementComponent componentRaw = systemRequestContext.ServiceHost.DeploymentServiceHost.DatabaseProperties.SqlConnectionInfo.CreateComponentRaw<DatabaseManagementComponent>())
        {
          using (ResultCollection resultCollection = componentRaw.QueryDatabases())
            items = resultCollection.GetCurrent<InternalDatabaseProperties>().Items;
        }
        foreach (ITeamFoundationDatabaseProperties updatedProperties in items)
          this.UpdateCache(systemRequestContext, updatedProperties);
        ITeamFoundationTaskService service2 = systemRequestContext.GetService<ITeamFoundationTaskService>();
        this.m_cacheRefreshTask = new TeamFoundationTask(new TeamFoundationTaskCallback(this.CacheRefreshTaskCallback), (object) null, (int) TimeSpan.FromMinutes(5.0).TotalMilliseconds);
        IVssRequestContext requestContext = systemRequestContext;
        TeamFoundationTask cacheRefreshTask = this.m_cacheRefreshTask;
        service2.AddTask(requestContext, cacheRefreshTask);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(99008, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, ex);
        throw;
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(99009, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, "ServiceStart");
      }
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
      TeamFoundationTracingService.TraceEnterRaw(99010, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, "ServiceEnd", new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      try
      {
        ITeamFoundationTaskService service = systemRequestContext.GetService<ITeamFoundationTaskService>();
        if (this.m_cacheRefreshTask != null)
        {
          service.RemoveTask(systemRequestContext, this.m_cacheRefreshTask);
          this.m_cacheRefreshTask = (TeamFoundationTask) null;
        }
        this.m_propertiesRegistration.Unregister(systemRequestContext);
        this.m_registeredNotification.Unregister(systemRequestContext);
        this.m_removedRegistration.Unregister(systemRequestContext);
        this.m_flushCacheRegistration.Unregister(systemRequestContext);
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(99011, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, "ServiceEnd");
      }
    }

    private void CacheRefreshTaskCallback(IVssRequestContext requestContext, object taskArgs) => this.QueryDatabases(requestContext, false);

    private void NotificationCallback(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      requestContext.TraceEnter(99012, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (NotificationCallback));
      try
      {
        if (eventClass == SqlNotificationEventClasses.DatabasePropertiesChanged)
          this.UpdateCacheFromNotification(requestContext, eventData);
        else if (eventClass == SqlNotificationEventClasses.FlushDatabaseCache)
          this.QueryDatabases(requestContext, false);
        else if (eventClass == SqlNotificationEventClasses.DatabaseRegistered)
        {
          this.UpdateCacheFromNotification(requestContext, eventData, false);
        }
        else
        {
          if (!(eventClass == SqlNotificationEventClasses.DatabaseRemoved))
            return;
          foreach (SerializableDatabaseProperties databaseProperties in TeamFoundationSerializationUtility.Deserialize<SerializableDatabaseProperties[]>(eventData))
            this.RemoveFromCache(requestContext, databaseProperties.DatabaseId);
        }
      }
      catch (Exception ex)
      {
        TeamFoundationEventLog.Default.LogException(requestContext, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Notification Processing Exception:\r\nEvent Class - {0}\r\nEvent Data - {1}", (object) eventClass, (object) SecretUtility.ScrubSecrets(eventData, false)), ex);
        requestContext.TraceException(99013, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(99014, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (NotificationCallback));
      }
    }

    public ITeamFoundationDatabaseProperties AcquireDatabasePartition(
      IVssRequestContext deploymentRequestContext,
      int databaseId,
      AcquirePartitionOptions acquireOptions,
      ITFLogger logger)
    {
      deploymentRequestContext.CheckDeploymentRequestContext();
      if (databaseId == -2)
        throw new VirtualServiceHostException();
      deploymentRequestContext.TraceEnter(99020, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (AcquireDatabasePartition));
      ArgumentUtility.CheckForNull<ITFLogger>(logger, nameof (logger));
      ITeamFoundationDatabaseProperties updatedProperties;
      using (DatabaseManagementComponent component = deploymentRequestContext.CreateComponent<DatabaseManagementComponent>())
      {
        updatedProperties = component.AcquireDatabasePartition(databaseId, acquireOptions);
        if (updatedProperties != null)
        {
          if (updatedProperties.DatabaseId != 0)
            goto label_9;
        }
        updatedProperties = (ITeamFoundationDatabaseProperties) component.GetDatabase(databaseId);
        throw new DatabaseUnavailableException(updatedProperties.DatabaseId);
      }
label_9:
      return (ITeamFoundationDatabaseProperties) this.UpdateCache(deploymentRequestContext, updatedProperties);
    }

    public ITeamFoundationDatabaseProperties AcquireDatabasePartition(
      IVssRequestContext deploymentRequestContext,
      string poolName,
      AcquirePartitionOptions acquireOptions,
      ITFLogger logger)
    {
      deploymentRequestContext.CheckDeploymentRequestContext();
      deploymentRequestContext.TraceEnter(99020, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (AcquireDatabasePartition));
      ArgumentUtility.CheckForNull<ITFLogger>(logger, nameof (logger));
      ArgumentUtility.CheckStringForNullOrEmpty(poolName, nameof (poolName));
      try
      {
        if (deploymentRequestContext.ExecutionEnvironment.IsHostedDeployment)
        {
          TeamFoundationJobService service = deploymentRequestContext.GetService<TeamFoundationJobService>();
          TeamFoundationJobReference foundationJobReference = new TeamFoundationJobReference(DatabaseManagementConstants.DatabasePrecreationJobId);
          IVssRequestContext requestContext = deploymentRequestContext;
          TeamFoundationJobReference[] jobReferences = new TeamFoundationJobReference[1]
          {
            foundationJobReference
          };
          int managementJobDelay = TeamFoundationDatabaseManagementService.s_PoolManagementJobDelay;
          service.QueueDelayedJobs(requestContext, (IEnumerable<TeamFoundationJobReference>) jobReferences, managementJobDelay, JobPriorityLevel.Highest);
        }
        ITeamFoundationDatabaseProperties updatedProperties;
        using (DatabaseManagementComponent component = deploymentRequestContext.CreateComponent<DatabaseManagementComponent>())
        {
          updatedProperties = component.AcquireDatabasePartition(poolName, acquireOptions);
          if (updatedProperties == null || updatedProperties.DatabaseId == 0)
          {
            using (deploymentRequestContext.AcquireExemptionLock())
            {
              using (deploymentRequestContext.GetService<ITeamFoundationLockingService>().AcquireLock(deploymentRequestContext, TeamFoundationLockMode.Exclusive, TeamFoundationDatabaseManagementService.s_OnDemandPartitionLockName + poolName))
              {
                updatedProperties = component.AcquireDatabasePartition(poolName, acquireOptions);
                if (updatedProperties != null)
                {
                  if (updatedProperties.DatabaseId != 0)
                    goto label_16;
                }
                string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "A database partition was not available from the pool = {0}.  Creating a database on-demand.", (object) poolName);
                deploymentRequestContext.Trace(99021, TraceLevel.Warning, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, message);
                logger.Warning(message);
                this.AddDatabaseToPool(deploymentRequestContext, poolName, logger: logger);
                deploymentRequestContext.Trace(99022, TraceLevel.Info, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, "AcquireDatabasePartition created a database on-demand.");
                updatedProperties = component.AcquireDatabasePartition(poolName, acquireOptions);
              }
            }
          }
label_16:
          if (updatedProperties != null)
          {
            if (updatedProperties.DatabaseId != 0)
              goto label_22;
          }
          deploymentRequestContext.Trace(99023, TraceLevel.Error, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, "A database was created on-demand, but prc_AcquireDatabasePartition did not return an available database. Pool name = {0}", (object) poolName);
          throw new AcquireDatabasePartitionException(poolName);
        }
label_22:
        return (ITeamFoundationDatabaseProperties) this.UpdateCache(deploymentRequestContext, updatedProperties);
      }
      catch (Exception ex)
      {
        deploymentRequestContext.TraceException(99028, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, ex);
        throw;
      }
      finally
      {
        deploymentRequestContext.TraceLeave(99029, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (AcquireDatabasePartition));
      }
    }

    private void SetCacheEntry(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties properties)
    {
      InternalDatabaseProperties databaseProperties = properties as InternalDatabaseProperties;
      if (databaseProperties.DatabaseId == -2)
        throw new VirtualServiceHostException();
      using (requestContext.AcquireWriterLock(this.m_cacheLockName))
        this.m_databasePropertiesCache[databaseProperties.DatabaseId] = databaseProperties;
    }

    private InternalDatabaseProperties UpdateCache(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties updatedProperties)
    {
      if (updatedProperties == null)
        throw new ArgumentNullException(nameof (updatedProperties));
      if (updatedProperties.DatabaseId == -2)
        throw new VirtualServiceHostException();
      InternalDatabaseProperties databaseProperties1;
      using (requestContext.AcquireWriterLock(this.m_cacheLockName))
      {
        using (requestContext.AcquireExemptionLock())
        {
          if (this.m_databasePropertiesCache.TryGetValue(updatedProperties.DatabaseId, out databaseProperties1))
          {
            if (updatedProperties.Version >= databaseProperties1.Version)
              databaseProperties1.Update(requestContext, updatedProperties);
          }
          else if (updatedProperties is InternalDatabaseProperties databaseProperties2)
          {
            try
            {
              databaseProperties2.UpdateSqlConnectionInfo(requestContext);
            }
            catch (CryptographicException ex)
            {
              requestContext.Trace(99024, TraceLevel.Error, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, "CryptographicException encountered while updating DBMS cache entry for database {0}.  The password is most likely not an encrypted value or improperly formatted.  Exception details: {1}", (object) databaseProperties2.DatabaseId, (object) ex);
            }
            catch (Exception ex)
            {
              requestContext.Trace(99024, TraceLevel.Error, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, "Exception encountered while updating DBMS cache entry for database {0}. Exception details {1}", (object) databaseProperties2.DatabaseId, (object) ex);
            }
            this.m_databasePropertiesCache[updatedProperties.DatabaseId] = databaseProperties2;
            databaseProperties1 = databaseProperties2;
          }
        }
      }
      return databaseProperties1;
    }

    private void UpdateCacheFromNotification(
      IVssRequestContext requestContext,
      string eventData,
      bool mergeOnly = true)
    {
      foreach (SerializableDatabaseProperties properties in TeamFoundationSerializationUtility.Deserialize<SerializableDatabaseProperties[]>(eventData))
        this.UpdateCacheFromNotification(requestContext, properties, mergeOnly);
    }

    private void UpdateCacheFromNotification(
      IVssRequestContext requestContext,
      SerializableDatabaseProperties properties,
      bool mergeOnly)
    {
      if (properties == null)
        return;
      if (properties.DatabaseId == -2)
        throw new VirtualServiceHostException();
      using (requestContext.AcquireWriterLock(this.m_cacheLockName))
      {
        using (requestContext.AcquireExemptionLock())
        {
          InternalDatabaseProperties databaseProperties1;
          if (this.m_databasePropertiesCache.TryGetValue(properties.DatabaseId, out databaseProperties1))
          {
            long? version1 = properties.Version;
            long version2 = databaseProperties1.Version;
            if (!(version1.GetValueOrDefault() >= version2 & version1.HasValue))
              return;
            databaseProperties1.Update(requestContext, properties);
          }
          else
          {
            if (mergeOnly)
              return;
            InternalDatabaseProperties databaseProperties2 = new InternalDatabaseProperties();
            databaseProperties2.Update(requestContext, properties);
            this.m_databasePropertiesCache[databaseProperties2.DatabaseId] = databaseProperties2;
          }
        }
      }
    }

    private bool TryReadCache(
      IVssRequestContext requestContext,
      int databaseId,
      out InternalDatabaseProperties properties)
    {
      InternalDatabaseProperties databaseProperties = (InternalDatabaseProperties) null;
      bool flag = false;
      if (databaseId != -2)
      {
        using (requestContext.AcquireReaderLock(this.m_cacheLockName))
          flag = this.m_databasePropertiesCache.TryGetValue(databaseId, out databaseProperties);
      }
      properties = databaseProperties;
      return flag;
    }

    public bool ThrottleDatabaseAccess(
      IVssRequestContext deploymentContext,
      ITeamFoundationDatabaseProperties databaseProperties,
      out string reason)
    {
      deploymentContext.CheckDeploymentRequestContext();
      reason = (string) null;
      if (databaseProperties == null)
        return false;
      TeamFoundationDatabaseManagementService.TeamFoundationDatabaseThrottleConfiguration throttleConfiguration = this.m_throttleConfigurationCache.Get(deploymentContext);
      DatabaseResourceStats databaseResourceStats = databaseProperties.GetDatabaseResourceStats(deploymentContext);
      if (databaseResourceStats != null)
      {
        DateTime currentTime = databaseResourceStats.CurrentTime;
        TimeSpan timeSpan = currentTime - databaseResourceStats.PerfTime;
        if ((int) throttleConfiguration.DataStaleSecondsThreshold > (int) DatabaseManagementConstants.InvalidThreshold && timeSpan >= TimeSpan.FromSeconds((double) throttleConfiguration.DataStaleSecondsThreshold))
        {
          reason += string.Format("Age (in seconds) of db perf data = {0}. Threshold = {1}. DateTime.UtcNow = {2}; dbResourceStats.PerfTime = {3}. ", (object) timeSpan.TotalSeconds, (object) throttleConfiguration.DataStaleSecondsThreshold, (object) currentTime.ToString("s"), (object) databaseResourceStats.PerfTime.ToString("s"));
          deploymentContext.Trace(522304005, TraceLevel.Error, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, "Stale DB stats: {0}", (object) reason);
        }
        if ((int) throttleConfiguration.AvgCpuThreshold > (int) DatabaseManagementConstants.InvalidThreshold && databaseResourceStats.AvgCpuPercent >= (Decimal) throttleConfiguration.AvgCpuThreshold)
          reason += string.Format("{0} = {1}. Threshold = {2}. ", (object) "Database AvgCpuPercent", (object) databaseResourceStats.AvgCpuPercent, (object) throttleConfiguration.AvgCpuThreshold);
        if ((int) throttleConfiguration.AvgDataIOThreshold > (int) DatabaseManagementConstants.InvalidThreshold && databaseResourceStats.AvgDataIOPercent >= (Decimal) throttleConfiguration.AvgDataIOThreshold)
          reason += string.Format("{0} = {1}. Threshold = {2}. ", (object) "Database AvgDataIOPercent", (object) databaseResourceStats.AvgDataIOPercent, (object) throttleConfiguration.AvgDataIOThreshold);
        if ((int) throttleConfiguration.AvgLogWriteThreshold > (int) DatabaseManagementConstants.InvalidThreshold && databaseResourceStats.AvgLogWritePercent >= (Decimal) throttleConfiguration.AvgLogWriteThreshold)
          reason += string.Format("{0} = {1}. Threshold = {2}. ", (object) "Database AvgLogWritePercent", (object) databaseResourceStats.AvgLogWritePercent, (object) throttleConfiguration.AvgLogWriteThreshold);
        if ((int) throttleConfiguration.AvgMemoryUsageThreshold > (int) DatabaseManagementConstants.InvalidThreshold && databaseResourceStats.AvgMemoryUsagePercent >= (Decimal) throttleConfiguration.AvgMemoryUsageThreshold)
          reason += string.Format("{0} = {1}. Threshold = {2}. ", (object) "Database AvgMemoryUsagePercent", (object) databaseResourceStats.AvgMemoryUsagePercent, (object) throttleConfiguration.AvgMemoryUsageThreshold);
        if ((int) throttleConfiguration.MaxWorkerThreshold > (int) DatabaseManagementConstants.InvalidThreshold && databaseResourceStats.MaxWorkerPercent >= (Decimal) throttleConfiguration.MaxWorkerThreshold)
          reason += string.Format("{0} = {1}. Threshold = {2}. ", (object) "Database MaxWorkerPercent", (object) databaseResourceStats.MaxWorkerPercent, (object) throttleConfiguration.MaxWorkerThreshold);
        if (throttleConfiguration.PageLatchAverageWaitTimeMSThreshold > (int) DatabaseManagementConstants.InvalidThreshold && databaseResourceStats.PageLatchAvgTimeMS >= throttleConfiguration.PageLatchAverageWaitTimeMSThreshold)
          reason += string.Format("{0} = {1}. Threshold = {2}. ", (object) "Page latch average wait time (ms)", (object) databaseResourceStats.PageLatchAvgTimeMS, (object) throttleConfiguration.PageLatchAverageWaitTimeMSThreshold);
      }
      return reason != null;
    }

    private void RemoveFromCache(IVssRequestContext requestContext, int databaseId)
    {
      if (databaseId == -2)
        throw new VirtualServiceHostException();
      using (requestContext.AcquireWriterLock(this.m_cacheLockName))
        this.m_databasePropertiesCache.TryRemove(databaseId, out InternalDatabaseProperties _);
    }

    public string GenerateDatabaseName(IVssRequestContext deploymentRequestContext, string poolName = null)
    {
      IVssRegistryService service = deploymentRequestContext.GetService<IVssRegistryService>();
      string databaseLabel = service.GetValue(deploymentRequestContext, (RegistryQuery) FrameworkServerConstants.DatabaseLabel, string.Empty);
      string databasePrefix = service.GetValue(deploymentRequestContext, (RegistryQuery) FrameworkServerConstants.DatabasePrefix, string.Empty);
      DatabasePoolExtendedProperties extendedProperties;
      if (!string.IsNullOrEmpty(poolName) && TeamFoundationDatabaseExtensions.DbPoolExtendedPropertiesMap.TryGetValue(poolName, out extendedProperties) && !extendedProperties.DbPartitionSplitDataspaceCategory.IsNullOrEmpty<char>())
        databasePrefix = databasePrefix + "_" + extendedProperties.DbPartitionSplitDataspaceCategory;
      return TeamFoundationDatabaseManagementService.GenerateDatabaseNameRaw(databaseLabel, databasePrefix);
    }

    internal ITeamFoundationDatabaseProperties AddDatabaseToPool(
      IVssRequestContext deploymentRequestContext,
      string poolName,
      TeamFoundationDatabaseFlags flags = TeamFoundationDatabaseFlags.None,
      string serviceObjective = null,
      TeamFoundationDatabaseStatus bringToState = TeamFoundationDatabaseStatus.Online,
      IDictionary<string, string> additionalTokens = null,
      ITFLogger logger = null)
    {
      deploymentRequestContext.TraceEnter(99190, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (AddDatabaseToPool));
      deploymentRequestContext.CheckServicingRequestContext();
      ArgumentUtility.CheckStringForNullOrEmpty(poolName, nameof (poolName));
      if (logger == null)
        logger = (ITFLogger) new NullLogger();
      TeamFoundationDatabasePool databasePool = this.GetDatabasePool(deploymentRequestContext, poolName);
      if (this.GetNumberOfDatabases(deploymentRequestContext, poolName, new TeamFoundationDatabaseStatus?()) >= databasePool.MaxDatabaseLimit)
        throw new DatabasePoolFullException(poolName);
      TeamFoundationDataTierService service = deploymentRequestContext.GetService<TeamFoundationDataTierService>();
      string databaseName = this.GenerateDatabaseName(deploymentRequestContext, databasePool.PoolName);
      ITeamFoundationDatabaseProperties databaseProperties = (ITeamFoundationDatabaseProperties) null;
      List<DataTierInfo> dataTierInfoList = (List<DataTierInfo>) null;
      deploymentRequestContext.TraceAlways(99191, TraceLevel.Info, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, "Acquiring datatier shared lock...");
      using (service.AcquireSharedLock(deploymentRequestContext))
      {
        List<DataTierInfo> dataTiers = service.GetDataTiers(deploymentRequestContext, true);
        dataTiers.Sort((Comparison<DataTierInfo>) ((dt1, dt2) => dt1.DatabaseCount.CompareTo(dt2.DatabaseCount)));
        foreach (DataTierInfo dataTierInfo in dataTiers.Where<DataTierInfo>((Func<DataTierInfo, bool>) (dt => dt.State == DataTierState.Active)))
        {
          try
          {
            VsoCreateDatabaseOptions dbOptions = new VsoCreateDatabaseOptions()
            {
              Flags = flags,
              ServiceObjective = serviceObjective
            };
            databaseProperties = this.CreateDatabase(deploymentRequestContext, databaseName, databasePool.PoolName, dataTierInfo.ConnectionInfo, dbOptions, logger);
            break;
          }
          catch (AzureDatabaseQuotaReachedException ex)
          {
            deploymentRequestContext.TraceException(99190, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, (Exception) ex);
            if (dataTierInfoList == null)
              dataTierInfoList = new List<DataTierInfo>();
            dataTierInfoList.Add(dataTierInfo);
          }
        }
      }
      if (dataTierInfoList != null)
      {
        foreach (DataTierInfo dataTierInfo in dataTierInfoList)
          service.SetDataTierState(deploymentRequestContext, dataTierInfo.DataSource, DataTierState.Full);
      }
      if (databaseProperties == null)
        throw new InvalidOperationException("All data tiers are full. SD team must add new data tier(s) to the system.");
      deploymentRequestContext.TraceAlways(99195, TraceLevel.Info, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, "Executing servicing on " + databaseProperties.DatabaseName);
      this.ExecuteServicing(deploymentRequestContext, databasePool, databaseProperties, additionalTokens, bringToState, logger);
      return databaseProperties;
    }

    public void ExecuteServicing(
      IVssRequestContext deploymentRequestContext,
      TeamFoundationDatabasePool pool,
      ITeamFoundationDatabaseProperties databaseProperties,
      IDictionary<string, string> additionalTokens = null,
      TeamFoundationDatabaseStatus bringToState = TeamFoundationDatabaseStatus.Online,
      ITFLogger logger = null)
    {
      deploymentRequestContext.TraceEnter(99230, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (ExecuteServicing));
      ServicingJobDetail servicingJobDetail = (ServicingJobDetail) null;
      if (logger == null)
        logger = (ITFLogger) new NullLogger();
      try
      {
        ISqlConnectionInfo dboConnectionInfo = databaseProperties.DboConnectionInfo;
        ISqlConnectionInfo sqlConnectionInfo = databaseProperties.SqlConnectionInfo;
        if (!string.IsNullOrEmpty(pool.ServicingOperations))
        {
          string[] operations = pool.ServicingOperations.Split(new char[1]
          {
            ';'
          }, StringSplitOptions.RemoveEmptyEntries);
          logger.Info("Executing job to create schema by executing operation(s) '{0}' on db '{1}'", (object) pool.ServicingOperations, (object) databaseProperties.DatabaseName);
          servicingJobDetail = this.PerformCreateDatabaseSchemaJob(deploymentRequestContext, dboConnectionInfo, sqlConnectionInfo, operations, pool, additionalTokens, logger);
        }
        if (servicingJobDetail != null && (servicingJobDetail.JobStatus == ServicingJobStatus.Failed || servicingJobDetail.Result == ServicingJobResult.Failed))
        {
          string str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Servicing Job {0} Failed.", (object) servicingJobDetail.JobId);
          deploymentRequestContext.Trace(99231, TraceLevel.Error, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, str);
          this.SetDatabaseStatus(deploymentRequestContext, databaseProperties.DatabaseId, TeamFoundationDatabaseStatus.Failed, str);
        }
        else
        {
          string serviceLevel;
          using (ExtendedAttributeComponent componentRaw = (dboConnectionInfo ?? sqlConnectionInfo).CreateComponentRaw<ExtendedAttributeComponent>())
            serviceLevel = componentRaw.ReadServiceLevelStamp();
          this.UpdateDatabaseProperties(deploymentRequestContext, databaseProperties.DatabaseId, (Action<TeamFoundationDatabaseProperties>) (editableProperties =>
          {
            editableProperties.ServiceLevel = serviceLevel;
            editableProperties.Status = bringToState;
            editableProperties.StatusReason = string.Empty;
            editableProperties.Flags &= ~TeamFoundationDatabaseFlags.PrecreationInProgress;
          }));
        }
      }
      catch (Exception ex1)
      {
        try
        {
          deploymentRequestContext.TraceException(99232, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, ex1);
          string str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Schema Creation Failed. Exception: {0}", (object) ex1.ToReadableStackTrace());
          this.SetDatabaseStatus(deploymentRequestContext, databaseProperties.DatabaseId, TeamFoundationDatabaseStatus.Failed, str);
          logger.Info(str);
        }
        catch (Exception ex2)
        {
        }
        throw;
      }
      finally
      {
        deploymentRequestContext.TraceLeave(99233, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (ExecuteServicing));
      }
    }

    private ServicingJobDetail PerformCreateDatabaseSchemaJob(
      IVssRequestContext deploymentRequestContext,
      ISqlConnectionInfo servicingConnectionInfo,
      ISqlConnectionInfo sqlConnectionInfo,
      string[] operations,
      TeamFoundationDatabasePool pool,
      IDictionary<string, string> tokens,
      ITFLogger logger)
    {
      string str1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, JobNameConstants.CreateSchemaFormat, (object) servicingConnectionInfo.InitialCatalog);
      if (str1.Length > JobNameConstants.MaximumJobNameLength)
        str1 = str1.Substring(0, JobNameConstants.MaximumJobNameLength);
      ServicingJobData servicingJobData1 = new ServicingJobData(operations)
      {
        OperationClass = "CreateSchema",
        ServicingOptions = ServicingFlags.None,
        ServicingHostId = deploymentRequestContext.ServiceHost.InstanceId,
        JobTitle = str1
      };
      servicingJobData1.ServicingItems[ServicingItemConstants.DboConnectionInfo] = (object) new SqlConnectionInfoWrapper(deploymentRequestContext, servicingConnectionInfo);
      servicingJobData1.ServicingItems[ServicingItemConstants.ConnectionInfo] = (object) new SqlConnectionInfoWrapper(deploymentRequestContext, sqlConnectionInfo);
      servicingJobData1.ServicingTokens["MaxServicingLockTimeInSeconds"] = "0";
      Guid guid;
      deploymentRequestContext.TryGetItem<Guid>(RequestContextItemsKeys.AzureSubscriptionId, out guid);
      logger.Info(string.Format("Read item {0}: {1}", (object) "AzureSubscriptionId", (object) guid));
      string str2;
      deploymentRequestContext.TryGetItem<string>(RequestContextItemsKeys.ResourceManagerAadTenantId, out str2);
      logger.Info("Read item ResourceManagerAadTenantId: " + str2);
      string str3;
      deploymentRequestContext.TryGetItem<string>(RequestContextItemsKeys.ResourceManagementUrl, out str3);
      logger.Info("Read item ResourceManagerEndpointUrl: " + str3);
      string str4;
      deploymentRequestContext.TryGetItem<string>(RequestContextItemsKeys.ResourceGroupName, out str4);
      logger.Info("Read item HostedServiceName: " + str4);
      if (guid != Guid.Empty && !string.IsNullOrEmpty(str2) && !string.IsNullOrEmpty(str3) && !string.IsNullOrEmpty(str4))
      {
        servicingJobData1.ServicingTokens[ServicingTokenConstants.AzureSubscriptionId] = guid.ToString();
        servicingJobData1.ServicingTokens[ServicingTokenConstants.ResourceManagerAadTenantId] = str2;
        servicingJobData1.ServicingTokens[ServicingTokenConstants.ResourceManagerEndpointUrl] = str3;
        servicingJobData1.ServicingTokens[ServicingTokenConstants.HostedServiceName] = str4;
      }
      if (tokens != null)
      {
        foreach (KeyValuePair<string, string> token in (IEnumerable<KeyValuePair<string, string>>) tokens)
          servicingJobData1.ServicingTokens.Add(token);
      }
      string str5 = pool.DatabaseType != TeamFoundationDatabaseType.Configuration ? (!servicingConnectionInfo.InitialCatalog.StartsWith("Tfs", StringComparison.Ordinal) ? TeamFoundationSqlResourceComponent.DatabaseTypeAccount : TeamFoundationSqlResourceComponent.DatabaseTypeCollection) : pool.DatabaseType.ToString();
      servicingJobData1.ServicingTokens[ServicingTokenConstants.DatabaseType] = str5;
      TeamFoundationServicingService service = deploymentRequestContext.GetService<TeamFoundationServicingService>();
      IVssRequestContext requestContext = deploymentRequestContext;
      ServicingJobData servicingJobData2 = servicingJobData1;
      ITFLogger tfLogger = logger;
      Guid? jobId = new Guid?();
      ITFLogger logger1 = tfLogger;
      return service.PerformServicingJob(requestContext, servicingJobData2, jobId, logger1);
    }

    public virtual ITeamFoundationDatabaseProperties GetDatabase(
      IVssRequestContext requestContext,
      int databaseId,
      bool useCache = true)
    {
      TeamFoundationTracingService.TraceEnterRaw(99070, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (GetDatabase), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      try
      {
        requestContext.CheckDeploymentRequestContext();
        if (databaseId == -2)
          throw new VirtualServiceHostException();
        InternalDatabaseProperties properties;
        if (!useCache || !this.TryReadCache(requestContext, databaseId, out properties))
        {
          using (DatabaseManagementComponent component = requestContext.CreateComponent<DatabaseManagementComponent>())
            properties = component.GetDatabase(databaseId);
          properties.UpdateSqlConnectionInfo(requestContext);
          properties = this.UpdateCache(requestContext, (ITeamFoundationDatabaseProperties) properties);
        }
        return (ITeamFoundationDatabaseProperties) properties;
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(99071, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, ex);
        throw;
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(99072, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (GetDatabase));
      }
    }

    public ISqlConnectionInfo GetSqlConnectionInfo(
      IVssRequestContext requestContext,
      int databaseId)
    {
      if (databaseId == -2)
        throw new VirtualServiceHostException();
      IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
      ITeamFoundationDatabaseProperties database = this.GetDatabase(requestContext1, databaseId, true);
      if (database.SqlConnectionInfo == null)
        database = this.GetDatabase(requestContext1, databaseId, false);
      return database.SqlConnectionInfo;
    }

    public ISqlConnectionInfo GetDboConnectionInfo(
      IVssRequestContext requestContext,
      int databaseId)
    {
      if (databaseId == -2)
        throw new VirtualServiceHostException();
      IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
      ITeamFoundationDatabaseProperties database = this.GetDatabase(requestContext1, databaseId, true);
      if (database.DboConnectionInfo == null)
        database = this.GetDatabase(requestContext1, databaseId, false);
      return database.DboConnectionInfo;
    }

    public static ITeamFoundationDatabaseProperties GetDatabaseRaw(
      ISqlConnectionInfo configDbConnectionInfo,
      int databaseId)
    {
      if (databaseId == -2)
        throw new VirtualServiceHostException();
      InternalDatabaseProperties database;
      using (DatabaseManagementComponent componentRaw = configDbConnectionInfo.CreateComponentRaw<DatabaseManagementComponent>())
      {
        database = componentRaw.GetDatabase(databaseId);
        database.UpdateSqlConnectionInfoRaw(configDbConnectionInfo);
      }
      return (ITeamFoundationDatabaseProperties) database;
    }

    public static List<ITeamFoundationDatabaseProperties> GetPartitionDatabasesRaw(
      ISqlConnectionInfo configDbConnectionInfo)
    {
      List<InternalDatabaseProperties> collection;
      using (DatabaseManagementComponent componentRaw = configDbConnectionInfo.CreateComponentRaw<DatabaseManagementComponent>())
        collection = componentRaw.QueryDatabases(TeamFoundationDatabaseType.Partition);
      foreach (InternalDatabaseProperties databaseProperties in collection)
        databaseProperties.UpdateSqlConnectionInfoRaw(configDbConnectionInfo);
      return new List<ITeamFoundationDatabaseProperties>((IEnumerable<ITeamFoundationDatabaseProperties>) collection);
    }

    public static ITeamFoundationDatabaseProperties RenameDatabaseRaw(
      ISqlConnectionInfo configDbConnectionInfo,
      int databaseId,
      string newDatabaseName,
      ITFLogger logger)
    {
      ITeamFoundationDatabaseProperties databaseRaw = TeamFoundationDatabaseManagementService.GetDatabaseRaw(configDbConnectionInfo, databaseId);
      string str = databaseRaw.DatabaseName.Split(';')[1];
      if (string.Equals(databaseRaw.PoolName, DatabaseManagementConstants.ConfigurationPoolName, StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("Cannot rename the configuration database");
      if (string.Equals(str, newDatabaseName, StringComparison.OrdinalIgnoreCase))
      {
        logger.Info("Database has already been renamed, doing nothing");
        return databaseRaw;
      }
      using (TeamFoundationDataTierComponent componentRaw = TeamFoundationDataTierService.FindAssociatedDataTierRaw(configDbConnectionInfo, databaseRaw.ConnectionInfoWrapper.ConnectionString).ConnectionInfo.CreateComponentRaw<TeamFoundationDataTierComponent>())
      {
        if (componentRaw.GetDatabaseInfo(newDatabaseName) == null)
        {
          logger.Info("Renaming database " + str + " to " + newDatabaseName);
          componentRaw.AlterDatabaseName(str, newDatabaseName);
          logger.Info("Rename complete");
        }
        else
          logger.Info("Database has already been renamed, continuing to update connection string");
      }
      using (DatabaseManagementComponent componentRaw = configDbConnectionInfo.CreateComponentRaw<DatabaseManagementComponent>())
      {
        logger.Info("Updating connection string");
        TeamFoundationDatabaseProperties editableProperties = databaseRaw.GetEditableProperties();
        editableProperties.UpdateConnectionString(new SqlConnectionStringBuilder(editableProperties.ConnectionInfoWrapper.ConnectionString)
        {
          InitialCatalog = newDatabaseName
        }.ToString());
        ITeamFoundationDatabaseProperties databaseProperties = componentRaw.UpdateDatabase(editableProperties);
        logger.Info("Connection string updated");
        return databaseProperties;
      }
    }

    internal static ITeamFoundationDatabaseProperties GetConfigurationDatabaseBootstrap(
      ISqlConnectionInfo configDbConnectionInfo)
    {
      InternalDatabaseProperties databaseBootstrap;
      using (DatabaseManagementComponent componentRaw = configDbConnectionInfo.CreateComponentRaw<DatabaseManagementComponent>())
      {
        databaseBootstrap = componentRaw.QueryDatabases(DatabaseManagementConstants.ConfigurationPoolName).FirstOrDefault<InternalDatabaseProperties>();
        if (databaseBootstrap == null)
          throw new InvalidConfigurationException("Unable to query configuration database.");
        databaseBootstrap.UpdateSqlConnectionInfoRaw(configDbConnectionInfo);
      }
      return (ITeamFoundationDatabaseProperties) databaseBootstrap;
    }

    public static ISqlConnectionInfo GetSqlConnectionInfoRaw(
      ISqlConnectionInfo configDbConnectionInfo,
      int databaseId)
    {
      if (databaseId == -2)
        throw new VirtualServiceHostException();
      return TeamFoundationDatabaseManagementService.GetDatabaseRaw(configDbConnectionInfo, databaseId).ConnectionInfoWrapper.ToSqlConnectionInfoRaw(configDbConnectionInfo);
    }

    public static ISqlConnectionInfo GetDboConnectionInfoRaw(
      ISqlConnectionInfo configDbConnectionInfo,
      int databaseId)
    {
      if (databaseId == -2)
        throw new VirtualServiceHostException();
      return TeamFoundationDatabaseManagementService.GetDatabaseRaw(configDbConnectionInfo, databaseId).DboConnectionInfoWrapper.ToSqlConnectionInfoRaw(configDbConnectionInfo);
    }

    public static ISqlConnectionInfo GetConfigDbDboConnectionInfoBootStrap(
      ISqlConnectionInfo defaultSqlConnectionInfo)
    {
      ITeamFoundationDatabaseProperties databaseBootstrap = TeamFoundationDatabaseManagementService.GetConfigurationDatabaseBootstrap(defaultSqlConnectionInfo);
      return databaseBootstrap.DboConnectionInfoWrapper == null ? defaultSqlConnectionInfo : databaseBootstrap.DboConnectionInfoWrapper.ToSqlConnectionInfoRaw(defaultSqlConnectionInfo);
    }

    public static void OptInPrivilegedSqlLogin(
      ISqlConnectionInfo dtConnectionInfo,
      ISqlConnectionInfo dboConnectionInfo,
      string execRoleUser,
      bool revokeViewServerState = false)
    {
      using (TeamFoundationSqlSecurityComponent componentRaw = dtConnectionInfo.CloneReplaceInitialCatalog(dboConnectionInfo.InitialCatalog).CreateComponentRaw<TeamFoundationSqlSecurityComponent>())
        componentRaw.DropRoleMember(DatabaseRoles.DbOwner, DatabaseRoles.TfsExecRole);
      if (!revokeViewServerState)
        return;
      Dictionary<string, bool> permissions = new Dictionary<string, bool>()
      {
        {
          "VIEW SERVER STATE",
          true
        },
        {
          "ALTER ANY CONNECTION",
          true
        }
      };
      TeamFoundationDatabaseManagementService.RevokeServerScopePermissionsToLogin(dtConnectionInfo, execRoleUser, permissions, (ITFLogger) new ServerTraceLogger());
    }

    public static void OptOutPrivilegedSqlLogin(
      ISqlConnectionInfo dtConnectionInfo,
      ISqlConnectionInfo dboConnectionInfo,
      string execRoleUser)
    {
      if (dboConnectionInfo == null)
        return;
      using (TeamFoundationSqlSecurityComponent componentRaw = dtConnectionInfo.CloneReplaceInitialCatalog(dboConnectionInfo.InitialCatalog).CreateComponentRaw<TeamFoundationSqlSecurityComponent>())
        componentRaw.AddRoleMember(DatabaseRoles.DbOwner, DatabaseRoles.TfsExecRole);
      Dictionary<string, bool> permissions = new Dictionary<string, bool>()
      {
        {
          "VIEW SERVER STATE",
          false
        },
        {
          "ALTER ANY CONNECTION",
          false
        }
      };
      TeamFoundationDatabaseManagementService.GrantServerScopePermissionsToLogin(dtConnectionInfo, execRoleUser, permissions, (ITFLogger) new ServerTraceLogger());
    }

    public static void RevokeExecutePermissionFromSqlObjects(
      ISqlConnectionInfo dboConnectionInfo,
      string execRoleLogin)
    {
      StringBuilder stringBuilder = new StringBuilder();
      string format = "REVOKE EXECUTE ON {0} FROM {1}";
      List<DatabaseObjectDefinitionComponent.DatabaseObjectDefinition> storedProcedures;
      using (DatabaseObjectDefinitionComponent componentRaw = dboConnectionInfo.CreateComponentRaw<DatabaseObjectDefinitionComponent>())
        storedProcedures = componentRaw.GetStoredProcedures(includeContent: false);
      foreach (DatabaseObjectDefinitionComponent.DatabaseObjectDefinition objectDefinition in storedProcedures)
        stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, (object) objectDefinition.FullName, (object) StringUtil.QuoteName(execRoleLogin)));
      List<DatabaseObjectDefinitionComponent.DatabaseObjectDefinition> scalarFunctions;
      using (DatabaseObjectDefinitionComponent componentRaw = dboConnectionInfo.CreateComponentRaw<DatabaseObjectDefinitionComponent>())
        scalarFunctions = componentRaw.GetScalarFunctions(includeContent: false);
      foreach (DatabaseObjectDefinitionComponent.DatabaseObjectDefinition objectDefinition in scalarFunctions)
        stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, (object) objectDefinition.FullName, (object) StringUtil.QuoteName(execRoleLogin)));
      List<DatabaseObjectDefinitionComponent.DatabaseObjectDefinition> tableTypes;
      using (DatabaseObjectDefinitionComponent componentRaw = dboConnectionInfo.CreateComponentRaw<DatabaseObjectDefinitionComponent>())
        tableTypes = componentRaw.GetTableTypes();
      foreach (DatabaseObjectDefinitionComponent.DatabaseObjectDefinition objectDefinition in tableTypes)
        stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, (object) ("TYPE::" + objectDefinition.FullName), (object) StringUtil.QuoteName(execRoleLogin)));
      SqlScript script = new SqlScript("Revoke EXECUTE permission", stringBuilder.ToString());
      using (SqlScriptResourceComponent componentRaw = dboConnectionInfo.CreateComponentRaw<SqlScriptResourceComponent>())
        componentRaw.ExecuteScript(script, 3600);
    }

    public static void ProvisionDatabaseUsersForSqlJit(
      ISqlConnectionInfo dbDtConnectionInfo,
      IList<AadAccountObjectId> sqlDevOpsSecurityGroups,
      ITFLogger logger,
      bool isMaster = false,
      AadAccountObjectId sqlAdministrator = null)
    {
      if (sqlDevOpsSecurityGroups == null)
        return;
      foreach (AadAccountObjectId opsSecurityGroup in (IEnumerable<AadAccountObjectId>) sqlDevOpsSecurityGroups)
      {
        if (opsSecurityGroup.AccountName.Contains("@"))
        {
          logger.Error("User Id " + opsSecurityGroup.AccountName + " should not be added to SqlDevOpsSecurityGroups!");
        }
        else
        {
          using (TeamFoundationSqlSecurityComponent componentRaw = dbDtConnectionInfo.CreateComponentRaw<TeamFoundationSqlSecurityComponent>())
          {
            if (componentRaw.IsSqlAzure)
            {
              logger.Info("Create AAD User for security group " + opsSecurityGroup.AccountName);
              string forServiceAccount = componentRaw.CreateAADUserForServiceAccount(opsSecurityGroup.AccountName, new Guid(opsSecurityGroup.ObjectId), true);
              if (!isMaster)
              {
                logger.Info("Add AAD User " + forServiceAccount + " to " + DatabaseRoles.VsoDiagRole);
                componentRaw.AddRoleMember(DatabaseRoles.VsoDiagRole, forServiceAccount);
              }
            }
          }
        }
      }
    }

    public static void ProvisionVsoDiagRole(ISqlConnectionInfo dbDtConnectionInfo, ITFLogger logger)
    {
      using (TeamFoundationSqlSecurityComponent componentRaw = dbDtConnectionInfo.CreateComponentRaw<TeamFoundationSqlSecurityComponent>())
      {
        logger.Info("Provision " + DatabaseRoles.VsoDiagRole + " if it doesn't exist.");
        componentRaw.ProvisionVsoDiagRole();
      }
    }

    public virtual AzureDatabaseProperties GetDatabaseProperties(ISqlConnectionInfo connectionInfo)
    {
      using (TeamFoundationDataTierComponent componentRaw = connectionInfo.CreateComponentRaw<TeamFoundationDataTierComponent>())
        return componentRaw.GetDatabaseProperties();
    }

    internal void SendLogUtilizationKpi(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties properties,
      ISqlConnectionInfo sqlConnectionInfo)
    {
      ArgumentUtility.CheckForNull<ITeamFoundationDatabaseProperties>(properties, nameof (properties));
      int num = sqlConnectionInfo.ApplicationIntent == ApplicationIntent.ReadOnly ? 1 : 0;
      string actualServerName = properties.GetActualServerName(requestContext);
      string initialCatalog = sqlConnectionInfo.InitialCatalog;
      LogspaceUtilization logUtilization = this.GetLogUtilization(requestContext, properties, sqlConnectionInfo);
      List<LogspaceSummary> summaries = logUtilization.Summaries;
      List<LogspaceDetails> details = logUtilization.Details;
      IKpiService service = requestContext.GetService<IKpiService>();
      string scope = num != 0 ? actualServerName + ";" + initialCatalog + ";ReadOnly" : actualServerName + ";" + initialCatalog;
      foreach (LogspaceSummary logspaceSummary in summaries)
      {
        string str1 = TeamFoundationDatabaseManagementService.NormalizeDbNameForLogUsageKpi(logspaceSummary.DatabaseName, properties.DatabaseName);
        if (str1 != null)
        {
          string str2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}LogSizeInMb", (object) str1);
          string str3 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}LogSpaceUsedPercent", (object) str1);
          service.EnsureKpiIsRegistered(requestContext, KpiAreas.DatabaseLogSizeMetrics, str2, scope, str2);
          service.EnsureKpiIsRegistered(requestContext, KpiAreas.DatabaseLogSizeMetrics, str3, scope, str3);
          List<KpiMetric> metrics = new List<KpiMetric>()
          {
            new KpiMetric()
            {
              Name = str2,
              Value = (double) logspaceSummary.LogSize
            },
            new KpiMetric()
            {
              Name = str3,
              Value = (double) logspaceSummary.LogspaceUsedPercent
            }
          };
          service.Publish(requestContext, KpiAreas.DatabaseLogSizeMetrics, scope, metrics);
        }
      }
      foreach (LogspaceDetails logspaceDetails in details)
        requestContext.Trace(314989846, TraceLevel.Error, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, logspaceDetails.ToString());
    }

    internal virtual LogspaceUtilization GetLogUtilization(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties properties,
      ISqlConnectionInfo sqlConnectionInfo)
    {
      requestContext.CheckDeploymentRequestContext();
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      int transactionsOlderThanSeconds = service.GetValue<int>(requestContext, (RegistryQuery) FrameworkServerConstants.TransactionErrorThresholdinSeconds, 3630);
      long transactionsMinimumBytes = service.GetValue<long>(requestContext, (RegistryQuery) FrameworkServerConstants.TransactionErrorThresholdinBytes, 524288000L);
      using (DiagnosticComponent componentRaw = sqlConnectionInfo.CreateComponentRaw<DiagnosticComponent>())
      {
        List<LogspaceDetails> details;
        List<LogspaceSummary> logUtilization = componentRaw.GetLogUtilization(transactionsOlderThanSeconds, transactionsMinimumBytes, out details);
        return new LogspaceUtilization()
        {
          Summaries = logUtilization,
          Details = details,
          TransactionErrorThresholdinBytes = transactionsMinimumBytes,
          TransactionErrorThresholdinSeconds = transactionsOlderThanSeconds
        };
      }
    }

    public void EnableTde(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties databaseProperties)
    {
      using (TeamFoundationDataTierComponent componentRaw = requestContext.GetService<TeamFoundationDataTierService>().FindAssociatedDataTier(requestContext, databaseProperties.ConnectionInfoWrapper.ConnectionString).ConnectionInfo.CloneReplaceInitialCatalog(TeamFoundationDataTierService.GetDatabaseName(databaseProperties.ConnectionInfoWrapper.ConnectionString)).CreateComponentRaw<TeamFoundationDataTierComponent>())
        componentRaw.EnableTde();
    }

    public DatabaseEncryptionState QueryTde(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties databaseProperties)
    {
      using (TeamFoundationDataTierComponent componentRaw = databaseProperties.GetDboConnectionInfo().CreateComponentRaw<TeamFoundationDataTierComponent>())
        return componentRaw.QueryTde();
    }

    public virtual TeamFoundationDatabasePool GetDatabasePool(
      IVssRequestContext requestContext,
      string poolName)
    {
      requestContext.CheckDeploymentRequestContext();
      TeamFoundationDatabasePool pool;
      if (!this.TryGetDatabasePool(requestContext, poolName, out pool))
        throw new DatabasePoolNotFoundException(poolName);
      return pool;
    }

    public bool TryGetDatabasePool(
      IVssRequestContext requestContext,
      string poolName,
      out TeamFoundationDatabasePool pool)
    {
      requestContext.TraceEnter(99075, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (TryGetDatabasePool));
      try
      {
        requestContext.CheckDeploymentRequestContext();
        ArgumentUtility.CheckStringForNullOrEmpty(poolName, nameof (poolName));
        using (DatabaseManagementComponent component = requestContext.CreateComponent<DatabaseManagementComponent>())
          pool = component.GetDatabasePool(poolName);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(99076, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(99077, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (TryGetDatabasePool));
      }
      return pool != null;
    }

    public int GetNumberOfDatabases(
      IVssRequestContext requestContext,
      string poolName,
      TeamFoundationDatabaseStatus? status)
    {
      requestContext.TraceEnter(99200, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (GetNumberOfDatabases));
      try
      {
        requestContext.CheckDeploymentRequestContext();
        using (DatabaseManagementComponent component = requestContext.CreateComponent<DatabaseManagementComponent>())
          return component.GetNumberOfDatabases(poolName, status);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(99208, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(99209, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (GetNumberOfDatabases));
      }
    }

    public void IncrementTenantsPendingDelete(
      IVssRequestContext requestContext,
      int databaseId,
      int tenantCount)
    {
      requestContext.TraceEnter(99220, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (IncrementTenantsPendingDelete));
      try
      {
        requestContext.CheckDeploymentRequestContext();
        ArgumentUtility.CheckForOutOfRange(tenantCount, nameof (tenantCount), 0, int.MaxValue);
        if (databaseId == -2)
          throw new VirtualServiceHostException();
        using (DatabaseManagementComponent component = requestContext.CreateComponent<DatabaseManagementComponent>())
        {
          if (component.Version < 2)
            throw new ServiceVersionNotSupportedException(string.Format("IncrementTenantsPendingDelete is available from DatabaseManagment starting at service level 2.  Current service level = {0}", (object) component.Version));
          component.IncrementTenantsPendingDelete(databaseId, tenantCount);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(99208, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(99229, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (IncrementTenantsPendingDelete));
      }
    }

    public virtual List<ITeamFoundationDatabaseProperties> QueryDatabases(
      IVssRequestContext requestContext,
      bool useCache = true)
    {
      TeamFoundationTracingService.TraceEnterRaw(99040, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (QueryDatabases), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      try
      {
        requestContext.CheckDeploymentRequestContext();
        if (!useCache)
        {
          List<InternalDatabaseProperties> items;
          using (DatabaseManagementComponent component = requestContext.CreateComponent<DatabaseManagementComponent>())
          {
            using (ResultCollection resultCollection = component.QueryDatabases())
              items = resultCollection.GetCurrent<InternalDatabaseProperties>().Items;
          }
          HashSet<int> intSet = new HashSet<int>(items.Select<InternalDatabaseProperties, int>((Func<InternalDatabaseProperties, int>) (x => x.DatabaseId)));
          foreach (int key in (IEnumerable<int>) this.m_databasePropertiesCache.Keys)
          {
            if (!intSet.Contains(key))
              this.RemoveFromCache(requestContext, key);
          }
          foreach (ITeamFoundationDatabaseProperties updatedProperties in items)
            this.UpdateCache(requestContext, updatedProperties);
        }
        using (requestContext.AcquireReaderLock(this.m_cacheLockName))
          return ((IEnumerable<ITeamFoundationDatabaseProperties>) this.m_databasePropertiesCache.Values).ToList<ITeamFoundationDatabaseProperties>();
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(99048, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, ex);
        throw;
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(99049, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (QueryDatabases));
      }
    }

    public virtual List<ITeamFoundationDatabaseProperties> QueryDatabases(
      IVssRequestContext requestContext,
      string poolName,
      bool useCache = true)
    {
      TeamFoundationTracingService.TraceEnterRaw(99040, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (QueryDatabases), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      try
      {
        if (!useCache)
        {
          requestContext.CheckDeploymentRequestContext();
          List<InternalDatabaseProperties> databasePropertiesList;
          using (DatabaseManagementComponent component = requestContext.CreateComponent<DatabaseManagementComponent>())
            databasePropertiesList = component.QueryDatabases(poolName);
          foreach (ITeamFoundationDatabaseProperties updatedProperties in databasePropertiesList)
            this.UpdateCache(requestContext, updatedProperties);
        }
        using (requestContext.AcquireReaderLock(this.m_cacheLockName))
          return ((IEnumerable<ITeamFoundationDatabaseProperties>) this.m_databasePropertiesCache.Values).Where<ITeamFoundationDatabaseProperties>((Func<ITeamFoundationDatabaseProperties, bool>) (db => db.PoolName.Equals(poolName, StringComparison.OrdinalIgnoreCase))).ToList<ITeamFoundationDatabaseProperties>();
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(99048, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, ex);
        throw;
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(99049, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (QueryDatabases));
      }
    }

    public List<ITeamFoundationDatabaseProperties> QueryDatabases(
      IVssRequestContext requestContext,
      TeamFoundationDatabaseType databaseType,
      bool useCache = true)
    {
      requestContext.TraceEnter(99040, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (QueryDatabases));
      try
      {
        requestContext.CheckDeploymentRequestContext();
        List<ITeamFoundationDatabaseProperties> databasePropertiesList1;
        if (useCache)
        {
          HashSet<string> poolNames = new HashSet<string>(this.QueryDatabasePools(requestContext).Where<TeamFoundationDatabasePool>((Func<TeamFoundationDatabasePool, bool>) (p => p.DatabaseType == databaseType)).Select<TeamFoundationDatabasePool, string>((Func<TeamFoundationDatabasePool, string>) (p => p.PoolName)), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          using (requestContext.AcquireReaderLock(this.m_cacheLockName))
            databasePropertiesList1 = ((IEnumerable<ITeamFoundationDatabaseProperties>) this.m_databasePropertiesCache.Values.Where<InternalDatabaseProperties>((Func<InternalDatabaseProperties, bool>) (db => poolNames.Contains(db.PoolName)))).ToList<ITeamFoundationDatabaseProperties>();
        }
        else
        {
          List<InternalDatabaseProperties> databasePropertiesList2;
          using (DatabaseManagementComponent component = requestContext.CreateComponent<DatabaseManagementComponent>())
            databasePropertiesList2 = component.QueryDatabases(databaseType);
          databasePropertiesList1 = new List<ITeamFoundationDatabaseProperties>();
          foreach (ITeamFoundationDatabaseProperties updatedProperties in databasePropertiesList2)
            databasePropertiesList1.Add((ITeamFoundationDatabaseProperties) this.UpdateCache(requestContext, updatedProperties));
        }
        return databasePropertiesList1;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(99048, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(99049, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (QueryDatabases));
      }
    }

    public ITeamFoundationDatabaseProperties RegisterDatabase(
      IVssRequestContext requestContext,
      string connectionString,
      string databaseName,
      string serviceLevel,
      string poolName,
      int tenants,
      int maxTenants,
      TeamFoundationDatabaseStatus status,
      DateTime? statusChangedDate,
      string statusReason,
      DateTime? lastTenantAdded,
      bool registerCredential,
      string userId,
      byte[] passwordEncrypted,
      Guid signingKeyId,
      TeamFoundationDatabaseFlags flags,
      string serviceObjective = null)
    {
      requestContext.TraceEnter(99050, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (RegisterDatabase));
      try
      {
        requestContext.CheckDeploymentRequestContext();
        ArgumentUtility.CheckStringForNullOrEmpty(connectionString, nameof (connectionString));
        ArgumentUtility.CheckStringForNullOrEmpty(poolName, nameof (poolName));
        ArgumentUtility.CheckForOutOfRange(tenants, nameof (tenants), 0, int.MaxValue);
        ArgumentUtility.CheckForOutOfRange(maxTenants, nameof (maxTenants), 0, int.MaxValue);
        SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
        if (registerCredential)
        {
          ArgumentUtility.CheckStringForNullOrEmpty(userId, nameof (userId));
          ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) passwordEncrypted, nameof (passwordEncrypted));
          byte[] numArray = (byte[]) null;
          try
          {
            numArray = requestContext.GetService<ITeamFoundationSigningService>().Decrypt(requestContext, signingKeyId, passwordEncrypted, SigningAlgorithm.SHA256);
            Array.Clear((Array) numArray, 0, numArray.Length);
          }
          catch (CryptographicException ex)
          {
            throw new ArgumentException(nameof (passwordEncrypted), (Exception) ex);
          }
          finally
          {
            if (numArray != null)
              Array.Clear((Array) numArray, 0, numArray.Length);
          }
        }
        if (!string.IsNullOrWhiteSpace(databaseName) && !databaseName.Contains<char>(';'))
          throw new ArgumentException("Parameter databaseName should include the data tier and database name. Current: " + databaseName, nameof (databaseName));
        ITeamFoundationDatabaseProperties updatedProperties;
        using (DatabaseManagementComponent component = requestContext.CreateComponent<DatabaseManagementComponent>())
          updatedProperties = component.RegisterDatabase(connectionStringBuilder.ToString(), databaseName, serviceLevel, poolName, tenants, maxTenants, status, statusChangedDate ?? DateTime.UtcNow, statusReason, lastTenantAdded, registerCredential, userId, passwordEncrypted, signingKeyId, flags, serviceObjective);
        return (ITeamFoundationDatabaseProperties) this.UpdateCache(requestContext, updatedProperties);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(99058, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(99059, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (RegisterDatabase));
      }
    }

    public ITeamFoundationDatabaseProperties RegisterDatabase(
      IVssRequestContext requestContext,
      string connectionString,
      string databaseName,
      string serviceLevel,
      string poolName,
      int tenants,
      int maxTenants,
      TeamFoundationDatabaseStatus status,
      DateTime? statusChangedDate,
      string statusReason,
      DateTime? lastTenantAdded,
      bool registerCredential,
      string userId,
      byte[] passwordEncrypted,
      TeamFoundationDatabaseFlags flags)
    {
      Guid databaseSigningKey = requestContext.GetService<ITeamFoundationSigningService>().GetDatabaseSigningKey(requestContext.Elevate());
      return this.RegisterDatabase(requestContext, connectionString, databaseName, serviceLevel, poolName, tenants, maxTenants, status, statusChangedDate, statusReason, lastTenantAdded, registerCredential, userId, passwordEncrypted, databaseSigningKey, flags, (string) null);
    }

    public void ReleaseDatabasePartition(
      IVssRequestContext requestContext,
      int databaseId,
      bool partitionDeleted)
    {
      this.ReleaseDatabasePartition(requestContext, databaseId, partitionDeleted, false);
    }

    internal void ReleaseDatabasePartition(
      IVssRequestContext requestContext,
      int databaseId,
      bool partitionDeleted,
      bool decrementMaxSize)
    {
      requestContext.TraceEnter(99060, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (ReleaseDatabasePartition));
      try
      {
        requestContext.CheckDeploymentRequestContext();
        if (databaseId == -2)
          throw new VirtualServiceHostException();
        using (DatabaseManagementComponent component = requestContext.CreateComponent<DatabaseManagementComponent>())
          component.ReleaseDatabasePartition(databaseId, partitionDeleted, decrementMaxSize);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(99068, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(99069, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (ReleaseDatabasePartition));
      }
    }

    internal void SetDatabaseStatus(
      IVssRequestContext requestContext,
      int databaseId,
      TeamFoundationDatabaseStatus status,
      string statusReason)
    {
      requestContext.TraceEnter(99090, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (SetDatabaseStatus));
      try
      {
        if (databaseId == -2)
          throw new VirtualServiceHostException();
        this.UpdateDatabaseProperties(requestContext, databaseId, 3, (Action<TeamFoundationDatabaseProperties>) (properties =>
        {
          properties.Status = status;
          properties.StatusReason = statusReason;
        }));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(99098, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(99099, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (SetDatabaseStatus));
      }
    }

    public void RemoveDatabase(IVssRequestContext requestContext, int databaseId)
    {
      requestContext.TraceEnter(99100, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (RemoveDatabase));
      try
      {
        requestContext.CheckDeploymentRequestContext();
        if (databaseId == -2)
          throw new VirtualServiceHostException();
        using (DatabaseManagementComponent component = requestContext.CreateComponent<DatabaseManagementComponent>())
          component.RemoveDatabase(databaseId);
        this.RemoveFromCache(requestContext, databaseId);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(99108, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(99109, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (RemoveDatabase));
      }
    }

    public bool TryGetDatabaseProperties(
      IVssRequestContext requestContext,
      string connectionString,
      out ITeamFoundationDatabaseProperties databaseProperties)
    {
      requestContext.TraceEnter(99210, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (TryGetDatabaseProperties));
      try
      {
        requestContext.CheckDeploymentRequestContext();
        databaseProperties = (ITeamFoundationDatabaseProperties) null;
        SqlConnectionStringBuilder sb1 = new SqlConnectionStringBuilder(connectionString);
        foreach (ITeamFoundationDatabaseProperties queryDatabase in this.QueryDatabases(requestContext, true))
        {
          if (queryDatabase.ConnectionInfoWrapper != null)
          {
            SqlConnectionStringBuilder sb2 = new SqlConnectionStringBuilder(queryDatabase.ConnectionInfoWrapper.ConnectionString);
            if (this.AreConnectionStringsEquivalent(sb1, sb2))
            {
              databaseProperties = queryDatabase;
              return true;
            }
          }
        }
        return false;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(99218, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(99219, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (TryGetDatabaseProperties));
      }
    }

    public bool TryGetDatabaseProperties(
      IVssRequestContext requestContext,
      string datasource,
      string initialCatalog,
      out ITeamFoundationDatabaseProperties databaseProperties)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(datasource, nameof (datasource));
      ArgumentUtility.CheckStringForNullOrEmpty(initialCatalog, nameof (initialCatalog));
      requestContext.TraceEnter(99240, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (TryGetDatabaseProperties));
      try
      {
        requestContext.CheckDeploymentRequestContext();
        return TeamFoundationDatabaseManagementService.TryFindDatabaseProperties(this.QueryDatabases(requestContext, true), datasource, initialCatalog, out databaseProperties);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(99218, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(99219, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (TryGetDatabaseProperties));
      }
    }

    public static bool TryGetPartitionDatabasePropertiesRaw(
      ISqlConnectionInfo configDbConnectionInfo,
      string datasource,
      string initialCatalog,
      out ITeamFoundationDatabaseProperties databaseProperties)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(datasource, nameof (datasource));
      ArgumentUtility.CheckStringForNullOrEmpty(initialCatalog, nameof (initialCatalog));
      TeamFoundationTracingService.TraceEnterRaw(99340, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (TryGetPartitionDatabasePropertiesRaw), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      try
      {
        return TeamFoundationDatabaseManagementService.TryFindDatabaseProperties(TeamFoundationDatabaseManagementService.GetPartitionDatabasesRaw(configDbConnectionInfo), datasource, initialCatalog, out databaseProperties);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(99318, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, ex);
        throw;
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(99319, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (TryGetPartitionDatabasePropertiesRaw));
      }
    }

    private static string FormatDataSource(string dataSource)
    {
      DataSourceOptions options = DataSourceOptions.RemoveProtocol | DataSourceOptions.RemoveDefaultSqlPort;
      return TeamFoundationDataTierService.ManipulateDataSource(dataSource, options);
    }

    private bool AreConnectionStringsEquivalent(
      SqlConnectionStringBuilder sb1,
      SqlConnectionStringBuilder sb2)
    {
      return sb1.DataSource.Equals(sb2.DataSource) && sb1.InitialCatalog.Equals(sb2.InitialCatalog) && sb1.IntegratedSecurity == sb2.IntegratedSecurity && sb1.Encrypt == sb2.Encrypt;
    }

    private void UpdateDatabaseProperties(
      IVssRequestContext requestContext,
      TeamFoundationDatabaseProperties databaseProperties)
    {
      requestContext.TraceEnter(99110, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (UpdateDatabaseProperties));
      try
      {
        requestContext.CheckDeploymentRequestContext();
        ArgumentUtility.CheckForNull<TeamFoundationDatabaseProperties>(databaseProperties, nameof (databaseProperties));
        ITeamFoundationDatabaseProperties updatedProperties;
        using (DatabaseManagementComponent component = requestContext.CreateComponent<DatabaseManagementComponent>())
          updatedProperties = component.UpdateDatabase(databaseProperties);
        this.UpdateCache(requestContext, updatedProperties);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(99118, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(99119, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (UpdateDatabaseProperties));
      }
    }

    public virtual void UpdateDatabaseProperties(
      IVssRequestContext requestContext,
      int databaseId,
      Action<TeamFoundationDatabaseProperties> action)
    {
      this.UpdateDatabaseProperties(requestContext, databaseId, 5, action);
    }

    public void UpdateDatabaseProperties(
      IVssRequestContext requestContext,
      int databaseId,
      int retries,
      Action<TeamFoundationDatabaseProperties> action)
    {
      TeamFoundationDatabaseProperties databaseProperties = databaseId != -2 ? this.GetDatabase(requestContext, databaseId, true).GetEditableProperties() : throw new VirtualServiceHostException();
      while (true)
      {
        action(databaseProperties);
        try
        {
          this.UpdateDatabaseProperties(requestContext, databaseProperties);
          break;
        }
        catch (DatabasePropertiesStaleException ex)
        {
          if (--retries < 0)
          {
            requestContext.Trace(99111, TraceLevel.Error, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, TeamFoundationDatabaseManagementService.s_stalePropertiesTrace, (object) databaseProperties.DatabaseId, (object) databaseProperties.Version, (object) ex);
            throw;
          }
          else
          {
            requestContext.Trace(99112, TraceLevel.Info, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, TeamFoundationDatabaseManagementService.s_stalePropertiesTrace + " Attempting cache refresh and retry.", (object) databaseProperties.DatabaseId, (object) databaseProperties.Version, (object) ex);
            databaseProperties = this.GetDatabase(requestContext, databaseId, false).GetEditableProperties();
          }
        }
      }
    }

    public void FlushDatabaseCache(IVssRequestContext requestContext)
    {
      using (DatabaseManagementComponent component = requestContext.CreateComponent<DatabaseManagementComponent>())
        component.FlushDatabaseCache();
      this.QueryDatabases(requestContext, false);
    }

    public List<TeamFoundationDatabaseTenantUsage> QueryDatabaseUsage(
      IVssRequestContext requestContext,
      int databaseId)
    {
      return this.QueryDatabaseUsage(requestContext, databaseId, true);
    }

    public List<TeamFoundationDatabaseTenantUsage> QueryDatabaseUsage(
      IVssRequestContext requestContext,
      int databaseId,
      bool includeHostInfo)
    {
      requestContext.TraceEnter(99150, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (QueryDatabaseUsage));
      try
      {
        if (databaseId == -2)
          throw new VirtualServiceHostException();
        List<TeamFoundationDatabaseTenantUsage> items;
        using (DatabasePartitionComponent component = DatabasePartitionComponent.CreateComponent(this.GetSqlConnectionInfo(requestContext, databaseId)))
          items = component.QueryPartitionUsage(new int?()).GetCurrent<TeamFoundationDatabaseTenantUsage>().Items;
        if (includeHostInfo)
        {
          TeamFoundationHostManagementService service = requestContext.GetService<TeamFoundationHostManagementService>();
          foreach (TeamFoundationDatabaseTenantUsage databaseTenantUsage in items)
          {
            HostProperties hostProperties = service.QueryServiceHostPropertiesCached(requestContext, databaseTenantUsage.TenantHostId);
            if (hostProperties != null)
            {
              databaseTenantUsage.TenantName = hostProperties.Name;
              databaseTenantUsage.LastAccess = databaseTenantUsage.LastAccess != DateTime.MinValue ? databaseTenantUsage.LastAccess : hostProperties.LastUserAccess;
            }
          }
        }
        return items;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(99158, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(99159, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (QueryDatabaseUsage));
      }
    }

    public QueryStoreOptions GetQueryStoreOptions(
      IVssRequestContext requestContext,
      string databaseName)
    {
      using (TeamFoundationDataTierComponent componentRaw = this.GetDatabaseForQueryStore(requestContext, databaseName).SqlConnectionInfo.CreateComponentRaw<TeamFoundationDataTierComponent>())
        return componentRaw.GetQueryStoreOptions();
    }

    public QueryStoreOptions ClearQueryStore(IVssRequestContext requestContext, string databaseName)
    {
      ITeamFoundationDatabaseProperties databaseForQueryStore = this.GetDatabaseForQueryStore(requestContext, databaseName);
      using (TeamFoundationDataTierComponent componentRaw = requestContext.GetService<TeamFoundationDataTierService>().FindAssociatedDataTier(requestContext, databaseForQueryStore.ConnectionInfoWrapper.ConnectionString).ConnectionInfo.CloneReplaceInitialCatalog(databaseName).CreateComponentRaw<TeamFoundationDataTierComponent>())
      {
        componentRaw.ClearQueryStore();
        return componentRaw.GetQueryStoreOptions();
      }
    }

    public QueryStoreOptions DisableQueryStore(
      IVssRequestContext requestContext,
      string databaseName)
    {
      ITeamFoundationDatabaseProperties databaseForQueryStore = this.GetDatabaseForQueryStore(requestContext, databaseName);
      using (TeamFoundationDataTierComponent componentRaw = requestContext.GetService<TeamFoundationDataTierService>().FindAssociatedDataTier(requestContext, databaseForQueryStore.ConnectionInfoWrapper.ConnectionString).ConnectionInfo.CloneReplaceInitialCatalog(databaseName).CreateComponentRaw<TeamFoundationDataTierComponent>())
      {
        componentRaw.DisableQueryStore();
        return componentRaw.GetQueryStoreOptions();
      }
    }

    public QueryStoreOptions SetQueryStoreOptions(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.Framework.Server.SetQueryStoreOptions queryStoreOptions)
    {
      ITeamFoundationDatabaseProperties databaseForQueryStore = this.GetDatabaseForQueryStore(requestContext, queryStoreOptions.DatabaseName);
      using (TeamFoundationDataTierComponent componentRaw = requestContext.GetService<TeamFoundationDataTierService>().FindAssociatedDataTier(requestContext, databaseForQueryStore.ConnectionInfoWrapper.ConnectionString).ConnectionInfo.CloneReplaceInitialCatalog(queryStoreOptions.DatabaseName).CreateComponentRaw<TeamFoundationDataTierComponent>())
      {
        componentRaw.SetQueryStoreOptions(queryStoreOptions);
        return componentRaw.GetQueryStoreOptions();
      }
    }

    public List<TableSizeProperties> QueryDatabaseTableSizeDetailed(
      IVssRequestContext requestContext,
      int databaseId)
    {
      requestContext.TraceEnter(99170, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (QueryDatabaseTableSizeDetailed));
      try
      {
        if (databaseId == -2)
          throw new VirtualServiceHostException();
        using (TeamFoundationDataTierComponent componentRaw = this.GetDatabase(requestContext, databaseId, true).SqlConnectionInfo.CreateComponentRaw<TeamFoundationDataTierComponent>())
          return componentRaw.GetTablesBySizeInDescendingOrder();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(99170, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(99170, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (QueryDatabaseTableSizeDetailed));
      }
    }

    public List<TeamFoundationDatabaseTableUsage> QueryDatabaseUsageDetailed(
      IVssRequestContext requestContext,
      int databaseId,
      int? tenantPartitionId,
      out List<TeamFoundationDatabaseTenantUsage> tenantUsages)
    {
      requestContext.TraceEnter(99150, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, "QueryTenantUsageDetailed");
      try
      {
        ITeamFoundationDatabaseProperties database = this.GetDatabase(requestContext, databaseId, true);
        if (database.PoolName == DatabaseManagementConstants.CollectionExportPool || database.PoolName == DatabaseManagementConstants.CollectionStagingPool)
          throw new ArgumentException(FrameworkResources.DatabaseWrongPool((object) database.PoolName));
        using (DatabasePartitionComponent5 component = (DatabasePartitionComponent5) DatabasePartitionComponent.CreateComponent(database.SqlConnectionInfo))
        {
          component.CommandTimeout = 3600;
          ResultCollection resultCollection = component.QueryPartitionUsageDetailed(tenantPartitionId);
          tenantUsages = resultCollection.GetCurrent<TeamFoundationDatabaseTenantUsage>().Items;
          TeamFoundationHostManagementService service = requestContext.GetService<TeamFoundationHostManagementService>();
          foreach (TeamFoundationDatabaseTenantUsage databaseTenantUsage in tenantUsages)
          {
            HostProperties hostProperties = service.QueryServiceHostPropertiesCached(requestContext, databaseTenantUsage.TenantHostId);
            if (hostProperties != null)
              databaseTenantUsage.TenantName = hostProperties.Name;
          }
          resultCollection.NextResult();
          return resultCollection.GetCurrent<TeamFoundationDatabaseTableUsage>().Items;
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(99158, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(99159, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, "QueryTenantUsageDetailed");
      }
    }

    public List<TeamFoundationDatabasePartitionTableUsage> QueryDatabaseUsageEstimated(
      IVssRequestContext requestContext,
      int databaseId,
      int tenantPartitionId,
      out TeamFoundationDatabasePartitionUsage tenantUsage)
    {
      requestContext.TraceEnter(99250, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (QueryDatabaseUsageEstimated));
      try
      {
        using (DatabasePartitionComponent component = DatabasePartitionComponent.CreateComponent(this.GetDatabase(requestContext, databaseId, true).DboConnectionInfo))
        {
          component.CommandTimeout = 3600;
          ResultCollection resultCollection = component.QueryPartitionUsageEstimated(tenantPartitionId);
          tenantUsage = resultCollection.GetCurrent<TeamFoundationDatabasePartitionUsage>().Items.FirstOrDefault<TeamFoundationDatabasePartitionUsage>();
          resultCollection.NextResult();
          return resultCollection.GetCurrent<TeamFoundationDatabasePartitionTableUsage>().Items;
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(99258, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(99259, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (QueryDatabaseUsageEstimated));
      }
    }

    public ITeamFoundationDatabaseProperties CopyDatabase(
      IVssRequestContext requestContext,
      int sourceDatabaseId,
      string destinationDatabaseName)
    {
      return this.CopyDatabase(requestContext, sourceDatabaseId, destinationDatabaseName, TimeSpan.FromHours(24.0));
    }

    public ITeamFoundationDatabaseProperties CopyDatabase(
      IVssRequestContext requestContext,
      int sourceDatabaseId,
      string destinationDatabaseName,
      TimeSpan timeout)
    {
      ITeamFoundationDatabaseProperties destinationDatabase = this.BeginCopyDatabase(requestContext, sourceDatabaseId, destinationDatabaseName);
      this.WaitTillCopyDatabaseCompletes(requestContext, destinationDatabase, timeout, true);
      return destinationDatabase;
    }

    internal ITeamFoundationDatabaseProperties BeginCopyDatabase(
      IVssRequestContext requestContext,
      int sourceDatabaseId,
      string destinationDatabaseName)
    {
      return this.BeginCopyDatabase(requestContext, sourceDatabaseId, (DataTierInfo) null, destinationDatabaseName);
    }

    private ITeamFoundationDatabaseProperties BeginCopyDatabase(
      IVssRequestContext requestContext,
      int sourceDatabaseId,
      DataTierInfo destinationDataTier,
      string destinationDatabaseName)
    {
      requestContext.TraceEnter(99150, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (BeginCopyDatabase));
      try
      {
        ArgumentUtility.CheckStringForNullOrEmpty(destinationDatabaseName, nameof (destinationDatabaseName));
        ITeamFoundationDatabaseProperties databaseProperties1 = sourceDatabaseId != -2 ? this.GetDatabase(requestContext, sourceDatabaseId, true) : throw new VirtualServiceHostException();
        if (destinationDataTier == null)
          destinationDataTier = requestContext.GetService<TeamFoundationDataTierService>().FindAssociatedDataTier(requestContext, databaseProperties1.ConnectionInfoWrapper.ConnectionString);
        if (string.Equals(databaseProperties1.DatabaseName, destinationDatabaseName, StringComparison.OrdinalIgnoreCase))
          throw new ArgumentException(FrameworkResources.SourceDestinationDatabaseNameMustDiffer((object) destinationDatabaseName));
        SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(databaseProperties1.ConnectionInfoWrapper.ConnectionString)
        {
          DataSource = destinationDataTier.DataSource,
          InitialCatalog = destinationDatabaseName
        };
        string dataSource = TeamFoundationDataTierService.GetDataSource(databaseProperties1.ConnectionInfoWrapper.ConnectionString, DataSourceOptions.RemoveProtocolAndDomain);
        string databaseName = TeamFoundationDataTierService.GetDatabaseName(databaseProperties1.ConnectionInfoWrapper.ConnectionString);
        string statusReason1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, TeamFoundationDatabaseManagementService.s_BeginCopyStatusReasonFormatString, (object) dataSource, (object) databaseName);
        ITeamFoundationDatabaseProperties databaseProperties2 = this.RegisterDatabase(requestContext, connectionStringBuilder.ConnectionString, (string) null, databaseProperties1.ServiceLevel, databaseProperties1.PoolName, databaseProperties1.Tenants, databaseProperties1.MaxTenants, TeamFoundationDatabaseStatus.Creating, new DateTime?(), statusReason1, new DateTime?(databaseProperties1.LastTenantAdded), true, databaseProperties1.ConnectionInfoWrapper.UserId, Encoding.UTF8.GetBytes(databaseProperties1.ConnectionInfoWrapper.PasswordEncrypted), databaseProperties1.ConnectionInfoWrapper.SigningKeyId, databaseProperties1.Flags, (string) null);
        try
        {
          requestContext.Trace(99151, TraceLevel.Info, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, "Beginning copy database operation. DestinationDatabaseId: {0}.", (object) databaseProperties2.DatabaseId);
          using (DatabaseManagementSqlInstanceOperationsComponent componentRaw = destinationDataTier.ConnectionInfo.CreateComponentRaw<DatabaseManagementSqlInstanceOperationsComponent>())
            componentRaw.BeginCopyDatabase(dataSource, databaseName, destinationDatabaseName);
        }
        catch (Exception ex)
        {
          string statusReason2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, TeamFoundationDatabaseManagementService.s_FailedBeginCopyStatusReasonFormatString, (object) dataSource, (object) databaseName, (object) ex.ToReadableStackTrace());
          this.SetDatabaseStatus(requestContext, databaseProperties2.DatabaseId, TeamFoundationDatabaseStatus.Failed, statusReason2);
          requestContext.Trace(99152, TraceLevel.Error, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, "Completed copy database operation. (Immediate Failure.) DestinationDatabaseId: {0}.", (object) databaseProperties2.DatabaseId);
          throw;
        }
        return databaseProperties2;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(99158, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(99159, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (BeginCopyDatabase));
      }
    }

    internal void WaitTillCopyDatabaseCompletes(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties destinationDatabaseId)
    {
      this.WaitTillCopyDatabaseCompletes(requestContext, destinationDatabaseId, TimeSpan.FromHours(24.0), true);
    }

    internal bool WaitTillCopyDatabaseCompletes(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties destinationDatabase,
      TimeSpan timeout,
      bool throwOnTimeout)
    {
      requestContext.TraceEnter(99160, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (WaitTillCopyDatabaseCompletes));
      try
      {
        int databaseId = destinationDatabase.DatabaseId;
        if (databaseId == -2)
          throw new VirtualServiceHostException();
        DateTime dateTime1 = destinationDatabase.Status == TeamFoundationDatabaseStatus.Creating ? destinationDatabase.StatusChangedDate : throw new ArgumentException(FrameworkResources.DatabaseIsNotBeingCopied((object) databaseId, (object) destinationDatabase.Status));
        if (dateTime1 > DateTime.UtcNow)
        {
          requestContext.Trace(99161, TraceLevel.Error, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, "Database StatusChangedDate is in the future.");
          dateTime1 = DateTime.UtcNow;
        }
        SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(destinationDatabase.ConnectionInfoWrapper.ConnectionString);
        string dataSource = connectionStringBuilder.DataSource;
        string initialCatalog = connectionStringBuilder.InitialCatalog;
        DataTierInfo associatedDataTier = requestContext.GetService<TeamFoundationDataTierService>().FindAssociatedDataTier(requestContext, destinationDatabase.ConnectionInfoWrapper.ConnectionString);
        DateTime dateTime2 = DateTime.UtcNow.AddMinutes(1.0);
        DateTime dateTime3 = dateTime1.AddMinutes(60.0);
        DateTime dateTime4 = DateTime.UtcNow + timeout;
        SqlInstanceDatabaseState? databaseState;
        while (true)
        {
          using (DatabaseManagementSqlInstanceOperationsComponent componentRaw = associatedDataTier.ConnectionInfo.CreateComponentRaw<DatabaseManagementSqlInstanceOperationsComponent>())
          {
            databaseState = componentRaw.GetDatabaseState(initialCatalog);
            requestContext.Trace(99162, TraceLevel.Verbose, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, "Destination database id: {0}. database state: {1}", (object) databaseId, (object) databaseState);
            if (databaseState.Value == SqlInstanceDatabaseState.Copying)
            {
              if (!(DateTime.UtcNow > dateTime2))
              {
                if (!(DateTime.UtcNow > dateTime3))
                  goto label_19;
              }
              TeamFoundationDatabaseCopyStatus copyStatus = componentRaw.GetCopyStatus(initialCatalog);
              if (DateTime.UtcNow > dateTime2)
              {
                requestContext.Trace(99163, TraceLevel.Info, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, "Database copy elapsed time: {0}. database id: {1} database state: {2}.  Copy status: {3}.", (object) (DateTime.UtcNow - dateTime1), (object) databaseId, (object) databaseState, (object) copyStatus);
                dateTime2 = dateTime2.AddMinutes(1.0);
              }
              if (DateTime.UtcNow > dateTime3)
              {
                requestContext.Trace(99164, TraceLevel.Warning, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, "Database copy taking over an hour. Elapsed time: {0}. database id: {1} database state: {2}.  Copy status: {3}.", (object) (DateTime.UtcNow - dateTime1), (object) databaseId, (object) databaseState, (object) copyStatus);
                dateTime3 = dateTime3.AddMinutes(60.0);
              }
            }
            else
              break;
          }
label_19:
          if (!(DateTime.UtcNow > dateTime4))
            Thread.Sleep(5000);
          else
            break;
        }
        if (databaseState.Value == SqlInstanceDatabaseState.Copying)
        {
          if (throwOnTimeout)
          {
            requestContext.Trace(99165, TraceLevel.Warning, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, "WaitTillCopyDatabaseCompletes timed out. Timeout specified: {0}. Database copy queued: {1}.", (object) timeout, (object) dateTime1);
            throw new TimeoutException(FrameworkResources.DatabaseCopyTimeoutError((object) databaseId, (object) timeout.TotalMinutes));
          }
          return false;
        }
        if (databaseState.Value != SqlInstanceDatabaseState.Online)
        {
          TeamFoundationDatabaseCopyException databaseCopyException;
          using (DatabaseManagementSqlInstanceOperationsComponent componentRaw = associatedDataTier.ConnectionInfo.CreateComponentRaw<DatabaseManagementSqlInstanceOperationsComponent>())
          {
            TeamFoundationDatabaseCopyStatus copyStatus = componentRaw.GetCopyStatus(initialCatalog);
            databaseCopyException = copyStatus == null ? new TeamFoundationDatabaseCopyException(databaseId, dataSource, initialCatalog) : new TeamFoundationDatabaseCopyException(databaseId, dataSource, initialCatalog, copyStatus.ErrorDescription);
          }
          this.SetDatabaseStatus(requestContext, databaseId, TeamFoundationDatabaseStatus.Failed, databaseCopyException.Message);
          requestContext.Trace(99166, TraceLevel.Error, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, "Completed copy database operation. (Eventual Failure.) DestinationDatabaseId: {1}.", (object) databaseId);
          throw databaseCopyException;
        }
        this.SetDatabaseStatus(requestContext, databaseId, TeamFoundationDatabaseStatus.Servicing, TeamFoundationDatabaseManagementService.s_CopyCompleteStatusReason);
        requestContext.Trace(99167, TraceLevel.Info, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, "Completed copy database operation. (Success.) DestinationDatabaseId: {1}.", (object) databaseId);
        return true;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(99168, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(99169, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (WaitTillCopyDatabaseCompletes));
      }
    }

    internal static TeamFoundationLock AcquireDbmsDeletionLock(
      IVssRequestContext requestContext,
      int databaseId)
    {
      TeamFoundationLockInfo lockInfo = databaseId != -2 ? new TeamFoundationLockInfo()
      {
        LockMode = TeamFoundationLockMode.Exclusive,
        LockName = DatabaseManagementConstants.PartitionDeletionLockPrefix + databaseId.ToString(),
        LockTimeout = -1
      } : throw new VirtualServiceHostException();
      return requestContext.GetService<ITeamFoundationLockingService>().AcquireLock(requestContext, lockInfo);
    }

    private static bool TryFindDatabaseProperties(
      List<ITeamFoundationDatabaseProperties> databases,
      string datasource,
      string initialCatalog,
      out ITeamFoundationDatabaseProperties databaseProperties)
    {
      foreach (ITeamFoundationDatabaseProperties database in databases)
      {
        if (database.SqlConnectionInfo != null && VssStringComparer.DataSource.Equals(TeamFoundationDatabaseManagementService.FormatDataSource(database.SqlConnectionInfo.DataSource), TeamFoundationDatabaseManagementService.FormatDataSource(datasource)) && VssStringComparer.DatabaseName.Equals(database.SqlConnectionInfo.InitialCatalog, initialCatalog))
        {
          databaseProperties = database;
          return true;
        }
      }
      databaseProperties = (ITeamFoundationDatabaseProperties) null;
      return false;
    }

    public void CreateDatabasePool(
      IVssRequestContext requestContext,
      TeamFoundationDatabaseType type,
      TeamFoundationDatabaseCredentialPolicy credentialPolicy,
      string collation,
      string poolName,
      int initialCapacity,
      int createThreshold,
      int growBy,
      string servicingOperations,
      int maxDatabaseLimit,
      string deltaOperationPrefixes,
      TeamFoundationDatabasePoolFlags flags,
      string serviceObjective)
    {
      requestContext.TraceEnter(99120, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (CreateDatabasePool));
      try
      {
        ArgumentUtility.CheckStringForNullOrEmpty(poolName, nameof (poolName));
        ArgumentUtility.CheckForOutOfRange(initialCapacity, nameof (initialCapacity), 0, int.MaxValue);
        ArgumentUtility.CheckForOutOfRange(createThreshold, nameof (createThreshold), 0, int.MaxValue);
        ArgumentUtility.CheckForOutOfRange(growBy, nameof (growBy), 0, int.MaxValue);
        ArgumentUtility.CheckForOutOfRange(maxDatabaseLimit, nameof (maxDatabaseLimit), 0, int.MaxValue);
        TeamFoundationDatabasePool pool = new TeamFoundationDatabasePool();
        pool.Initialize(DatabaseManagementConstants.UnassignedPoolId, type, credentialPolicy, collation, poolName, initialCapacity, createThreshold, growBy, servicingOperations, maxDatabaseLimit, deltaOperationPrefixes, flags, serviceObjective);
        using (DatabaseManagementComponent component = requestContext.CreateComponent<DatabaseManagementComponent>())
          component.CreateDatabasePool(pool);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(99128, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(99129, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (CreateDatabasePool));
      }
    }

    public void DeleteDatabasePool(IVssRequestContext requestContext, string poolName)
    {
      requestContext.TraceEnter(99180, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (DeleteDatabasePool));
      try
      {
        using (DatabaseManagementComponent component = requestContext.CreateComponent<DatabaseManagementComponent>())
          component.DeleteDatabasePool(poolName);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(99188, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(99189, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (DeleteDatabasePool));
      }
    }

    public IEnumerable<TeamFoundationDatabasePool> GetDatabasePoolsToGrow(
      IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(99210, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (GetDatabasePoolsToGrow));
      try
      {
        using (DatabaseManagementComponent component = requestContext.CreateComponent<DatabaseManagementComponent>())
        {
          using (ResultCollection databasePoolsToGrow = component.GetDatabasePoolsToGrow())
            return (IEnumerable<TeamFoundationDatabasePool>) databasePoolsToGrow.GetCurrent<TeamFoundationDatabasePool>().Items;
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(99218, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(99219, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (GetDatabasePoolsToGrow));
      }
    }

    public List<TeamFoundationDatabasePool> QueryDatabasePools(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(99130, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (QueryDatabasePools));
      try
      {
        using (DatabaseManagementComponent component = requestContext.CreateComponent<DatabaseManagementComponent>())
        {
          using (ResultCollection resultCollection = component.QueryDatabasePools())
            return resultCollection.GetCurrent<TeamFoundationDatabasePool>().Items;
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(99138, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(99139, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (QueryDatabasePools));
      }
    }

    public void UpdateDatabasePool(
      IVssRequestContext requestContext,
      TeamFoundationDatabasePool databasePool)
    {
      requestContext.TraceEnter(99140, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, "UpdatePoolDefinition");
      try
      {
        ArgumentUtility.CheckForNull<TeamFoundationDatabasePool>(databasePool, nameof (databasePool));
        using (DatabaseManagementComponent component = requestContext.CreateComponent<DatabaseManagementComponent>())
          component.UpdateDatabasePool(databasePool);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(99148, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(99149, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, "UpdatePoolDefinition");
      }
    }

    public List<KeyValue<Exception, string>> UpdateDatabaseSizeAndSetStats(
      IVssRequestContext requestContext,
      out string resultMessage)
    {
      resultMessage = (string) null;
      IKpiService service = requestContext.GetService<IKpiService>();
      requestContext.GetService<TeamFoundationDataTierService>();
      TeamFoundationDatabaseManagementService.ResizeSettings resizeSettings = new TeamFoundationDatabaseManagementService.ResizeSettings(requestContext);
      int num1 = 0;
      int num2 = 0;
      int num3 = 0;
      List<KeyValue<Exception, string>> keyValueList = new List<KeyValue<Exception, string>>();
      foreach (ITeamFoundationDatabaseProperties databaseProperties1 in this.QueryDatabases(requestContext, true).Where<ITeamFoundationDatabaseProperties>((Func<ITeamFoundationDatabaseProperties, bool>) (db => !db.IsExternalDatabase() && !TeamFoundationDatabaseManagementService.s_databasePoolsThatShouldNotPublishKpi.Contains(db.PoolName))))
      {
        try
        {
          string str = (string) null;
          DatabaseReplicationContext replicationContext = DatabaseReplicationContext.Default;
          if (requestContext.IsFeatureEnabled(FrameworkServerConstants.SqlFailoverGroupEnabled))
            replicationContext = requestContext.GetService<IDatabaseFailoverGroupService>().GetDatabaseReplicationContext(requestContext, databaseProperties1);
          if (replicationContext != DatabaseReplicationContext.Default)
            str = replicationContext.PrimaryServerName;
          string scope = str != null ? str + ";" + databaseProperties1.SqlConnectionInfo.InitialCatalog : databaseProperties1.DatabaseName;
          List<KpiMetric> metrics1 = new List<KpiMetric>(3)
          {
            new KpiMetric()
            {
              Name = "DatabaseTenants",
              Value = (double) databaseProperties1.Tenants
            },
            new KpiMetric()
            {
              Name = "DatabaseTenantsPendingDelete",
              Value = (double) databaseProperties1.TenantsPendingDelete
            },
            new KpiMetric()
            {
              Name = "DatabaseMaxTenants",
              Value = (double) databaseProperties1.MaxTenants
            }
          };
          service.EnsureKpiIsRegistered(requestContext, KpiAreas.DatabaseCapacityMetrics, "DatabaseTenants", scope, "Database Tenants");
          service.EnsureKpiIsRegistered(requestContext, KpiAreas.DatabaseCapacityMetrics, "DatabaseTenantsPendingDelete", scope, "Database Tenants Pending Delete");
          service.EnsureKpiIsRegistered(requestContext, KpiAreas.DatabaseCapacityMetrics, "DatabaseMaxTenants", scope, "Database Max Tenants");
          service.Publish(requestContext, KpiAreas.DatabaseCapacityMetrics, scope, metrics1);
          if (databaseProperties1.PoolName == DatabaseManagementConstants.DefaultPartitionPoolName || databaseProperties1.PoolName == DatabaseManagementConstants.RestrictedAcquisitionPartitionPool)
          {
            num1 += databaseProperties1.Tenants;
            num2 += databaseProperties1.TenantsPendingDelete;
            num3 += databaseProperties1.MaxTenants;
          }
          if (databaseProperties1.ConnectionInfoWrapper == null || !databaseProperties1.ConnectionInfoWrapper.IsValidSecurityConfiguration)
          {
            requestContext.Trace(1012503, TraceLevel.Warning, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, "Cannot connect to database {0} (Id = {1}). The credentials are not valid.", (object) databaseProperties1.DatabaseName, (object) databaseProperties1.DatabaseId);
          }
          else
          {
            AzureDatabaseProperties databaseProperties2 = this.GetDatabaseProperties(databaseProperties1.GetDboConnectionInfo());
            if (databaseProperties2 != null)
            {
              List<KpiMetric> metrics2 = new List<KpiMetric>()
              {
                new KpiMetric()
                {
                  Name = "DatabaseSizeMB",
                  Value = (double) databaseProperties2.SizeInMB
                }
              };
              service.EnsureKpiIsRegistered(requestContext, KpiAreas.DatabaseSizeMetrics, "DatabaseSizeMB", scope, "Database Size");
              if (databaseProperties2.Edition != DatabaseServiceEdition.HyperScale)
              {
                TeamFoundationDatabaseManagementService.DatabaseSizeProperties sizeProperties = new TeamFoundationDatabaseManagementService.DatabaseSizeProperties(databaseProperties2, resizeSettings);
                int newDatabaseSize = this.GetNewDatabaseSize(requestContext, databaseProperties1, sizeProperties);
                if (newDatabaseSize != 0 && newDatabaseSize != sizeProperties.CurrentMaxSizeInGB)
                  this.AlterDBSize(requestContext, databaseProperties1, newDatabaseSize, sizeProperties);
                this.UpdateDBPropertySizes(requestContext, databaseProperties1, sizeProperties);
                metrics2.Insert(1, new KpiMetric()
                {
                  Name = "DatabaseCapacityMB",
                  Value = (double) databaseProperties2.CurrentMaxSizeInMB
                });
                service.EnsureKpiIsRegistered(requestContext, KpiAreas.DatabaseSizeMetrics, "DatabaseCapacityMB", scope, "Database Capacity");
                if (sizeProperties.CurrentToMaxSizeRatio > 0.0)
                {
                  metrics2.Insert(2, new KpiMetric()
                  {
                    Name = "DatabaseSizeRatio",
                    Value = sizeProperties.CurrentToMaxSizeRatio
                  });
                  metrics2.Insert(3, new KpiMetric()
                  {
                    Name = "MaxDatabaseSizeRatio",
                    Value = sizeProperties.CurrentToMaxSupportedRatio
                  });
                  service.EnsureKpiIsRegistered(requestContext, KpiAreas.DatabaseSizeMetrics, "DatabaseSizeRatio", scope, "Database Size Ratio");
                  service.EnsureKpiIsRegistered(requestContext, KpiAreas.DatabaseSizeMetrics, "MaxDatabaseSizeRatio", scope, "Max Supported Database Size Ratio");
                }
                if (databaseProperties1.SizeChangedDate > DateTime.MinValue)
                {
                  double num4 = ((double) databaseProperties2.SizeInMB - (double) databaseProperties1.DatabaseSize) / (DateTime.UtcNow - databaseProperties1.SizeChangedDate).TotalHours;
                  if (num4 < 0.0)
                  {
                    requestContext.TraceAlways(1012504, TraceLevel.Info, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, "Database: {0}, Id = {1}. Growth Rate: {2}", (object) databaseProperties1.DatabaseName, (object) databaseProperties1.DatabaseId, (object) num4);
                    num4 = 0.0;
                  }
                  metrics2.Add(new KpiMetric()
                  {
                    Name = "DatabaseGrowthRate",
                    Value = num4
                  });
                  service.EnsureKpiIsRegistered(requestContext, KpiAreas.DatabaseMetrics, "DatabaseGrowthRate", scope, "Database Growth Rate");
                }
              }
              service.Publish(requestContext, KpiAreas.DatabaseSizeMetrics, scope, metrics2);
            }
            else
              requestContext.Trace(1012500, TraceLevel.Warning, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, "Did not find database size properties for database {0} with id {1}.", (object) databaseProperties1.DatabaseName, (object) databaseProperties1.DatabaseId);
          }
        }
        catch (Exception ex)
        {
          requestContext.TraceException(1012508, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, ex);
          if (keyValueList.Count < 100)
            keyValueList.Add(new KeyValue<Exception, string>()
            {
              Key = ex,
              Value = databaseProperties1.DatabaseName
            });
        }
      }
      if (num1 > 0 && num3 > 0)
      {
        double num5 = (double) num1 / (double) num3;
        long num6 = (long) (100 * num1 / num3);
        List<KpiMetric> metrics = new List<KpiMetric>(4)
        {
          new KpiMetric()
          {
            Name = "PartitionDatabasesTotalMaxTenants",
            Value = (double) num3
          },
          new KpiMetric()
          {
            Name = "PartitionDatabasesTotalTenants",
            Value = (double) num1
          },
          new KpiMetric()
          {
            Name = "PartitionDatabasesTotalTenantsPendingDelete",
            Value = (double) num2
          },
          new KpiMetric()
          {
            Name = "PartitionDatabasesFillRate",
            Value = (double) num6
          }
        };
        service.Publish(requestContext, KpiAreas.DatabaseCapacityMetrics, metrics);
      }
      if (keyValueList.Count > 0)
      {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (KeyValue<Exception, string> keyValue in keyValueList)
        {
          Exception key = keyValue.Key;
          string str = keyValue.Value;
          stringBuilder.AppendLine(str);
          stringBuilder.AppendLine(key.ToReadableStackTrace());
          stringBuilder.AppendLine("-----------------------------------");
        }
        resultMessage = stringBuilder.ToString();
      }
      return keyValueList;
    }

    public static bool ShouldSealDatabase(
      ITeamFoundationDatabaseProperties database,
      double supportedUsageRatio,
      double usageThreshold)
    {
      return database.MaxTenants > database.Tenants && supportedUsageRatio > usageThreshold;
    }

    public bool IsDowngradeOrDownsizeDisabled(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties database,
      out DateTime downgradeDisabledUntilTime)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string str = string.Format(FrameworkServerConstants.DisableDatabaseDownsizeDuringMigrationsUntil, (object) database.DatabaseId);
      IVssRequestContext requestContext1 = requestContext;
      // ISSUE: explicit reference operation
      ref RegistryQuery local = @(RegistryQuery) str;
      string s = service.GetValue(requestContext1, in local, (string) null);
      downgradeDisabledUntilTime = DateTime.MinValue;
      return !string.IsNullOrEmpty(s) && DateTime.TryParseExact(s, "yyyy-MM-ddTHH:mm:ss", (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out downgradeDisabledUntilTime) && downgradeDisabledUntilTime > DateTime.UtcNow;
    }

    public void EnsureDatabaseMaintenanceJobCreated(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties database)
    {
      requestContext.CheckDeploymentRequestContext();
      requestContext.CheckHostedDeployment();
      TeamFoundationJobDefinition foundationJobDefinition = new TeamFoundationJobDefinition(database.MaintenanceJobId, "Team Foundation Server Database Optimization", "Microsoft.TeamFoundation.JobService.Extensions.Core.DatabaseOptimizationJob", (XmlNode) null)
      {
        DisableDuringUpgrade = true
      };
      foundationJobDefinition.Schedule.Add(new TeamFoundationJobSchedule()
      {
        ScheduledTime = DateTime.Parse("2011-07-02T09:00:00.0000000Z"),
        Interval = (int) TimeSpan.FromDays(7.0).TotalSeconds
      });
      requestContext.GetService<ITeamFoundationJobService>().UpdateJobDefinitions(requestContext, (IEnumerable<TeamFoundationJobDefinition>) new TeamFoundationJobDefinition[1]
      {
        foundationJobDefinition
      });
    }

    internal virtual int GetAllocatedStorageInGB(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties database)
    {
      DataTierInfo associatedDataTier = requestContext.GetService<TeamFoundationDataTierService>().FindAssociatedDataTier(requestContext, database.ConnectionInfoWrapper.ConnectionString);
      string databaseName = TeamFoundationDataTierService.GetDatabaseName(database.ConnectionInfoWrapper.ConnectionString);
      using (TeamFoundationDataTierComponent componentRaw = associatedDataTier.ConnectionInfo.CreateComponentRaw<TeamFoundationDataTierComponent>())
        return componentRaw.GetAllocatedStorageInGB(databaseName);
    }

    internal virtual void AlterSqlAzureDatabaseMaxSize(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties database,
      int newMaxSizeInGB)
    {
      DataTierInfo associatedDataTier = requestContext.GetService<TeamFoundationDataTierService>().FindAssociatedDataTier(requestContext, database.ConnectionInfoWrapper.ConnectionString);
      string databaseName = TeamFoundationDataTierService.GetDatabaseName(database.ConnectionInfoWrapper.ConnectionString);
      using (TeamFoundationDataTierComponent componentRaw = associatedDataTier.ConnectionInfo.CreateComponentRaw<TeamFoundationDataTierComponent>())
        componentRaw.AlterSqlAzureDatabaseMaxSize(databaseName, newMaxSizeInGB);
    }

    internal static string NormalizeDbNameForLogUsageKpi(
      string summaryDatabaseName,
      string databaseName)
    {
      if (summaryDatabaseName.Equals(TeamFoundationSqlResourceComponent.Master, StringComparison.OrdinalIgnoreCase) || summaryDatabaseName.Equals("tempdb", StringComparison.OrdinalIgnoreCase) || summaryDatabaseName.Equals("msdb", StringComparison.OrdinalIgnoreCase))
        return summaryDatabaseName.ToLowerInvariant();
      return databaseName.IndexOf(summaryDatabaseName, StringComparison.OrdinalIgnoreCase) >= 0 ? string.Empty : (string) null;
    }

    public ITeamFoundationDatabaseProperties CreateDatabase(
      IVssRequestContext deploymentRequestContext,
      string databaseName,
      string poolName,
      ISqlConnectionInfo dataTierConnectionInfo,
      VsoCreateDatabaseOptions dbOptions,
      ITFLogger logger)
    {
      deploymentRequestContext.CheckServicingRequestContext();
      ArgumentUtility.CheckStringForNullOrEmpty(databaseName, nameof (databaseName));
      ArgumentUtility.CheckForNull<ISqlConnectionInfo>(dataTierConnectionInfo, nameof (dataTierConnectionInfo));
      deploymentRequestContext.TraceEnter(99170, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (CreateDatabase));
      ITeamFoundationDatabaseProperties database = (ITeamFoundationDatabaseProperties) null;
      TeamFoundationDatabasePool databasePool = this.GetDatabasePool(deploymentRequestContext, poolName);
      bool databaseCreated = false;
      List<string> stringList = new List<string>();
      try
      {
        if (logger == null)
          logger = (ITFLogger) new ServerTraceLogger();
        if (dbOptions.Collation == null)
        {
          string collation = databasePool.Collation;
        }
        SqlConnectionStringBuilder connectionStringBuilder1 = new SqlConnectionStringBuilder(dataTierConnectionInfo.ConnectionString)
        {
          InitialCatalog = databaseName
        };
        connectionStringBuilder1.InitialCatalog = databaseName;
        ISqlConnectionInfo connectionInfo = dataTierConnectionInfo.Create(connectionStringBuilder1.ConnectionString);
        if (deploymentRequestContext.ExecutionEnvironment.IsHostedDeployment)
        {
          connectionStringBuilder1.Remove("User ID");
          connectionStringBuilder1.Remove("Password");
        }
        string connectionString = connectionStringBuilder1.ConnectionString;
        int maxTenants = !dbOptions.MaxTenants.HasValue ? databasePool.InitialCapacity : dbOptions.MaxTenants.Value;
        try
        {
          deploymentRequestContext.TraceAlways(99192, TraceLevel.Info, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, "Registering new database in " + poolName + " pool on " + dataTierConnectionInfo.DataSource + ".");
          database = this.RegisterDatabase(deploymentRequestContext, connectionString, (string) null, (string) null, poolName, 0, maxTenants, TeamFoundationDatabaseStatus.Creating, new DateTime?(DateTime.UtcNow), "Creating the database", new DateTime?(), false, (string) null, (byte[]) null, Guid.Empty, dbOptions.Flags, dbOptions.ServiceObjective ?? databasePool.ServiceObjective);
          string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The database has been registered in the pool. Database: {0}, Server: {1}, DatabaseId: {2}", (object) databaseName, (object) connectionStringBuilder1.DataSource, (object) database.DatabaseId);
          deploymentRequestContext.Trace(99171, TraceLevel.Info, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, message);
          logger.Info(message);
        }
        catch (DatabaseAlreadyRegisteredException ex)
        {
          logger.Info(ex.Message);
          database = this.GetDatabase(deploymentRequestContext, ex.DatabaseId, true);
          if (string.Equals(database.ConnectionInfoWrapper.ConnectionString, connectionStringBuilder1.ConnectionString, StringComparison.Ordinal) && string.Equals(database.PoolName, poolName, StringComparison.Ordinal))
          {
            if (database.Status != TeamFoundationDatabaseStatus.Creating)
            {
              if (database.Status == TeamFoundationDatabaseStatus.Online)
                goto label_13;
            }
            else
              goto label_13;
          }
          throw;
        }
label_13:
        bool flag1;
        using (TeamFoundationDataTierComponent componentRaw1 = dataTierConnectionInfo.CreateComponentRaw<TeamFoundationDataTierComponent>())
        {
          string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Creating database. Name: {0}, DataSource: {1}", (object) databaseName, (object) componentRaw1.DataSource);
          logger.Info(str);
          deploymentRequestContext.TraceAlways(99193, TraceLevel.Info, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, str);
          flag1 = componentRaw1.CheckIfDatabaseExists(databaseName);
          if (flag1)
          {
            deploymentRequestContext.Trace(99172, TraceLevel.Info, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, "Database already exists. Database Name: {0}. Server: {1}", (object) databaseName, (object) componentRaw1.DataSource);
            bool flag2;
            using (TeamFoundationDataTierComponent componentRaw2 = connectionInfo.CreateComponentRaw<TeamFoundationDataTierComponent>(logger: logger))
              flag2 = componentRaw2.CheckIfDatabaseIsEmpty();
            if (!flag2)
              throw new DatabaseAlreadyExistsException(databaseName, componentRaw1.DataSource);
          }
        }
        if (!flag1)
        {
          if (deploymentRequestContext.ExecutionEnvironment.IsOnPremisesDeployment && dataTierConnectionInfo.IsSqlAzure)
            throw new InvalidOperationException("Creating Databases on Sql Azure is not supported");
          Stopwatch stopwatch = Stopwatch.StartNew();
          using (TeamFoundationDataTierComponent componentRaw = dataTierConnectionInfo.CreateComponentRaw<TeamFoundationDataTierComponent>(logger: logger))
          {
            VsoCreateDatabaseOptions createDatabaseOptions1 = dbOptions;
            string str;
            if (createDatabaseOptions1.Collation == null)
              createDatabaseOptions1.Collation = str = databasePool.Collation;
            VsoCreateDatabaseOptions createDatabaseOptions2 = dbOptions;
            if (createDatabaseOptions2.ServiceObjective == null)
              createDatabaseOptions2.ServiceObjective = str = databasePool.ServiceObjective;
            if (dataTierConnectionInfo.IsSqlAzure)
              logger.Info("Creating the database. Database name: " + databaseName + ". Collation: " + dbOptions.Collation + ". ServiceObjective: " + dbOptions.ServiceObjective + ".");
            else
              logger.Info("Creating the database. Database name: " + databaseName + ". Collation: " + dbOptions.Collation + ".");
            componentRaw.CreateDatabase(databaseName, dbOptions, out databaseCreated);
          }
          stopwatch.Stop();
          string str1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The Database was created successfully. Name: {0}, Server: {1}. It took {2} ms to create database.", (object) databaseName, (object) dataTierConnectionInfo.DataSource, (object) stopwatch.ElapsedMilliseconds);
          if (stopwatch.Elapsed > TimeSpan.FromSeconds(10.0))
          {
            logger.Warning(str1);
            deploymentRequestContext.TraceAlways(99173, TraceLevel.Warning, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, str1);
          }
          else
          {
            logger.Info(str1);
            deploymentRequestContext.TraceAlways(99174, TraceLevel.Info, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, str1);
          }
          if (dataTierConnectionInfo.IsSqlAzure)
          {
            IRuntimeSqlManagementHelper managementHelper = RuntimeSqlManagementHelperLoader.LoadRuntimeSqlManagementHelper();
            bool flag3 = database.IsEligibleForAvailabilityZone();
            IVssRequestContext requestContext = deploymentRequestContext;
            string dataSource = TeamFoundationDataTierService.ManipulateDataSource(dataTierConnectionInfo.DataSource, DataSourceOptions.RemoveProtocolAndDomain);
            string databaseName1 = databaseName;
            int num = flag3 ? 1 : 0;
            ITFLogger logger1 = logger;
            managementHelper.AddDatabaseToFailoverGroupAndEnableAZ(requestContext, dataSource, databaseName1, num != 0, logger1);
          }
        }
        SqlConnectionStringBuilder connectionStringBuilder2 = new SqlConnectionStringBuilder(connectionInfo.ConnectionString);
        if (connectionStringBuilder2.ConnectTimeout < 120)
          connectionStringBuilder2.ConnectTimeout = 120;
        ISqlConnectionInfo sqlConnectionInfo = connectionInfo.Create(connectionStringBuilder2.ConnectionString);
        if (deploymentRequestContext.ExecutionEnvironment.IsHostedDeployment)
        {
          using (TeamFoundationSqlSecurityComponent componentRaw = sqlConnectionInfo.CreateComponentRaw<TeamFoundationSqlSecurityComponent>(logger: logger))
            componentRaw.ProvisionTfsRoles();
          DatabaseReplicationContext replicationContext = TeamFoundationDatabaseManagementService.GetSqlFailoverGroupReplicationContext(deploymentRequestContext, database, dataTierConnectionInfo, logger);
          string loginName;
          string loginPassword;
          this.ReplaceDatabaseLogin(deploymentRequestContext, dataTierConnectionInfo, database, databasePool, DatabaseCredentialNames.DefaultCredential, (string) null, logger, true, replicationContext, out loginName, out loginPassword);
          stringList.Add(loginName);
          this.ReplaceDatabaseLogin(deploymentRequestContext, dataTierConnectionInfo, database, databasePool, DatabaseCredentialNames.DbOwnerCredential, (string) null, logger, true, replicationContext, out loginName, out loginPassword);
          stringList.Add(loginName);
          database = this.GetDatabase(deploymentRequestContext, database.DatabaseId, true);
          CachedRegistryService service = deploymentRequestContext.GetService<CachedRegistryService>();
          CachedRegistryService registryService1 = service;
          IVssRequestContext requestContext1 = deploymentRequestContext;
          RegistryQuery registryQuery = (RegistryQuery) FrameworkServerConstants.OptInPrivilegedSqlLogin;
          ref RegistryQuery local1 = ref registryQuery;
          if (!registryService1.GetValue<bool>(requestContext1, in local1))
          {
            using (TeamFoundationSqlSecurityComponent componentRaw = sqlConnectionInfo.CreateComponentRaw<TeamFoundationSqlSecurityComponent>())
              componentRaw.AddRoleMember(DatabaseRoles.DbOwner, DatabaseRoles.TfsExecRole);
          }
          if (dataTierConnectionInfo.IsSqlAzure)
          {
            CachedRegistryService registryService2 = service;
            IVssRequestContext requestContext2 = deploymentRequestContext;
            registryQuery = (RegistryQuery) FrameworkServerConstants.SqlDevOpsSecurityGroups;
            ref RegistryQuery local2 = ref registryQuery;
            string str = registryService2.GetValue<string>(requestContext2, in local2);
            List<AadAccountObjectId> aadAccountObjectIdList;
            if (!string.IsNullOrEmpty(str))
              aadAccountObjectIdList = ((IEnumerable<string>) str.Split(',')).Select<string, AadAccountObjectId>((Func<string, AadAccountObjectId>) (s => AadAccountObjectId.FromAnnotatedObjectIdString(s))).ToList<AadAccountObjectId>();
            else
              aadAccountObjectIdList = (List<AadAccountObjectId>) null;
            IList<AadAccountObjectId> sqlDevOpsSecurityGroups = (IList<AadAccountObjectId>) aadAccountObjectIdList;
            TeamFoundationDatabaseManagementService.ProvisionDatabaseUsersForSqlJit(sqlConnectionInfo, sqlDevOpsSecurityGroups, logger);
            RuntimeSqlManagementHelperLoader.LoadRuntimeSqlManagementHelper().UpgradeToHyperscale(deploymentRequestContext, database, dbOptions.ServiceObjective, logger);
          }
        }
        else
        {
          using (TeamFoundationSqlSecurityComponent componentRaw = sqlConnectionInfo.CreateComponentRaw<TeamFoundationSqlSecurityComponent>(logger: logger))
            componentRaw.ProvisionTfsRolesOnprem();
        }
        if (deploymentRequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          this.CreateDatabaseUsersForServiceAccounts(deploymentRequestContext, dataTierConnectionInfo, databaseName, logger);
        this.SetDatabaseStatus(deploymentRequestContext, database.DatabaseId, TeamFoundationDatabaseStatus.Creating, "Empty database successfully created.");
      }
      catch (Exception ex1)
      {
        deploymentRequestContext.TraceException(99175, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, ex1);
        logger.Error(ex1);
        if (deploymentRequestContext.ExecutionEnvironment.IsHostedDeployment)
        {
          try
          {
            bool flag = false;
            if (databaseCreated)
            {
              using (TeamFoundationDataTierComponent componentRaw = dataTierConnectionInfo.CreateComponentRaw<TeamFoundationDataTierComponent>(logger: logger))
              {
                componentRaw.DropDatabase(databaseName, DropDatabaseOptions.CloseExistingConnections);
                flag = true;
              }
            }
            if (stringList.Count > 0)
            {
              foreach (string loginName in stringList)
              {
                using (TeamFoundationSqlSecurityComponent componentRaw = dataTierConnectionInfo.CreateComponentRaw<TeamFoundationSqlSecurityComponent>(logger: logger))
                  componentRaw.DropLogin(loginName);
              }
            }
            if (database != null)
            {
              if (flag)
                this.RemoveDatabase(deploymentRequestContext, database.DatabaseId);
              else
                this.SetDatabaseStatus(deploymentRequestContext, database.DatabaseId, TeamFoundationDatabaseStatus.Failed, "Hit exception during DB create. Message: " + ex1.Message);
            }
          }
          catch (Exception ex2)
          {
            deploymentRequestContext.TraceException(99176, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, ex2);
            logger.Error(ex2);
          }
        }
        throw;
      }
      finally
      {
        deploymentRequestContext.TraceLeave(99179, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, nameof (CreateDatabase));
      }
      return database;
    }

    public void CreateDatabaseUsersForServiceAccounts(
      IVssRequestContext deploymentRequestContext,
      ISqlConnectionInfo dataTierConnectionInfo,
      string databaseName,
      ITFLogger logger)
    {
      deploymentRequestContext.CheckDeploymentRequestContext();
      ArgumentUtility.CheckForNull<ISqlConnectionInfo>(dataTierConnectionInfo, nameof (dataTierConnectionInfo));
      ArgumentUtility.CheckStringForNullOrEmpty(databaseName, nameof (databaseName));
      if (deploymentRequestContext.ExecutionEnvironment.IsHostedDeployment)
        throw new InvalidOperationException("This method is not supported on hosted TFS.");
      ISqlConnectionInfo frameworkConnectionInfo = deploymentRequestContext.FrameworkConnectionInfo;
      string connectionString1 = TeamFoundationDatabaseManagementService.GetMachineNameFromConnectionString(frameworkConnectionInfo.ConnectionString);
      string connectionString2 = TeamFoundationDatabaseManagementService.GetMachineNameFromConnectionString(dataTierConnectionInfo.ConnectionString);
      if (frameworkConnectionInfo.IsSqlAzure)
      {
        logger.Info("Skipping creation of DB users for SQL Azure");
      }
      else
      {
        logger.Info("Configuration database machine: {0}", (object) connectionString1);
        logger.Info("Querying TFSEXECROLE members, connection string: {0}", (object) frameworkConnectionInfo.ConnectionString);
        List<SqlLoginInfo> roleLogins;
        SqlLoginInfo dboLogin;
        using (TeamFoundationSqlSecurityComponent componentRaw = frameworkConnectionInfo.CreateComponentRaw<TeamFoundationSqlSecurityComponent>())
        {
          roleLogins = componentRaw.GetRoleLogins(DatabaseRoles.TfsExecRole, true);
          logger.Info("{0} login(s) returned.", (object) roleLogins.Count);
          logger.Info("Querying dbo login, connection string: {0}", (object) frameworkConnectionInfo.ConnectionString);
          dboLogin = componentRaw.GetDboLogin();
        }
        if (dboLogin != null && dboLogin.IsNTName)
        {
          logger.Info("dbo login: {0}, sid: {1}", (object) dboLogin.LoginName, (object) dboLogin.SecurityIdentifier);
          if (((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) this.GetServiceIdentities(deploymentRequestContext)).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (serviceIdentity => new SecurityIdentifier(serviceIdentity.Descriptor.Identifier) == dboLogin.SecurityIdentifier)) != null)
          {
            logger.Info("{0} is a member of the Team Foundation Service Accounts group.", (object) dboLogin.LoginName);
            roleLogins.Add(dboLogin);
          }
          else
            logger.Info("{0} is not a member of the Team Foundation Service Accounts group.", (object) dboLogin.LoginName);
        }
        else
          logger.Info("dbo user is mapped to non-Windows login");
        logger.Info("DataTier connection string: {0}", (object) dataTierConnectionInfo.ConnectionString);
        bool flag1 = UriUtility.IsSameMachine(connectionString1, connectionString2);
        if (!flag1 && OSDetails.IsMachineInWorkgroup())
          throw new InvalidOperationException(FrameworkResources.RemoteSqlServerNotSupportedWhenNotJoinedToDomain());
        SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(dataTierConnectionInfo.ConnectionString)
        {
          InitialCatalog = databaseName
        };
        ISqlConnectionInfo connectionInfo = dataTierConnectionInfo.Create(connectionStringBuilder.ConnectionString);
        using (TeamFoundationSqlSecurityComponent componentRaw = connectionInfo.CreateComponentRaw<TeamFoundationSqlSecurityComponent>())
          componentRaw.ProvisionTfsRolesOnprem();
        foreach (SqlLoginInfo sqlLoginInfo1 in roleLogins)
        {
          bool flag2 = false;
          string loginName = (string) null;
          if (flag1)
            loginName = sqlLoginInfo1.LoginName;
          else if (sqlLoginInfo1.IsNetworkService)
          {
            logger.Info("Resolving domain for {0} computer.", (object) connectionString1);
            try
            {
              loginName = ComputerInfo.GetComputerAccount(connectionString1, logger);
              logger.Info("Computer account: {0}", (object) loginName);
            }
            catch (Exception ex)
            {
              logger.Info("Failed to resolve domain name for {0} computer. Please contact your DNS administrator.", (object) connectionString1);
              logger.Info(ex.Message);
              throw;
            }
          }
          else if (sqlLoginInfo1.IsMachineLogin)
          {
            if (UriUtility.IsSameMachine(sqlLoginInfo1.GetMachineName(), connectionString2))
              flag2 = true;
            else
              loginName = sqlLoginInfo1.LoginName;
          }
          else if (!(sqlLoginInfo1.SecurityIdentifier.AccountDomainSid == (SecurityIdentifier) null))
            loginName = sqlLoginInfo1.LoginName;
          else
            continue;
          SqlLoginInfo sqlLoginInfo2 = (SqlLoginInfo) null;
          try
          {
            if (flag2)
              sqlLoginInfo2 = TeamFoundationDatabaseManagementService.CreateSqlLoginIncludingAvailabilityGroup(dataTierConnectionInfo, logger);
            else if (loginName != null)
              sqlLoginInfo2 = TeamFoundationDatabaseManagementService.CreateSqlLoginIncludingAvailabilityGroup(dataTierConnectionInfo, logger, loginName);
          }
          catch (Exception ex)
          {
            logger.Error(ex);
            throw;
          }
          if (sqlLoginInfo2 != null)
          {
            Dictionary<string, bool> permissions = new Dictionary<string, bool>()
            {
              {
                "VIEW ANY DEFINITION",
                false
              },
              {
                "VIEW SERVER STATE",
                false
              }
            };
            logger.Info("Grant SQL server level permissions to " + sqlLoginInfo2.LoginName);
            TeamFoundationDatabaseManagementService.GrantServerScopePermissionsToLogin(dataTierConnectionInfo, sqlLoginInfo2.LoginName, permissions, logger, true);
          }
          using (TeamFoundationSqlSecurityComponent componentRaw = connectionInfo.CreateComponentRaw<TeamFoundationSqlSecurityComponent>())
          {
            if (sqlLoginInfo2 != null)
            {
              int num = (int) componentRaw.ModifyExecRole(sqlLoginInfo2.LoginName, DatabaseRoles.TfsExecRole, AccountsOperation.Add);
              logger.Info("Granted access to the {0} database to {1}.", (object) databaseName, (object) sqlLoginInfo2.LoginName);
            }
          }
        }
      }
    }

    internal bool ReplaceConfigDbLogin(
      IVssRequestContext requestContext,
      string serviceName,
      ITFLogger logger,
      out Dictionary<string, KeyValuePair<string, string>> loginPasswords)
    {
      TeamFoundationDatabaseManagementService.CheckDeploymentRequestContext(requestContext);
      TeamFoundationDatabaseManagementService.CheckHosted(requestContext);
      requestContext.CheckServicingRequestContext();
      if (logger == null)
        logger = (ITFLogger) new ServerTraceLogger();
      loginPasswords = new Dictionary<string, KeyValuePair<string, string>>();
      TeamFoundationDatabasePool databasePool = this.GetDatabasePool(requestContext, DatabaseManagementConstants.ConfigurationPoolName);
      ITeamFoundationDatabaseProperties databaseProperties = this.QueryDatabases(requestContext, databasePool.PoolName, true).Single<ITeamFoundationDatabaseProperties>();
      string loginName;
      string loginPassword;
      this.ReplaceConfigDbDatabaseLogin(requestContext, databaseProperties, databasePool, DatabaseCredentialNames.DefaultCredential, serviceName, logger, true, out loginName, out loginPassword);
      loginPasswords.Add(DatabaseCredentialNames.DefaultCredential, new KeyValuePair<string, string>(loginName, loginPassword));
      this.ReplaceConfigDbDatabaseLogin(requestContext, databaseProperties, databasePool, DatabaseCredentialNames.DbOwnerCredential, serviceName, logger, true, out loginName, out loginPassword);
      loginPasswords.Add(DatabaseCredentialNames.DbOwnerCredential, new KeyValuePair<string, string>(loginName, loginPassword));
      return true;
    }

    public bool ReplaceTenantSqlLogins(IVssRequestContext requestContext, ITFLogger logger)
    {
      TeamFoundationDatabaseManagementService.CheckDeploymentRequestContext(requestContext);
      TeamFoundationDatabaseManagementService.CheckHosted(requestContext);
      if (logger == null)
        logger = (ITFLogger) new ServerTraceLogger();
      bool flag = true;
      foreach (TeamFoundationDatabasePool queryDatabasePool in this.QueryDatabasePools(requestContext))
      {
        if (queryDatabasePool.CredentialPolicy == TeamFoundationDatabaseCredentialPolicy.InProcessRotation && !this.ReplacePoolSqlLogins(requestContext, queryDatabasePool, logger))
          flag = false;
      }
      if (flag)
        requestContext.ServiceHost.ServiceHostInternal().FlushNotificationQueue(requestContext);
      return flag;
    }

    public bool ReplaceTenantSqlLogins(
      IVssRequestContext requestContext,
      int databaseId,
      ITFLogger logger)
    {
      TeamFoundationDatabaseManagementService.CheckDeploymentRequestContext(requestContext);
      TeamFoundationDatabaseManagementService.CheckHosted(requestContext);
      if (logger == null)
        logger = (ITFLogger) new ServerTraceLogger();
      bool flag = true;
      ITeamFoundationDatabaseProperties database = this.GetDatabase(requestContext, databaseId, true);
      if (database == null)
      {
        logger.Error(string.Format("Skipping database id {0} not found!", (object) databaseId));
        return false;
      }
      TeamFoundationDatabasePool databasePool = this.GetDatabasePool(requestContext, database.PoolName);
      if (databasePool.CredentialPolicy != TeamFoundationDatabaseCredentialPolicy.InProcessRotation)
      {
        logger.Warning(string.Format("Skipping database {0} - database pool {1} credential policy is {2}.", (object) database.DatabaseName, (object) databasePool.PoolName, (object) databasePool.CredentialPolicy));
        return flag;
      }
      if (database.Status != TeamFoundationDatabaseStatus.Online)
      {
        logger.Warning(string.Format("Skipping database {0} - database status is {1}.", (object) database.DatabaseName, (object) database.Status));
        return flag;
      }
      if (!this.ReplaceDatabaseLogins(requestContext, database, databasePool, (string) null, logger, false))
        flag = false;
      if (flag)
        requestContext.ServiceHost.ServiceHostInternal().FlushNotificationQueue(requestContext);
      return flag;
    }

    public bool DisableOldTenantLogins(
      IVssRequestContext requestContext,
      TimeSpan? minReplacedTime,
      bool verifyOnly,
      ITFLogger logger)
    {
      TeamFoundationDatabaseManagementService.CheckDeploymentRequestContext(requestContext);
      TeamFoundationDatabaseManagementService.CheckHosted(requestContext);
      if (logger == null)
        logger = (ITFLogger) new ServerTraceLogger();
      CachedRegistryService service = requestContext.GetService<CachedRegistryService>();
      if (!minReplacedTime.HasValue)
        minReplacedTime = new TimeSpan?(service.GetValue<TimeSpan>(requestContext, (RegistryQuery) FrameworkServerConstants.MinReplacedTimeForDisableLogin, TeamFoundationDatabaseManagementService.s_defaultMinReplacedTime));
      logger.Info("Minimum replaced time for login to be disabled: {0}", (object) minReplacedTime);
      bool flag = true;
      foreach (TeamFoundationDatabasePool queryDatabasePool in this.QueryDatabasePools(requestContext))
      {
        if (queryDatabasePool.CredentialPolicy == TeamFoundationDatabaseCredentialPolicy.InProcessRotation && !this.DisableOldPoolLogins(requestContext, queryDatabasePool, minReplacedTime.Value, verifyOnly, logger))
          flag = false;
      }
      return flag;
    }

    public bool DropOldTenantUsersAndLogins(
      IVssRequestContext requestContext,
      TimeSpan? minDisabledTime,
      bool checkProcessStartTimes,
      bool verifyOnly,
      ITFLogger logger)
    {
      TeamFoundationDatabaseManagementService.CheckDeploymentRequestContext(requestContext);
      TeamFoundationDatabaseManagementService.CheckHosted(requestContext);
      if (logger == null)
        logger = (ITFLogger) new ServerTraceLogger();
      CachedRegistryService service = requestContext.GetService<CachedRegistryService>();
      if (!minDisabledTime.HasValue)
        minDisabledTime = new TimeSpan?(service.GetValue<TimeSpan>(requestContext, (RegistryQuery) FrameworkServerConstants.MinDisabledTimeForDropLogin, TeamFoundationDatabaseManagementService.s_defaultMinDisabledTime));
      logger.Info("Minimum disabled time for login to be dropped: {0}", (object) minDisabledTime);
      bool flag = true;
      foreach (TeamFoundationDatabasePool queryDatabasePool in this.QueryDatabasePools(requestContext))
      {
        if (queryDatabasePool.CredentialPolicy == TeamFoundationDatabaseCredentialPolicy.InProcessRotation && !this.DropOldPoolUsersAndLogins(requestContext, queryDatabasePool, minDisabledTime.Value, checkProcessStartTimes, verifyOnly, logger))
          flag = false;
      }
      return flag;
    }

    private ITeamFoundationDatabaseProperties GetDatabaseForQueryStore(
      IVssRequestContext requestContext,
      string databaseName)
    {
      ITeamFoundationDatabaseProperties db = this.QueryDatabases(requestContext, true).Where<ITeamFoundationDatabaseProperties>((Func<ITeamFoundationDatabaseProperties, bool>) (dbInfo => string.Equals(dbInfo?.SqlConnectionInfo?.InitialCatalog, databaseName, StringComparison.OrdinalIgnoreCase))).FirstOrDefault<ITeamFoundationDatabaseProperties>();
      if (db == null)
        throw new DatabaseNotFoundException(databaseName);
      if (db.Status != TeamFoundationDatabaseStatus.Online && db.Status != TeamFoundationDatabaseStatus.Servicing)
        throw new InvalidOperationException(string.Format("Unable to get database for Query Store, the database {0} is not online or servicing, its current status is {1}", (object) db.DatabaseName, (object) db.Status));
      return !db.IsExternalDatabase() ? db : throw new InvalidOperationException("Unable to get database for Query Store, the database " + db.DatabaseName + " is an external database");
    }

    private bool ReplacePoolSqlLogins(
      IVssRequestContext requestContext,
      TeamFoundationDatabasePool pool,
      ITFLogger logger)
    {
      bool flag = true;
      requestContext.Trace(99257, TraceLevel.Verbose, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, "Replacing default and dbo logins for databases '{0}' database pool.", (object) pool.PoolName);
      foreach (ITeamFoundationDatabaseProperties queryDatabase in this.QueryDatabases(requestContext, pool.PoolName, true))
      {
        if (queryDatabase.Status != TeamFoundationDatabaseStatus.Online)
          logger.Warning("Skipping {0} database - database status is {1}.", (object) queryDatabase.DatabaseName, (object) queryDatabase.Status);
        else if (!this.ReplaceDatabaseLogins(requestContext, queryDatabase, pool, (string) null, logger, false))
          flag = false;
      }
      return flag;
    }

    public bool ReplaceDatabaseLogins(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties databaseProperties,
      TeamFoundationDatabasePool pool,
      string serviceName,
      ITFLogger logger,
      bool throwExceptions)
    {
      DataTierInfo associatedDataTier = requestContext.GetService<TeamFoundationDataTierService>().FindAssociatedDataTier(requestContext, databaseProperties.ConnectionInfoWrapper.ConnectionString);
      DatabaseReplicationContext replicationContext = TeamFoundationDatabaseManagementService.GetDatabaseReplicationContext(requestContext, databaseProperties, associatedDataTier.ConnectionInfo, logger);
      if (!replicationContext.IsPrimary)
      {
        logger.Warning("Skipping " + databaseProperties.DatabaseName + " database - database is not primary");
        return true;
      }
      string loginName;
      string loginPassword;
      bool flag = this.ReplaceDatabaseLogin(requestContext, associatedDataTier.ConnectionInfo, databaseProperties, pool, DatabaseCredentialNames.DefaultCredential, serviceName, logger, throwExceptions, replicationContext, out loginName, out loginPassword);
      if (flag)
        flag = this.ReplaceDatabaseLogin(requestContext, associatedDataTier.ConnectionInfo, databaseProperties, pool, DatabaseCredentialNames.DbOwnerCredential, serviceName, logger, throwExceptions, replicationContext, out loginName, out loginPassword);
      return flag;
    }

    private bool ReplaceConfigDbDatabaseLogin(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties databaseProperties,
      TeamFoundationDatabasePool pool,
      string credentialName,
      string serviceName,
      ITFLogger logger,
      bool throwExceptions,
      out string loginName,
      out string loginPassword)
    {
      DataTierInfo associatedDataTier = requestContext.GetService<TeamFoundationDataTierService>().FindAssociatedDataTier(requestContext, databaseProperties.ConnectionInfoWrapper.ConnectionString);
      DatabaseReplicationContext replicationContext = TeamFoundationDatabaseManagementService.GetSqlFailoverGroupReplicationContext(requestContext, databaseProperties, associatedDataTier.ConnectionInfo, logger);
      return this.ReplaceDatabaseLogin(requestContext, associatedDataTier.ConnectionInfo, databaseProperties, pool, credentialName, serviceName, logger, throwExceptions, replicationContext, out loginName, out loginPassword);
    }

    private bool ReplaceDatabaseLogin(
      IVssRequestContext requestContext,
      ISqlConnectionInfo dataTierConnectionInfo,
      ITeamFoundationDatabaseProperties databaseProperties,
      TeamFoundationDatabasePool pool,
      string credentialName,
      string serviceName,
      ITFLogger logger,
      bool throwExceptions,
      DatabaseReplicationContext replicationContext,
      out string loginName,
      out string loginPassword)
    {
      if (pool.CredentialPolicy != TeamFoundationDatabaseCredentialPolicy.InProcessRotation && pool.CredentialPolicy != TeamFoundationDatabaseCredentialPolicy.ProvisionNewCredentialForEachUse && pool.CredentialPolicy != TeamFoundationDatabaseCredentialPolicy.DeploymentRotation)
      {
        logger.Error("Credential policy '{0}' is not supported by this method.", (object) pool.CredentialPolicy);
        if (throwExceptions)
          throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Credential policy '{0}' is not supported by this method.", (object) pool.CredentialPolicy));
        loginName = (string) null;
        loginPassword = (string) null;
        return false;
      }
      requestContext.Trace(99258, TraceLevel.Verbose, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, "Replacing primary login for {0}", (object) databaseProperties.DatabaseName);
      this.GenerateSqlLogin(requestContext, pool.DatabaseType, credentialName.Equals(DatabaseCredentialNames.DbOwnerCredential), serviceName, out loginName, out loginPassword);
      TeamFoundationDatabaseCredential credential = this.RegisterCredential(requestContext, databaseProperties, loginName, loginPassword, credentialName);
      foreach (IDatabaseReplicaInfo replica in replicationContext.Replicas)
        replica.RegisterCredential(requestContext, loginName, loginPassword, credentialName, logger);
      string roleName;
      bool grantViewServerState;
      if (credentialName.Equals(DatabaseCredentialNames.DbOwnerCredential, StringComparison.InvariantCultureIgnoreCase))
      {
        roleName = DatabaseRoles.VsoDboRole;
        grantViewServerState = true;
      }
      else if (credentialName.Equals(DatabaseCredentialNames.DefaultCredential, StringComparison.InvariantCultureIgnoreCase))
      {
        roleName = !TeamFoundationDatabaseExtensions.ShouldBuildDataspaceSplitOnCreate(requestContext) || !requestContext.IsFeatureEnabled("Microsoft.TeamFoundation.Framework.RestrictPermissionForSplitDb") || !pool.PoolName.Equals(DatabaseManagementConstants.BuildPartitionPoolName, StringComparison.OrdinalIgnoreCase) ? DatabaseRoles.TfsExecRole : DatabaseRoles.TfsBuildExecRole;
        grantViewServerState = false;
      }
      else
      {
        roleName = DatabaseRoles.TfsReaderRole;
        grantViewServerState = false;
      }
      return this.ProvisionSqlLoginAndUser(requestContext, logger, dataTierConnectionInfo, databaseProperties, replicationContext, credential, loginPassword, roleName, grantViewServerState);
    }

    internal bool AddDatabaseAlternateLogin(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties databaseProperties,
      TeamFoundationDatabaseType databaseType,
      ITFLogger logger,
      string credentialName,
      string roleName,
      out TeamFoundationDatabaseCredential newCredential)
    {
      DataTierInfo associatedDataTier = requestContext.GetService<TeamFoundationDataTierService>().FindAssociatedDataTier(requestContext, databaseProperties.ConnectionInfoWrapper.ConnectionString);
      return this.AddDatabaseAlternateLogin(requestContext, associatedDataTier.ConnectionInfo, databaseProperties, databaseType, logger, credentialName, roleName, out newCredential);
    }

    private bool AddDatabaseAlternateLogin(
      IVssRequestContext requestContext,
      ISqlConnectionInfo dataTierConnectionInfo,
      ITeamFoundationDatabaseProperties databaseProperties,
      TeamFoundationDatabaseType databaseType,
      ITFLogger logger,
      string credentialName,
      string roleName,
      out TeamFoundationDatabaseCredential newCredential)
    {
      requestContext.Trace(99259, TraceLevel.Verbose, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, "Adding alternate login for {0}", (object) databaseProperties.DatabaseName);
      bool isDbOwner = credentialName.Equals(DatabaseCredentialNames.DbOwnerCredential);
      bool flag = credentialName.Equals(DatabaseCredentialNames.DefaultCredential);
      bool markOtherProvisioningCredsForDeletion = isDbOwner | flag;
      bool grantViewServerState = isDbOwner;
      string loginName;
      string loginPassword;
      this.GenerateSqlLogin(requestContext, databaseType, isDbOwner, (string) null, out loginName, out loginPassword);
      newCredential = this.RegisterCredential(requestContext, databaseProperties, loginName, loginPassword, credentialName, markOtherProvisioningCredsForDeletion: markOtherProvisioningCredsForDeletion);
      return this.ProvisionSqlLoginAndUser(requestContext, logger, dataTierConnectionInfo, databaseProperties, DatabaseReplicationContext.Default, newCredential, loginPassword, roleName, grantViewServerState);
    }

    internal void AddDatabaseAlternateLoginNoCredential(
      IVssRequestContext requestContext,
      ITFLogger logger,
      ITeamFoundationDatabaseProperties databaseProperties,
      TeamFoundationDatabaseType databaseType,
      string credentialName,
      string roleName,
      out string loginName,
      out string loginPassword,
      out byte[] sid)
    {
      requestContext.Trace(99260, TraceLevel.Verbose, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, "Adding alternate login without registering credential for {0}", (object) databaseProperties.DatabaseName);
      DataTierInfo associatedDataTier = requestContext.GetService<TeamFoundationDataTierService>().FindAssociatedDataTier(requestContext, databaseProperties.ConnectionInfoWrapper.ConnectionString);
      bool isDbOwner = credentialName.Equals(DatabaseCredentialNames.DbOwnerCredential);
      bool isDefault = credentialName.Equals(DatabaseCredentialNames.DefaultCredential);
      bool grantViewServerState = isDbOwner;
      this.GenerateSqlLogin(requestContext, databaseType, isDbOwner, (string) null, out loginName, out loginPassword);
      SqlLoginInfo sqlLoginInfo = this.ProvisionSqlLoginAndUser(requestContext, logger, associatedDataTier.ConnectionInfo, databaseProperties, DatabaseReplicationContext.Default, loginName, loginPassword, roleName, grantViewServerState, isDefault);
      sid = sqlLoginInfo.Sid;
    }

    private bool DisableOldPoolLogins(
      IVssRequestContext requestContext,
      TeamFoundationDatabasePool pool,
      TimeSpan minReplacedTime,
      bool verifyOnly,
      ITFLogger logger)
    {
      requestContext.Trace(99261, TraceLevel.Verbose, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, "Disabling old logins for databases '{0}' database pool.", (object) pool.PoolName);
      List<ITeamFoundationDatabaseProperties> databasePropertiesList = this.QueryDatabases(requestContext, pool.PoolName, true);
      bool flag = true;
      foreach (ITeamFoundationDatabaseProperties databaseProperties in databasePropertiesList)
      {
        if (databaseProperties.Status != TeamFoundationDatabaseStatus.Online)
          logger.Warning("Skipping {0} database - database status is {1}.", (object) databaseProperties.DatabaseName, (object) databaseProperties.Status);
        else if (!this.DisableOldDatabaseLogins(requestContext, databaseProperties, pool, minReplacedTime, verifyOnly, logger))
          flag = false;
      }
      return flag;
    }

    private bool DropOldPoolUsersAndLogins(
      IVssRequestContext requestContext,
      TeamFoundationDatabasePool pool,
      TimeSpan minDisabledTime,
      bool checkProcessStartTimes,
      bool verifyOnly,
      ITFLogger logger)
    {
      requestContext.Trace(99262, TraceLevel.Verbose, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, "Dropping old logins for databases '{0}' database pool.", (object) pool.PoolName);
      List<ITeamFoundationDatabaseProperties> databasePropertiesList = this.QueryDatabases(requestContext, pool.PoolName, true);
      bool flag = true;
      foreach (ITeamFoundationDatabaseProperties databaseProperties in databasePropertiesList)
      {
        if (databaseProperties.Status != TeamFoundationDatabaseStatus.Online)
          logger.Warning("Skipping {0} database - database status is {1}.", (object) databaseProperties.DatabaseName, (object) databaseProperties.Status);
        else if (!this.DropOldDatabaseUsersAndLogins(requestContext, databaseProperties, pool, minDisabledTime, checkProcessStartTimes, verifyOnly, logger))
          flag = false;
      }
      return flag;
    }

    public bool DisableOldDatabaseLogins(
      IVssRequestContext requestContext,
      int databaseId,
      TimeSpan? minReplacedTime,
      bool verifyOnly,
      ITFLogger logger)
    {
      TeamFoundationDatabaseManagementService.CheckDeploymentRequestContext(requestContext);
      TeamFoundationDatabaseManagementService.CheckHosted(requestContext);
      requestContext.CheckServicingRequestContext();
      if (databaseId == -2)
        throw new VirtualServiceHostException();
      if (logger == null)
        logger = (ITFLogger) new ServerTraceLogger();
      CachedRegistryService service = requestContext.GetService<CachedRegistryService>();
      if (!minReplacedTime.HasValue)
        minReplacedTime = new TimeSpan?(service.GetValue<TimeSpan>(requestContext, (RegistryQuery) FrameworkServerConstants.MinReplacedTimeForDisableLogin, TeamFoundationDatabaseManagementService.s_defaultMinReplacedTime));
      logger.Info("Minimum replaced time for login to be disabled: {0}", (object) minReplacedTime);
      ITeamFoundationDatabaseProperties database = this.GetDatabase(requestContext, databaseId, true);
      TeamFoundationDatabasePool databasePool = this.GetDatabasePool(requestContext, database.PoolName);
      this.DisableOldDatabaseLogins(requestContext, database, databasePool, minReplacedTime.Value, verifyOnly, logger);
      return true;
    }

    private bool DisableOldDatabaseLogins(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties databaseProperties,
      TeamFoundationDatabasePool pool,
      TimeSpan minDisabledTime,
      bool verifyOnly,
      ITFLogger logger)
    {
      DataTierInfo associatedDataTier = requestContext.GetService<TeamFoundationDataTierService>().FindAssociatedDataTier(requestContext, databaseProperties.ConnectionInfoWrapper.ConnectionString);
      DatabaseReplicationContext replicationContext = TeamFoundationDatabaseManagementService.GetDatabaseReplicationContext(requestContext, databaseProperties, associatedDataTier.ConnectionInfo, logger);
      if (replicationContext.IsPrimary)
        return this.DisableOldDatabaseLogins(requestContext, associatedDataTier.ConnectionInfo, databaseProperties, pool, minDisabledTime, verifyOnly, replicationContext, logger);
      logger.Warning("Skipping " + databaseProperties.DatabaseName + " database - database is not primary");
      return true;
    }

    private bool DisableOldDatabaseLogins(
      IVssRequestContext requestContext,
      ISqlConnectionInfo dataTierConnectionInfo,
      ITeamFoundationDatabaseProperties databaseProperties,
      TeamFoundationDatabasePool pool,
      TimeSpan minDisabledTime,
      bool verifyOnly,
      DatabaseReplicationContext replicationContext,
      ITFLogger logger)
    {
      if (pool.CredentialPolicy != TeamFoundationDatabaseCredentialPolicy.InProcessRotation && pool.CredentialPolicy != TeamFoundationDatabaseCredentialPolicy.ProvisionNewCredentialForEachUse && pool.CredentialPolicy != TeamFoundationDatabaseCredentialPolicy.DeploymentRotation)
        throw new NotSupportedException(string.Format("Credential policy '{0}' is not supported by this method.", (object) pool.CredentialPolicy));
      SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(databaseProperties.ConnectionInfoWrapper.ConnectionString);
      string dataSource = connectionStringBuilder.DataSource;
      string initialCatalog = connectionStringBuilder.InitialCatalog;
      List<TeamFoundationDatabaseCredential> credentialsForDatabase = this.GetCredentialsForDatabase(requestContext, databaseProperties.DatabaseId);
      requestContext.Trace(99263, TraceLevel.Verbose, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, "Database {0} has {1} credentials.", (object) databaseProperties.DatabaseId, (object) credentialsForDatabase.Count);
      List<ISqlConnectionInfo> list = TeamFoundationDatabaseManagementService.GetReplicaNodesFromAvailabilityGroup(dataTierConnectionInfo, logger).Select<AvailabilityReplica, ISqlConnectionInfo>((Func<AvailabilityReplica, ISqlConnectionInfo>) (n => dataTierConnectionInfo.CloneReplaceDataSource(n.Node))).ToList<ISqlConnectionInfo>();
      if (list.Count == 0)
        list.Add(dataTierConnectionInfo);
      foreach (IDatabaseReplicaInfo replica in replicationContext.Replicas)
        list.Add(replica.DataTierConnectionInfo);
      bool success = true;
      foreach (TeamFoundationDatabaseCredential credential in credentialsForDatabase)
      {
        if (this.IsLoginEligibleForDisabling(requestContext, dataTierConnectionInfo, dataSource, initialCatalog, pool, minDisabledTime, credential, logger, ref success))
        {
          foreach (ISqlConnectionInfo dataTierConnectionInfo1 in list)
            success &= this.DisableDatabaseLogin(requestContext, dataTierConnectionInfo1, credential, verifyOnly, logger);
        }
      }
      return success;
    }

    public virtual bool DropOldDatabaseUsersAndLogins(
      IVssRequestContext requestContext,
      int databaseId,
      TimeSpan? minDisabledTime,
      bool checkProcessStartTimes,
      bool verifyOnly,
      ITFLogger logger)
    {
      TeamFoundationDatabaseManagementService.CheckDeploymentRequestContext(requestContext);
      TeamFoundationDatabaseManagementService.CheckHosted(requestContext);
      requestContext.CheckServicingRequestContext();
      if (databaseId == -2)
        throw new VirtualServiceHostException();
      if (logger == null)
        logger = (ITFLogger) new ServerTraceLogger();
      CachedRegistryService service = requestContext.GetService<CachedRegistryService>();
      if (!minDisabledTime.HasValue)
        minDisabledTime = new TimeSpan?(service.GetValue<TimeSpan>(requestContext, (RegistryQuery) FrameworkServerConstants.MinDisabledTimeForDropLogin, TeamFoundationDatabaseManagementService.s_defaultMinDisabledTime));
      logger.Info("Minimum disabled time for login to be dropped: {0}", (object) minDisabledTime);
      ITeamFoundationDatabaseProperties database = this.GetDatabase(requestContext, databaseId, true);
      TeamFoundationDatabasePool databasePool = this.GetDatabasePool(requestContext, database.PoolName);
      return this.DropOldDatabaseUsersAndLogins(requestContext, database, databasePool, minDisabledTime.Value, checkProcessStartTimes, verifyOnly, logger);
    }

    private bool DropOldDatabaseUsersAndLogins(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties databaseProperties,
      TeamFoundationDatabasePool pool,
      TimeSpan minDisabledTime,
      bool checkProcessStartTimes,
      bool verifyOnly,
      ITFLogger logger)
    {
      DataTierInfo associatedDataTier = requestContext.GetService<TeamFoundationDataTierService>().FindAssociatedDataTier(requestContext, databaseProperties.ConnectionInfoWrapper.ConnectionString);
      DatabaseReplicationContext replicationContext = TeamFoundationDatabaseManagementService.GetDatabaseReplicationContext(requestContext, databaseProperties, associatedDataTier.ConnectionInfo, logger);
      if (replicationContext.IsPrimary)
        return this.DropOldDatabaseUsersAndLogins(requestContext, associatedDataTier.ConnectionInfo, databaseProperties, pool, minDisabledTime, checkProcessStartTimes, verifyOnly, replicationContext, logger);
      logger.Warning("Skipping " + databaseProperties.DatabaseName + " database - database is not primary");
      return true;
    }

    private bool DropOldDatabaseUsersAndLogins(
      IVssRequestContext requestContext,
      ISqlConnectionInfo dataTierConnectionInfo,
      ITeamFoundationDatabaseProperties databaseProperties,
      TeamFoundationDatabasePool pool,
      TimeSpan minDisabledTime,
      bool checkProcessStartTimes,
      bool verifyOnly,
      DatabaseReplicationContext replicationContext,
      ITFLogger logger)
    {
      if (pool.CredentialPolicy != TeamFoundationDatabaseCredentialPolicy.InProcessRotation && pool.CredentialPolicy != TeamFoundationDatabaseCredentialPolicy.ProvisionNewCredentialForEachUse && pool.CredentialPolicy != TeamFoundationDatabaseCredentialPolicy.DeploymentRotation)
        throw new NotSupportedException(string.Format("Credential policy '{0}' is not supported by this method.", (object) pool.CredentialPolicy));
      if (databaseProperties.DatabaseId == -2)
        throw new VirtualServiceHostException();
      List<TeamFoundationDatabaseCredential> credentialsForDatabase = this.GetCredentialsForDatabase(requestContext, databaseProperties.DatabaseId);
      requestContext.Trace(99264, TraceLevel.Verbose, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, "Database {0} has {1} credentials.", (object) databaseProperties.DatabaseId, (object) credentialsForDatabase.Count);
      string dataSource = databaseProperties.SqlConnectionInfo.DataSource;
      string initialCatalog = databaseProperties.SqlConnectionInfo.InitialCatalog;
      this.AlterAuthorizationToDbo(dataTierConnectionInfo, initialCatalog, logger);
      List<AvailabilityReplica> availabilityGroup = TeamFoundationDatabaseManagementService.GetReplicaNodesFromAvailabilityGroup(dataTierConnectionInfo, logger);
      bool success = true;
      foreach (TeamFoundationDatabaseCredential credential in credentialsForDatabase)
      {
        if (this.IsLoginEligibleForDropping(requestContext, dataTierConnectionInfo, dataSource, initialCatalog, pool, true, minDisabledTime, checkProcessStartTimes, credential, logger, ref success))
        {
          bool dropUser1 = true;
          bool removeCredential1 = true;
          if (availabilityGroup != null && availabilityGroup.Count > 0)
          {
            foreach (AvailabilityReplica availabilityReplica in availabilityGroup)
            {
              ISqlConnectionInfo dataTierConnectionInfo1 = dataTierConnectionInfo.CloneReplaceDataSource(availabilityReplica.Node);
              bool removeCredential2;
              bool dropUser2 = removeCredential2 = availabilityReplica.Role == AvailabilityReplicaRole.Primary;
              success &= this.DropDatabaseUserAndLogin(requestContext, dataTierConnectionInfo1, initialCatalog, pool, credential, verifyOnly, dropUser2, removeCredential2, logger);
            }
          }
          else
          {
            foreach (IDatabaseReplicaInfo replica in replicationContext.Replicas)
            {
              success &= this.DropDatabaseUserAndLogin(requestContext, replica.DataTierConnectionInfo, replica.DatabaseName, pool, credential, verifyOnly, false, false, logger);
              success &= replica.UnregisterCredential(requestContext, credential, logger);
            }
            success &= this.DropDatabaseUserAndLogin(requestContext, dataTierConnectionInfo, initialCatalog, pool, credential, verifyOnly, dropUser1, removeCredential1, logger);
          }
        }
      }
      return success;
    }

    internal bool DropDatabaseUserAndLogin(
      IVssRequestContext requestContext,
      string userId,
      int databaseId,
      ITFLogger logger)
    {
      if (this.GetCredentialsForDatabase(requestContext, databaseId).FirstOrDefault<TeamFoundationDatabaseCredential>((Func<TeamFoundationDatabaseCredential, bool>) (c => c.UserId == userId)) != null)
        throw new ArgumentException("This version of DropDatabaseUserAndLogin should not be used to drop users and logins that have associated credentials.", nameof (userId));
      ITeamFoundationDatabaseProperties database = this.GetDatabase(requestContext, databaseId, true);
      TeamFoundationDatabasePool databasePool = this.GetDatabasePool(requestContext, database.PoolName);
      DataTierInfo associatedDataTier = requestContext.GetService<TeamFoundationDataTierService>().FindAssociatedDataTier(requestContext, database.ConnectionInfoWrapper.ConnectionString);
      SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(database.ConnectionInfoWrapper.ConnectionString);
      string dataSource = connectionStringBuilder.DataSource;
      string initialCatalog = connectionStringBuilder.InitialCatalog;
      bool flag = true;
      List<AvailabilityReplica> availabilityGroup = TeamFoundationDatabaseManagementService.GetReplicaNodesFromAvailabilityGroup(associatedDataTier.ConnectionInfo, logger);
      bool dropUser1 = true;
      if (availabilityGroup != null && availabilityGroup.Count > 0)
      {
        foreach (AvailabilityReplica availabilityReplica in availabilityGroup)
        {
          ISqlConnectionInfo dataTierConnectionInfo = associatedDataTier.ConnectionInfo.CloneReplaceDataSource(availabilityReplica.Node);
          bool dropUser2 = availabilityReplica.Role == AvailabilityReplicaRole.Primary;
          flag &= this.DropDatabaseUserAndLogin(requestContext, dataTierConnectionInfo, initialCatalog, databasePool, userId, false, dropUser2, logger);
        }
      }
      else
        flag &= this.DropDatabaseUserAndLogin(requestContext, associatedDataTier.ConnectionInfo, initialCatalog, databasePool, userId, false, dropUser1, logger);
      return flag;
    }

    internal bool DropDatabaseUserAndLogin(
      IVssRequestContext requestContext,
      TeamFoundationDatabaseCredential credential,
      ITFLogger logger)
    {
      if (credential.InUseEndTime.HasValue)
        throw new ArgumentException("This overload of DropDatabaseUserAndLogin is currently only intended for alternate credentials due to its lack of a grace period.");
      if (credential.DatabaseId == -2)
        throw new VirtualServiceHostException();
      ITeamFoundationDatabaseProperties database = this.GetDatabase(requestContext, credential.DatabaseId, true);
      TeamFoundationDatabasePool databasePool = this.GetDatabasePool(requestContext, database.PoolName);
      DataTierInfo associatedDataTier = requestContext.GetService<TeamFoundationDataTierService>().FindAssociatedDataTier(requestContext, database.ConnectionInfoWrapper.ConnectionString);
      SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(database.ConnectionInfoWrapper.ConnectionString);
      string dataSource = connectionStringBuilder.DataSource;
      string initialCatalog = connectionStringBuilder.InitialCatalog;
      bool success = true;
      if (!this.IsLoginEligibleForDropping(requestContext, associatedDataTier.ConnectionInfo, dataSource, initialCatalog, databasePool, false, TimeSpan.Zero, false, credential, logger, ref success))
        return success;
      List<AvailabilityReplica> availabilityGroup = TeamFoundationDatabaseManagementService.GetReplicaNodesFromAvailabilityGroup(associatedDataTier.ConnectionInfo, logger);
      bool dropUser1 = true;
      bool removeCredential1 = true;
      if (availabilityGroup != null && availabilityGroup.Count > 0)
      {
        foreach (AvailabilityReplica availabilityReplica in availabilityGroup)
        {
          ISqlConnectionInfo dataTierConnectionInfo = associatedDataTier.ConnectionInfo.CloneReplaceDataSource(availabilityReplica.Node);
          bool removeCredential2;
          bool dropUser2 = removeCredential2 = availabilityReplica.Role == AvailabilityReplicaRole.Primary;
          success &= this.DropDatabaseUserAndLogin(requestContext, dataTierConnectionInfo, initialCatalog, databasePool, credential, false, dropUser2, removeCredential2, logger);
        }
      }
      else
        success &= this.DropDatabaseUserAndLogin(requestContext, associatedDataTier.ConnectionInfo, initialCatalog, databasePool, credential, false, dropUser1, removeCredential1, logger);
      return success;
    }

    private bool IsLoginEligibleForDisabling(
      IVssRequestContext requestContext,
      ISqlConnectionInfo dataTierConnectionInfo,
      string sqlInstance,
      string databaseName,
      TeamFoundationDatabasePool pool,
      TimeSpan minReplacedTime,
      TeamFoundationDatabaseCredential credential,
      ITFLogger logger,
      ref bool success)
    {
      if (credential.CredentialStatus != TeamFoundationDatabaseCredentialStatus.PendingDelete)
      {
        requestContext.Trace(99265, TraceLevel.Verbose, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, "Credential {0} ('{1}') is not marked for deletion.", (object) credential.Id, (object) credential.UserId);
        return false;
      }
      if (this.ReferenceSameSqlLogin(requestContext.FrameworkConnectionInfo, sqlInstance, credential.UserId))
      {
        string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Credential {0} ('{1}') is marked for deletion, but is being used to make this very call! Sql Instance: {2}", (object) credential.Id, (object) credential.UserId, (object) dataTierConnectionInfo.DataSource);
        logger.Error(message);
        requestContext.Trace(99266, TraceLevel.Error, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, message);
        success = false;
        return false;
      }
      SqlLoginInfo login;
      using (TeamFoundationSqlSecurityComponent componentRaw = dataTierConnectionInfo.CreateComponentRaw<TeamFoundationSqlSecurityComponent>(logger: logger))
        login = componentRaw.GetLogin(credential.UserId);
      if (login == null)
      {
        string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Credential {0} ('{1}') not found on SQL instance {2}", (object) credential.Id, (object) credential.UserId, (object) dataTierConnectionInfo.DataSource);
        logger.Warning(message);
        requestContext.Trace(99267, TraceLevel.Error, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, message);
      }
      if (login != null && !login.IsEnabled)
      {
        string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Credential {0} ('{1}') is already disabled.", (object) credential.Id, (object) credential.UserId);
        logger.Info(message);
        requestContext.Trace(99268, TraceLevel.Info, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, message);
        return false;
      }
      DateTime? inUseEndTime = credential.InUseEndTime;
      if (inUseEndTime.HasValue)
      {
        DateTime utcNow = DateTime.UtcNow;
        inUseEndTime = credential.InUseEndTime;
        DateTime dateTime = inUseEndTime.Value;
        TimeSpan timeSpan = utcNow - dateTime;
        if (timeSpan < TimeSpan.Zero)
        {
          string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Credential {0} ('{1}') has an InUseEndTime is in the future. UtcNow: {2}; InUseEndTime: {3}; Difference: {4}", (object) credential.Id, (object) credential.UserId, (object) DateTime.UtcNow, (object) credential.InUseEndTime, (object) timeSpan);
          logger.Error(message);
          requestContext.Trace(99269, TraceLevel.Error, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, message);
          success = false;
          return false;
        }
        if (timeSpan < minReplacedTime)
        {
          string message = string.Format("Credential {0} ('{1}') has been only replaced for {2}. Not eligible for disabling.", (object) credential.Id, (object) credential.UserId, (object) timeSpan);
          requestContext.Trace(99270, TraceLevel.Info, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, message);
          logger.Info(message);
          return false;
        }
        requestContext.Trace(99271, TraceLevel.Verbose, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, "Credential {0} ('{1}') was in use, but has been replaced for {2}, which is long enough.", (object) credential.Id, (object) credential.UserId, (object) timeSpan);
      }
      else
        requestContext.Trace(99272, TraceLevel.Verbose, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, "Credential {0} ('{1}') was never in use and can be disabled immediately.", (object) credential.Id, (object) credential.UserId);
      if (this.IsDatabaseSpecificLogin(credential))
        return true;
      string message1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Credential {0} ('{1}') may not be a database-specific login. Not eligible for disabling.", (object) credential.Id, (object) credential.UserId);
      logger.Error(message1);
      requestContext.Trace(99273, TraceLevel.Error, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, message1);
      success = false;
      return false;
    }

    private bool IsLoginEligibleForDropping(
      IVssRequestContext requestContext,
      ISqlConnectionInfo dataTierConnectionInfo,
      string sqlInstance,
      string databaseName,
      TeamFoundationDatabasePool pool,
      bool loginMustBeDisabled,
      TimeSpan minDisabledTime,
      bool checkProcessStartTimes,
      TeamFoundationDatabaseCredential credential,
      ITFLogger logger,
      ref bool success)
    {
      if (credential.CredentialStatus != TeamFoundationDatabaseCredentialStatus.PendingDelete)
      {
        requestContext.Trace(99274, TraceLevel.Verbose, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, "Credential {0} ('{1}') is not marked for deletion. Not eligible for login.", (object) credential.Id, (object) credential.UserId);
        return false;
      }
      if (this.ReferenceSameSqlLogin(requestContext.FrameworkConnectionInfo, sqlInstance, credential.UserId))
      {
        string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Credential {0} ('{1}') is marked for deletion, but is being used to make this very call! Sql Instance: {2}", (object) credential.Id, (object) credential.UserId, (object) dataTierConnectionInfo.DataSource);
        logger.Error(message);
        requestContext.Trace(99275, TraceLevel.Error, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, message);
        success = false;
        return false;
      }
      SqlLoginInfo login;
      using (TeamFoundationSqlSecurityComponent componentRaw = dataTierConnectionInfo.CreateComponentRaw<TeamFoundationSqlSecurityComponent>(logger: logger))
        login = componentRaw.GetLogin(credential.UserId);
      if (login == null)
      {
        string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Credential {0} ('{1}') not found on SQL instance {2}", (object) credential.Id, (object) credential.UserId, (object) dataTierConnectionInfo.DataSource);
        logger.Warning(message);
        requestContext.Trace(99276, TraceLevel.Warning, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, message);
      }
      if (loginMustBeDisabled && login != null)
      {
        if (login.IsEnabled)
        {
          string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Credential {0} ('{1}') has not been disabled. Not eligible for dropping.", (object) credential.Id, (object) credential.UserId);
          logger.Info(message);
          requestContext.Trace(99277, TraceLevel.Info, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, message);
          return false;
        }
        TimeSpan timeSpan = DateTime.UtcNow - login.ModifyDate;
        if (timeSpan < minDisabledTime)
        {
          string message = string.Format("Credential {0} ('{1}') has been only disabled for {2}. Not eligible for dropping.", (object) credential.Id, (object) credential.UserId, (object) timeSpan);
          requestContext.Trace(99278, TraceLevel.Info, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, message);
          logger.Info(message);
          return false;
        }
      }
      DateTime? inUseEndTime = credential.InUseEndTime;
      if (inUseEndTime.HasValue && checkProcessStartTimes)
      {
        List<TeamFoundationServiceHostProcess> serviceHostProcessList = requestContext.GetService<TeamFoundationHostManagementService>().QueryServiceHostProcesses(requestContext, Guid.Empty);
        int num = 0;
        foreach (TeamFoundationServiceHostProcess serviceHostProcess in serviceHostProcessList)
        {
          DateTime startTime = serviceHostProcess.StartTime;
          inUseEndTime = credential.InUseEndTime;
          DateTime dateTime = inUseEndTime.Value;
          if (startTime <= dateTime)
          {
            string message = string.Format("Credential {0} ('{1}') could potentially still be in use by {2} running on {3} (processId {4}).", (object) credential.Id, (object) credential.UserId, (object) serviceHostProcess.ProcessName, (object) serviceHostProcess.MachineName, (object) serviceHostProcess.OSProcessId);
            requestContext.Trace(99279, TraceLevel.Info, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, message);
            logger.Info(message);
            ++num;
          }
        }
        if (num > 0)
        {
          string message = string.Format("Credential {0} ('{1}') could potentially still be in use by {2} server processes. Not eligible for dropping.", (object) credential.Id, (object) credential.UserId, (object) num);
          requestContext.Trace(99280, TraceLevel.Info, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, message);
          logger.Info(message);
          return false;
        }
      }
      if (this.IsDatabaseSpecificLogin(credential))
        return true;
      string message1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Credential {0} ('{1}') may not be a database-specific login. Not eligible for dropping.", (object) credential.Id, (object) credential.UserId);
      logger.Error(message1);
      requestContext.Trace(99281, TraceLevel.Error, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, message1);
      success = false;
      return false;
    }

    private bool DisableDatabaseLogin(
      IVssRequestContext requestContext,
      ISqlConnectionInfo dataTierConnectionInfo,
      TeamFoundationDatabaseCredential credential,
      bool verifyOnly,
      ITFLogger logger)
    {
      bool flag = true;
      string dataSource = dataTierConnectionInfo.DataSource;
      SqlLoginInfo login;
      using (TeamFoundationSqlSecurityComponent componentRaw = dataTierConnectionInfo.CreateComponentRaw<TeamFoundationSqlSecurityComponent>(logger: logger))
      {
        login = componentRaw.GetLogin(credential.UserId);
        if (login != null)
        {
          if (!verifyOnly)
            componentRaw.DisableSqlLogin(credential.UserId);
        }
      }
      if (login != null)
      {
        if (!verifyOnly)
        {
          string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Disabled login for credential {0} ('{1}').\r\nIf service issues are detected, re-enable the login by running this statement against the master database on {2}: ALTER LOGIN [{1}] ENABLE\r\n\r\n", (object) credential.Id, (object) credential.UserId, (object) dataSource);
          logger.Info(message);
          requestContext.Trace(99282, TraceLevel.Info, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, message);
        }
        else
          logger.Info("VerifyOnly: Skipping disabling login for credential {0} ('{1}').", (object) credential.Id, (object) credential.UserId);
      }
      else
      {
        string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "No login {0} on sql instance {1}.", (object) credential.UserId, (object) dataSource);
        logger.Warning(message);
        requestContext.Trace(99283, TraceLevel.Warning, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, message);
      }
      return flag;
    }

    private bool DropDatabaseUserAndLogin(
      IVssRequestContext requestContext,
      ISqlConnectionInfo dataTierConnectionInfo,
      string databaseName,
      TeamFoundationDatabasePool pool,
      string userId,
      bool verifyOnly,
      bool dropUser,
      ITFLogger logger)
    {
      bool flag1 = true;
      bool flag2 = false;
      bool flag3 = false;
      bool flag4 = false;
      string dataSource = dataTierConnectionInfo.DataSource;
      SqlLoginInfo loginInfo;
      using (TeamFoundationSqlSecurityComponent componentRaw = dataTierConnectionInfo.CreateComponentRaw<TeamFoundationSqlSecurityComponent>(logger: logger))
      {
        loginInfo = componentRaw.GetLogin(userId);
        flag2 = componentRaw.IsSqlAzure;
      }
      if (loginInfo != null)
      {
        if (!flag2)
          new RetryManager(5, TimeSpan.FromSeconds(2.0), (Action<Exception>) (ex => logger.Warning(ex))).Invoke((Action) (() =>
          {
            List<DmvSession> dmvSessionList1 = new List<DmvSession>();
            List<DmvSession> dmvSessionList2;
            using (SessionManagementComponent componentRaw = dataTierConnectionInfo.CreateComponentRaw<SessionManagementComponent>(logger: logger))
              dmvSessionList2 = componentRaw.QuerySessionsByLogin(loginInfo.LoginName);
            if (dmvSessionList2.Count <= 0)
              return;
            foreach (DmvSession dmvSession in dmvSessionList2)
            {
              if (dmvSession.SessionStatus.Equals("sleeping", StringComparison.InvariantCultureIgnoreCase))
              {
                using (SessionManagementComponent componentRaw = dataTierConnectionInfo.CreateComponentRaw<SessionManagementComponent>(logger: logger))
                {
                  logger.Info("Kill session {0} in sleeping status", (object) dmvSession.SessionId);
                  componentRaw.Kill(dmvSession.SessionId);
                }
              }
              else
                dmvSessionList1.Add(dmvSession);
            }
            if (dmvSessionList1.Count > 0)
            {
              StringBuilder stringBuilder = new StringBuilder();
              stringBuilder.AppendFormat("Wait for following session(s) before we can drop login {0}\n", (object) loginInfo.LoginName);
              foreach (DmvSession dmvSession in dmvSessionList1)
                stringBuilder.AppendLine(dmvSession.ToString());
              dmvSessionList1.Clear();
              throw new TeamFoundationServiceException(stringBuilder.ToString());
            }
          }));
        if (dropUser)
        {
          SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(dataTierConnectionInfo.ConnectionString)
          {
            InitialCatalog = databaseName
          };
          if (connectionStringBuilder.ConnectTimeout < 120)
            connectionStringBuilder.ConnectTimeout = 120;
          using (TeamFoundationSqlSecurityComponent componentRaw = dataTierConnectionInfo.Create(connectionStringBuilder.ConnectionString).CreateComponentRaw<TeamFoundationSqlSecurityComponent>(logger: logger))
          {
            string databaseUserNameBySid = componentRaw.GetDatabaseUserNameBySid(loginInfo.Sid);
            if (databaseUserNameBySid != null)
            {
              if (!verifyOnly)
              {
                componentRaw.DropDatabaseUser(databaseUserNameBySid);
                requestContext.Trace(99284, TraceLevel.Verbose, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, "Dropped database user '{0}' associated with login '{1}'.", (object) databaseUserNameBySid, (object) userId);
                flag3 = true;
              }
              else
                logger.Info("VerifyOnly: Skipping drop of database user '{0}' associated with login '{1}'.", (object) databaseUserNameBySid, (object) userId);
            }
            else
            {
              string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "No database user associated with login '{0}'. Skipping database user drop.", (object) userId);
              logger.Warning(message);
              requestContext.Trace(99285, TraceLevel.Warning, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, message);
            }
          }
        }
        else
          logger.Info("Drop database user for login " + userId + " is skipped");
        using (TeamFoundationSqlSecurityComponent componentRaw = dataTierConnectionInfo.CreateComponentRaw<TeamFoundationSqlSecurityComponent>(logger: logger))
        {
          if (!verifyOnly)
          {
            try
            {
              componentRaw.DropLogin(userId);
              requestContext.Trace(99286, TraceLevel.Verbose, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, "Dropped login {0} on sql instance {1}.", (object) userId, (object) dataSource);
              flag4 = true;
            }
            catch (Exception ex)
            {
              flag1 = false;
              logger.Error(string.Format("Fail to drop the login {0}. Credential will not be dropped from tbl_databaseCredential. Please check the error and solve the issue and try again.\r\nException: {1}", (object) userId, (object) ex));
            }
          }
          else
            logger.Info("VerifyOnly: Skipping drop of login {0} on sql instance {1}.", (object) userId, (object) dataSource);
        }
      }
      else
      {
        string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "No login {0} on sql instance {1}.", (object) userId, (object) dataSource);
        logger.Warning(message);
        requestContext.Trace(99287, TraceLevel.Warning, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, message);
      }
      if (!verifyOnly & flag1)
      {
        string message = string.Format("Database user dropped: {0}. Login dropped: {1}.", (object) flag3, (object) flag4);
        logger.Info(message);
        requestContext.Trace(99288, TraceLevel.Info, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, message);
      }
      return flag1;
    }

    private bool DropDatabaseUserAndLogin(
      IVssRequestContext requestContext,
      ISqlConnectionInfo dataTierConnectionInfo,
      string databaseName,
      TeamFoundationDatabasePool pool,
      TeamFoundationDatabaseCredential credential,
      bool verifyOnly,
      bool dropUser,
      bool removeCredential,
      ITFLogger logger)
    {
      bool flag = this.DropDatabaseUserAndLogin(requestContext, dataTierConnectionInfo, databaseName, pool, credential.UserId, verifyOnly, dropUser, logger);
      if (!verifyOnly)
      {
        if (flag)
        {
          if (removeCredential)
          {
            this.RemoveCredentials(requestContext, (IEnumerable<int>) new int[1]
            {
              credential.Id
            });
            string message = string.Format("Removed credential {0} ('{1}').", (object) credential.Id, (object) credential.UserId);
            logger.Info(message);
            requestContext.Trace(99289, TraceLevel.Info, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, message);
          }
          else
            logger.Info("Skipping removal of credential {0} ('{1}'). RemoveCredential=false", (object) credential.Id, (object) credential.UserId);
        }
        else
          logger.Info("Skipping removal of credential {0} ('{1}'). Failed to remove login and/or user.", (object) credential.Id, (object) credential.UserId);
      }
      else
        logger.Info("VerifyOnly: Skipping removal of credential {0} ('{1}').", (object) credential.Id, (object) credential.UserId);
      return flag;
    }

    private void AlterAuthorizationToDbo(
      ISqlConnectionInfo dbConnectionInfo,
      string databaseName,
      ITFLogger logger)
    {
      using (TeamFoundationSqlSecurityComponent componentRaw = dbConnectionInfo.CloneReplaceInitialCatalog(databaseName).CreateComponentRaw<TeamFoundationSqlSecurityComponent>(logger: logger))
      {
        logger.Info("Alter authorization of schemas and fulltext catatlog to dbo");
        componentRaw.AlterAuthorizationToDbo();
      }
    }

    public List<TeamFoundationDatabaseCredential> GetCredentialsForDatabase(
      IVssRequestContext requestContext,
      int databaseId,
      string credentialName = null)
    {
      TeamFoundationDatabaseManagementService.CheckDeploymentRequestContext(requestContext);
      if (databaseId == -2)
        throw new VirtualServiceHostException();
      List<TeamFoundationDatabaseCredential> source;
      using (DatabaseCredentialsComponent component = requestContext.CreateComponent<DatabaseCredentialsComponent>())
        source = component.QueryDatabaseCredentials(databaseId);
      if (!string.IsNullOrEmpty(credentialName))
        source = source.Where<TeamFoundationDatabaseCredential>((Func<TeamFoundationDatabaseCredential, bool>) (cred => cred.Name.Equals(credentialName, StringComparison.OrdinalIgnoreCase))).ToList<TeamFoundationDatabaseCredential>();
      return source;
    }

    public List<TeamFoundationDatabaseCredential> QueryDatabaseCredentials(
      IVssRequestContext requestContext)
    {
      TeamFoundationDatabaseManagementService.CheckDeploymentRequestContext(requestContext);
      using (DatabaseCredentialsComponent component = requestContext.CreateComponent<DatabaseCredentialsComponent>())
        return component.QueryDatabaseCredentials();
    }

    public void RemoveCredentials(
      IVssRequestContext requestContext,
      IEnumerable<TeamFoundationDatabaseCredential> credentials)
    {
      IEnumerable<int> credentialIds = credentials.Select<TeamFoundationDatabaseCredential, int>((Func<TeamFoundationDatabaseCredential, int>) (c => c.Id));
      this.RemoveCredentials(requestContext, credentialIds);
    }

    public void RemoveCredentials(IVssRequestContext requestContext, IEnumerable<int> credentialIds)
    {
      TeamFoundationDatabaseManagementService.CheckDeploymentRequestContext(requestContext);
      if (new List<int>(credentialIds).Count <= 0)
        return;
      using (DatabaseCredentialsComponent component = requestContext.CreateComponent<DatabaseCredentialsComponent>())
        component.RemoveDatabaseCredentials(credentialIds);
    }

    public bool CheckAvailabilityGroupHealth(
      IVssRequestContext requestContext,
      List<ITeamFoundationDatabaseProperties> dbs,
      ITFLogger logger)
    {
      TeamFoundationDataTierService service = requestContext.GetService<TeamFoundationDataTierService>();
      HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (ITeamFoundationDatabaseProperties db in dbs)
      {
        DataTierInfo associatedDataTier = service.FindAssociatedDataTier(requestContext, db.SqlConnectionInfo.ConnectionString);
        if (!stringSet.Contains(associatedDataTier.DataSource))
        {
          stringSet.Add(associatedDataTier.DataSource);
          foreach (AvailabilityReplica availabilityReplica in TeamFoundationDatabaseManagementService.GetReplicaNodesFromAvailabilityGroup(associatedDataTier.ConnectionInfo, logger))
          {
            if (availabilityReplica.DatabaseCount > 0 && availabilityReplica.Health != AvailabilityReplicaSynchronizationState.Healthy || availabilityReplica.Role != AvailabilityReplicaRole.Primary && availabilityReplica.Role != AvailabilityReplicaRole.Secondary)
            {
              logger.Error(string.Format("Data tier {0} has availability group set up but replica {1} is not healthy: Role = {2}; Health = {3}.", (object) associatedDataTier.DataSource, (object) availabilityReplica.Node, (object) availabilityReplica.Role, (object) availabilityReplica.Health));
              return false;
            }
          }
        }
      }
      return true;
    }

    private void GenerateSqlLogin(
      IVssRequestContext deploymentRequestContext,
      TeamFoundationDatabaseType databaseType,
      bool isDbOwner,
      string serviceName,
      out string loginName,
      out string loginPassword)
    {
      if (isDbOwner)
      {
        if (serviceName == null)
          serviceName = deploymentRequestContext.GetService<CachedRegistryService>().GetValue<string>(deploymentRequestContext, (RegistryQuery) FrameworkServerConstants.DatabasePrefix, (string) null);
        loginName = SqlAzureLoginGenerator.CreateDboLoginName(serviceName);
      }
      else if (databaseType == TeamFoundationDatabaseType.Partition)
      {
        string prefix = deploymentRequestContext.GetService<CachedRegistryService>().GetValue<string>(deploymentRequestContext, (RegistryQuery) FrameworkServerConstants.AccountDatabaseLoginPrefix, (string) null);
        loginName = string.IsNullOrEmpty(prefix) ? SqlAzureLoginGenerator.CreateCollectionDbLoginName() : SqlAzureLoginGenerator.CreateLoginName(prefix);
      }
      else
      {
        if (databaseType != TeamFoundationDatabaseType.Configuration)
          throw new ArgumentException(FrameworkResources.DatabaseTypeNotSupported((object) databaseType), nameof (databaseType));
        loginName = SqlAzureLoginGenerator.CreateConfigDbLoginName(serviceName);
      }
      loginPassword = SqlAzureLoginGenerator.CreateLoginPassword(loginName);
    }

    private bool IsDatabaseSpecificLogin(TeamFoundationDatabaseCredential credential) => credential.Name.Equals(DatabaseCredentialNames.DefaultCredential) || credential.Name.Equals(DatabaseCredentialNames.DbOwnerCredential) || credential.Name.Equals(DatabaseCredentialNames.MigrationReadOnlyCredential) || credential.Name.Equals(DatabaseCredentialNames.BulkMigrationReadOnlyCredential) || credential.Name.Equals(DatabaseCredentialNames.BulkMigrationReadWriteCredential);

    private bool ReferenceSameSqlLogin(
      ISqlConnectionInfo connectionInfo,
      string sqlInstance,
      string userId)
    {
      if (!string.Equals(((ISupportSqlCredential) connectionInfo).UserId, userId, StringComparison.OrdinalIgnoreCase))
        return false;
      DataSourceOptions options = DataSourceOptions.RemoveProtocol;
      return string.Equals(TeamFoundationDataTierService.GetDataSource(connectionInfo.ConnectionString, options), TeamFoundationDataTierService.ManipulateDataSource(sqlInstance, options), StringComparison.OrdinalIgnoreCase);
    }

    private bool ProvisionSqlLoginAndUser(
      IVssRequestContext requestContext,
      ITFLogger logger,
      ISqlConnectionInfo dataTierConnectionInfo,
      ITeamFoundationDatabaseProperties databaseProperties,
      DatabaseReplicationContext replicationContext,
      TeamFoundationDatabaseCredential credential,
      string rawPassword,
      string roleName,
      bool grantViewServerState)
    {
      TeamFoundationDatabaseManagementService.CheckDeploymentRequestContext(requestContext);
      TeamFoundationDatabaseManagementService.CheckHosted(requestContext);
      requestContext.CheckServicingRequestContext();
      if (credential.Id < 1)
        throw new ArgumentException("Credentials have to be registered prior to provisioning.", nameof (credential));
      if (credential.CredentialStatus != TeamFoundationDatabaseCredentialStatus.Provisioning && credential.CredentialStatus != TeamFoundationDatabaseCredentialStatus.InUse)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Credential status {0} not supported.", (object) credential.CredentialStatus), nameof (credential));
      this.ProvisionSqlLoginAndUser(requestContext, logger, dataTierConnectionInfo, databaseProperties, replicationContext, credential.UserId, rawPassword, roleName, grantViewServerState, credential.Name.Equals(DatabaseCredentialNames.DefaultCredential));
      if (credential.Name.Equals(DatabaseCredentialNames.MigrationReadOnlyCredential) || credential.Name.Equals(DatabaseCredentialNames.BulkMigrationReadOnlyCredential))
      {
        using (TeamFoundationSqlSecurityComponent componentRaw = dataTierConnectionInfo.CloneReplaceInitialCatalog(databaseProperties.SqlConnectionInfo.InitialCatalog).CreateComponentRaw<TeamFoundationSqlSecurityComponent>())
          componentRaw.GrantExecutePermission(DatabaseRoles.TfsReaderRole, "dbo.prc_PrepareExecution");
      }
      credential.CredentialStatus = TeamFoundationDatabaseCredentialStatus.InUse;
      this.UpdateCredential(requestContext, credential);
      replicationContext.WaitForDatabaseCopy(requestContext);
      foreach (IDatabaseReplicaInfo replica in replicationContext.Replicas)
      {
        if (replica.NewCredential != null)
        {
          replica.NewCredential.CredentialStatus = TeamFoundationDatabaseCredentialStatus.InUse;
          replica.UpdateCredential(requestContext, logger);
        }
      }
      return true;
    }

    private SqlLoginInfo ProvisionSqlLoginAndUser(
      IVssRequestContext requestContext,
      ITFLogger logger,
      ISqlConnectionInfo dataTierConnectionInfo,
      ITeamFoundationDatabaseProperties databaseProperties,
      DatabaseReplicationContext replicationContext,
      string userId,
      string rawPassword,
      string roleName,
      bool grantViewServerState,
      bool isDefault)
    {
      string initialCatalog = new SqlConnectionStringBuilder(databaseProperties.ConnectionInfoWrapper.ConnectionString).InitialCatalog;
      SqlLoginInfo sqlLogin = this.CreateSqlLogin(requestContext, dataTierConnectionInfo, userId, rawPassword, grantViewServerState, logger);
      foreach (ISqlConnectionInfo dataTierConnectionInfo1 in TeamFoundationDatabaseManagementService.GetReplicaNodesFromAvailabilityGroup(dataTierConnectionInfo, logger).Where<AvailabilityReplica>((Func<AvailabilityReplica, bool>) (r => r.Role != AvailabilityReplicaRole.Primary)).Select<AvailabilityReplica, ISqlConnectionInfo>((Func<AvailabilityReplica, ISqlConnectionInfo>) (r => dataTierConnectionInfo.CloneReplaceDataSource(r.Node))).ToList<ISqlConnectionInfo>())
        this.CreateSqlLogin(requestContext, dataTierConnectionInfo1, userId, rawPassword, grantViewServerState, logger, sqlLogin.Sid);
      foreach (IDatabaseReplicaInfo replica in replicationContext.Replicas)
        this.CreateSqlLogin(requestContext, replica.DataTierConnectionInfo, userId, rawPassword, grantViewServerState, logger, sqlLogin.Sid);
      this.ProvisionUser(requestContext, dataTierConnectionInfo, initialCatalog, userId, logger, roleName);
      if (isDefault && !requestContext.GetService<CachedRegistryService>().GetValue<bool>(requestContext, (RegistryQuery) FrameworkServerConstants.OptInPrivilegedSqlLogin, false, false))
        TeamFoundationDatabaseManagementService.OptOutPrivilegedSqlLogin(dataTierConnectionInfo, databaseProperties.DboConnectionInfo, userId);
      if (databaseProperties.Flags.HasFlag((Enum) TeamFoundationDatabaseFlags.IntentReadOnlyOnSqlAzureDatabase))
      {
        logger.Info(string.Format("Sleep for {0} seconds to make sure replicas are in sync", (object) 30));
        Thread.Sleep(TimeSpan.FromSeconds(30.0));
      }
      return sqlLogin;
    }

    public static string GenerateDatabaseNameRaw(string databaseLabel, string databasePrefix)
    {
      databaseLabel = TeamFoundationDatabaseManagementService.AppendSeparator(databaseLabel);
      databasePrefix = TeamFoundationDatabaseManagementService.AppendSeparator(databasePrefix);
      return !string.IsNullOrEmpty(databasePrefix) ? databasePrefix + databaseLabel + Guid.NewGuid().ToString("D") : databaseLabel + Guid.NewGuid().ToString("D");
    }

    private static string AppendSeparator(string value)
    {
      if (!value.IsNullOrEmpty<char>() && !value.EndsWith("_"))
        value += "_";
      return value;
    }

    internal static List<AvailabilityReplica> GetReplicaNodesFromAvailabilityGroup(
      ISqlConnectionInfo dataTierConnectionInfo,
      ITFLogger logger)
    {
      List<AvailabilityReplica> availabilityGroup = new List<AvailabilityReplica>();
      using (TeamFoundationSqlSecurityComponent componentRaw = dataTierConnectionInfo.CreateComponentRaw<TeamFoundationSqlSecurityComponent>(logger: logger))
      {
        if (!componentRaw.IsSqlAzure)
        {
          if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("TFS_SKIP_SQL_AVAILABILITY_CHECK")))
          {
            DataSourceOptions options = DataSourceOptions.RemoveProtocol;
            string str1 = TeamFoundationDataTierService.ManipulateDataSource(dataTierConnectionInfo.DataSource, options);
            logger.Info("Check if availability group exists for dataSource " + str1 + "...");
            string[] strArray = str1.Split(',');
            string str2 = strArray[0];
            int startIndex = str2.IndexOf('.');
            int num = str2.IndexOf('\\');
            string domainExtension = startIndex < 0 ? string.Empty : (num > startIndex ? str2.Substring(startIndex, num - startIndex) : str2.Substring(startIndex));
            availabilityGroup = componentRaw.GetAvailabilityReplicas(domainExtension, strArray.Length > 1 ? strArray[1] : (string) null);
            if (availabilityGroup.Count == 0)
              logger.Info("Availability group is not set up for server " + str1);
          }
        }
      }
      foreach (AvailabilityReplica availabilityReplica in availabilityGroup)
      {
        logger.Info(string.Format("Availability Group Replica: {0}, Role: {1}.", (object) availabilityReplica.Node, (object) availabilityReplica.Role));
        if (availabilityReplica.Role == AvailabilityReplicaRole.Disconnected || availabilityReplica.Role == AvailabilityReplicaRole.Resolving)
        {
          string message = string.Format("Replica {0} in availability group is in {1} Role.", (object) availabilityReplica.Node, (object) availabilityReplica.Role);
          logger.Warning(message);
        }
        if (availabilityReplica.DatabaseCount > 0 && availabilityReplica.Health == AvailabilityReplicaSynchronizationState.NotHealthy || availabilityReplica.Health == AvailabilityReplicaSynchronizationState.PartiallyHealthy)
        {
          string message = string.Format("The synchronization state of Replica {0} in availability group is {1}.", (object) availabilityReplica.Node, (object) availabilityReplica.Health);
          logger.Warning(message);
        }
        if (availabilityReplica.DatabaseCount == 0 && availabilityReplica.Health == AvailabilityReplicaSynchronizationState.NotHealthy)
        {
          string message = "No database is joined in the availability group, the synchronization state will show not healthy but we can proceed.";
          logger.Info(message);
        }
      }
      return availabilityGroup;
    }

    internal static List<ISqlConnectionInfo> GetSecondaryDboConnections(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties db,
      Predicate<AvailabilityReplica> predicate = null,
      ApplicationIntent applicationIntent = ApplicationIntent.ReadWrite)
    {
      List<ISqlConnectionInfo> secondaryDboConnections = new List<ISqlConnectionInfo>();
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        return secondaryDboConnections;
      List<AvailabilityReplica> availabilityReplicaList = new List<AvailabilityReplica>();
      if (predicate == null)
        predicate = (Predicate<AvailabilityReplica>) (r => true);
      DataTierInfo associatedDataTier = requestContext.GetService<TeamFoundationDataTierService>().FindAssociatedDataTier(requestContext, db.SqlConnectionInfo.ConnectionString);
      object obj;
      List<AvailabilityReplica> source;
      if (requestContext.RootContext.Items.TryGetValue(associatedDataTier.DataSource, out obj) && obj is List<AvailabilityReplica>)
      {
        source = (List<AvailabilityReplica>) obj;
      }
      else
      {
        source = TeamFoundationDatabaseManagementService.GetReplicaNodesFromAvailabilityGroup(associatedDataTier.ConnectionInfo, (ITFLogger) new ServerTraceLogger());
        requestContext.RootContext.Items[associatedDataTier.DataSource] = (object) source;
      }
      return source.Where<AvailabilityReplica>((Func<AvailabilityReplica, bool>) (r => r.Role == AvailabilityReplicaRole.Secondary)).Where<AvailabilityReplica>((Func<AvailabilityReplica, bool>) (r => predicate(r))).Select<AvailabilityReplica, ISqlConnectionInfo>((Func<AvailabilityReplica, ISqlConnectionInfo>) (r => db.GetDboConnectionInfo().CloneReplaceDataSourceAndApplicationIntent(r.Node, applicationIntent))).ToList<ISqlConnectionInfo>();
    }

    internal static SqlLoginInfo CreateSqlLoginIncludingAvailabilityGroup(
      ISqlConnectionInfo masterConnectionInfo,
      ITFLogger logger,
      string loginName = null)
    {
      SqlLoginInfo availabilityGroup1 = (SqlLoginInfo) null;
      List<AvailabilityReplica> availabilityGroup2 = TeamFoundationDatabaseManagementService.GetReplicaNodesFromAvailabilityGroup(masterConnectionInfo, logger);
      string str = string.IsNullOrEmpty(loginName) ? "Network Service Account" : loginName;
      if (availabilityGroup2 != null && availabilityGroup2.Count > 0)
      {
        foreach (AvailabilityReplica availabilityReplica in availabilityGroup2)
        {
          if (availabilityReplica.Health != AvailabilityReplicaSynchronizationState.Healthy)
          {
            logger.Warning(string.Format("[Best Effort] {0} is not healthy:  Role={1}; Health={2}.\r\nPlease create the Sql login {3} manually on this replica.", (object) availabilityReplica.Node, (object) availabilityReplica.Role, (object) availabilityReplica.Health, (object) str));
          }
          else
          {
            logger.Info("Create Sql Login " + str + " for availability group replica " + availabilityReplica.Node);
            ISqlConnectionInfo connectionInfo = masterConnectionInfo.CloneReplaceDataSource(availabilityReplica.Node);
            try
            {
              if (availabilityReplica.Role == AvailabilityReplicaRole.Primary)
                availabilityGroup1 = TeamFoundationDatabaseManagementService.CreateSqlLoginOnSqlInstance(connectionInfo, logger, loginName);
              else
                TeamFoundationDatabaseManagementService.CreateSqlLoginOnSqlInstance(connectionInfo, logger, loginName);
            }
            catch
            {
              string message = string.Format("[Best Effort] Failed to create Sql login on replica. Synchronization state of {0}:  Role={1}; Health={2}.\r\nPlease create the Sql login {3} manually on this replica.", (object) availabilityReplica.Node, (object) availabilityReplica.Role, (object) availabilityReplica.Health, (object) str);
              if (availabilityReplica.Node.Contains("\\"))
                message = message + "\r\nTroubleshoot: The replica " + availabilityReplica.Node + " is a named instance. Please check if Sql Server Browser service is running, or create an alias for the replica with port 1433.";
              logger.Warning(message);
            }
          }
        }
      }
      if (availabilityGroup1 == null)
        availabilityGroup1 = TeamFoundationDatabaseManagementService.CreateSqlLoginOnSqlInstance(masterConnectionInfo, logger, loginName);
      return availabilityGroup1;
    }

    internal static void GrantServerScopePermissionsToLogin(
      ISqlConnectionInfo dtConnectionInfo,
      string loginName,
      Dictionary<string, bool> permissions,
      ITFLogger logger,
      bool bestEfforOnReplica = false)
    {
      List<AvailabilityReplica> availabilityGroup = TeamFoundationDatabaseManagementService.GetReplicaNodesFromAvailabilityGroup(dtConnectionInfo, logger);
      bool flag = false;
      if (availabilityGroup.Count > 0)
      {
        foreach (AvailabilityReplica availabilityReplica in availabilityGroup)
        {
          using (TeamFoundationSqlSecurityComponent componentRaw = dtConnectionInfo.CloneReplaceDataSource(availabilityReplica.Node).CreateComponentRaw<TeamFoundationSqlSecurityComponent>(logger: logger))
          {
            try
            {
              foreach (KeyValuePair<string, bool> permission in permissions)
                componentRaw.GrantServerScopePermission(loginName, permission.Key, permission.Value);
            }
            catch (Exception ex) when (bestEfforOnReplica)
            {
              flag = true;
              logger.Warning(string.Format("[Best Effort] Failed to grant server scope permission on replica. Synchronization state of {0}:  Role={1}; Health={2}", (object) availabilityReplica.Node, (object) availabilityReplica.Role, (object) availabilityReplica.Health));
            }
          }
        }
      }
      if (!(availabilityGroup.Count == 0 | flag))
        return;
      using (TeamFoundationSqlSecurityComponent componentRaw = dtConnectionInfo.CreateComponentRaw<TeamFoundationSqlSecurityComponent>(logger: logger))
      {
        if (componentRaw.IsSqlAzure)
          return;
        foreach (KeyValuePair<string, bool> permission in permissions)
          componentRaw.GrantServerScopePermission(loginName, permission.Key, permission.Value);
      }
    }

    internal static void RevokeServerScopePermissionsToLogin(
      ISqlConnectionInfo connectionInfo,
      string loginName,
      Dictionary<string, bool> permissions,
      ITFLogger logger)
    {
      List<ISqlConnectionInfo> list = TeamFoundationDatabaseManagementService.GetReplicaNodesFromAvailabilityGroup(connectionInfo, logger).Select<AvailabilityReplica, ISqlConnectionInfo>((Func<AvailabilityReplica, ISqlConnectionInfo>) (n => connectionInfo.CloneReplaceDataSource(n.Node))).ToList<ISqlConnectionInfo>();
      if (list.Count == 0)
        list.Add(connectionInfo);
      foreach (ISqlConnectionInfo connectionInfo1 in list)
      {
        using (TeamFoundationSqlSecurityComponent componentRaw = connectionInfo1.CreateComponentRaw<TeamFoundationSqlSecurityComponent>(logger: logger))
        {
          foreach (KeyValuePair<string, bool> permission in permissions)
          {
            if (!componentRaw.IsSqlAzure)
              componentRaw.RevokeServerScopePermission(loginName, permission.Key, (string) null, permission.Value);
          }
        }
      }
    }

    private static SqlLoginInfo CreateSqlLoginOnSqlInstance(
      ISqlConnectionInfo connectionInfo,
      ITFLogger logger,
      string loginName = null,
      string password = null)
    {
      using (TeamFoundationSqlSecurityComponent componentRaw = connectionInfo.CreateComponentRaw<TeamFoundationSqlSecurityComponent>(logger: logger))
      {
        if (string.IsNullOrEmpty(loginName))
          return componentRaw.CreateNetworkServiceLogin();
        return string.IsNullOrEmpty(password) ? componentRaw.CreateWindowsAuthLogin(loginName) : componentRaw.CreateSqlAuthLogin(loginName, password);
      }
    }

    private TeamFoundationDatabaseCredential RegisterCredential(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties databaseProperties,
      string loginName,
      string loginPassword,
      string credentialName = null,
      string description = null,
      bool markOtherProvisioningCredsForDeletion = true)
    {
      if (databaseProperties.DatabaseId == -2)
        throw new VirtualServiceHostException();
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      ITeamFoundationSigningService service = vssRequestContext.GetService<ITeamFoundationSigningService>();
      Guid databaseSigningKey = service.GetDatabaseSigningKey(requestContext);
      byte[] passwordEncrypted = service.Encrypt(vssRequestContext, databaseSigningKey, Encoding.UTF8.GetBytes(loginPassword), SigningAlgorithm.SHA256);
      using (DatabaseCredentialsComponent component = requestContext.CreateComponent<DatabaseCredentialsComponent>())
        return component.RegisterDatabaseCredential(databaseProperties.DatabaseId, loginName, passwordEncrypted, databaseSigningKey, markOtherProvisioningCredsForDeletion, credentialName ?? DatabaseCredentialNames.DefaultCredential, description);
    }

    internal void UpdateCredential(
      IVssRequestContext requestContext,
      TeamFoundationDatabaseCredential credential)
    {
      if (credential.DatabaseId == -2)
        throw new VirtualServiceHostException();
      using (DatabaseCredentialsComponent component = requestContext.CreateComponent<DatabaseCredentialsComponent>())
      {
        if (component is DatabaseCredentialsComponent5)
        {
          ((DatabaseCredentialsComponent5) component).UpdateDatabaseCredential(credential);
        }
        else
        {
          int num = !credential.Name.Equals(DatabaseCredentialNames.DefaultCredential) ? 0 : (credential.CredentialStatus == TeamFoundationDatabaseCredentialStatus.InUse ? 1 : 0);
          component.UpdateDatabaseCredential(credential, false);
        }
      }
      if (!credential.Name.Equals(DatabaseCredentialNames.DbOwnerCredential) && !credential.Name.Equals(DatabaseCredentialNames.DefaultCredential))
        return;
      this.GetDatabase(requestContext, credential.DatabaseId, false);
    }

    internal SqlLoginInfo CreateSqlLogin(
      IVssRequestContext requestContext,
      ISqlConnectionInfo dataTierConnectionInfo,
      string userId,
      string rawPassword,
      bool grantViewServerState,
      ITFLogger logger,
      byte[] sid = null)
    {
      string dataSource = dataTierConnectionInfo.DataSource;
      using (TeamFoundationSqlSecurityComponent componentRaw = dataTierConnectionInfo.CreateComponentRaw<TeamFoundationSqlSecurityComponent>(logger: logger))
      {
        SqlLoginInfo sqlLogin = componentRaw.GetLogin(userId);
        if (sqlLogin != null)
        {
          string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SQL login '{0}' already exists on sql instance '{1}'. Skipping login creation.", (object) userId, (object) dataSource);
          requestContext.Trace(99290, TraceLevel.Warning, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, message);
          logger.Warning(message);
        }
        else
        {
          sqlLogin = sid == null ? componentRaw.CreateSqlAuthLogin(userId, rawPassword) : componentRaw.CreateSqlAuthLoginForSid(userId, rawPassword, sid);
          string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Created SQL login '{0}' on sql instance '{1}'.", (object) userId, (object) dataSource);
          requestContext.Trace(99291, TraceLevel.Info, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, message);
          logger.Info(message);
        }
        if (!componentRaw.IsSqlAzure & grantViewServerState)
        {
          componentRaw.GrantViewServerStatePermission(userId);
          componentRaw.GrantAlterAnyConnectionPermission(userId, false);
        }
        if (sid != null && !((IEnumerable<byte>) sqlLogin.Sid).SequenceEqual<byte>((IEnumerable<byte>) sid))
          throw new InvalidConfigurationException("The Sid of login " + userId + " on " + dataTierConnectionInfo.DataSource + " does not match with primary!");
        return sqlLogin;
      }
    }

    internal void ProvisionUser(
      IVssRequestContext requestContext,
      ISqlConnectionInfo dataTierConnectionInfo,
      string databaseName,
      string loginName,
      ITFLogger logger,
      string roleName,
      bool isAADUser = false,
      string aadGuid = "")
    {
      TeamFoundationDatabaseManagementService.CheckDeploymentRequestContext(requestContext);
      TeamFoundationDatabaseManagementService.CheckHosted(requestContext);
      requestContext.CheckServicingRequestContext();
      SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(dataTierConnectionInfo.ConnectionString)
      {
        InitialCatalog = databaseName
      };
      if (connectionStringBuilder.ConnectTimeout < 120)
        connectionStringBuilder.ConnectTimeout = 120;
      using (TeamFoundationSqlSecurityComponent componentRaw = dataTierConnectionInfo.Create(connectionStringBuilder.ConnectionString).CreateComponentRaw<TeamFoundationSqlSecurityComponent>(logger: logger))
      {
        string user = componentRaw.CreateUser(loginName, isAADUser, aadGuid);
        componentRaw.EnsureRoleExists(roleName);
        componentRaw.AddRoleMember(roleName, user);
      }
    }

    private static void CheckDeploymentRequestContext(IVssRequestContext deploymentRequestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(deploymentRequestContext, nameof (deploymentRequestContext));
      if (!deploymentRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(deploymentRequestContext.ServiceHost.HostType);
    }

    private static void CheckHosted(IVssRequestContext requestContext)
    {
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        throw new InvalidOperationException(FrameworkResources.MethodAvailableInHostedTfsOnly());
    }

    private Microsoft.VisualStudio.Services.Identity.Identity[] GetServiceIdentities(
      IVssRequestContext deploymentRequestContext)
    {
      deploymentRequestContext.CheckDeploymentRequestContext();
      IdentityService service = deploymentRequestContext.GetService<IdentityService>();
      return service.ReadIdentities(deploymentRequestContext, (IList<IdentityDescriptor>) (service.ReadIdentities(deploymentRequestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        GroupWellKnownIdentityDescriptors.ServiceUsersGroup
      }, QueryMembership.Direct, (IEnumerable<string>) null)[0] ?? throw new IdentityNotFoundException(GroupWellKnownIdentityDescriptors.ServiceUsersGroup)).Members.ToArray<IdentityDescriptor>(), QueryMembership.None, (IEnumerable<string>) null).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (identity => identity != null && identity.Descriptor.IdentityType == typeof (WindowsIdentity).ToString())).ToArray<Microsoft.VisualStudio.Services.Identity.Identity>();
    }

    private static string GetMachineNameFromConnectionString(string connectionString)
    {
      string connectionString1 = new SqlConnectionStringBuilder(connectionString).DataSource.Split('\\', ',')[0];
      if (connectionString1.StartsWith(FrameworkServerConstants.TcpProtocolPrefix, StringComparison.OrdinalIgnoreCase))
        connectionString1 = connectionString1.Substring(FrameworkServerConstants.TcpProtocolPrefix.Length);
      if (connectionString1.StartsWith(FrameworkServerConstants.NamedPipesProtocolPrefix, StringComparison.OrdinalIgnoreCase))
        connectionString1 = connectionString1.Substring(FrameworkServerConstants.NamedPipesProtocolPrefix.Length);
      return connectionString1;
    }

    private static DatabaseReplicationContext GetDatabaseReplicationContext(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties databaseProperties,
      ISqlConnectionInfo datatierConnectionInfo,
      ITFLogger logger)
    {
      DatabaseReplicationContext replicationContext = requestContext.GetService<IDatabaseReplicationService>().GetDatabaseReplicationContext(requestContext, databaseProperties, datatierConnectionInfo, logger);
      if (!replicationContext.IsPrimary)
      {
        logger.Warning("Skipping database " + databaseProperties.DatabaseName + " - database is not primary");
        return replicationContext;
      }
      if (replicationContext.Replicas.Count == 0)
        replicationContext = TeamFoundationDatabaseManagementService.GetSqlFailoverGroupReplicationContext(requestContext, databaseProperties, datatierConnectionInfo, logger);
      return replicationContext;
    }

    private static DatabaseReplicationContext GetSqlFailoverGroupReplicationContext(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties databaseProperties,
      ISqlConnectionInfo datatierConnectionInfo,
      ITFLogger logger)
    {
      return requestContext.GetService<IDatabaseFailoverGroupService>().GetDatabaseReplicationContext(requestContext, databaseProperties, datatierConnectionInfo, logger);
    }

    private int GetNewDatabaseSize(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties database,
      TeamFoundationDatabaseManagementService.DatabaseSizeProperties sizeProperties)
    {
      int newMaxSizeInGB = 0;
      if (sizeProperties.CurrentToMaxSizeRatio > (double) sizeProperties.ResizeSettings.SizePercentageToGrowDatabase && !sizeProperties.CanGrowDatabase(out newMaxSizeInGB))
        requestContext.TraceAlways(1012505, TraceLevel.Warning, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, "Database {0} (Id = {1}) already at max size {2}GB, unable to increase max size.", (object) database.DatabaseName, (object) database.DatabaseId, (object) sizeProperties.CurrentMaxSizeInGB);
      if (newMaxSizeInGB == 0 && requestContext.IsFeatureEnabled(FrameworkServerConstants.DatabaseDownSizeEnabled))
      {
        DateTime downgradeDisabledUntilTime;
        if (this.IsDowngradeOrDownsizeDisabled(requestContext, database, out downgradeDisabledUntilTime))
          requestContext.TraceAlways(1012513, TraceLevel.Info, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, string.Format("Downsize is disabled for database {0} due to on-fly host move/migration disabled the downsize/downgrade until {1}.", (object) database.DatabaseName, (object) downgradeDisabledUntilTime));
        else if (sizeProperties.IsVCoreBased() && database.IsEligibleForRightSizing())
        {
          int allocatedStorageInGb = this.GetAllocatedStorageInGB(requestContext, database);
          if (sizeProperties.CurrentMaxSizeInGB > allocatedStorageInGb && sizeProperties.CurrentToMaxSizeRatio < (double) sizeProperties.ResizeSettings.SizePercentageToDownsizeVcoreDatabase)
          {
            int num = sizeProperties.ResizeSettings.SizePercentageToDownsizeVcoreDatabase + 10;
            newMaxSizeInGB = Math.Max(allocatedStorageInGb, Math.Max(sizeProperties.CurrentSizeInGB * 100 / num, sizeProperties.CurrentSizeInGB + sizeProperties.ResizeSettings.OverheadSizeInGBVcoreDatabase));
            if (sizeProperties.CurrentMaxSizeInGB - newMaxSizeInGB < 5)
              newMaxSizeInGB = 0;
          }
        }
      }
      return newMaxSizeInGB;
    }

    private void AlterDBSize(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties database,
      int newMaxDBSizeInGB,
      TeamFoundationDatabaseManagementService.DatabaseSizeProperties sizeProperties)
    {
      try
      {
        string str = newMaxDBSizeInGB > sizeProperties.CurrentMaxSizeInGB ? "Increasing" : "Decreasing";
        requestContext.TraceAlways(1012507, TraceLevel.Warning, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, string.Format("{0} database {1} (Id = {2}) max size from {3}GB to {4}GB.", (object) str, (object) database.DatabaseName, (object) database.DatabaseId, (object) sizeProperties.CurrentMaxSizeInGB, (object) newMaxDBSizeInGB));
        this.AlterSqlAzureDatabaseMaxSize(requestContext, database, newMaxDBSizeInGB);
        requestContext.To(TeamFoundationHostType.Deployment).GetService<IChangeRecordService>().LogCompletedChangeEvent(str + " database " + database.DatabaseName + " size", string.Format("DatabaseId = {0} {1} max size from {2}GB to {3}GB.", (object) database.DatabaseId, (object) str, (object) sizeProperties.CurrentMaxSizeInGB, (object) newMaxDBSizeInGB));
        sizeProperties.CurrentMaxSizeInGB = newMaxDBSizeInGB;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1012506, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, ex);
      }
    }

    private void UpdateDBPropertySizes(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties database,
      TeamFoundationDatabaseManagementService.DatabaseSizeProperties sizeProperties)
    {
      int num = database.MaxTenants - database.Tenants;
      requestContext.TraceAlways(1012511, TraceLevel.Info, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, string.Format("Database: {0}, MaxTenants: {1}, Tenants: {2}, FreeDatabaseTenants: {3}.", (object) database.DatabaseName, (object) database.MaxTenants, (object) database.Tenants, (object) num));
      if (database.DatabaseSize == sizeProperties.CurrentSizeInMB && database.DatabaseCapacity == sizeProperties.CurrentMaxSizeInMB)
        return;
      this.UpdateDatabaseProperties(requestContext, database.DatabaseId, (Action<TeamFoundationDatabaseProperties>) (editableProperties =>
      {
        if (TeamFoundationDatabaseManagementService.ShouldSealDatabase(database, sizeProperties.CurrentToMaxSupportedRatio, (double) sizeProperties.ResizeSettings.SizePercentageToSealDatabase))
        {
          requestContext.TraceAlways(1012509, TraceLevel.Info, TeamFoundationDatabaseManagementService.s_Area, TeamFoundationDatabaseManagementService.s_Layer, string.Format("Database {0} usage ({1}% to current max size / {2}% to SKU's max size) is over the sealing threshold of {3}%. Sealing database.", (object) database.DatabaseName, (object) sizeProperties.CurrentToMaxSizeRatio, (object) sizeProperties.CurrentToMaxSupportedRatio, (object) sizeProperties.ResizeSettings.SizePercentageToSealDatabase));
          editableProperties.MaxTenants = database.Tenants;
        }
        editableProperties.DatabaseSize = sizeProperties.CurrentSizeInMB;
        editableProperties.DatabaseCapacity = sizeProperties.CurrentMaxSizeInMB;
      }));
    }

    internal static string ParseUniqueConnectionStringFields(string connectionString)
    {
      SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
      string str = connectionStringBuilder.DataSource;
      string initialCatalog = connectionStringBuilder.InitialCatalog;
      if (str.StartsWith("tcp:", StringComparison.OrdinalIgnoreCase))
        str = str.Substring(4);
      return str + ";" + initialCatalog;
    }

    [ClientEditorBrowsable(EditorBrowsableState.Never)]
    internal void RefreshThrottleConfigurationCache(
      IVssRequestContext requestContext,
      TimeSpan? refreshInterval = null,
      bool allowStaleValues = true)
    {
      this.m_throttleConfigurationCache = new VssRefreshCache<TeamFoundationDatabaseManagementService.TeamFoundationDatabaseThrottleConfiguration>(refreshInterval ?? TimeSpan.FromSeconds(DatabaseManagementConstants.PerfCacheRefreshInterval), (Func<IVssRequestContext, TeamFoundationDatabaseManagementService.TeamFoundationDatabaseThrottleConfiguration>) (deploymentContext => new TeamFoundationDatabaseManagementService.TeamFoundationDatabaseThrottleConfiguration(deploymentContext)), allowStaleValues);
    }

    private class TeamFoundationDatabaseThrottleConfiguration
    {
      public TeamFoundationDatabaseThrottleConfiguration(IVssRequestContext deploymentContext)
      {
        deploymentContext.CheckDeploymentRequestContext();
        deploymentContext = deploymentContext.Elevate(false);
        IVssRegistryService service = deploymentContext.GetService<IVssRegistryService>();
        this.DataStaleSecondsThreshold = service.GetValue<short>(deploymentContext, (RegistryQuery) FrameworkServerConstants.RequestThrottlingDTDataStaleSeconds, DatabaseManagementConstants.DefaultDTDataStaleSeconds);
        this.AvgCpuThreshold = service.GetValue<short>(deploymentContext, (RegistryQuery) FrameworkServerConstants.RequestThrottlingDTAvgCpuPercent, DatabaseManagementConstants.DefaultDTAvgCpuPercentThreshold);
        this.AvgDataIOThreshold = service.GetValue<short>(deploymentContext, (RegistryQuery) FrameworkServerConstants.RequestThrottlingDTAvgDataIOPercent, DatabaseManagementConstants.DefaultDTAvgDataIOPercentThreshold);
        this.AvgLogWriteThreshold = service.GetValue<short>(deploymentContext, (RegistryQuery) FrameworkServerConstants.RequestThrottlingDTAvgLogWritePercent, DatabaseManagementConstants.DefaultDTAvgLogWritePercentThreshold);
        this.AvgMemoryUsageThreshold = service.GetValue<short>(deploymentContext, (RegistryQuery) FrameworkServerConstants.RequestThrottlingDTAvgMemoryUsagePercent, DatabaseManagementConstants.DefaultDTAvgMemoryUsagePercentThreshold);
        this.MaxWorkerThreshold = service.GetValue<short>(deploymentContext, (RegistryQuery) FrameworkServerConstants.RequestThrottlingDTMaxWorkerPercent, DatabaseManagementConstants.DefaultDTMaxWorkerPercentThreshold);
        this.PageLatchAverageWaitTimeMSThreshold = service.GetValue<int>(deploymentContext, (RegistryQuery) FrameworkServerConstants.RequestThrottlingPageLatchAverageWaitTimeMSThreshold, DatabaseManagementConstants.DefaultPageLatchAverageWaitTimeMSThreshold);
      }

      public short DataStaleSecondsThreshold { get; }

      public short AvgCpuThreshold { get; }

      public short AvgDataIOThreshold { get; }

      public short AvgLogWriteThreshold { get; }

      public short AvgMemoryUsageThreshold { get; }

      public short MaxWorkerThreshold { get; }

      public int PageLatchAverageWaitTimeMSThreshold { get; }
    }

    private class DatabaseSizeProperties
    {
      private readonly AzureDatabaseProperties m_dbProperties;

      public DatabaseSizeProperties(
        AzureDatabaseProperties properties,
        TeamFoundationDatabaseManagementService.ResizeSettings resizeSettings)
      {
        this.ResizeSettings = resizeSettings;
        this.m_dbProperties = properties;
      }

      public bool IsVCoreBased() => this.m_dbProperties.Edition.IsVCoreBased();

      public bool CanGrowDatabase(out int newMaxSizeInGB) => this.m_dbProperties.CanGrowDatabase(out newMaxSizeInGB);

      public double CurrentToMaxSizeRatio => this.m_dbProperties.CurrentMaxSizeInMB <= 0 ? 0.0 : (double) this.m_dbProperties.SizeInMB / (double) this.m_dbProperties.CurrentMaxSizeInMB * 100.0;

      public double CurrentToMaxSupportedRatio => this.m_dbProperties.CurrentMaxSizeInMB <= 0 ? 0.0 : 100.0 * (double) this.m_dbProperties.SizeInMB / Math.Max((double) this.m_dbProperties.MaxStoragePossibleInGB * 1024.0, (double) this.m_dbProperties.CurrentMaxSizeInMB);

      public int CurrentMaxSizeInMB => this.m_dbProperties.CurrentMaxSizeInMB;

      public int CurrentMaxSizeInGB
      {
        get => this.m_dbProperties.CurrentMaxSizeInMB / 1024;
        set => this.m_dbProperties.CurrentMaxSizeInMB = value * 1024;
      }

      public int CurrentSizeInMB => this.m_dbProperties.SizeInMB;

      public int CurrentSizeInGB => this.m_dbProperties.SizeInMB / 1024;

      public TeamFoundationDatabaseManagementService.ResizeSettings ResizeSettings { get; }
    }

    private struct ResizeSettings
    {
      public ResizeSettings(IVssRequestContext requestContext)
        : this()
      {
        IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
        this.SizePercentageToSealDatabase = service.GetValue<int>(requestContext, (RegistryQuery) FrameworkServerConstants.SizePercentageToSealDatabase, 75);
        this.SizePercentageToGrowDatabase = service.GetValue<int>(requestContext, (RegistryQuery) FrameworkServerConstants.SizePercentageToGrowDatabase, 80);
        this.SizePercentageToDownsizeVcoreDatabase = service.GetValue<int>(requestContext, (RegistryQuery) FrameworkServerConstants.SizePercentageToDownsizeVcoreDatabase, 50);
        this.OverheadSizeInGBVcoreDatabase = service.GetValue<int>(requestContext, (RegistryQuery) FrameworkServerConstants.OverheadSizeInGBVcoreDatabase, 20);
      }

      public int SizePercentageToSealDatabase { get; }

      public int SizePercentageToGrowDatabase { get; }

      public int SizePercentageToDownsizeVcoreDatabase { get; }

      public int OverheadSizeInGBVcoreDatabase { get; }
    }
  }
}
