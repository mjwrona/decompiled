// Decompiled with JetBrains decompiler
// Type: Nest.BulkAllDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Collections.Generic;

namespace Nest
{
  public class BulkAllDescriptor<T> : 
    DescriptorBase<BulkAllDescriptor<T>, IBulkAllRequest<T>>,
    IBulkAllRequest<T>,
    IHelperCallable
    where T : class
  {
    private readonly IEnumerable<T> _documents;

    public BulkAllDescriptor(IEnumerable<T> documents)
    {
      this._documents = documents;
      ((IBulkAllRequest<T>) this).Index = (IndexName) typeof (T);
    }

    int? IBulkAllRequest<T>.BackOffRetries { get; set; }

    Time IBulkAllRequest<T>.BackOffTime { get; set; }

    ProducerConsumerBackPressure IBulkAllRequest<T>.BackPressure { get; set; }

    Action<BulkDescriptor, IList<T>> IBulkAllRequest<T>.BufferToBulk { get; set; }

    bool IBulkAllRequest<T>.ContinueAfterDroppedDocuments { get; set; }

    IEnumerable<T> IBulkAllRequest<T>.Documents => this._documents;

    Action<BulkResponseItemBase, T> IBulkAllRequest<T>.DroppedDocumentCallback { get; set; }

    IndexName IBulkAllRequest<T>.Index { get; set; }

    int? IBulkAllRequest<T>.MaxDegreeOfParallelism { get; set; }

    string IBulkAllRequest<T>.Pipeline { get; set; }

    Indices IBulkAllRequest<T>.RefreshIndices { get; set; }

    bool IBulkAllRequest<T>.RefreshOnCompleted { get; set; }

    Func<BulkResponseItemBase, T, bool> IBulkAllRequest<T>.RetryDocumentPredicate { get; set; }

    Nest.Routing IBulkAllRequest<T>.Routing { get; set; }

    int? IBulkAllRequest<T>.Size { get; set; }

    Time IBulkAllRequest<T>.Timeout { get; set; }

    int? IBulkAllRequest<T>.WaitForActiveShards { get; set; }

    Action<BulkResponse> IBulkAllRequest<T>.BulkResponseCallback { get; set; }

    RequestMetaData IHelperCallable.ParentMetaData { get; set; }

    public BulkAllDescriptor<T> MaxDegreeOfParallelism(int? parallelism) => this.Assign<int?>(parallelism, (Action<IBulkAllRequest<T>, int?>) ((a, v) => a.MaxDegreeOfParallelism = v));

    public BulkAllDescriptor<T> Size(int? size) => this.Assign<int?>(size, (Action<IBulkAllRequest<T>, int?>) ((a, v) => a.Size = v));

    public BulkAllDescriptor<T> BackOffRetries(int? backoffs) => this.Assign<int?>(backoffs, (Action<IBulkAllRequest<T>, int?>) ((a, v) => a.BackOffRetries = v));

    public BulkAllDescriptor<T> BackOffTime(Time time) => this.Assign<Time>(time, (Action<IBulkAllRequest<T>, Time>) ((a, v) => a.BackOffTime = v));

    public BulkAllDescriptor<T> Index(IndexName index) => this.Assign<IndexName>(index, (Action<IBulkAllRequest<T>, IndexName>) ((a, v) => a.Index = v));

    public BulkAllDescriptor<T> Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IBulkAllRequest<T>, Type>) ((a, v) => a.Index = (IndexName) v));

    public BulkAllDescriptor<T> RefreshOnCompleted(bool refresh = true) => this.Assign<bool>(refresh, (Action<IBulkAllRequest<T>, bool>) ((a, v) => a.RefreshOnCompleted = v));

    public BulkAllDescriptor<T> RefreshIndices(Indices indicesToRefresh) => this.Assign<Indices>(indicesToRefresh, (Action<IBulkAllRequest<T>, Indices>) ((a, v) => a.RefreshIndices = v));

    public BulkAllDescriptor<T> Routing(Nest.Routing routing) => this.Assign<Nest.Routing>(routing, (Action<IBulkAllRequest<T>, Nest.Routing>) ((a, v) => a.Routing = v));

    public BulkAllDescriptor<T> Timeout(Time timeout) => this.Assign<Time>(timeout, (Action<IBulkAllRequest<T>, Time>) ((a, v) => a.Timeout = v));

    public BulkAllDescriptor<T> Pipeline(string pipeline) => this.Assign<string>(pipeline, (Action<IBulkAllRequest<T>, string>) ((a, v) => a.Pipeline = v));

    public BulkAllDescriptor<T> BufferToBulk(Action<BulkDescriptor, IList<T>> modifier) => this.Assign<Action<BulkDescriptor, IList<T>>>(modifier, (Action<IBulkAllRequest<T>, Action<BulkDescriptor, IList<T>>>) ((a, v) => a.BufferToBulk = v));

    public BulkAllDescriptor<T> RetryDocumentPredicate(Func<BulkResponseItemBase, T, bool> predicate) => this.Assign<Func<BulkResponseItemBase, T, bool>>(predicate, (Action<IBulkAllRequest<T>, Func<BulkResponseItemBase, T, bool>>) ((a, v) => a.RetryDocumentPredicate = v));

    public BulkAllDescriptor<T> BackPressure(int maxConcurrency, int? backPressureFactor = null) => this.Assign<ProducerConsumerBackPressure>(new ProducerConsumerBackPressure(backPressureFactor, maxConcurrency), (Action<IBulkAllRequest<T>, ProducerConsumerBackPressure>) ((a, v) => a.BackPressure = v));

    public BulkAllDescriptor<T> ContinueAfterDroppedDocuments(bool proceed = true) => this.Assign<bool>(proceed, (Action<IBulkAllRequest<T>, bool>) ((a, v) => a.ContinueAfterDroppedDocuments = v));

    public BulkAllDescriptor<T> DroppedDocumentCallback(Action<BulkResponseItemBase, T> callback) => this.Assign<Action<BulkResponseItemBase, T>>(callback, (Action<IBulkAllRequest<T>, Action<BulkResponseItemBase, T>>) ((a, v) => a.DroppedDocumentCallback = v));

    public BulkAllDescriptor<T> BulkResponseCallback(Action<BulkResponse> callback) => this.Assign<Action<BulkResponse>>(callback, (Action<IBulkAllRequest<T>, Action<BulkResponse>>) ((a, v) => a.BulkResponseCallback = v));
  }
}
