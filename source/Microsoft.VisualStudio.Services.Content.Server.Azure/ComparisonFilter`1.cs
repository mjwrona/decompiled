// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.ComparisonFilter`1
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using System;
using System.Text;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public class ComparisonFilter<T> : IFilter<T> where T : IColumn
  {
    public readonly IColumnValue<T> ColumnValue;
    public readonly T Column;
    public readonly ComparisonOperator Operator;

    public ComparisonFilter(IColumnValue<T> columnValue, ComparisonOperator comparisonOperator)
    {
      this.Column = columnValue.Column;
      this.Operator = comparisonOperator;
      this.ColumnValue = columnValue;
    }

    public bool IsNull => false;

    public StringBuilder CreateFilter(StringBuilder builder) => this.ColumnValue.CreateFilterCondition(builder, this.Operator);

    public bool IsMatch(ITableEntityWithColumns ste)
    {
      switch (this.Operator)
      {
        case ComparisonOperator.Equal:
          IValue other1;
          return ste.TryGetValue<T>(this.ColumnValue, out other1) && this.ColumnValue.Value.Equals(other1);
        case ComparisonOperator.NotEqual:
          IValue other2;
          return ste.TryGetValue<T>(this.ColumnValue, out other2) && !this.ColumnValue.Value.Equals(other2);
        case ComparisonOperator.GreaterThan:
          IValue other3;
          return ste.TryGetValue<T>(this.ColumnValue, out other3) && this.ColumnValue.Value.CompareTo(other3) < 0;
        case ComparisonOperator.GreaterThanOrEqual:
          IValue other4;
          return ste.TryGetValue<T>(this.ColumnValue, out other4) && this.ColumnValue.Value.CompareTo(other4) <= 0;
        case ComparisonOperator.LessThan:
          IValue other5;
          return ste.TryGetValue<T>(this.ColumnValue, out other5) && this.ColumnValue.Value.CompareTo(other5) > 0;
        case ComparisonOperator.LessThanOrEqual:
          IValue other6;
          return ste.TryGetValue<T>(this.ColumnValue, out other6) && this.ColumnValue.Value.CompareTo(other6) >= 0;
        default:
          throw new InvalidOperationException(string.Format("unable to intepret operator {0}", (object) this.Operator));
      }
    }
  }
}
