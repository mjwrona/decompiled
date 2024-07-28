// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TcmQueryTranslator
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal abstract class TcmQueryTranslator : ServerQueryEngine
  {
    private TestManagementRequestContext m_context;

    public TcmQueryTranslator(TestManagementRequestContext context, ResultsStoreQuery query)
      : base(context, query, (List<Tuple<Type, string, string>>) null)
    {
      this.m_context = context;
    }

    public TcmQueryTranslator(
      TestManagementRequestContext context,
      ResultsStoreQuery query,
      Func<string, string, string> conditionNodeValueTranslator = null,
      List<Tuple<Type, string, string>> tables = null)
      : base(context, query, conditionNodeValueTranslator, tables)
    {
      this.m_context = context;
    }

    public virtual string TranslateQuery()
    {
      this.TranslateQueryInternal();
      return this.GetFinalTranslatedQueryString();
    }

    protected virtual void TranslateQueryInternal()
    {
      this.TranslateSelectionFields();
      this.TranslateConditionFields();
      this.TranslateOrderBy();
      this.TranslateGroupBy();
    }

    protected virtual string GetFinalTranslatedQueryString()
    {
      string empty = string.Empty;
      string translatedQueryString;
      if (this.Root.Fields == null)
        translatedQueryString = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SELECT {0} FROM {1} {2} {3} {4}", (object) string.Join(",", (IEnumerable<string>) this.GetAllTableFields()), (object) this.Root.From.ToString(), this.Root.Where != null ? (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "WHERE {0}", (object) this.Root.Where.ToString()) : (object) string.Empty, this.Root.OrderBy != null ? (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ORDER BY {0}", (object) this.Root.OrderBy.ToString()) : (object) string.Empty, this.Root.GroupBy != null ? (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "GROUP BY {0}", (object) this.Root.GroupBy.ToString()) : (object) string.Empty);
      else
        translatedQueryString = this.Root.ToString();
      return translatedQueryString;
    }

    protected abstract IDictionary<string, string> GetFieldMap();

    protected abstract string GetCategoryRefName();

    protected abstract IList<string> GetAllTableFields();

    protected abstract void TranslateValue(NodeCondition cond);

    protected virtual IList<string> GetMappedStateValues(string stateString, bool converse) => (IList<string>) new List<string>();

    private void TranslateGroupBy()
    {
      if (this.Root.GroupBy == null)
        return;
      this.TranslateList((Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) this.Root.GroupBy);
    }

    private void TranslateOrderBy()
    {
      if (this.Root.OrderBy == null)
        return;
      this.TranslateList((Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) this.Root.OrderBy);
    }

    protected virtual bool TranslateFieldName(NodeFieldName nodeFieldName)
    {
      IDictionary<string, string> fieldMap = this.GetFieldMap();
      string b;
      if (fieldMap.TryGetValue(nodeFieldName.ToString(), out b) && string.Equals(nodeFieldName.ToString(), b, StringComparison.OrdinalIgnoreCase))
        return true;
      if (!fieldMap.TryGetValue(nodeFieldName.Value, out b))
        return false;
      nodeFieldName.Value = b;
      return true;
    }

    private void TranslateConditionFields()
    {
      if (this.Root.Where == null)
        return;
      this.TranslateNode(this.Root.Where);
    }

    internal void TranslateNode(Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node)
    {
      switch (node.NodeType)
      {
        case NodeType.Number:
          break;
        case NodeType.String:
          break;
        case NodeType.FieldName:
          this.TranslateFieldName((NodeFieldName) node);
          break;
        case NodeType.FieldCondition:
          this.TranslateCondition((NodeCondition) node);
          break;
        case NodeType.ValueList:
          this.TranslateList(node);
          break;
        case NodeType.BoolConst:
          break;
        case NodeType.BoolValue:
          break;
        case NodeType.Not:
          this.TranslateNegation((NodeNotOperator) node);
          break;
        case NodeType.And:
        case NodeType.Or:
          this.TranslateList(node);
          break;
        case NodeType.Variable:
          break;
        case NodeType.Arithmetic:
          this.TranslateArithmetic((NodeArithmetic) node);
          break;
        default:
          Tools.EnsureSyntax(false, SyntaxError.InvalidNodeType, node);
          break;
      }
    }

    private void TranslateNegation(NodeNotOperator not) => this.TranslateNode(not.Value);

    internal virtual void TranslateCondition(NodeCondition cond)
    {
      switch (cond.Condition)
      {
        case Condition.Equals:
        case Condition.NotEquals:
        case Condition.In:
        case Condition.EqualsAlias:
        case Condition.NotEqualsAlias:
          this.TranslateValue(cond);
          goto case Condition.Less;
        case Condition.Less:
        case Condition.Greater:
        case Condition.LessOrEquals:
        case Condition.GreaterOrEquals:
          this.TranslateFieldName(cond.Left);
          this.TranslateNode(cond.Right);
          break;
        case Condition.Under:
        case Condition.Contains:
          this.TranslateFieldName(cond.Left);
          break;
        default:
          Tools.EnsureSyntax(true, SyntaxError.InvalidConditionalOperator, (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) cond);
          break;
      }
    }

    private void TranslateList(Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node list)
    {
      foreach (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node in list)
        this.TranslateNode(node);
    }

    private void TranslateArithmetic(NodeArithmetic node)
    {
      this.TranslateNode(node.Left);
      this.TranslateNode(node.Right);
    }

    private void TranslateSelectionFields()
    {
      if (this.Root.Fields == null)
        return;
      for (int i = 0; i < this.Root.Fields.Count; ++i)
        this.TranslateFieldName(this.Root.Fields[i]);
    }

    protected virtual void TranslateTable()
    {
    }

    protected virtual string GetDefaultProjectName() => string.Empty;

    protected virtual int GetDataspaceIdWithLazyInitialization(Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node)
    {
      string constStringValue = node.ConstStringValue;
      if (string.IsNullOrEmpty(constStringValue) && node is NodeVariable)
        constStringValue = Convert.ToString(this.GetVarTag((NodeVariable) node));
      Guid projectGuidFromName = Validator.CheckAndGetProjectGuidFromName(this.m_context, constStringValue);
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(this.m_context))
        return managementDatabase.GetDataspaceIdWithLazyInitialization(projectGuidFromName);
    }

    protected virtual void HandleProjectNameCondition(NodeCondition cond)
    {
      if (cond.Condition == Condition.In)
      {
        NodeValueList right = (NodeValueList) cond.Right;
        if (right != null)
        {
          List<string> mappedValues = new List<string>();
          foreach (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node in (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) right)
            mappedValues.Add(this.GetDataspaceIdWithLazyInitialization(node).ToString());
          cond.Right = (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) TcmQueryTranslator.GetNodeValueList((IList<string>) mappedValues);
        }
        else
          cond.Right = (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) new NodeString(this.GetDataspaceIdWithLazyInitialization(cond.Right).ToString());
      }
      else
        cond.Right = (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) new NodeString(this.GetDataspaceIdWithLazyInitialization(cond.Right).ToString());
    }

    protected virtual string GetTCMAreaIterationPath(Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node)
    {
      string constStringValue = node.ConstStringValue;
      if (string.IsNullOrEmpty(constStringValue) && node is NodeVariable)
        constStringValue = Convert.ToString(this.GetVarTag((NodeVariable) node));
      return this.m_context.CSSHelper.WorkItemToTCMPath(constStringValue);
    }

    protected virtual void HandleAreaIterationPathCondition(NodeCondition cond)
    {
      switch (cond.Condition)
      {
        case Condition.Under:
          string areaIterationPath = this.GetTCMAreaIterationPath(cond.Right);
          cond.Right = (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) new NodeString(areaIterationPath);
          break;
        case Condition.In:
          NodeValueList right = (NodeValueList) cond.Right;
          if (right != null)
          {
            List<string> mappedValues = new List<string>();
            foreach (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node in (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) right)
              mappedValues.Add(this.GetTCMAreaIterationPath(node));
            cond.Right = (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) TcmQueryTranslator.GetNodeValueList((IList<string>) mappedValues);
            break;
          }
          cond.Right = (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) new NodeString(this.GetTCMAreaIterationPath(cond.Right));
          break;
        default:
          cond.Right = (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) new NodeString(this.GetTCMAreaIterationPath(cond.Right));
          break;
      }
    }

    protected virtual void HandleStateCondition(NodeCondition cond)
    {
      switch (cond.Condition)
      {
        case Condition.Equals:
        case Condition.EqualsAlias:
          this.TranslateStateCondition(cond, false);
          break;
        case Condition.NotEquals:
        case Condition.NotEqualsAlias:
          this.TranslateStateCondition(cond, true);
          break;
        case Condition.In:
          NodeValueList right = (NodeValueList) cond.Right;
          Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.CurrentCultureIgnoreCase);
          foreach (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node in (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) right)
          {
            if (!(node is NodeNumber nodeNumber))
              throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidTcmState, (object) node.ToString(), (object) this.GetCategoryRefName().ToLower(CultureInfo.CurrentCulture)));
            foreach (string mappedStateValue in (IEnumerable<string>) this.GetMappedStateValues(nodeNumber.ConstStringValue, false))
            {
              if (!dictionary.ContainsKey(mappedStateValue))
                dictionary[mappedStateValue] = mappedStateValue;
            }
          }
          cond.Right = (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) TcmQueryTranslator.GetNodeValueList((IList<string>) dictionary.Keys.ToList<string>());
          break;
        default:
          throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidConditionForTcmField, (object) cond.ToString(), (object) cond.Left.Value));
      }
    }

    private void TranslateStateCondition(NodeCondition cond, bool notEquals)
    {
      IList<string> mappedStateValues = this.GetMappedStateValues(cond.Right.ConstStringValue, false);
      if (mappedStateValues.Count == 1)
      {
        cond.Right = (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) new NodeString(mappedStateValues[0]);
      }
      else
      {
        if (notEquals)
          mappedStateValues = this.GetMappedStateValues(cond.Right.ConstStringValue, true);
        cond.Condition = Condition.In;
        NodeValueList nodeValueList = TcmQueryTranslator.GetNodeValueList(mappedStateValues);
        cond.Right = (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) nodeValueList;
      }
    }

    protected static NodeValueList GetNodeValueList(IList<string> mappedValues)
    {
      NodeValueList nodeValueList = new NodeValueList();
      foreach (string mappedValue in (IEnumerable<string>) mappedValues)
        nodeValueList.Add((Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) new NodeString(mappedValue));
      return nodeValueList;
    }
  }
}
