// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Elastic.PoolState
// Assembly: Microsoft.TeamFoundation.DistributedTask.Elastic, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6202E83A-3164-4101-8FDA-8C4FB25E62EC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Elastic.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Elastic
{
  public class PoolState
  {
    private int m_successfullyCreatedNodes;
    private bool m_isElasticPoolHealthy;
    private int m_requestsQueued;
    private int m_reimagingNodes;
    private int m_failedToCreateNodes;
    private int m_unhealthyNodes;

    public bool IsValid { get; private set; }

    public bool IsScalesetStable { get; private set; }

    public bool WasSizeUpSuccessful { get; private set; }

    public int ComputeSize { get; private set; }

    public int DesiredSize { get; private set; }

    public bool IsSizeUpJustDone { get; private set; }

    public bool IsReimageOngoing { get; private set; }

    public List<ElasticNode> DeleteList { get; private set; }

    public List<ElasticNode> ReimageList { get; private set; }

    public List<ElasticNode> IdleList { get; private set; }

    public List<ElasticNode> LongIdleList { get; private set; }

    public List<ElasticNode> SavedList { get; private set; }

    public List<ElasticNode> StuckList { get; private set; }

    public List<ElasticNode> RetryList { get; private set; }

    public List<ElasticNode> UnhealthyVmList { get; private set; }

    public static async Task<PoolState> GetStateAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      StringBuilder sb)
    {
      PoolState poolState = new PoolState();
      await poolState.PopulateAsync(requestContext, elasticPool, sb);
      PoolState stateAsync = poolState;
      poolState = (PoolState) null;
      return stateAsync;
    }

    private PoolState()
    {
      this.m_successfullyCreatedNodes = 0;
      this.m_reimagingNodes = 0;
      this.m_failedToCreateNodes = 0;
      this.m_unhealthyNodes = 0;
      this.IsScalesetStable = true;
      this.IsReimageOngoing = false;
      this.DeleteList = new List<ElasticNode>();
      this.ReimageList = new List<ElasticNode>();
      this.IdleList = new List<ElasticNode>();
      this.LongIdleList = new List<ElasticNode>();
      this.SavedList = new List<ElasticNode>();
      this.StuckList = new List<ElasticNode>();
      this.RetryList = new List<ElasticNode>();
      this.UnhealthyVmList = new List<ElasticNode>();
    }

    private async Task PopulateAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      StringBuilder sb)
    {
      try
      {
        NodeObserver nodesAsync = await NodeObserver.GetNodesAsync(requestContext, elasticPool, sb);
        this.IsValid = nodesAsync.IsValid;
        if (!this.IsValid)
        {
          sb.AppendLine("Scaleset is unreachable. Skipping.");
        }
        else
        {
          this.IsSizeUpJustDone = nodesAsync.IsSizeUpJustDone;
          this.ComputeSize = nodesAsync.ComputeSize;
          this.m_requestsQueued = nodesAsync.QueuedRequestCount;
          this.AnalyzeNodes(requestContext, elasticPool, nodesAsync, sb);
          this.DeterminePoolHealth(requestContext, elasticPool, sb);
          this.DetermineSizing(requestContext, elasticPool, nodesAsync, sb);
          sb.AppendLine(string.Format("IsScalesetStable:{0}, IsHealthy:{1}, WasSizeUpSuccessful:{2}, ComputeSize:{3}, DesiredSize:{4}, Idle:{5}, LongIdle:{6}, Delete:{7}, Reimage:{8}, Saved:{9}, IsSizeUpJustDone:{10}, IsReimageOngoing {11}", (object) this.IsScalesetStable, (object) this.m_isElasticPoolHealthy, (object) this.WasSizeUpSuccessful, (object) this.ComputeSize, (object) this.DesiredSize, (object) (this.IdleList.Count + this.LongIdleList.Count), (object) this.LongIdleList.Count, (object) this.DeleteList.Count, (object) this.ReimageList.Count, (object) this.SavedList.Count, (object) this.IsSizeUpJustDone, (object) this.IsReimageOngoing));
          await this.UpdateElasticPoolAsync(requestContext, elasticPool, sb);
        }
      }
      catch (Exception ex)
      {
        this.IsValid = false;
        requestContext.TraceException(0, "DistributedTask", "ElasticPoolSizingJob", ex);
        sb.AppendLine(ex.ToString());
      }
    }

    private void AnalyzeNodes(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      NodeObserver nodes,
      StringBuilder sb)
    {
      foreach (ElasticNode elasticNode in nodes.AllNodes.Values)
      {
        if ((elasticPool.OrchestrationType == OrchestrationType.Flexible ? (elasticNode.IsFlexibleNodeStable() ? 1 : 0) : (elasticNode.IsUniformNodeStable() ? 1 : 0)) == 0)
          this.IsScalesetStable = false;
        if (elasticNode.WasSizeUpSuccessful())
          ++this.m_successfullyCreatedNodes;
        if (elasticNode.State == ElasticNodeState.ReimagingCompute || elasticNode.State == ElasticNodeState.RestartingAgent || elasticNode.State == ElasticNodeState.AssignedPendingDelete && elasticNode.ComputeState == ElasticComputeState.Reimaging)
        {
          ++this.m_reimagingNodes;
          this.IsReimageOngoing = true;
        }
        if (elasticNode.State == ElasticNodeState.FailedVMPendingDelete || elasticNode.State == ElasticNodeState.FailedToStartPendingDelete || elasticNode.State == ElasticNodeState.AssignedPendingDelete && elasticNode.ComputeState == ElasticComputeState.Creating)
          ++this.m_failedToCreateNodes;
        if (elasticNode.State == ElasticNodeState.UnhealthyVm || elasticNode.State == ElasticNodeState.UnhealthyVmPendingDelete)
          ++this.m_unhealthyNodes;
        switch (elasticNode.State)
        {
          case ElasticNodeState.Idle:
            if ((DateTime.UtcNow - elasticNode.StateChangedOn).TotalMinutes >= (double) elasticPool.TimeToLiveMinutes)
            {
              this.LongIdleList.Add(elasticNode);
              continue;
            }
            this.IdleList.Add(elasticNode);
            continue;
          case ElasticNodeState.PendingReimage:
            this.ReimageList.Add(elasticNode);
            continue;
          case ElasticNodeState.PendingDelete:
          case ElasticNodeState.FailedVMPendingDelete:
            if (requestContext.IsFeatureEnabled("DistributedTask.ElasticPoolDeleteStuckVM") && (elasticNode.ComputeState == ElasticComputeState.Creating || elasticNode.ComputeState == ElasticComputeState.Reimaging))
              this.StuckList.Add(elasticNode);
            this.DeleteList.Add(elasticNode);
            continue;
          case ElasticNodeState.Saved:
            this.SavedList.Add(elasticNode);
            continue;
          case ElasticNodeState.FailedToStartPendingDelete:
          case ElasticNodeState.FailedToRestartPendingDelete:
            this.DeleteList.Add(elasticNode);
            continue;
          case ElasticNodeState.RetryDelete:
            this.RetryList.Add(elasticNode);
            this.DeleteList.Add(elasticNode);
            continue;
          case ElasticNodeState.UnhealthyVmPendingDelete:
            this.UnhealthyVmList.Add(elasticNode);
            continue;
          default:
            continue;
        }
      }
    }

    private void DeterminePoolHealth(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      StringBuilder sb)
    {
      this.m_isElasticPoolHealthy = elasticPool.State == ElasticPoolState.Online;
      if (this.IsScalesetStable)
      {
        int num = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/DistributedTask/ElasticPool/AttemptsBeforeBackoff", 2);
        if (this.m_successfullyCreatedNodes >= elasticPool.DesiredSize)
        {
          this.WasSizeUpSuccessful = true;
          this.m_isElasticPoolHealthy = true;
        }
        else if (this.IsSizeUpJustDone && this.m_failedToCreateNodes == 0 && elasticPool.SizingAttempts >= num)
        {
          this.WasSizeUpSuccessful = true;
          this.m_isElasticPoolHealthy = false;
        }
        else if (elasticPool.SizingAttempts >= num)
        {
          this.m_isElasticPoolHealthy = false;
          this.WasSizeUpSuccessful = false;
        }
        else if (this.StuckList.Count > 0 && elasticPool.OrchestrationType != OrchestrationType.Flexible && requestContext.IsFeatureEnabled("DistributedTask.ElasticPoolDeleteStuckVM"))
        {
          this.m_isElasticPoolHealthy = false;
        }
        else
        {
          if (this.RetryList.Count <= 0 || elasticPool.OrchestrationType == OrchestrationType.Flexible || !requestContext.IsFeatureEnabled("DistributedTask.ElasticPoolRetryDelete"))
            return;
          this.m_isElasticPoolHealthy = false;
        }
      }
      else
      {
        if (!this.IsSizeUpJustDone || this.m_reimagingNodes <= 0 || this.m_failedToCreateNodes != 0)
          return;
        sb.AppendLine("Size up succeeded but some VMs are still reimaging.");
        this.WasSizeUpSuccessful = true;
        this.m_isElasticPoolHealthy = true;
      }
    }

    private void DetermineSizing(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      NodeObserver nodes,
      StringBuilder sb)
    {
      this.DesiredSize = elasticPool.DesiredSize;
      int num1 = this.IdleList.Count + this.LongIdleList.Count;
      if (elasticPool.OrchestrationType == OrchestrationType.Flexible)
      {
        int num2 = nodes.AllNodes.Values.Count<ElasticNode>((Func<ElasticNode, bool>) (x => x.State == ElasticNodeState.CreatingCompute || x.State == ElasticNodeState.StartingAgent || x.State == ElasticNodeState.RestartingAgent));
        int num3 = num1 + num2;
        IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
        int num4 = service.GetValue<int>(requestContext, (RegistryQuery) "/Service/DistributedTask/ElasticPool/AttemptsBeforeBackoff", 2);
        if (this.ComputeSize < elasticPool.MaxCapacity && (num3 < elasticPool.DesiredIdle || num3 < this.m_requestsQueued && this.m_requestsQueued > 0) && this.m_isElasticPoolHealthy && elasticPool.SizingAttempts < num4)
        {
          int val2 = elasticPool.DesiredIdle - num3;
          int val1;
          if (this.m_requestsQueued < elasticPool.DesiredIdle || this.m_requestsQueued < num3)
          {
            val1 = val2;
          }
          else
          {
            int num5 = elasticPool.RecycleAfterEachUse ? service.GetValue<int>(requestContext, (RegistryQuery) "/Service/DistributedTask/ElasticPool/SingleUseScaleUpPercentageFlexible", 25) : service.GetValue<int>(requestContext, (RegistryQuery) "/Service/DistributedTask/ElasticPool/ScaleUpPercentageFlexible", 20);
            val1 = Math.Max((int) Math.Ceiling((double) elasticPool.MaxCapacity * (double) num5 / 100.0), val2);
          }
          if (!this.m_isElasticPoolHealthy)
          {
            int num6 = Math.Max(1, elasticPool.SizingAttempts + 1 - num4);
            val1 = Math.Max(1, val1 / (2 * num6));
          }
          this.DesiredSize = this.ComputeSize + Math.Min(val1, elasticPool.MaxCapacity - this.ComputeSize);
        }
        else
          this.DecreaseSize(requestContext, elasticPool);
      }
      else
      {
        if (!this.IsScalesetStable)
          return;
        if (this.ComputeSize < elasticPool.MaxCapacity && (num1 < elasticPool.DesiredIdle || num1 == 0 && this.m_requestsQueued > 0))
          this.IncreaseSize(requestContext, elasticPool);
        else
          this.DecreaseSize(requestContext, elasticPool);
      }
    }

    private void IncreaseSize(IVssRequestContext requestContext, ElasticPool elasticPool)
    {
      if (!this.m_isElasticPoolHealthy && this.DeleteList.Count != 0)
        return;
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      int num1 = !elasticPool.RecycleAfterEachUse ? service.GetValue<int>(requestContext, (RegistryQuery) "/Service/DistributedTask/ElasticPool/ScaleUpPercentage", 25) : service.GetValue<int>(requestContext, (RegistryQuery) "/Service/DistributedTask/ElasticPool/SingleUseScaleUpPercentage", 35);
      int val1 = Math.Max((int) Math.Ceiling((double) elasticPool.MaxCapacity * (double) num1 / 100.0), elasticPool.DesiredIdle - (this.IdleList.Count + this.LongIdleList.Count));
      if (!this.m_isElasticPoolHealthy)
      {
        int num2 = service.GetValue<int>(requestContext, (RegistryQuery) "/Service/DistributedTask/ElasticPool/AttemptsBeforeBackoff", 2);
        int num3 = Math.Max(1, elasticPool.SizingAttempts + 1 - num2);
        val1 = Math.Max(1, val1 / (2 * num3));
      }
      this.DesiredSize = this.ComputeSize + Math.Min(val1, elasticPool.MaxCapacity - this.ComputeSize);
    }

    private void DecreaseSize(IVssRequestContext requestContext, ElasticPool elasticPool)
    {
      this.DesiredSize = this.ComputeSize;
      if (this.IsScalesetStable)
      {
        this.WasSizeUpSuccessful = true;
        this.m_isElasticPoolHealthy = true;
      }
      int num = Math.Min(Math.Max(Math.Max(0, this.LongIdleList.Count - elasticPool.DesiredIdle), this.ComputeSize - elasticPool.MaxCapacity), this.IdleList.Count + this.LongIdleList.Count);
      if (num <= 0)
        return;
      this.DesiredSize -= num;
    }

    private async Task UpdateElasticPoolAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      StringBuilder sb)
    {
      bool flag = false;
      NullableElasticPool elasticPool1 = new NullableElasticPool(elasticPool.PoolId);
      if (elasticPool.State == ElasticPoolState.Offline)
      {
        elasticPool1.State = new ElasticPoolState?(ElasticPoolState.Online);
        elasticPool1.OfflineSince = new DateTime?();
        flag = true;
        sb.AppendLine("Setting pool state from Offline to Online");
      }
      else if (elasticPool.State == ElasticPoolState.Online && !this.m_isElasticPoolHealthy)
      {
        elasticPool1.State = new ElasticPoolState?(ElasticPoolState.Unhealthy);
        flag = true;
        sb.AppendLine("Setting pool state from Online to Unhealthy");
      }
      else if (elasticPool.State == ElasticPoolState.Unhealthy && this.WasSizeUpSuccessful)
      {
        elasticPool1.State = new ElasticPoolState?(ElasticPoolState.Online);
        flag = true;
        sb.AppendLine("Setting pool state from Unhealthy to Online");
      }
      if (!this.IsScalesetStable && this.IsSizeUpJustDone && this.m_reimagingNodes > 0 && this.m_failedToCreateNodes == 0)
      {
        elasticPool1.SizingAttempts = new int?(0);
        flag = true;
        sb.AppendLine("Setting pool sizing attempt to 0");
      }
      if (!flag)
        return;
      ElasticPool elasticPool2 = await requestContext.GetService<IElasticPoolService>().UpdateElasticPoolAsync(requestContext, elasticPool1);
    }
  }
}
