// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpressionValue
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class QueryExpressionValue
  {
    private QueryExpressionValue[] m_arrayValue;

    public QueryExpressionValue() => this.IsNull = true;

    public DateTime DateValue { get; set; }

    public bool BoolValue { get; set; }

    public string StringValue { get; set; }

    public int NumberValue { get; set; }

    public double DoubleValue { get; set; }

    public Guid GuidValue { get; set; }

    public IEnumerable<Guid> IdentityGuidValues { get; set; }

    public FieldEntry ColumnValue { get; set; }

    public QueryExpressionValueType ValueType { get; set; }

    public bool IsNull { get; set; }

    public bool IsVariable { get; set; }

    public bool IsArithmetic { get; set; }

    public string ArgumentsString { get; internal set; }

    public override string ToString()
    {
      if (this.IsNull || this.ValueType == QueryExpressionValueType.Array)
        return string.Empty;
      switch (this.ValueType)
      {
        case QueryExpressionValueType.IdentityGuid:
          return string.Join<Guid>(",", this.IdentityGuidValues);
        case QueryExpressionValueType.Column:
          return this.ColumnValue.ReferenceName;
        case QueryExpressionValueType.Number:
          return this.NumberValue.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo);
        case QueryExpressionValueType.DateTime:
          return this.DateValue.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo);
        case QueryExpressionValueType.UniqueIdentifier:
          return this.GuidValue.ToString();
        case QueryExpressionValueType.Boolean:
          return this.BoolValue.ToString();
        case QueryExpressionValueType.Double:
          return this.DoubleValue.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo);
        default:
          return this.StringValue;
      }
    }

    internal static QueryExpressionValue CreateArrayValue(QueryExpressionValue[] value) => new QueryExpressionValue()
    {
      IsNull = false,
      ValueType = QueryExpressionValueType.Array,
      m_arrayValue = value
    };

    public QueryExpressionValue[] GetArrayValue() => this.m_arrayValue;
  }
}
