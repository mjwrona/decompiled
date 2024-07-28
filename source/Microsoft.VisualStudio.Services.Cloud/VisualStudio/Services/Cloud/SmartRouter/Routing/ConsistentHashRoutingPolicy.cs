// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SmartRouter.Routing.ConsistentHashRoutingPolicy
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.SmartRouter.Common;
using System;
using System.Collections.Generic;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cloud.SmartRouter.Routing
{
  internal class ConsistentHashRoutingPolicy : RoutingPolicyBase
  {
    public ConsistentHashRoutingPolicy()
      : base("VisualStudio.Services.Cloud.SmartRouter.RoutingPolicy.ConsistentHash.Enabled")
    {
    }

    internal ConsistentHashRoutingPolicy(
      int? minimumServerNodesPerHost,
      int? minimumServerNodesPerScaleUnit,
      int? minimumPercentServerNodesPerHost)
      : this()
    {
      this.OverrideMinimumServerNodesPerHost = minimumServerNodesPerHost;
      this.OverrideMinimumServerNodesPerScaleUnit = minimumServerNodesPerScaleUnit;
      this.OverrideMinimumPercentServerNodesPerScaleUnit = minimumPercentServerNodesPerHost;
    }

    protected override ServerNodeWithHash? OnSelectTargetServerNode(
      IVssRequestContext requestContext,
      SmartRouterContext smartRouterContext,
      IReadOnlyList<ServerNodeWithHash> servers)
    {
      int affinityPoolSize;
      if (!this.TryGetAffinityPoolSize(requestContext, smartRouterContext, servers.Count, out affinityPoolSize))
        return (ServerNodeWithHash) null;
      MinHeap<RankedServerNode> rankedServerList = this.GetRankedServerList(this.GetAffinityKey(requestContext), servers);
      ServerNodeWithHash[] serverNodeWithHashArray = new ServerNodeWithHash[affinityPoolSize];
      for (int index = 0; index < serverNodeWithHashArray.Length; ++index)
        serverNodeWithHashArray[index] = rankedServerList.Extract().ServerNode;
      int index1 = ThreadStaticRandom.Get().Next(serverNodeWithHashArray.Length);
      return serverNodeWithHashArray[index1];
    }

    private MinHeap<RankedServerNode> GetRankedServerList(
      string affinityKey,
      IReadOnlyList<ServerNodeWithHash> servers)
    {
      return this.RankedServerListProvider.Value.GetRankedServerList(affinityKey, servers);
    }

    private bool TryGetAffinityPoolSize(
      IVssRequestContext requestContext,
      SmartRouterContext smartRouterContext,
      int serverCount,
      out int affinityPoolSize)
    {
      int nodesPerScaleUnit = this.GetMinimumServerNodesPerScaleUnit(requestContext);
      if (serverCount < nodesPerScaleUnit && !requestContext.IsDebugEnvironment())
      {
        smartRouterContext.SetNotRouted("InsufficientServersForSU");
        this.Tracer.TraceVerbose(requestContext, SmartRouterBase.TracePoint.NotEnoughServers, "server count < SU mininmum, minimum={0}, available={1}", (object) nodesPerScaleUnit, (object) serverCount);
        affinityPoolSize = 0;
        return false;
      }
      affinityPoolSize = this.GetAffinityPoolSize(requestContext, serverCount);
      if (affinityPoolSize <= 0 || serverCount <= 0)
      {
        smartRouterContext.SetNotRouted("NoServersAvailable");
        this.Tracer.TraceVerbose(requestContext, SmartRouterBase.TracePoint.NotEnoughServers, "pool size <= 0, affinityPoolSize={0}, available={1}", (object) affinityPoolSize, (object) serverCount);
        return false;
      }
      if (serverCount <= affinityPoolSize)
      {
        if (!requestContext.IsDebugEnvironment())
        {
          smartRouterContext.SetNotRouted("InsufficientServersForHost");
          this.Tracer.TraceVerbose(requestContext, SmartRouterBase.TracePoint.NotEnoughServers, "pool size >= available, affinityPoolSize={0}, available={1}", (object) affinityPoolSize, (object) serverCount);
          return false;
        }
        affinityPoolSize = Math.Min(affinityPoolSize, serverCount);
      }
      this.Tracer.TraceVerbose(requestContext, SmartRouterBase.TracePoint.ConsistentHashPolicyServerCount, "affinityPoolSize={0}, available={1}", (object) affinityPoolSize, (object) serverCount);
      return true;
    }

    internal int GetAffinityPoolSize(IVssRequestContext requestContext, int serverCount)
    {
      if (!requestContext.IsMinimumPercentNodesPerHostFeatureEnabled())
        return this.GetSmartRouterMinimumServerNodesPerHost(requestContext);
      int serverNodesPerHost = this.GetSmartRouterMinimumServerNodesPerHost(requestContext);
      double num = (double) this.GetSmartRouterMinimumPercentServerNodesPerHost(requestContext) / 100.0;
      return Math.Max((int) Math.Ceiling((double) serverCount * num), serverNodesPerHost);
    }

    private int GetMinimumServerNodesPerScaleUnit(IVssRequestContext requestContext) => this.OverrideMinimumServerNodesPerScaleUnit ?? requestContext.GetSmartRouterMinimumNodesPerScaleUnitSetting();

    private int GetSmartRouterMinimumServerNodesPerHost(IVssRequestContext requestContext) => this.OverrideMinimumServerNodesPerHost ?? requestContext.GetSmartRouterMinimumNodesPerHostSetting();

    private int GetSmartRouterMinimumPercentServerNodesPerHost(IVssRequestContext requestContext) => this.OverrideMinimumPercentServerNodesPerScaleUnit ?? requestContext.GetSmartRouterMinimumPercentNodesPerHostSetting();

    private string GetAffinityKey(IVssRequestContext requestContext) => requestContext.ServiceHost.InstanceId.ToString();

    private Lazy<IRankedServerListProvider> RankedServerListProvider { get; } = new Lazy<IRankedServerListProvider>((Func<IRankedServerListProvider>) (() => (IRankedServerListProvider) new Microsoft.VisualStudio.Services.Cloud.SmartRouter.Routing.RankedServerListProvider()));

    private int? OverrideMinimumServerNodesPerHost { get; }

    private int? OverrideMinimumServerNodesPerScaleUnit { get; }

    private int? OverrideMinimumPercentServerNodesPerScaleUnit { get; }
  }
}
