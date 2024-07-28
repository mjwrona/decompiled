// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Query.QueryProcessor.QueryPredicateTag
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Devops.Tags.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Query.QueryProcessor
{
  internal class QueryPredicateTag
  {
    private QueryProcessorContext _context;
    private string _workItemsTableAlias;
    private QueryComparisonExpressionNode _expressionNode;
    private SqlStatement _statement;

    private QueryPredicateTag(
      QueryProcessorContext context,
      string workItemsTableAlias,
      QueryComparisonExpressionNode expressionNode,
      SqlStatement statement)
    {
      this._context = context;
      this._workItemsTableAlias = workItemsTableAlias;
      this._expressionNode = expressionNode;
      this._statement = statement;
    }

    public static void AppendTagPredicate(
      QueryProcessorContext context,
      string workItemsTableAlias,
      QueryComparisonExpressionNode expressionNode,
      SqlStatement statement)
    {
      new QueryPredicateTag(context, workItemsTableAlias, expressionNode, statement).AppendTagPredicate();
    }

    private void AppendTagPredicate() => this._context.m_requestContext.TraceBlock(906029, 906030, "Query", "QueryProcessor", nameof (AppendTagPredicate), (Action) (() =>
    {
      this.Validate();
      this.AppendTempTableResults(this.CreateTempTable());
    }));

    private void Validate()
    {
      if (!this._context.m_requestContext.GetService<ITeamFoundationTaggingService>().DatabaseSupportsGetTaggedArtifactsFunctions(this._context.m_requestContext))
        throw new WorkItemTrackingQueryException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.CannotQueryOnTags());
    }

    private int CreateTempTable()
    {
      string namesStringTable = this.CreateTagNamesStringTable();
      int tempTableId;
      if (QueryPredicateCommon.GetTempTableId(this._context, this._expressionNode, out tempTableId))
      {
        string workItemArtifactKindGuid = WorkItemArtifactKinds.WorkItem.ToString();
        if (string.IsNullOrEmpty(this._context.m_asOfDatesParam))
          this.CreateSingleDateTable(tempTableId, namesStringTable, workItemArtifactKindGuid);
        else
          this.CreateMultipleDateTable(tempTableId, namesStringTable, workItemArtifactKindGuid);
      }
      return tempTableId;
    }

    private string CreateTagNamesStringTable()
    {
      IEnumerable<string> strings;
      switch (this._expressionNode.Value.ValueType)
      {
        case QueryExpressionValueType.Array:
          strings = (IEnumerable<string>) ((IEnumerable<QueryExpressionValue>) this._expressionNode.Value.GetArrayValue()).Select<QueryExpressionValue, string>((Func<QueryExpressionValue, string>) (x => x.StringValue)).ToHashSet<string>();
          break;
        default:
          strings = (IEnumerable<string>) new string[1]
          {
            this._expressionNode.Value.StringValue
          };
          break;
      }
      return QueryParameter.DefineTableValuedParameter<string>(this._context, (WorkItemTrackingTableValueParameter<string>) new Microsoft.TeamFoundation.WorkItemTracking.Server.StringTable(strings));
    }

    private void CreateSingleDateTable(
      int tempTableId,
      string tagNameStringTable,
      string workItemArtifactKindGuid)
    {
      string str = this._context.m_asOfParam ?? "NULL";
      this._context.m_queryText.Append(string.Format("\r\nCREATE TABLE #temp{0}\r\n(\r\n    id INT NOT NULL,\r\n    UNIQUE CLUSTERED (id) WITH (IGNORE_DUP_KEY=ON)\r\n)\r\n\r\nINSERT #temp{1}\r\nSELECT * FROM dbo.GetTaggedArtifacts({2}, {3}, '{4}', {5}) ", (object) tempTableId, (object) tempTableId, (object) this._context.PartitionId, (object) tagNameStringTable, (object) workItemArtifactKindGuid, (object) str));
    }

    private void CreateMultipleDateTable(
      int tempTableId,
      string tagNameStringTable,
      string workItemArtifactKindGuid)
    {
      this._context.m_queryText.Append(string.Format("\r\nCREATE TABLE #temp{0}\r\n(\r\n    id INT NOT NULL,\r\n    date DATETIME NOT NULL,\r\n    UNIQUE CLUSTERED (id, date) WITH (IGNORE_DUP_KEY=ON)\r\n)\r\n\r\nINSERT #temp{1}\r\nSELECT * FROM dbo.func_GetTaggedArtifactsByDates({2}, {3}, '{4}', {5}) ", (object) tempTableId, (object) tempTableId, (object) this._context.PartitionId, (object) tagNameStringTable, (object) workItemArtifactKindGuid, (object) this._context.m_asOfDatesParam));
    }

    private void AppendTempTableResults(int tempTableId)
    {
      string str = this._expressionNode.Operator.IsNegated() ? "NOT " : string.Empty;
      this._statement.Append(this._workItemsTableAlias + ".Id " + str + "IN (SELECT id FROM #temp" + tempTableId.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo) + ")");
      if (string.IsNullOrWhiteSpace(this._context.m_asOfDatesParam))
        return;
      this._statement.Append(" AND " + this._workItemsTableAlias + ".Id " + str + "IN (SELECT id FROM #temp" + tempTableId.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo) + " where date = AsOfDates.Val)");
    }
  }
}
