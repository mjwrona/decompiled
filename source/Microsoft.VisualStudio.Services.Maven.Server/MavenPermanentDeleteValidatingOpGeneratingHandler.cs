// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.MavenPermanentDeleteValidatingOpGeneratingHandler
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Maven.Server.DeleteProcessingJob;
using Microsoft.VisualStudio.Services.Maven.Server.Implementations.CommitLog.OperationData;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobStore;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  public class MavenPermanentDeleteValidatingOpGeneratingHandler : 
    IAsyncHandler<PackageRequest<MavenPackageIdentity>, IPermanentDeleteOperationData>,
    IHaveInputType<PackageRequest<MavenPackageIdentity>>,
    IHaveOutputType<IPermanentDeleteOperationData>
  {
    private readonly IAsyncHandler<PackageRequest<MavenPackageIdentity>, IMavenMetadataEntry> metadataHandler;
    private readonly IConverter<MavenPackageRequestWithMetadata, IEnumerable<BlobReferenceIdentifier>> blobIdsConverter;

    public MavenPermanentDeleteValidatingOpGeneratingHandler(
      IAsyncHandler<PackageRequest<MavenPackageIdentity>, IMavenMetadataEntry> metadataHandler,
      IConverter<MavenPackageRequestWithMetadata, IEnumerable<BlobReferenceIdentifier>> blobIdsConverter)
    {
      this.metadataHandler = metadataHandler;
      this.blobIdsConverter = blobIdsConverter;
    }

    public async Task<IPermanentDeleteOperationData> Handle(
      PackageRequest<MavenPackageIdentity> request)
    {
      IMavenMetadataEntry metadata = await this.metadataHandler.Handle(request);
      int num = metadata != null ? (int) PackageStateMachineValidator.ValidatePermanentDeleteTransition<MavenPackageIdentity>((IMetadataEntry) metadata, (IPackageRequest<MavenPackageIdentity>) request) : throw ExceptionHelper.PackageNotFound((IPackageIdentity) request.PackageId, request.Feed);
      IEnumerable<BlobReferenceIdentifier> extraAssetsBlobReferences = this.blobIdsConverter.Convert(new MavenPackageRequestWithMetadata(request, metadata));
      return (IPermanentDeleteOperationData) new MavenPermanentDeleteOperationData(metadata.PackageIdentity, extraAssetsBlobReferences);
    }
  }
}
