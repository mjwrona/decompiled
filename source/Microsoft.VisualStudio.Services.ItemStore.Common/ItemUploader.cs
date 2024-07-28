// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.Common.ItemUploader
// Assembly: Microsoft.VisualStudio.Services.ItemStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 44753C0C-D541-4975-AF3F-2B606DE6FF70
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Common.Tracing;
using Microsoft.VisualStudio.Services.ItemStore.Common.Telemetry;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ItemStore.Common
{
  [CLSCompliant(false)]
  public class ItemUploader : IItemUploader, IDisposable
  {
    private static readonly ConcurrentDictionary<(Uri, IDomainId), Lazy<IBlobStoreHttpClient>> BlobStoreHttpClients = new ConcurrentDictionary<(Uri, IDomainId), Lazy<IBlobStoreHttpClient>>();
    private static readonly byte[] EmptyByteArray = Array.Empty<byte>();
    private readonly ArtifactHttpClientFactory clientFactory;
    private readonly int maxParallelItemUploading;
    private readonly int associateAsyncMaxRetries;
    private readonly IFileSystem fileSystem;
    private readonly IAppTraceSource traceSource;
    private bool disposedValue;
    private OverrideBlobStoreUri overrideBlobStoreUri;

    protected IEnvironment Environment
    {
      set
      {
        this.overrideBlobStoreUri = new OverrideBlobStoreUri(value);
        if (!this.overrideBlobStoreUri.IsOverrideInPlace)
          return;
        this.traceSource?.TraceEvent(TraceEventType.Verbose, 0, string.Format("BlobStoreOverride set to {0}", (object) this.overrideBlobStoreUri.BlobStoreOverrideUri));
      }
    }

    public ItemUploader(
      ArtifactHttpClientFactory clientFactory,
      IAppTraceSource traceSource,
      int maxParallelItemUploading,
      int associateAsyncMaxRetries,
      IFileSystem fileSystem)
    {
      ArgumentUtility.CheckForNull<ArtifactHttpClientFactory>(clientFactory, nameof (clientFactory));
      ArgumentUtility.CheckForNull<IAppTraceSource>(traceSource, nameof (traceSource));
      this.clientFactory = clientFactory;
      this.traceSource = traceSource;
      ArgumentUtility.CheckForOutOfRange(maxParallelItemUploading, nameof (maxParallelItemUploading), 1);
      ArgumentUtility.CheckForOutOfRange(associateAsyncMaxRetries, nameof (associateAsyncMaxRetries), 0);
      this.maxParallelItemUploading = maxParallelItemUploading;
      this.associateAsyncMaxRetries = associateAsyncMaxRetries;
      ArgumentUtility.CheckForNull<IFileSystem>(fileSystem, nameof (fileSystem));
      this.fileSystem = fileSystem;
      this.Environment = (IEnvironment) RealEnvironment.Instance;
    }

    public async Task<ItemAssociationResult> AssociateFilesAsync(
      IEnumerable<FileBlobDescriptor> fileChunkIds,
      IItemAssociator itemAssociator,
      ItemTreeInfo specItemTreeInfo,
      bool abortIfAlreadyExists,
      IDedupUploadSession uploadSession,
      CancellationToken cancellationToken,
      object routeValues)
    {
      List<FileBlobDescriptor> list = fileChunkIds.ToList<FileBlobDescriptor>();
      Dictionary<Locator, FileItem> itemsFromFileChunks = ItemUploader.ComputeFileItemsFromFileChunks(list);
      return await this.AssociateHelperAsync(list, itemsFromFileChunks, itemAssociator, specItemTreeInfo, abortIfAlreadyExists, uploadSession, cancellationToken, routeValues).ConfigureAwait(false);
    }

    public async Task<List<ItemUploaderRecord>> UploadFilesAsync(
      IEnumerable<FileBlobDescriptor> fileChunkIds,
      IItemAssociator itemAssociator,
      ItemTreeInfo specItemTreeInfo,
      bool abortIfAlreadyExists,
      IDedupUploadSession uploadSession,
      IDomainId domainId,
      CancellationToken cancellationToken,
      object routeValues)
    {
      List<FileBlobDescriptor> fileChunks = fileChunkIds.ToList<FileBlobDescriptor>();
      Dictionary<Locator, FileItem> fileItems = ItemUploader.ComputeFileItemsFromFileChunks(fileChunks);
      ItemAssociationResult associationResult = await this.AssociateHelperAsync(fileChunks, fileItems, itemAssociator, specItemTreeInfo, abortIfAlreadyExists, uploadSession, cancellationToken, routeValues).ConfigureAwait(false);
      List<ItemUploaderRecord> itemUploaderRecordList = await this.UploadAndAssociateFilesHelperAsync(associationResult.AssociationStatus, associationResult.ItemUploaderRecord, fileChunks, fileItems, itemAssociator, abortIfAlreadyExists, uploadSession, domainId, cancellationToken, routeValues).ConfigureAwait(false);
      fileChunks = (List<FileBlobDescriptor>) null;
      fileItems = (Dictionary<Locator, FileItem>) null;
      return itemUploaderRecordList;
    }

    public Task<List<ItemUploaderRecord>> UploadAndAssociateFilesAsync(
      AssociationsStatus firstAssociateResult,
      ItemUploaderRecord publishRecord,
      IEnumerable<FileBlobDescriptor> fileChunkIds,
      IItemAssociator itemAssociator,
      bool abortIfAlreadyExists,
      IDedupUploadSession uploadSession,
      IDomainId domainId,
      CancellationToken cancellationToken,
      object routeValues)
    {
      List<FileBlobDescriptor> list = fileChunkIds.ToList<FileBlobDescriptor>();
      Dictionary<Locator, FileItem> itemsFromFileChunks = ItemUploader.ComputeFileItemsFromFileChunks(list);
      return this.UploadAndAssociateFilesHelperAsync(firstAssociateResult, publishRecord, list, itemsFromFileChunks, itemAssociator, abortIfAlreadyExists, uploadSession, domainId, cancellationToken, routeValues);
    }

    public void Dispose() => this.Dispose(true);

    internal FileStream SafeCreateStream(string networkPath)
    {
      try
      {
        return this.SafeCreateStreamHelper(networkPath);
      }
      catch (Exception ex)
      {
        this.traceSource.TraceEvent(TraceEventType.Warning, 0, "Could not create stream at {0}. Exception {1}", (object) networkPath, (object) ex.ToString());
        return (FileStream) null;
      }
    }

    internal virtual FileStream SafeCreateStreamHelper(string networkPath) => FileStreamUtils.OpenFileStreamForAsync(networkPath, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Delete);

    protected virtual IBlobStoreHttpClient GetBlobClient(
      Uri contentServiceUri,
      IDomainId domainId,
      CancellationToken cancellationToken)
    {
      Uri blobStoreUri = this.overrideBlobStoreUri.GetUri(contentServiceUri);
      return ItemUploader.BlobStoreHttpClients.GetOrAdd((blobStoreUri, domainId), (Func<(Uri, IDomainId), Lazy<IBlobStoreHttpClient>>) (uri => new Lazy<IBlobStoreHttpClient>((Func<IBlobStoreHttpClient>) (() =>
      {
        IBlobStoreHttpClient blobClient = !domainId.Equals(WellKnownDomainIds.DefaultDomainId) ? (IBlobStoreHttpClient) new DomainBlobHttpClientWrapper(domainId, BlobStoreHttpClientFactory.GetDomainClient(blobStoreUri, this.clientFactory)) : BlobStoreHttpClientFactory.GetClient(blobStoreUri, this.clientFactory);
        blobClient.GetOptionsAsync(cancellationToken).SyncResult();
        return blobClient;
      }), LazyThreadSafetyMode.ExecutionAndPublication))).Value;
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.disposedValue)
        return;
      int num = disposing ? 1 : 0;
      this.disposedValue = true;
    }

    private static Dictionary<Locator, FileItem> ComputeFileItemsFromFileChunks(
      List<FileBlobDescriptor> fileChunks)
    {
      return fileChunks.ToDictionary<FileBlobDescriptor, Locator, FileItem>((Func<FileBlobDescriptor, Locator>) (fileChunkId => Locator.Parse(fileChunkId.RelativePath)), (Func<FileBlobDescriptor, FileItem>) (fileChunkId => ItemUploader.CreateFileItem(fileChunkId)));
    }

    private static FileItem CreateFileItem(FileBlobDescriptor descriptor)
    {
      if (!descriptor.FileSize.HasValue)
        throw new ArgumentException("FileBlobDescriptor is missing a FileSize: BlobId=" + descriptor.BlobIdentifier.ValueString + ", RelativePath=" + descriptor.RelativePath);
      if (descriptor.RelativePath.EndsWith(FileBlobDescriptorConstants.EmptyDirectoryEndingPattern))
      {
        FileItem fileItem = new FileItem(Enum.GetName(typeof (FileBlobType), (object) FileBlobType.EmptyDirectory).ToLower());
        fileItem.BlobIdentifier = descriptor.BlobIdentifier;
        fileItem.Length = descriptor.FileSize.Value;
        return fileItem;
      }
      FileItem fileItem1 = new FileItem();
      fileItem1.BlobIdentifier = descriptor.BlobIdentifier;
      fileItem1.Length = descriptor.FileSize.Value;
      FileItem fileItem2 = fileItem1;
      if (!string.IsNullOrWhiteSpace(descriptor.SymbolicLink))
        fileItem2.SymbolicLink = descriptor.SymbolicLink;
      if (descriptor.PermissionValue != 0U)
        fileItem2.PermissionValue = descriptor.PermissionValue;
      return fileItem2;
    }

    private async Task<ItemAssociationResult> AssociateHelperAsync(
      List<FileBlobDescriptor> fileChunks,
      Dictionary<Locator, FileItem> fileItems,
      IItemAssociator itemAssociator,
      ItemTreeInfo specItemTreeInfo,
      bool abortIfAlreadyExists,
      IDedupUploadSession uploadSession,
      CancellationToken cancellationToken,
      object routeValues)
    {
      ItemUploaderRecord record = new ItemUploaderRecord()
      {
        ItemsCount = (long) fileItems.Count,
        IsChunked = uploadSession != null
      };
      AssociationsStatus status = (AssociationsStatus) null;
      if (!fileChunks.Any<FileBlobDescriptor>())
      {
        status = new AssociationsStatus()
        {
          Missing = (IEnumerable<BlobIdentifier>) new List<BlobIdentifier>(0),
          ItemTreeInfo = specItemTreeInfo
        };
        record.AssociateElapsedTimeMs = 0L;
      }
      else
      {
        ItemUploaderRecord itemUploaderRecord = record;
        itemUploaderRecord.AssociateElapsedTimeMs = await TimedExecutionHelper.TimedExecuteAsync((Func<Task>) (async () => status = await this.AssociateItemsAsync((IEnumerable<KeyValuePair<Locator, FileItem>>) fileItems, itemAssociator, abortIfAlreadyExists, specItemTreeInfo, uploadSession, cancellationToken, routeValues, (IEnumerable<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks>) null).ConfigureAwait(false))).ConfigureAwait(false);
        itemUploaderRecord = (ItemUploaderRecord) null;
      }
      record.AssociateSuccess = true;
      HashSet<BlobIdentifier> pendingUploadBlobIds;
      this.CalculatePendingUploads(status.Missing.ToList<BlobIdentifier>(), fileChunks, out List<FileBlobDescriptor> _, out pendingUploadBlobIds);
      record.SumCachedBytes = fileItems.Where<KeyValuePair<Locator, FileItem>>((Func<KeyValuePair<Locator, FileItem>, bool>) (kvp => !pendingUploadBlobIds.Contains(kvp.Value.BlobIdentifier))).Sum<KeyValuePair<Locator, FileItem>>((Func<KeyValuePair<Locator, FileItem>, long>) (fileItem => fileItem.Value.Length));
      ItemAssociationResult associationResult = new ItemAssociationResult()
      {
        AssociationStatus = status,
        ItemUploaderRecord = record
      };
      record = (ItemUploaderRecord) null;
      return associationResult;
    }

    private async Task<List<ItemUploaderRecord>> UploadAndAssociateFilesHelperAsync(
      AssociationsStatus firstAssociateResult,
      ItemUploaderRecord publishRecord,
      List<FileBlobDescriptor> files,
      Dictionary<Locator, FileItem> fileItems,
      IItemAssociator itemAssociator,
      bool abortIfAlreadyExists,
      IDedupUploadSession uploadSession,
      IDomainId domainId,
      CancellationToken cancellationToken,
      object routeValues)
    {
      List<ItemUploaderRecord> stats = new List<ItemUploaderRecord>();
      List<BlobIdentifier> list = firstAssociateResult.Missing.ToList<BlobIdentifier>();
      if (!list.Any<BlobIdentifier>())
      {
        this.traceSource.TraceEvent(TraceEventType.Verbose, 0, "UploadAndAssociateFilesAsync had no missing files");
        publishRecord.SumCachedBytes = files.Select<FileBlobDescriptor, long>((Func<FileBlobDescriptor, long>) (i => i.FileSize.Value)).Sum();
        publishRecord.SealResult = SealResultType.Unnecessary;
        stats.Add(publishRecord);
        return stats;
      }
      int attempt = 2;
      AssociationsStatus status = (AssociationsStatus) null;
      ItemUploaderRecord currPublishRecord = new ItemUploaderRecord(publishRecord);
      do
      {
        this.traceSource.TraceEvent(TraceEventType.Verbose, 0, "UploadAndAssociateFilesAsync attempt {0} of {1}", (object) (2 - attempt), (object) 2);
        --attempt;
        if (attempt < 0)
        {
          currPublishRecord.SealResult = SealResultType.SealFailed;
          throw new AssociateException("Associate failed: " + (status?.ToJson().ToString() ?? "{null}"))
          {
            TransferFilesTelemetryRecord = currPublishRecord
          };
        }
        currPublishRecord = new ItemUploaderRecord(publishRecord);
        HashSet<BlobIdentifier> pendingUploadBlobIds;
        List<FileBlobDescriptor> pendingUploads;
        this.CalculatePendingUploads(list, files, out pendingUploads, out pendingUploadBlobIds);
        currPublishRecord.SumTransferredBytes = pendingUploads.Sum<FileBlobDescriptor>((Func<FileBlobDescriptor, long>) (file => file.FileSize.Value));
        currPublishRecord.FilesTransferredCount = (long) pendingUploads.Count;
        IEnumerable<KeyValuePair<Locator, FileItem>> remainingFileItems = fileItems.Where<KeyValuePair<Locator, FileItem>>((Func<KeyValuePair<Locator, FileItem>, bool>) (fileItem => pendingUploadBlobIds.Contains(fileItem.Value.BlobIdentifier)));
        this.traceSource.TraceEvent(TraceEventType.Verbose, 0, "UploadAndAssociateFilesAsync transferring {0} files", (object) pendingUploads.Count);
        ItemUploaderRecord itemUploaderRecord;
        if (pendingUploadBlobIds.All<BlobIdentifier>((Func<BlobIdentifier, bool>) (b => b.AlgorithmId == (byte) 0)))
        {
          IEnumerable<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks> uploadedBlobs = (IEnumerable<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks>) null;
          itemUploaderRecord = currPublishRecord;
          itemUploaderRecord.TransferTimeMs = await TimedExecutionHelper.TimedExecuteAsync((Func<Task>) (async () => uploadedBlobs = await this.UploadMissing(domainId, firstAssociateResult.ItemTreeInfo.BlobStoreUri, pendingUploads, cancellationToken).ConfigureAwait(false))).ConfigureAwait(false);
          itemUploaderRecord = (ItemUploaderRecord) null;
          itemUploaderRecord = currPublishRecord;
          itemUploaderRecord.SealTimeMs = await TimedExecutionHelper.TimedExecuteAsync((Func<Task>) (async () => status = await this.AssociateItemsAsync(remainingFileItems, itemAssociator, abortIfAlreadyExists, firstAssociateResult.ItemTreeInfo, (IDedupUploadSession) null, cancellationToken, routeValues, uploadedBlobs).ConfigureAwait(false))).ConfigureAwait(false);
          itemUploaderRecord = (ItemUploaderRecord) null;
        }
        else
        {
          if (!pendingUploadBlobIds.All<BlobIdentifier>((Func<BlobIdentifier, bool>) (b => b.AlgorithmId > (byte) 0)))
            throw new NotSupportedException("files contained a mix of both file-level and chunk-level FileBlobDescriptors. Please provide all file-level or all chunk-level.");
          if (uploadSession == null)
            throw new ArgumentNullException("uploadSession was null, but it must be provided when the files are chunk-level FileBlobDescriptors");
          itemUploaderRecord = currPublishRecord;
          itemUploaderRecord.TransferTimeMs = await TimedExecutionHelper.TimedExecuteAsync((Func<Task>) (async () =>
          {
            List<DedupNode> rootChildren = new List<DedupNode>(pendingUploads.Count);
            foreach (FileBlobDescriptor fileBlobDescriptor in pendingUploads)
            {
              DedupNode dedupNode;
              if (ChunkerHelper.IsChunk(fileBlobDescriptor.BlobIdentifier.AlgorithmId))
              {
                dedupNode = new DedupNode(new ChunkInfo(0UL, (uint) fileBlobDescriptor.FileSize.Value, fileBlobDescriptor.BlobIdentifier.AlgorithmResultBytes));
              }
              else
              {
                if (!ChunkerHelper.IsNode(fileBlobDescriptor.BlobIdentifier.AlgorithmId))
                  throw new NotImplementedException();
                dedupNode = await ChunkerHelper.CreateFromFileAsync(this.fileSystem, fileBlobDescriptor.AbsolutePath, cancellationToken, false).ConfigureAwait(false);
              }
              rootChildren.Add(dedupNode);
            }
            while (rootChildren.Count > 512)
              rootChildren = rootChildren.GetPages<DedupNode>(512).Select<List<DedupNode>, DedupNode>((Func<List<DedupNode>, DedupNode>) (page => new DedupNode((IEnumerable<DedupNode>) page))).ToList<DedupNode>();
            DedupNode node = new DedupNode((IEnumerable<DedupNode>) rootChildren);
            KeepUntilReceipt keepUntilReceipt = await uploadSession.UploadAsync(node, (IReadOnlyDictionary<DedupIdentifier, string>) pendingUploads.ToDictionary<FileBlobDescriptor, DedupIdentifier, string>((Func<FileBlobDescriptor, DedupIdentifier>) (d => d.BlobIdentifier.ToDedupIdentifier()), (Func<FileBlobDescriptor, string>) (d => d.AbsolutePath)), cancellationToken).ConfigureAwait(false);
            rootChildren = (List<DedupNode>) null;
          })).ConfigureAwait(false);
          itemUploaderRecord = (ItemUploaderRecord) null;
          itemUploaderRecord = currPublishRecord;
          itemUploaderRecord.SealTimeMs = await TimedExecutionHelper.TimedExecuteAsync((Func<Task>) (async () => status = await this.AssociateItemsAsync(remainingFileItems, itemAssociator, abortIfAlreadyExists, firstAssociateResult.ItemTreeInfo, uploadSession, cancellationToken, routeValues, (IEnumerable<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks>) null).ConfigureAwait(false))).ConfigureAwait(false);
          itemUploaderRecord = (ItemUploaderRecord) null;
        }
        currPublishRecord.SealResult = SealResultType.Sealed;
        list = status.Missing.ToList<BlobIdentifier>();
        stats.Add(currPublishRecord);
      }
      while (list.Any<BlobIdentifier>());
      currPublishRecord = (ItemUploaderRecord) null;
      return stats;
    }

    private void CalculatePendingUploads(
      List<BlobIdentifier> missingFiles,
      List<FileBlobDescriptor> fileChunks,
      out List<FileBlobDescriptor> pendingUploads,
      out HashSet<BlobIdentifier> pendingUploadBlobIds)
    {
      HashSet<BlobIdentifier> blobIdentifierSet = new HashSet<BlobIdentifier>((IEnumerable<BlobIdentifier>) missingFiles);
      int count1 = blobIdentifierSet.Count;
      pendingUploads = blobIdentifierSet.Select<BlobIdentifier, FileBlobDescriptor>((Func<BlobIdentifier, FileBlobDescriptor>) (missingBlobId => fileChunks.FirstOrDefault<FileBlobDescriptor>((Func<FileBlobDescriptor, bool>) (file => file.BlobIdentifier == missingBlobId)))).Where<FileBlobDescriptor>((Func<FileBlobDescriptor, bool>) (pendingUpload => pendingUpload != null)).ToList<FileBlobDescriptor>();
      int count2 = pendingUploads.Count;
      if (count2 < count1)
      {
        IEnumerable<BlobIdentifier> second = fileChunks.Select<FileBlobDescriptor, BlobIdentifier>((Func<FileBlobDescriptor, BlobIdentifier>) (d => d.BlobIdentifier)).Distinct<BlobIdentifier>();
        string str = string.Join(", ", blobIdentifierSet.Except<BlobIdentifier>(second).Select<BlobIdentifier, string>((Func<BlobIdentifier, string>) (b => b.ValueString)));
        throw new AssociateException(string.Format("{0} blobs are missing but only {1} would be uploaded because these {2}s do not have a corresponding {3}: {4}", (object) count1, (object) count2, (object) "BlobIdentifier", (object) "FileBlobDescriptor", (object) str));
      }
      pendingUploadBlobIds = new HashSet<BlobIdentifier>((IEnumerable<BlobIdentifier>) pendingUploads.Select<FileBlobDescriptor, BlobIdentifier>((Func<FileBlobDescriptor, BlobIdentifier>) (fileBlobDesc => fileBlobDesc.BlobIdentifier)).ToList<BlobIdentifier>());
    }

    private Task<AssociationsStatus> AssociateItemsAsync(
      IEnumerable<KeyValuePair<Locator, FileItem>> fileItems,
      IItemAssociator itemAssociator,
      bool abortIfAlreadyExists,
      ItemTreeInfo specItemTreeInfo,
      IDedupUploadSession uploadSession,
      CancellationToken cancellationToken,
      object routeValues,
      IEnumerable<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks> blobsUploaded)
    {
      cancellationToken.ThrowIfCancellationRequested();
      if (routeValues == null)
        routeValues = (object) new{  };
      AssociationsItem spec;
      if (uploadSession != null)
      {
        AssociationsItem associationsItem = new AssociationsItem(abortIfAlreadyExists);
        associationsItem.ItemTreeInfo = specItemTreeInfo;
        associationsItem.Items = fileItems.BaseCast<Locator, FileItem, Locator, StoredItem>();
        associationsItem.DedupUploadStats = uploadSession.UploadStatistics;
        spec = associationsItem;
        spec.ProofNodes = ProofHelper.CreateProofNodes(uploadSession.AllNodes, uploadSession.ParentLookup, fileItems.Select<KeyValuePair<Locator, FileItem>, DedupIdentifier>((Func<KeyValuePair<Locator, FileItem>, DedupIdentifier>) (f => f.Value.BlobIdentifier.ToDedupIdentifier())).Distinct<DedupIdentifier>()).Select<DedupNode, byte[]>((Func<DedupNode, byte[]>) (n => n.Serialize()));
      }
      else
      {
        AssociationsItem associationsItem = new AssociationsItem(abortIfAlreadyExists);
        associationsItem.ItemTreeInfo = specItemTreeInfo;
        associationsItem.Items = fileItems.BaseCast<Locator, FileItem, Locator, StoredItem>();
        spec = associationsItem;
      }
      if (blobsUploaded != null)
        spec.BlobsUploaded = blobsUploaded;
      return AsyncHttpRetryHelper<AssociationsStatus>.InvokeAsync((Func<Task<AssociationsStatus>>) (() => itemAssociator.AssociateAsync(routeValues, spec, cancellationToken)), this.associateAsyncMaxRetries, this.traceSource, (Func<Exception, bool>) null, cancellationToken, false, nameof (AssociateItemsAsync));
    }

    private async Task<IEnumerable<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks>> UploadMissing(
      IDomainId domainId,
      Uri blobStoreUri,
      List<FileBlobDescriptor> files,
      CancellationToken cancellationToken)
    {
      ItemUploader itemUploader = this;
      cancellationToken.ThrowIfCancellationRequested();
      // ISSUE: reference to a compiler-generated method
      return await itemUploader.GetBlobClient(blobStoreUri, domainId, cancellationToken).UploadBlocksForBlobsAsync(files.Select<FileBlobDescriptor, BlobToUriMapping>(new Func<FileBlobDescriptor, BlobToUriMapping>(itemUploader.\u003CUploadMissing\u003Eb__26_0)), cancellationToken).ConfigureAwait(false);
    }

    private Lazy<Stream> CreateStreamFactory(FileBlobDescriptor fileChunkId) => new Lazy<Stream>((Func<Stream>) (() =>
    {
      IEnumerable<string> list = (IEnumerable<string>) (fileChunkId.NetworkPaths ?? new List<string>()).Where<string>((Func<string, bool>) (p => !string.IsNullOrWhiteSpace(p))).ToList<string>();
      Stream streamFactory;
      if (fileChunkId.BlobIdentifier.IsOfNothing())
        streamFactory = (Stream) new MemoryStream(ItemUploader.EmptyByteArray);
      else if (!list.Any<string>())
      {
        streamFactory = (Stream) this.fileSystem.OpenFileStreamForAsync(fileChunkId.AbsolutePath, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Delete);
      }
      else
      {
        streamFactory = (Stream) list.Select<string, FileStream>(new Func<string, FileStream>(this.SafeCreateStream)).FirstOrDefault<FileStream>((Func<FileStream, bool>) (s => s != null));
        if (streamFactory == null)
          throw new NetworkPathsNotFoundException(string.Format("Could not open stream for network file: {0}@{1}", (object) fileChunkId.BlobIdentifier, (object) fileChunkId.RelativePath));
      }
      return streamFactory;
    }));
  }
}
