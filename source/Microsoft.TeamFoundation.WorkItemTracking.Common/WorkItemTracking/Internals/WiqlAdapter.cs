// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Internals.WiqlAdapter
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Provision;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Internals
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class WiqlAdapter : IExternal
  {
    private IWiqlAdapterHelper m_helper;
    private IDictionary m_context;
    private bool m_dayPrecision;
    private bool m_isAtTodayClause;
    private bool m_isDateBasedMacro;
    private const string c_me = "me";
    private const string c_today = "today";
    private const string c_project = "project";
    private const string c_team = "team";
    private static readonly DateTime s_startOfLastDay = DateTime.MaxValue - new TimeSpan(1, 0, 0, 0);
    private static string[] s_Operators = new string[48]
    {
      null,
      null,
      null,
      null,
      "equals",
      "notequals",
      "ever",
      null,
      "notequals",
      "equals",
      null,
      "ever",
      "less",
      "equalsgreater",
      null,
      null,
      "greater",
      "equalsless",
      null,
      null,
      "equalsless",
      "greater",
      null,
      null,
      "equalsgreater",
      "less",
      null,
      null,
      "under",
      "notunder",
      null,
      null,
      null,
      null,
      null,
      null,
      "contains",
      "notcontains",
      "evercontains",
      "nevercontains",
      "containswords",
      "notcontainswords",
      "evercontainswords",
      "nevercontainswords",
      "equals",
      "notequals",
      null,
      null
    };

    public WiqlAdapter(IWiqlAdapterHelper helper) => this.m_helper = helper;

    public bool DayPrecision
    {
      get => this.m_dayPrecision;
      set => this.m_dayPrecision = value;
    }

    public IDictionary Context
    {
      get
      {
        if (this.m_context == null)
          this.m_context = (IDictionary) new Hashtable((IEqualityComparer) TFStringComparer.StoredQueryText);
        return this.m_context;
      }
      set
      {
        if (value == null)
          return;
        this.m_context = (IDictionary) new Hashtable((IEqualityComparer) TFStringComparer.StoredQueryText);
        foreach (DictionaryEntry dictionaryEntry in value)
          this.m_context[dictionaryEntry.Key] = dictionaryEntry.Value;
      }
    }

    public CultureInfo CultureInfo => this.m_helper.CultureInfo;

    public TimeZone TimeZone => this.m_helper.TimeZone;

    public static string Me => "me";

    public static string Today => "today";

    public static string Project => "project";

    public static string Team => "team";

    private int GetTreeID(string path, TreeStructureType type) => this.m_helper.GetTreeID(path, type);

    public object FindTable(string name)
    {
      LinkQueryMode queryMode = WiqlAdapter.GetQueryMode(new NodeTableName(name));
      switch (queryMode)
      {
        case LinkQueryMode.Unknown:
          return (object) null;
        case LinkQueryMode.WorkItems:
          return (object) queryMode;
        default:
          if (!this.m_helper.IsSupported("WorkItemLinks"))
            throw new NotSupportedException(InternalsResourceStrings.Get("ErrorWitLinksNotSupported"));
          goto case LinkQueryMode.WorkItems;
      }
    }

    public virtual object FindField(string name, string prefix, object tableTag) => this.m_helper.FindField(name, prefix, tableTag);

    public DataType GetFieldDataType(object fieldTag)
    {
      if (this.m_helper.GetFieldId(fieldTag) == 100)
        return DataType.String;
      switch (this.m_helper.GetFieldType(fieldTag))
      {
        case InternalFieldType.String:
        case InternalFieldType.PlainText:
        case InternalFieldType.Html:
        case InternalFieldType.TreePath:
        case InternalFieldType.History:
          return DataType.String;
        case InternalFieldType.Integer:
        case InternalFieldType.Double:
          return DataType.Numeric;
        case InternalFieldType.DateTime:
          return DataType.Date;
        case InternalFieldType.Guid:
          return DataType.Guid;
        case InternalFieldType.Boolean:
          return DataType.Bool;
        default:
          return DataType.Unknown;
      }
    }

    public object FindVariable(string name, NodeParameters parameters)
    {
      if (this.Context.Contains((object) name))
        return (object) name;
      if (string.Equals(name, "me", StringComparison.OrdinalIgnoreCase))
        return (object) "me";
      if (string.Equals(name, "today", StringComparison.OrdinalIgnoreCase))
        return (object) "today";
      object variableValue = this.m_helper.GetVariableValue(name, parameters);
      if (variableValue == null)
        return (object) null;
      string key = Guid.NewGuid().ToString();
      this.Context.Add((object) key, variableValue);
      return (object) key;
    }

    public void ValidateParameters(
      string macroName,
      NodeTableName tableContext,
      NodeFieldName fieldContext,
      NodeParameters parameters)
    {
      this.m_helper.ValidateParameters(macroName, tableContext, fieldContext, parameters);
    }

    public DataType GetVariableDataType(string name)
    {
      if (string.Equals(name, "me", StringComparison.OrdinalIgnoreCase))
        return DataType.String;
      if (string.Equals(name, "today", StringComparison.OrdinalIgnoreCase))
        return DataType.Date;
      DataType variableType = this.m_helper.GetVariableType(name);
      if (variableType != DataType.Unknown || !this.Context.Contains((object) name))
        return variableType;
      object val = this.Context[(object) name];
      switch (val)
      {
        case DateTime _:
          return DataType.Date;
        case int _:
        case double _:
label_8:
          return DataType.Numeric;
        default:
          if (!this.IsListNodeNumber(val))
            return DataType.String;
          goto label_8;
      }
    }

    public bool DoesMacroExtensionHandleOffset(string macroName) => this.m_helper.DoesMacroExtensionHandleOffset(macroName);

    private bool IsListNodeNumber(object val) => val is NodeValueList && ((IEnumerable) val).Cast<object>().All<object>((Func<object, bool>) (n => n is NodeNumber));

    private bool IsListNodeString(object val) => val is NodeValueList && ((IEnumerable) val).Cast<object>().All<object>((Func<object, bool>) (n => n is NodeString));

    private void VerifyCondition(
      NodeFieldName left,
      Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node right,
      Condition op,
      Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node errorNode,
      LinkQueryMode mode)
    {
      Tools.EnsureSyntax(op != 0, SyntaxError.InvalidConditionalOperator, errorNode);
      this.m_helper.GetFieldReferenceName(left.Tag);
      int fieldId = this.m_helper.GetFieldId(left.Tag);
      InternalFieldType fieldType1 = this.m_helper.GetFieldType(left.Tag);
      if (right != null && right.NodeType == NodeType.FieldName)
      {
        if (!this.m_helper.IsSupported("QueryFieldsComparison"))
          throw new NotSupportedException(InternalsResourceStrings.Get("ErrorQueryFieldsComparisonNotSupported"));
        switch (mode)
        {
          case LinkQueryMode.LinksMustContain:
          case LinkQueryMode.LinksMayContain:
          case LinkQueryMode.LinksDoesNotContain:
          case LinkQueryMode.LinksRecursive:
          case LinkQueryMode.LinksRecursiveReturnMatchingChildren:
            Tools.EnsureSyntax(string.Equals(left.Prefix, ((NodeFieldName) right).Prefix, StringComparison.OrdinalIgnoreCase), SyntaxError.FieldConditionsInLinkQueries, errorNode);
            break;
        }
        Tools.EnsureSyntax(fieldType1 != InternalFieldType.PlainText && fieldType1 != InternalFieldType.Html && fieldType1 != InternalFieldType.History, SyntaxError.InvalidFieldTypeForCondition, errorNode);
        InternalFieldType fieldType2 = this.m_helper.GetFieldType(((NodeFieldName) right).Tag);
        int num;
        switch (fieldType2)
        {
          case InternalFieldType.PlainText:
          case InternalFieldType.Html:
            num = 0;
            break;
          default:
            num = fieldType2 != InternalFieldType.History ? 1 : 0;
            break;
        }
        Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node = errorNode;
        Tools.EnsureSyntax(num != 0, SyntaxError.InvalidFieldTypeForCondition, node);
      }
      else
      {
        switch (op)
        {
          case Condition.In:
            IEnumerator enumerator = right.GetEnumerator();
            try
            {
              while (enumerator.MoveNext())
              {
                Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node current = (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) enumerator.Current;
                this.VerifyCondition(left, current, Condition.Equals, errorNode, mode);
              }
              return;
            }
            finally
            {
              if (enumerator is IDisposable disposable)
                disposable.Dispose();
            }
          case Condition.Group:
            if (fieldId == 25)
            {
              if (!this.m_helper.IsSupported("WorkItemTypeCategories"))
                throw new NotSupportedException(InternalsResourceStrings.Get("ErrorWitCategoriesNotSupported"));
              break;
            }
            if (!this.m_helper.IsSupported("QueryInGroup"))
              throw new NotSupportedException(InternalsResourceStrings.Get("ErrorInGroupNotSupported"));
            if (fieldType1 != InternalFieldType.String)
              throw new NotSupportedException(InternalsResourceStrings.Format("ErrorWrongFieldTypeInInGroup", (object) this.m_helper.GetFieldFriendlyName(left.Tag), (object) ProvisionValues.FieldTypeString));
            break;
        }
        string constStringValue = right?.ConstStringValue;
        if (constStringValue != null && constStringValue.Length == 0)
          Tools.EnsureSyntax(op == Condition.Equals || op == Condition.NotEquals, SyntaxError.InvalidConditionForEmptyString, errorNode);
        if (fieldType1 == InternalFieldType.TreePath)
        {
          Tools.EnsureSyntax(op == Condition.Equals || op == Condition.NotEquals || op == Condition.Under, SyntaxError.InvalidConditionForTreeField, errorNode);
          Tools.EnsureSyntax(right.DataType == DataType.String, SyntaxError.PathMustBeAStringNotStartingWithBackslash, right);
          switch (constStringValue)
          {
            case null:
              goto label_46;
            case "":
              if (op != Condition.Under)
                goto label_46;
              else
                break;
          }
          Tools.EnsureSyntax(!constStringValue.StartsWith("\\", StringComparison.OrdinalIgnoreCase), SyntaxError.PathMustBeAStringNotStartingWithBackslash, right);
          TreeStructureType type = fieldId == -7 ? TreeStructureType.Area : TreeStructureType.Iteration;
          Tools.EnsureSyntax(this.GetTreeID(constStringValue, type) != -1, type == TreeStructureType.Area ? SyntaxError.AreaPathIsNotFoundInHierarchy : SyntaxError.IterationPathIsNotFoundInHierarchy, right);
        }
        else if (fieldType1 == InternalFieldType.PlainText || fieldType1 == InternalFieldType.History || fieldType1 == InternalFieldType.Html)
        {
          if (constStringValue != null)
          {
            Tools.EnsureSyntax(op == Condition.Contains || op == Condition.ContainsWords, SyntaxError.InvalidConditionForLongTextField, errorNode);
            Tools.EnsureSyntax(constStringValue.Trim().Length > 0, SyntaxError.InvalidLongTextSearchForWhitespace, errorNode);
          }
        }
        else if (fieldType1 == InternalFieldType.String && fieldId == -42)
        {
          Tools.EnsureSyntax(op == Condition.Equals || op == Condition.NotEquals, SyntaxError.InvalidConditionForNodeField, errorNode);
          switch (constStringValue)
          {
            case null:
              goto label_46;
            case "":
              if (op != Condition.Under)
                goto label_46;
              else
                break;
          }
          Tools.EnsureSyntax(constStringValue.IndexOf('\\') == -1, SyntaxError.InvalidProjectName, right);
          Tools.EnsureSyntax(this.GetTreeID(constStringValue, TreeStructureType.Area) != -1, SyntaxError.ProjectNotFound, right);
        }
        else if (fieldType1 == InternalFieldType.Integer && fieldId == 100)
        {
          Tools.EnsureSyntax(op == Condition.Equals || op == Condition.NotEquals, SyntaxError.InvalidConditionForLinkType, errorNode);
          switch (constStringValue)
          {
            case null:
              goto label_46;
            case "":
              if (op == Condition.NotEquals)
                goto label_46;
              else
                break;
          }
          Tools.EnsureSyntax(this.m_helper.HasLinkType(constStringValue), SyntaxError.InvalidLinkTypeName, right);
          if (mode == LinkQueryMode.LinksRecursive || mode == LinkQueryMode.LinksRecursiveReturnMatchingChildren)
            Tools.EnsureSyntax(this.m_helper.GetLinkTypeTopology(constStringValue) == 28 && this.m_helper.GetLinkTypeIsForward(constStringValue), SyntaxError.InvalidLinkTypeNameRecursive, right);
        }
        else if (fieldType1 == InternalFieldType.Integer && constStringValue != null && constStringValue.Length != 0)
          Tools.EnsureSyntax(Regex.IsMatch(constStringValue, "^[+-]?[0-9]+$"), SyntaxError.IncompatibleRightConst, right);
label_46:
        if (op == Condition.Under)
          Tools.EnsureSyntax(fieldType1 == InternalFieldType.TreePath, SyntaxError.UnderCanBeUsedForTreePathFieldOnly, (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) left);
        if ((op == Condition.IsEmpty || op == Condition.IsNotEmpty) && fieldType1 != InternalFieldType.PlainText && fieldType1 != InternalFieldType.History && fieldType1 != InternalFieldType.Html)
          throw new SyntaxException(InternalsResourceStrings.Format("IsEmptyValidWith"));
      }
    }

    public void VerifyNode(Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node, NodeTableName tableContext, NodeFieldName fieldContext)
    {
      TeamFoundationTrace.Verbose("--> VerifyNode()");
      if (node.NodeType == NodeType.FieldCondition)
      {
        NodeCondition errorNode = (NodeCondition) node;
        this.VerifyCondition(errorNode.Left, errorNode.Right, errorNode.Condition, (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) errorNode, (LinkQueryMode) tableContext.Tag);
      }
      else if (node.NodeType == NodeType.Ever)
      {
        NodeEverOperator nodeEverOperator = (NodeEverOperator) node;
        Tools.EnsureSyntax(nodeEverOperator.Value.NodeType == NodeType.FieldCondition, SyntaxError.TooComplexEverOperator, node);
        NodeCondition nodeCondition = (NodeCondition) nodeEverOperator.Value;
        Tools.EnsureSyntax(nodeCondition.Condition == Condition.Equals || nodeCondition.Condition == Condition.Contains || nodeCondition.Condition == Condition.ContainsWords, SyntaxError.EverNotEqualOperator, node);
        NodeFieldName left = nodeCondition.Left;
        if (left != null)
        {
          if (left.DataType == DataType.Date && this.m_dayPrecision)
            Tools.EnsureSyntax(nodeCondition.Right.ConstStringValue == string.Empty, SyntaxError.EverWithDatePrecision, node);
          Tools.EnsureSyntax((this.m_helper.GetFieldUsage(left.Tag) & InternalFieldUsages.WorkItem) != 0, SyntaxError.EverWithLinkQuery, node);
        }
      }
      else if (node.NodeType == NodeType.FieldList)
      {
        foreach (NodeFieldName nodeFieldName in node)
          Tools.EnsureSyntax(this.m_helper.GetFieldIsQueryable(nodeFieldName.Tag), SyntaxError.NonQueryableField, (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) nodeFieldName);
      }
      else if (node.NodeType == NodeType.OrderFieldList)
      {
        foreach (NodeFieldName nodeFieldName1 in node)
        {
          int fieldUsage = (int) this.m_helper.GetFieldUsage(nodeFieldName1.Tag);
          int fieldId = this.m_helper.GetFieldId(nodeFieldName1.Tag);
          if ((fieldUsage & 2) != 0)
            Tools.EnsureSyntax((LinkQueryMode) tableContext.Tag != LinkQueryMode.WorkItems, SyntaxError.OrderByLinkField, (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) nodeFieldName1);
          foreach (NodeFieldName nodeFieldName2 in node)
          {
            if (nodeFieldName2 != nodeFieldName1)
              Tools.EnsureSyntax(fieldId != this.m_helper.GetFieldId(nodeFieldName2.Tag), SyntaxError.DuplicateOrderByField, (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) nodeFieldName1);
            else
              break;
          }
          Tools.EnsureSyntax(this.m_helper.GetFieldCanSortBy(nodeFieldName1.Tag), SyntaxError.NonSortableField, (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) nodeFieldName1);
        }
      }
      else if (node.NodeType != NodeType.GroupFieldList)
      {
        if (node.NodeType == NodeType.Arithmetic)
        {
          NodeArithmetic nodeArithmetic = (NodeArithmetic) node;
          Tools.EnsureSyntax(nodeArithmetic.Left.CanCastTo(DataType.Date, this.CultureInfo) || nodeArithmetic.Left.CanCastTo(DataType.Numeric, this.CultureInfo) || nodeArithmetic.Left is NodeVariable && (nodeArithmetic.Left as NodeVariable).DoesMacroExtensionHandleOffset, SyntaxError.WrongTypeForArithmetic, node);
          Tools.EnsureSyntax(nodeArithmetic.Right.DataType == DataType.Numeric, SyntaxError.WrongTypeForArithmeticRightOperand, node);
        }
        else if (node.NodeType == NodeType.Mode)
        {
          LinkQueryMode queryMode = WiqlAdapter.GetQueryMode((NodeMode) node, tableContext);
          Tools.EnsureSyntax(queryMode != 0, SyntaxError.UnknownMode, node);
          tableContext.Tag = queryMode != LinkQueryMode.LinksRecursiveReturnMatchingChildren || this.m_helper.IsSupported("QueryRecursiveReturnMatchingChildren") ? (object) queryMode : throw new NotSupportedException("TODO: recursive return matching children is not supported on the server");
        }
        else if (node.NodeType == NodeType.Select)
        {
          NodeSelect nodeSelect = (NodeSelect) node;
          Tools.EnsureSyntax(nodeSelect.From.Tag != null, SyntaxError.ExpectingTableName, (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) nodeSelect);
          Tools.EnsureSyntax(nodeSelect.GroupBy == null, SyntaxError.GroupByIsNotSupported, (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) nodeSelect.GroupBy);
          if ((LinkQueryMode) nodeSelect.From.Tag == LinkQueryMode.LinksRecursive || (LinkQueryMode) nodeSelect.From.Tag == LinkQueryMode.LinksRecursiveReturnMatchingChildren)
            Tools.EnsureSyntax(nodeSelect.AsOf == null, SyntaxError.NotSupportedTreeQuery, nodeSelect.AsOf);
        }
      }
      TeamFoundationTrace.Verbose("<-- VerifyNode()");
    }

    public Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node OptimizeNode(
      Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node,
      NodeTableName tableContext,
      NodeFieldName fieldContext)
    {
      TeamFoundationTrace.Verbose("--> OptimizeNode()");
      if (node.NodeType == NodeType.FieldCondition)
      {
        NodeCondition condition = (NodeCondition) node;
        Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node rewritten;
        if (this.m_helper.RewriteCondition(condition, out rewritten))
          return rewritten.Optimize((IExternal) this, tableContext, fieldContext);
        if (condition.Condition == Condition.In)
        {
          NodeOrOperator nodeOrOperator = new NodeOrOperator();
          NodeValueList right1 = (NodeValueList) condition.Right;
          foreach (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node right2 in (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) (right1.OfType<NodeValueList>().FirstOrDefault<NodeValueList>() ?? right1))
            nodeOrOperator.Add((Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) new NodeCondition(Condition.Equals, condition.Left, right2));
          Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node1 = nodeOrOperator.Optimize((IExternal) this, tableContext, fieldContext);
          TeamFoundationTrace.Verbose("<-- OptimizeNode()");
          return node1;
        }
        if (condition.Condition == Condition.Contains || condition.Condition == Condition.ContainsWords)
        {
          string constStringValue = condition.Right.ConstStringValue;
          if (constStringValue != null && constStringValue.Length == 0)
          {
            NodeBoolConst nodeBoolConst = new NodeBoolConst(true);
            TeamFoundationTrace.Verbose("<-- OptimizeNode()");
            return (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) nodeBoolConst;
          }
        }
        NodeFieldName left = condition.Left;
        int fieldId = this.m_helper.GetFieldId(left.Tag);
        if (fieldId == -42)
        {
          string constStringValue = condition.Right.ConstStringValue;
          if (constStringValue != null)
          {
            node = (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) new NodeCondition(Condition.Under, new NodeFieldName(left.Prefix, this.m_helper.GetFieldFriendlyName("System.AreaPath")), (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) new NodeString(constStringValue));
            if (condition.Condition == Condition.NotEquals)
              node = (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) new NodeNotOperator(node);
            node.Bind((IExternal) this, tableContext, fieldContext);
          }
        }
        if (WiqlAdapter.IsNonNullableField(fieldId))
        {
          string constStringValue = condition.Right.ConstStringValue;
          if (constStringValue != null && constStringValue.Length == 0 && condition.Condition == Condition.NotEquals)
          {
            NodeBoolConst nodeBoolConst = new NodeBoolConst(true);
            TeamFoundationTrace.Verbose("<-- OptimizeNode()");
            return (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) nodeBoolConst;
          }
        }
        if (left.DataType == DataType.Date)
        {
          string constStringValue = condition.Right.ConstStringValue;
          if (!string.IsNullOrEmpty(constStringValue))
          {
            DateTime source = DateTime.Parse(constStringValue, (IFormatProvider) this.CultureInfo, DateTimeStyles.RoundtripKind);
            NodeString right3 = new NodeString(XmlConvert.ToString(this.ConvertDateTimeToUtc(source), XmlDateTimeSerializationMode.Utc));
            if ((this.m_dayPrecision || this.m_isAtTodayClause) && !this.m_isDateBasedMacro)
            {
              if (!this.m_isAtTodayClause)
                Tools.EnsureSyntax(source.TimeOfDay == TimeSpan.Zero, SyntaxError.NonZeroTime, node);
              bool flag = false;
              NodeString right4;
              if (source > WiqlAdapter.s_startOfLastDay)
              {
                right4 = new NodeString(XmlConvert.ToString(DateTime.MaxValue, XmlDateTimeSerializationMode.Utc));
                flag = true;
              }
              else
                right4 = new NodeString(XmlConvert.ToString(this.ConvertDateTimeToUtc(source + new TimeSpan(1, 0, 0, 0)), XmlDateTimeSerializationMode.Utc));
              switch (condition.Condition)
              {
                case Condition.Equals:
                  if (flag)
                  {
                    condition.Condition = Condition.GreaterOrEquals;
                    condition.Right = (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) right3;
                    break;
                  }
                  NodeAndOperator nodeAndOperator = new NodeAndOperator();
                  nodeAndOperator.Add((Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) new NodeCondition(Condition.GreaterOrEquals, condition.Left, (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) right3));
                  nodeAndOperator.Add((Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) new NodeCondition(Condition.Less, condition.Left, (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) right4));
                  node = (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) nodeAndOperator;
                  node.Bind((IExternal) this, tableContext, fieldContext);
                  break;
                case Condition.NotEquals:
                  if (flag)
                  {
                    condition.Condition = Condition.Less;
                    condition.Right = (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) right3;
                    break;
                  }
                  NodeOrOperator nodeOrOperator = new NodeOrOperator();
                  nodeOrOperator.Add((Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) new NodeCondition(Condition.Less, condition.Left, (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) right3));
                  nodeOrOperator.Add((Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) new NodeCondition(Condition.GreaterOrEquals, condition.Left, (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) right4));
                  node = (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) nodeOrOperator;
                  node.Bind((IExternal) this, tableContext, fieldContext);
                  break;
                case Condition.Less:
                case Condition.GreaterOrEquals:
                  condition.Right = (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) right3;
                  break;
                case Condition.Greater:
                  if (flag)
                  {
                    NodeBoolConst nodeBoolConst = new NodeBoolConst(false);
                    TeamFoundationTrace.Verbose("<-- OptimizeNode()");
                    return (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) nodeBoolConst;
                  }
                  condition.Condition = Condition.GreaterOrEquals;
                  condition.Right = (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) right4;
                  break;
                case Condition.LessOrEquals:
                  if (flag)
                  {
                    NodeBoolConst nodeBoolConst = new NodeBoolConst(true);
                    TeamFoundationTrace.Verbose("<-- OptimizeNode()");
                    return (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) nodeBoolConst;
                  }
                  condition.Condition = Condition.Less;
                  condition.Right = (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) right4;
                  break;
                default:
                  Tools.EnsureSyntax(false, SyntaxError.InvalidConditionalOperator, node);
                  break;
              }
            }
            else
              condition.Right = (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) right3;
          }
        }
        this.m_isAtTodayClause = false;
        this.m_isDateBasedMacro = false;
      }
      else if (node.NodeType == NodeType.Variable)
      {
        NodeVariable nodeVariable = (NodeVariable) node;
        object variableValue = nodeVariable.GetVariableValue((IExternal) this, this.Context);
        switch (variableValue)
        {
          case null:
            if (object.Equals(nodeVariable.Tag, (object) "me"))
            {
              NodeString nodeString = new NodeString(this.m_helper.UserDisplayName);
              TeamFoundationTrace.Verbose("<-- OptimizeNode()");
              return (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) nodeString;
            }
            if (object.Equals(nodeVariable.Tag, (object) "today"))
            {
              NodeString nodeString = new NodeString(XmlConvert.ToString(this.TimeZone.ToLocalTime(DateTime.UtcNow).Date, XmlDateTimeSerializationMode.Local));
              TeamFoundationTrace.Verbose("<-- OptimizeNode()");
              this.m_isAtTodayClause = true;
              return (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) nodeString;
            }
            break;
          case int _:
          case double _:
            NodeNumber nodeNumber = new NodeNumber(variableValue.ToString());
            TeamFoundationTrace.Verbose("<-- OptimizeNode()");
            return (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) nodeNumber;
          case DateTime dateTime:
            this.m_isDateBasedMacro = true;
            NodeString nodeString1 = new NodeString(XmlConvert.ToString(dateTime, XmlDateTimeSerializationMode.Local));
            TeamFoundationTrace.Verbose("<-- OptimizeNode() - DateTime");
            return (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) nodeString1;
          default:
            if (this.IsListNodeNumber(variableValue) || this.IsListNodeString(variableValue))
            {
              Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node2 = (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) variableValue;
              TeamFoundationTrace.Verbose("<-- OptimizeNode()");
              return node2;
            }
            NodeString nodeString2 = new NodeString(variableValue.ToString());
            TeamFoundationTrace.Verbose("<-- OptimizeNode()");
            return (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) nodeString2;
        }
      }
      else if (node.NodeType == NodeType.Arithmetic)
      {
        NodeArithmetic nodeArithmetic = (NodeArithmetic) node;
        NodeNumber right = (NodeNumber) nodeArithmetic.Right;
        if (nodeArithmetic.Left is NodeVariable)
        {
          NodeVariable left = (NodeVariable) nodeArithmetic.Left;
          double num = double.Parse(right.Value);
          if (nodeArithmetic.Arithmetic == Arithmetic.Subtract)
            num = -num;
          left.Parameters.Offset = num;
          return this.OptimizeNode((Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) left, tableContext, fieldContext);
        }
        if (fieldContext != null && fieldContext.DataType == DataType.Numeric)
        {
          double num1 = double.Parse(((NodeItem) nodeArithmetic.Left).Value, (IFormatProvider) CultureInfo.InvariantCulture);
          double num2 = double.Parse(right.Value, (IFormatProvider) CultureInfo.InvariantCulture);
          double num3;
          switch (nodeArithmetic.Arithmetic)
          {
            case Arithmetic.Add:
              num3 = num1 + num2;
              break;
            case Arithmetic.Subtract:
              num3 = num1 - num2;
              break;
            default:
              throw new NotImplementedException();
          }
          NodeNumber nodeNumber = new NodeNumber(XmlConvert.ToString(num3));
          TeamFoundationTrace.Verbose("<-- OptimizeNode()");
          return (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) nodeNumber;
        }
        if (fieldContext != null && fieldContext.DataType == DataType.Date)
        {
          DateTime dateTime = DateTime.Parse(((NodeItem) nodeArithmetic.Left).Value, (IFormatProvider) this.CultureInfo, DateTimeStyles.RoundtripKind);
          TimeSpan timeSpan = new TimeSpan((long) (double.Parse(right.Value, (IFormatProvider) CultureInfo.InvariantCulture) * 864000000000.0 + 0.5));
          DateTime source;
          switch (nodeArithmetic.Arithmetic)
          {
            case Arithmetic.Add:
              source = dateTime + timeSpan;
              break;
            case Arithmetic.Subtract:
              source = dateTime - timeSpan;
              break;
            default:
              throw new NotImplementedException();
          }
          NodeString nodeString = new NodeString(XmlConvert.ToString(this.ConvertDateTimeToUtc(source), XmlDateTimeSerializationMode.Utc));
          TeamFoundationTrace.Verbose("<-- OptimizeNode()");
          return (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) nodeString;
        }
      }
      TeamFoundationTrace.Verbose("<-- OptimizeNode(). Node not optimized");
      return node;
    }

    private string GetConditionalOperator(int c, bool not, bool ever, Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node errorNode)
    {
      string conditionalOperator = WiqlAdapter.s_Operators[c * 4 + (ever ? 2 : 0) + (not ? 1 : 0)];
      Tools.EnsureSyntax(conditionalOperator != null, SyntaxError.InvalidConditionalOperator, errorNode);
      return conditionalOperator;
    }

    private XmlElement QueryXml(Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node, XmlDocument doc, bool not, bool ever, bool num)
    {
      switch (node.NodeType)
      {
        case NodeType.Number:
          XmlElement element1 = doc.CreateElement(num ? "Number" : "String");
          element1.AppendChild((XmlNode) doc.CreateTextNode(((NodeItem) node).Value));
          return element1;
        case NodeType.String:
          XmlElement element2 = doc.CreateElement("String");
          element2.AppendChild((XmlNode) doc.CreateTextNode(((NodeItem) node).Value));
          return element2;
        case NodeType.FieldName:
          XmlElement element3 = doc.CreateElement("Column");
          element3.AppendChild((XmlNode) doc.CreateTextNode(this.m_helper.GetFieldReferenceName(((NodeFieldName) node).Tag)));
          return element3;
        case NodeType.FieldCondition:
          NodeCondition errorNode = (NodeCondition) node;
          NodeFieldName left = errorNode.Left;
          int num1 = this.m_helper.GetFieldId(left.Tag);
          string fieldName = this.m_helper.GetFieldReferenceName(left.Tag);
          Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node1 = errorNode.Right;
          switch (num1)
          {
            case -105:
              string constStringValue1 = node1.ConstStringValue;
              Tools.EnsureSyntax(constStringValue1 != null, SyntaxError.ExpectingValue, node1);
              num1 = -104;
              fieldName = "System.IterationId";
              if (constStringValue1.Length != 0)
              {
                int treeId = this.GetTreeID(constStringValue1, TreeStructureType.Iteration);
                Tools.EnsureSyntax(treeId != -1, SyntaxError.IterationPathIsNotFoundInHierarchy, node1);
                node1 = (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) new NodeNumber(XmlConvert.ToString(treeId));
                break;
              }
              break;
            case -7:
              string constStringValue2 = node1.ConstStringValue;
              num1 = -2;
              fieldName = "System.AreaId";
              if (constStringValue2.Length != 0)
              {
                int treeId = this.GetTreeID(constStringValue2, TreeStructureType.Area);
                Tools.EnsureSyntax(treeId != -1, SyntaxError.AreaPathIsNotFoundInHierarchy, node1);
                node1 = (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) new NodeNumber(XmlConvert.ToString(treeId));
                break;
              }
              break;
            case 100:
              Tools.EnsureSyntax(this.m_helper.HasLinkType(node1.ConstStringValue), SyntaxError.InvalidLinkTypeName, node1);
              node1 = (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) new NodeNumber(XmlConvert.ToString(this.m_helper.GetLinkTypeId(node1.ConstStringValue)));
              break;
          }
          XmlElement element4 = doc.CreateElement("Expression");
          element4.SetAttribute("Column", fieldName);
          element4.SetAttribute("FieldType", XmlConvert.ToString(this.m_helper.GetFieldPsFieldType(fieldName)));
          if (errorNode.Condition == Condition.Contains || errorNode.Condition == Condition.ContainsWords)
          {
            int condition = (int) errorNode.Condition;
            string constStringValue3 = errorNode.Right.ConstStringValue;
            if (num1 == 54)
              ever = true;
            element4.SetAttribute("Operator", this.GetConditionalOperator(condition, not, ever, (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) errorNode));
            XmlElement element5 = doc.CreateElement("String");
            element5.AppendChild((XmlNode) doc.CreateTextNode(constStringValue3));
            element4.AppendChild((XmlNode) element5);
          }
          else if (errorNode.Condition == Condition.Group)
          {
            element4.SetAttribute("Operator", this.GetConditionalOperator((int) errorNode.Condition, not, ever, (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) errorNode));
            element4.SetAttribute("ExpandConstant", bool.TrueString);
            string constStringValue4 = errorNode.Right.ConstStringValue;
            XmlElement element6 = doc.CreateElement("String");
            element6.AppendChild((XmlNode) doc.CreateTextNode(constStringValue4));
            element4.AppendChild((XmlNode) element6);
          }
          else
          {
            element4.SetAttribute("Operator", this.GetConditionalOperator((int) errorNode.Condition, not, ever, (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) errorNode));
            if (node1.NodeType == NodeType.FieldName)
              element4.AppendChild((XmlNode) this.QueryXml(node1, doc, not, ever, false));
            else if (left.DataType == DataType.Date)
            {
              string constStringValue5 = node1.ConstStringValue;
              Tools.EnsureSyntax(constStringValue5 != null, SyntaxError.ExpectingValue, node1);
              if (constStringValue5.Length != 0)
                constStringValue5 = XmlConvert.ToString(DateTime.Parse(constStringValue5, (IFormatProvider) this.CultureInfo), XmlDateTimeSerializationMode.Utc);
              XmlElement element7 = doc.CreateElement("DateTime");
              element7.AppendChild((XmlNode) doc.CreateTextNode(constStringValue5));
              element4.AppendChild((XmlNode) element7);
            }
            else if (left.DataType == DataType.Guid)
            {
              XmlElement element8 = doc.CreateElement("Guid");
              element8.InnerText = node1.ConstStringValue;
              element4.AppendChild((XmlNode) element8);
            }
            else
            {
              num = this.m_helper.GetFieldType(fieldName) == InternalFieldType.Integer;
              element4.AppendChild((XmlNode) this.QueryXml(node1, doc, not, ever, num));
            }
          }
          return element4;
        case NodeType.BoolConst:
          string str1 = ((NodeBoolConst) node).Value == not ? "equals" : "notequals";
          XmlElement element9 = doc.CreateElement("Expression");
          element9.SetAttribute("Column", "System.Id");
          element9.SetAttribute("Operator", str1);
          XmlElement element10 = doc.CreateElement("Number");
          element10.AppendChild((XmlNode) doc.CreateTextNode("0"));
          element9.AppendChild((XmlNode) element10);
          return element9;
        case NodeType.BoolValue:
          XmlElement element11 = doc.CreateElement("Number");
          element11.AppendChild((XmlNode) doc.CreateTextNode(((NodeBoolValue) node).BoolValue ? "1" : "0"));
          return element11;
        case NodeType.Not:
          return this.QueryXml(((NodeNotOperator) node).Value, doc, !not, ever, num);
        case NodeType.Ever:
          return this.QueryXml(((NodeEverOperator) node).Value, doc, not, true, num);
        case NodeType.And:
        case NodeType.Or:
          string str2 = node.NodeType == NodeType.Or == not ? "and" : "or";
          XmlElement element12 = doc.CreateElement("Group");
          element12.SetAttribute("GroupOperator", str2);
          int count = node.Count;
          for (int i = 0; i < count; ++i)
            element12.AppendChild((XmlNode) this.QueryXml(node[i], doc, not, ever, num));
          return element12;
        default:
          Tools.EnsureSyntax(false, SyntaxError.InvalidNodeType, node);
          return (XmlElement) null;
      }
    }

    public Dictionary<int, bool> ComputeLinkTypes(Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node)
    {
      TeamFoundationTrace.Verbose("--> ComputeLinkTypes()");
      switch (node.NodeType)
      {
        case NodeType.FieldCondition:
          NodeCondition nodeCondition = (NodeCondition) node;
          if (this.m_helper.GetFieldId(nodeCondition.Left.Tag) == 100)
          {
            Tools.EnsureSyntax(nodeCondition.Right.ConstStringValue != null, SyntaxError.InvalidConditionForLinkType, (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) nodeCondition);
            Tools.EnsureSyntax(this.m_helper.HasLinkType(nodeCondition.Right.ConstStringValue), SyntaxError.InvalidLinkTypeName, nodeCondition.Right);
            int linkTypeId = this.m_helper.GetLinkTypeId(nodeCondition.Right.ConstStringValue);
            if (nodeCondition.Condition == Condition.Equals)
            {
              Dictionary<int, bool> linkTypes = new Dictionary<int, bool>();
              linkTypes.Add(linkTypeId, true);
              TeamFoundationTrace.Verbose("<-- ComputeLinkTypes()");
              return linkTypes;
            }
            if (nodeCondition.Condition == Condition.NotEquals)
            {
              Dictionary<int, bool> linkTypes = new Dictionary<int, bool>();
              foreach (int allLinkTypeId in this.m_helper.GetAllLinkTypeIds())
              {
                if (allLinkTypeId != linkTypeId)
                  linkTypes.Add(allLinkTypeId, true);
              }
              TeamFoundationTrace.Verbose("<-- ComputeLinkTypes()");
              return linkTypes;
            }
            Tools.EnsureSyntax(false, SyntaxError.InvalidConditionForLinkType, (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) nodeCondition);
          }
          TeamFoundationTrace.Verbose("<-- ComputeLinkTypes()");
          return (Dictionary<int, bool>) null;
        case NodeType.Not:
          Dictionary<int, bool> linkTypes1 = this.ComputeLinkTypes(((NodeNotOperator) node).Value);
          if (linkTypes1 != null)
          {
            Dictionary<int, bool> linkTypes2 = new Dictionary<int, bool>();
            foreach (int allLinkTypeId in this.m_helper.GetAllLinkTypeIds())
            {
              if (!linkTypes1.ContainsKey(allLinkTypeId))
                linkTypes2.Add(allLinkTypeId, true);
            }
            TeamFoundationTrace.Verbose("<-- ComputeLinkTypes()");
            return linkTypes2;
          }
          TeamFoundationTrace.Verbose("<-- ComputeLinkTypes()");
          return (Dictionary<int, bool>) null;
        case NodeType.And:
          NodeAndOperator nodeAndOperator = (NodeAndOperator) node;
          Dictionary<int, bool> linkTypes3 = (Dictionary<int, bool>) null;
          for (int i = 0; i < nodeAndOperator.Count; ++i)
          {
            Dictionary<int, bool> linkTypes4 = this.ComputeLinkTypes(nodeAndOperator[i]);
            if (linkTypes3 == null)
              linkTypes3 = linkTypes4;
            else if (linkTypes4 != null)
            {
              Dictionary<int, bool> dictionary = new Dictionary<int, bool>();
              foreach (int key in linkTypes4.Keys)
              {
                if (linkTypes3.ContainsKey(key))
                  dictionary.Add(key, true);
              }
              linkTypes3 = dictionary;
            }
          }
          TeamFoundationTrace.Verbose("<-- ComputeLinkTypes()");
          return linkTypes3;
        case NodeType.Or:
          NodeOrOperator nodeOrOperator = (NodeOrOperator) node;
          Dictionary<int, bool> linkTypes5 = (Dictionary<int, bool>) null;
          for (int i = 0; i < nodeOrOperator.Count; ++i)
          {
            Dictionary<int, bool> linkTypes6 = this.ComputeLinkTypes(nodeOrOperator[i]);
            if (linkTypes5 == null)
              linkTypes5 = linkTypes6;
            else if (linkTypes6 != null)
            {
              foreach (int key in linkTypes6.Keys)
                linkTypes5[key] = true;
            }
          }
          TeamFoundationTrace.Verbose("<-- ComputeLinkTypes()");
          return linkTypes5;
        default:
          Tools.EnsureSyntax(false, SyntaxError.InvalidConditionForLinkType, node);
          TeamFoundationTrace.Verbose("<-- ComputeLinkTypes()");
          return (Dictionary<int, bool>) null;
      }
    }

    private DateTime ConvertDateTimeToUtc(DateTime source)
    {
      if (source.Kind == DateTimeKind.Utc)
        return source;
      try
      {
        return DateTime.SpecifyKind(source - this.TimeZone.GetUtcOffset(source), DateTimeKind.Utc);
      }
      catch (ArgumentOutOfRangeException ex)
      {
        TeamFoundationTrace.Verbose(string.Format("Unable to convert the time {0} to UTC", (object) source));
        return source;
      }
    }

    public DateTime GetAsOfUtc(NodeSelect nodeSelect)
    {
      DateTime result;
      return nodeSelect.AsOf == null || !DateTime.TryParse(((NodeItem) nodeSelect.AsOf).Value, (IFormatProvider) this.CultureInfo, DateTimeStyles.RoundtripKind, out result) && !DateTime.TryParse(((NodeItem) nodeSelect.AsOf).Value, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out result) ? DateTime.MinValue : this.ConvertDateTimeToUtc(result);
    }

    public XmlElement GetQueryXml(NodeSelect nodeSelect)
    {
      TeamFoundationTrace.Verbose("--> GetQueryXml()");
      TeamFoundationTrace.Verbose("WIQL: {0}", (object) nodeSelect.ToString());
      Tools.EnsureSyntax((LinkQueryMode) nodeSelect.From.Tag == LinkQueryMode.WorkItems, SyntaxError.IncorrectQueryMethod, (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) nodeSelect.From);
      XmlDocument doc = new XmlDocument();
      doc.PreserveWhitespace = true;
      XmlElement element = doc.CreateElement("Query");
      element.SetAttribute("Product", string.Empty);
      Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node = nodeSelect.Where ?? (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) new NodeBoolConst(true);
      element.AppendChild((XmlNode) this.QueryXml(node, doc, false, false, false));
      DateTime asOfUtc = this.GetAsOfUtc(nodeSelect);
      if (asOfUtc != DateTime.MinValue)
        element.SetAttribute("AsOf", XmlConvert.ToString(asOfUtc, XmlDateTimeSerializationMode.Utc));
      TeamFoundationTrace.Verbose("<-- GetQueryXml()");
      TeamFoundationTrace.Verbose("QueryXml: {0}", (object) element.OuterXml);
      return element;
    }

    public XmlElement GetQueryXml(
      string wiql,
      IDictionary context,
      bool isLinkQuery,
      bool dayPrecision)
    {
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.PreserveWhitespace = true;
      XmlElement queryXml = !isLinkQuery ? xmlDocument.CreateElement("Query") : xmlDocument.CreateElement("LinksQuery");
      queryXml.SetAttribute("Product", string.Empty);
      XmlElement element1 = xmlDocument.CreateElement("Wiql");
      element1.AppendChild((XmlNode) xmlDocument.CreateTextNode(wiql));
      queryXml.AppendChild((XmlNode) element1);
      XmlElement element2 = xmlDocument.CreateElement(QueryXmlConstants.DayPrecision);
      element2.AppendChild((XmlNode) xmlDocument.CreateTextNode(dayPrecision.ToString()));
      queryXml.AppendChild((XmlNode) element2);
      if (context != null)
      {
        foreach (DictionaryEntry dictionaryEntry in context)
        {
          if (dictionaryEntry.Key != null && dictionaryEntry.Value != null)
          {
            XmlElement element3 = xmlDocument.CreateElement("Context");
            element3.SetAttribute("Key", dictionaryEntry.Key.ToString());
            element3.SetAttribute("Value", dictionaryEntry.Value.ToString());
            element3.SetAttribute(QueryXmlConstants.ValueType, this.GetContextValueType(dictionaryEntry.Value));
            queryXml.AppendChild((XmlNode) element3);
          }
        }
      }
      return queryXml;
    }

    private string GetContextValueType(object val)
    {
      switch (val)
      {
        case DateTime _:
          return "DateTime";
        case int _:
          return "Number";
        case double _:
          return "Double";
        default:
          return "String";
      }
    }

    public XmlElement GetLinkQueryXml(NodeSelect nodeSelect, out NodeAndOperator linkGroup)
    {
      TeamFoundationTrace.Verbose("--> GetLinkQueryXml()");
      TeamFoundationTrace.Verbose("WIQL: {0}", (object) nodeSelect.ToString());
      LinkQueryMode tag = (LinkQueryMode) nodeSelect.From.Tag;
      string str = (string) null;
      switch (tag)
      {
        case LinkQueryMode.LinksMustContain:
          str = "mustcontain";
          break;
        case LinkQueryMode.LinksMayContain:
        case LinkQueryMode.LinksRecursive:
        case LinkQueryMode.LinksRecursiveReturnMatchingChildren:
          str = "maycontain";
          break;
        case LinkQueryMode.LinksDoesNotContain:
          str = "doesnotcontain";
          break;
        default:
          Tools.EnsureSyntax(false, SyntaxError.IncorrectQueryMethod, (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) nodeSelect.From);
          break;
      }
      XmlDocument doc = new XmlDocument();
      doc.PreserveWhitespace = true;
      XmlElement element1 = doc.CreateElement("LinksQuery");
      element1.SetAttribute("Type", str);
      Dictionary<string, NodeAndOperator> whereGroups = nodeSelect.GetWhereGroups();
      NodeAndOperator nodeAndOperator1 = (NodeAndOperator) null;
      whereGroups.TryGetValue("Source", out nodeAndOperator1);
      if (nodeAndOperator1 != null)
      {
        XmlElement element2 = doc.CreateElement("LeftQuery");
        element2.AppendChild((XmlNode) this.QueryXml((Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) nodeAndOperator1, doc, false, false, false));
        element1.AppendChild((XmlNode) element2);
      }
      whereGroups.TryGetValue(string.Empty, out linkGroup);
      if (tag == LinkQueryMode.LinksRecursive || tag == LinkQueryMode.LinksRecursiveReturnMatchingChildren)
      {
        Tools.EnsureSyntax(linkGroup != null, SyntaxError.TreeQueryNeedsOneLinkType, (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) linkGroup);
        Dictionary<int, bool> linkTypes = this.ComputeLinkTypes((Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) linkGroup);
        Tools.EnsureSyntax(linkTypes.Count == 1, SyntaxError.TreeQueryNeedsOneLinkType, (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) linkGroup);
        foreach (int key in linkTypes.Keys)
          element1.SetAttribute("RecursionID", XmlConvert.ToString(key));
        if (tag == LinkQueryMode.LinksRecursiveReturnMatchingChildren)
          element1.SetAttribute("ReturnMatchingParents", XmlConvert.ToString(false));
      }
      else if (linkGroup != null)
      {
        XmlElement element3 = doc.CreateElement("LinkQuery");
        element3.AppendChild((XmlNode) this.QueryXml((Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) linkGroup, doc, false, false, false));
        element1.AppendChild((XmlNode) element3);
      }
      NodeAndOperator nodeAndOperator2 = (NodeAndOperator) null;
      whereGroups.TryGetValue("Target", out nodeAndOperator2);
      if (nodeAndOperator2 != null)
      {
        XmlElement element4 = doc.CreateElement("RightQuery");
        element4.AppendChild((XmlNode) this.QueryXml((Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) nodeAndOperator2, doc, false, false, false));
        element1.AppendChild((XmlNode) element4);
      }
      DateTime asOfUtc = this.GetAsOfUtc(nodeSelect);
      if (asOfUtc != DateTime.MinValue)
        element1.SetAttribute("AsOf", XmlConvert.ToString(asOfUtc, XmlDateTimeSerializationMode.Utc));
      TeamFoundationTrace.Verbose("<-- GetLinkQueryXml()");
      TeamFoundationTrace.Verbose("QueryXml: {0}", (object) element1.OuterXml);
      return element1;
    }

    public NodeAndOperator GetLinkGroup(NodeSelect nodeSelect)
    {
      NodeAndOperator linkGroup = (NodeAndOperator) null;
      nodeSelect.GetWhereGroups().TryGetValue(string.Empty, out linkGroup);
      return linkGroup;
    }

    public List<object> GetDisplayFieldList(NodeSelect nodeSelect) => this.m_helper.GetDisplayFieldList(nodeSelect);

    public List<object> GetSortFieldList(NodeSelect nodeSelect) => this.m_helper.GetSortFieldList(nodeSelect);

    public void SetDisplayFieldList(NodeSelect nodeSelect, IEnumerable<object> list) => this.m_helper.SetDisplayFieldList(nodeSelect, list);

    public void SetSortFieldList(NodeSelect nodeSelect, IEnumerable<object> list) => this.m_helper.SetSortFieldList(nodeSelect, list);

    public static bool IsNonNullableField(int fieldId) => fieldId == 25 || fieldId == 2 || fieldId == 100;

    private static LinkQueryMode GetQueryMode(NodeTableName tableContext)
    {
      string a = tableContext.Value;
      if (a != null)
      {
        if (string.Equals(a, "issue", StringComparison.OrdinalIgnoreCase) || string.Equals(a, "issues", StringComparison.OrdinalIgnoreCase) || string.Equals(a, "workitem", StringComparison.OrdinalIgnoreCase) || string.Equals(a, "WorkItems", StringComparison.OrdinalIgnoreCase))
          return LinkQueryMode.WorkItems;
        if (string.Equals(a, "links", StringComparison.OrdinalIgnoreCase) || string.Equals(a, "WorkItemLinks", StringComparison.OrdinalIgnoreCase))
          return LinkQueryMode.LinksMustContain;
      }
      return LinkQueryMode.Unknown;
    }

    private static LinkQueryMode GetQueryMode(NodeMode nodeMode, NodeTableName tableContext)
    {
      LinkQueryMode queryMode = WiqlAdapter.GetQueryMode(tableContext);
      if (nodeMode != null)
      {
        Tools.EnsureSyntax(queryMode > LinkQueryMode.WorkItems, SyntaxError.ModeOnWorkItems, (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) nodeMode);
        int num = 0;
        bool flag = true;
        for (int i = 0; i < nodeMode.Count; ++i)
        {
          string a = ((NodeItem) nodeMode[i]).Value;
          if (string.Equals(a, "MustContain", StringComparison.OrdinalIgnoreCase))
            num |= 1;
          else if (string.Equals(a, "MayContain", StringComparison.OrdinalIgnoreCase))
            num |= 2;
          else if (string.Equals(a, "DoesNotContain", StringComparison.OrdinalIgnoreCase))
            num |= 4;
          else if (string.Equals(a, "Recursive", StringComparison.OrdinalIgnoreCase))
            num |= 8;
          else if (string.Equals(a, "ReturnMatchingChildren", StringComparison.OrdinalIgnoreCase))
            flag = false;
          else
            num = -1;
        }
        switch (num)
        {
          case 0:
          case 1:
            return LinkQueryMode.LinksMustContain;
          case 2:
            return LinkQueryMode.LinksMayContain;
          case 4:
            return LinkQueryMode.LinksDoesNotContain;
          case 8:
          case 10:
            return !flag ? LinkQueryMode.LinksRecursiveReturnMatchingChildren : LinkQueryMode.LinksRecursive;
          default:
            queryMode = LinkQueryMode.Unknown;
            break;
        }
      }
      return queryMode;
    }

    public static LinkQueryMode GetQueryMode(NodeSelect nodeSelect)
    {
      Tools.EnsureSyntax(nodeSelect.From != null, SyntaxError.FromIsNotSpecified, (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) null);
      return WiqlAdapter.GetQueryMode(nodeSelect.Mode, nodeSelect.From);
    }
  }
}
