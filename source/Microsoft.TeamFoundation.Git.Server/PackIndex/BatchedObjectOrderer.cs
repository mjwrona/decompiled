// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.PackIndex.BatchedObjectOrderer
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Graph;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server.PackIndex
{
  internal class BatchedObjectOrderer : IObjectOrderer
  {
    private readonly IVssRequestContext m_rc;
    private readonly OdbId m_odbId;
    private readonly Stopwatch m_sw;
    private readonly AncestralGraphAlgorithm<int, Sha1Id> m_algorithm;
    private Sha1IdLookup m_order;
    private IntSha1IdGraph m_graph;
    private int m_maxGraphSize;
    private int m_numBatches;
    private readonly Func<bool> m_batchTrigger;
    private const int c_smallBatchSize = 104857600;
    private const int c_largeBatchSize = 524288000;
    private const string c_layer = "BatchedObjectOrderer";

    public BatchedObjectOrderer(IVssRequestContext rc, OdbId odbId, bool useExtraResources = false)
      : this()
    {
      BatchedObjectOrderer batchedObjectOrderer = this;
      this.m_rc = rc;
      this.m_odbId = odbId;
      int batchSize = useExtraResources ? 524288000 : 104857600;
      this.m_batchTrigger = (Func<bool>) (() => batchedObjectOrderer.m_graph.GetSize() > (long) batchSize);
    }

    internal BatchedObjectOrderer(Func<bool> batchTrigger)
      : this()
    {
      this.m_batchTrigger = batchTrigger;
    }

    public BatchedObjectOrderer()
    {
      this.m_rc = (IVssRequestContext) null;
      this.m_odbId = new OdbId();
      this.m_order = new Sha1IdLookup();
      this.m_graph = new IntSha1IdGraph();
      this.m_algorithm = new AncestralGraphAlgorithm<int, Sha1Id>();
      this.m_sw = new Stopwatch();
    }

    public List<Sha1Id> DequeueAll()
    {
      this.AppendGraphToOrder();
      List<Sha1Id> list = this.m_order.ToList<Sha1Id>();
      IVssRequestContext rc = this.m_rc;
      if (rc != null)
        rc.TraceConditionally(1013718, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (BatchedObjectOrderer), (Func<string>) (() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Ordered ODB {0} in {1} millis. Max graph size: {2}, Order Size: {3}, Num Batches: {4}.", (object) this.m_odbId, (object) this.m_sw.ElapsedMilliseconds, (object) this.m_maxGraphSize, (object) this.m_order.GetSize(), (object) this.m_numBatches)));
      this.m_order = new Sha1IdLookup();
      this.m_numBatches = 0;
      this.m_maxGraphSize = 0;
      return list;
    }

    public void EnqueueObject(Sha1Id objectId, IEnumerable<Sha1Id> neighbors)
    {
      int index;
      if (this.m_order.TryGetIndex(objectId, out index))
        return;
      this.m_graph.AddVertex(objectId, neighbors.Where<Sha1Id>((Func<Sha1Id, bool>) (neighbor => !this.m_order.TryGetIndex(neighbor, out index))));
      Func<bool> batchTrigger = this.m_batchTrigger;
      if ((batchTrigger != null ? (batchTrigger() ? 1 : 0) : 0) == 0)
        return;
      this.AppendGraphToOrder();
    }

    private void AppendGraphToOrder()
    {
      this.m_sw.Start();
      foreach (int orderByLabel in this.m_algorithm.OrderByLabels((IDirectedGraph<int, Sha1Id>) this.m_graph, Enumerable.Range(0, this.m_graph.NumVertices)))
        this.m_order.Add(this.m_graph.GetVertex(orderByLabel));
      this.m_maxGraphSize = (int) Math.Max((long) this.m_maxGraphSize, this.m_graph.GetSize());
      ++this.m_numBatches;
      this.m_graph = new IntSha1IdGraph();
      this.m_sw.Stop();
    }

    public struct Stats
    {
      public Stats(int numBatches, long maxGraphSize, long orderSize)
      {
        this.NumBatches = numBatches;
        this.MaxGraphSize = maxGraphSize;
        this.OrderSize = orderSize;
      }

      public int NumBatches { get; }

      public long MaxGraphSize { get; }

      public long OrderSize { get; }
    }
  }
}
