// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataTableWrapper
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DataTableWrapper : IPayloadTableSchema
  {
    private DataTable m_dataTable;

    public DataTableWrapper(DataTable dataTable)
    {
      this.m_dataTable = dataTable;
      this.ColumnCount = dataTable == null ? 0 : dataTable.Rows.Count;
    }

    public int ColumnCount { get; private set; }

    public string GetColumnName(int index)
    {
      if (index >= this.ColumnCount)
        throw new IndexOutOfRangeException();
      return (string) this.m_dataTable.Rows[index]["ColumnName"];
    }

    public Type GetColumnType(int index)
    {
      if (index >= this.ColumnCount)
        throw new IndexOutOfRangeException();
      return (Type) this.m_dataTable.Rows[index]["DataType"];
    }
  }
}
