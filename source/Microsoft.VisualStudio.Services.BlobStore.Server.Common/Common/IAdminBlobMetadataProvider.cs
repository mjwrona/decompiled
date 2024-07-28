// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.IAdminBlobMetadataProvider
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.Domains;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.Models;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  public interface IAdminBlobMetadataProvider : IBlobMetadataProvider, IDisposable
  {
    Task PerformSweepOperationAsync(
      VssRequestPump.Processor processor,
      SweepOperation sweepOperation,
      BlobIdentifier firstBlobId,
      BlobIdentifier lastBlobId);

    Task FixMetadataAfterDisasterAsync(
      VssRequestPump.Processor vssProcessor,
      FixMetadataAfterDisasterResults results,
      IBlobProviderDomain domain,
      BlobIdentifier startingBlobId,
      BlobIdentifier lastBlobId,
      DateTime lastSyncTime,
      bool skipDeletion,
      Func<int, TraceLevel, string, Task> traceFunc,
      CancellationToken cancellationToken);

    IConcurrentIterator<KeyValuePair<BlobIdentifier, DateTime>> GetBlobIdentifiersWithExpiredKeepUntilConcurrentIterator(
      VssRequestPump.Processor processor,
      BlobIdentifier startingBlobId,
      BlobIdentifier endingBlobIdOrNull,
      IteratorPartition partition,
      DateTime expiredUntilDate);

    IConcurrentIterator<IBlobMetadataSizeInfo> GetBlobMetadataSizeConcurrentIterator(
      VssRequestPump.Processor processor,
      BlobIdentifier startingBlobIdOrNull,
      BlobIdentifier endingBlobIdOrNull,
      IteratorPartition partition,
      bool excludeStartId,
      IEnumerable<IBlobIdReferenceRowVisitor> idBlobReferenceVisitors);

    IConcurrentIterator<IBlobMetadataProjectScopedSizeInfo> GetBlobMetadataProjectScopedSizeConcurrentIterator(
      VssRequestPump.Processor processor,
      BlobIdentifier startingBlobIdOrNull,
      BlobIdentifier endingBlobIdOrNull,
      IteratorPartition partition,
      bool excludeStartId,
      bool enableFeedInfoExport);

    IConcurrentIterator<BlobReferenceDetailInfo> GetAllReferencesConcurrentIterator(
      VssRequestPump.Processor processor);
  }
}
