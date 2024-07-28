// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.IAdminDedupStore
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public interface IAdminDedupStore : IDedupStore, IVssFrameworkService
  {
    IConcurrentIterator<IEnumerable<DedupMetadataEntry>> GetRootPages(
      VssRequestPump.SecuredDomainProcessor processor,
      DedupMetadataPageRetrievalOption option,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer);

    Task HardDeleteRootAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      DedupMetadataEntry dedupMetadataEntry);

    Task<DedupNode?> GetDedupNodeAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      NodeDedupIdentifier nodeId);

    Task<long> RestoreDedupTree(
      VssRequestPump.SecuredDomainProcessor processor,
      DedupIdentifier dedupId,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer);

    Task<DedupsValidationResult> VerifyRootsAsync(
      IVssRequestContext requestContext,
      ChunkDedupGCCheckpoint checkpoint,
      IDomainId domainId,
      DateTimeOffset keepUntil);

    Task<MarkResult> MarkRootsAsync(
      IVssRequestContext requestContext,
      ChunkDedupGCCheckpoint checkpoint,
      IDomainId domainId,
      DateTimeOffset expiryKeepUntil);

    Task<DeleteResult> DeleteExpiredDedupsAsync(
      IVssRequestContext requestContext,
      ChunkDedupGCCheckpoint checkpoint,
      IDomainId domainId,
      DateTimeOffset expiryKeepUntil);

    Task<bool> UpdateRootSizeAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      DedupMetadataEntry rootEntry);
  }
}
