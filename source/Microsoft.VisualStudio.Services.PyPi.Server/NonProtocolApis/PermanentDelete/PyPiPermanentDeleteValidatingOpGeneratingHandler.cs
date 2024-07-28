// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.NonProtocolApis.PermanentDelete.PyPiPermanentDeleteValidatingOpGeneratingHandler
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobStore;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.PermanentDelete;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.PyPi.Server.Metadata;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.PyPi.Server.NonProtocolApis.PermanentDelete
{
  public class PyPiPermanentDeleteValidatingOpGeneratingHandler : 
    IAsyncHandler<PackageRequest<PyPiPackageIdentity>, IPermanentDeleteOperationData>,
    IHaveInputType<PackageRequest<PyPiPackageIdentity>>,
    IHaveOutputType<IPermanentDeleteOperationData>
  {
    private readonly IAsyncHandler<PackageRequest<PyPiPackageIdentity>, IPyPiMetadataEntry> metadataHandler;

    public PyPiPermanentDeleteValidatingOpGeneratingHandler(
      IAsyncHandler<PackageRequest<PyPiPackageIdentity>, IPyPiMetadataEntry> metadataHandler)
    {
      this.metadataHandler = metadataHandler;
    }

    public async Task<IPermanentDeleteOperationData> Handle(
      PackageRequest<PyPiPackageIdentity> request)
    {
      IPyPiMetadataEntry metadataEntry = await this.metadataHandler.Handle(request);
      int num = metadataEntry != null ? (int) PackageStateMachineValidator.ValidatePermanentDeleteTransition<PyPiPackageIdentity>((IMetadataEntry) metadataEntry, (IPackageRequest<PyPiPackageIdentity>) request) : throw ExceptionHelper.PackageNotFound((IPackageIdentity) request.PackageId, request.Feed);
      IEnumerable<BlobReferenceIdentifier> extraAssetsBlobReferences = new PackageBlobRefsEnumeratingConverter<PyPiPackageIdentity, IPyPiMetadataEntry>((IFeedRequest) request).Convert(metadataEntry);
      return (IPermanentDeleteOperationData) new PyPiPermanentDeleteOperationData(metadataEntry.PackageIdentity, extraAssetsBlobReferences);
    }
  }
}
