// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.TaskActivityDispatcherSharding.ActivityShardLocatorRandom
// Assembly: Microsoft.VisualStudio.Services.Orchestration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C0C603F4-BE31-455B-860A-9FD3B046611C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.dll

using System;

namespace Microsoft.VisualStudio.Services.Orchestration.TaskActivityDispatcherSharding
{
  public class ActivityShardLocatorRandom : IActivityShardLocator
  {
    private readonly Random m_random = new Random();
    private int m_activityDispatcherShardsCount;

    public ActivityShardLocatorRandom(int activityDispatcherShardsCount) => this.m_activityDispatcherShardsCount = activityDispatcherShardsCount;

    public virtual bool TryGetShardValue(
      IActivityShardKey key,
      out string shardValue,
      string dispatcherType = null)
    {
      if (this.m_activityDispatcherShardsCount <= 1)
      {
        shardValue = (string) null;
        return false;
      }
      int num = Math.Abs(this.m_random.Next()) % this.m_activityDispatcherShardsCount;
      shardValue = (string.IsNullOrEmpty(dispatcherType) ? (object) "" : (object) (dispatcherType + " ")).ToString() + (object) num;
      return true;
    }
  }
}
