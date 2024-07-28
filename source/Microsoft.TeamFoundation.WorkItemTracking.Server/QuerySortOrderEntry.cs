// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.QuerySortOrderEntry
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public struct QuerySortOrderEntry
  {
    private string m_columnName;
    private bool m_ascending;
    private bool? m_nullsFirst;

    public string ColumnName
    {
      get => this.m_columnName;
      set => this.m_columnName = value;
    }

    public bool Ascending
    {
      get => this.m_ascending;
      set => this.m_ascending = value;
    }

    public bool? NullsFirst
    {
      get => this.m_nullsFirst;
      set => this.m_nullsFirst = value;
    }
  }
}
