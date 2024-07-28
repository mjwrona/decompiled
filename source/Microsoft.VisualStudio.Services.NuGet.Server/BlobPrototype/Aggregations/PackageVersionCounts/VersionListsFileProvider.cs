// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts.VersionListsFileProvider
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Google.Protobuf;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts
{
  public class VersionListsFileProvider : IVersionListsFileProvider
  {
    private readonly IConverter<IAggregation, Locator> aggVersionToLocatorConverter;
    private readonly IExecutionEnvironment executionEnvironment;
    private readonly IPackagingTraces packagingTraces;
    private readonly ITracerService tracer;
    private readonly IVersionCountsImplementationMetricsRecorder versionCountsMetricsRecorder;
    private readonly IFactory<ContainerAddress, IBlobService> blobServiceFactory;
    private readonly IAggregation aggregation;
    private readonly IFeatureFlagService featureFlagService;

    public VersionListsFileProvider(
      IAggregation aggregation,
      IFactory<ContainerAddress, IBlobService> blobServiceFactory,
      IConverter<IAggregation, Locator> aggVersionToLocatorConverter,
      IExecutionEnvironment executionEnvironment,
      IPackagingTraces packagingTraces,
      ITracerService tracer,
      IVersionCountsImplementationMetricsRecorder versionCountsMetricsRecorder,
      IFeatureFlagService featureFlagService)
    {
      this.blobServiceFactory = blobServiceFactory;
      this.aggVersionToLocatorConverter = aggVersionToLocatorConverter;
      this.executionEnvironment = executionEnvironment;
      this.packagingTraces = packagingTraces;
      this.tracer = tracer;
      this.versionCountsMetricsRecorder = versionCountsMetricsRecorder;
      this.featureFlagService = featureFlagService;
      this.aggregation = aggregation;
    }

    public async Task<EtagValue<IMutableVersionListsFile>> GetVersionListDocument(
      IFeedRequest feedRequest)
    {
      VersionListsFileProvider sendInTheThisObject = this;
      using (sendInTheThisObject.tracer.Enter((object) sendInTheThisObject, nameof (GetVersionListDocument)))
      {
        byte[] numArray;
        string etag1;
        // ISSUE: explicit non-virtual call
        (await __nonvirtual (sendInTheThisObject.GetVersionListDocumentBytes(feedRequest))).Deconstruct<byte[]>(out numArray, out etag1);
        byte[] data = numArray;
        string etag2 = etag1;
        if (data.Length == 0)
          return new EtagValue<IMutableVersionListsFile>((IMutableVersionListsFile) new VersionListsFileWrapper(VersionListsFile.CreateNewUnpacked()), etag2);
        VersionListsFile from = VersionListsFile.Parser.ParseFrom(data);
        VersionListsFileUnpacker.Unpack(from);
        sendInTheThisObject.versionCountsMetricsRecorder.Record((IVersionCountsImplementationMetrics) from);
        return new EtagValue<IMutableVersionListsFile>((IMutableVersionListsFile) new VersionListsFileWrapper(from), etag2);
      }
    }

    public async Task<EtagValue<byte[]>> GetVersionListDocumentBytes(IFeedRequest feedRequest)
    {
      VersionListsFileProvider sendInTheThisObject = this;
      using (ITracerBlock traceBlock = sendInTheThisObject.tracer.Enter((object) sendInTheThisObject, nameof (GetVersionListDocumentBytes)))
      {
        Locator versionListLocator = sendInTheThisObject.GetVersionListLocator(feedRequest);
        string blobAsync;
        byte[] numArray;
        using (MemoryStream stream = new MemoryStream())
        {
          blobAsync = await sendInTheThisObject.GetBlobService().GetBlobAsync(versionListLocator, (Stream) stream);
          if (blobAsync == null || stream.Length == 0L)
            return new EtagValue<byte[]>(Array.Empty<byte>(), blobAsync);
          numArray = stream.ToArray();
        }
        sendInTheThisObject.packagingTraces.AddProperty("LastVersionListsBlobSize", (object) numArray.Length);
        traceBlock.TraceInfo(string.Format("Version list blob length {0}. First bytes of blob: {1}", (object) numArray.Length, (object) string.Join(" ", ((IEnumerable<byte>) numArray).Take<byte>(10).Select<byte, string>((Func<byte, string>) (x => x.ToString("X2"))))));
        if (numArray[0] == (byte) 120)
        {
          numArray = CompressionHelper.InflateByteArray(numArray, true);
          traceBlock.TraceInfoAlways("Had to decompress content expected to be already decompressed by GetBlobAsync. First bytes after decompression: " + string.Join(" ", ((IEnumerable<byte>) numArray).Take<byte>(10).Select<byte, string>((Func<byte, string>) (x => x.ToString("X2")))));
        }
        return new EtagValue<byte[]>(numArray, blobAsync);
      }
    }

    public async Task<string> PutVersionListDocument(
      IFeedRequest feedRequest,
      ILazyVersionListsFile versionListsFile,
      string etag)
    {
      VersionListsFileProvider sendInTheThisObject = this;
      string str1;
      using (sendInTheThisObject.tracer.Enter((object) sendInTheThisObject, nameof (PutVersionListDocument)))
      {
        VersionListsFileWrapper versionListsFileWrapper = (VersionListsFileWrapper) versionListsFile;
        Locator versionListLocator = sendInTheThisObject.GetVersionListLocator(feedRequest);
        VersionListsFilePacker.Pack(versionListsFileWrapper.Wrapped);
        sendInTheThisObject.versionCountsMetricsRecorder.Record((IVersionCountsImplementationMetrics) versionListsFile, "LastSaved");
        string str2 = await sendInTheThisObject.GetBlobService().PutBlobAsync(versionListLocator, versionListsFileWrapper.Wrapped.ToByteArray().AsArraySegment(), etag, sendInTheThisObject.featureFlagService.IsEnabled("NuGet.DeflateVersionListFile"));
        versionListsFileWrapper.NotifySaved();
        str1 = str2;
      }
      return str1;
    }

    private Locator GetVersionListLocator(IFeedRequest feedRequest) => new Locator(new string[2]
    {
      feedRequest.Feed.Id.ToString(),
      "all.bin"
    });

    private IBlobService GetBlobService() => this.blobServiceFactory.Get(new ContainerAddress((CollectionId) this.executionEnvironment.HostId, this.aggVersionToLocatorConverter.Convert(this.aggregation)));
  }
}
