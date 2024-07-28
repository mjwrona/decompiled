// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.GalleryServiceConstants
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal static class GalleryServiceConstants
  {
    public const string DefaultBlobProvider = "/Configuration/Service/Gallery/BlobProvider";
    public const string DefaultAssetPath = "/Configuration/Service/Gallery/Assets/{0}";
    public const string DisableContactOption_RegistryPath = "/Configuration/Service/Gallery/DisableContactOption";
    public const string DefaultAssetPathContentType = "/Configuration/Service/Gallery/Assets/{0}/ContentType";
    public const string ReservedPublisherDisplay = "/Configuration/Service/Gallery/DisallowedWordsInPublisherDisplayName";
    public const string SimilarityPercentageBoundary_RegistryPath = "/Configuration/Service/Gallery/SimilarityPercentageBoundary";
    public const string ExtensionDownloadCountMonitoringCacheExpiry = "/Configuration/Service/Gallery/ExtensionDownloadCountMonitoringCacheExpiry";
    public const string ExtensionCreationLimitBySinglePublisherPath = "/Configuration/Service/Gallery/ExtensionCreationLimitBySinglePublisherPath";
    public const string TimeDurationInMinutesForExtensionCreationLimitPath = "/Configuration/Service/Gallery/TimeDurationInMinutesForExtensionCreationLimitPath";
    public const string VsIdeExtensionTagsLimitPath = "/Configuration/Service/Gallery/VsIdeExtensionTagsLimitPath";
    public const string VsCodeExtensionTagsLimitPath = "/Configuration/Service/Gallery/VsCodeExtensionTagsLimitPath";
    public const int DefaultTimeDurationInMinutesForExtensionCreationLimit = 1440;
    public const int DefaultExtensionCreationLimitBySinglePublisher = 5;
    public const int DefaultVsIdeExtensionTagsLimit = 10;
    public const int DefaultVsCodeExtensionTagsLimit = 10;
    public const string Unsubscribe = "/unsubscribe";
    public const string VSCodeUserAgentPrefix = "VSCode";
    public const string TfxCliUserAgent = "node-Gallery-api";
    public const string VSCodeDocumentationHost = "code.visualstudio.com";
    public const string VSIdeUserAgentPrefix = "VSIDE";
    public const string VSCodeVersionCDNSupportedNonVsix = "1.8";
    public const string RC1UserAgent = "VSServices/15.103.25603.0 (TfsJobAgent.exe)";
    public const string VisualStudioCode = "Visual Studio Code";
    public const string BackConsolidatedExtPreConsolidationVsixId = "PreConsolidationVsixId";
    public const string VsixMetadataPrefix = "VsixId";
    public const string PostUploadExtensionProcessingJob = "Microsoft.VisualStudio.Services.Gallery.Extensions.PostUploadExtensionProcessingJob";
    public const string VersionValidationStatusJob = "Microsoft.VisualStudio.Services.Gallery.Extensions.VersionValidationStatusJob";
    public const string VSCodeWebExtensionTagPopulatorJob = "Microsoft.VisualStudio.Services.Gallery.Extensions.JobAgentPlugins.VSCodeWebExtensionTagPopulatorJob";
    public const string UploadExtensionAssetsToCDNJob = "Microsoft.VisualStudio.Services.Gallery.Extensions.UploadExtensionAssetsToCDNJob";
    public const string IndexOperationJob = "Microsoft.VisualStudio.Services.Gallery.Extensions.JobAgentPlugins.IndexOperationJob";
    public const string DeleteExtensionDataJobDisplayName = "Delete extension data like stats, reviews and qna.";
    public const string DeleteExtensionDataJobName = "Microsoft.VisualStudio.Services.Gallery.Extensions.JobAgentPlugins.DeleteExtensionDataJob";
    public const string UseInMemoryExtensionCacheFeatureFlag = "Microsoft.VisualStudio.Services.Gallery.UseInMemoryExtensionCache";
    public static readonly Guid DeleteReviewsJobId = new Guid("04172A3C-F93C-48CF-B2FA-5404E1991ED4");
    public static readonly Guid ExtensionVersionArtifactKind = new Guid("BDEF69E2-625C-4446-B414-4EFAA2172439");
    public static readonly Guid CreateExtensionLastContactArtifactKind = new Guid("DA88F278-B79F-40BE-A681-9CFABF6F3F9C");
    public static readonly Guid ExtensionNameArtifactKind = new Guid("6DB1EC5A-CE9A-43F6-B82E-150794A6E275");
    public static readonly Guid RatingReviewMailNotifiicationArtifactKind = new Guid("94090715-AEA1-4699-8F13-1BC2BE79DD5E");
    public static readonly Guid ExtensionRedirectionArtifactKind = new Guid("6DB1EC5A-CE9A-43F6-B82E-150794A6E275");
    public static List<string> ValidCategoryLanguageCodes = new List<string>()
    {
      "en-us"
    };
    public const string TagPopulatorJobExtensionName = "Microsoft.VisualStudio.Services.Gallery.Extensions.JobAgentPlugins.TagPopulatorJob";
    public const string DeleteGDPREventsPerUserIdJobExtensionName = "Microsoft.VisualStudio.Services.Gallery.Extensions.JobAgentPlugins.DeleteGDPREventsPerUserIdJob";
    public const string DeleteGDPREventsPerUserIdJobName = "Validate and enqueue GDPR deletions per user ID";
    public const string DeleteGDPREventsJobExtensionName = "Microsoft.VisualStudio.Services.Gallery.Extensions.JobAgentPlugins.DeleteGDPREventsJob";
    public const string DeleteGDPREventsJobName = "Delete GDPR events from Gallery";
    public const string RemoveGDPRMessageFromServiceBusJobExtensionName = "Microsoft.VisualStudio.Services.Gallery.Extensions.JobAgentPlugins.RemoveGDPRMessageFromServiceBusJob";
    public const string RemoveGDPRMessageFromServiceBusJobName = "Removes GDPR message from service bus";
    public const string RemoveGDPRMessageTimeOffSetPath = "/Configuration/Service/Gallery/GDPRJob/TimeOffSet";
    public const string ProductExtensionCachePopulationDbCallTimeoutInSeconds = "/Configuration/Service/Gallery/ExtensionCachePopulator/DbCallTimeoutInSeconds";
    public const string CacheTimeoutOverridenValueInSeconds = "/Configuration/Service/Gallery/ExtensionCachePopulator/CacheTimeoutOverridenValueInSeconds";
    public const string BackConsolidationCacheTimeoutValueInSeconds = "/Configuration/Service/Gallery/BackConsolidation/CacheTimeoutValueInSeconds";
    public const string PublisherDisplayNameCacheExiryTimeInMinutes = "/Configuration/Service/Gallery/PublisherCachePopulator/DisplayNameCacheExiryTimeInMinutes";
    public const string PublisherDisplayNameCachePopulatorType = "/Configuration/Service/Gallery/PublisherCachePopulator/DisplayNameCachePopulatorType";
    public static string HiddenTagPrefix = "__";
    public const string GomezUserAgentString = "GomezAgent";
    public static readonly string LastContactDetails = "lastContactDetails";
    public static readonly string CurrentUserOrganizations = "currentUserOrganizations";
    public static readonly string TenantOrganizations = "tenantOrganizations";
    public static readonly string AnonymizedUserName = "user-0000";
    public static readonly string SubscriptionProductType = "subscriptions";
    public static readonly string UnsubscribeFromRnRSetting = "UnsubscribeFromRnR";
    public static readonly string UnsubscribeFromPublisherContactSetting = "UnsubscribeFromPublisherContact";
    public static readonly string UnsubscribeFromExtensionPublishSuccessSetting = "UnsubscribeFromExtensionPublishSuccess";
    public static readonly string UnsubscribeFromExtensionPublishFailedSetting = "UnsubscribeFromExtensionPublishFailed";
    public static readonly List<string> SupportedSettings = new List<string>()
    {
      GalleryServiceConstants.UnsubscribeFromRnRSetting,
      GalleryServiceConstants.UnsubscribeFromPublisherContactSetting,
      GalleryServiceConstants.UnsubscribeFromExtensionPublishSuccessSetting,
      GalleryServiceConstants.UnsubscribeFromExtensionPublishFailedSetting
    };
    public const char ReservedPublisherNamesRegistryEntryValuedelimiter = ',';
    public static readonly string DisabledPIISetting = nameof (DisabledPIISetting);
    public const string DeploymentTechnology = "DeploymentTechnology";
    public const string ReferralLink = "Referral Link";
    public const string ReviewSectionId = "#review-details";
    public const string ReservedMicrosoftPublisherDisplayName = "Microsoft";
    public const string ReservedMicrosoftDevlabsPublisherDisplayName = "Microsoft DevLabs";
    public const string BypassScopeCheck = "bypass-scope-check";
    public const string IsPublisherSigned = "IsPublisherSigned";
    public static readonly IList<string> DefaultReservedPublisherDisplayNames = (IList<string>) new List<string>()
    {
      "microsoft",
      "ms",
      "microsoft devlabs",
      "azure",
      "windowsazure",
      "office",
      "office365",
      "visual studio",
      "visualstudio",
      "vs",
      "vs-ide",
      "azure devops",
      "azdev",
      "vscode",
      "vsmac",
      "vsts",
      "tfs",
      "teamservices",
      "windows",
      "system",
      "sqlserver",
      "sql server",
      "sql",
      "xbox",
      "appservice",
      "xamarin",
      "appinsights"
    };
    public const int Dev17VersionNumber = 17;
    public const string NewPrimaryAzureSearchNameConfigPath = "/Configuration/Service/Gallery/NewPrimaryAzureSearch/Name";
    public const string NewAzureSearchVsCodeIndexNameConfigPath = "/Configuration/Service/Gallery/NewAzureSearch/VsCodeIndexName";
    public const string NewPrimaryAzureSearchAdminKey = "NewPrimaryAzureSearchAdminKey";
    public const string NewSecondaryAzureSearchNameConfigPath = "/Configuration/Service/Gallery/NewSecondaryAzureSearch/Name";
    public const string NewSecondaryAzureSearchAdminKey = "NewSecondaryAzureSearchAdminKey";
    public const string UpdateStatisticsInNewSearchJobPageSizeConfigPath = "/Configuration/Service/Gallery/UpdateStatisticsInNewSearchJob/PageSize";
    public const string VsInstallationTargetPrefix = "Microsoft.VisualStudio.";
    public const string CorrelationIdRequestHeader = "X-PackageManagement-CorrelationId";
    public const string OverrideExtensionNameHeader = "X-VsCodeServer-ExtensionNameOverride";

    internal static class GeoLocation
    {
      public const string GeoLocationServiceUrl = "https://inference.location.live.net/inferenceservice/v21/Pox/LookupGeoLocationByIpAddress";
      public const string GeoLocationNamespace = "http://inference.location.live.com";
      public const string GeoLocationDataFileId = "/Configuration/Service/Gallery/GeoLocationDataFileId";
      public const string ItemRequestContextCountryKey = "RequestCountry";

      internal static class CountryCode
      {
        public const string China = "CN";
      }
    }

    internal static class ExtensionProperty
    {
      public const string CustomerQnALinkProperty = "Microsoft.VisualStudio.Services.CustomerQnALink";
      public const string EnableMarketplaceQnAProperty = "Microsoft.VisualStudio.Services.EnableMarketplaceQnA";
      public const string SourceProperty = "Microsoft.VisualStudio.Services.Links.Source";
      public const string GitHubLinkProperty = "Microsoft.VisualStudio.Services.Links.GitHub";
      public const string PayloadFileNameProperty = "Microsoft.VisualStudio.Services.Payload.FileName";
      public const string PropertyMetadataPrefix = "PROPERTY::";
      public const string Support = "Microsoft.VisualStudio.Services.Links.Support";
      public const string License = "Microsoft.VisualStudio.Services.Links.License";
      public const string PrivacyPolicy = "Microsoft.VisualStudio.Services.Links.Privacypolicy";
    }

    internal static class PublisherAsset
    {
      public const string ItemsRequestContextKey = "PublisherAsset";
      public const string Root = "/Configuration/Service/Gallery/PublisherAsset/*";
      public const string Host = "/Configuration/Service/Gallery/PublisherAsset/Host";
      public const string CDNHost = "/Configuration/Service/Gallery/PublisherAsset/CDNHost";
      public const string ChinaCDNHost = "/Configuration/Service/Gallery/PublisherAsset/ChinaCDNHost";
      public const string Path = "/Configuration/Service/Gallery/PublisherAsset/Path";
    }

    public static class CDHeaderJobConstants
    {
      public const string CDHeaderCorrectionFFEnabledDateRegPath = "Configuration/Service/Gallery/CDHeaderCorrectionFFDate";
    }

    public static class PMPEventConstants
    {
      internal const string EventTypePrefix = "com.visualstudio.marketplace.";
      internal const string ArtifactFilePublishedEventType = "artifactfile.published";
      internal const string ArtifactFileDeletedEventType = "artifactfile.deleted";
      internal const string PackageAggregateLockedEventType = "packageaggregate.locked";
      internal const string PackageAggregateUnlockedEventType = "packageaggregate.unlocked";
      internal const string PackageAggregateDeletedEventType = "packageaggregate.deleted";
      internal const string PackageAggregatePublishedEventType = "packageaggregate.archived";
      internal const string PackageAggregateUnPublishedEventType = "packageaggregate.unarchived";
      internal const string PackageAggregateForceIndexEventType = "packageaggregate.forceindex";
      internal const string PackageDeletedEventType = "package.deleted";
      internal const string PublisherUpdatedEventType = "publisher.updated";
      internal const string EventSourceDefaultValue = "https://marketplace.visualstudio.com";
    }

    public static class VSCodeIndexUpdaterConstants
    {
      public const string PrimaryServiceBusConnectionString = "PrimaryServiceBusConnectionString";
      public const string SecondaryServiceBusConnectionString = "SecondaryServiceBusConnectionString";
    }

    internal static class CustomerContactProperty
    {
      public const string EventIds = "EventIds";
      public const string PublisherDisplayName = "PublisherDisplayName";
      public const string ExtensionDisplayName = "ExtensionDisplayName";
      public const string PublisherName = "PublisherName";
      public const string ExtensionName = "ExtensionName";
      public const string ContactText = "ContactText";
      public const string ReasonCode = "ReasonCode";
      public const string ReasonText = "ReasonText";
      public const string AccountName = "AccountName";
      public const string UninstallDate = "UninstallDate";
      public const string EventDate = "EventDate";
      public const string EventType = "EventType";
      public const string EventId = "EventId";
      public const string LastContactKey = "LastContactKey";
    }

    internal static class PublisherMemberContactProperty
    {
      public const string UserId = "UserId";
      public const string PublisherAction = "PublisherAction";
      public const string PublisherName = "PublisherName";
      public const string PublisherDisplayName = "PublisherDisplayName";
    }

    internal static class WellKnownPublisheraActions
    {
      public const string AddMember = "add";
      public const string RemoveMember = "remove";
    }

    internal static class VSGalleryDB
    {
      public const string DBName = "/Configuration/Service/Gallery/VSGallery/DBName";
      public const string DBSqlInstanceName = "/Configuration/Service/Gallery/VSGallery/SqlInstance";
      public const string DrawerName = "VSDBConnectionCredential";
      public const string PasswordKey = "VSGalleryDBSqlPassword";
      public const string LoginKey = "VSGalleryDBSqlLogin";
    }

    internal static class VSGalleryRedirection
    {
      public const string HostName = "/Configuration/Service/Gallery/VSGallery/HostedUrl";
      public const string ReviewAPIPath = "api/v1/rate";
    }

    internal static class VSGalleryBanner
    {
      public const string BannerTextPath = "/Configuration/Service/Gallery/BannerText";
    }

    internal static class VSGalleryPreventDeleteExtension
    {
      public const string RemoveExtensionMinAcquisitionCount = "/Configuration/Service/Gallery/RemoveExtensionMinAcquisitionCount";
      public const string RemoveExtensionMinDayCount = "/Configuration/Service/Gallery/RemoveExtensionMinDayCount";
      public const int DefaultRemoveExtensionMinAcquisitionCount = 100;
      public const int DefaultRemoveExtensionMinDayCount = 30;
    }

    internal static class GalleryAssetCDN
    {
      public const string StrongBoxKey = "GalleryAssetCDNStorageConnectionString";
      public const string OmegaStrongBoxKey = "GalleryAssetCDNOmegaStorageConnectionString";
      public const string DefaultContainer = "extensions";
      public const string DefaultContainerRegistryPath = "/Configuration/Service/Gallery/CDN/DefaultContainer";
      public const string GzipEncoding = "gzip";
      public const string UploadRoot = "{0}/{1}/{2}/{3}";
      public const string PublisherAssetUploadRoot = "{0}";
    }

    internal static class PMPConstants
    {
      public const string RegistryRoot = "/Configuration/Service/Gallery/PMP/**";
      public const string UploadServiceTargetHostPath = "/Configuration/Service/Gallery/PMP/UploadService/TargetHost";
      public const string PMPUploadServiceUrlPath = "/Configuration/Service/Gallery/PMP/UploadService/Url";
      public const string DefaultPMPUploadServiceUrl = "https://{0}/api/upload?RegistryName={1}&ArtifactFileTypeMoniker={2}";
      public const string UploadServiceRequestRetryCountPath = "/Configuration/Service/Gallery/PMP/UploadService/Times/RetryCount";
      public const int DefaultPMPServiceRequestRetryCount = 3;
      public const string UploadServiceDelayTimePath = "/Configuration/Service/Gallery/PMP/UploadService/Times/DelayTime";
      public const int DefaultDelayTimeSeconds = 2;
      public const string GraphQLServiceTargetHostPath = "/Configuration/Service/Gallery/PMP/GraphQLService/TargetHost";
      public const string GraphQLServiceAdminTargetHostPath = "/Configuration/Service/Gallery/PMP/GraphQLAdminService/TargetHost";
      public const string PMPWebApiResourceUri = "/Configuration/Service/Gallery/PMP/WebApiResourceUri";
      public const string VSCodeRegistryName = "vscode";
      public const string VsCodeArtifactFileTypeMoniker = "vscodevsix";
      public const string UpdatePackageAggregateArchiveStateMutation = "updatePackageAggregateArchiveState";
      public const string UpdatePackageAggregateLockStateMutation = "updatePackageAggregateLockState";
      public const string RemoveFullPackageAggregateMutation = "removeFullPackageAggregate";
      public const string RemovePackageMutation = "removePackage";
      public const string PMPUploadService = "Upload";
      public const string PMPGraphQLService = "GraphQL";
      public const string ServiceBusRetryCountPath = "/Configuration/Service/Gallery/PMPIntegration/ServiceBusRetryCount";
      public const int DefaultServiceBusRetryCount = 3;
      public const string ServiceBusDelayTimePath = "/Configuration/Service/Gallery/PMPIntegration/ServiceBusDelayTime";
      public const int DefaultServiceBusDelayTimeSeconds = 2;
      public const string ServiceBusNamespacePath = "/Configuration/Service/Gallery/PMPIntegration/ServiceBusNamesapce";
      public const string DefaultServiceBusNamespace = "vsm-dev-ncus-bus.servicebus.windows.net";
      public const string ServiceBusConnectionStringPath = "/Configuration/Service/Gallery/PMPIntegration/ServiceBusConnectionString";
      public const string DefaultServiceBusConnectionString = "vsm-dev-ncus-bus.servicebus.windows.net";
      public const string VSCodeEventsTopicPath = "/Configuration/Service/Gallery/PMP/VSCodeEventsTopic";
      public const string DefaultVSCodeEventsTopic = "vscodeevents";
      public const string SyncFunctionTopicPath = "/Configuration/Service/Gallery/PMP/SyncFunctionTopic";
      public const string DefaultSyncFunctionTopic = "vscodedataImport";
    }

    internal static class VsExtensionConsolidation
    {
      public const string ExtensionAllowList = "/Configuration/Service/Gallery/VsConsolidation/ExtensionAllowList";
    }

    internal static class PackageVerification
    {
      public const string RegistryRoot = "/Configuration/Service/Gallery/PackageVerification/**";
      public const string DefaultBlobContainer = "packageverification";
      public const string BlobContainerRegistryPath = "/Configuration/Service/Gallery/PackageVerification/BlobContainer";
      public const string MlSpamValidationServiceEndpoint = "/Configuration/Service/Gallery/PackageVerification/MlSpamValidationServiceEndpoint";
      public const string UploadRoot = "{0}";
      public const string GalleryPVStorageConnectionString = "GalleryPVStorageConnectionString";
      public const string GalleryPVOmegaStorageConnectionString = "GalleryPVOmegaStorageConnectionString";

      internal static class UnifiedValidationPipeline
      {
        public const string UploadRoot = "{0}/uvp/{1}/{2}";
        public const string BlobContainerRegistryPath = "/Configuration/Service/Gallery/PackageVerification/UVP/BlobContainer";
        public const string DefaultBlobContainer = "unifiedpackageverification";
        public const string UVPServiceBusRequestRetryCountPath = "/Configuration/Service/Gallery/PackageVerification/UVP/Times/UVPRetryCount";
        public const int DefaultServiceBusRequestRetryCount = 3;
        public const string UVPDelayTimePath = "/Configuration/Service/Gallery/PackageVerification/UVP/Times/UVPDelayTime";
        public const int DefaultDelayTimeSeconds = 2;
        public const string UVPServiceBusTopicNamePath = "/Configuration/Service/Gallery/PackageVerification/UVP/UVPServiceBusTopicNamePath";
        public const string DefaultServiceBusTopicName = "validation";
        public const string UnifiedValidationPipelineSBConnectionString = "UnifiedValidationPipelineSBConnectionString";
        public const string VsCodePackage = "VsCodeExtensionV1";
      }

      internal static class CVS
      {
        public const string ServerRegistryPath = "/Configuration/Service/Gallery/PackageVerification/CVS/Server";
        public const string CVSAuthCertificate = "CVSAuthCertificate";
        public const string CVSAPIMKey = "CVSAPIMKey";
        public const string UploadRoot = "{0}/cvs/{1}";
        public const string CallbackEnabled = "/Configuration/Service/Gallery/PackageVerification/CVS/CallbackEnabled";
        public const string defaultApiVersion = "4.1-preview";
        public const string CVSRetryCountpath = "/Configuration/Service/Gallery/PackageVerification/CVS/CVSRetryCountpath";
        public const long CVSRetryCountDefault = 2;

        internal static class Times
        {
          public const string CVSRecheckTimePath = "/Configuration/Service/Gallery/PackageVerification/CVS/Times/CVSRecheckTime";
          public const long CVSRecheckTimeDefault = 1800;
          public const string CVSTimeoutPath = "/Configuration/Service/Gallery/PackageVerification/CVS/Times/CVSTimeout";
          public const long CVSTimeoutDefault = 86400;
          public const string CVSBlockedDuration = "/Configuration/Service/Gallery/PackageVerification/CVS/Times/CVSBlockedDuration ";
          public const long CVSBlockedDurationDefault = 1296000;
          public const string CVSBlockedNotifyWindow = "/Configuration/Service/Gallery/PackageVerification/CVS/Times/CVSBlockedNotifyWindow";
          public const long CVSBlockedNotifyWindowDefault = 172800;
        }
      }

      internal static class ESRP
      {
        public const string SubmitScan = "SubmitScan";
        public const string ScanStatus = "ScanStatus";
        public const string SubmitSignCommand = "SubmitSign";
        public const string SignStatusCommand = "SignStatus";
        public const string CertificatePrefix = "CN=";
        public const string AuthMechanism = "AAD_CERT";
        public const string OAuthMechanism = "AAD_OAUTH_TOKEN";
        public const string RuntimeServicePrincipleClientId = "RuntimeServicePrincipalClientId";
        public const string RuntimeServicePrincipleCertStoreLocation = "LocalMachine";
        public const string RuntimeServicePrincipleCertStoreName = "My";
        public const string RuntimeServicePrincipalCertThumbprint = "RuntimeServicePrincipalCertThumbprint";
        public const string ESRPCertificateStoreLocation = "LocalMachine";
        public const string ESRPCertificateStoreName = "My";
        public const string AMETenantId = "33e01921-4d64-4f8c-a055-5bdaffd5e33d";
        public const string ERSPClientTokenStrongBoxKeyName = "ESRPClientToken";
        public const string ESRPApiEndpoint = "https://api.esrp.microsoft.com/.default";
        public const string OAuthEnvironmentVariableName = "ESRPOAuthToken";
        public const string ESRPClientIdRegistryPath = "/Configuration/Service/Gallery/PackageVerification/ESRP/ESRPClientId";
        public const string ESRPTenantIdRegistryPath = "/Configuration/Service/Gallery/PackageVerification/ESRP/ESRPTenantId";
        public const string ESRPApiEndpointRegistryPath = "/Configuration/Service/Gallery/PackageVerification/ESRP/ESRPApiEndpoint";
        public const string ESRPCertificateSubjectNameRegistryPath = "/Configuration/Service/Gallery/PackageVerification/ESRP/CertificateSubjectName";
        public const string RepositorySigningKeyCodeRegistryPath = "/Configuration/Service/Gallery/PackageVerification/ESRP/RepositorySigningKeyCodeRegistryPath";
        public const string RepositorySigningKeyCodeDefaultValue = "RepositorySigningKeyCodeDefaultValue";
        public const string PolicyIntent = "Product Release";
        public const string PolicyContentType = "App";
        public const string PolicyContentOrigin = "3rd Party";
        public const string PolicyAudience = "External Broad";
        public const string ServerRegistryPath = "/Configuration/Service/Gallery/PackageVerification/ESRP/Server";
        public const string ClientPath = "bin\\ESRP\\ESRPClient.exe";
        public const int ClientTimeoutInSec = 2400;
        public const string UploadRoot = "{0}/esrp/";

        public enum EsrpRequestFileType
        {
          AuthFile = 1,
          PolicyFile = 2,
          ConfigFile = 3,
        }

        internal static class Times
        {
          public const string VirusScanRecheckTimeInProgressPath = "/Configuration/Service/Gallery/PackageVerification/ESRP/Times/VirusScanRecheckTimeInProgress";
          public const long VirusScanRecheckTimeInProgressDefault = 30;
          public const string VirusScanRecheckTimePendingAnalysisPath = "/Configuration/Service/Gallery/PackageVerification/ESRP/Times/VirusScanRecheckTimePendingAnalysis";
          public const long VirusScanRecheckTimePendingAnalysisDefault = 10800;
          public const string VirusScanTimeoutInProgressPath = "/Configuration/Service/Gallery/PackageVerification/ESRP/Times/VirusScanTimeoutInProgress";
          public const long VirusScanTimeoutInProgressDefault = 3600;
          public const string VirusScanTimeoutPendingAnalysisPath = "/Configuration/Service/Gallery/PackageVerification/ESRP/Times/VirusScanTimeoutPendingAnalysis";
          public const long VirusScanTimeoutPendingAnalysisDefault = 180000;
          public const string FileSigningRecheckTimeInProgressPath = "/Configuration/Service/Gallery/PackageVerification/ESRP/Times/FileSigningRecheckTimeInProgress";
          public const long FileSigningRecheckTimeInProgressDefault = 30;
          public const string FileSigningRecheckTimePendingAnalysisPath = "/Configuration/Service/Gallery/PackageVerification/ESRP/Times/FileSigningRecheckTimePendingAnalysis";
          public const long FileSigningRecheckTimePendingAnalysisDefault = 7200;
          public const string FileSigningTimeoutInProgressPath = "/Configuration/Service/Gallery/PackageVerification/ESRP/Times/FileSigningTimeoutInProgress";
          public const long FileSigningTimeoutInProgressDefault = 3600;
          public const string FileSigningTimeoutPendingAnalysisPath = "/Configuration/Service/Gallery/PackageVerification/ESRP/Times/FileSigningTimeoutPendingAnalysis";
          public const long FileSigningTimeoutPendingAnalysisDefault = 180000;
        }

        internal enum ESRPClientReturnCodes
        {
          Success,
        }
      }
    }

    internal static class RepositorySigning
    {
      public const string SigningToolPath = "bin\\signing\\vsce-sign.exe";
      public const int SigningToolTimeOutInSeconds = 2400;
      public const string GenerateManifestCommand = "generatemanifest";
      public const string GenerateSignatureArchiveCommand = "zip";
      public const string VerifySignatureCommand = "verify";
      public const string SignatureManifestFileExtension = ".signature.manifest";
      public const string SignatureFileExtension = ".signature.p7s";
      public const string SignatureArchiveFileExtension = ".signature.zip";
      public const string RegistryRoot = "/Configuration/Service/Gallery/RepositorySigning/**";
      public const string AdditionalValidationPolicyRegistryPath = "/Configuration/Service/Gallery/RepositorySigning/AdditionalValidationPolicyRegistryPath";
      public const string AdditionalValidationPolicyDefaultValue = "VSMP";
      public const string RepositorySignExistingExtensions = "Microsoft.VisualStudio.Services.Gallery.Extensions.JobAgentPlugins.RepositorySignExtensionsJob";
    }

    internal static class TyposquattingConfigurations
    {
      public const string RegistryRoot = "/Configuration/Service/Gallery/TyposquattingConfigurations/**";
      public const string ExtensionDisplayNameMatchThresholdPath = "ExtensionDisplayNameMatch/Threshold";
      public const string ExtensionDisplayNameMatchThresholdDefaultValue = "0/50:1;50/100:2;100/129:3";
      public const string PublisherDisplayNameMatchThresholdPath = "PublisherDisplayNameMatch/Threshold";
      public const string PublisherDisplayNameMatchThresholdDefaultValue = "0/50:1;50/100:2;100/129:3";
      public const string PublisherSkipListForTyposquatting = "Skiplist";
    }

    internal static class CustomerIntelligence
    {
      public const string GalleryArea = "Microsoft.VisualStudio.Services.Gallery";
      public const string Source = "Source";
      public const string PublisherFeature = "Publisher";
      public const string ExtensionFeature = "Extension";
      public const string ExtensionUploadExternalMetadata = "Extension_Upload_External_Metadata";
      public const string ExternalMetadata = "External_Metadata";
      public const string Github = "Github";
      public const string BadgeLink = "BadgeLink";
      public const string BadgeImgUri = "BadgeImgUri";
      public const string BadgeCount = "BadgeCount";
      public const string VSCodeInstall = "VsCodeInstall";
      public const string VSIdeInstall = "VsIdeInstall";
      public const string VSIdeUpdate = "VsIdeUpdate";
      public const string Category = "Category";
      public const string CaptchaInCreateVsIdeExtension = "CaptchaInCreateVsIdeExtension";
      public const string CaptchaInEditVsIdeExtension = "CaptchaInEditVsIdeExtension";
      public const string VSCodeVersionImmutability = "VsCodeVersionImmutability";
      public const string RatingAndReviewFeature = "RatingAndReviews";
      public const string QnAFeature = "Q&A";
      public const string QnAResponseFeature = "Q&AResponse";
      public const string VSIdeExtensionFeature = "ExtensionForVSIde";
      public const string VSCodeExtensionFeature = "ExtensionForVSCode";
      public const string CustomerContactNotification = "CustomerContactNotification";
      public const string CreatedAction = "Created";
      public const string DeletedAction = "Deleted";
      public const string UpdatedAction = "Updated";
      public const string UnpublishedAction = "Unpublished";
      public const string PublishedAction = "Published";
      public const string SharedAction = "Shared";
      public const string UnsharedAction = "Unshared";
      public const string AddIndexedTerm = "AddIndexedTerm";
      public const string RemoveIndexedTerm = "RemoveIndexedTerm";
      public const string SearchAction = "Searched";
      public const string QueryExtensionsAction = "QueriedExtensions";
      public const string LockedAction = "Locked";
      public const string UnlockedAction = "Unlocked";
      public const string CreateScenario = "CreateScenario";
      public const string UpdateScenario = "UpdateScenario";
      public const string Scenario = "Scenario";
      public const string IsRequestFromChinaRegion = "IsRequestFromChinaRegion";
      public const string PMPUploadService = "PMPUploadService";
      public const string InMemoryQuery = "InMemoryQuery";
      public const string ProductExtensionCacheRefresh = "{0}ExtensionCacheRefresh";
      public const string VSCodeExtensionCacheRefresh = "VSCodeExtensionCacheRefresh";
      public const string VstsExtensionCacheRefresh = "VstsExtensionCacheRefresh";
      public const string BackConsolidatedExtensionCacheRefresh = "BackConsolidatedExtensionCacheRefresh";
      public const string AsssetTypeCaseMismatch = "AsssetTypeCaseMismatch";
      public const string GetAssestRedirect = "GetAssestRedirect";
      public const string VSCodeFallback = "VSCodeFallback";
      public const string GalleryUserIdentityProperty = "GalleryUserId";
      public const string ChinaAssetCDNRequests = "ChinaAssetCDNRequests";
      public const string GeoLocationException = "GeoLocationException";
      public const string OnPremConnected = "OnPremConnected";
      public const string OnPremConnectedNavigationFeature = "OnPremConnectedNavigationFeature";
      public const string OnPremConnectedTfsServerInternetAccess = "InternetAccess";
      public const string OnPremConnectedTfsServerVersionCheckForInstall = "OnPremConnectedServerVersionCheck";
      public const string OnPremConnectedExtensionName = "OnPremConnectedExtensionName";
      public const string OnPremConnectedExtensionVersion = "OnPremConnectedExtensionVersion";
      public const string OnPremConnectedTfsServerVersion = "OnPremConnectedTfsServerVersion";
      public const string OnPremConnectedTfsInstallSupported = "OnPremConnectedTfsInstallSupported";
      public const string IsMarketplaceExtension = "IsMarketplaceExtension";
      public const string IsPaidExtension = "IsPaid";
      public const string Publisher360Feature = "Publisher360";
      public const string HomepageCacheFeature = "HomepageCache";
      public const string QueryExtensionCacheFeature = "QueryExtensionCache";
      public const string ExtensionOverviewCacheFeature = "ExtensionOverviewCacheFeature";
      public const string AcquisitionPagePerformance = "AcquisitionPagePerformance";
      public const string ESRPFeature = "ESRP";
      public const string ESRPOneTimeScanFeature = "ESRPOneTimeScan";
      public const string ESRPConfigFilesRegeneration = "ESRPConfigFilesRegeneration";
      public const string CDHeaderJobBase = "CDHeaderJobBase";
      public const string CDHeaderPreValidatorJob = "CDHeaderPreValidatorJob";
      public const string CDHeaderCorrectionJob = "CDHeaderCorrectionJob";
      public const string CDHeaderPostValidatorJob = "CDHeaderPostValidatorJob";
      public const string CVSFeature = "CVS";
      public const string CVSOneTimeScanFeature = "CVSOneTimeScan";
      public const string CVSViolationTimeExceeded = "CVSViolationTimeExceeded";
      public const string UVPFeature = "UVP";
      public const string PMPFeature = "PMP";
      public const string IndexUpdaterFeature = "IndexUpdater";
      public const string RepositorySigningFeature = "RepositorySigningFeature";
      public const string InternalExtensionNavigation = "InternalExtensionNavigation";
      public const string ExtensionOrganizationSharing = "ExtensionOrganizationSharing";
      public const string GDPRDeleteFeature = "GDPRDelete";
      public const string ExternalSearchFeature = "ExternalSearch";
      public const string TopPublisherFeature = "TopPublisher";
      public const string SpamPublisher = "SpamPublisher";
      public const string CustomerSupportRequest = "CustomerSupportRequest";
      public const string SpamExtensionValidation = "SpamExtensionValidation";
      public const string SpamExtensionOverviewContentValidation = "SpamExtensionOverviewContentValidation";
      public const string SpamExtensionUrlValidation = "SpamExtensionUrlValidation";
      public const string SpamExtensionKeyWordValidation = "SpamExtensionKeyWordValidation";
      public const string SpamPublisherKeyWordValidation = "SpamPublisherKeyWordValidation";
      public const string SpamPublisherValidation = "SpamPublisherValidation";
      public const string ExceededMaxNumberOfPublishersPerAccount = "ExceededMaxNumberOfPublishersPerAccount";
      public const string ReCaptchaValidation = "ReCaptchaValidation";
      public const string Invalid = "Invalid";
      public const string Valid = "Valid";
      public const string NoReCaptcha = "NoReCaptcha";
      public const string ZipSlipValidationSuccess = "ZipSlipValidationSuccess";
      public const string ZipSlipValidationFailure = "ZipSlipValidationFailure";
      public const string ZipSlipValidationPlatformSpecificError = "ZipSlipValidationPlatformSpecificError";
      public const string UpdateStatisticsForUniversalTargetPlatform = "UpdateStatisticsForUniversalTargetPlatform";
      public const string AddOrUpdateDomain = "AddOrUpdateDomain";
      public const string VerifyDnsRecord = "VerifyDnsRecord";
      public const string MarkPublisherDomainAsVerified = "MarkPublisherDomainAsVerified";
      public const string VsExtensionVersionImmutabilityError = "VsExtensionVersionImmutabilityError";
      public const string EnabledVsixConsolidationForVsExtension = "EnabledVsixConsolidationForVsExtension";
      public const string EnableVsixConsolidationWarningOnUpload = "EnableVsixConsolidationWarningOnUpload";
      public const string MlSpamValidationStep = "MlSpamValidation";
      public const string VsCodePublisherIdValidationFailure = "VsCodePublisherIdValidationFailure";
      public const string DuplicatePublisherDisplayNameCheck = "DuplicatePublisherDisplayNameCheck";
    }

    internal static class ConnectedExperience
    {
      public const string MarketpPlaceUrlRegistryPath = "/Configuration/Service/Gallery/MarketplaceRootURL";
      public const string DefaultMarketplaceUrl = "https://marketplace.visualstudio.com/";

      internal static class CustomerIntelligence
      {
        public const string ConnectedInstall = "ConnectedInstallClick";
        public const string ModalDialogConfirmClick = "ModalDialogConfirmClick";
        public const string PublishTimeError = "PublishTimeError";
        public const string InstallError = "InstallError";
      }
    }

    public static class DeploymentTechnologyConstants
    {
      public const string ReferralLink = "referral link";
      public const string Exe = "exe";
      public const string Msi = "msi";
      public const string Vsix = "vsix";
    }

    internal static class ProductServiceConstants
    {
      public const string ProductFamilyType = "ms.vss-gallery.product-family";
      public const string ProductType = "ms.vss-gallery.product";
      public const string ReleaseType = "ms.vss-gallery.product-release";
      public const string InstallationTargetProperty = "installationTargets";
      public const string NameProperty = "name";
      public const string DisplayNameProperty = "displayName";
      public const string MinVersionProperty = "minVersion";
      public const string MaxVersionProperty = "maxVersion";
      public const string VstsProductFamily = "ms.VstsProducts.vsts";
      public const string VsProductFamily = "ms.VsProducts.vs";
    }

    internal static class BatchSize
    {
      public const string ExtensionStatBatchSizeRegistryPath = "/Configuration/Service/Gallery/BatchSize/ExtensionStat";
      public const int ExtensionStatDefaultBatchSize = 100;
      public const string PublisherStatBatchSizeRegistryPath = "/Configuration/Service/Gallery/BatchSize/PublisherStat";
      public const int PublisherStatDefaultBatchSize = 100;
      public const string AuditLogBatchSizeRegistryPath = "/Configuration/Service/Gallery/BatchSize/AuditLog";
      public const int AuditLogDefaultBatchSize = 100;
    }
  }
}
