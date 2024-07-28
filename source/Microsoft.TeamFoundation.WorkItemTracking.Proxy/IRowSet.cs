// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Proxy.IRowSet
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Proxy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF15D8B4-8AC0-4915-8153-9054E8546EA2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Proxy.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Proxy
{
  public interface IRowSet
  {
    bool ContainsColumn(string name);

    int RowCount { get; }

    int ColumnCount { get; }

    string this[int column] { get; }

    object this[int column, int row] { get; }

    object this[string column, int row] { get; }

    void SwapRows(int row1, int row2);
  }
}
