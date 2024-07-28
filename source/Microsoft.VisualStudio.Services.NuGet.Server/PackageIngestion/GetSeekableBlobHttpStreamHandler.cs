// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.PackageIngestion.GetSeekableBlobHttpStreamHandler
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.HttpStreams;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.NuGet.Server.PackageIngestion
{
  public class GetSeekableBlobHttpStreamHandler : 
    IAsyncHandler<BlobIdentifier, Stream>,
    IHaveInputType<BlobIdentifier>,
    IHaveOutputType<Stream>
  {
    private readonly IContentBlobStore contentBlobStore;
    private readonly IFactory<DateTimeOffset> sasTokenExpiryFactory;

    public GetSeekableBlobHttpStreamHandler(
      IContentBlobStore contentBlobStore,
      IFactory<DateTimeOffset> sasTokenExpiryFactory)
    {
      this.contentBlobStore = contentBlobStore;
      this.sasTokenExpiryFactory = sasTokenExpiryFactory;
    }

    public async Task<Stream> Handle(BlobIdentifier blobId)
    {
      blobId = BlobIdentifier.Deserialize(blobId.ValueString);
      DateTimeOffset expiryTime = this.sasTokenExpiryFactory.Get();
      IDictionary<BlobIdentifier, PreauthenticatedUri> downloadUrisAsync = await this.TryGetDownloadUrisAsync(blobId, expiryTime);
      PreauthenticatedUri preauthenticatedUri;
      if (downloadUrisAsync == null || !downloadUrisAsync.TryGetValue(blobId, out preauthenticatedUri))
        return (Stream) await this.GetFileBasedStreamAsync(blobId);
      VssHttpRetryOptions options = VssHttpRetryOptions.Default;
      VssHttpRetryMessageHandler retryMessageHandler = new VssHttpRetryMessageHandler(options);
      retryMessageHandler.InnerHandler = (HttpMessageHandler) new HttpClientHandler();
      retryMessageHandler.ClientName = "Azure Blob Storage";
      VssHttpRetryMessageHandler messageHandler = retryMessageHandler;
      HttpSeekableStream httpSeekableStream = new HttpSeekableStream(preauthenticatedUri.NotNullUri, messageHandler: (HttpMessageHandler) messageHandler);
      httpSeekableStream.ReadTimeout = 2 * options.MaxRetries * (int) options.MaxBackoff.TotalMilliseconds;
      return (Stream) httpSeekableStream;
    }

    private async Task<IDictionary<BlobIdentifier, PreauthenticatedUri>> TryGetDownloadUrisAsync(
      BlobIdentifier blobId,
      DateTimeOffset expiryTime)
    {
      try
      {
        return await this.contentBlobStore.GetDownloadUrisAsync((IEnumerable<BlobIdentifier>) new BlobIdentifier[1]
        {
          blobId
        }, EdgeCache.NotAllowed, new DateTimeOffset?(expiryTime));
      }
      catch (NotImplementedException ex)
      {
        return (IDictionary<BlobIdentifier, PreauthenticatedUri>) null;
      }
    }

    private async Task<FileStream> GetFileBasedStreamAsync(BlobIdentifier blobId)
    {
      FileStream basedStreamAsync;
      using (Stream blobStream = await this.contentBlobStore.GetBlobAsync(blobId))
      {
        FileStream fileStream = File.Create(Path.GetTempFileName(), 1000, FileOptions.DeleteOnClose);
        await blobStream.CopyToAsync((Stream) fileStream);
        basedStreamAsync = fileStream;
      }
      return basedStreamAsync;
    }
  }
}
