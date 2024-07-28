// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Redis.V1.SetVolatileValueParameter
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using StackExchange.Redis;
using System;

namespace Microsoft.VisualStudio.Services.Redis.V1
{
  public struct SetVolatileValueParameter
  {
    public SetVolatileValueParameter(RedisKey key, RedisValue value, TimeSpan? expiry = null)
      : this()
    {
      this.Key = key;
      this.Value = value;
      this.Expiry = expiry;
    }

    public RedisKey Key { get; set; }

    public RedisValue Value { get; set; }

    public TimeSpan? Expiry { get; set; }
  }
}
