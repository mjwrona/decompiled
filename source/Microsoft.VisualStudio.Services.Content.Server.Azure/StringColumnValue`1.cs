// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.StringColumnValue`1
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Cosmos.Table;
using System;
using System.Text;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public abstract class StringColumnValue<T> : IColumnValue<T> where T : IColumn
  {
    public readonly StringValue ColumnValue;

    public abstract T Column { get; }

    protected StringColumnValue(string columnValue) => this.ColumnValue = !columnValue.Contains("'") ? new StringValue(columnValue) : throw new NotImplementedException("Escaping single apostrophe is not yet supported.");

    public IValue Value => (IValue) this.ColumnValue;

    public StringBuilder CreateFilterCondition(StringBuilder builder, ComparisonOperator op) => builder.Append(TableQuery.GenerateFilterCondition(this.Column.Name, EnumExtension.ToString(op), this.ColumnValue.Value));
  }
}
