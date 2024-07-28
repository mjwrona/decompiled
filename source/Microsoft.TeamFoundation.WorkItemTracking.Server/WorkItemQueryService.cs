// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemQueryService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Identity;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class WorkItemQueryService : IWorkItemQueryService, IVssFrameworkService
  {
    private const string c_today = "today";
    public const string RecycleBinQueryWiql = "SELECT [System.Id], [System.WorkItemType], [System.Title], [System.AreaPath], [System.ChangedDate], [System.ChangedBy], [System.Tags] FROM WorkItems\r\n                                            WHERE [System.TeamProject] = @project AND [System.IsDeleted] = true\r\n                                            AND ([System.WorkItemType] NOT IN GROUP 'Microsoft.TestPlanCategory' AND [System.WorkItemType] NOT IN GROUP 'Microsoft.TestSuiteCategory'\r\n                                                 AND [System.WorkItemType] NOT IN GROUP 'Microsoft.TestCaseCategory' AND [System.WorkItemType] NOT IN GROUP 'Microsoft.SharedParameterCategory'\r\n                                                 AND [System.WorkItemType] NOT IN GROUP 'Microsoft.SharedStepCategory')\r\n                                                 ORDER BY [System.ChangedDate] DESC";
    private IQueryExecutionLogger m_executionLogger;
    private static readonly int[] s_operators = new int[56]
    {
      int.MinValue,
      int.MinValue,
      int.MinValue,
      int.MinValue,
      0,
      1,
      8,
      int.MinValue,
      1,
      0,
      int.MinValue,
      8,
      4,
      7,
      int.MinValue,
      int.MinValue,
      6,
      5,
      int.MinValue,
      int.MinValue,
      5,
      6,
      int.MinValue,
      int.MinValue,
      7,
      4,
      int.MinValue,
      int.MinValue,
      13,
      14,
      int.MinValue,
      int.MinValue,
      23,
      24,
      int.MinValue,
      int.MinValue,
      9,
      10,
      15,
      16,
      11,
      12,
      17,
      18,
      0,
      1,
      int.MinValue,
      int.MinValue,
      2,
      3,
      2,
      3,
      3,
      2,
      3,
      2
    };
    private static readonly HashSet<QueryExpressionOperator> s_validNamesReplacementOperators = new HashSet<QueryExpressionOperator>()
    {
      QueryExpressionOperator.Equals,
      QueryExpressionOperator.In,
      QueryExpressionOperator.Ever,
      QueryExpressionOperator.Under,
      QueryExpressionOperator.NotEquals,
      QueryExpressionOperator.NotIn,
      QueryExpressionOperator.NotUnder
    };
    private static readonly HashSet<QueryExpressionOperator> s_validNegationNamesReplacementOperators = new HashSet<QueryExpressionOperator>()
    {
      QueryExpressionOperator.NotEquals,
      QueryExpressionOperator.NotIn,
      QueryExpressionOperator.NotUnder
    };

    public void ServiceStart(IVssRequestContext systemRequestContext) => this.m_executionLogger = systemRequestContext.GetService<IQueryExecutionLogger>();

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public virtual QueryResult ExecuteQuery(
      IVssRequestContext requestContext,
      string wiql,
      IDictionary context = null,
      Guid? projectId = null,
      int topCount = 2147483647,
      WITQueryApplicationIntentOverride applicationIntentOverride = WITQueryApplicationIntentOverride.Default,
      bool skipWiqlTextLimitValidation = false,
      QuerySource querySource = QuerySource.Unknown)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<string>(wiql, nameof (wiql));
      IVssRequestContext requestContext1 = requestContext;
      string wiql1 = wiql;
      IDictionary context1 = context;
      int num = skipWiqlTextLimitValidation ? 1 : 0;
      Guid? nullable = projectId;
      Guid? queryId = new Guid?();
      Guid? filterProjectId = nullable;
      QueryExpression queryExpression = this._ConvertToQueryExpression(requestContext1, wiql1, context1, skipWiqlTextLimitValidation: num != 0, queryId: queryId, filterProjectId: filterProjectId, collectMacro: true);
      Guid guid;
      requestContext.Items.TryGetGuid("queryId", out guid);
      queryExpression.QueryId = new Guid?(guid);
      return this.ExecuteQuery(requestContext, queryExpression, projectId, topCount, applicationIntentOverride, querySource);
    }

    public virtual QueryResult ExecuteRecycleBinQuery(
      IVssRequestContext requestContext,
      Guid projectId,
      WITQueryApplicationIntentOverride applicationIntentOverride = WITQueryApplicationIntentOverride.Default)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      IVssRequestContext requestContext1 = requestContext;
      IDictionary systemQueryContext = this.GetSystemQueryContext(requestContext, projectId);
      Guid? nullable = new Guid?(projectId);
      Guid? queryId = new Guid?();
      Guid? filterProjectId = nullable;
      QueryExpression queryExpression = this._ConvertToQueryExpression(requestContext1, "SELECT [System.Id], [System.WorkItemType], [System.Title], [System.AreaPath], [System.ChangedDate], [System.ChangedBy], [System.Tags] FROM WorkItems\r\n                                            WHERE [System.TeamProject] = @project AND [System.IsDeleted] = true\r\n                                            AND ([System.WorkItemType] NOT IN GROUP 'Microsoft.TestPlanCategory' AND [System.WorkItemType] NOT IN GROUP 'Microsoft.TestSuiteCategory'\r\n                                                 AND [System.WorkItemType] NOT IN GROUP 'Microsoft.TestCaseCategory' AND [System.WorkItemType] NOT IN GROUP 'Microsoft.SharedParameterCategory'\r\n                                                 AND [System.WorkItemType] NOT IN GROUP 'Microsoft.SharedStepCategory')\r\n                                                 ORDER BY [System.ChangedDate] DESC", systemQueryContext, queryId: queryId, filterProjectId: filterProjectId);
      int topCount = CommonWITUtils.HasCrossProjectQueryPermission(requestContext) ? requestContext.WitContext().ServerSettings.MaxQueryResultSize : requestContext.WitContext().ServerSettings.MaxQueryResultSizeForPublicUser;
      return this.ExecuteQuery(requestContext, queryExpression, new Guid?(projectId), topCount, applicationIntentOverride, QuerySource.RecycleBin);
    }

    public virtual QueryResult ExecuteQuery(
      IVssRequestContext requestContext,
      QueryExpression query,
      Guid? projectId = null,
      int topCount = 2147483647,
      WITQueryApplicationIntentOverride applicationIntentOverride = WITQueryApplicationIntentOverride.Default,
      QuerySource querySource = QuerySource.Unknown)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<QueryExpression>(query, nameof (query));
      ArgumentUtility.CheckForOutOfRange(topCount, nameof (topCount), 0);
      return this.ExecuteQueryInternal(requestContext, query, projectId, topCount, applicationIntentOverride, querySource);
    }

    public QueryExecutionDetailsPayload GetQueryExecutionDetailsByQueryIdOrHash(
      IVssRequestContext requestContext,
      string queryIdOrHash)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(queryIdOrHash, nameof (queryIdOrHash));
      return requestContext.TraceBlock<QueryExecutionDetailsPayload>(906052, 906054, 906053, "Services", nameof (WorkItemQueryService), nameof (GetQueryExecutionDetailsByQueryIdOrHash), (Func<QueryExecutionDetailsPayload>) (() =>
      {
        QueryExecutionDetailsPayload detailsByQueryIdOrHash = (QueryExecutionDetailsPayload) null;
        using (QuerySqlComponent replicaAwareComponent = requestContext.CreateReadReplicaAwareComponent<QuerySqlComponent>(new WitReadReplicaContext?(new WitReadReplicaContext("WorkItemTracking.Server.ReadFromReadReplica"))))
        {
          QueryExecutionDetailRowModel executionDetailRowModel = (QueryExecutionDetailRowModel) null;
          if (replicaAwareComponent is QuerySqlComponent10 querySqlComponent10_2)
          {
            Guid result;
            if (Guid.TryParse(queryIdOrHash, out result))
            {
              ArgumentUtility.CheckForEmptyGuid(result, "queryId");
              executionDetailRowModel = querySqlComponent10_2.GetQueryExecutionDetailsByQueryId(result);
            }
            else
              executionDetailRowModel = querySqlComponent10_2.GetQueryExecutionDetailsByQueryHash(queryIdOrHash);
          }
          if (executionDetailRowModel == null)
            return detailsByQueryIdOrHash;
          return new QueryExecutionDetailsPayload()
          {
            QueryHash = executionDetailRowModel.QueryHash,
            WiqlText = executionDetailRowModel.WiqlText,
            LastRunTime = executionDetailRowModel.LastRunTime
          };
        }
      }));
    }

    private QueryResult ExecuteQueryInternal(
      IVssRequestContext requestContext,
      QueryExpression query,
      Guid? projectId = null,
      int topCount = 2147483647,
      WITQueryApplicationIntentOverride applicationIntentOverride = WITQueryApplicationIntentOverride.Default,
      QuerySource querySource = QuerySource.Unknown)
    {
      QueryResult queryResult = (QueryResult) null;
      QueryExecutionDetail detail = (QueryExecutionDetail) null;
      ApplicationIntent applicationIntent = ApplicationIntent.ReadWrite;
      QueryCategory queryCategory = QueryCategory.None;
      QueryOptimizationInstance optimizationInstance = (QueryOptimizationInstance) null;
      Stopwatch stopwatch = new Stopwatch();
      DateTime utcNow = DateTime.UtcNow;
      try
      {
        requestContext.TraceBlock(906041, 906042, "Query", nameof (WorkItemQueryService), "ExecuteQuery", (Action) (() =>
        {
          this.ResolveIdentitiesAndGenerateQueryHash(requestContext, query);
          detail = this.GenerateQueryExecutionDetailBasedOnOptimization(requestContext, this.GetQueryProcessor(requestContext), query, querySource, (object) topCount, out queryCategory, out optimizationInstance);
          IPermissionCheckHelper permissionCheckHelper = this.GetPermissionCheckHelper(requestContext);
          ExtendedQueryExecutionResult extendedResult = (ExtendedQueryExecutionResult) null;
          List<Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.LinkQueryResultEntry> linkResult = (List<Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.LinkQueryResultEntry>) null;
          bool flag = this.HasCrossProjectQueryPermission(requestContext);
          stopwatch.Start();
          switch (query.QueryType)
          {
            case QueryType.LinksOneHopMustContain:
            case QueryType.LinksOneHopMayContain:
            case QueryType.LinksOneHopDoesNotContain:
              using (QuerySqlComponent component = QuerySqlComponent.CreateComponent(requestContext, applicationIntentOverride, flag ? new Guid?() : projectId))
              {
                applicationIntent = component.ApplicationIntent;
                linkResult = component.QueryLink(detail, out extendedResult);
              }
              if (query.SortFields == null || !query.SortFields.Any<QuerySortField>())
                linkResult.Sort((IComparer<Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.LinkQueryResultEntry>) new WorkItemQueryService.OneHopLinkQueryResultComparer());
              queryResult = new QueryResult(query.QueryType, extendedResult);
              this.FilterOneHopLinkResult(requestContext, permissionCheckHelper, queryResult, linkResult);
              break;
            case QueryType.LinksRecursiveMayContain:
              using (QuerySqlComponent component = QuerySqlComponent.CreateComponent(requestContext, applicationIntentOverride, flag ? new Guid?() : projectId))
              {
                applicationIntent = component.ApplicationIntent;
                linkResult = component.QueryLink(detail, out extendedResult);
              }
              if (linkResult.Any<Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.LinkQueryResultEntry>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.LinkQueryResultEntry, bool>) (r => r.SourceId == 0)))
                requestContext.TraceConditionally(906051, TraceLevel.Verbose, "Query", string.Format("WorkItemQuery_{0}", (object) query.QueryHash), (Func<string>) (() => string.Join<int>(",", linkResult.Where<Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.LinkQueryResultEntry>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.LinkQueryResultEntry, bool>) (result => result.SourceId == 0)).Select<Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.LinkQueryResultEntry, int>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.LinkQueryResultEntry, int>) (result => result.TargetId)))));
              if (query.SortFields == null || !query.SortFields.Any<QuerySortField>())
                linkResult.Sort((IComparer<Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.LinkQueryResultEntry>) new WorkItemQueryService.TreeLinkQueryResultComparer());
              queryResult = new QueryResult(query.QueryType, extendedResult);
              queryResult.SetWorkItemLinkResult(TreeLinkUtilities.FilterTreeLinkResult(requestContext, permissionCheckHelper, query.RecursionOption, query.RecursionLinkTypeId, query.RightGroup != null, linkResult));
              break;
            default:
              List<WorkItemQueryResultEntry> workItemResult;
              using (QuerySqlComponent component = QuerySqlComponent.CreateComponent(requestContext, applicationIntentOverride, flag ? new Guid?() : projectId))
              {
                applicationIntent = component.ApplicationIntent;
                workItemResult = component.QueryWorkItem(detail, out extendedResult);
              }
              if (workItemResult.Count > 0)
                requestContext.TraceConditionally(906051, TraceLevel.Verbose, "Query", string.Format("WorkItemQuery_{0}", (object) query.QueryHash), (Func<string>) (() => string.Join<int>(",", workItemResult.Select<WorkItemQueryResultEntry, int>((Func<WorkItemQueryResultEntry, int>) (workItem => workItem.Id)))));
              bool sort = query.SortFields == null || !query.SortFields.Any<QuerySortField>();
              queryResult = new QueryResult(query.QueryType, extendedResult);
              this.FilterWorkItemResult(requestContext, permissionCheckHelper, queryResult, workItemResult, sort);
              break;
          }
          stopwatch.Stop();
        }));
        WorkItemTrackingTelemetry.TraceCustomerIntelligence(requestContext, QueryTelemetry.Feature, (object) query, (object) queryResult, (object) querySource, (object) requestContext.GetService<IQueryExperimentService>().GetCurrentExperimentState(requestContext), (object) queryCategory, (object) optimizationInstance, (object) stopwatch.ElapsedMilliseconds, (object) detail.FieldsDoStringComparison, (object) applicationIntent, (object) topCount);
      }
      catch (CircuitBreakerExceededConcurrencyException ex)
      {
        throw new WorkItemTrackingQueryTooManyConcurrentUsersException();
      }
      catch (CircuitBreakerShortCircuitException ex)
      {
        throw new WorkItemTrackingQueryServerBusyException();
      }
      catch (WorkItemTrackingQueryTimeoutException ex)
      {
        stopwatch.Stop();
        WorkItemTrackingTelemetry.TraceCustomerIntelligence(requestContext, QueryTelemetry.Feature, (object) query, (object) ex, (object) querySource, (object) requestContext.GetService<IQueryExperimentService>().GetCurrentExperimentState(requestContext), (object) queryCategory, (object) optimizationInstance, (object) stopwatch.ElapsedMilliseconds, (object) detail.FieldsDoStringComparison, (object) applicationIntent, (object) topCount);
        throw;
      }
      finally
      {
        if (requestContext.IsCanceled)
          requestContext.RequestContextInternal().ResetCancel();
        if (!requestContext.IsCanceled && detail != null)
        {
          if (stopwatch.IsRunning)
            stopwatch.Stop();
          this.RecordQueryExecutionInformation(requestContext, queryResult?.Count, query, detail.QueryText, applicationIntent, queryCategory, optimizationInstance, (int) stopwatch.ElapsedMilliseconds, utcNow);
        }
      }
      WorkItemKpiTracer.TraceKpi(requestContext, (WorkItemTrackingKpi) new ExecuteQueryKpi(requestContext), (WorkItemTrackingKpi) new CountKpi(requestContext, "QueryResultCount", queryResult.Count), (WorkItemTrackingKpi) new ExecuteQueryWithIdentityInGroupKpi(requestContext, query));
      return queryResult;
    }

    internal virtual AsOfDateTimesQueryResult ExecuteQueryAsOfTimes(
      IVssRequestContext requestContext,
      string wiql,
      IDictionary context,
      IEnumerable<DateTime> asOfDateTimes,
      Guid? projectId = null,
      QuerySource querySource = QuerySource.Unknown)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<string>(wiql, nameof (wiql));
      ArgumentUtility.CheckForNull<IEnumerable<DateTime>>(asOfDateTimes, nameof (asOfDateTimes));
      IVssRequestContext requestContext1 = requestContext;
      string wiql1 = wiql;
      IDictionary context1 = context;
      Guid? nullable = projectId;
      Guid? queryId = new Guid?();
      Guid? filterProjectId = nullable;
      QueryExpression queryExpression = this._ConvertToQueryExpression(requestContext1, wiql1, context1, queryId: queryId, filterProjectId: filterProjectId, collectMacro: true);
      Guid guid;
      requestContext.Items.TryGetGuid("queryId", out guid);
      queryExpression.QueryId = new Guid?(guid);
      return this.ExecuteQueryAsOfTimes(requestContext, queryExpression, asOfDateTimes, projectId, querySource: querySource);
    }

    internal virtual AsOfDateTimesQueryResult ExecuteQueryAsOfTimes(
      IVssRequestContext requestContext,
      QueryExpression query,
      IEnumerable<DateTime> asOfDateTimes,
      Guid? projectId = null,
      WITQueryApplicationIntentOverride applicationIntentOverride = WITQueryApplicationIntentOverride.Default,
      QuerySource querySource = QuerySource.Unknown)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<QueryExpression>(query, nameof (query));
      ArgumentUtility.CheckForNull<IEnumerable<DateTime>>(asOfDateTimes, nameof (asOfDateTimes));
      return this.ExecuteQueryAsOfTimesInternal(requestContext, query, asOfDateTimes, projectId, applicationIntentOverride, querySource);
    }

    private AsOfDateTimesQueryResult ExecuteQueryAsOfTimesInternal(
      IVssRequestContext requestContext,
      QueryExpression query,
      IEnumerable<DateTime> asOfDateTimes,
      Guid? projectId = null,
      WITQueryApplicationIntentOverride applicationIntentOverride = WITQueryApplicationIntentOverride.Default,
      QuerySource querySource = QuerySource.Unknown)
    {
      AsOfDateTimesQueryResult result = (AsOfDateTimesQueryResult) null;
      IEnumerable<AsOfQueryResultEntry> queryResult = (IEnumerable<AsOfQueryResultEntry>) null;
      QueryExecutionDetail detail = (QueryExecutionDetail) null;
      ApplicationIntent applicationIntent = ApplicationIntent.ReadWrite;
      QueryCategory queryCategory = QueryCategory.None;
      QueryOptimizationInstance optimizationInstance = (QueryOptimizationInstance) null;
      Stopwatch stopwatch = new Stopwatch();
      DateTime utcNow = DateTime.UtcNow;
      try
      {
        requestContext.TraceBlock(906043, 906044, "Query", nameof (WorkItemQueryService), "ExecuteQueryAsOfTimes", (Action) (() =>
        {
          asOfDateTimes = (IEnumerable<DateTime>) asOfDateTimes.Select<DateTime, DateTime>((Func<DateTime, DateTime>) (dt => DateTime.SpecifyKind(dt, DateTimeKind.Utc))).ToArray<DateTime>();
          this.ResolveIdentitiesAndGenerateQueryHash(requestContext, query);
          detail = this.GenerateQueryExecutionDetailBasedOnOptimization(requestContext, this.GetQueryProcessor(requestContext), query, querySource, (object) asOfDateTimes, out queryCategory, out optimizationInstance);
          IPermissionCheckHelper permissionCheckHelper = this.GetPermissionCheckHelper(requestContext);
          stopwatch.Start();
          if (query.QueryType != QueryType.WorkItems)
            throw new NotSupportedException(ServerResources.AsOfWorkItemQueriesNotSupported());
          using (QuerySqlComponent component = QuerySqlComponent.CreateComponent(requestContext, applicationIntentOverride, this.HasCrossProjectQueryPermission(requestContext) ? new Guid?() : projectId))
          {
            queryResult = component.QueryWorkItem(detail);
            applicationIntent = component.ApplicationIntent;
          }
          result = this.FilterWorkItemResult(requestContext, permissionCheckHelper, asOfDateTimes, queryResult);
          stopwatch.Stop();
        }));
        WorkItemTrackingTelemetry.TraceCustomerIntelligence(requestContext, TrendQueryTelemetry.Feature, (object) query, (object) queryResult, (object) querySource, (object) requestContext.GetService<IQueryExperimentService>().GetCurrentExperimentState(requestContext), (object) queryCategory, (object) optimizationInstance, (object) stopwatch.ElapsedMilliseconds, (object) detail.FieldsDoStringComparison, (object) applicationIntent, (object) asOfDateTimes);
      }
      catch (CircuitBreakerExceededConcurrencyException ex)
      {
        throw new WorkItemTrackingQueryTooManyConcurrentUsersException();
      }
      catch (CircuitBreakerShortCircuitException ex)
      {
        throw new WorkItemTrackingQueryServerBusyException();
      }
      catch (WorkItemTrackingQueryTimeoutException ex)
      {
        stopwatch.Stop();
        WorkItemTrackingTelemetry.TraceCustomerIntelligence(requestContext, TrendQueryTelemetry.Feature, (object) query, (object) ex, (object) querySource, (object) requestContext.GetService<IQueryExperimentService>().GetCurrentExperimentState(requestContext), (object) queryCategory, (object) optimizationInstance, (object) stopwatch.ElapsedMilliseconds, (object) detail.FieldsDoStringComparison, (object) applicationIntent, (object) asOfDateTimes);
        throw;
      }
      finally
      {
        if (requestContext.IsCanceled)
          requestContext.RequestContextInternal().ResetCancel();
        if (!requestContext.IsCanceled && detail != null)
        {
          if (stopwatch.IsRunning)
            stopwatch.Stop();
          IVssRequestContext requestContext1 = requestContext;
          AsOfDateTimesQueryResult timesQueryResult = result;
          int? resultCount = timesQueryResult != null ? new int?(timesQueryResult.WorkItemResults.Count<AsOfQueryResultEntry>()) : new int?();
          QueryExpression query1 = query;
          string queryText = detail.QueryText;
          int num1 = (int) applicationIntent;
          long num2 = (long) queryCategory;
          QueryOptimizationInstance optimizationInstance1 = optimizationInstance;
          int elapsedMilliseconds = (int) stopwatch.ElapsedMilliseconds;
          DateTime runTime = utcNow;
          this.RecordQueryExecutionInformation(requestContext1, resultCount, query1, queryText, (ApplicationIntent) num1, (QueryCategory) num2, optimizationInstance1, elapsedMilliseconds, runTime);
        }
      }
      return result;
    }

    private void CalculateQueryHash(
      IVssRequestContext requestContext,
      QueryExpression query,
      ResolvedIdentityNamesInfo resolvedNamesInfo)
    {
      string sha1HashString = CommonWITUtils.GetSha1HashString(CommonWITUtils.NormalizeWiql(WiqlUtils.TransformNamesToIds(requestContext, query.Wiql, resolvedNamesInfo)));
      query.QueryHash = sha1HashString;
    }

    private QueryOptimizationInstance GetQueryOptimizations(
      IVssRequestContext requestContext,
      QueryExpression query,
      QueryOptimizationStrategy strategyToMatchForFuzzMatchOnId)
    {
      QueryOptimizationInstance queryOptimizations1 = (QueryOptimizationInstance) null;
      query.OptimizationSource = QueryOptimizationSource.None;
      string queryOptimizations2;
      if (requestContext.TryGetItem<string>("QueryOptimizationsFromRest", out queryOptimizations2) && !string.IsNullOrEmpty(queryOptimizations2))
      {
        query.Optimizations = WorkItemTrackingQueryOptimizationConfiguration.ParseQueryOptimization(queryOptimizations2);
        query.OptimizationSource = QueryOptimizationSource.Header;
      }
      else
      {
        WorkItemTrackingQueryOptimizationConfiguration optimizationSettings = requestContext.WitContext()?.ServerSettings?.QueryOptimizationSettings;
        IReadOnlyDictionary<Guid, QueryOptimization> optimizationsById = optimizationSettings?.QueryOptimizationsById;
        IReadOnlyDictionary<string, QueryOptimization> optimizationsByHash = optimizationSettings?.QueryOptimizationsByHash;
        Guid? queryId = query.QueryId;
        if (queryId.HasValue && optimizationsById != null)
        {
          IReadOnlyDictionary<Guid, QueryOptimization> readOnlyDictionary1 = optimizationsById;
          queryId = query.QueryId;
          Guid key1 = queryId.Value;
          if (readOnlyDictionary1.ContainsKey(key1))
          {
            QueryExpression queryExpression = query;
            IReadOnlyDictionary<Guid, QueryOptimization> readOnlyDictionary2 = optimizationsById;
            queryId = query.QueryId;
            Guid key2 = queryId.Value;
            int num = (int) readOnlyDictionary2[key2];
            queryExpression.Optimizations = (QueryOptimization) num;
            query.OptimizationSource = QueryOptimizationSource.Registry;
            goto label_7;
          }
        }
        if (!string.IsNullOrEmpty(query.QueryHash) && optimizationsByHash != null && optimizationsByHash.ContainsKey(query.QueryHash))
        {
          query.Optimizations = optimizationsByHash[query.QueryHash];
          query.OptimizationSource = QueryOptimizationSource.Registry;
        }
      }
label_7:
      if (query.Optimizations == QueryOptimization.None && WorkItemTrackingFeatureFlags.IsQueryAutoOptimizationEnabled(requestContext))
      {
        queryOptimizations1 = requestContext.GetService<IQueryOptimizationCacheService>().GetQueryOptimizationInstance(requestContext, query.QueryId, query.QueryHash, strategyToMatchForFuzzMatchOnId);
        QueryOptimization? queryOptimization = queryOptimizations1?.GetCurrentQueryOptimization();
        if (queryOptimization.HasValue)
        {
          CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
          CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
          intelligenceData.Add("CurrentOptimization", (object) queryOptimization.Value);
          IVssRequestContext requestContext1 = requestContext;
          CustomerIntelligenceData properties = intelligenceData;
          service.Publish(requestContext1, nameof (WorkItemQueryService), nameof (GetQueryOptimizations), properties);
          query.Optimizations = queryOptimization.Value;
          query.OptimizationSource = QueryOptimizationSource.DynamicSwitch;
        }
      }
      return queryOptimizations1;
    }

    private QueryExecutionDetail GenerateQueryExecutionDetailBasedOnOptimization(
      IVssRequestContext requestContext,
      QueryProcessor processor,
      QueryExpression query,
      QuerySource querySource,
      object topCountOrAsofDates,
      out QueryCategory queryCategory,
      out QueryOptimizationInstance optimizationInstance)
    {
      optimizationInstance = this.GetQueryOptimizations(requestContext, query, (QueryOptimizationStrategy) null);
      QueryExecutionDetail queryExecutionDetail = this.GenerateQueryExecutionDetail(requestContext, processor, query, querySource, topCountOrAsofDates);
      queryCategory = queryExecutionDetail.QueryCategory;
      return queryExecutionDetail;
    }

    private void ResolveIdentitiesAndGenerateQueryHash(
      IVssRequestContext requestContext,
      QueryExpression queryExpression)
    {
      string str;
      requestContext.Items.TryGetValue<string>("AsOfHistoryRange", out str);
      if (!string.IsNullOrEmpty(str))
        queryExpression.Wiql = queryExpression.Wiql + "::" + str;
      ResolvedIdentityNamesInfo resolvedNamesInfo = this.ResolveQueryExpressionIdentities(requestContext, queryExpression);
      this.CalculateQueryHash(requestContext, queryExpression, resolvedNamesInfo);
    }

    private QueryExecutionDetail GenerateQueryExecutionDetail(
      IVssRequestContext requestContext,
      QueryProcessor processor,
      QueryExpression query,
      QuerySource querySource,
      object topCountOrAsofDates)
    {
      QueryExecutionDetail queryExecutionDetail = (QueryExecutionDetail) null;
      switch (topCountOrAsofDates)
      {
        case int topCount:
          queryExecutionDetail = processor.GenerateQueryExecutionDetail(query, topCount, querySource);
          break;
        case IEnumerable<DateTime> asOfDateTimes:
          queryExecutionDetail = processor.GenerateQueryExecutionDetail(query, asOfDateTimes, querySource);
          break;
      }
      requestContext.Trace(906050, TraceLevel.Verbose, "Query", string.Format("WorkItemQuery_{0}", (object) query.QueryHash), queryExecutionDetail.QueryText);
      return queryExecutionDetail;
    }

    private AsOfDateTimesQueryResult FilterWorkItemResult(
      IVssRequestContext requestContext,
      IPermissionCheckHelper helper,
      IEnumerable<DateTime> asOfDateTimes,
      IEnumerable<AsOfQueryResultEntry> rawResult)
    {
      AsOfDateTimesQueryResult result = new AsOfDateTimesQueryResult(QueryType.WorkItems, asOfDateTimes);
      int maxDailyResults = requestContext.WitContext().ServerSettings.MaxTrendChartTimeSliceResultSize;
      HashSet<Guid> guidSet = new HashSet<Guid>();
      requestContext.TraceBlock(906001, 906002, "Query", nameof (WorkItemQueryService), nameof (FilterWorkItemResult), (Action) (() =>
      {
        int rawCount = 0;
        AsOfQueryResultEntry[] array = rawResult.Where<AsOfQueryResultEntry>((Func<AsOfQueryResultEntry, bool>) (x =>
        {
          ++rawCount;
          return helper.HasWorkItemPermission(x.AreaId, 16);
        })).ToArray<AsOfQueryResultEntry>();
        CommonWITUtils.TraceRawAndFilteredResultCount(requestContext, rawCount, array.Length);
        this.BlockLargeHistoricalRequests((IEnumerable<AsOfQueryResultEntry>) array, asOfDateTimes, maxDailyResults);
        result.WorkItemResults = (IEnumerable<AsOfQueryResultEntry>) array;
      }));
      return result;
    }

    private void BlockLargeHistoricalRequests(
      IEnumerable<AsOfQueryResultEntry> filteredResult,
      IEnumerable<DateTime> asOfDateTimes,
      int maxDailyItemss)
    {
      bool flag = filteredResult.Count<AsOfQueryResultEntry>() > asOfDateTimes.Count<DateTime>() * maxDailyItemss;
      if (!flag && filteredResult.GroupBy<AsOfQueryResultEntry, DateTime, int>((Func<AsOfQueryResultEntry, DateTime>) (o => o.AsOfDateTime), (Func<AsOfQueryResultEntry, int>) (o => o.Id)).Any<IGrouping<DateTime, int>>((Func<IGrouping<DateTime, int>, bool>) (o => o.Count<int>() > maxDailyItemss)))
        flag = true;
      if (flag)
        throw new WorkItemTrackingTrendQueryResultSizeLimitExceededException(maxDailyItemss);
    }

    private void FilterWorkItemResult(
      IVssRequestContext requestContext,
      IPermissionCheckHelper helper,
      QueryResult queryResult,
      List<WorkItemQueryResultEntry> rawResult,
      bool sort)
    {
      requestContext.TraceBlock(906001, 906002, "Query", nameof (WorkItemQueryService), nameof (FilterWorkItemResult), (Action) (() =>
      {
        Dictionary<int, string> workitemIdToTokenLookup = new Dictionary<int, string>();
        int[] array = rawResult.Where<WorkItemQueryResultEntry>((Func<WorkItemQueryResultEntry, bool>) (x =>
        {
          int num = helper.HasWorkItemPermission(x.AreaId, 16) ? 1 : 0;
          if (num == 0)
            return num != 0;
          workitemIdToTokenLookup[x.Id] = helper.GetWorkItemSecurityToken(x.AreaId);
          return num != 0;
        })).Select<WorkItemQueryResultEntry, int>((Func<WorkItemQueryResultEntry, int>) (x => x.Id)).ToArray<int>();
        if (sort)
          Array.Sort<int>(array);
        CommonWITUtils.TraceRawAndFilteredResultCount(requestContext, rawResult.Count, array.Length);
        queryResult.SetWorkItemResult((ICollection<int>) array, (IDictionary<int, string>) workitemIdToTokenLookup);
      }));
    }

    private void FilterOneHopLinkResult(
      IVssRequestContext requestContext,
      IPermissionCheckHelper helper,
      QueryResult queryResult,
      List<Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.LinkQueryResultEntry> rawResult)
    {
      requestContext.TraceBlock(906003, 906004, "Query", nameof (WorkItemQueryService), nameof (FilterOneHopLinkResult), (Action) (() =>
      {
        Dictionary<int, WorkItemQueryService.OneHopLinkInfo> dictionary = new Dictionary<int, WorkItemQueryService.OneHopLinkInfo>();
        CommonWITUtils.TraceRawResultCount(requestContext, rawResult.Count);
        for (int index = 0; index < rawResult.Count; ++index)
        {
          Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.LinkQueryResultEntry resultEntry = rawResult[index];
          if (!helper.HasWorkItemPermission(resultEntry.SourceAreaId, 16))
          {
            resultEntry.IsValid = false;
          }
          else
          {
            WorkItemQueryService.OneHopLinkInfo oneHopLinkInfo;
            if (!dictionary.TryGetValue(resultEntry.SourceId, out oneHopLinkInfo))
            {
              oneHopLinkInfo = new WorkItemQueryService.OneHopLinkInfo(resultEntry);
              dictionary[resultEntry.SourceId] = oneHopLinkInfo;
            }
            if (resultEntry.TargetId != 0 && (resultEntry.TargetAreaId == 0 || helper.HasWorkItemPermission(resultEntry.TargetAreaId, 16)))
            {
              ++oneHopLinkInfo.TargetCount;
              if (queryResult.QueryType == QueryType.LinksOneHopDoesNotContain)
                resultEntry.IsValid = false;
            }
            else
              resultEntry.IsValid = false;
          }
        }
        int filteredResultCount = 0;
        if (queryResult.QueryType != QueryType.LinksOneHopDoesNotContain)
        {
          IEnumerable<WorkItemQueryService.OneHopLinkInfo> source = dictionary.Values.Where<WorkItemQueryService.OneHopLinkInfo>((Func<WorkItemQueryService.OneHopLinkInfo, bool>) (x => x.TargetCount > 0));
          requestContext.Trace(906010, TraceLevel.Verbose, "Query", nameof (WorkItemQueryService), "Adding count for may/must contains. Number of items has targets: {0}.", (object) source.Count<WorkItemQueryService.OneHopLinkInfo>());
          foreach (WorkItemQueryService.OneHopLinkInfo oneHopLinkInfo in source)
            filteredResultCount += oneHopLinkInfo.TargetCount + 1;
        }
        if (queryResult.QueryType != QueryType.LinksOneHopMustContain)
        {
          IEnumerable<WorkItemQueryService.OneHopLinkInfo> source = dictionary.Values.Where<WorkItemQueryService.OneHopLinkInfo>((Func<WorkItemQueryService.OneHopLinkInfo, bool>) (x => x.TargetCount == 0));
          requestContext.Trace(906011, TraceLevel.Verbose, "Query", nameof (WorkItemQueryService), "Adding count for may/maynot contains. Number of items has 0 target: {0}.", (object) source.Count<WorkItemQueryService.OneHopLinkInfo>());
          foreach (WorkItemQueryService.OneHopLinkInfo oneHopLinkInfo in source)
          {
            oneHopLinkInfo.FirstTarget.IsValid = true;
            ++filteredResultCount;
            if (queryResult.QueryType == QueryType.LinksOneHopMayContain)
              oneHopLinkInfo.FirstTarget.TargetId = 0;
          }
        }
        CommonWITUtils.TraceRawAndFilteredResultCount(requestContext, rawResult.Count, filteredResultCount);
        if (queryResult.QueryType == QueryType.LinksOneHopDoesNotContain)
        {
          Dictionary<int, string> workitemToTokenLookup = new Dictionary<int, string>();
          queryResult.SetWorkItemResult((ICollection<int>) rawResult.Where<Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.LinkQueryResultEntry>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.LinkQueryResultEntry, bool>) (x =>
          {
            if (x.IsValid)
              workitemToTokenLookup[x.SourceId] = helper.GetWorkItemSecurityToken(x.SourceAreaId);
            return x.IsValid;
          })).Select<Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.LinkQueryResultEntry, int>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.LinkQueryResultEntry, int>) (x => x.SourceId)).ToArray<int>(), (IDictionary<int, string>) workitemToTokenLookup);
        }
        else
        {
          LinkQueryResultEntry[] workItemLinks = new LinkQueryResultEntry[filteredResultCount];
          int num1 = 0;
          int num2 = 0;
          for (int index = 0; index < rawResult.Count; ++index)
          {
            Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.LinkQueryResultEntry queryResultEntry = rawResult[index];
            if (queryResultEntry.IsValid)
            {
              if (num2 != queryResultEntry.SourceId)
              {
                workItemLinks[num1++] = new LinkQueryResultEntry()
                {
                  SourceId = 0,
                  TargetId = queryResultEntry.SourceId,
                  LinkTypeId = (short) 0,
                  IsLocked = false,
                  SourceToken = string.Empty,
                  TargetToken = helper.GetWorkItemSecurityToken(queryResultEntry.SourceAreaId)
                };
                num2 = queryResultEntry.SourceId;
              }
              if (queryResultEntry.TargetId > 0)
                workItemLinks[num1++] = new LinkQueryResultEntry()
                {
                  SourceId = queryResultEntry.SourceId,
                  TargetId = queryResultEntry.TargetId,
                  LinkTypeId = queryResultEntry.LinkTypeId,
                  IsLocked = queryResultEntry.IsLocked,
                  SourceToken = helper.GetWorkItemSecurityToken(queryResultEntry.SourceAreaId),
                  TargetToken = helper.GetWorkItemSecurityToken(queryResultEntry.TargetAreaId)
                };
            }
          }
          queryResult.SetWorkItemLinkResult((ICollection<LinkQueryResultEntry>) workItemLinks);
        }
      }));
    }

    private void RecordQueryExecutionInformation(
      IVssRequestContext requestContext,
      int? resultCount,
      QueryExpression query,
      string SqlText,
      ApplicationIntent applicationIntent,
      QueryCategory queryCategory,
      QueryOptimizationInstance optimizationInstance,
      int elapsedTImeInMs,
      DateTime runTime)
    {
      if (!this.IsTrackingNeeded(requestContext, query))
        return;
      this.m_executionLogger.RecordQueryExecutionInformation(requestContext, query, runTime, requestContext.GetUserId(), elapsedTImeInMs, resultCount, query.QueryType, SqlText, applicationIntent == ApplicationIntent.ReadOnly, queryCategory, optimizationInstance);
    }

    protected virtual bool IsTrackingNeeded(
      IVssRequestContext requestContext,
      QueryExpression query)
    {
      return query.IsTrackingNeeded;
    }

    private ResolvedIdentityNamesInfo ResolveQueryExpressionIdentities(
      IVssRequestContext requestContext,
      QueryExpression query)
    {
      if (query == null)
        return (ResolvedIdentityNamesInfo) null;
      List<QueryComparisonExpressionNode> identityNodes = this.GetIdentityNodes(query);
      if (!identityNodes.Any<QueryComparisonExpressionNode>())
        return (ResolvedIdentityNamesInfo) null;
      ResolvedIdentityNamesInfo resolvedNamesInfo = this.ResolveIdentityNodeValues(requestContext, identityNodes);
      bool hostedDeployment = requestContext.ExecutionEnvironment.IsHostedDeployment;
      if (hostedDeployment)
        this.ResolveInGroupNodeValues(requestContext, (IEnumerable<QueryComparisonExpressionNode>) identityNodes);
      this.ReplaceDistinctNameIdentityNodes(requestContext, query, resolvedNamesInfo, hostedDeployment);
      return resolvedNamesInfo;
    }

    private void ReplaceDistinctNameIdentityNodes(
      IVssRequestContext requestContext,
      QueryExpression query,
      ResolvedIdentityNamesInfo resolvedNamesInfo,
      bool notInjectgAmbiguousIdentity)
    {
      query.LeftGroup = this.ReplaceDistinctNameNodes(requestContext, query.LeftGroup, resolvedNamesInfo, notInjectgAmbiguousIdentity);
      query.RightGroup = this.ReplaceDistinctNameNodes(requestContext, query.RightGroup, resolvedNamesInfo, notInjectgAmbiguousIdentity);
    }

    private ResolvedIdentityNamesInfo ResolveIdentityNodeValues(
      IVssRequestContext requestContext,
      List<QueryComparisonExpressionNode> identityNodes)
    {
      if (identityNodes == null || !identityNodes.Any<QueryComparisonExpressionNode>())
        return new ResolvedIdentityNamesInfo();
      HashSet<string> identityNames = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      List<QueryComparisonExpressionNode> comparisonExpressionNodeList = new List<QueryComparisonExpressionNode>();
      foreach (QueryComparisonExpressionNode identityNode in identityNodes)
      {
        if (identityNode.Field.IsIdentity && !string.IsNullOrEmpty(identityNode.Value.StringValue) && WorkItemQueryService.s_validNamesReplacementOperators.Contains(identityNode.Operator))
        {
          identityNames.Add(identityNode.Value.StringValue);
          comparisonExpressionNodeList.Add(identityNode);
        }
      }
      return comparisonExpressionNodeList.Any<QueryComparisonExpressionNode>() ? this.ReplaceIdentityNames(requestContext, (IEnumerable<QueryComparisonExpressionNode>) comparisonExpressionNodeList, identityNames) : new ResolvedIdentityNamesInfo();
    }

    private ResolvedIdentityNamesInfo ReplaceIdentityNames(
      IVssRequestContext requestContext,
      IEnumerable<QueryComparisonExpressionNode> identityNodes,
      HashSet<string> identityNames)
    {
      ResolvedIdentityNamesInfo resolvedNamesInfo = new ResolvedIdentityNamesInfo();
      if (identityNames.Any<string>())
      {
        resolvedNamesInfo = requestContext.GetService<IWorkItemIdentityService>().ResolveIdentityNames(requestContext.WitContext(), (IEnumerable<string>) identityNames, true, false);
        WorkItemTrackingTelemetry.TraceCustomerIntelligence(requestContext, WorkItemIdentityTelemetry.Feature, (object) WorkItemIdentityTelemetrySource.Query, (object) resolvedNamesInfo);
        Lazy<IDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>> tfIdToIdentityLookup = new Lazy<IDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>>((Func<IDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>>) (() => (IDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>) resolvedNamesInfo.IdentityMap.Value.ToDedupedDictionary<KeyValuePair<string, Microsoft.VisualStudio.Services.Identity.Identity>, Guid, Microsoft.VisualStudio.Services.Identity.Identity>((Func<KeyValuePair<string, Microsoft.VisualStudio.Services.Identity.Identity>, Guid>) (kvp => kvp.Value.Id), (Func<KeyValuePair<string, Microsoft.VisualStudio.Services.Identity.Identity>, Microsoft.VisualStudio.Services.Identity.Identity>) (kvp => kvp.Value))));
        foreach (QueryComparisonExpressionNode identityNode in identityNodes)
        {
          ConstantsSearchRecord constantsSearchRecord = (ConstantsSearchRecord) null;
          Microsoft.VisualStudio.Services.Identity.Identity identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
          if (resolvedNamesInfo.NamesLookup.TryGetValue(identityNode.Value.StringValue, out constantsSearchRecord))
          {
            identityNode.Value.GuidValue = constantsSearchRecord.TeamFoundationId;
            identityNode.Value.StringValue = constantsSearchRecord.DisplayPart;
          }
          else
          {
            Guid guid;
            if (resolvedNamesInfo.ResolvedNonLicensedIdentities.TryGetValue(identityNode.Value.StringValue, out guid))
              identityNode.Value.GuidValue = guid;
            else if (resolvedNamesInfo.AadIdentityLookup.TryGetValue(identityNode.Value.StringValue, out identity))
            {
              identityNode.Value.GuidValue = identity.Id;
              identityNode.Value.StringValue = identity.GetLegacyDistinctDisplayName();
            }
            else
            {
              ConstantsSearchRecord[] source;
              if (resolvedNamesInfo.AmbiguousNamesLookup.TryGetValue(identityNode.Value.StringValue, out source))
              {
                List<\u003C\u003Ef__AnonymousType56<ConstantsSearchRecord, Microsoft.VisualStudio.Services.Identity.Identity>> list = ((IEnumerable<ConstantsSearchRecord>) source).Select(r => new
                {
                  Record = r,
                  Identity = tfIdToIdentityLookup.Value.GetValueOrDefault<Guid, Microsoft.VisualStudio.Services.Identity.Identity>(r.TeamFoundationId)
                }).Where(x => x.Identity != null && x.Identity.IsContainer).OrderBy(x => !x.Identity.IsActive).ToList();
                if (list.Any())
                {
                  ConstantsSearchRecord record = list.First().Record;
                  identityNode.Value.GuidValue = record.TeamFoundationId;
                  identityNode.Value.StringValue = record.DisplayPart;
                  if (list.Count(x => x.Identity.IsActive) != 1)
                  {
                    CustomerIntelligenceData properties = new CustomerIntelligenceData();
                    properties.Add("AmbiguousIdentity", (object) ((IEnumerable<ConstantsSearchRecord>) source).Select(r => new
                    {
                      TeamFoundationId = r.TeamFoundationId,
                      ConstId = r.Id
                    }));
                    requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, nameof (WorkItemQueryService), "QueryAmbiguousIdentities", properties);
                  }
                }
              }
            }
          }
        }
      }
      return resolvedNamesInfo;
    }

    private void ResolveInGroupNodeValues(
      IVssRequestContext requestContext,
      IEnumerable<QueryComparisonExpressionNode> identityNodes)
    {
      IEnumerable<QueryComparisonExpressionNode> comparisonExpressionNodes = identityNodes.Where<QueryComparisonExpressionNode>((Func<QueryComparisonExpressionNode, bool>) (node => node.ExpandConstant && (node.Operator == QueryExpressionOperator.Equals || node.Operator == QueryExpressionOperator.NotEquals) && node.Value.GuidValue != Guid.Empty));
      if (!comparisonExpressionNodes.Any<QueryComparisonExpressionNode>())
        return;
      IDictionary<Guid, IEnumerable<Guid>> dictionary = WorkItemQueryService.ExpandGroups(requestContext, comparisonExpressionNodes, requestContext.WitContext().ServerSettings.MaxIdentityInGroupSize);
      foreach (QueryComparisonExpressionNode comparisonExpressionNode in comparisonExpressionNodes)
      {
        IEnumerable<Guid> guids;
        if (dictionary.TryGetValue(comparisonExpressionNode.Value.GuidValue, out guids))
        {
          comparisonExpressionNode.Value.ValueType = QueryExpressionValueType.IdentityGuid;
          comparisonExpressionNode.Value.IdentityGuidValues = guids;
        }
      }
    }

    private static IDictionary<Guid, IEnumerable<Guid>> ExpandGroups(
      IVssRequestContext requestContext,
      IEnumerable<QueryComparisonExpressionNode> groupNodes,
      int maxGroupSize)
    {
      requestContext.TraceEnter(909101, "Query", nameof (WorkItemQueryService), nameof (ExpandGroups));
      Stopwatch stopwatch = Stopwatch.StartNew();
      IDictionary<Guid, IEnumerable<Guid>> source1 = (IDictionary<Guid, IEnumerable<Guid>>) null;
      try
      {
        IEnumerable<Guid> source2 = groupNodes.Select<QueryComparisonExpressionNode, Guid>((Func<QueryComparisonExpressionNode, Guid>) (node => node.Value.GuidValue)).Distinct<Guid>();
        IdentityService service1 = requestContext.GetService<IdentityService>();
        IGraphMembershipTraversalService service2 = requestContext.GetService<IGraphMembershipTraversalService>();
        IVssRequestContext requestContext1 = requestContext;
        Guid[] array = source2.ToArray<Guid>();
        IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> source3 = service1.ReadIdentities(requestContext1, (IList<Guid>) array, QueryMembership.None, (IEnumerable<string>) null).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (i => i != null));
        List<SubjectDescriptor> list = source3.Select<Microsoft.VisualStudio.Services.Identity.Identity, SubjectDescriptor>((Func<Microsoft.VisualStudio.Services.Identity.Identity, SubjectDescriptor>) (i => i.SubjectDescriptor)).ToList<SubjectDescriptor>();
        Dictionary<SubjectDescriptor, Guid> descriptorToVsidLookup = source3.ToDedupedDictionary<Microsoft.VisualStudio.Services.Identity.Identity, SubjectDescriptor, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, SubjectDescriptor>) (key => key.SubjectDescriptor), (Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (value => value.Id));
        IReadOnlyDictionary<SubjectDescriptor, GraphMembershipTraversal> source4 = service2.LookupDescendantsTraversals(requestContext, (IEnumerable<SubjectDescriptor>) list, -1);
        GraphMembershipTraversal membershipTraversal = source4.Values.FirstOrDefault<GraphMembershipTraversal>((Func<GraphMembershipTraversal, bool>) (tr => !tr.IsComplete));
        if (membershipTraversal != null)
          throw new WorkItemTrackingQueryException(membershipTraversal.IncompletenessReason);
        source1 = (IDictionary<Guid, IEnumerable<Guid>>) source4.ToDedupedDictionary<KeyValuePair<SubjectDescriptor, GraphMembershipTraversal>, Guid, IEnumerable<Guid>>((Func<KeyValuePair<SubjectDescriptor, GraphMembershipTraversal>, Guid>) (key => descriptorToVsidLookup[key.Key]), (Func<KeyValuePair<SubjectDescriptor, GraphMembershipTraversal>, IEnumerable<Guid>>) (value => value.Value.TraversedSubjectIds));
        if (!source1.Any<KeyValuePair<Guid, IEnumerable<Guid>>>())
          requestContext.TraceConditionally(909103, TraceLevel.Verbose, "Query", nameof (WorkItemQueryService), (Func<string>) (() => string.Format("Groups {0} to be expanded could not be found.", (object) string.Join(",", groupNodes.Select<QueryComparisonExpressionNode, string>((Func<QueryComparisonExpressionNode, string>) (node => node.Value.StringValue))))));
        Guid groupExceedingMaxGroupSize = source1.FirstOrDefault<KeyValuePair<Guid, IEnumerable<Guid>>>((Func<KeyValuePair<Guid, IEnumerable<Guid>>, bool>) (result => result.Value.Count<Guid>() > maxGroupSize)).Key;
        if (groupExceedingMaxGroupSize != Guid.Empty)
          throw new WorkItemTrackingQueryException(ServerResources.QueryInGroupTooLarge((object) groupNodes.First<QueryComparisonExpressionNode>((Func<QueryComparisonExpressionNode, bool>) (node => node.Value.GuidValue == groupExceedingMaxGroupSize)).Value, (object) maxGroupSize));
        return source1;
      }
      catch (TooManyRequestedItemsException ex)
      {
        int? nullable = ex.RequestedCount;
        string message;
        if (nullable.HasValue)
        {
          nullable = ex.MaxLimit;
          if (nullable.HasValue)
          {
            message = ServerResources.QueryHasTooManyAADGroupsToExpand_DiscloseCountAndLimit((object) ex.RequestedCount, (object) ex.MaxLimit);
            goto label_12;
          }
        }
        message = ServerResources.QueryHasTooManyAADGroupsToExpand();
label_12:
        throw new WorkItemTrackingQueryException(message);
      }
      finally
      {
        stopwatch.Stop();
        requestContext.Items["ExpandIdentityDuration"] = (object) stopwatch.ElapsedMilliseconds;
        if (source1 != null)
        {
          Dictionary<string, int> dictionary = new Dictionary<string, int>();
          foreach (QueryComparisonExpressionNode groupNode in groupNodes)
          {
            IEnumerable<Guid> source5;
            if (source1.TryGetValue(groupNode.Value.GuidValue, out source5))
              dictionary[groupNode.Value.StringValue] = source5.Count<Guid>();
          }
          requestContext.Items["ExpandIdentityCount"] = (object) dictionary;
        }
        requestContext.TraceLeave(909102, "Query", nameof (WorkItemQueryService), nameof (ExpandGroups));
      }
    }

    private List<QueryComparisonExpressionNode> GetIdentityNodes(QueryExpression query)
    {
      List<QueryComparisonExpressionNode> nodes = new List<QueryComparisonExpressionNode>();
      this.GetIdentityNodes(query.LeftGroup, nodes);
      this.GetIdentityNodes(query.RightGroup, nodes);
      return nodes;
    }

    private void GetIdentityNodes(
      QueryExpressionNode expressionNode,
      List<QueryComparisonExpressionNode> nodes)
    {
      switch (expressionNode)
      {
        case QueryComparisonExpressionNode _:
          QueryComparisonExpressionNode comparisonExpressionNode = expressionNode as QueryComparisonExpressionNode;
          if (comparisonExpressionNode.Field == null || !comparisonExpressionNode.Field.IsIdentity)
            break;
          nodes.Add(comparisonExpressionNode);
          break;
        case QueryLogicalExpressionNode _:
          QueryLogicalExpressionNode logicalExpressionNode = expressionNode as QueryLogicalExpressionNode;
          if (logicalExpressionNode.Children == null || !logicalExpressionNode.Children.Any<QueryExpressionNode>())
            break;
          using (IEnumerator<QueryExpressionNode> enumerator = logicalExpressionNode.Children.GetEnumerator())
          {
            while (enumerator.MoveNext())
              this.GetIdentityNodes(enumerator.Current, nodes);
            break;
          }
      }
    }

    private QueryExpressionNode ReplaceDistinctNameNodes(
      IVssRequestContext requestContext,
      QueryExpressionNode expressionNode,
      ResolvedIdentityNamesInfo resolvedNameInfo,
      bool notInjectAmbiguousIdentity)
    {
      switch (expressionNode)
      {
        case null:
          return (QueryExpressionNode) null;
        case QueryComparisonExpressionNode _:
          QueryComparisonExpressionNode queryComparisonExpressionNode = expressionNode as QueryComparisonExpressionNode;
          bool flag1 = this.IsBackCompatIdentityQueryExpressionField(requestContext, queryComparisonExpressionNode);
          if (queryComparisonExpressionNode.ExpandConstant && (notInjectAmbiguousIdentity || !flag1))
          {
            queryComparisonExpressionNode.IdentityNamesInfo = resolvedNameInfo;
            return expressionNode;
          }
          if (flag1)
          {
            if (!string.IsNullOrEmpty(queryComparisonExpressionNode.Value.StringValue) && resolvedNameInfo.AmbiguousNamesLookup.ContainsKey(queryComparisonExpressionNode.Value.StringValue))
            {
              List<QueryExpressionNode> queryExpressionNodeList = new List<QueryExpressionNode>();
              QueryLogicalExpressionNode logicalExpressionNode = new QueryLogicalExpressionNode()
              {
                Operator = WorkItemQueryService.s_validNegationNamesReplacementOperators.Contains(queryComparisonExpressionNode.Operator) ? QueryLogicalExpressionOperator.And : QueryLogicalExpressionOperator.Or
              };
              List<ConstantsSearchRecord> constantsSearchRecordList = new List<ConstantsSearchRecord>();
              foreach (ConstantsSearchRecord constantsSearchRecord in resolvedNameInfo.AmbiguousNamesLookup[queryComparisonExpressionNode.Value.StringValue])
              {
                Microsoft.VisualStudio.Services.Identity.Identity identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
                if (resolvedNameInfo.IdentityMap.Value.TryGetValue(constantsSearchRecord.DisplayPart, out identity) && identity.IsActive)
                  constantsSearchRecordList.Add(constantsSearchRecord);
              }
              if (constantsSearchRecordList.Count == 1 && requestContext.GetIdentityDisplayType() != IdentityDisplayType.DisplayName)
              {
                QueryComparisonExpressionNode ambiguousIdentityNode = this.CreateAmbiguousIdentityNode(queryComparisonExpressionNode.ExpandConstant, queryComparisonExpressionNode.Field, queryComparisonExpressionNode.Operator, constantsSearchRecordList[0].DisplayPart, false);
                queryExpressionNodeList.Add((QueryExpressionNode) ambiguousIdentityNode);
              }
              else
              {
                foreach (ConstantsSearchRecord constantsSearchRecord in resolvedNameInfo.AmbiguousNamesLookup[queryComparisonExpressionNode.Value.StringValue])
                {
                  QueryComparisonExpressionNode ambiguousIdentityNode = this.CreateAmbiguousIdentityNode(queryComparisonExpressionNode.ExpandConstant, queryComparisonExpressionNode.Field, queryComparisonExpressionNode.Operator, constantsSearchRecord.DisplayPart, false);
                  queryExpressionNodeList.Add((QueryExpressionNode) ambiguousIdentityNode);
                }
              }
              queryExpressionNodeList.Add((QueryExpressionNode) queryComparisonExpressionNode);
              logicalExpressionNode.Children = (IEnumerable<QueryExpressionNode>) queryExpressionNodeList;
              return (QueryExpressionNode) logicalExpressionNode;
            }
            break;
          }
          if (this.IsDistinctNameIdentityField(queryComparisonExpressionNode))
          {
            QueryComparisonExpressionNode ambiguousIdentityNode = this.CreateAmbiguousIdentityNode(queryComparisonExpressionNode.ExpandConstant, queryComparisonExpressionNode.Field, queryComparisonExpressionNode.Operator, IdentityHelper.GetDisplayNameFromDistinctDisplayName(queryComparisonExpressionNode.Value.StringValue), false);
            return (QueryExpressionNode) new QueryLogicalExpressionNode()
            {
              Operator = (WorkItemQueryService.s_validNegationNamesReplacementOperators.Contains(queryComparisonExpressionNode.Operator) ? QueryLogicalExpressionOperator.And : QueryLogicalExpressionOperator.Or),
              Children = (IEnumerable<QueryExpressionNode>) new List<QueryExpressionNode>()
              {
                expressionNode,
                (QueryExpressionNode) ambiguousIdentityNode
              }
            };
          }
          break;
        case QueryLogicalExpressionNode _:
          QueryLogicalExpressionNode logicalExpressionNode1 = expressionNode as QueryLogicalExpressionNode;
          if (logicalExpressionNode1.Children != null && logicalExpressionNode1.Children.Any<QueryExpressionNode>())
          {
            List<KeyValuePair<QueryExpressionNode, QueryExpressionNode>> source1 = new List<KeyValuePair<QueryExpressionNode, QueryExpressionNode>>();
            List<QueryExpressionNode> list1 = logicalExpressionNode1.Children.ToList<QueryExpressionNode>();
            bool flag2 = false;
            if (list1.Count >= 2 && list1.Count<QueryExpressionNode>((Func<QueryExpressionNode, bool>) (child => child is QueryComparisonExpressionNode)) == list1.Count && logicalExpressionNode1.Operator == QueryLogicalExpressionOperator.Or)
            {
              IEnumerable<QueryComparisonExpressionNode> source2 = list1.Where<QueryExpressionNode>((Func<QueryExpressionNode, bool>) (child => child is QueryComparisonExpressionNode)).Cast<QueryComparisonExpressionNode>();
              HashSet<string> nondistinctNames = new HashSet<string>(source2.Where<QueryComparisonExpressionNode>((Func<QueryComparisonExpressionNode, bool>) (child => child.Value != null && !string.IsNullOrEmpty(child.Value.StringValue) && this.IsDistinctNameIdentityField(child))).Select<QueryComparisonExpressionNode, string>((Func<QueryComparisonExpressionNode, string>) (node => IdentityHelper.GetDisplayNameFromDistinctDisplayName(node.Value.StringValue))), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
              if (nondistinctNames.Count > 0 && source2.Count<QueryComparisonExpressionNode>((Func<QueryComparisonExpressionNode, bool>) (child => child.Value != null && nondistinctNames.Contains(child.Value.StringValue))) == nondistinctNames.Count)
                flag2 = true;
            }
            if (!flag2)
            {
              foreach (QueryExpressionNode child in logicalExpressionNode1.Children)
              {
                if (child is QueryComparisonExpressionNode)
                {
                  if ((child as QueryComparisonExpressionNode).Field.IsIdentity)
                  {
                    QueryExpressionNode queryExpressionNode = this.ReplaceDistinctNameNodes(requestContext, child, resolvedNameInfo, notInjectAmbiguousIdentity);
                    if (queryExpressionNode != null)
                      source1.Add(new KeyValuePair<QueryExpressionNode, QueryExpressionNode>(child, queryExpressionNode));
                  }
                }
                else
                  this.ReplaceDistinctNameNodes(requestContext, child, resolvedNameInfo, notInjectAmbiguousIdentity);
              }
            }
            if (source1.Any<KeyValuePair<QueryExpressionNode, QueryExpressionNode>>())
            {
              List<QueryExpressionNode> list2 = logicalExpressionNode1.Children.ToList<QueryExpressionNode>();
              foreach (KeyValuePair<QueryExpressionNode, QueryExpressionNode> keyValuePair in source1)
              {
                int index = list2.IndexOf(keyValuePair.Key);
                if (index != -1)
                {
                  list2.RemoveAt(index);
                  list2.Insert(index, keyValuePair.Value);
                }
              }
              logicalExpressionNode1.Children = (IEnumerable<QueryExpressionNode>) list2;
              break;
            }
            break;
          }
          break;
      }
      return expressionNode;
    }

    private QueryComparisonExpressionNode CreateAmbiguousIdentityNode(
      bool expandConstant,
      FieldEntry field,
      QueryExpressionOperator expressionOperator,
      string value,
      bool isNull)
    {
      return new QueryComparisonExpressionNode()
      {
        ExpandConstant = expandConstant,
        Field = field,
        Operator = expressionOperator,
        Value = new QueryExpressionValue()
        {
          ValueType = QueryExpressionValueType.String,
          StringValue = value,
          IsNull = isNull
        }
      };
    }

    private bool IsDistinctNameIdentityField(
      QueryComparisonExpressionNode queryComparisonExpressionNode)
    {
      if (!queryComparisonExpressionNode.Field.IsIdentity || string.IsNullOrEmpty(queryComparisonExpressionNode.Value.StringValue) || queryComparisonExpressionNode.Value.StringValue.IndexOf(">") != queryComparisonExpressionNode.Value.StringValue.Length - 1)
        return false;
      return queryComparisonExpressionNode.Operator == QueryExpressionOperator.Equals || queryComparisonExpressionNode.Operator == QueryExpressionOperator.NotEquals || queryComparisonExpressionNode.Operator == QueryExpressionOperator.Ever;
    }

    private bool IsBackCompatIdentityQueryExpressionField(
      IVssRequestContext requestContext,
      QueryComparisonExpressionNode queryComparisonExpressionNode)
    {
      return queryComparisonExpressionNode.Field.IsIdentity && !this.IsDistinctNameIdentityField(queryComparisonExpressionNode);
    }

    public virtual QueryExpression ConvertToQueryExpression(
      IVssRequestContext requestContext,
      string wiql,
      Guid projectId,
      WebApiTeam team = null,
      bool dayPrecision = true,
      bool forDisplay = false,
      bool skipWiqlTextLimitValidation = false,
      Guid? queryId = null,
      bool collectMacro = false)
    {
      IDictionary systemQueryContext = this.GetSystemQueryContext(requestContext, projectId);
      if (team != null)
        systemQueryContext[(object) WiqlAdapter.Team] = (object) team.Name;
      return this._ConvertToQueryExpression(requestContext, wiql, systemQueryContext, dayPrecision, forDisplay, skipWiqlTextLimitValidation: skipWiqlTextLimitValidation, queryId: queryId, filterProjectId: new Guid?(projectId), collectMacro: collectMacro);
    }

    public virtual QueryExpression ConvertToQueryExpression(
      IVssRequestContext requestContext,
      string wiql,
      IDictionary context = null,
      bool dayPrecision = true,
      bool forDisplay = false,
      bool skipWiqlTextLimitValidation = false,
      Guid? queryId = null,
      bool collectMacro = false,
      Guid? filterProjectId = null)
    {
      IVssRequestContext requestContext1 = requestContext;
      string wiql1 = wiql;
      IDictionary context1 = context;
      int num1 = dayPrecision ? 1 : 0;
      int num2 = forDisplay ? 1 : 0;
      int num3 = skipWiqlTextLimitValidation ? 1 : 0;
      Guid? queryId1 = queryId;
      bool flag = collectMacro;
      Guid? filterProjectId1 = filterProjectId;
      int num4 = flag ? 1 : 0;
      return this._ConvertToQueryExpression(requestContext1, wiql1, context1, num1 != 0, num2 != 0, skipWiqlTextLimitValidation: num3 != 0, queryId: queryId1, filterProjectId: filterProjectId1, collectMacro: num4 != 0);
    }

    public QueryExpression ValidateWiql(
      IVssRequestContext requestContext,
      string wiql,
      Guid projectId,
      bool dayPrecision = true,
      bool forDisplay = false,
      string teamName = null,
      bool collectMacro = false)
    {
      IDictionary systemQueryContext = this.GetSystemQueryContext(requestContext, projectId, teamName);
      IVssRequestContext requestContext1 = requestContext;
      string wiql1 = wiql;
      IDictionary context = systemQueryContext;
      int num1 = dayPrecision ? 1 : 0;
      int num2 = forDisplay ? 1 : 0;
      Guid? nullable = new Guid?(projectId);
      bool flag = collectMacro;
      Guid? queryId = new Guid?();
      Guid? filterProjectId = nullable;
      int num3 = flag ? 1 : 0;
      return this._ConvertToQueryExpression(requestContext1, wiql1, context, num1 != 0, num2 != 0, true, queryId: queryId, filterProjectId: filterProjectId, collectMacro: num3 != 0);
    }

    public QueryType GetQueryType(IVssRequestContext requestContext, string wiql)
    {
      WiqlAdapter wiqlAdapter = new WiqlAdapter((IWiqlAdapterHelper) new ServerWiqlAdapterHelper(requestContext, validationOnly: true));
      NodeSelect syntax = Parser.ParseSyntax(wiql);
      QueryType queryType = QueryType.WorkItems;
      if (syntax.Mode != null)
      {
        syntax.Mode.Bind((IExternal) wiqlAdapter, syntax.From, (NodeFieldName) null);
        queryType = WorkItemQueryService.GetQueryTypeFromLinkQueryMode((Microsoft.TeamFoundation.WorkItemTracking.Internals.LinkQueryMode) syntax.From.Tag);
      }
      return queryType;
    }

    protected virtual IDictionary GetSystemQueryContext(
      IVssRequestContext requestContext,
      Guid projectId,
      string teamName = null)
    {
      string str = (string) null;
      if (projectId != Guid.Empty)
        str = requestContext.GetService<IProjectService>().GetProjectName(requestContext.Elevate(), projectId);
      Hashtable systemQueryContext = new Hashtable((IEqualityComparer) TFStringComparer.WorkItemQueryText)
      {
        {
          (object) "project",
          (object) str
        },
        {
          (object) "me",
          (object) requestContext.WitContext().RequestIdentity.GetLegacyDistinctDisplayName()
        }
      };
      if (!string.IsNullOrEmpty(teamName))
        systemQueryContext[(object) "team"] = (object) teamName;
      return (IDictionary) systemQueryContext;
    }

    private QueryExpression _ConvertToQueryExpression(
      IVssRequestContext requestContext,
      string wiql,
      IDictionary context = null,
      bool dayPrecision = true,
      bool forDisplay = false,
      bool validationOnly = false,
      bool skipWiqlTextLimitValidation = false,
      Guid? queryId = null,
      Guid? filterProjectId = null,
      bool collectMacro = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<string>(wiql, nameof (wiql));
      bool hasCrossProjectQueryPermission = this.HasCrossProjectQueryPermission(requestContext);
      if (!hasCrossProjectQueryPermission && (!filterProjectId.HasValue || filterProjectId.Value == Guid.Empty))
        throw new WorkItemTrackingQueryCrossProjectPermissionException(nameof (filterProjectId));
      if (!skipWiqlTextLimitValidation)
        WiqlTextHelper.ValidateWiqlTextRequirements(requestContext, wiql);
      QueryExpression qe = new QueryExpression();
      qe.QueryId = queryId;
      qe.Wiql = wiql;
      requestContext.TraceBlock(906007, 906008, "Query", nameof (WorkItemQueryService), "ConvertToQueryExpression", (Action) (() =>
      {
        ProjectInfo project = (ProjectInfo) null;
        WebApiTeam team = (WebApiTeam) null;
        WorkItemQueryService.TryGetProjectAndTeam(requestContext, context, out project, out team);
        ServerWiqlAdapterHelper helper = new ServerWiqlAdapterHelper(requestContext, project, team, validationOnly);
        WiqlAdapter wiqlAdapter = new WiqlAdapter((IWiqlAdapterHelper) helper);
        wiqlAdapter.Context = context;
        wiqlAdapter.DayPrecision = dayPrecision;
        requestContext.Trace(906017, TraceLevel.Verbose, "Query", nameof (WorkItemQueryService), "Parsing WIQL");
        NodeSelect nodeSelect = Parser.ParseSyntax(wiql);
        nodeSelect.Bind((IExternal) wiqlAdapter, (NodeTableName) null, (NodeFieldName) null);
        if (collectMacro && nodeSelect.Where != null)
          qe.MacrosUsed = WiqlUtils.ExtractMacroUsage(nodeSelect.Where);
        if (!forDisplay)
          nodeSelect = (NodeSelect) nodeSelect.Optimize((IExternal) wiqlAdapter, (NodeTableName) null, (NodeFieldName) null);
        qe.DisplayFieldsExplicitlySet = nodeSelect.Fields != null;
        Microsoft.TeamFoundation.WorkItemTracking.Internals.LinkQueryMode tag = (Microsoft.TeamFoundation.WorkItemTracking.Internals.LinkQueryMode) nodeSelect.From.Tag;
        qe.QueryType = WorkItemQueryService.GetQueryTypeFromLinkQueryMode(tag);
        if (tag == Microsoft.TeamFoundation.WorkItemTracking.Internals.LinkQueryMode.LinksRecursiveReturnMatchingChildren || tag == Microsoft.TeamFoundation.WorkItemTracking.Internals.LinkQueryMode.LinksRecursive)
          qe.RecursionOption = tag == Microsoft.TeamFoundation.WorkItemTracking.Internals.LinkQueryMode.LinksRecursiveReturnMatchingChildren ? QueryRecursionOption.ChildFirst : QueryRecursionOption.ParentFirst;
        if (nodeSelect.Where != null && WiqlUtils.ExtractFieldsFromWhere(nodeSelect.Where).ContainsKey(-35))
          qe.IsParentQuery = true;
        DateTime asOfUtc = wiqlAdapter.GetAsOfUtc(nodeSelect);
        if (asOfUtc != DateTime.MinValue)
          qe.AsOfDateTime = new DateTime?(asOfUtc);
        if (qe.QueryType == QueryType.WorkItems)
        {
          qe.LeftGroup = this.GenerateQueryExpressionNode((IWiqlAdapterHelper) helper, nodeSelect.Where, forDisplay: forDisplay);
        }
        else
        {
          Dictionary<string, NodeAndOperator> whereGroups = nodeSelect.GetWhereGroups();
          NodeAndOperator nodeAndOperator1 = (NodeAndOperator) null;
          whereGroups.TryGetValue("Source", out nodeAndOperator1);
          qe.LeftGroup = this.GenerateQueryExpressionNode((IWiqlAdapterHelper) helper, (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) nodeAndOperator1, forDisplay: forDisplay);
          NodeAndOperator nodeAndOperator2 = (NodeAndOperator) null;
          whereGroups.TryGetValue(string.Empty, out nodeAndOperator2);
          if (qe.QueryType == QueryType.LinksRecursiveMayContain)
          {
            Tools.EnsureSyntax(nodeAndOperator2 != null, SyntaxError.TreeQueryNeedsOneLinkType, (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) nodeAndOperator2);
            qe.RecursionLinkTypeId = (short) wiqlAdapter.ComputeLinkTypes((Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) nodeAndOperator2).Keys.First<int>();
            if (forDisplay)
              qe.LinkGroup = this.GenerateQueryExpressionNode((IWiqlAdapterHelper) helper, (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) nodeAndOperator2, forDisplay: forDisplay);
          }
          else
            qe.LinkGroup = this.GenerateQueryExpressionNode((IWiqlAdapterHelper) helper, (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) nodeAndOperator2, forDisplay: forDisplay);
          NodeAndOperator nodeAndOperator3 = (NodeAndOperator) null;
          whereGroups.TryGetValue("Target", out nodeAndOperator3);
          qe.RightGroup = this.GenerateQueryExpressionNode((IWiqlAdapterHelper) helper, (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) nodeAndOperator3, forDisplay: forDisplay);
        }
        qe.DisplayFields = wiqlAdapter.GetDisplayFieldList(nodeSelect).Cast<string>();
        List<object> sortFieldList = wiqlAdapter.GetSortFieldList(nodeSelect);
        requestContext.Trace(906018, TraceLevel.Verbose, "Query", nameof (WorkItemQueryService), "Parsing sort order. Number of sort fields {0}.", (object) sortFieldList.Count);
        bool flag = false;
        List<QuerySortField> querySortFieldList = new List<QuerySortField>(sortFieldList.Count);
        foreach (QuerySortOrderEntry querySortOrderEntry in sortFieldList.Cast<QuerySortOrderEntry>())
        {
          FieldEntry field = (FieldEntry) helper.FindField(querySortOrderEntry.ColumnName, (string) null, (object) null);
          if (field.Usage != InternalFieldUsages.None)
          {
            if (field.Usage != InternalFieldUsages.WorkItemLink)
            {
              querySortFieldList.Add(new QuerySortField()
              {
                Field = field,
                Descending = !querySortOrderEntry.Ascending,
                NullsFirst = querySortOrderEntry.NullsFirst
              });
              if (field.FieldId == -3)
                flag = true;
            }
            else if (qe.QueryType == QueryType.LinksRecursiveMayContain)
              querySortFieldList.Add(new QuerySortField()
              {
                Field = field,
                Descending = !querySortOrderEntry.Ascending,
                NullsFirst = querySortOrderEntry.NullsFirst,
                TableAlias = QueryTableAlias.Link
              });
          }
        }
        if (qe.QueryType.IsOneHopQuery() && qe.QueryType != QueryType.LinksOneHopDoesNotContain)
        {
          if (!flag)
            querySortFieldList.Add(new QuerySortField()
            {
              Field = (FieldEntry) helper.FindField("System.Id", (string) null, (object) null)
            });
          foreach (QuerySortOrderEntry querySortOrderEntry in sortFieldList.Cast<QuerySortOrderEntry>())
          {
            FieldEntry field = (FieldEntry) helper.FindField(querySortOrderEntry.ColumnName, (string) null, (object) null);
            if (field.Usage != InternalFieldUsages.None)
              querySortFieldList.Add(new QuerySortField()
              {
                Field = field,
                Descending = !querySortOrderEntry.Ascending,
                NullsFirst = querySortOrderEntry.NullsFirst,
                TableAlias = field.Usage == InternalFieldUsages.WorkItemLink ? QueryTableAlias.Link : QueryTableAlias.Right
              });
          }
          if (!flag)
            querySortFieldList.Add(new QuerySortField()
            {
              Field = (FieldEntry) helper.FindField("System.Id", (string) null, (object) null),
              TableAlias = QueryTableAlias.Right
            });
        }
        if (!hasCrossProjectQueryPermission)
        {
          int id = requestContext.WitContext().TreeService.GetTreeNode(filterProjectId.Value, string.Empty, TreeStructureType.None).Id;
          switch (qe.QueryType)
          {
            case QueryType.WorkItems:
              WorkItemQueryService.EnsureTopLevelProjectPredicate(qe, true, id, helper);
              break;
            case QueryType.LinksOneHopMustContain:
            case QueryType.LinksOneHopMayContain:
            case QueryType.LinksOneHopDoesNotContain:
            case QueryType.LinksRecursiveMayContain:
              WorkItemQueryService.EnsureTopLevelProjectPredicate(qe, true, id, helper);
              WorkItemQueryService.EnsureTopLevelProjectPredicate(qe, false, id, helper);
              break;
            default:
              throw new NotImplementedException("Unsupported query type");
          }
        }
        requestContext.Trace(906019, TraceLevel.Verbose, "Query", nameof (WorkItemQueryService), "Finished parsing sort order. HasId {0}. Number of sort fields {1}.", (object) flag, (object) querySortFieldList.Count);
        qe.SortFields = (IEnumerable<QuerySortField>) querySortFieldList;
      }));
      return qe;
    }

    private static void EnsureTopLevelProjectPredicate(
      QueryExpression qe,
      bool isLeftGroup,
      int rootAreaId,
      ServerWiqlAdapterHelper helper)
    {
      QueryExpressionNode queryExpressionNode = isLeftGroup ? qe.LeftGroup : qe.RightGroup;
      bool flag = true;
      if (queryExpressionNode is QueryLogicalExpressionNode && (queryExpressionNode as QueryLogicalExpressionNode).Operator == QueryLogicalExpressionOperator.And && (qe.LeftGroup as QueryLogicalExpressionNode).Children.FirstOrDefault<QueryExpressionNode>() is QueryComparisonExpressionNode comparisonExpressionNode && comparisonExpressionNode.Operator == QueryExpressionOperator.Under && comparisonExpressionNode.Field.ReferenceName == "System.AreaId" && comparisonExpressionNode.Value.NumberValue == rootAreaId)
        flag = false;
      if (!flag)
        return;
      QueryLogicalExpressionNode logicalExpressionNode = new QueryLogicalExpressionNode()
      {
        Operator = QueryLogicalExpressionOperator.And,
        Children = (IEnumerable<QueryExpressionNode>) new List<QueryExpressionNode>(2)
        {
          (QueryExpressionNode) new QueryComparisonExpressionNode()
          {
            Field = (FieldEntry) helper.FindField("System.AreaId", (string) null, (object) null),
            Operator = QueryExpressionOperator.Under,
            Value = new QueryExpressionValue()
            {
              ValueType = QueryExpressionValueType.Number,
              NumberValue = rootAreaId,
              IsNull = false
            }
          }
        }
      };
      if (queryExpressionNode != null)
        ((List<QueryExpressionNode>) logicalExpressionNode.Children).Add(queryExpressionNode);
      if (isLeftGroup)
        qe.LeftGroup = (QueryExpressionNode) logicalExpressionNode;
      else
        qe.RightGroup = (QueryExpressionNode) logicalExpressionNode;
    }

    private static QueryType GetQueryTypeFromLinkQueryMode(Microsoft.TeamFoundation.WorkItemTracking.Internals.LinkQueryMode mode)
    {
      QueryType fromLinkQueryMode;
      switch (mode)
      {
        case Microsoft.TeamFoundation.WorkItemTracking.Internals.LinkQueryMode.LinksMustContain:
          fromLinkQueryMode = QueryType.LinksOneHopMustContain;
          break;
        case Microsoft.TeamFoundation.WorkItemTracking.Internals.LinkQueryMode.LinksMayContain:
          fromLinkQueryMode = QueryType.LinksOneHopMayContain;
          break;
        case Microsoft.TeamFoundation.WorkItemTracking.Internals.LinkQueryMode.LinksDoesNotContain:
          fromLinkQueryMode = QueryType.LinksOneHopDoesNotContain;
          break;
        case Microsoft.TeamFoundation.WorkItemTracking.Internals.LinkQueryMode.LinksRecursive:
        case Microsoft.TeamFoundation.WorkItemTracking.Internals.LinkQueryMode.LinksRecursiveReturnMatchingChildren:
          fromLinkQueryMode = QueryType.LinksRecursiveMayContain;
          break;
        default:
          fromLinkQueryMode = QueryType.WorkItems;
          break;
      }
      return fromLinkQueryMode;
    }

    private static bool TryGetProjectAndTeam(
      IVssRequestContext requestContext,
      IDictionary context,
      out ProjectInfo project,
      out WebApiTeam team)
    {
      project = (ProjectInfo) null;
      team = (WebApiTeam) null;
      string projectName;
      if (context != null && !string.IsNullOrWhiteSpace(projectName = (string) context[(object) WiqlAdapter.Project]))
      {
        IProjectService service1 = requestContext.GetService<IProjectService>();
        try
        {
          project = service1.GetProject(requestContext.Elevate(), projectName);
        }
        catch (ProjectDoesNotExistWithNameException ex)
        {
          requestContext.Trace(906025, TraceLevel.Verbose, "Query", nameof (WorkItemQueryService), "ProjectDoesNotExistWithNameException {0}.", (object) projectName);
          return false;
        }
        string teamIdOrName;
        if (project != null && !string.IsNullOrWhiteSpace(teamIdOrName = (string) context[(object) WiqlAdapter.Team]))
        {
          ITeamService service2 = requestContext.GetService<ITeamService>();
          team = service2.GetTeamInProject(requestContext, project.Id, teamIdOrName);
        }
      }
      return true;
    }

    private QueryExpressionNode GenerateQueryExpressionNode(
      IWiqlAdapterHelper helper,
      Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node,
      bool not = false,
      bool ever = false,
      bool forDisplay = false)
    {
      if (node == null)
        return (QueryExpressionNode) null;
      switch (node.NodeType)
      {
        case NodeType.FieldCondition:
        case NodeType.BoolConst:
          NodeValueList valueList = (NodeValueList) null;
          bool flag1 = false;
          bool flag2 = false;
          bool isArithmetic = false;
          object obj;
          QueryExpressionOperator expressionOperator;
          NodeItem node1;
          if (node.NodeType == NodeType.BoolConst)
          {
            obj = helper.FindField("System.Id", (string) null, (object) null);
            expressionOperator = ((NodeBoolConst) node).Value == not ? QueryExpressionOperator.Equals : QueryExpressionOperator.NotEquals;
            node1 = (NodeItem) new NodeNumber("0");
          }
          else
          {
            NodeCondition nodeCondition = (NodeCondition) node;
            if (nodeCondition.Right is NodeValueList)
            {
              valueList = this.ConvertListWithArithmetictoValueList((NodeValueList) nodeCondition.Right, ref isArithmetic);
              node1 = (NodeItem) null;
              flag2 = true;
            }
            else if (nodeCondition.Right is NodeArithmetic)
            {
              NodeArithmetic right = (NodeArithmetic) nodeCondition.Right;
              valueList = new NodeValueList();
              valueList.Add(right.Left);
              valueList.Add((Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) new NodeString(this.ConvertArithmeticToString(right.Arithmetic)));
              valueList.Add(right.Right);
              isArithmetic = true;
              node1 = (NodeItem) null;
              flag2 = true;
            }
            else
              node1 = (NodeItem) nodeCondition.Right;
            obj = nodeCondition.Left.Tag;
            int fieldId = ((FieldEntry) obj).FieldId;
            if (fieldId == -7 && !forDisplay)
            {
              obj = helper.FindField("System.AreaId", (string) null, (object) null);
              node1 = (NodeItem) new NodeNumber(helper.GetTreeID(node1.ConstStringValue, TreeStructureType.Area).ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
            }
            else if (fieldId == -105 && !forDisplay)
            {
              obj = helper.FindField("System.IterationId", (string) null, (object) null);
              node1 = (NodeItem) new NodeNumber(helper.GetTreeID(node1.ConstStringValue, TreeStructureType.Iteration).ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
            }
            else if (fieldId == 100)
            {
              if (!flag2 && !string.IsNullOrEmpty(node1.ConstStringValue))
              {
                Tools.EnsureSyntax(helper.HasLinkType(node1.ConstStringValue), SyntaxError.InvalidLinkTypeName, (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) node1);
                node1 = (NodeItem) new NodeNumber(helper.GetLinkTypeId(node1.ConstStringValue).ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
              }
              else if (flag2)
              {
                NodeValueList nodeValueList = new NodeValueList();
                foreach (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node2 in (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) valueList)
                {
                  NodeString nodeString = node2 as NodeString;
                  Tools.EnsureSyntax(nodeString != null, SyntaxError.InvalidLinkTypeName, node2);
                  Tools.EnsureSyntax(helper.HasLinkType(nodeString.ConstStringValue), SyntaxError.InvalidLinkTypeName, node2);
                  nodeValueList.Add((Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) new NodeNumber(helper.GetLinkTypeId(nodeString.ConstStringValue).ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)));
                }
                valueList = nodeValueList;
              }
            }
            if (fieldId == 54)
              ever = true;
            if (nodeCondition.Condition == Condition.Group)
              flag1 = true;
            expressionOperator = this.GetQueryExpressionOperator((int) nodeCondition.Condition, not, ever);
          }
          return (QueryExpressionNode) new QueryComparisonExpressionNode()
          {
            Field = (FieldEntry) obj,
            Operator = expressionOperator,
            Value = (flag2 ? this.GetQueryExpressionArrayValue(valueList, ((FieldEntry) obj).FieldType, isArithmetic) : this.GetQueryExpressionValue(node1, ((FieldEntry) obj).FieldType)),
            ExpandConstant = flag1
          };
        case NodeType.Not:
          return this.GenerateQueryExpressionNode(helper, ((NodeNotOperator) node).Value, !not, ever, forDisplay);
        case NodeType.Ever:
          return this.GenerateQueryExpressionNode(helper, ((NodeEverOperator) node).Value, not, true, forDisplay);
        case NodeType.And:
        case NodeType.Or:
          QueryExpressionNode[] queryExpressionNodeArray = new QueryExpressionNode[node.Count];
          for (int i = 0; i < node.Count; ++i)
            queryExpressionNodeArray[i] = this.GenerateQueryExpressionNode(helper, node[i], not, ever, forDisplay);
          return (QueryExpressionNode) new QueryLogicalExpressionNode()
          {
            Operator = (node.NodeType == NodeType.Or == not ? QueryLogicalExpressionOperator.And : QueryLogicalExpressionOperator.Or),
            Children = (IEnumerable<QueryExpressionNode>) queryExpressionNodeArray
          };
        default:
          throw new NotSupportedException();
      }
    }

    private QueryExpressionValue GetQueryExpressionArrayValue(
      NodeValueList valueList,
      InternalFieldType type,
      bool isArithmetic)
    {
      if (valueList.Count <= 0)
        return (QueryExpressionValue) null;
      QueryExpressionValue[] queryExpressionValueArray = new QueryExpressionValue[valueList.Count];
      for (int i = 0; i < valueList.Count; ++i)
      {
        NodeItem node = (NodeItem) valueList[i];
        InternalFieldType type1 = isArithmetic ? this.ConvertToInternalFieldType(node.DataType) : type;
        if (node.NodeType == NodeType.Variable)
        {
          queryExpressionValueArray[i] = new QueryExpressionValue()
          {
            IsNull = string.IsNullOrWhiteSpace(node.Value),
            IsVariable = node.NodeType == NodeType.Variable,
            StringValue = node.Value
          };
          WorkItemQueryService.AppendVariableArgumentString(node, queryExpressionValueArray[i]);
        }
        else
          queryExpressionValueArray[i] = this.GetQueryExpressionValue(node, type1);
      }
      QueryExpressionValue arrayValue = QueryExpressionValue.CreateArrayValue(queryExpressionValueArray);
      arrayValue.IsArithmetic = isArithmetic;
      return arrayValue;
    }

    private NodeValueList ConvertListWithArithmetictoValueList(
      NodeValueList valueList,
      ref bool isArithmetic)
    {
      NodeValueList nodeValueList = new NodeValueList();
      for (int i = 0; i < valueList.Count; ++i)
      {
        if (valueList[i] is NodeArithmetic)
        {
          NodeArithmetic nodeArithmetic = valueList[i] as NodeArithmetic;
          nodeValueList.Add(nodeArithmetic.Left);
          nodeValueList.Add((Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) new NodeString(this.ConvertArithmeticToString(nodeArithmetic.Arithmetic)));
          nodeValueList.Add(nodeArithmetic.Right);
          isArithmetic = true;
        }
        else
          nodeValueList.Add(valueList[i]);
      }
      return nodeValueList;
    }

    private InternalFieldType ConvertToInternalFieldType(DataType nodeDataType)
    {
      switch (nodeDataType)
      {
        case DataType.Numeric:
          return InternalFieldType.Integer;
        case DataType.Date:
          return InternalFieldType.DateTime;
        case DataType.String:
          return InternalFieldType.String;
        case DataType.Guid:
          return InternalFieldType.Guid;
        default:
          throw new NotImplementedException();
      }
    }

    private string ConvertArithmeticToString(Arithmetic arithmetic)
    {
      if (arithmetic == Arithmetic.Add)
        return "+";
      if (arithmetic == Arithmetic.Subtract)
        return "-";
      throw new NotImplementedException();
    }

    private QueryExpressionValue GetQueryExpressionValue(NodeItem node, InternalFieldType type)
    {
      if (node == null)
        return new QueryExpressionValue();
      if (node.NodeType == NodeType.FieldName)
        return new QueryExpressionValue()
        {
          ValueType = QueryExpressionValueType.Column,
          IsNull = false,
          ColumnValue = (FieldEntry) ((NodeFieldName) node).Tag,
          IsVariable = false
        };
      QueryExpressionValue queryExpressionValue = new QueryExpressionValue()
      {
        IsNull = string.IsNullOrWhiteSpace(node.Value),
        IsVariable = node.NodeType == NodeType.Variable,
        StringValue = node.Value
      };
      WorkItemQueryService.AppendVariableArgumentString(node, queryExpressionValue);
      switch (type)
      {
        case InternalFieldType.String:
        case InternalFieldType.PlainText:
        case InternalFieldType.Html:
        case InternalFieldType.TreePath:
        case InternalFieldType.History:
          queryExpressionValue.ValueType = QueryExpressionValueType.String;
          if (!queryExpressionValue.IsNull)
          {
            queryExpressionValue.StringValue = node.Value;
            break;
          }
          break;
        case InternalFieldType.Integer:
          queryExpressionValue.ValueType = QueryExpressionValueType.Number;
          if (!queryExpressionValue.IsNull)
          {
            int result;
            if (!int.TryParse(node.Value, NumberStyles.Integer, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result))
              throw new ArgumentException(ServerResources.QueryInvalidNumberValueException(), "query");
            queryExpressionValue.NumberValue = result;
            break;
          }
          break;
        case InternalFieldType.DateTime:
          queryExpressionValue.ValueType = QueryExpressionValueType.DateTime;
          if (!queryExpressionValue.IsNull)
          {
            if (node.Value.Equals("today", StringComparison.InvariantCultureIgnoreCase) && node.NodeType == NodeType.Variable)
            {
              DateTime localTime = TimeZone.CurrentTimeZone.ToLocalTime(DateTime.UtcNow);
              node.Value = XmlConvert.ToString(localTime.Date, XmlDateTimeSerializationMode.Local);
            }
            if (node.NodeType != NodeType.Variable)
            {
              DateTime result;
              if (!DateTime.TryParse(node.Value, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out result))
                throw new ArgumentException(ServerResources.QueryInvalidDateValueException(), "query");
              queryExpressionValue.DateValue = result;
              break;
            }
            break;
          }
          break;
        case InternalFieldType.Double:
          queryExpressionValue.ValueType = QueryExpressionValueType.Double;
          if (!queryExpressionValue.IsNull)
          {
            double result;
            if (!double.TryParse(node.Value, NumberStyles.Number, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result))
              throw new ArgumentException(ServerResources.QueryInvalidDoubleValueException(), "query");
            queryExpressionValue.DoubleValue = result;
            break;
          }
          break;
        case InternalFieldType.Guid:
          if (!queryExpressionValue.IsNull)
          {
            Guid result;
            if (Guid.TryParse(node.Value, out result))
            {
              queryExpressionValue.GuidValue = result;
              queryExpressionValue.ValueType = QueryExpressionValueType.UniqueIdentifier;
              break;
            }
            queryExpressionValue.StringValue = node.Value;
            queryExpressionValue.ValueType = QueryExpressionValueType.String;
            break;
          }
          break;
        case InternalFieldType.Boolean:
          queryExpressionValue.ValueType = QueryExpressionValueType.Boolean;
          if (!queryExpressionValue.IsNull)
          {
            if (node is NodeBoolValue)
            {
              queryExpressionValue.BoolValue = ((NodeBoolValue) node).BoolValue;
              break;
            }
            bool result;
            if (!bool.TryParse(node.Value, out result))
              throw new ArgumentException(ServerResources.QueryInvalidBooleanValueException(), "query");
            queryExpressionValue.BoolValue = result;
            break;
          }
          break;
        default:
          throw new NotSupportedException();
      }
      return queryExpressionValue;
    }

    private static void AppendVariableArgumentString(NodeItem node, QueryExpressionValue value)
    {
      if (!(node is NodeVariable nodeVariable) || nodeVariable?.Parameters?.Arguments == null || nodeVariable.Parameters.Arguments.Count <= 0)
        return;
      StringBuilder builder = new StringBuilder();
      IList<NodeItem> arguments = nodeVariable.Parameters.Arguments;
      builder.Append("(");
      for (int index = 0; index < arguments.Count; ++index)
      {
        arguments[index].AppendTo(builder);
        if (index + 1 < arguments.Count)
          builder.Append(", ");
      }
      builder.Append(")");
      value.ArgumentsString = builder.ToString();
    }

    private QueryExpressionOperator GetQueryExpressionOperator(int c, bool not, bool ever)
    {
      int num = WorkItemQueryService.s_operators[c * 4 + (ever ? 2 : 0) + (not ? 1 : 0)];
      return num >= 0 ? (QueryExpressionOperator) num : throw new NotSupportedException();
    }

    protected virtual IPermissionCheckHelper GetPermissionCheckHelper(
      IVssRequestContext requestContext)
    {
      return (IPermissionCheckHelper) new PermissionCheckHelper(requestContext);
    }

    public virtual bool HasCrossProjectQueryPermission(IVssRequestContext requestContext) => CommonWITUtils.HasCrossProjectQueryPermission(requestContext);

    internal virtual QueryProcessor GetQueryProcessor(IVssRequestContext requestContext) => new QueryProcessor(requestContext);

    private class OneHopLinkInfo
    {
      public Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.LinkQueryResultEntry FirstTarget;
      public int TargetCount;

      public OneHopLinkInfo(Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.LinkQueryResultEntry resultEntry) => this.FirstTarget = resultEntry;
    }

    private class OneHopLinkQueryResultComparer : IComparer<Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.LinkQueryResultEntry>
    {
      public int Compare(Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.LinkQueryResultEntry a, Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.LinkQueryResultEntry b)
      {
        int num = a.SourceId - b.SourceId;
        if (num == 0)
          num = a.TargetId - b.TargetId;
        return num;
      }
    }

    private class TreeLinkQueryResultComparer : IComparer<Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.LinkQueryResultEntry>
    {
      public int Compare(Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.LinkQueryResultEntry a, Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.LinkQueryResultEntry b) => a.TargetId - b.TargetId;
    }
  }
}
