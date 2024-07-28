// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.RestoreToFeedValidatingOpGeneratingHandler`3
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Types;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class RestoreToFeedValidatingOpGeneratingHandler<TPackageIdentity, TMetadataEntry, TOperationData> : 
    IAsyncHandler<PackageRequest<TPackageIdentity, IRecycleBinPackageVersionDetails>, TOperationData>,
    IHaveInputType<PackageRequest<TPackageIdentity, IRecycleBinPackageVersionDetails>>,
    IHaveOutputType<TOperationData>
    where TPackageIdentity : IPackageIdentity
    where TMetadataEntry : IMetadataEntry
    where TOperationData : class, IRestoreToFeedOperationData
  {
    private readonly IAsyncHandler<PackageRequest<TPackageIdentity>, TMetadataEntry> metadataHandler;
    private readonly IAsyncHandler<PackageRequest<TPackageIdentity>, TOperationData> requestToOpConverter;

    public RestoreToFeedValidatingOpGeneratingHandler(
      IAsyncHandler<PackageRequest<TPackageIdentity>, TMetadataEntry> metadataHandler,
      IAsyncHandler<PackageRequest<TPackageIdentity>, TOperationData> requestToOpHandler)
    {
      this.metadataHandler = metadataHandler;
      this.requestToOpConverter = requestToOpHandler;
    }

    public async Task<TOperationData> Handle(
      PackageRequest<TPackageIdentity, IRecycleBinPackageVersionDetails> request)
    {
      IRecycleBinPackageVersionDetails additionalData = request.AdditionalData;
      bool? deleted = additionalData.Deleted;
      if (!deleted.HasValue)
        return default (TOperationData);
      deleted = additionalData.Deleted;
      if (deleted.Value)
        throw new NotSupportedException(Resources.Error_DeletionNotSupportedOnPatch());
      int feedTransition = (int) PackageStateMachineValidator.ValidateRestoreToFeedTransition<TPackageIdentity>((IMetadataEntry) await this.metadataHandler.Handle((PackageRequest<TPackageIdentity>) request), (IPackageRequest<TPackageIdentity>) request);
      return await this.requestToOpConverter.Handle((PackageRequest<TPackageIdentity>) request);
    }
  }
}
