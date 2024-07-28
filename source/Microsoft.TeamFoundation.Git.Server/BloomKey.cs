// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.BloomKey
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  public sealed class BloomKey
  {
    private readonly uint[] m_hashes;

    internal BloomKey(string key)
      : this(key, BloomFilterSettings.Default)
    {
    }

    internal BloomKey(string key, BloomFilterSettings settings)
    {
      this.Key = key;
      this.m_hashes = settings.HashFunction.GetHashValues(key, (uint) settings.NumBitsOnInMask);
    }

    public string Key { get; }

    public IReadOnlyList<uint> BaseHashes => (IReadOnlyList<uint>) this.m_hashes;
  }
}
