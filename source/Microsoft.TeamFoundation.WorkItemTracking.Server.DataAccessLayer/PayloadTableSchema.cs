// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.PayloadTableSchema
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class PayloadTableSchema : IPayloadTableSchema
  {
    private PayloadTableSchema.Column[] m_col;

    public PayloadTableSchema(PayloadTableSchema.Column[] cols)
    {
      if (cols == null)
        return;
      this.m_col = (PayloadTableSchema.Column[]) cols.Clone();
    }

    public IEnumerator<PayloadTableSchema.Column> GetEnumerator() => ((IEnumerable<PayloadTableSchema.Column>) this.m_col).GetEnumerator();

    public int ColumnCount => this.m_col != null ? this.m_col.Length : 0;

    public string GetColumnName(int index)
    {
      if (index >= this.ColumnCount)
        throw new IndexOutOfRangeException();
      return this.m_col[index].Name;
    }

    public Type GetColumnType(int index)
    {
      if (index >= this.ColumnCount)
        throw new IndexOutOfRangeException();
      return this.m_col[index].Type;
    }

    public struct Column
    {
      public Column(string name, Type type)
        : this()
      {
        this.Name = name;
        this.Type = type;
      }

      public string Name { get; private set; }

      public Type Type { get; private set; }
    }
  }
}
