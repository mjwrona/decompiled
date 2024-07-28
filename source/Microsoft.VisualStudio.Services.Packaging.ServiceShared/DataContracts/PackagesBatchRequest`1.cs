// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts.PackagesBatchRequest`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts
{
  public class PackagesBatchRequest<TIdentity> : FeedRequest where TIdentity : IPackageIdentity
  {
    public PackagesBatchRequest(
      IFeedRequest feedRequest,
      BatchOperationData batchOperationData,
      IBatchOperationType operationType,
      IReadOnlyCollection<IPackageRequest<TIdentity>> requests)
      : base(feedRequest)
    {
      this.BatchOperationData = batchOperationData;
      this.OperationType = operationType;
      this.Requests = requests;
    }

    public BatchOperationData BatchOperationData { get; }

    public IBatchOperationType OperationType { get; }

    public IReadOnlyCollection<IPackageRequest<TIdentity>> Requests { get; }
  }
}
