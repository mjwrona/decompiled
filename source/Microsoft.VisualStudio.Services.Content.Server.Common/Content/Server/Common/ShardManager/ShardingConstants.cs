// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.ShardManager.ShardingConstants
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Content.Server.Common.ShardManager
{
  public static class ShardingConstants
  {
    public const string LinearShardingStrategy = "LinearSharding";
    public const string ConsistentHashingShardingStrategy = "ConsistentHashing";
    public static readonly string[] ShardingStrategies = new string[2]
    {
      "LinearSharding",
      "ConsistentHashing"
    };
    public const string ShardingStrategyKey = "ShardingStrategy";

    public static bool UseLinearSharding(string shardingStrategy) => string.Equals(shardingStrategy, "LinearSharding", StringComparison.OrdinalIgnoreCase);

    public static bool UseConsistentHashingSharding(string shardingStrategy) => string.Equals(shardingStrategy, "ConsistentHashing", StringComparison.OrdinalIgnoreCase);
  }
}
