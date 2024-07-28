// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Metadata.CargoMetadataHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cargo.Server.Aggregations;
using Microsoft.VisualStudio.Services.Cargo.Server.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Cargo.Server.Upstreams;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList;
using System;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Metadata
{
  public class CargoMetadataHandlerBootstrapper : 
    IBootstrapper<IAsyncHandler<PackageRequest<CargoPackageIdentity>, ICargoMetadataEntry>>
  {
    private readonly IVssRequestContext requestContext;

    public CargoMetadataHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<PackageRequest<CargoPackageIdentity>, ICargoMetadataEntry> Bootstrap() => CargoAggregationResolver.Bootstrap(this.requestContext).HandlerFor<PackageRequest<CargoPackageIdentity>, ICargoMetadataEntry, ICargoPackageMetadataAggregationAccessor, IUpstreamVersionListService<CargoPackageName, CargoPackageVersion>>((Func<ICargoPackageMetadataAggregationAccessor, IUpstreamVersionListService<CargoPackageName, CargoPackageVersion>, IAsyncHandler<PackageRequest<CargoPackageIdentity>, ICargoMetadataEntry>>) ((metadataAccessor, upstreamVersionListService) =>
    {
      IReadMetadataDocumentService<CargoPackageIdentity, ICargoMetadataEntry> upstreamFetchingMetadata = new CargoUpstreamFetchingMetadataServiceBootstrapper(this.requestContext, (IReadMetadataDocumentService<CargoPackageIdentity, ICargoMetadataEntry>) metadataAccessor, upstreamVersionListService).Bootstrap();
      return ByAsyncFuncAsyncHandler.For<PackageRequest<CargoPackageIdentity>, ICargoMetadataEntry>((Func<PackageRequest<CargoPackageIdentity>, Task<ICargoMetadataEntry>>) (async request => await upstreamFetchingMetadata.GetPackageVersionStateAsync((IPackageRequest<CargoPackageIdentity>) request)));
    }));
  }
}
