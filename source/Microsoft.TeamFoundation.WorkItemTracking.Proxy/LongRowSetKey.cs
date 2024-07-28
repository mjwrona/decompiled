// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Proxy.LongRowSetKey
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Proxy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF15D8B4-8AC0-4915-8153-9054E8546EA2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Proxy.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Proxy
{
  public sealed class LongRowSetKey
  {
    private int m_fldID;
    private int m_itemID;

    public LongRowSetKey(int fldID, int itemID)
    {
      this.m_fldID = fldID;
      this.m_itemID = itemID;
    }

    public int FldID => this.m_fldID;

    public int ItemID => this.m_itemID;
  }
}
