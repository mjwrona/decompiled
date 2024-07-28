// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTrackingConfigurationSettingService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessValidator;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class WorkItemTrackingConfigurationSettingService : CacheServiceBase
  {
    public const long DefaultMaxAttachmentSize = 62914560;
    public const DataImportMode DefaultDataImportMode = DataImportMode.InheritedAndXML;
    private bool m_fullTextEnabled;
    private CultureInfo m_serverCulture;
    private StringComparer m_serverStringComparer;
    private ILockName m_lockName;

    protected override IEnumerable<MetadataTable> MetadataTables => Enumerable.Empty<MetadataTable>();

    private WorkItemTrackingConfigurationSettingService.DatabaseProperties GetDatabaseProperties(
      IVssRequestContext requestContext)
    {
      return requestContext.TraceBlock<WorkItemTrackingConfigurationSettingService.DatabaseProperties>(908513, 908514, 908515, "Services", nameof (WorkItemTrackingConfigurationSettingService), "WitSettings.GetDatabaseProperties", (Func<WorkItemTrackingConfigurationSettingService.DatabaseProperties>) (() =>
      {
        using (WorkItemTrackingMetadataComponent component = requestContext.CreateComponent<WorkItemTrackingMetadataComponent>())
          return new WorkItemTrackingConfigurationSettingService.DatabaseProperties()
          {
            IsFullTextEnabled = component.InstallWorkItemWordsContains(),
            Collation = component.GetCollationInfo()
          };
      }));
    }

    protected override void ServiceStart(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(900279, "Services", nameof (WorkItemTrackingConfigurationSettingService), nameof (ServiceStart));
      this.m_lockName = requestContext.ServiceHost.CreateUniqueLockName(this.GetType().Name);
      base.ServiceStart(requestContext);
      WorkItemTrackingConfigurationSettingService.DatabaseProperties databaseProperties = requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) ? this.GetDatabaseProperties(requestContext) : throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      this.m_fullTextEnabled = databaseProperties.IsFullTextEnabled;
      this.m_serverCulture = new CultureInfo(databaseProperties.Collation.LocaleId);
      this.m_serverStringComparer = StringComparer.Create(this.m_serverCulture, true);
      this.RegisterNotification(requestContext);
      requestContext.TraceLeave(900280, "Services", nameof (WorkItemTrackingConfigurationSettingService), nameof (ServiceStart));
    }

    protected override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      this.UnregisterNotification(systemRequestContext);
      base.ServiceEnd(systemRequestContext);
    }

    private void RegisterNotification(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnWitSettingsChanged), true, "/Service/WorkItemTracking/Settings/**");

    private void UnregisterNotification(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnWitSettingsChanged));

    private void OnWitSettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.InvalidateCache(requestContext);
    }

    public virtual IWorkItemTrackingConfigurationInfo GetConfigurationInfo(
      IVssRequestContext requestContext)
    {
      // ISSUE: reference to a compiler-generated method
      return requestContext.TraceBlock<IWorkItemTrackingConfigurationInfo>(908504, 908505, 908506, "Services", nameof (WorkItemTrackingConfigurationSettingService), "WitSettings.GetConfigurationInfo", (Func<IWorkItemTrackingConfigurationInfo>) (() => (IWorkItemTrackingConfigurationInfo) this.\u003C\u003En__0<WorkItemTrackingConfigurationSettingService.WorkItemTrackingConfigurationInfo>(requestContext, false)));
    }

    protected override CacheSnapshotBase CreateSnapshot(
      IVssRequestContext requestContext,
      CacheSnapshotBase existingSnapshot)
    {
      return (CacheSnapshotBase) requestContext.TraceBlock<WorkItemTrackingConfigurationSettingService.WorkItemTrackingConfigurationInfo>(908501, 908502, 908503, "Services", nameof (WorkItemTrackingConfigurationSettingService), "WitSettings.CreateSnapshot", (Func<WorkItemTrackingConfigurationSettingService.WorkItemTrackingConfigurationInfo>) (() =>
      {
        IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
        return WorkItemTrackingConfigurationSettingService.WorkItemTrackingConfigurationInfo.Create(requestContext, service.ReadEntriesFallThru(requestContext, (RegistryQuery) "/Service/WorkItemTracking/Settings/**"), this.m_fullTextEnabled, this.m_serverCulture, this.m_serverStringComparer, service.ReadEntriesFallThru(requestContext, (RegistryQuery) "/Service/WorkItemTracking/Settings/ReadReplicaUsers/..."), service.ReadEntriesFallThru(requestContext, (RegistryQuery) "/Service/WorkItemTracking/Settings/ReadReplicaUserAgents/..."), service.ReadEntriesFallThru(requestContext, (RegistryQuery) "/Service/WorkItemTracking/Settings/ReadReplicaEnabledCommands/..."), service.ReadEntriesFallThru(requestContext, (RegistryQuery) "/Service/WorkItemTracking/Settings/ForcedReadReplicaCommands/..."), service.ReadEntriesFallThru(requestContext, (RegistryQuery) "/Service/WorkItemTracking/Settings/QueryOptimizations/..."), service.ReadEntriesFallThru(requestContext, (RegistryQuery) "/Service/WorkItemTracking/Settings/QueryExecutionLogging/..."), new HashSet<string>(service.ReadEntriesFallThru(requestContext, (RegistryQuery) "/Service/WorkItemTracking/Settings/EmptyText/*").Select<RegistryEntry, string>((Func<RegistryEntry, string>) (e => e.Value)), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase));
      }));
    }

    protected virtual void SetRegistryValue<T>(
      IVssRequestContext requestContext,
      string path,
      T value)
    {
      requestContext.TraceBlock(908504, 908508, 908509, "Services", nameof (WorkItemTrackingConfigurationSettingService), "WitSettings.SetRegistryValue", (Action) (() =>
      {
        using (requestContext.AcquireWriterLock(this.m_lockName))
        {
          requestContext.Elevate().GetService<CachedRegistryService>().SetValue<T>(requestContext.Elevate(), path, value);
          this.InvalidateCache(requestContext);
        }
      }));
    }

    public virtual void SetInProcBuildCompletionNotificationAvailability(
      IVssRequestContext requestContext,
      bool isEnabled)
    {
      this.SetRegistryValue<bool>(requestContext, "/Service/WorkItemTracking/Settings/IsProcBuildCompletionNotificationEnabled", isEnabled);
    }

    public virtual void SetMaxAttachmentSize(IVssRequestContext requestContext, long maxSize) => this.SetRegistryValue<long>(requestContext, "/Service/WorkItemTracking/Settings/MaxAttachmentSize", maxSize);

    public virtual void SetMaxBuildListSize(IVssRequestContext requestContext, int maxBuildListSize) => this.SetRegistryValue<int>(requestContext, "/Service/WorkItemTracking/Settings/MaxBuildListSize", maxBuildListSize);

    public virtual void SetWorkItemQueryTimeout(
      IVssRequestContext requestContext,
      int workItemQueryTimeout)
    {
      if (workItemQueryTimeout < 0)
        throw new ArgumentException(ServerResources.InvalidWorkItemQueryTimeout(), nameof (workItemQueryTimeout));
      this.SetRegistryValue<int>(requestContext, "/Service/WorkItemTracking/Settings/WorkItemQueryTimeout", workItemQueryTimeout);
    }

    private class WorkItemTrackingConfigurationInfo : 
      CacheSnapshotBase,
      IWorkItemTrackingConfigurationInfo
    {
      private bool m_metadataFilterEnabled;
      private const int c_defaultMaxBuildListSize = 10;
      private const int c_maxLongTextSize = 1048576;
      private const int c_maxRevisionLongTextSize = 1048576;
      private const int c_maxRevisionsSupportedByGetWorkItemHistory = 25000;
      private const int c_maxRevisionsSupportedByUpdateWorkItem = 10000;
      private const int c_defaultQueryTimeoutOnPrem = 3600;
      private const int c_defaultQueryTimeoutOnHosted = 30;
      private const int c_defaultQueryTimeoutOnHostedReadReplica = 120;
      private const int c_defaultQueryTimeoutAnonymousUser = 10;
      private const int c_defaultMaxQueriesBatchSize = 1000;
      private const int c_defaultMaxQueryItemChildren = 1000;
      private const int c_maxGetQueriesDepth = 2;
      private const int c_maxQueryResultSizeOnPrem = 2147483647;
      private const int c_maxQueryResultSizeOnHosted = 20000;
      private const int c_MaxTrendChartTimeSliceResultSize = 1000;
      private const int c_defaultMaxIdentityInGroupSize = 999;
      private const int c_defaultWorkItemReclassificationTimeout = 1;
      private const int c_defaultGetMetadataSoapTimeoutInSeconds = 600;
      private const int c_defaultWorkItemReclassificationStatusCheckInterval = 300;
      private const int c_defaultWorkItemSyncApiBatchSize = 10000;
      private const int c_defaultWorkItemMaxJsonLength = 52428800;
      private const int c_defaultWorkItemMetadataCacheMaxAgeInDays = 7;
      private const int c_defaultControlContributionInputLimit = 100;
      private const int c_defaultWorkItemLinksLimit = 1000;
      private const int c_defaultWorkItemRemoteLinksLimit = 25;
      private const int c_defaultMaxWorkItemTagLimit = 100;
      private const int c_defaultLinksCountCiThreshold = 100;
      private const int c_defaultLinkUpdateNotificationThreshold = 10;
      private const int c_defaultPromotePerProjectSleepTimeInSeconds = 60;
      private const int c_maxQueryItemResultSizeForPartialHierarchy = 20000;
      private const int c_maxQueryItemResultSizeForEntireHierarchy = 1048576;
      private const int c_defaultMaxWiqlTextLengthHosted = 32000;
      private const int c_defaultMaxWiqlTextLengtForMSProjecthHosted = 128000;
      private const int c_defaultMaxWiqlTextLengthOnPrem = 2147483647;
      private const int c_defaultMaxAllowedWorkItemAttachmentsHosted = 100;
      private const int c_defaultMaxAllowedWorkItemAttachmentsOnPrem = 2147483647;
      private const string c_webAccessServiceName = "Web Access";
      private const int c_richTextFieldSerializationLength = 2048;
      private const int c_maxSecretsScanContentLength = 5242880;
      private const int c_maxSecretsScanServiceRequestTimeoutInMilliseconds = 100;
      private const int c_defaultReclassificationSqlCommandTimeoutOverride = 0;
      private const int c_defaultQueryMaxDOPValue = -1;
      private const int c_defaultQueryMaxGrantPercentHosted = -1;
      private const int c_defaultQueryMaxGrantPercentOnPrem = -1;
      private const int c_defaultQueryInThreshold = 25;
      private const int c_defaultTopLevelOrOptimizationMaxClauseNumber = 3;
      private const int c_defaultGetIdentityChangesPageSize = 5000;
      private const int c_defaultMinimalImsSyncIntervalInSeconds = 300;
      private const int c_defaultMaxNumberOfWorkItemsToProcessInExternalMentions = 100;
      private const int c_defaultUncommittedLinkChangesLookbackWindowInSeconds = 600;
      private const int c_defaultBadgeTimeOutInMilliseconds = 5000;
      private const int c_defaultBadgeOutputCacheDurationInMinutes = 5;

      private WorkItemTrackingConfigurationInfo()
        : base((MetadataDBStamps) null)
      {
      }

      public int MaxWiqlTextLength { get; set; }

      public int MaxWiqlTextLengthForDataImport { get; set; }

      public int MaxWiqlTextLengthForMSProject { get; set; }

      public int MaxAllowedWorkItemAttachments { get; set; }

      public bool EventsEnabled { get; set; }

      public int DefaultWebAccessQueryResultSize { get; set; }

      public string[] ExcludedUserAgents { get; private set; }

      public bool FullTextEnabled { get; set; }

      public bool IsInProcBuildCompletionNotificationEnabled { get; set; }

      public long MaxAttachmentSize { get; set; }

      public long MaxAttachmentSizeForPublicUser { get; set; }

      public int MaxWorkItemTagLimit { get; set; }

      public int MaxBuildListSize { get; set; }

      public int MaxGetQueriesDepth { get; set; }

      public int MaxLongTextSize { get; set; }

      public int MaxQueriesBatchSize { get; set; }

      public int MaxQueryItemChildrenUnderParent { get; set; }

      public int MaxQueryItemResultSizeForEntireHierarchy { get; set; }

      public int MaxQueryItemResultSizeforPartialHierarchy { get; set; }

      public int MaxIdentityInGroupSize { get; set; }

      public int MaxQueryResultSize { get; set; }

      public int MaxQueryResultSizeForPublicUser { get; set; }

      public int MaxRevisionLongTextSize { get; set; }

      public int MaxRevisionsSupportedByGetWorkItemHistory { get; set; }

      public int MaxRevisionsSupportedByUpdateWorkItem { get; set; }

      public int MaxWorkItemPageSize { get; set; }

      public bool MetadataFilterEnabled
      {
        get
        {
          HttpContext current = HttpContext.Current;
          if (current == null)
            return this.m_metadataFilterEnabled;
          if (!this.m_metadataFilterEnabled)
            return false;
          if (current.ApplicationInstance != null && current.ApplicationInstance is VisualStudioServicesApplication)
          {
            VisualStudioServicesApplication applicationInstance = current.ApplicationInstance as VisualStudioServicesApplication;
            if (applicationInstance.VssRequestContext != null && !string.IsNullOrWhiteSpace(applicationInstance.VssRequestContext.ServiceName) && string.Equals(applicationInstance.VssRequestContext.ServiceName, "Web Access", StringComparison.Ordinal))
              return true;
          }
          HttpRequest request = current.Request;
          if (request.IsLocal)
            return false;
          string userAgent = request.UserAgent;
          foreach (string excludedUserAgent in this.ExcludedUserAgents)
          {
            if (!string.IsNullOrWhiteSpace(userAgent) && userAgent.IndexOf(excludedUserAgent, StringComparison.OrdinalIgnoreCase) >= 0)
              return false;
          }
          return true;
        }
      }

      public int MinClientVersionToProvision { get; set; }

      public CultureInfo ServerCulture { get; set; }

      public StringComparer ServerStringComparer { get; set; }

      public int WorkItemQueryTimeoutInSecond { get; set; }

      public int WorkItemQueryTimeoutReadReplicaInSecond { get; set; }

      public int GetWorkItemQueryTimeoutInSecond(bool readReplica) => !readReplica ? this.WorkItemQueryTimeoutInSecond : this.WorkItemQueryTimeoutReadReplicaInSecond;

      public int WorkItemQueryTimeoutAnonymousUser { get; set; }

      public int WorkItemReclassificationTimeout { get; set; }

      public int GetMetadataSoapTimeoutInSeconds { get; set; }

      public int MaxTrendChartTimeSliceResultSize { get; set; }

      public int WorkItemReclassificationStatusCheckInterval { get; set; }

      public int WorkItemSyncApiBatchSize { get; set; }

      public long WorkItemChangeWatermarkOffset { get; set; }

      public IWitProcessTemplateValidatorConfiguration ValidatorConfiguration { get; set; }

      public int WorkItemMaxJsonLength { get; set; }

      public int WorkItemMetadataCacheMaxAgeInDays { get; set; }

      public int? ReclassificationSqlCommandTimeoutOverride { get; set; }

      public int MaxQueryDOPValue { get; set; }

      public int QueryMaxGrantPercent { get; set; }

      public int QueryInThreshold { get; set; }

      public int TopLevelOrOptimizationMaxClauseNumber { get; set; }

      public DataImportMode DataImportMode { get; set; }

      public int ConfigWebLayoutVersion { get; set; }

      public int CollectionWebLayoutVersion { get; set; }

      public int WorkItemUpdateEventsAggregationTime { get; set; }

      public WorkItemTrackingReadReplicaConfiguration ReadReplicaSettings { get; set; }

      public WorkItemTrackingQueryExecutionLoggingConfiguration QueryExecutionLoggingConfig { get; set; }

      public WorkItemTrackingQueryOptimizationConfiguration QueryOptimizationSettings { get; set; }

      public int ControlContributionInputLimit { get; set; }

      public int WorkItemLinksLimit { get; set; }

      public int WorkItemRestoreLinksLimit { get; set; }

      public int WorkItemRemoteLinksLimit { get; set; }

      public int LinksCountCiThreshold { get; set; }

      public int LinkUpdateNotificationThreshold { get; set; }

      public int PromotePerProjectSleepTimeInSeconds { get; set; }

      public int RichTextFieldSerializationLength { get; set; }

      public int MaxSecretsScanContentLength { get; set; }

      public int MaxSecretsScanServiceRequestTimeoutInMilliseconds { get; set; }

      public HashSet<string> EmptyAliases { get; set; }

      public int GetIdentityChangesPageSize { get; set; }

      public int MinimalImsSyncIntervalInSeconds { get; set; }

      public int MaxNumberOfWorkItemsToProcessInExternalMentions { get; set; }

      public int UncommittedLinkChangesLookbackWindowInSeconds { get; set; }

      public int ExternalConnectionMaxReposCountLimit { get; set; }

      public int BadgeTimeOutInMilliseconds { get; private set; }

      public int BadgeBrowserCacheDurationInMinutes { get; private set; }

      internal override bool IsFresh(
        IVssRequestContext context,
        IEnumerable<MetadataTable> metadataTables)
      {
        return true;
      }

      public static WorkItemTrackingConfigurationSettingService.WorkItemTrackingConfigurationInfo Create(
        IVssRequestContext context,
        RegistryEntryCollection settings,
        bool fullTextEnabled,
        CultureInfo serverCulture,
        StringComparer serverStringComparer,
        RegistryEntryCollection readReplicaUsers = null,
        RegistryEntryCollection readReplicaUserAgents = null,
        RegistryEntryCollection readReplicaEnabledCommands = null,
        RegistryEntryCollection forcedReadReplicaCommands = null,
        RegistryEntryCollection queryOptimizations = null,
        RegistryEntryCollection queryExecutionLoggingConfig = null,
        HashSet<string> emptyAliases = null)
      {
        WorkItemTrackingConfigurationSettingService.WorkItemTrackingConfigurationInfo configurationInfo = new WorkItemTrackingConfigurationSettingService.WorkItemTrackingConfigurationInfo();
        configurationInfo.Initialize(context, settings, fullTextEnabled, serverCulture, serverStringComparer, readReplicaUsers, readReplicaUserAgents, readReplicaEnabledCommands, forcedReadReplicaCommands, queryOptimizations, queryExecutionLoggingConfig, emptyAliases);
        return configurationInfo;
      }

      private static DataImportMode ParseDataImportMode(string dataImportMode)
      {
        try
        {
          return (DataImportMode) Enum.Parse(typeof (DataImportMode), dataImportMode);
        }
        catch
        {
          return DataImportMode.InheritedAndXML;
        }
      }

      private void Initialize(
        IVssRequestContext context,
        RegistryEntryCollection settings,
        bool fullTextEnabled,
        CultureInfo serverCulture,
        StringComparer serverStringComparer,
        RegistryEntryCollection readReplicaUsers,
        RegistryEntryCollection readReplicaUserAgents,
        RegistryEntryCollection readReplicaEnabledCommands,
        RegistryEntryCollection forcedReadReplicaCommands,
        RegistryEntryCollection queryOptimizations,
        RegistryEntryCollection queryExecutionLoggingConfig,
        HashSet<string> emptyAliases)
      {
        bool valueFromPath1 = settings.GetValueFromPath<bool>("/Service/WorkItemTracking/Settings/FilterMetadata", false);
        string[] strArray = settings.GetValueFromPath<string>("/Service/WorkItemTracking/Settings/ExcludedAgents", string.Empty).Split(new char[1]
        {
          ':'
        }, StringSplitOptions.RemoveEmptyEntries);
        this.m_metadataFilterEnabled = valueFromPath1;
        this.ExcludedUserAgents = strArray;
        this.MaxIdentityInGroupSize = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/MaxIdentityInGroupSize", 999);
        if (this.MaxIdentityInGroupSize <= 0)
          this.MaxIdentityInGroupSize = 999;
        int defaultValue1 = context.ExecutionEnvironment.IsHostedDeployment ? 20000 : int.MaxValue;
        this.MaxQueryResultSize = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/MaxResultsSize", defaultValue1);
        if (this.MaxQueryResultSize <= 0)
          this.MaxQueryResultSize = defaultValue1;
        this.MaxQueryResultSizeForPublicUser = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/MaxResultsSizeForPublicUser", defaultValue1);
        if (this.MaxQueryResultSizeForPublicUser <= 0)
          this.MaxQueryResultSizeForPublicUser = defaultValue1;
        this.MaxQueryItemResultSizeforPartialHierarchy = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/MaxQueryItemResultsSizeForPartialHierarchyPath", 20000);
        if (this.MaxQueryItemResultSizeforPartialHierarchy <= 0)
          this.MaxQueryItemResultSizeforPartialHierarchy = 20000;
        this.MaxQueryItemResultSizeForEntireHierarchy = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/MaxQueryItemResultsSizeForEntireHierarchy", 1048576);
        if (this.MaxQueryItemResultSizeForEntireHierarchy <= 0)
          this.MaxQueryItemResultSizeForEntireHierarchy = 1048576;
        this.DefaultWebAccessQueryResultSize = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/DefaultWebAccessResultsSize", defaultValue1);
        if (this.DefaultWebAccessQueryResultSize <= 0 || this.DefaultWebAccessQueryResultSize > this.MaxQueryResultSize)
          this.DefaultWebAccessQueryResultSize = this.MaxQueryResultSize;
        this.MaxTrendChartTimeSliceResultSize = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/MaxTrendChartTimeSliceResultSize", 1000);
        if (this.MaxTrendChartTimeSliceResultSize <= 0)
          this.MaxTrendChartTimeSliceResultSize = 1000;
        this.EventsEnabled = settings.GetValueFromPath<bool>("/Service/WorkItemTracking/Settings/BisNotification", true);
        this.MaxLongTextSize = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/MaxLongTextSize", 1048576);
        if (this.MaxLongTextSize <= 0)
          this.MaxLongTextSize = int.MaxValue;
        this.MaxWiqlTextLength = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/MaxWiqlTextLength", context.ExecutionEnvironment.IsOnPremisesDeployment ? int.MaxValue : 32000);
        if (this.MaxWiqlTextLength < 0)
          this.MaxWiqlTextLength = int.MaxValue;
        this.MaxWiqlTextLengthForDataImport = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/MaxWiqlTextLength", 32000);
        if (this.MaxWiqlTextLengthForDataImport < 0)
          this.MaxWiqlTextLengthForDataImport = 32000;
        TeamFoundationExecutionEnvironment executionEnvironment = context.ExecutionEnvironment;
        this.MaxWiqlTextLengthForMSProject = executionEnvironment.IsOnPremisesDeployment ? int.MaxValue : settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/MaxWiqlTextForMSProjectLengthPath", 128000);
        if (this.MaxWiqlTextLengthForMSProject < 0)
          this.MaxWiqlTextLengthForMSProject = 128000;
        RegistryEntryCollection registryEntryCollection = settings;
        executionEnvironment = context.ExecutionEnvironment;
        int defaultValue2 = executionEnvironment.IsOnPremisesDeployment ? int.MaxValue : 100;
        this.MaxAllowedWorkItemAttachments = registryEntryCollection.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/MaxAllowedWorkItemAttachments", defaultValue2);
        if (this.MaxAllowedWorkItemAttachments < 0)
          this.MaxAllowedWorkItemAttachments = int.MaxValue;
        this.MaxRevisionLongTextSize = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/MaxRevisionLongTextSize", 1048576);
        if (this.MaxRevisionLongTextSize <= 0)
          this.MaxRevisionLongTextSize = int.MaxValue;
        this.MaxRevisionsSupportedByGetWorkItemHistory = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/MaxRevisionsSupportedByGetWorkItemHistory", 25000);
        if (this.MaxRevisionsSupportedByGetWorkItemHistory <= 0)
          this.MaxRevisionsSupportedByGetWorkItemHistory = 25000;
        this.MaxRevisionsSupportedByUpdateWorkItem = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/MaxRevisionsSupportedByUpdateWorkItem", 10000);
        if (this.MaxRevisionsSupportedByUpdateWorkItem <= 0)
          this.MaxRevisionsSupportedByUpdateWorkItem = 10000;
        this.MaxWorkItemPageSize = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/WorkItemPageSize", 200);
        if (this.MaxWorkItemPageSize <= 0)
          this.MaxWorkItemPageSize = 200;
        this.MaxAttachmentSize = settings.GetValueFromPath<long>("/Service/WorkItemTracking/Settings/MaxAttachmentSize", 62914560L);
        this.MaxAttachmentSizeForPublicUser = settings.GetValueFromPath<long>("/Service/WorkItemTracking/Settings/MaxAttachmentSizeForPublicUser", 62914560L);
        this.MaxWorkItemTagLimit = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/CreateWorkItemMaxTagLimit", 100);
        if (this.MaxWorkItemTagLimit <= 0)
          this.MaxWorkItemTagLimit = int.MaxValue;
        executionEnvironment = context.ExecutionEnvironment;
        int defaultValue3 = executionEnvironment.IsHostedDeployment ? 30 : 3600;
        this.WorkItemQueryTimeoutInSecond = Math.Min(settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/WorkItemQueryTimeout", defaultValue3), settings.GetValueFromPath<int>(string.Format("/Service/WorkItemTracking/Settings/WorkItemQueryServicingTimeout/db_{0}", (object) context.ServiceHost.ServiceHostInternal().DatabaseId), int.MaxValue));
        if (this.WorkItemQueryTimeoutInSecond <= 0)
          this.WorkItemQueryTimeoutInSecond = int.MaxValue;
        this.WorkItemQueryTimeoutReadReplicaInSecond = Math.Max(this.WorkItemQueryTimeoutInSecond, settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/WorkItemQueryTimeoutReadReplica", 120));
        this.WorkItemQueryTimeoutAnonymousUser = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/WorkItemQueryTimeoutNoCrossProjectPermission", 10);
        int valueFromPath2 = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/ReclassificationSqlCommandTimeoutOverride", 0);
        if (valueFromPath2 > 0)
          this.ReclassificationSqlCommandTimeoutOverride = new int?(valueFromPath2);
        this.MaxQueriesBatchSize = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/MaxQueriesBatchSize", 1000);
        this.MaxQueryItemChildrenUnderParent = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/MaxQueryItemChildren", 1000);
        this.MaxGetQueriesDepth = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/MaxGetQueriesDepth", 2);
        this.MaxBuildListSize = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/MaxBuildListSize", 10);
        this.IsInProcBuildCompletionNotificationEnabled = settings.GetValueFromPath<bool>("/Service/WorkItemTracking/Settings/IsProcBuildCompletionNotificationEnabled", true);
        this.MinClientVersionToProvision = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/MinClientVersionToProvision", 8);
        this.FullTextEnabled = fullTextEnabled;
        this.ServerCulture = serverCulture;
        this.ServerStringComparer = serverStringComparer;
        this.WorkItemReclassificationTimeout = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/WorkItemReclassificationTimeout", 1);
        this.GetMetadataSoapTimeoutInSeconds = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/GetMetadataSoapTimeoutInSeconds", 600);
        this.WorkItemReclassificationStatusCheckInterval = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/WorkItemReclassificationStatusCheckInterval", 300);
        this.WorkItemSyncApiBatchSize = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/WorkItemSyncApiBatchSize", 10000);
        this.WorkItemChangeWatermarkOffset = settings.GetValueFromPath<long>("/Service/WorkItemTracking/Settings/WorkItemChangeWatermarkOffset", 0L);
        this.WorkItemMaxJsonLength = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/WorkItemMaxJsonLength", 52428800);
        this.WorkItemMetadataCacheMaxAgeInDays = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/WorkItemMetadataCacheMaxAgeInDays", 7);
        this.ValidatorConfiguration = (IWitProcessTemplateValidatorConfiguration) new WitProcessTemplateValidatorConfiguration((IEqualityComparer<string>) this.ServerStringComparer, settings);
        this.MaxQueryDOPValue = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/MaxQueryDOPValue", -1);
        this.QueryInThreshold = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/QueryInThreshold", 25);
        executionEnvironment = context.ExecutionEnvironment;
        int defaultValue4 = executionEnvironment.IsOnPremisesDeployment ? -1 : -1;
        this.QueryMaxGrantPercent = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/QueryMaxGrantPercent", defaultValue4);
        this.TopLevelOrOptimizationMaxClauseNumber = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/TopLevelOrOptimizationMaxClauseNumber", 3);
        this.DataImportMode = WorkItemTrackingConfigurationSettingService.WorkItemTrackingConfigurationInfo.ParseDataImportMode(settings.GetValueFromPath<string>("/Service/WorkItemTracking/Settings/DataImportMode", "DefaultDataImportMode"));
        this.ConfigWebLayoutVersion = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/ConfigWebLayoutVersion", 0);
        this.CollectionWebLayoutVersion = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/CollectionWebLayoutVersion", 0);
        this.WorkItemUpdateEventsAggregationTime = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/WorkItemUpdateEventsAggregationTime", 5);
        this.ReadReplicaSettings = new WorkItemTrackingReadReplicaConfiguration(readReplicaUsers, readReplicaUserAgents, readReplicaEnabledCommands, forcedReadReplicaCommands);
        this.QueryOptimizationSettings = new WorkItemTrackingQueryOptimizationConfiguration(queryOptimizations);
        this.QueryExecutionLoggingConfig = new WorkItemTrackingQueryExecutionLoggingConfiguration(queryExecutionLoggingConfig);
        this.ControlContributionInputLimit = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/ControlContributionInputLimit", 100);
        this.WorkItemLinksLimit = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/WorkItemLinksLimit", 1000);
        this.WorkItemRestoreLinksLimit = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/WorkItemRestoreLinksLimit", 10000);
        this.WorkItemRemoteLinksLimit = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/WorkItemRemoteLinksLimit", 25);
        this.LinksCountCiThreshold = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/LinksCountCiThreshold", 100);
        this.LinkUpdateNotificationThreshold = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/LinkUpdateNotificationThreshold", 10);
        this.PromotePerProjectSleepTimeInSeconds = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/PromotePerProjectSleepTimeInSeconds", 60);
        this.RichTextFieldSerializationLength = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/RichTextFieldSerializationLength", 2048);
        this.MaxSecretsScanContentLength = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/MaxSecretsScanContentLength", 5242880);
        this.MaxSecretsScanServiceRequestTimeoutInMilliseconds = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/MaxSecretsScanServiceRequestTimeoutInMilliseconds", 100);
        this.EmptyAliases = emptyAliases;
        this.GetIdentityChangesPageSize = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/GetIdentityChangesPageSize", 5000);
        this.MinimalImsSyncIntervalInSeconds = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/MinimalImsSyncIntervalInSeconds", 300);
        this.MaxNumberOfWorkItemsToProcessInExternalMentions = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/MaxNumberOfWorkItemsToProcessInExternalMentions", 100);
        this.UncommittedLinkChangesLookbackWindowInSeconds = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/UncommittedLinkChangesLookbackWindowInSeconds", 600);
        this.ExternalConnectionMaxReposCountLimit = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/ExternalConnectionMaxReposCountLimit", 250);
        this.BadgeTimeOutInMilliseconds = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/BadgeTimeOutInMilliseconds", 5000);
        this.BadgeBrowserCacheDurationInMinutes = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/BadgeBrowserCacheDurationInMinutes", 5);
      }
    }

    private struct DatabaseProperties
    {
      public bool IsFullTextEnabled { get; set; }

      internal DatabaseCollationInfo Collation { get; set; }
    }
  }
}
