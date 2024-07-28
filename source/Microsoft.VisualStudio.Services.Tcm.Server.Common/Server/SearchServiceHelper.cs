// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.SearchServiceHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.WebApi;
using Microsoft.VisualStudio.Services.Search.WebApi.Contracts.WorkItem;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class SearchServiceHelper : ISearchServiceHelper
  {
    public List<WorkItemResponse> FetchWorkItemSearchResults(
      IVssRequestContext requestContext,
      WorkItemRequest request,
      Guid project)
    {
      using (PerfManager.Measure(requestContext, "CrossService", TraceUtils.GetActionName(nameof (FetchWorkItemSearchResults), "Search")))
      {
        try
        {
          requestContext.TraceEnter("BusinessLayer", string.Format("SearchServiceHelper.FetchWorkItemSearchResults"));
          WorkItemSearchRequest searchRequestModel = this.GetWorkItemSearchRequestModel(request);
          WorkItemSearchResponse workItemSearchResponse;
          try
          {
            workItemSearchResponse = requestContext.GetClient<SearchHttpClient>().FetchWorkItemSearchResultsAsync(searchRequestModel, project).Result;
          }
          catch (Exception ex)
          {
            requestContext.Trace(1015070, TraceLevel.Warning, "TestResultsInsights", "BusinessLayer", ex.ToString());
            workItemSearchResponse = (WorkItemSearchResponse) null;
          }
          return this.PopulateWorkItems(workItemSearchResponse);
        }
        finally
        {
          requestContext.TraceLeave("BusinessLayer", string.Format("SearchServiceHelper.FetchWorkItemSearchResults"));
        }
      }
    }

    private WorkItemSearchRequest GetWorkItemSearchRequestModel(WorkItemRequest request)
    {
      WorkItemSearchRequest searchRequestModel = new WorkItemSearchRequest();
      searchRequestModel.SearchText = request.QueryText;
      searchRequestModel.Skip = request.Skip;
      searchRequestModel.Top = request.Top;
      return searchRequestModel;
    }

    private List<WorkItemResponse> PopulateWorkItems(WorkItemSearchResponse workItemSearchResponse)
    {
      if (workItemSearchResponse == null)
        return (List<WorkItemResponse>) null;
      List<WorkItemResponse> workItemResponseList = new List<WorkItemResponse>();
      if (workItemSearchResponse != null && workItemSearchResponse.Results.Count<WorkItemResult>() > 0)
      {
        string id = "-1";
        string tags = (string) null;
        workItemResponseList = workItemSearchResponse.Results.Where<WorkItemResult>((Func<WorkItemResult, bool>) (response => response.Fields.TryGetValue("system.id", out id))).Where<WorkItemResult>((Func<WorkItemResult, bool>) (response => response.Fields.TryGetValue("system.tags", out tags))).Select<WorkItemResult, WorkItemResponse>((Func<WorkItemResult, WorkItemResponse>) (response => new WorkItemResponse(id, tags))).ToList<WorkItemResponse>();
      }
      return workItemResponseList;
    }
  }
}
