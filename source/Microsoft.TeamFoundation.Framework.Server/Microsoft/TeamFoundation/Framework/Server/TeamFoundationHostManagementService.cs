// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationHostManagementService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server.Authorization;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public sealed class TeamFoundationHostManagementService : 
    IInternalTeamFoundationHostManagementService,
    ITeamFoundationHostManagementService,
    IVssFrameworkService
  {
    private object m_hostStateValidationDriverCreateLock = new object();
    private TeamFoundationHostStateValidationDriver m_hostStateValidationDriver;
    private static readonly int s_defaultDormancyInterval = 5;
    private static readonly string s_commandCacheLimitPath = "/Configuration/ResourceUsage/CommandCacheLimit";
    private static readonly string s_xmlParameterChunkSettings = "/Service/Framework/Settings";
    private static readonly string s_xmlParameterChunkThreshold = TeamFoundationHostManagementService.s_xmlParameterChunkSettings + "XmlParameterChunkThreshold";
    private static readonly string s_xmlParameterChunkThresholdInBytes = TeamFoundationHostManagementService.s_xmlParameterChunkSettings + "XmlParameterChunkThresholdSettingInBytes";
    private static readonly string s_configurationPath = FrameworkServerConstants.HostManagementRoot + "/...";
    internal const string c_storageAccountConfigurationPathPrefix = "/Configuration/FileService/StorageAccounts";
    internal const string c_fileServiceStorageAccountPrefix = "FileServiceStorageAccount";
    private static readonly RegistryQuery[] s_notificationFilters = new RegistryQuery[4]
    {
      new RegistryQuery(TeamFoundationHostManagementService.s_commandCacheLimitPath),
      new RegistryQuery(TeamFoundationHostManagementService.s_xmlParameterChunkThreshold),
      new RegistryQuery(TeamFoundationHostManagementService.s_xmlParameterChunkThresholdInBytes),
      new RegistryQuery(TeamFoundationHostManagementService.s_configurationPath)
    };
    private DeploymentServiceHost m_deploymentServiceHost;
    private Guid m_processId;
    private Guid m_machineId;
    private static readonly string s_machineName;
    private static readonly string s_processName;
    private CancellationTokenSource m_masterCancellationTokenSource;
    private DateTime m_leaseExpiry;
    private const int c_leaseDuration = 15;
    private const int c_leaseRenewal = 14;
    private const int c_leaseDurationDebug = 20;
    private TimeSpan m_timeToSleep;
    private int m_dormancyThreads;
    private int m_maxHosts;
    private int m_updateRecentlyAccessedHostsSeconds;
    private readonly HashSet<Guid> m_recentUserAccessHosts = new HashSet<Guid>();
    private readonly ReaderWriterLock m_recentUserAccessHostsLock = new ReaderWriterLock();
    private Task m_leaseRenewalTask;
    private Timer m_dormancyTimer;
    private Timer m_selfDestructionTimer;
    private bool m_forceLastUserAccessUpdate;
    private readonly object m_dormancyLock = new object();
    private IDisposableReadOnlyList<IHostManagementExtension> m_extensions;
    private static readonly string s_Area = "HostManagement";
    private static readonly string s_Layer = "BusinessLogic";
    [ThreadStatic]
    private static bool ts_IsNotificationThread;
    private TimeSpan m_leaseDuration;
    private HostManagementKernel<VssServiceHost, VssRequestContext> m_kernel;
    private const int c_queryChildrenServiceHostPropertiesBatchSizeLimit = 1000;
    private const int c_queryServiceHostPropertiesBatchSizeLimit = 1000;
    internal static readonly string[] s_v1ServiceVirtualDirectories = new string[7]
    {
      "Build/",
      "Lab/",
      "Services/",
      "VersionControl/",
      "TestManagement/",
      "WorkItemTracking/",
      "Warehouse/"
    };

    static TeamFoundationHostManagementService()
    {
      TeamFoundationHostManagementService.s_machineName = Environment.MachineName;
      try
      {
        TeamFoundationHostManagementService.s_processName = Microsoft.TeamFoundation.Common.Internal.NativeMethods.GetModuleFileName();
        if (!string.IsNullOrEmpty(TeamFoundationHostManagementService.s_processName))
          TeamFoundationHostManagementService.s_processName = Path.GetFileName(TeamFoundationHostManagementService.s_processName);
        else
          TeamFoundationHostManagementService.s_processName = "Unknown";
      }
      catch (Exception ex)
      {
        TeamFoundationHostManagementService.s_processName = "Unknown";
      }
    }

    internal TeamFoundationHostManagementService()
    {
    }

    private int GetConfigSetting(string settingName, int min, int max, int def)
    {
      int configSetting = def;
      if (ConfigurationManager.AppSettings[settingName] != null)
      {
        int result;
        if (int.TryParse(ConfigurationManager.AppSettings[settingName], out result))
        {
          if (result >= min && result <= max)
          {
            TeamFoundationTracingService.TraceRawAlwaysOn(57009, TraceLevel.Warning, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, "Overriding Host Management Setting {0} with {1}", (object) settingName, (object) configSetting);
            configSetting = result;
          }
          else
            TeamFoundationTracingService.TraceRawAlwaysOn(57008, TraceLevel.Error, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, "Host Management Setting {0} value of {1} is not within acceptable bounds [{2}-{3}] - default {4} will be used", (object) settingName, (object) configSetting, (object) min, (object) max, (object) def);
        }
        else
          TeamFoundationTracingService.TraceRawAlwaysOn(57007, TraceLevel.Error, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, "Host Management Setting {0} value of {1} is not valid", (object) ConfigurationManager.AppSettings[settingName], (object) configSetting);
      }
      return configSetting;
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      TeamFoundationTracingService.TraceEnterRaw(57000, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, "ServiceStart", new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
      this.m_deploymentServiceHost = (DeploymentServiceHost) systemRequestContext.ServiceHost.DeploymentServiceHost;
      int num = 0;
      this.m_processId = Guid.NewGuid();
      TeamFoundationTracingService.TraceRaw(57001, TraceLevel.Info, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, "Process Id {0}", (object) this.m_processId);
      if (systemRequestContext.ServiceHost.HasDatabaseAccess)
      {
        using (ExtendedAttributeComponent componentRaw = systemRequestContext.FrameworkConnectionInfo.CreateComponentRaw<ExtendedAttributeComponent>())
          this.DeploymentType = componentRaw.ReadDeploymentTypeStamp();
        try
        {
          (num, this.m_leaseDuration) = TeamFoundationHostManagementService.GetSettingsFromRegistry(RegistryHelpers.GetDeploymentValuesRaw(systemRequestContext.FrameworkConnectionInfo, FrameworkServerConstants.HostManagementRoot), systemRequestContext.ServiceHost.DeploymentServiceHost.ProcessType);
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(57002, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, ex);
        }
      }
      if (this.m_leaseDuration == TimeSpan.Zero)
      {
        switch (this.m_deploymentServiceHost.ProcessType)
        {
          case HostProcessType.Generic:
            this.m_leaseDuration = HostManagementConstants.ServiceHostProcessLeaseRenewalGeneric;
            break;
          case HostProcessType.ApplicationTier:
            this.m_leaseDuration = HostManagementConstants.ServiceHostProcessLeaseRenewalAT;
            break;
          case HostProcessType.JobAgent:
            this.m_leaseDuration = HostManagementConstants.ServiceHostProcessLeaseRenewalJA;
            break;
          default:
            this.m_leaseDuration = HostManagementConstants.ServiceHostProcessLeaseRenewalOther;
            break;
        }
      }
      if (num == 0)
        num = 7680;
      int configSetting1 = this.GetConfigSetting("VssfHostPartitions", 2, 32, 16);
      int configSetting2 = this.GetConfigSetting("VssfMaxHosts", 1, (int) short.MaxValue, num);
      int configSetting3 = this.GetConfigSetting("VssfMinHosts", 1, 512, 1);
      TeamFoundationTracingService.TraceRawAlwaysOn(57003, TraceLevel.Info, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, "Max Total Hosts configured to:{0}, Lease Duration:{1}", (object) configSetting2, (object) this.m_leaseDuration);
      long requestId = (long) -Environment.CurrentManagedThreadId;
      this.m_kernel = new HostManagementKernel<VssServiceHost, VssRequestContext>((VssServiceHost) systemRequestContext.ServiceHost, new HostManagementKernel<VssServiceHost, VssRequestContext>.LoadHost(this.LoadHost), requestId, configSetting1, configSetting2, configSetting3);
      TeamFoundationTracingService.TraceLeaveRaw(57004, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, "ServiceStart");
    }

    internal static (int MaxHosts, TimeSpan LeaseDuration) GetSettingsFromRegistry(
      IEnumerable<RegistryEntry> registryEntries,
      HostProcessType hostProcessType)
    {
      int num = 0;
      TimeSpan timeSpan = TimeSpan.Zero;
      Dictionary<string, string> dictionary = registryEntries.ToDictionary<RegistryEntry, string, string>((Func<RegistryEntry, string>) (r => r.Path), (Func<RegistryEntry, string>) (r => r.Value));
      string s1;
      if (dictionary.TryGetValue(FrameworkServerConstants.MaxTotalHosts, out s1))
      {
        int result;
        int.TryParse(s1, out result);
        if (result > 0)
          num = result;
      }
      string s2;
      int result1;
      if ((dictionary.TryGetValue(RegistryHelpers.CombinePath(FrameworkServerConstants.HostManagementLeaseDuration, hostProcessType.ToString()), out s2) || dictionary.TryGetValue(FrameworkServerConstants.HostManagementLeaseDuration, out s2)) && int.TryParse(s2, out result1) && result1 > 2 && result1 < 1440)
        timeSpan = TimeSpan.FromMinutes((double) result1);
      return (num, timeSpan);
    }

    internal void CalculateStorageAccountId(
      IVssRequestContext deploymentContext,
      ref TeamFoundationServiceHostProperties hostProperties)
    {
      int num = -1;
      if (hostProperties.HostType == TeamFoundationHostType.ProjectCollection)
      {
        TeamFoundationServiceHostProperties serviceHostProperties = this.QueryServiceHostProperties(deploymentContext, hostProperties.ParentId);
        if (serviceHostProperties.StorageAccountId != -2)
          num = serviceHostProperties.StorageAccountId;
      }
      if (num == -1)
        num = TeamFoundationHostManagementService.CalculateStorageAccountIdRaw(deploymentContext, hostProperties.Id);
      hostProperties.StorageAccountId = num;
    }

    internal static int CalculateStorageAccountIdRaw(
      IVssRequestContext deploymentContext,
      Guid hostId)
    {
      TeamFoundationTracingService.TraceEnterRaw(57033, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, nameof (CalculateStorageAccountIdRaw), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      try
      {
        List<int> intList = new List<int>();
        ITeamFoundationStrongBoxService service = deploymentContext.GetService<ITeamFoundationStrongBoxService>();
        Guid drawerId = service.UnlockDrawer(deploymentContext, "ConfigurationSecrets", !deploymentContext.ServiceHost.IsProduction);
        if (!drawerId.Equals(Guid.Empty))
        {
          RegistryEntryCollection registryEntryCollection = deploymentContext.GetService<ISqlRegistryService>().ReadEntries(deploymentContext, (RegistryQuery) "/Configuration/FileService/StorageAccounts/...");
          foreach (StrongBoxItemInfo strongBoxItemInfo in service.GetDrawerContents(deploymentContext, drawerId).Where<StrongBoxItemInfo>((Func<StrongBoxItemInfo, bool>) (item => item.LookupKey.StartsWith("FileServiceStorageAccount", StringComparison.InvariantCultureIgnoreCase))))
          {
            string s = strongBoxItemInfo.LookupKey.Substring("FileServiceStorageAccount".Length);
            int result;
            if (!string.IsNullOrEmpty(s) && int.TryParse(s, out result))
            {
              if (registryEntryCollection.GetValueFromPath<bool>("/Configuration/FileService/StorageAccounts/Account." + s + "/Enabled", true))
                intList.Add(result);
              else
                deploymentContext.Trace(57036, TraceLevel.Warning, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, "Ignoring Storage account {0} ({1}) because it is marked as disabled", (object) strongBoxItemInfo.LookupKey, (object) result);
            }
            else
              deploymentContext.Trace(57035, TraceLevel.Error, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, "Found malformed strongbox item {0} while enumerating accounts", (object) strongBoxItemInfo.LookupKey);
          }
        }
        return intList.Count == 0 ? 0 : intList[(hostId.GetHashCode() & int.MaxValue) % intList.Count];
      }
      catch (Exception ex)
      {
        deploymentContext.TraceException(57037, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, ex);
        throw;
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(57034, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, nameof (CalculateStorageAccountIdRaw));
      }
    }

    private VssServiceHost LoadHost(HostProperties properties, VssServiceHost parentHost)
    {
      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();
      TeamFoundationTracingService.TraceRaw(57007, TraceLevel.Verbose, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, "Start of LoadHost for {0} : host type = {1} : parent host type = {2} ", (object) properties.Id, (object) properties.HostType, (object) parentHost.HostType);
      VssPerformanceEventSource.Log.LoadHostStart(properties.Id);
      try
      {
        return new VssServiceHost(properties, parentHost);
      }
      finally
      {
        stopwatch.Stop();
        VssPerformanceEventSource.Log.LoadHostStop(properties.Id, stopwatch.ElapsedMilliseconds);
        TeamFoundationTracingService.TraceRaw(57005, TraceLevel.Info, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, "Host {0} loaded in {1} ms", (object) properties.Id, (object) stopwatch.ElapsedMilliseconds);
        if (stopwatch.ElapsedMilliseconds > 5000L)
        {
          TraceEvent trace = new TraceEvent("Host '{0}' ({1}) took {2} ms to start up", (object) properties.Name, (object) properties.Id, (object) stopwatch.ElapsedMilliseconds);
          TeamFoundationTracingService.GetTraceEvent(ref trace, 57006, TraceLevel.Error, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, (string[]) null, (string) null);
          trace.ServiceHost = properties.Id;
          TeamFoundationTracingService.TraceRaw(ref trace);
        }
        if (properties.HostType == TeamFoundationHostType.ProjectCollection)
          VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_RequestsForDormantCollectionsPerSec").Increment();
      }
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
      TeamFoundationTracingService.TraceEnterRaw(57008, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, "ServiceEnd", new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      try
      {
        if (this.LeaseRenewed != null && this.LeaseRenewed.GetInvocationList().Length != 0)
          TeamFoundationTracingService.TraceRaw(0, TraceLevel.Info, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, "Nulling out LeaseRenewed, which has {0} registered delegates.", (object) this.LeaseRenewed.GetInvocationList().Length);
        this.LeaseRenewed = (EventHandler) null;
        if (this.m_extensions == null)
          return;
        this.m_extensions.Dispose();
        this.m_extensions = (IDisposableReadOnlyList<IHostManagementExtension>) null;
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(57009, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, "ServiceEnd");
      }
    }

    internal void Dispose()
    {
      this.DisposeTimers();
      lock (this.m_hostStateValidationDriverCreateLock)
      {
        if (this.m_hostStateValidationDriver != null)
        {
          this.m_hostStateValidationDriver.Dispose();
          this.m_hostStateValidationDriver = (TeamFoundationHostStateValidationDriver) null;
        }
      }
      lock (this)
      {
        if (this.m_selfDestructionTimer == null)
          return;
        this.m_selfDestructionTimer.Dispose();
      }
    }

    public DeploymentType DeploymentType { get; private set; }

    internal bool ForceLastUserAccessUpdate => this.m_forceLastUserAccessUpdate;

    public bool IsHosted => this.DeploymentType == DeploymentType.Cloud || this.DeploymentType == DeploymentType.DevFabric;

    public int HostDormancySeconds { get; private set; }

    public void CreateServiceHost(
      IVssRequestContext requestContext,
      TeamFoundationServiceHostProperties hostProperties,
      ISqlConnectionInfo connectionInfo)
    {
      this.CreateServiceHost(requestContext, hostProperties, connectionInfo, CreateHostOptions.Default);
    }

    public void CreateServiceHost(
      IVssRequestContext requestContext,
      TeamFoundationServiceHostProperties hostProperties,
      ISqlConnectionInfo connectionInfo,
      CreateHostOptions createOptions)
    {
      TeamFoundationTracingService.TraceEnterRaw(57030, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, "CreateServiceHost {0}", (object) hostProperties);
      try
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckForNull<TeamFoundationServiceHostProperties>(hostProperties, nameof (hostProperties));
        ArgumentUtility.CheckForEmptyGuid(hostProperties.Id, "hostProperties.Id");
        if (hostProperties.DatabaseId != -2)
          ArgumentUtility.CheckForNull<ISqlConnectionInfo>(connectionInfo, nameof (connectionInfo));
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        if ((createOptions & CreateHostOptions.AssignStorage) != CreateHostOptions.None)
          this.CalculateStorageAccountId(vssRequestContext, ref hostProperties);
        if (hostProperties.Status != TeamFoundationServiceHostStatus.Started && hostProperties.SubStatus == ServiceHostSubStatus.None)
          hostProperties.SubStatus = ServiceHostSubStatus.Creating;
        this.EnsureExtensionsLoaded(requestContext);
        foreach (IHostManagementExtension extension in (IEnumerable<IHostManagementExtension>) this.m_extensions)
          extension.OnBeforeCreateServiceHost(requestContext, hostProperties, connectionInfo);
        using (HostManagementComponent component = vssRequestContext.CreateComponent<HostManagementComponent>())
          component.CreateServiceHost(hostProperties.Id, hostProperties.ParentId, hostProperties.Name, hostProperties.Description, hostProperties.Status, hostProperties.StatusReason, hostProperties.DatabaseId, hostProperties.HostType, hostProperties.ServiceLevel, hostProperties.StorageAccountId, hostProperties.SubStatus);
        if ((createOptions & CreateHostOptions.CreatePartition) != CreateHostOptions.None)
        {
          DatabasePartition databasePartition;
          using (DatabasePartitionComponent component = DatabasePartitionComponent.CreateComponent(connectionInfo))
          {
            component.CreatePartition(hostProperties.Id, DatabasePartitionState.Active, hostProperties.HostType, new int?());
            databasePartition = component.QueryPartition(hostProperties.Id);
          }
          if (databasePartition.State == DatabasePartitionState.Deleted)
            throw new PartitionAlreadyExistsException(hostProperties.Id, hostProperties.DatabaseId);
        }
        this.m_kernel.OnServiceHostCreated((HostProperties) hostProperties, this.RequestId);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(57031, TraceLevel.Error, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, ex);
        throw;
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(57032, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, nameof (CreateServiceHost));
      }
    }

    public void UpdateServiceHost(
      IVssRequestContext requestContext,
      TeamFoundationServiceHostProperties hostProperties)
    {
      TeamFoundationTracingService.TraceEnterRaw(57040, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, nameof (UpdateServiceHost), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      try
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckForNull<TeamFoundationServiceHostProperties>(hostProperties, nameof (hostProperties));
        ArgumentUtility.CheckForEmptyGuid(hostProperties.Id, "hostProperties.Id");
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        TeamFoundationTracingService.TraceRaw(57041, TraceLevel.Verbose, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, "Determining Existing Host Properties");
        TeamFoundationServiceHostProperties oldProperties = this.QueryServiceHostProperties(vssRequestContext, hostProperties.Id);
        if (oldProperties == null)
        {
          TeamFoundationTracingService.TraceRaw(57042, TraceLevel.Warning, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, "Host {0} does not exist", (object) hostProperties.Id);
          throw new HostDoesNotExistException(hostProperties.Id);
        }
        TeamFoundationTracingService.TraceRaw(57043, TraceLevel.Verbose, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, "Found Host Properties {0}", (object) hostProperties);
        this.EnsureExtensionsLoaded(requestContext);
        foreach (IHostManagementExtension extension in (IEnumerable<IHostManagementExtension>) this.m_extensions)
          extension.OnBeforeUpdateServiceHost(requestContext, hostProperties, oldProperties);
        using (HostManagementComponent component = vssRequestContext.CreateComponent<HostManagementComponent>())
        {
          int databaseId = hostProperties.DatabaseId;
          int storageAccountId = hostProperties.StorageAccountId;
          if (hostProperties.BackupData != null)
          {
            databaseId = hostProperties.BackupData.DatabaseId;
            storageAccountId = hostProperties.BackupData.StorageAccountId;
          }
          component.UpdateServiceHost(hostProperties.Id, hostProperties.ParentId, hostProperties.Name, hostProperties.Description, databaseId, storageAccountId, hostProperties.ServiceLevel, hostProperties.SubStatus);
        }
        foreach (IHostManagementExtension extension in (IEnumerable<IHostManagementExtension>) this.m_extensions)
          extension.OnAfterUpdateServiceHost(requestContext, hostProperties, oldProperties);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(57043, TraceLevel.Error, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, ex);
        throw;
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(57044, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, nameof (UpdateServiceHost));
      }
    }

    public void DeleteServiceHost(
      IVssRequestContext requestContext,
      Guid hostId,
      HostDeletionReason deletionReason,
      DeleteHostResourceOptions deleteHostResourceOptions)
    {
      TeamFoundationTracingService.TraceEnterRaw(57050, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, nameof (DeleteServiceHost), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(hostId, nameof (hostId));
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        TeamFoundationServiceHostProperties hostProperties = this.QueryServiceHostProperties(vssRequestContext, hostId);
        if (hostProperties != null && hostProperties.DatabaseId != -2)
        {
          ITeamFoundationDatabaseProperties database = vssRequestContext.GetService<TeamFoundationDatabaseManagementService>().GetDatabase(vssRequestContext, hostProperties.DatabaseId, true);
          DatabasePartition partition = (DatabasePartition) null;
          if (deleteHostResourceOptions != DeleteHostResourceOptions.None)
          {
            try
            {
              using (DatabasePartitionComponent component = DatabasePartitionComponent.CreateComponent(database.SqlConnectionInfo))
                partition = component.QueryPartition(hostId);
            }
            catch (DatabasePartitionNotFoundException ex)
            {
              TeamFoundationTracingService.TraceExceptionRaw(57051, TraceLevel.Warning, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, (Exception) ex);
            }
            catch (Exception ex)
            {
              TeamFoundationTracingService.TraceExceptionRaw(57051, TraceLevel.Warning, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, ex);
              if (vssRequestContext.ExecutionEnvironment.IsHostedDeployment)
                throw;
            }
          }
          if (deleteHostResourceOptions.HasFlag((Enum) DeleteHostResourceOptions.MarkForDeletion) || deleteHostResourceOptions.HasFlag((Enum) DeleteHostResourceOptions.DeleteImmediately))
            TeamFoundationHostManagementService.MarkPartitionForDeletion(vssRequestContext, hostProperties, database, partition, deleteHostResourceOptions);
        }
        this.EnsureExtensionsLoaded(requestContext);
        foreach (IHostManagementExtension extension in (IEnumerable<IHostManagementExtension>) this.m_extensions)
          extension.OnBeforeDeleteServiceHost(requestContext, hostId, deletionReason, deleteHostResourceOptions);
        using (HostManagementComponent component = vssRequestContext.CreateComponent<HostManagementComponent>())
          component.DeleteServiceHost(hostId, deletionReason);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(57051, TraceLevel.Error, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, ex);
        throw;
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(57052, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, nameof (DeleteServiceHost));
      }
    }

    private static void MarkPartitionForDeletion(
      IVssRequestContext deploymentContext,
      TeamFoundationServiceHostProperties hostProperties,
      ITeamFoundationDatabaseProperties dbProperties,
      DatabasePartition partition,
      DeleteHostResourceOptions options)
    {
      if (partition == null)
        return;
      if (!options.HasFlag((Enum) DeleteHostResourceOptions.SkipMarkingBlobsForDeletion))
        TeamFoundationHostManagementService.SetStorageAccountIdInPartitionDb(hostProperties, dbProperties, partition);
      try
      {
        List<int> list;
        using (DataspaceComponent componentRaw = TeamFoundationResourceManagementService.CreateComponentRaw<DataspaceComponent>(dbProperties))
        {
          componentRaw.PartitionId = partition.PartitionId;
          list = componentRaw.QueryDataspaces().Where<Dataspace>((Func<Dataspace, bool>) (ds => ds.DatabaseId != dbProperties.DatabaseId)).Select<Dataspace, int>((Func<Dataspace, int>) (ds => ds.DatabaseId)).Distinct<int>().ToList<int>();
        }
        list.Add(dbProperties.DatabaseId);
        ITeamFoundationDatabaseManagementService service = deploymentContext.GetService<ITeamFoundationDatabaseManagementService>();
        for (int index = 0; index < list.Count; ++index)
        {
          ITeamFoundationDatabaseProperties database = service.GetDatabase(deploymentContext, list[index]);
          try
          {
            using (DatabasePartitionComponent component = DatabasePartitionComponent.CreateComponent(database.SqlConnectionInfo))
              component.SetPartitionState(hostProperties.Id, DatabasePartitionState.Deleted);
          }
          catch (DatabasePartitionNotFoundException ex)
          {
            TeamFoundationTracingService.TraceExceptionRaw(57051, TraceLevel.Warning, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, (Exception) ex);
          }
        }
      }
      catch (DatabasePartitionNotFoundException ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(57051, TraceLevel.Warning, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, (Exception) ex);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(57051, TraceLevel.Warning, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, ex);
        if (!deploymentContext.ExecutionEnvironment.IsHostedDeployment)
          return;
        throw;
      }
    }

    private static void SetStorageAccountIdInPartitionDb(
      TeamFoundationServiceHostProperties hostProperties,
      ITeamFoundationDatabaseProperties dbProperties,
      DatabasePartition partition)
    {
      if (partition == null)
        return;
      try
      {
        RegistryHelpers.SetValueRaw<int>(dbProperties.SqlConnectionInfo, partition.PartitionId, FrameworkServerConstants.DeletedServiceHostStorageAccount, hostProperties.StorageAccountId, "Host Deletion");
      }
      catch (RegistryUninitializedException ex)
      {
        TeamFoundationTracingService.TraceRaw(57053, TraceLevel.Error, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, "Error while deleting a partiton and attempting to set the storage account id into the partition registry.  The host may not be property formed, or may be re-attempting deletion.  Deletion will continue. Exeception Message: {0}.", (object) ex);
      }
    }

    internal List<TeamFoundationServiceHostProcess> QueryServiceHostProcesses(
      IVssRequestContext requestContext,
      Guid machineId)
    {
      TeamFoundationTracingService.TraceEnterRaw(57057, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, nameof (QueryServiceHostProcesses), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      try
      {
        using (HostManagementComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<HostManagementComponent>())
          return component.QueryServiceHostProcesses(machineId).GetCurrent<TeamFoundationServiceHostProcess>().Items;
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(57058, TraceLevel.Error, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, ex);
        throw;
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(57059, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, "QueryServiceHostProcess");
      }
    }

    public HostProperties QueryServiceHostPropertiesCached(
      IVssRequestContext requestContext,
      Guid hostId)
    {
      return this.m_kernel.GetServiceHostProperties(hostId, this.RequestId);
    }

    public IEnumerable<HostProperties> QueryChildrenServiceHostPropertiesCached(
      IVssRequestContext requestContext,
      Guid parentHostId)
    {
      return (IEnumerable<HostProperties>) this.m_kernel.GetServiceHostChildrenProperties(parentHostId, this.RequestId);
    }

    public IList<HostProperties> QueryChildrenServiceHostProperties(
      IVssRequestContext requestContext,
      IList<Guid> parentHostIds)
    {
      requestContext.CheckDeploymentRequestContext();
      if (parentHostIds.IsNullOrEmpty<Guid>())
        return (IList<HostProperties>) new List<HostProperties>();
      if (parentHostIds.Count > 1000)
        throw new ArgumentException(string.Format("The maximum batch size allowed is: {0}, the input list size is: {1}.", (object) 1000, (object) parentHostIds.Count));
      using (HostManagementComponent component = requestContext.CreateComponent<HostManagementComponent>())
        return component.QueryChildrenServiceHostProperties(parentHostIds);
    }

    public IEnumerable<HostProperties> QueryServiceHostProperties(
      IVssRequestContext requestContext,
      DateTime? lastUserAccessFrom,
      DateTime? lastUserAccessTo)
    {
      using (HostManagementComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<HostManagementComponent>())
        return (IEnumerable<HostProperties>) component.QueryServiceHosts(lastUserAccessFrom, lastUserAccessTo).GetCurrent<HostProperties>().Items;
    }

    public List<HostProperties> QueryServiceHostProperties(
      IVssRequestContext requestContext,
      int databaseId,
      int maxResults)
    {
      using (HostManagementComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<HostManagementComponent>())
        return component.QueryServiceHosts(databaseId, maxResults).GetCurrent<HostProperties>().Items;
    }

    public ICollection<HostProperties> QueryServiceHostProperties(
      IVssRequestContext requestContext,
      int databaseId,
      int batchSize,
      Guid? minHostId)
    {
      ArgumentUtility.CheckForOutOfRange(batchSize, nameof (batchSize), 1, 1024);
      using (HostManagementComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<HostManagementComponent>())
        return (ICollection<HostProperties>) component.QueryServiceHosts(databaseId, batchSize, minHostId).GetCurrent<HostProperties>().Items;
    }

    public IEnumerable<HostProperties> QueryServiceHostProperties(
      IVssRequestContext requestContext,
      TeamFoundationHostType hostType,
      Guid? minHostId,
      int? batchSize)
    {
      using (HostManagementComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<HostManagementComponent>())
        return (IEnumerable<HostProperties>) component.QueryServiceHosts(hostType, minHostId, batchSize).GetCurrent<HostProperties>().Items;
    }

    public int GetHostsCountByType(
      IVssRequestContext requestContext,
      TeamFoundationHostType hostType)
    {
      using (HostManagementComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<HostManagementComponent>())
        return component.GetHostsCountByType(hostType);
    }

    public TeamFoundationServiceHostProperties QueryServiceHostProperties(
      IVssRequestContext requestContext,
      Guid hostId)
    {
      return this.QueryServiceHostProperties(requestContext, hostId, ServiceHostFilterFlags.None);
    }

    public IList<HostProperties> QueryServiceHostPropertiesBatch(
      IVssRequestContext requestContext,
      ICollection<Guid> hostIds)
    {
      requestContext.CheckDeploymentRequestContext();
      if (hostIds.IsNullOrEmpty<Guid>())
        return (IList<HostProperties>) new List<HostProperties>();
      if (hostIds.Count > 1000)
        throw new ArgumentOutOfRangeException(string.Format("The maximum batch size allowed is: {0}, the input list size is: {1}.", (object) 1000, (object) hostIds.Count));
      using (HostManagementComponent component = requestContext.CreateComponent<HostManagementComponent>())
        return component.QueryServiceHostPropertiesBatch((IEnumerable<Guid>) hostIds);
    }

    public TeamFoundationServiceHostProperties QueryServiceHostProperties(
      IVssRequestContext requestContext,
      Guid hostId,
      ServiceHostFilterFlags filterFlags)
    {
      TeamFoundationTracingService.TraceEnterRaw(57060, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, nameof (QueryServiceHostProperties), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      try
      {
        ArgumentUtility.CheckForEmptyGuid(hostId, nameof (hostId));
        IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment);
        filterFlags &= ~ServiceHostFilterFlags.IncludeProcessDetails;
        using (HostManagementComponent component = context.CreateComponent<HostManagementComponent>())
        {
          ResultCollection resultCollection = component.QueryServiceHostProperties(hostId, filterFlags);
          TeamFoundationServiceHostProperties serviceHostProperties1 = (TeamFoundationServiceHostProperties) null;
          List<TeamFoundationServiceHostProperties> items1 = resultCollection.GetCurrent<TeamFoundationServiceHostProperties>().Items;
          if (items1.Count != 0)
          {
            serviceHostProperties1 = items1[0];
            serviceHostProperties1.Children = new List<TeamFoundationServiceHostProperties>();
            if (items1.Count > 1)
            {
              Dictionary<Guid, TeamFoundationServiceHostProperties> dictionary = new Dictionary<Guid, TeamFoundationServiceHostProperties>();
              dictionary[serviceHostProperties1.Id] = serviceHostProperties1;
              for (int index = 1; index < items1.Count; ++index)
              {
                TeamFoundationServiceHostProperties serviceHostProperties2 = dictionary[items1[index].ParentId];
                if (serviceHostProperties2 != null)
                {
                  if (serviceHostProperties2.Children == null)
                    serviceHostProperties2.Children = new List<TeamFoundationServiceHostProperties>();
                  serviceHostProperties2.Children.Add(items1[index]);
                }
                dictionary[items1[index].Id] = items1[index];
              }
            }
          }
          if ((filterFlags & ServiceHostFilterFlags.IncludeAllServicingDetails) != ServiceHostFilterFlags.None)
          {
            resultCollection.NextResult();
            List<TeamFoundationServiceHostProperties> items2 = resultCollection.GetCurrent<TeamFoundationServiceHostProperties>().Items;
            if (items2.Count > 0)
            {
              if (serviceHostProperties1 != null)
              {
                if (serviceHostProperties1.Children == null)
                  serviceHostProperties1.Children = items2;
                else
                  serviceHostProperties1.Children.AddRange((IEnumerable<TeamFoundationServiceHostProperties>) items2);
              }
              else
              {
                serviceHostProperties1 = items2[0];
                if (serviceHostProperties1.Children == null)
                  serviceHostProperties1.Children = new List<TeamFoundationServiceHostProperties>();
              }
            }
          }
          return serviceHostProperties1;
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(57068, TraceLevel.Error, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, ex);
        throw;
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(57069, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, nameof (QueryServiceHostProperties));
      }
    }

    internal List<ServicingHostProperties> QueryServiceHostPropertiesByServiceLevel(
      IVssRequestContext requestContext,
      string targetServiceLevel,
      TeamFoundationHostType hostType)
    {
      List<ServicingHostProperties> list;
      using (HostManagementComponent component = requestContext.CreateComponent<HostManagementComponent>())
        list = component.QueryServiceHostPropertiesByServiceLevel(targetServiceLevel, hostType).GetCurrent<ServicingHostProperties>().ToList<ServicingHostProperties>();
      ServicingHostProperties.SortByAccessTimeDesc(list);
      return list;
    }

    internal void BulkUpdateHostServiceLevel(
      IVssRequestContext requestContext,
      int databaseId,
      TeamFoundationHostType hostType,
      string currentServiceLevel,
      string newServiceLevel)
    {
      using (HostManagementComponent component = requestContext.CreateComponent<HostManagementComponent>())
        component.BulkUpdateHostServiceLevel(databaseId, hostType, currentServiceLevel, newServiceLevel);
    }

    public TeamFoundationExecutionState QueryExecutionState(IVssRequestContext requestContext) => this.QueryExecutionState(requestContext, Guid.Empty);

    public TeamFoundationExecutionState QueryExecutionState(
      IVssRequestContext requestContext,
      Guid hostId)
    {
      TeamFoundationTracingService.TraceEnterRaw(57070, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, nameof (QueryExecutionState), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      try
      {
        using (HostManagementComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<HostManagementComponent>())
        {
          ResultCollection resultCollection = component.QueryServiceHostProperties(hostId, ServiceHostFilterFlags.IncludeProcessDetails);
          List<TeamFoundationServiceHostProperties> items1 = resultCollection.GetCurrent<TeamFoundationServiceHostProperties>().Items;
          resultCollection.NextResult();
          List<TeamFoundationServiceHostProcess> items2 = resultCollection.GetCurrent<TeamFoundationServiceHostProcess>().Items;
          resultCollection.NextResult();
          List<TeamFoundationServiceHostInstance> items3 = resultCollection.GetCurrent<TeamFoundationServiceHostInstance>().Items;
          Dictionary<Guid, TeamFoundationServiceHostProperties> dictionary1 = new Dictionary<Guid, TeamFoundationServiceHostProperties>();
          Dictionary<Guid, TeamFoundationServiceHostProcess> dictionary2 = new Dictionary<Guid, TeamFoundationServiceHostProcess>();
          foreach (TeamFoundationServiceHostProperties serviceHostProperties in items1)
          {
            TeamFoundationTracingService.TraceRaw(57075, TraceLevel.Verbose, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, "Adding {0} to hostProperties Map {1}", (object) serviceHostProperties.Name, (object) serviceHostProperties.Id);
            dictionary1.Add(serviceHostProperties.Id, serviceHostProperties);
          }
          foreach (TeamFoundationServiceHostProcess serviceHostProcess in items2)
          {
            TeamFoundationTracingService.TraceRaw(57076, TraceLevel.Verbose, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, "Adding {0} to hostProcesses {1}", (object) serviceHostProcess.ProcessName, (object) serviceHostProcess.ProcessId);
            dictionary2.Add(serviceHostProcess.ProcessId, serviceHostProcess);
          }
          foreach (TeamFoundationServiceHostInstance serviceHostInstance in items3)
          {
            TeamFoundationServiceHostProperties serviceHostProperties;
            if (dictionary1.TryGetValue(serviceHostInstance.HostId, out serviceHostProperties))
              serviceHostInstance.HostProperties = serviceHostProperties;
            TeamFoundationServiceHostProcess serviceHostProcess;
            if (dictionary2.TryGetValue(serviceHostInstance.ProcessId, out serviceHostProcess))
              serviceHostInstance.Process = serviceHostProcess;
          }
          return new TeamFoundationExecutionState(items1, items2, items3);
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(57078, TraceLevel.Error, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, ex);
        throw;
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(57079, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, nameof (QueryExecutionState));
      }
    }

    public TeamFoundationHostReadyState QueryHostReadyState(
      IVssRequestContext requestContext,
      TeamFoundationServiceHostProperties hostProperties)
    {
      TeamFoundationTracingService.TraceEnterRaw(57080, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, nameof (QueryHostReadyState), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      try
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckForNull<TeamFoundationServiceHostProperties>(hostProperties, nameof (hostProperties));
        return this.GetHostStateValidationDriver(requestContext).QueryHostReadyState(requestContext, hostProperties);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(57088, TraceLevel.Error, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, ex);
        throw;
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(57089, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, nameof (QueryHostReadyState));
      }
    }

    public List<ServiceHostHistoryEntry> QueryServiceHostHistory(
      IVssRequestContext requestContext,
      int watermark)
    {
      using (HostManagementComponent component = requestContext.CreateComponent<HostManagementComponent>())
        return component.QueryServiceHostHistory(watermark)?.GetCurrent<ServiceHostHistoryEntry>().Items;
    }

    public bool PingHostProcess(
      IVssRequestContext requestContext,
      Guid processId,
      TimeSpan pingTimeout)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      vssRequestContext.TraceEnter(57090, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, nameof (PingHostProcess));
      try
      {
        return vssRequestContext.ServiceHost.ServiceHostInternal().FlushNotificationQueue(vssRequestContext, processId, pingTimeout);
      }
      finally
      {
        vssRequestContext.TraceLeave(57099, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, nameof (PingHostProcess));
      }
    }

    public DateTime GetConfigDataTierTime(IVssRequestContext requestContext)
    {
      using (HostManagementComponent component = requestContext.CreateComponent<HostManagementComponent>())
        return component.GetConfigDataTierTime();
    }

    public void StartHost(
      IVssRequestContext requestContext,
      Guid hostId,
      ServiceHostSubStatus subStatus = ServiceHostSubStatus.None)
    {
      TeamFoundationTracingService.TraceEnterRaw(57100, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, nameof (StartHost), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      try
      {
        ArgumentUtility.CheckForEmptyGuid(hostId, nameof (hostId));
        IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
        TeamFoundationHostReadyState foundationHostReadyState = this.QueryHostReadyState(requestContext1, this.QueryServiceHostProperties(requestContext1, hostId) ?? throw new HostDoesNotExistException(hostId));
        if (!foundationHostReadyState.IsReady)
          throw new HostStatusChangeException(foundationHostReadyState.Message, hostId);
        ((IInternalTeamFoundationHostManagementService) this).StartHostInternal(requestContext1, hostId, subStatus);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(57108, TraceLevel.Error, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, ex);
        throw;
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(57109, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, nameof (StartHost));
      }
    }

    void IInternalTeamFoundationHostManagementService.StartHostInternal(
      IVssRequestContext requestContext,
      Guid hostId,
      ServiceHostSubStatus subStatus,
      bool reenableJobs)
    {
      TeamFoundationTracingService.TraceEnterRaw(57110, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, "StartHostInternal", new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        using (HostManagementComponent component = vssRequestContext.CreateComponent<HostManagementComponent>())
          component.StartServiceHost(hostId, new ServiceHostSubStatus?(subStatus));
        if (!reenableJobs)
          return;
        this.ReenableJobs(vssRequestContext, (ICollection<Guid>) new Guid[1]
        {
          hostId
        });
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(57111, TraceLevel.Error, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, ex);
        throw;
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(57112, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, "StartHostInternal");
      }
    }

    internal void StartRootHost(
      DeploymentServiceHost rootServiceHost,
      VssRequestContext systemRequestContext,
      ISqlConnectionInfo rootConnectionInfo,
      bool initialStartup)
    {
      TeamFoundationTracingService.TraceEnterRaw(57310, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, nameof (StartRootHost), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      try
      {
        if (rootConnectionInfo != null && !string.IsNullOrEmpty(rootConnectionInfo.ConnectionString))
        {
          TeamFoundationTracingService.TraceRaw(57311, TraceLevel.Info, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, "Registering Service Host Instance");
          using (HostManagementComponent componentRaw = rootConnectionInfo.CreateComponentRaw<HostManagementComponent>())
          {
            TeamFoundationServiceHostProperties serviceHostProperties = componentRaw.QueryServiceHostProperties(rootServiceHost.InstanceId, ServiceHostFilterFlags.None).GetCurrent<TeamFoundationServiceHostProperties>().Items.FirstOrDefault<TeamFoundationServiceHostProperties>();
            if (serviceHostProperties != null)
            {
              if (serviceHostProperties.DatabaseId != 0)
              {
                TeamFoundationTracingService.TraceRaw(57313, TraceLevel.Warning, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, "Resetting DatabaseId to {0}", (object) serviceHostProperties.DatabaseId);
                rootServiceHost.ServiceHostProperties.DatabaseId = serviceHostProperties.DatabaseId;
              }
              if (serviceHostProperties.StorageAccountId != -1)
                rootServiceHost.ServiceHostProperties.StorageAccountId = serviceHostProperties.StorageAccountId;
              rootServiceHost.ServiceHostProperties.HostType = TeamFoundationHostType.Application | TeamFoundationHostType.Deployment;
            }
          }
          systemRequestContext.GetService<SqlRegistryService>();
          TeamFoundationTaskService service1 = systemRequestContext.GetService<TeamFoundationTaskService>();
          ITeamFoundationSqlNotificationService service2 = systemRequestContext.GetService<ITeamFoundationSqlNotificationService>();
          systemRequestContext.GetService<TeamFoundationTracingService>();
          service2.RegisterNotification((IVssRequestContext) systemRequestContext, "Default", SqlNotificationEventClasses.RegistrySettingsChanged, new SqlNotificationHandler(this.NotificationCallback), false);
          this.SetMachineIdFromEnvironment(systemRequestContext.RequestTracer);
          DateTime expiry = this.RegisterServiceHostInstance((IVssRequestContext) systemRequestContext, (VssServiceHost) rootServiceHost);
          rootServiceHost.ServiceHostInternal().SetRegistered();
          systemRequestContext.Trace(57319, TraceLevel.Verbose, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, "Adding Lease Renewal Timer");
          this.m_masterCancellationTokenSource = new CancellationTokenSource();
          this.m_leaseRenewalTask = TimeoutTaskHelper.DoWork(new Func<CancellationToken, Task<DateTime>>(this.LeaseRenewalTask), expiry, TimeSpan.FromMinutes(1.0), (Func<Exception, bool>) (e => e is HostProcessNotFoundException), this.m_masterCancellationTokenSource.Token);
          service2.RegisterNotification((IVssRequestContext) systemRequestContext, "Default", SqlNotificationEventClasses.ServiceHostCreated, new SqlNotificationHandler(this.NotificationCallback), false);
          service2.RegisterNotification((IVssRequestContext) systemRequestContext, "Default", SqlNotificationEventClasses.ServiceHostDeleted, new SqlNotificationHandler(this.NotificationCallback), false);
          service2.RegisterNotification((IVssRequestContext) systemRequestContext, "Default", SqlNotificationEventClasses.ServiceHostModified, new SqlNotificationHandler(this.NotificationCallback), false);
          service2.RegisterNotification((IVssRequestContext) systemRequestContext, "Default", SqlNotificationEventClasses.ServiceHostLevelModified, new SqlNotificationHandler(this.NotificationCallback), false);
          service2.RegisterNotification((IVssRequestContext) systemRequestContext, "Default", SqlNotificationEventClasses.ServiceHostStatusChanged, new SqlNotificationHandler(this.NotificationCallback), false);
          service2.RegisterNotification((IVssRequestContext) systemRequestContext, "Default", SqlNotificationEventClasses.IsHostLoaded, new SqlNotificationHandler(this.IsHostLoadedCallback), true);
          service2.RegisterNotification((IVssRequestContext) systemRequestContext, "Default", SqlNotificationEventClasses.ClearAllSqlConnectionPools, new SqlNotificationHandler(this.NotificationCallback), false);
          service2.RegisterNotification((IVssRequestContext) systemRequestContext, "Default", SqlNotificationEventClasses.ForceGCRequested, new SqlNotificationHandler(this.NotificationCallback), false);
          RegistryQuery query1 = new RegistryQuery(TeamFoundationHostManagementService.s_configurationPath);
          RegistryEntryCollection registryEntries = new RegistryEntryCollection(query1.Path, RegistryHelpers.DeploymentReadRaw(systemRequestContext.FrameworkConnectionInfo, query1).Select<RegistryItem, RegistryEntry>((Func<RegistryItem, RegistryEntry>) (s => new RegistryEntry(s.Path, s.Value))));
          this.LoadSettings(registryEntries);
          systemRequestContext.Trace(57315, TraceLevel.Verbose, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, "Adding UpdateRecentlyAccessedHosts Task");
          VssRequestContext requestContext = systemRequestContext;
          TeamFoundationTask task = new TeamFoundationTask(new TeamFoundationTaskCallback(this.UpdateLastUserAccess), (object) null, this.m_updateRecentlyAccessedHostsSeconds * 1000);
          service1.AddTask((IVssRequestContext) requestContext, task);
          systemRequestContext.Trace(57317, TraceLevel.Verbose, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, "Adding Service Host Request Monitoring Task");
          int valueFromPath = registryEntries.GetValueFromPath<int>(FrameworkServerConstants.HostDormancyCheckInterval, 60000);
          this.m_forceLastUserAccessUpdate = registryEntries.GetValueFromPath<bool>(FrameworkServerConstants.ForceUpdateLastUserAccess, false);
          Command.CommandCacheLimit = RegistryHelpers.GetDeploymentValueRaw<int>(systemRequestContext.FrameworkConnectionInfo, TeamFoundationHostManagementService.s_commandCacheLimitPath, 33554432);
          RegistryQuery query2 = new RegistryQuery(TeamFoundationHostManagementService.s_xmlParameterChunkSettings + "/*");
          RegistryEntryCollection registryEntryCollection = new RegistryEntryCollection(query2.Path, RegistryHelpers.DeploymentReadRaw(systemRequestContext.FrameworkConnectionInfo, query2).Select<RegistryItem, RegistryEntry>((Func<RegistryItem, RegistryEntry>) (s => new RegistryEntry(s.Path, s.Value))));
          DbPagingManagerSettings.XmlParameterChunkThresholdSetting = registryEntryCollection.GetValueFromPath<int>(TeamFoundationHostManagementService.s_xmlParameterChunkThreshold, DbPagingManagerSettings.DefaultXmlParameterChunkThresholdSetting);
          DbPagingManagerSettings.XmlParameterChunkThresholdSettingInBytes = registryEntryCollection.GetValueFromPath<int>(TeamFoundationHostManagementService.s_xmlParameterChunkThresholdInBytes, DbPagingManagerSettings.DefaultXmlParameterChunkThresholdSettingInBytes);
          systemRequestContext.Trace(57319, TraceLevel.Verbose, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, "Adding Lease Renewal Task");
          this.m_dormancyTimer = new Timer(new TimerCallback(this.CheckForDormantHosts), (object) null, valueFromPath, valueFromPath);
          this.EnsureMachineExists((IVssRequestContext) systemRequestContext);
        }
        else
        {
          rootServiceHost.ServiceHostInternal().Initialize(DateTime.UtcNow, TeamFoundationServiceHostStatus.Starting);
          Command.CommandCacheLimit = 33554432;
          DbPagingManagerSettings.XmlParameterChunkThresholdSetting = DbPagingManagerSettings.DefaultXmlParameterChunkThresholdSetting;
          DbPagingManagerSettings.XmlParameterChunkThresholdSettingInBytes = DbPagingManagerSettings.DefaultXmlParameterChunkThresholdSettingInBytes;
        }
        TeamFoundationTracingService.TraceRawAlwaysOn(57321, TraceLevel.Info, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, "Starting Deployment Host Process {0} on Machine {1}", (object) this.m_processId, (object) this.m_machineId);
        List<HostProperties> serviceHostProperties1 = (List<HostProperties>) null;
        if (this.IsHosted || rootConnectionInfo == null || string.IsNullOrEmpty(rootConnectionInfo.ConnectionString))
        {
          serviceHostProperties1 = new List<HostProperties>(1)
          {
            rootServiceHost.ServiceHostProperties
          };
        }
        else
        {
          using (HostManagementComponent componentRaw = rootConnectionInfo.CreateComponentRaw<HostManagementComponent>())
            serviceHostProperties1 = componentRaw.QueryServiceHosts().GetCurrent<HostProperties>().Items;
        }
        this.m_kernel.Start(serviceHostProperties1, new Func<Guid, List<HostProperties>>(this.ResolveHost), this.RequestId);
        if (this.m_leaseRenewalTask == null)
          return;
        if (this.m_leaseRenewalTask.IsFaulted)
          this.m_leaseRenewalTask.Wait();
        else
          this.m_leaseRenewalTask.ContinueWith(new Action<Task>(this.NotifyLeaseExpired), TaskContinuationOptions.OnlyOnFaulted);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(57329, TraceLevel.Error, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, ex);
        throw;
      }
    }

    private void SetMachineIdFromEnvironment(ITraceRequest requestContext)
    {
      if (!(this.m_machineId == Guid.Empty))
        return;
      Guid result;
      if (Guid.TryParse(Environment.GetEnvironmentVariable("AzureDevOpsMachineId", EnvironmentVariableTarget.Machine), out result))
      {
        requestContext.Trace(57601, TraceLevel.Info, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, string.Format("Setting machine id from environment variable to {0}", (object) result));
        this.m_machineId = result;
      }
      else
        requestContext.TraceAlways(57602, TraceLevel.Info, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, "Unable to retrieve machine id from environment variable");
    }

    public void Stop(IVssRequestContext systemRequestContext)
    {
      using (ManualResetEvent notifyObject = new ManualResetEvent(false))
      {
        if (this.m_dormancyTimer != null)
        {
          if (this.m_dormancyTimer.Dispose((WaitHandle) notifyObject))
            notifyObject.WaitOne();
          this.m_dormancyTimer = (Timer) null;
        }
      }
      if (this.m_kernel == null)
        return;
      this.m_kernel.Stop(this.RequestId);
    }

    private void NotificationCallback(IVssRequestContext requestContext, NotificationEventArgs args)
    {
      TeamFoundationTracingService.TraceEnterRaw(57420, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, nameof (NotificationCallback), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      if (!TeamFoundationHostManagementService.ts_IsNotificationThread)
        TeamFoundationHostManagementService.ts_IsNotificationThread = true;
      try
      {
        if (args.Class == SqlNotificationEventClasses.RegistrySettingsChanged)
        {
          IEnumerable<RegistryItem> entries = SqlRegistryService.DeserializeSqlNotification(requestContext, args.Data);
          if (((IEnumerable<RegistryQuery>) TeamFoundationHostManagementService.s_notificationFilters).SelectMany<RegistryQuery, RegistryItem>((Func<RegistryQuery, IEnumerable<RegistryItem>>) (s => entries.Filter(s))).Any<RegistryItem>())
            this.LoadSettings(requestContext.GetService<SqlRegistryService>().ReadEntries(requestContext, (RegistryQuery) TeamFoundationHostManagementService.s_configurationPath));
        }
        else if (args.Class == SqlNotificationEventClasses.ServiceHostCreated || args.Class == SqlNotificationEventClasses.ServiceHostDeleted || args.Class == SqlNotificationEventClasses.ServiceHostModified || args.Class == SqlNotificationEventClasses.ServiceHostStatusChanged)
        {
          foreach (TeamFoundationServiceHostProperties serviceHostProperties in args.Deserialize<TeamFoundationServiceHostProperties[]>())
          {
            if (args.Class == SqlNotificationEventClasses.ServiceHostCreated)
              this.m_kernel.OnServiceHostCreated((HostProperties) serviceHostProperties, this.RequestId);
            else if (args.Class == SqlNotificationEventClasses.ServiceHostDeleted)
              this.m_kernel.OnServiceHostDeleted((HostProperties) serviceHostProperties, this.RequestId);
            else if (args.Class == SqlNotificationEventClasses.ServiceHostModified)
              this.m_kernel.OnServiceHostModified((HostProperties) serviceHostProperties, this.RequestId);
            else if (args.Class == SqlNotificationEventClasses.ServiceHostStatusChanged)
            {
              bool flag = this.m_kernel.OnServiceHostStatusChanged((HostProperties) serviceHostProperties, this.RequestId, TimeSpan.FromMinutes(5.0));
              if (serviceHostProperties.Status == TeamFoundationServiceHostStatus.Stopped)
                requestContext.GetService<ITeamFoundationSqlNotificationService>().SendNotification(requestContext, SqlNotificationEventClasses.ServiceHostStatusChangedResponse, TeamFoundationSerializationUtility.SerializeToString<ServiceHostStatusChangedResponse>(new ServiceHostStatusChangedResponse()
                {
                  HostId = serviceHostProperties.Id,
                  ProcessId = this.m_processId,
                  TimeProcessed = DateTime.UtcNow,
                  Status = flag ? TeamFoundationServiceHostStatus.Stopped : TeamFoundationServiceHostStatus.Stopping
                }));
            }
          }
        }
        else if (args.Class == SqlNotificationEventClasses.ServiceHostLevelModified)
        {
          ServiceHostLevelModifiedEvent hostLevelModifiedEvent = args.Deserialize<ServiceHostLevelModifiedEvent>();
          this.m_kernel.OnServiceHostPropertiesChanged((Action<HostProperties>) (hostProperties =>
          {
            if (hostProperties.DatabaseId != hostLevelModifiedEvent.DatabaseId || hostProperties.HostType != hostLevelModifiedEvent.HostType || !(hostProperties.ServiceLevel == hostLevelModifiedEvent.CurrentServiceLevel) && !string.IsNullOrEmpty(hostLevelModifiedEvent.CurrentServiceLevel))
              return;
            hostProperties.ServiceLevel = hostLevelModifiedEvent.NewServiceLevel;
          }), this.RequestId);
        }
        else if (args.Class == SqlNotificationEventClasses.ClearAllSqlConnectionPools)
        {
          SqlConnection.ClearAllPools();
        }
        else
        {
          if (!(args.Class == SqlNotificationEventClasses.ForceGCRequested))
            throw new InvalidOperationException();
          this.OnForceGCRequested(requestContext);
        }
      }
      catch (Exception ex)
      {
        TeamFoundationEventLog.Default.LogException(requestContext, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Notification Processing Exception:\r\nEvent Class - {0}\r\nEvent Data - {1}", (object) args.Class, (object) SecretUtility.ScrubSecrets(args.Data, false)), ex);
        requestContext.TraceException(57423, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, ex);
        throw;
      }
      TeamFoundationTracingService.TraceLeaveRaw(57422, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, nameof (NotificationCallback));
    }

    private void IsHostLoadedCallback(IVssRequestContext requestContext, NotificationEventArgs args)
    {
      requestContext.TraceEnter(96417942, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, nameof (IsHostLoadedCallback));
      try
      {
        IsHostLoadedRequest hostLoadedRequest = args.Deserialize<IsHostLoadedRequest>();
        if (!(hostLoadedRequest.ProcessId == this.ProcessId))
          return;
        using (IVssRequestContext vssRequestContext = this.BeginRequest(requestContext, hostLoadedRequest.HostId, RequestContextType.SystemContext, false, false))
        {
          bool objectToSerialize = false;
          if (vssRequestContext != null)
            objectToSerialize = true;
          requestContext.Trace(7978489, TraceLevel.Info, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, "Sending IsHostLoadedResponse {0}, {1}, {2}", (object) hostLoadedRequest.RequestId, (object) hostLoadedRequest.HostId, (object) objectToSerialize);
          requestContext.GetService<ITeamFoundationSqlNotificationService>().SendNotification(requestContext, hostLoadedRequest.RequestId, TeamFoundationSerializationUtility.SerializeToString<bool>(objectToSerialize));
        }
      }
      finally
      {
        requestContext.TraceLeave(86675601, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, nameof (IsHostLoadedCallback));
      }
    }

    public void DetectInactiveProcesses(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(57115, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, nameof (DetectInactiveProcesses));
      try
      {
        using (HostManagementComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<HostManagementComponent>())
          component.DetectInactiveProcesses();
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(57118, TraceLevel.Error, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, ex);
        throw;
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(57119, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, nameof (DetectInactiveProcesses));
      }
    }

    public bool StopHost(
      IVssRequestContext requestContext,
      Guid hostId,
      ServiceHostSubStatus subStatusToSet,
      string reason,
      TimeSpan timeout)
    {
      requestContext.TraceEnter(57120, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, nameof (StopHost));
      this.DetectAndUnregisterProcesses(requestContext);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      using (HostStatusChangeCoordinator coordinator = new HostStatusChangeCoordinator(requestContext, hostId, TeamFoundationHostManagementService.\u003C\u003EO.\u003C0\u003E__GetProcessesWithValidLeases ?? (TeamFoundationHostManagementService.\u003C\u003EO.\u003C0\u003E__GetProcessesWithValidLeases = new Func<IVssRequestContext, List<TeamFoundationServiceHostProcess>>(GetProcessesWithValidLeases))))
      {
        SqlNotificationCallback callback = (SqlNotificationCallback) ((request, eventClass, eventData) =>
        {
          ServiceHostStatusChangedResponse response = TeamFoundationSerializationUtility.Deserialize<ServiceHostStatusChangedResponse>(eventData);
          coordinator.ProcessMessage(request, response);
        });
        ITeamFoundationSqlNotificationService service = requestContext.GetService<ITeamFoundationSqlNotificationService>();
        try
        {
          service.RegisterNotification(requestContext, "Default", SqlNotificationEventClasses.ServiceHostStatusChangedResponse, callback, false);
          using (HostManagementComponent component = requestContext.CreateComponent<HostManagementComponent>())
            component.StopServiceHost(hostId, reason, subStatusToSet);
          return timeout > TimeSpan.Zero && coordinator.Wait(requestContext, timeout);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(57128, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, ex);
          throw;
        }
        finally
        {
          service?.UnregisterNotification(requestContext, "Default", SqlNotificationEventClasses.ServiceHostStatusChangedResponse, callback, false);
          requestContext.TraceLeave(57129, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, nameof (StopHost));
        }
      }

      static List<TeamFoundationServiceHostProcess> GetProcessesWithValidLeases(
        IVssRequestContext requestContext)
      {
        using (HostManagementComponent component = requestContext.CreateComponent<HostManagementComponent>())
          return component.QueryServiceHostProcesses(Guid.Empty).GetCurrent<TeamFoundationServiceHostProcess>().Items;
      }
    }

    internal void EnsureMachineExists(IVssRequestContext requestContext)
    {
      using (IDisposableReadOnlyList<IEnsureMachineExistsExtension> extensionsRaw = VssExtensionManagementService.GetExtensionsRaw<IEnsureMachineExistsExtension>(requestContext.ServiceHost.PlugInDirectory))
      {
        if (extensionsRaw.Count <= 0)
          return;
        extensionsRaw.First<IEnsureMachineExistsExtension>().EnsureMachineExists(requestContext, this.m_processId, TeamFoundationHostManagementService.s_machineName, ref this.m_machineId);
      }
    }

    internal bool DetectAndUnregisterProcesses(IVssRequestContext requestContext)
    {
      bool flag = false;
      TeamFoundationHostManagementService service = requestContext.GetService<TeamFoundationHostManagementService>();
      foreach (TeamFoundationServiceHostProcess serviceHostProcess in service.QueryServiceHostProcesses(requestContext.Elevate(), Guid.Empty))
      {
        if (this.IsInactive(requestContext, service, serviceHostProcess))
        {
          requestContext.Trace(1112, TraceLevel.Error, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, "Process {0} ID:{1} disappeared without unregistering!", (object) serviceHostProcess.ProcessName, (object) serviceHostProcess.OSProcessId);
          service.UnregisterProcess(requestContext.Elevate(), serviceHostProcess.ProcessId);
          requestContext.Trace(1113, TraceLevel.Info, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, "Process {0} ID:{1} successfully unregistered", (object) serviceHostProcess.ProcessName, (object) serviceHostProcess.OSProcessId);
          flag = true;
        }
      }
      return flag;
    }

    internal void UnregisterProcess(IVssRequestContext requestContext, Guid processId)
    {
      TeamFoundationTracingService.TraceEnterRaw(57170, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, nameof (UnregisterProcess), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      try
      {
        using (HostManagementComponent component = requestContext.CreateComponent<HostManagementComponent>())
          component.UnregisterServiceHostInstance(processId, Guid.Empty);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(57174, TraceLevel.Error, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, ex);
        throw;
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(57175, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, nameof (UnregisterProcess));
      }
    }

    private bool IsInactive(
      IVssRequestContext requestContext,
      TeamFoundationHostManagementService hostService,
      TeamFoundationServiceHostProcess hostProcess)
    {
      if (hostProcess.MachineId != Guid.Empty && hostService.MachineId != Guid.Empty)
      {
        if (hostProcess.MachineId != hostService.MachineId && string.Equals(hostProcess.MachineName, Environment.MachineName, StringComparison.OrdinalIgnoreCase))
        {
          requestContext.Trace(1115, TraceLevel.Error, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, string.Format("Skipping Process {0} because the machine Id don't match ({1},{2}) but the machine names do: ({3},{4})!!!", (object) hostProcess.ProcessId, (object) hostProcess.MachineId, (object) hostService.MachineId, (object) hostProcess.MachineName, (object) Environment.MachineName));
          return false;
        }
        if (hostProcess.MachineId == hostService.MachineId && !string.Equals(hostProcess.MachineName, Environment.MachineName, StringComparison.OrdinalIgnoreCase))
        {
          requestContext.Trace(1116, TraceLevel.Error, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, string.Format("Skipping Process {0} because the machine names don't match ({1},{2}) but the machine ids do: {3}!!!", (object) hostProcess.ProcessId, (object) hostProcess.MachineName, (object) Environment.MachineName, (object) hostService.MachineId));
          return false;
        }
        if (hostProcess.MachineId != hostService.MachineId)
        {
          requestContext.Trace(1117, TraceLevel.Info, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, string.Format("Skipping Process {0} because it is not local to this machine", (object) hostProcess.ProcessId));
          return false;
        }
      }
      else if (!string.Equals(hostProcess.MachineName, Environment.MachineName, StringComparison.OrdinalIgnoreCase))
      {
        requestContext.Trace(1118, TraceLevel.Info, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, string.Format("Skipping Process {0} because it is not local to this machine", (object) hostProcess.ProcessId));
        return false;
      }
      try
      {
        Process processById = Process.GetProcessById(hostProcess.OSProcessId);
        if (hostProcess.ProcessName.StartsWith(processById.ProcessName, StringComparison.OrdinalIgnoreCase))
        {
          requestContext.Trace(1111, TraceLevel.Verbose, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, string.Format("Process {0} ID:{1} still exists", (object) hostProcess.ProcessName, (object) hostProcess.OSProcessId));
          return false;
        }
      }
      catch (ArgumentException ex)
      {
      }
      return true;
    }

    private TeamFoundationHostStateValidationDriver GetHostStateValidationDriver(
      IVssRequestContext requestContext)
    {
      TeamFoundationTracingService.TraceEnterRaw(57180, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, nameof (GetHostStateValidationDriver), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      try
      {
        TeamFoundationHostStateValidationDriver validationDriver = this.m_hostStateValidationDriver;
        if (validationDriver == null)
        {
          lock (this.m_hostStateValidationDriverCreateLock)
          {
            if (this.m_hostStateValidationDriver == null)
              this.m_hostStateValidationDriver = new TeamFoundationHostStateValidationDriver(requestContext.ServiceHost.PlugInDirectory);
            validationDriver = this.m_hostStateValidationDriver;
          }
        }
        return validationDriver;
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(57184, TraceLevel.Error, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, ex);
        throw;
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(57185, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, nameof (GetHostStateValidationDriver));
      }
    }

    private List<HostProperties> ResolveHost(Guid hostId)
    {
      if (this.m_deploymentServiceHost.InstanceId == hostId)
        return new List<HostProperties>(1)
        {
          this.m_deploymentServiceHost.ServiceHostProperties
        };
      using (IVssRequestContext systemContext = this.m_deploymentServiceHost.CreateSystemContext(false))
      {
        using (HostManagementComponent component = systemContext.CreateComponent<HostManagementComponent>())
          return component.ResolveHost(hostId);
      }
    }

    internal DateTime RegisterServiceHostInstance(
      IVssRequestContext requestContext,
      VssServiceHost serviceHost)
    {
      TeamFoundationTracingService.TraceEnterRaw(57220, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, nameof (RegisterServiceHostInstance), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      if (this.m_processId == Guid.Empty)
        TeamFoundationTracingService.TraceRaw(57221, TraceLevel.Error, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, "ProcessId is ZERO!");
      try
      {
        if (!requestContext.ServiceHost.HasDatabaseAccess)
          return DateTime.MaxValue;
        DateTime leaseExpiry;
        using (HostManagementComponent componentRaw = requestContext.FrameworkConnectionInfo.CreateComponentRaw<HostManagementComponent>())
        {
          DateTime startTime;
          TeamFoundationServiceHostStatus initialStatus = componentRaw.RegisterServiceHostInstance(serviceHost.InstanceId, this.m_machineId, TeamFoundationHostManagementService.s_machineName, this.m_processId, TeamFoundationHostManagementService.s_processName, Process.GetCurrentProcess().Id, UserNameUtil.CurrentUserName, out startTime, out leaseExpiry);
          serviceHost.ServiceHostInternal().Initialize(startTime, initialStatus);
        }
        return leaseExpiry;
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(57224, TraceLevel.Error, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, ex);
        throw;
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(57225, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, nameof (RegisterServiceHostInstance));
      }
    }

    internal void UnregisterServiceHostInstance(
      IVssRequestContext requestContext,
      IVssServiceHost serviceHost)
    {
      TeamFoundationTracingService.TraceEnterRaw(57230, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, nameof (UnregisterServiceHostInstance), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      try
      {
        if (!requestContext.ServiceHost.HasDatabaseAccess)
          return;
        try
        {
          using (HostManagementComponent componentRaw = requestContext.FrameworkConnectionInfo.CreateComponentRaw<HostManagementComponent>())
            componentRaw.UnregisterServiceHostInstance(this.m_processId, serviceHost.InstanceId);
        }
        catch (TeamFoundationServiceException ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(57234, TraceLevel.Error, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, (Exception) ex);
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(57235, TraceLevel.Error, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, ex);
        throw;
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(57236, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, nameof (UnregisterServiceHostInstance));
      }
    }

    internal bool SetHostStatus(
      IVssRequestContext requestContext,
      Guid hostId,
      TeamFoundationServiceHostStatus newStatus)
    {
      TeamFoundationTracingService.TraceEnterRaw(57290, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, nameof (SetHostStatus), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      try
      {
        if (!requestContext.ServiceHost.HasDatabaseAccess)
          return true;
        using (HostManagementComponent componentRaw = requestContext.FrameworkConnectionInfo.CreateComponentRaw<HostManagementComponent>(60))
        {
          bool flag = componentRaw.UpdateServiceHostInstance(hostId, this.m_processId, newStatus);
          TeamFoundationTracingService.TraceRaw(57297, TraceLevel.Info, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, "HMC returned {0} for UpdateServiceHostInstance: {1} Process: {2} to status {3}", (object) flag, (object) hostId, (object) this.m_processId, (object) newStatus);
          return flag;
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(57298, TraceLevel.Error, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, ex);
        throw;
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(57299, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, nameof (SetHostStatus));
      }
    }

    public IVssRequestContext BeginUserRequest(
      IVssRequestContext requestContext,
      Guid instanceId,
      IdentityDescriptor userContext,
      bool throwIfShutdown = false)
    {
      bool flag = false;
      IVssRequestContext vssRequestContext = (IVssRequestContext) null;
      try
      {
        vssRequestContext = this.BeginRequest(requestContext, instanceId, RequestContextType.UserContext, true, throwIfShutdown);
        this.SetupUserContext(vssRequestContext, userContext, true);
        requestContext.LinkCancellation(vssRequestContext);
        flag = true;
      }
      finally
      {
        if (!flag && vssRequestContext != null)
          vssRequestContext.Dispose();
      }
      return vssRequestContext;
    }

    public IVssRequestContext BeginUserRequest(
      IVssRequestContext requestContext,
      Guid instanceId,
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity,
      bool throwIfShutdown = false)
    {
      bool flag = false;
      IVssRequestContext vssRequestContext = (IVssRequestContext) null;
      try
      {
        vssRequestContext = this.BeginRequest(requestContext, instanceId, RequestContextType.UserContext, true, throwIfShutdown);
        this.SetupUserContext(vssRequestContext, userIdentity);
        requestContext.LinkCancellation(vssRequestContext);
        flag = true;
      }
      finally
      {
        if (!flag && vssRequestContext != null)
          vssRequestContext.Dispose();
      }
      return vssRequestContext;
    }

    void IInternalTeamFoundationHostManagementService.SetupUserContext(
      IVssRequestContext userRequestContext,
      IdentityDescriptor userContext)
    {
      this.SetupUserContext(userRequestContext, userContext, true);
    }

    internal void SetupUserContext(
      IVssRequestContext userRequestContext,
      IdentityDescriptor userContext,
      bool setupOrgUserRole = true)
    {
      IdentityService service = userRequestContext.GetService<IdentityService>();
      Microsoft.VisualStudio.Services.Identity.Identity identity1 = service.ReadIdentities(userRequestContext, (IList<IdentityDescriptor>) new List<IdentityDescriptor>()
      {
        userContext
      }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (identity1 == null)
      {
        if (userRequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        {
          Microsoft.VisualStudio.Services.Identity.Identity identity2 = service.ReadIdentitiesFromSource(userRequestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
          {
            userContext
          }, QueryMembership.None).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
          if (identity2 != null)
            throw new IdentityNotFoundInCollectionException(identity2.DisplayName);
        }
        throw new IdentityNotFoundException(userContext);
      }
      this.SetupUserContextFromIdentity(userRequestContext, identity1, setupOrgUserRole);
    }

    private void SetupUserContext(
      IVssRequestContext userRequestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      bool setupOrgUserRole = true)
    {
      IVssRequestContext vssRequestContext = userRequestContext.Elevate();
      IdentityService service = vssRequestContext.GetService<IdentityService>();
      if (identity.Id == Guid.Empty)
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity1 = service.ReadIdentities(vssRequestContext, (IList<IdentityDescriptor>) new List<IdentityDescriptor>()
        {
          identity.Descriptor
        }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
        if (identity1 != null)
          identity = identity1;
      }
      this.SetupUserContextFromIdentity(userRequestContext, identity, setupOrgUserRole);
    }

    private void SetupUserContextFromIdentity(
      IVssRequestContext userRequestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      bool setupOrgUserRole = true)
    {
      IRequestActor actor = !setupOrgUserRole || !IdentityHelper.IsOrgUser(userRequestContext, identity) ? RequestActor.CreateRequestActor(userRequestContext, identity.Descriptor, identity.Id) : RequestActor.CreateRequestActorWithMultipleIdentitySubjects(userRequestContext, identity.Descriptor, identity.Id, OrgAccessConstants.OrgUserSubject.ToDescriptor());
      string domainUserName = IdentityHelper.GetDomainUserName(identity);
      this.SetupUserContext(userRequestContext, actor.ToList(), domainUserName, domainUserName);
      userRequestContext.SetUserIdentityTracingItems(identity);
      string key = identity.Descriptor.ToString();
      if (userRequestContext.Items.ContainsKey(key))
        return;
      userRequestContext.Items.Add(key, (object) identity);
    }

    void IInternalTeamFoundationHostManagementService.SetupUserContext(
      IVssRequestContext userRequestContext,
      IReadOnlyList<IRequestActor> actors,
      string authenticatedUserName,
      string domainUserName)
    {
      this.SetupUserContext(userRequestContext, actors, authenticatedUserName, domainUserName);
    }

    internal void SetupUserContext(
      IVssRequestContext userRequestContext,
      IReadOnlyList<IRequestActor> actors,
      string authenticatedUserName = null,
      string domainUserName = null)
    {
      userRequestContext.RequestContextInternal().ClearActors();
      ((VssRequestContext) userRequestContext).ClearUserContextCache();
      IRequestContextInternal requestContextInternal = userRequestContext.RequestContextInternal();
      requestContextInternal.Actors = actors;
      if (authenticatedUserName != null)
        requestContextInternal.SetAuthenticatedUserName(authenticatedUserName);
      if (domainUserName != null)
        requestContextInternal.SetDomainUserName(domainUserName);
      TeamFoundationApplicationCore.ApplyLicensePrincipals(userRequestContext);
    }

    public IVssRequestContext BeginRequest(
      IVssRequestContext requestContext,
      Guid instanceId,
      RequestContextType contextType,
      bool loadIfNecessary = true,
      bool throwIfShutdown = true)
    {
      return (IVssRequestContext) this.BeginRequest(requestContext, instanceId, contextType, loadIfNecessary, throwIfShutdown, (IReadOnlyList<IRequestActor>) null, HostRequestType.Default, Array.Empty<object>());
    }

    internal IVssRequestContext BeginImpersonatedRequest(
      IVssRequestContext requestContext,
      Guid instanceId,
      RequestContextType contextType,
      IReadOnlyList<IRequestActor> actors,
      bool loadIfNecessary = true,
      bool throwIfShutdown = true)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) actors, nameof (actors));
      return (IVssRequestContext) this.BeginRequest(requestContext, instanceId, contextType, loadIfNecessary, throwIfShutdown, actors, HostRequestType.Default, Array.Empty<object>());
    }

    IVssRequestContext IInternalTeamFoundationHostManagementService.BeginRequest(
      IVssRequestContext requestContext,
      Guid instanceId,
      RequestContextType contextType,
      bool loadIfNecessary,
      bool throwIfShutdown,
      IReadOnlyList<IRequestActor> actors,
      HostRequestType type,
      params object[] additionalParameters)
    {
      return (IVssRequestContext) this.BeginRequest(requestContext, instanceId, contextType, loadIfNecessary, throwIfShutdown, actors, type, additionalParameters);
    }

    internal VssRequestContext BeginRequest(
      IVssRequestContext requestContext,
      Guid instanceId,
      RequestContextType contextType,
      bool loadIfNecessary,
      bool throwIfShutdown,
      IReadOnlyList<IRequestActor> actors,
      HostRequestType type,
      params object[] additionalParameters)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      VssRequestContext request = this.m_kernel.CreateRequest(instanceId, this.RequestId, contextType, type, loadIfNecessary, throwIfShutdown, !TeamFoundationHostManagementService.ts_IsNotificationThread, additionalParameters);
      if (request != null)
      {
        try
        {
          this.SetupAdditionalRequestContextItems(requestContext, request);
          if (request.RequestContextInternal().IsRootContext)
          {
            if (actors != null && actors.Count > 0)
            {
              if (type != HostRequestType.Default)
                throw new InvalidOperationException("Impersonation scenario: If actors are set, HostRequestType must be HostRequestType.Default.");
              this.SetupUserContext((IVssRequestContext) request, actors, (string) null, (string) null);
            }
            else if (contextType == RequestContextType.UserContext && (type == HostRequestType.Default || type == HostRequestType.GenericHttp || type == HostRequestType.Ssh))
            {
              IRequestContextInternal requestContextInternal = requestContext.RequestContextInternal();
              if (requestContextInternal.Actors != null && requestContextInternal.Actors.Count > 0)
                this.SetupUserContext((IVssRequestContext) request, requestContextInternal.Actors, requestContext.AuthenticatedUserName, requestContext.DomainUserName);
            }
          }
          else if (actors != null)
            throw new InvalidOperationException("Cannot set actors on non-root context");
          return request;
        }
        catch (Exception ex)
        {
          request?.Dispose();
          requestContext.TraceException(0, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, ex);
          throw;
        }
      }
      else
      {
        requestContext.Trace(57760, TraceLevel.Info, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, "Not Starting a new request for host {0} because it is dormant and loadIfNecessary is false", (object) instanceId);
        return (VssRequestContext) null;
      }
    }

    private void SetupAdditionalRequestContextItems(
      IVssRequestContext requestContext,
      VssRequestContext targetRequestContext)
    {
      if (!requestContext.Items.ContainsKey("RequestContextItemKeyForStakeholderLicenseCheckBypass"))
        return;
      targetRequestContext.Items["RequestContextItemKeyForStakeholderLicenseCheckBypass"] = requestContext.Items["RequestContextItemKeyForStakeholderLicenseCheckBypass"];
    }

    DateTime IInternalTeamFoundationHostManagementService.UpdateServiceHostLastUserAccess(
      IVssRequestContext requestContext,
      IEnumerable<Guid> hostsToUpdate)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) hostsToUpdate, nameof (hostsToUpdate));
      using (HostManagementComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<HostManagementComponent>())
        return component.UpdateServiceHostLastUserAccess(hostsToUpdate);
    }

    internal void AssertNoLocksHeld(string message)
    {
      if (this.m_kernel == null)
        return;
      this.m_kernel.AssertNoLocksHeld(this.RequestId, message);
    }

    private void LogHostUpdateException(Exception exception, DeploymentServiceHost host)
    {
      TeamFoundationTracingService.TraceEnterRaw(57360, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, nameof (LogHostUpdateException), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      string str1 = (string) null;
      string str2 = (string) null;
      if (host != null)
      {
        str1 = host.GetType().Name;
        str2 = host.InstanceId.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture);
      }
      using (IVssRequestContext systemContext = host.CreateSystemContext(false))
        TeamFoundationEventLog.Default.LogException(systemContext, FrameworkResources.ServiceHostUpdateError((object) str2, (object) str1), exception, TeamFoundationEventId.HostStatusChangeError, EventLogEntryType.Error);
    }

    private void LoadSettings(RegistryEntryCollection registryEntries)
    {
      this.HostDormancySeconds = Math.Max(0, registryEntries.GetValueFromPath<int>(FrameworkServerConstants.HostDormancyInterval, TeamFoundationHostManagementService.s_defaultDormancyInterval)) * 60;
      this.m_updateRecentlyAccessedHostsSeconds = Math.Min(Math.Max(60, this.HostDormancySeconds - 60), 300);
      this.m_dormancyThreads = Math.Max(1, registryEntries.GetValueFromPath<int>(FrameworkServerConstants.HostDormancyThreads, 4));
      this.m_timeToSleep = TimeSpan.FromMinutes((double) Math.Max(1, registryEntries.GetValueFromPath<int>(FrameworkServerConstants.HostTimeToSleep, 5)));
      int valueFromPath = registryEntries.GetValueFromPath<int>(FrameworkServerConstants.HostTimeToSleep + "/" + Path.GetFileNameWithoutExtension(TeamFoundationHostManagementService.s_processName), 0);
      if (valueFromPath > 0)
        this.m_timeToSleep = TimeSpan.FromMinutes((double) valueFromPath);
      this.m_maxHosts = registryEntries.GetValueFromPath<int>(FrameworkServerConstants.MaxTotalHosts, 7680);
    }

    private void OnForceGCRequested(IVssRequestContext requestContext)
    {
      long totalMemory1 = GC.GetTotalMemory(false);
      GC.Collect();
      GC.WaitForPendingFinalizers();
      long totalMemory2 = GC.GetTotalMemory(true);
      TeamFoundationTracingService.TraceRaw(0, TraceLevel.Verbose, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, "OnForceGCRequested-Force GC Mem Before={0} After={1} Delta={2}", (object) totalMemory1, (object) totalMemory2, (object) (totalMemory1 - totalMemory2));
    }

    internal void DisposeTimers()
    {
      if (this.m_masterCancellationTokenSource != null && !this.m_masterCancellationTokenSource.IsCancellationRequested)
        this.m_masterCancellationTokenSource.Cancel();
      if (this.m_leaseRenewalTask != null)
      {
        this.m_leaseRenewalTask.Wait();
        this.m_leaseRenewalTask = (Task) null;
      }
      if (this.m_masterCancellationTokenSource == null)
        return;
      this.m_masterCancellationTokenSource.Dispose();
      this.m_masterCancellationTokenSource = (CancellationTokenSource) null;
    }

    internal event EventHandler LeaseRenewed;

    internal event EventHandler ShutdownRequested;

    private Task<DateTime> LeaseRenewalTask(CancellationToken ct)
    {
      TeamFoundationTracingService.TraceEnterRaw(57440, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, nameof (LeaseRenewalTask), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      using (IVssRequestContext systemContext = this.m_deploymentServiceHost.CreateSystemContext(false))
      {
        DateTime utcNow = DateTime.UtcNow;
        TimeSpan timeSpan = Debugger.IsAttached ? TimeSpan.FromMinutes(20.0) : this.m_leaseDuration;
        using (HostManagementComponent hmc = systemContext.FrameworkConnectionInfo.CreateComponentRaw<HostManagementComponent>(30))
        {
          using (ct.Register((Action) (() => hmc.Cancel())))
          {
            try
            {
              hmc.RenewLease(this.m_processId, (int) timeSpan.TotalMinutes);
              this.m_leaseExpiry = utcNow.AddMinutes((double) (int) timeSpan.TotalMinutes);
              TeamFoundationTracingService.TraceRaw(57442, TraceLevel.Info, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, "Lease Expiry is now {0}", (object) this.m_leaseExpiry);
              EventHandler leaseRenewed = this.LeaseRenewed;
              if (leaseRenewed != null)
                leaseRenewed((object) this, (EventArgs) null);
              return Task.FromResult<DateTime>(this.m_leaseExpiry);
            }
            catch (HostProcessNotFoundException ex)
            {
              TeamFoundationTracingService.TraceExceptionRaw(57444, TraceLevel.Error, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, (Exception) ex);
              TeamFoundationTracingService.TraceRaw(57446, TraceLevel.Error, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, "Lease lost for process id {0}", (object) this.m_processId);
              throw;
            }
            catch (TeamFoundationServiceException ex)
            {
              TeamFoundationTracingService.TraceExceptionRaw(57448, TraceLevel.Error, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, (Exception) ex);
              throw;
            }
          }
        }
      }
    }

    private void NotifyLeaseExpired(object state)
    {
      TeamFoundationTracingService.TraceEnterRaw(57450, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, nameof (NotifyLeaseExpired), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      TeamFoundationTracingService.TraceRaw(57451, TraceLevel.Error, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, "NotifyLeaseExpired was triggered for processID {0} at {1}", (object) this.m_processId, (object) DateTime.UtcNow);
      try
      {
        ISqlConnectionInfo sqlConnectionInfo = this.m_deploymentServiceHost.DatabaseProperties.SqlConnectionInfo;
        TeamFoundationEventLog.Default.Log(FrameworkResources.SQLInstanceUnreachable((object) sqlConnectionInfo.InitialCatalog, (object) sqlConnectionInfo.DataSource), TeamFoundationEventId.ApplicationLeaseExpired, EventLogEntryType.Warning);
        TeamFoundationTracingService.TraceRaw(57452, TraceLevel.Error, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, "Stopping ProcessID {0} at {1} because lease expired", (object) this.m_processId, (object) DateTime.UtcNow);
        lock (this)
          this.m_selfDestructionTimer = new Timer((TimerCallback) (s =>
          {
            TeamFoundationTracingService.TraceRawAlwaysOn(58065, TraceLevel.Error, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, "ExitingProcess now");
            Environment.Exit(258);
          }), (object) null, TimeSpan.FromSeconds(30.0), TimeSpan.Zero);
        try
        {
          EventHandler shutdownRequested = this.ShutdownRequested;
          if (shutdownRequested != null)
            shutdownRequested((object) this, (EventArgs) null);
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(57454, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, ex);
        }
        TeamFoundationTracingService.TraceRaw(57453, TraceLevel.Error, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, "Stopped ProcessID {0} at {1}", (object) this.m_processId, (object) DateTime.UtcNow);
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(57459, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, nameof (NotifyLeaseExpired));
      }
    }

    private void CheckForDormantHosts(object taskArg)
    {
      TeamFoundationTracingService.TraceEnterRaw(57470, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, nameof (CheckForDormantHosts), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      if (!Monitor.TryEnter(this.m_dormancyLock, 0))
        return;
      try
      {
        this.m_kernel.CheckForDormantHosts(this.m_timeToSleep, this.m_dormancyThreads);
        HostManagementTraceHelper.TraceThreadPool(true);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(57478, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, ex);
      }
      finally
      {
        Monitor.Exit(this.m_dormancyLock);
        TeamFoundationTracingService.TraceLeaveRaw(57479, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, nameof (CheckForDormantHosts));
      }
    }

    internal bool CheckRequestId(long requestId) => requestId == 0L || requestId == this.RequestId;

    internal void SetRequestIdForNotificationThread()
    {
      int cHashBuckets = this.m_kernel.c_hashBuckets;
      LockHelperContext.SetRequestId(((long) int.MinValue - (long) Math.Abs(Environment.CurrentManagedThreadId)) * (long) cHashBuckets);
    }

    internal long RequestId
    {
      get
      {
        if (LockHelperContext.RequestIdIsSet)
          return LockHelperContext.RequestId;
        HttpContext current = HttpContext.Current;
        if (current != null)
        {
          long? nullable = current.Items[(object) nameof (RequestId)] as long?;
          if (nullable.HasValue)
            return nullable.Value;
        }
        return (long) -Math.Abs(Environment.CurrentManagedThreadId);
      }
    }

    internal void SetLastAccessTime(IVssRequestContext requestContext) => this.m_kernel.SetLastAccessTime(this.RequestId, requestContext.ServiceHost.InstanceId);

    private void UpdateLastUserAccess(IVssRequestContext requestContext, object taskArgs)
    {
      List<Guid> hostsToUpdate = (List<Guid>) null;
      TeamFoundationTracingService.TraceEnterRaw(57510, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, nameof (UpdateLastUserAccess), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      try
      {
        hostsToUpdate = new List<Guid>();
        try
        {
          this.m_recentUserAccessHostsLock.AcquireWriterLock(-1);
          hostsToUpdate.AddRange((IEnumerable<Guid>) this.m_recentUserAccessHosts);
          this.m_recentUserAccessHosts.Clear();
        }
        finally
        {
          if (this.m_recentUserAccessHostsLock.IsWriterLockHeld)
            this.m_recentUserAccessHostsLock.ReleaseWriterLock();
        }
        if (hostsToUpdate.Count <= 0)
          return;
        DateTime configDataTierTime;
        using (HostManagementComponent component = requestContext.CreateComponent<HostManagementComponent>())
          configDataTierTime = component.GetConfigDataTierTime();
        DateTime dateTime1 = configDataTierTime.AddSeconds((double) (-1 * this.HostDormancySeconds));
        List<Guid> hosts = new List<Guid>();
        foreach (Guid hostId in hostsToUpdate)
        {
          HostProperties hostProperties = this.QueryServiceHostPropertiesCached(requestContext, hostId);
          if (hostProperties != null)
          {
            if (hostProperties.LastUserAccess < dateTime1)
              hosts.Add(hostId);
          }
          else
          {
            requestContext.Trace(57516, TraceLevel.Warning, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, "ServiceHostProperties not found for HostId {0}", (object) hostId);
            hosts.Add(hostId);
          }
        }
        this.ReenableJobs(requestContext, (ICollection<Guid>) hosts);
        DateTime dateTime2;
        using (HostManagementComponent component = requestContext.CreateComponent<HostManagementComponent>())
          dateTime2 = component.UpdateServiceHostLastUserAccess((IEnumerable<Guid>) hostsToUpdate);
        foreach (Guid hostId in hostsToUpdate)
        {
          HostProperties hostProperties = this.QueryServiceHostPropertiesCached(requestContext, hostId);
          if (hostProperties != null)
            hostProperties.LastUserAccess = dateTime2;
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(57514, TraceLevel.Error, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, ex);
        if (hostsToUpdate != null)
        {
          if (hostsToUpdate.Count > 0)
          {
            try
            {
              this.m_recentUserAccessHostsLock.AcquireWriterLock(-1);
              foreach (Guid guid in hostsToUpdate)
                this.m_recentUserAccessHosts.Add(guid);
            }
            finally
            {
              if (this.m_recentUserAccessHostsLock.IsWriterLockHeld)
                this.m_recentUserAccessHostsLock.ReleaseWriterLock();
            }
          }
        }
        throw;
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(57515, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, nameof (UpdateLastUserAccess));
      }
    }

    internal bool LastUserAccessUpdateScheduled(IVssRequestContext requestContext, Guid hostId)
    {
      try
      {
        this.m_recentUserAccessHostsLock.AcquireReaderLock(-1);
        return this.m_recentUserAccessHosts.Contains(hostId);
      }
      finally
      {
        if (this.m_recentUserAccessHostsLock.IsReaderLockHeld || this.m_recentUserAccessHostsLock.IsWriterLockHeld)
          this.m_recentUserAccessHostsLock.ReleaseReaderLock();
      }
    }

    internal void ScheduleLastUserAccessUpdate(IVssRequestContext requestContext, Guid hostId)
    {
      try
      {
        TeamFoundationTracingService.TraceEnterRaw(57530, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, nameof (ScheduleLastUserAccessUpdate), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
        try
        {
          this.m_recentUserAccessHostsLock.AcquireWriterLock(-1);
          if (this.m_recentUserAccessHosts.Add(hostId))
            TeamFoundationTracingService.TraceRaw(57531, TraceLevel.Info, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, "{0} added to recentUserAccessHosts", (object) hostId);
          else
            TeamFoundationTracingService.TraceRaw(57533, TraceLevel.Info, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, "{0} was already in recentUserAccessHosts", (object) hostId);
        }
        finally
        {
          if (this.m_recentUserAccessHostsLock.IsWriterLockHeld)
            this.m_recentUserAccessHostsLock.ReleaseWriterLock();
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(57538, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, ex);
        throw;
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(57539, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, nameof (ScheduleLastUserAccessUpdate));
      }
    }

    internal Dictionary<Guid, IVssRequestContext[]> GetActiveRequests(
      IVssRequestContext requestContext,
      Guid[] hosts)
    {
      requestContext.TraceEnter(57540, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, nameof (GetActiveRequests));
      try
      {
        Dictionary<Guid, IVssRequestContext[]> activeRequestsByHost = new Dictionary<Guid, IVssRequestContext[]>();
        this.m_kernel.RunOnEachHost((HostManagementKernel<VssServiceHost, VssRequestContext>.HostCallback) ((request, state) => activeRequestsByHost.Add(request.ServiceHost.InstanceId, request.ServiceHost.ServiceHostInternal().GetActiveRequests())), (object) null, (IEnumerable<Guid>) hosts, this.RequestId);
        return activeRequestsByHost;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(57544, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(57545, TeamFoundationHostManagementService.s_Area, TeamFoundationHostManagementService.s_Layer, nameof (GetActiveRequests));
      }
    }

    private void EnsureExtensionsLoaded(IVssRequestContext requestContext)
    {
      if (this.m_extensions != null)
        return;
      IDisposableReadOnlyList<IHostManagementExtension> extensions = requestContext.GetExtensions<IHostManagementExtension>();
      foreach (IHostManagementExtension managementExtension in (IEnumerable<IHostManagementExtension>) extensions)
        managementExtension.Initialize(requestContext);
      if (Interlocked.CompareExchange<IDisposableReadOnlyList<IHostManagementExtension>>(ref this.m_extensions, extensions, (IDisposableReadOnlyList<IHostManagementExtension>) null) == null)
        return;
      extensions.Dispose();
    }

    private void ReenableJobs(IVssRequestContext requestContext, ICollection<Guid> hosts) => requestContext.GetService<TeamFoundationJobService>().ReenableJobs(requestContext, hosts);

    internal Guid ProcessId => this.m_processId;

    Guid IInternalTeamFoundationHostManagementService.ProcessId => this.ProcessId;

    internal Guid MachineId => this.m_machineId;
  }
}
