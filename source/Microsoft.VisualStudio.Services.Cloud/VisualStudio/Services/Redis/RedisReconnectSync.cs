// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Redis.RedisReconnectSync
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Redis
{
  internal class RedisReconnectSync
  {
    private readonly TimeSpan m_reconnectionPeriod;
    private int m_shouldReconnectFlag = 1;
    private DateTimeOffset m_lastReconnect = DateTimeOffset.MinValue;

    internal RedisReconnectSync(TimeSpan reconnectionPeriod) => this.m_reconnectionPeriod = reconnectionPeriod;

    public bool ShouldReconnect()
    {
      if (Interlocked.CompareExchange(ref this.m_shouldReconnectFlag, 0, 1) != 1)
        return false;
      this.m_lastReconnect = DateTimeOffset.UtcNow;
      Task.Delay(this.m_reconnectionPeriod).ContinueWith<int>((Func<Task, int>) (t => this.m_shouldReconnectFlag = 1));
      return true;
    }

    public string ReconnectionPeriod => this.m_reconnectionPeriod.ToString("c");

    public string LastReconnectionAttempt => !(this.m_lastReconnect == DateTimeOffset.MinValue) ? this.m_lastReconnect.ToString("u") : "never";
  }
}
