// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Upstreams.InternalUpstreamClient.SameCollectionUpstreamCargoClient
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cargo.Server.Aggregations;
using Microsoft.VisualStudio.Services.Cargo.Server.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.Cargo.Server.Controllers.Cargo.Index;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Upstreams.InternalUpstreamClient
{
  public class SameCollectionUpstreamCargoClient : IUpstreamCargoClient
  {
    private readonly IVssRequestContext collectionContext;
    private readonly FeedServiceFacade feedServiceFacade;
    private readonly UpstreamSource upstreamSource;
    private readonly IHttpClient httpClient;

    public SameCollectionUpstreamCargoClient(
      IVssRequestContext collectionContext,
      FeedServiceFacade feedServiceFacade,
      UpstreamSource upstreamSource,
      IHttpClient httpClient)
    {
      this.collectionContext = collectionContext;
      this.feedServiceFacade = feedServiceFacade;
      this.upstreamSource = upstreamSource;
      this.httpClient = httpClient;
    }

    public async Task<IReadOnlyList<LimitedCargoMetadata>> GetLimitedMetadataList(
      CargoPackageName packageName)
    {
      IPackageNameRequest<CargoPackageName> packageRequest = this.GetFeedRequest().WithPackageName<CargoPackageName>(packageName);
      IReadOnlyList<LimitedCargoMetadata> list = (IReadOnlyList<LimitedCargoMetadata>) (await (await this.GetMetadataService((IFeedRequest) packageRequest)).GetPackageVersionStatesAsync(new PackageNameQuery<ICargoMetadataEntry>((IPackageNameRequest) packageRequest))).Where<ICargoMetadataEntry>((Func<ICargoMetadataEntry, bool>) (packageMetadata => packageMetadata != null && !packageMetadata.IsDeleted())).Select<ICargoMetadataEntry, LimitedCargoMetadata>((Func<ICargoMetadataEntry, LimitedCargoMetadata>) (x => new LimitedCargoMetadata(VersionWithSourceChain.FromInternalSource<CargoPackageVersion>(x.PackageIdentity.Version, x.SourceChain), CargoIndexVersionRow.FromMetadataEntry(x).LazySerialize(), (IImmutableList<HashAndType>) x.Hashes.ToImmutableList<HashAndType>()))).ToList<LimitedCargoMetadata>();
      packageRequest = (IPackageNameRequest<CargoPackageName>) null;
      return list;
    }

    public async Task<Stream> GetPackageContentStreamAsync(CargoPackageIdentity packageIdentity)
    {
      SameCollectionUpstreamCargoClient upstreamCargoClient = this;
      // ISSUE: reference to a compiler-generated method
      IFactory<IFeedRequest, Task<IAsyncHandler<IPackageFileRequest<CargoPackageIdentity>, HttpResponseMessage>>> factory = CargoAggregationResolver.Bootstrap(upstreamCargoClient.collectionContext).FactoryFor<IAsyncHandler<IPackageFileRequest<CargoPackageIdentity>, HttpResponseMessage>, IMetadataDocumentService<CargoPackageIdentity, ICargoMetadataEntry>, IMetadataCacheService>(new Func<IMetadataDocumentService<CargoPackageIdentity, ICargoMetadataEntry>, IMetadataCacheService, IAsyncHandler<IPackageFileRequest<CargoPackageIdentity>, HttpResponseMessage>>(upstreamCargoClient.\u003CGetPackageContentStreamAsync\u003Eb__6_0));
      IPackageFileRequest<CargoPackageIdentity> packageRequest = upstreamCargoClient.GetFeedRequest().WithPackage<CargoPackageIdentity>(packageIdentity).WithFile<CargoPackageIdentity>(packageIdentity.GetCanonicalCrateFileName());
      IPackageFileRequest<CargoPackageIdentity> input = packageRequest;
      HttpResponseMessage httpResponseMessage = await (await factory.Get((IFeedRequest) input)).Handle(packageRequest);
      Uri location = httpResponseMessage.Headers.Location;
      if (location != (Uri) null)
        httpResponseMessage = await upstreamCargoClient.httpClient.GetAsync(location, HttpCompletionOption.ResponseHeadersRead);
      httpResponseMessage.EnsureSuccessStatusCode();
      Stream contentStreamAsync = await httpResponseMessage.Content.ReadAsStreamAsync();
      packageRequest = (IPackageFileRequest<CargoPackageIdentity>) null;
      return contentStreamAsync;
    }

    public async Task<CargoUpstreamMetadata> GetUpstreamMetadata(
      CargoPackageIdentity packageIdentity)
    {
      IPackageRequest<CargoPackageIdentity> packageRequest = this.GetFeedRequest().WithPackage<CargoPackageIdentity>(packageIdentity);
      ICargoMetadataEntry versionStateAsync = await (await this.GetMetadataService((IFeedRequest) packageRequest)).GetPackageVersionStateAsync(packageRequest);
      if (versionStateAsync == null || versionStateAsync.Metadata == null)
        throw new PackageNotFoundException(Microsoft.VisualStudio.Services.Cargo.Server.Resources.Error_PackageVersionNotFound((object) packageIdentity.Name.DisplayName, (object) packageIdentity.Version.DisplayVersion));
      CargoUpstreamMetadata upstreamMetadata = new CargoUpstreamMetadata(CargoIndexVersionRow.FromMetadataEntry(versionStateAsync).LazySerialize(), versionStateAsync.Metadata.Serialized.PublishManifest, versionStateAsync.SourceChain, versionStateAsync.PackageStorageId);
      packageRequest = (IPackageRequest<CargoPackageIdentity>) null;
      return upstreamMetadata;
    }

    private IFeedRequest GetFeedRequest() => (IFeedRequest) new FeedRequest(this.feedServiceFacade.GetFeed(this.upstreamSource.GetProjectId() ?? Guid.Empty, this.upstreamSource.GetFullyQualifiedFeedId()), (IProtocol) Protocol.Cargo);

    private async Task<ICargoPackageMetadataAggregationAccessor> GetMetadataService(
      IFeedRequest feedRequest)
    {
      return await CargoAggregationResolver.Bootstrap(this.collectionContext).FactoryFor<ICargoPackageMetadataAggregationAccessor>().Get(feedRequest);
    }
  }
}
