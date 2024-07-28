// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.WorkItemSearchResponseConvertor
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public static class WorkItemSearchResponseConvertor
  {
    private static readonly IDictionary<string, string> s_filterKeyMapping = (IDictionary<string, string>) new FriendlyDictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.Project,
        Microsoft.VisualStudio.Services.Search.WebApi.Contracts.WorkItem.Constants.FacetNames.Project
      },
      {
        Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.WorkItemAreaPath,
        Microsoft.VisualStudio.Services.Search.WebApi.Contracts.WorkItem.Constants.FacetNames.WorkItemAreaPath
      },
      {
        Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.WorkItemAssignedTo,
        Microsoft.VisualStudio.Services.Search.WebApi.Contracts.WorkItem.Constants.FacetNames.WorkItemAssignedTo
      },
      {
        Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.WorkItemState,
        Microsoft.VisualStudio.Services.Search.WebApi.Contracts.WorkItem.Constants.FacetNames.WorkItemState
      },
      {
        Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.WorkItemType,
        Microsoft.VisualStudio.Services.Search.WebApi.Contracts.WorkItem.Constants.FacetNames.WorkItemType
      }
    };

    private static ILocationDataProvider GetlocationDataProvider(IVssRequestContext requestContext) => requestContext.GetService<ILocationService>().GetLocationData(requestContext, ServiceInstanceTypes.TFS);

    private static string GetWorkItemUrl(
      IVssRequestContext requestContext,
      ILocationDataProvider tfsLocationData,
      int workItemId)
    {
      try
      {
        return tfsLocationData.GetResourceUri(requestContext, "wit", WitConstants.WorkItemTrackingLocationIds.WorkItems, (object) new
        {
          id = workItemId
        }).AbsoluteUri;
      }
      catch (Exception ex)
      {
        throw new SearchServiceException(FormattableString.Invariant(FormattableStringFactory.Create("Search platform response: Url cannot be constructed for the work item having id {0} due to {1}", (object) workItemId, (object) ex)));
      }
    }

    public static Microsoft.VisualStudio.Services.Search.WebApi.Contracts.WorkItem.WorkItemSearchResponse ToNewResponseContract(
      this Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.WorkItemSearchResponse response,
      IVssRequestContext requestContext,
      string systemIdField)
    {
      if (response == null)
        throw new ArgumentNullException(nameof (response));
      Microsoft.VisualStudio.Services.Search.WebApi.Contracts.WorkItem.WorkItemSearchResponse responseContract = new Microsoft.VisualStudio.Services.Search.WebApi.Contracts.WorkItem.WorkItemSearchResponse()
      {
        Count = response.Results.Count
      };
      List<Microsoft.VisualStudio.Services.Search.WebApi.Contracts.WorkItem.WorkItemResult> workItemResultList = new List<Microsoft.VisualStudio.Services.Search.WebApi.Contracts.WorkItem.WorkItemResult>();
      ILocationDataProvider tfsLocationData = WorkItemSearchResponseConvertor.GetlocationDataProvider(requestContext);
      foreach (Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.WorkItemResult workItemResult1 in response.Results.Values)
      {
        Microsoft.VisualStudio.Services.Search.WebApi.Contracts.WorkItem.WorkItemResult workItemResult2 = new Microsoft.VisualStudio.Services.Search.WebApi.Contracts.WorkItem.WorkItemResult()
        {
          Project = new Project()
          {
            Name = workItemResult1.Project,
            Id = new Guid(workItemResult1.ProjectId)
          }
        };
        IDictionary<string, string> dictionary = (IDictionary<string, string>) new Dictionary<string, string>();
        foreach (WorkItemField field in workItemResult1.Fields)
        {
          if (field.ReferenceName.Equals(systemIdField, StringComparison.OrdinalIgnoreCase))
            workItemResult2.Url = WorkItemSearchResponseConvertor.GetWorkItemUrl(requestContext, tfsLocationData, int.Parse(field.Value, (IFormatProvider) CultureInfo.InvariantCulture));
          dictionary[field.ReferenceName] = field.Value;
        }
        workItemResult2.Fields = dictionary;
        Microsoft.VisualStudio.Services.Search.WebApi.Contracts.WorkItem.WorkItemResult workItemResult3 = workItemResult2;
        IEnumerable<Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.WorkItemHit> hits = workItemResult1.Hits;
        IEnumerable<Microsoft.VisualStudio.Services.Search.WebApi.Contracts.WorkItem.WorkItemHit> workItemHits = hits != null ? hits.Select<Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.WorkItemHit, Microsoft.VisualStudio.Services.Search.WebApi.Contracts.WorkItem.WorkItemHit>((Func<Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.WorkItemHit, Microsoft.VisualStudio.Services.Search.WebApi.Contracts.WorkItem.WorkItemHit>) (h => new Microsoft.VisualStudio.Services.Search.WebApi.Contracts.WorkItem.WorkItemHit()
        {
          FieldReferenceName = h.FieldReferenceName,
          Highlights = h.Highlights
        })) : (IEnumerable<Microsoft.VisualStudio.Services.Search.WebApi.Contracts.WorkItem.WorkItemHit>) null;
        workItemResult3.Hits = workItemHits;
        workItemResultList.Add(workItemResult2);
      }
      responseContract.Results = (IEnumerable<Microsoft.VisualStudio.Services.Search.WebApi.Contracts.WorkItem.WorkItemResult>) workItemResultList;
      InfoCodes infoCode;
      ErrorCodeConvertor.TryConvertToInfoCode(response.Errors, out infoCode);
      responseContract.InfoCode = (int) infoCode;
      Microsoft.VisualStudio.Services.Search.WebApi.Contracts.WorkItem.WorkItemSearchResponse itemSearchResponse = responseContract;
      IEnumerable<FilterCategory> filterCategories = response.FilterCategories;
      Dictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>> dictionary1 = filterCategories != null ? filterCategories.ToDictionary<FilterCategory, string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>>((Func<FilterCategory, string>) (fc => WorkItemSearchResponseConvertor.s_filterKeyMapping[fc.Name]), (Func<FilterCategory, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>>) (fc =>
      {
        IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter> filters = fc.Filters;
        return filters == null ? (IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>) null : filters.Select<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>((Func<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>) (f => new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter(f.Name, f.Id, f.ResultCount)));
      })) : (Dictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>>) null;
      itemSearchResponse.Facets = (IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>>) dictionary1;
      return responseContract;
    }
  }
}
