// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.SearchIndexer
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class SearchIndexer : ISearchIndexer
  {
    private const string s_area = "gallery";
    private const string s_layer = "SearchIndexer";
    private SearchProviderType m_searchProviderType;
    private ISearchProvider m_primarySearchProvider;
    private ISearchProvider m_secondarySearchProvider;
    private SearchEnabledStatus m_searchEnabledStatus;

    public SearchIndexer(IVssRequestContext requestContext) => this.InitializeSearchIndexer(requestContext);

    public SearchIndexer(IVssRequestContext requestContext, string productType) => this.InitializeSearchIndexer(requestContext, productType);

    public SearchIndexer(
      IVssRequestContext requestContext,
      string productType,
      string searchServiceName,
      bool isPrimary)
    {
      if (productType.IsNullOrEmpty<char>())
        requestContext.TraceAlways(12060107, TraceLevel.Warning, "gallery", nameof (SearchIndexer), "Found a null/empty product type string which is not expected. productType: " + (productType == null ? "null" : "empty"));
      else
        this.InitializeSearchIndexerWithSearchServiceName(requestContext, searchServiceName, isPrimary, productType);
    }

    internal SearchIndexer(
      ISearchProvider primarySearchProvider,
      ISearchProvider secondarySearchProvider,
      SearchProviderType providerType,
      SearchEnabledStatus searchEnabledStatus = SearchEnabledStatus.EnabledForNone)
    {
      this.m_primarySearchProvider = primarySearchProvider;
      this.m_secondarySearchProvider = secondarySearchProvider;
      this.m_searchProviderType = providerType;
      this.m_searchEnabledStatus = searchEnabledStatus;
    }

    internal SearchIndexer()
    {
    }

    private void InitializeSearchIndexer(IVssRequestContext requestContext, string productType = null)
    {
      SearchHelper searchHelper = new SearchHelper();
      this.m_searchProviderType = searchHelper.GetSearchProviderType(requestContext);
      this.m_searchEnabledStatus = this.GetSearchEnabledStatus(requestContext);
      if (this.m_searchProviderType != SearchProviderType.AzureSearchProvider)
        return;
      try
      {
        string searchServiceName1 = searchHelper.GetSearchServiceName(requestContext, true);
        string searchServiceName2 = searchHelper.GetSearchServiceName(requestContext, false);
        if (!searchServiceName1.IsNullOrEmpty<char>())
        {
          string searchIndexKey = searchHelper.GetSearchIndexKey(requestContext, AzureClientMode.Admin, true);
          this.m_primarySearchProvider = productType.IsNullOrEmpty<char>() ? (ISearchProvider) new AzureSearchProvider(requestContext, AzureClientMode.Admin, searchServiceName1, searchIndexKey) : (ISearchProvider) new AzureSearchProvider(requestContext, AzureClientMode.Admin, searchServiceName1, searchIndexKey, productType);
        }
        if (!searchServiceName2.IsNullOrEmpty<char>())
        {
          string searchIndexKey = searchHelper.GetSearchIndexKey(requestContext, AzureClientMode.Admin, false);
          this.m_secondarySearchProvider = productType.IsNullOrEmpty<char>() ? (ISearchProvider) new AzureSearchProvider(requestContext, AzureClientMode.Admin, searchServiceName2, searchIndexKey) : (ISearchProvider) new AzureSearchProvider(requestContext, AzureClientMode.Admin, searchServiceName2, searchIndexKey, productType);
        }
        if (this.m_primarySearchProvider != null || this.m_secondarySearchProvider != null)
          return;
        string format = "Failed to create primary and secondary search provider, check the search service name registry key setting";
        requestContext.TraceAlways(12060107, TraceLevel.Warning, "gallery", nameof (SearchIndexer), format);
      }
      catch (ExternalSearchException ex)
      {
        string format = "Failed to create a search provider, check the search service name registry key setting. Exception " + ex.Message;
        requestContext.TraceAlways(12060107, TraceLevel.Warning, "gallery", nameof (SearchIndexer), format);
        this.m_searchProviderType = SearchProviderType.None;
      }
    }

    private void InitializeSearchIndexerWithSearchServiceName(
      IVssRequestContext requestContext,
      string searchServiceName,
      bool isPrimary,
      string productType = null)
    {
      SearchHelper searchHelper = new SearchHelper();
      this.m_searchProviderType = searchHelper.GetSearchProviderType(requestContext);
      this.m_searchEnabledStatus = this.GetSearchEnabledStatus(requestContext);
      if (this.m_searchProviderType != SearchProviderType.AzureSearchProvider)
        return;
      string searchIndexKey = searchHelper.GetSearchIndexKey(requestContext, AzureClientMode.Admin, isPrimary);
      this.m_primarySearchProvider = productType.IsNullOrEmpty<char>() ? (ISearchProvider) new AzureSearchProvider(requestContext, AzureClientMode.Admin, searchServiceName, searchIndexKey) : (ISearchProvider) new AzureSearchProvider(requestContext, AzureClientMode.Admin, searchServiceName, searchIndexKey, productType);
    }

    public virtual void PopulateIndex(
      IVssRequestContext requestContext,
      List<PublishedExtension> extensions,
      string productType = null)
    {
      if (!this.IsExternalSearchEnabledForIndexing(requestContext))
        return;
      if (!extensions.IsNullOrEmpty<PublishedExtension>())
      {
        List<Task> executableTasks = new List<Task>();
        bool useNewIndexDefinition = requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableSortByInstallCount");
        bool useProductArchitectureInfo = requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableProductArchitectureSupportForVS");
        bool isPlatformSpecificExtensionsForVSCodeEnabled = requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnablePlatformSpecificExtensionsForVSCode");
        bool usePublisherDomainInfo = requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableDomainInfoForSearchIndex");
        if (this.m_primarySearchProvider != null)
          executableTasks.Add(Task.Run((Action) (() => this.m_primarySearchProvider.PopulateIndex(this.m_searchEnabledStatus, extensions, productType, useNewIndexDefinition, useProductArchitectureInfo, isPlatformSpecificExtensionsForVSCodeEnabled, usePublisherDomainInfo))));
        if (this.m_secondarySearchProvider != null)
          executableTasks.Add(Task.Run((Action) (() => this.m_secondarySearchProvider.PopulateIndex(this.m_searchEnabledStatus, extensions, productType, useNewIndexDefinition, useProductArchitectureInfo, isPlatformSpecificExtensionsForVSCodeEnabled, usePublisherDomainInfo))));
        this.ExecuteAllTasks(requestContext, executableTasks);
      }
      else
        requestContext.TraceAlways(12060103, TraceLevel.Warning, "gallery", nameof (SearchIndexer), "Empty/null extension list provided for indexing.");
    }

    public void CreateIndex(IVssRequestContext requestContext, string productType = null)
    {
      if (this.m_searchProviderType == SearchProviderType.None)
        return;
      List<Task> executableTasks = new List<Task>();
      bool useNewVSCodeIndexDefinition = requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.UseNewVSCodeIndexDefinition");
      if (this.m_primarySearchProvider != null)
        executableTasks.Add(Task.Run((Action) (() => this.m_primarySearchProvider.CreateIndex(requestContext, productType, useNewVSCodeIndexDefinition))));
      if (this.m_secondarySearchProvider != null)
        executableTasks.Add(Task.Run((Action) (() => this.m_secondarySearchProvider.CreateIndex(requestContext, productType, useNewVSCodeIndexDefinition))));
      this.ExecuteAllTasks(requestContext, executableTasks);
    }

    public void CreateOrUpdateIndex(IVssRequestContext requestContext, string productType = null)
    {
      if (this.m_searchProviderType == SearchProviderType.None)
        return;
      List<Task> executableTasks = new List<Task>();
      if (this.m_primarySearchProvider != null)
        executableTasks.Add(Task.Run((Action) (() => this.m_primarySearchProvider.CreateOrUpdateIndex(requestContext, productType))));
      if (this.m_secondarySearchProvider != null)
        executableTasks.Add(Task.Run((Action) (() => this.m_secondarySearchProvider.CreateOrUpdateIndex(requestContext, productType))));
      this.ExecuteAllTasks(requestContext, executableTasks);
    }

    public void UploadSynonymMap(
      IVssRequestContext requestContext,
      string synonymMapName,
      string productType = null)
    {
      if (this.m_searchProviderType == SearchProviderType.None)
        return;
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string query = AzureSearchConstants.ExternalSearchSettingsRootPath + "/" + productType + AzureSearchConstants.SynonymPath;
      string synonymMapValue = service.GetValue(requestContext, (RegistryQuery) query, (string) null);
      List<Task> executableTasks = new List<Task>();
      if (this.m_primarySearchProvider != null)
        executableTasks.Add(Task.Run((Action) (() => this.m_primarySearchProvider.UploadSynonymMap(requestContext, synonymMapName, productType, synonymMapValue))));
      if (this.m_secondarySearchProvider != null)
        executableTasks.Add(Task.Run((Action) (() => this.m_secondarySearchProvider.UploadSynonymMap(requestContext, synonymMapName, productType, synonymMapValue))));
      this.ExecuteAllTasks(requestContext, executableTasks);
    }

    public void RemoveSynonymMap(
      IVssRequestContext requestContext,
      string synonymMapName,
      string productType = null)
    {
      if (this.m_searchProviderType == SearchProviderType.None)
        return;
      List<Task> executableTasks = new List<Task>();
      if (this.m_primarySearchProvider != null)
        executableTasks.Add(Task.Run((Action) (() => this.m_primarySearchProvider.RemoveSynonymMap(requestContext, synonymMapName, productType))));
      if (this.m_secondarySearchProvider != null)
        executableTasks.Add(Task.Run((Action) (() => this.m_secondarySearchProvider.RemoveSynonymMap(requestContext, synonymMapName, productType))));
      this.ExecuteAllTasks(requestContext, executableTasks);
    }

    public virtual void DeleteEntries(
      IVssRequestContext requestContext,
      List<PublishedExtension> extensions)
    {
      if (!this.IsExternalSearchEnabledForIndexing(requestContext) || extensions.IsNullOrEmpty<PublishedExtension>())
        return;
      List<Task> executableTasks = new List<Task>();
      if (this.m_primarySearchProvider != null)
        executableTasks.Add(Task.Run((Action) (() => this.m_primarySearchProvider.DeleteEntries(this.m_searchEnabledStatus, extensions))));
      if (this.m_secondarySearchProvider != null)
        executableTasks.Add(Task.Run((Action) (() => this.m_secondarySearchProvider.DeleteEntries(this.m_searchEnabledStatus, extensions))));
      this.ExecuteAllTasks(requestContext, executableTasks);
    }

    public void DeleteIndex(IVssRequestContext requestContext, string productType = null)
    {
      if (this.m_searchProviderType == SearchProviderType.None)
        return;
      try
      {
        this.m_primarySearchProvider?.DeleteIndex(requestContext, productType);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12060103, "gallery", nameof (SearchIndexer), ex);
        throw;
      }
    }

    private void ExecuteAllTasks(IVssRequestContext requestContext, List<Task> executableTasks)
    {
      try
      {
        Task.WaitAll(executableTasks.ToArray());
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12060103, "gallery", nameof (SearchIndexer), ex);
        throw;
      }
    }

    private bool IsExternalSearchEnabledForIndexing(IVssRequestContext requestContext)
    {
      bool flag = false;
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        flag = false;
      return this.m_searchProviderType == SearchProviderType.AzureSearchProvider && (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableExternalSearchIndexPopulationForAzureDevOps") || requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableExternalSearchIndexPopulationForVSCode") || requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableExternalSearchIndexPopulationForVS"));
    }

    private SearchEnabledStatus GetSearchEnabledStatus(IVssRequestContext requestContext)
    {
      SearchEnabledStatus searchEnabledStatus = SearchEnabledStatus.EnabledForNone;
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableExternalSearchIndexPopulationForAzureDevOps"))
        searchEnabledStatus |= SearchEnabledStatus.EnabledForAzureDevOps;
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableExternalSearchIndexPopulationForVSCode"))
        searchEnabledStatus |= SearchEnabledStatus.EnabledForVSCode;
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableExternalSearchIndexPopulationForVS"))
        searchEnabledStatus |= SearchEnabledStatus.EnabledForVS;
      return searchEnabledStatus;
    }

    internal ISearchProvider GetSearchProvider(bool isPrimary) => !isPrimary ? this.m_secondarySearchProvider : this.m_primarySearchProvider;
  }
}
