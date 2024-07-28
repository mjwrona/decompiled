// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardsQueryWITDataSource
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.Azure.Boards.Settings;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.Query;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.Search;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Utilities;
using Microsoft.TeamFoundation.WebPlatform.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Search;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Identity;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Favorites.Server;
using Microsoft.VisualStudio.Services.Favorites.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public class BoardsQueryWITDataSource
  {
    private static readonly string WiovWhereClause = "[System.TeamProject] = @project and [System.CreatedBy] = @me";
    private const string AtMe = "@me";
    private const string AtProject = "@project";
    private static readonly string RecentSearchesPath = "/Recent/WorkItemsSearch";

    public static QueryResultPayload PageWorkItems(
      IVssRequestContext tfsRequestContext,
      string traceArea,
      string workItemIds,
      string fields,
      DateTime? asOf,
      bool? omitHeaders,
      bool isDeleted = false)
    {
      ArgumentUtility.CheckForNull<string>(workItemIds, nameof (workItemIds));
      int[] intArray = BoardsQueryWITDataSource.IntStringToIntArray(workItemIds, nameof (workItemIds));
      ArgumentUtility.CheckForOutOfRange(intArray.Length, "workItemIds.count()", 1, 200);
      ArgumentUtility.CheckForNull<string>(fields, nameof (fields));
      string[] strArray = fields.Split(new char[1]{ ',' }, StringSplitOptions.None);
      BoardsQueryWITDataSource.ValidateFieldNames(tfsRequestContext, (IEnumerable<string>) strArray, nameof (fields), false);
      tfsRequestContext.Trace(516165, TraceLevel.Info, traceArea, nameof (PageWorkItems), "PageWorkItems: workItemIds: [{0}] fields: [{1}] asOf: [{2}] omitHeaders: [{3}]", (object) workItemIds, (object) fields, (object) asOf.GetValueOrDefault(), (object) omitHeaders.GetValueOrDefault());
      GenericDataReader dataReader = tfsRequestContext.GetService<WebAccessWorkItemService>().PageWorkItems(tfsRequestContext, (IEnumerable<int>) intArray, (IEnumerable<string>) strArray, asOf, isDeleted);
      return new QueryResultPayload(tfsRequestContext, dataReader, omitHeaders.GetValueOrDefault(false));
    }

    public static QueryResultPayload PageWorkItemsByIdRev(
      IVssRequestContext tfsRequestContext,
      string workItemIds,
      string workItemRevisions,
      string fields,
      bool isDeleted = false)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(fields, nameof (fields));
      ArgumentUtility.CheckStringForNullOrEmpty(workItemIds, nameof (workItemIds));
      ArgumentUtility.CheckStringForNullOrEmpty(workItemRevisions, nameof (workItemRevisions));
      int[] intArray1 = BoardsQueryWITDataSource.IntStringToIntArray(workItemIds, nameof (workItemIds));
      int[] intArray2 = BoardsQueryWITDataSource.IntStringToIntArray(workItemRevisions, nameof (workItemRevisions));
      string[] strArray = fields.Split(new char[1]{ ',' }, StringSplitOptions.None);
      if (intArray1.Length != intArray2.Length)
        throw new ArgumentException(Resources.ErrorIdsAndRevsLengthMustMatch);
      BoardsQueryWITDataSource.ValidateFieldNames(tfsRequestContext, (IEnumerable<string>) strArray, nameof (fields), false);
      GenericDataReader dataReader = tfsRequestContext.GetService<WebAccessWorkItemService>().PageWorkItems(tfsRequestContext, intArray1, intArray2, (IEnumerable<string>) strArray, isDeleted ? WorkItemRetrievalMode.Deleted : WorkItemRetrievalMode.NonDeleted);
      return new QueryResultPayload(tfsRequestContext, dataReader, false);
    }

    public static AdHocQueriesResult AdHocQueries(
      IVssRequestContext tfsRequestContext,
      Guid currentProjectGuid)
    {
      using (WebUserSettingsHive hive = new WebUserSettingsHive(tfsRequestContext))
        return new AdHocQueriesResult()
        {
          RecycleBin = AdhocQueryProvider.GetRecycleBinQuery(tfsRequestContext, currentProjectGuid, hive).ToJson(true)
        };
    }

    public static JsObject UpdateAdHocQuery(
      IVssRequestContext tfsRequestContext,
      Guid currentProjectGuid,
      string traceArea,
      Guid queryId,
      string wiql)
    {
      string str = wiql;
      ArgumentUtility.CheckForEmptyGuid(queryId, nameof (queryId));
      if (!((IEnumerable<Guid>) AdhocQueryProvider.QueryIds).Contains<Guid>(queryId))
        throw new ArgumentException(Resources.QueryIdNotAdhocQuery, nameof (queryId));
      try
      {
        WiqlHelper.ParseSyntax(wiql);
        wiql = WiqlTransformUtils.TransformNamesToIds(tfsRequestContext, wiql, false);
      }
      catch (Exception ex)
      {
        tfsRequestContext.TraceException(516482, traceArea, nameof (UpdateAdHocQuery), ex);
        throw;
      }
      try
      {
        AdhocQueryProvider.SaveAdhocQuery(tfsRequestContext, currentProjectGuid, queryId, wiql);
      }
      catch (Exception ex)
      {
        tfsRequestContext.TraceException(516481, traceArea, nameof (UpdateAdHocQuery), ex);
        throw;
      }
      JsObject jsObject = new JsObject();
      jsObject["id"] = (object) queryId;
      jsObject[nameof (wiql)] = (object) str;
      return jsObject;
    }

    public static QueryResultModel Search(
      IVssRequestContext tfsRequestContext,
      string traceArea,
      Guid currentProjectGuid,
      ProjectInfo project,
      WebApiTeam team,
      string searchText)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(searchText, nameof (searchText), tfsRequestContext.ServiceName);
      SearchWiqlBuilder searchWiqlBuilder = new SearchWiqlBuilder((IWiqlAdapterHelper) new ServerWiqlAdapterHelper(tfsRequestContext), (ISearchTokenHelper) new SearchTokenHelper(SearchQueryParser.ParseSearchString(searchText).ToArray<SearchToken>()), project.Name);
      bool skipReproStepsFieldForSearch = !tfsRequestContext.ExecutionEnvironment.IsHostedDeployment && !BoardsQueryWITDataSource.DoesReproStepsExistForCurrentProject(tfsRequestContext, currentProjectGuid);
      string orderWiql = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ORDER BY [{0}] DESC", (object) CoreFieldReferenceNames.ChangedDate);
      string wiql;
      try
      {
        wiql = searchWiqlBuilder.BuildWiql(orderWiql, includeTags: true, skipReproStepsFieldForSearch: skipReproStepsFieldForSearch);
      }
      catch (TeamFoundationServerException ex)
      {
        throw new TeamFoundationServiceException(ex.Message, (Exception) ex);
      }
      tfsRequestContext.Trace(516280, TraceLevel.Info, traceArea, nameof (Search), "WebAccess.WorkItem", (object) TfsTraceLayers.Controller, (object) "Search wiql");
      tfsRequestContext.RequestTimeout = TimeSpan.FromMinutes(60.0);
      using (WebUserSettingsHive userSettingsHive = new WebUserSettingsHive(tfsRequestContext))
      {
        string str1 = userSettingsHive.ReadSetting<string>(BoardsQueryWITDataSource.RecentSearchesPath, (string) null);
        string str2 = ((IEnumerable<string>) new string[1]
        {
          searchText
        }).Concat<string>(str1.Split<string>(';')).Distinct<string>().Take<string>(5).StringJoin<string>(';');
        userSettingsHive.WriteValue(BoardsQueryWITDataSource.RecentSearchesPath, str2);
      }
      return BoardsQueryWITDataSource.QueryJsonFormat(tfsRequestContext, traceArea, currentProjectGuid, project, team, true, wiql, (IEnumerable<string>) null, (IEnumerable<string>) null, new DateTime?(), new bool?(), new bool?(), new bool?(true), new Guid?(), new int?());
    }

    public static UpdateColumnOptionsResult UpdateColumnOptions(
      IVssRequestContext tfsRequestContext,
      Guid currentProjectGuid,
      Guid? persistenceId,
      IEnumerable<string> fields)
    {
      ArgumentUtility.CheckForNull<Guid>(persistenceId, nameof (persistenceId));
      ArgumentUtility.CheckForEmptyGuid(persistenceId.Value, nameof (persistenceId));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) fields, nameof (fields));
      IEnumerable<KeyValuePair<string, int>> columns = BoardsQueryWITDataSource.ParseColumns(fields);
      BoardsQueryWITDataSource.ValidateFieldNames(tfsRequestContext, columns.Select<KeyValuePair<string, int>, string>((Func<KeyValuePair<string, int>, string>) (pair => pair.Key)), nameof (fields), false);
      BoardsQueryWITDataSource.PersistColumnSizes(tfsRequestContext, currentProjectGuid, persistenceId.Value, columns);
      return new UpdateColumnOptionsResult()
      {
        Success = true
      };
    }

    public static HtmlQueryResult QueryHtmlFormat(
      IVssRequestContext tfsRequestContext,
      string traceArea,
      Guid currentProjectGuid,
      ProjectInfo project,
      WebApiTeam team,
      bool areStandardFeaturesAvailable,
      string wiql,
      IEnumerable<string> fields,
      IEnumerable<string> sortFields,
      DateTime? asOf,
      bool? runQuery,
      bool? includePayload,
      bool? includeEditInfo,
      Guid? persistenceId,
      int? top,
      IEnumerable<int> workItemIdFilter = null,
      bool isDirty = false,
      bool useIsoDateFormat = false)
    {
      QueryResultModel queryResultModel1 = BoardsQueryWITDataSource.ExecuteQuery(tfsRequestContext, traceArea, currentProjectGuid, project, team, (areStandardFeaturesAvailable ? 1 : 0) != 0, wiql, fields, sortFields, asOf, runQuery, includePayload, includeEditInfo, persistenceId, top, workItemIdFilter, (IEnumerable<string>) new string[1]
      {
        CoreFieldReferenceNames.Title
      }, (isDirty ? 1 : 0) != 0, (useIsoDateFormat ? 1 : 0) != 0);
      if (runQuery.HasValue && !runQuery.Value)
        tfsRequestContext.Trace(516470, TraceLevel.Warning, traceArea, nameof (QueryHtmlFormat), "Requesting html output but 'runQuery' is explicitly false");
      if (includePayload.HasValue && !includePayload.Value)
        tfsRequestContext.Trace(516470, TraceLevel.Warning, traceArea, nameof (QueryHtmlFormat), "Requesting html output but 'includePayload' is explicitly false");
      QueryResultHtmlFormatter resultHtmlFormatter = new QueryResultHtmlFormatter(tfsRequestContext);
      bool flag = workItemIdFilter != null && workItemIdFilter.Any<int>();
      QueryResultModel queryResultModel2 = queryResultModel1;
      Guid? queryId = persistenceId;
      int num = flag ? 1 : 0;
      string htmlForQueryResult = resultHtmlFormatter.GenerateHtmlForQueryResult(queryResultModel2, queryId, num != 0);
      return new HtmlQueryResult()
      {
        WorkItemCount = queryResultModel1.TargetIds.Count<int>(),
        MaxWorkItemCount = 200,
        Html = htmlForQueryResult
      };
    }

    public static QueryResultModel QueryJsonFormat(
      IVssRequestContext tfsRequestContext,
      string traceArea,
      Guid currentProjectGuid,
      ProjectInfo project,
      WebApiTeam team,
      bool areStandardFeaturesAvailable,
      string wiql,
      IEnumerable<string> fields,
      IEnumerable<string> sortFields,
      DateTime? asOf,
      bool? runQuery,
      bool? includePayload,
      bool? includeEditInfo,
      Guid? persistenceId,
      int? top,
      IEnumerable<int> workItemIdFilter = null,
      bool isDirty = false,
      bool useIsoDateFormat = false)
    {
      QueryResultModel queryResultModel = BoardsQueryWITDataSource.ExecuteQuery(tfsRequestContext, traceArea, currentProjectGuid, project, team, areStandardFeaturesAvailable, wiql, fields, sortFields, asOf, runQuery, includePayload, includeEditInfo, persistenceId, top, workItemIdFilter, isDirty: isDirty, useIsoDateFormat: useIsoDateFormat);
      if (queryResultModel != null && queryResultModel.IsLinkQuery)
      {
        string message = queryResultModel.Payload != null ? (queryResultModel.Payload.Rows != null ? "Wiql: " + wiql + ". First payload row: " + queryResultModel.Payload.Rows.FirstOrDefault<object[]>()?.ToString() : "Wiql: " + wiql + ". Payload.Rows is null") : "Wiql: " + wiql + ". Payload is null";
        tfsRequestContext.Trace(516491, TraceLevel.Info, traceArea, nameof (QueryJsonFormat), message);
      }
      return queryResultModel;
    }

    internal static QueryResultModel ExecuteQuery(
      IVssRequestContext tfsRequestContext,
      string traceArea,
      Guid currentProjectGuid,
      ProjectInfo project,
      WebApiTeam team,
      bool areStandardFeaturesAvailable,
      string wiql,
      IEnumerable<string> fields,
      IEnumerable<string> sortFields,
      DateTime? asOf,
      bool? runQuery,
      bool? includePayload,
      bool? includeEditInfo,
      Guid? persistenceId,
      int? top,
      IEnumerable<int> workItemIdFilter,
      IEnumerable<string> requiredColumns = null,
      bool isDirty = false,
      bool useIsoDateFormat = false)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(wiql, nameof (wiql), tfsRequestContext.ServiceName);
      try
      {
        WiqlHelper.ParseSyntax(wiql);
      }
      catch (SyntaxException ex)
      {
        throw new ArgumentException(ex.Details, nameof (wiql), (Exception) ex).Expected(tfsRequestContext.ServiceName);
      }
      IEnumerable<KeyValuePair<string, int>> array;
      IEnumerable<KeyValuePair<string, int>> source;
      try
      {
        array = (IEnumerable<KeyValuePair<string, int>>) BoardsQueryWITDataSource.ParseColumns(fields).ToArray<KeyValuePair<string, int>>();
        source = array;
      }
      catch (Exception ex)
      {
        throw new ArgumentException(nameof (fields), ex).Expected(tfsRequestContext.ServiceName);
      }
      IEnumerable<QuerySortOrderEntry> querySortOrderEntries;
      try
      {
        querySortOrderEntries = (IEnumerable<QuerySortOrderEntry>) BoardsQueryWITDataSource.ParseSortColumns(sortFields).ToArray<QuerySortOrderEntry>();
      }
      catch (Exception ex)
      {
        throw new ArgumentException(nameof (sortFields), ex);
      }
      if (array.Any<KeyValuePair<string, int>>())
        BoardsQueryWITDataSource.ValidateFieldNames(tfsRequestContext, array.Select<KeyValuePair<string, int>, string>((Func<KeyValuePair<string, int>, string>) (pair => pair.Key)), nameof (fields), false);
      if (querySortOrderEntries.Any<QuerySortOrderEntry>())
        BoardsQueryWITDataSource.ValidateFieldNames(tfsRequestContext, querySortOrderEntries.Select<QuerySortOrderEntry, string>((Func<QuerySortOrderEntry, string>) (qse => qse.ColumnName)), nameof (sortFields), false);
      if (!includeEditInfo.HasValue)
        includeEditInfo = new bool?(true);
      if (!includePayload.HasValue)
        includePayload = new bool?(true);
      if (!runQuery.HasValue)
        runQuery = new bool?(true);
      if (persistenceId.HasValue && persistenceId.Value != Guid.Empty)
      {
        if (array.Any<KeyValuePair<string, int>>())
          BoardsQueryWITDataSource.PersistColumnSizes(tfsRequestContext, currentProjectGuid, persistenceId.Value, array);
        else
          source = (IEnumerable<KeyValuePair<string, int>>) BoardsQueryWITDataSource.GetPersistedColumnSizes(tfsRequestContext, currentProjectGuid, persistenceId.Value).ToArray<KeyValuePair<string, int>>();
        if (persistenceId.Value == AdhocQueryProvider.RecycleBin)
          top = new int?(tfsRequestContext.WitContext().ServerSettings.MaxQueryResultSize - 1);
      }
      string name1 = project.Name;
      try
      {
        Hashtable context = new Hashtable((IEqualityComparer) TFStringComparer.WorkItemQueryText);
        ServerDefaultValueTransformer valueTransformer = new ServerDefaultValueTransformer(tfsRequestContext);
        context[(object) "me"] = (object) valueTransformer.CurrentUser;
        context[(object) nameof (project)] = (object) name1;
        if (team != null)
        {
          context[(object) nameof (team)] = (object) team.Name;
        }
        else
        {
          WebApiTeam defaultTeam = tfsRequestContext.GetService<ITeamService>().GetDefaultTeam(tfsRequestContext, project.Id);
          if (defaultTeam != null)
            context[(object) nameof (team)] = (object) defaultTeam.Name;
        }
        context[(object) "me"] = (object) tfsRequestContext.GetUserIdentity().GetLegacyDistinctDisplayName();
        string wiql1 = wiql;
        IWorkItemQueryService service = tfsRequestContext.GetService<IWorkItemQueryService>();
        Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression queryExpression1 = service.ConvertToQueryExpression(tfsRequestContext, wiql, (IDictionary) context, collectMacro: true);
        Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression editorQueryInfo = queryExpression1;
        Dictionary<string, int> macrosUsed = queryExpression1.MacrosUsed;
        fields = array.Any<KeyValuePair<string, int>>() ? array.Select<KeyValuePair<string, int>, string>((Func<KeyValuePair<string, int>, string>) (pair => pair.Key)) : queryExpression1.DisplayFields;
        if (!querySortOrderEntries.Any<QuerySortOrderEntry>())
          querySortOrderEntries = queryExpression1.SortFields.Where<QuerySortField>((Func<QuerySortField, bool>) (sf => sf.TableAlias != QueryTableAlias.Right)).Select<QuerySortField, QuerySortOrderEntry>((Func<QuerySortField, QuerySortOrderEntry>) (sf => new QuerySortOrderEntry()
          {
            Ascending = !sf.Descending,
            ColumnName = sf.Field.Name
          }));
        if (asOf.HasValue)
        {
          DateTime dateTime = asOf.Value;
          DateTime? asOfDateTime = queryExpression1.AsOfDateTime;
          if ((asOfDateTime.HasValue ? (dateTime != asOfDateTime.GetValueOrDefault() ? 1 : 0) : 1) != 0)
            goto label_34;
        }
        if (fields.SequenceEqual<string>(queryExpression1.DisplayFields, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) && querySortOrderEntries.SequenceEqual<QuerySortOrderEntry>(queryExpression1.SortFields.Select<QuerySortField, QuerySortOrderEntry>((Func<QuerySortField, QuerySortOrderEntry>) (sf => new QuerySortOrderEntry()
        {
          Ascending = !sf.Descending,
          ColumnName = sf.Field.Name
        }))))
          goto label_35;
label_34:
        wiql1 = WiqlHelper.GenerateWiql(wiql, fields, querySortOrderEntries, asOf);
        queryExpression1 = service.ConvertToQueryExpression(tfsRequestContext, wiql1, (IDictionary) context);
        queryExpression1.MacrosUsed = macrosUsed;
        editorQueryInfo = queryExpression1;
label_35:
        if (!areStandardFeaturesAvailable)
        {
          wiql1 = WiqlHelper.GenerateWiql(wiql, fields, querySortOrderEntries, asOf, BoardsQueryWITDataSource.WiovWhereClause);
          queryExpression1 = service.ConvertToQueryExpression(tfsRequestContext, wiql1, (IDictionary) context);
          queryExpression1.MacrosUsed = macrosUsed;
          if (queryExpression1.MacrosUsed == null)
            queryExpression1.MacrosUsed = new Dictionary<string, int>();
          queryExpression1.MacrosUsed.AddOrUpdate<string, int>("@me", 1, (Func<string, int, int>) ((name, count) => count + 1));
          queryExpression1.MacrosUsed.AddOrUpdate<string, int>("@project", 1, (Func<string, int, int>) ((name, count) => count + 1));
        }
        Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression queryExpression2 = queryExpression1;
        Guid? nullable1;
        Guid? nullable2;
        if (!isDirty || !tfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
        {
          if (persistenceId.HasValue)
          {
            nullable1 = persistenceId;
            Guid customWiqlQuery = AdhocQueryProvider.CustomWiqlQuery;
            if ((nullable1.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() == customWiqlQuery ? 1 : 0) : 1) : 0) == 0)
            {
              nullable1 = persistenceId;
              Guid searchResults = AdhocQueryProvider.SearchResults;
              if ((nullable1.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() == searchResults ? 1 : 0) : 1) : 0) == 0)
                goto label_43;
            }
            nullable1 = new Guid?();
            nullable2 = nullable1;
            goto label_46;
          }
