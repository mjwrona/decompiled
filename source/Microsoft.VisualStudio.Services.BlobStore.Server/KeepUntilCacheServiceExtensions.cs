// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.KeepUntilCacheServiceExtensions
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public static class KeepUntilCacheServiceExtensions
  {
    internal static event KeepUntilCacheServiceExtensions.OnHitHandler OnHit;

    public static List<BlobReference> GetKeepUntilCacheMisses(
      this IBlobKeepUntilCacheService cache,
      IVssRequestContext requestContext,
      IDomainId domainId,
      BlobIdentifier blobId,
      IEnumerable<BlobReference> references,
      TimeSpan clockSkew)
    {
      List<BlobReference> misses = new List<BlobReference>();
      foreach (BlobReference reference1 in references)
      {
        BlobReference reference = reference1;
        reference.Match((Action<IdBlobReference>) (idReference => misses.Add(reference)), (Action<KeepUntilBlobReference>) (keepUntilReference =>
        {
          DateTime keepUntil;
          if (!cache.TryGetKeepUntil(requestContext, domainId, blobId, out keepUntil))
            misses.Add(reference);
          else if (keepUntil < keepUntilReference.KeepUntil + clockSkew)
          {
            misses.Add(reference);
          }
          else
          {
            KeepUntilCacheServiceExtensions.OnHitHandler onHit = KeepUntilCacheServiceExtensions.OnHit;
            if (onHit == null)
              return;
            onHit(blobId);
          }
        }));
      }
      return misses;
    }

    internal delegate void OnHitHandler(BlobIdentifier blobId);
  }
}
