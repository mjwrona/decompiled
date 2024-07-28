// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.UpdatePackageVersions.ThrowWithBadOperationMessageHandler`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.UpdatePackageVersions
{
  public class ThrowWithBadOperationMessageHandler<TPackageId> : 
    IAsyncHandler<PackagesBatchRequest<TPackageId>, BatchCommitOperationData>,
    IHaveInputType<PackagesBatchRequest<TPackageId>>,
    IHaveOutputType<BatchCommitOperationData>
    where TPackageId : IPackageIdentity
  {
    public Task<BatchCommitOperationData> Handle(PackagesBatchRequest<TPackageId> request) => throw new InvalidUserRequestException(Resources.Error_InvalidBatchOperation((object) request.OperationType.ToString()));
  }
}