label_43:
          nullable2 = persistenceId;
        }
        else
          nullable2 = new Guid?(Guid.Empty);
label_46:
        queryExpression2.QueryId = nullable2;
        Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression queryExpression3 = queryExpression1;
        int num;
        if (!tfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
        {
          if (!isDirty && persistenceId.HasValue)
          {
            nullable1 = persistenceId;
            Guid empty = Guid.Empty;
            num = nullable1.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() != empty ? 1 : 0) : 0) : 1;
          }
          else
            num = 0;
        }
        else
          num = 1;
        queryExpression3.IsTrackingNeeded = num != 0;
        if (!top.HasValue)
          top = new int?(tfsRequestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(tfsRequestContext).DefaultWebAccessQueryResultSize);
        else if (top.Value < 0)
          top = new int?(0);
        Dictionary<string, int> dictionary = source.GroupBy<KeyValuePair<string, int>, string>((Func<KeyValuePair<string, int>, string>) (pair => pair.Key), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).Select<IGrouping<string, KeyValuePair<string, int>>, KeyValuePair<string, int>>((Func<IGrouping<string, KeyValuePair<string, int>>, KeyValuePair<string, int>>) (g => g.First<KeyValuePair<string, int>>())).ToDictionary<KeyValuePair<string, int>, string, int>((Func<KeyValuePair<string, int>, string>) (pair => pair.Key), (Func<KeyValuePair<string, int>, int>) (pair => pair.Value));
        return new QueryResultModel(tfsRequestContext, wiql1, queryExpression1, editorQueryInfo, (IDictionary<string, int>) dictionary, includePayload.Value, includeEditInfo.Value, name1, runQuery.Value, workItemIdFilter, requiredColumns, top.Value, useIsoDateFormat);
      }
      catch (Exception ex)
      {
        tfsRequestContext.TraceException(516412, traceArea, nameof (ExecuteQuery), ex);
        if (includeEditInfo.Value)
        {
          QueryEditorModel editorModel = new QueryEditorModel(tfsRequestContext, wiql, true, (IDictionary<string, int>) source.ToDedupedDictionary<KeyValuePair<string, int>, string, int>((Func<KeyValuePair<string, int>, string>) (pair => pair.Key), (Func<KeyValuePair<string, int>, int>) (pair => pair.Value), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase), name1);
          return new QueryResultModel(tfsRequestContext, editorModel, ex);
        }
        throw;
      }
    }

    private static int[] IntStringToIntArray(string intString, string argumentName) => ((IEnumerable<string>) intString.Split(new char[1]
    {
      ','
    }, StringSplitOptions.None)).Select<string, int>((Func<string, int>) (idString =>
    {
      int result;
      if (int.TryParse(idString, out result))
        return result;
      throw new InvalidArgumentValueException(argumentName);
    })).ToArray<int>();

    private static IEnumerable<KeyValuePair<string, int>> GetPersistedColumnSizes(
      IVssRequestContext tfsRequestContext,
      Guid currentProjectGuid,
      Guid persistenceId)
    {
      using (WebUserSettingsHive hive = new WebUserSettingsHive(tfsRequestContext))
      {
        string persistedColumnSizes = QueryColumnHelper.GetPersistedColumnSizes(hive, persistenceId, currentProjectGuid);
        IEnumerable<string> fields = (IEnumerable<string>) null;
        if (!string.IsNullOrEmpty(persistedColumnSizes))
          fields = (IEnumerable<string>) persistedColumnSizes.Split(new char[1]
          {
            '&'
          }, StringSplitOptions.RemoveEmptyEntries);
        return BoardsQueryWITDataSource.ParseColumns(fields);
      }
    }

    private static IEnumerable<KeyValuePair<string, int>> ParseColumns(IEnumerable<string> fields)
    {
      if (fields != null)
      {
        foreach (string field in fields)
        {
          string key = field;
          int result = -1;
          if (!string.IsNullOrWhiteSpace(field))
          {
            string[] strArray = field.Split(';');
            key = strArray[0].Trim();
            if (strArray.Length > 1 && !int.TryParse(strArray[1], NumberStyles.Any, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result))
              result = -1;
            if (result == -1)
              result = QueryResultModel.GetDefaultColumnWidth(int.MaxValue, InternalFieldType.String);
          }
          yield return new KeyValuePair<string, int>(key, result);
        }
      }
    }

    private static IEnumerable<QuerySortOrderEntry> ParseSortColumns(IEnumerable<string> sortFields)
    {
      if (sortFields != null)
      {
        foreach (string sortField in sortFields)
        {
          string str = sortField;
          bool flag = true;
          if (!string.IsNullOrWhiteSpace(sortField))
          {
            string[] strArray = sortField.Split(';');
            str = strArray[0].Trim();
            if (strArray.Length > 1)
              flag = !StringComparer.OrdinalIgnoreCase.Equals(strArray[1], "desc");
          }
          yield return new QuerySortOrderEntry()
          {
            ColumnName = str,
            Ascending = flag
          };
        }
      }
    }

    private static void PersistColumnSizes(
      IVssRequestContext tfsRequestContext,
      Guid currentProjectGuid,
      Guid persistenceId,
      IEnumerable<KeyValuePair<string, int>> columns)
    {
      using (WebUserSettingsHive hive = new WebUserSettingsHive(tfsRequestContext))
        QueryColumnHelper.PersistColumnSizes(hive, persistenceId, currentProjectGuid, string.Join("&", columns.Select<KeyValuePair<string, int>, string>((Func<KeyValuePair<string, int>, string>) (pair => pair.Key + ";" + pair.Value.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)))));
    }

    private static void ValidateFieldNames(
      IVssRequestContext tfsRequestContext,
      IEnumerable<string> fieldNames,
      string paramName,
      bool throwOnEmptyFieldNames)
    {
      if (throwOnEmptyFieldNames)
        ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) fieldNames, paramName);
      if (fieldNames == null)
        return;
      WorkItemTrackingFieldService witFieldService = tfsRequestContext.GetService<WorkItemTrackingFieldService>();
      CommonUtility.CheckEnumerableElements<string>(fieldNames, paramName, (Action<string, string>) ((fieldName, indexedParamName) =>
      {
        ArgumentUtility.CheckStringForNullOrEmpty(fieldName, indexedParamName);
        if (!witFieldService.TryGetField(tfsRequestContext, fieldName, out FieldEntry _))
          throw new ArgumentException("Unknown field '{FormatCurrent(fieldName)}.", indexedParamName);
      }));
    }

    private static bool DoesReproStepsExistForCurrentProject(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return requestContext.GetService<WebAccessWorkItemService>().GetWorkItemTypes(requestContext, projectId).SelectMany<IWorkItemType, FieldDefinition>((Func<IWorkItemType, IEnumerable<FieldDefinition>>) (wit => (IEnumerable<FieldDefinition>) wit.GetFields(requestContext))).Any<FieldDefinition>((Func<FieldDefinition, bool>) (f => TFStringComparer.WorkItemFieldReferenceName.Equals(f.ReferenceName, "Microsoft.VSTS.TCM.ReproSteps")));
    }

    public static object QueryFavorites(IVssRequestContext requestContext, Guid teamId)
    {
      IFavoriteService service1 = requestContext.GetService<IFavoriteService>();
      FavoriteFilter filter = new FavoriteFilter()
      {
        Type = "Microsoft.TeamFoundation.WorkItemTracking.QueryItem",
        ArtifactScope = new ArtifactScope("Project", requestContext.GetService<IRequestProjectService>().GetProject(requestContext)?.Id.ToString())
      };
      IEnumerable<Favorite> source1 = service1.GetFavorites(requestContext, filter, false, (OwnerScope) null).Where<Favorite>((Func<Favorite, bool>) (item => !item.ArtifactIsDeleted));
      IEnumerable<Favorite> source2 = Enumerable.Empty<Favorite>();
      if (teamId != Guid.Empty)
        source2 = service1.GetFavorites(requestContext, filter, false, OwnerScope.Team(teamId)).Where<Favorite>((Func<Favorite, bool>) (item => !item.ArtifactIsDeleted));
      IEnumerable<Guid> guids = source2.Select<Favorite, Guid>((Func<Favorite, Guid>) (x => new Guid(x.ArtifactId))).Union<Guid>(source1.Select<Favorite, Guid>((Func<Favorite, Guid>) (x => new Guid(x.ArtifactId)))).Distinct<Guid>();
      ITeamFoundationQueryItemService service2 = requestContext.GetService<ITeamFoundationQueryItemService>();
      IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem> source3 = Enumerable.Empty<Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem>();
      if (guids.Any<Guid>())
        source3 = service2.GetQueriesById(requestContext, guids, new int?(1), true);
      Dictionary<Guid, Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem> queryItemLookup = source3.ToDictionary<Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem, Guid>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem, Guid>) (x => x.Id));
      return (object) new
      {
        myFavorites = source1.Where<Favorite>((Func<Favorite, bool>) (x => queryItemLookup.ContainsKey(new Guid(x.ArtifactId)))).Select(x => new
        {
          id = x.Id,
          queryItem = BoardsQueryWITDataSource.CreateQueryItem(queryItemLookup[new Guid(x.ArtifactId)])
        }),
        teamFavorites = source2.Where<Favorite>((Func<Favorite, bool>) (x => queryItemLookup.ContainsKey(new Guid(x.ArtifactId)))).Select(x => new
        {
          id = x.Id,
          queryItem = BoardsQueryWITDataSource.CreateQueryItem(queryItemLookup[new Guid(x.ArtifactId)])
        })
      };
    }

    private static object CreateQueryItem(Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem queryItem)
    {
      string str = (string) null;
      if (queryItem is Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.Query query)
        str = query.Wiql;
      return (object) new
      {
        id = queryItem.Id,
        name = queryItem.Name,
        wiql = str,
        isFolder = false,
        isPublic = queryItem.IsPublic,
        path = queryItem.Path
      };
    }
  }
}
