// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList.SourceChainMap
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList
{
  public class SourceChainMap
  {
    private readonly Dictionary<string, int> sourceChainsByKey = new Dictionary<string, int>();

    public List<IImmutableList<UpstreamSourceInfo>> SourceChains { get; } = new List<IImmutableList<UpstreamSourceInfo>>();

    public int GetOrAddSourceChainIndex(IReadOnlyList<UpstreamSourceInfo> sourceChain)
    {
      string key = string.Join<Guid>(",", sourceChain.Select<UpstreamSourceInfo, Guid>((Func<UpstreamSourceInfo, Guid>) (x => x.Id)));
      int count;
      if (this.sourceChainsByKey.TryGetValue(key, out count))
        return count;
      count = this.SourceChains.Count;
      this.SourceChains.Add((IImmutableList<UpstreamSourceInfo>) sourceChain.ToImmutableList<UpstreamSourceInfo>());
      this.sourceChainsByKey[key] = count;
      return count;
    }
  }
}
