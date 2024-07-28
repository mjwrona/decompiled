// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.UpdatePackageVersions.PackagesBatchRequestOpGeneratingHandler`4
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.UpdatePackageVersions
{
  public class PackagesBatchRequestOpGeneratingHandler<TPackageId, TBatchRequest, TIndividualRequest, TOp> : 
    IAsyncHandler<PackagesBatchRequest<TPackageId>, BatchCommitOperationData>,
    IHaveInputType<PackagesBatchRequest<TPackageId>>,
    IHaveOutputType<BatchCommitOperationData>
    where TPackageId : IPackageIdentity
    where TBatchRequest : PackagesBatchRequest<TPackageId>
    where TIndividualRequest : IPackageRequest<TPackageId>
    where TOp : ICommitOperationData
  {
    private readonly IBatchOperationType batchOperationType;
    private readonly IConverter<PackagesBatchRequest<TPackageId>, TBatchRequest> batchRequestConverter;
    private readonly IConverter<PackagesBatchRequestOpGeneratingHandler.PackageFromBatchRequest<TPackageId, TBatchRequest>, TIndividualRequest> individualRequestConverter;
    private readonly IAsyncHandler<TIndividualRequest, TOp> individualOpGeneratingHandler;

    public PackagesBatchRequestOpGeneratingHandler(
      IBatchOperationType batchOperationType,
      IConverter<PackagesBatchRequest<TPackageId>, TBatchRequest> batchRequestConverter,
      IConverter<PackagesBatchRequestOpGeneratingHandler.PackageFromBatchRequest<TPackageId, TBatchRequest>, TIndividualRequest> individualRequestConverter,
      IAsyncHandler<TIndividualRequest, TOp> individualOpGeneratingHandler)
    {
      this.batchOperationType = batchOperationType;
      this.batchRequestConverter = batchRequestConverter;
      this.individualRequestConverter = individualRequestConverter;
      this.individualOpGeneratingHandler = individualOpGeneratingHandler;
    }

    public async Task<BatchCommitOperationData> Handle(PackagesBatchRequest<TPackageId> request)
    {
      if (request.OperationType != this.batchOperationType)
        return (BatchCommitOperationData) null;
      TBatchRequest batchRequest = this.batchRequestConverter.Convert(request);
      if ((object) batchRequest == null)
        return (BatchCommitOperationData) null;
      List<ICommitOperationData> operations = await this.GetOperations(batchRequest);
      if (!operations.Any<ICommitOperationData>())
        return (BatchCommitOperationData) null;
      ICommitOperationData firstOp = operations.First<ICommitOperationData>();
      if (firstOp == null || operations.Any<ICommitOperationData>((Func<ICommitOperationData, bool>) (op => op == null)))
        throw new InvalidCommitOperationDataException();
      if (operations.Any<ICommitOperationData>((Func<ICommitOperationData, bool>) (x => x.RingOrder != firstOp.RingOrder || x.PermissionDemand != firstOp.PermissionDemand)))
        throw new InvalidOperationException("RingOrder and PermissionDemand must be the same for all commits in a batch.");
      return new BatchCommitOperationData((IReadOnlyCollection<ICommitOperationData>) operations);
    }

    private async Task<List<ICommitOperationData>> GetOperations(TBatchRequest batchRequest)
    {
      List<ICommitOperationData> ops = new List<ICommitOperationData>();
      // ISSUE: reference to a compiler-generated field
      foreach (TIndividualRequest request in batchRequest.Requests.Select<IPackageRequest<TPackageId>, TIndividualRequest>((Func<IPackageRequest<TPackageId>, TIndividualRequest>) (r => this.\u003C\u003E4__this.individualRequestConverter.Convert(new PackagesBatchRequestOpGeneratingHandler.PackageFromBatchRequest<TPackageId, TBatchRequest>(batchRequest, r.PackageId)))))
      {
        List<ICommitOperationData> commitOperationDataList = ops;
        commitOperationDataList.Add((ICommitOperationData) await this.individualOpGeneratingHandler.Handle(request));
        commitOperationDataList = (List<ICommitOperationData>) null;
      }
      List<ICommitOperationData> operations = ops;
      ops = (List<ICommitOperationData>) null;
      return operations;
    }
  }
}
