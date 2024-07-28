// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.DateTimeColumnValue`1
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Cosmos.Table;
using System;
using System.Text;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public abstract class DateTimeColumnValue<T> : IColumnValue<T> where T : IColumn
  {
    private readonly DateTimeValue columnValue;

    public abstract T Column { get; }

    public DateTimeColumnValue(DateTime date) => this.columnValue = date.Kind == DateTimeKind.Utc ? new DateTimeValue(date) : throw new ArgumentException("Doesn't have kind UTC", nameof (date));

    public IValue Value => (IValue) this.columnValue;

    public StringBuilder CreateFilterCondition(StringBuilder builder, ComparisonOperator op) => builder.Append(TableQuery.GenerateFilterConditionForDate(this.Column.Name, EnumExtension.ToString(op), (DateTimeOffset) this.columnValue.Value));
  }
}
