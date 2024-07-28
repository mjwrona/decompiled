// Decompiled with JetBrains decompiler
// Type: Nest.IBulkAllRequest`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public interface IBulkAllRequest<T> where T : class
  {
    int? BackOffRetries { get; set; }

    Time BackOffTime { get; set; }

    ProducerConsumerBackPressure BackPressure { get; set; }

    Action<BulkDescriptor, IList<T>> BufferToBulk { get; set; }

    bool ContinueAfterDroppedDocuments { get; set; }

    IEnumerable<T> Documents { get; }

    Action<BulkResponseItemBase, T> DroppedDocumentCallback { get; set; }

    IndexName Index { get; set; }

    int? MaxDegreeOfParallelism { get; set; }

    string Pipeline { get; set; }

    Indices RefreshIndices { get; set; }

    bool RefreshOnCompleted { get; set; }

    Func<BulkResponseItemBase, T, bool> RetryDocumentPredicate { get; set; }

    Routing Routing { get; set; }

    int? Size { get; set; }

    Time Timeout { get; set; }

    int? WaitForActiveShards { get; set; }

    Action<BulkResponse> BulkResponseCallback { get; set; }
  }
}
