// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.IBlobProvider
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  public interface IBlobProvider : IDisposable
  {
    Task<string> GetBlobEtagAsync(VssRequestPump.Processor processor, BlobIdentifier blobId);

    Task<long?> GetBlobLengthAsync(VssRequestPump.Processor processor, BlobIdentifier blobId);

    Task<DisposableEtagValue<Stream>> GetBlobAsync(
      VssRequestPump.Processor processor,
      BlobIdentifier blobId);

    Task PutBlobBlockByteArrayAsync(
      VssRequestPump.Processor processor,
      BlobIdentifier blobId,
      ArraySegment<byte> blobBlock,
      string blockName,
      bool useHttpClient);

    Task<IDictionary<BlobIdentifier, PreauthenticatedUri>> GetDownloadUrisAsync(
      VssRequestPump.Processor processor,
      IEnumerable<BlobIdentifier> identifiers,
      SASUriExpiry expiry,
      string policyId,
      (string, Guid)[] sasTracing);

    Task<PreauthenticatedUri> GetDownloadUriAsync(
      VssRequestPump.Processor processor,
      BlobIdWithHeaders blobId,
      SASUriExpiry expiry,
      string policyId,
      (string, Guid)[] sasTracing);

    Task<EtagValue<IList<BlockInfo>>> GetBlockListAsync(
      VssRequestPump.Processor processor,
      BlobIdentifier blobId);

    Task<EtagValue<bool>> PutBlockListAsync(
      VssRequestPump.Processor processor,
      BlobIdentifier blobId,
      string etagToMatch,
      IEnumerable<string> blockIds);

    Task<EtagValue<bool>> PutBlobByteArrayAsync(
      VssRequestPump.Processor processor,
      BlobIdentifier blobId,
      string etagToMatch,
      ArraySegment<byte> data,
      bool useHttpClient);

    IConcurrentIterator<BlobIdentifier> GetBlobIdentifiersConcurrentIterator(
      VssRequestPump.Processor processor);

    Task<EtagValue<bool>> DeleteBlobAsync(
      VssRequestPump.Processor processor,
      BlobIdentifier blobId,
      string etagToDelete);

    Task<PreauthenticatedUri> GetContainerUri(VssRequestPump.Processor processor, string policyId);

    IConcurrentIterator<IEnumerable<BasicBlobMetadata>> GetBasicBlobMetadataConcurrentIterator(
      VssRequestPump.Processor processor,
      string prefix);
  }
}
