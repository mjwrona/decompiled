// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.NonProtocolApis.RestoreToFeed.CargoRestoreToFeedValidatingOpGeneratingHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.VisualStudio.Services.Cargo.Server.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Types;
using System.Collections.Generic;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.NonProtocolApis.RestoreToFeed
{
  public class CargoRestoreToFeedValidatingOpGeneratingHandlerBootstrapper : 
    RequireAggHandlerBootstrapper<PackageRequest<CargoPackageIdentity, IRecycleBinPackageVersionDetails>, IRestoreToFeedOperationData, ICargoPackageMetadataAggregationAccessor>
  {
    protected override IAsyncHandler<PackageRequest<CargoPackageIdentity, IRecycleBinPackageVersionDetails>, IRestoreToFeedOperationData> Bootstrap(
      ICargoPackageMetadataAggregationAccessor metadataAccessor)
    {
      return UntilNonNullHandler.Create<PackageRequest<CargoPackageIdentity, IRecycleBinPackageVersionDetails>, IRestoreToFeedOperationData>((IAsyncHandler<PackageRequest<CargoPackageIdentity, IRecycleBinPackageVersionDetails>, IRestoreToFeedOperationData>) new RestoreToFeedValidatingOpGeneratingHandler<CargoPackageIdentity, ICargoMetadataEntry, IRestoreToFeedOperationData>((IAsyncHandler<PackageRequest<CargoPackageIdentity>, ICargoMetadataEntry>) metadataAccessor.ToPointQueryHandler<CargoPackageIdentity, ICargoMetadataEntry>(), (IAsyncHandler<PackageRequest<CargoPackageIdentity>, IRestoreToFeedOperationData>) new RestoreToFeedOpGeneratingHandler<CargoPackageIdentity>()), (IAsyncHandler<PackageRequest<CargoPackageIdentity, IRecycleBinPackageVersionDetails>, IRestoreToFeedOperationData>) new ThrowWithBadInputMessageHandler<PackageRequest<CargoPackageIdentity, IRecycleBinPackageVersionDetails>, IRestoreToFeedOperationData>((IEnumerable<string>) new string[1]
      {
        "Deleted"
      }));
    }
  }
}
