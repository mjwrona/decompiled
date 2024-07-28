// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TcmQueryParser
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TcmQueryParser
  {
    private Dictionary<string, List<object>> m_parametersMap;
    private NodeSelect m_root;

    public TcmQueryParser(string queryText)
    {
      this.m_root = Parser.ParseSyntax(queryText);
      this.m_parametersMap = new Dictionary<string, List<object>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    public Dictionary<string, List<object>> GetParametersMap()
    {
      if (this.m_root.Where != null)
        this.ParseNode(this.m_root.Where);
      return this.m_parametersMap;
    }

    private void ParseNode(Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node)
    {
      switch (node.NodeType)
      {
        case NodeType.FieldCondition:
          this.ParseConditionNode((NodeCondition) node);
          break;
        case NodeType.And:
          this.ParseValueList((NodeVariableList) node);
          break;
        default:
          Tools.EnsureSyntax(false, SyntaxError.InvalidNodeType, node);
          break;
      }
    }

    private void ParseValueList(NodeVariableList list)
    {
      foreach (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node in (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) list)
        this.ParseNode(node);
    }

    private void ParseConditionNode(NodeCondition cond)
    {
      switch (cond.Condition)
      {
        case Condition.Equals:
          this.UpdateNonRangeParametersInMap(cond);
          break;
        case Condition.In:
          this.UpdateRangeParametersInMap(cond);
          break;
        default:
          Tools.EnsureSyntax(false, SyntaxError.InvalidConditionalOperator, (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) cond);
          break;
      }
    }

    private void UpdateNonRangeParametersInMap(NodeCondition cond) => this.m_parametersMap[cond.Left.Value] = new List<object>()
    {
      this.GetFieldValue(cond.Right)
    };

    private void UpdateRangeParametersInMap(NodeCondition cond) => this.m_parametersMap[cond.Left.Value] = this.GetFieldRange(cond.Right);

    private object GetFieldValue(Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node)
    {
      object fieldValue = (object) null;
      switch (node.NodeType)
      {
        case NodeType.Number:
          int result;
          if (int.TryParse(node.ConstStringValue, out result))
          {
            fieldValue = (object) result;
            break;
          }
          break;
        case NodeType.String:
          fieldValue = (object) node.ConstStringValue;
          break;
        case NodeType.BoolValue:
          fieldValue = (object) ((NodeBoolValue) node).BoolValue;
          break;
        default:
          Tools.EnsureSyntax(false, SyntaxError.InvalidNodeType, node);
          break;
      }
      return fieldValue;
    }

    private List<object> GetFieldRange(Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node)
    {
      List<object> fieldRange = new List<object>();
      if (node.NodeType != NodeType.ValueList)
        Tools.EnsureSyntax(false, SyntaxError.InvalidNodeType, node);
      foreach (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node1 in node)
      {
        object fieldValue = this.GetFieldValue(node1);
        fieldRange.Add(fieldValue);
      }
      return fieldRange;
    }
  }
}
