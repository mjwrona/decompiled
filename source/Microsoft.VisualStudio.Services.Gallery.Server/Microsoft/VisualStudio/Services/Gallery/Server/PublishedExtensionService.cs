// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.PublishedExtensionService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Commerce;
using Microsoft.VisualStudio.Services.Commerce.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Gallery.Server.Components;
using Microsoft.VisualStudio.Services.Gallery.Server.CVS;
using Microsoft.VisualStudio.Services.Gallery.Server.Extension;
using Microsoft.VisualStudio.Services.Gallery.Server.Extension.Cache;
using Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPayloadValidator;
using Microsoft.VisualStudio.Services.Gallery.Server.Extension.VsGalleryMigration;
using Microsoft.VisualStudio.Services.Gallery.Server.Facade;
using Microsoft.VisualStudio.Services.Gallery.Server.Facade.PMP;
using Microsoft.VisualStudio.Services.Gallery.Server.Facade.PMP.IndexUpdater;
using Microsoft.VisualStudio.Services.Gallery.Server.Facade.Search;
using Microsoft.VisualStudio.Services.Gallery.Server.SoapService.Models;
using Microsoft.VisualStudio.Services.Gallery.Server.Telemetry;
using Microsoft.VisualStudio.Services.Gallery.Server.Utility;
using Microsoft.VisualStudio.Services.Gallery.Types.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class PublishedExtensionService : IPublishedExtensionService, IVssFrameworkService
  {
    private const int MaxQueryPageSize = 1000;
    private const int VSCodeDefaultQueryPageSize = 1;
    private PublishedExtensionService.CategoriesData m_categoriesData;
    private readonly HashSet<string> m_AssetsToPublishAtCreateUpdate = new HashSet<string>()
    {
      "Microsoft.VisualStudio.Services.VSIXPackage",
      "Microsoft.VisualStudio.Services.Icons.Default",
      "Microsoft.VisualStudio.Services.Icons.Small",
      "Microsoft.VisualStudio.Services.Content.Details"
    };
    private readonly HashSet<string> m_VSIXAssetToPublishAtCreateUpdate = new HashSet<string>()
    {
      "Microsoft.VisualStudio.Services.VSIXPackage"
    };
    private readonly IDictionary<ExtensionQueryFilterType, SearchFilterType> FilterTypeMap = (IDictionary<ExtensionQueryFilterType, SearchFilterType>) new Dictionary<ExtensionQueryFilterType, SearchFilterType>()
    {
      {
        ExtensionQueryFilterType.SearchText,
        SearchFilterType.SearchPhrase
      },
      {
        ExtensionQueryFilterType.InstallationTarget,
        SearchFilterType.InstallationTarget
      },
      {
        ExtensionQueryFilterType.Category,
        SearchFilterType.Category
      },
      {
        ExtensionQueryFilterType.Tag,
        SearchFilterType.Tag
      },
      {
        ExtensionQueryFilterType.ExcludeWithFlags,
        SearchFilterType.ExcludeExtensionsWithFlags
      },
      {
        ExtensionQueryFilterType.IncludeWithFlags,
        SearchFilterType.IncludeExtensionsWithFlags
      },
      {
        ExtensionQueryFilterType.Lcid,
        SearchFilterType.Lcid
      },
      {
        ExtensionQueryFilterType.InstallationTargetVersion,
        SearchFilterType.InstallationTargetVersion
      },
      {
        ExtensionQueryFilterType.InstallationTargetVersionRange,
        SearchFilterType.InstallationTargetVersionRange
      },
      {
        ExtensionQueryFilterType.PublisherDisplayName,
        SearchFilterType.ExactPublisherDisplayName
      },
      {
        ExtensionQueryFilterType.IncludeWithPublisherFlags,
        SearchFilterType.IncludeExtensionsWithPublisherFlags
      },
      {
        ExtensionQueryFilterType.OrganizationSharedWith,
        SearchFilterType.OrganizationSharedWith
      },
      {
        ExtensionQueryFilterType.ProductArchitecture,
        SearchFilterType.ProductArchitecture
      },
      {
        ExtensionQueryFilterType.TargetPlatform,
        SearchFilterType.TargetPlatform
      }
    };
    private readonly HashSet<string> m_AllowedExtensionsAllowedToConvertFromPaidToFree = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "ms.vss-plans",
      "ms.feed"
    };
    private bool m_categoriesRequireLoad = true;
    private readonly object m_loaderLock = new object();
    private static readonly Guid s_extensionChanged = new Guid("66737488-7B6E-406A-910D-E3EBAF214B59");
    private readonly SearchQueryParser searchQueryParser;
    private InMemoryQuery m_inMemoryQuery;
    private IMemoryCache<CachedExtensionData> m_inMemoryCacheForVsCode;
    private IMemoryCache<CachedExtensionData> m_inMemoryCacheForVsts;
    private IMemoryCache<CachedBackConsolidatedExtensionData> m_inMemoryCacheForBackConsolidation;
    private const string m_DeleteExtensionNotificationName = "DeleteExtension";
    private const string m_UpdateExtensionPropertiesNotificationName = "UpdateExtensionProperties";
    private const string m_ProcessValidationResultNotificationName = "ProcessValidationResult";
    private const string m_BackConsolidationNotificationName = "BackConsolidation";
    private ProductExtensionsCacheBuilder m_VscodeExtensionsCacheBuilder;
    private ProductExtensionsCacheBuilder m_VstsExtensionsCacheBuilder;
    private BackConsolidatedExtensionsCacheBuilder m_BackConsExtensionsCacheBuilder;
    private ISearchServiceFacade m_searchServiceFacade;
    private ISearchService m_ExternalSearchService;
    private SearchHelper m_SearchHelper;
    internal Microsoft.VisualStudio.Services.Identity.Identity m_everyoneGroup;
    internal const string RegistryPathForPercentageOfSearchTrafficToRedirect = "/Configuration/Service/Gallery/SearchService/PercentageofSearchTrafficToRedirect";
    internal const string RegistryPathForPercentageOfChinaSearchTrafficToRedirect = "/Configuration/Service/Gallery/SearchService/PercentageOfChinaSearchTrafficToRedirect";
    internal const string RegistryPathForNewSearchServiceEndpoint = "/Configuration/Service/Gallery/SearchService/SearchServiceEndpoint";
    private const string s_assetCountLimitRegistryRootPath = "/Configuration/Service/Gallery/AssetCountLimit";
    private const string s_assetCountLimitRegistryPathVSTSNewExtension = "/Configuration/Service/Gallery/AssetCountLimit/VSTSNew";
    private const int assetCountDefaultLimitVSTSNewExtension = 1000;
    private const string s_assetCountLimitRegistryPathVSTSUpdateExtension = "/Configuration/Service/Gallery/AssetCountLimit/VSTSUpdate";
    private const int assetCountDefaultLimitVSTSUpdateExtension = 5000;
    private const string s_assetCountLimitRegistryPathVSIde = "/Configuration/Service/Gallery/AssetCountLimit/VSIde";
    private const int assetCountDefaultLimitVSIde = 250;
    private const string s_assetCountLimitRegistryPathVSForMac = "/Configuration/Service/Gallery/AssetCountLimit/VSForMac";
    private const int assetCountDefaultLimitVSForMac = 250;
    private const string s_assetCountLimitRegistryPathVSCode = "/Configuration/Service/Gallery/AssetCountLimit/VSCode";
    private const int assetCountDefaultLimitVSCode = 50;
    private const string CommandKeyForSearchExtensions = "SearchExtensions";
    private const int SearchExtensionsCircuitBreakerTimeoutInSeconds = 15;
    private const int SearchExtensionsMaxConcurrentRequestsPerMinute = 50;
    private const int SearchExtensionsMaxRequestsPerMinute = 250;
    private const int SearchExtensionsRollingStatisticalWindowInMilliseconds = 60000;
    private const int SearchExtensionsErrorThresholdPercentage = 70;
    private const string CommandKeyForQueryExtensions = "QueryExtensions";
    private const int QueryExtensionsCircuitBreakerTimeoutInSeconds = 5;
    private const int QueryExtensionsMaxConcurrentRequestsPerMinute = 50;
    private const int QueryExtensionsMaxRequestsPerMinute = 700;
    private const int QueryExtensionsRollingStatisticalWindowInMilliseconds = 60000;
    private const int QueryExtensionsErrorThresholdPercentage = 70;
    private const string CommandKeyForNewSearchExtensionsService = "NewSearchExtensionsService";
    private const int NewSearchServiceErrorThresholdPercentage = 10;

    public PublishedExtensionService()
    {
      this.searchQueryParser = new SearchQueryParser();
      this.m_inMemoryQuery = new InMemoryQuery();
      this.m_VscodeExtensionsCacheBuilder = ProductExtensionsCacheBuilderFactory.GetProductExtensionsCacheBuilder("vscode");
      this.m_VstsExtensionsCacheBuilder = ProductExtensionsCacheBuilderFactory.GetProductExtensionsCacheBuilder("vsts");
      this.m_BackConsExtensionsCacheBuilder = (BackConsolidatedExtensionsCacheBuilder) new BackConsolidationExtensionsCacheBuilder();
      this.m_inMemoryCacheForVsCode = (IMemoryCache<CachedExtensionData>) new MemoryCache<CachedExtensionData>(this.m_VscodeExtensionsCacheBuilder.CacheTimeoutInSeconds, new Func<IVssRequestContext, CachedExtensionData>(this.m_VscodeExtensionsCacheBuilder.ExtensionCachedDataBuilder));
      this.m_inMemoryCacheForVsts = (IMemoryCache<CachedExtensionData>) new MemoryCache<CachedExtensionData>(this.m_VstsExtensionsCacheBuilder.CacheTimeoutInSeconds, new Func<IVssRequestContext, CachedExtensionData>(this.m_VstsExtensionsCacheBuilder.ExtensionCachedDataBuilder));
      this.m_inMemoryCacheForBackConsolidation = (IMemoryCache<CachedBackConsolidatedExtensionData>) new MemoryCache<CachedBackConsolidatedExtensionData>(this.m_VstsExtensionsCacheBuilder.CacheTimeoutInSeconds, new Func<IVssRequestContext, CachedBackConsolidatedExtensionData>(this.m_BackConsExtensionsCacheBuilder.BackConsolidatedExtensionCachedDataBuilder));
      this.m_SearchHelper = new SearchHelper();
    }

    internal PublishedExtensionService(
      SearchQueryParser searchQueryParser,
      IMemoryCache<CachedExtensionData> memoryCacheForVsCode,
      IMemoryCache<CachedExtensionData> memoryCacheForVsts,
      IMemoryCache<CachedBackConsolidatedExtensionData> memoryCacheForBackConsolidation,
      InMemoryQuery inMemoryQuery,
      ISearchService searchService,
      SearchHelper searchHelper,
      ISearchServiceFacade searchServiceFacade)
    {
      this.searchQueryParser = searchQueryParser;
      this.m_inMemoryQuery = inMemoryQuery;
      this.m_inMemoryCacheForVsCode = memoryCacheForVsCode;
      this.m_inMemoryCacheForVsts = memoryCacheForVsts;
      this.m_inMemoryCacheForBackConsolidation = memoryCacheForBackConsolidation;
      this.m_VscodeExtensionsCacheBuilder = ProductExtensionsCacheBuilderFactory.GetProductExtensionsCacheBuilder("vscode");
      this.m_VstsExtensionsCacheBuilder = ProductExtensionsCacheBuilderFactory.GetProductExtensionsCacheBuilder("vsts");
      this.m_BackConsExtensionsCacheBuilder = (BackConsolidatedExtensionsCacheBuilder) new BackConsolidationExtensionsCacheBuilder();
      this.m_ExternalSearchService = searchService;
      this.m_SearchHelper = searchHelper;
      this.m_searchServiceFacade = searchServiceFacade ?? throw new ArgumentNullException(nameof (m_searchServiceFacade));
    }

    internal void SetPublishedExtensionServiceForCacheBuilder()
    {
      this.m_VscodeExtensionsCacheBuilder.SetPublishedExtensionService(this);
      this.m_VstsExtensionsCacheBuilder.SetPublishedExtensionService(this);
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      try
      {
        IdentityService service = systemRequestContext.GetService<IdentityService>();
        IdentityDescriptor identityDescriptor = IdentityDomain.MapFromWellKnownIdentifier(service.GetScope(systemRequestContext, systemRequestContext.ServiceHost.InstanceId).Id, GroupWellKnownIdentityDescriptors.EveryoneGroup);
        this.m_everyoneGroup = service.ReadIdentities(systemRequestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
        {
          identityDescriptor
        }, QueryMembership.None, (IEnumerable<string>) null)[0];
      }
      catch (ServiceOwnerNotFoundException ex)
      {
        if (systemRequestContext.IsServicingContext && systemRequestContext.ExecutionEnvironment.IsDevFabricDeployment)
        {
          this.m_everyoneGroup = new Microsoft.VisualStudio.Services.Identity.Identity();
          this.m_everyoneGroup.Id = Guid.Empty;
        }
        else
          throw;
      }
      this.m_searchServiceFacade = this.m_searchServiceFacade ?? (ISearchServiceFacade) new SearchServiceFacade(Microsoft.VisualStudio.Services.Gallery.Server.Facade.HttpClientFactory.New(), (ITracerFacade) new VssRequestContextTracerFacade(systemRequestContext.RequestTracer));
      this.m_ExternalSearchService = systemRequestContext.GetService<ISearchService>();
      this.EnsureCategoriesLoaded(systemRequestContext);
      ITeamFoundationSqlNotificationService service1 = systemRequestContext.GetService<ITeamFoundationSqlNotificationService>();
      if (PublishedExtensionService.CanUseInMemoryCache(systemRequestContext))
      {
        if (systemRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableQueryExtensionsCacheForVSTS"))
        {
          try
          {
            this.m_VstsExtensionsCacheBuilder.SetPublishedExtensionService(this);
            this.m_inMemoryCacheForVsts.GetCachedData(systemRequestContext);
          }
          catch (Exception ex)
          {
            systemRequestContext.TraceException(12062052, "gallery", "VstsExtensionCacheRefreshServiceStart", ex);
            throw;
          }
          finally
          {
            this.m_VstsExtensionsCacheBuilder.SetPublishedExtensionService((PublishedExtensionService) null);
          }
        }
        try
        {
          this.m_VscodeExtensionsCacheBuilder.SetPublishedExtensionService(this);
          this.m_inMemoryCacheForVsCode.GetCachedData(systemRequestContext);
        }
        catch (Exception ex)
        {
          systemRequestContext.TraceException(12061107, "gallery", "VSCodeExtensionCacheRefreshServiceStart", ex);
          throw;
        }
        finally
        {
          this.m_VscodeExtensionsCacheBuilder.SetPublishedExtensionService((PublishedExtensionService) null);
        }
        if (systemRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableBackConsolidation"))
        {
          try
          {
            this.m_inMemoryCacheForBackConsolidation.GetCachedData(systemRequestContext);
          }
          catch (Exception ex)
          {
            systemRequestContext.TraceException(12062113, "gallery", "BackConsolidatedExtensionCacheRefresh", ex);
            throw;
          }
          service1.RegisterNotification(systemRequestContext, "Default", GalleryNotificationEventIds.BackConsolidationUpdate, new SqlNotificationCallback(this.UpdateBackConsolidatedExtensionInCache), false);
        }
        service1.RegisterNotification(systemRequestContext, "Default", GalleryNotificationEventIds.ExtensionUpdateDelete, new SqlNotificationCallback(this.UpdateExtensionInCache), false);
      }
      service1.RegisterNotification(systemRequestContext, "Default", GalleryServerUtil.CategorySQLNotifications, new SqlNotificationHandler(this.CategoryChangeCallback), true);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      ITeamFoundationSqlNotificationService service = systemRequestContext.GetService<ITeamFoundationSqlNotificationService>();
      service.UnregisterNotification(systemRequestContext, "Default", GalleryServerUtil.CategorySQLNotifications, new SqlNotificationHandler(this.CategoryChangeCallback), true);
      if (!PublishedExtensionService.CanUseInMemoryCache(systemRequestContext))
        return;
      service.UnregisterNotification(systemRequestContext, "Default", GalleryNotificationEventIds.ExtensionUpdateDelete, new SqlNotificationCallback(this.UpdateExtensionInCache), false);
      if (!systemRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableBackConsolidation"))
        return;
      service.UnregisterNotification(systemRequestContext, "Default", GalleryNotificationEventIds.BackConsolidationUpdate, new SqlNotificationCallback(this.UpdateBackConsolidatedExtensionInCache), false);
    }

    public void PublishReCaptchaTokenCIForVSCodeExtension(
      IVssRequestContext requestContext,
      IDictionary<string, object> ciData)
    {
      ArgumentUtility.CheckForNull<IDictionary<string, object>>(ciData, nameof (ciData));
      Guid userId = requestContext.GetUserId();
      ciData.Add("Vsid", (object) userId);
      CustomerIntelligenceData properties = new CustomerIntelligenceData(ciData);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", Convert.ToString(ciData[CustomerIntelligenceProperty.Action]), properties);
    }

    public PublishedExtension CreateExtension(
      IVssRequestContext requestContext,
      ExtensionPackage extensionPackage,
      string requestingPublisherName)
    {
      using (Stream extensionPackageStream = GalleryServerUtil.GetExtensionPackageStream(extensionPackage))
        return this.CreateExtension(requestContext, extensionPackageStream, requestingPublisherName);
    }

    public PublishedExtension CreateExtension(
      IVssRequestContext requestContext,
      Stream extensionPackageStream,
      string requestingPublisherName)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Stream>(extensionPackageStream, nameof (extensionPackageStream));
      return this.CreateExtensionFromStream(requestContext, extensionPackageStream, requestingPublisherName, (IEnumerable<ExtensionFile>) null, false);
    }

    public PublishedExtension CreateExtension(
      IVssRequestContext requestContext,
      ExtensionPackage extensionPackage,
      string requestingPublisherName,
      IEnumerable<ExtensionFile> uploadedAssets,
      bool immediateVersionValidation)
    {
      using (Stream extensionPackageStream = GalleryServerUtil.GetExtensionPackageStream(extensionPackage))
        return this.CreateExtension(requestContext, extensionPackageStream, requestingPublisherName, uploadedAssets, immediateVersionValidation);
    }

    public PublishedExtension CreateExtension(
      IVssRequestContext requestContext,
      Stream extensionPackageStream,
      string requestingPublisherName,
      IEnumerable<ExtensionFile> uploadedAssets,
      bool immediateVersionValidation)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Stream>(extensionPackageStream, nameof (extensionPackageStream));
      if (requestContext.ExecutionEnvironment.IsHostedDeployment && !requestContext.ExecutionEnvironment.IsDevFabricDeployment)
        throw new NotSupportedException("This interface cannot be invoked in a hosted scenario.");
      return this.CreateExtensionFromStream(requestContext, extensionPackageStream, requestingPublisherName, uploadedAssets, immediateVersionValidation);
    }

    public void DeleteExtension(
      IVssRequestContext requestContext,
      Guid extensionId,
      string version,
      bool deleteStatisticsAndReviews = true)
    {
      PublishedExtension extension;
      using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
        extension = component.QueryExtensionById(extensionId, (string) null, Guid.Empty, ExtensionQueryFlags.IncludeVersions | ExtensionQueryFlags.IncludeFiles | ExtensionQueryFlags.IncludeInstallationTargets | ExtensionQueryFlags.IncludeStatistics | ExtensionQueryFlags.IncludeMetadata);
      this.DeleteExtension(requestContext, extension, version, deleteStatisticsAndReviews);
    }

    public void DeleteExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string version,
      bool deleteStatisticsAndReviews = true)
    {
      GalleryUtil.CheckPublisherName(publisherName);
      GalleryUtil.CheckExtensionName(extensionName);
      PublishedExtension extension;
      using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
        extension = component.QueryExtension(publisherName, extensionName, (string) null, Guid.Empty, ExtensionQueryFlags.IncludeVersions | ExtensionQueryFlags.IncludeFiles | ExtensionQueryFlags.IncludeInstallationTargets | ExtensionQueryFlags.IncludeStatistics | ExtensionQueryFlags.IncludeMetadata);
      this.DeleteExtension(requestContext, extension, version, deleteStatisticsAndReviews);
    }

    public bool IsValidAzurePublisherAndExtension(
      IVssRequestContext requestContext,
      AzureRestApiRequestModel azureRestApiRequestModel,
      ExtensionQueryFlags flags)
    {
      Microsoft.VisualStudio.Services.Gallery.WebApi.AssetDetails assetDetails = azureRestApiRequestModel.AssetDetails;
      Answers answers = assetDetails != null ? assetDetails.Answers : throw new ArgumentNullException("azureRestApiRequestModel.AssetDetails");
      string publisherName = answers != null ? answers.VSMarketplacePublisherName : throw new ArgumentNullException("azureRestApiRequestModel.AssetDetails.Answers");
      string marketplaceExtensionName = answers.VSMarketplaceExtensionName;
      GalleryUtil.CheckPublisherName(publisherName);
      GalleryUtil.CheckExtensionName(marketplaceExtensionName);
      AzurePublisher azurePublisher;
      using (AzurePublisherComponent component = requestContext.CreateComponent<AzurePublisherComponent>())
        azurePublisher = component.QueryAssociatedAzurePublisher(publisherName);
      string naturalIdentifier = assetDetails.PublisherNaturalIdentifier;
      if (!string.Equals(azurePublisher.AzurePublisherId, naturalIdentifier, StringComparison.OrdinalIgnoreCase))
        throw new ArgumentException(GalleryResources.AzurePublisherDoesNotExist((object) naturalIdentifier, (object) "https://go.microsoft.com/fwlink/?linkid=823198"));
      using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
        component.QueryExtension(publisherName, marketplaceExtensionName, (string) null, Guid.Empty, flags);
      return true;
    }

    private void DeleteExtension(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      string version,
      bool deleteStatisticsAndReviews = true)
    {
      if (string.IsNullOrEmpty(version))
        this.ValidateIfDeleteExtensionPermitted(requestContext, extension);
      else
        PublishedExtensionService.ValidateIfDeleteVersionPermitted(requestContext, extension, version);
      using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
        component.DeleteExtension(extension.Publisher.PublisherName, extension.ExtensionName, version);
      requestContext.GetService<IPublisherAssetService>().DeleteExtensionAssets(requestContext, extension, version);
      this.PublishEvent(requestContext, ExtensionEventType.ExtensionDeleted, extension, version);
      if (string.IsNullOrEmpty(version))
      {
        GallerySecurity.OnDataChanged(requestContext);
        string str = "";
        if (extension.InstallationTargets != null)
          str = GalleryUtil.GetProductTypeForInstallationTargets((IEnumerable<InstallationTarget>) extension.InstallationTargets);
        GalleryServerUtil.NotifyGalleryDataChanged(requestContext, GalleryNotificationEventIds.ExtensionUpdateDelete, "DeleteExtension: " + str + " " + extension.Publisher.PublisherName + "." + extension.ExtensionName);
        if (deleteStatisticsAndReviews)
          GalleryServerUtil.QueueExtensionDataCleanUpJob(requestContext, new List<PublishedExtension>()
          {
            extension
          });
        if (extension.IsVsExtension())
        {
          string vsixId = extension.GetVsixId();
          if (!vsixId.IsNullOrEmpty<char>())
          {
            IVsixIdManagerService service = requestContext.GetService<IVsixIdManagerService>();
            ReservedVsixId reservedVsixId1 = new ReservedVsixId()
            {
              VsixId = vsixId,
              Purpose = ReservedVsixIdPurposeType.Reserved,
              UserId = requestContext.GetUserId().ToString()
            };
            IVssRequestContext requestContext1 = requestContext;
            ReservedVsixId reservedVsixId2 = reservedVsixId1;
            service.AddReservedVsixId(requestContext1, reservedVsixId2);
          }
        }
      }
      else if (extension.IsVsCodeExtension() && requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableVSCodeWebTagPopulatorJob"))
        this.QueueVSCodeWebExtensionTagPopulatorJob(requestContext, extension);
      try
      {
        SearchHelper searchHelper = this.m_SearchHelper;
        IVssRequestContext requestContext2 = requestContext;
        List<PublishedExtension> extensionList = new List<PublishedExtension>();
        extensionList.Add(extension);
        int num = string.IsNullOrEmpty(version) ? 1 : 0;
        searchHelper.UpdateSearchIndex(requestContext2, extensionList, num != 0);
      }
      catch (Exception ex)
      {
        string format = (string.IsNullOrEmpty(version) ? "Failed to delete extension from the search index." : "Failed to update extension in search index after an extension version was deleted.") + string.Format("Extension id: {0}, Extension: {1}.{2}", (object) extension.ExtensionId, (object) extension.Publisher.PublisherName, (object) extension.ExtensionName) + "ExceptionMessage: " + ex.Message + ", ExceptionStack: " + ex.StackTrace;
        requestContext.TraceAlways(12060103, TraceLevel.Error, "gallery", nameof (DeleteExtension), format);
      }
      IVssRequestContext requestContext3 = requestContext;
      PublishedExtension extension1 = extension;
      List<InstallationTarget> installationTargets = extension.InstallationTargets;
      string str1 = version;
      TagType? tagType = new TagType?();
      string version1 = str1;
      PublishedExtensionService.PublishCustomerIntelligenceEvent(requestContext3, extension1, "Deleted", installationTargets: (IEnumerable<InstallationTarget>) installationTargets, tagType: tagType, logTelemetryForOnPremise: true, version: version1);
      if (!extension.IsVsCodeExtension() || !requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnablePMPIntegration") || !requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnablePMPMutationsIntegration"))
        return;
      if (!requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnablePMPHardDeleteMutation"))
        return;
      try
      {
        if (string.IsNullOrEmpty(version))
          TaskExtensions.SyncResult(requestContext.GetService<IPackageManagementPlatformServiceFacade>().DeletePackageAggregate(requestContext, "vscode", extension));
        else
          TaskExtensions.SyncResult(requestContext.GetService<IPackageManagementPlatformServiceFacade>().DeletePackage(requestContext, "vscode", extension, version));
      }
      catch (Exception ex)
      {
        if (string.IsNullOrEmpty(version))
          requestContext.TraceException(12062089, "Gallery", "Failed to remove extension from PMP", ex);
        else
          requestContext.TraceException(12062089, "Gallery", "Failed to remove extension version from PMP", ex);
      }
      if (!requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableVSCodeExtensionMutationEventsPublishing"))
        return;
      IExtensionNameConflictService service1 = requestContext.GetService<IExtensionNameConflictService>();
      string extensionName1 = extension.ExtensionName;
      string extensionName2 = extension.Publisher.PublisherName + "." + extension.ExtensionName;
      string existsInConflictList = service1.GetNewExtensionNameIfExistsInConflictList(extensionName2);
      if (!string.IsNullOrEmpty(existsInConflictList))
        extensionName1 = existsInConflictList;
      IEventPublisherService service2 = requestContext.GetService<IEventPublisherService>();
      if (string.IsNullOrWhiteSpace(version))
        service2.PublishPackageAggregateDeletedEvent(requestContext, extensionName1);
      else
        service2.PublishPackageDeletedEvent(requestContext, extensionName1, version);
    }

    public PublishedExtension QueryExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string version,
      ExtensionQueryFlags flags,
      string accountToken,
      bool useCache = false)
    {
      GalleryUtil.CheckPublisherName(publisherName);
      GalleryUtil.CheckExtensionName(extensionName);
      flags = this.ProcessQueryFlags(flags);
      ExtensionQueryFlags flags1 = flags | ExtensionQueryFlags.IncludeSharedAccounts | ExtensionQueryFlags.IncludeSharedOrganizations;
      PublishedExtension extension;
      if (useCache && requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.QueryExtensionCache"))
      {
        extension = requestContext.GetService<QueryExtensionCacheService>().QueryExtension(requestContext, publisherName, extensionName, version, Guid.Empty, flags1);
        if (extension == null || extension.Versions == null || extension.Versions.Count == 0)
        {
          requestContext.TraceAlways(12062074, TraceLevel.Error, "gallery", "QueryExtensionCache", string.Format("Extension {0}.{1} version {2} not found in cache. queryFlags:{3}, ", (object) publisherName, (object) extensionName, (object) version, (object) flags1) + string.Format("useCache:{0}, accountTokenIsNullOrEmpty:{1}", (object) useCache, (object) string.IsNullOrEmpty(accountToken)));
          throw new ExtensionDoesNotExistException(GalleryResources.ExtensionDoesNotExistInCache((object) publisherName, (object) extensionName, (object) version));
        }
      }
      else
      {
        using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
          extension = component.QueryExtension(publisherName, extensionName, version, Guid.Empty, flags1);
      }
      if (!GallerySecurity.HasExtensionPermission(requestContext, extension, accountToken, PublisherPermissions.PrivateRead, true))
        GallerySecurity.CheckExtensionPermission(requestContext, extension, accountToken, PublisherPermissions.Read, true);
      if (flags.HasFlag((Enum) ExtensionQueryFlags.IncludeVersions) && flags.HasFlag((Enum) ExtensionQueryFlags.IncludeVersionProperties))
        this.ReadExtensionProperties(requestContext, extension);
      this.PrepareExtensionForClient(requestContext, extension, flags, accountToken);
      return extension;
    }

    public List<ExtensionShare> GetAccountsSharedWithUser(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      Guid userId)
    {
      if (extension.SharedWith == null)
        return (List<ExtensionShare>) null;
      ISubscriptionService service = requestContext.GetService<ISubscriptionService>();
      IEnumerable<ISubscriptionAccount> subscriptionAccounts;
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.CallNewGetAccountsVersionAPI"))
        subscriptionAccounts = (IEnumerable<ISubscriptionAccount>) requestContext.GetClient<SubscriptionHttpClient>().GetAccountsByIdentityForOfferIdV2Async(AccountProviderNamespace.VisualStudioOnline, userId, false, false, true, (IEnumerable<Guid>) new Guid[3]
        {
          ServiceInstanceTypes.TFS,
          ServiceInstanceTypes.SPS,
          CommerceServiceInstanceTypes.Commerce
        }, extension.Publisher.PublisherName + "." + extension.ExtensionName, new bool?(false), new bool?(true)).SyncResult<List<SubscriptionAccount>>();
      else
        subscriptionAccounts = service.GetAccounts(requestContext, AccountProviderNamespace.VisualStudioOnline, new Guid?(userId), false, includeMSAAccounts: true, serviceOwners: (IEnumerable<Guid>) new Guid[2]
        {
          ServiceInstanceTypes.TFS,
          ServiceInstanceTypes.SPS
        }, galleryId: extension.Publisher.PublisherName + "." + extension.ExtensionName, queryAccountsByUpn: true);
      HashSet<Guid> guidSet = new HashSet<Guid>();
      HashSet<Guid> shareWithOrgHash = new HashSet<Guid>();
      foreach (ExtensionShare extensionShare in extension.SharedWith)
      {
        Guid hostId;
        if (Guid.TryParse(extensionShare.Id, out hostId))
        {
          if (extensionShare.Type == "account")
            guidSet.Add(hostId);
          else if (extensionShare.Type == "organization")
          {
            if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableTenantAsEnterpriseBoundary"))
            {
              IList<Guid> userAccounts = GalleryOrganizationHelper.GetAccountsForUserInMicrosoftTenant(requestContext, hostId, userId);
              requestContext.TraceConditionally(12061132, TraceLevel.Info, "gallery", nameof (GetAccountsSharedWithUser), (Func<string>) (() => string.Format("For user {0} with extension {1}, enterprise id {2}, UAM as {3}", (object) userId, (object) extension.ExtensionName, (object) hostId, (object) string.Join<Guid>(",", (IEnumerable<Guid>) userAccounts))));
              shareWithOrgHash.AddRange<Guid, HashSet<Guid>>((IEnumerable<Guid>) userAccounts);
            }
            else
            {
              IList<Microsoft.VisualStudio.Services.Organization.Client.Collection> collectionsForOrg = GalleryOrganizationHelper.GetCollectionsForOrg(requestContext, hostId);
              if (collectionsForOrg != null)
                collectionsForOrg.ForEach<Microsoft.VisualStudio.Services.Organization.Client.Collection>(closure_0 ?? (closure_0 = (Action<Microsoft.VisualStudio.Services.Organization.Client.Collection>) (collection => shareWithOrgHash.Add(collection.Id))));
            }
          }
        }
      }
      List<ExtensionShare> accountsSharedWithUser = new List<ExtensionShare>();
      foreach (ISubscriptionAccount subscriptionAccount in subscriptionAccounts)
      {
        if (guidSet.Contains(subscriptionAccount.AccountId))
          accountsSharedWithUser.Add(new ExtensionShare()
          {
            Id = subscriptionAccount.AccountId.ToString(),
            Type = "account",
            Name = subscriptionAccount.AccountName
          });
        else if (shareWithOrgHash.Contains(subscriptionAccount.AccountId))
          accountsSharedWithUser.Add(new ExtensionShare()
          {
            Id = subscriptionAccount.AccountId.ToString(),
            Type = "account",
            Name = subscriptionAccount.AccountName,
            IsOrg = new bool?(true)
          });
      }
      return accountsSharedWithUser;
    }

    private static bool IsRequestFromChinaRegion(IVssRequestContext requestContext)
    {
      string requestCountryCode = requestContext.GetService<IGeoLocationService>().GetRequestCountryCode(requestContext);
      return !string.IsNullOrWhiteSpace(requestCountryCode) && requestCountryCode.Equals("CN", StringComparison.OrdinalIgnoreCase);
    }

    public ExtensionQueryResult SearchExtensions(
      IVssRequestContext requestContext,
      ExtensionQuery extensionQuery,
      string accountToken,
      SearchOverrideFlags searchOverrides = SearchOverrideFlags.None,
      IEnumerable<MetadataFilterItem> metadataFilterItems = null)
    {
      bool isRequestFromChinaRegion = PublishedExtensionService.IsRequestFromChinaRegion(requestContext);
      Func<ExtensionQueryResult> run = (Func<ExtensionQueryResult>) (() => this.m_searchServiceFacade.GetSearchResults(extensionQuery, requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/SearchService/SearchServiceEndpoint", (string) null), requestContext, isRequestFromChinaRegion));
      Func<ExtensionQueryResult> func = (Func<ExtensionQueryResult>) (() =>
      {
        bool useNewViews = false;
        if (extensionQuery.Filters.Count != 1)
          throw new NotSupportedException("There should be exactly one Filter for search query");
        Stopwatch stopwatch = Stopwatch.StartNew();
        Stopwatch componentStopwatch = (Stopwatch) null;
        QueryFilter filter = extensionQuery.Filters[0];
        ExtensionSearchResult extensionSearchResult = (ExtensionSearchResult) null;
        int num1 = 0;
        int num2 = 0;
        int num3 = 0;
        int num4 = 0;
        int num5 = 0;
        int num6 = 0;
        ExtensionSearchParams searchParams = new ExtensionSearchParams();
        searchParams.CriteriaList = (IList<SearchCriteria>) new List<SearchCriteria>();
        searchParams.PageSize = PublishedExtensionService.GetValidPageSize(requestContext, filter);
        searchParams.PageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber;
        searchParams.SortBy = this.GetValidSortByType(filter.SortBy);
        searchParams.SortOrder = this.GetValidSortOrderType(filter.SortOrder);
        searchParams.MetadataFlags = (ExtensionQueryResultMetadataFlags) ((int) extensionQuery.MetadataFlags ?? (int) PublishedExtensionService.GetDefaultMetadataFlags());
        searchParams.Product = GalleryUtil.GetProductTypeFromExtensionQuery(extensionQuery);
        bool flag1 = false;
        List<string> categoryStrings1 = new List<string>();
        List<SearchCriteria> enumerable1 = (List<SearchCriteria>) null;
        List<InstallationTarget> installationTargets = new List<InstallationTarget>();
        if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.UseNewViewsForSearch"))
          useNewViews = true;
        foreach (FilterCriteria criterion in filter.Criteria)
        {
          ExtensionQueryFilterType filterType = (ExtensionQueryFilterType) criterion.FilterType;
          SearchFilterOperatorType operatorType = SearchFilterOperatorType.And;
          string str;
          switch (filterType)
          {
            case ExtensionQueryFilterType.Tag:
              str = criterion.Value;
              break;
            case ExtensionQueryFilterType.Category:
              categoryStrings1.Add(criterion.Value);
              continue;
            case ExtensionQueryFilterType.InstallationTarget:
              str = criterion.Value;
              installationTargets.Add(new InstallationTarget()
              {
                Target = criterion.Value
              });
              break;
            case ExtensionQueryFilterType.SearchText:
              if (++num1 != 1)
                this.FailInvalidQuery(GalleryResources.InvalidExtensionQuery((object) GalleryResources.MustSupplySingleQueryValue((object) filterType.ToString())));
              searchParams.RawQuery = criterion.Value;
              enumerable1 = this.searchQueryParser.Parse(requestContext, criterion.Value);
              if (enumerable1.IsNullOrEmpty<SearchCriteria>())
              {
                flag1 = true;
                continue;
              }
              continue;
            case ExtensionQueryFilterType.ExcludeWithFlags:
              if (++num2 != 1)
                this.FailInvalidQuery(GalleryResources.InvalidExtensionQuery((object) GalleryResources.MustSupplySingleQueryValue((object) filterType.ToString())));
              int result1;
              if (int.TryParse(criterion.Value, out result1))
              {
                str = result1.ToString((IFormatProvider) CultureInfo.InvariantCulture);
                break;
              }
              continue;
            case ExtensionQueryFilterType.IncludeWithFlags:
              if (++num3 != 1)
                this.FailInvalidQuery(GalleryResources.InvalidExtensionQuery((object) GalleryResources.MustSupplySingleQueryValue((object) filterType.ToString())));
              int result2;
              if (int.TryParse(criterion.Value, out result2))
              {
                str = result2.ToString((IFormatProvider) CultureInfo.InvariantCulture);
                break;
              }
              continue;
            case ExtensionQueryFilterType.Lcid:
              if (int.TryParse(criterion.Value, out int _))
              {
                str = criterion.Value;
                break;
              }
              continue;
            case ExtensionQueryFilterType.InstallationTargetVersion:
              if (++num4 != 1)
                this.FailInvalidQuery(GalleryResources.InvalidExtensionQuery((object) GalleryResources.MustSupplySingleQueryValue((object) filterType.ToString())));
              if (GalleryServerUtil.IsValidVersion(criterion.Value.Trim()))
              {
                str = criterion.Value.Trim();
                break;
              }
              continue;
            case ExtensionQueryFilterType.InstallationTargetVersionRange:
              if (++num5 != 1)
                this.FailInvalidQuery(GalleryResources.InvalidExtensionQuery((object) GalleryResources.MustSupplySingleQueryValue((object) filterType.ToString())));
              if (string.IsNullOrEmpty(criterion.Value) || !criterion.Value.Contains("-"))
                this.FailInvalidQuery(GalleryResources.InvalidExtensionQuery((object) GalleryResources.InvalidVersionFormat((object) criterion.Value)));
              string[] source = criterion.Value.Split(new string[1]
              {
                "-"
              }, StringSplitOptions.RemoveEmptyEntries);
              if (((IEnumerable<string>) source).Count<string>() == 2 && GalleryServerUtil.IsValidVersion(source[0]) && GalleryServerUtil.IsValidVersion(source[1]))
              {
                str = criterion.Value.Trim();
                break;
              }
              continue;
            case ExtensionQueryFilterType.PublisherDisplayName:
              str = criterion.Value;
              break;
            case ExtensionQueryFilterType.IncludeWithPublisherFlags:
              if (++num6 != 1)
                this.FailInvalidQuery(GalleryResources.InvalidExtensionQuery((object) GalleryResources.MustSupplySingleQueryValue((object) filterType.ToString())));
              int result3;
              if (int.TryParse(criterion.Value, out result3))
              {
                str = result3.ToString((IFormatProvider) CultureInfo.InvariantCulture);
                break;
              }
              continue;
            case ExtensionQueryFilterType.OrganizationSharedWith:
              if (requestContext?.UserContext != (IdentityDescriptor) null && !ServicePrincipals.IsServicePrincipal(requestContext, requestContext.UserContext))
              {
                List<Guid> enumerable2;
                if (string.Equals(criterion.Value, "Me", StringComparison.InvariantCultureIgnoreCase))
                  enumerable2 = GalleryOrganizationHelper.GetOrganizationIdsForIdentity(requestContext, requestContext.GetAuthenticatedIdentity());
                else
                  enumerable2 = new List<Guid>()
                  {
                    Guid.Parse(criterion.Value)
                  };
                if (!enumerable2.IsNullOrEmpty<Guid>())
                {
                  enumerable2.ForEach((Action<Guid>) (orgId => searchParams.CriteriaList.Add(new SearchCriteria()
                  {
                    FilterType = this.FilterTypeMap[filterType],
                    FilterValue = orgId.ToString(),
                    OperatorType = operatorType
                  })));
                  searchOverrides |= SearchOverrideFlags.IncludePrivate;
                  continue;
                }
                continue;
              }
              continue;
            case ExtensionQueryFilterType.ProductArchitecture:
              str = criterion.Value;
              break;
            case ExtensionQueryFilterType.TargetPlatform:
              str = criterion.Value;
              break;
            default:
              throw new ArgumentException();
          }
          searchParams.CriteriaList.Add(new SearchCriteria()
          {
            FilterType = this.FilterTypeMap[filterType],
            FilterValue = str,
            OperatorType = operatorType
          });
        }
        if (metadataFilterItems != null)
        {
          foreach (MetadataFilterItem metadataFilterItem in metadataFilterItems)
            searchParams.CriteriaList.Add(new SearchCriteria()
            {
              FilterType = SearchFilterType.Metadata,
              FilterValue = metadataFilterItem.Key + ":" + metadataFilterItem.Value,
              OperatorType = metadataFilterItem.Operator == MetadataFilterItem.ComparisonOperator.Equal ? SearchFilterOperatorType.Equal : SearchFilterOperatorType.NotEqual
            });
        }
        if ("vscode".Equals(searchParams.Product))
          searchParams.MetadataFlags |= ExtensionQueryResultMetadataFlags.IncludeTargetPlatforms;
        List<string> categoryStrings2 = new List<string>();
        foreach (SearchCriteria searchCriteria in enumerable1)
        {
          if (searchCriteria.FilterType == SearchFilterType.Category)
          {
            if (searchCriteria.OperatorType == SearchFilterOperatorType.Or)
              categoryStrings2.Add(searchCriteria.FilterValue);
            else if (searchCriteria.OperatorType == SearchFilterOperatorType.And)
              categoryStrings1.Add(searchCriteria.FilterValue);
          }
          else
          {
            if (searchCriteria.FilterType == SearchFilterType.InstallationTarget)
              installationTargets.Add(new InstallationTarget()
              {
                Target = searchCriteria.FilterValue
              });
            searchParams.CriteriaList.Add(searchCriteria);
          }
        }
        bool flag2 = this.m_ExternalSearchService.IsExternalSearchEnabled(requestContext, installationTargets, searchOverrides);
        if (flag2)
          searchOverrides |= SearchOverrideFlags.DoNotTranslateCategoryFilter;
        if (!flag1 && categoryStrings2.Count > 0)
        {
          foreach (KeyValuePair<int, string> namesForCategory in (IEnumerable<KeyValuePair<int, string>>) this.GetInternalNamesForCategories(requestContext, categoryStrings2, searchOverrides))
            searchParams.CriteriaList.Add(new SearchCriteria()
            {
              FilterType = this.FilterTypeMap[ExtensionQueryFilterType.Category],
              FilterValue = namesForCategory.Value,
              OperatorType = SearchFilterOperatorType.Or
            });
        }
        if (!flag1 && categoryStrings1.Count > 0)
        {
          IList<KeyValuePair<int, string>> namesForCategories = this.GetInternalNamesForCategories(requestContext, categoryStrings1, searchOverrides);
          if (namesForCategories.IsNullOrEmpty<KeyValuePair<int, string>>())
          {
            flag1 = true;
          }
          else
          {
            foreach (KeyValuePair<int, string> keyValuePair in (IEnumerable<KeyValuePair<int, string>>) namesForCategories)
              searchParams.CriteriaList.Add(new SearchCriteria()
              {
                FilterType = this.FilterTypeMap[ExtensionQueryFilterType.Category],
                FilterValue = keyValuePair.Value,
                OperatorType = SearchFilterOperatorType.And
              });
          }
        }
        if (!flag1)
        {
          if (searchOverrides.HasFlag((Enum) SearchOverrideFlags.IncludePrivate))
            searchParams.CriteriaList.Add(new SearchCriteria()
            {
              FilterType = SearchFilterType.IncludePrivateExtensions,
              FilterValue = "1",
              OperatorType = SearchFilterOperatorType.And
            });
          if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.DoNotUseInternalNameForSearch"))
            searchParams.FeatureFlags |= SearchFeatureFlags.DoNotUseInternalNameForSearch;
          if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.UseRatingsDownloadsForRelevance"))
            searchParams.FeatureFlags |= SearchFeatureFlags.UseRatingsDownloadsForRelevance;
          if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.UseNewRelevanceForVSTS"))
            searchParams.FeatureFlags |= SearchFeatureFlags.UseNewRelevanceForVSTS;
          if (searchOverrides.HasFlag((Enum) SearchOverrideFlags.IncludePrivate))
            useNewViews = false;
          extensionQuery.Flags = this.ProcessQueryFlags(extensionQuery.Flags);
          ExtensionQueryFlags extensionQueryFlags = extensionQuery.Flags | ExtensionQueryFlags.IncludeSharedAccounts | ExtensionQueryFlags.IncludeSharedOrganizations;
          if (flag2)
          {
            componentStopwatch = Stopwatch.StartNew();
            if (this.m_ExternalSearchService == null)
              this.m_ExternalSearchService = requestContext.GetService<ISearchService>();
            ExtensionQueryResult extensionQueryResult = this.m_ExternalSearchService.Search(requestContext, searchParams, extensionQueryFlags);
            extensionSearchResult = new ExtensionSearchResult()
            {
              Results = extensionQueryResult != null ? extensionQueryResult.Results[0].Extensions : new List<PublishedExtension>(),
              ResultMetadata = extensionQueryResult != null ? extensionQueryResult.Results[0].ResultMetadata : new List<ExtensionFilterResultMetadata>()
            };
            componentStopwatch.Stop();
          }
          else
          {
            using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
            {
              PublishedExtensionComponent29 extensionComponent29 = component as PublishedExtensionComponent29;
              if (extensionComponent29 != null & useNewViews)
              {
                componentStopwatch = Stopwatch.StartNew();
                extensionSearchResult = extensionComponent29.SearchExtensionsWithViews(searchParams, extensionQueryFlags);
                componentStopwatch.Stop();
              }
              else
              {
                componentStopwatch = Stopwatch.StartNew();
                extensionSearchResult = component.SearchExtensions(searchParams, extensionQueryFlags);
                componentStopwatch.Stop();
              }
            }
          }
        }
        ExtensionFilterResult extensionResults = new ExtensionFilterResult();
        extensionResults.Extensions = extensionSearchResult != null ? extensionSearchResult.Results : new List<PublishedExtension>();
        extensionResults.ResultMetadata = extensionSearchResult?.ResultMetadata;
        if (!flag2)
          this.PrepareMetadataForClient(requestContext, extensionResults.ResultMetadata, searchOverrides);
        ExtensionQueryResult queryResult = new ExtensionQueryResult()
        {
          Results = new List<ExtensionFilterResult>()
        };
        queryResult.Results.Add(extensionResults);
        this.FilterExtensionsPostDBQuery(extensionQuery, requestContext, queryResult, accountToken, externalSearchUsed: flag2);
        this.FilterAssets(extensionQuery, queryResult);
        stopwatch.Stop();
        PublishedExtensionService.PublishCustomerIntelligenceEventForSearch(requestContext, searchParams, categoryStrings1, extensionResults, stopwatch, componentStopwatch, extensionQuery, metadataFilterItems, useNewViews, flag2, isRequestFromChinaRegion);
        return queryResult;
      });
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableSearchTrafficToNewService") && PublishedExtensionService.IsAzureSearchRequired(requestContext, searchOverrides) && GalleryServerUtil.IsVsCodeExtensionRequest(extensionQuery) && PublishedExtensionService.ShouldSearchRequestRedirectToNewService(requestContext, isRequestFromChinaRegion))
      {
        CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Marketplace.").AndCommandKey((CommandKey) "NewSearchExtensionsService").AndCommandPropertiesDefaults(PublishedExtensionService.NewSearchExtensionsCircuitBreakerSettings);
        return new CommandService<ExtensionQueryResult>(requestContext, setter, run, func).Execute();
      }
      CommandSetter setter1 = CommandSetter.WithGroupKey((CommandGroupKey) "Marketplace.").AndCommandKey((CommandKey) nameof (SearchExtensions)).AndCommandPropertiesDefaults(PublishedExtensionService.SearchExtensionsCircuitBreakerSettings);
      return new CommandService<ExtensionQueryResult>(requestContext, setter1, func).Execute();
    }

    private static bool IsAzureSearchRequired(
      IVssRequestContext requestContext,
      SearchOverrideFlags searchOverride)
    {
      return !requestContext.ExecutionEnvironment.IsOnPremisesDeployment && !searchOverride.HasFlag((Enum) SearchOverrideFlags.UseDbForSearch);
    }

    private static bool ShouldSearchRequestRedirectToNewService(
      IVssRequestContext requestContext,
      bool isRequestFromChinaRegion)
    {
      int num1 = RandomNumberGeneratorUtility.Next();
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      if (isRequestFromChinaRegion)
      {
        int num2 = service.GetValue<int>(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/SearchService/PercentageOfChinaSearchTrafficToRedirect", 0);
        return num1 < num2;
      }
      int num3 = service.GetValue<int>(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/SearchService/PercentageofSearchTrafficToRedirect", 100);
      return num1 < num3;
    }

    private IList<KeyValuePair<int, string>> GetInternalNamesForCategories(
      IVssRequestContext requestContext,
      List<string> categoryStrings,
      SearchOverrideFlags searchOverrideFlags)
    {
      categoryStrings = categoryStrings.Select<string, string>((Func<string, string>) (x => !GalleryServerUtil.c_oldCategoryToVerticalCategoryMapping.ContainsKey(x) ? x : x.Replace(x, GalleryServerUtil.c_oldCategoryToVerticalCategoryMapping[x]))).ToList<string>();
      IList<KeyValuePair<int, string>> namesForCategories;
      if (searchOverrideFlags.HasFlag((Enum) SearchOverrideFlags.DoNotTranslateCategoryFilter))
      {
        namesForCategories = (IList<KeyValuePair<int, string>>) new List<KeyValuePair<int, string>>();
        foreach (string categoryString in categoryStrings)
        {
          string str = string.Empty;
          if (!GalleryServerUtil.c_oldCategoryToVerticalCategoryMapping.TryGetValue(categoryString, out str))
            str = categoryString;
          namesForCategories.Add(new KeyValuePair<int, string>(5, str));
        }
      }
      else
        namesForCategories = (IList<KeyValuePair<int, string>>) this.ConvertCategoryNamesToTags(requestContext, (IEnumerable<string>) categoryStrings, "en-us", (string) null, true);
      return namesForCategories;
    }

    private void PrepareMetadataForClient(
      IVssRequestContext requestContext,
      List<ExtensionFilterResultMetadata> resultMetadataList,
      SearchOverrideFlags searchOverrides)
    {
      if (resultMetadataList == null || searchOverrides.HasFlag((Enum) SearchOverrideFlags.DoNotTranslateCategoryFilter))
        return;
      foreach (ExtensionFilterResultMetadata resultMetadata in resultMetadataList)
      {
        if (resultMetadata.MetadataType.Equals(QueryMetadataConstants.Categories, StringComparison.OrdinalIgnoreCase) || resultMetadata.MetadataType.Equals(QueryMetadataConstants.ResultSetCategories, StringComparison.OrdinalIgnoreCase))
        {
          List<string> categories = new List<string>();
          foreach (MetadataItem metadataItem in resultMetadata.MetadataItems)
          {
            if (!string.IsNullOrEmpty(metadataItem.Name))
              categories.Add(metadataItem.Name);
          }
          List<string> names = this.ConvertCategoryIdsToNames(requestContext, (IEnumerable<string>) categories, "en-us", false);
          int index1 = 0;
          int index2 = 0;
          while (index1 < resultMetadata.MetadataItems.Count)
          {
            if (!string.IsNullOrEmpty(resultMetadata.MetadataItems[index1].Name))
            {
              if (names[index2].IsNullOrEmpty<char>())
              {
                resultMetadata.MetadataItems.RemoveAt(index1);
                ++index2;
                continue;
              }
              resultMetadata.MetadataItems[index1].Name = names[index2];
              ++index2;
            }
            ++index1;
          }
        }
      }
    }

    private static void PublishCustomerIntelligenceEventForSearch(
      IVssRequestContext requestContext,
      ExtensionSearchParams searchParams,
      List<string> categoryStrings,
      ExtensionFilterResult extensionResults,
      Stopwatch stopwatch,
      Stopwatch componentStopwatch,
      ExtensionQuery extensionQuery,
      IEnumerable<MetadataFilterItem> metadataFilterItems,
      bool useNewViews,
      bool useExternalSearch,
      bool isRequestFromChinaRegion)
    {
      ClientTraceData clientTraceData = new ClientTraceData();
      clientTraceData.Add(CustomerIntelligenceProperty.Action, (object) "Searched");
      List<string> stringList1 = new List<string>();
      List<string> stringList2 = new List<string>();
      string rawQuery = searchParams.RawQuery;
      foreach (SearchCriteria criteria in (IEnumerable<SearchCriteria>) searchParams.CriteriaList)
      {
        switch (criteria.FilterType)
        {
          case SearchFilterType.InstallationTarget:
            stringList1.Add(criteria.FilterValue);
            continue;
          case SearchFilterType.Tag:
            stringList2.Add(criteria.FilterValue);
            continue;
          default:
            continue;
        }
      }
      clientTraceData.Add("Query", (object) rawQuery);
      clientTraceData.Add("PageNumber", (object) searchParams.PageNumber);
      clientTraceData.Add("PageSize", (object) searchParams.PageSize);
      if (extensionResults != null)
      {
        int num = int.MinValue;
        if (extensionResults.ResultMetadata != null)
        {
          foreach (ExtensionFilterResultMetadata filterResultMetadata in extensionResults.ResultMetadata)
          {
            if (filterResultMetadata.MetadataType.Equals(QueryMetadataConstants.ResultCount, StringComparison.OrdinalIgnoreCase))
            {
              using (List<MetadataItem>.Enumerator enumerator = filterResultMetadata.MetadataItems.GetEnumerator())
              {
                while (enumerator.MoveNext())
                {
                  MetadataItem current = enumerator.Current;
                  if (current.Name.Equals(QueryMetadataConstants.TotalCount, StringComparison.OrdinalIgnoreCase))
                  {
                    num = current.Count;
                    break;
                  }
                }
                break;
              }
            }
          }
        }
        if (num == int.MinValue)
          num = extensionResults.Extensions.Count;
        clientTraceData.Add("TotalCount", (object) num);
        clientTraceData.Add("ResultCount", (object) extensionResults.Extensions.Count);
      }
      if (!stringList1.IsNullOrEmpty<string>())
      {
        string str = string.Join(";", stringList1.Select<string, string>((Func<string, string>) (x => x ?? string.Empty)));
        clientTraceData.Add("InstallationTargets", (object) str);
      }
      if (!categoryStrings.IsNullOrEmpty<string>())
      {
        string str = string.Join(";", categoryStrings.Select<string, string>((Func<string, string>) (x => x ?? string.Empty)));
        clientTraceData.Add("Categories", (object) str);
      }
      if (!stringList2.IsNullOrEmpty<string>())
      {
        string str = string.Join(";", stringList2.Select<string, string>((Func<string, string>) (x => x ?? string.Empty)));
        clientTraceData.Add("Tags", (object) str);
      }
      if (!metadataFilterItems.IsNullOrEmpty<MetadataFilterItem>())
      {
        string str = string.Join(";", metadataFilterItems.Select<MetadataFilterItem, string>((Func<MetadataFilterItem, string>) (x => x == null ? string.Empty : x.Key + "=" + x.Value)));
        clientTraceData.Add("VsixMetadataFilter", (object) str);
      }
      clientTraceData.AddGalleryUserIdentifier(requestContext);
      clientTraceData.Add("SearchDuration", (object) stopwatch.ElapsedMilliseconds);
      clientTraceData.Add("UseNewViews", (object) useNewViews);
      clientTraceData.Add("IsRequestFromChinaRegion", (object) isRequestFromChinaRegion);
      if (componentStopwatch != null)
        clientTraceData.Add("SearchComponentDuration", (object) componentStopwatch.ElapsedMilliseconds);
      else
        clientTraceData.Add("SearchComponentDuration", (object) null);
      try
      {
        clientTraceData.Add("ExtensionQuery", (object) JsonConvert.SerializeObject((object) extensionQuery, new JsonSerializerSettings()
        {
          ContractResolver = (IContractResolver) new CamelCasePropertyNamesContractResolver()
        }));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12062047, "gallery", "LogClientTraceForSearchExtension", ex);
      }
      clientTraceData.Add("ExternalSearchUsed", (object) useExternalSearch);
      requestContext.GetService<ClientTraceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", "Extension", clientTraceData);
    }

    public PublishedExtension QueryExtensionById(
      IVssRequestContext requestContext,
      Guid extensionId,
      string version,
      ExtensionQueryFlags flags,
      Guid validationId)
    {
      flags = this.ProcessQueryFlags(flags);
      PublishedExtension extension;
      using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
        extension = component.QueryExtensionById(extensionId, version, validationId, flags);
      if (!GallerySecurity.HasExtensionPermission(requestContext, extension, (string) null, PublisherPermissions.PrivateRead, false))
        GallerySecurity.CheckExtensionPermission(requestContext, extension, (string) null, PublisherPermissions.Read, false);
      if (flags.HasFlag((Enum) ExtensionQueryFlags.IncludeVersionProperties))
        this.ReadExtensionProperties(requestContext, extension);
      this.PrepareExtensionForClient(requestContext, extension, flags);
      return extension;
    }

    public ExtensionsByInstallationTargetsResult QueryExtensionsForCacheBuilding(
      IVssRequestContext requestContext,
      List<InstallationTarget> targets,
      ExtensionQueryFlags flags,
      int pageNumber,
      int pageSize)
    {
      ExtensionsByInstallationTargetsResult installationTargetsResult = (ExtensionsByInstallationTargetsResult) null;
      using (PublishedExtensionComponent30 component = (PublishedExtensionComponent30) requestContext.CreateComponent<PublishedExtensionComponent>())
      {
        if (component != null)
          installationTargetsResult = component.QueryExtensionsForCacheBuilding(targets, pageNumber, pageSize, flags);
      }
      if (installationTargetsResult != null)
      {
        int? count = installationTargetsResult.Extensions?.Count;
        int num = 0;
        if (count.GetValueOrDefault() > num & count.HasValue)
        {
          foreach (PublishedExtension extension in installationTargetsResult.Extensions)
          {
            this.PrepareExtensionForClient(requestContext, extension, flags, filterInternalTags: false);
            this.ReadExtensionProperties(requestContext, extension);
          }
        }
      }
      return installationTargetsResult;
    }

    public static PublishedExtension GetExtensionByPublisherNameAndExtensionName(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName)
    {
      ExtensionQueryFlags extensionQueryFlags = ExtensionQueryFlags.ExcludeNonValidated | ExtensionQueryFlags.IncludeInstallationTargets | ExtensionQueryFlags.IncludeLatestVersionOnly;
      IPublishedExtensionService service = requestContext.GetService<IPublishedExtensionService>();
      bool flag = requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.QueryExtensionCacheExtended");
      IVssRequestContext requestContext1 = requestContext;
      string publisherName1 = publisherName;
      string extensionName1 = extensionName;
      int flags = (int) extensionQueryFlags;
      int num = flag ? 1 : 0;
      return service.QueryExtension(requestContext1, publisherName1, extensionName1, (string) null, (ExtensionQueryFlags) flags, (string) null, num != 0);
    }

    public ExtensionQueryResult QueryExtensions(
      IVssRequestContext requestContext,
      ExtensionQuery extensionQuery,
      string accountToken,
      Uri referrer = null,
      bool isCacheBuildCall = false)
    {
      return this.QueryExtensions(requestContext, extensionQuery, accountToken, false, referrer, isCacheBuildCall);
    }

    public ExtensionQueryResult QueryExtensions(
      IVssRequestContext requestContext,
      ExtensionQuery extensionQuery,
      string accountToken,
      bool includeLatestAndValidatedVersionsOnlyFlag,
      Uri referrer = null,
      bool isCacheBuildCall = false)
    {
      Stopwatch componentStopwatch = (Stopwatch) null;
      Stopwatch stopwatch = Stopwatch.StartNew();
      ExtensionQueryResult queryResult = (ExtensionQueryResult) null;
      string accountToken1 = GallerySecurity.ParseAccountToken(requestContext, accountToken);
      bool flag1 = false;
      bool flag2 = false;
      bool flag3 = false;
      string empty = string.Empty;
      bool flag4 = PublishedExtensionService.CanUseInMemoryCache(requestContext);
      List<QueryFilterValue> filterValues = new List<QueryFilterValue>();
      for (int index1 = 0; index1 < extensionQuery.Filters.Count; ++index1)
      {
        QueryFilter queryFilter = extensionQuery.Filters[index1];
        queryFilter.QueryIndex = index1;
        int num1 = 0;
        int num2 = 0;
        int num3 = 0;
        flag4 &= !queryFilter.DoNotUseCache;
        queryFilter.PageSize = PublishedExtensionService.GetValidPageSize(requestContext, queryFilter);
        queryFilter.PageNumber = queryFilter.PageNumber < 1 ? 1 : queryFilter.PageNumber;
        queryFilter.SortBy = this.GetValidSortByType(queryFilter.SortBy);
        queryFilter.SortOrder = this.GetValidSortOrderType(queryFilter.SortOrder);
        if (queryFilter.SortBy == 0)
          queryFilter.SortBy = 4;
        for (int index2 = 0; index2 < queryFilter.Criteria.Count; ++index2)
        {
          FilterCriteria criterion = queryFilter.Criteria[index2];
          ExtensionQueryFilterType filterType = (ExtensionQueryFilterType) criterion.FilterType;
          string str;
          switch (filterType)
          {
            case ExtensionQueryFilterType.Tag:
            case ExtensionQueryFilterType.DisplayName:
            case ExtensionQueryFilterType.ContributionType:
            case ExtensionQueryFilterType.FeaturedInCategory:
            case ExtensionQueryFilterType.PublisherName:
            case ExtensionQueryFilterType.PublisherDisplayName:
              str = criterion.Value;
              break;
            case ExtensionQueryFilterType.Private:
              if (++num1 != 1)
                this.FailInvalidQuery(GalleryResources.InvalidExtensionQuery((object) GalleryResources.MustSupplySingleQueryValue((object) filterType.ToString())));
              string accountToken2 = GallerySecurity.ParseAccountToken(requestContext, criterion.Value);
              if (!string.IsNullOrEmpty(accountToken1) && accountToken2 != accountToken1)
                this.FailInvalidQuery(GalleryResources.InvalidExtensionQuery((object) GalleryResources.QueryAccountTokenMismatch()));
              str = accountToken2;
              break;
            case ExtensionQueryFilterType.Id:
              if (!Guid.TryParse(criterion.Value, out Guid _))
                this.FailInvalidQuery(GalleryResources.InvalidExtensionQuery((object) GalleryResources.InvalidExtensionIdFormat((object) criterion.Value)));
              str = criterion.Value;
              break;
            case ExtensionQueryFilterType.Category:
              flag2 = true;
              string verticalAlignedCategory = PublishedExtensionService.ConvertOldVstsCategoryToVerticalAlignedCategory(requestContext, criterion.Value);
              str = this.ConvertCategoryNamesToTags(requestContext, (IEnumerable<string>) new string[1]
              {
                verticalAlignedCategory
              }, "en-us", (string) null, false)[0].Value;
              break;
            case ExtensionQueryFilterType.Name:
              if (criterion.Value == null || !criterion.Value.Contains("."))
                this.FailInvalidQuery(GalleryResources.InvalidExtensionQuery((object) GalleryResources.InvalidFullyQualifiedName()));
              str = criterion.Value;
              break;
            case ExtensionQueryFilterType.InstallationTarget:
            case ExtensionQueryFilterType.InstallationTargetVersion:
              flag1 = true;
              str = criterion.Value;
              break;
            case ExtensionQueryFilterType.Featured:
              str = "$featured";
              break;
            case ExtensionQueryFilterType.ExcludeWithFlags:
              if (++num2 != 1)
                this.FailInvalidQuery(GalleryResources.InvalidExtensionQuery((object) GalleryResources.MustSupplySingleQueryValue((object) filterType.ToString())));
              int result1;
              if (int.TryParse(criterion.Value, out result1))
              {
                str = result1.ToString();
                break;
              }
              continue;
            case ExtensionQueryFilterType.IncludeWithFlags:
              if (++num3 != 1)
                this.FailInvalidQuery(GalleryResources.InvalidExtensionQuery((object) GalleryResources.MustSupplySingleQueryValue((object) filterType.ToString())));
              int result2;
              if (int.TryParse(criterion.Value, out result2))
              {
                str = result2.ToString();
                break;
              }
              continue;
            case ExtensionQueryFilterType.OrganizationSharedWith:
              try
              {
                if (requestContext?.UserContext != (IdentityDescriptor) null)
                {
                  if (!ServicePrincipals.IsServicePrincipal(requestContext, requestContext.UserContext))
                  {
                    List<Guid> enumerable;
                    if (string.Equals(criterion.Value, "Me", StringComparison.InvariantCultureIgnoreCase))
                      enumerable = GalleryOrganizationHelper.GetOrganizationIdsForIdentity(requestContext, requestContext.GetAuthenticatedIdentity());
                    else
                      enumerable = new List<Guid>()
                      {
                        Guid.Parse(criterion.Value)
                      };
                    if (!enumerable.IsNullOrEmpty<Guid>())
                    {
                      enumerable.ForEach((Action<Guid>) (orgId => filterValues.Add(new QueryFilterValue()
                      {
                        QueryIndex = queryFilter.QueryIndex,
                        FilterIndex = 0,
                        FilterValueType = (int) filterType,
                        FilterValue = orgId.ToString()
                      })));
                      continue;
                    }
                    continue;
                  }
                  continue;
                }
                continue;
              }
              catch (VssServiceResponseException ex)
              {
                requestContext.TraceException(12061130, "gallery", nameof (QueryExtensions), (Exception) ex);
                continue;
              }
            case ExtensionQueryFilterType.ProductArchitecture:
              str = criterion.Value;
              break;
            case ExtensionQueryFilterType.TargetPlatform:
              str = criterion.Value;
              break;
            case ExtensionQueryFilterType.ExtensionName:
              if (string.IsNullOrEmpty(criterion.Value))
                this.FailInvalidQuery(GalleryResources.InvalidExtensionQuery((object) GalleryResources.MustSupplyExtensionName((object) filterType.ToString())));
              flag3 = true;
              empty = criterion.Value;
              if (criterion.Value.Contains("."))
              {
                string[] array = ((IEnumerable<string>) criterion.Value.Split(new string[1]
                {
                  "."
                }, StringSplitOptions.RemoveEmptyEntries)).Select<string, string>((Func<string, string>) (p => p.Trim())).ToArray<string>();
                if (((IEnumerable<string>) array).Count<string>() != 2)
                  this.FailInvalidQuery(GalleryResources.InvalidExtensionQuery((object) GalleryResources.InvalidExtensionNameSupplied((object) criterion.Value.ToString())));
                str = array[1];
                break;
              }
              str = criterion.Value;
              break;
            default:
              throw new ArgumentException(GalleryResources.UnRecognizedFilterType((object) filterType)).Expected("Gallery");
          }
          filterValues.Add(new QueryFilterValue()
          {
            QueryIndex = queryFilter.QueryIndex,
            FilterIndex = 0,
            FilterValueType = (int) filterType,
            FilterValue = str
          });
        }
      }
      extensionQuery.Flags = this.ProcessQueryFlags(extensionQuery.Flags);
      ExtensionQueryFlags extensionQueryFlags1 = extensionQuery.Flags | ExtensionQueryFlags.IncludeSharedAccounts | ExtensionQueryFlags.IncludeSharedOrganizations;
      bool isDbUsed = true;
      if (flag4)
      {
        if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableExtendedQueryExtensionsCache"))
        {
          ApplicableMemoryCacheType applicableMemoryCacheType = this.m_inMemoryQuery.GetApplicableMemoryCacheType(requestContext, extensionQuery);
          if (applicableMemoryCacheType != ApplicableMemoryCacheType.None)
          {
            if (flag2)
            {
              for (int index = 0; index < extensionQuery.Filters[0].Criteria.Count; ++index)
              {
                if (extensionQuery.Filters[0].Criteria[index].FilterType == 5)
                  filterValues[index].FilterValue = PublishedExtensionService.ConvertOldVstsCategoryToVerticalAlignedCategory(requestContext, extensionQuery.Filters[0].Criteria[index].Value);
              }
            }
            if (applicableMemoryCacheType.HasFlag((Enum) ApplicableMemoryCacheType.VSTS))
            {
              CachedExtensionData cachedData = this.m_inMemoryCacheForVsts.GetCachedData(requestContext);
              if (cachedData != null)
              {
                isDbUsed = false;
                queryResult = this.m_inMemoryQuery.QueryExtensionsNew(requestContext, cachedData, extensionQuery.Filters, filterValues, extensionQueryFlags1, accountToken);
              }
            }
            int? count;
            if (queryResult != null)
            {
              if (queryResult != null)
              {
                count = queryResult.Results?.Count;
                int num = 1;
                if (!(count.GetValueOrDefault() == num & count.HasValue) || queryResult.Results[0].Extensions.Count != 0)
                  goto label_69;
              }
              else
                goto label_69;
            }
            if (applicableMemoryCacheType.HasFlag((Enum) ApplicableMemoryCacheType.VSCode))
            {
              CachedExtensionData cachedData = this.m_inMemoryCacheForVsCode.GetCachedData(requestContext);
              if (cachedData != null)
              {
                isDbUsed = false;
                queryResult = this.m_inMemoryQuery.QueryExtensionsNew(requestContext, cachedData, extensionQuery.Filters, filterValues, extensionQueryFlags1, accountToken);
              }
            }
label_69:
            if (!requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableQueryExtensionsCacheForVSTS"))
            {
              if (queryResult != null)
              {
                if (queryResult != null)
                {
                  count = queryResult.Results?.Count;
                  int num = 1;
                  if (!(count.GetValueOrDefault() == num & count.HasValue) || queryResult.Results[0].Extensions.Count != 0)
                    goto label_75;
                }
                else
                  goto label_75;
              }
              if (!flag1)
                isDbUsed = true;
            }
label_75:
            this.UpdateAssetUrlsAndFilterInternalTagsInQueryResult(requestContext, queryResult, extensionQueryFlags1);
          }
        }
        else if (this.m_inMemoryQuery.IsQueryApplicableForCache(requestContext, extensionQuery, referrer))
        {
          CachedExtensionData cachedData = this.m_inMemoryCacheForVsCode.GetCachedData(requestContext);
          if (cachedData != null)
          {
            isDbUsed = false;
            queryResult = this.m_inMemoryQuery.QueryExtensions(requestContext, cachedData, extensionQuery.Filters, filterValues, extensionQueryFlags1);
            this.UpdateAssetUrlsAndFilterInternalTagsInQueryResult(requestContext, queryResult, extensionQueryFlags1);
          }
        }
      }
      if (isDbUsed)
      {
        string fromExtensionQuery = GalleryUtil.GetProductTypeFromExtensionQuery(extensionQuery);
        bool flag5 = extensionQueryFlags1.HasFlag((Enum) ExtensionQueryFlags.IncludeInstallationTargets);
        int num = PublishedExtensionService.IsPostDBQueryFilteringOnInstallationTargetRequired(extensionQuery) ? 1 : 0;
        if (num != 0)
          extensionQueryFlags1 |= ExtensionQueryFlags.IncludeInstallationTargets;
        componentStopwatch = Stopwatch.StartNew();
        queryResult = this.FetchQueryResultFromDb(requestContext, extensionQuery.Filters, filterValues, extensionQueryFlags1, fromExtensionQuery);
        componentStopwatch.Stop();
        this.FilterExtensionsPostDBQuery(extensionQuery, requestContext, queryResult, accountToken, isCacheBuildCall);
        if (num != 0)
        {
          this.FilterExtensionsPostDbQueryOnInstallationTarget(requestContext, extensionQuery, queryResult);
          if (!flag5)
          {
            ExtensionQueryFlags extensionQueryFlags2 = extensionQueryFlags1 ^ ExtensionQueryFlags.IncludeInstallationTargets;
            queryResult.Results.ForEach((Action<ExtensionFilterResult>) (result => result.Extensions.ForEach((Action<PublishedExtension>) (e => e.InstallationTargets = (List<InstallationTarget>) null))));
          }
        }
      }
      if (includeLatestAndValidatedVersionsOnlyFlag)
        this.FilterLatestAndValidatedVersionsOnly(requestContext, queryResult);
      if (flag3)
        PublishedExtensionService.FilterResultForUniqueExtensionName(requestContext, queryResult, empty);
      this.FilterAssets(extensionQuery, queryResult);
      stopwatch.Stop();
      if (queryResult.Results.Count > 0)
        PublishedExtensionService.PublishTelemetryEventForQueryExtensions(requestContext, queryResult.Results[0], stopwatch, componentStopwatch, extensionQuery, isDbUsed);
      return queryResult;
    }

    private static bool IsPostDBQueryFilteringOnInstallationTargetRequired(
      ExtensionQuery extensionQuery)
    {
      return extensionQuery.Filters.Count > 0 && extensionQuery.Filters[0].Criteria.Any<FilterCriteria>((Func<FilterCriteria, bool>) (c => c.FilterType == 8)) & extensionQuery.Filters[0].Criteria.Any<FilterCriteria>((Func<FilterCriteria, bool>) (c => c.FilterType == 15));
    }

    private ExtensionQueryResult FetchQueryResultFromDb(
      IVssRequestContext requestContext,
      List<QueryFilter> queryFilters,
      List<QueryFilterValue> filterValues,
      ExtensionQueryFlags queryFlags,
      string productType)
    {
      Func<ExtensionQueryResult> run = (Func<ExtensionQueryResult>) (() =>
      {
        using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
          return component.QueryExtensionsByQuery(queryFilters, filterValues, queryFlags, productType);
      });
      ExtensionQueryResult extensionQueryResult;
      if ((requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.QueryExtensionsCircuitBreaker") ? 1 : 0) != 0)
      {
        CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Marketplace.").AndCommandKey((CommandKey) "QueryExtensions").AndCommandPropertiesDefaults(PublishedExtensionService.QueryExtensionsCircuitBreakerSettings);
        extensionQueryResult = new CommandService<ExtensionQueryResult>(requestContext, setter, run).Execute();
      }
      else
        extensionQueryResult = run();
      return extensionQueryResult;
    }

    private static void PublishTelemetryEventForQueryExtensions(
      IVssRequestContext requestContext,
      ExtensionFilterResult extensionResults,
      Stopwatch stopwatch,
      Stopwatch componentStopwatch,
      ExtensionQuery extensionQuery,
      bool isDbUsed)
    {
      int num = int.MinValue;
      ClientTraceData clientTraceData = new ClientTraceData();
      clientTraceData.Add(CustomerIntelligenceProperty.Action, (object) "QueriedExtensions");
      if (extensionResults.ResultMetadata != null)
      {
        foreach (ExtensionFilterResultMetadata filterResultMetadata in extensionResults.ResultMetadata)
        {
          if (filterResultMetadata.MetadataType.Equals(QueryMetadataConstants.ResultCount, StringComparison.OrdinalIgnoreCase))
          {
            using (List<MetadataItem>.Enumerator enumerator = filterResultMetadata.MetadataItems.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                MetadataItem current = enumerator.Current;
                if (current.Name.Equals(QueryMetadataConstants.TotalCount, StringComparison.OrdinalIgnoreCase))
                {
                  num = current.Count;
                  break;
                }
              }
              break;
            }
          }
        }
      }
      if (num == int.MinValue)
        num = extensionResults.Extensions.Count;
      clientTraceData.Add("TotalCount", (object) num);
      clientTraceData.Add("ResultCount", (object) extensionResults.Extensions.Count);
      clientTraceData.AddGalleryUserIdentifier(requestContext);
      clientTraceData.Add("TotalQueryDuration", (object) stopwatch.ElapsedMilliseconds);
      if (componentStopwatch != null)
        clientTraceData.Add("QueryComponentDuration", (object) componentStopwatch.ElapsedMilliseconds);
      else
        clientTraceData.Add("QueryComponentDuration", (object) null);
      try
      {
        clientTraceData.Add("ExtensionQuery", (object) JsonConvert.SerializeObject((object) extensionQuery, new JsonSerializerSettings()
        {
          ContractResolver = (IContractResolver) new CamelCasePropertyNamesContractResolver()
        }));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12062047, "gallery", "LogClientTraceForQueryExtension", ex);
      }
      clientTraceData.Add("IsDbUsed", (object) isDbUsed);
      requestContext.GetService<ClientTraceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", "Extension", clientTraceData);
    }

    private static int GetValidPageSize(IVssRequestContext requestContext, QueryFilter queryFilter)
    {
      int validPageSize = queryFilter.PageSize;
      if (queryFilter.ForcePageSize && validPageSize >= 0 || validPageSize > 0 && validPageSize <= 1000)
        return validPageSize;
      validPageSize = validPageSize > 0 || requestContext.UserAgent == null || !requestContext.UserAgent.StartsWith("VSCode", StringComparison.OrdinalIgnoreCase) ? 1000 : 1;
      return validPageSize;
    }

    private int GetValidSortByType(int sortByType) => !Enum.IsDefined(typeof (SortByType), (object) sortByType) ? 0 : sortByType;

    private int GetValidSortOrderType(int sortOrderType)
    {
      SortOrderType sortOrderType1 = (SortOrderType) sortOrderType;
      return sortOrderType1.Equals((object) SortOrderType.Ascending) || sortOrderType1.Equals((object) SortOrderType.Descending) ? sortOrderType : 0;
    }

    private static ExtensionQueryResultMetadataFlags GetDefaultMetadataFlags() => ExtensionQueryResultMetadataFlags.IncludeResultCount | ExtensionQueryResultMetadataFlags.IncludePreCategoryFilterCategories;

    private void FilterAssets(ExtensionQuery extensionQuery, ExtensionQueryResult queryResult)
    {
      HashSet<string> stringSet1 = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (queryResult == null || queryResult.Results == null || extensionQuery.AssetTypes == null || extensionQuery.AssetTypes.Count <= 0 || !extensionQuery.Flags.HasFlag((Enum) ExtensionQueryFlags.IncludeFiles) && !extensionQuery.Flags.HasFlag((Enum) ExtensionQueryFlags.IncludeAssetUri))
        return;
      HashSet<string> stringSet2 = new HashSet<string>((IEnumerable<string>) extensionQuery.AssetTypes, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (ExtensionFilterResult result in queryResult.Results)
      {
        if (result.Extensions != null)
        {
          foreach (PublishedExtension extension in result.Extensions)
          {
            string fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(extension.Publisher.PublisherName, extension.ExtensionName);
            if (!stringSet1.Contains(fullyQualifiedName) && extension.Versions != null && extension.Versions.Count > 0)
            {
              foreach (ExtensionVersion version in extension.Versions)
              {
                if (version.Files != null)
                {
                  List<ExtensionFile> extensionFileList = new List<ExtensionFile>();
                  for (int index = 0; index < version.Files.Count; ++index)
                  {
                    ExtensionFile file = version.Files[index];
                    if (stringSet2.Contains(file.AssetType))
                      extensionFileList.Add(file);
                  }
                  version.Files = extensionFileList;
                }
              }
              stringSet1.Add(fullyQualifiedName);
            }
          }
        }
      }
    }

    private void FilterExtensionsPostDBQuery(
      ExtensionQuery extensionQuery,
      IVssRequestContext requestContext,
      ExtensionQueryResult queryResult,
      string accountToken,
      bool isCacheBuildCall = false,
      bool externalSearchUsed = false)
    {
      List<PublishedExtension> extensions = new List<PublishedExtension>();
      HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (ExtensionFilterResult result in queryResult.Results)
      {
        if (result.Extensions != null)
        {
          requestContext.Trace(12061033, TraceLevel.Info, "gallery", "QueryExtensions", string.Format("FilterExtensionsPostDBQuery | Extension count from DB:{0}", (object) result.Extensions.Count));
          for (int index = 0; index < result.Extensions.Count; ++index)
          {
            PublishedExtension extension = result.Extensions[index];
            if (!externalSearchUsed && extensionQuery.Flags.HasFlag((Enum) ExtensionQueryFlags.ExcludeNonValidated) && !extension.Flags.HasFlag((Enum) PublishedExtensionFlags.Validated))
            {
              requestContext.Trace(12061031, TraceLevel.Info, "gallery", "QueryExtensions", string.Format("FilterExtensionsPostDBQuery | Removing Extension (non-validated) | ExtensionId={0}, ExtensionName:{1}, ExtensionFlags={2}, PublisherId={3}, PublisherName={4}, PublisherFlags={5}, AccountToken={6}", (object) extension.ExtensionId, (object) extension.DisplayName, (object) extension.Flags, (object) extension.Publisher.PublisherId, (object) extension.Publisher.PublisherName, (object) extension.Publisher.Flags, (object) accountToken));
              result.Extensions.RemoveAt(index--);
            }
            else if (!GallerySecurity.HasExtensionPermission(requestContext, extension, accountToken, PublisherPermissions.Read, true) && !GallerySecurity.HasExtensionPermission(requestContext, extension, accountToken, PublisherPermissions.PrivateRead, true))
            {
              requestContext.Trace(12061034, TraceLevel.Info, "gallery", "QueryExtensions", string.Format("FilterExtensionsPostDBQuery | Removing Extension (no-permission) | ExtensionId={0}, ExtensionName:{1}, ExtensionFlags={2}, PublisherId={3}, PublisherName={4}, PublisherFlags={5}, AccountToken:{6}", (object) extension.ExtensionId, (object) extension.DisplayName, (object) extension.Flags, (object) extension.Publisher.PublisherId, (object) extension.Publisher.PublisherName, (object) extension.Publisher.Flags, (object) accountToken));
              result.Extensions.RemoveAt(index--);
            }
          }
        }
      }
      foreach (ExtensionFilterResult result in queryResult.Results)
      {
        if (result.Extensions != null)
        {
          foreach (PublishedExtension extension in result.Extensions)
          {
            string fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(extension.Publisher.PublisherName, extension.ExtensionName);
            if (!stringSet.Contains(fullyQualifiedName))
            {
              if (!externalSearchUsed && extensionQuery.Flags.HasFlag((Enum) ExtensionQueryFlags.IncludeVersions) && extensionQuery.Flags.HasFlag((Enum) ExtensionQueryFlags.IncludeVersionProperties))
                extensions.Add(extension);
              this.PrepareExtensionForClient(requestContext, extension, extensionQuery.Flags, accountToken, !isCacheBuildCall, externalSearchUsed);
              stringSet.Add(fullyQualifiedName);
            }
          }
        }
      }
      if (extensions == null)
        return;
      this.ReadExtensionProperties(requestContext, (IEnumerable<PublishedExtension>) extensions);
    }

    private void FilterExtensionsPostDbQueryOnInstallationTarget(
      IVssRequestContext requestContext,
      ExtensionQuery query,
      ExtensionQueryResult queryResult)
    {
      if (query.Filters.Count != queryResult.Results.Count)
        return;
      int index = 0;
      for (int count = query.Filters.Count; index < count; ++index)
      {
        QueryFilter filter = query.Filters[index];
        ExtensionFilterResult result = queryResult.Results[index];
        List<string> ProductArchitectureList = new List<string>();
        List<string> InstallationTargetList = new List<string>();
        string targetVersion = (string) null;
        filter.Criteria.ForEach((Action<FilterCriteria>) (criteria =>
        {
          switch ((ExtensionQueryFilterType) criteria.FilterType)
          {
            case ExtensionQueryFilterType.InstallationTarget:
              InstallationTargetList.Add(criteria.Value);
              break;
            case ExtensionQueryFilterType.InstallationTargetVersion:
              targetVersion = criteria.Value;
              break;
            case ExtensionQueryFilterType.ProductArchitecture:
              ProductArchitectureList.Add(criteria.Value);
              break;
          }
        }));
        bool useProductArchitectureInfo = requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableProductArchitectureSupportForVS");
        result.Extensions.RemoveAll((Predicate<PublishedExtension>) (extension =>
        {
          bool flag1 = !ProductArchitectureList.IsNullOrEmpty<string>() & useProductArchitectureInfo && extension.InstallationTargets.Any<InstallationTarget>((Func<InstallationTarget, bool>) (it => it.ProductArchitecture != null));
          foreach (string str in InstallationTargetList)
          {
            string target = str;
            bool flag2;
            if (flag1)
            {
              Version version = GalleryServerUtil.ConvertStringToVersion(targetVersion, false);
              flag2 = extension.InstallationTargets.Any<InstallationTarget>((Func<InstallationTarget, bool>) (it => it.IsApplicableForVersion(version, target) && ProductArchitectureList.Contains(it.ProductArchitecture)));
            }
            else
            {
              Version version = GalleryServerUtil.ConvertStringToVersion(targetVersion, false);
              flag2 = extension.InstallationTargets.Any<InstallationTarget>((Func<InstallationTarget, bool>) (it => it.IsApplicableForVersion(version, target)));
            }
            if (flag2)
              return false;
          }
          return true;
        }));
        PublishedExtensionService.UpdateTotalCountInResultMetadata(result);
      }
    }

    private static void UpdateTotalCountInResultMetadata(ExtensionFilterResult filterResult)
    {
      if (filterResult.ResultMetadata.IsNullOrEmpty<ExtensionFilterResultMetadata>())
        return;
      ExtensionFilterResultMetadata filterResultMetadata = filterResult.ResultMetadata.Find((Predicate<ExtensionFilterResultMetadata>) (m => m.MetadataType == QueryMetadataConstants.ResultCount));
      if (filterResultMetadata == null)
        return;
      MetadataItem metadataItem = filterResultMetadata.MetadataItems.Find((Predicate<MetadataItem>) (mi => mi.Name == QueryMetadataConstants.TotalCount));
      if (metadataItem == null)
        return;
      metadataItem.Count = filterResult.Extensions.Count<PublishedExtension>();
    }

    private static void FilterResultForUniqueExtensionName(
      IVssRequestContext requestContext,
      ExtensionQueryResult queryResult,
      string extensionNameQuery)
    {
      if (queryResult?.Results == null || queryResult.Results.Count<ExtensionFilterResult>() <= 0 || queryResult.Results[0].Extensions == null)
        return;
      List<PublishedExtension> extensions = queryResult.Results[0].Extensions;
      if (extensions.Count <= 1)
        return;
      string empty = string.Empty;
      if (extensionNameQuery.Contains("."))
        extensions.RemoveAll((Predicate<PublishedExtension>) (extension => !string.Equals(extensionNameQuery, GalleryUtil.CreateFullyQualifiedName(extension.Publisher.PublisherName, extension.ExtensionName), StringComparison.OrdinalIgnoreCase)));
      else
        extensions.RemoveAll((Predicate<PublishedExtension>) (extension => extension.IsVsCodeExtension() && requestContext.GetService<IExtensionNameConflictService>().ExtensionNameInConflictList(GalleryUtil.CreateFullyQualifiedName(extension.Publisher.PublisherName, extension.ExtensionName))));
    }

    private void FilterLatestAndValidatedVersionsOnly(
      IVssRequestContext requestContext,
      ExtensionQueryResult queryResult)
    {
      if (queryResult == null)
        return;
      foreach (ExtensionFilterResult result in queryResult.Results)
      {
        foreach (PublishedExtension extension in result.Extensions)
        {
          if (extension.IsVsCodeExtension() && requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnablePlatformSpecificExtensionsUIForManagePages"))
          {
            List<ExtensionVersion> extensionVersionList = new List<ExtensionVersion>();
            if (!extension.Versions.IsNullOrEmpty<ExtensionVersion>())
            {
              List<ExtensionVersion> supportedTargetPlatforms = GalleryUtil.GetLatestExtensionVersionForSupportedTargetPlatforms(extension.Versions);
              if (!supportedTargetPlatforms.IsNullOrEmpty<ExtensionVersion>())
                extensionVersionList = supportedTargetPlatforms;
            }
            extension.Versions = extensionVersionList;
          }
          else
          {
            List<ExtensionVersion> extensionVersionList = new List<ExtensionVersion>();
            ExtensionVersion extensionVersion1;
            if (extension == null)
            {
              extensionVersion1 = (ExtensionVersion) null;
            }
            else
            {
              List<ExtensionVersion> versions = extension.Versions;
              extensionVersion1 = versions != null ? versions.FirstOrDefault<ExtensionVersion>() : (ExtensionVersion) null;
            }
            ExtensionVersion extensionVersion2 = extensionVersion1;
            if (extensionVersion2 != null)
            {
              extensionVersionList.Add(extensionVersion2);
              if ((extensionVersion2.Flags & ExtensionVersionFlags.Validated) == ExtensionVersionFlags.None)
              {
                ExtensionVersion validatedVersion = PublishedExtensionService.GetLatestValidatedVersion(extension.Versions);
                if (validatedVersion != null)
                  extensionVersionList.Add(validatedVersion);
              }
            }
            extension.Versions = extensionVersionList;
          }
        }
      }
    }

    private static ExtensionVersion GetLatestValidatedVersion(List<ExtensionVersion> versions)
    {
      if (versions != null)
      {
        for (int index = 0; index < versions.Count; ++index)
        {
          ExtensionVersion version = versions[index];
          if ((version.Flags & ExtensionVersionFlags.Validated) != ExtensionVersionFlags.None)
            return version;
        }
      }
      return (ExtensionVersion) null;
    }

    public string GetVersionQuery(string version)
    {
      string versionQuery = version;
      ArgumentUtility.CheckStringForNullOrEmpty(version, nameof (version));
      if (string.Equals("latest", version, StringComparison.OrdinalIgnoreCase))
        versionQuery = (string) null;
      return versionQuery;
    }

    public CategoriesResult QueryAvailableCategories(
      IVssRequestContext requestContext,
      IEnumerable<string> languages,
      string categoryName = "all",
      string product = null)
    {
      CategoriesResult categoriesResult = new CategoriesResult();
      List<ExtensionCategory> enumerable1 = new List<ExtensionCategory>();
      IDictionary<int, ExtensionCategory> dictionary = (IDictionary<int, ExtensionCategory>) new Dictionary<int, ExtensionCategory>();
      categoriesResult.Categories = enumerable1;
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) languages, nameof (languages));
      this.EnsureCategoriesLoaded(requestContext);
      bool flag = !string.Equals("all", categoryName, StringComparison.OrdinalIgnoreCase);
      List<ExtensionCategory> enumerable2;
      if (!product.IsNullOrEmpty<char>())
      {
        PublishedExtensionService.ProductCategoryData productCategoryData;
        if (!this.m_categoriesData.ProductWiseCategoryDataMap.TryGetValue(product, out productCategoryData))
          return categoriesResult;
        enumerable2 = productCategoryData.Categories;
      }
      else
        enumerable2 = this.m_categoriesData.AllCategories;
      if (!enumerable2.IsNullOrEmpty<ExtensionCategory>())
      {
        foreach (ExtensionCategory extensionCategory1 in enumerable2)
        {
          if (!flag || string.Equals(extensionCategory1.CategoryName, categoryName, StringComparison.OrdinalIgnoreCase))
          {
            List<CategoryLanguageTitle> filteredLanguageTitles = PublishedExtensionService.GetFilteredLanguageTitles(extensionCategory1.LanguageTitles, languages);
            if (!filteredLanguageTitles.IsNullOrEmpty<CategoryLanguageTitle>())
            {
              ExtensionCategory extensionCategory2 = extensionCategory1.ShallowCopy();
              extensionCategory2.LanguageTitles = filteredLanguageTitles;
              enumerable1.Add(extensionCategory2);
              dictionary.Add(extensionCategory2.CategoryId, extensionCategory2);
            }
          }
        }
      }
      if (enumerable1.IsNullOrEmpty<ExtensionCategory>())
        throw new ArgumentException(GalleryResources.UnsupportedCategoryLanguage((object) string.Join(",", languages)));
      foreach (ExtensionCategory extensionCategory3 in enumerable1)
      {
        if (extensionCategory3.Parent != null)
        {
          ExtensionCategory extensionCategory4 = (ExtensionCategory) null;
          if (!dictionary.TryGetValue(extensionCategory3.Parent.CategoryId, out extensionCategory4))
          {
            extensionCategory4 = extensionCategory3.Parent.ShallowCopy();
            extensionCategory4.LanguageTitles = PublishedExtensionService.GetFilteredLanguageTitles(extensionCategory3.Parent.LanguageTitles, languages);
            dictionary.Add(extensionCategory3.Parent.CategoryId, extensionCategory4);
          }
          extensionCategory3.Parent = extensionCategory4;
        }
      }
      return categoriesResult;
    }

    private static List<CategoryLanguageTitle> GetFilteredLanguageTitles(
      List<CategoryLanguageTitle> languageTitles,
      IEnumerable<string> languages)
    {
      List<CategoryLanguageTitle> filteredLanguageTitles = new List<CategoryLanguageTitle>();
      foreach (CategoryLanguageTitle languageTitle in languageTitles)
      {
        if (languages.Contains<string>(languageTitle.Lang, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
          filteredLanguageTitles.Add(languageTitle);
      }
      return filteredLanguageTitles;
    }

    internal void UpdateBackConsolidatedExtensionInCache(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      try
      {
        Stopwatch stopwatch = Stopwatch.StartNew();
        if (eventData.StartsWith("BackConsolidation", StringComparison.OrdinalIgnoreCase))
        {
          using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
          {
            IReadOnlyDictionary<string, BackConsolidationMappingEntry> consolidationMapping = component.GetBackConsolidationMapping(requestContext);
            this.m_inMemoryCacheForBackConsolidation.GetCachedData(requestContext, true)?.AddNewExtensionMappingInCache(consolidationMapping);
          }
        }
        stopwatch.Stop();
        string format = string.Format("UpdateBackConsolidatedExtensionInCache: TimeTakenMs:{0}, eventData:{1}, eventClass:{2}", (object) stopwatch.ElapsedMilliseconds, (object) eventData, (object) eventClass);
        requestContext.TraceAlways(12062053, TraceLevel.Info, "gallery", nameof (UpdateBackConsolidatedExtensionInCache), format);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12062053, TraceLevel.Error, "gallery", nameof (UpdateBackConsolidatedExtensionInCache), ex);
      }
    }

    internal void UpdateExtensionInCache(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      try
      {
        if (!requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableProductExtensionsCacheUpdateOnNotification") || string.IsNullOrWhiteSpace(eventData))
          return;
        Stopwatch stopwatch = Stopwatch.StartNew();
        string[] source = eventData.Split((char[]) null);
        if (source.Length > 2 && !((IEnumerable<string>) source).Last<string>().IsNullOrEmpty<char>())
        {
          string a = source[1];
          if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableQueryExtensionsCacheForVSTS") && string.Equals(a, "vsts", StringComparison.OrdinalIgnoreCase))
          {
            string str = source[0];
            string extensionIdentifier = source[2];
            if (str.StartsWith("DeleteExtension", StringComparison.OrdinalIgnoreCase))
              this.m_inMemoryCacheForVsts.GetCachedData(requestContext, true)?.RemoveExtension(extensionIdentifier);
            else if (str.StartsWith("UpdateExtensionProperties", StringComparison.OrdinalIgnoreCase) || str.StartsWith("ProcessValidationResult", StringComparison.OrdinalIgnoreCase))
            {
              string[] strArray = extensionIdentifier.Split('.');
              PublishedExtension extension = this.QueryExtension(requestContext, strArray[0], strArray[1], (string) null, ExtensionQueryFlags.AllAttributes, (string) null, false);
              this.m_inMemoryCacheForVsts.GetCachedData(requestContext, true)?.UpdateExtension(extension);
            }
          }
          else if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableVsCodeProductExtensionsCacheUpdateOnNotification") && string.Equals(a, "vscode", StringComparison.OrdinalIgnoreCase))
            this.UpdateVsCodeExtensionInCache(requestContext, source[0], source[2]);
        }
        stopwatch.Stop();
        string format = string.Format("UpdateExtensionInCache: TimeTakenMs:{0}, eventData:{1}, eventClass:{2}", (object) stopwatch.ElapsedMilliseconds, (object) eventData, (object) eventClass);
        requestContext.TraceAlways(12062053, TraceLevel.Info, "gallery", nameof (UpdateExtensionInCache), format);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12062053, TraceLevel.Error, "gallery", nameof (UpdateExtensionInCache), ex);
      }
    }

    private void UpdateVsCodeExtensionInCache(
      IVssRequestContext requestContext,
      string eventName,
      string fullyQualifiedExtensionName)
    {
      if (eventName.StartsWith("DeleteExtension", StringComparison.OrdinalIgnoreCase))
      {
        CachedExtensionData cachedData = this.m_inMemoryCacheForVsCode.GetCachedData(requestContext, true);
        if (cachedData == null)
          return;
        cachedData.RemoveExtension(fullyQualifiedExtensionName);
        requestContext.TraceAlways(12062053, TraceLevel.Info, "gallery", nameof (UpdateVsCodeExtensionInCache), fullyQualifiedExtensionName + " Extension removed.");
      }
      else
      {
        if (!eventName.StartsWith("UpdateExtensionProperties", StringComparison.OrdinalIgnoreCase) && !eventName.StartsWith("ProcessValidationResult", StringComparison.OrdinalIgnoreCase))
          return;
        string[] strArray = fullyQualifiedExtensionName.Split('.');
        PublishedExtension extension = this.QueryExtension(requestContext, strArray[0], strArray[1], (string) null, ExtensionQueryFlags.AllAttributes, (string) null, false);
        CachedExtensionData cachedData = this.m_inMemoryCacheForVsCode.GetCachedData(requestContext, true);
        if (cachedData == null)
          return;
        cachedData.UpdateExtension(extension);
        requestContext.TraceAlways(12062053, TraceLevel.Info, "gallery", nameof (UpdateVsCodeExtensionInCache), fullyQualifiedExtensionName + " Extension added/updated.");
      }
    }

    public ExtensionCategory CreateCategory(
      IVssRequestContext requestContext,
      ExtensionCategory category)
    {
      GallerySecurity.CheckRootPermission(requestContext, PublisherPermissions.Admin);
      ArgumentUtility.CheckForNull<ExtensionCategory>(category, nameof (category));
      GalleryUtil.CheckCategoryName(category.CategoryName);
      if (category.CategoryName.Contains(" "))
        throw new ArgumentException(GalleryResources.InvalidCategoryName((object) category.CategoryName));
      if (category.LanguageTitles.IsNullOrEmpty<CategoryLanguageTitle>())
        throw new ArgumentException(GalleryResources.InvalidCategoryName((object) null));
      foreach (CategoryLanguageTitle languageTitle in category.LanguageTitles)
      {
        CategoryLanguageTitle categoryLanguageTitle = languageTitle;
        GalleryUtil.CheckCategoryName(categoryLanguageTitle.Title);
        if (GalleryServiceConstants.ValidCategoryLanguageCodes.Find((Predicate<string>) (x => string.Equals(x, categoryLanguageTitle.Lang))) == null)
          throw new ArgumentException(GalleryResources.InValidCategoryLanguageCode((object) string.Join(",", GalleryServiceConstants.ValidCategoryLanguageCodes.ToArray())));
      }
      int num;
      using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
        num = component.AddCategory(category);
      category.CategoryId = num;
      this.PublishCategoryIntelligenceEvent(requestContext, category, "AddCategory");
      return category;
    }

    public void ShareExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string shareType,
      string name,
      bool remove)
    {
      GalleryUtil.CheckPublisherName(publisherName);
      GalleryUtil.CheckExtensionName(extensionName);
      PublishedExtension extension;
      using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
        extension = component.QueryExtension(publisherName, extensionName, (string) null, Guid.Empty, ExtensionQueryFlags.IncludeInstallationTargets);
      this.ShareExtension(requestContext, extension, shareType, name, remove);
    }

    public void ShareExtension(
      IVssRequestContext requestContext,
      Guid extensionId,
      string shareType,
      string name,
      bool remove)
    {
      PublishedExtension extension;
      using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
        extension = component.QueryExtensionById(extensionId, (string) null, Guid.Empty, ExtensionQueryFlags.IncludeInstallationTargets);
      this.ShareExtension(requestContext, extension, shareType, name, remove);
    }

    private void ShareExtension(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      string shareType,
      string name,
      bool remove)
    {
      GallerySecurity.CheckExtensionPermission(requestContext, extension, (string) null, PublisherPermissions.UpdateExtension, false);
      if (extension.Flags.HasFlag((Enum) PublishedExtensionFlags.Public))
        throw new InvalidExtensionDefinitionException(GalleryResources.InvalidExtensionDefinition((object) GalleryResources.PublicExtensionCantBeShared()));
      if (extension.Flags.HasFlag((Enum) PublishedExtensionFlags.Locked))
        throw new InvalidExtensionDefinitionException(GalleryResources.InvalidExtensionDefinition((object) GalleryResources.LockedExtensionEditErrorMessage()));
      TagType shareType1 = TagType.SharedAccount;
      IVssRequestContext deploymentContext = requestContext.To(TeamFoundationHostType.Deployment);
      Guid empty1 = Guid.Empty;
      Guid empty2 = Guid.Empty;
      try
      {
        if (string.Equals(shareType, "account", StringComparison.CurrentCultureIgnoreCase))
        {
          if (!HostNameResolver.TryGetCollectionServiceHostId(deploymentContext, name, out empty2))
            throw new CollectionNotFoundException(GalleryResources.CollectionNotFound());
        }
        else if (string.Equals(shareType, "organization", StringComparison.CurrentCultureIgnoreCase))
        {
          shareType1 = TagType.SharedOrganization;
          bool flag;
          if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableTenantAsEnterpriseBoundaryInShareAndSearch"))
          {
            flag = name.Equals("Microsoft", StringComparison.OrdinalIgnoreCase);
            empty2 = requestContext.GetService<IVssRegistryService>().GetValue<Guid>(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/MsOrgId", Guid.Parse("b962ed4f-6670-4a37-ad24-8b9381530e67"));
          }
          else
            flag = HostNameResolver.TryGetOrganizationServiceHostId(deploymentContext, name, out empty2);
          if (!flag)
            throw new OrganizationNotFoundException(GalleryResources.OrganizationNotFound());
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12061072, "gallery", nameof (PublishedExtensionService), ex);
        throw;
      }
      using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
      {
        switch (component)
        {
          case PublishedExtensionComponent28 extensionComponent28:
            extensionComponent28.ShareExtension(requestContext, extension.Publisher.PublisherName, extension.ExtensionName, name, empty2, shareType1, remove);
            break;
          case PublishedExtensionComponent23 extensionComponent23:
            extensionComponent23.ShareExtension(requestContext, extension.Publisher.PublisherName, extension.ExtensionName, name, empty2, remove);
            break;
          default:
            component.ShareExtension(requestContext, extension.Publisher.PublisherName, extension.ExtensionName, name, empty2, empty2, remove);
            break;
        }
      }
      GallerySecurity.OnDataChanged(requestContext);
      this.PublishEvent(requestContext, remove ? ExtensionEventType.ExtensionUnshared : ExtensionEventType.ExtensionShared, extension, string.Empty, empty2);
      PublishedExtensionService.PublishCustomerIntelligenceEvent(requestContext, extension, remove ? "Unshared" : "Shared", name, tagType: new TagType?(shareType1), logTelemetryForOnPremise: true);
      try
      {
        this.m_SearchHelper.UpdateSearchIndex(requestContext, new List<PublishedExtension>()
        {
          extension
        });
      }
      catch (Exception ex)
      {
        string format = "Failed to update extension in the search index after the extension was " + (remove ? "Unshared" : "Shared") + "." + string.Format("Extension id: {0}, Extension: {1}.{2}", (object) extension.ExtensionId, (object) extension.Publisher.PublisherName, (object) extension.ExtensionName) + "ExceptionMessage: " + ex.Message + ", ExceptionStack: " + ex.StackTrace;
        requestContext.TraceAlways(12060103, TraceLevel.Error, "gallery", nameof (ShareExtension), format);
      }
    }

    public void UpdateExtensionAfterValidation(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      bool updateExtension,
      bool isVsExtension)
    {
      if (!updateExtension || extension == null)
        return;
      this.WriteExtensionProperties(requestContext, extension);
      if (isVsExtension)
        return;
      this.UpdateExtensionInstallationTarget(requestContext, extension);
    }

    public PublishedExtension ProcessValidationResult(
      IVssRequestContext requestContext,
      Guid extensionId,
      string version,
      string targetPlatform,
      Guid validationId,
      PublishedExtensionFlags existingExtensionFlags,
      string message,
      bool success,
      PublishedExtension extension,
      bool updateExtension,
      bool isVsExtension,
      int? fileId)
    {
      this.UpdateExtensionAfterValidation(requestContext, extension, updateExtension, isVsExtension);
      using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
      {
        try
        {
          component.ProcessValidationResult(extensionId, version, targetPlatform, validationId, message, success, fileId);
        }
        catch (NotImplementedException ex)
        {
          component.ProcessValidationResult(extensionId, version, validationId, message, success, fileId);
        }
      }
      try
      {
        if (success)
        {
          if (extension != null)
            this.m_SearchHelper.UpdateSearchIndex(requestContext, new List<PublishedExtension>()
            {
              extension
            });
        }
      }
      catch (Exception ex)
      {
        string format = "Failed to update extension in the search index after the processing extension validation result." + string.Format("Extension id: {0}, Extension: {1}.{2}", (object) extension.ExtensionId, (object) extension.Publisher.PublisherName, (object) extension.ExtensionName) + "ExceptionMessage: " + ex.Message + ", ExceptionStack: " + ex.StackTrace;
        requestContext.TraceAlways(12060103, TraceLevel.Error, "gallery", nameof (ProcessValidationResult), format);
      }
      return extension;
    }

    public ExtensionVersionValidation QueryVersionValidation(
      IVssRequestContext requestContext,
      Guid extensionId,
      string version,
      string targetPlatform = null)
    {
      using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
      {
        try
        {
          return component.QueryVersionValidation(extensionId, version, targetPlatform);
        }
        catch (NotImplementedException ex)
        {
          return component.QueryVersionValidation(extensionId, version);
        }
      }
    }

    public PublishedExtension UpdateExtension(
      IVssRequestContext requestContext,
      ExtensionPackage extensionPackage,
      string requestedExtensionName,
      string requestingPublisherName)
    {
      using (Stream extensionPackageStream = GalleryServerUtil.GetExtensionPackageStream(extensionPackage))
        return this.UpdateExtensionFromStream(requestContext, extensionPackageStream, requestedExtensionName, requestingPublisherName, (IEnumerable<ExtensionFile>) null, false);
    }

    public PublishedExtension UpdateExtension(
      IVssRequestContext requestContext,
      Stream extensionPackageStream,
      string requestedExtensionName,
      string requestingPublisherName)
    {
      return this.UpdateExtensionFromStream(requestContext, extensionPackageStream, requestedExtensionName, requestingPublisherName, (IEnumerable<ExtensionFile>) null, false);
    }

    public PublishedExtension UpdateExtension(
      IVssRequestContext requestContext,
      ExtensionPackage extensionPackage,
      string requestedExtensionName,
      string requestingPublisherName,
      IEnumerable<ExtensionFile> uploadedAssets,
      bool immediateVersionValidation)
    {
      using (Stream extensionPackageStream = GalleryServerUtil.GetExtensionPackageStream(extensionPackage))
        return this.UpdateExtension(requestContext, extensionPackageStream, requestedExtensionName, requestingPublisherName, uploadedAssets, immediateVersionValidation);
    }

    public PublishedExtension UpdateExtension(
      IVssRequestContext requestContext,
      Stream extensionPackageStream,
      string requestedExtensionName,
      string requestingPublisherName,
      IEnumerable<ExtensionFile> uploadedAssets,
      bool immediateVersionValidation)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Stream>(extensionPackageStream, nameof (extensionPackageStream));
      if (requestContext.ExecutionEnvironment.IsHostedDeployment && !requestContext.ExecutionEnvironment.IsDevFabricDeployment)
        throw new NotSupportedException("This interface cannot be invoked in a hosted scenario.");
      return this.UpdateExtensionFromStream(requestContext, extensionPackageStream, requestedExtensionName, requestingPublisherName, uploadedAssets, immediateVersionValidation);
    }

    public PublishedExtension UpdateExtensionProperties(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string displayName,
      PublishedExtensionFlags flags,
      string shortDescription,
      string longDescription)
    {
      GalleryUtil.CheckPublisherName(publisherName);
      GalleryUtil.CheckExtensionName(extensionName);
      PublishedExtension extension1;
      using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
        extension1 = component.QueryExtension(publisherName, extensionName, (string) null, Guid.Empty, ExtensionQueryFlags.IncludeInstallationTargets);
      if (flags.HasFlag((Enum) PublishedExtensionFlags.BuiltIn))
      {
        if (!requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableHiddenFlagAddition") || flags.HasFlag((Enum) PublishedExtensionFlags.Paid))
          flags |= PublishedExtensionFlags.Trusted | PublishedExtensionFlags.Public;
        else
          flags |= PublishedExtensionFlags.Trusted | PublishedExtensionFlags.Public | PublishedExtensionFlags.Hidden;
      }
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        flags |= PublishedExtensionFlags.Public;
      this.ValidateIfFlagsUpdatePermitted(extension1, flags);
      this.ValidatePaidExtensionStateTransition(flags, extension1);
      GallerySecurity.CheckExtensionPermission(requestContext, extension1, (string) null, PublisherPermissions.UpdateExtension, false);
      GallerySecurity.CheckExtensionChangePermissions(requestContext, publisherName, extension1.Flags, flags, (IEnumerable<InstallationTarget>) extension1.InstallationTargets);
      if (extension1.Flags.HasFlag((Enum) PublishedExtensionFlags.Validated))
        flags |= PublishedExtensionFlags.Validated;
      ExtensionEventType eventType = !flags.HasFlag((Enum) PublishedExtensionFlags.Disabled) || extension1.Flags.HasFlag((Enum) PublishedExtensionFlags.Disabled) ? (flags.HasFlag((Enum) PublishedExtensionFlags.Disabled) || !extension1.Flags.HasFlag((Enum) PublishedExtensionFlags.Disabled) ? (!flags.HasFlag((Enum) PublishedExtensionFlags.Locked) || extension1.Flags.HasFlag((Enum) PublishedExtensionFlags.Locked) ? (flags.HasFlag((Enum) PublishedExtensionFlags.Locked) || !extension1.Flags.HasFlag((Enum) PublishedExtensionFlags.Locked) ? ExtensionEventType.ExtensionUpdated : ExtensionEventType.ExtensionUnlocked) : ExtensionEventType.ExtensionLocked) : ExtensionEventType.ExtensionEnabled) : ExtensionEventType.ExtensionDisabled;
      string str1 = "Updated";
      if (flags.HasFlag((Enum) PublishedExtensionFlags.Locked) && !extension1.Flags.HasFlag((Enum) PublishedExtensionFlags.Locked))
        str1 = "Locked";
      else if (!flags.HasFlag((Enum) PublishedExtensionFlags.Locked) && extension1.Flags.HasFlag((Enum) PublishedExtensionFlags.Locked))
        str1 = "Unlocked";
      else if (flags.HasFlag((Enum) PublishedExtensionFlags.Unpublished) && !extension1.Flags.HasFlag((Enum) PublishedExtensionFlags.Unpublished))
        str1 = "Unpublished";
      else if (!flags.HasFlag((Enum) PublishedExtensionFlags.Unpublished) && extension1.Flags.HasFlag((Enum) PublishedExtensionFlags.Unpublished))
        str1 = "Published";
      List<InstallationTarget> installationTargets = extension1.InstallationTargets;
      PublishedExtension extension2;
      using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
        extension2 = component.UpdateExtensionProperties(requestContext, publisherName, extensionName, displayName, flags, shortDescription, longDescription, this.m_everyoneGroup.Id);
      try
      {
        this.m_SearchHelper.UpdateSearchIndex(requestContext, new List<PublishedExtension>()
        {
          extension2
        });
      }
      catch (Exception ex)
      {
        string format = "Failed to update extension properties in the search index." + string.Format("Extension id: {0}, Extension: {1}.{2}", (object) extension1.ExtensionId, (object) extension1.Publisher.PublisherName, (object) extension1.ExtensionName) + "ExceptionMessage: " + ex.Message + ", ExceptionStack: " + ex.StackTrace;
        requestContext.TraceAlways(12060103, TraceLevel.Error, "gallery", nameof (UpdateExtensionProperties), format);
        using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
          component.UpdateExtensionProperties(requestContext, extension1.Publisher.PublisherName, extension1.ExtensionName, extension1.DisplayName, extension1.Flags, extension1.ShortDescription, extension1.LongDescription, this.m_everyoneGroup.Id);
        throw;
      }
      extension2.InstallationTargets = installationTargets;
      this.PublishEvent(requestContext, eventType, extension2, (string) null);
      GallerySecurity.OnDataChanged(requestContext);
      string str2 = "";
      if (extension2.InstallationTargets != null)
        str2 = GalleryUtil.GetProductTypeForInstallationTargets((IEnumerable<InstallationTarget>) extension2.InstallationTargets);
      GalleryServerUtil.NotifyGalleryDataChanged(requestContext, GalleryNotificationEventIds.ExtensionUpdateDelete, "UpdateExtensionProperties: " + str2 + " " + publisherName + "." + extensionName);
      this.PrepareExtensionForClient(requestContext, extension2, ExtensionQueryFlags.None);
      PublishedExtensionService.PublishCustomerIntelligenceEvent(requestContext, extension2, str1, installationTargets: (IEnumerable<InstallationTarget>) extension2.InstallationTargets, logTelemetryForOnPremise: true);
      if (extension1.IsVsCodeExtension())
      {
        if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnablePMPIntegration"))
        {
          try
          {
            if (str1 == "Unpublished" || str1 == "Published")
            {
              HttpResponseMessage httpResponseMessage = PublishedExtensionService.UpdateExtensionArchivedStateInPMP(requestContext, extension1, !(str1 == "Published"));
              if (httpResponseMessage != null && !httpResponseMessage.IsSuccessStatusCode)
                return extension2;
            }
            if (eventType != ExtensionEventType.ExtensionLocked)
            {
              if (eventType != ExtensionEventType.ExtensionUnlocked)
                goto label_44;
            }
            PublishedExtensionService.UpdateExtensionLockedStateInPMP(requestContext, extension1, eventType == ExtensionEventType.ExtensionLocked);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(12062089, "Gallery", str1, ex);
          }
label_44:
          if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableVSCodeExtensionMutationEventsPublishing"))
          {
            IExtensionNameConflictService service1 = requestContext.GetService<IExtensionNameConflictService>();
            string extensionName1 = extension1.ExtensionName;
            string extensionName2 = extension1.Publisher.PublisherName + "." + extension1.ExtensionName;
            string existsInConflictList = service1.GetNewExtensionNameIfExistsInConflictList(extensionName2);
            if (!string.IsNullOrEmpty(existsInConflictList))
              extensionName1 = existsInConflictList;
            IEventPublisherService service2 = requestContext.GetService<IEventPublisherService>();
            switch (str1)
            {
              case "Published":
                service2.PublishPackageAggregateUnarchivedEvent(requestContext, extensionName1);
                break;
              case "Unpublished":
                service2.PublishPackageAggregateArchivedEvent(requestContext, extensionName1);
                break;
              case "Locked":
                service2.PublishPackageAggregateLockedEvent(requestContext, extensionName1);
                break;
              case "Unlocked":
                service2.PublishPackageAggregateUnlockedEvent(requestContext, extensionName1);
                break;
            }
          }
        }
      }
      return extension2;
    }

    private static HttpResponseMessage UpdateExtensionArchivedStateInPMP(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      bool isArchived)
    {
      return TaskExtensions.SyncResult(requestContext.GetService<IPackageManagementPlatformServiceFacade>().UpdatePackageAggregateArchivedState(requestContext, "vscode", extension, isArchived));
    }

    private static HttpResponseMessage UpdateExtensionLockedStateInPMP(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      bool isLocked)
    {
      return TaskExtensions.SyncResult(requestContext.GetService<IPackageManagementPlatformServiceFacade>().UpdatePackageAggregateLockState(requestContext, "vscode", extension, isLocked));
    }

    public PublishedExtension AddExtensionIndexedTerm(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      TagType tagType,
      string tagValue,
      bool notify = true)
    {
      PublishedExtension extension = this.ApplyExtensionIndexedTerm(requestContext, publisherName, extensionName, tagType, tagValue, notify: notify);
      this.PrepareExtensionForClient(requestContext, extension, ExtensionQueryFlags.None);
      PublishedExtensionService.PublishCustomerIntelligenceEvent(requestContext, extension, "AddIndexedTerm", tagType: new TagType?(tagType), tagValue: tagValue);
      return extension;
    }

    public PublishedExtension RemoveExtensionIndexedTerm(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      TagType tagType,
      string tagValue,
      bool notify = true)
    {
      PublishedExtension extension = this.ApplyExtensionIndexedTerm(requestContext, publisherName, extensionName, tagType, tagValue, true, notify);
      this.PrepareExtensionForClient(requestContext, extension, ExtensionQueryFlags.None);
      PublishedExtensionService.PublishCustomerIntelligenceEvent(requestContext, extension, "RemoveIndexedTerm", tagType: new TagType?(tagType), tagValue: tagValue);
      return extension;
    }

    public List<PublishedExtensionUpdate> GetExtensionData(IVssRequestContext requestContext)
    {
      GallerySecurity.CheckRootPermission(requestContext, PublisherPermissions.Admin);
      using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
        return component.GetExtensionData(requestContext);
    }

    public void UpdateExtensionReleaseDate(
      IVssRequestContext requestContext,
      Guid extensionId,
      DateTime releaseDateToUpdate)
    {
      using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
        component.UpdateReleaseDate(extensionId, releaseDateToUpdate);
    }

    public void UpdateExtensionCDNProperties(
      IVssRequestContext requestContext,
      Guid extensionId,
      string version,
      string cdnDirectory = null,
      bool isCdnEnabled = false,
      string targetPlatform = null)
    {
      using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
      {
        try
        {
          component.UpdateCDNProperties(extensionId, version, cdnDirectory, isCdnEnabled, targetPlatform);
        }
        catch (NotImplementedException ex)
        {
          component.UpdateCDNProperties(extensionId, version, cdnDirectory, isCdnEnabled);
        }
      }
    }

    public void AddAssetForExtensionVersion(
      IVssRequestContext requestContext,
      Guid extensionId,
      string version,
      string assetType,
      string contentType,
      int fileId,
      string shortDescription)
    {
      if (!requestContext.IsSystemContext)
        throw new NotSupportedException("AddAssetForExtensionVersion should be called as a system context");
      using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
        component.AddAssetForExtensionVersion(extensionId, version, assetType, contentType, fileId, shortDescription);
    }

    public string GetAndValidateExtensionProductFamily(
      IVssRequestContext requestContext,
      IList<InstallationTarget> targets,
      IList<InstallationTarget> existingTargets)
    {
      IProductService service = requestContext.GetService<IProductService>();
      IDictionary<InstallationTarget, ProductFamily> dictionary = service.QueryProductFamilies(requestContext, targets);
      string extensionProductFamily = string.Empty;
      foreach (KeyValuePair<InstallationTarget, ProductFamily> keyValuePair in (IEnumerable<KeyValuePair<InstallationTarget, ProductFamily>>) dictionary)
      {
        if (keyValuePair.Value != null)
        {
          if (!string.IsNullOrEmpty(extensionProductFamily) && !extensionProductFamily.Equals(keyValuePair.Value.Id, StringComparison.OrdinalIgnoreCase))
            throw new InvalidExtensionDefinitionException(GalleryResources.MultipleProductFamilyError());
          extensionProductFamily = keyValuePair.Value.Id;
        }
      }
      if (!existingTargets.IsNullOrEmpty<InstallationTarget>())
      {
        foreach (KeyValuePair<InstallationTarget, ProductFamily> queryProductFamily in (IEnumerable<KeyValuePair<InstallationTarget, ProductFamily>>) service.QueryProductFamilies(requestContext, existingTargets))
        {
          if (queryProductFamily.Value != null && !extensionProductFamily.Equals(queryProductFamily.Value.Id, StringComparison.OrdinalIgnoreCase))
            throw new InvalidExtensionDefinitionException(GalleryResources.ProductFamilyChangeError());
        }
      }
      return extensionProductFamily;
    }

    public void PublishExtensionCreateUpdateNotifications(
      IVssRequestContext requestContext,
      bool sendMessageBusNotification,
      PublishedExtension extension)
    {
      string str = "";
      if (extension == null || extension.Versions.IsNullOrEmpty<ExtensionVersion>())
        return;
      if (extension.InstallationTargets != null)
        str = GalleryUtil.GetProductTypeForInstallationTargets((IEnumerable<InstallationTarget>) extension.InstallationTargets);
      string eventData = "ProcessValidationResult: " + str + " " + extension.Publisher.PublisherName + "." + extension.ExtensionName;
      GalleryServerUtil.NotifyGalleryDataChanged(requestContext, GalleryNotificationEventIds.ExtensionUpdateDelete, eventData);
      if (sendMessageBusNotification)
        this.PublishEvent(requestContext, extension.Flags.HasFlag((Enum) PublishedExtensionFlags.Validated) ? ExtensionEventType.ExtensionUpdated : ExtensionEventType.ExtensionCreated, extension, extension.Versions[0].Version);
      string format = string.Format("PublishExtensionCreateUpdateNotifications: Firing Sql notification for extension create/update: eventData:{0}, eventClass:{1}", (object) eventData, (object) GalleryNotificationEventIds.ExtensionUpdateDelete);
      requestContext.TraceAlways(12062053, TraceLevel.Info, "gallery", nameof (PublishExtensionCreateUpdateNotifications), format);
    }

    public ExtensionVersionValidationStep CreateVersionValidationStep(
      IVssRequestContext requestContext,
      Guid stepId,
      Guid parentValidationId,
      int stepType,
      int stepStatus,
      DateTime startTime)
    {
      using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
        return component.CreateValidationStep(requestContext, stepId, parentValidationId, stepType, stepStatus, startTime);
    }

    public IEnumerable<ExtensionVersionValidationStep> GetAllValidationSteps(
      IVssRequestContext requestContext)
    {
      using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
        return (IEnumerable<ExtensionVersionValidationStep>) component.GetAllValidationSteps(requestContext, Guid.Empty);
    }

    public IEnumerable<ExtensionVersionValidationStep> GetAllValidationStepsByParent(
      IVssRequestContext requestContext,
      Guid parentValidationId)
    {
      using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
        return (IEnumerable<ExtensionVersionValidationStep>) component.GetAllValidationSteps(requestContext, parentValidationId);
    }

    public ExtensionValidationResult GetValidationResult(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string version,
      string targetPlatform = null)
    {
      Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher;
      using (PublisherComponent component = requestContext.CreateComponent<PublisherComponent>())
        publisher = component.QueryPublisher(publisherName, PublisherQueryFlags.None);
      GallerySecurity.CheckPublisherPermission(requestContext, publisher, PublisherPermissions.Read | PublisherPermissions.UpdateExtension | PublisherPermissions.PublishExtension | PublisherPermissions.PrivateRead | PublisherPermissions.DeleteExtension | PublisherPermissions.ViewPermissions);
      PublishedExtension publishedExtension;
      using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
        publishedExtension = component.QueryExtension(publisherName, extensionName, version, Guid.Empty, ExtensionQueryFlags.IncludeVersions);
      if (publishedExtension != null && publishedExtension.Versions != null)
        publishedExtension.Versions.RemoveAll((Predicate<ExtensionVersion>) (extensionVersion => !string.Equals(targetPlatform, extensionVersion.TargetPlatform)));
      if (publishedExtension == null || publishedExtension.Versions == null || publishedExtension.Versions.Count != 1)
      {
        string str;
        if (targetPlatform != null)
          str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ExtensionDoesNotExistException for {0}.{1}-{2}@{3}", (object) publisherName, (object) extensionName, (object) version, (object) targetPlatform);
        else
          str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ExtensionDoesNotExistException for {0}.{1}-{2}", (object) publisherName, (object) extensionName, (object) version);
        string message1 = str;
        requestContext.Trace(12061099, TraceLevel.Info, "gallery", nameof (GetValidationResult), message1);
        string message2;
        if (targetPlatform != null)
          message2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Extension not found for extension: {0}.{1}-{2}@{3}", (object) publisherName, (object) extensionName, (object) version, (object) targetPlatform);
        else
          message2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Extension not found for extension: {0}.{1}-{2}", (object) publisherName, (object) extensionName, (object) version);
        throw new ExtensionDoesNotExistException(message2);
      }
      using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
        return component.GetValidationResult(requestContext, publishedExtension.ExtensionId, version, targetPlatform);
    }

    public ExtensionVersionValidationStep UpdateVersionValidationStep(
      IVssRequestContext requestContext,
      ExtensionVersionValidationStep validationStep)
    {
      using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
        return component.UpdateValidationStep(requestContext, validationStep);
    }

    public void DeleteValidationStepsByParentId(
      IVssRequestContext requestContext,
      Guid parentValidationId)
    {
      using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
        component.DeleteValidationStepsByParentId(requestContext, parentValidationId);
    }

    private PublishedExtension ApplyExtensionIndexedTerm(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      TagType tagType,
      string tagValue,
      bool remove = false,
      bool notify = true)
    {
      GalleryUtil.CheckPublisherName(publisherName);
      GalleryUtil.CheckExtensionName(extensionName);
      PublishedExtension extension;
      using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
        extension = component.QueryExtension(publisherName, extensionName, (string) null, Guid.Empty, ExtensionQueryFlags.IncludeInstallationTargets);
      if (!extension.Flags.HasFlag((Enum) PublishedExtensionFlags.Public))
        throw new InvalidOperationException(GalleryResources.CannotManageIndexedTermsNonPublicExtensions());
      if (tagType != TagType.BasicTag && tagType != TagType.Category && tagType != TagType.Featured && tagType != TagType.FeaturedInCategory)
        throw new InvalidTagException(GalleryResources.TagTypeNotSupportedInMgmtIndexedTerms((object) tagType));
      string product = "";
      if (extension.InstallationTargets != null)
        product = GalleryUtil.GetProductTypeForInstallationTargets((IEnumerable<InstallationTarget>) extension.InstallationTargets);
      switch (tagType)
      {
        case TagType.BasicTag:
          if (tagValue.StartsWith("$") && GallerySecurity.HasRootPermission(requestContext, PublisherPermissions.Admin))
          {
            GalleryUtil.CheckTag(tagValue.Substring(1));
            break;
          }
          GalleryUtil.CheckTag(tagValue);
          break;
        case TagType.Category:
          tagValue = this.ConvertCategoryNamesToTags(requestContext, (IEnumerable<string>) new string[1]
          {
            tagValue
          }, "en-us", product, false)[0].Value;
          break;
        case TagType.Featured:
          tagValue = "$Featured";
          GallerySecurity.CheckRootPermission(requestContext, PublisherPermissions.Admin);
          break;
      }
      GallerySecurity.CheckExtensionPermission(requestContext, extension, (string) null, PublisherPermissions.UpdateExtension, false);
      using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
        extension = component.ApplyExtensionIndexedTerm(requestContext, extension.Publisher.PublisherName, extension.ExtensionName, tagType, tagValue, remove, notify);
      if (tagType == TagType.Category || tagType == TagType.Featured || tagType == TagType.FeaturedInCategory)
        GalleryServerUtil.NotifyGalleryDataChanged(requestContext, GalleryNotificationEventIds.ExtensionUpdateDelete, "ApplyExtensionIndexedTerm: " + product + " " + extension.Publisher.PublisherName + "." + extension.ExtensionName);
      return extension;
    }

    internal virtual void PrepareExtensionForClient(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      ExtensionQueryFlags flags,
      string accountToken = null,
      bool filterInternalTags = true,
      bool externalSearchUsed = false)
    {
      if (filterInternalTags)
        this.FilterInternalTags(extension);
      bool isCDNURIEnabled = PublishedExtensionService.IsCDNUriEnabled(requestContext, flags);
      if (!externalSearchUsed)
        extension.Categories = this.ConvertCategoryIdsToNames(requestContext, (IEnumerable<string>) extension.Categories, "en-us", true);
      if (extension.Flags.HasFlag((Enum) PublishedExtensionFlags.Public) || !GallerySecurity.HasExtensionPermission(requestContext, extension, accountToken, PublisherPermissions.UpdateExtension, false))
      {
        extension.SharedWith = (List<ExtensionShare>) null;
      }
      else
      {
        if (!extension.SharedWith.IsNullOrEmpty<ExtensionShare>())
        {
          if (!flags.HasFlag((Enum) ExtensionQueryFlags.IncludeSharedAccounts))
            extension.SharedWith.RemoveAll((Predicate<ExtensionShare>) (sw => string.Equals(sw.Type, "account", StringComparison.InvariantCultureIgnoreCase)));
          if (!flags.HasFlag((Enum) ExtensionQueryFlags.IncludeSharedOrganizations))
            extension.SharedWith.RemoveAll((Predicate<ExtensionShare>) (sw => string.Equals(sw.Type, "organization", StringComparison.InvariantCultureIgnoreCase)));
        }
        if (extension.SharedWith != null && extension.SharedWith.Count == 0)
          extension.SharedWith = (List<ExtensionShare>) null;
      }
      if (!flags.HasFlag((Enum) ExtensionQueryFlags.IncludeFiles) && !flags.HasFlag((Enum) ExtensionQueryFlags.IncludeAssetUri))
        return;
      PublishedExtensionService.UpdateExtensionAssetUrls(requestContext, extension, flags, isCDNURIEnabled);
    }

    private void UpdateAssetUrlsAndFilterInternalTagsInQueryResult(
      IVssRequestContext requestContext,
      ExtensionQueryResult queryResult,
      ExtensionQueryFlags flags)
    {
      if (queryResult == null || queryResult.Results == null)
        return;
      if (flags.HasFlag((Enum) ExtensionQueryFlags.IncludeFiles) || flags.HasFlag((Enum) ExtensionQueryFlags.IncludeAssetUri))
      {
        HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        bool isCDNURIEnabled = PublishedExtensionService.IsCDNUriEnabled(requestContext, flags);
        foreach (ExtensionFilterResult result in queryResult.Results)
        {
          if (result.Extensions != null)
          {
            foreach (PublishedExtension extension in result.Extensions)
            {
              string fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(extension.Publisher.PublisherName, extension.ExtensionName);
              if (!stringSet.Contains(fullyQualifiedName) && extension.Versions != null && extension.Versions.Count > 0)
              {
                PublishedExtensionService.UpdateExtensionAssetUrls(requestContext, extension, flags, isCDNURIEnabled);
                stringSet.Add(fullyQualifiedName);
              }
            }
          }
        }
      }
      foreach (ExtensionFilterResult result in queryResult.Results)
      {
        if (result.Extensions != null)
        {
          foreach (PublishedExtension extension in result.Extensions)
            this.FilterInternalTags(extension);
        }
      }
    }

    private static void UpdateExtensionAssetUrls(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      ExtensionQueryFlags flags,
      bool isCDNURIEnabled)
    {
      if (extension == null || extension.Versions == null || extension.Versions.Count <= 0)
        return;
      IPublisherAssetService service = requestContext.GetService<IPublisherAssetService>();
      ExtensionAsset extensionAsset = new ExtensionAsset()
      {
        Extension = extension.ShallowCopy()
      };
      extensionAsset.Extension.Versions = new List<ExtensionVersion>();
      extensionAsset.AssetFile = new ExtensionFile();
      extensionAsset.AssetFile.IsPublic = extension.Flags.HasFlag((Enum) PublishedExtensionFlags.Public);
      foreach (ExtensionVersion version in extension.Versions)
      {
        extensionAsset.Extension.Versions.Clear();
        extensionAsset.Extension.Versions.Add(version);
        extensionAsset.AssetFile.AssetType = "";
        version.AssetUri = service.GetAssetUri(requestContext, extensionAsset, isCDNURIEnabled).AbsoluteUri;
        version.FallbackAssetUri = service.GetAssetUri(requestContext, extensionAsset, !isCDNURIEnabled).AbsoluteUri;
        if (flags.HasFlag((Enum) ExtensionQueryFlags.IncludeFiles) && version.Files != null)
        {
          for (int index = 0; index < version.Files.Count; ++index)
          {
            ExtensionFile file = version.Files[index];
            extensionAsset.AssetFile.AssetType = file.AssetType;
            file.Source = service.GetAssetUri(requestContext, version.AssetUri, extensionAsset.AssetFile.AssetType).AbsoluteUri;
          }
        }
      }
    }

    private static bool IsCDNUriEnabled(
      IVssRequestContext requestContext,
      ExtensionQueryFlags flags)
    {
      if (flags.HasFlag((Enum) ExtensionQueryFlags.UseFallbackAssetUri))
        return false;
      if (requestContext.UserAgent == null)
        return true;
      return (!requestContext.UserAgent.StartsWith("VSCode", StringComparison.OrdinalIgnoreCase) || PublishedExtensionService.IsUAVsCodeAndVersionSupported(requestContext, "1.8")) && !requestContext.UserAgent.Equals("VSServices/15.103.25603.0 (TfsJobAgent.exe)", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsUAVsCodeAndVersionSupported(
      IVssRequestContext requestContext,
      string minSupportedVerion)
    {
      bool flag = false;
      string userAgent = requestContext?.UserAgent;
      if (!string.IsNullOrWhiteSpace(userAgent))
      {
        string[] strArray = userAgent.Split(new char[2]
        {
          ' ',
          '-'
        }, StringSplitOptions.RemoveEmptyEntries);
        Version result1;
        Version result2;
        if (strArray.Length >= 2 && strArray[0].Equals("VSCode", StringComparison.OrdinalIgnoreCase) && Version.TryParse(minSupportedVerion, out result1) && Version.TryParse(strArray[1], out result2) && result2 >= result1)
          flag = true;
      }
      return flag;
    }

    private bool IsExtensionVisible(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      Guid accountId)
    {
      bool flag = true;
      if (!extension.Flags.HasFlag((Enum) PublishedExtensionFlags.Public))
      {
        flag = false;
        if (accountId != Guid.Empty && extension.SharedWith != null)
        {
          string str = accountId.ToString();
          foreach (ExtensionShare extensionShare in extension.SharedWith)
          {
            if (extensionShare.Id.Equals(str, StringComparison.OrdinalIgnoreCase))
            {
              flag = true;
              break;
            }
          }
        }
      }
      return flag;
    }

    private void AddOtherCategoryIfNoneExists(
      InstallationTarget[] installationTargetArray,
      ref IEnumerable<string> categories)
    {
      if (!categories.IsNullOrEmpty<string>() || ((IEnumerable<InstallationTarget>) installationTargetArray).Any<InstallationTarget>((Func<InstallationTarget, bool>) (t => string.Equals(t.Target, "Microsoft.VisualStudio.Offer", StringComparison.OrdinalIgnoreCase))))
        return;
      categories = (IEnumerable<string>) new string[1]
      {
        "Other"
      };
    }

    private void ReadExtensionProperties(
      IVssRequestContext requestContext,
      PublishedExtension extension)
    {
      this.ReadExtensionProperties(requestContext, (IEnumerable<PublishedExtension>) new PublishedExtension[1]
      {
        extension
      });
    }

    private void ReadExtensionProperties(
      IVssRequestContext requestContext,
      IEnumerable<PublishedExtension> extensions)
    {
      Dictionary<string, ExtensionVersion> dictionary = new Dictionary<string, ExtensionVersion>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      ITeamFoundationPropertyService service = requestContext.GetService<ITeamFoundationPropertyService>();
      List<ArtifactSpec> artifactSpecList = new List<ArtifactSpec>();
      foreach (PublishedExtension extension in extensions)
      {
        if (extension.Versions != null)
        {
          foreach (ExtensionVersion version in extension.Versions)
          {
            ArtifactSpec versionArtifactSpec = GalleryServerUtil.GetExtensionVersionArtifactSpec(extension, version.Version, version.TargetPlatform);
            dictionary[versionArtifactSpec.Moniker] = version;
            artifactSpecList.Add(versionArtifactSpec);
          }
        }
      }
      using (TeamFoundationDataReader properties = service.GetProperties(requestContext, (IEnumerable<ArtifactSpec>) artifactSpecList, (IEnumerable<string>) null))
      {
        if (properties == null)
          return;
        foreach (ArtifactPropertyValue current in properties.CurrentEnumerable<ArtifactPropertyValue>())
        {
          foreach (PropertyValue propertyValue in current.PropertyValues)
          {
            if (propertyValue != null)
            {
              if (dictionary[current.Spec.Moniker].Properties == null)
                dictionary[current.Spec.Moniker].Properties = new List<KeyValuePair<string, string>>();
              dictionary[current.Spec.Moniker].Properties.Add(new KeyValuePair<string, string>(propertyValue.PropertyName, propertyValue.Value == null ? (string) null : propertyValue.Value.ToString()));
            }
          }
        }
      }
    }

    public void UpdateExtensionInstallationTarget(
      IVssRequestContext requestContext,
      PublishedExtension extension)
    {
      bool isExtensionPublic = extension.Flags.HasFlag((Enum) PublishedExtensionFlags.Public);
      using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
        component.UpdateExtensionInstallationTargets(extension.Publisher.PublisherName, extension.ExtensionName, isExtensionPublic, (IEnumerable<InstallationTarget>) extension.InstallationTargets);
    }

    public void WriteExtensionProperties(
      IVssRequestContext requestContext,
      PublishedExtension extension)
    {
      ArgumentUtility.CheckForNull<PublishedExtension>(extension, nameof (extension));
      ITeamFoundationPropertyService service = requestContext.GetService<ITeamFoundationPropertyService>();
      List<ArtifactPropertyValue> artifactPropertyValueList1 = new List<ArtifactPropertyValue>();
      List<ArtifactPropertyValue> artifactPropertyValueList2 = new List<ArtifactPropertyValue>();
      List<ArtifactPropertyValue> artifactPropertyValueList3 = new List<ArtifactPropertyValue>();
      List<PropertyValue> propertyValueList1 = new List<PropertyValue>();
      List<PropertyValue> propertyValueList2 = new List<PropertyValue>();
      foreach (ExtensionVersion version in extension.Versions)
      {
        List<PropertyValue> propertyValueList3 = new List<PropertyValue>();
        if (version.Properties != null)
        {
          foreach (KeyValuePair<string, string> property in version.Properties)
          {
            if (property.Key.StartsWith("Microsoft.VisualStudio.Services.TaskId", StringComparison.OrdinalIgnoreCase))
              propertyValueList1.Add(new PropertyValue(property.Key, (object) property.Value));
            else if (property.Key.StartsWith("Microsoft.VisualStudio.Services.ServiceEndpointName", StringComparison.OrdinalIgnoreCase))
              propertyValueList2.Add(new PropertyValue(property.Key, (object) property.Value));
            else
              propertyValueList3.Add(new PropertyValue(property.Key, (object) property.Value));
          }
        }
        ArtifactPropertyValue artifactPropertyValue = new ArtifactPropertyValue(GalleryServerUtil.GetExtensionVersionArtifactSpec(extension, version.Version, version.TargetPlatform), (IEnumerable<PropertyValue>) propertyValueList3);
        artifactPropertyValueList1.Add(artifactPropertyValue);
      }
      service.SetProperties(requestContext, (IEnumerable<ArtifactPropertyValue>) artifactPropertyValueList1);
      if (propertyValueList1.Count > 0)
      {
        ArtifactPropertyValue artifactPropertyValue = new ArtifactPropertyValue(GalleryServerUtil.GetExtensionNameArtifactSpec(extension), (IEnumerable<PropertyValue>) propertyValueList1);
        artifactPropertyValueList2.Add(artifactPropertyValue);
        service.SetProperties(requestContext, (IEnumerable<ArtifactPropertyValue>) artifactPropertyValueList2);
      }
      if (propertyValueList2.Count <= 0)
        return;
      ArtifactPropertyValue artifactPropertyValue1 = new ArtifactPropertyValue(GalleryServerUtil.GetExtensionNameArtifactSpec(extension), (IEnumerable<PropertyValue>) propertyValueList2);
      artifactPropertyValueList3.Add(artifactPropertyValue1);
      service.SetProperties(requestContext, (IEnumerable<ArtifactPropertyValue>) artifactPropertyValueList3);
    }

    private ExtensionQueryFlags ProcessQueryFlags(ExtensionQueryFlags flags)
    {
      if (flags.HasFlag((Enum) ExtensionQueryFlags.IncludeVersionProperties))
        flags |= ExtensionQueryFlags.IncludeVersions;
      return flags;
    }

    private List<KeyValuePair<int, string>> GetIndexedTerms(
      IVssRequestContext requestContext,
      string displayName,
      PublishedExtensionFlags flags,
      IEnumerable<InstallationTarget> installationTargets,
      IEnumerable<string> categories,
      IEnumerable<string> tags,
      bool includeInstallationTargets)
    {
      List<KeyValuePair<int, string>> indexedTerms = new List<KeyValuePair<int, string>>();
      if (installationTargets != null && !includeInstallationTargets)
      {
        foreach (InstallationTarget installationTarget in installationTargets)
          indexedTerms.Add(new KeyValuePair<int, string>(7, installationTarget.Target));
      }
      string installationTargets1 = GalleryUtil.GetProductTypeForInstallationTargets(installationTargets);
      indexedTerms.AddRange((IEnumerable<KeyValuePair<int, string>>) this.ConvertCategoryNamesToTags(requestContext, categories, "en-us", installationTargets1, false));
      indexedTerms.AddRange((IEnumerable<KeyValuePair<int, string>>) this.GetInternalTags(flags));
      if (tags != null)
      {
        foreach (string tag1 in tags)
        {
          TagType key = TagType.BasicTag;
          string tag2 = tag1;
          if (tag1.StartsWith("#", StringComparison.OrdinalIgnoreCase))
          {
            GallerySecurity.CheckRootPermission(requestContext, PublisherPermissions.TrustedPartner);
            if (!(tag1.Substring(1, 3) == "CT:"))
              throw new InvalidTagException(GalleryResources.InvalidTag((object) tag1));
            key = TagType.ContributionType;
            tag2 = tag1.Substring(4);
          }
          if (tag2.StartsWith("$", StringComparison.OrdinalIgnoreCase) && GallerySecurity.HasRootPermission(requestContext, PublisherPermissions.TrustedPartner))
          {
            GalleryUtil.CheckTag(tag2.Substring(1));
            if (tag2.Equals("$featured", StringComparison.OrdinalIgnoreCase))
              key = TagType.Featured;
          }
          else
            GalleryUtil.CheckTag(tag2);
          indexedTerms.Add(new KeyValuePair<int, string>((int) key, tag2));
        }
      }
      if (flags.HasFlag((Enum) PublishedExtensionFlags.Public) && !flags.HasFlag((Enum) PublishedExtensionFlags.System))
      {
        string str1 = displayName;
        char[] separator = new char[4]{ ' ', ',', ';', '.' };
        foreach (string str2 in str1.Split(separator, StringSplitOptions.RemoveEmptyEntries))
          indexedTerms.Add(new KeyValuePair<int, string>(5, str2));
      }
      return indexedTerms;
    }

    private static void PublishCustomerIntelligenceEventForPublisherNameValidation(
      IVssRequestContext requestContext,
      string action,
      ExtensionVersionToPublish extensionToPublish)
    {
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      intelligenceData.Add(CustomerIntelligenceProperty.Action, action);
      intelligenceData.Add("Publisher", extensionToPublish.PublisherName);
      intelligenceData.Add("ExtensionName", extensionToPublish.ExtensionName);
      intelligenceData.Add("ExtensionId", (object) extensionToPublish.ExtensionId);
      intelligenceData.Add("ExtensionVersion", extensionToPublish.Version);
      intelligenceData.Add("TargetPlatform", extensionToPublish.TargetPlatform);
      if (extensionToPublish.InstallationTargets != null)
      {
        string str = string.Join(";", extensionToPublish.InstallationTargets.Select<InstallationTarget, string>((Func<InstallationTarget, string>) (x => x == null ? string.Empty : x.Target)));
        intelligenceData.Add("InstallationTargets", str);
        intelligenceData.Add("ProductType", GalleryUtil.GetProductTypeForInstallationTargets(extensionToPublish.InstallationTargets));
      }
      intelligenceData.Add("ErrorMessage", "Unable to parse the publisher info from package.json file");
      intelligenceData.AddGalleryUserIdentifier(requestContext);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", "Published", intelligenceData);
    }

    private static void PublishCustomerIntelligenceEvent(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      string action,
      string account = null,
      IEnumerable<InstallationTarget> installationTargets = null,
      TagType? tagType = null,
      string tagValue = null,
      string previousVersion = null,
      string newVersion = null,
      long packageSize = 0,
      bool logTelemetryForOnPremise = false,
      string version = null,
      IEnumerable<string> categories = null,
      string targetPlatform = null)
    {
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      intelligenceData.Add(CustomerIntelligenceProperty.Action, action);
      intelligenceData.Add("Publisher", extension.Publisher.PublisherName);
      intelligenceData.Add("Name", extension.ExtensionName);
      intelligenceData.Add("PublisherDisplayName", extension.Publisher.DisplayName);
      intelligenceData.Add("ExtensionDisplayName", extension.DisplayName);
      intelligenceData.Add("ExtensionState", string.Equals(action, "Deleted", StringComparison.OrdinalIgnoreCase) ? "Deleted" : GalleryServerUtil.GetExtensionState(extension.Flags));
      intelligenceData.Add("LastUpdatedDate", extension.LastUpdated.ToString("o"));
      intelligenceData.Add("CreatedDate", extension.PublishedDate.ToString("o"));
      intelligenceData.Add("ReleasedDate", extension.ReleaseDate != DateTime.MinValue ? extension.ReleaseDate.ToString("o") : string.Empty);
      intelligenceData.Add(nameof (targetPlatform), targetPlatform);
      if (!string.IsNullOrEmpty(version))
        intelligenceData.Add("Version", version);
      if (!string.IsNullOrEmpty(account))
        intelligenceData.Add("Account", account);
      if (installationTargets != null)
      {
        string str = string.Join(";", installationTargets.Select<InstallationTarget, string>((Func<InstallationTarget, string>) (x => x == null ? string.Empty : x.Target)));
        intelligenceData.Add("InstallationTargets", str);
        intelligenceData.Add("ProductType", GalleryUtil.GetProductTypeForInstallationTargets(installationTargets));
      }
      if (packageSize > 0L)
        intelligenceData.Add("PackageSize", packageSize.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (tagType.HasValue)
        intelligenceData.Add("TagType", (object) tagType.Value);
      if (!string.IsNullOrEmpty(tagValue))
        intelligenceData.Add("TagValue", tagValue);
      intelligenceData.Add("VerifiedPublisher", extension.Publisher.Flags.HasFlag((Enum) PublisherFlags.Verified));
      intelligenceData.Add("PublicExtension", extension.Flags.HasFlag((Enum) PublishedExtensionFlags.Public));
      intelligenceData.Add("PreviewExtension", extension.Flags.HasFlag((Enum) PublishedExtensionFlags.Preview));
      if (!previousVersion.IsNullOrEmpty<char>() && !newVersion.IsNullOrEmpty<char>())
      {
        intelligenceData.Add("PreviousVersion", previousVersion);
        intelligenceData.Add("NewVersion", newVersion);
      }
      if (categories != null)
      {
        string str = string.Join(";", categories);
        intelligenceData.Add("Categories", str);
        intelligenceData.Add("CountCategories", (double) categories.Count<string>());
      }
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
      {
        if (!logTelemetryForOnPremise)
          return;
        requestContext.GetService<IGalleryTelemetryHelperService>().PublishAppInsightsTelemetry(requestContext, action, false);
      }
      else
      {
        intelligenceData.AddGalleryUserIdentifier(requestContext);
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", "Extension", intelligenceData);
      }
    }

    private void PublishCategoryIntelligenceEvent(
      IVssRequestContext requestContext,
      ExtensionCategory category,
      string action)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add(CustomerIntelligenceProperty.Action, action);
      properties.Add("CategoryName", category.CategoryName);
      if (!category.LanguageTitles.IsNullOrEmpty<CategoryLanguageTitle>())
      {
        string str = "";
        foreach (CategoryLanguageTitle languageTitle in category.LanguageTitles)
          str = str + languageTitle.Lang + ", ";
        properties.Add("Language", str);
      }
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", "Category", properties);
    }

    private void PublishEvent(
      IVssRequestContext requestContext,
      ExtensionEventType eventType,
      PublishedExtension extension,
      string version,
      Guid accountId = default (Guid))
    {
      if (!GalleryUtil.HasInterestingTargetsForEMS((IEnumerable<InstallationTarget>) extension.InstallationTargets))
        return;
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      extension.Flags &= ~PublishedExtensionFlags.Unpublished;
      ExtensionChangeEvent extensionChangeEvent = new ExtensionChangeEvent()
      {
        EventType = eventType,
        PublisherName = extension.Publisher.PublisherName,
        ExtensionName = extension.ExtensionName,
        Flags = (int) extension.Flags,
        Version = !string.IsNullOrEmpty(version) ? version : string.Empty,
        HostId = accountId
      };
      IMessageBusPublisherService service = vssRequestContext.GetService<IMessageBusPublisherService>();
      ServiceEvent serviceEvent = new ServiceEvent()
      {
        EventType = extensionChangeEvent.EventType.ToString(),
        Resource = (object) extensionChangeEvent,
        ResourceVersion = GalleryConstants.MessageVersions[0],
        Publisher = new Microsoft.VisualStudio.Services.WebApi.Publisher()
        {
          Name = "Gallery",
          ServiceOwnerId = new Guid("00000029-0000-8888-8000-000000000000")
        }
      };
      if (!requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
      {
        service.Publish(vssRequestContext, "Microsoft.VisualStudio.Services.Gallery", (object[]) new ServiceEvent[1]
        {
          serviceEvent
        });
      }
      else
      {
        ExtensionChangeNotification objectToSerialize = new ExtensionChangeNotification();
        objectToSerialize.EventType = extensionChangeEvent.EventType;
        objectToSerialize.ExtensionName = extensionChangeEvent.ExtensionName;
        objectToSerialize.PublisherName = extensionChangeEvent.PublisherName;
        objectToSerialize.Version = extensionChangeEvent.Version;
        objectToSerialize.Flags = extensionChangeEvent.Flags;
        objectToSerialize.HostId = extensionChangeEvent.HostId;
        string eventData = TeamFoundationSerializationUtility.SerializeToString<ExtensionChangeNotification>(objectToSerialize);
        vssRequestContext.GetService<ITeamFoundationSqlNotificationService>().SendNotification(vssRequestContext, PublishedExtensionService.s_extensionChanged, eventData);
      }
    }

    public void QueueValidation(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      string version,
      string targetPlatform = null,
      bool validateNow = false)
    {
      Guid empty = Guid.Empty;
      ExtensionVersionValidation versionValidation;
      using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
      {
        try
        {
          versionValidation = component.QueryVersionValidation(extension.Publisher.PublisherName, extension.ExtensionName, version, targetPlatform);
        }
        catch (NotImplementedException ex)
        {
          versionValidation = component.QueryVersionValidation(extension.Publisher.PublisherName, extension.ExtensionName, version);
        }
      }
      if (versionValidation == null)
        return;
      Guid validationId = versionValidation.ValidationId;
      Guid userVsid = GalleryServerUtil.GetUserVsid(requestContext);
      if (validateNow)
      {
        string resultMessage;
        if (requestContext.GetService<IProcessExtensionService>().ProcessExtension(requestContext, extension.Publisher.PublisherName, extension.ExtensionName, extension.ExtensionId, version, targetPlatform, validationId, userVsid, out resultMessage))
          return;
        requestContext.Trace(12061088, TraceLevel.Warning, "gallery", nameof (PublishedExtensionService), resultMessage);
      }
      else
      {
        ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
        XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) new ExtensionProcessingJobData()
        {
          ExtensionId = extension.ExtensionId,
          PublisherName = extension.Publisher.PublisherName,
          ExtensionName = extension.ExtensionName,
          Version = version,
          TargetPlatform = targetPlatform,
          ValidationId = validationId,
          Vsid = new Guid?(userVsid)
        });
        string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Process Version Validation (validationId: {0})", (object) validationId);
        IVssRequestContext requestContext1 = requestContext;
        string jobName = str;
        XmlNode jobData = xml;
        service.QueueOneTimeJob(requestContext1, jobName, "Microsoft.VisualStudio.Services.Gallery.Extensions.PostUploadExtensionProcessingJob", jobData, true);
      }
    }

    private static bool IsInvalidTrialExtensionTargets(InstallationTarget[] installationTargetArray) => installationTargetArray == null || !((IEnumerable<InstallationTarget>) installationTargetArray).Any<InstallationTarget>((Func<InstallationTarget, bool>) (t => string.Equals(t.Target, "Microsoft.VisualStudio.Services.Integration", StringComparison.OrdinalIgnoreCase) || string.Equals(t.Target, "Microsoft.VisualStudio.Services.Cloud.Integration", StringComparison.OrdinalIgnoreCase) || string.Equals(t.Target, "Microsoft.TeamFoundation.Server.Integration", StringComparison.OrdinalIgnoreCase) || string.Equals(t.Target, "Microsoft.VisualStudio.Ide", StringComparison.OrdinalIgnoreCase) || string.Equals(t.Target, "Microsoft.VisualStudio.Code", StringComparison.OrdinalIgnoreCase)));

    public void QueueVSCodeWebExtensionTagPopulatorJob(
      IVssRequestContext requestContext,
      PublishedExtension extension)
    {
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) new VSCodeWebExtensionTagPopulatorJobData()
      {
        PublisherName = extension.Publisher.PublisherName,
        ExtensionName = extension.ExtensionName
      });
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Queuing web extension tag populator job for product (extension: {0})", (object) extension.ExtensionId.ToString());
      IVssRequestContext requestContext1 = requestContext;
      string jobName = str;
      XmlNode jobData = xml;
      service.QueueOneTimeJob(requestContext1, jobName, "Microsoft.VisualStudio.Services.Gallery.Extensions.JobAgentPlugins.VSCodeWebExtensionTagPopulatorJob", jobData, false);
    }

    private bool IsInvalidPaidExtensionTargets(
      InstallationTarget[] installationTargetArray,
      string publisherDisplayName)
    {
      if (installationTargetArray == null)
        return true;
      return !GalleryUtil.IsFirstParty(publisherDisplayName) && !((IEnumerable<InstallationTarget>) installationTargetArray).Any<InstallationTarget>((Func<InstallationTarget, bool>) (t => string.Equals(t.Target, "Microsoft.VisualStudio.Services", StringComparison.OrdinalIgnoreCase) || string.Equals(t.Target, "Microsoft.VisualStudio.Services.Cloud", StringComparison.OrdinalIgnoreCase) || string.Equals(t.Target, "Microsoft.TeamFoundation.Server", StringComparison.OrdinalIgnoreCase) || string.Equals(t.Target, "Microsoft.VisualStudio.Services.Integration", StringComparison.OrdinalIgnoreCase) || string.Equals(t.Target, "Microsoft.TeamFoundation.Server.Integration", StringComparison.OrdinalIgnoreCase) || string.Equals(t.Target, "Microsoft.VisualStudio.Services.Cloud.Integration", StringComparison.OrdinalIgnoreCase) || string.Equals(t.Target, "Microsoft.VisualStudio.Ide", StringComparison.OrdinalIgnoreCase)));
    }

    private void ValidatePaidExtensionStateTransition(
      PublishedExtensionFlags flags,
      PublishedExtension extension)
    {
      if (extension.IsVsExtension())
        return;
      if (flags.HasFlag((Enum) PublishedExtensionFlags.Paid) && !this.CanConvertExtensionToPaid(extension))
        throw new InvalidExtensionDefinitionException(GalleryResources.ConversionToPaidRestricted());
      if (this.CanConvertExtensionFromPaidToFree(flags, extension) && !this.IsExtensionAllowedToConvertFromPaidToFree(extension))
        throw new InvalidExtensionDefinitionException(GalleryResources.InvalidExtensionDefinition((object) GalleryResources.PaidExtensionCantBeMadeFree()));
    }

    private void ValidatePaidExtensionForSupportLink(
      PublishedExtensionFlags flags,
      List<Microsoft.VisualStudio.Services.Gallery.WebApi.ExtensionProperty> extensionProperties,
      InstallationTarget[] installationsTargets)
    {
      if (GalleryUtil.InstallationTargetsHasVSTS((IEnumerable<InstallationTarget>) installationsTargets) && flags.HasFlag((Enum) PublishedExtensionFlags.Paid) && !PublishedExtensionService.ContainSupportProperty(extensionProperties))
        throw new InvalidExtensionDefinitionException(GalleryResources.InvalidExtensionDefinition((object) GalleryResources.PaidExtensionShouldContainSupportLink()));
    }

    private static bool ContainSupportProperty(List<Microsoft.VisualStudio.Services.Gallery.WebApi.ExtensionProperty> extensionProperties)
    {
      bool flag = false;
      foreach (Microsoft.VisualStudio.Services.Gallery.WebApi.ExtensionProperty extensionProperty in extensionProperties)
      {
        if (extensionProperty.Id != null && extensionProperty.Id.Equals("Microsoft.VisualStudio.Services.Links.Support", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(extensionProperty.Value))
        {
          flag = true;
          break;
        }
      }
      return flag;
    }

    private bool CanConvertExtensionToPaid(PublishedExtension oldExtension) => oldExtension.Flags.HasFlag((Enum) PublishedExtensionFlags.Public) && oldExtension.Flags.HasFlag((Enum) PublishedExtensionFlags.Preview) || !oldExtension.Flags.HasFlag((Enum) PublishedExtensionFlags.Public) && oldExtension.Flags.HasFlag((Enum) PublishedExtensionFlags.Preview) || !oldExtension.Flags.HasFlag((Enum) PublishedExtensionFlags.Public) || oldExtension.Flags.HasFlag((Enum) PublishedExtensionFlags.Paid);

    private List<string> SynchronizePaidFlagAndPaidTag(
      ref PublishedExtensionFlags flags,
      IEnumerable<string> tags,
      PublishedExtension extension = null)
    {
      List<string> stringList = (tags != null ? tags.ToList<string>() : (List<string>) null) ?? new List<string>();
      if (extension != null && this.CanConvertExtensionFromPaidToFree(flags, extension) && stringList.Find((Predicate<string>) (tag => tag.Equals("$IsPaid"))) != null)
        stringList.Remove("$IsPaid");
      else if (tags != null)
      {
        if (flags.HasFlag((Enum) PublishedExtensionFlags.Paid) && stringList.Find((Predicate<string>) (tag => tag.Equals("$IsPaid"))) == null)
          stringList.Add("$IsPaid");
        else if (stringList.Find((Predicate<string>) (tag => tag.Equals("$IsPaid"))) != null && !flags.HasFlag((Enum) PublishedExtensionFlags.Paid))
          flags |= PublishedExtensionFlags.Paid;
      }
      else if (flags.HasFlag((Enum) PublishedExtensionFlags.Paid))
        stringList = new List<string>() { "$IsPaid" };
      return stringList;
    }

    private bool CanConvertExtensionFromPaidToFree(
      PublishedExtensionFlags flags,
      PublishedExtension extension)
    {
      if (extension.IsVsExtension())
        return true;
      return PublishedExtensionService.IsConvertToFreeRequest(flags, extension) && extension.Flags.HasFlag((Enum) PublishedExtensionFlags.Public);
    }

    private static bool IsConvertToFreeRequest(
      PublishedExtensionFlags flags,
      PublishedExtension extension)
    {
      return !flags.HasFlag((Enum) PublishedExtensionFlags.Paid) && extension.Flags.HasFlag((Enum) PublishedExtensionFlags.Paid);
    }

    private void TryParseFullyQualifiedExtensionId(
      string extensionId,
      out string publisherName,
      out string extensionName)
    {
      if (string.IsNullOrEmpty(extensionId))
        throw new ArgumentException(GalleryResources.InvalidExtensionIdentifier((object) extensionId));
      publisherName = (string) null;
      extensionName = (string) null;
      string[] strArray = extensionId.Split('.');
      if (strArray.Length > 1)
      {
        publisherName = strArray[0];
        extensionName = strArray[1];
      }
      GalleryUtil.CheckPublisherName(publisherName);
      GalleryUtil.CheckExtensionName(extensionName);
    }

    private void FilterInternalTags(PublishedExtension extension)
    {
      if (extension.Tags == null)
        return;
      for (int index = 0; index < extension.Tags.Count; ++index)
      {
        if (GalleryServerUtil.ExtensionInternalTags.Contains(extension.Tags[index]))
          extension.Tags.RemoveAt(index--);
      }
    }

    private List<KeyValuePair<int, string>> GetInternalTags(PublishedExtensionFlags flags)
    {
      List<KeyValuePair<int, string>> internalTags = new List<KeyValuePair<int, string>>();
      if (flags.HasFlag((Enum) PublishedExtensionFlags.BuiltIn))
        internalTags.Add(new KeyValuePair<int, string>(1, "$BuiltIn"));
      return internalTags;
    }

    private void CategoryChangeCallback(
      IVssRequestContext requestContext,
      NotificationEventArgs eventArgs)
    {
      lock (this.m_loaderLock)
        this.m_categoriesRequireLoad = true;
    }

    private void EnsureCategoriesLoaded(IVssRequestContext requestContext)
    {
      if (!this.m_categoriesRequireLoad)
        return;
      lock (this.m_loaderLock)
      {
        List<ExtensionCategory> categories;
        using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
          categories = component.GetCategories(requestContext);
        PublishedExtensionService.RemoveOldVstsCategoryFromCategoryList(requestContext, ref categories);
        PublishedExtensionService.CategoriesData categoriesData = new PublishedExtensionService.CategoriesData(categories);
        foreach (ExtensionCategory extensionCategory in categories)
        {
          foreach (string associatedProduct in extensionCategory.AssociatedProducts)
          {
            PublishedExtensionService.ProductCategoryData productCategoryData;
            if (!categoriesData.ProductWiseCategoryDataMap.TryGetValue(associatedProduct, out productCategoryData))
            {
              productCategoryData = new PublishedExtensionService.ProductCategoryData();
              categoriesData.ProductWiseCategoryDataMap.Add(associatedProduct, productCategoryData);
            }
            productCategoryData.Categories.Add(extensionCategory);
          }
          foreach (CategoryLanguageTitle languageTitle in extensionCategory.LanguageTitles)
          {
            bool skipThisNameToId = false;
            string updatedCategoryTitle = this.GetUpdatedCategoryTitle(extensionCategory, languageTitle, out skipThisNameToId);
            PublishedExtensionService.CategoryIdToNameMap categoryIdToNameMap;
            if (!categoriesData.LanguageWiseIdToNameMap.TryGetValue(languageTitle.Lang, out categoryIdToNameMap))
            {
              categoryIdToNameMap = new PublishedExtensionService.CategoryIdToNameMap();
              categoriesData.LanguageWiseIdToNameMap.Add(languageTitle.Lang, categoryIdToNameMap);
            }
            categoryIdToNameMap.IdToName.Add(extensionCategory.CategoryId, updatedCategoryTitle);
            foreach (string associatedProduct in extensionCategory.AssociatedProducts)
            {
              PublishedExtensionService.ProductCategoryData wiseCategoryData = categoriesData.ProductWiseCategoryDataMap[associatedProduct];
              PublishedExtensionService.CategoryNameToIdMap categoryNameToIdMap;
              if (!wiseCategoryData.LanguageWiseNameToIdMap.TryGetValue(languageTitle.Lang, out categoryNameToIdMap))
              {
                categoryNameToIdMap = new PublishedExtensionService.CategoryNameToIdMap();
                wiseCategoryData.LanguageWiseNameToIdMap.Add(languageTitle.Lang, categoryNameToIdMap);
              }
              if (!skipThisNameToId)
                categoryNameToIdMap.NameToId.Add(updatedCategoryTitle, extensionCategory.CategoryId);
            }
            PublishedExtensionService.CategoryNameToIdMap categoryNameToIdMap1;
            if (!categoriesData.LanguageWiseNameToIdMap.TryGetValue(languageTitle.Lang, out categoryNameToIdMap1))
            {
              categoryNameToIdMap1 = new PublishedExtensionService.CategoryNameToIdMap();
              categoriesData.LanguageWiseNameToIdMap.Add(languageTitle.Lang, categoryNameToIdMap1);
            }
            categoryNameToIdMap1.NameToId.Add(updatedCategoryTitle, extensionCategory.CategoryId);
          }
        }
        this.m_categoriesData = categoriesData;
        this.m_categoriesRequireLoad = false;
      }
    }

    private string GetUpdatedCategoryTitle(
      ExtensionCategory extensionCategory,
      CategoryLanguageTitle categoryLanguageTitle,
      out bool skipThisNameToId)
    {
      skipThisNameToId = false;
      string enumerable = "";
      foreach (string associatedProduct in extensionCategory.AssociatedProducts)
      {
        if (string.Equals(associatedProduct, "vs", StringComparison.OrdinalIgnoreCase) && extensionCategory.Parent != null)
        {
          enumerable = this.GetTitleForLang(extensionCategory.Parent, categoryLanguageTitle.Lang);
          if (!enumerable.IsNullOrEmpty<char>())
          {
            enumerable += "/";
            break;
          }
          skipThisNameToId = true;
          break;
        }
      }
      return enumerable + categoryLanguageTitle.Title;
    }

    private string GetTitleForLang(ExtensionCategory category, string language)
    {
      foreach (CategoryLanguageTitle languageTitle in category.LanguageTitles)
      {
        if (string.Equals(languageTitle.Lang, language))
          return languageTitle.Title;
      }
      return string.Empty;
    }

    public List<string> ConvertCategoryIdsToNames(
      IVssRequestContext requestContext,
      IEnumerable<string> categories,
      string language,
      bool filterNullCategories = true)
    {
      List<string> names = (List<string>) null;
      if (categories != null)
      {
        names = new List<string>();
        this.EnsureCategoriesLoaded(requestContext);
        PublishedExtensionService.CategoryIdToNameMap categoryIdToNameMap;
        if (!this.m_categoriesData.LanguageWiseIdToNameMap.TryGetValue(language, out categoryIdToNameMap))
          throw new ArgumentException(GalleryResources.UnsupportedCategoryLanguage((object) language));
        foreach (string category in categories)
        {
          string str;
          categoryIdToNameMap.IdToName.TryGetValue(int.Parse(category), out str);
          names.Add(str);
        }
      }
      if (filterNullCategories && names != null)
        names.RemoveAll((Predicate<string>) (x => x.IsNullOrEmpty<char>()));
      return names;
    }

    public List<KeyValuePair<int, string>> ConvertCategoryNamesToTags(
      IVssRequestContext requestContext,
      IEnumerable<string> categories,
      string language,
      string product = null,
      bool ignoreInvalidCategoryNames = false)
    {
      List<KeyValuePair<int, string>> tags = new List<KeyValuePair<int, string>>();
      if (!string.IsNullOrWhiteSpace(product) && product.Equals(GalleryServiceConstants.SubscriptionProductType, StringComparison.InvariantCultureIgnoreCase) || categories == null)
        return tags;
      this.EnsureCategoriesLoaded(requestContext);
      PublishedExtensionService.CategoryNameToIdMap categoryNameToIdMap;
      if (!product.IsNullOrEmpty<char>())
      {
        PublishedExtensionService.ProductCategoryData productCategoryData;
        if (!this.m_categoriesData.ProductWiseCategoryDataMap.TryGetValue(product, out productCategoryData))
          throw new ArgumentException("Unsupported product");
        if (!productCategoryData.LanguageWiseNameToIdMap.TryGetValue(language, out categoryNameToIdMap))
          throw new ArgumentException(GalleryResources.UnsupportedCategoryLanguage((object) language));
      }
      else if (!this.m_categoriesData.LanguageWiseNameToIdMap.TryGetValue(language, out categoryNameToIdMap))
        throw new ArgumentException(GalleryResources.UnsupportedCategoryLanguage((object) language));
      foreach (string category in categories)
      {
        string key = category;
        if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.TreatLanguagesAsProgrammingLanguages") && key.Equals("Languages", StringComparison.OrdinalIgnoreCase))
          key = "Programming Languages";
        int num;
        if (!categoryNameToIdMap.NameToId.TryGetValue(key, out num))
        {
          if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          {
            categoryNameToIdMap.NameToId.TryGetValue("Other", out num);
          }
          else
          {
            if (!ignoreInvalidCategoryNames)
            {
              if (requestContext.UserAgent != null && requestContext.UserAgent.StartsWith("VSCode", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException(GalleryResources.UnsupportedCategory((object) language, (object) key)).Expected("Gallery");
              throw new ArgumentException(GalleryResources.UnsupportedCategory((object) language, (object) key));
            }
            continue;
          }
        }
        tags.Add(new KeyValuePair<int, string>(2, num.ToString()));
      }
      return tags;
    }

    public List<SearchCriteria> FixCategoryNamesCase(
      IVssRequestContext requestContext,
      IEnumerable<SearchCriteria> categories,
      string language)
    {
      List<SearchCriteria> searchCriteriaList = (List<SearchCriteria>) null;
      if (categories != null)
      {
        searchCriteriaList = new List<SearchCriteria>();
        this.EnsureCategoriesLoaded(requestContext);
        PublishedExtensionService.CategoryNameToIdMap categoryNameToIdMap;
        if (!this.m_categoriesData.LanguageWiseNameToIdMap.TryGetValue(language, out categoryNameToIdMap))
          throw new ArgumentException(GalleryResources.UnsupportedCategoryLanguage((object) language));
        PublishedExtensionService.CategoryIdToNameMap categoryIdToNameMap;
        if (!this.m_categoriesData.LanguageWiseIdToNameMap.TryGetValue(language, out categoryIdToNameMap))
          throw new ArgumentException(GalleryResources.UnsupportedCategoryLanguage((object) language));
        foreach (SearchCriteria category in categories)
        {
          SearchCriteria searchCriteria = category;
          int key;
          string str;
          if (categoryNameToIdMap.NameToId.TryGetValue(category.FilterValue, out key) && categoryIdToNameMap.IdToName.TryGetValue(key, out str))
            searchCriteria = new SearchCriteria()
            {
              FilterType = category.FilterType,
              FilterValue = str,
              OperatorType = category.OperatorType
            };
          searchCriteriaList.Add(searchCriteria);
        }
      }
      return searchCriteriaList;
    }

    private void ValidateIfFlagsUpdatePermitted(
      PublishedExtension extension,
      PublishedExtensionFlags flags)
    {
      if (!extension.IsVsExtension() && !extension.IsVsForMacExtension() && !flags.HasFlag((Enum) PublishedExtensionFlags.Public) && extension.Flags.HasFlag((Enum) PublishedExtensionFlags.Public))
        throw new InvalidExtensionDefinitionException(GalleryResources.InvalidExtensionDefinition((object) GalleryResources.PublicExtensionCantBeMadePrivatePublish()));
    }

    internal void ValidateExtensionImmutabilityForVSCode(
      IVssRequestContext requestContext,
      ExtensionVersionToPublish extensionToPublish,
      PublishedExtension publishedExtension)
    {
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment || !requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableExtensionVersionImmutabilityForVSCode") || !GalleryUtil.IsVSCodeInstallationTargets(extensionToPublish.InstallationTargets))
        return;
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnablePlatformSpecificExtensionsForVSCode"))
      {
        this.ValidateExtensionImmutabilityForVSCodePlatformSpecificExtensionVersion(requestContext, extensionToPublish, publishedExtension);
      }
      else
      {
        bool blockValidatingReuploads = requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.BlockValidatingExtensionReuploadsForVSCodeSpecialCase") && extensionToPublish.PublisherName == "drewgil" || requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.BlockValidatingExtensionReuploadsForVSCode");
        if (publishedExtension.Versions.Any<ExtensionVersion>((Func<ExtensionVersion, bool>) (v =>
        {
          if (!v.Version.Equals(extensionToPublish.Version))
            return false;
          return blockValidatingReuploads || v.Flags == ExtensionVersionFlags.Validated;
        })))
        {
          PublishedExtensionService.PublishVsCodeExtensionImmutabilityEvent(requestContext, extensionToPublish);
          throw new ExtensionVersionAlreadyExistsException(GalleryResources.ErrorExtensionVersionAlreadyExists((object) extensionToPublish.Version));
        }
      }
    }

    internal void ValidateExtensionImmutabilityForVSCodePlatformSpecificExtensionVersion(
      IVssRequestContext requestContext,
      ExtensionVersionToPublish extensionToPublish,
      PublishedExtension publishedExtension)
    {
      if (((!requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.BlockValidatingExtensionReuploadsForVSCodeSpecialCase") ? 0 : (extensionToPublish.PublisherName == "drewgil" ? 1 : 0)) != 0 || requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.BlockValidatingExtensionReuploadsForVSCode") ? publishedExtension.Versions : publishedExtension.Versions.Where<ExtensionVersion>((Func<ExtensionVersion, bool>) (extensionVersion => extensionVersion.Flags == ExtensionVersionFlags.Validated)).ToList<ExtensionVersion>()).Find((Predicate<ExtensionVersion>) (v => v.Version.Equals(extensionToPublish.Version) && string.Equals(v.TargetPlatform, extensionToPublish.TargetPlatform))) == null)
        return;
      PublishedExtensionService.PublishVsCodeExtensionImmutabilityEvent(requestContext, extensionToPublish);
      if (extensionToPublish.TargetPlatform != null)
        throw new ExtensionVersionWithTargetPlatformAlreadyExistsException(GalleryResources.ErrorExtensionVersionWithTargetPlatformAlreadyExists((object) extensionToPublish.TargetPlatform, (object) extensionToPublish.DisplayName, (object) extensionToPublish.Version));
      throw new ExtensionVersionAlreadyExistsException(GalleryResources.ErrorExtensionVersionAlreadyExists((object) extensionToPublish.Version));
    }

    private static void PublishVsCodeExtensionImmutabilityEvent(
      IVssRequestContext requestContext,
      ExtensionVersionToPublish extensionToPublish)
    {
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      intelligenceData.Add(CustomerIntelligenceProperty.Action, "VsCodeVersionImmutability");
      intelligenceData.Add("PublisherName", extensionToPublish.PublisherName);
      intelligenceData.Add("ExtensionName", extensionToPublish.ExtensionName);
      intelligenceData.Add("ExtensionDisplayName", extensionToPublish.DisplayName);
      intelligenceData.Add("Version", extensionToPublish.Version);
      intelligenceData.Add("TargetPlatform", extensionToPublish.TargetPlatform);
      intelligenceData.AddGalleryUserIdentifier(requestContext);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", "Extension", intelligenceData);
    }

    private static void ValidateIfDeleteVersionPermitted(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      string version)
    {
      if (!GallerySecurity.HasRootPermission(requestContext, PublisherPermissions.Admin) || extension.Versions[0].Version == version)
        throw new InvalidOperationException(GalleryResources.ExtensionVersionCannotBeDeleted((object) version, (object) extension.DisplayName));
    }

    private void ValidateIfDeleteExtensionPermitted(
      IVssRequestContext requestContext,
      PublishedExtension extension)
    {
      if (GallerySecurity.HasRootPermission(requestContext, PublisherPermissions.Admin))
        return;
      PublisherPermissions requestedPermissions = PublisherPermissions.DeleteExtension;
      GallerySecurity.CheckExtensionPermission(requestContext, extension, (string) null, requestedPermissions, false);
      if (!extension.InstallationTargets.IsNullOrEmpty<InstallationTarget>())
      {
        string installationTargets = GalleryUtil.GetProductTypeForInstallationTargets((IEnumerable<InstallationTarget>) extension.InstallationTargets);
        if (string.Equals(installationTargets, "vscode", StringComparison.OrdinalIgnoreCase) || string.Equals(installationTargets, "vs", StringComparison.OrdinalIgnoreCase) || string.Equals(installationTargets, "vsformac", StringComparison.OrdinalIgnoreCase))
          return;
      }
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment || extension == null || !extension.Flags.HasFlag((Enum) PublishedExtensionFlags.Public))
        return;
      if (extension.Statistics != null)
      {
        this.FailForNonZeroExtensionInstallCount(extension);
      }
      else
      {
        using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
          extension = component.QueryExtensionById(extension.ExtensionId, (string) null, Guid.Empty, ExtensionQueryFlags.IncludeStatistics);
        if (extension.Statistics == null)
          return;
        this.FailForNonZeroExtensionInstallCount(extension);
      }
    }

    private void FailForNonZeroExtensionInstallCount(PublishedExtension extension)
    {
      if (extension.Statistics.Any<ExtensionStatistic>((Func<ExtensionStatistic, bool>) (stat => stat.StatisticName.Equals("install", StringComparison.OrdinalIgnoreCase) && stat.Value > 0.0)))
        throw new InvalidOperationException(GalleryResources.PublicExtensionWithNonZeroInstallCantBeDeleted((object) extension.DisplayName));
    }

    private void LogExternalMetadataTelemetry(
      IVssRequestContext requestContext,
      ManifestMetadata metadata)
    {
      if (metadata == null || metadata.Identity == null || metadata.Identity.Version == null)
        return;
      ClientTraceData properties = new ClientTraceData();
      string str = (string) null;
      if (metadata.Properties != null && metadata.Properties.Count != 0)
      {
        foreach (Microsoft.VisualStudio.Services.Gallery.WebApi.ExtensionProperty property in metadata.Properties)
        {
          if (StringComparer.OrdinalIgnoreCase.Equals(property.Id, "Microsoft.VisualStudio.Services.Links.GitHub"))
          {
            str = property.Value;
            break;
          }
        }
      }
      if (str != null)
        properties.Add("Github", (object) str);
      if (metadata.Badges != null && metadata.Badges.Count != 0)
      {
        for (int index = 0; index < metadata.Badges.Count; ++index)
        {
          properties.Add("BadgeLink" + (object) (index + 1), (object) metadata.Badges[index].Link);
          properties.Add("BadgeImgUri" + (object) (index + 1), (object) metadata.Badges[index].ImgUri);
        }
        properties.Add("BadgeCount", (object) metadata.Badges.Count);
      }
      if (str == null && metadata.Badges.Count == 0)
        return;
      requestContext.GetService<ClientTraceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", "Extension_Upload_External_Metadata", properties);
    }

    public PublishedExtension CreateExtensionFromUnpackagedData(
      IVssRequestContext requestContext,
      string requestingPublisherName,
      IEnumerable<ExtensionFile> uploadedAssets,
      UnpackagedExtensionData extensionData)
    {
      if (extensionData == null)
        throw new ArgumentNullException(nameof (extensionData));
      ExtensionVersionToPublish fromUnpackagedData = this.ExtractDataFromUnpackagedData(requestContext, requestingPublisherName, extensionData);
      IEnumerable<string> names = (IEnumerable<string>) this.ConvertCategoryIdsToNames(requestContext, (IEnumerable<string>) extensionData.Categories, "en-us", true);
      IEnumerable<string> tags = (IEnumerable<string>) extensionData.Tags;
      List<Microsoft.VisualStudio.Services.Gallery.WebApi.ExtensionProperty> extensionProperties = new List<Microsoft.VisualStudio.Services.Gallery.WebApi.ExtensionProperty>();
      fromUnpackagedData.Assets = uploadedAssets;
      bool shouldNotify;
      IEnumerable<InstallationTarget> installationTargets;
      this.ValidateAndProcessExtensionDataBeforeCreation(requestContext, fromUnpackagedData, names, tags, (PublisherDetails) null, extensionProperties, out shouldNotify, out installationTargets, true);
      foreach (ExtensionFile asset in fromUnpackagedData.Assets)
        asset.Version = fromUnpackagedData.Version;
      fromUnpackagedData.PrivateIdentityId = this.m_everyoneGroup.Id;
      return this.CreateExtensionInDbAndPublishEvents(requestContext, fromUnpackagedData, shouldNotify, false, 0L, (PackageDetails) null, installationTargets, names);
    }

    public PublishedExtension UpdateExtensionFromUnpackagedData(
      IVssRequestContext requestContext,
      string requestingPublisherName,
      string requestedExtensionName,
      IEnumerable<ExtensionFile> uploadedAssets,
      UnpackagedExtensionData extensionData,
      bool validationNeeded)
    {
      if (extensionData == null)
        throw new ArgumentNullException(nameof (extensionData));
      ExtensionVersionToPublish fromUnpackagedData = this.ExtractDataFromUnpackagedData(requestContext, requestingPublisherName, extensionData);
      string extensionName = fromUnpackagedData.ExtensionName;
      if (string.IsNullOrEmpty(extensionName) || !string.Equals(requestedExtensionName, extensionName, StringComparison.OrdinalIgnoreCase))
        throw new ArgumentException(GalleryResources.PathMustMatchExtension((object) requestedExtensionName, (object) extensionName));
      IEnumerable<string> names = (IEnumerable<string>) this.ConvertCategoryIdsToNames(requestContext, (IEnumerable<string>) extensionData.Categories, "en-us", true);
      IEnumerable<string> tags = (IEnumerable<string>) extensionData.Tags;
      List<Microsoft.VisualStudio.Services.Gallery.WebApi.ExtensionProperty> extensionProperties = new List<Microsoft.VisualStudio.Services.Gallery.WebApi.ExtensionProperty>();
      IEnumerable<ExtensionFile> extensionFiles = uploadedAssets;
      fromUnpackagedData.Assets = extensionFiles;
      bool queueValidation = !fromUnpackagedData.Flags.HasFlag((Enum) PublishedExtensionFlags.Validated) & validationNeeded;
      bool shouldNotify;
      string updatedVersionString;
      string originalVersionString;
      IEnumerable<InstallationTarget> installationTargets;
      this.ValidateAndProcessExtensionDataBeforeUpdate(requestContext, fromUnpackagedData, names, tags, extensionProperties, out shouldNotify, out updatedVersionString, out originalVersionString, out installationTargets, true);
      foreach (ExtensionFile asset in fromUnpackagedData.Assets)
        asset.Version = fromUnpackagedData.Version;
      fromUnpackagedData.PrivateIdentityId = this.m_everyoneGroup.Id;
      PublishedExtension extension = this.UpdateExtensionInDbAndPublishEvents(requestContext, fromUnpackagedData, shouldNotify, queueValidation, updatedVersionString, originalVersionString, false, 0L, (PackageDetails) null, installationTargets, names);
      if (!validationNeeded)
        this.ProcessExtensionDataAfterUpdate(requestContext, extension, fromUnpackagedData);
      return extension;
    }

    public void EnableVsExtensionConsolidation(
      IVssRequestContext requestContext,
      PublishedExtension extension)
    {
      using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
        component.EnableVsExtensionConsolidation(extension.ExtensionId);
    }

    private void ProcessExtensionDataAfterUpdate(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      ExtensionVersionToPublish extensionToPublish)
    {
      Guid validationId = Guid.Empty;
      using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
      {
        ExtensionVersionValidation versionValidation = component.QueryVersionValidation(extension.Publisher.PublisherName, extension.ExtensionName, extensionToPublish.Version);
        if (versionValidation == null)
          return;
        validationId = versionValidation.ValidationId;
      }
      using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
        component.ProcessValidationResult(extension.ExtensionId, extensionToPublish.Version, validationId, "", true);
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnablePassiveCVSScan"))
      {
        IVssRequestContext requestContext1 = requestContext;
        Guid guid = extension.ExtensionId;
        string extensionIds = guid.ToString();
        guid = GalleryServerUtil.GetUserVsid(requestContext);
        string id = guid.ToString();
        CVSJobHelper.ScheduleSubmitScanJob(requestContext1, extensionIds, id);
      }
      this.QueueUploadAssetsToCDNjob(requestContext, extensionToPublish.Version, extension.ExtensionId);
      try
      {
        this.m_SearchHelper.UpdateSearchIndex(requestContext, new List<PublishedExtension>()
        {
          extension
        });
      }
      catch (Exception ex)
      {
        string format = "Failed to update extension in Search index when the validation was not needed. " + string.Format("Extension id: {0}, Extension: {1}.{2}", (object) extension.ExtensionId, (object) extension.Publisher.PublisherName, (object) extension.ExtensionName) + "ExceptionMessage: " + ex.Message + ", ExceptionStack: " + ex.StackTrace;
        requestContext.TraceAlways(12060103, TraceLevel.Error, "gallery", nameof (ProcessExtensionDataAfterUpdate), format);
      }
    }

    public void UpdateExtensionInAzureSearch(
      IVssRequestContext requestContext,
      List<PublishedExtension> extensionList)
    {
      try
      {
        this.m_SearchHelper.UpdateSearchIndex(requestContext, extensionList);
      }
      catch (Exception ex)
      {
        string format = "Failed to update extension in Search index. " + string.Format("TotalExtensionCount: {0},", (object) extensionList.Count) + "ExceptionMessage: " + ex.Message + ", ExceptionStack: " + ex.StackTrace;
        requestContext.TraceAlways(12060103, TraceLevel.Error, "gallery", nameof (UpdateExtensionInAzureSearch), format);
      }
    }

    public void CreateBackConsolidationMapping(
      IVssRequestContext requestContext,
      Guid sourceExtensionId,
      string sourceExtensionVsixId,
      Guid targetExtensionId,
      string targetExtensionVsixId)
    {
      using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
      {
        component.CreateBackConsolidationMapping(sourceExtensionId, sourceExtensionVsixId, targetExtensionId, targetExtensionVsixId);
        GalleryServerUtil.NotifyGalleryDataChanged(requestContext, GalleryNotificationEventIds.BackConsolidationUpdate, "BackConsolidation");
      }
    }

    public virtual IReadOnlyDictionary<string, BackConsolidationMappingEntry> GetBackConsolidationList(
      IVssRequestContext requestContext)
    {
      CachedBackConsolidatedExtensionData cachedData = this.m_inMemoryCacheForBackConsolidation.GetCachedData(requestContext);
      if (cachedData != null)
        return (IReadOnlyDictionary<string, BackConsolidationMappingEntry>) cachedData.VsixIdToExtensionMap;
      using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
        return component.GetBackConsolidationMapping(requestContext);
    }

    public bool UploadSignatureAsset(
      IVssRequestContext requestContext,
      Stream signatureAssetStream,
      PublishedExtension publishedExtension,
      Guid validationId)
    {
      bool flag = false;
      IPublisherAssetService service = requestContext.GetService<IPublisherAssetService>();
      string cdnRootDirectory = string.Empty;
      if (publishedExtension != null)
      {
        List<ExtensionVersion> versions = publishedExtension.Versions;
        // ISSUE: explicit non-virtual call
        if ((versions != null ? (__nonvirtual (versions.Count) > 0 ? 1 : 0) : 0) != 0)
        {
          cdnRootDirectory = publishedExtension.Versions[0].CdnDirectory;
          requestContext.Trace(12062096, TraceLevel.Info, "Gallery", nameof (UploadSignatureAsset), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "CDN directory: {0}", (object) cdnRootDirectory));
        }
      }
      try
      {
        int fileService = service.UploadVsixSignatureToFileService(requestContext, signatureAssetStream);
        requestContext.Trace(12062096, TraceLevel.Info, "Gallery", nameof (UploadSignatureAsset), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "File uploaded to file service with Id: {0}", (object) fileService));
        if (fileService > 0)
        {
          using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
          {
            string str = "/extension.signature";
            List<ManifestFile> manifestFileList = new List<ManifestFile>();
            ManifestFile manifestFile = new ManifestFile();
            manifestFile.FullPath = str;
            manifestFile.ContentType = "application/zip";
            manifestFile.AssetType = "Microsoft.VisualStudio.Services.VsixSignature";
            manifestFile.Version = publishedExtension.Versions[0].Version;
            manifestFile.FileId = fileService;
            manifestFileList.Add(manifestFile);
            List<ManifestFile> assets = manifestFileList;
            component.AddAssetsForExtensionVersion(publishedExtension, Guid.Empty, (IEnumerable<ExtensionFile>) assets);
            if (validationId != Guid.Empty)
              component.AddAssetsForExtensionVersion(publishedExtension, validationId, (IEnumerable<ExtensionFile>) assets);
            requestContext.Trace(12062096, TraceLevel.Info, "Gallery", nameof (UploadSignatureAsset), "Signing asset entries updated in Gallery DB");
          }
          flag = true;
          if (!string.IsNullOrWhiteSpace(cdnRootDirectory))
          {
            requestContext.Trace(12062096, TraceLevel.Info, "Gallery", nameof (UploadSignatureAsset), "Uploading signature archive to blob storage");
            flag = service.UploadVsixSignatureToBlobStorage(requestContext, signatureAssetStream, publishedExtension, cdnRootDirectory, "Microsoft.VisualStudio.Services.VsixSignature");
            requestContext.Trace(12062096, TraceLevel.Info, "Gallery", nameof (UploadSignatureAsset), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "File uploaded Blob storage with status: {0}", (object) flag));
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12062096, "Gallery", nameof (UploadSignatureAsset), ex);
      }
      return flag;
    }

    private ExtensionVersionToPublish ExtractDataFromUnpackagedData(
      IVssRequestContext requestContext,
      string requestingPublisherName,
      UnpackagedExtensionData extensionData)
    {
      ExtensionVersionFlags versionFlags = ExtensionVersionFlags.None;
      string publisherName = extensionData.PublisherName;
      string extensionName = extensionData.ExtensionName;
      string displayName = extensionData.DisplayName;
      string version = extensionData.Version;
      string description = extensionData.Description;
      string longDescription = (string) null;
      string empty = string.Empty;
      PublishedExtensionFlags flags = extensionData.Flags;
      IEnumerable<InstallationTarget> installationTargets = (IEnumerable<InstallationTarget>) extensionData.InstallationTargets;
      string targetPlatform = (string) null;
      string str = extensionData.Metadata.Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (m => m.Key.Contains("HasConsolidatedVsix"))).FirstOrDefault<KeyValuePair<string, string>>().Value;
      if ((str == null ? 0 : (bool.TrueString.Equals(str, StringComparison.OrdinalIgnoreCase) ? 1 : 0)) != 0)
      {
        targetPlatform = Guid.NewGuid().ToString();
        foreach (InstallationTarget installationTarget in installationTargets)
        {
          installationTarget.ExtensionVersion = version;
          installationTarget.TargetPlatform = targetPlatform;
        }
      }
      if (!string.IsNullOrEmpty(requestingPublisherName) && !string.Equals(requestingPublisherName, publisherName, StringComparison.OrdinalIgnoreCase))
        throw new ArgumentException(GalleryResources.PathMustMatchPublisher((object) requestingPublisherName, (object) publisherName));
      Guid userId = requestContext.GetUserId();
      return PublishedExtensionService.GetExtensionVersionToPublish(publisherName, extensionName, displayName, userId, flags, description, longDescription, (IEnumerable<ExtensionFile>) null, (IList<KeyValuePair<int, string>>) null, (IList<KeyValuePair<string, string>>) extensionData.Metadata, (IList<int>) extensionData.Lcids, version, empty, versionFlags, Guid.Empty, Guid.NewGuid(), DateTime.UtcNow, (string) null, false, installationTargets, targetPlatform);
    }

    private PublishedExtension CreateExtensionFromStream(
      IVssRequestContext requestContext,
      Stream extensionPackageStream,
      string requestingPublisherName,
      IEnumerable<ExtensionFile> uploadedAssets,
      bool immediateVersionValidation)
    {
      PackageDetails vsixPackage = GalleryServerUtil.ParseVSIXPackage(requestContext, extensionPackageStream);
      if (GalleryServerUtil.IsVsExtensionPackage(vsixPackage))
      {
        if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.DisableVSExtensionCreation"))
          throw new HttpException(400, GalleryResources.MaintenanceMessage());
        return this.CreateOrUpdateVsExtensionFromStream(requestContext, extensionPackageStream, requestingPublisherName, (string) null, vsixPackage);
      }
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.DisableVsCodeExtensionCreation") && PublishedExtensionService.IsVsCodeExtension(vsixPackage))
        throw new HttpException(400, GalleryResources.MaintenanceMessage());
      return this.CreateNonVsExtensionFromStream(requestContext, extensionPackageStream, vsixPackage, requestingPublisherName, uploadedAssets, immediateVersionValidation);
    }

    private static bool IsVsCodeExtension(PackageDetails packageDetails)
    {
      List<InstallationTarget> installationTargetList = new List<InstallationTarget>();
      if (packageDetails != null && packageDetails.Manifest != null)
        installationTargetList = packageDetails.Manifest.Installation;
      return GalleryUtil.IsVSCodeInstallationTargets((IEnumerable<InstallationTarget>) installationTargetList);
    }

    private static IEnumerable<string> AppendTagsForByolExtension(
      List<Microsoft.VisualStudio.Services.Gallery.WebApi.ExtensionProperty> extensionProperties,
      IEnumerable<string> tags)
    {
      List<string> stringList = (List<string>) null;
      if (tags != null)
      {
        stringList = tags.ToList<string>();
        if (tags.Any<string>((Func<string, bool>) (s => s.Equals("__BYOL", StringComparison.OrdinalIgnoreCase))) || tags.Any<string>((Func<string, bool>) (s => s.Equals("__BYOLEnforced", StringComparison.OrdinalIgnoreCase))))
        {
          Microsoft.VisualStudio.Services.Gallery.WebApi.ExtensionProperty extensionProperty = extensionProperties.Find((Predicate<Microsoft.VisualStudio.Services.Gallery.WebApi.ExtensionProperty>) (x => x.Id == "Microsoft.VisualStudio.Services.GalleryProperties.TrialDays"));
          if (extensionProperty != null && extensionProperty.Value != null)
            stringList.Add(string.Format("{0}:{1}", (object) "__TrialDays", (object) extensionProperty.Value));
        }
      }
      return (IEnumerable<string>) stringList;
    }

    private static void CheckIfMaximumTagLimitExceedForExtensions(
      IVssRequestContext requestContext,
      ExtensionVersionToPublish extensionToPublish,
      IEnumerable<string> tags)
    {
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.LimitNumberOfTagsForVsIdeExtensions") && GalleryUtil.IsVSInstallationTargets(extensionToPublish.InstallationTargets))
      {
        IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
        RegistryQuery registryQuery = new RegistryQuery("/Configuration/Service/Gallery/VsIdeExtensionTagsLimitPath");
        IVssRequestContext requestContext1 = requestContext;
        ref RegistryQuery local = ref registryQuery;
        int num = service.GetValue<int>(requestContext1, in local, 10);
        if (tags != null && num >= 0 && tags.Count<string>() > num)
          throw new ExtensionTagsLimitExceededException(GalleryResources.ExtensionTagsLimitExceedException((object) num));
      }
      if (!requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.LimitNumberOfTagsForVsCodeExtensions") || !GalleryUtil.IsVSCodeInstallationTargets(extensionToPublish.InstallationTargets))
        return;
      IVssRegistryService service1 = requestContext.GetService<IVssRegistryService>();
      RegistryQuery registryQuery1 = new RegistryQuery("/Configuration/Service/Gallery/VsCodeExtensionTagsLimitPath");
      IVssRequestContext requestContext2 = requestContext;
      ref RegistryQuery local1 = ref registryQuery1;
      int num1 = service1.GetValue<int>(requestContext2, in local1, 10);
      if (tags != null && num1 >= 0 && tags.Where<string>((Func<string, bool>) (r => !r.StartsWith("__"))).Count<string>() > num1)
        throw new ExtensionTagsLimitExceededException(GalleryResources.ExtensionTagsLimitExceedException((object) num1));
    }

    private static void CheckIfCreateExtensionLimitExceed(
      IVssRequestContext requestContext,
      string publisherName)
    {
      if (!requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.LimitNumberOfExtensionCreationBySinglePublisher"))
        return;
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      RegistryQuery query = new RegistryQuery("/Configuration/Service/Gallery/ExtensionCreationLimitBySinglePublisherPath");
      int num = service.GetValue<int>(requestContext, in query, 5);
      query = new RegistryQuery("/Configuration/Service/Gallery/TimeDurationInMinutesForExtensionCreationLimitPath");
      int durationInMinutes = service.GetValue<int>(requestContext, in query, 1440);
      using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
      {
        if (component.GetNumberOfExtensionsCreatedByPublisherForDuration(publisherName, durationInMinutes) >= num)
          throw new ExtensionCreationLimitExceedException(GalleryResources.ExtensionCreationLimitExceedException((object) GalleryServerUtil.ConvertTimeSpanToReadableString(TimeSpan.FromMinutes((double) durationInMinutes))));
      }
    }

    private PublishedExtension CreateNonVsExtensionFromStream(
      IVssRequestContext requestContext,
      Stream extensionPackageStream,
      PackageDetails packageDetails,
      string requestingPublisherName,
      IEnumerable<ExtensionFile> uploadedAssets,
      bool immediateVersionValidation)
    {
      ExtensionVersionToPublish fromPackageDetails = this.ExtractDataFromPackageDetails(requestContext, packageDetails, requestingPublisherName);
      string requestingPublisherName1 = requestingPublisherName;
      if (string.IsNullOrEmpty(requestingPublisherName1))
        requestingPublisherName1 = packageDetails.Manifest.Metadata.Identity.PublisherName;
      this.ValidateVsCodePublisherName(requestContext, extensionPackageStream, requestingPublisherName1, fromPackageDetails);
      if (GalleryUtil.IsVSCodeInstallationTargets(fromPackageDetails.InstallationTargets))
      {
        PublishedExtensionService.CheckIfCreateExtensionLimitExceed(requestContext, fromPackageDetails.PublisherName);
        PublishedExtensionService.CheckIfMaximumTagLimitExceedForExtensions(requestContext, fromPackageDetails, packageDetails.Tags);
      }
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableDuplicateExtensionNameCheckForVSCode"))
        PublishedExtensionService.ValidateVSCodeExtensionUniqueName(requestContext, fromPackageDetails);
      long suggestedMaxPackageSize = GalleryServerUtil.GetSuggestedMaxPackageSize(requestContext, fromPackageDetails.InstallationTargets, fromPackageDetails.PublisherName, fromPackageDetails.ExtensionName);
      GalleryServerUtil.ValidatePackageSize(requestContext, extensionPackageStream.Length, suggestedMaxPackageSize);
      IEnumerable<string> categories = packageDetails.Categories;
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.PreventOtherWithMultipleCategories"))
        categories = GalleryUtil.GetCleanedCategoriesListForVSCode(packageDetails);
      PublisherDetails publisher = packageDetails.Manifest.Metadata.Publisher;
      List<Microsoft.VisualStudio.Services.Gallery.WebApi.ExtensionProperty> properties = packageDetails.Manifest.Metadata.Properties;
      IEnumerable<ExtensionFile> extensionFiles = (IEnumerable<ExtensionFile>) ((object) uploadedAssets ?? (object) packageDetails.Manifest.Assets);
      fromPackageDetails.Assets = extensionFiles;
      PublishedExtensionService.UpdateVsCodeExtensionPricingTag(requestContext, fromPackageDetails, properties);
      packageDetails.Tags = PublishedExtensionService.AppendTagsForByolExtension(properties, packageDetails.Tags);
      IEnumerable<string> tags = packageDetails.Tags;
      fromPackageDetails.PackageDetails = packageDetails;
      PublishedExtensionService.ValidateVSCodeExtensionDisplayName(requestContext, fromPackageDetails);
      if (PublishedExtensionService.ShouldUploadToPMP(requestContext, fromPackageDetails))
        this.UploadArtifactFilesToPMP(requestContext, extensionPackageStream, fromPackageDetails, categories, tags, properties, false, false);
      bool shouldNotify;
      IEnumerable<InstallationTarget> installationTargets;
      this.ValidateAndProcessExtensionDataBeforeCreation(requestContext, fromPackageDetails, categories, tags, publisher, properties, out shouldNotify, out installationTargets, false);
      this.UploadExtensionAssets(requestContext, fromPackageDetails, uploadedAssets, extensionPackageStream, true, ref packageDetails);
      fromPackageDetails.PrivateIdentityId = this.m_everyoneGroup.Id;
      return this.CreateExtensionInDbAndPublishEvents(requestContext, fromPackageDetails, shouldNotify, immediateVersionValidation, extensionPackageStream.Length, packageDetails, installationTargets, categories);
    }

    private static bool ShouldUploadToPMP(
      IVssRequestContext requestContext,
      ExtensionVersionToPublish extensionToPublish)
    {
      return requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnablePMPIntegration") && GalleryUtil.IsVSCodeInstallationTargets(extensionToPublish.InstallationTargets);
    }

    private void UploadExtensionAssets(
      IVssRequestContext requestContext,
      ExtensionVersionToPublish extensionToPublish,
      IEnumerable<ExtensionFile> uploadedAssets,
      Stream extensionPackageStream,
      bool isCreateExtension,
      ref PackageDetails packageDetails)
    {
      string str = (string) null;
      if (uploadedAssets == null)
      {
        HashSet<string> IncludeAssetTypes = (HashSet<string>) null;
        IPublisherAssetService service = requestContext.GetService<IPublisherAssetService>();
        if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.UploadAssetsAsync"))
        {
          IncludeAssetTypes = this.m_AssetsToPublishAtCreateUpdate;
          string message = GalleryResources.ExtensionAssetUpload(isCreateExtension ? (object) "creating" : (object) "updating");
          requestContext.Trace(12061098, TraceLevel.Info, "gallery", isCreateExtension ? "CreateExtension" : "UpdateExtension", message);
        }
        packageDetails = service.UploadAssets(requestContext, extensionPackageStream, packageDetails, IncludeAssetTypes);
        extensionToPublish.Assets = (IEnumerable<ExtensionFile>) packageDetails.Manifest.Assets;
        str = packageDetails.Manifest.AssetCDNRoot;
      }
      bool flag = !string.IsNullOrEmpty(str);
      extensionToPublish.IsCdnEnabled = flag;
      extensionToPublish.CdnDirectory = str;
    }

    private void ValidateVsCodePublisherName(
      IVssRequestContext requestContext,
      Stream extensionPackageStream,
      string requestingPublisherName,
      ExtensionVersionToPublish extensionToPublish)
    {
      if (!requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableInternalPublisherIdValidation") || !GalleryUtil.IsVSCodeInstallationTargets(extensionToPublish.InstallationTargets) || string.IsNullOrEmpty(requestingPublisherName))
        return;
      string nameFromPackageJson = VSIXPackage.ExtractPublisherNameFromPackageJson(extensionPackageStream);
      if (!string.IsNullOrWhiteSpace(nameFromPackageJson))
      {
        if (!requestingPublisherName.Equals(nameFromPackageJson, StringComparison.OrdinalIgnoreCase))
          throw new InvalidPublisherNameException(GalleryResources.InvalidPublisherName((object) requestingPublisherName, (object) nameFromPackageJson));
        bool flag;
        if (((!requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableFirstPartyOnlyPublisherSigningValidation") ? 0 : (requestContext.TryGetItem<bool>("IsPublisherSigned", out flag) ? 1 : 0)) & (flag ? 1 : 0)) != 0 && !GalleryUtil.IsFirstParty(requestingPublisherName))
          throw new InvalidPublisherNameException(GalleryResources.PublisherSignedOnlyFirstPartyExtensionsAllowedException());
      }
      else
      {
        PublishedExtensionService.PublishCustomerIntelligenceEventForPublisherNameValidation(requestContext, "VsCodePublisherIdValidationFailure", extensionToPublish);
        throw new InvalidPublisherNameException(GalleryResources.ManifestFileParsingErrorMessage());
      }
    }

    private ExtensionVersionToPublish ExtractDataFromPackageDetails(
      IVssRequestContext requestContext,
      PackageDetails packageDetails,
      string requestingPublisherName)
    {
      ExtensionVersionFlags versionFlags = ExtensionVersionFlags.None;
      string publisherName = packageDetails.Manifest.Metadata.Identity.PublisherName;
      string extensionName = packageDetails.Manifest.Metadata.Identity.ExtensionName;
      string targetPlatform = packageDetails.Manifest.Metadata.Identity.TargetPlatform;
      string displayName = packageDetails.Manifest.Metadata.DisplayName;
      string version = packageDetails.Manifest.Metadata.Identity.Version;
      string description = packageDetails.Manifest.Metadata.Description;
      string releaseNotes = packageDetails.Manifest.Metadata.ReleaseNotes;
      string empty = string.Empty;
      PublishedExtensionFlags flags = packageDetails.Manifest.Metadata.Flags;
      IEnumerable<InstallationTarget> installation = (IEnumerable<InstallationTarget>) packageDetails.Manifest.Installation;
      if (!string.IsNullOrEmpty(requestingPublisherName) && !string.Equals(requestingPublisherName, publisherName, StringComparison.OrdinalIgnoreCase))
        throw new ArgumentException(GalleryResources.PathMustMatchPublisher((object) requestingPublisherName, (object) publisherName));
      Guid userId = requestContext.GetUserId();
      return PublishedExtensionService.GetExtensionVersionToPublish(publisherName, extensionName, displayName, userId, flags, description, releaseNotes, (IEnumerable<ExtensionFile>) null, (IList<KeyValuePair<int, string>>) null, (IList<KeyValuePair<string, string>>) new List<KeyValuePair<string, string>>(), (IList<int>) null, version, empty, versionFlags, Guid.Empty, Guid.NewGuid(), DateTime.UtcNow, (string) null, false, installation, targetPlatform);
    }

    internal virtual void ValidatePackageForAssetCountLimit(
      IVssRequestContext requestContext,
      IEnumerable<InstallationTarget> installationTargets,
      bool isCreateFlow,
      ExtensionVersionToPublish extensionToPublish)
    {
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment || !requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.AssetCountLimitValidation") || GalleryServerUtil.IsMsPublisher(extensionToPublish.PublisherName))
        return;
      int num1 = extensionToPublish.PackageDetails != null ? extensionToPublish.PackageDetails.Manifest.Assets.Count<ManifestFile>((Func<ManifestFile, bool>) (t => t.Addressable)) : extensionToPublish.Assets.Count<ExtensionFile>();
      if (installationTargets == null || num1 <= 0)
        return;
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string empty = string.Empty;
      string registryPathPattern;
      int num2;
      if (GalleryUtil.IsVSTSOrTFSInstallationTargets(installationTargets) || GalleryUtil.IsVSSExtensionInstallationTarget(installationTargets))
      {
        if (isCreateFlow)
        {
          registryPathPattern = "/Configuration/Service/Gallery/AssetCountLimit/VSTSNew";
          num2 = 1000;
        }
        else
        {
          registryPathPattern = "/Configuration/Service/Gallery/AssetCountLimit/VSTSUpdate";
          num2 = 5000;
        }
      }
      else if (GalleryUtil.IsVSCodeInstallationTargets(installationTargets))
      {
        registryPathPattern = "/Configuration/Service/Gallery/AssetCountLimit/VSCode";
        num2 = 50;
      }
      else if (GalleryUtil.IsVSInstallationTargets(installationTargets))
      {
        registryPathPattern = "/Configuration/Service/Gallery/AssetCountLimit/VSIde";
        num2 = 250;
      }
      else if (GalleryUtil.IsVSForMacInstallationTargets(installationTargets))
      {
        registryPathPattern = "/Configuration/Service/Gallery/AssetCountLimit/VSForMac";
        num2 = 250;
      }
      else
      {
        registryPathPattern = "/";
        num2 = 1000;
      }
      RegistryQuery registryQuery = new RegistryQuery(registryPathPattern);
      IVssRequestContext requestContext1 = requestContext;
      ref RegistryQuery local = ref registryQuery;
      int defaultValue = num2;
      int num3 = service.GetValue<int>(requestContext1, in local, defaultValue);
      requestContext.Trace(12062039, TraceLevel.Info, "gallery", nameof (ValidatePackageForAssetCountLimit), string.Format("Extension: {0}.{1}.{2}, registryQueryPath:{3}, isCreateFlow:{4}, assetCountLimitForProduct:{5}, assetCountInPackage:{6}", (object) extensionToPublish.PublisherName, (object) extensionToPublish.ExtensionName, (object) extensionToPublish.Version, (object) registryPathPattern, (object) isCreateFlow, (object) num3, (object) num1));
      if (num1 > num3)
        throw new ExtensionAssetCountLimitExceededException(GalleryResources.AssetCountLimitExceededMessage((object) extensionToPublish.PublisherName, (object) extensionToPublish.ExtensionName, (object) extensionToPublish.Version, (object) num3));
      if ((double) num1 <= (double) num3 * 0.85)
        return;
      requestContext.TraceAlways(12062040, TraceLevel.Info, "gallery", nameof (ValidatePackageForAssetCountLimit), string.Format("Extension: {0}.{1}.{2} is approaching asset count threshold. registryQueryPath:{3}, isCreateFlow:{4}, assetCountLimitForProduct:{5}, assetCountInPackage:{6}", (object) extensionToPublish.PublisherName, (object) extensionToPublish.ExtensionName, (object) extensionToPublish.Version, (object) registryPathPattern, (object) isCreateFlow, (object) num3, (object) num1));
    }

    private static void ValidateVSCodeExtensionUniqueName(
      IVssRequestContext requestContext,
      ExtensionVersionToPublish extensionToPublish)
    {
      using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
      {
        if (component.DoesExtensionNameExists(extensionToPublish.ExtensionName, "Microsoft.VisualStudio.Code"))
          throw new DuplicateExtensionNameException(GalleryResources.ExtensionNameAlreadyExists((object) extensionToPublish.ExtensionName));
      }
    }

    private static void ValidateVSCodeExtensionDisplayName(
      IVssRequestContext requestContext,
      ExtensionVersionToPublish extensionToPublish,
      bool isUpdateFlow = false)
    {
      if (!GalleryUtil.IsVSCodeInstallationTargets(extensionToPublish.InstallationTargets) || !requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableDuplicateExtensionDisplayNameCheckUsingDB") && !requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableTypoSquattingCheckForExtensionDisplayName"))
        return;
      bool flag1 = true;
      bool flag2 = true;
      PublishedExtension publishedExtension = (PublishedExtension) null;
      if (isUpdateFlow)
      {
        using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
          publishedExtension = component.QueryExtension(extensionToPublish.PublisherName, extensionToPublish.ExtensionName, (string) null, Guid.Empty, ExtensionQueryFlags.IncludeVersions | ExtensionQueryFlags.IncludeInstallationTargets);
        if (publishedExtension != null && !string.IsNullOrWhiteSpace(extensionToPublish.DisplayName) && !string.IsNullOrWhiteSpace(publishedExtension.DisplayName) && string.Equals(publishedExtension.DisplayName.Trim(), extensionToPublish.DisplayName.Trim(), StringComparison.OrdinalIgnoreCase))
        {
          flag1 = false;
          flag2 = false;
        }
      }
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableDuplicateExtensionDisplayNameCheckUsingDB") & flag1)
      {
        using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
        {
          if (component.DoesExtensionDisplayNameExists(extensionToPublish.DisplayName, "Microsoft.VisualStudio.Code"))
            throw new DuplicateExtensionDisplayNameException(GalleryResources.ExtensionDisplayNameAlreadyExists((object) GalleryServerUtil.EllipticalString(extensionToPublish.DisplayName, 50)));
        }
      }
      HashSet<string> collidedExtensionDisplayNames;
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableTypoSquattingCheckForExtensionDisplayName") & flag2 && requestContext.GetService<ITyposquattingService>().DoesSimilarExtensionDisplayNameExist(requestContext, extensionToPublish.DisplayName, extensionToPublish.PublisherName, out collidedExtensionDisplayNames) && (!isUpdateFlow || publishedExtension == null || !collidedExtensionDisplayNames.Contains(publishedExtension.DisplayName)))
        throw new SimilarExtensionDisplayNameException(GalleryResources.SimilarExtensionDisplayNameExists((object) string.Join(",", (IEnumerable<string>) collidedExtensionDisplayNames)));
    }

    private void ValidateAndProcessExtensionDataBeforeCreation(
      IVssRequestContext requestContext,
      ExtensionVersionToPublish extensionToPublish,
      IEnumerable<string> categories,
      IEnumerable<string> tags,
      PublisherDetails publisherInfo,
      List<Microsoft.VisualStudio.Services.Gallery.WebApi.ExtensionProperty> extensionProperties,
      out bool shouldNotify,
      out IEnumerable<InstallationTarget> installationTargets,
      bool isVsPublishAllowed)
    {
      shouldNotify = false;
      GalleryUtil.CheckPublisherName(extensionToPublish.PublisherName);
      GalleryUtil.CheckExtensionName(extensionToPublish.ExtensionName);
      PublishedExtensionService.CheckIfExtensionAlreadyExists(requestContext, extensionToPublish);
      Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher;
      try
      {
        using (PublisherComponent component = requestContext.CreateComponent<PublisherComponent>())
          publisher = component.QueryPublisher(extensionToPublish.PublisherName, PublisherQueryFlags.None);
        IFirstPartyPublisherAccessService service = requestContext.GetService<IFirstPartyPublisherAccessService>();
        if (!ServicePrincipals.IsServicePrincipal(requestContext, requestContext?.UserContext))
        {
          if (!service.CheckAtMicrosftDotComAccessIfRequired(requestContext, publisher))
            throw new NotSupportedException(GalleryResources.ErrorNotMicrosoftEmployee());
        }
      }
      catch (PublisherDoesNotExistException ex)
      {
        if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        {
          IPublisherService service = requestContext.GetService<IPublisherService>();
          string str = extensionToPublish.PublisherName;
          if (publisherInfo != null && !string.IsNullOrEmpty(publisherInfo.DisplayName))
            str = publisherInfo.DisplayName;
          IVssRequestContext requestContext1 = requestContext;
          string publisherName1 = extensionToPublish.PublisherName;
          string displayName = str;
          string publisherName2 = extensionToPublish.PublisherName;
          string publisherName3 = extensionToPublish.PublisherName;
          publisher = service.CreatePublisher(requestContext1, publisherName1, displayName, PublisherFlags.None, publisherName2, publisherName3);
        }
        else
          throw;
      }
      InstallationTarget[] installationTargetArray = (InstallationTarget[]) null;
      bool isUpdateVstsExtensionInstallationTarget = false;
      PublishedExtensionFlags flags = extensionToPublish.Flags;
      this.FixUpFieldsAndValidateCommon(requestContext, extensionToPublish.Assets, extensionToPublish.InstallationTargets, (IEnumerable<InstallationTarget>) null, ref installationTargetArray, ref flags, ref shouldNotify, ref isUpdateVstsExtensionInstallationTarget, PublishedExtensionService.isPublisherFirstParty(publisher), isVsPublishAllowed);
      installationTargets = (IEnumerable<InstallationTarget>) installationTargetArray;
      GallerySecurity.CheckPublisherPermission(requestContext, publisher, PublisherPermissions.PublishExtension);
      GallerySecurity.CheckExtensionChangePermissions(requestContext, extensionToPublish.PublisherName, PublishedExtensionFlags.None, flags, (IEnumerable<InstallationTarget>) installationTargetArray);
      this.ValidateTrialFlag(flags, installationTargetArray);
      bool isVsExtensionWithConsolidatedVsixs = GalleryServerUtil.IsVsixConsolidationEnabledForVsExtension(extensionToPublish.MetadataValues);
      PublishedExtensionService.ValidatePackageTargetPlatform(requestContext, installationTargets, extensionToPublish.TargetPlatform, isVsExtensionWithConsolidatedVsixs);
      List<string> tags1 = this.SynchronizePaidFlagAndPaidTag(ref flags, tags);
      if (requestContext.ExecutionEnvironment.IsHostedDeployment && flags.HasFlag((Enum) PublishedExtensionFlags.Paid) && this.IsInvalidPaidExtensionTargets(installationTargetArray, publisher.DisplayName))
        throw new NotSupportedException(GalleryResources.PaidSupportedInstallationTarget((object) "Microsoft.VisualStudio.Services", (object) "Microsoft.VisualStudio.Services.Cloud", (object) "Microsoft.TeamFoundation.Server", (object) "Microsoft.VisualStudio.Services.Integration", (object) "Microsoft.TeamFoundation.Server.Integration", (object) "Microsoft.VisualStudio.Services.Cloud.Integration"));
      extensionToPublish.Flags = flags;
      if (!extensionToPublish.Flags.HasFlag((Enum) PublishedExtensionFlags.Paid) && GalleryUtil.IsHostedResourceInstallationTarget((IEnumerable<InstallationTarget>) installationTargetArray))
        throw new NotSupportedException(GalleryResources.SupportTextForHostedResource((object) "Microsoft.VisualStudio.Services.Resource.Cloud"));
      this.ValidatePaidExtensionForSupportLink(extensionToPublish.Flags, extensionProperties, installationTargetArray);
      this.ValidateByolExtension(requestContext, tags, flags, extensionToPublish?.PackageDetails?.Manifest?.Assets, extensionProperties);
      this.ValidatePreviewExtensionForMicrosoftDevLabs(requestContext, publisher, flags);
      PublishedExtensionService.CheckIfMaximumTagLimitExceedForExtensions(requestContext, extensionToPublish, tags);
      this.ValidatePackageForAssetCountLimit(requestContext, installationTargets, true, extensionToPublish);
      this.ValidateS2SAuthForTrustedExtensions(requestContext, extensionToPublish, extensionToPublish.Flags);
      if (GalleryUtil.IsVSCodeInstallationTargets(installationTargets) && requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableSpamChecksForVSCode"))
        PublishedExtensionService.ValidateVSCodeExtensionForSpam(requestContext, extensionToPublish, publisher);
      categories = GalleryServerUtil.MapOldCategoriesToVerticalAlignedCategories(requestContext, flags, installationTargetArray, categories, requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.MandateNewCategoriesForVstsInCreate"));
      this.AddOtherCategoryIfNoneExists(installationTargetArray, ref categories);
      List<KeyValuePair<int, string>> indexedTerms = this.GetIndexedTerms(requestContext, extensionToPublish.DisplayName, extensionToPublish.Flags, (IEnumerable<InstallationTarget>) installationTargetArray, categories, (IEnumerable<string>) tags1, isUpdateVstsExtensionInstallationTarget);
      extensionToPublish.Tags = (IList<KeyValuePair<int, string>>) indexedTerms;
      extensionToPublish.InstallationTargets = isUpdateVstsExtensionInstallationTarget ? (IEnumerable<InstallationTarget>) (InstallationTarget[]) null : (IEnumerable<InstallationTarget>) installationTargetArray;
    }

    private void ValidateAndProcessVSCodeExtensionDataBeforeCreation(
      IVssRequestContext requestContext,
      ExtensionVersionToPublish extensionToPublish,
      IEnumerable<string> categories,
      IEnumerable<string> tags,
      PublisherDetails publisherInfo,
      Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher,
      List<Microsoft.VisualStudio.Services.Gallery.WebApi.ExtensionProperty> extensionProperties,
      ref bool shouldNotify,
      ref IEnumerable<InstallationTarget> installationTargets,
      ref PublishedExtensionFlags flags,
      bool isVsPublishAllowed,
      bool isUpdateVstsExtensionInstallationTarget)
    {
      GalleryUtil.CheckExtensionName(extensionToPublish.ExtensionName);
      PublishedExtensionService.CheckIfExtensionAlreadyExists(requestContext, extensionToPublish);
      InstallationTarget[] installationTargetArray = (InstallationTarget[]) null;
      this.FixUpFieldsAndValidateCommon(requestContext, extensionToPublish.Assets, extensionToPublish.InstallationTargets, (IEnumerable<InstallationTarget>) null, ref installationTargetArray, ref flags, ref shouldNotify, ref isUpdateVstsExtensionInstallationTarget, PublishedExtensionService.isPublisherFirstParty(publisher), isVsPublishAllowed);
      installationTargets = (IEnumerable<InstallationTarget>) installationTargetArray;
      this.ValidateTrialFlag(flags, installationTargetArray);
      bool isVsExtensionWithConsolidatedVsixs = false;
      PublishedExtensionService.ValidatePackageTargetPlatform(requestContext, installationTargets, extensionToPublish.TargetPlatform, isVsExtensionWithConsolidatedVsixs);
      List<string> tags1 = this.SynchronizePaidFlagAndPaidTag(ref flags, tags);
      if (requestContext.ExecutionEnvironment.IsHostedDeployment && flags.HasFlag((Enum) PublishedExtensionFlags.Paid) && this.IsInvalidPaidExtensionTargets(installationTargetArray, publisher.DisplayName))
        throw new NotSupportedException(GalleryResources.PaidSupportedInstallationTarget((object) "Microsoft.VisualStudio.Services", (object) "Microsoft.VisualStudio.Services.Cloud", (object) "Microsoft.TeamFoundation.Server", (object) "Microsoft.VisualStudio.Services.Integration", (object) "Microsoft.TeamFoundation.Server.Integration", (object) "Microsoft.VisualStudio.Services.Cloud.Integration"));
      extensionToPublish.Flags = flags;
      if (!extensionToPublish.Flags.HasFlag((Enum) PublishedExtensionFlags.Paid) && GalleryUtil.IsHostedResourceInstallationTarget((IEnumerable<InstallationTarget>) installationTargetArray))
        throw new NotSupportedException(GalleryResources.SupportTextForHostedResource((object) "Microsoft.VisualStudio.Services.Resource.Cloud"));
      this.ValidateByolExtension(requestContext, tags, flags, extensionToPublish?.PackageDetails?.Manifest?.Assets, extensionProperties);
      this.ValidatePreviewExtensionForMicrosoftDevLabs(requestContext, publisher, flags);
      PublishedExtensionService.CheckIfMaximumTagLimitExceedForExtensions(requestContext, extensionToPublish, tags);
      this.ValidatePackageForAssetCountLimit(requestContext, installationTargets, true, extensionToPublish);
      this.ValidateS2SAuthForTrustedExtensions(requestContext, extensionToPublish, extensionToPublish.Flags);
      categories = GalleryServerUtil.MapOldCategoriesToVerticalAlignedCategories(requestContext, flags, installationTargetArray, categories, requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.MandateNewCategoriesForVstsInCreate"));
      this.AddOtherCategoryIfNoneExists(installationTargetArray, ref categories);
      List<KeyValuePair<int, string>> indexedTerms = this.GetIndexedTerms(requestContext, extensionToPublish.DisplayName, extensionToPublish.Flags, (IEnumerable<InstallationTarget>) installationTargetArray, categories, (IEnumerable<string>) tags1, isUpdateVstsExtensionInstallationTarget);
      extensionToPublish.Tags = (IList<KeyValuePair<int, string>>) indexedTerms;
      extensionToPublish.InstallationTargets = isUpdateVstsExtensionInstallationTarget ? (IEnumerable<InstallationTarget>) (InstallationTarget[]) null : (IEnumerable<InstallationTarget>) installationTargetArray;
    }

    private static void ValidatePublisherDetails(
      IVssRequestContext requestContext,
      ExtensionVersionToPublish extensionToPublish,
      IEnumerable<string> categories,
      IEnumerable<string> tags,
      List<Microsoft.VisualStudio.Services.Gallery.WebApi.ExtensionProperty> extensionProperties,
      bool isVsPublishAllowed,
      bool isUpdateExtensionFlow,
      out bool shouldNotify,
      out IEnumerable<InstallationTarget> installationTargets,
      out Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher,
      out PublishedExtensionFlags flags,
      out bool isUpdateVstsExtensionInstallationTarget)
    {
      shouldNotify = false;
      isUpdateVstsExtensionInstallationTarget = false;
      GalleryUtil.CheckPublisherName(extensionToPublish.PublisherName);
      using (PublisherComponent component = requestContext.CreateComponent<PublisherComponent>())
        publisher = component.QueryPublisher(extensionToPublish.PublisherName, PublisherQueryFlags.None);
      IFirstPartyPublisherAccessService service = requestContext.GetService<IFirstPartyPublisherAccessService>();
      if (!ServicePrincipals.IsServicePrincipal(requestContext, requestContext?.UserContext) && !service.CheckAtMicrosftDotComAccessIfRequired(requestContext, publisher))
        throw new NotSupportedException(GalleryResources.ErrorNotMicrosoftEmployee());
      InstallationTarget[] installationTargetArray1 = (InstallationTarget[]) null;
      flags = extensionToPublish.Flags;
      if (extensionToPublish.InstallationTargets != null)
      {
        GalleryServerUtil.ParseInstallationTargetVersion(extensionToPublish.InstallationTargets);
        if (!(extensionToPublish.InstallationTargets is InstallationTarget[] installationTargetArray2))
          installationTargetArray2 = extensionToPublish.InstallationTargets.ToArray<InstallationTarget>();
        installationTargetArray1 = installationTargetArray2;
      }
      installationTargets = (IEnumerable<InstallationTarget>) installationTargetArray1;
      if (isUpdateExtensionFlow)
      {
        PublishedExtension extension;
        using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
          extension = component.QueryExtension(extensionToPublish.PublisherName, extensionToPublish.ExtensionName, (string) null, Guid.Empty, ExtensionQueryFlags.IncludeVersions | ExtensionQueryFlags.IncludeInstallationTargets);
        GallerySecurity.CheckExtensionPermission(requestContext, extension, (string) null, PublisherPermissions.UpdateExtension, false);
      }
      else
        GallerySecurity.CheckPublisherPermission(requestContext, publisher, PublisherPermissions.PublishExtension);
      GallerySecurity.CheckExtensionChangePermissions(requestContext, extensionToPublish.PublisherName, PublishedExtensionFlags.None, flags, (IEnumerable<InstallationTarget>) installationTargetArray1);
    }

    private static void CheckIfExtensionAlreadyExists(
      IVssRequestContext requestContext,
      ExtensionVersionToPublish extensionToPublish)
    {
      try
      {
        using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
        {
          if (component.QueryExtension(extensionToPublish.PublisherName, extensionToPublish.ExtensionName, (string) null, Guid.Empty, ExtensionQueryFlags.None) != null)
            throw new ExtensionExistsException(GalleryResources.ExtensionAlreadyExistsPublish());
        }
      }
      catch (ExtensionDoesNotExistException ex)
      {
      }
    }

    private static void PerformTagPermission(
      IVssRequestContext requestContext,
      IEnumerable<string> tags)
    {
      if (tags == null)
        return;
      foreach (string tag in tags)
      {
        string str;
        if ((str = tag).StartsWith("#", StringComparison.OrdinalIgnoreCase) || str.StartsWith("$", StringComparison.OrdinalIgnoreCase))
          GallerySecurity.CheckRootPermission(requestContext, PublisherPermissions.TrustedPartner);
      }
    }

    private PublishedExtension CreateExtensionInDbAndPublishEvents(
      IVssRequestContext requestContext,
      ExtensionVersionToPublish extensionToPublish,
      bool shouldNotify,
      bool immediateVersionValidation,
      long extensionPackageStreamLength,
      PackageDetails packageDetails,
      IEnumerable<InstallationTarget> installationTargets,
      IEnumerable<string> categories)
    {
      PublishedExtension extension1;
      using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
        extension1 = component.CreateExtension(requestContext, extensionToPublish);
      GallerySecurity.OnDataChanged(requestContext);
      if (shouldNotify)
      {
        string installationTargets1 = GalleryUtil.GetProductTypeForInstallationTargets(installationTargets);
        GalleryServerUtil.NotifyGalleryDataChanged(requestContext, GalleryNotificationEventIds.ExtensionUpdateDelete, "CreateExtension: " + installationTargets1 + " " + extensionToPublish.PublisherName + "." + extensionToPublish.ExtensionName);
      }
      this.PrepareExtensionForClient(requestContext, extension1, ExtensionQueryFlags.None);
      if (!extensionToPublish.Flags.HasFlag((Enum) PublishedExtensionFlags.Validated))
        this.QueueValidation(requestContext, extension1, extensionToPublish.Version, extensionToPublish.TargetPlatform, immediateVersionValidation);
      IVssRequestContext requestContext1 = requestContext;
      PublishedExtension extension2 = extension1;
      long num = extensionPackageStreamLength;
      IEnumerable<InstallationTarget> installationTargets2 = installationTargets;
      string version1 = extensionToPublish.Version;
      IEnumerable<string> strings = categories;
      string targetPlatform1 = extensionToPublish.TargetPlatform;
      TagType? tagType = new TagType?();
      long packageSize = num;
      string version2 = version1;
      IEnumerable<string> categories1 = strings;
      string targetPlatform2 = targetPlatform1;
      PublishedExtensionService.PublishCustomerIntelligenceEvent(requestContext1, extension2, "Created", installationTargets: installationTargets2, tagType: tagType, packageSize: packageSize, logTelemetryForOnPremise: true, version: version2, categories: categories1, targetPlatform: targetPlatform2);
      this.LogExternalMetadataTelemetry(requestContext, packageDetails?.Manifest.Metadata);
      return extension1;
    }

    private PublishedExtension UpdateExtensionFromStream(
      IVssRequestContext requestContext,
      Stream extensionPackageStream,
      string requestedExtensionName,
      string requestingPublisherName,
      IEnumerable<ExtensionFile> uploadedAssets,
      bool immediateVersionValidation)
    {
      PackageDetails vsixPackage = GalleryServerUtil.ParseVSIXPackage(requestContext, extensionPackageStream);
      return GalleryServerUtil.IsVsExtensionPackage(vsixPackage) ? this.CreateOrUpdateVsExtensionFromStream(requestContext, extensionPackageStream, requestingPublisherName, requestedExtensionName, vsixPackage) : this.UpdateNonVsExtensionFromStream(requestContext, extensionPackageStream, vsixPackage, requestedExtensionName, requestingPublisherName, uploadedAssets, immediateVersionValidation);
    }

    private PublishedExtension UpdateNonVsExtensionFromStream(
      IVssRequestContext requestContext,
      Stream extensionPackageStream,
      PackageDetails packageDetails,
      string requestedExtensionName,
      string requestingPublisherName,
      IEnumerable<ExtensionFile> uploadedAssets,
      bool immediateVersionValidation)
    {
      ExtensionVersionToPublish fromPackageDetails = this.ExtractDataFromPackageDetails(requestContext, packageDetails, requestingPublisherName);
      string requestingPublisherName1 = requestingPublisherName;
      if (string.IsNullOrEmpty(requestingPublisherName1))
        requestingPublisherName1 = packageDetails.Manifest.Metadata.Identity.PublisherName;
      this.ValidateVsCodePublisherName(requestContext, extensionPackageStream, requestingPublisherName1, fromPackageDetails);
      long suggestedMaxPackageSize = GalleryServerUtil.GetSuggestedMaxPackageSize(requestContext, fromPackageDetails.InstallationTargets, fromPackageDetails.PublisherName, fromPackageDetails.ExtensionName);
      GalleryServerUtil.ValidatePackageSize(requestContext, extensionPackageStream.Length, suggestedMaxPackageSize);
      string extensionName = fromPackageDetails.ExtensionName;
      if (string.IsNullOrEmpty(extensionName) || !string.Equals(requestedExtensionName, extensionName, StringComparison.OrdinalIgnoreCase))
        throw new ArgumentException(GalleryResources.PathMustMatchExtension((object) requestedExtensionName, (object) extensionName));
      IEnumerable<string> categories = packageDetails.Categories;
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.PreventOtherWithMultipleCategories"))
        categories = GalleryUtil.GetCleanedCategoriesListForVSCode(packageDetails);
      List<Microsoft.VisualStudio.Services.Gallery.WebApi.ExtensionProperty> properties = packageDetails.Manifest.Metadata.Properties;
      IEnumerable<ExtensionFile> extensionFiles = (IEnumerable<ExtensionFile>) ((object) uploadedAssets ?? (object) packageDetails.Manifest.Assets);
      fromPackageDetails.Assets = extensionFiles;
      bool queueValidation = !fromPackageDetails.Flags.HasFlag((Enum) PublishedExtensionFlags.Validated);
      PublishedExtensionService.UpdateVsCodeExtensionPricingTag(requestContext, fromPackageDetails, properties);
      PublishedExtensionService.CheckIfMaximumTagLimitExceedForExtensions(requestContext, fromPackageDetails, packageDetails.Tags);
      packageDetails.Tags = PublishedExtensionService.AppendTagsForByolExtension(properties, packageDetails.Tags);
      IEnumerable<string> tags = packageDetails.Tags;
      fromPackageDetails.PackageDetails = packageDetails;
      PublishedExtensionService.ValidateVSCodeExtensionDisplayName(requestContext, fromPackageDetails, true);
      if (PublishedExtensionService.ShouldUploadToPMP(requestContext, fromPackageDetails))
        this.UploadArtifactFilesToPMP(requestContext, extensionPackageStream, fromPackageDetails, categories, tags, properties, false, true);
      bool shouldNotify;
      string updatedVersionString;
      string originalVersionString;
      IEnumerable<InstallationTarget> installationTargets;
      this.ValidateAndProcessExtensionDataBeforeUpdate(requestContext, fromPackageDetails, categories, tags, properties, out shouldNotify, out updatedVersionString, out originalVersionString, out installationTargets, false, extensionPackageStream);
      this.UploadExtensionAssets(requestContext, fromPackageDetails, uploadedAssets, extensionPackageStream, false, ref packageDetails);
      fromPackageDetails.PrivateIdentityId = this.m_everyoneGroup.Id;
      return this.UpdateExtensionInDbAndPublishEvents(requestContext, fromPackageDetails, shouldNotify, queueValidation, updatedVersionString, originalVersionString, immediateVersionValidation, extensionPackageStream.Length, packageDetails, installationTargets, categories);
    }

    private static void UpdateVsCodeExtensionPricingTag(
      IVssRequestContext requestContext,
      ExtensionVersionToPublish extensionToPublish,
      List<Microsoft.VisualStudio.Services.Gallery.WebApi.ExtensionProperty> extensionProperties)
    {
      if (!requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnablePricingTagForVsCodeExtension") || extensionToPublish == null || extensionProperties == null || !GalleryUtil.IsVSCodeInstallationTargets(extensionToPublish.InstallationTargets))
        return;
      foreach (Microsoft.VisualStudio.Services.Gallery.WebApi.ExtensionProperty extensionProperty in extensionProperties)
      {
        if (extensionProperty.Id.Equals("Microsoft.VisualStudio.Services.Content.Pricing", StringComparison.OrdinalIgnoreCase))
        {
          if (extensionProperty.Value.Equals("Trial", StringComparison.OrdinalIgnoreCase))
          {
            extensionToPublish.Flags |= PublishedExtensionFlags.Trial;
            break;
          }
          if (extensionProperty.Value.Equals("Free", StringComparison.OrdinalIgnoreCase))
            break;
          throw new VSCodeExtensionInvalidPricingTagException(GalleryResources.VSCodeExtensionInvalidPricingTagException());
        }
      }
    }

    private void UploadArtifactFilesToPMP(
      IVssRequestContext requestContext,
      Stream extensionPackageStream,
      ExtensionVersionToPublish extensionToPublish,
      IEnumerable<string> categories,
      IEnumerable<string> tags,
      List<Microsoft.VisualStudio.Services.Gallery.WebApi.ExtensionProperty> extensionProperties,
      bool isVsPublishAllowed,
      bool isUpdateExtensionFlow)
    {
      ArgumentUtility.CheckForNull<PublishedExtensionService>(this, nameof (extensionPackageStream));
      ArgumentUtility.CheckForNull<PublishedExtensionService>(this, nameof (extensionToPublish));
      PublishedExtensionFlags flags = extensionToPublish.Flags;
      requestContext.TraceAlways(12062087, TraceLevel.Info, "gallery", nameof (UploadArtifactFilesToPMP), "UploadArtifactFilesToPMP ExtensionName {0} streamSize {1} streamPos {2} ", (object) extensionToPublish.ExtensionName, (object) extensionPackageStream.Length, (object) extensionPackageStream.Position);
      long position = extensionPackageStream.Position;
      try
      {
        PublishedExtensionService.ValidatePublisherDetails(requestContext, extensionToPublish, categories, tags, extensionProperties, false, isUpdateExtensionFlow, out bool _, out IEnumerable<InstallationTarget> _, out Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher _, out flags, out bool _);
        PublishedExtensionService.PerformTagPermission(requestContext, tags);
        requestContext.GetService<IPackageManagementPlatformServiceFacade>().UploadArtifactFile(requestContext, extensionPackageStream, extensionToPublish, "vscode", "vscodevsix");
      }
      catch (Exception ex) when (!(ex is PmpUploadServiceBadRequestException))
      {
        requestContext.TraceException(12062087, "Gallery", nameof (UploadArtifactFilesToPMP), ex);
      }
      finally
      {
        extensionPackageStream.Position = position;
      }
    }

    private void ValidateAndProcessExtensionDataBeforeUpdate(
      IVssRequestContext requestContext,
      ExtensionVersionToPublish extensionToPublish,
      IEnumerable<string> categories,
      IEnumerable<string> tags,
      List<Microsoft.VisualStudio.Services.Gallery.WebApi.ExtensionProperty> extensionProperties,
      out bool shouldNotify,
      out string updatedVersionString,
      out string originalVersionString,
      out IEnumerable<InstallationTarget> installationTargets,
      bool isVsPublishAllowed,
      Stream packageStream = null)
    {
      shouldNotify = false;
      bool isUpdateVstsExtensionInstallationTarget = false;
      updatedVersionString = string.Empty;
      originalVersionString = string.Empty;
      GalleryUtil.CheckPublisherName(extensionToPublish.PublisherName);
      GalleryUtil.CheckExtensionName(extensionToPublish.ExtensionName);
      ArgumentUtility.CheckForNull<IEnumerable<ExtensionFile>>(extensionToPublish.Assets, "assets");
      Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher;
      try
      {
        using (PublisherComponent component = requestContext.CreateComponent<PublisherComponent>())
          publisher = component.QueryPublisher(extensionToPublish.PublisherName, PublisherQueryFlags.None);
        IFirstPartyPublisherAccessService service = requestContext.GetService<IFirstPartyPublisherAccessService>();
        if (!ServicePrincipals.IsServicePrincipal(requestContext, requestContext?.UserContext))
        {
          if (!service.CheckAtMicrosftDotComAccessIfRequired(requestContext, publisher))
            throw new NotSupportedException(GalleryResources.ErrorNotMicrosoftEmployee());
        }
      }
      catch (PublisherDoesNotExistException ex)
      {
        throw;
      }
      PublishedExtension publishedExtension;
      using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
        publishedExtension = component.QueryExtension(extensionToPublish.PublisherName, extensionToPublish.ExtensionName, (string) null, Guid.Empty, ExtensionQueryFlags.IncludeVersions | ExtensionQueryFlags.IncludeInstallationTargets);
      InstallationTarget[] installationTargetArray = (InstallationTarget[]) null;
      PublishedExtensionFlags flags = extensionToPublish.Flags;
      this.FixUpFieldsAndValidateCommon(requestContext, extensionToPublish.Assets, extensionToPublish.InstallationTargets, (IEnumerable<InstallationTarget>) publishedExtension.InstallationTargets, ref installationTargetArray, ref flags, ref shouldNotify, ref isUpdateVstsExtensionInstallationTarget, PublishedExtensionService.isPublisherFirstParty(publisher), isVsPublishAllowed);
      installationTargets = (IEnumerable<InstallationTarget>) installationTargetArray;
      this.ValidateIfFlagsUpdatePermitted(publishedExtension, flags);
      bool isVsExtensionWithConsolidatedVsixs = GalleryServerUtil.IsVsixConsolidationEnabledForVsExtension(extensionToPublish.MetadataValues);
      PublishedExtensionService.ValidatePackageTargetPlatform(requestContext, installationTargets, extensionToPublish.TargetPlatform, isVsExtensionWithConsolidatedVsixs);
      this.ValidateExtensionImmutabilityForVSCode(requestContext, extensionToPublish, publishedExtension);
      PublishedExtensionService.CheckIfMaximumTagLimitExceedForExtensions(requestContext, extensionToPublish, tags);
      this.ValidatePackageForAssetCountLimit(requestContext, installationTargets, false, extensionToPublish);
      this.ValidateIfExtensionScopesChanged(requestContext, packageStream, extensionToPublish, publishedExtension);
      this.ValidateByolExtension(requestContext, tags, flags, extensionToPublish?.PackageDetails?.Manifest?.Assets, extensionProperties);
      this.ValidatePreviewExtensionForMicrosoftDevLabs(requestContext, publisher, flags);
      this.ValidateS2SAuthForTrustedExtensions(requestContext, extensionToPublish, extensionToPublish.Flags | publishedExtension.Flags);
      if (GalleryUtil.IsVSCodeInstallationTargets(installationTargets) && requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableSpamChecksForVSCodeExtensionUpdate"))
        PublishedExtensionService.ValidateVSCodeExtensionForSpam(requestContext, extensionToPublish, publisher);
      if (publishedExtension.Flags.HasFlag((Enum) PublishedExtensionFlags.Unpublished))
      {
        flags |= PublishedExtensionFlags.Unpublished;
        shouldNotify = false;
      }
      string version = extensionToPublish.Version;
      if (!flags.HasFlag((Enum) PublishedExtensionFlags.MultiVersion) && GalleryUtil.HasInterestingTargetsForEMS(extensionToPublish.InstallationTargets))
      {
        string fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(extensionToPublish.PublisherName, extensionToPublish.ExtensionName);
        Version result1;
        updatedVersionString = Version.TryParse(version, out result1) ? version : throw new InvalidVersionException(GalleryResources.InvalidVersionNumber((object) fullyQualifiedName, (object) version));
        Version result2;
        if (!Version.TryParse(publishedExtension.Versions[0].Version, out result2))
          throw new InvalidVersionException(GalleryResources.InvalidVersionNumber((object) fullyQualifiedName, (object) publishedExtension.Versions[0].Version));
        originalVersionString = publishedExtension.Versions[0].Version.ToString();
        if (result1.CompareTo(result2) <= 0)
          throw new VersionNotIncrementedException(GalleryResources.VersionNumberMustIncrease((object) fullyQualifiedName, (object) result2, (object) result1));
      }
      GallerySecurity.CheckExtensionPermission(requestContext, publishedExtension, (string) null, PublisherPermissions.UpdateExtension, false);
      GallerySecurity.CheckExtensionChangePermissions(requestContext, extensionToPublish.PublisherName, publishedExtension.Flags, flags, (IEnumerable<InstallationTarget>) installationTargetArray);
      this.ValidateTrialFlag(flags, installationTargetArray);
      List<string> tags1 = this.SynchronizePaidFlagAndPaidTag(ref flags, tags, publishedExtension);
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        if (flags.HasFlag((Enum) PublishedExtensionFlags.Paid) && this.IsInvalidPaidExtensionTargets(installationTargetArray, publishedExtension.Publisher.DisplayName))
          throw new NotSupportedException(GalleryResources.PaidSupportedInstallationTarget((object) "Microsoft.VisualStudio.Services", (object) "Microsoft.VisualStudio.Services.Cloud", (object) "Microsoft.TeamFoundation.Server", (object) "Microsoft.VisualStudio.Services.Integration", (object) "Microsoft.TeamFoundation.Server.Integration", (object) "Microsoft.VisualStudio.Services.Cloud.Integration"));
        if (!flags.HasFlag((Enum) PublishedExtensionFlags.Paid) && GalleryUtil.IsHostedResourceInstallationTarget((IEnumerable<InstallationTarget>) installationTargetArray))
          throw new NotSupportedException(GalleryResources.SupportTextForHostedResource((object) "Microsoft.VisualStudio.Services.Resource.Cloud"));
        this.ValidatePaidExtensionForSupportLink(flags, extensionProperties, installationTargetArray);
        this.ValidatePaidExtensionStateTransition(flags, publishedExtension);
      }
      if (this.CanConvertExtensionFromPaidToFree(flags, publishedExtension) && !publishedExtension.IsVsExtension() && !this.IsExtensionAllowedToConvertFromPaidToFree(publishedExtension))
        throw new InvalidExtensionDefinitionException(GalleryResources.InvalidExtensionDefinition((object) GalleryResources.PaidExtensionCantBeMadeFree()));
      categories = GalleryServerUtil.MapOldCategoriesToVerticalAlignedCategories(requestContext, flags, installationTargetArray, categories, requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.MandateNewCategoriesForVstsInUpdate"));
      this.AddOtherCategoryIfNoneExists(installationTargetArray, ref categories);
      List<KeyValuePair<int, string>> indexedTerms = this.GetIndexedTerms(requestContext, extensionToPublish.DisplayName, flags, (IEnumerable<InstallationTarget>) installationTargetArray, categories, (IEnumerable<string>) tags1, isUpdateVstsExtensionInstallationTarget);
      extensionToPublish.Tags = (IList<KeyValuePair<int, string>>) indexedTerms;
      extensionToPublish.InstallationTargets = isUpdateVstsExtensionInstallationTarget ? (IEnumerable<InstallationTarget>) (InstallationTarget[]) null : (IEnumerable<InstallationTarget>) installationTargetArray;
      extensionToPublish.Flags = flags;
      extensionToPublish.Flags |= publishedExtension.Flags & PublishedExtensionFlags.Validated;
    }

    internal virtual void ValidateByolExtension(
      IVssRequestContext requestContext,
      IEnumerable<string> tags,
      PublishedExtensionFlags flags,
      List<ManifestFile> assets,
      List<Microsoft.VisualStudio.Services.Gallery.WebApi.ExtensionProperty> extensionProperties)
    {
      bool flag1 = tags != null && (tags.Any<string>((Func<string, bool>) (s => s.Equals("__BYOL", StringComparison.OrdinalIgnoreCase))) || tags.Any<string>((Func<string, bool>) (s => s.Equals("__BYOLEnforced", StringComparison.OrdinalIgnoreCase))));
      if (flag1)
      {
        if ((flags & PublishedExtensionFlags.Paid) == PublishedExtensionFlags.None)
          throw new InvalidExtensionDefinitionException(GalleryResources.InvalidExtensionDefinition((object) GalleryResources.ByolTagOnlyApplicableForPaidExtension((object) "__BYOL", (object) "__BYOLEnforced")));
        bool flag2 = false;
        if (assets != null && assets.Count > 0)
          flag2 = assets.Any<ManifestFile>((Func<ManifestFile, bool>) (t => t.Addressable && t.AssetType.Equals("Microsoft.VisualStudio.Services.Content.Pricing", StringComparison.OrdinalIgnoreCase)));
        if (!flag2)
          throw new InvalidExtensionDefinitionException(GalleryResources.InvalidExtensionDefinition((object) GalleryResources.ByolTagNoPricingContentAvailable((object) "__BYOL", (object) "__BYOLEnforced")));
      }
      Microsoft.VisualStudio.Services.Gallery.WebApi.ExtensionProperty extensionProperty = extensionProperties.Find((Predicate<Microsoft.VisualStudio.Services.Gallery.WebApi.ExtensionProperty>) (x => x.Id.Equals("Microsoft.VisualStudio.Services.GalleryProperties.TrialDays", StringComparison.OrdinalIgnoreCase)));
      if (extensionProperty == null)
        return;
      if (!flag1)
        throw new InvalidExtensionDefinitionException(GalleryResources.InvalidExtensionDefinition((object) GalleryResources.TrialDaysWithoutByol((object) "__BYOL", (object) "__BYOLEnforced")));
      int result;
      if (!int.TryParse(extensionProperty.Value, out result) || result <= 0)
        throw new InvalidExtensionDefinitionException(GalleryResources.InvalidExtensionDefinition((object) GalleryResources.TrialDaysInvalidValue()));
    }

    internal virtual void ValidatePreviewExtensionForMicrosoftDevLabs(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher,
      PublishedExtensionFlags flags)
    {
      if (string.Equals("Microsoft DevLabs", publisher.DisplayName, StringComparison.OrdinalIgnoreCase) & (flags & PublishedExtensionFlags.Preview) != 0)
        throw new InvalidExtensionDefinitionException(GalleryResources.InvalidExtensionDefinition((object) GalleryResources.MicrosoftDevLabsPreview()));
    }

    internal virtual void ValidateS2SAuthForTrustedExtensions(
      IVssRequestContext requestContext,
      ExtensionVersionToPublish extensionToPublish,
      PublishedExtensionFlags flags)
    {
      if (!this.ShouldSkipS2SAuthValidation(requestContext) && (flags.HasFlag((Enum) PublishedExtensionFlags.BuiltIn) || flags.HasFlag((Enum) PublishedExtensionFlags.Trusted) || flags.HasFlag((Enum) PublishedExtensionFlags.System)) && !this.IsServicePrincipal(requestContext))
        throw new NotSupportedException(GalleryResources.ExtensionPublishFromNonServicePrincipal((object) (extensionToPublish.PublisherName + "." + extensionToPublish.ExtensionName), (object) flags));
    }

    private static void ValidateVSCodeExtensionForSpam(
      IVssRequestContext requestContext,
      ExtensionVersionToPublish extensionToPublish,
      Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher)
    {
      bool flag = false;
      UnpackagedExtensionData extensionData = new UnpackagedExtensionData();
      extensionData.ExtensionName = extensionToPublish.ExtensionName;
      extensionData.DisplayName = extensionToPublish.DisplayName;
      extensionData.Description = extensionToPublish.ShortDescription;
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.CheckForExtensionSpam") && !AntiSpamService.IsKnownGenuinePublisher(requestContext, publisher))
        flag = AntiSpamService.ExtensionHasSuspectedSpamContent(requestContext, extensionData, publisher, false);
      if (flag)
        throw new HttpException(400, GalleryResources.ExtensionMetadataHasSuspiciousContent());
    }

    internal virtual bool ShouldSkipS2SAuthValidation(IVssRequestContext requestContext)
    {
      bool flag = true;
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableTrustedExtensionPublishS2SCheck") && requestContext.ExecutionEnvironment.IsHostedDeployment)
        flag = false;
      return flag;
    }

    internal virtual bool IsServicePrincipal(IVssRequestContext requestContext)
    {
      IdentityDescriptor authenticatedDescriptor = requestContext.GetAuthenticatedDescriptor();
      return ServicePrincipals.IsServicePrincipal(requestContext, authenticatedDescriptor);
    }

    internal virtual void ValidateIfExtensionScopesChanged(
      IVssRequestContext requestContext,
      Stream packageStream,
      ExtensionVersionToPublish extensionToPublish,
      PublishedExtension extensionFromDb)
    {
      if (this.ShouldSkipExtensionScopeValidation(requestContext, extensionFromDb) || this.ShouldSkipExtensionScopeValidationForTfxCli(requestContext) || packageStream == null || extensionToPublish == null || extensionFromDb == null)
        return;
      ClientTraceData properties = new ClientTraceData();
      bool flag = false;
      try
      {
        ExtensionManifest extensionVsoManifest = this.GetExtensionVsoManifest(requestContext, extensionFromDb);
        ExtensionManifest manifestFromStream = this.GetExtensionVsoManifestFromStream(requestContext, packageStream, extensionToPublish.PublisherName, extensionToPublish.ExtensionName, extensionToPublish.Version);
        if (extensionVsoManifest?.Scopes == null || manifestFromStream?.Scopes == null)
          return;
        HashSet<string> other = new HashSet<string>((IEnumerable<string>) extensionVsoManifest.Scopes, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        if (!new HashSet<string>((IEnumerable<string>) manifestFromStream.Scopes, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).SetEquals((IEnumerable<string>) other))
        {
          properties.Add("OldScopes", (object) string.Join(";", (IEnumerable<string>) extensionVsoManifest.Scopes));
          properties.Add("NewScopes", (object) string.Join(";", (IEnumerable<string>) manifestFromStream.Scopes));
          flag = true;
          throw new ExtensionScopesChangedException(GalleryResources.ExtensionScopesChangedMessage());
        }
      }
      finally
      {
        properties.Add(CustomerIntelligenceProperty.Action, (object) "ExtensionScopeCheck");
        properties.Add("PublisherName", (object) extensionToPublish.PublisherName);
        properties.Add("ExtensionName", (object) extensionToPublish.ExtensionName);
        properties.Add("Version", (object) extensionToPublish.Version);
        properties.Add("ScopesChanged", (object) flag);
        requestContext.GetService<ClientTraceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", "Extension", properties);
      }
    }

    internal virtual ExtensionManifest GetExtensionVsoManifest(
      IVssRequestContext requestContext,
      PublishedExtension extension)
    {
      ExtensionManifest extensionVsoManifest = (ExtensionManifest) null;
      AssetInfo[] assetTypes = new AssetInfo[1]
      {
        new AssetInfo("Microsoft.VisualStudio.Services.Manifest", (string) null)
      };
      IPublisherAssetService service = requestContext.GetService<IPublisherAssetService>();
      ExtensionVersion validatedVersion = PublishedExtensionService.GetLatestValidatedVersion(extension.Versions);
      if (validatedVersion == null)
        return extensionVsoManifest;
      ExtensionFile assetFile = service.QueryAsset(requestContext, extension.Publisher.PublisherName, extension.ExtensionName, validatedVersion.Version, Guid.Empty, (IEnumerable<AssetInfo>) assetTypes, (string) null, (string) null, true).AssetFile;
      if (assetFile != null)
      {
        using (Stream manifestStream = requestContext.GetService<ITeamFoundationFileService>().RetrieveFile(requestContext, (long) assetFile.FileId, false, out byte[] _, out long _, out CompressionType _))
          extensionVsoManifest = ExtensionUtil.LoadManifest(extension.Publisher.PublisherName, extension.ExtensionName, validatedVersion.Version, manifestStream);
      }
      return extensionVsoManifest;
    }

    internal virtual ExtensionManifest GetExtensionVsoManifestFromStream(
      IVssRequestContext requestContext,
      Stream packageStream,
      string requestingPublisherName,
      string requestedExtensionName,
      string version)
    {
      ExtensionManifest extensionVsoManifest = (ExtensionManifest) null;
      VSIXPackage.Parse(packageStream, (Func<ManifestFile, Stream, bool>) ((manifestFile, fileStream) =>
      {
        if (manifestFile.AssetType.Equals("Microsoft.VisualStudio.Services.Manifest", StringComparison.OrdinalIgnoreCase))
          extensionVsoManifest = ExtensionUtil.LoadManifest(requestingPublisherName, requestedExtensionName, version, fileStream);
        return false;
      }));
      return extensionVsoManifest;
    }

    internal virtual bool ShouldSkipExtensionScopeValidation(
      IVssRequestContext requestContext,
      PublishedExtension extensionFromDb)
    {
      bool flag1 = false;
      if (!GalleryUtil.IsVSSExtensionInstallationTarget((IEnumerable<InstallationTarget>) extensionFromDb.InstallationTargets))
        flag1 = true;
      if (!flag1 && (!requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.ValidateExtensionScopeChanges") || requestContext.ExecutionEnvironment.IsOnPremisesDeployment || extensionFromDb.IsMSExtension()))
        flag1 = true;
      bool flag2;
      if (((flag1 ? 0 : (requestContext.TryGetItem<bool>("bypass-scope-check", out flag2) ? 1 : 0)) & (flag2 ? 1 : 0)) != 0)
        flag1 = true;
      return flag1;
    }

    internal virtual bool ShouldSkipExtensionScopeValidationForTfxCli(
      IVssRequestContext requestContext)
    {
      bool flag = false;
      if (requestContext.UserAgent.StartsWith("node-Gallery-api", StringComparison.OrdinalIgnoreCase) && requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.BypassScopeCheckForTfxCli"))
        flag = true;
      return flag;
    }

    private PublishedExtension UpdateExtensionInDbAndPublishEvents(
      IVssRequestContext requestContext,
      ExtensionVersionToPublish extensionToPublish,
      bool shouldNotify,
      bool queueValidation,
      string updatedVersionString,
      string originalVersionString,
      bool immediateVersionValidation,
      long extensionPackageStreamLength,
      PackageDetails packageDetails,
      IEnumerable<InstallationTarget> installationTargets,
      IEnumerable<string> categories)
    {
      PublishedExtension extension1;
      using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
        extension1 = component.UpdateExtension(requestContext, extensionToPublish);
      GallerySecurity.OnDataChanged(requestContext);
      if (shouldNotify)
      {
        string installationTargets1 = GalleryUtil.GetProductTypeForInstallationTargets(installationTargets);
        GalleryServerUtil.NotifyGalleryDataChanged(requestContext, GalleryNotificationEventIds.ExtensionUpdateDelete, "UpdateExtension: " + installationTargets1 + " " + extensionToPublish.PublisherName + "." + extensionToPublish.ExtensionName);
      }
      this.PrepareExtensionForClient(requestContext, extension1, ExtensionQueryFlags.None);
      if (queueValidation)
        this.QueueValidation(requestContext, extension1, extensionToPublish.Version, extensionToPublish.TargetPlatform, immediateVersionValidation);
      IVssRequestContext requestContext1 = requestContext;
      PublishedExtension extension2 = extension1;
      IEnumerable<InstallationTarget> installationTargets2 = installationTargets;
      string str1 = originalVersionString;
      long num = extensionPackageStreamLength;
      string str2 = updatedVersionString;
      string version1 = extensionToPublish.Version;
      IEnumerable<string> strings = categories;
      string targetPlatform1 = extensionToPublish.TargetPlatform;
      TagType? tagType = new TagType?();
      string previousVersion = str1;
      string newVersion = str2;
      long packageSize = num;
      string version2 = version1;
      IEnumerable<string> categories1 = strings;
      string targetPlatform2 = targetPlatform1;
      PublishedExtensionService.PublishCustomerIntelligenceEvent(requestContext1, extension2, "Updated", installationTargets: installationTargets2, tagType: tagType, previousVersion: previousVersion, newVersion: newVersion, packageSize: packageSize, logTelemetryForOnPremise: true, version: version2, categories: categories1, targetPlatform: targetPlatform2);
      this.LogExternalMetadataTelemetry(requestContext, packageDetails?.Manifest.Metadata);
      return extension1;
    }

    private void QueueUploadAssetsToCDNjob(
      IVssRequestContext requestContext,
      string version,
      Guid extensionId)
    {
      requestContext.Trace(12061115, TraceLevel.Info, "Gallery", "QueueValidationNotNeeded", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Entered QueueUploadAssetToCDNJob For Extension: {0} and Version: {1}", (object) extensionId, (object) version));
      PublishedExtension extension = this.QueryExtensionById(requestContext, extensionId, version, ExtensionQueryFlags.AllAttributes | ExtensionQueryFlags.IncludeMetadata, Guid.Empty);
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) new UploadExtensionAssetsToCdnJobData()
      {
        PublisherName = extension.Publisher.PublisherName,
        ExtensionName = extension.ExtensionName,
        Version = extension.Versions[0].Version,
        NoCompression = extension.IsVsExtension()
      });
      requestContext.Trace(12061115, TraceLevel.Info, "Gallery", nameof (QueueUploadAssetsToCDNjob), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Queueing UploadAssetsToCDNjob for extension: {0}.{1} version {2} targetPlatform {3}", (object) extension.Publisher.PublisherName, (object) extension.ExtensionName, (object) extension.Versions[0].Version, (object) extension.Versions[0].TargetPlatform));
      string jobName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Upload To CDN Job for extension {0} and version {1}", (object) extension.GetFullyQualifiedName(), (object) extension.Versions[0].Version);
      service.QueueOneTimeJob(requestContext, jobName, "Microsoft.VisualStudio.Services.Gallery.Extensions.UploadExtensionAssetsToCDNJob", xml, true);
    }

    private PublishedExtension CreateOrUpdateVsExtensionFromStream(
      IVssRequestContext requestContext,
      Stream extensionPackageStream,
      string publisherName,
      string extensionName,
      PackageDetails packageDetails)
    {
      bool flag = extensionName != null;
      if (!flag)
        PublishedExtensionService.CheckIfCreateExtensionLimitExceed(requestContext, publisherName);
      ExtensionVersionToPublish fromPackageDetails = this.ExtractDataFromPackageDetails(requestContext, packageDetails, publisherName);
      PublishedExtensionService.CheckIfMaximumTagLimitExceedForExtensions(requestContext, fromPackageDetails, packageDetails.Tags);
      extensionPackageStream.Seek(0L, SeekOrigin.Begin);
      using (Package package = Package.Open(extensionPackageStream))
      {
        if (package == null)
          throw new InvalidPackageFormatException(GalleryWebApiResources.InvalidPackageStream());
        string payloadFileName = (string) null;
        Stream payloadStream = (Stream) new MemoryStream();
        IExtensionDraftService service = requestContext.GetService<IExtensionDraftService>();
        for (int index = 0; index < packageDetails.Manifest.Assets.Count; ++index)
        {
          ManifestFile asset = packageDetails.Manifest.Assets[index];
          if (string.Equals(asset.AssetType, "Microsoft.VisualStudio.Ide.Payload", StringComparison.OrdinalIgnoreCase))
          {
            asset.FullPath = asset.FullPath.StartsWith("/") ? asset.FullPath : "/" + asset.FullPath;
            payloadFileName = asset.FullPath.Substring(1);
            asset.Version = packageDetails.Manifest.Metadata.Identity.Version;
            payloadStream = package.GetPart(new Uri(asset.FullPath, UriKind.Relative)).GetStream();
            break;
          }
        }
        ExtensionDraft extensionDraft;
        if (flag)
        {
          extensionDraft = service.CreateExtensionDraftForEditExtension(requestContext, publisherName, extensionName);
          if (extensionDraft.ValidationErrors.IsNullOrEmpty<KeyValuePair<string, string>>() && payloadFileName != null)
            extensionDraft = service.UpdatePayloadInDraft(requestContext, publisherName, extensionName, extensionDraft.Id, payloadFileName, payloadStream);
        }
        else
          extensionDraft = service.CreateExtensionDraftForNewExtension(requestContext, publisherName, payloadFileName, "vs", payloadStream);
        PublishedExtensionService.ValidateExtensionDraft(extensionDraft, packageDetails);
        for (int index = 0; index < packageDetails.Manifest.Assets.Count; ++index)
        {
          ManifestFile asset = packageDetails.Manifest.Assets[index];
          if (asset.Addressable && !string.Equals(asset.AssetType, "Microsoft.VisualStudio.Ide.Payload", StringComparison.OrdinalIgnoreCase))
          {
            asset.FullPath = asset.FullPath.StartsWith("/") ? asset.FullPath : "/" + asset.FullPath;
            asset.Version = packageDetails.Manifest.Metadata.Identity.Version;
            Stream stream = package.GetPart(new Uri(asset.FullPath, UriKind.Relative)).GetStream();
            if (flag)
              service.AddAssetInDraftForEditExtension(requestContext, publisherName, extensionName, extensionDraft.Id, asset.AssetType, stream);
            else
              service.AddAssetInDraftForNewExtension(requestContext, publisherName, extensionDraft.Id, asset.AssetType, stream);
          }
        }
        UnpackagedExtensionData extensionData = PublishedExtensionService.BuildUnpackagedExtensionData(extensionDraft, packageDetails);
        PublishedExtensionService.ValidateExtensionDraft(!flag ? service.CreateExtensionFromDraft(requestContext, publisherName, extensionDraft.Id, extensionData) : service.UpdateExtensionFromDraft(requestContext, publisherName, extensionName, extensionDraft.Id, extensionData), packageDetails);
        return PublishedExtensionService.GetExtension(requestContext, extensionData, packageDetails);
      }
    }

    private static void ValidateExtensionDraft(
      ExtensionDraft extensionDraft,
      PackageDetails packageDetails)
    {
      List<KeyValuePair<string, string>> validationErrors = extensionDraft.ValidationErrors;
      // ISSUE: explicit non-virtual call
      if ((validationErrors != null ? (__nonvirtual (validationErrors.Count) > 0 ? 1 : 0) : 0) != 0)
      {
        string message = string.Empty;
        foreach (KeyValuePair<string, string> validationError in extensionDraft.ValidationErrors)
          message = message + validationError.Key + ": " + validationError.Value + "\n";
        throw new InvalidPackageException(message);
      }
      List<KeyValuePair<string, string>> validationWarnings = extensionDraft.ValidationWarnings;
      // ISSUE: explicit non-virtual call
      if ((validationWarnings != null ? (__nonvirtual (validationWarnings.Count) > 0 ? 1 : 0) : 0) == 0)
        return;
      HashSet<string> stringSet = new HashSet<string>();
      string message1 = string.Empty;
      if (!string.IsNullOrEmpty(packageDetails.Manifest.Metadata.IgnoreWarnings))
        stringSet = new HashSet<string>(((IEnumerable<string>) packageDetails.Manifest.Metadata.IgnoreWarnings.Split(',')).Select<string, string>((Func<string, string>) (warningId => warningId?.Trim())));
      foreach (KeyValuePair<string, string> validationWarning in extensionDraft.ValidationWarnings)
      {
        if (!stringSet.Contains(validationWarning.Key))
          message1 = message1 + validationWarning.Key + " - " + validationWarning.Value + "\n";
      }
      if (!string.IsNullOrEmpty(message1))
        throw new PackageValidationWarningException(message1, 1);
    }

    private static PublishedExtension GetExtension(
      IVssRequestContext requestContext,
      UnpackagedExtensionData extensionData,
      PackageDetails packageDetails)
    {
      IPublishedExtensionService service = requestContext.GetService<IPublishedExtensionService>();
      PublishedExtension extension = service.QueryExtension(requestContext, extensionData.PublisherName, extensionData.ExtensionName, (string) null, ExtensionQueryFlags.IncludeInstallationTargets, (string) null);
      if (packageDetails.Manifest.Metadata.Flags.HasFlag((Enum) PublishedExtensionFlags.Public))
        extension = service.UpdateExtensionProperties(requestContext, extensionData.PublisherName, extensionData.ExtensionName, extension.DisplayName, packageDetails.Manifest.Metadata.Flags, extension.ShortDescription, extension.LongDescription);
      return extension;
    }

    private static UnpackagedExtensionData BuildUnpackagedExtensionData(
      ExtensionDraft extensionDraft,
      PackageDetails packageDetails)
    {
      UnpackagedExtensionData unpackagedExtensionData1 = new UnpackagedExtensionData();
      if (extensionDraft.Payload != null && extensionDraft.Payload.Type == ExtensionDeploymentTechnology.Vsix)
      {
        unpackagedExtensionData1.DisplayName = extensionDraft.Payload.DisplayName;
        unpackagedExtensionData1.Description = extensionDraft.Payload.Description;
        unpackagedExtensionData1.InstallationTargets = extensionDraft.Payload.InstallationTargets;
        if (extensionDraft.Payload.Metadata != null)
        {
          foreach (KeyValuePair<string, string> keyValuePair in extensionDraft.Payload.Metadata)
          {
            if (string.Equals(keyValuePair.Key, "VsixId", StringComparison.OrdinalIgnoreCase))
              unpackagedExtensionData1.VsixId = keyValuePair.Value;
            else if (string.Equals(keyValuePair.Key, "VsixVersion", StringComparison.OrdinalIgnoreCase))
              unpackagedExtensionData1.Version = keyValuePair.Value;
          }
        }
      }
      else
      {
        unpackagedExtensionData1.DisplayName = packageDetails.Manifest.Metadata.DisplayName;
        unpackagedExtensionData1.Description = packageDetails.Manifest.Metadata.Description;
        unpackagedExtensionData1.InstallationTargets = packageDetails.Manifest.Installation;
        unpackagedExtensionData1.Version = packageDetails.Manifest.Metadata.Identity.Version;
        unpackagedExtensionData1.VsixId = packageDetails.Manifest.Metadata.VsixId;
      }
      unpackagedExtensionData1.DraftId = extensionDraft.Id;
      unpackagedExtensionData1.Product = "vs";
      unpackagedExtensionData1.ExtensionName = packageDetails.Manifest.Metadata.Identity.ExtensionName;
      unpackagedExtensionData1.PublisherName = packageDetails.Manifest.Metadata.Identity.PublisherName;
      if (packageDetails.Manifest.Metadata.Properties != null)
      {
        foreach (Microsoft.VisualStudio.Services.Gallery.WebApi.ExtensionProperty property in packageDetails.Manifest.Metadata.Properties)
        {
          if (string.Equals(property.Id, "Microsoft.VisualStudio.Services.EnableMarketplaceQnA", StringComparison.OrdinalIgnoreCase))
            unpackagedExtensionData1.QnAEnabled = bool.Parse(property.Value);
          else if (string.Equals(property.Id, "Microsoft.VisualStudio.Services.Links.Source", StringComparison.OrdinalIgnoreCase))
            unpackagedExtensionData1.RepositoryUrl = property.Value;
          else if (string.Equals(property.Id, "Microsoft.VisualStudio.Services.Links.Getstarted", StringComparison.OrdinalIgnoreCase) && extensionDraft.Payload.Type == ExtensionDeploymentTechnology.ReferralLink)
            unpackagedExtensionData1.ReferralUrl = property.Value;
        }
      }
      unpackagedExtensionData1.PricingCategory = GalleryServerUtil.GetPricingCategory(packageDetails.Manifest.Metadata.Flags);
      UnpackagedExtensionData unpackagedExtensionData2 = unpackagedExtensionData1;
      string tags = packageDetails.Manifest.Metadata.Tags;
      List<string> stringList1;
      if (tags == null)
        stringList1 = (List<string>) null;
      else
        stringList1 = ((IEnumerable<string>) tags.Split(',')).ToList<string>();
      unpackagedExtensionData2.Tags = stringList1;
      UnpackagedExtensionData unpackagedExtensionData3 = unpackagedExtensionData1;
      string categories = packageDetails.Manifest.Metadata.Categories;
      List<string> stringList2;
      if (categories == null)
        stringList2 = (List<string>) null;
      else
        stringList2 = ((IEnumerable<string>) categories.Split(',')).ToList<string>();
      unpackagedExtensionData3.Categories = stringList2;
      return unpackagedExtensionData1;
    }

    private void ValidateTrialFlag(
      PublishedExtensionFlags flags,
      InstallationTarget[] installationTargetArray)
    {
      if (flags.HasFlag((Enum) PublishedExtensionFlags.Trial) && PublishedExtensionService.IsInvalidTrialExtensionTargets(installationTargetArray))
        throw new NotSupportedException(GalleryResources.TrialSupportedInstallationTarget((object) "Microsoft.VisualStudio.Services.Integration", (object) "Microsoft.VisualStudio.Services.Cloud.Integration", (object) "Microsoft.TeamFoundation.Server.Integration"));
    }

    internal static void ValidatePackageTargetPlatform(
      IVssRequestContext requestContext,
      IEnumerable<InstallationTarget> installationTargets,
      string targetPlatform,
      bool isVsExtensionWithConsolidatedVsixs = false)
    {
      if (targetPlatform == null || GalleryUtil.IsVSInstallationTargets(installationTargets) & isVsExtensionWithConsolidatedVsixs)
        return;
      if (!GalleryUtil.IsVSCodeInstallationTargets(installationTargets))
        throw new NotSupportedException(GalleryResources.PlatformSpecificExtensionsNotSupportedForNonVSCodeExtensions());
      if (!requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnablePlatformSpecificExtensionsForVSCode"))
        throw new NotSupportedException(GalleryResources.VSCodePlatformSpecificExtensionsDisabled());
      if (!GalleryServerUtil.GetAllowedVSCodeTargetPlatforms(requestContext).Contains<string>(targetPlatform))
        throw new NotSupportedException(GalleryServerUtil.GetLegacyVSCodeTargetPlatforms(requestContext).Contains<string>(targetPlatform) ? GalleryResources.ErrorDeprecatedVSCodeTargetPlatformSupplied((object) targetPlatform) : GalleryResources.ErrorUnSupportedVSCodeTargetPlatformSupplied((object) targetPlatform));
    }

    private static ExtensionVersionToPublish GetExtensionVersionToPublish(
      string publisherName,
      string extensionName,
      string displayName,
      Guid ownerId,
      PublishedExtensionFlags flags,
      string shortDescription,
      string longDescription,
      IEnumerable<ExtensionFile> assets,
      IList<KeyValuePair<int, string>> tags,
      IList<KeyValuePair<string, string>> metadataValues,
      IList<int> extensionLcids,
      string version,
      string versionDescription,
      ExtensionVersionFlags versionFlags,
      Guid privateIdenitityId,
      Guid extensionId,
      DateTime publishedTime,
      string cdnDirectory,
      bool isCdnEnabled,
      IEnumerable<InstallationTarget> installationTargets,
      string targetPlatform = null)
    {
      return new ExtensionVersionToPublish()
      {
        PublisherName = publisherName,
        ExtensionName = extensionName,
        DisplayName = displayName,
        OwnerId = ownerId,
        Flags = flags,
        ShortDescription = shortDescription,
        LongDescription = longDescription,
        Assets = assets,
        Tags = tags,
        MetadataValues = metadataValues,
        ExtensionLcids = extensionLcids,
        Version = version,
        VersionDescription = versionDescription,
        VersionFlags = versionFlags,
        PrivateIdentityId = privateIdenitityId,
        ExtensionId = extensionId,
        PublishedTime = publishedTime,
        CdnDirectory = cdnDirectory,
        IsCdnEnabled = isCdnEnabled,
        InstallationTargets = installationTargets,
        TargetPlatform = targetPlatform
      };
    }

    private void FixUpFieldsAndValidateCommon(
      IVssRequestContext requestContext,
      IEnumerable<ExtensionFile> assets,
      IEnumerable<InstallationTarget> installationTargets,
      IEnumerable<InstallationTarget> existingInstallationTargets,
      ref InstallationTarget[] installationTargetArray,
      ref PublishedExtensionFlags flags,
      ref bool shouldNotify,
      ref bool isUpdateVstsExtensionInstallationTarget,
      bool isFirstParty,
      bool isVsPublishAllowed)
    {
      if (installationTargets != null)
      {
        try
        {
          string extensionProductFamily = this.GetAndValidateExtensionProductFamily(requestContext, (IList<InstallationTarget>) installationTargets.ToList<InstallationTarget>(), existingInstallationTargets != null ? (IList<InstallationTarget>) existingInstallationTargets.ToList<InstallationTarget>() : (IList<InstallationTarget>) null);
          if (extensionProductFamily.Equals("ms.VstsProducts.vsts"))
          {
            isUpdateVstsExtensionInstallationTarget = true;
          }
          else
          {
            isUpdateVstsExtensionInstallationTarget = false;
            if (extensionProductFamily.Equals("ms.VsProducts.vs"))
            {
              if (requestContext.ExecutionEnvironment.IsHostedDeployment)
              {
                if (isVsPublishAllowed)
                  goto label_11;
              }
              throw new VsIdeExtensionNotSupported(GalleryResources.VsExtensionExceptionMessage());
            }
          }
        }
        catch (ServiceOwnerNotFoundException ex)
        {
          if (requestContext.ExecutionEnvironment.IsDevFabricDeployment && requestContext.IsServicingContext)
            isUpdateVstsExtensionInstallationTarget = GalleryUtil.IsVSTSOrTFSInstallationTargets(installationTargets);
          else
            throw;
        }
label_11:
        GalleryServerUtil.ParseInstallationTargetVersion(installationTargets);
        ref InstallationTarget[] local = ref installationTargetArray;
        if (!(installationTargets is InstallationTarget[] installationTargetArray1))
          installationTargetArray1 = installationTargets.ToArray<InstallationTarget>();
        local = installationTargetArray1;
      }
      bool flag1 = false;
      bool flag2 = false;
      bool flag3 = false;
      bool flag4 = false;
      foreach (ExtensionFile asset in assets)
      {
        if (!string.IsNullOrEmpty(asset.AssetType))
          GalleryUtil.CheckAssetType(asset.AssetType);
        if (asset.AssetType.Equals("Microsoft.VisualStudio.Services.Content.License", StringComparison.OrdinalIgnoreCase))
          flag1 = true;
        if (asset.AssetType.Equals("Microsoft.VisualStudio.Services.Content.PrivacyPolicy", StringComparison.OrdinalIgnoreCase))
          flag2 = true;
        if (asset.AssetType.Equals("Microsoft.VisualStudio.Services.Icons.Default", StringComparison.OrdinalIgnoreCase))
          flag3 = true;
        if (asset.AssetType.Equals("Microsoft.VisualStudio.Services.Content.Details", StringComparison.OrdinalIgnoreCase))
          flag4 = true;
      }
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.PrivacyPage") && flags.HasFlag((Enum) PublishedExtensionFlags.Paid) && GalleryUtil.InstallationTargetsHasVSTS(installationTargets) && !flags.HasFlag((Enum) PublishedExtensionFlags.Preview) && !isFirstParty)
      {
        if (!flag1)
          throw new InvalidExtensionDefinitionException(GalleryResources.InvalidExtensionDefinition((object) GalleryResources.PaidExtensionLicenseAsset()));
        if (!flag2)
          throw new InvalidExtensionDefinitionException(GalleryResources.InvalidExtensionDefinition((object) GalleryResources.PaidExtensionPrivacyAsset()));
      }
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.DetailsUploadRestriction") && flags.HasFlag((Enum) PublishedExtensionFlags.Public) && GalleryUtil.InstallationTargetsHasVSTS(installationTargets) && !flags.HasFlag((Enum) PublishedExtensionFlags.BuiltIn) && !flags.HasFlag((Enum) PublishedExtensionFlags.System) && !flag4)
        throw new InvalidExtensionDefinitionException(GalleryResources.InvalidExtensionDefinition((object) GalleryResources.PublicExtensionDetailsAsset()));
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.IconUploadRestriction") && flags.HasFlag((Enum) PublishedExtensionFlags.Public) && GalleryUtil.InstallationTargetsHasVSTS(installationTargets) && !flags.HasFlag((Enum) PublishedExtensionFlags.BuiltIn) && !flags.HasFlag((Enum) PublishedExtensionFlags.System) && !flag3)
        throw new InvalidExtensionDefinitionException(GalleryResources.PublicExtensionDefaultIconAsset());
      if (flags.HasFlag((Enum) PublishedExtensionFlags.Public))
        shouldNotify = true;
      if (flags.HasFlag((Enum) PublishedExtensionFlags.BuiltIn))
      {
        if (!requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableHiddenFlagAddition") || flags.HasFlag((Enum) PublishedExtensionFlags.Paid))
          flags |= PublishedExtensionFlags.Trusted | PublishedExtensionFlags.Public;
        else
          flags |= PublishedExtensionFlags.Trusted | PublishedExtensionFlags.Public | PublishedExtensionFlags.Hidden;
        shouldNotify = false;
      }
      if (!requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return;
      flags |= PublishedExtensionFlags.Public;
      if (installationTargetArray == null || !GalleryUtil.IsVSTSOrTFSInstallationTargets((IEnumerable<InstallationTarget>) installationTargetArray))
        throw new NotSupportedException(GalleryResources.OnPremisesUnsupportedInstallationTarget((object) "Microsoft.VisualStudio.Services", (object) "Microsoft.TeamFoundation.Server.Integration", (object) "Microsoft.TeamFoundation.Server"));
    }

    private static bool isPublisherFirstParty(Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher) => string.Equals(publisher.DisplayName, "Microsoft", StringComparison.OrdinalIgnoreCase);

    private void FailInvalidQuery(string message) => throw new InvalidExtensionQueryException(message);

    private bool IsExtensionAllowedToConvertFromPaidToFree(PublishedExtension extension) => extension != null && extension.Publisher != null && this.m_AllowedExtensionsAllowedToConvertFromPaidToFree.Contains(extension.GetFullyQualifiedName());

    private static bool CanUseInMemoryCache(IVssRequestContext requestContext) => !requestContext.IsHostProcessType(HostProcessType.JobAgent) && requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.UseInMemoryExtensionCache");

    private static string ConvertOldVstsCategoryToVerticalAlignedCategory(
      IVssRequestContext requestContext,
      string inputCategoryTitle)
    {
      string verticalAlignedCategory = inputCategoryTitle;
      if (!inputCategoryTitle.IsNullOrEmpty<char>())
      {
        string str = "";
        if (GalleryServerUtil.c_oldCategoryToVerticalCategoryMapping.TryGetValue(inputCategoryTitle, out str))
          verticalAlignedCategory = str;
      }
      return verticalAlignedCategory;
    }

    private static void RemoveOldVstsCategoryFromCategoryList(
      IVssRequestContext requestContext,
      ref List<ExtensionCategory> categories)
    {
      categories.RemoveAll((Predicate<ExtensionCategory>) (x => x.AssociatedProducts.Contains("vsts") && x.GetCategoryTitleForLanguage("en-us") != null && GalleryServerUtil.c_oldCategoryToVerticalCategoryMapping.ContainsKey(x.GetCategoryTitleForLanguage("en-us"))));
    }

    public PublishedExtensionFlags GetExtensionFlags(
      IVssRequestContext requestContext,
      Guid extensionId)
    {
      using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
        return component.GetExtensionFlags(extensionId);
    }

    public void SetExtensionFlags(
      IVssRequestContext requestContext,
      Guid extensionId,
      PublishedExtensionFlags flags)
    {
      using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
        component.SetExtensionFlags(extensionId, flags);
    }

    internal static CommandPropertiesSetter SearchExtensionsCircuitBreakerSettings => new CommandPropertiesSetter().WithCircuitBreakerErrorThresholdPercentage(70).WithExecutionTimeout(TimeSpan.FromSeconds(15.0)).WithExecutionMaxRequests(250).WithExecutionMaxConcurrentRequests(50).WithMetricsRollingStatisticalWindowInMilliseconds(60000);

    internal static CommandPropertiesSetter NewSearchExtensionsCircuitBreakerSettings => new CommandPropertiesSetter().WithCircuitBreakerErrorThresholdPercentage(10).WithExecutionTimeout(TimeSpan.FromSeconds(15.0)).WithExecutionMaxRequests(250).WithExecutionMaxConcurrentRequests(50).WithMetricsRollingStatisticalWindowInMilliseconds(60000);

    internal static CommandPropertiesSetter QueryExtensionsCircuitBreakerSettings => new CommandPropertiesSetter().WithCircuitBreakerErrorThresholdPercentage(70).WithExecutionTimeout(TimeSpan.FromSeconds(5.0)).WithExecutionMaxRequests(700).WithExecutionMaxConcurrentRequests(50).WithMetricsRollingStatisticalWindowInMilliseconds(60000);

    private class CategoriesData
    {
      public CategoriesData(List<ExtensionCategory> allCategories)
      {
        this.LanguageWiseIdToNameMap = new Dictionary<string, PublishedExtensionService.CategoryIdToNameMap>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        this.LanguageWiseNameToIdMap = new Dictionary<string, PublishedExtensionService.CategoryNameToIdMap>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        this.ProductWiseCategoryDataMap = new Dictionary<string, PublishedExtensionService.ProductCategoryData>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        this.AllCategories = allCategories;
      }

      public List<ExtensionCategory> AllCategories { get; }

      public Dictionary<string, PublishedExtensionService.ProductCategoryData> ProductWiseCategoryDataMap { get; private set; }

      public Dictionary<string, PublishedExtensionService.CategoryNameToIdMap> LanguageWiseNameToIdMap { get; private set; }

      public Dictionary<string, PublishedExtensionService.CategoryIdToNameMap> LanguageWiseIdToNameMap { get; private set; }
    }

    private class ProductCategoryData
    {
      public ProductCategoryData()
      {
        this.Categories = new List<ExtensionCategory>();
        this.LanguageWiseNameToIdMap = new Dictionary<string, PublishedExtensionService.CategoryNameToIdMap>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      }

      public List<ExtensionCategory> Categories { get; private set; }

      public Dictionary<string, PublishedExtensionService.CategoryNameToIdMap> LanguageWiseNameToIdMap { get; private set; }
    }

    private class CategoryIdToNameMap
    {
      public CategoryIdToNameMap() => this.IdToName = new Dictionary<int, string>();

      public Dictionary<int, string> IdToName { get; private set; }
    }

    private class CategoryNameToIdMap
    {
      public CategoryNameToIdMap() => this.NameToId = new Dictionary<string, int>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

      public Dictionary<string, int> NameToId { get; private set; }
    }
  }
}
