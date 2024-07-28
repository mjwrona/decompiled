// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.UpdatePackageVersions.PackagesBatchRequestOpGeneratingHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.UpdatePackageVersions
{
  public static class PackagesBatchRequestOpGeneratingHandler
  {
    public static IAsyncHandler<PackagesBatchRequest<TPackageId>, BatchCommitOperationData> Create<TPackageId, TData, TOp>(
      IBatchOperationType batchOperationType,
      IConverter<PackagesBatchRequest<TPackageId>, PackagesBatchRequest<TPackageId, TData>> requestWithDataConverter,
      IAsyncHandler<PackageRequest<TPackageId, TData>, TOp> individualOpGeneratingHandler)
      where TPackageId : IPackageIdentity
      where TData : class
      where TOp : ICommitOperationData
    {
      return (IAsyncHandler<PackagesBatchRequest<TPackageId>, BatchCommitOperationData>) new PackagesBatchRequestOpGeneratingHandler<TPackageId, PackagesBatchRequest<TPackageId, TData>, PackageRequest<TPackageId, TData>, TOp>(batchOperationType, requestWithDataConverter, (IConverter<PackagesBatchRequestOpGeneratingHandler.PackageFromBatchRequest<TPackageId, PackagesBatchRequest<TPackageId, TData>>, PackageRequest<TPackageId, TData>>) new PackagesBatchRequestOpGeneratingHandler.SimpleIndividualRequestConverter<TPackageId, TData>(), individualOpGeneratingHandler);
    }

    public static IAsyncHandler<PackagesBatchRequest<TPackageId>, BatchCommitOperationData> Create<TPackageId, TOp>(
      IBatchOperationType batchOperationType,
      IAsyncHandler<PackageRequest<TPackageId>, TOp> individualOpGeneratingHandler)
      where TPackageId : IPackageIdentity
      where TOp : ICommitOperationData
    {
      return (IAsyncHandler<PackagesBatchRequest<TPackageId>, BatchCommitOperationData>) new PackagesBatchRequestOpGeneratingHandler<TPackageId, PackagesBatchRequest<TPackageId>, PackageRequest<TPackageId>, TOp>(batchOperationType, (IConverter<PackagesBatchRequest<TPackageId>, PackagesBatchRequest<TPackageId>>) new NoOpConverter<PackagesBatchRequest<TPackageId>>(), (IConverter<PackagesBatchRequestOpGeneratingHandler.PackageFromBatchRequest<TPackageId, PackagesBatchRequest<TPackageId>>, PackageRequest<TPackageId>>) new PackagesBatchRequestOpGeneratingHandler.SimpleIndividualRequestConverter<TPackageId>(), individualOpGeneratingHandler);
    }

    public class PackageFromBatchRequest<TPackageId, TBatchRequest>
      where TPackageId : IPackageIdentity
      where TBatchRequest : PackagesBatchRequest<TPackageId>
    {
      public TBatchRequest BatchRequest { get; }

      public TPackageId PackageId { get; }

      public PackageFromBatchRequest(TBatchRequest batchRequest, TPackageId package)
      {
        this.BatchRequest = batchRequest;
        this.PackageId = package;
      }
    }

    private class SimpleIndividualRequestConverter<TPackageId> : 
      IConverter<PackagesBatchRequestOpGeneratingHandler.PackageFromBatchRequest<TPackageId, PackagesBatchRequest<TPackageId>>, PackageRequest<TPackageId>>,
      IHaveInputType<PackagesBatchRequestOpGeneratingHandler.PackageFromBatchRequest<TPackageId, PackagesBatchRequest<TPackageId>>>,
      IHaveOutputType<PackageRequest<TPackageId>>
      where TPackageId : IPackageIdentity
    {
      public PackageRequest<TPackageId> Convert(
        PackagesBatchRequestOpGeneratingHandler.PackageFromBatchRequest<TPackageId, PackagesBatchRequest<TPackageId>> input)
      {
        return new PackageRequest<TPackageId>((IFeedRequest) input.BatchRequest, input.PackageId);
      }
    }

    private class SimpleIndividualRequestConverter<TPackageId, T> : 
      IConverter<PackagesBatchRequestOpGeneratingHandler.PackageFromBatchRequest<TPackageId, PackagesBatchRequest<TPackageId, T>>, PackageRequest<TPackageId, T>>,
      IHaveInputType<PackagesBatchRequestOpGeneratingHandler.PackageFromBatchRequest<TPackageId, PackagesBatchRequest<TPackageId, T>>>,
      IHaveOutputType<PackageRequest<TPackageId, T>>
      where TPackageId : IPackageIdentity
      where T : class
    {
      public PackageRequest<TPackageId, T> Convert(
        PackagesBatchRequestOpGeneratingHandler.PackageFromBatchRequest<TPackageId, PackagesBatchRequest<TPackageId, T>> input)
      {
        return new PackageRequest<TPackageId, T>((IPackageRequest<TPackageId>) new PackageRequest<TPackageId>((IFeedRequest) input.BatchRequest, input.PackageId), input.BatchRequest.AdditionalData);
      }
    }
  }
}
