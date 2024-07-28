// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.IVssRequestContextExtensions
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Extension;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement;
using Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers;
using Microsoft.VisualStudio.Services.Search.Common.SearchPlatform;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public static class IVssRequestContextExtensions
  {
    public static string GetConfigValue(this IVssRequestContext requestContext, string key) => requestContext.GetConfigValue<string>(key);

    public static T GetConfigValue<T>(
      this IVssRequestContext requestContext,
      string key,
      TeamFoundationHostType targetHostType,
      T defaultValue = null)
    {
      return requestContext.GetConfigValue<T>(key, targetHostType, false, defaultValue);
    }

    public static T GetConfigValue<T>(
      this IVssRequestContext requestContext,
      string key,
      TeamFoundationHostType targetHostType,
      bool fallThru,
      T defaultValue = null)
    {
      IVssRequestContext vssRequestContext = IVssRequestContextExtensions.ElevateAsNeeded(requestContext, targetHostType);
      return vssRequestContext.GetService<IVssRegistryService>().GetValue<T>(vssRequestContext, (RegistryQuery) key, fallThru, defaultValue);
    }

    public static T GetConfigValue<T>(
      this IVssRequestContext requestContext,
      string key,
      T defaultValue = null)
    {
      return requestContext.GetConfigValue<T>(key, TeamFoundationHostType.Deployment, defaultValue);
    }

    public static string GetCurrentHostConfigValue(
      this IVssRequestContext requestContext,
      string key)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      return requestContext.GetConfigValue<string>(key, requestContext.ServiceHost.HostType);
    }

    public static T GetCurrentHostConfigValue<T>(
      this IVssRequestContext requestContext,
      string key,
      bool fallThru = false,
      T defaultValue = null)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      return requestContext.GetConfigValue<T>(key, requestContext.ServiceHost.HostType, fallThru, defaultValue);
    }

    public static int GetConfigValueOrDefault(
      this IVssRequestContext requestContext,
      string configKey,
      int defaultValue)
    {
      int configValue = requestContext.GetConfigValue<int>(configKey);
      return configValue != 0 ? configValue : defaultValue;
    }

    public static double GetConfigValueOrDefault(
      this IVssRequestContext requestContext,
      string configKey,
      double defaultValue)
    {
      double configValue = requestContext.GetConfigValue<double>(configKey);
      return configValue != 0.0 ? configValue : defaultValue;
    }

    public static int GetCurrentHostConfigValueOrDefault(
      this IVssRequestContext requestContext,
      string configKey,
      int defaultValue)
    {
      int currentHostConfigValue = requestContext.GetCurrentHostConfigValue<int>(configKey);
      return currentHostConfigValue != 0 ? currentHostConfigValue : defaultValue;
    }

    public static void SetCurrentHostConfigValue<T>(
      this IVssRequestContext requestContext,
      string key,
      T value)
    {
      requestContext.GetService<IVssRegistryService>().SetValue<T>(requestContext, key, value);
    }

    public static void DeleteCurrentHostConfigValue(
      this IVssRequestContext requestContext,
      string key)
    {
      requestContext.GetService<IVssRegistryService>().DeleteEntries(requestContext, key);
    }

    public static void SetConfigValue<T>(
      this IVssRequestContext requestContext,
      string key,
      T value)
    {
      IVssRequestContext vssRequestContext = IVssRequestContextExtensions.ElevateAsNeeded(requestContext);
      vssRequestContext.GetService<IVssRegistryService>().SetValue<T>(vssRequestContext, key, value);
    }

    public static void SetCollectionConfigValue<T>(
      this IVssRequestContext requestContext,
      string key,
      T value)
    {
      IVssRequestContext vssRequestContext = IVssRequestContextExtensions.ElevateAsNeeded(requestContext, TeamFoundationHostType.ProjectCollection);
      vssRequestContext.GetService<IVssRegistryService>().SetValue<T>(vssRequestContext, key, value);
    }

    public static Guid GetCollectionID(this IVssRequestContext requestContext)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      return requestContext.ServiceHost.CollectionServiceHost.InstanceId;
    }

    public static string GetOrganizationName(this IVssRequestContext requestContext)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      return requestContext.ServiceHost.OrganizationServiceHost.Name;
    }

    public static Guid GetOrganizationID(this IVssRequestContext requestContext)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      return requestContext.ServiceHost.OrganizationServiceHost.InstanceId;
    }

    public static string GetCollectionName(this IVssRequestContext requestContext)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      return requestContext.ServiceHost.CollectionServiceHost.Name;
    }

    public static string GetAccountIdAsNormalizedString(this IVssRequestContext requestContext)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      return requestContext.ServiceHost.OrganizationServiceHost.InstanceId.ToString().NormalizeString();
    }

    public static string GetCollectionIdAsNormalizedString(this IVssRequestContext requestContext)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      return requestContext.ServiceHost.CollectionServiceHost.InstanceId.ToString().NormalizeString();
    }

    public static string GetCurrentHostIdAsNormalizedString(this IVssRequestContext requestContext) => requestContext.GetCurrentHostId().ToString().NormalizeString();

    public static Guid GetCurrentHostId(this IVssRequestContext requestContext)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      return requestContext.ServiceHost.InstanceId;
    }

    public static ExecutionContext GetExecutionContext(
      this IVssRequestContext requestContext,
      string operation,
      int jobTrigger)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      ITracerCICorrelationDetails tracerCICorrelationDetails;
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
      {
        TracerCICorrelationForCollectionDetails collectionDetails = new TracerCICorrelationForCollectionDetails(requestContext.ActivityId.ToString(), "Collection", new TracerIndexingUnitData(requestContext.GetOrganizationName(), requestContext.GetOrganizationID()), new TracerIndexingUnitData(requestContext.GetCollectionName(), requestContext.GetCollectionID()));
        collectionDetails.Operation = operation;
        collectionDetails.Trigger = jobTrigger;
        tracerCICorrelationDetails = (ITracerCICorrelationDetails) collectionDetails;
      }
      else if (requestContext.ServiceHost.Is(TeamFoundationHostType.Application))
      {
        TracerCICorrelationForOrganizationDetails organizationDetails = new TracerCICorrelationForOrganizationDetails(requestContext.ActivityId.ToString(), "Organization", new TracerIndexingUnitData(requestContext.GetOrganizationName(), requestContext.GetOrganizationID()));
        organizationDetails.Operation = operation;
        organizationDetails.Trigger = jobTrigger;
        tracerCICorrelationDetails = (ITracerCICorrelationDetails) organizationDetails;
      }
      else
        tracerCICorrelationDetails = (ITracerCICorrelationDetails) new TracerCICorrelationForDeploymentDetails(requestContext.ActivityId.ToString(), "Deployment")
        {
          Operation = operation,
          Trigger = jobTrigger
        };
      return requestContext.GetExecutionContext(tracerCICorrelationDetails);
    }

    public static ExecutionContext GetExecutionContext(
      this IVssRequestContext requestContext,
      ITracerCICorrelationDetails tracerCICorrelationDetails)
    {
      return new ExecutionContext(requestContext, tracerCICorrelationDetails)
      {
        FaultService = (IIndexerFaultService) new IndexerV1FaultService(requestContext, (IFaultStore) new RegistryServiceFaultStore(requestContext))
      };
    }

    public static bool IsSearchConfigured(this IVssRequestContext requestContext)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (!requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return true;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      return vssRequestContext.GetService<IVssRegistryService>().GetValue<bool>(vssRequestContext, (RegistryQuery) "/Service/ALMSearch/Settings/IsSearchConfigured", false, false);
    }

    public static bool IsSearchIndexingOnSafeModeForOnPremises(
      this IVssRequestContext requestContext,
      IEntityType entityType)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (entityType == null)
        throw new ArgumentNullException(nameof (entityType));
      if (!requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return false;
      CachedRegistryService service = requestContext.GetService<CachedRegistryService>();
      string query = FormattableString.Invariant(FormattableStringFactory.Create("{0}/{1}/{2}", (object) "/Service/ALMSearch/Settings/IsExtensionOperationInProgress", (object) entityType.Name, (object) InstalledExtensionMessageChangeType.Uninstalled));
      return service.GetValue<bool>(requestContext, (RegistryQuery) query, false, false);
    }

    public static bool IsCodeIndexingEnabled(this IVssRequestContext requestContext)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (!requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return requestContext.IsFeatureEnabled("Search.Server.Code.Indexing");
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      return (requestContext.IsSearchConfigured() || requestContext.IsSearchIndexingOnSafeModeForOnPremises((IEntityType) CodeEntityType.GetInstance())) && requestContext.IsFeatureEnabled("Search.Server.Code.Indexing") && service.GetValue<bool>(requestContext, (RegistryQuery) "/Service/ALMSearch/Settings/IsCollectionIndexed", false, false);
    }

    public static bool IsLargeRepository(
      this IVssRequestContext requestContext,
      string repositoryName)
    {
      if (repositoryName == null || string.IsNullOrWhiteSpace(repositoryName))
        return false;
      string currentHostConfigValue = requestContext.GetCurrentHostConfigValue("/Service/ALMSearch/Settings/LargeRepositoriesName");
      if (!string.IsNullOrWhiteSpace(currentHostConfigValue))
      {
        if (((IEnumerable<string>) currentHostConfigValue.Split(',')).Contains<string>(repositoryName))
          return true;
      }
      return false;
    }

    public static bool IsWorkItemIndexingEnabled(this IVssRequestContext requestContext)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (!requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return requestContext.IsFeatureEnabled("Search.Server.WorkItem.Indexing");
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      return (requestContext.IsSearchConfigured() || requestContext.IsSearchIndexingOnSafeModeForOnPremises((IEntityType) WorkItemEntityType.GetInstance())) && requestContext.IsFeatureEnabled("Search.Server.WorkItem.Indexing") && service.GetValue<bool>(requestContext, (RegistryQuery) "/Service/ALMSearch/Settings/IsCollectionIndexedForWorkItem", false, false);
    }

    public static bool IsWikiIndexingEnabled(this IVssRequestContext requestContext)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (!requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return requestContext.IsFeatureEnabled("Search.Server.Wiki.Indexing");
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      return (requestContext.IsSearchConfigured() || requestContext.IsSearchIndexingOnSafeModeForOnPremises((IEntityType) WikiEntityType.GetInstance())) && requestContext.IsFeatureEnabled("Search.Server.Wiki.Indexing") && service.GetValue<bool>(requestContext, (RegistryQuery) "/Service/ALMSearch/Settings/IsCollectionIndexedForWiki", false, false);
    }

    public static bool IsPackageIndexingEnabled(this IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("Search.Server.Package.Indexing");

    public static int GetLocaleId(this IVssRequestContext requestContext)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      int defaultValue = 1033;
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment && !requestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/OverrideTfsConfiguredLocale"))
        defaultValue = requestContext.GetConfigValueOrDefault("/Service/ALMSearch/Settings/LocaleId", defaultValue);
      return defaultValue;
    }

    public static bool IsIndexingEnabled(
      this IVssRequestContext requestContext,
      IEntityType entityType)
    {
      if (entityType == null)
        throw new ArgumentNullException(nameof (entityType));
      switch (entityType.Name)
      {
        case "Code":
          return requestContext.IsCodeIndexingEnabled();
        case "WorkItem":
          return requestContext.IsWorkItemIndexingEnabled();
        case "Wiki":
          return requestContext.IsWikiIndexingEnabled();
        case "Package":
          return requestContext.IsPackageIndexingEnabled();
        default:
          return false;
      }
    }

    public static bool IsContinuousIndexingEnabled(
      this IVssRequestContext requestContext,
      IEntityType entityType)
    {
      return requestContext.IsIndexingEnabled(entityType) && requestContext.IsCrudOperationsFeatureEnabled(entityType);
    }

    public static bool IsCrudOperationsFeatureEnabled(
      this IVssRequestContext requestContext,
      IEntityType entityType)
    {
      if (entityType == null)
        throw new ArgumentNullException(nameof (entityType));
      switch (entityType.Name)
      {
        case "Code":
          return requestContext.IsFeatureEnabled("Search.Server.Code.CrudOperations");
        case "WorkItem":
          return requestContext.IsFeatureEnabled("Search.Server.WorkItem.CrudOperations");
        case "Wiki":
          return requestContext.IsFeatureEnabled("Search.Server.Wiki.ContinuousIndexing");
        case "Package":
          return requestContext.IsFeatureEnabled("Search.Server.Package.ContinuousIndexing");
        default:
          return false;
      }
    }

    public static bool IsWorkItemCrudOperationsEnabled(this IVssRequestContext requestContext) => requestContext.IsWorkItemIndexingEnabled() && requestContext.IsFeatureEnabled("Search.Server.WorkItem.CrudOperations");

    public static bool IsCodeCrudOperationsEnabled(this IVssRequestContext requestContext) => requestContext.IsCodeIndexingEnabled() && requestContext.IsFeatureEnabled("Search.Server.Code.CrudOperations");

    public static bool IsCodeNotificationHandlingEnabled(this IVssRequestContext requestContext) => requestContext.IsCodeIndexingEnabled() && requestContext.IsFeatureEnabled("Search.Server.Code.CrudOperations") && requestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/CodeNotificationHandlingEnabled", TeamFoundationHostType.ProjectCollection, true);

    public static bool IsWorkItemSkipDiscussionsCrawlingEnabled(
      this IVssRequestContext requestContext)
    {
      return requestContext.IsFeatureEnabled("Search.Server.WorkItem.SkipDiscussionsCrawling");
    }

    public static IVssRequestContext ElevateAsNeeded(IVssRequestContext requestContext) => IVssRequestContextExtensions.ElevateAsNeeded(requestContext, TeamFoundationHostType.Deployment);

    public static IVssRequestContext ElevateAsNeeded(
      IVssRequestContext requestContext,
      TeamFoundationHostType targetHostType)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      return requestContext.ServiceHost.HostType != targetHostType ? requestContext.To(targetHostType).Elevate() : requestContext;
    }

    public static bool IsAccountWithLargeRepository(this IVssRequestContext requestContext) => !string.IsNullOrWhiteSpace(requestContext.GetCurrentHostConfigValue<string>("/Service/ALMSearch/Settings/LargeRepositoriesName"));

    public static ISet<string> GetLargeRepoIdSet(this IVssRequestContext requestContext)
    {
      ISet<string> largeRepoIdSet = (ISet<string>) null;
      string configValue = requestContext.GetConfigValue<string>("/Service/ALMSearch/Settings/LargeRepositoriesGuids", TeamFoundationHostType.ProjectCollection);
      if (!string.IsNullOrWhiteSpace(configValue))
        largeRepoIdSet = (ISet<string>) new HashSet<string>((IEnumerable<string>) configValue.Trim().ToLowerInvariant().Split(','), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (largeRepoIdSet == null)
        largeRepoIdSet = (ISet<string>) new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      return largeRepoIdSet;
    }

    public static bool IsCollectionSoftDeleted(this IVssRequestContext requestContext)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return false;
      return requestContext.GetService<ICollectionService>().GetCollection(requestContext, (IEnumerable<string>) new string[1]
      {
        "SystemProperty.LastLogicalDeletedDate"
      }).Status == CollectionStatus.LogicallyDeleted;
    }

    public static bool IsProjectSoftDeleted(
      this IVssRequestContext requestContext,
      string projectId)
    {
      if (!string.IsNullOrWhiteSpace(projectId))
      {
        string currentHostConfigValue = requestContext.GetCurrentHostConfigValue("/Service/SearchShared/Settings/SoftDeletedProjectIds");
        if (!string.IsNullOrWhiteSpace(currentHostConfigValue))
        {
          if (((IEnumerable<string>) currentHostConfigValue.Split(',')).Contains<string>(projectId, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
            return true;
        }
      }
      return false;
    }

    public static void SetFeatureFlagState(
      this IVssRequestContext requestContext,
      string featureFlag,
      FeatureAvailabilityState state,
      TraceMetaData traceMedata)
    {
      IVssRequestContext vssRequestContext = requestContext != null ? requestContext.To(TeamFoundationHostType.ProjectCollection).Elevate() : throw new ArgumentNullException(nameof (requestContext));
      ITeamFoundationFeatureAvailabilityService service = vssRequestContext.GetService<ITeamFoundationFeatureAvailabilityService>();
      FeatureAvailabilityInformation featureInformation = service.GetFeatureInformation(vssRequestContext, featureFlag);
      service.SetFeatureState(vssRequestContext, featureFlag, state);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(traceMedata, FormattableString.Invariant(FormattableStringFactory.Create("FeatureFlag:{0}, Changed state from: {1} to: {2}", (object) featureFlag, (object) featureInformation.EffectiveState, (object) state)));
    }

    public static T GetRedirectedClientIfNeeded<T>(
      this IVssRequestContext requestContext,
      VssHttpClientOptions httpClientOptions = null)
      where T : VssHttpClientBase
    {
      return requestContext.GetService<ICollectionRedirectionService>().GetClient<T>(requestContext, httpClientOptions);
    }

    public static T GetRedirectedClientIfNeeded<T>(
      this IVssRequestContext requestContext,
      Guid serviceAreaId)
      where T : VssHttpClientBase
    {
      return requestContext.GetService<ICollectionRedirectionService>().GetClient<T>(requestContext, serviceAreaId);
    }

    public static bool IsFTSEnabled(this IVssRequestContext requestContext) => requestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/EnableCodeSearchOnOriginalContent", true) && requestContext.IsFeatureEnabled("Search.Server.IndexOriginalCodeContent");

    public static bool IsJobEnabled(this IVssRequestContext requestContext, Guid jobId) => requestContext.GetCurrentHostConfigValue<bool>(ConfigKeys.JobEnabledState(jobId), defaultValue: true);

    public static bool IsZeroStalenessReindexingEnabled(
      this IVssRequestContext requestContext,
      IEntityType entityType)
    {
      switch (entityType.Name)
      {
        case "Code":
          return requestContext.IsCodeReindexingWithZeroStalenessFeatureEnabled();
        case "WorkItem":
          return requestContext.IsWorkItemReindexingWithZeroStalenessFeatureEnabled();
        default:
          return false;
      }
    }

    public static bool IsCollectionFinalizationPaused(
      this IVssRequestContext requestContext,
      IEntityType entityType)
    {
      switch (entityType.Name)
      {
        case "Code":
          return requestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/PauseCodeCollectionFinalizationDuringZLRI", true);
        case "WorkItem":
          return requestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/PauseWorkItemCollectionFinalizationDuringZLRI", true);
        default:
          return false;
      }
    }

    public static bool IsCodeReindexingWithZeroStalenessFeatureEnabled(
      this IVssRequestContext requestContext)
    {
      return requestContext.IsFeatureEnabled("Search.Server.Code.ShadowIndexing");
    }

    public static bool IsWorkItemReindexingWithZeroStalenessFeatureEnabled(
      this IVssRequestContext requestContext)
    {
      return requestContext.IsFeatureEnabled("Search.Server.WorkItem.ShadowIndexing");
    }

    public static bool IsBoardIndexingFeatureFlagEnabled(this IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("Search.Server.Board.Indexing");

    public static bool IsNoPayloadCodeSearchHighlighterV2FeatureEnabled(
      this IVssRequestContext requestContext)
    {
      return requestContext.IsFeatureEnabled("Search.Server.Code.NoPayloadCodeSearchHighlighterV2");
    }

    public static bool IsDLITStrictValidationEnabled(this IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("Search.Server.DLIT.StrictValidationEnabled");

    public static bool IsCodesearchV4HighlighterFeatureEnabled(
      this IVssRequestContext requestContext)
    {
      return requestContext.IsFeatureEnabled("Search.Server.Code.CodesearchV4Highlighter");
    }

    public static bool IsWildcardConstantScoreRewriteFeatureEnabled(
      this IVssRequestContext requestContext)
    {
      return requestContext.IsFeatureEnabled("Search.Server.Code.WildcardConstantScoreRewrite");
    }

    public static bool IsProjectScopedResultsCountFeatureEnabled(
      this IVssRequestContext requestContext)
    {
      return requestContext.IsFeatureEnabled("Search.Server.Code.ProjectScopedResultsCount");
    }

    public static bool IsDLITDocumentCreationEnabled(this IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("Search.Server.DLIT.DocumentCreationEnabled");

    public static bool IsQueryingNGramsEnabled(this IVssRequestContext requestContext)
    {
      requestContext.CheckProjectCollectionRequestContext();
      return requestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/QueryCodeNGrams");
    }

    public static string GetElasticsearchPlatformSettings(
      this IVssRequestContext requestContext,
      string platformSettingsKey,
      string defaultValue = null)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      string platformSettings = requestContext.GetConfigValue<string>(platformSettingsKey, TeamFoundationHostType.Deployment, defaultValue);
      KeyValuePair<string, string> credentials = requestContext.To(TeamFoundationHostType.Deployment).GetService<ISearchPlatformService>().GetCredentials(requestContext);
      if (!credentials.Equals((object) new KeyValuePair<string, string>()))
        platformSettings = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0},{1}={2},{3}={4}", (object) platformSettings, (object) "User", (object) credentials.Key, (object) "Password", (object) credentials.Value);
      return platformSettings;
    }

    public static bool IsGetLatestRefEnabled(this IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("Search.Server.Code.EnableGetLatestRef");
  }
}
