// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.NonProtocolApis.PermanentDelete.NpmPermanentDeleteValidatingOpGeneratingHandler
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Npm.Server.CodeOnly;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Npm.Server.NonProtocolApis.PermanentDelete
{
  public class NpmPermanentDeleteValidatingOpGeneratingHandler : 
    IAsyncHandler<PackageRequest<NpmPackageIdentity>, IPermanentDeleteOperationData>,
    IHaveInputType<PackageRequest<NpmPackageIdentity>>,
    IHaveOutputType<IPermanentDeleteOperationData>
  {
    private readonly IAsyncHandler<PackageRequest<NpmPackageIdentity>, INpmMetadataEntry> metadataFetchingHandler;

    public NpmPermanentDeleteValidatingOpGeneratingHandler(
      IAsyncHandler<PackageRequest<NpmPackageIdentity>, INpmMetadataEntry> metadataFetchingHandler)
    {
      this.metadataFetchingHandler = metadataFetchingHandler;
    }

    public async Task<IPermanentDeleteOperationData> Handle(
      PackageRequest<NpmPackageIdentity> request)
    {
      INpmMetadataEntry state = await this.metadataFetchingHandler.Handle(request);
      if (state == null)
        return (IPermanentDeleteOperationData) null;
      int num = (int) PackageStateMachineValidator.ValidatePermanentDeleteTransition<NpmPackageIdentity>((IMetadataEntry) state, (IPackageRequest<NpmPackageIdentity>) request);
      return (IPermanentDeleteOperationData) new NpmPermanentDeleteOperationData(state.PackageIdentity, state.PackageStorageId, await new NpmGetExtraAssetsBlobRefIdsHandler().Handle(new NpmPackageRequestWithMetadata(request, state)));
    }
  }
}
