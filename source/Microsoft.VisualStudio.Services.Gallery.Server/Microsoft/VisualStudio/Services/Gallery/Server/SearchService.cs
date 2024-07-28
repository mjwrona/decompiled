// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.SearchService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal class SearchService : ISearchService, IVssFrameworkService
  {
    private const string s_area = "gallery";
    private const string s_layer = "SearchService";
    private SearchProviderType m_searchProviderType;
    private ISearchProvider m_primarySearchProvider;
    private ISearchProvider m_secondarySearchProvider;

    public SearchService()
    {
    }

    internal SearchService(
      ISearchProvider primarySearchProvider,
      ISearchProvider secondarySearchProvider,
      SearchProviderType providerType)
    {
      this.m_primarySearchProvider = primarySearchProvider;
      this.m_secondarySearchProvider = secondarySearchProvider;
      this.m_searchProviderType = providerType;
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.ExternalSearchServiceNameChangeCallback), false, AzureSearchConstants.ExternalSearchServiceNameRootPath + "/*");
      systemRequestContext.GetService<ITeamFoundationStrongBoxService>().RegisterNotification(systemRequestContext, new StrongBoxItemChangedCallback(this.OnStrongBoxItemChanged), "ConfigurationSecrets", (IEnumerable<string>) new string[2]
      {
        AzureSearchConstants.PrimaryPrefix + AzureSearchConstants.AzureSearchQueryKey,
        AzureSearchConstants.SecondaryPrefix + AzureSearchConstants.AzureSearchQueryKey
      });
      this.InitSearchProvider(systemRequestContext);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.ExternalSearchServiceNameChangeCallback));
      systemRequestContext.GetService<ITeamFoundationStrongBoxService>().UnregisterNotification(systemRequestContext, new StrongBoxItemChangedCallback(this.OnStrongBoxItemChanged));
    }

    private void ExternalSearchServiceNameChangeCallback(
      IVssRequestContext systemRequestContext,
      RegistryEntryCollection changedEntries)
    {
      this.InitSearchProvider(systemRequestContext);
    }

    private void OnStrongBoxItemChanged(
      IVssRequestContext systemRequestContext,
      IEnumerable<StrongBoxItemName> itemNames)
    {
      this.InitSearchProvider(systemRequestContext);
    }

    public ExtensionQueryResult Search(
      IVssRequestContext requestContext,
      ExtensionSearchParams searchParams,
      ExtensionQueryFlags queryFlags)
    {
      ExtensionQueryResult extensionQueryResult = (ExtensionQueryResult) null;
      SearchEnabledStatus searchEnabledStatus = this.GetSearchEnabledStatus(requestContext);
      try
      {
        extensionQueryResult = this.m_primarySearchProvider.Search(requestContext, searchEnabledStatus, searchParams, queryFlags);
      }
      catch (Exception ex1)
      {
        if (this.m_primarySearchProvider != null)
          requestContext.TraceException(12060106, "gallery", nameof (SearchService), ex1);
        try
        {
          if (this.m_secondarySearchProvider != null)
            extensionQueryResult = this.m_secondarySearchProvider.Search(requestContext, searchEnabledStatus, searchParams, queryFlags);
        }
        catch (Exception ex2)
        {
          requestContext.TraceException(12060106, "gallery", nameof (SearchService), ex2);
          throw;
        }
      }
      return extensionQueryResult;
    }

    public bool IsExternalSearchEnabled(
      IVssRequestContext requestContext,
      List<InstallationTarget> installationTargets,
      SearchOverrideFlags searchOverrides)
    {
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment || searchOverrides.HasFlag((Enum) SearchOverrideFlags.UseDbForSearch))
        return false;
      bool flag = false;
      if (this.m_searchProviderType == SearchProviderType.AzureSearchProvider)
      {
        switch (GalleryServerUtil.GetProductTypeEnumForInstallationTargets((IEnumerable<InstallationTarget>) installationTargets, true))
        {
          case GalleryProductTypesEnum.Vs:
            flag = HttpContext.Current?.Request.Cookies["EnableExternalSearchForVS"]?.Value != null || requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableExternalSearchForVS");
            break;
          case GalleryProductTypesEnum.VsCode:
            flag = HttpContext.Current?.Request.Cookies["EnableExternalSearchForVSCode"]?.Value != null || requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableExternalSearchForVSCode");
            break;
          case GalleryProductTypesEnum.Vsts:
          case GalleryProductTypesEnum.AzureDevOps:
            flag = HttpContext.Current?.Request.Cookies["EnableExternalSearchForAzureDevOps"]?.Value != null || requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableExternalSearchForAzureDevOps");
            break;
        }
      }
      else
        flag = false;
      return flag;
    }

    private void InitSearchProvider(IVssRequestContext systemRequestContext)
    {
      this.m_primarySearchProvider = (ISearchProvider) null;
      this.m_secondarySearchProvider = (ISearchProvider) null;
      SearchHelper searchHelper = new SearchHelper();
      this.m_searchProviderType = searchHelper.GetSearchProviderType(systemRequestContext);
      if (this.m_searchProviderType == SearchProviderType.None)
      {
        string format = "No search provider was configured!";
        systemRequestContext.TraceAlways(12060106, TraceLevel.Warning, "gallery", nameof (SearchService), format);
      }
      else
      {
        if (this.m_searchProviderType != SearchProviderType.AzureSearchProvider)
          return;
        try
        {
          string searchServiceName1 = searchHelper.GetSearchServiceName(systemRequestContext, true);
          string searchServiceName2 = searchHelper.GetSearchServiceName(systemRequestContext, false);
          if (!searchServiceName1.IsNullOrEmpty<char>())
          {
            string searchIndexKey = searchHelper.GetSearchIndexKey(systemRequestContext, AzureClientMode.Query, true);
            this.m_primarySearchProvider = (ISearchProvider) new AzureSearchProvider(systemRequestContext, AzureClientMode.Query, searchServiceName1, searchIndexKey);
          }
          if (!searchServiceName2.IsNullOrEmpty<char>())
          {
            string searchIndexKey = searchHelper.GetSearchIndexKey(systemRequestContext, AzureClientMode.Query, false);
            this.m_secondarySearchProvider = (ISearchProvider) new AzureSearchProvider(systemRequestContext, AzureClientMode.Query, searchServiceName2, searchIndexKey);
          }
          if (this.m_primarySearchProvider != null || this.m_secondarySearchProvider != null)
            return;
          string format = "Failed to create a search provider, check the search service name registry key setting";
          systemRequestContext.TraceAlways(12060107, TraceLevel.Warning, "gallery", nameof (SearchService), format);
        }
        catch (ExternalSearchException ex)
        {
          string format = "Failed to create a search provider, check the search service name registry key setting. Exception " + ex.Message;
          systemRequestContext.TraceAlways(12060107, TraceLevel.Warning, "gallery", nameof (SearchService), format);
          this.m_searchProviderType = SearchProviderType.None;
        }
      }
    }

    private SearchEnabledStatus GetSearchEnabledStatus(IVssRequestContext requestContext)
    {
      SearchEnabledStatus searchEnabledStatus = SearchEnabledStatus.EnabledForNone;
      if (HttpContext.Current?.Request.Cookies["EnableExternalSearchForAzureDevOps"]?.Value != null || requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableExternalSearchForAzureDevOps"))
        searchEnabledStatus |= SearchEnabledStatus.EnabledForAzureDevOps;
      if (HttpContext.Current?.Request.Cookies["EnableExternalSearchForVSCode"]?.Value != null || requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableExternalSearchForVSCode"))
        searchEnabledStatus |= SearchEnabledStatus.EnabledForVSCode;
      if (HttpContext.Current?.Request.Cookies["EnableExternalSearchForVS"]?.Value != null || requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableExternalSearchForVS"))
        searchEnabledStatus |= SearchEnabledStatus.EnabledForVS;
      return searchEnabledStatus;
    }
  }
}
