// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.NonProtocolApis.UpdatePackageVersions.CargoRecycleBinBatchValidatingOpGeneratingHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cargo.Server.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.Cargo.Server.NonProtocolApis.PermanentDelete;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Cargo.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.UpdatePackageVersions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Types;
using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.NonProtocolApis.UpdatePackageVersions
{
  public class CargoRecycleBinBatchValidatingOpGeneratingHandlerBootstrapper : 
    RequireAggHandlerBootstrapper<PackagesBatchRequest<CargoPackageIdentity>, BatchCommitOperationData, ICargoPackageMetadataAggregationAccessor>
  {
    private readonly IVssRequestContext requestContext;

    public CargoRecycleBinBatchValidatingOpGeneratingHandlerBootstrapper(
      IVssRequestContext requestContext)
    {
      this.requestContext = requestContext;
    }

    protected override IAsyncHandler<PackagesBatchRequest<CargoPackageIdentity>, BatchCommitOperationData> Bootstrap(
      ICargoPackageMetadataAggregationAccessor metadataAccessor)
    {
      IAsyncHandler<IPackageRequest<CargoPackageIdentity>, ICargoMetadataEntry> pointQueryHandler = metadataAccessor.ToPointQueryHandler<CargoPackageIdentity, ICargoMetadataEntry>(true);
      RestoreToFeedValidatingOpGeneratingHandler<CargoPackageIdentity, ICargoMetadataEntry, IRestoreToFeedOperationData> individualOpGeneratingHandler = new RestoreToFeedValidatingOpGeneratingHandler<CargoPackageIdentity, ICargoMetadataEntry, IRestoreToFeedOperationData>((IAsyncHandler<PackageRequest<CargoPackageIdentity>, ICargoMetadataEntry>) pointQueryHandler, (IAsyncHandler<PackageRequest<CargoPackageIdentity>, IRestoreToFeedOperationData>) new RestoreToFeedOpGeneratingHandler<CargoPackageIdentity>());
      ByFuncConverter<PackagesBatchRequest<CargoPackageIdentity>, PackagesBatchRequest<CargoPackageIdentity, IRecycleBinPackageVersionDetails>> requestWithDataConverter = new ByFuncConverter<PackagesBatchRequest<CargoPackageIdentity>, PackagesBatchRequest<CargoPackageIdentity, IRecycleBinPackageVersionDetails>>((Func<PackagesBatchRequest<CargoPackageIdentity>, PackagesBatchRequest<CargoPackageIdentity, IRecycleBinPackageVersionDetails>>) (r => new PackagesBatchRequest<CargoPackageIdentity, IRecycleBinPackageVersionDetails>(r, (IRecycleBinPackageVersionDetails) new CargoRecycleBinPackageVersionDetails()
      {
        Deleted = new bool?(false)
      })));
      return UntilNonNullHandler.Create<PackagesBatchRequest<CargoPackageIdentity>, BatchCommitOperationData>(PackagesBatchRequestOpGeneratingHandler.Create<CargoPackageIdentity, IRecycleBinPackageVersionDetails, IRestoreToFeedOperationData>(BatchOperationType.RestoreToFeed, (IConverter<PackagesBatchRequest<CargoPackageIdentity>, PackagesBatchRequest<CargoPackageIdentity, IRecycleBinPackageVersionDetails>>) requestWithDataConverter, (IAsyncHandler<PackageRequest<CargoPackageIdentity, IRecycleBinPackageVersionDetails>, IRestoreToFeedOperationData>) individualOpGeneratingHandler), PackagesBatchRequestOpGeneratingHandler.Create<CargoPackageIdentity, IPermanentDeleteOperationData>(BatchOperationType.PermanentDelete, (IAsyncHandler<PackageRequest<CargoPackageIdentity>, IPermanentDeleteOperationData>) new CargoPermanentDeleteValidatingOpGeneratingHandler((IAsyncHandler<PackageRequest<CargoPackageIdentity>, ICargoMetadataEntry>) pointQueryHandler)), (IAsyncHandler<PackagesBatchRequest<CargoPackageIdentity>, BatchCommitOperationData>) new ThrowWithBadOperationMessageHandler<CargoPackageIdentity>());
    }
  }
}
