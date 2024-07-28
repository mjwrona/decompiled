// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.DeletePackageVersion.DeleteOpGenerator`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.DeletePackageVersion
{
  public class DeleteOpGenerator<TIdentity, TMetadataEntry> : 
    IAsyncHandler<
    #nullable disable
    IPackageRequest<TIdentity, DeleteRequestAdditionalData>, IDeleteOperationData>,
    IHaveInputType<IPackageRequest<TIdentity, DeleteRequestAdditionalData>>,
    IHaveOutputType<IDeleteOperationData>
    where TIdentity : IPackageIdentity
    where TMetadataEntry : IMetadataEntry<TIdentity>
  {
    private readonly IReadSingleVersionMetadataService<TIdentity, TMetadataEntry> metadataService;
    private readonly IAsyncHandler<DateTime, DateTime> scheduledPermanentDeleteDateGeneratingHandler;

    public DeleteOpGenerator(
      IReadSingleVersionMetadataService<TIdentity, TMetadataEntry> metadataService,
      IAsyncHandler<DateTime, DateTime> scheduledPermanentDeleteDateGeneratingHandler)
    {
      this.metadataService = metadataService;
      this.scheduledPermanentDeleteDateGeneratingHandler = scheduledPermanentDeleteDateGeneratingHandler;
    }

    public async Task<IDeleteOperationData> Handle(
      IPackageRequest<TIdentity, DeleteRequestAdditionalData> request)
    {
      int num = (int) PackageStateMachineValidator.ValidateDeleteTransition<TIdentity>((IMetadataEntry) await this.metadataService.GetPackageVersionStateAsync((IPackageRequest<TIdentity>) request), (IPackageRequest<TIdentity>) request);
      DateTime deletedDate = request.AdditionalData.RequestTime;
      return (IDeleteOperationData) new DeleteOperationData((IPackageIdentity) request.PackageId, deletedDate, new DateTime?(await this.scheduledPermanentDeleteDateGeneratingHandler.Handle(deletedDate)));
    }
  }
}
