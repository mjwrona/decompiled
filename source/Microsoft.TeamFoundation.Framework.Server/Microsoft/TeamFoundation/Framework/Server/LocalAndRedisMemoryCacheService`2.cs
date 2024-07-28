// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.LocalAndRedisMemoryCacheService`2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class LocalAndRedisMemoryCacheService<K, V> : VssMemoryCacheService<K, V> where K : IEquatable<K>
  {
    public LocalAndRedisMemoryCacheService()
      : base(LocalAndRedisCacheConstants.MemoryCacheCleanupInterval)
    {
    }

    public override string Name => this.GetType().Name + "-" + typeof (K).Name + "-" + typeof (V).Name;

    public void SetExpiryInterval(TimeSpan value) => this.ExpiryInterval.Value = value;

    public void SetMaxCacheLength(int value) => this.MaxCacheLength.Value = value;

    public string RegistryPath() => "/Configuration/Caching/MemoryCache/" + this.Name;
  }
}
