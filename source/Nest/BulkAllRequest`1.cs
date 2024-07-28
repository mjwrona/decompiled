// Decompiled with JetBrains decompiler
// Type: Nest.BulkAllRequest`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Collections.Generic;

namespace Nest
{
  public class BulkAllRequest<T> : IBulkAllRequest<T>, IHelperCallable where T : class
  {
    public BulkAllRequest(IEnumerable<T> documents)
    {
      this.Documents = documents;
      this.Index = (IndexName) typeof (T);
    }

    public int? BackOffRetries { get; set; }

    public Time BackOffTime { get; set; }

    public ProducerConsumerBackPressure BackPressure { get; set; }

    public Action<BulkDescriptor, IList<T>> BufferToBulk { get; set; }

    public bool ContinueAfterDroppedDocuments { get; set; }

    public IEnumerable<T> Documents { get; }

    public Action<BulkResponseItemBase, T> DroppedDocumentCallback { get; set; }

    public IndexName Index { get; set; }

    public int? MaxDegreeOfParallelism { get; set; }

    public string Pipeline { get; set; }

    public Indices RefreshIndices { get; set; }

    public bool RefreshOnCompleted { get; set; }

    public Func<BulkResponseItemBase, T, bool> RetryDocumentPredicate { get; set; }

    public Routing Routing { get; set; }

    public int? Size { get; set; }

    public Time Timeout { get; set; }

    public int? WaitForActiveShards { get; set; }

    public Action<BulkResponse> BulkResponseCallback { get; set; }

    internal RequestMetaData ParentMetaData { get; set; }

    RequestMetaData IHelperCallable.ParentMetaData
    {
      get => this.ParentMetaData;
      set => this.ParentMetaData = value;
    }
  }
}
