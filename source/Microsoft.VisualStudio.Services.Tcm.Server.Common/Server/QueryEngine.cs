// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.QueryEngine
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal abstract class QueryEngine
  {
    private bool m_dayPrecision;
    private QueryEngine.QueryEnvironment m_environment;
    private Func<string, string, string> m_conditionNodeValueTranslator;
    private SortedList<string, int> m_groups;
    private int m_nextListId;
    private NodeSelect m_root;
    private string m_queryText;
    private StringBuilder m_sql;
    private string m_teamProjectName;
    private TimeZoneInfo m_timeZone;
    private string m_userName;
    private List<string> m_additionalClauses = new List<string>();

    internal QueryEngine(
      ResultsStoreQuery query,
      string userName,
      Func<string, string, string> conditionNodeValueTranslator,
      string expectedServiceName,
      List<Tuple<Type, string, string>> tables)
    {
      ArgumentUtility.CheckForNull<ResultsStoreQuery>(query, nameof (query), expectedServiceName);
      this.m_queryText = query.QueryText;
      this.m_userName = userName;
      this.m_teamProjectName = query.TeamProjectName;
      this.m_dayPrecision = query.DayPrecision;
      try
      {
        this.m_timeZone = TimeZoneInfo.FromSerializedString(query.TimeZone);
      }
      catch (SerializationException ex)
      {
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidFieldValue, (object) "TimeZone")).Expected(expectedServiceName);
      }
      this.AllowRecursion = true;
      this.m_environment = new QueryEngine.QueryEnvironment(this, tables, this.StartTime);
      this.m_conditionNodeValueTranslator = conditionNodeValueTranslator;
    }

    internal QueryEngine(ResultsStoreQuery query, string userName, string expectedServiceName)
      : this(query, userName, (Func<string, string, string>) null, expectedServiceName, (List<Tuple<Type, string, string>>) null)
    {
    }

    internal QueryEngine(
      ResultsStoreQuery query,
      string userName,
      string expectedServiceName,
      List<Tuple<Type, string, string>> tables)
      : this(query, userName, (Func<string, string, string>) null, expectedServiceName, tables)
    {
    }

    protected virtual DateTime StartTime => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, this.m_timeZone);

    internal void AppendClause(string clause) => this.m_additionalClauses.Add(clause);

    internal bool AllowRecursion { get; set; }

    protected void Prepare()
    {
      try
      {
        this.m_root = Parser.ParseSyntax(this.m_queryText);
        this.m_root.Bind((IExternal) this.m_environment, (NodeTableName) null, (NodeFieldName) null);
        this.m_root = (NodeSelect) this.m_root.Optimize((IExternal) this.m_environment, (NodeTableName) null, (NodeFieldName) null);
      }
      catch (SyntaxException ex)
      {
        throw new TestManagementValidationException(ex.Details);
      }
    }

    public string GenerateWhereClause(int dataspaceId) => this.GenerateWhereClause(false, dataspaceId);

    public string GenerateWhereClauseInMultipleProjects() => this.GenerateWhereClause(true);

    protected virtual string GenerateWhereClause(bool isMultipleProjects, int dataspaceId = 0)
    {
      this.AddAppendedClauses();
      if (this.m_root.Where != null)
      {
        this.m_sql = new StringBuilder();
        try
        {
          this.EmitNode(this.m_root.Where);
          this.m_sql.Insert(0, "WHERE (");
          this.m_sql.Append(")");
          if (!isMultipleProjects)
          {
            this.m_sql.Append(" AND [DataspaceId] = ");
            this.m_sql.Append(dataspaceId);
          }
          return this.m_sql.ToString();
        }
        finally
        {
          this.m_sql = (StringBuilder) null;
        }
      }
      else
        return !isMultipleProjects ? "WHERE [DataspaceId] = " + dataspaceId.ToString() : string.Empty;
    }

    private void AddAppendedClauses()
    {
      if (this.m_additionalClauses.Count <= 0)
        return;
      bool flag = true;
      StringBuilder stringBuilder = new StringBuilder("SELECT * FROM ");
      stringBuilder.Append(this.m_root.From.Value);
      stringBuilder.Append(" WHERE ");
      foreach (string additionalClause in this.m_additionalClauses)
      {
        if (!flag)
          stringBuilder.Append(" AND ");
        else
          flag = false;
        stringBuilder.Append("(" + additionalClause + ")");
      }
      NodeSelect nodeSelect;
      try
      {
        NodeSelect syntax = Parser.ParseSyntax(stringBuilder.ToString());
        syntax.Bind((IExternal) this.m_environment, (NodeTableName) null, (NodeFieldName) null);
        nodeSelect = (NodeSelect) syntax.Optimize((IExternal) this.m_environment, (NodeTableName) null, (NodeFieldName) null);
      }
      catch (SyntaxException ex)
      {
        throw new TestManagementValidationException(ex.Details);
      }
      if (this.m_root.Where == null)
      {
        this.m_root.Where = nodeSelect.Where;
      }
      else
      {
        if (nodeSelect.Where == null)
          return;
        NodeAndOperator nodeAndOperator = new NodeAndOperator();
        nodeAndOperator.Add(this.m_root.Where);
        nodeAndOperator.Add(nodeSelect.Where);
        this.m_root.Where = (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) nodeAndOperator;
      }
    }

    public string GenerateOrderClause()
    {
      if (this.m_root.OrderBy == null)
        return string.Empty;
      this.m_sql = new StringBuilder("ORDER BY ");
      try
      {
        this.EmitList((Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) this.m_root.OrderBy);
        return this.m_sql.ToString();
      }
      finally
      {
        this.m_sql = (StringBuilder) null;
      }
    }

    public object GetVarTag(NodeVariable var) => this.m_environment.FindVariable(var.Value, var.Parameters);

    internal List<KeyValuePair<int, string>> GenerateValueLists()
    {
      List<KeyValuePair<int, string>> ret = new List<KeyValuePair<int, string>>();
      if (this.m_groups != null)
      {
        foreach (KeyValuePair<string, int> group in this.m_groups)
        {
          KeyValuePair<string, int> pair = group;
          Array.ForEach<string>(this.GetGroupMembership(pair.Key), (Action<string>) (user => ret.Add(new KeyValuePair<int, string>(pair.Value, user))));
        }
      }
      return ret;
    }

    public NodeSelect Root => this.m_root;

    private int MarkObjectForRetrieval(string objectName)
    {
      if (this.m_groups == null)
        this.m_groups = new SortedList<string, int>((IComparer<string>) VssStringComparer.SID);
      int num;
      if (!this.m_groups.TryGetValue(objectName, out num))
      {
        num = this.m_nextListId++;
        this.m_groups.Add(objectName, num);
      }
      return num;
    }

    private void EmitNode(Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node)
    {
      switch (node.NodeType)
      {
        case NodeType.Number:
          this.EmitNumber(node.ConstStringValue);
          break;
        case NodeType.String:
          this.EmitString(node.ConstStringValue);
          break;
        case NodeType.FieldName:
          this.EmitFieldName((NodeFieldName) node);
          break;
        case NodeType.FieldCondition:
          this.EmitCondition((NodeCondition) node);
          break;
        case NodeType.ValueList:
          this.EmitValueList(node);
          break;
        case NodeType.BoolConst:
          this.EmitBoolConst((NodeBoolConst) node);
          break;
        case NodeType.BoolValue:
          this.EmitBoolValue((NodeBoolValue) node);
          break;
        case NodeType.Not:
          this.EmitNegation((NodeNotOperator) node);
          break;
        case NodeType.And:
          this.EmitConnector(" AND ", (NodeVariableList) node);
          break;
        case NodeType.Or:
          this.EmitConnector(" OR ", (NodeVariableList) node);
          break;
        case NodeType.Variable:
          this.EmitVariable((NodeVariable) node);
          break;
        case NodeType.Arithmetic:
          this.EmitArithmetic((NodeArithmetic) node);
          break;
        default:
          Tools.EnsureSyntax(false, SyntaxError.InvalidNodeType, node);
          break;
      }
    }

    private void EmitNegation(NodeNotOperator not)
    {
      this.m_sql.Append("NOT (");
      this.EmitNode(not.Value);
      this.m_sql.Append(")");
    }

    private void EmitArithmetic(NodeArithmetic node)
    {
      this.m_sql.Append('(');
      this.EmitNode(node.Left);
      switch (node.Arithmetic)
      {
        case Arithmetic.Add:
          this.m_sql.Append(" + ");
          break;
        case Arithmetic.Subtract:
          this.m_sql.Append(" - ");
          break;
      }
      this.EmitNode(node.Right);
      this.m_sql.Append(')');
    }

    private void EmitBoolConst(NodeBoolConst node)
    {
      if (node.Value)
        this.m_sql.Append("1 = 1");
      else
        this.m_sql.Append("1 = 0");
    }

    private void EmitBoolValue(NodeBoolValue node) => this.m_sql.Append(node.BoolValue ? "1" : "0");

    private void EmitConnector(string sqlConnector, NodeVariableList list)
    {
      this.m_sql.Append('(');
      bool flag = true;
      foreach (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node in (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) list)
      {
        if (flag)
          flag = false;
        else
          this.m_sql.Append(sqlConnector);
        this.EmitNode(node);
      }
      this.m_sql.Append(')');
    }

    private void EmitCondition(NodeCondition cond)
    {
      string str;
      switch (cond.Condition)
      {
        case Condition.Equals:
        case Condition.EqualsAlias:
          str = " = ";
          break;
        case Condition.NotEquals:
        case Condition.NotEqualsAlias:
          str = " <> ";
          break;
        case Condition.Less:
          str = " < ";
          break;
        case Condition.Greater:
          str = " > ";
          break;
        case Condition.LessOrEquals:
          str = " <= ";
          break;
        case Condition.GreaterOrEquals:
          str = " >= ";
          break;
        case Condition.Under:
          this.m_sql.Append('(');
          this.EmitFieldName(cond.Left);
          this.m_sql.Append(" LIKE N'");
          string constantNode1 = this.EvaluateConstantNode(cond.Right);
          this.m_sql.Append(QueryEngine.EscapeLikeWildcards(constantNode1) + "\\%");
          this.m_sql.Append("'");
          this.m_sql.Append(" OR ");
          this.EmitFieldName(cond.Left);
          this.m_sql.Append(" = ");
          this.EmitString(constantNode1);
          this.m_sql.Append(')');
          return;
        case Condition.In:
          str = " IN ";
          break;
        case Condition.Contains:
          this.EmitFieldName(cond.Left);
          string constantNode2 = this.EvaluateConstantNode(cond.Right);
          this.m_sql.Append(" LIKE N'%");
          this.m_sql.Append(QueryEngine.EscapeLikeWildcards(constantNode2));
          this.m_sql.Append("%'");
          return;
        case Condition.Group:
          this.EmitListQuery(cond);
          return;
        default:
          Tools.EnsureSyntax(true, SyntaxError.InvalidConditionalOperator, (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) cond);
          return;
      }
      this.EmitFieldName(cond.Left);
      this.m_sql.Append(str);
      if (this.m_conditionNodeValueTranslator != null && cond.Right.NodeType == NodeType.String)
        this.EmitString(this.m_conditionNodeValueTranslator(cond.Left.Value, cond.Right.ConstStringValue));
      else
        this.EmitNode(cond.Right);
    }

    private static string EscapeLikeWildcards(string value) => QueryEngine.EscapeQuotes(value).Replace("[", "[[]").Replace("%", "[%]").Replace("_", "[_]");

    private void EmitFieldName(NodeFieldName node)
    {
      WiqlFieldInfo tag = (WiqlFieldInfo) node.Tag;
      this.m_sql.Append('[');
      this.m_sql.Append(tag.SqlFieldName);
      this.m_sql.Append(']');
      if (node.Direction != Direction.Descending)
        return;
      this.m_sql.Append(" DESC");
    }

    private void EmitNumber(string num) => this.m_sql.Append(num);

    private void EmitString(string s)
    {
      string str = QueryEngine.EscapeQuotes(s);
      this.m_sql.Append("N'");
      this.m_sql.Append(str);
      this.m_sql.Append("'");
    }

    private static string EscapeQuotes(string s) => s.Replace("'", "''");

    private void EmitVariable(NodeVariable var)
    {
      object varTag = this.GetVarTag(var);
      switch (var.DataType)
      {
        case DataType.Date:
          this.EmitString(QueryEngine.ToSqlDateString((DateTime) varTag));
          break;
        case DataType.String:
          this.EmitString((string) varTag);
          break;
        default:
          Tools.EnsureSyntax(false, SyntaxError.UnknownVariableType, (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) var);
          break;
      }
    }

    private void EmitValueList(Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node list)
    {
      this.m_sql.Append("(");
      this.EmitList(list);
      this.m_sql.Append(")");
    }

    private void EmitList(Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node list)
    {
      bool flag = true;
      foreach (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node in list)
      {
        if (!flag)
          this.m_sql.Append(",");
        else
          flag = false;
        this.EmitNode(node);
      }
    }

    private void EmitListQuery(NodeCondition node)
    {
      this.m_sql.Append((object) node.Left);
      this.m_sql.Append(" IN (SELECT Data FROM @valueListTable WHERE Number = ");
      this.EmitNumber(node.Right.ConstStringValue);
      this.m_sql.Append(")");
    }

    private string EvaluateConstantNode(Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node)
    {
      if (node.NodeType == NodeType.Variable)
        return (string) this.GetVarTag((NodeVariable) node);
      if (node.NodeType == NodeType.String || node.NodeType == NodeType.Number || node.NodeType == NodeType.BoolConst)
        return node.ConstStringValue;
      Tools.EnsureSyntax(false, SyntaxError.ExpectingConst, node);
      return (string) null;
    }

    private static string ToSqlDateString(DateTime value) => value.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffK", (IFormatProvider) CultureInfo.InvariantCulture);

    protected abstract string[] GetGroupMembership(string groupId);

    private class QueryEnvironment : IExternal
    {
      private DateTime m_startTime;
      private QueryEngine m_engine;
      private static IDictionary<string, WiqlTableInfo> s_tables;
      private static object s_tablesLock = new object();

      internal QueryEnvironment(
        QueryEngine engine,
        List<Tuple<Type, string, string>> tablesToAdd,
        DateTime start)
      {
        if (QueryEngine.QueryEnvironment.s_tables == null || QueryEngine.QueryEnvironment.s_tables.Keys.Count == 5)
        {
          lock (QueryEngine.QueryEnvironment.s_tablesLock)
          {
            if (QueryEngine.QueryEnvironment.s_tables == null)
            {
              Dictionary<string, WiqlTableInfo> tables = new Dictionary<string, WiqlTableInfo>();
              tables.Add("Attachment", new WiqlTableInfo(typeof (TestResultAttachment), "vw_Attachment", "Attachment"));
              tables.Add("TestResult", new WiqlTableInfo(typeof (TestCaseResult), "vw_TestResult", "TestResult"));
              tables.Add("TestRun", new WiqlTableInfo(typeof (TestRun), "vw_TestRun", "TestRun"));
              tables.Add("TestConfiguration", new WiqlTableInfo(typeof (TestConfiguration), "vw_Configuration", "TestConfiguration"));
              tables.Add("TestSettings", new WiqlTableInfo(typeof (TestSettings), "vw_TestSettings", "TestSettings"));
              tablesToAdd?.ForEach((Action<Tuple<Type, string, string>>) (t => tables.Add(t.Item3, new WiqlTableInfo(t.Item1, t.Item2, t.Item3))));
              QueryEngine.QueryEnvironment.s_tables = (IDictionary<string, WiqlTableInfo>) tables;
            }
            else if (QueryEngine.QueryEnvironment.s_tables.Keys.Count == 5)
              tablesToAdd?.ForEach((Action<Tuple<Type, string, string>>) (t => QueryEngine.QueryEnvironment.s_tables.Add(t.Item3, new WiqlTableInfo(t.Item1, t.Item2, t.Item3))));
          }
        }
        this.m_engine = engine;
        this.m_startTime = start;
      }

      public CultureInfo CultureInfo => CultureInfo.CurrentCulture;

      public object FindField(string name, string prefix, object tableTag)
      {
        if (tableTag == null && this.m_engine.m_root.From != null)
          tableTag = this.m_engine.m_root.From.Tag;
        WiqlFieldInfo wiqlFieldInfo;
        if (!((WiqlTableInfo) tableTag).Fields.TryGetValue(name, out wiqlFieldInfo))
          return (object) null;
        return StringComparer.OrdinalIgnoreCase.Equals(wiqlFieldInfo.SqlFieldName, "RecursiveSuiteId") && !this.m_engine.AllowRecursion ? (object) null : (object) wiqlFieldInfo;
      }

      public object FindTable(string name)
      {
        WiqlTableInfo wiqlTableInfo;
        return QueryEngine.QueryEnvironment.s_tables.TryGetValue(name, out wiqlTableInfo) ? (object) wiqlTableInfo : (object) null;
      }

      public object FindVariable(string name, NodeParameters parameters)
      {
        if (StringComparer.InvariantCultureIgnoreCase.Equals(name, "me"))
          return (object) this.m_engine.m_userName;
        if (StringComparer.InvariantCultureIgnoreCase.Equals(name, "project"))
          return (object) this.m_engine.m_teamProjectName;
        return StringComparer.InvariantCultureIgnoreCase.Equals(name, "today") ? (object) this.m_startTime : (object) null;
      }

      public DataType GetFieldDataType(object fieldTag) => ((WiqlFieldInfo) fieldTag).DataType;

      public DataType GetVariableDataType(object varTag)
      {
        switch (varTag)
        {
          case string _:
            return DataType.String;
          case DateTime _:
            return DataType.Date;
          default:
            return DataType.Unknown;
        }
      }

      private object GetVarTag(NodeVariable var) => this.FindVariable(var.Value, var.Parameters);

      public Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node OptimizeNode(
        Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node,
        NodeTableName tableContext,
        NodeFieldName fieldContext)
      {
        if (node.NodeType == NodeType.FieldCondition)
        {
          NodeCondition cond = (NodeCondition) node;
          if ((cond.Left.Tag as WiqlFieldInfo).EnumerationValues != null)
            this.ConvertEnums(cond);
          else if (this.m_engine.m_dayPrecision && cond.Left.DataType == DataType.Date)
            node = this.HandleDates(node, tableContext, fieldContext);
          else if (cond.Condition == Condition.Group)
          {
            int num = this.m_engine.MarkObjectForRetrieval(this.m_engine.EvaluateConstantNode(cond.Right));
            cond.Right = (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) new NodeNumber(num.ToString((IFormatProvider) CultureInfo.InvariantCulture));
          }
        }
        else if (node.NodeType == NodeType.Arithmetic)
        {
          NodeArithmetic nodeArithmetic = (NodeArithmetic) node;
          if (nodeArithmetic.Left.CanCastTo(DataType.Numeric, this.CultureInfo))
          {
            double num1 = double.Parse(this.m_engine.EvaluateConstantNode(nodeArithmetic.Left), (IFormatProvider) this.CultureInfo);
            double num2 = double.Parse(this.m_engine.EvaluateConstantNode(nodeArithmetic.Right), (IFormatProvider) this.CultureInfo);
            node = (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) new NodeNumber((nodeArithmetic.Arithmetic != Arithmetic.Add ? num1 - num2 : num1 + num2).ToString((IFormatProvider) CultureInfo.InvariantCulture));
          }
          else if (nodeArithmetic.Left.CanCastTo(DataType.Date, this.CultureInfo))
          {
            TcmTrace.TraceAndDebugAssert(nameof (QueryEngine), nodeArithmetic.Left.NodeType == NodeType.Variable, "Expected variable for left value");
            DateTime varTag = (DateTime) this.GetVarTag((NodeVariable) nodeArithmetic.Left);
            double num = double.Parse(this.m_engine.EvaluateConstantNode(nodeArithmetic.Right), (IFormatProvider) this.CultureInfo);
            node = (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) new NodeString(XmlConvert.ToString(nodeArithmetic.Arithmetic != Arithmetic.Add ? varTag.AddDays(-num) : varTag.AddDays(num), XmlDateTimeSerializationMode.RoundtripKind));
          }
          else
            TcmTrace.TraceAndDebugFail(nameof (QueryEngine), "Unsupported arithmetic operation");
        }
        return node;
      }

      private Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node HandleDates(
        Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node,
        NodeTableName tableContext,
        NodeFieldName fieldContext)
      {
        NodeCondition nodeCondition = (NodeCondition) node;
        if (nodeCondition.Right.NodeType == NodeType.ValueList)
        {
          NodeValueList right1 = (NodeValueList) nodeCondition.Right;
          NodeOrOperator nodeOrOperator = new NodeOrOperator();
          foreach (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node right2 in (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) right1)
          {
            Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node1 = this.HandleDatePrecision((Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) new NodeCondition(Condition.Equals, nodeCondition.Left, right2), tableContext, fieldContext);
            nodeOrOperator.Add(node1);
          }
          node = (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) nodeOrOperator;
        }
        else
          node = this.HandleDatePrecision(node, tableContext, fieldContext);
        return node;
      }

      private void ConvertEnums(NodeCondition cond)
      {
        WiqlFieldInfo tag = (WiqlFieldInfo) cond.Left.Tag;
        if (cond.Right.NodeType == NodeType.ValueList)
        {
          NodeValueList right = (NodeValueList) cond.Right;
          for (int i = 0; i < right.Count; ++i)
            right[i] = (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) this.ConvertEnumNode(right[i], tag);
        }
        else
          cond.Right = (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) this.ConvertEnumNode(cond.Right, tag);
      }

      private NodeNumber ConvertEnumNode(Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node, WiqlFieldInfo fieldInfo)
      {
        string constantNode = this.m_engine.EvaluateConstantNode(node);
        int maxValue;
        if (!fieldInfo.EnumerationValues.TryGetValue(constantNode, out maxValue))
          maxValue = (int) byte.MaxValue;
        return new NodeNumber(maxValue.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      }

      private Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node HandleDatePrecision(
        Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node,
        NodeTableName tableContext,
        NodeFieldName fieldContext)
      {
        NodeCondition nodeCondition = (NodeCondition) node;
        DateTime dateTime;
        if (nodeCondition.Right.NodeType == NodeType.Variable)
        {
          dateTime = (DateTime) this.GetVarTag((NodeVariable) nodeCondition.Right);
          TcmTrace.TraceAndDebugAssert(nameof (QueryEngine), dateTime.Kind == DateTimeKind.Unspecified, "Got DateTimeKind.Unspecified");
        }
        else
          dateTime = DateTime.Parse(this.m_engine.EvaluateConstantNode(nodeCondition.Right), (IFormatProvider) this.CultureInfo);
        dateTime = dateTime.Date;
        if (dateTime.Kind == DateTimeKind.Unspecified)
          dateTime = TimeZoneInfo.ConvertTimeToUtc(dateTime, this.m_engine.m_timeZone);
        NodeString right1 = new NodeString(XmlConvert.ToString(dateTime, XmlDateTimeSerializationMode.Utc));
        NodeString right2 = new NodeString(XmlConvert.ToString(dateTime.AddDays(1.0), XmlDateTimeSerializationMode.Utc));
        switch (nodeCondition.Condition)
        {
          case Condition.Equals:
            NodeAndOperator nodeAndOperator = new NodeAndOperator();
            nodeAndOperator.Add((Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) new NodeCondition(Condition.GreaterOrEquals, nodeCondition.Left, (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) right1));
            nodeAndOperator.Add((Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) new NodeCondition(Condition.Less, nodeCondition.Left, (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) right2));
            node = (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) nodeAndOperator;
            node.Bind((IExternal) this, tableContext, fieldContext);
            break;
          case Condition.NotEquals:
            NodeOrOperator nodeOrOperator = new NodeOrOperator();
            nodeOrOperator.Add((Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) new NodeCondition(Condition.Less, nodeCondition.Left, (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) right1));
            nodeOrOperator.Add((Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) new NodeCondition(Condition.GreaterOrEquals, nodeCondition.Left, (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) right2));
            node = (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) nodeOrOperator;
            node.Bind((IExternal) this, tableContext, fieldContext);
            break;
          case Condition.Less:
          case Condition.GreaterOrEquals:
            nodeCondition.Right = (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) right1;
            break;
          case Condition.Greater:
            nodeCondition.Condition = Condition.GreaterOrEquals;
            nodeCondition.Right = (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) right2;
            break;
          case Condition.LessOrEquals:
            nodeCondition.Condition = Condition.Less;
            nodeCondition.Right = (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) right2;
            break;
          default:
            Tools.EnsureSyntax(false, SyntaxError.InvalidConditionalOperator, node);
            break;
        }
        return node;
      }

      public TimeZone TimeZone => TimeZone.CurrentTimeZone;

      public void VerifyNode(Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node, NodeTableName tableContext, NodeFieldName fieldContext)
      {
        if (node.NodeType == NodeType.Arithmetic)
        {
          NodeArithmetic nodeArithmetic = (NodeArithmetic) node;
          Tools.EnsureSyntax(nodeArithmetic.Left.CanCastTo(DataType.Date, this.CultureInfo) || nodeArithmetic.Left.CanCastTo(DataType.Numeric, this.CultureInfo), SyntaxError.WrongTypeForArithmetic, node);
          Tools.EnsureSyntax(nodeArithmetic.Right.DataType == DataType.Numeric, SyntaxError.WrongTypeForArithmeticRightOperand, node);
        }
        else if (node.NodeType == NodeType.Ever || node.NodeType == NodeType.Mode)
          Tools.EnsureSyntax(false, SyntaxError.InvalidNodeType, node);
        else if (node.NodeType == NodeType.FieldCondition)
        {
          NodeCondition nodeCondition = (NodeCondition) node;
          WiqlFieldInfo tag = (WiqlFieldInfo) nodeCondition.Left.Tag;
          if (nodeCondition.Condition != Condition.Under)
            return;
          Tools.EnsureSyntax(StringComparer.OrdinalIgnoreCase.Equals(tag.WiqlFieldName, "TestCaseArea") || StringComparer.OrdinalIgnoreCase.Equals(tag.WiqlFieldName, "Iteration") || StringComparer.OrdinalIgnoreCase.Equals(tag.WiqlFieldName, "AreaPath"), SyntaxError.UnderCanBeUsedForTreePathFieldOnly, (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) nodeCondition.Left);
        }
        else
        {
          if (node.NodeType != NodeType.Select)
            return;
          NodeSelect nodeSelect = (NodeSelect) node;
          Tools.EnsureSyntax(nodeSelect.AsOf == null, SyntaxError.InvalidNodeType, nodeSelect.AsOf);
        }
      }

      public DataType GetVariableDataType(string name) => this.GetVariableDataType(this.FindVariable(name, (NodeParameters) null));

      public bool DoesMacroExtensionHandleOffset(string name) => false;

      public void ValidateParameters(
        string macroName,
        NodeTableName tableContext,
        NodeFieldName fieldContext,
        NodeParameters parameters)
      {
      }
    }
  }
}
