// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.AzureSearchProvider
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal class AzureSearchProvider : ISearchProvider
  {
    private const string s_area = "gallery";
    private const string s_layer = "AzureSearchProvider";
    private AzureIndexProvider m_azDevOpsIndexProvider;
    private AzureIndexProvider m_vsCodeIndexProvider;
    private AzureIndexProvider m_vsIndexProvider;

    public AzureSearchProvider(
      IVssRequestContext requestContext,
      AzureClientMode mode,
      string searchServiceName,
      string searchIndexKey)
    {
      List<string> stringList = new List<string>()
      {
        "azuredevops",
        "vscode",
        "vs"
      };
      for (int index = 0; index < stringList.Count; ++index)
        this.InitializeAzureSearchProvider(requestContext, mode, searchServiceName, searchIndexKey, stringList[index]);
    }

    internal AzureSearchProvider()
    {
    }

    internal AzureSearchProvider(
      AzureIndexProvider azDevOpsIndexProvider,
      AzureIndexProvider vsCodeIndexProvider,
      AzureIndexProvider vsIndexProvider)
    {
      this.m_azDevOpsIndexProvider = azDevOpsIndexProvider;
      this.m_vsCodeIndexProvider = vsCodeIndexProvider;
      this.m_vsIndexProvider = vsIndexProvider;
    }

    public AzureSearchProvider(
      IVssRequestContext requestContext,
      AzureClientMode mode,
      string searchServiceName,
      string searchIndexKey,
      string productType)
    {
      this.InitializeAzureSearchProvider(requestContext, mode, searchServiceName, searchIndexKey, productType);
    }

    private void InitializeAzureSearchProvider(
      IVssRequestContext requestContext,
      AzureClientMode mode,
      string searchServiceName,
      string searchIndexKey,
      string productType)
    {
      if (productType.IsNullOrEmpty<char>())
      {
        StackTrace stackTrace = new StackTrace();
        requestContext.TraceAlways(12060107, TraceLevel.Warning, "gallery", nameof (AzureSearchProvider), "Found a null/empty product type string which is not expected. productType: " + (productType == null ? "null" : "empty") + ".\nStack: " + stackTrace.ToString());
      }
      else
      {
        GalleryProductTypesEnum result = GalleryProductTypesEnum.None;
        if (!Enum.TryParse<GalleryProductTypesEnum>(productType, true, out result))
        {
          requestContext.TraceAlways(12060107, TraceLevel.Error, "gallery", nameof (AzureSearchProvider), "Found a invalid product type string value. productType: " + productType);
        }
        else
        {
          try
          {
            switch (result)
            {
              case GalleryProductTypesEnum.Vs:
                this.m_vsIndexProvider = new AzureIndexProvider(requestContext, searchServiceName, this.GetReadIndexName(requestContext, productType), searchIndexKey, mode);
                break;
              case GalleryProductTypesEnum.VsCode:
                this.m_vsCodeIndexProvider = new AzureIndexProvider(requestContext, searchServiceName, this.GetReadIndexName(requestContext, productType), searchIndexKey, mode);
                break;
              case GalleryProductTypesEnum.AzureDevOps:
                this.m_azDevOpsIndexProvider = new AzureIndexProvider(requestContext, searchServiceName, this.GetReadIndexName(requestContext, productType), searchIndexKey, mode);
                break;
            }
          }
          catch (ExternalSearchException ex)
          {
            string format = "ProductType: " + productType + " Failed to get the index name, check the index name registry key setting. Exception " + ex.Message;
            requestContext.TraceAlways(12060107, TraceLevel.Warning, "gallery", nameof (AzureSearchProvider), format);
          }
        }
      }
    }

    public void CreateIndex(
      IVssRequestContext requestContext,
      string productType,
      bool useNewVSCodeIndexDefinition = false)
    {
      GalleryProductTypesEnum productTypeEnum = GalleryProductTypesEnum.None;
      this.ValidateProductType(requestContext, productType, out productTypeEnum);
      switch (productTypeEnum)
      {
        case GalleryProductTypesEnum.Vs:
          this.m_vsIndexProvider?.CreateIndex(new AzureSearchIndexDefinitionConstants());
          break;
        case GalleryProductTypesEnum.VsCode:
          AzureSearchIndexDefinitionConstants indexConstants = new AzureSearchIndexDefinitionConstants();
          if (useNewVSCodeIndexDefinition)
          {
            this.m_vsCodeIndexProvider?.CreateVSCodeIndex(indexConstants);
            break;
          }
          this.m_vsCodeIndexProvider?.CreateIndex(indexConstants);
          break;
        case GalleryProductTypesEnum.Vsts:
        case GalleryProductTypesEnum.AzureDevOps:
          this.m_azDevOpsIndexProvider?.CreateIndex((AzureSearchIndexDefinitionConstants) new AzureDevOpsSearchIndexDefinitionConstants());
          break;
      }
    }

    public void CreateOrUpdateIndex(
      IVssRequestContext requestContext,
      string productType,
      bool useNewVSCodeIndexDefinition = false)
    {
      GalleryProductTypesEnum productTypeEnum = GalleryProductTypesEnum.None;
      this.ValidateProductType(requestContext, productType, out productTypeEnum);
      switch (productTypeEnum)
      {
        case GalleryProductTypesEnum.Vs:
          this.m_vsIndexProvider?.CreateOrUpdateIndex(new AzureSearchIndexDefinitionConstants());
          break;
        case GalleryProductTypesEnum.VsCode:
          AzureSearchIndexDefinitionConstants indexConstants = new AzureSearchIndexDefinitionConstants();
          if (useNewVSCodeIndexDefinition)
          {
            this.m_vsCodeIndexProvider?.CreateOrUpdateVSCodeIndex(indexConstants);
            break;
          }
          this.m_vsCodeIndexProvider?.CreateOrUpdateIndex(indexConstants);
          break;
        case GalleryProductTypesEnum.Vsts:
        case GalleryProductTypesEnum.AzureDevOps:
          this.m_azDevOpsIndexProvider?.CreateOrUpdateIndex((AzureSearchIndexDefinitionConstants) new AzureDevOpsSearchIndexDefinitionConstants());
          break;
      }
    }

    public void UploadSynonymMap(
      IVssRequestContext requestContext,
      string synonymMapName,
      string productType,
      string synonymMapValue = "")
    {
      GalleryProductTypesEnum productTypeEnum = GalleryProductTypesEnum.None;
      this.ValidateProductType(requestContext, productType, out productTypeEnum);
      switch (productTypeEnum)
      {
        case GalleryProductTypesEnum.Vs:
          this.m_vsIndexProvider?.UploadSynonymMap(synonymMapName, synonymMapValue);
          break;
        case GalleryProductTypesEnum.VsCode:
          this.m_vsCodeIndexProvider?.UploadSynonymMap(synonymMapName, synonymMapValue);
          break;
        case GalleryProductTypesEnum.Vsts:
        case GalleryProductTypesEnum.AzureDevOps:
          this.m_azDevOpsIndexProvider?.UploadSynonymMap(synonymMapName, synonymMapValue);
          break;
      }
    }

    public void RemoveSynonymMap(
      IVssRequestContext requestContext,
      string synonymMapName,
      string productType)
    {
      GalleryProductTypesEnum productTypeEnum = GalleryProductTypesEnum.None;
      this.ValidateProductType(requestContext, productType, out productTypeEnum);
      switch (productTypeEnum)
      {
        case GalleryProductTypesEnum.Vs:
          this.m_vsIndexProvider?.RemoveSynonymMap(synonymMapName);
          break;
        case GalleryProductTypesEnum.VsCode:
          this.m_vsCodeIndexProvider?.RemoveSynonymMap(synonymMapName);
          break;
        case GalleryProductTypesEnum.Vsts:
        case GalleryProductTypesEnum.AzureDevOps:
          this.m_azDevOpsIndexProvider?.RemoveSynonymMap(synonymMapName);
          break;
      }
    }

    public void DeleteIndex(IVssRequestContext requestContext, string productType)
    {
      GalleryProductTypesEnum productTypeEnum = GalleryProductTypesEnum.None;
      this.ValidateProductType(requestContext, productType, out productTypeEnum);
      switch (productTypeEnum)
      {
        case GalleryProductTypesEnum.Vs:
          this.m_vsIndexProvider?.DeleteIndex();
          break;
        case GalleryProductTypesEnum.VsCode:
          this.m_vsCodeIndexProvider?.DeleteIndex();
          break;
        case GalleryProductTypesEnum.Vsts:
        case GalleryProductTypesEnum.AzureDevOps:
          this.m_azDevOpsIndexProvider?.DeleteIndex();
          break;
      }
    }

    public void DeleteEntries(
      SearchEnabledStatus searchEnabledStatus,
      List<PublishedExtension> extensionsList)
    {
      Dictionary<GalleryProductTypesEnum, List<PublishedExtension>> productToExtensionsMap = this.GetProductToExtensionsMap(extensionsList);
      if (productToExtensionsMap.IsNullOrEmpty<KeyValuePair<GalleryProductTypesEnum, List<PublishedExtension>>>())
        return;
      if (productToExtensionsMap.ContainsKey(GalleryProductTypesEnum.AzureDevOps) && searchEnabledStatus.HasFlag((Enum) SearchEnabledStatus.EnabledForAzureDevOps))
        this.m_azDevOpsIndexProvider?.DeleteEntries(productToExtensionsMap[GalleryProductTypesEnum.AzureDevOps]);
      if (productToExtensionsMap.ContainsKey(GalleryProductTypesEnum.VsCode) && searchEnabledStatus.HasFlag((Enum) SearchEnabledStatus.EnabledForVSCode))
        this.m_vsCodeIndexProvider?.DeleteEntries(productToExtensionsMap[GalleryProductTypesEnum.VsCode]);
      if (!productToExtensionsMap.ContainsKey(GalleryProductTypesEnum.Vs) || !searchEnabledStatus.HasFlag((Enum) SearchEnabledStatus.EnabledForVS))
        return;
      this.m_vsIndexProvider?.DeleteEntries(productToExtensionsMap[GalleryProductTypesEnum.Vs]);
    }

    public void PopulateIndex(
      SearchEnabledStatus searchEnabledStatus,
      List<PublishedExtension> extensionsList,
      string productType = null,
      bool useNewIndexDefinition = false,
      bool useProductArchitectureInfo = false,
      bool isPlatformSpecificExtensionsForVSCodeEnabled = false,
      bool usePublisherDomainInfo = false)
    {
      GalleryProductTypesEnum result = GalleryProductTypesEnum.None;
      Dictionary<GalleryProductTypesEnum, List<PublishedExtension>> enumerable;
      if (!productType.IsNullOrEmpty<char>() && Enum.TryParse<GalleryProductTypesEnum>(productType, true, out result))
        enumerable = new Dictionary<GalleryProductTypesEnum, List<PublishedExtension>>()
        {
          {
            result,
            extensionsList
          }
        };
      else
        enumerable = this.GetProductToExtensionsMap(extensionsList);
      if (!enumerable.IsNullOrEmpty<KeyValuePair<GalleryProductTypesEnum, List<PublishedExtension>>>())
      {
        if (enumerable.ContainsKey(GalleryProductTypesEnum.AzureDevOps) && searchEnabledStatus.HasFlag((Enum) SearchEnabledStatus.EnabledForAzureDevOps))
          this.m_azDevOpsIndexProvider?.PopulateIndex(enumerable[GalleryProductTypesEnum.AzureDevOps], useNewIndexDefinition, usePublisherDomainInfo: usePublisherDomainInfo);
        if (enumerable.ContainsKey(GalleryProductTypesEnum.VsCode) && searchEnabledStatus.HasFlag((Enum) SearchEnabledStatus.EnabledForVSCode))
          this.m_vsCodeIndexProvider?.PopulateIndex(enumerable[GalleryProductTypesEnum.VsCode], useNewIndexDefinition, isPlatformSpecificExtensionsForVSCodeEnabled: isPlatformSpecificExtensionsForVSCodeEnabled, usePublisherDomainInfo: usePublisherDomainInfo);
        if (!enumerable.ContainsKey(GalleryProductTypesEnum.Vs) || !searchEnabledStatus.HasFlag((Enum) SearchEnabledStatus.EnabledForVS))
          return;
        List<PublishedExtension> extensionsList1 = enumerable[GalleryProductTypesEnum.Vs];
        foreach (PublishedExtension extension in extensionsList1)
          GalleryUtil.LoadExtensionDeploymentType(extension);
        this.m_vsIndexProvider?.PopulateIndex(extensionsList1, useNewIndexDefinition, useProductArchitectureInfo, usePublisherDomainInfo: usePublisherDomainInfo);
      }
      else
      {
        StackTrace stackTrace = new StackTrace();
        TeamFoundationTracingService.TraceRaw(12060106, TraceLevel.Error, "gallery", nameof (AzureSearchProvider), "Not able to decide which index to use. ProductType " + (productType.IsNullOrEmpty<char>() ? "null" : productType) + "\nStack Trace: " + stackTrace.ToString());
      }
    }

    public ExtensionQueryResult Search(
      IVssRequestContext requestContext,
      SearchEnabledStatus searchEnabledStatus,
      ExtensionSearchParams searchParams,
      ExtensionQueryFlags queryFlags)
    {
      ExtensionQueryResult queryResult = (ExtensionQueryResult) null;
      List<InstallationTarget> list = searchParams.CriteriaList.Where<SearchCriteria>((Func<SearchCriteria, bool>) (x => x.FilterType == SearchFilterType.InstallationTarget)).Select<SearchCriteria, InstallationTarget>((Func<SearchCriteria, InstallationTarget>) (x => new InstallationTarget()
      {
        Target = x.FilterValue
      })).ToList<InstallationTarget>();
      if (list.IsNullOrEmpty<InstallationTarget>())
      {
        StackTrace stackTrace = new StackTrace();
        string message = "No installation targets specified in the search query!";
        TeamFoundationTracingService.TraceRaw(12060106, TraceLevel.Error, "gallery", nameof (AzureSearchProvider), message + "\nStack Trace: " + stackTrace.ToString());
        throw new InvalidExtensionQueryException(message);
      }
      GalleryProductTypesEnum installationTargets = GalleryServerUtil.GetProductTypeEnumForInstallationTargets((IEnumerable<InstallationTarget>) list, true);
      ExtensionSearchParams extSearchParams = this.FixFilterFields(requestContext, searchParams, installationTargets);
      bool useNewIndexDefinition = requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableSortByInstallCount");
      bool enableSortByInstallCountUI = requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableSortByInstallCountUI");
      switch (installationTargets)
      {
        case GalleryProductTypesEnum.Vs:
          bool useProductArchitectureInfo = requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableProductArchitectureSupportForVS");
          bool includeInstallationTargetWithAndWithoutProductArchitecture = requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.IncludeTargetWithAndWithoutProductArchitectureForVSSearch");
          bool enableFilterOnTagsWithSearchText = requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableFilterOnTagsWithSearchText");
          queryResult = this.m_vsIndexProvider?.Search(extSearchParams, queryFlags, useNewIndexDefinition, enableSortByInstallCountUI, useProductArchitectureInfo, includeInstallationTargetWithAndWithoutProductArchitecture, enableFilterOnTagsWithSearchText);
          break;
        case GalleryProductTypesEnum.VsCode:
          queryResult = this.m_vsCodeIndexProvider?.Search(extSearchParams, queryFlags, useNewIndexDefinition, enableSortByInstallCountUI);
          break;
        case GalleryProductTypesEnum.Vsts:
        case GalleryProductTypesEnum.AzureDevOps:
          queryResult = this.m_azDevOpsIndexProvider?.Search(extSearchParams, queryFlags, useNewIndexDefinition, enableSortByInstallCountUI);
          break;
      }
      if (searchParams.MetadataFlags.HasFlag((Enum) ExtensionQueryResultMetadataFlags.IncludeResultSetCategories))
        this.ConvertCategoryTitleToCategoryId(requestContext, installationTargets.ToString().ToLowerInvariant(), searchParams, queryResult);
      return queryResult;
    }

    private void ConvertCategoryTitleToCategoryId(
      IVssRequestContext requestContext,
      string product,
      ExtensionSearchParams searchParams,
      ExtensionQueryResult queryResult)
    {
      IPublishedExtensionService service = requestContext.GetService<IPublishedExtensionService>();
      foreach (ExtensionFilterResultMetadata filterResultMetadata in queryResult.Results[0].ResultMetadata)
      {
        if (filterResultMetadata.MetadataType.Equals(QueryMetadataConstants.ResultSetCategories))
        {
          foreach (MetadataItem metadataItem in filterResultMetadata.MetadataItems)
          {
            List<KeyValuePair<int, string>> tags = service.ConvertCategoryNamesToTags(requestContext, (IEnumerable<string>) new string[1]
            {
              metadataItem.Name
            }, "en-us", product, true);
            if (!tags.IsNullOrEmpty<KeyValuePair<int, string>>())
              metadataItem.Name = tags[0].Value;
          }
        }
      }
    }

    private void ValidateProductType(
      IVssRequestContext requestContext,
      string productType,
      out GalleryProductTypesEnum productTypeEnum)
    {
      if (productType.IsNullOrEmpty<char>())
      {
        Exception exception = (Exception) new ArgumentException("Expected a valid product type. Received : " + (productType == null ? "null" : productType));
        requestContext.TraceException(12060106, "gallery", nameof (AzureSearchProvider), exception);
        throw exception;
      }
      if (!Enum.TryParse<GalleryProductTypesEnum>(productType, true, out productTypeEnum))
      {
        Exception exception = (Exception) new ArgumentException("Invalid product type! Expected one of azuredevops, vscode, vs or vsformac. Received: " + productType);
        requestContext.TraceException(12060106, "gallery", nameof (AzureSearchProvider), exception);
        throw exception;
      }
    }

    private string GetReadIndexName(IVssRequestContext requestContext, string productType)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string str1 = AzureSearchConstants.ExternalSearchSettingsRootPath + AzureSearchConstants.ReadIndexPath + "/" + productType;
      IVssRequestContext requestContext1 = requestContext;
      // ISSUE: explicit reference operation
      ref RegistryQuery local = @(RegistryQuery) str1;
      string str2 = service.GetValue(requestContext1, in local, (string) null);
      requestContext.Trace(12060107, TraceLevel.Info, "gallery", nameof (AzureSearchProvider), "Queryed registry: " + str1 + " to get read index name as '" + (string.IsNullOrEmpty(str2) ? "null" : str2) + "'");
      return !string.IsNullOrEmpty(str2) ? str2 : throw new ExternalSearchException("Cannot find the index name for the product : " + productType);
    }

    private string GetWriteIndexName(IVssRequestContext requestContext, string productType)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string str1 = AzureSearchConstants.ExternalSearchSettingsRootPath + AzureSearchConstants.WriteIndexPath + "/" + productType;
      IVssRequestContext requestContext1 = requestContext;
      // ISSUE: explicit reference operation
      ref RegistryQuery local = @(RegistryQuery) str1;
      string str2 = service.GetValue(requestContext1, in local, (string) null);
      requestContext.Trace(12060107, TraceLevel.Info, "gallery", nameof (AzureSearchProvider), "Queryed registry: " + str1 + " to get write index name as '" + (string.IsNullOrEmpty(str2) ? "null" : str2) + "'");
      return !string.IsNullOrEmpty(str2) ? str2 : throw new ExternalSearchException("Cannot find the index name for the product : " + productType);
    }

    private Dictionary<GalleryProductTypesEnum, List<PublishedExtension>> GetProductToExtensionsMap(
      List<PublishedExtension> extensionsList)
    {
      Dictionary<GalleryProductTypesEnum, List<PublishedExtension>> productToExtensionsMap = (Dictionary<GalleryProductTypesEnum, List<PublishedExtension>>) null;
      if (!extensionsList.IsNullOrEmpty<PublishedExtension>())
      {
        productToExtensionsMap = new Dictionary<GalleryProductTypesEnum, List<PublishedExtension>>();
        List<PublishedExtension> enumerable1 = new List<PublishedExtension>();
        List<PublishedExtension> enumerable2 = new List<PublishedExtension>();
        List<PublishedExtension> enumerable3 = new List<PublishedExtension>();
        List<PublishedExtension> enumerable4 = new List<PublishedExtension>();
        foreach (PublishedExtension extensions in extensionsList)
        {
          switch (GalleryServerUtil.GetProductTypeEnumForInstallationTargets((IEnumerable<InstallationTarget>) extensions.InstallationTargets, true))
          {
            case GalleryProductTypesEnum.Vs:
              enumerable2.Add(extensions);
              continue;
            case GalleryProductTypesEnum.VsForMac:
              enumerable4.Add(extensions);
              continue;
            case GalleryProductTypesEnum.VsCode:
              enumerable3.Add(extensions);
              continue;
            case GalleryProductTypesEnum.Vsts:
            case GalleryProductTypesEnum.AzureDevOps:
              enumerable1.Add(extensions);
              continue;
            default:
              continue;
          }
        }
        if (!enumerable1.IsNullOrEmpty<PublishedExtension>())
          productToExtensionsMap[GalleryProductTypesEnum.AzureDevOps] = enumerable1;
        if (!enumerable3.IsNullOrEmpty<PublishedExtension>())
          productToExtensionsMap[GalleryProductTypesEnum.VsCode] = enumerable3;
        if (!enumerable2.IsNullOrEmpty<PublishedExtension>())
          productToExtensionsMap[GalleryProductTypesEnum.Vs] = enumerable2;
        if (!enumerable4.IsNullOrEmpty<PublishedExtension>())
          productToExtensionsMap[GalleryProductTypesEnum.VsForMac] = enumerable4;
      }
      return productToExtensionsMap;
    }

    private ExtensionSearchParams FixFilterFields(
      IVssRequestContext requestContext,
      ExtensionSearchParams searchParams,
      GalleryProductTypesEnum productTypeEnum)
    {
      ExtensionSearchParams extensionSearchParams = new ExtensionSearchParams();
      extensionSearchParams.FeatureFlags = searchParams.FeatureFlags;
      extensionSearchParams.MetadataFlags = searchParams.MetadataFlags;
      extensionSearchParams.PageNumber = searchParams.PageNumber;
      extensionSearchParams.PageSize = searchParams.PageSize;
      extensionSearchParams.Product = searchParams.Product;
      extensionSearchParams.RawQuery = searchParams.RawQuery;
      extensionSearchParams.SortBy = searchParams.SortBy;
      extensionSearchParams.SortOrder = searchParams.SortOrder;
      extensionSearchParams.CriteriaList = (IList<SearchCriteria>) new List<SearchCriteria>();
      List<string> stringList = new List<string>();
      List<SearchCriteria> searchCriteriaList = new List<SearchCriteria>();
      List<SearchCriteria> source = new List<SearchCriteria>();
      string language = "en-us";
      SearchFilterOperatorType filterOperatorType = SearchFilterOperatorType.And;
      for (int index = 0; index < searchParams.CriteriaList.Count; ++index)
      {
        SearchCriteria criteria = searchParams.CriteriaList[index];
        switch (criteria.FilterType)
        {
          case SearchFilterType.InstallationTarget:
            source.Add(criteria);
            break;
          case SearchFilterType.Category:
            if (int.TryParse(criteria.FilterValue, out int _))
            {
              stringList.Add(criteria.FilterValue);
              filterOperatorType = criteria.OperatorType;
              break;
            }
            searchCriteriaList.Add(criteria);
            break;
          default:
            extensionSearchParams.CriteriaList.Add(criteria);
            break;
        }
      }
      if (stringList.Count<string>() > 0)
      {
        List<string> names = requestContext.GetService<IPublishedExtensionService>().ConvertCategoryIdsToNames(requestContext, (IEnumerable<string>) stringList, language);
        for (int index = 0; index < names.Count; ++index)
          extensionSearchParams.CriteriaList.Add(new SearchCriteria()
          {
            FilterType = SearchFilterType.Category,
            FilterValue = names[index],
            OperatorType = filterOperatorType
          });
      }
      else if (searchCriteriaList.Count<SearchCriteria>() > 0)
      {
        List<SearchCriteria> collection = requestContext.GetService<IPublishedExtensionService>().FixCategoryNamesCase(requestContext, (IEnumerable<SearchCriteria>) searchCriteriaList, language);
        ((List<SearchCriteria>) extensionSearchParams.CriteriaList).AddRange((IEnumerable<SearchCriteria>) collection);
      }
      if (source.Count<SearchCriteria>() > 0)
      {
        HashSet<InstallationTarget> installationTargetSet1 = (HashSet<InstallationTarget>) null;
        switch (productTypeEnum)
        {
          case GalleryProductTypesEnum.Vs:
            installationTargetSet1 = GalleryInstallationTargets.VsInstallationTargets;
            break;
          case GalleryProductTypesEnum.VsCode:
            installationTargetSet1 = GalleryInstallationTargets.VsCodeInstallationTargets;
            break;
          case GalleryProductTypesEnum.Vsts:
          case GalleryProductTypesEnum.AzureDevOps:
            installationTargetSet1 = GalleryInstallationTargets.VstsInstallationTargets;
            break;
        }
        List<SearchCriteria> collection;
        if (installationTargetSet1 != null)
        {
          collection = new List<SearchCriteria>();
          for (int index = 0; index < source.Count<SearchCriteria>(); ++index)
          {
            HashSet<InstallationTarget> installationTargetSet2 = installationTargetSet1;
            InstallationTarget equalValue = new InstallationTarget();
            equalValue.Target = source[index].FilterValue;
            InstallationTarget installationTarget;
            ref InstallationTarget local = ref installationTarget;
            SearchCriteria searchCriteria;
            if (installationTargetSet2.TryGetValue(equalValue, out local))
              searchCriteria = new SearchCriteria()
              {
                FilterType = source[index].FilterType,
                FilterValue = installationTarget.Target,
                OperatorType = source[index].OperatorType
              };
            else
              searchCriteria = source[index];
            collection.Add(searchCriteria);
          }
        }
        else
          collection = source;
        ((List<SearchCriteria>) extensionSearchParams.CriteriaList).AddRange((IEnumerable<SearchCriteria>) collection);
      }
      return extensionSearchParams;
    }
  }
}
