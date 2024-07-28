// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.LeaseManagement.DocumentServiceLeaseUpdaterInMemory
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.ChangeFeed.Exceptions;
using Microsoft.Azure.Cosmos.Core.Trace;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.ChangeFeed.LeaseManagement
{
  internal sealed class DocumentServiceLeaseUpdaterInMemory : DocumentServiceLeaseUpdater
  {
    private const int RetryCountOnConflict = 5;
    private readonly ConcurrentDictionary<string, DocumentServiceLease> container;

    public DocumentServiceLeaseUpdaterInMemory(
      ConcurrentDictionary<string, DocumentServiceLease> container)
    {
      this.container = container;
    }

    public override Task<DocumentServiceLease> UpdateLeaseAsync(
      DocumentServiceLease cachedLease,
      string itemId,
      PartitionKey partitionKey,
      Func<DocumentServiceLease, DocumentServiceLease> updateLease)
    {
      DocumentServiceLease documentServiceLease = cachedLease;
      for (int index = 5; index >= 0; --index)
      {
        documentServiceLease = updateLease(documentServiceLease);
        if (documentServiceLease == null)
          return (Task<DocumentServiceLease>) null;
        DocumentServiceLease comparisonValue;
        if (!this.container.TryGetValue(itemId, out comparisonValue))
          throw new LeaseLostException(documentServiceLease);
        if (this.container.TryUpdate(itemId, documentServiceLease, comparisonValue))
          return Task.FromResult<DocumentServiceLease>(documentServiceLease);
        DefaultTrace.TraceInformation("Lease with token {0} update conflict. ", (object) documentServiceLease.CurrentLeaseToken);
      }
      throw new LeaseLostException(documentServiceLease);
    }
  }
}
