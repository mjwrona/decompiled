// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.SecurityChecksCache.RepoHash
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Query.SecurityChecksCache
{
  public class RepoHash
  {
    private readonly byte[] m_hash;

    public byte[] Hash => this.m_hash;

    public RepoHash(byte[] hash) => this.m_hash = hash;

    public override bool Equals(object obj)
    {
      RepoHash repoHash = obj as RepoHash;
      if (this == obj)
        return true;
      if (repoHash == null)
        return false;
      if (this.Hash != null)
        return ((IEnumerable<byte>) this.Hash).SequenceEqual<byte>((IEnumerable<byte>) repoHash.Hash);
      return repoHash.Hash == null;
    }

    public override int GetHashCode() => BitConverter.ToInt32(this.Hash, 0);
  }
}
