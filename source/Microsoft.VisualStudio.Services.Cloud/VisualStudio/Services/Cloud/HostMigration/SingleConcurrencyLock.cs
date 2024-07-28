// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostMigration.SingleConcurrencyLock
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

namespace Microsoft.VisualStudio.Services.Cloud.HostMigration
{
  public class SingleConcurrencyLock : IMigrationConcurrencyLock
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

    public virtual bool Acquire()
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
