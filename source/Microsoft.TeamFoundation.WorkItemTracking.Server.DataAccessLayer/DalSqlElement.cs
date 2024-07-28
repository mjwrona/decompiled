// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalSqlElement
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Globalization;
using System.Text;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalSqlElement
  {
    protected SqlBatchBuilder m_sqlBatch;
    protected ElementGroup m_elementGroup;
    protected int m_outputs;
    protected int m_index = -1;
    protected Update m_update;
    public static readonly object SQL_NULL = new object();
    public static readonly string DEFAULT_VALUE = "default";
    protected static readonly string NULL_VALUE = "null";
    private static readonly string PartitionIdPlusComma = "@partitionId, ";

    public int Version => this.m_sqlBatch.Version;

    public int GetDataspaceId(IVssRequestContext requestContext, Guid projectId) => requestContext.GetService<IDataspaceService>().QueryDataspace(requestContext, "WorkItem", projectId, false).DataspaceId;

    public Guid GetDataspaceIdentifier(IVssRequestContext requestContext, int dataspaceId) => requestContext.GetService<IDataspaceService>().QueryDataspace(requestContext, dataspaceId).DataspaceIdentifier;

    public virtual void Initialize(SqlBatchBuilder sqlBatch, Update update)
    {
      this.m_sqlBatch = sqlBatch;
      this.m_update = update;
    }

    public static T GetElement<T>(SqlBatchBuilder sqlBatch, Update update = null) where T : DalSqlElement, new()
    {
      T element = new T();
      element.Initialize(sqlBatch, update);
      return element;
    }

    public static DalSqlElement GetElement(
      Type elementType,
      SqlBatchBuilder sqlBatch,
      Update update = null)
    {
      DalSqlElement instance = (DalSqlElement) Activator.CreateInstance(elementType);
      instance.Initialize(sqlBatch, update);
      return instance;
    }

    public bool IsNeeded { set; get; }

    protected void SetOutputs(int expectedOutputs) => this.m_outputs = expectedOutputs;

    public int GetOutputs() => this.m_outputs;

    protected void SetGroup(ElementGroup group)
    {
      this.m_elementGroup = group;
      if (this.m_elementGroup != null)
        this.m_index = this.m_elementGroup.AddElementToGroup(this.m_outputs);
      else
        this.m_index = this.m_sqlBatch.AddExpectedReturnedDataTables(this.m_outputs);
    }

    protected virtual void AppendSql(string sql)
    {
      if (this.m_elementGroup != null)
        this.m_elementGroup.AppendSql(this.m_index, sql);
      else
        this.m_sqlBatch.AppendSql(sql);
    }

    protected void AppendSql(params string[] sqlSegments)
    {
      foreach (string sqlSegment in sqlSegments)
        this.AppendSql(sqlSegment);
    }

    internal bool IsSchemaPartitioned => this.Version >= 5;

    protected void AppendPartitionIdVariable(StringBuilder builder, bool appendComma = true)
    {
      if (!this.IsSchemaPartitioned)
        return;
      builder.Append("@partitionId");
      if (!appendComma)
        return;
      builder.Append(",");
    }

    protected void AppendPartitionIdVariable(bool appendComma = true)
    {
      if (!this.IsSchemaPartitioned)
        return;
      this.AppendSql("@partitionId");
      if (!appendComma)
        return;
      this.AppendSql(",");
    }

    internal string FormatPartitionClause(string clause) => this.IsSchemaPartitioned ? clause : string.Empty;

    protected string FormatPartitionIdVariable(bool appendComma = true)
    {
      if (!this.IsSchemaPartitioned)
        return string.Empty;
      return appendComma ? DalSqlElement.PartitionIdPlusComma : "@partitionId";
    }

    public int GetResultId()
    {
      if (this.m_outputs == 0)
        return 0;
      int outputRowSetIndex = this.GetOutputRowSetIndex();
      return this.m_sqlBatch.ResultPayload.Tables.Count <= outputRowSetIndex || this.m_sqlBatch.ResultPayload.Tables[outputRowSetIndex].Rows.Count < 1 || this.m_sqlBatch.ResultPayload.Tables[outputRowSetIndex].Columns.Count < 1 ? 0 : (int) this.m_sqlBatch.ResultPayload.Tables[outputRowSetIndex].Rows[0][0];
    }

    public virtual PayloadTable GetResultTable() => this.GetResultTable(0);

    public object GetSingleResult()
    {
      if (this.m_outputs == 0)
        return (object) 0;
      int outputRowSetIndex = this.GetOutputRowSetIndex();
      return this.m_sqlBatch.ResultPayload.Tables.Count <= outputRowSetIndex || this.m_sqlBatch.ResultPayload.Tables[outputRowSetIndex].Rows.Count < 1 || this.m_sqlBatch.ResultPayload.Tables[outputRowSetIndex].Columns.Count < 1 ? (object) null : this.m_sqlBatch.ResultPayload.Tables[outputRowSetIndex].Rows[0][0];
    }

    public PayloadTable GetResultTable(int elementOutputNumber)
    {
      if (this.m_outputs == 0 || elementOutputNumber > this.m_outputs - 1)
        return (PayloadTable) null;
      int index = this.GetOutputRowSetIndex() + elementOutputNumber;
      return this.m_sqlBatch.ResultPayload.Tables.Count <= index ? (PayloadTable) null : this.m_sqlBatch.ResultPayload.Tables[index];
    }

    public int GetOutputRowSetIndex() => this.m_elementGroup == null ? this.m_index : this.m_elementGroup.GetOutputIndex(this.m_index);

    protected SqlBatchBuilder SqlBatch => this.m_sqlBatch;

    public int ClientVersion => this.m_sqlBatch.ClientVersion;

    protected void AppendExecProc(string procName, params string[] formattedParams)
    {
      this.AppendSql("exec dbo.[");
      this.AppendSql(procName);
      this.AppendSql("] ");
      for (int index = 0; index < formattedParams.Length; ++index)
      {
        this.AppendSql(formattedParams[index]);
        if (index < formattedParams.Length - 1)
          this.AppendSql(",");
      }
      this.AppendSql(" if @@trancount = 0 return");
      this.AppendSql(Environment.NewLine);
    }

    protected string ParamOrDefault(object value) => DalSqlElement.IsNullOrEmpty(value) ? DalSqlElement.DEFAULT_VALUE : this.Param(value);

    protected string Param(object value)
    {
      if (value == DalSqlElement.SQL_NULL)
        return DalSqlElement.NULL_VALUE;
      switch (value)
      {
        case string _:
          return (value as string).Equals(DalSqlElement.DEFAULT_VALUE) ? DalSqlElement.DEFAULT_VALUE : this.SqlBatch.AddParameterNVarChar(value as string);
        case int parameterValue1:
          return this.SqlBatch.AddParameterInt(parameterValue1);
        case bool parameterValue2:
          return this.SqlBatch.AddParameterBit(parameterValue2);
        case double parameterValue3:
          return this.SqlBatch.AddParameterDouble(parameterValue3);
        case byte[] _:
          return this.SqlBatch.AddParameterBinary((byte[]) value);
        case Guid guid:
          return this.SqlBatch.AddParameterNVarChar(guid.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture));
        default:
          throw new NotImplementedException("Currently only certain param types are currently supported; add more as needed.");
      }
    }

    protected static string InlineOrDefault(object value) => DalSqlElement.IsNullOrEmpty(value) ? DalSqlElement.DEFAULT_VALUE : DalSqlElement.Inline(value);

    protected static bool IsNullOrEmpty(object value) => value is string && string.IsNullOrEmpty(value as string) || value is Guid guid && guid == Guid.Empty || value == null;

    protected static string Inline(object value)
    {
      if (value == DalSqlElement.SQL_NULL)
        return DalSqlElement.NULL_VALUE;
      switch (value)
      {
        case char ch:
          return DalSqlElement.InlineChar(ch);
        case int num1:
          return DalSqlElement.InlineInt(num1);
        case bool flag:
          return DalSqlElement.InlineBit(flag);
        case double num2:
          return DalSqlElement.InlineDouble(num2);
        case Guid guid:
          return DalSqlElement.InlineGuid(guid);
        case DateTime dateTime:
          return DalSqlElement.InlineDateTime(dateTime);
        default:
          throw new NotImplementedException("Currently only certain param types are currently supported; add more as needed.");
      }
    }

    protected static string InlineBit(bool value) => !value ? "0" : "1";

    protected static string InlineInt(int value) => value.ToString((IFormatProvider) CultureInfo.InvariantCulture);

    protected static string InlineChar(char value) => "'" + value.ToString() + "'";

    protected static string InlineDouble(double value) => value.ToString((IFormatProvider) CultureInfo.InvariantCulture);

    protected static string InlineGuid(Guid value) => "'" + value.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture) + "'";

    protected static string InlineDateTime(DateTime value)
    {
      SqlBatchBuilder.CheckIsValidSqlDateTime(value);
      return "'" + value.ToString("yyyy-MM-ddTHH:mm:ss.fff", (IFormatProvider) DateTimeFormatInfo.InvariantInfo) + "'";
    }
  }
}
