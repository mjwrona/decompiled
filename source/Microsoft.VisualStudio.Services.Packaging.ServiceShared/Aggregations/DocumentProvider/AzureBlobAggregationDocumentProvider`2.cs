// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.DocumentProvider.AzureBlobAggregationDocumentProvider`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.DocumentProvider
{
  public class AzureBlobAggregationDocumentProvider<TDoc, TSpecifier> : 
    IAggregationDocumentProvider<TDoc, TSpecifier>
  {
    private readonly IAggregationDocumentProcessor<TDoc> serializer;
    private readonly IPackagingTraces packagingTraces;
    private readonly ITracerService tracer;
    private readonly IFactory<IBlobService> blobServiceFactory;
    private readonly IOrgLevelPackagingSetting<bool> shouldDeflateSetting;
    private readonly string blobSizePropertyName;
    private readonly IAggregationDocumentLocatorProvider<TSpecifier> locatorProvider;

    public AzureBlobAggregationDocumentProvider(
      IPackagingTraces packagingTraces,
      ITracerService tracer,
      IAggregationDocumentProcessor<TDoc> serializer,
      IFactory<IBlobService> blobServiceFactory,
      IOrgLevelPackagingSetting<bool> shouldDeflateSetting,
      string blobSizePropertyName,
      IAggregationDocumentLocatorProvider<TSpecifier> locatorProvider)
    {
      this.packagingTraces = packagingTraces;
      this.tracer = tracer;
      this.serializer = serializer;
      this.blobServiceFactory = blobServiceFactory;
      this.shouldDeflateSetting = shouldDeflateSetting;
      this.blobSizePropertyName = blobSizePropertyName;
      this.locatorProvider = locatorProvider;
    }

    public async Task<EtagValue<TDoc>> GetDocumentAsync(
      IFeedRequest feedRequest,
      TSpecifier specifier)
    {
      AzureBlobAggregationDocumentProvider<TDoc, TSpecifier> sendInTheThisObject = this;
      EtagValue<TDoc> documentAsync;
      using (sendInTheThisObject.tracer.Enter((object) sendInTheThisObject, nameof (GetDocumentAsync)))
      {
        byte[] numArray;
        string etag1;
        // ISSUE: explicit non-virtual call
        (await __nonvirtual (sendInTheThisObject.GetDocumentBytesAsync(feedRequest, specifier))).Deconstruct<byte[]>(out numArray, out etag1);
        byte[] buffer = numArray;
        string etag2 = etag1;
        documentAsync = buffer.Length != 0 ? new EtagValue<TDoc>(sendInTheThisObject.serializer.Deserialize(buffer), etag2) : new EtagValue<TDoc>(sendInTheThisObject.serializer.GetEmptyDocument(), etag2);
      }
      return documentAsync;
    }

    public async Task<EtagValue<byte[]>> GetDocumentBytesAsync(
      IFeedRequest feedRequest,
      TSpecifier specifier)
    {
      AzureBlobAggregationDocumentProvider<TDoc, TSpecifier> sendInTheThisObject = this;
      using (ITracerBlock traceBlock = sendInTheThisObject.tracer.Enter((object) sendInTheThisObject, nameof (GetDocumentBytesAsync)))
      {
        Locator locator = sendInTheThisObject.locatorProvider.GetLocator(feedRequest, specifier);
        string blobAsync;
        byte[] numArray;
        using (MemoryStream stream = new MemoryStream())
        {
          blobAsync = await sendInTheThisObject.blobServiceFactory.Get().GetBlobAsync(locator, (Stream) stream);
          if (blobAsync == null || stream.Length == 0L)
            return new EtagValue<byte[]>(Array.Empty<byte>(), blobAsync);
          numArray = stream.ToArray();
        }
        sendInTheThisObject.packagingTraces.AddProperty(sendInTheThisObject.blobSizePropertyName, (object) numArray.Length);
        traceBlock.TraceInfo(string.Format("Version list blob length {0}. First bytes of blob: {1}", (object) numArray.Length, (object) string.Join(" ", ((IEnumerable<byte>) numArray).Take<byte>(10).Select<byte, string>((Func<byte, string>) (x => x.ToString("X2"))))));
        if (numArray[0] == (byte) 120)
        {
          numArray = CompressionHelper.InflateByteArray(numArray, true);
          traceBlock.TraceInfoAlways("Had to decompress content expected to be already decompressed by GetBlobAsync. First bytes after decompression: " + string.Join(" ", ((IEnumerable<byte>) numArray).Take<byte>(10).Select<byte, string>((Func<byte, string>) (x => x.ToString("X2")))));
        }
        return new EtagValue<byte[]>(numArray, blobAsync);
      }
    }

    public async Task<string> PutDocumentAsync(
      IFeedRequest feedRequest,
      TSpecifier specifier,
      TDoc doc,
      string etag)
    {
      AzureBlobAggregationDocumentProvider<TDoc, TSpecifier> sendInTheThisObject = this;
      string str1;
      using (sendInTheThisObject.tracer.Enter((object) sendInTheThisObject, nameof (PutDocumentAsync)))
      {
        byte[] bytes = sendInTheThisObject.serializer.Serialize(doc);
        Locator locator = sendInTheThisObject.locatorProvider.GetLocator(feedRequest, specifier);
        string str2 = await sendInTheThisObject.blobServiceFactory.Get().PutBlobAsync(locator, bytes.AsArraySegment(), etag, sendInTheThisObject.shouldDeflateSetting.Get());
        sendInTheThisObject.serializer.NotifySaved(doc);
        str1 = str2;
      }
      return str1;
    }

    public async Task RemoveDocumentAsync(IFeedRequest feedRequest, TSpecifier specifier)
    {
      AzureBlobAggregationDocumentProvider<TDoc, TSpecifier> sendInTheThisObject = this;
      using (sendInTheThisObject.tracer.Enter((object) sendInTheThisObject, nameof (RemoveDocumentAsync)))
      {
        Locator locator = sendInTheThisObject.locatorProvider.GetLocator(feedRequest, specifier);
        int num = await sendInTheThisObject.blobServiceFactory.Get().DeleteBlobAsync(locator) ? 1 : 0;
      }
    }
  }
}
