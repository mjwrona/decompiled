// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.ExtensionQueryHelper
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class ExtensionQueryHelper : IExtensionQueryHelper
  {
    private string Layer;
    private int TracePoint;

    public ExtensionQueryHelper(string layer, int tracePoint)
    {
      this.Layer = layer;
      this.TracePoint = tracePoint;
    }

    public IList<PublishedExtension> GetAllExtensions(
      IVssRequestContext requestContext,
      string[] installationTargets)
    {
      IPublishedExtensionService service = requestContext.GetService<IPublishedExtensionService>();
      return this.GetAllExtensions(requestContext, service, installationTargets, ExtensionQueryFlags.AllAttributes);
    }

    public IList<PublishedExtension> GetAllExtensions(
      IVssRequestContext requestContext,
      IPublishedExtensionService extensionService,
      string[] installationTargets)
    {
      return this.GetAllExtensions(requestContext, extensionService, installationTargets, ExtensionQueryFlags.AllAttributes);
    }

    public IList<PublishedExtension> GetAllExtensions(
      IVssRequestContext requestContext,
      IPublishedExtensionService extensionService,
      string[] installationTargets,
      ExtensionQueryFlags queryOptions)
    {
      return this.GetAllExtensions(requestContext, extensionService, installationTargets, queryOptions, 2000, (string) null);
    }

    public IList<PublishedExtension> GetAllExtensions(
      IVssRequestContext requestContext,
      IPublishedExtensionService extensionService,
      string[] installationTargets,
      ExtensionQueryFlags queryOptions,
      string searchTerm)
    {
      return this.GetAllExtensions(requestContext, extensionService, installationTargets, queryOptions, 2000, searchTerm);
    }

    public IList<PublishedExtension> GetAllExtensions(
      IVssRequestContext requestContext,
      IPublishedExtensionService extensionService,
      string[] installationTargets,
      ExtensionQueryFlags queryOptions,
      PublishedExtensionFlags? includeFlags,
      PublishedExtensionFlags? excludeFlags)
    {
      return this.InternalGetAllExtensions(requestContext, extensionService, installationTargets, queryOptions, 2000, (string) null, false, includeFlags, excludeFlags);
    }

    public IList<PublishedExtension> GetAllExtensions(
      IVssRequestContext requestContext,
      IPublishedExtensionService extensionService,
      string[] installationTargets,
      ExtensionQueryFlags queryOptions,
      int pageSize,
      string searchTerm)
    {
      return this.InternalGetAllExtensions(requestContext, extensionService, installationTargets, queryOptions, pageSize, searchTerm, false, new PublishedExtensionFlags?(), new PublishedExtensionFlags?());
    }

    public IList<PublishedExtension> GetAllExtensions(
      IVssRequestContext requestContext,
      IPublishedExtensionService extensionService,
      string[] installationTargets,
      ExtensionQueryFlags queryOptions,
      string searchTerm,
      bool includePrivate)
    {
      return this.InternalGetAllExtensions(requestContext, extensionService, installationTargets, queryOptions, 2000, searchTerm, includePrivate, new PublishedExtensionFlags?(), new PublishedExtensionFlags?());
    }

    public virtual IList<PublishedExtension> GetPagedExtensions(
      IVssRequestContext requestContext,
      IPublishedExtensionService extensionService,
      string[] installationTargets,
      ExtensionQueryFlags queryOptions,
      int pageSize,
      int currentPageNumber,
      string searchTerm,
      bool includePrivate)
    {
      IList<PublishedExtension> pagedExtensions = (IList<PublishedExtension>) null;
      if (extensionService != null)
        pagedExtensions = this.InternalGetPagedExtensions(requestContext, extensionService, installationTargets, queryOptions, pageSize, currentPageNumber, searchTerm, includePrivate, new PublishedExtensionFlags?(), new PublishedExtensionFlags?());
      return pagedExtensions;
    }

    private IList<PublishedExtension> InternalGetAllExtensions(
      IVssRequestContext requestContext,
      IPublishedExtensionService extensionService,
      string[] installationTargets,
      ExtensionQueryFlags queryOptions,
      int pageSize,
      string searchTerm,
      bool includePrivate,
      PublishedExtensionFlags? includeFlags,
      PublishedExtensionFlags? excludeFlags)
    {
      requestContext.TraceEnter(this.TracePoint, "Gallery", this.Layer, nameof (InternalGetAllExtensions));
      List<PublishedExtension> allExtensions = new List<PublishedExtension>();
      int currentPageNumber = 1;
      if (extensionService != null)
      {
        List<PublishedExtension> list;
        do
        {
          list = this.InternalGetPagedExtensions(requestContext, extensionService, installationTargets, queryOptions, pageSize, currentPageNumber, searchTerm, includePrivate, includeFlags, excludeFlags).ToList<PublishedExtension>();
          ++currentPageNumber;
          if (list != null)
            allExtensions.AddRange((IEnumerable<PublishedExtension>) list);
        }
        while (!list.IsNullOrEmpty<PublishedExtension>());
      }
      requestContext.TraceLeave(this.TracePoint, "Gallery", this.Layer, nameof (InternalGetAllExtensions));
      return (IList<PublishedExtension>) allExtensions;
    }

    private IList<PublishedExtension> InternalGetPagedExtensions(
      IVssRequestContext requestContext,
      IPublishedExtensionService extensionService,
      string[] installationTargets,
      ExtensionQueryFlags queryOptions,
      int pageSize,
      int currentPageNumber,
      string searchTerm,
      bool includePrivate,
      PublishedExtensionFlags? includeFlags,
      PublishedExtensionFlags? excludeFlags)
    {
      bool useSearch;
      ExtensionQuery allExtensionsQuery = this.GetAllExtensionsQuery(requestContext, installationTargets, pageSize, currentPageNumber, queryOptions, searchTerm, includeFlags, excludeFlags, out useSearch);
      ExtensionQueryResult queryResult;
      try
      {
        string message1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Querying extensions: page {0}", (object) currentPageNumber);
        requestContext.Trace(this.TracePoint, TraceLevel.Info, "Gallery", this.Layer, message1);
        queryResult = useSearch ? extensionService.SearchExtensions(requestContext, allExtensionsQuery, (string) null, (SearchOverrideFlags) ((includePrivate ? 2 : 0) | 4)) : extensionService.QueryExtensions(requestContext, allExtensionsQuery, (string) null);
        string message2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Queried extensions: page {0}", (object) currentPageNumber);
        requestContext.Trace(this.TracePoint, TraceLevel.Info, "Gallery", this.Layer, message2);
      }
      catch (InvalidExtensionQueryException ex)
      {
        requestContext.TraceException(this.TracePoint, "Gallery", this.Layer, (Exception) ex);
        throw;
      }
      catch (ArgumentException ex)
      {
        requestContext.TraceException(this.TracePoint, "Gallery", this.Layer, (Exception) ex);
        throw;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(this.TracePoint, "Gallery", this.Layer, ex);
        throw;
      }
      return (IList<PublishedExtension>) this.GetAllExtensionsFromExtensionQueryResult(requestContext, queryResult);
    }

    private ExtensionQuery GetAllExtensionsQuery(
      IVssRequestContext requestContext,
      string[] installationTargets,
      int pageSize,
      int pageNumber,
      ExtensionQueryFlags extensionQueryFlags,
      string searchTerm,
      PublishedExtensionFlags? includeFlags,
      PublishedExtensionFlags? excludeFlags,
      out bool useSearch)
    {
      requestContext.TraceEnter(this.TracePoint, "Gallery", this.Layer, "GetAllExtensionsQueryForInstallationTarget");
      ExtensionQuery allExtensionsQuery = new ExtensionQuery();
      QueryFilter queryFilter = new QueryFilter();
      List<FilterCriteria> filterCriteriaList1 = new List<FilterCriteria>();
      if (installationTargets != null && installationTargets.Length != 0)
      {
        foreach (string installationTarget in installationTargets)
          filterCriteriaList1.Add(new FilterCriteria()
          {
            FilterType = 8,
            Value = installationTarget
          });
      }
      int num;
      if (includeFlags.HasValue)
      {
        List<FilterCriteria> filterCriteriaList2 = filterCriteriaList1;
        FilterCriteria filterCriteria = new FilterCriteria();
        filterCriteria.FilterType = 13;
        num = (int) includeFlags.Value;
        filterCriteria.Value = num.ToString();
        filterCriteriaList2.Add(filterCriteria);
      }
      if (excludeFlags.HasValue)
      {
        List<FilterCriteria> filterCriteriaList3 = filterCriteriaList1;
        FilterCriteria filterCriteria = new FilterCriteria();
        filterCriteria.FilterType = 12;
        num = (int) excludeFlags.Value;
        filterCriteria.Value = num.ToString();
        filterCriteriaList3.Add(filterCriteria);
      }
      if (!string.IsNullOrWhiteSpace(searchTerm))
      {
        filterCriteriaList1.Add(new FilterCriteria()
        {
          FilterType = 10,
          Value = searchTerm
        });
        useSearch = true;
      }
      else
        useSearch = false;
      queryFilter.Criteria = filterCriteriaList1;
      queryFilter.PageSize = pageSize;
      if (pageSize > 1000)
        queryFilter.ForcePageSize = true;
      queryFilter.PageNumber = pageNumber;
      queryFilter.SortBy = 2;
      queryFilter.SortOrder = 0;
      allExtensionsQuery.Filters = new List<QueryFilter>()
      {
        queryFilter
      };
      allExtensionsQuery.Flags = extensionQueryFlags;
      requestContext.TraceLeave(this.TracePoint, "Gallery", this.Layer, "GetAllExtensionsQueryForInstallationTarget");
      return allExtensionsQuery;
    }

    private List<PublishedExtension> GetAllExtensionsFromExtensionQueryResult(
      IVssRequestContext requestContext,
      ExtensionQueryResult queryResult)
    {
      requestContext.TraceEnter(this.TracePoint, "Gallery", this.Layer, nameof (GetAllExtensionsFromExtensionQueryResult));
      List<PublishedExtension> extensionQueryResult = new List<PublishedExtension>();
      if (queryResult != null && queryResult.Results != null && queryResult.Results.Count > 0 && queryResult.Results[0].Extensions != null)
        extensionQueryResult.AddRange((IEnumerable<PublishedExtension>) queryResult.Results[0].Extensions);
      requestContext.TraceLeave(this.TracePoint, "Gallery", this.Layer, nameof (GetAllExtensionsFromExtensionQueryResult));
      return extensionQueryResult;
    }
  }
}
