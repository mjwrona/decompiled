// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.PermanentDeleteValidatingOpGeneratingHandler`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class PermanentDeleteValidatingOpGeneratingHandler<TPackageIdentity, TMetadataEntry> : 
    IAsyncHandler<PackageRequest<TPackageIdentity>, IPermanentDeleteOperationData>,
    IHaveInputType<PackageRequest<TPackageIdentity>>,
    IHaveOutputType<IPermanentDeleteOperationData>
    where TPackageIdentity : IPackageIdentity
    where TMetadataEntry : IMetadataEntry<TPackageIdentity>
  {
    private readonly IAsyncHandler<PackageRequest<TPackageIdentity>, TMetadataEntry> metadataHandler;

    public PermanentDeleteValidatingOpGeneratingHandler(
      IAsyncHandler<PackageRequest<TPackageIdentity>, TMetadataEntry> metadataHandler)
    {
      this.metadataHandler = metadataHandler;
    }

    public async Task<IPermanentDeleteOperationData> Handle(PackageRequest<TPackageIdentity> request)
    {
      TMetadataEntry metadataEntry = await this.metadataHandler.Handle(request);
      if ((object) metadataEntry == null)
        throw ExceptionHelper.PackageNotFound((IPackageIdentity) request.PackageId, request.Feed);
      int num = metadataEntry.PackageStorageId != null ? (int) PackageStateMachineValidator.ValidatePermanentDeleteTransition<TPackageIdentity>((IMetadataEntry) metadataEntry, (IPackageRequest<TPackageIdentity>) request) : throw new ArgumentException("unexpected: storage id should not be null when permanently deleting. Use a different handler for file-level protocols.");
      return (IPermanentDeleteOperationData) new PermanentDeleteOperationData((IPackageIdentity) metadataEntry.PackageIdentity, metadataEntry.PackageStorageId);
    }
  }
}
