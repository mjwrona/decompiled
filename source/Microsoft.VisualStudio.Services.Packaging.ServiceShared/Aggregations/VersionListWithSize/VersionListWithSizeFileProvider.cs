// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.VersionListWithSize.VersionListWithSizeFileProvider
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Google.Protobuf;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.VersionListWithSize
{
  public class VersionListWithSizeFileProvider : IVersionListWithSizeFileProvider
  {
    private readonly IConverter<IAggregation, Locator> aggVersionToLocatorConverter;
    private readonly IExecutionEnvironment executionEnvironment;
    private readonly IPackagingTraces packagingTraces;
    private readonly ITracerService tracer;
    private readonly IVersionCountsImplementationMetricsRecorder versionCountsMetricsRecorder;
    private readonly IFactory<ContainerAddress, IBlobService> blobServiceFactory;
    private readonly IAggregation aggregation;
    private readonly IFeatureFlagService featureFlagService;
    private readonly IConverter<string, IPackageName> PackageNameConverter;
    private readonly IConverter<string, IPackageVersion> PackageVersionConverter;
    private readonly IEqualityComparer<string> FileNameComparer;

    public VersionListWithSizeFileProvider(
      IAggregation aggregation,
      IFactory<ContainerAddress, IBlobService> blobServiceFactory,
      IConverter<IAggregation, Locator> aggVersionToLocatorConverter,
      IExecutionEnvironment executionEnvironment,
      IPackagingTraces packagingTraces,
      ITracerService tracer,
      IVersionCountsImplementationMetricsRecorder versionCountsMetricsRecorder,
      IFeatureFlagService featureFlagService,
      IConverter<string, IPackageName> packageNameConverter,
      IConverter<string, IPackageVersion> packageVersionConverter,
      IEqualityComparer<string> fileNameComparer)
    {
      this.blobServiceFactory = blobServiceFactory;
      this.aggVersionToLocatorConverter = aggVersionToLocatorConverter;
      this.executionEnvironment = executionEnvironment;
      this.packagingTraces = packagingTraces;
      this.tracer = tracer;
      this.versionCountsMetricsRecorder = versionCountsMetricsRecorder;
      this.featureFlagService = featureFlagService;
      this.PackageNameConverter = packageNameConverter;
      this.PackageVersionConverter = packageVersionConverter;
      this.FileNameComparer = fileNameComparer;
      this.aggregation = aggregation;
    }

    public async Task<EtagValue<IMutableVersionListWithSizeFile>> GetVersionListWithSizeDocument(
      IFeedRequest feedRequest)
    {
      VersionListWithSizeFileProvider sendInTheThisObject = this;
      using (ITracerBlock traceBlock = sendInTheThisObject.tracer.Enter((object) sendInTheThisObject, nameof (GetVersionListWithSizeDocument)))
      {
        Locator versionListLocator = sendInTheThisObject.GetVersionListLocator(feedRequest);
        string blobAsync;
        byte[] numArray;
        using (MemoryStream stream = new MemoryStream())
        {
          blobAsync = await sendInTheThisObject.GetBlobService().GetBlobAsync(versionListLocator, (Stream) stream);
          if (blobAsync == null || stream.Length == 0L)
            return new EtagValue<IMutableVersionListWithSizeFile>((IMutableVersionListWithSizeFile) new VersionListWithSizeFileWrapper(VersionListWithSizeFile.CreateNewUnpacked(), sendInTheThisObject.PackageNameConverter, sendInTheThisObject.PackageVersionConverter, sendInTheThisObject.FileNameComparer), blobAsync);
          numArray = stream.ToArray();
        }
        sendInTheThisObject.packagingTraces.AddProperty("LastVersionListBlobSize", (object) numArray.Length);
        traceBlock.TraceInfo(string.Format("Version list blob length {0}. First bytes of blob: {1}", (object) numArray.Length, (object) string.Join(" ", ((IEnumerable<byte>) numArray).Take<byte>(10).Select<byte, string>((Func<byte, string>) (x => x.ToString("X2"))))));
        if (numArray[0] == (byte) 120)
        {
          numArray = CompressionHelper.InflateByteArray(numArray, true);
          traceBlock.TraceInfoAlways("Had to decompress content expected to be already decompressed by GetBlobAsync. First bytes after decompression: " + string.Join(" ", ((IEnumerable<byte>) numArray).Take<byte>(10).Select<byte, string>((Func<byte, string>) (x => x.ToString("X2")))));
        }
        VersionListWithSizeFile from = VersionListWithSizeFile.Parser.ParseFrom(numArray);
        VersionListWithSizeFileUnpacker.Unpack(from);
        sendInTheThisObject.versionCountsMetricsRecorder.Record((IVersionCountsImplementationMetrics) from);
        return new EtagValue<IMutableVersionListWithSizeFile>((IMutableVersionListWithSizeFile) new VersionListWithSizeFileWrapper(from, sendInTheThisObject.PackageNameConverter, sendInTheThisObject.PackageVersionConverter, sendInTheThisObject.FileNameComparer), blobAsync);
      }
    }

    public async Task<string> PutVersionListWithSizeDocument(
      IFeedRequest feedRequest,
      ILazyVersionListWithSizeFile versionListWithSizeFile,
      string etag)
    {
      VersionListWithSizeFileProvider sendInTheThisObject = this;
      string str1;
      using (sendInTheThisObject.tracer.Enter((object) sendInTheThisObject, nameof (PutVersionListWithSizeDocument)))
      {
        VersionListWithSizeFileWrapper versionListWithSizeFileWrapper = (VersionListWithSizeFileWrapper) versionListWithSizeFile;
        Locator versionListLocator = sendInTheThisObject.GetVersionListLocator(feedRequest);
        VersionListWithSizeFilePacker.Pack(versionListWithSizeFileWrapper.Wrapped);
        sendInTheThisObject.versionCountsMetricsRecorder.Record((IVersionCountsImplementationMetrics) versionListWithSizeFile, "LastSaved");
        string str2 = await sendInTheThisObject.GetBlobService().PutBlobAsync(versionListLocator, versionListWithSizeFileWrapper.Wrapped.ToByteArray().AsArraySegment(), etag, sendInTheThisObject.featureFlagService.IsEnabled("Packaging.DeflateVersionListWithSizeFile"));
        versionListWithSizeFileWrapper.NotifySaved();
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
