// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.LibraryWorkItemsDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 53130500-4E07-459F-A593-E61E658993AF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.WebPlatform.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Settings;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins
{
  public abstract class LibraryWorkItemsDataProvider
  {
    protected readonly List<string> m_fixedColumnsList = new List<string>()
    {
      WorkItemFieldRefNames.Id,
      WorkItemFieldRefNames.Title
    };
    protected readonly List<string> m_defaultColumnsList = new List<string>()
    {
      WorkItemFieldRefNames.AssignedTo,
      WorkItemFieldRefNames.AreaPath,
      WorkItemFieldRefNames.Tags,
      WorkItemFieldRefNames.State
    };
    protected List<LibraryWorkItemsDataProvider.WorkItemQueryAndType> m_queries;
    protected static readonly string c_wiqlSourcePrefix = "[Source].";
    private static readonly string c_LibraryRegistryRootPath = "/Service/TestManagement/Settings/Library/";
    private static readonly string c_InitialLibraryWorkItemPageSize = LibraryWorkItemsDataProvider.c_LibraryRegistryRootPath + "InitialWorkItemPageSize";
    private static readonly string c_DelayLoadedLibraryWorkItemPageSize = LibraryWorkItemsDataProvider.c_LibraryRegistryRootPath + "DelayLoadedWorkItemPageSize";
    private static readonly string c_TestCasesPivotColumnOptions = "TestCasesPivotColumnOptions";
    private static readonly int c_DefaultLibraryInitialWorkItemPageSize = 25;
    private static readonly int c_DefaultLibraryDelayLoadedWorkItemPageSize = 250;

    protected LibraryWorkItemsDataProvider.WorkItemQueryAndType GetWorkItemQueryAndType(
      IVssRequestContext requestContext,
      TestPlansLibraryQuery libraryQuery)
    {
      LibraryWorkItemsDataProvider.WorkItemQueryAndType itemQueryAndType = this.m_queries.Where<LibraryWorkItemsDataProvider.WorkItemQueryAndType>((Func<LibraryWorkItemsDataProvider.WorkItemQueryAndType, bool>) (q => q.QueryType == libraryQuery)).FirstOrDefault<LibraryWorkItemsDataProvider.WorkItemQueryAndType>();
      if (itemQueryAndType != null)
        return itemQueryAndType;
      requestContext.Trace(1015694, TraceLevel.Error, "TestManagement", "WebService", string.Format("libraryQuery {0} couldn't be mapped to the set of queries", (object) libraryQuery));
      return itemQueryAndType;
    }

    protected object GetPaginatedWorkItemData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext)
    {
      Guid empty1 = Guid.Empty;
      string empty2 = string.Empty;
      (Guid projectId, string projectName) = this.GetProjectDetails(requestContext);
      LibraryWorkItemsDataProviderRequest requestData;
      int continuationToken = this.ReadWorkItemDataProviderArgs(providerContext, out requestData);
      LibraryWorkItemsDataProvider.WorkItemQueryAndType itemQueryAndType = this.GetWorkItemQueryAndType(requestContext, requestData.LibraryQueryType);
      LibraryWorkItemsData data = new LibraryWorkItemsData();
      data.ReturnCode = LibraryTestCasesDataReturnCode.Success;
      if (itemQueryAndType == null)
      {
        requestContext.Trace(1015694, TraceLevel.Error, "TestManagement", "WebService", "Received null query from GetWorkItemQueryAndType in LibraryWorkItemsDataProvider");
        data.ReturnCode = LibraryTestCasesDataReturnCode.Error;
        return (object) data;
      }
      try
      {
        Hashtable workItemContext = this.GetWorkItemContext(projectName);
        LibraryWorkItemsDataProvider.WorkItemQueryAndType query = this.AppendFilters(itemQueryAndType, requestData.FilterValues);
        int num = this.GetWorkItemQueryLimit(requestContext) - 1;
        int workItemPageSize1 = this.GetInitialWorkItemPageSize(requestContext);
        int workItemPageSize2 = this.GetDelayLoadedWorkItemPageSize(requestContext);
        if (string.IsNullOrEmpty(requestData.OrderByField))
          requestData.OrderByField = WorkItemFieldRefNames.Id;
        bool flag = this.IsOrderByBasedOnId(requestData);
        int pageSizeOverride;
        if (continuationToken > 0)
        {
          if (!flag)
            throw new ArgumentException("When providing with continuationToken, OrderByField should be " + WorkItemFieldRefNames.Id + " only");
          pageSizeOverride = workItemPageSize2;
          query = this.AppendWhereClauseForId(query, continuationToken, requestData.IsAscending);
        }
        else
          pageSizeOverride = flag || !requestData.WorkItemIds.IsNullOrEmpty<int>() ? workItemPageSize1 : num;
        if (!string.IsNullOrEmpty(requestData.OrderByField))
          query = this.AppendOrderBy(query, requestData.OrderByField, requestData.IsAscending);
        LibraryWorkItemsDataProvider.WorkItemQueryAndType queryAndType = this.AppendWiqlMode(requestContext, query);
        IList<int> intList1 = (IList<int>) new List<int>();
        IList<int> intList2;
        if (requestData.WorkItemIds.IsNullOrEmpty<int>())
        {
          bool hasMoreResults;
          IList<int> andLogIfOverflow = this.GetWorkItemIdsAndLogIfOverflow(requestContext, projectId, queryAndType, workItemContext, out hasMoreResults, pageSizeOverride);
          if (pageSizeOverride == num)
          {
            if (hasMoreResults)
              data.ExceededWorkItemQueryLimit = true;
            intList2 = (IList<int>) andLogIfOverflow.Take<int>(workItemPageSize1).ToList<int>();
            if (andLogIfOverflow.Count > workItemPageSize1)
              data.HasMoreElements = true;
            data.WorkItemIds = andLogIfOverflow;
          }
          else
          {
            data.HasMoreElements = hasMoreResults;
            intList2 = andLogIfOverflow;
            if (hasMoreResults)
              data.ContinuationToken = andLogIfOverflow.Last<int>().ToString();
          }
        }
        else
          intList2 = requestData.WorkItemIds;
        List<string> columnOptions = this.GetColumnOptions(requestContext, projectId, requestData);
        data.ColumnOptions = columnOptions;
        List<string> list = this.m_fixedColumnsList.Concat<string>((IEnumerable<string>) columnOptions).ToList<string>();
        if (!intList2.IsNullOrEmpty<int>())
        {
          IList<WorkItemFieldData> workItemFieldData = this.GetWorkItemFieldData(requestContext, intList2, (IList<string>) list, queryAndType.QueryType);
          this.PostProcessWorkItemFields(requestContext, (IList<string>) list, intList2, workItemFieldData, data, queryAndType.QueryType);
        }
        else
          requestContext.Trace(1015694, TraceLevel.Warning, "TestManagement", "WebService", "workItemIdsToProcess is " + (intList2 == null ? "null" : "empty"));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1015694, "TestManagement", "WebService", ex);
        data.ReturnCode = LibraryTestCasesDataReturnCode.Error;
      }
      return (object) data;
    }

    protected int ReadWorkItemDataProviderArgs(
      DataProviderContext providerContext,
      out LibraryWorkItemsDataProviderRequest requestData)
    {
      requestData = providerContext.GetPropertiesAs<LibraryWorkItemsDataProviderRequest>();
      int result = 0;
      if (requestData.LibraryQueryType == TestPlansLibraryQuery.None)
        requestData.LibraryQueryType = TestPlansLibraryQuery.AllTestCases;
      if (!requestData.ContinuationToken.IsNullOrEmpty<char>() && !int.TryParse(requestData.ContinuationToken, out result))
        throw new ArgumentException("Continuation token not valid");
      if (requestData.FilterValues != null && requestData.FilterValues.Count > 0)
        requestData.FilterValues.ForEach((Action<TestPlansLibraryWorkItemFilter>) (fv => this.ValidateWorkItemFilter(fv)));
      return result;
    }

    protected IList<int> GetWorkItemIdsAndLogIfOverflow(
      IVssRequestContext requestContext,
      Guid projectId,
      LibraryWorkItemsDataProvider.WorkItemQueryAndType queryAndType,
      Hashtable workItemContext,
      out bool hasMoreResults,
      int pageSizeOverride = -1)
    {
      int workItemQueryLimit = this.GetWorkItemQueryLimit(requestContext);
      int pageSize = pageSizeOverride <= 0 || pageSizeOverride >= workItemQueryLimit ? workItemQueryLimit - 1 : pageSizeOverride;
      IList<int> workItemIds = this.GetWorkItemIds(requestContext, queryAndType, workItemContext, pageSize, out hasMoreResults);
      if (!(pageSize == workItemQueryLimit - 1 & hasMoreResults))
        return workItemIds;
      ClientTraceData properties = new ClientTraceData();
      requestContext.GetService<ClientTraceService>().Publish(requestContext, "TestManagement", "LibraryTestCasesQueryLimitReached", properties);
      return workItemIds;
    }

    protected IList<WorkItemFieldData> GetWorkItemFieldData(
      IVssRequestContext requestContext,
      IList<int> workItemIds,
      IList<string> fieldsList,
      TestPlansLibraryQuery queryType)
    {
      using (PerformanceTimer.StartMeasure(requestContext, "LibraryWorkItemsDataProvider.GetWorkItemFieldValues", Enum.GetName(typeof (TestPlansLibraryQuery), (object) queryType)))
        return (IList<WorkItemFieldData>) requestContext.GetService<ITeamFoundationWorkItemService>().GetWorkItemFieldValues(requestContext, (IEnumerable<int>) workItemIds, (IEnumerable<string>) fieldsList, useWorkItemIdentity: true).ToList<WorkItemFieldData>();
    }

    protected Hashtable GetWorkItemContext(string projectName) => new Hashtable((IEqualityComparer) TFStringComparer.WorkItemQueryText)
    {
      [(object) WiqlAdapter.Project] = (object) projectName
    };

    protected void PostProcessWorkItemFields(
      IVssRequestContext requestContext,
      IList<string> fieldsList,
      IList<int> workItemIds,
      IList<WorkItemFieldData> workItemFieldData,
      LibraryWorkItemsData data,
      TestPlansLibraryQuery queryType)
    {
      using (PerformanceTimer.StartMeasure(requestContext, "LibraryWorkItemsDataProvider.PostProcessing", Enum.GetName(typeof (TestPlansLibraryQuery), (object) queryType)))
      {
        if (!workItemFieldData.IsNullOrEmpty<WorkItemFieldData>())
        {
          workItemFieldData = (IList<WorkItemFieldData>) workItemFieldData.OrderBy<WorkItemFieldData, int>((Func<WorkItemFieldData, int>) (x => workItemIds.IndexOf(x.Id))).ToList<WorkItemFieldData>();
          data.WorkItems = (IList<WorkItemDetails>) new List<WorkItemDetails>();
          int count = fieldsList.Count;
          foreach (WorkItemFieldData workItemFieldData1 in (IEnumerable<WorkItemFieldData>) workItemFieldData)
          {
            WorkItemDetails workItemDetails = new WorkItemDetails();
            workItemDetails.Id = workItemFieldData1.Id;
            workItemDetails.Name = workItemFieldData1.Title;
            workItemDetails.WorkItemFields = new List<object>();
            for (int index = 0; index < count; ++index)
            {
              object fieldValue = workItemFieldData1.GetFieldValue(requestContext, fieldsList[index]);
              if (fieldValue != null)
                workItemDetails.WorkItemFields.Add((object) new Dictionary<string, object>()
                {
                  {
                    fieldsList[index],
                    fieldValue
                  }
                });
            }
            data.WorkItems.Add(workItemDetails);
          }
        }
        else
          requestContext.Trace(1015694, TraceLevel.Warning, "TestManagement", "WebService", "queryResult.WorkItemIds is null or empty");
      }
    }

    protected (Guid projectId, string projectName) GetProjectDetails(
      IVssRequestContext requestContext)
    {
      Guid guid = Guid.Empty;
      string str = string.Empty;
      using (PerformanceTimer.StartMeasure(requestContext, "LibraryWorkItemsDataProvider.GetProject"))
      {
        ProjectInfo project = requestContext.GetService<IRequestProjectService>().GetProject(requestContext);
        str = project.Name;
        guid = project.Id;
      }
      return (guid, str);
    }

    protected int GetWorkItemQueryLimit(IVssRequestContext requestContext)
    {
      int maxQueryResultSize = requestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(requestContext).MaxQueryResultSize;
      requestContext.Trace(1015694, TraceLevel.Info, "TestManagement", "WebService", string.Format("maxQueryResultSize = {0}", (object) maxQueryResultSize));
      return maxQueryResultSize;
    }

    protected int GetInitialWorkItemPageSize(IVssRequestContext requestContext)
    {
      int workItemQueryLimit = this.GetWorkItemQueryLimit(requestContext);
      int num = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) LibraryWorkItemsDataProvider.c_InitialLibraryWorkItemPageSize, LibraryWorkItemsDataProvider.c_DefaultLibraryInitialWorkItemPageSize);
      int workItemPageSize = num >= workItemQueryLimit ? workItemQueryLimit - 1 : num;
      requestContext.Trace(1015694, TraceLevel.Info, "TestManagement", "WebService", string.Format("defaultWorkItemPageSize = {0}", (object) workItemPageSize));
      return workItemPageSize;
    }

    protected int GetDelayLoadedWorkItemPageSize(IVssRequestContext requestContext)
    {
      int workItemQueryLimit = this.GetWorkItemQueryLimit(requestContext);
      int num = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) LibraryWorkItemsDataProvider.c_DelayLoadedLibraryWorkItemPageSize, LibraryWorkItemsDataProvider.c_DefaultLibraryDelayLoadedWorkItemPageSize);
      int workItemPageSize = num >= workItemQueryLimit ? workItemQueryLimit - 1 : num;
      requestContext.Trace(1015694, TraceLevel.Info, "TestManagement", "WebService", string.Format("delayLoadedWorkItemPageSize = {0}", (object) workItemPageSize));
      return workItemPageSize;
    }

    protected LibraryWorkItemsDataProvider.WorkItemQueryAndType AppendWhereClauseForId(
      LibraryWorkItemsDataProvider.WorkItemQueryAndType query,
      int continuationToken,
      bool isAscending)
    {
      string str = string.Empty;
      if (query.WitQueryType != QueryType.WorkItems)
        str = LibraryWorkItemsDataProvider.c_wiqlSourcePrefix;
      query.Query = !isAscending ? string.Format("{0} {1}", (object) query.Query, (object) string.Format(" AND {0} < {1}", (object) (str + WorkItemFieldRefNames.Id), (object) continuationToken)) : string.Format("{0} {1}", (object) query.Query, (object) string.Format(" AND {0} > {1}", (object) (str + WorkItemFieldRefNames.Id), (object) continuationToken));
      return query;
    }

    protected LibraryWorkItemsDataProvider.WorkItemQueryAndType AppendOrderBy(
      LibraryWorkItemsDataProvider.WorkItemQueryAndType query,
      string fieldName,
      bool isAscending = true)
    {
      string str1 = query.Query;
      string str2 = string.Empty;
      if (query.WitQueryType != QueryType.WorkItems)
        str2 = LibraryWorkItemsDataProvider.c_wiqlSourcePrefix;
      if (!string.IsNullOrEmpty(fieldName))
      {
        string str3 = isAscending ? "ASC" : "DESC";
        str1 = string.Format("{0} {1}", (object) str1, (object) (" ORDER BY " + str2 + fieldName + " " + str3));
      }
      query.Query = str1;
      return query;
    }

    protected LibraryWorkItemsDataProvider.WorkItemQueryAndType AppendWiqlMode(
      IVssRequestContext requestContext,
      LibraryWorkItemsDataProvider.WorkItemQueryAndType query)
    {
      string str = string.Empty;
      if (query.WitQueryType != QueryType.WorkItems)
      {
        switch (query.WitQueryType)
        {
          case QueryType.LinksOneHopMustContain:
            str = "mode(MustContain)";
            break;
          case QueryType.LinksOneHopDoesNotContain:
            str = "mode(DoesNotContain)";
            break;
          default:
            requestContext.Trace(1015694, TraceLevel.Warning, "TestManagement", "WebService", string.Format("Unknown QueryType value encountered in AppendWiqlMode: {0}", (object) query.WitQueryType));
            break;
        }
      }
      if (!string.IsNullOrEmpty(str))
        query.Query = query.Query + " " + str;
      return query;
    }

    protected bool IsOrderByBasedOnId(LibraryWorkItemsDataProviderRequest requestData) => !string.IsNullOrEmpty(requestData.OrderByField) && requestData.OrderByField.IndexOf(WorkItemFieldRefNames.Id, StringComparison.OrdinalIgnoreCase) >= 0;

    protected void ValidateWorkItemFilter(TestPlansLibraryWorkItemFilter filter)
    {
      if (string.IsNullOrEmpty(filter.FieldName) || filter.FieldValues == null || filter.FieldValues.Count == 0 || filter.FieldValues.Any<string>((Func<string, bool>) (fv => string.IsNullOrEmpty(fv))))
        throw new ArgumentException("Invalid filterValues argument");
      if (!filter.FieldName.Equals(WorkItemFieldRefNames.Priority, StringComparison.OrdinalIgnoreCase))
        return;
      foreach (string fieldValue in filter.FieldValues)
      {
        if (!int.TryParse(fieldValue, out int _))
          throw new ArgumentException("Priority provided in filter must be a valid integer");
      }
    }

    protected string BuildWorkItemFilters(
      List<TestPlansLibraryWorkItemFilter> filterValues,
      List<string> filterFieldsToConsider,
      string wiqlColumnPrefix = "")
    {
      string currentFilter = string.Empty;
      if (filterFieldsToConsider.Contains(WorkItemFieldRefNames.Title))
      {
        TestPlansLibraryWorkItemFilter libraryWorkItemFilter = filterValues.Where<TestPlansLibraryWorkItemFilter>((Func<TestPlansLibraryWorkItemFilter, bool>) (f => f.FieldName.Equals(WorkItemFieldRefNames.Title, StringComparison.OrdinalIgnoreCase))).FirstOrDefault<TestPlansLibraryWorkItemFilter>();
        if (libraryWorkItemFilter != null)
          currentFilter = this.ConcatenateFilters(currentFilter, " AND " + wiqlColumnPrefix + WorkItemFieldRefNames.Title + " CONTAINS WORDS '" + libraryWorkItemFilter.FieldValues[0] + "'");
      }
      if (filterFieldsToConsider.Contains(WorkItemFieldRefNames.State))
      {
        TestPlansLibraryWorkItemFilter filter = filterValues.Where<TestPlansLibraryWorkItemFilter>((Func<TestPlansLibraryWorkItemFilter, bool>) (f => f.FieldName.Equals(WorkItemFieldRefNames.State, StringComparison.OrdinalIgnoreCase))).FirstOrDefault<TestPlansLibraryWorkItemFilter>();
        if (filter != null)
        {
          string filterToAdd = this.BuildWorkItemFilterQuery(filter, WorkItemFieldRefNames.State, wiqlColumnPrefix);
          currentFilter = this.ConcatenateFilters(currentFilter, filterToAdd);
        }
      }
      if (filterFieldsToConsider.Contains(WorkItemFieldRefNames.AssignedTo))
      {
        TestPlansLibraryWorkItemFilter filter = filterValues.Where<TestPlansLibraryWorkItemFilter>((Func<TestPlansLibraryWorkItemFilter, bool>) (f => f.FieldName.Equals(WorkItemFieldRefNames.AssignedTo, StringComparison.OrdinalIgnoreCase))).FirstOrDefault<TestPlansLibraryWorkItemFilter>();
        if (filter != null)
        {
          string filterToAdd = this.BuildWorkItemFilterQuery(filter, WorkItemFieldRefNames.AssignedTo, wiqlColumnPrefix);
          currentFilter = this.ConcatenateFilters(currentFilter, filterToAdd);
        }
      }
      if (filterFieldsToConsider.Contains(WorkItemFieldRefNames.Priority))
      {
        TestPlansLibraryWorkItemFilter filter = filterValues.Where<TestPlansLibraryWorkItemFilter>((Func<TestPlansLibraryWorkItemFilter, bool>) (f => f.FieldName.Equals(WorkItemFieldRefNames.Priority, StringComparison.OrdinalIgnoreCase))).FirstOrDefault<TestPlansLibraryWorkItemFilter>();
        if (filter != null)
        {
          string filterToAdd = this.BuildWorkItemFilterQuery(filter, WorkItemFieldRefNames.Priority, wiqlColumnPrefix, true);
          currentFilter = this.ConcatenateFilters(currentFilter, filterToAdd);
        }
      }
      if (filterFieldsToConsider.Contains(WorkItemFieldRefNames.AreaPath))
      {
        TestPlansLibraryWorkItemFilter filter = filterValues.Where<TestPlansLibraryWorkItemFilter>((Func<TestPlansLibraryWorkItemFilter, bool>) (f => f.FieldName.Equals(WorkItemFieldRefNames.AreaPath, StringComparison.OrdinalIgnoreCase))).FirstOrDefault<TestPlansLibraryWorkItemFilter>();
        if (filter != null)
        {
          string filterToAdd = this.BuildWorkItemFilterQuery(filter, WorkItemFieldRefNames.AreaPath, wiqlColumnPrefix);
          currentFilter = this.ConcatenateFilters(currentFilter, filterToAdd);
        }
      }
      return currentFilter;
    }

    protected string BuildWorkItemFilterQuery(
      TestPlansLibraryWorkItemFilter filter,
      string workItemFieldRefName,
      string wiqlColumnPrefix = "",
      bool isIntegerFilter = false)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(" AND ");
      List<string> fieldValues = filter.FieldValues;
      stringBuilder.Append("(");
      for (int index = 0; index < fieldValues.Count; ++index)
      {
        if (index > 0)
          stringBuilder.Append(filter.FilterMode == TestPlansLibraryWorkItemFilterMode.Or ? " OR" : " AND");
        if (!isIntegerFilter)
          stringBuilder.Append(" " + wiqlColumnPrefix + workItemFieldRefName + " = '" + fieldValues[index] + "'");
        else
          stringBuilder.Append(" " + wiqlColumnPrefix + workItemFieldRefName + " = " + fieldValues[index]);
      }
      stringBuilder.Append(" )");
      return stringBuilder.ToString();
    }

    protected virtual LibraryWorkItemsDataProvider.WorkItemQueryAndType AppendFilters(
      LibraryWorkItemsDataProvider.WorkItemQueryAndType query,
      List<TestPlansLibraryWorkItemFilter> filterValues)
    {
      throw new NotImplementedException("AppendFilters not implemented");
    }

    private string ConcatenateFilters(string currentFilter, string filterToAdd) => string.Format("{0} {1}", (object) currentFilter, (object) filterToAdd);

    private IList<int> GetWorkItemIds(
      IVssRequestContext requestContext,
      LibraryWorkItemsDataProvider.WorkItemQueryAndType queryAndType,
      Hashtable workItemContext,
      int pageSize,
      out bool hasMoreResults)
    {
      IList<int> workItemIds = (IList<int>) null;
      hasMoreResults = false;
      using (PerformanceTimer.StartMeasure(requestContext, "LibraryWorkItemsDataProvider.ExecuteWiqlQuery", Enum.GetName(typeof (TestPlansLibraryQuery), (object) queryAndType.QueryType)))
      {
        requestContext.Trace(1015694, TraceLevel.Info, "TestManagement", "WebService", "Issuing wiql query: " + queryAndType.Query);
        IWorkItemQueryService service = requestContext.GetService<IWorkItemQueryService>();
        IVssRequestContext requestContext1 = requestContext;
        string query = queryAndType.Query;
        Hashtable context = workItemContext;
        int num = pageSize;
        Guid? projectId = new Guid?();
        int topCount = num;
        QueryResult queryResult = service.ExecuteQuery(requestContext1, query, (IDictionary) context, projectId, topCount);
        if (queryResult == null)
          requestContext.Trace(1015694, TraceLevel.Warning, "TestManagement", "WebService", "queryResult is null");
        else if (queryResult.ResultType == QueryResultType.WorkItem)
        {
          if (queryResult.WorkItemIds == null)
            requestContext.Trace(1015694, TraceLevel.Warning, "TestManagement", "WebService", "queryResult.WorkItemIds is null");
          else
            workItemIds = (IList<int>) queryResult.WorkItemIds.ToList<int>();
        }
        else if (queryResult.ResultType == QueryResultType.WorkItemLink)
        {
          if (queryResult.WorkItemLinks == null)
            requestContext.Trace(1015694, TraceLevel.Warning, "TestManagement", "WebService", "queryResult.WorkItemLinks is null");
          else
            workItemIds = (IList<int>) queryResult.WorkItemLinks.Select<LinkQueryResultEntry, int>((Func<LinkQueryResultEntry, int>) (x => x.SourceId)).Where<int>((Func<int, bool>) (x => x > 0)).Distinct<int>().ToList<int>();
        }
        if (queryResult.HasMoreResult)
          hasMoreResults = true;
      }
      return workItemIds;
    }

    private List<string> GetColumnOptions(
      IVssRequestContext requestContext,
      Guid projectId,
      LibraryWorkItemsDataProviderRequest requestData)
    {
      List<string> stringList = new List<string>();
      List<string> columnOptions;
      if (requestData.ColumnOptions == null)
      {
        string str = requestContext.GetService<ISettingsService>().GetValue<string>(requestContext, SettingsUserScope.User, "Project", projectId.ToString(), LibraryWorkItemsDataProvider.c_TestCasesPivotColumnOptions, "", false);
        if (!string.IsNullOrEmpty(str))
          columnOptions = ((IEnumerable<string>) str.Split(',')).ToList<string>();
        else
          columnOptions = this.m_defaultColumnsList;
      }
      else
        columnOptions = ((IEnumerable<string>) requestData.ColumnOptions).ToList<string>();
      return columnOptions;
    }

    protected class WorkItemQueryAndType
    {
      public TestPlansLibraryQuery QueryType { get; set; }

      public string Query { get; set; }

      public QueryType WitQueryType { get; set; }
    }
  }
}
