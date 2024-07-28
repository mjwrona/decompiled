// Decompiled with JetBrains decompiler
// Type: Nest.ClusterRerouteDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.ClusterApi;
using System;
using System.Collections.Generic;

namespace Nest
{
  public class ClusterRerouteDescriptor : 
    RequestDescriptorBase<ClusterRerouteDescriptor, ClusterRerouteRequestParameters, IClusterRerouteRequest>,
    IClusterRerouteRequest,
    IRequest<ClusterRerouteRequestParameters>,
    IRequest
  {
    IList<IClusterRerouteCommand> IClusterRerouteRequest.Commands { get; set; } = (IList<IClusterRerouteCommand>) new List<IClusterRerouteCommand>();

    public ClusterRerouteDescriptor Move(
      Func<MoveClusterRerouteCommandDescriptor, IMoveClusterRerouteCommand> selector)
    {
      return this.AddCommand(selector != null ? (IClusterRerouteCommand) selector(new MoveClusterRerouteCommandDescriptor()) : (IClusterRerouteCommand) null);
    }

    public ClusterRerouteDescriptor Cancel(
      Func<CancelClusterRerouteCommandDescriptor, ICancelClusterRerouteCommand> selector)
    {
      return this.AddCommand(selector != null ? (IClusterRerouteCommand) selector(new CancelClusterRerouteCommandDescriptor()) : (IClusterRerouteCommand) null);
    }

    public ClusterRerouteDescriptor AllocateReplica(
      Func<AllocateReplicaClusterRerouteCommandDescriptor, IAllocateClusterRerouteCommand> selector)
    {
      return this.AddCommand(selector != null ? (IClusterRerouteCommand) selector(new AllocateReplicaClusterRerouteCommandDescriptor()) : (IClusterRerouteCommand) null);
    }

    public ClusterRerouteDescriptor AllocateEmptyPrimary(
      Func<AllocateEmptyPrimaryRerouteCommandDescriptor, IAllocateEmptyPrimaryRerouteCommand> selector)
    {
      return this.AddCommand(selector != null ? (IClusterRerouteCommand) selector(new AllocateEmptyPrimaryRerouteCommandDescriptor()) : (IClusterRerouteCommand) null);
    }

    public ClusterRerouteDescriptor AllocateStalePrimary(
      Func<AllocateStalePrimaryRerouteCommandDescriptor, IAllocateStalePrimaryRerouteCommand> selector)
    {
      return this.AddCommand(selector != null ? (IClusterRerouteCommand) selector(new AllocateStalePrimaryRerouteCommandDescriptor()) : (IClusterRerouteCommand) null);
    }

    private ClusterRerouteDescriptor AddCommand(IClusterRerouteCommand rerouteCommand) => this.Assign<IClusterRerouteCommand>(rerouteCommand, (Action<IClusterRerouteRequest, IClusterRerouteCommand>) ((a, v) =>
    {
      IList<IClusterRerouteCommand> commands = a.Commands;
      if (commands == null)
        return;
      commands.AddIfNotNull<IClusterRerouteCommand>(v);
    }));

    internal override ApiUrls ApiUrls => ApiUrlsLookups.ClusterReroute;

    public ClusterRerouteDescriptor DryRun(bool? dryrun = true) => this.Qs("dry_run", (object) dryrun);

    public ClusterRerouteDescriptor Explain(bool? explain = true) => this.Qs(nameof (explain), (object) explain);

    public ClusterRerouteDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    public ClusterRerouteDescriptor Metric(params string[] metric) => this.Qs(nameof (metric), (object) metric);

    public ClusterRerouteDescriptor RetryFailed(bool? retryfailed = true) => this.Qs("retry_failed", (object) retryfailed);

    public ClusterRerouteDescriptor Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);
  }
}
