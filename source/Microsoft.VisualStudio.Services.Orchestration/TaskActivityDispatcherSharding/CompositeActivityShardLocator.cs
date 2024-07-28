// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.TaskActivityDispatcherSharding.CompositeActivityShardLocator
// Assembly: Microsoft.VisualStudio.Services.Orchestration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C0C603F4-BE31-455B-860A-9FD3B046611C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Orchestration.TaskActivityDispatcherSharding
{
  public class CompositeActivityShardLocator : IActivityShardLocator
  {
    private readonly IList<IActivityShardLocator> m_activityShardLocators;

    public CompositeActivityShardLocator(IList<IActivityShardLocator> activityShardLocators) => this.m_activityShardLocators = activityShardLocators;

    public bool TryGetShardValue(
      IActivityShardKey key,
      out string shardValue,
      string dispatcherType = null)
    {
      foreach (IActivityShardLocator activityShardLocator in (IEnumerable<IActivityShardLocator>) this.m_activityShardLocators)
      {
        string shardValue1;
        if (activityShardLocator.TryGetShardValue(key, out shardValue1, dispatcherType))
        {
          shardValue = shardValue1;
          return true;
        }
      }
      shardValue = (string) null;
      return false;
    }
  }
}
