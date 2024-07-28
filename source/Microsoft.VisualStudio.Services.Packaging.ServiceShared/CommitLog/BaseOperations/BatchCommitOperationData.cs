// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations.BatchCommitOperationData
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations
{
  public class BatchCommitOperationData : IBatchCommitOperationData, ICommitOperationData
  {
    public BatchCommitOperationData(
      IReadOnlyCollection<ICommitOperationData> operations)
    {
      this.Operations = (IEnumerable<ICommitOperationData>) operations;
      if (!operations.Any<ICommitOperationData>())
        throw new ArgumentException("Batch commits must have at least one operation.");
    }

    public IEnumerable<ICommitOperationData> Operations { get; }

    public RingOrder RingOrder => this.Operations.First<ICommitOperationData>().RingOrder;

    public FeedPermissionConstants PermissionDemand => this.Operations.First<ICommitOperationData>().PermissionDemand;
  }
}
