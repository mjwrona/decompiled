// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.LocalAndRedisCacheConfiguration
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class LocalAndRedisCacheConfiguration
  {
    private static readonly LocalAndRedisCacheConfiguration s_defaultConfiguration = new LocalAndRedisCacheConfiguration().WithExpiryInterval(TimeSpan.FromHours(1.0)).WithMemoryCacheMaxLength(int.MaxValue);

    public TimeSpan? ExpiryInterval { get; private set; }

    public int? MemoryCacheMaxLength { get; private set; }

    public Guid? RedisNamespace { get; private set; }

    public static LocalAndRedisCacheConfiguration Default => LocalAndRedisCacheConfiguration.s_defaultConfiguration;

    public LocalAndRedisCacheConfiguration WithMemoryCacheMaxLength(int maxLength)
    {
      this.MemoryCacheMaxLength = new int?(maxLength);
      return this;
    }

    public LocalAndRedisCacheConfiguration WithExpiryInterval(TimeSpan interval)
    {
      this.ExpiryInterval = new TimeSpan?(interval);
      return this;
    }

    public LocalAndRedisCacheConfiguration WithRedisNamespace(Guid redisNamespace)
    {
      this.RedisNamespace = new Guid?(redisNamespace);
      return this;
    }
  }
}
