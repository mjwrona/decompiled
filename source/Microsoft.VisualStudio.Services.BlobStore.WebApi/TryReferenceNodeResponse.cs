// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.TryReferenceNodeResponse
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using Microsoft.VisualStudio.Services.Content.Common;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  public class TryReferenceNodeResponse : 
    TaggedUnion<DedupNodeNotFound, DedupNodeChildrenNotEnoughKeepUntil, DedupNodeUpdated>
  {
    public TryReferenceNodeResponse(DedupNodeNotFound arg)
      : base(arg)
    {
    }

    public TryReferenceNodeResponse(DedupNodeChildrenNotEnoughKeepUntil arg)
      : base(arg)
    {
    }

    public TryReferenceNodeResponse(DedupNodeUpdated arg)
      : base(arg)
    {
    }
  }
}
