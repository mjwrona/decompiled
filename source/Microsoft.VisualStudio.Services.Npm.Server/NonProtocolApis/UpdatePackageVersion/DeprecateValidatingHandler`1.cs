// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.NonProtocolApis.UpdatePackageVersion.DeprecateValidatingHandler`1
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
using Microsoft.VisualStudio.Services.Npm.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Npm.Server.NonProtocolApis.UpdatePackageVersion
{
  public class DeprecateValidatingHandler<TData> : 
    IAsyncHandler<PackageRequest<NpmPackageIdentity, TData>, ICommitOperationData>,
    IHaveInputType<PackageRequest<NpmPackageIdentity, TData>>,
    IHaveOutputType<ICommitOperationData>
    where TData : class, INpmDeprecateData
  {
    private readonly IAsyncHandler<PackageRequest<NpmPackageIdentity>, INpmMetadataEntry> metadataHandler;

    public DeprecateValidatingHandler(
      IAsyncHandler<PackageRequest<NpmPackageIdentity>, INpmMetadataEntry> metadataHandler)
    {
      this.metadataHandler = metadataHandler;
    }

    public async Task<ICommitOperationData> Handle(PackageRequest<NpmPackageIdentity, TData> request)
    {
      string deprecateMessage = request.AdditionalData.GetDeprecateMessage();
      if (deprecateMessage == null)
        return (ICommitOperationData) null;
      INpmMetadataEntry metadataEntry = await this.metadataHandler.Handle((PackageRequest<NpmPackageIdentity>) request);
      if (metadataEntry == null || metadataEntry.IsDeleted() || metadataEntry.IsPermanentlyDeleted())
        throw new PackageNotFoundException(Resources.Error_PackageVersionNotFound((object) request.PackageId.Name.FullName, (object) request.PackageId.Version.DisplayVersion, (object) request.Feed.FullyQualifiedName));
      PackageStateMachineValidator.ThrowIfUningestedUpstreamPackage<NpmPackageIdentity>((IMetadataEntry) metadataEntry, (IPackageRequest<NpmPackageIdentity>) request);
      return (ICommitOperationData) new NpmDeprecateOperationData((IPackageIdentity) request.PackageId, deprecateMessage);
    }
  }
}
