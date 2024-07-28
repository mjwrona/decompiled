// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.WiqlHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models
{
  internal static class WiqlHelper
  {
    public static void SplitFilters(
      string wiql,
      out NodeSelect wiqlNodes,
      out Microsoft.TeamFoundation.WorkItemTracking.Internals.LinkQueryMode queryMode,
      out string filter,
      out string sourceFilter,
      out string linkFilter,
      out string targetFilter)
    {
      wiqlNodes = Parser.ParseSyntax(wiql);
      queryMode = WiqlHelper.GetQueryMode(wiqlNodes);
      filter = wiqlNodes.Where == null ? string.Empty : wiqlNodes.Where.ToString();
      if (queryMode > Microsoft.TeamFoundation.WorkItemTracking.Internals.LinkQueryMode.WorkItems)
      {
        Dictionary<string, NodeAndOperator> whereGroups = wiqlNodes.GetWhereGroups();
        NodeAndOperator nodeAndOperator;
        sourceFilter = !whereGroups.TryGetValue("Source", out nodeAndOperator) ? string.Empty : nodeAndOperator.ToString();
        linkFilter = !whereGroups.TryGetValue(string.Empty, out nodeAndOperator) ? string.Empty : nodeAndOperator.ToString();
        if (whereGroups.TryGetValue("Target", out nodeAndOperator))
          targetFilter = nodeAndOperator.ToString();
        else
          targetFilter = string.Empty;
      }
      else
      {
        sourceFilter = filter;
        targetFilter = string.Empty;
        linkFilter = string.Empty;
      }
    }

    public static string ExtractFilter(string wiql) => WiqlHelper.ExtractFilter(Parser.ParseSyntax(wiql));

    public static string ExtractFilter(NodeSelect wiqlNodes) => wiqlNodes.Where != null ? wiqlNodes.Where.ToString() : string.Empty;

    public static string GenerateWiql(
      string wiql,
      IEnumerable<string> fields,
      IEnumerable<QuerySortOrderEntry> sortFields,
      DateTime? asOf)
    {
      return WiqlHelper.GenerateWiql(wiql, fields, sortFields, asOf, (string) null);
    }

    public static string GenerateWiql(
      string wiql,
      IEnumerable<string> fields,
      IEnumerable<QuerySortOrderEntry> sortFields,
      DateTime? asOf,
      string additionalWhereClause)
    {
      NodeSelect syntax = Parser.ParseSyntax(wiql);
      return WiqlHelper.GenerateWiql(WiqlHelper.GetQueryMode(syntax), WiqlHelper.ExtractFilter(syntax), fields, sortFields, asOf, additionalWhereClause);
    }

    public static string GenerateWiql(
      Microsoft.TeamFoundation.WorkItemTracking.Internals.LinkQueryMode queryMode,
      string filter,
      IEnumerable<string> fields,
      IEnumerable<QuerySortOrderEntry> sortFields,
      DateTime? asOf,
      string additionalWhereClause)
    {
      StringBuilder stringBuilder = new StringBuilder(512);
      stringBuilder.Append(WiqlHelper.GetSelectClause(fields));
      stringBuilder.Append(" FROM ");
      if (queryMode == Microsoft.TeamFoundation.WorkItemTracking.Internals.LinkQueryMode.WorkItems)
        stringBuilder.Append("WorkItems");
      else
        stringBuilder.Append("WorkItemLinks");
      bool flag1 = !string.IsNullOrEmpty(additionalWhereClause);
      bool flag2 = filter.Trim().Length > 0;
      if (flag2 | flag1)
      {
        stringBuilder.Append(" WHERE ");
        if (flag1)
          stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, " {0} ", (object) additionalWhereClause);
        if (flag1 & flag2)
          stringBuilder.Append(" AND ");
        if (flag2)
          stringBuilder.Append(filter);
      }
      string orderByClause = WiqlHelper.GetOrderByClause(sortFields, queryMode);
      if (!string.IsNullOrEmpty(orderByClause))
      {
        stringBuilder.Append(' ');
        stringBuilder.Append(orderByClause);
      }
      int num = asOf.HasValue ? 1 : 0;
      string str = string.Empty;
      switch (queryMode)
      {
        case Microsoft.TeamFoundation.WorkItemTracking.Internals.LinkQueryMode.LinksMustContain:
          str = "MustContain";
          break;
        case Microsoft.TeamFoundation.WorkItemTracking.Internals.LinkQueryMode.LinksMayContain:
          str = "MayContain";
          break;
        case Microsoft.TeamFoundation.WorkItemTracking.Internals.LinkQueryMode.LinksDoesNotContain:
          str = "DoesNotContain";
          break;
        case Microsoft.TeamFoundation.WorkItemTracking.Internals.LinkQueryMode.LinksRecursive:
          str = "Recursive";
          break;
        case Microsoft.TeamFoundation.WorkItemTracking.Internals.LinkQueryMode.LinksRecursiveReturnMatchingChildren:
          str = "Recursive,ReturnMatchingChildren";
          break;
      }
      if (!string.IsNullOrEmpty(str))
      {
        stringBuilder.Append(" mode(");
        stringBuilder.Append(str);
        stringBuilder.Append(')');
      }
      if (asOf.HasValue && asOf.Value != DateTime.MinValue)
      {
        stringBuilder.Append(" asof '");
        stringBuilder.Append(asOf.Value.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture));
        stringBuilder.Append("'");
      }
      return stringBuilder.ToString();
    }

    public static string GetSelectClause(IEnumerable<string> fields)
    {
      if (fields == null)
        fields = Enumerable.Empty<string>();
      return "SELECT " + string.Join(", ", fields.DefaultIfEmpty<string>("System.Id").Select<string, string>((Func<string, string>) (f => "[" + f + "]")));
    }

    private static string GetOrderByClause(
      IEnumerable<QuerySortOrderEntry> sortFields,
      Microsoft.TeamFoundation.WorkItemTracking.Internals.LinkQueryMode queryMode)
    {
      if (sortFields != null)
      {
        IEnumerable<QuerySortOrderEntry> source = sortFields;
        if (queryMode <= Microsoft.TeamFoundation.WorkItemTracking.Internals.LinkQueryMode.WorkItems)
          source = (IEnumerable<QuerySortOrderEntry>) sortFields.Where<QuerySortOrderEntry>((Func<QuerySortOrderEntry, bool>) (sf => !StringComparer.OrdinalIgnoreCase.Equals(sf.ColumnName, "System.Links.LinkType"))).ToArray<QuerySortOrderEntry>();
        if (source.Any<QuerySortOrderEntry>())
          return "ORDER BY " + string.Join(", ", sortFields.Select<QuerySortOrderEntry, string>((Func<QuerySortOrderEntry, string>) (sf => "[" + sf.ColumnName + "]" + (sf.Ascending ? "" : " DESC"))));
      }
      return string.Empty;
    }

    public static NodeSelect ParseSyntax(string wiql) => Parser.ParseSyntax(wiql);

    public static Microsoft.TeamFoundation.WorkItemTracking.Internals.LinkQueryMode GetQueryMode(
      NodeSelect select)
    {
      return WiqlAdapter.GetQueryMode(select);
    }

    public static bool ParseFilter(
      string filter,
      string prefix,
      out string parsedWiql,
      out Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node whereNode)
    {
      whereNode = (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) null;
      parsedWiql = filter != null ? filter.Trim() : string.Empty;
      if (!parsedWiql.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
      {
        if (parsedWiql.Length <= 0)
          return false;
        string str = string.IsNullOrEmpty(prefix) ? "Workitems" : "WorkitemLinks";
        if (!parsedWiql.StartsWith("WHERE", StringComparison.OrdinalIgnoreCase))
          parsedWiql = "WHERE " + parsedWiql;
        parsedWiql = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SELECT ID FROM {0} {1}", (object) str, (object) parsedWiql);
      }
      NodeSelect syntax = Parser.ParseSyntax(parsedWiql);
      whereNode = syntax.Where;
      if (!string.IsNullOrEmpty(prefix))
      {
        Dictionary<string, NodeAndOperator> whereGroups = syntax.GetWhereGroups();
        if (whereGroups.ContainsKey(prefix))
          whereNode = (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) whereGroups[prefix];
      }
      return true;
    }

    public static List<FilterClause> ProcessFilterNode(
      Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node whereNode,
      string originalWiql,
      QueryAdapter adapter,
      List<FilterGroup> groups)
    {
      List<FilterClause> clauseList = new List<FilterClause>();
      if (whereNode != null && whereNode.Count > 0)
      {
        Hashtable hash = new Hashtable();
        WiqlHelper.ParseWiqlNode(clauseList, whereNode, "", hash, adapter);
        groups.Clear();
        WiqlHelper.ProcessChildGroups(groups, whereNode, hash, originalWiql);
      }
      return clauseList;
    }

    public static string GetCommonTeamProject(QueryAdapter adapter, string whereClause)
    {
      Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node whereNode;
      if (!WiqlHelper.ParseFilter(whereClause, string.Empty, out string _, out whereNode) || whereNode == null || whereNode.Count <= 0 || whereNode.NodeType == NodeType.Or)
        return string.Empty;
      string projectName1 = string.Empty;
      if (WiqlHelper.TryGetTeamProjectFromNode(adapter, whereNode, out projectName1))
        return projectName1;
      foreach (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node n in whereNode)
      {
        string projectName2;
        if (WiqlHelper.TryGetTeamProjectFromNode(adapter, n, out projectName2))
        {
          if (string.IsNullOrEmpty(projectName2) || !string.IsNullOrEmpty(projectName1) && !TFStringComparer.TeamProjectName.Equals(projectName2, projectName1))
            return string.Empty;
          projectName1 = projectName2;
        }
      }
      return projectName1;
    }

    private static bool TryGetTeamProjectFromNode(
      QueryAdapter adapter,
      Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node n,
      out string projectName)
    {
      projectName = string.Empty;
      if (n != null && n.NodeType == NodeType.FieldCondition)
      {
        NodeCondition nodeCondition = n as NodeCondition;
        if (nodeCondition.Condition == Condition.Equals && adapter.IsTeamProjectField(nodeCondition.Left.Value))
        {
          string str = (nodeCondition.Right is NodeVariable right1 ? right1.ToString() : (string) null) ?? (nodeCondition.Right is NodeString right2 ? right2.Value : (string) null);
          projectName = str;
          return true;
        }
      }
      return false;
    }

    private static void ParseWiqlNode(
      List<FilterClause> clauseList,
      Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node,
      string parentOperator,
      Hashtable hash,
      QueryAdapter adapter)
    {
      if (node == null)
        return;
      if (node.NodeType == NodeType.And || node.NodeType == NodeType.Or)
      {
        for (int i = 0; i < node.Count; ++i)
        {
          if (i == 0)
            WiqlHelper.ParseWiqlNode(clauseList, node[i], parentOperator, hash, adapter);
          else
            WiqlHelper.ParseWiqlNode(clauseList, node[i], node.NodeType == NodeType.And ? "AND" : "OR", hash, adapter);
        }
      }
      else
        WiqlHelper.AddNodeAsRow(clauseList, node, parentOperator, hash, adapter);
    }

    private static void AddNodeAsRow(
      List<FilterClause> clauseList,
      Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node,
      string parentOperator,
      Hashtable hash,
      QueryAdapter adapter)
    {
      WiqlHelper.NodeConditionType conditionType;
      NodeCondition nodeCondition = WiqlHelper.GetNodeCondition(node, out conditionType);
      if (nodeCondition == null || nodeCondition.Count != 2)
        throw new TeamFoundationServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.UnexpectedNode, (object) node.ToString()));
      string str1 = nodeCondition.Left.Value;
      string str2 = ConditionalOperators.GetString(nodeCondition.Condition);
      Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node right = nodeCondition.Right;
      switch (conditionType)
      {
        case WiqlHelper.NodeConditionType.Ever:
          str2 = "EVER";
          break;
        case WiqlHelper.NodeConditionType.Not:
          str2 = "NOT " + str2;
          break;
        case WiqlHelper.NodeConditionType.NotEver:
          str2 = "NOT EVER";
          break;
      }
      string localizedOperator = adapter.GetLocalizedOperator(parentOperator);
      string localizedFieldName = adapter.GetLocalizedFieldName(str1);
      string str3;
      string localizedOperatorName;
      if (right != null && right.NodeType == NodeType.FieldName)
      {
        string fieldName = ((NodeItem) right).Value;
        str3 = adapter.GetLocalizedFieldName(fieldName);
        localizedOperatorName = adapter.GetFieldComparisonOperator(str2);
      }
      else if (nodeCondition.Condition == Condition.IsEmpty || nodeCondition.Condition == Condition.IsNotEmpty)
      {
        localizedOperatorName = adapter.GetLocalizedOperator(str2);
        str3 = "";
      }
      else
      {
        localizedOperatorName = adapter.GetLocalizedOperator(str2);
        str3 = adapter.GetLocalizedFieldValue(str1, str2, right);
      }
      adapter.OnLoadFromWiql(localizedFieldName, ref localizedOperatorName, ref str3);
      FilterClause filterClause = new FilterClause();
      filterClause.LogicalOperator = localizedOperator;
      filterClause.FieldName = localizedFieldName;
      filterClause.Operator = localizedOperatorName;
      filterClause.Value = str3;
      filterClause.Index = clauseList.Count + 1;
      clauseList.Add(filterClause);
      hash.Add((object) nodeCondition, (object) filterClause.Index);
    }

    private static NodeCondition GetNodeCondition(
      Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node,
      out WiqlHelper.NodeConditionType conditionType)
    {
      NodeCondition nodeCondition;
      if (node.NodeType == NodeType.FieldCondition)
      {
        conditionType = WiqlHelper.NodeConditionType.Normal;
        nodeCondition = node as NodeCondition;
      }
      else if (node.NodeType == NodeType.Not)
      {
        conditionType = WiqlHelper.NodeConditionType.Not;
        if (!(node is NodeNotOperator nodeNotOperator) || nodeNotOperator.Count != 1)
          throw new TeamFoundationServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.UnexpectedNode, (object) node.ToString()));
        WiqlHelper.NodeConditionType conditionType1;
        nodeCondition = WiqlHelper.GetNodeCondition(nodeNotOperator[0], out conditionType1);
        if (conditionType1 == WiqlHelper.NodeConditionType.Ever)
          conditionType = WiqlHelper.NodeConditionType.NotEver;
      }
      else
      {
        if (node.NodeType != NodeType.Ever)
          throw new TeamFoundationServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.UnexpectedNode, (object) node.ToString()));
        conditionType = WiqlHelper.NodeConditionType.Ever;
        if (!(node is NodeEverOperator nodeEverOperator) || nodeEverOperator.Count != 1)
          throw new TeamFoundationServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.UnexpectedNode, (object) node.ToString()));
        WiqlHelper.NodeConditionType conditionType2;
        nodeCondition = WiqlHelper.GetNodeCondition(nodeEverOperator[0], out conditionType2);
        if (conditionType2 != WiqlHelper.NodeConditionType.Normal)
          throw new TeamFoundationServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.UnexpectedNode, (object) node.ToString()));
      }
      return nodeCondition;
    }

    private static void ProcessChildGroups(
      List<FilterGroup> groups,
      Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node,
      Hashtable hash,
      string wiql)
    {
      for (int i = 0; i < node.Count; ++i)
      {
        Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node1 = node[i];
        if (node1 != null)
        {
          if (node1.NodeType == NodeType.And || node1.NodeType == NodeType.Or)
            WiqlHelper.ProcessGroupNode(groups, node, node1, hash, wiql);
          if (node1.Count > 0)
            WiqlHelper.ProcessChildGroups(groups, node1, hash, wiql);
        }
      }
    }

    private static void ProcessGroupNode(
      List<FilterGroup> groups,
      Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node parentNode,
      Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node,
      Hashtable hash,
      string wiql)
    {
      if (node.NodeType != NodeType.And && node.NodeType != NodeType.Or || node.Priority <= parentNode.Priority && !WiqlHelper.HasExplicitGrouping(node, wiql))
        return;
      Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node firstCondition = WiqlHelper.GetFirstCondition(node);
      if (firstCondition == null)
        throw new Exception(Resources.MissingGroupingFirst);
      Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node lastCondition = WiqlHelper.GetLastCondition(node);
      if (lastCondition == null)
        throw new Exception(Resources.MissingGroupingLast);
      if (!hash.Contains((object) firstCondition) || !hash.Contains((object) lastCondition))
        throw new Exception(Resources.MissingGroupingIndices);
      int firstRow = (int) hash[(object) firstCondition];
      int lastRow = (int) hash[(object) lastCondition];
      WiqlHelper.AddGrouping(groups, firstRow, lastRow);
    }

    private static Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node GetFirstCondition(
      Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node)
    {
      if (node != null)
      {
        if (node.NodeType == NodeType.FieldCondition)
          return node;
        if (node.Count > 0)
          return WiqlHelper.GetFirstCondition(node[0]);
      }
      return (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) null;
    }

    private static Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node GetLastCondition(
      Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node)
    {
      if (node != null)
      {
        if (node.NodeType == NodeType.FieldCondition)
          return node;
        if (node.Count > 0)
          return WiqlHelper.GetLastCondition(node[node.Count - 1]);
      }
      return (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) null;
    }

    private static bool HasExplicitGrouping(Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node, string wiql)
    {
      string str = WiqlHelper.WiqlNodeAsString(node, wiql);
      return str.StartsWith("(", StringComparison.OrdinalIgnoreCase) && str.EndsWith(")", StringComparison.OrdinalIgnoreCase);
    }

    public static void AddGrouping(List<FilterGroup> groups, int firstRow, int lastRow)
    {
      if (firstRow >= lastRow || firstRow < 0)
        throw new ArgumentOutOfRangeException(nameof (firstRow));
      foreach (FilterGroup group in groups)
      {
        if (group.Start == firstRow && group.End == lastRow)
          return;
        if (group.End == firstRow || group.Start == lastRow)
          throw new Exception(Resources.CanNotGroup);
        if (group.Start < firstRow && group.End > firstRow && group.End < lastRow || group.Start < lastRow && group.End > lastRow && group.Start > firstRow)
          throw new Exception(Resources.CanNotGroup);
      }
      groups.Add(new FilterGroup(firstRow, lastRow));
    }

    public static string BuildFilterExpression(
      FilterClause[] clauses,
      List<FilterGroup> groups,
      QueryAdapter adapter,
      string fieldNamePrefix)
    {
      List<string> stringList1 = new List<string>();
      List<FilterClause> filterClauseList = new List<FilterClause>();
      if (clauses != null)
      {
        foreach (FilterClause clause in clauses)
        {
          if (clause.IsValid())
            filterClauseList.Add(clause);
        }
      }
      for (int index = 0; index < filterClauseList.Count; ++index)
      {
        FilterClause filterClause = filterClauseList[index];
        List<string> stringList2 = new List<string>();
        if (index > 0)
          stringList2.Add(adapter.GetInvariantOperator(filterClause.LogicalOperator));
        int count1 = 0;
        foreach (FilterGroup group in groups)
        {
          if (group.Start == filterClause.Index)
            ++count1;
        }
        if (count1 > 0)
          stringList2.Add(new string('(', count1));
        string fieldName = filterClause.FieldName;
        string operatorName = filterClause.Operator;
        string str1 = filterClause.Value;
        adapter.OnSaveToWiql(fieldName, ref operatorName, ref str1);
        string invariantFieldName1 = adapter.GetInvariantFieldName(fieldName);
        string invariantOperator = adapter.GetInvariantOperator(operatorName);
        string str2;
        if (adapter.IsFieldComparisonOperator(operatorName))
        {
          string invariantFieldName2 = adapter.GetInvariantFieldName(str1);
          str2 = string.IsNullOrEmpty(fieldNamePrefix) ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[{0}]", (object) invariantFieldName2) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[{0}].[{1}]", (object) fieldNamePrefix, (object) invariantFieldName2);
        }
        else
          str2 = adapter.GetInvariantFieldValue(invariantFieldName1, invariantOperator, str1);
        if (!string.IsNullOrEmpty(fieldNamePrefix))
          stringList2.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[{0}].[{1}]", (object) fieldNamePrefix, (object) invariantFieldName1));
        else
          stringList2.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[{0}]", (object) invariantFieldName1));
        stringList2.Add(invariantOperator);
        stringList2.Add(str2);
        int count2 = 0;
        foreach (FilterGroup group in groups)
        {
          if (group.End == filterClause.Index)
            ++count2;
        }
        if (count2 > 0)
          stringList2.Add(new string(')', count2));
        stringList1.Add(string.Join(" ", stringList2.ToArray()));
      }
      return string.Join(" ", stringList1.ToArray());
    }

    public static string WiqlNodeAsString(Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node, string wiqlStatement) => wiqlStatement == null || node.StartOffset < 0 || node.StartOffset >= wiqlStatement.Length || node.EndOffset < 0 || node.EndOffset > wiqlStatement.Length ? string.Empty : (node.EndOffset != wiqlStatement.Length ? wiqlStatement.Substring(node.StartOffset, node.EndOffset - node.StartOffset + 1) : wiqlStatement.Substring(node.StartOffset)).Trim();

    public static string QuoteStringValue(string value) => string.IsNullOrEmpty(value) ? "''" : "'" + value.Replace("'", "''") + "'";

    internal static string ModifyWiql(NodeSelect wiqlNodes, string wiqlKey, string value) => WiqlHelper.ModifyWiql(wiqlNodes, new string[1]
    {
      wiqlKey
    }, new string[1]{ value });

    internal static string ModifyWiql(NodeSelect wiqlNodes, string[] wiqlKeys, string[] values)
    {
      StringBuilder stringBuilder = new StringBuilder();
      string[] strArray = new string[7]
      {
        " {0}",
        " from {0}",
        " where {0}",
        " group by {0}",
        " order by {0}",
        " asof {0}",
        " mode ({0})"
      };
      List<string> replacedKeys = new List<string>((IEnumerable<string>) wiqlKeys);
      stringBuilder.Append("select");
      for (int i = 0; i < strArray.Length; ++i)
      {
        int valueIndex = WiqlHelper.GetValueIndex(strArray[i], replacedKeys);
        if (valueIndex >= 0)
        {
          string str = values[valueIndex];
          if (!string.IsNullOrEmpty(str))
            stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, strArray[i], (object) str);
        }
        else
        {
          Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node wiqlNode = wiqlNodes[i];
          if (wiqlNode != null)
          {
            string str = wiqlNode.ToString();
            if (!string.IsNullOrEmpty(str))
              stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, strArray[i], (object) str);
          }
        }
      }
      return stringBuilder.ToString();
    }

    private static int GetValueIndex(string keyword, List<string> replacedKeys) => replacedKeys != null ? replacedKeys.FindIndex((Predicate<string>) (s => keyword.StartsWith(" " + s, StringComparison.OrdinalIgnoreCase))) : -1;

    internal enum NodeConditionType
    {
      Normal,
      Ever,
      Not,
      NotEver,
    }
  }
}
