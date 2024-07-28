// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.DeleteProcessingJob.PackageDeletionStateFromMetadataHandler`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.DeleteProcessingJob
{
  public class PackageDeletionStateFromMetadataHandler<TPackageIdentity, TMetadataEntry> : 
    IAsyncHandler<PackageRequest<TPackageIdentity>, PackageDeletionState>,
    IHaveInputType<PackageRequest<TPackageIdentity>>,
    IHaveOutputType<PackageDeletionState>
    where TPackageIdentity : IPackageIdentity
    where TMetadataEntry : IMetadataEntry<TPackageIdentity>
  {
    private readonly IAsyncHandler<PackageRequest<TPackageIdentity>, TMetadataEntry> metadataFetchingHandler;

    public PackageDeletionStateFromMetadataHandler(
      IAsyncHandler<PackageRequest<TPackageIdentity>, TMetadataEntry> metadataFetchingHandler)
    {
      this.metadataFetchingHandler = metadataFetchingHandler;
    }

    public async Task<PackageDeletionState> Handle(PackageRequest<TPackageIdentity> request)
    {
      TMetadataEntry metadataEntry = await this.metadataFetchingHandler.Handle(request);
      if ((object) metadataEntry == null)
        throw ExceptionHelper.PackageNotFound((IPackageIdentity) request.PackageId, request.Feed);
      return new PackageDeletionState(metadataEntry.DeletedDate, metadataEntry.ScheduledPermanentDeleteDate, metadataEntry.PermanentDeletedDate);
    }
  }
}
