// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WiqlUtils
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Identity;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal static class WiqlUtils
  {
    private static HashSet<Condition> ValidReplacementConditions = new HashSet<Condition>()
    {
      Condition.Equals,
      Condition.EqualsAlias,
      Condition.Group,
      Condition.In,
      Condition.NotEquals,
      Condition.NotEqualsAlias
    };

    private static bool IsAreaPathField(IFieldTypeDictionary fieldsDict, string fieldName)
    {
      FieldEntry field;
      return fieldsDict.TryGetField(fieldName, out field) && field.IsAreaPath;
    }

    private static bool IsPortfolioProjectField(IFieldTypeDictionary fieldsDict, string fieldName)
    {
      FieldEntry field;
      return fieldsDict.TryGetField(fieldName, out field) && field.IsPortfolioProject;
    }

    private static bool IsIterationPathField(IFieldTypeDictionary fieldsDict, string fieldName)
    {
      FieldEntry field;
      return fieldsDict.TryGetField(fieldName, out field) && field.IsIterationPath;
    }

    private static bool IsPersonField(IFieldTypeDictionary fieldsDict, string fieldName)
    {
      FieldEntry field;
      return fieldsDict.TryGetField(fieldName, out field) && field.IsPerson;
    }

    private static bool IsIdentityField(IFieldTypeDictionary fieldsDict, string fieldName)
    {
      FieldEntry field;
      return fieldsDict.TryGetField(fieldName, out field) && field.IsIdentity;
    }

    internal static string TransformNamesToIds(
      IVssRequestContext requestContext,
      string wiql,
      ResolvedIdentityNamesInfo resolvedNamesInfo)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      Stopwatch stopwatch = Stopwatch.StartNew();
      requestContext.TraceEnter(906047, "Query", "WorkItemQueryService", "WiqlUtils.TransformNamesToIds");
      try
      {
        if (string.IsNullOrEmpty(wiql))
          return wiql;
        NodeSelect syntax = Parser.ParseSyntax(wiql);
        WiqlUtils.TransformNamesToIdsInternal(requestContext, syntax, resolvedNamesInfo);
        return syntax.ToString();
      }
      catch (Exception ex)
      {
        requestContext.Trace(906048, TraceLevel.Verbose, "Query", "WorkItemQueryService", ex.Message);
        return wiql;
      }
      finally
      {
        stopwatch.Stop();
        requestContext.Items["TransformNamesToIdsTime"] = (object) stopwatch.ElapsedMilliseconds;
        requestContext.TraceLeave(906049, "Query", "WorkItemQueryService", "WiqlUtils.TransformNamesToIds");
      }
    }

    internal static Dictionary<int, FieldEntry> ExtractFieldsFromWhere(Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node whereNode)
    {
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node>(whereNode, nameof (whereNode));
      Dictionary<int, FieldEntry> fields = new Dictionary<int, FieldEntry>();
      whereNode.Walk((Action<Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node>) (node =>
      {
        if (node == null || node.NodeType != NodeType.FieldCondition || !(((NodeCondition) node).Left?.Tag is FieldEntry tag2))
          return;
        fields[tag2.FieldId] = tag2;
      }));
      return fields;
    }

    internal static Dictionary<string, int> ExtractMacroUsage(Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node whereNode)
    {
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node>(whereNode, nameof (whereNode));
      Dictionary<string, int> macrosUsed = new Dictionary<string, int>();
      HashSet<NodeVariable> visited = new HashSet<NodeVariable>();
      whereNode.Walk((Action<Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node>) (node =>
      {
        if (node is NodeVariable nodeVariable2 && !visited.Contains(nodeVariable2))
          macrosUsed.AddOrUpdate<string, int>(nodeVariable2.Value.ToLower(), 1, (Func<string, int, int>) ((name, count) => count + 1));
        else if (node is NodeArithmetic nodeArithmetic2 && nodeArithmetic2.Left is NodeVariable left2 && nodeArithmetic2.Right is NodeNumber right2 && nodeArithmetic2.Arithmetic != Arithmetic.None)
        {
          string str = nodeArithmetic2.Arithmetic == Arithmetic.Add ? "+" : "-";
          macrosUsed.AddOrUpdate<string, int>((left2.Value + str + right2.ConstStringValue).ToLower(), 1, (Func<string, int, int>) ((name, count) => count + 1));
          visited.Add(left2);
        }
        if (!(node is NodeCondition nodeCondition2))
          return;
        if (nodeCondition2.Condition == Condition.IsEmpty)
          macrosUsed.AddOrUpdate<string, int>("IsEmpty", 1, (Func<string, int, int>) ((_, count) => count + 1));
        if (nodeCondition2.Condition != Condition.IsNotEmpty)
          return;
        macrosUsed.AddOrUpdate<string, int>("IsNotEmpty", 1, (Func<string, int, int>) ((_, count) => count + 1));
      }));
      return macrosUsed;
    }

    private static void TransformNamesToIdsInternal(
      IVssRequestContext requestContext,
      NodeSelect rootNode,
      ResolvedIdentityNamesInfo resolvedNamesInfo)
    {
      IFieldTypeDictionary fieldTypes = requestContext.GetService<WorkItemTrackingFieldService>().GetFieldsSnapshot(requestContext);
      ITreeDictionary treeService = requestContext.WitContext().TreeService;
      WiqlUtils.WalkWiqlNode(rootNode.Where, (Func<NodeCondition, NodeFieldName, Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node, Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node>) ((conditionNode, fieldNameNode, valueNode) =>
      {
        if (string.IsNullOrEmpty(valueNode?.ConstStringValue) || valueNode.ConstStringValue[0] == '\a')
          return valueNode;
        string str = "\a";
        if (WiqlUtils.IsAreaPathField(fieldTypes, fieldNameNode.Value) || WiqlUtils.IsPortfolioProjectField(fieldTypes, fieldNameNode.Value))
        {
          TreeNode treeNode;
          if (!treeService.TryGetNodeFromPath(requestContext, valueNode.ConstStringValue, TreeStructureType.Area, out treeNode))
            throw new SyntaxException(valueNode, SyntaxError.AreaPathIsNotFoundInHierarchy);
          return (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) new NodeString(str + string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ARPID:'{0}:{1:0000000000}'", (object) treeNode.ProjectId, (object) treeNode.Id));
        }
        if (WiqlUtils.IsIterationPathField(fieldTypes, fieldNameNode.Value))
        {
          TreeNode treeNode;
          if (!treeService.TryGetNodeFromPath(requestContext, valueNode.ConstStringValue, TreeStructureType.Iteration, out treeNode))
            throw new SyntaxException(valueNode, SyntaxError.IterationPathIsNotFoundInHierarchy);
          return (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) new NodeString(str + string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ITPID:'{0}:{1:0000000000}'", (object) treeNode.ProjectId, (object) treeNode.Id));
        }
        if (resolvedNamesInfo == null || !WiqlUtils.IsPersonField(fieldTypes, fieldNameNode.Value) && (!WiqlUtils.IsIdentityField(fieldTypes, fieldNameNode.Value) || conditionNode == null || !WiqlUtils.ValidReplacementConditions.Contains(conditionNode.Condition)))
          return valueNode;
        string s;
        if (resolvedNamesInfo.NamesLookup.ContainsKey(valueNode.ConstStringValue) && resolvedNamesInfo.NamesLookup[valueNode.ConstStringValue].TeamFoundationId != Guid.Empty)
          s = str + string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TFID:'{0}'", (object) resolvedNamesInfo.NamesLookup[valueNode.ConstStringValue].TeamFoundationId);
        else if (resolvedNamesInfo.AadNamesLookup.ContainsKey(valueNode.ConstStringValue))
        {
          string key = resolvedNamesInfo.AadNamesLookup[valueNode.ConstStringValue];
          s = !resolvedNamesInfo.IdentityMap.Value.ContainsKey(key) ? str + key : str + string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TFID:'{0}'", (object) resolvedNamesInfo.IdentityMap.Value[key].Id);
        }
        else
          s = str + valueNode.ConstStringValue;
        return (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) new NodeString(s);
      }));
    }

    private static void WalkWiqlNode(
      Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node,
      Func<NodeCondition, NodeFieldName, Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node, Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node> visitor)
    {
      if (node == null)
        return;
      switch (node.NodeType)
      {
        case NodeType.FieldCondition:
          NodeCondition nodeCondition = (NodeCondition) node;
          NodeFieldName left = nodeCondition.Left;
          Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node right = nodeCondition.Right;
          if ((right != null ? (right.NodeType == NodeType.ValueList ? 1 : 0) : 0) != 0)
          {
            for (int i = 0; i < nodeCondition.Right.Count; ++i)
              nodeCondition.Right[i] = visitor(nodeCondition, left, nodeCondition.Right[i]);
            break;
          }
          nodeCondition.Right = visitor(nodeCondition, left, nodeCondition.Right);
          break;
        case NodeType.Not:
          WiqlUtils.WalkWiqlNode(((NodeNotOperator) node).Value, visitor);
          break;
        case NodeType.Ever:
          WiqlUtils.WalkWiqlNode(((NodeEverOperator) node).Value, visitor);
          break;
        case NodeType.And:
        case NodeType.Or:
          int count = node.Count;
          for (int i = 0; i < count; ++i)
            WiqlUtils.WalkWiqlNode(node[i], visitor);
          break;
      }
    }
  }
}
