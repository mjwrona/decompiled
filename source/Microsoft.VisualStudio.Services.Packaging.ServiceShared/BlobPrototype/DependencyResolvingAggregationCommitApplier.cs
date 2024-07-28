// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.DependencyResolvingAggregationCommitApplier
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class DependencyResolvingAggregationCommitApplier : IAggregationCommitApplier
  {
    private readonly IGraphBuilder<IAggregationAccessor> dependencyGraphBuilder;
    private readonly IGraphReverser<IAggregationAccessor> graphReverser;
    private readonly ITracerService tracer;
    private readonly IPackagingTraces packagingTraces;
    private readonly bool isOpportunisticApplyInAppTier;

    public DependencyResolvingAggregationCommitApplier(
      IGraphBuilder<IAggregationAccessor> dependencyGraphBuilder,
      IGraphReverser<IAggregationAccessor> graphReverser,
      ITracerService tracer,
      IPackagingTraces packagingTraces,
      bool isOpportunisticApplyInAppTier)
    {
      this.graphReverser = graphReverser;
      this.tracer = tracer;
      this.packagingTraces = packagingTraces;
      this.isOpportunisticApplyInAppTier = isOpportunisticApplyInAppTier;
      this.dependencyGraphBuilder = dependencyGraphBuilder;
    }

    public async Task<AggregationApplyTimings> ApplyCommitAsync(
      IReadOnlyList<IAggregationAccessor> aggregationAccessors,
      IFeedRequest feedRequest,
      IReadOnlyList<ICommitLogEntry> commitLogEntries)
    {
      DependencyResolvingAggregationCommitApplier sendInTheThisObject = this;
      AggregationApplyTimings aggregationApplyTimings;
      using (ITracerBlock tracerBlock = sendInTheThisObject.tracer.Enter((object) sendInTheThisObject, nameof (ApplyCommitAsync)))
      {
        if (commitLogEntries.Count != 1)
          throw new Exception("dependency resolver does not support more than one commit");
        IEnumerable<GraphNode<IAggregationAccessor>> dependencyGraph = sendInTheThisObject.dependencyGraphBuilder.Build((IEnumerable<IAggregationAccessor>) aggregationAccessors);
        ICommitLogEntry commitLogEntry = commitLogEntries.Single<ICommitLogEntry>();
        tracerBlock.TraceInfo(sendInTheThisObject.GetTraceLineHeader(feedRequest.Feed, commitLogEntry) + "Accessors to Process: " + string.Join(", ", aggregationAccessors.Select<IAggregationAccessor, string>(new Func<IAggregationAccessor, string>(sendInTheThisObject.GetPrintableAggregationVersion))));
        aggregationApplyTimings = await ProcessGraphBasedOnRingOrder(commitLogEntry, dependencyGraph);
      }
      return aggregationApplyTimings;

      async Task<AggregationApplyTimings> ProcessGraphBasedOnRingOrder(
        ICommitLogEntry commitLogEntry,
        IEnumerable<GraphNode<IAggregationAccessor>> dependencyGraph)
      {
        using (this.tracer.Enter((object) this, "ApplyCommitAsync"))
        {
          switch (commitLogEntry.CommitOperationData.RingOrder)
          {
            case RingOrder.InnerToOuter:
              return await TierBasedApplyCommitAsync(dependencyGraph, commitLogEntry);
            case RingOrder.OuterToInner:
              return await TierBasedApplyCommitAsync(this.graphReverser.Reverse(dependencyGraph, new Func<IAggregationAccessor, IAggregationAccessor, bool>(this.ShouldReverseTheEdgeForOuterToInnerRingOrder)), commitLogEntry);
            default:
              throw new ArgumentException(string.Format("Unrecognized ring order: {0}", (object) commitLogEntry.CommitOperationData.RingOrder));
          }
        }
      }
      Func<GraphNode<IAggregationAccessor>, string> func;

      async Task<AggregationApplyTimings> TierBasedApplyCommitAsync(
        IEnumerable<GraphNode<IAggregationAccessor>> dependencyGraph,
        ICommitLogEntry commitLogEntry)
      {
        AggregationApplyTimings aggregationApplyTimings;
        using (ITracerBlock tracerBlock = this.tracer.Enter((object) this, "ApplyCommitAsync"))
        {
          AggregationApplyTimings timings = new AggregationApplyTimings();
          Stopwatch stopwatch = new Stopwatch();
          HashSet<GraphNode<IAggregationAccessor>> processedNodes = new HashSet<GraphNode<IAggregationAccessor>>();
          if (!(dependencyGraph is IList<GraphNode<IAggregationAccessor>> graphNodeList))
            graphNodeList = (IList<GraphNode<IAggregationAccessor>>) dependencyGraph.ToList<GraphNode<IAggregationAccessor>>();
          IList<GraphNode<IAggregationAccessor>> graphNodes = graphNodeList;
          for (List<GraphNode<IAggregationAccessor>> list = this.GetNextTierNodes((IEnumerable<GraphNode<IAggregationAccessor>>) graphNodes, processedNodes).ToList<GraphNode<IAggregationAccessor>>(); list.Any<GraphNode<IAggregationAccessor>>(); list = this.GetNextTierNodes((IEnumerable<GraphNode<IAggregationAccessor>>) graphNodes, processedNodes).ToList<GraphNode<IAggregationAccessor>>())
          {
            tracerBlock.TraceInfo(this.GetTraceLineHeader(feedRequest.Feed, commitLogEntry) + string.Format("Processing tier nodes (RingOrder = {0}): ", (object) commitLogEntry.CommitOperationData.RingOrder) + string.Join(",", list.Select<GraphNode<IAggregationAccessor>, string>(func ?? (func = (Func<GraphNode<IAggregationAccessor>, string>) (x => this.GetPrintableAggregationVersion(x.Data))))));
            foreach (GraphNode<IAggregationAccessor> node in list)
            {
              stopwatch.Restart();
              AggregationDefinition definition = node.Data.Aggregation.Definition;
              bool aggregationIsOptional = this.isOpportunisticApplyInAppTier && definition.IsOptionalForOpportunisticAppTierApply;
              try
              {
                await node.Data.ApplyCommitAsync(feedRequest, (IReadOnlyList<ICommitLogEntry>) new ICommitLogEntry[1]
                {
                  commitLogEntry
                });
              }
              catch (Exception ex) when (aggregationIsOptional)
              {
                tracerBlock.TraceException(ex);
                this.packagingTraces.Increment("OptionalAggApplyFailures." + this.GetPrintableAggregationVersion(node.Data));
              }
              finally
              {
                timings.AddTime(this.GetPrintableAggregationVersion(node.Data), stopwatch.ElapsedMilliseconds);
                processedNodes.Add(node);
              }
            }
          }
          aggregationApplyTimings = timings;
        }
        return aggregationApplyTimings;
      }
    }

    private bool ShouldReverseTheEdgeForOuterToInnerRingOrder(
      IAggregationAccessor source,
      IAggregationAccessor target)
    {
      return !(source.Aggregation.Definition is CacheAggregationDefinition) && !(target.Aggregation.Definition is CacheAggregationDefinition);
    }

    private string GetPrintableAggregationVersion(IAggregationAccessor aggAccessor)
    {
      IAggregation aggregation = aggAccessor.Aggregation;
      return aggregation.Definition.Name + "." + aggregation.VersionName;
    }

    private string GetTraceLineHeader(FeedCore feed, ICommitLogEntry commitLogEntry) => string.Format("[f: {0}][c: {1}] ", (object) feed.Id, (object) commitLogEntry.CommitId);

    private IEnumerable<GraphNode<IAggregationAccessor>> GetNextTierNodes(
      IEnumerable<GraphNode<IAggregationAccessor>> graph,
      HashSet<GraphNode<IAggregationAccessor>> processedNodes)
    {
      using (this.tracer.Enter((object) this, nameof (GetNextTierNodes)))
        return (IEnumerable<GraphNode<IAggregationAccessor>>) graph.Where<GraphNode<IAggregationAccessor>>((Func<GraphNode<IAggregationAccessor>, bool>) (node => !processedNodes.Contains(node) && node.Edges.All<GraphNode<IAggregationAccessor>>(new Func<GraphNode<IAggregationAccessor>, bool>(processedNodes.Contains)))).ToList<GraphNode<IAggregationAccessor>>();
    }
  }
}
