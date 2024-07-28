// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Sharding.ServerTaskShardLocatorRandom
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.VisualStudio.Services.Orchestration.TaskActivityDispatcherSharding;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Sharding
{
  internal class ServerTaskShardLocatorRandom : ActivityShardLocatorRandom
  {
    public ServerTaskShardLocatorRandom(int activityDispatcherShardsCount)
      : base(activityDispatcherShardsCount)
    {
    }

    public override bool TryGetShardValue(
      IActivityShardKey key,
      out string shardValue,
      string dispatcherType = null)
    {
      if (dispatcherType == "Server")
        return base.TryGetShardValue(key, out shardValue, dispatcherType);
      shardValue = (string) null;
      return false;
    }
  }
}
