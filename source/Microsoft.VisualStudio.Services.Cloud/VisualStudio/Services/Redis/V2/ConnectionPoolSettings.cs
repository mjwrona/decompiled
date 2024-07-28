// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Redis.V2.ConnectionPoolSettings
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

namespace Microsoft.VisualStudio.Services.Redis.V2
{
  internal class ConnectionPoolSettings
  {
    public int DatabaseId { get; set; }

    public int PoolSize { get; set; } = 3;

    public int MaxMessageSize { get; set; } = -262144;

    public int RetryCount { get; set; }

    public bool NeedsPubSub { get; set; } = true;

    public int MaxFailuresPerRequest { get; set; } = int.MaxValue;

    internal IPartitionPolicy PartitionPolicy { get; set; } = (IPartitionPolicy) new RoundRobinPartitionPolicy();
  }
}
