// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Extension.Cache.VsCodeExtensionsCacheBuilder
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Extension.Cache
{
  internal class VsCodeExtensionsCacheBuilder : ProductExtensionsCacheBuilder
  {
    private const int m_VsCodeExtensionCacheCircuitBreakerExceptionID = 160011;
    private const int m_VsCodeExtensionCacheCircuitBreakerThrottlingExceptionID = 160012;
    private int m_VsCodeExtensionsCacheTimeoutInSeconds = 600;

    public override int CacheTimeoutInSeconds
    {
      get => this.m_VsCodeExtensionsCacheTimeoutInSeconds;
      set => this.m_VsCodeExtensionsCacheTimeoutInSeconds = value;
    }

    protected override string ProductType
    {
      get => "vscode";
      set
      {
      }
    }

    protected override int CiruitBreakerExceptionId
    {
      get => 160011;
      set
      {
      }
    }

    protected override int CircuitBreakerThrottlingExceptionID
    {
      get => 160012;
      set
      {
      }
    }

    protected override List<PublishedExtension> GetExtensionPage(
      IVssRequestContext requestContext,
      int pageNumber)
    {
      List<PublishedExtension> extensionPage = new List<PublishedExtension>();
      ExtensionQuery extensionQuery = new ExtensionQuery();
      extensionQuery.Filters = new List<QueryFilter>();
      extensionQuery.Flags = ExtensionQueryFlags.AllAttributes;
      QueryFilter queryFilter = new QueryFilter();
      queryFilter.Criteria = new List<FilterCriteria>();
      queryFilter.Criteria.AddRange((IEnumerable<FilterCriteria>) this.GetInstallationTargetFilters());
      queryFilter.PageNumber = pageNumber;
      queryFilter.PageSize = 2000;
      queryFilter.ForcePageSize = true;
      queryFilter.DoNotUseCache = true;
      queryFilter.SortBy = 4;
      queryFilter.SortOrder = 0;
      extensionQuery.Filters.Add(queryFilter);
      ExtensionQueryResult extensionQueryResult = (this.m_PublishedExtensionService ?? requestContext.GetService<IPublishedExtensionService>()).QueryExtensions(requestContext, extensionQuery, (string) null, (Uri) null, true);
      if (extensionQueryResult != null)
      {
        int? count = extensionQueryResult.Results?.Count;
        int num = 0;
        if (count.GetValueOrDefault() > num & count.HasValue && extensionQueryResult.Results[0].Extensions != null)
          extensionPage = extensionQueryResult.Results[0].Extensions;
      }
      return extensionPage;
    }
  }
}
