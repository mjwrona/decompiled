// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.IPublishedExtensionService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Server.Extension;
using Microsoft.VisualStudio.Services.Gallery.Server.SoapService.Models;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [DefaultServiceImplementation(typeof (PublishedExtensionService))]
  public interface IPublishedExtensionService : IVssFrameworkService
  {
    void PublishReCaptchaTokenCIForVSCodeExtension(
      IVssRequestContext requestContext,
      IDictionary<string, object> ciData);

    PublishedExtension CreateExtension(
      IVssRequestContext requestContext,
      ExtensionPackage extensionPackage,
      string requestingPublisherName);

    PublishedExtension CreateExtension(
      IVssRequestContext requestContext,
      Stream extensionPackageStream,
      string requestingPublisherName);

    PublishedExtension CreateExtensionFromUnpackagedData(
      IVssRequestContext requestContext,
      string requestingPublisherName,
      IEnumerable<ExtensionFile> uploadedAssets,
      UnpackagedExtensionData extensionData);

    PublishedExtension UpdateExtensionFromUnpackagedData(
      IVssRequestContext requestContext,
      string requestingPublisherName,
      string requestedExtensionName,
      IEnumerable<ExtensionFile> uploadedAssets,
      UnpackagedExtensionData extensionData,
      bool validationNeeded);

    PublishedExtensionFlags GetExtensionFlags(IVssRequestContext requestContext, Guid extensionId);

    void UpdateExtensionInstallationTarget(
      IVssRequestContext requestContext,
      PublishedExtension extension);

    void SetExtensionFlags(
      IVssRequestContext requestContext,
      Guid extensionId,
      PublishedExtensionFlags flags);

    PublishedExtension CreateExtension(
      IVssRequestContext requestContext,
      ExtensionPackage extensionPackage,
      string requestingPublisherName,
      IEnumerable<ExtensionFile> uploadedAssets,
      bool immediateVersionValidation);

    PublishedExtension CreateExtension(
      IVssRequestContext requestContext,
      Stream extensionPackageStream,
      string requestingPublisherName,
      IEnumerable<ExtensionFile> uploadedAssets,
      bool immediateVersionValidation);

    void DeleteExtension(
      IVssRequestContext requestContext,
      Guid extensionId,
      string version,
      bool deleteStatisticsAndReviews = true);

    void DeleteExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string version,
      bool deleteStatisticsAndReviews = true);

    bool IsValidAzurePublisherAndExtension(
      IVssRequestContext requestContext,
      AzureRestApiRequestModel azureRestApiRequestModel,
      ExtensionQueryFlags flags);

    void UpdateExtensionAfterValidation(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      bool updateExtension,
      bool isVsExtension);

    PublishedExtension ProcessValidationResult(
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
      int? fileId);

    string GetAndValidateExtensionProductFamily(
      IVssRequestContext requestContext,
      IList<InstallationTarget> targets,
      IList<InstallationTarget> existingTargets);

    ExtensionsByInstallationTargetsResult QueryExtensionsForCacheBuilding(
      IVssRequestContext requestContext,
      List<InstallationTarget> targets,
      ExtensionQueryFlags flags,
      int pageNumber,
      int pageSize);

    PublishedExtension QueryExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string version,
      ExtensionQueryFlags flags,
      string accountToken,
      bool useCache = false);

    List<ExtensionShare> GetAccountsSharedWithUser(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      Guid userId);

    PublishedExtension QueryExtensionById(
      IVssRequestContext requestContext,
      Guid extensionId,
      string version,
      ExtensionQueryFlags flags,
      Guid validationId);

    ExtensionQueryResult QueryExtensions(
      IVssRequestContext requestContext,
      ExtensionQuery extensionQuery,
      string accountToken,
      bool includeLatestAndValidatedVersionsOnlyFlag,
      Uri referrer = null,
      bool isCacheBuildCall = false);

    ExtensionQueryResult QueryExtensions(
      IVssRequestContext requestContext,
      ExtensionQuery extensionQuery,
      string accountToken,
      Uri referrer = null,
      bool isCacheBuildCall = false);

    ExtensionQueryResult SearchExtensions(
      IVssRequestContext requestContext,
      ExtensionQuery extensionQuery,
      string accountToken,
      SearchOverrideFlags searchOverrides = SearchOverrideFlags.None,
      IEnumerable<MetadataFilterItem> metadataFilterItems = null);

    CategoriesResult QueryAvailableCategories(
      IVssRequestContext requestContext,
      IEnumerable<string> languages,
      string categoryName = "all",
      string product = null);

    ExtensionCategory CreateCategory(IVssRequestContext requestContext, ExtensionCategory category);

    ExtensionVersionValidation QueryVersionValidation(
      IVssRequestContext requestContext,
      Guid extensionId,
      string version,
      string targetPlatform = null);

    void ShareExtension(
      IVssRequestContext requestContext,
      Guid extensionId,
      string shareType,
      string name,
      bool remove);

    void ShareExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string shareType,
      string name,
      bool remove);

    PublishedExtension UpdateExtension(
      IVssRequestContext requestContext,
      ExtensionPackage extensionPackage,
      string requestedExtensionName,
      string requestingPublisherName);

    PublishedExtension UpdateExtension(
      IVssRequestContext requestContext,
      Stream extensionPackageStream,
      string requestedExtensionName,
      string requestingPublisherName);

    PublishedExtension UpdateExtension(
      IVssRequestContext requestContext,
      ExtensionPackage extensionPackage,
      string requestedExtensionName,
      string requestingPublisherName,
      IEnumerable<ExtensionFile> uploadedAssets,
      bool immediateVersionValidation);

    PublishedExtension UpdateExtension(
      IVssRequestContext requestContext,
      Stream extensionPackageStream,
      string requestedExtensionName,
      string requestingPublisherName,
      IEnumerable<ExtensionFile> uploadedAssets,
      bool immediateVersionValidation);

    PublishedExtension UpdateExtensionProperties(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string displayName,
      PublishedExtensionFlags flags,
      string shortDescription,
      string longDescription);

    PublishedExtension AddExtensionIndexedTerm(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      TagType tagType,
      string tagValue,
      bool notify = true);

    PublishedExtension RemoveExtensionIndexedTerm(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      TagType tagType,
      string tagValue,
      bool notify = true);

    List<PublishedExtensionUpdate> GetExtensionData(IVssRequestContext requestContext);

    void UpdateExtensionReleaseDate(
      IVssRequestContext requestContext,
      Guid extensionId,
      DateTime releaseDateToUpdate);

    void UpdateExtensionCDNProperties(
      IVssRequestContext requestContext,
      Guid extensionId,
      string version,
      string cdnDirectory = null,
      bool isCdnEnabled = false,
      string targetPlatform = null);

    void PublishExtensionCreateUpdateNotifications(
      IVssRequestContext requestContext,
      bool sendMessageBusNotification,
      PublishedExtension extension);

    void AddAssetForExtensionVersion(
      IVssRequestContext requestContext,
      Guid extensionId,
      string version,
      string assetType,
      string contentType,
      int fileId,
      string shortDescription);

    string GetVersionQuery(string version);

    void QueueValidation(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      string version,
      string targetPlatform = null,
      bool validateNow = false);

    void QueueVSCodeWebExtensionTagPopulatorJob(
      IVssRequestContext requestContext,
      PublishedExtension extension);

    ExtensionVersionValidationStep CreateVersionValidationStep(
      IVssRequestContext requestContext,
      Guid stepId,
      Guid parentValidationId,
      int stepType,
      int stepStatus,
      DateTime startTime);

    IEnumerable<ExtensionVersionValidationStep> GetAllValidationSteps(
      IVssRequestContext requestContext);

    IEnumerable<ExtensionVersionValidationStep> GetAllValidationStepsByParent(
      IVssRequestContext requestContext,
      Guid parentValidationId);

    ExtensionVersionValidationStep UpdateVersionValidationStep(
      IVssRequestContext requestContext,
      ExtensionVersionValidationStep validationStep);

    void DeleteValidationStepsByParentId(IVssRequestContext requestContext, Guid parentValidationId);

    ExtensionValidationResult GetValidationResult(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string version,
      string targetPlatform = null);

    void WriteExtensionProperties(IVssRequestContext requestContext, PublishedExtension extension);

    List<KeyValuePair<int, string>> ConvertCategoryNamesToTags(
      IVssRequestContext requestContext,
      IEnumerable<string> categories,
      string language,
      string product = null,
      bool ignoreInvalidCategoryNames = false);

    List<string> ConvertCategoryIdsToNames(
      IVssRequestContext requestContext,
      IEnumerable<string> categories,
      string language,
      bool filterNullCategories = true);

    List<SearchCriteria> FixCategoryNamesCase(
      IVssRequestContext requestContext,
      IEnumerable<SearchCriteria> categories,
      string language);

    void UpdateExtensionInAzureSearch(
      IVssRequestContext requestContext,
      List<PublishedExtension> extensionList);

    void EnableVsExtensionConsolidation(
      IVssRequestContext requestContext,
      PublishedExtension extension);

    bool UploadSignatureAsset(
      IVssRequestContext requestContext,
      Stream signatureAssetStream,
      PublishedExtension publishedExtension,
      Guid validationId);

    void CreateBackConsolidationMapping(
      IVssRequestContext requestContext,
      Guid sourceExtensionId,
      string sourceExtensionVsixId,
      Guid targetExtensionId,
      string targetExtensionVsixId);

    IReadOnlyDictionary<string, BackConsolidationMappingEntry> GetBackConsolidationList(
      IVssRequestContext requestContext);
  }
}
