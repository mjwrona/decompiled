// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Upstreams.CargoUpstreamMetadataManagerFactoryBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cargo.Server.Aggregations;
using Microsoft.VisualStudio.Services.Cargo.Server.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList;
using System;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Upstreams
{
  public class CargoUpstreamMetadataManagerFactoryBootstrapper : 
    IBootstrapper<IFactory<IFeedRequest, Task<IUpstreamMetadataManager>>>
  {
    private readonly IVssRequestContext requestContext;

    public CargoUpstreamMetadataManagerFactoryBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IFactory<IFeedRequest, Task<IUpstreamMetadataManager>> Bootstrap()
    {
      Func<ICargoPackageMetadataAggregationAccessor, IUpstreamVersionListService<CargoPackageName, CargoPackageVersion>, IUpstreamMetadataManager> bootstrapFunc = (Func<ICargoPackageMetadataAggregationAccessor, IUpstreamVersionListService<CargoPackageName, CargoPackageVersion>, IUpstreamMetadataManager>) ((localMetadataService, upstreamVersionListService) => new CargoUpstreamMetadataManagerExistingLocalMetadataBootstrapper(this.requestContext, (IReadMetadataDocumentService<CargoPackageIdentity, ICargoMetadataEntry>) localMetadataService, upstreamVersionListService).Bootstrap());
      return CargoAggregationResolver.Bootstrap(this.requestContext).FactoryFor<IUpstreamMetadataManager, ICargoPackageMetadataAggregationAccessor, IUpstreamVersionListService<CargoPackageName, CargoPackageVersion>>(bootstrapFunc);
    }
  }
}
