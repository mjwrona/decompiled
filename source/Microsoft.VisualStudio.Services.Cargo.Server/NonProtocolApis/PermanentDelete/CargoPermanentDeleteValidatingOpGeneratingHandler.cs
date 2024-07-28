// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.NonProtocolApis.PermanentDelete.CargoPermanentDeleteValidatingOpGeneratingHandler
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.NonProtocolApis.PermanentDelete
{
  public class CargoPermanentDeleteValidatingOpGeneratingHandler : 
    IAsyncHandler<PackageRequest<CargoPackageIdentity>, IPermanentDeleteOperationData>,
    IHaveInputType<PackageRequest<CargoPackageIdentity>>,
    IHaveOutputType<IPermanentDeleteOperationData>
  {
    private readonly IAsyncHandler<PackageRequest<CargoPackageIdentity>, ICargoMetadataEntry> metadataHandler;

    public CargoPermanentDeleteValidatingOpGeneratingHandler(
      IAsyncHandler<PackageRequest<CargoPackageIdentity>, ICargoMetadataEntry> metadataHandler)
    {
      this.metadataHandler = metadataHandler;
    }

    public async Task<IPermanentDeleteOperationData> Handle(
      PackageRequest<CargoPackageIdentity> request)
    {
      ICargoMetadataEntry cargoMetadataEntry = await this.metadataHandler.Handle(request);
      int num = cargoMetadataEntry != null ? (int) PackageStateMachineValidator.ValidatePermanentDeleteTransition<CargoPackageIdentity>((IMetadataEntry) cargoMetadataEntry, (IPackageRequest<CargoPackageIdentity>) request) : throw ExceptionHelper.PackageNotFound((IPackageIdentity) request.PackageId, request.Feed);
      return (IPermanentDeleteOperationData) new PermanentDeleteOperationData((IPackageIdentity) cargoMetadataEntry.PackageIdentity, cargoMetadataEntry.PackageStorageId);
    }
  }
}
