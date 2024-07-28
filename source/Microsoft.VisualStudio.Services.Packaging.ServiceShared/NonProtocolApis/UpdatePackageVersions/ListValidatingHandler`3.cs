// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.UpdatePackageVersions.ListValidatingHandler`3
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.UpdatePackageVersions
{
  public class ListValidatingHandler<TListData, TIdentity, TMetadataEntry> : 
    IAsyncHandler<IPackageRequest<TIdentity, TListData>, IListingStateChangeOperationData>,
    IHaveInputType<IPackageRequest<TIdentity, TListData>>,
    IHaveOutputType<IListingStateChangeOperationData>
    where TListData : class
    where TIdentity : IPackageIdentity
    where TMetadataEntry : IMetadataEntry
  {
    private readonly IAsyncHandler<IPackageRequest<TIdentity>, TMetadataEntry> metadataHandler;
    private readonly IConverter<IPackageRequest<TIdentity, TListData>, IListingStateChangeOperationData> listRequestToOpConverter;

    public ListValidatingHandler(
      IAsyncHandler<IPackageRequest<TIdentity>, TMetadataEntry> metadataHandler,
      IConverter<IPackageRequest<TIdentity, TListData>, IListingStateChangeOperationData> listRequestToOpConverter)
    {
      this.metadataHandler = metadataHandler;
      this.listRequestToOpConverter = listRequestToOpConverter;
    }

    public async Task<IListingStateChangeOperationData> Handle(
      IPackageRequest<TIdentity, TListData> request)
    {
      IListingStateChangeOperationData op = this.listRequestToOpConverter.Convert(request);
      if (op != null)
      {
        TMetadataEntry metadataEntry = await this.metadataHandler.Handle((IPackageRequest<TIdentity>) request);
        if ((object) metadataEntry == null || metadataEntry.IsDeleted() || metadataEntry.IsPermanentlyDeleted())
          throw ExceptionHelper.PackageNotFound((IPackageIdentity) request.PackageId, request.Feed);
        PackageStateMachineValidator.ThrowIfUningestedUpstreamPackage<TIdentity>((IMetadataEntry) metadataEntry, (IPackageRequest<TIdentity>) request);
      }
      IListingStateChangeOperationData changeOperationData = op;
      op = (IListingStateChangeOperationData) null;
      return changeOperationData;
    }
  }
}
