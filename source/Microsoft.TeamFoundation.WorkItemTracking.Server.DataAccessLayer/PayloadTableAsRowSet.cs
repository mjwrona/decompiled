// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.PayloadTableAsRowSet
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.WorkItemTracking.Internals;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class PayloadTableAsRowSet : IRowSetHelper
  {
    private PayloadTable m_pt;

    public PayloadTableAsRowSet(PayloadTable pt) => this.m_pt = pt;

    public int RowCount => this.m_pt.RowCount;

    public int ColumnCount => this.m_pt.Columns.Count;

    public string this[int column] => this.m_pt.Columns[column].Name;

    public object this[int row, int column] => this.m_pt.Rows[row][column];

    public object this[int row, string column] => this.m_pt.Rows[row][column];
  }
}
