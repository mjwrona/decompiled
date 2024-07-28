// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.ServerSideDedup.ServerDeduplicationService
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.ServerSideDedup
{
  public class ServerDeduplicationService : IServerDeduplicationService, IVssFrameworkService
  {
    public async Task<UploadResult> ChunkAndUploadAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      Stream stream,
      CancellationToken cancellationToken)
    {
      DedupUploadQueue uploadQueue = new DedupUploadQueue(requestContext, domainId, cancellationToken);
      UploadResult uploadResult = new UploadResult(await new StreamChunker((IDedupProcessingQueue) uploadQueue, cancellationToken).CreateFromStreamAsync(stream), uploadQueue.IsNewRoot);
      uploadQueue = (DedupUploadQueue) null;
      return uploadResult;
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }
  }
}
