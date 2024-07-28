// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.SingleConcurrencyLock
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

namespace Microsoft.VisualStudio.Services.Content.Server.Common
{
  public class SingleConcurrencyLock : IConcurrencyLock
  {
    private int m_max;
    private int m_count;
    private object syncLock;

    public SingleConcurrencyLock(int max)
    {
      this.m_count = 0;
      this.m_max = max;
      this.syncLock = new object();
    }

    public virtual bool Require()
    {
      if (this.m_count < this.m_max)
      {
        lock (this.syncLock)
        {
          if (this.m_count < this.m_max)
          {
            ++this.m_count;
            return true;
          }
        }
      }
      return false;
    }

    public virtual void Release()
    {
      lock (this.syncLock)
      {
        if (this.m_count <= 0)
          return;
        --this.m_count;
      }
    }
  }
}
