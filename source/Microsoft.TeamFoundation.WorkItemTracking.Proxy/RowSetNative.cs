// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Proxy.RowSetNative
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Proxy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF15D8B4-8AC0-4915-8153-9054E8546EA2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Proxy.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Proxy
{
  internal class RowSetNative : IRowSetNative
  {
    private IRowSet m_rs;
    private ISerializeRow m_sr;

    public RowSetNative(IRowSet rs)
    {
      this.m_rs = rs;
      this.m_sr = (ISerializeRow) rs;
    }

    int IRowSetNative.GetRowCount() => this.m_rs.RowCount;

    string[] IRowSetNative.GetColumns()
    {
      int columnCount = this.m_rs.ColumnCount;
      string[] columns = new string[columnCount];
      for (int column = 0; column < columnCount; ++column)
        columns[column] = this.m_rs[column];
      return columns;
    }

    void IRowSetNative.GetRow(int row, IntPtr p, int esz) => this.m_sr.CopyRow(row, p, esz);
  }
}
