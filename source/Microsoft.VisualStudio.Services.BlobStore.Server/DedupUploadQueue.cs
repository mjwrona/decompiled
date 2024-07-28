// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.DedupUploadQueue
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.BlobStore.WebApi;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Common.Tracing;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  internal class DedupUploadQueue : IDedupProcessingQueue
  {
    public const string QueueSizeEnvVar = "VSO_AS_DEDUP_UPLOAD_QUEUE_SIZE";
    public const int DefaultQueueSize = 1024;
    public const int DefaultKeepUntilDay = 1;
    public const int DefaultRetries = 5;
    private BlockingCollection<(DedupIdentifier, byte[])> _dedups;
    private Task consumer;
    private readonly IVssRequestContext requestContext;
    private readonly IDomainId domainId;
    private readonly CancellationToken cancellationToken;

    public bool IsNewRoot { get; private set; }

    public int Capacity => this._dedups.BoundedCapacity;

    public int Retries { get; set; } = 5;

    public DedupUploadQueue(
      IVssRequestContext requestContext,
      IDomainId domainId,
      CancellationToken cancellationToken)
    {
      this._dedups = new BlockingCollection<(DedupIdentifier, byte[])>(int.Parse(Environment.GetEnvironmentVariable("VSO_AS_DEDUP_UPLOAD_QUEUE_SIZE") ?? 1024.ToString()));
      this.requestContext = requestContext;
      this.domainId = domainId;
      this.cancellationToken = cancellationToken;
      this.Start();
    }

    private void Start() => this.consumer = this.requestContext.Fork((Func<IVssRequestContext, Task>) (forkedContext => this.ConsumeAndUpload(forkedContext, this.cancellationToken)), nameof (Start));

    public void Add(DedupIdentifier dedupId, byte[] content) => this._dedups.Add((dedupId, content));

    public Task FlushAsync()
    {
      this._dedups.CompleteAdding();
      this.consumer.Wait();
      this._dedups.Dispose();
      return Task.CompletedTask;
    }

    private async Task ConsumeAndUpload(
      IVssRequestContext requestContext,
      CancellationToken cancellationToken)
    {
      IDedupStore service = requestContext.GetService<IDedupStore>();
      DateTime dateTime = DateTime.UtcNow;
      dateTime = dateTime.AddDays(1.0);
      KeepUntilBlobReference keepUntil = new KeepUntilBlobReference(dateTime.ToString(KeepUntilBlobReference.KeepUntilFormat));
      foreach ((DedupIdentifier dedupIdentifier, byte[] uncompressed) in this._dedups.GetConsumingEnumerable(cancellationToken))
      {
        using (DedupCompressedBuffer buffer = DedupCompressedBuffer.FromUncompressed(uncompressed))
          await AsyncHttpRetryHelper.InvokeVoidAsync((Func<Task>) (async () =>
          {
            if (ChunkerHelper.IsChunk(dedupId.AlgorithmId))
            {
              if (!(await service.TryKeepUntilReferenceChunkAsync(requestContext, this.domainId, (ChunkDedupIdentifier) dedupId, keepUntil) == (KeepUntilReceipt) null))
                return;
              resultChunk = await service.PutChunkAndKeepUntilReferenceAsync(requestContext, this.domainId, (ChunkDedupIdentifier) dedupId, buffer, keepUntil);
              this.IsNewRoot = true;
            }
            else
            {
              resultNode = await service.PutNodeAndKeepUntilReferenceAsync(requestContext, this.domainId, (NodeDedupIdentifier) dedupId, buffer, keepUntil, (SummaryKeepUntilReceipt) null);
              resultNode.Match(closure_5 ?? (closure_5 = (Action<DedupNodeChildrenNeedAction>) (needAction =>
              {
                throw new BlobNotFoundException(string.Format("Missing children nodes of {0}", (object) dedupIdentifier));
              })), closure_4 ?? (closure_4 = (Action<DedupNodeUpdated>) (added => this.IsNewRoot = added.IsNewNode)));
            }
          }), this.Retries, (IAppTraceSource) NoopAppTraceSource.Instance, (Func<Exception, bool>) (e => e is BlobNotFoundException), cancellationToken, true, string.Format("Upload node {0} to domain {1}.", (object) dedupId, (object) this.domainId));
      }
    }
  }
}
