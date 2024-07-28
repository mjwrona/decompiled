// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.SearchHelper
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal class SearchHelper
  {
    private const string s_area = "gallery";
    private const string s_layer = "SearchHelper";

    public virtual SearchIndexer GetSearchIndexer(IVssRequestContext requestContext) => new SearchIndexer(requestContext);

    public SearchProviderType GetSearchProviderType(IVssRequestContext requestContext)
    {
      SearchProviderType result = SearchProviderType.None;
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string str = AzureSearchConstants.ExternalSearchSettingsRootPath + AzureSearchConstants.SearchProviderSettingPath;
      IVssRequestContext requestContext1 = requestContext;
      // ISSUE: explicit reference operation
      ref RegistryQuery local = @(RegistryQuery) str;
      Enum.TryParse<SearchProviderType>(service.GetValue(requestContext1, in local, "AzureSearchProvider"), out result);
      return result;
    }

    public virtual void UpdateSearchIndex(
      IVssRequestContext requestContext,
      List<PublishedExtension> extensionList,
      bool deleteEntry = false)
    {
      SearchIndexer searchIndexer = this.GetSearchIndexer(requestContext);
      if (deleteEntry)
      {
        searchIndexer.DeleteEntries(requestContext, extensionList);
      }
      else
      {
        List<PublishedExtension> extensions = new List<PublishedExtension>();
        IPublishedExtensionService service = requestContext.GetService<IPublishedExtensionService>();
        ExtensionQueryFlags extensionQueryFlags = this.GetExtensionQueryFlags();
        for (int index = 0; index < extensionList.Count; ++index)
        {
          try
          {
            PublishedExtension publishedExtension = service.QueryExtensionById(requestContext, extensionList[index].ExtensionId, (string) null, extensionQueryFlags, Guid.Empty);
            extensions.Add(publishedExtension);
          }
          catch (ExtensionDoesNotExistException ex)
          {
          }
        }
        searchIndexer.PopulateIndex(requestContext, extensions, (string) null);
      }
    }

    public ExtensionQueryFlags GetExtensionQueryFlags() => ExtensionQueryFlags.AllAttributes | ExtensionQueryFlags.ExcludeNonValidated | ExtensionQueryFlags.IncludeLatestVersionOnly | ExtensionQueryFlags.IncludeMetadata | ExtensionQueryFlags.IncludeLcids;

    public string GetSearchServiceName(IVssRequestContext requestContext, bool isPrimary)
    {
      string empty = string.Empty;
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string str = AzureSearchConstants.ExternalSearchServiceNameRootPath + "/" + (isPrimary ? AzureSearchConstants.PrimaryPrefix : AzureSearchConstants.SecondaryPrefix) + AzureSearchConstants.AzureSearchServicePath;
      IVssRequestContext requestContext1 = requestContext;
      // ISSUE: explicit reference operation
      ref RegistryQuery local = @(RegistryQuery) str;
      string searchServiceName = service.GetValue(requestContext1, in local, (string) null);
      requestContext.TraceAlways(12060107, TraceLevel.Warning, "gallery", nameof (SearchHelper), "Queried registry: " + str + " to get search service name as '" + (string.IsNullOrEmpty(searchServiceName) ? "null" : searchServiceName) + "'");
      return searchServiceName;
    }

    public string GetSearchIndexKey(
      IVssRequestContext requestContext,
      AzureClientMode mode,
      bool isPrimary)
    {
      string empty = string.Empty;
      string azureSearchQueryKey = AzureSearchConstants.AzureSearchQueryKey;
      string str = isPrimary ? AzureSearchConstants.PrimaryPrefix : AzureSearchConstants.SecondaryPrefix;
      IVssRequestContext vssRequestContext = requestContext;
      ITeamFoundationStrongBoxService service;
      string lookupKey;
      if (mode == AzureClientMode.Admin)
      {
        vssRequestContext = requestContext.Elevate();
        service = vssRequestContext.GetService<ITeamFoundationStrongBoxService>();
        lookupKey = str + AzureSearchConstants.AzureSearchAdminKey;
      }
      else
      {
        service = vssRequestContext.GetService<ITeamFoundationStrongBoxService>();
        lookupKey = str + AzureSearchConstants.AzureSearchQueryKey;
      }
      StrongBoxItemInfo itemInfo = service.GetItemInfo(vssRequestContext, "ConfigurationSecrets", lookupKey, false);
      if (itemInfo == null)
        throw new ExternalSearchException("Unable to read azure search " + mode.ToString() + " key of " + str + " Search Service from strong box drawer ConfigurationSecrets for key:" + lookupKey);
      string enumerable = service.GetString(vssRequestContext, itemInfo.DrawerId, itemInfo.LookupKey);
      return !enumerable.IsNullOrEmpty<char>() ? enumerable : throw new ExternalSearchException("Unable to read azure search " + mode.ToString() + " key of " + str + " Search Service from strong box item info for key:" + lookupKey);
    }
  }
}
