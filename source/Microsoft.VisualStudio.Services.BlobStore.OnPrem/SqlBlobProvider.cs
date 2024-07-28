// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.OnPrem.SqlBlobProvider
// Assembly: Microsoft.VisualStudio.Services.BlobStore.OnPrem, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EA52CF3A-8E8F-49A1-8A12-783B16F9478A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.OnPrem.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.BlobStore.WebApi;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.OnPrem
{
  public sealed class SqlBlobProvider : Microsoft.VisualStudio.Services.BlobStore.Server.Common.IBlobProvider, IDisposable
  {
    private readonly int m_containerId;
    private readonly TraceData traceData = new TraceData()
    {
      Area = "BlobStore",
      Layer = nameof (SqlBlobProvider)
    };
    private const string blobAreaPath = "blob/blobs/";
    private readonly Func<IVssRequestContext, BlobStoreComponent> m_blobStoreComponentFactory;
    private readonly Func<FileComponent> m_fileComponentFactory;

    internal static OwnerId OwnerId => OwnerId.BlobStore;

    public SqlBlobProvider(string suffix)
      : this(suffix, SqlBlobProvider.\u003C\u003EO.\u003C0\u003E__DefaultFactory ?? (SqlBlobProvider.\u003C\u003EO.\u003C0\u003E__DefaultFactory = new Func<IVssRequestContext, BlobStoreComponent>(SqlBlobProvider.DefaultFactory)), (FileComponentConstructionProperties) null)
    {
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
    }

    internal SqlBlobProvider(
      string suffix,
      Func<IVssRequestContext, BlobStoreComponent> blobStoreComponentFactory,
      FileComponentConstructionProperties properties)
    {
      this.m_containerId = SqlBlobProvider.ConvertBlobSuffixToId(suffix);
      this.m_blobStoreComponentFactory = blobStoreComponentFactory;
      if (properties == null)
        return;
      int version;
      using (FileComponent componentRaw = properties.SqlConnectionInfo.CreateComponentRaw<FileComponent>())
        version = componentRaw.Version;
      IComponentCreator componentCreator = FileComponent.ComponentFactory.GetComponentCreator(version, version);
      this.m_fileComponentFactory = (Func<FileComponent>) (() =>
      {
        FileComponent fileComponent = componentCreator.Create(properties.SqlConnectionInfo, 3600, 0, 200, (ITFLogger) new NullLogger(), (CircuitBreakerDatabaseProperties) null) as FileComponent;
        fileComponent.PartitionId = properties.PartitionId;
        return fileComponent;
      });
    }

    public static int ConvertBlobSuffixToId(string suffix) => suffix.GetUTF8Bytes().CalculateBlockHash((IBlobHasher) Microsoft.VisualStudio.Services.BlobStore.Common.VsoHash.Instance).GetHashCode();

    public async Task<EtagValue<bool>> PutBlobByteArrayAsync(
      VssRequestPump.Processor processor,
      BlobIdentifier blobId,
      string etagToMatch,
      ArraySegment<byte> data,
      bool useHttpClient = false)
    {
      if (data.Array == null)
        throw new ArgumentNullException(nameof (data));
      if (blobId == (BlobIdentifier) null)
        throw new ArgumentNullException(nameof (blobId));
      EtagValue<bool> evalue = new EtagValue<bool>(true, etagToMatch);
      using (MemoryStream ms = data.AsMemoryStream())
        await Microsoft.VisualStudio.Services.BlobStore.Common.VsoHash.WalkBlocksAsync((Stream) ms, (SemaphoreSlim) null, false, (Microsoft.VisualStudio.Services.BlobStore.Common.SingleBlockBlobCallbackAsync) (async (block, blockLength, blockHash) =>
        {
          await this.PutBlobBlockInternalAsync(processor, blobId, new ArraySegment<byte>(block, 0, blockLength), blockHash.BlockHashes[0].HashString, true);
          evalue = await this.PutBlockListAsync(processor, blobId, etagToMatch, blockHash.BlockHashes.Select<Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash, string>((Func<Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash, string>) (hash => hash.HashString)));
        }), (MultiBlockBlobCallbackAsync) (async (block, blockLength, blockHash, isFinalBlock) => await this.PutBlobBlockByteArrayAsync(processor, blobId, new ArraySegment<byte>(block, 0, blockLength), blockHash.HashString, false)), (MultiBlockBlobSealCallbackAsync) (async blobIdWithBlocks => evalue = await this.PutBlockListAsync(processor, blobId, etagToMatch, blobIdWithBlocks.BlockHashes.Select<Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash, string>((Func<Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash, string>) (hash => hash.HashString))))).ConfigureAwait(true);
      return evalue;
    }

    public Task PutBlobBlockByteArrayAsync(
      VssRequestPump.Processor processor,
      BlobIdentifier blobId,
      ArraySegment<byte> blobBlock,
      string blockName,
      bool useHttpClient)
    {
      return this.PutBlobBlockInternalAsync(processor, blobId, blobBlock, blockName, false);
    }

    public Task<EtagValue<bool>> PutBlockListAsync(
      VssRequestPump.Processor processor,
      BlobIdentifier blobId,
      string etagToMatch,
      IEnumerable<string> blockIds)
    {
      return processor.ExecuteWorkAsync<EtagValue<bool>>((Func<IVssRequestContext, EtagValue<bool>>) (requestContext =>
      {
        using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, this.traceData, 5707020, nameof (PutBlockListAsync)))
        {
          if (!blockIds.Any<string>())
          {
            InvalidOperationException operationException = new InvalidOperationException("Cannot seal blob \"" + blobId.ValueString + "\" with an empty block list. Aborting PutBlockListAsync.");
            tracer.TraceException((Exception) operationException);
            throw operationException;
          }
          string etag = (string) null;
          bool isSealed = false;
          List<SqlBlockInfo> source = (List<SqlBlockInfo>) null;
          using (BlobStoreComponent blobStoreComponent = this.CreateBlobStoreComponent(requestContext))
            source = blobStoreComponent.GetBlockList(blobId, out etag, out isSealed);
          if (etag != etagToMatch)
          {
            tracer.TraceInfo("ETags don't match for block list of blob \"" + blobId.ValueString + "\". Aborting PutBlockListAsync.");
            return new EtagValue<bool>(false, etag);
          }
          IEnumerable<Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash> blockHashes = blockIds.Select<string, Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash>((Func<string, Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash>) (blockId => new Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash(blockId)));
          Dictionary<Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash, int> dictionary = source.ToDictionary<SqlBlockInfo, Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash, int>((Func<SqlBlockInfo, Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash>) (blockInfo => blockInfo.BlockHash), (Func<SqlBlockInfo, int>) (blockInfo => blockInfo.BlockFileId));
          foreach (Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash key in blockHashes)
          {
            int num = 0;
            if (!dictionary.TryGetValue(key, out num))
            {
              InvalidOperationException operationException = new InvalidOperationException(string.Format(Resources.BlockUploadingIncomplete((object) blobId.ValueString)));
              tracer.TraceException((Exception) operationException);
              throw operationException;
            }
          }
          using (BlobStoreComponent blobStoreComponent = this.CreateBlobStoreComponent(requestContext))
            return new EtagValue<bool>(blobStoreComponent.PutBlockList(blobId, blockHashes, ref etagToMatch), etagToMatch);
        }
      }));
    }

    public Task<EtagValue<bool>> DeleteBlobAsync(
      VssRequestPump.Processor processor,
      BlobIdentifier blobId,
      string etagToDelete)
    {
      return processor.ExecuteWorkAsync<EtagValue<bool>>((Func<IVssRequestContext, EtagValue<bool>>) (requestContext =>
      {
        using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, this.traceData, 5707040, nameof (DeleteBlobAsync)))
        {
          bool matched = false;
          string etag = etagToDelete;
          List<SqlBlockInfo> source = (List<SqlBlockInfo>) null;
          using (BlobStoreComponent blobStoreComponent = this.CreateBlobStoreComponent(requestContext))
            source = blobStoreComponent.DeleteBlob(blobId, ref etag, out matched);
          if (!matched)
          {
            tracer.TraceInfo("ETags don't match for blob \"" + blobId.ValueString + "\". Aborting DeleteBlobAsync.");
            return new EtagValue<bool>(false, etag);
          }
          requestContext.GetService<ITeamFoundationFileService>().DeleteFiles(requestContext, source.Select<SqlBlockInfo, int>((Func<SqlBlockInfo, int>) (block => block.BlockFileId)));
          return new EtagValue<bool>(true, etag);
        }
      }));
    }

    public Task<PreauthenticatedUri> GetContainerUri(
      VssRequestPump.Processor processor,
      string policyId)
    {
      throw new NotSupportedException();
    }

    public Task<DisposableEtagValue<Stream>> GetBlobAsync(
      VssRequestPump.Processor processor,
      BlobIdentifier blobId)
    {
      return processor.ExecuteWorkAsync<DisposableEtagValue<Stream>>((Func<IVssRequestContext, DisposableEtagValue<Stream>>) (requestContext =>
      {
        Tuple<Stream, string> tuple = this.RetrieveStreamWithEtag(requestContext, blobId, false);
        return tuple == null ? new DisposableEtagValue<Stream>((Stream) null, (string) null) : new DisposableEtagValue<Stream>(tuple.Item1, tuple.Item2);
      }));
    }

    public Task<string> GetBlobEtagAsync(VssRequestPump.Processor processor, BlobIdentifier blobId) => processor.ExecuteWorkAsync<string>((Func<IVssRequestContext, string>) (requestContext =>
    {
      using (BlobStoreComponent blobStoreComponent = this.CreateBlobStoreComponent(requestContext))
      {
        SqlBlobInfo blobInfo = this.GetBlobInfo(blobStoreComponent, blobId);
        return !blobInfo.IsSealed ? (string) null : blobInfo.ETag;
      }
    }));

    public Task<long?> GetBlobLengthAsync(VssRequestPump.Processor processor, BlobIdentifier blobId) => Task.FromResult<long?>(new long?());

    public IConcurrentIterator<BlobIdentifier> GetBlobIdentifiersConcurrentIterator(
      VssRequestPump.Processor processor)
    {
      throw new NotImplementedException();
    }

    public Task<EtagValue<IList<BlockInfo>>> GetBlockListAsync(
      VssRequestPump.Processor processor,
      BlobIdentifier blobId)
    {
      return processor.ExecuteWorkAsync<EtagValue<IList<BlockInfo>>>((Func<IVssRequestContext, EtagValue<IList<BlockInfo>>>) (requestContext =>
      {
        using (BlobStoreComponent blobStoreComponent = this.CreateBlobStoreComponent(requestContext))
        {
          string etag = (string) null;
          bool isSealed = false;
          List<SqlBlockInfo> blockList = blobStoreComponent.GetBlockList(blobId, out etag, out isSealed);
          IList<BlockInfo> blockInfoList = (IList<BlockInfo>) new List<BlockInfo>();
          foreach (SqlBlockInfo sqlBlockInfo in blockList)
          {
            BlockInfo blockInfo = new BlockInfo(sqlBlockInfo.BlockHash.HashString)
            {
              Length = sqlBlockInfo.BlockFileLength,
              Committed = sqlBlockInfo.IsCommitted
            };
            blockInfoList.Add(blockInfo);
          }
          return new EtagValue<IList<BlockInfo>>(blockInfoList, etag);
        }
      }));
    }

    public Task<PreauthenticatedUri> GetDownloadUriAsync(
      VssRequestPump.Processor processor,
      BlobIdWithHeaders blobId,
      SASUriExpiry expiry,
      string policyId,
      (string, Guid)[] sasTracing)
    {
      throw new NotImplementedException();
    }

    public Task<IDictionary<BlobIdentifier, PreauthenticatedUri>> GetDownloadUrisAsync(
      VssRequestPump.Processor processor,
      IEnumerable<BlobIdentifier> identifiers,
      SASUriExpiry expiry,
      string policyId,
      (string, Guid)[] sasTracing)
    {
      throw new NotImplementedException();
    }

    public IConcurrentIterator<IEnumerable<BasicBlobMetadata>> GetBasicBlobMetadataConcurrentIterator(
      VssRequestPump.Processor processor,
      string prefix)
    {
      throw new NotImplementedException();
    }

    internal bool Cleanup(IVssRequestContext requestContext, int batchCount)
    {
      using (BlobStoreComponent blobStoreComponent = this.CreateBlobStoreComponent(requestContext))
      {
        if (!(blobStoreComponent is BlobStoreComponent2 blobStoreComponent2))
          return true;
        DateTime cutoffDateTime;
        List<SqlBlobBlockInfo> unusedBlocks = blobStoreComponent2.GetUnusedBlocks(out cutoffDateTime, batchCount);
        if (unusedBlocks.Count == 0)
          return false;
        requestContext.GetService<ITeamFoundationFileService>().DeleteFiles(requestContext, unusedBlocks.Select<SqlBlobBlockInfo, int>((Func<SqlBlobBlockInfo, int>) (bbi => bbi.BlockFileId)));
        blobStoreComponent2.DeleteUnusedBlocks((IEnumerable<SqlBlobBlockInfo>) unusedBlocks, cutoffDateTime);
        return unusedBlocks.Count >= batchCount;
      }
    }

    public void Dispose()
    {
    }

    private BlobStoreComponent CreateBlobStoreComponent(IVssRequestContext requestContext)
    {
      BlobStoreComponent blobStoreComponent = this.m_blobStoreComponentFactory(requestContext);
      blobStoreComponent.ContainerId = this.m_containerId;
      return blobStoreComponent;
    }

    private static BlobStoreComponent DefaultFactory(IVssRequestContext requestContext) => requestContext.CreateComponent<BlobStoreComponent>();

    private SqlBlobInfo GetBlobInfo(BlobStoreComponent bsc, BlobIdentifier blobId)
    {
      string etag = (string) null;
      bool isSealed = false;
      bsc.GetBlobBlocks(blobId, out etag, out isSealed);
      return new SqlBlobInfo()
      {
        BlobId = blobId,
        ETag = etag,
        IsSealed = isSealed
      };
    }

    private Task PutBlobBlockInternalAsync(
      VssRequestPump.Processor processor,
      BlobIdentifier blobId,
      ArraySegment<byte> blobBlock,
      string blockName,
      bool allowEmptyBlock)
    {
      if (blobBlock.Count == 0 && !allowEmptyBlock)
        throw new ArgumentException("Block of zero length is not supported.");
      return (Task) processor.ExecuteWorkAsync<int>((Func<IVssRequestContext, int>) (requestContext =>
      {
        using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, this.traceData, 5707010, nameof (PutBlobBlockInternalAsync)))
        {
          ITeamFoundationFileService service = requestContext.GetService<ITeamFoundationFileService>();
          string etag = (string) null;
          bool isSealed = false;
          List<SqlBlockInfo> source = (List<SqlBlockInfo>) null;
          using (BlobStoreComponent blobStoreComponent = this.CreateBlobStoreComponent(requestContext))
            source = blobStoreComponent.GetBlockList(blobId, out etag, out isSealed);
          Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash hash = new Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash(blockName);
          SqlBlockInfo sqlBlockInfo = source.FirstOrDefault<SqlBlockInfo>((Func<SqlBlockInfo, bool>) (blockInfo => blockInfo.BlockHash == hash));
          int fileId1 = 0;
          int fileId2 = 0;
          if (sqlBlockInfo != null)
          {
            fileId1 = sqlBlockInfo.BlockFileId;
            if (service.GetFileStatistics(requestContext, (long) fileId1) == null)
              fileId1 = 0;
            Stream stream = SqlBlobProvider.RetrieveFileStream(requestContext, service, fileId1);
            if (stream != null)
            {
              int length = (int) stream.Length;
              using (MemoryStream destination = new MemoryStream(length))
              {
                stream.CopyTo((Stream) destination);
                if (Microsoft.VisualStudio.Services.BlobStore.Common.VsoHash.HashBlock(destination.ToArray(), length) != hash)
                {
                  fileId2 = fileId1;
                  fileId1 = 0;
                }
              }
            }
            else
              fileId1 = 0;
          }
          if (fileId1 == 0)
          {
            long count = (long) blobBlock.Count;
            fileId1 = this.TryUploadFile(requestContext, service, blobBlock, count);
            using (BlobStoreComponent blobStoreComponent = this.CreateBlobStoreComponent(requestContext))
              blobStoreComponent.AddBlock(blobId, hash, fileId1, count);
          }
          if (fileId2 != 0)
            service.DeleteFile(requestContext, (long) fileId2);
          return fileId1;
        }
      }));
    }

    private Tuple<Stream, string> RetrieveStreamWithEtag(
      IVssRequestContext requestContext,
      BlobIdentifier blobId,
      bool throwIfNotFound)
    {
      string etag = (string) null;
      bool isSealed = false;
      List<SqlBlockInfo> sqlBlockInfoList = (List<SqlBlockInfo>) null;
      using (BlobStoreComponent blobStoreComponent = this.CreateBlobStoreComponent(requestContext))
        sqlBlockInfoList = blobStoreComponent.GetBlobBlocks(blobId, out etag, out isSealed);
      if (!isSealed)
      {
        if (throwIfNotFound)
          throw BlobNotFoundException.Create(blobId.ValueString);
        return (Tuple<Stream, string>) null;
      }
      long totalLength = 0;
      List<StreamFactory> streamFactories = new List<StreamFactory>();
      ISqlBlobRetriever sqlBlobRetriever = this.GetSqlBlobRetriever(requestContext);
      foreach (SqlBlockInfo sqlBlockInfo in sqlBlockInfoList)
      {
        Tuple<FileStatistics, StreamFactory> tuple = this.RetrieveStatAndStreamFactory(requestContext, sqlBlobRetriever, sqlBlockInfo.BlockFileId);
        totalLength += tuple.Item1.FileLength;
        streamFactories.Add(tuple.Item2);
      }
      return new Tuple<Stream, string>((Stream) new ConcatenatedOnDemandStream((IList<StreamFactory>) streamFactories, sqlBlobRetriever.ReadOnce, totalLength), etag);
    }

    private Tuple<FileStatistics, StreamFactory> RetrieveStatAndStreamFactory(
      IVssRequestContext requestContext,
      ISqlBlobRetriever retriever,
      int fileId)
    {
      return new Tuple<FileStatistics, StreamFactory>(retriever.GetFileStatistics(requestContext, fileId), retriever.RetrieveFileStreamFactory(requestContext, fileId));
    }

    internal static Stream RetrieveFileStream(
      IVssRequestContext requestContext,
      ITeamFoundationFileService fileService,
      int fileId)
    {
      return fileService.RetrieveFile(requestContext, (long) fileId, false, out byte[] _, out long _, out CompressionType _);
    }

    private int TryUploadFile(
      IVssRequestContext requestContext,
      ITeamFoundationFileService fileService,
      ArraySegment<byte> data,
      long length)
    {
      int fileId = 0;
      using (MemoryStream content = data.AsMemoryStream())
        fileService.UploadFile(requestContext, ref fileId, (Stream) content, (byte[]) null, length, length, 0L, CompressionType.None, SqlBlobProvider.OwnerId, Guid.Empty, (string) null);
      return fileId;
    }

    private ISqlBlobRetriever GetSqlBlobRetriever(IVssRequestContext reqContext) => this.m_fileComponentFactory == null ? (ISqlBlobRetriever) new FileServiceSqlBlobRetriever(reqContext) : (ISqlBlobRetriever) new FileComponentSqlBlobRetriever(this.m_fileComponentFactory);
  }
}
