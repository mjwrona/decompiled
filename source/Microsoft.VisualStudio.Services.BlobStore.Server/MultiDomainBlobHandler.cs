// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.MultiDomainBlobHandler
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.BlobStore.WebApi;
using Microsoft.VisualStudio.Services.BlobStore.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public class MultiDomainBlobHandler
  {
    private IVssRequestContext requestContext;
    private HttpRequestMessage requestMessage;
    private IDomainId domainId;

    private static HttpStatusCode ResponseStatusForAddingOperation => HttpStatusCode.NoContent;

    public MultiDomainBlobHandler(
      IVssRequestContext requestContext,
      HttpRequestMessage requestMessage,
      IDomainId domainId)
    {
      this.requestContext = requestContext.AllowMultiDomainOperations(domainId) ? requestContext : throw new FeatureDisabledException("Multi-Domain");
      this.requestMessage = requestMessage;
      this.domainId = domainId;
    }

    public async Task<HttpResponseMessage> GetBlobByIdentifierAsync(string blobId)
    {
      IBlobStore service = this.requestContext.GetService<IBlobStore>();
      BlobIdentifier parsedBlobId = BlobIdentifier.Deserialize(blobId);
      IVssRequestContext requestContext = this.requestContext;
      IDomainId domainId = this.domainId;
      BlobIdentifier blobId1 = parsedBlobId;
      Stream content = await service.GetBlobAsync(requestContext, domainId, blobId1).ConfigureAwait(true);
      this.requestContext.RequestTimer.SetTimeToFirstPageEnd();
      StreamContent streamContent = content != null ? new StreamContent(content) : throw new BlobNotFoundException("Blob does not exist: " + parsedBlobId?.ToString());
      streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      HttpResponseMessage byIdentifierAsync = new HttpResponseMessage()
      {
        Content = (HttpContent) streamContent,
        StatusCode = HttpStatusCode.OK
      };
      parsedBlobId = (BlobIdentifier) null;
      return byIdentifierAsync;
    }

    public async Task<HttpResponseMessage> AddBlockBlobBlockAsync(string blobId, string blockHash)
    {
      BlobIdentifier givenIdentifier = BlobIdentifier.Deserialize(blobId);
      Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash parsedBlockHash = new Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash(blockHash);
      IBlockCacheService cache = this.requestContext.GetService<IBlockCacheService>();
      Tuple<IDomainId, byte[]> cacheKey = new Tuple<IDomainId, byte[]>(this.domainId, ((IEnumerable<byte>) parsedBlockHash.HashBytes).Concat<byte>((IEnumerable<byte>) givenIdentifier.Bytes).ToArray<byte>());
      switch (cache.GetBlockStatus(this.requestContext, cacheKey))
      {
        case BlockUploadStatus.Uploading:
          HttpResponseMessage errorResponse = this.requestMessage.CreateErrorResponse(HttpStatusCode.ServiceUnavailable, "service temporarily unavailable. Please retry");
          errorResponse.Headers.RetryAfter = new RetryConditionHeaderValue(TimeSpan.FromSeconds(5.0));
          return errorResponse;
        case BlockUploadStatus.Uploaded:
          HttpResponseMessage httpResponseMessage1 = new HttpResponseMessage(MultiDomainBlobHandler.ResponseStatusForAddingOperation);
          httpResponseMessage1.Headers.Add("X-Content-Uploaded", "no");
          return httpResponseMessage1;
        default:
          int len = this.GetLength();
          try
          {
            cache.SetBlockStatus(this.requestContext, cacheKey, BlockUploadStatus.Uploading);
            using (ByteArray pooledArray = new ByteArray(len, BlobStoreBufferPoolsProvider.Instance))
            {
              byte[] buffer = pooledArray.Bytes;
              int bufferAsync = await this.ReadToBufferAsync(buffer, len);
              if (bufferAsync != len)
              {
                BlobServiceException serviceException = new BlobServiceException(Resources.ContentLengthMismatchError((object) bufferAsync, (object) len));
                if (this.requestContext.IsFeatureEnabled("Blobstore.Features.ThrowOnContentLengthMismatch"))
                  throw serviceException;
                this.requestContext.TraceException(5704032, Microsoft.VisualStudio.Services.BlobStore.Server.Common.Telemetry.TracePoints.MultiDomainBlobHandler.TraceData.Area, Microsoft.VisualStudio.Services.BlobStore.Server.Common.Telemetry.TracePoints.MultiDomainBlobHandler.TraceData.Layer, (Exception) serviceException);
              }
              await this.requestContext.GetService<IBlobStore>().PutBlobBlockAsync(this.requestContext, this.domainId, givenIdentifier, buffer, len, parsedBlockHash).ConfigureAwait(true);
              buffer = (byte[]) null;
            }
            cache.SetBlockStatus(this.requestContext, cacheKey, BlockUploadStatus.Uploaded);
            HttpResponseMessage httpResponseMessage2 = new HttpResponseMessage(MultiDomainBlobHandler.ResponseStatusForAddingOperation);
            httpResponseMessage2.Headers.Add("X-Content-Uploaded", "yes");
            return httpResponseMessage2;
          }
          catch (Exception ex1)
          {
            try
            {
              cache.SetBlockStatus(this.requestContext, cacheKey, BlockUploadStatus.Unknown);
            }
            catch (Exception ex2)
            {
              this.requestContext.TraceException(5704033, Microsoft.VisualStudio.Services.BlobStore.Server.Common.Telemetry.TracePoints.MultiDomainBlobHandler.TraceData.Area, Microsoft.VisualStudio.Services.BlobStore.Server.Common.Telemetry.TracePoints.MultiDomainBlobHandler.TraceData.Layer, ex2);
            }
            throw;
          }
      }
    }

    public async Task<HttpResponseMessage> AddSingleBlockBlobAsync(
      BlobIdentifier blobId,
      BlobReference reference)
    {
      int len = this.GetLength();
      HttpResponseMessage httpResponseMessage;
      using (ByteArray pooledArray = new ByteArray(len, BlobStoreBufferPoolsProvider.Instance))
      {
        byte[] buffer = pooledArray.Bytes;
        int bufferAsync = await this.ReadToBufferAsync(buffer, len);
        await this.requestContext.GetService<IBlobStore>().PutSingleBlockBlobAndReferenceAsync(this.requestContext, this.domainId, blobId, buffer, len, reference).ConfigureAwait(true);
        httpResponseMessage = this.AddLocationHeader(new HttpResponseMessage(MultiDomainBlobHandler.ResponseStatusForAddingOperation));
      }
      return httpResponseMessage;
    }

    private async Task<int> ReadToBufferAsync(byte[] buffer, int len)
    {
      IDisposable exclusionBlock;
      Stream stream;
      int bufferAsync;
      using (this.requestContext.Enter(Microsoft.VisualStudio.Services.BlobStore.Server.Common.Telemetry.TracePoints.MultiDomainBlobHandler.ReadToBufferAsyncCall, nameof (ReadToBufferAsync)))
      {
        exclusionBlock = this.requestContext.RequestTimer.CreateTimeToFirstPageExclusionBlock();
        try
        {
          stream = await this.requestMessage.Content.ReadAsStreamAsync();
          try
          {
            int offset = 0;
            while (true)
            {
              int num;
              if ((num = await stream.ReadAsync(buffer, offset, len - offset)) != 0)
                offset += num;
              else
                break;
            }
            bufferAsync = offset;
          }
          finally
          {
            stream?.Dispose();
          }
        }
        finally
        {
          exclusionBlock?.Dispose();
        }
      }
      exclusionBlock = (IDisposable) null;
      stream = (Stream) null;
      return bufferAsync;
    }

    private int GetLength()
    {
      long num = (this.requestMessage.Content.Headers.ContentLength ?? throw new ContentLengthMissingException(Resources.ContentLengthUnavailable())).Value;
      return num <= 2097152L ? (int) num : throw new ContentLengthTooLargeException(Resources.ContentLengthTooLarge((object) 2097152));
    }

    private HttpResponseMessage AddLocationHeader(HttpResponseMessage msg)
    {
      string str = this.requestMessage.RequestUri.ToString();
      int length = str.IndexOf('?');
      if (length != -1)
        str = str.Substring(0, length);
      msg.Headers.Add("Location", str);
      return msg;
    }
  }
}
