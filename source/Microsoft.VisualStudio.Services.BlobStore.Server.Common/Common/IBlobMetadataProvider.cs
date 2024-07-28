// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.IBlobMetadataProvider
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  public interface IBlobMetadataProvider : IDisposable
  {
    Task<IBlobMetadata> GetAsync(VssRequestPump.Processor processor, BlobIdentifier blobId);

    Task<bool> TryUpdateAsync(VssRequestPump.Processor processor, IBlobMetadata updatedMetadata);

    Task<IEnumerable<ReferenceResult>> TryReferenceAsync(
      VssRequestPump.Processor processor,
      BlobIdentifier blobId,
      IEnumerable<BlobReference> references);

    Task<IEnumerable<ReferenceResult>> TryReferenceWithBlobAsync(
      VssRequestPump.Processor processor,
      IBlobMetadata metadata,
      IEnumerable<BlobReference> references);

    Task<IEnumerable<RemoveReferenceResult>> RemoveReferencesAsync(
      VssRequestPump.Processor processor,
      BlobIdentifier blobId,
      IEnumerable<IdBlobReference> references);

    Task<KeepUntilBlobReference?> GetKeepUntilReferenceAsync(
      VssRequestPump.Processor processor,
      BlobIdentifier blobId);

    Task<bool> TryDeleteAsync(VssRequestPump.Processor processor, IBlobMetadata metadata);

    bool HasReferences(IBlobMetadata metadata);

    TimeSpan MaxExpectedClockSkew { get; }
  }
}
