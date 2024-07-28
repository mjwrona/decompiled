// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.QueryTelemetry
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class QueryTelemetry : WorkItemTrackingTelemetry
  {
    private const string c_thresholdRegistryPath = "/Service/WorkItemTracking/Settings/TraceQueryAboveThreshold";
    private const int c_defaultThresholdTime = 0;
    private static IReadOnlyCollection<QueryExpressionOperator> containsOperators = (IReadOnlyCollection<QueryExpressionOperator>) new HashSet<QueryExpressionOperator>()
    {
      QueryExpressionOperator.Contains,
      QueryExpressionOperator.Contains,
      QueryExpressionOperator.NotContains,
      QueryExpressionOperator.ContainsWords,
      QueryExpressionOperator.NotContainsWords,
      QueryExpressionOperator.EverContains,
      QueryExpressionOperator.NeverContains,
      QueryExpressionOperator.EverContainsWords,
      QueryExpressionOperator.NeverContainsWords,
      QueryExpressionOperator.FTContains,
      QueryExpressionOperator.NotFTContains,
      QueryExpressionOperator.EverFTContains,
      QueryExpressionOperator.NeverFTContains
    };
    private static IReadOnlyCollection<QueryExpressionOperator> inOperators = (IReadOnlyCollection<QueryExpressionOperator>) new HashSet<QueryExpressionOperator>()
    {
      QueryExpressionOperator.In,
      QueryExpressionOperator.NotIn
    };

    public QueryTelemetry(IVssRequestContext requestContext, string feature)
      : base(requestContext, feature, "/Service/WorkItemTracking/Settings/TraceQueryAboveThreshold", 0)
    {
      long num1;
      if (requestContext.TryGetItem<long>("TransformNamesToIdsTime", out num1))
        this.ClientTraceData.Add("TransformNamesToIdsTime", (object) num1);
      string str;
      if (requestContext.TryGetItem<string>("QueryOptimizationsFromRest", out str))
        this.ClientTraceData.Add("QueryOptimizationsFromRest", (object) str);
      Dictionary<string, int> dictionary;
      if (requestContext.TryGetItem<Dictionary<string, int>>("ExpandIdentityCount", out dictionary))
        this.ClientTraceData.Add("ExpandIdentityCount", (object) dictionary);
      long num2;
      if (requestContext.TryGetItem<long>("ExpandIdentityDuration", out num2))
        this.ClientTraceData.Add("ExpandIdentityDuration", (object) num2);
      this.ClientTraceData.Add("Method", (object) requestContext.Method?.Name);
    }

    protected QueryTelemetry(
      IVssRequestContext requestContext,
      string feature,
      string registryPath,
      int defaultThresholdTime)
      : base(requestContext, feature, registryPath, defaultThresholdTime)
    {
    }

    public static string Feature => "Query";

    public override void AddData(params object[] param) => this.AddDataImpl(param.Length != 0 ? param[0] as QueryExpression : (QueryExpression) null, param.Length > 1 ? param[1] : (object) null, param.Length > 2 ? param[2].ToString() : (string) null, param.Length > 3 ? param[3].ToString() : (string) null, param.Length > 4 ? param[4].ToString() : (string) null, param.Length > 5 ? param[5] as QueryOptimizationInstance : (QueryOptimizationInstance) null, param.Length > 6 ? (long) param[6] : -1L, param.Length > 7 ? param[7] as HashSet<string> : (HashSet<string>) null, param.Length > 8 ? param[8] as ApplicationIntent? : new ApplicationIntent?(), param.Length > 9 ? param[9] as int? : new int?());

    private void AddDataImpl(
      QueryExpression queryExpression,
      object resultOrException,
      string querySource,
      string queryExperiment,
      string queryCategory,
      QueryOptimizationInstance optimizationInstance,
      long elapsedTimeInMs,
      HashSet<string> fieldsDoStringComparison,
      ApplicationIntent? applicationIntent,
      int? topParam)
    {
      if (queryExpression != null)
        this.AddQueryExpressionData(queryExpression);
      if (resultOrException is QueryResult queryResult)
        this.ClientTraceData.Add("ResultCount", (object) queryResult.Count);
      else if (resultOrException is Exception exception)
        this.ClientTraceData.Add("Exception", (object) (exception.GetType().Name + ": " + exception.Message));
      if (querySource != null)
        this.ClientTraceData.Add("QuerySource", (object) querySource);
      if (queryExperiment != null)
        this.ClientTraceData.Add("QueryExperiment", (object) queryExperiment);
      if (queryCategory != null)
        this.ClientTraceData.Add("QueryCategory", (object) queryCategory);
      if (optimizationInstance != null)
        this.AddOptimizationInstanceData(optimizationInstance);
      if (elapsedTimeInMs >= 0L)
        this.ClientTraceData.Add("ElapsedTimeInMs", (object) elapsedTimeInMs);
      if (fieldsDoStringComparison != null && fieldsDoStringComparison.Count > 0)
        this.ClientTraceData.Add("FieldsDoStringComparison", (object) fieldsDoStringComparison);
      if (applicationIntent.HasValue)
        this.ClientTraceData.Add("ApplicationIntent", (object) applicationIntent);
      if (!topParam.HasValue)
        return;
      this.ClientTraceData.Add("TopParameter", (object) topParam);
    }

    private void AddQueryExpressionData(QueryExpression queryExpression)
    {
      this.ClientTraceData.Add("QueryId", (object) queryExpression.QueryId.GetValueOrDefault());
      this.ClientTraceData.Add("QueryType", (object) queryExpression.QueryType.ToString());
      if (queryExpression.AsOfDateTime.HasValue)
        this.ClientTraceData.Add("AsOfDateTime", (object) queryExpression.AsOfDateTime);
      string str = string.Empty;
      if (queryExpression.SortFields != null && queryExpression.SortFields.Any<QuerySortField>())
        str = string.Join(",", queryExpression.SortFields.Select<QuerySortField, string>((Func<QuerySortField, string>) (f => f.Field.ReferenceName)).ToArray<string>());
      this.ClientTraceData.Add("SortFields", (object) str);
      this.ClientTraceData.Add("IsQueryingOnTags", (object) queryExpression.IsQueryingOnTags);
      this.ClientTraceData.Add("QueryHash", (object) queryExpression.QueryHash);
      this.ClientTraceData.Add("QueryOptimizations", (object) queryExpression.Optimizations.ToString());
      this.ClientTraceData.Add("MacrosUsed", (object) queryExpression.GetStringForMacrosUsed());
      this.ClientTraceData.Add("OptimizationSource", (object) queryExpression.OptimizationSource.ToString());
      this.ClientTraceData.Add("RecursionOption", (object) queryExpression.RecursionOption.ToString());
      this.ClientTraceData.Add("RecursionLinkTypeId", (object) queryExpression.RecursionLinkTypeId);
      this.ClientTraceData.Add("IsInGroupIdentityQuery", (object) queryExpression.IsInGroupIdentityQuery);
      this.ClientTraceData.Add("WiqlLength", (object) queryExpression.Wiql.Length);
      this.ClientTraceData.Add("IsParentQuery", (object) queryExpression.IsParentQuery);
      this.AddQueryNodeData(queryExpression.LeftGroup, "Left");
      this.AddQueryNodeData(queryExpression.RightGroup, "Right");
      this.AddQueryNodeData(queryExpression.LinkGroup, "Link");
    }

    private void AddQueryNodeData(QueryExpressionNode node, string qualifier)
    {
      if (node == null)
      {
        this.ClientTraceData.Add("Has" + qualifier + "Node", (object) false);
      }
      else
      {
        this.ClientTraceData.Add("Has" + qualifier + "Node", (object) true);
        QueryExpressionStatistics stats = new QueryExpressionStatistics();
        this.ExtractDataFromNode(node, 0, stats);
        this.ClientTraceData.Add(qualifier + "NodeFieldsReferenced", (object) string.Join(",", (IEnumerable<string>) stats.fieldsReferenced));
        if (stats.nodeDepths.Count > 0)
        {
          this.ClientTraceData.Add(qualifier + "NodeMeanDepth", (object) QueryTelemetry.Mean(stats.nodeDepths));
          this.ClientTraceData.Add(qualifier + "NodeMedianDepth", (object) QueryTelemetry.Median(stats.nodeDepths));
          this.ClientTraceData.Add(qualifier + "NodeMaxDepth", (object) QueryTelemetry.Max(stats.nodeDepths));
        }
        int count1 = stats.orExpressionChildCounts.Count;
        this.ClientTraceData.Add(qualifier + "NodeOrCount", (object) count1);
        if (count1 > 0)
        {
          this.ClientTraceData.Add(qualifier + "NodeOrMeanChildCount", (object) QueryTelemetry.Mean(stats.orExpressionChildCounts));
          this.ClientTraceData.Add(qualifier + "NodeOrMedianChildCount", (object) QueryTelemetry.Median(stats.orExpressionChildCounts));
          this.ClientTraceData.Add(qualifier + "NodeOrMaxChildCount", (object) QueryTelemetry.Max(stats.orExpressionChildCounts));
        }
        int count2 = stats.andExpressionChildCounts.Count;
        this.ClientTraceData.Add(qualifier + "NodeAndCount", (object) count2);
        if (count2 > 0)
        {
          this.ClientTraceData.Add(qualifier + "NodeAndMeanChildCount", (object) QueryTelemetry.Mean(stats.andExpressionChildCounts));
          this.ClientTraceData.Add(qualifier + "NodeAndMedianChildCount", (object) QueryTelemetry.Median(stats.andExpressionChildCounts));
          this.ClientTraceData.Add(qualifier + "NodeAndMaxChildCount", (object) QueryTelemetry.Max(stats.andExpressionChildCounts));
        }
        if (stats.containsValueLengths.Count > 0)
        {
          this.ClientTraceData.Add(qualifier + "NodeMeanContainsValueLength", (object) QueryTelemetry.Mean(stats.containsValueLengths));
          this.ClientTraceData.Add(qualifier + "NodeMedianContainsValueLength", (object) QueryTelemetry.Median(stats.containsValueLengths));
          this.ClientTraceData.Add(qualifier + "NodeMaxContainsValueLength", (object) QueryTelemetry.Max(stats.containsValueLengths));
        }
        if (stats.inValueLengths.Count > 0)
        {
          this.ClientTraceData.Add(qualifier + "NodeMeanInValueLength", (object) QueryTelemetry.Mean(stats.inValueLengths));
          this.ClientTraceData.Add(qualifier + "NodeMedianInValueLength", (object) QueryTelemetry.Median(stats.inValueLengths));
          this.ClientTraceData.Add(qualifier + "NodeMaxInValueLength", (object) QueryTelemetry.Max(stats.inValueLengths));
        }
        foreach (QueryExpressionOperator key in stats.operatorCounts.Keys)
          this.ClientTraceData.Add(string.Format("{0}NodeOperator{1}Count", (object) qualifier, (object) key), (object) stats.operatorCounts[key]);
      }
    }

    private void ExtractDataFromNode(
      QueryExpressionNode node,
      int depth,
      QueryExpressionStatistics stats)
    {
      QueryLogicalExpressionNode node1 = node as QueryLogicalExpressionNode;
      QueryComparisonExpressionNode node2 = node as QueryComparisonExpressionNode;
      if (node1 != null)
      {
        this.ExtractDataFromLogicalNode(node1, depth, stats);
      }
      else
      {
        if (node2 == null)
          return;
        this.ExtractDataFromComparisonNode(node2, depth, stats);
      }
    }

    private void ExtractDataFromLogicalNode(
      QueryLogicalExpressionNode node,
      int depth,
      QueryExpressionStatistics stats)
    {
      stats.nodeDepths.Add(depth);
      int num = node.Children.Count<QueryExpressionNode>();
      if (node.Operator == QueryLogicalExpressionOperator.And)
        stats.andExpressionChildCounts.Add(num);
      else if (node.Operator == QueryLogicalExpressionOperator.Or)
        stats.orExpressionChildCounts.Add(num);
      foreach (QueryExpressionNode child in node.Children)
        this.ExtractDataFromNode(child, depth + 1, stats);
    }

    private void ExtractDataFromComparisonNode(
      QueryComparisonExpressionNode node,
      int depth,
      QueryExpressionStatistics stats)
    {
      stats.nodeDepths.Add(depth);
      stats.fieldsReferenced.Add(node.Field.ReferenceName);
      if (node.Value.ValueType == QueryExpressionValueType.Column && node.Value?.ColumnValue?.ReferenceName != null)
        stats.fieldsReferenced.Add(node.Value.ColumnValue.ReferenceName);
      if (QueryTelemetry.containsOperators.Contains<QueryExpressionOperator>(node.Operator) && node.Value?.StringValue != null)
        stats.containsValueLengths.Add(node.Value.StringValue.Length);
      if (QueryTelemetry.inOperators.Contains<QueryExpressionOperator>(node.Operator) && node.Value.ValueType == QueryExpressionValueType.Array)
        stats.inValueLengths.Add(node.Value.GetArrayValue().Length);
      stats.operatorCounts.AddOrUpdate<QueryExpressionOperator, int>(node.Operator, 1, (Func<QueryExpressionOperator, int, int>) ((key, count) => ++count));
    }

    private void AddOptimizationInstanceData(QueryOptimizationInstance optimizationInstance)
    {
      this.ClientTraceData.Add("OptimizationState", (object) optimizationInstance.OptimizationState.ToString());
      this.ClientTraceData.Add("LastStateChangeTime", (object) optimizationInstance.LastStateChangeTime);
      this.ClientTraceData.Add("Index", (object) optimizationInstance.StrategyIndex);
      this.ClientTraceData.Add("SlownessThresholdInMsFromHistory", (object) optimizationInstance.SlownessThresholdInMsFromHistory);
      this.ClientTraceData.Add("NormalRunCountInCurrentOpt", (object) optimizationInstance.NormalRunCountInCurrentOpt);
      this.ClientTraceData.Add("SlowRunCountInCurrentOpt", (object) optimizationInstance.SlowRunCountInCurrentOpt);
      this.ClientTraceData.Add("DeltaNormalRunCount", (object) optimizationInstance.DeltaNormalRunCount);
      this.ClientTraceData.Add("DeltaSlowRunCount", (object) optimizationInstance.DeltaSlowRunCount);
      this.ClientTraceData.Add("IsNormalRunCountReset", (object) optimizationInstance.IsNormalRunCountReset);
      this.ClientTraceData.Add("IsSlowRunCountReset", (object) optimizationInstance.IsSlowRunCountReset);
      this.ClientTraceData.Add("IsStateForkedFromOtherInstance", (object) optimizationInstance.IsStateForkedFromOtherInstance);
    }

    private static float Mean(IList<int> entries)
    {
      if (entries.Count == 0)
        throw new ArgumentException("Cannot find the mean of an empty list");
      return (float) entries.Aggregate<int>((Func<int, int, int>) ((a, b) => a + b)) / (float) entries.Count;
    }

    private static float Median(IList<int> entries)
    {
      List<int> intList = entries.Count != 0 ? entries.OrderBy<int, int>((Func<int, int>) (v => v)).ToList<int>() : throw new ArgumentException("Cannot find the median of an empty list");
      int index = intList.Count / 2;
      return intList.Count % 2 == 1 ? (float) intList[index] : (float) (intList[index] + intList[index - 1]) / 2f;
    }

    private static int Max(IList<int> entries) => entries.Count != 0 ? entries.Max() : throw new ArgumentException("Cannot find the max of an empty list");
  }
}
