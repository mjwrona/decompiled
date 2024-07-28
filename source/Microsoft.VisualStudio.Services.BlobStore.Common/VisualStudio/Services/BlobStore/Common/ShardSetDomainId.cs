// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.ShardSetDomainId
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  [Serializable]
  public sealed class ShardSetDomainId : IDomainId
  {
    public Guid ShardSetId { get; }

    public ShardSetDomainId()
    {
    }

    [JsonConstructor]
    public ShardSetDomainId(Guid shardSetId) => this.ShardSetId = shardSetId;

    public override bool Equals(IDomainId other) => other is ShardSetDomainId shardSetDomainId && this.ShardSetId.Equals(shardSetDomainId.ShardSetId);

    public override int GetHashCode() => this.Serialize().GetHashCode();

    public override string Serialize() => this.ShardSetId.ToString("N");

    public static bool TryParse(string input, out ShardSetDomainId result, out string error)
    {
      result = (ShardSetDomainId) null;
      error = string.Empty;
      if (string.IsNullOrWhiteSpace(input))
      {
        error = Resources.DomainIdNullError();
        return false;
      }
      Guid result1;
      if (!Guid.TryParse(input, out result1))
      {
        error = Resources.InvalidShardSetIdFormatError((object) input);
        return false;
      }
      if (result1 == Guid.Empty)
      {
        error = Resources.EmptyGuidShardSetIdError();
        return false;
      }
      result = new ShardSetDomainId(result1);
      return true;
    }
  }
}
