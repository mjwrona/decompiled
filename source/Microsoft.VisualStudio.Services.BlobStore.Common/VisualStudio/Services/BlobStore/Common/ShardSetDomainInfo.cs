// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.ShardSetDomainInfo
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using Microsoft.VisualStudio.Services.Content.Common.MultiDomainExceptions;
using Newtonsoft.Json;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  public class ShardSetDomainInfo : MultiDomainInfo
  {
    [JsonConverter(typeof (DomainIdJsonConverter))]
    public string ContainerSetId { get; }

    public string ShardSetBaseUrl { get; }

    public ShardSetDomainInfo(
      ShardSetDomainId shardSetDomainId,
      string region,
      string redundancyType,
      string containerSetId,
      string shardSetBaseUrl)
      : base((IDomainId) shardSetDomainId, false, region, redundancyType)
    {
      if (!Guid.TryParse(containerSetId, out Guid _))
        throw new InvalidContainerSetIdException("containerSetId was invalid. containerSetId must be a guid string.");
      if (!Uri.TryCreate(shardSetBaseUrl, UriKind.Absolute, out Uri _))
        throw new InvalidShardSetUrlException("shardSetBaseUrl was invalid.");
      this.ContainerSetId = containerSetId;
      this.ShardSetBaseUrl = shardSetBaseUrl;
    }
  }
}
