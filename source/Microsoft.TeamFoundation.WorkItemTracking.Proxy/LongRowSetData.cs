// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Proxy.LongRowSetData
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Proxy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF15D8B4-8AC0-4915-8153-9054E8546EA2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Proxy.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Proxy
{
  public sealed class LongRowSetData
  {
    private DateTime m_dt;
    private string m_words;

    public LongRowSetData(DateTime dt, string words)
    {
      this.m_dt = dt;
      this.m_words = words;
    }

    public DateTime AddedDate => this.m_dt;

    public string Words => this.m_words;
  }
}
