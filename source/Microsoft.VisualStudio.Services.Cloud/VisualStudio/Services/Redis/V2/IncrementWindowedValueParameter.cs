// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Redis.V2.IncrementWindowedValueParameter
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using StackExchange.Redis;

namespace Microsoft.VisualStudio.Services.Redis.V2
{
  public struct IncrementWindowedValueParameter
  {
    public IncrementWindowedValueParameter(RedisKey key, long value, long maximum)
      : this()
    {
      this.Key = key;
      this.Value = value;
      this.Maximum = maximum;
    }

    public RedisKey Key { get; set; }

    public long Value { get; set; }

    public long Maximum { get; set; }
  }
}
