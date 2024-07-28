// Decompiled with JetBrains decompiler
// Type: Pegasus.Common.CacheKey
// Assembly: Pegasus.Common, Version=4.0.14.0, Culture=neutral, PublicKeyToken=28c69b6c6d100f4a
// MVID: 081C50C0-D236-41F7-86F9-E3F2168B7118
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Pegasus.Common.dll

using System.Diagnostics;

namespace Pegasus.Common
{
  [DebuggerDisplay("{ruleName}:{location}:{stateKey}")]
  public class CacheKey
  {
    private readonly int hash;
    private readonly int location;
    private readonly string ruleName;
    private readonly int stateKey;

    public CacheKey(string ruleName, int stateKey, int location)
    {
      this.ruleName = ruleName;
      this.stateKey = stateKey;
      this.location = location;
      this.hash = ((-2128831035 * 16777619 ^ (this.ruleName == null ? 0 : this.ruleName.GetHashCode())) * 16777619 ^ this.stateKey) * 16777619 ^ this.location;
    }

    public override bool Equals(object obj)
    {
      if (this == obj)
        return true;
      return obj is CacheKey cacheKey && this.location == cacheKey.location && this.stateKey == cacheKey.stateKey && this.ruleName == cacheKey.ruleName;
    }

    public override int GetHashCode() => this.hash;
  }
}
