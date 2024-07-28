// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.UpdatePackageVersion.PromoteValidatingHandler`3
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.UpdatePackageVersion
{
  public class PromoteValidatingHandler<TIdentity, TViewIdSourceData, TMetadataEntry> : 
    IAsyncHandler<PackageRequest<TIdentity, TViewIdSourceData>, ICommitOperationData>,
    IHaveInputType<PackageRequest<TIdentity, TViewIdSourceData>>,
    IHaveOutputType<ICommitOperationData>
    where TIdentity : IPackageIdentity
    where TViewIdSourceData : class
    where TMetadataEntry : IMetadataEntry
  {
    private readonly IAsyncHandler<PackageRequest<TIdentity>, TMetadataEntry> metadataHandler;
    private readonly IConverter<PackageRequest<TIdentity, TViewIdSourceData>, IViewOperationData> requestToOpConverter;

    public PromoteValidatingHandler(
      IAsyncHandler<PackageRequest<TIdentity>, TMetadataEntry> metadataHandler,
      IConverter<PackageRequest<TIdentity, TViewIdSourceData>, IViewOperationData> requestToOpConverter)
    {
      this.metadataHandler = metadataHandler;
      this.requestToOpConverter = requestToOpConverter;
    }

    public async Task<ICommitOperationData> Handle(
      PackageRequest<TIdentity, TViewIdSourceData> request)
    {
      IViewOperationData op = this.requestToOpConverter.Convert(request);
      if (op != null)
      {
        int num = (int) PackageStateMachineValidator.ValidateViewPromotionTransition<TIdentity>((IMetadataEntry) await this.metadataHandler.Handle((PackageRequest<TIdentity>) request), (IPackageRequest<TIdentity>) request);
      }
      ICommitOperationData commitOperationData = (ICommitOperationData) op;
      op = (IViewOperationData) null;
      return commitOperationData;
    }
  }
}
