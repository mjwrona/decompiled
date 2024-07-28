// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.IDedupProvider
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupGC;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupGC.KeepUntil;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.Domains;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Azure;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  public interface IDedupProvider
  {
    bool ProviderRequireVss { get; }

    Task InitializeAsync(VssRequestPump.Processor processor, IDomainSecurityValidator validator);

    Task<DedupCompressedBuffer> GetDedupAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      DedupIdentifier dedupId);

    Task<IDedupInfo> GetDedupInfoAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      DedupIdentifier dedupId);

    Task<IDictionary<DedupIdentifier, PreauthenticatedUri>> GetDownloadUrlsAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      ISet<DedupIdentifier> dedupIds,
      SASUriExpiry expiry,
      (string, Guid)[] sasTracing);

    Task<DateTime> PutDedupAndKeepUntilReferenceAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      DedupIdentifier dedupId,
      DedupCompressedBuffer dedup,
      DateTime keepUntil,
      bool useHttpClient);

    Task<IDictionary<DedupIdentifier, DateTime?>> TryAddKeepUntilReferencesAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      ISet<DedupIdentifier> dedupIds,
      DateTime keepUntil);

    Task<MetadataOperationResult> TryExtendKeepUntilReferenceAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      DedupIdentifier dedupId,
      IKeepUntil keepUntil);

    Task<IDictionary<DedupIdentifier, KeepUntilResult?>> ValidateKeepUntilReferencesAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      ISet<DedupIdentifier> dedupIds,
      DateTime keepUntil);

    Task AddRootAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      DedupIdentifier dedupId,
      IdBlobReference rootRef,
      long size);

    Task RemoveRootAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      DedupIdentifier dedupId,
      IdBlobReference rootRef);

    Task RestoreRootAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      DedupIdentifier dedupId,
      IdBlobReference rootRef);

    Task UpdateRootSizeAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      DedupMetadataEntry entry);

    Task HardDeleteRootAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      DedupIdentifier dedupId,
      IdBlobReference rootRef);

    IConcurrentIterator<IEnumerable<DedupMetadataEntry>> GetRootPagesAsync(
      VssRequestPump.Processor processor,
      DedupMetadataPageRetrievalOption option);

    IConcurrentIterator<IEnumerable<KeyValuePair<DedupIdentifier, DateTime>>> GetDedupPagesAsync(
      VssRequestPump.SecuredDomainProcessor processor);

    IConcurrentIterator<BasicBlobMetadata> GetBasicBlobMetadataConcurrentIterator(
      VssRequestPump.SecuredDomainProcessor processor,
      IteratorPartition partition,
      string prefix);

    Task<DeleteResult> DeleteExpiredDedupsAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      ChunkDedupGCCheckpoint checkpointData,
      DateTimeOffset expiryKeepUntil,
      int boundedCapacity = 0);

    Task<bool> RestoreIfNotExists(
      VssRequestPump.SecuredDomainProcessor processor,
      DedupIdentifier dedupId);

    Task<DateTime> GetKeepUntilAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      DedupIdentifier dedupId);

    IEnumerable<Uri> GetContainerUrls(IDomainSecurityValidator validator);

    List<ICloudBlobContainer> Containers { get; }

    void ResetStorageStatistics(ITraceRequest tracer);

    void EnableStatistics(bool isEnabled);

    void EnableBlobOperationTimeout(bool isEnabled);
  }
}
