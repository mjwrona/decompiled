// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Upstreams.InternalUpstreamClient.PyPiInternalUpstreamHelper
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.PyPi.Client.Internal;
using Microsoft.VisualStudio.Services.PyPi.Server.Controllers;
using Microsoft.VisualStudio.Services.PyPi.Server.Metadata;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.PyPi.Server.Upstreams.InternalUpstreamClient
{
  public class PyPiInternalUpstreamHelper
  {
    private readonly 
    #nullable disable
    IVssRequestContext collectionContext;
    private readonly FeedServiceFacade feedServiceFacade;
    private readonly string feedId;
    private readonly Guid projectId;
    private readonly Uri upstreamSourceUri;

    public PyPiInternalUpstreamHelper(
      IVssRequestContext collectionContext,
      FeedServiceFacade feedServiceFacade,
      Uri upstreamSourceUri,
      string feedId,
      Guid? projectId)
    {
      this.collectionContext = collectionContext;
      this.feedServiceFacade = feedServiceFacade;
      this.feedId = feedId;
      this.upstreamSourceUri = upstreamSourceUri;
      this.projectId = projectId ?? Guid.Empty;
    }

    public async Task<Stream> GetFile(PyPiPackageIdentity packageIdentity, string filePath)
    {
      if ((await this.GetUpstreamMetadata(packageIdentity, filePath)).RawFileMetadata == null)
        throw new InternalUpstreamFailureException(Microsoft.VisualStudio.Services.PyPi.Server.Resources.Error_InternalUpstreamDoesNotHaveFileRequested((object) filePath, (object) packageIdentity.DisplayStringForMessages, (object) this.feedId), this.upstreamSourceUri);
      IFactory<IFeedRequest, Task<IAsyncHandler<RawPackageFileRequest, HttpResponseMessage>>> factory = PyPiAggregationResolver.Bootstrap(this.collectionContext).FactoryFor<IAsyncHandler<RawPackageFileRequest, HttpResponseMessage>>((IRequireAggBootstrapper<IAsyncHandler<RawPackageFileRequest, HttpResponseMessage>>) new PyPiBlobDownloadPackageFileHandlerBootstrapper(this.collectionContext));
      RawPackageFileRequest rawPackageFileRequest = new RawPackageFileRequest(this.GetFeed(), packageIdentity.Name.NormalizedName, packageIdentity.Version.NormalizedVersion, filePath);
      RawPackageFileRequest input = rawPackageFileRequest;
      HttpResponseMessage httpResponseMessage = await (await factory.Get((IFeedRequest) input)).Handle(rawPackageFileRequest);
      Uri location = httpResponseMessage.Headers.Location;
      if (location != (Uri) null)
        return await new HttpClient().GetStreamAsync(location);
      httpResponseMessage.EnsureSuccessStatusCode();
      return await httpResponseMessage.Content.ReadAsStreamAsync();
    }

    public async Task<Stream> GetGpgSignatureForFile(
      PyPiPackageIdentity packageIdentity,
      string filePath)
    {
      PyPiInternalUpstreamMetadata upstreamMetadata = await this.GetUpstreamMetadata(packageIdentity, filePath);
      return !string.IsNullOrWhiteSpace(upstreamMetadata.Base64ZippedGpgSignature) ? (Stream) new MemoryStream(DeflateCompressibleBytes.FromDeflatedBase64String(upstreamMetadata.Base64ZippedGpgSignature).AsUncompressedBytes(), false) : (Stream) null;
    }

    public async Task<IEnumerable<LimitedPyPiMetadata>> GetLimitedMetadataList(
      PyPiPackageName packageName,
      IEnumerable<PyPiPackageVersion> versions)
    {
      HashSet<IPackageVersion> versionsSet = ((IEnumerable<IPackageVersion>) versions).ToHashSet<IPackageVersion>((IEqualityComparer<IPackageVersion>) PackageVersionComparer.NormalizedVersion);
      IPackageNameRequest<PyPiPackageName> packageNameRequest = this.GetFeed().WithPackageName<PyPiPackageName>(packageName);
      IEnumerable<LimitedPyPiMetadata> limitedMetadataList = (await (await this.GetLimitedMetadataDocumentService((IFeedRequest) packageNameRequest)).GetPackageVersionStatesAsync(new PackageNameQuery<IPyPiMetadataEntry>((IPackageNameRequest) packageNameRequest))).Where<IPyPiMetadataEntry>((Func<IPyPiMetadataEntry, bool>) (e => !e.IsDeleted() && versionsSet.Contains((IPackageVersion) e.PackageIdentity.Version))).Select<IPyPiMetadataEntry, LimitedPyPiMetadata>((Func<IPyPiMetadataEntry, LimitedPyPiMetadata>) (e => new LimitedPyPiMetadata(e.PackageIdentity, e.RequiresPython?.ToString(), e.PackageFiles.ToImmutableArray<PyPiPackageFile>().CastArray<IUnstoredPyPiPackageFile>())));
      packageNameRequest = (IPackageNameRequest<PyPiPackageName>) null;
      return limitedMetadataList;
    }

    public async Task<IReadOnlyList<VersionWithSourceChain<PyPiPackageVersion>>> GetPackageVersions(
      PyPiPackageName packageName)
    {
      IFeedRequest feedRequest = this.GetFeed();
      IReadOnlyList<VersionWithSourceChain<PyPiPackageVersion>> list = (IReadOnlyList<VersionWithSourceChain<PyPiPackageVersion>>) (await (await this.GetLimitedMetadataDocumentService(feedRequest)).GetPackageVersionStatesAsync(new PackageNameQuery<IPyPiMetadataEntry>((IPackageNameRequest) new PackageNameRequest<PyPiPackageName>(feedRequest, packageName)))).Where<IPyPiMetadataEntry>((Func<IPyPiMetadataEntry, bool>) (e => !e.IsDeleted())).Select<IPyPiMetadataEntry, VersionWithSourceChain<PyPiPackageVersion>>((Func<IPyPiMetadataEntry, VersionWithSourceChain<PyPiPackageVersion>>) (entry => VersionWithSourceChain.FromInternalSource<PyPiPackageVersion>(entry.PackageIdentity.Version, entry.SourceChain))).ToList<VersionWithSourceChain<PyPiPackageVersion>>();
      feedRequest = (IFeedRequest) null;
      return list;
    }

    public async Task<PyPiInternalUpstreamMetadata> GetUpstreamMetadata(
      PyPiPackageIdentity packageIdentity,
      string filePath)
    {
      IPackageFileRequest<PyPiPackageIdentity> packageFileRequest = this.GetFeed().WithPackage<PyPiPackageIdentity>(packageIdentity).WithFile<PyPiPackageIdentity>(filePath);
      PyPiInternalUpstreamMetadata upstreamMetadata = await new PyPiUpstreamPackageFileRequestToUpstreamMetadataConverter(await PyPiAggregationResolver.Bootstrap(this.collectionContext).FactoryFor<IMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntryWithRawMetadata>>().Get((IFeedRequest) packageFileRequest)).Convert(packageFileRequest) ?? throw new PackageNotFoundException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_PackageFileNotFound((object) packageFileRequest.Feed.FullyQualifiedName, (object) packageFileRequest.PackageId, (object) packageFileRequest.FilePath));
      packageFileRequest = (IPackageFileRequest<PyPiPackageIdentity>) null;
      return upstreamMetadata;
    }

    private async Task<IMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntry>> GetLimitedMetadataDocumentService(
      IFeedRequest feedRequest)
    {
      return (IMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntry>) await PyPiAggregationResolver.Bootstrap(this.collectionContext).FactoryFor<IPyPiMetadataAggregationAccessor>().Get(feedRequest);
    }

    private IFeedRequest GetFeed() => (IFeedRequest) new FeedRequest(this.feedServiceFacade.GetFeed(this.projectId, this.feedId), (IProtocol) Protocol.PyPi);
  }
}
