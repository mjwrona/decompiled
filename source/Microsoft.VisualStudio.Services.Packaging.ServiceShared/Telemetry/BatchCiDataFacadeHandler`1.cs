// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry.BatchCiDataFacadeHandler`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry
{
  public class BatchCiDataFacadeHandler<TPackageId> : 
    IAsyncHandler<(PackagesBatchRequest<TPackageId> Request, BatchCommitOperationData Op), ICiData>,
    IHaveInputType<(PackagesBatchRequest<TPackageId> Request, BatchCommitOperationData Op)>,
    IHaveOutputType<ICiData>
    where TPackageId : IPackageIdentity
  {
    private readonly IVssRequestContext requestContext;

    public BatchCiDataFacadeHandler(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public Task<ICiData> Handle(
      (PackagesBatchRequest<TPackageId> Request, BatchCommitOperationData Op) input)
    {
      PackagesBatchRequest<TPackageId> request = input.Request;
      IVssRequestContext requestContext = this.requestContext;
      IProtocol protocol = request.Protocol;
      FeedCore feed = request.Feed;
      string name = request.OperationType.Name;
      BatchOperationData batchOperationData = request.BatchOperationData;
      string serializedBatchData = batchOperationData != null ? batchOperationData.Serialize<BatchOperationData>() : (string) null;
      int count = request.Requests.Count;
      return Task.FromResult<ICiData>((ICiData) new BatchOperationCiData(requestContext, protocol, feed, name, serializedBatchData, count));
    }
  }
}
