// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.ItemStoreBlobService
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.ItemStore.Common;
using Microsoft.VisualStudio.Services.ItemStore.Server.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class ItemStoreBlobService : IBlobService, IBlobServiceContainerDeleter
  {
    public const string CodeOnlyDeploymentsBlobs = "CodeOnlyDeploymentsBlobs";
    private readonly IVssRequestContext requestContext;
    private readonly IFactory<IItemStore> itemStoreFactory;
    private readonly ITracerService tracerService;
    private readonly Locator containerLocator;
    private readonly IItemStoreBlobEncodingStrategy encodingStrategy;

    public ItemStoreBlobService(
      IVssRequestContext requestContext,
      ContainerAddress containerAddress,
      IFactory<IItemStore> itemStoreFactory,
      ITracerService tracerService,
      IItemStoreBlobEncodingStrategy encodingStrategy)
    {
      this.requestContext = requestContext;
      this.itemStoreFactory = itemStoreFactory;
      this.tracerService = tracerService;
      this.encodingStrategy = encodingStrategy;
      this.containerLocator = new Locator(new Locator(new string[1]
      {
        nameof (CodeOnlyDeploymentsBlobs)
      }), containerAddress.Path);
    }

    public async Task<string> GetBlobAsync(Locator blobPath, Stream stream)
    {
      ItemStoreBlobService sendInTheThisObject = this;
      using (ITracerBlock tracer = sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (GetBlobAsync)))
      {
        ItemStoreBlobService.WrappedItem itemAsync = await sendInTheThisObject.itemStoreFactory.Get().GetItemAsync<ItemStoreBlobService.WrappedItem>(sendInTheThisObject.requestContext, sendInTheThisObject.containerLocator, blobPath);
        if (itemAsync == null)
          return (string) null;
        string input = itemAsync["content"];
        if (input == null)
          throw new Exception(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_NoContentIdInWrappedItem());
        string str = itemAsync["Content-Encoding"];
        bool shouldInflate = str == "deflate";
        if (shouldInflate && sendInTheThisObject.encodingStrategy.CanOnlyEncodeUtf8Text)
          throw new Exception("Cannot successfully decode deflated content with an encodingStrategy of " + sendInTheThisObject.encodingStrategy.GetType().Name);
        using (InflateIfNecessaryWritableWrapperStream writableWrapperStream = new InflateIfNecessaryWritableWrapperStream(stream, (Func<bool>) (() => shouldInflate)))
        {
          sendInTheThisObject.encodingStrategy.DecodeTo(input, (Stream) writableWrapperStream);
          tracer.TraceInfo(string.Format("Got blob.\r\nContent-Encoding: {0}\r\nDetected zlib header: {1}\r\nShould inflate: {2}\r\nShould inflate (from stream): {3}", (object) str, (object) writableWrapperStream.DetectedZLibHeader, (object) shouldInflate, (object) writableWrapperStream.ShouldInflateResult));
        }
        return itemAsync.StorageETag;
      }
    }

    public Task<string> PutBlobAsync(Locator blobPath, ArraySegment<byte> bytes, string etag) => this.PutBlobAsync(blobPath, bytes, etag, false);

    public async Task<string> PutBlobAsync(
      Locator blobPath,
      ArraySegment<byte> bytes,
      string etag,
      bool deflate)
    {
      ItemStoreBlobService sendInTheThisObject = this;
      if (deflate && sendInTheThisObject.encodingStrategy.CanOnlyEncodeUtf8Text)
        throw new Exception("Cannot successfully encode deflated content with an encodingStrategy of " + sendInTheThisObject.encodingStrategy.GetType().Name);
      using (ITracerBlock tracer = sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (PutBlobAsync)))
      {
        IItemStore itemStore = sendInTheThisObject.itemStoreFactory.Get();
        ContainerItem addContainerAsync = await itemStore.GetOrAddContainerAsync(sendInTheThisObject.requestContext, new ContainerItem()
        {
          Name = sendInTheThisObject.containerLocator
        });
        ItemStoreBlobService.WrappedItem wrappedItem = new ItemStoreBlobService.WrappedItem();
        wrappedItem.StorageETag = etag;
        ItemStoreBlobService.WrappedItem item = wrappedItem;
        if (deflate)
        {
          item["Content-Encoding"] = nameof (deflate);
          bytes = CompressionHelper.DeflateByteArray(bytes, true).AsArraySegment();
        }
        using (MemoryStream input = bytes.AsMemoryStream())
          item["content"] = sendInTheThisObject.encodingStrategy.Encode((Stream) input);
        if (!await itemStore.CompareSwapItemAsync(sendInTheThisObject.requestContext, sendInTheThisObject.containerLocator, blobPath, (StoredItem) item))
          return (string) null;
        tracer.TraceInfo(string.Format("successful itemstore blob put against path: {0} container: {1} resulting etag: {2}", (object) blobPath, (object) sendInTheThisObject.containerLocator, (object) item.StorageETag));
        return item.StorageETag;
      }
    }

    public Task<IEnumerable<EtagValue<Locator>>> Under(Locator blobDirectoryPath) => throw new NotImplementedException();

    public Task<bool> DeleteBlobAsync(Locator blobPath) => throw new NotImplementedException();

    public async Task<bool> DeleteContainer()
    {
      IItemStore itemStore = this.itemStoreFactory.Get();
      ContainerItem containerAsync = await itemStore.GetContainerAsync(this.requestContext, this.containerLocator);
      return containerAsync != null && await itemStore.DeleteContainerAsync(this.requestContext, containerAsync);
    }

    public class WrappedItem : StoredItem
    {
      public WrappedItem()
        : base(nameof (WrappedItem))
      {
      }

      public WrappedItem(IItemData data)
        : base(data, nameof (WrappedItem))
      {
      }
    }
  }
}
