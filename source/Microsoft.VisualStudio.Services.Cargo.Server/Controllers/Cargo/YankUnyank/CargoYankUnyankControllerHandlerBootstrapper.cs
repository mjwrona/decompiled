// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Controllers.Cargo.YankUnyank.CargoYankUnyankControllerHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cargo.Server.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.Cargo.Server.CiData;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.ListUnlistPackageVersion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.UpdatePackageVersions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Controllers.Cargo.YankUnyank
{
  public static class CargoYankUnyankControllerHandlerBootstrapper
  {
    public static IAsyncHandler<IPackageRequest<CargoPackageIdentity, ListingOperationRequestAdditionalData>, CargoOkSuccessResult> Bootstrap(
      IVssRequestContext requestContext)
    {
      return new CargoWriteBootstrapper<IPackageRequest<CargoPackageIdentity, ListingOperationRequestAdditionalData>, IListingStateChangeOperationData, CargoOkSuccessResult>(requestContext, ByFuncRequireAggHandlerBootstrapper.For<IPackageRequest<CargoPackageIdentity, ListingOperationRequestAdditionalData>, IListingStateChangeOperationData, ICargoPackageMetadataAggregationAccessor>((Func<ICargoPackageMetadataAggregationAccessor, IAsyncHandler<IPackageRequest<CargoPackageIdentity, ListingOperationRequestAdditionalData>, IListingStateChangeOperationData>>) (metadataAccessor => (IAsyncHandler<IPackageRequest<CargoPackageIdentity, ListingOperationRequestAdditionalData>, IListingStateChangeOperationData>) new ListValidatingHandler<ListingOperationRequestAdditionalData, CargoPackageIdentity, ICargoMetadataEntry>(metadataAccessor.ToPointQueryHandler<CargoPackageIdentity, ICargoMetadataEntry>(), (IConverter<IPackageRequest<CargoPackageIdentity, ListingOperationRequestAdditionalData>, IListingStateChangeOperationData>) new ListingOperationDataGeneratingConverter()))), (IAsyncHandler<ICommitLogEntry, CargoOkSuccessResult>) new AlwaysReturnCargoOkSuccessResultHandler(), (IAsyncHandler<(IPackageRequest<CargoPackageIdentity, ListingOperationRequestAdditionalData>, IListingStateChangeOperationData), ICiData>) new CargoYankCiDataFacadeHandler(requestContext), false).Bootstrap();
    }
  }
}
