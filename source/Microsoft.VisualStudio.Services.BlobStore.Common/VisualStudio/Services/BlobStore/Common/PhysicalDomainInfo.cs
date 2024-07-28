// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.PhysicalDomainInfo
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using Microsoft.VisualStudio.Services.Content.Common.MultiDomainExceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  [Serializable]
  public class PhysicalDomainInfo : MultiDomainInfo
  {
    [JsonIgnore]
    public readonly HashSet<string> Shards;

    public PhysicalDomainInfo(
      IDomainId domainId,
      bool isDefault,
      string region,
      string redundancyType,
      HashSet<string> shards)
      : base(domainId, isDefault, region, redundancyType)
    {
      if (shards == null || shards.Count == 0)
        throw new InvalidDomainShardListException("shards was null or empty for domainId " + domainId.Serialize() + ".");
      this.Shards = !shards.Any<string>((Func<string, bool>) (s => string.IsNullOrWhiteSpace(s))) ? new HashSet<string>((IEnumerable<string>) shards, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : throw new InvalidDomainShardListException("shards contains a blank shard name");
    }
  }
}
