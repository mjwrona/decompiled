// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.StorageLogStats.OperationAggregate
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

namespace Microsoft.VisualStudio.Services.BlobStore.Server.StorageLogStats
{
  public class OperationAggregate
  {
    public OperationAggregate(string operationType) => this.OperationType = operationType;

    public string OperationType { get; private set; }

    public long TotalTransactionCount { get; private set; }

    public double TotalDurationInMilliseconds { get; private set; }

    public void RecordOperation(double durationInMilliseconds)
    {
      ++this.TotalTransactionCount;
      this.TotalDurationInMilliseconds += durationInMilliseconds;
    }
  }
}
