// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.IDedupProviderExtensions
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  public static class IDedupProviderExtensions
  {
    public static async Task<PreauthenticatedUri> GetDownloadUrlAsync(
      this IDedupProvider provider,
      VssRequestPump.SecuredDomainProcessor processor,
      DedupIdentifier dedupId,
      SASUriExpiry expiry,
      (string, Guid)[] sasTracing)
    {
      return (await provider.GetDownloadUrlsAsync(processor, (ISet<DedupIdentifier>) new HashSet<DedupIdentifier>((IEnumerable<DedupIdentifier>) new DedupIdentifier[1]
      {
        dedupId
      }), expiry, sasTracing)).Single<KeyValuePair<DedupIdentifier, PreauthenticatedUri>>().Value;
    }

    public static async Task<DateTime?> TryAddKeepUntilReferenceAsync(
      this IDedupProvider provider,
      VssRequestPump.SecuredDomainProcessor processor,
      DedupIdentifier dedupId,
      DateTime keepUntil)
    {
      return (await provider.TryAddKeepUntilReferencesAsync(processor, (ISet<DedupIdentifier>) new HashSet<DedupIdentifier>((IEnumerable<DedupIdentifier>) new DedupIdentifier[1]
      {
        dedupId
      }), keepUntil)).Single<KeyValuePair<DedupIdentifier, DateTime?>>().Value;
    }

    public static async Task<KeepUntilResult?> ValidateKeepUntilReferenceAsync(
      this IDedupProvider provider,
      VssRequestPump.SecuredDomainProcessor processor,
      DedupIdentifier dedupId,
      DateTime keepUntil)
    {
      return (await provider.ValidateKeepUntilReferencesAsync(processor, (ISet<DedupIdentifier>) new HashSet<DedupIdentifier>((IEnumerable<DedupIdentifier>) new DedupIdentifier[1]
      {
        dedupId
      }), keepUntil)).Single<KeyValuePair<DedupIdentifier, KeepUntilResult?>>().Value;
    }
  }
}
