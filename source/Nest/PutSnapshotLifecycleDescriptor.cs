// Decompiled with JetBrains decompiler
// Type: Nest.PutSnapshotLifecycleDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.SnapshotLifecycleManagementApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class PutSnapshotLifecycleDescriptor : 
    RequestDescriptorBase<PutSnapshotLifecycleDescriptor, PutSnapshotLifecycleRequestParameters, IPutSnapshotLifecycleRequest>,
    IPutSnapshotLifecycleRequest,
    IRequest<PutSnapshotLifecycleRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.SnapshotLifecycleManagementPutSnapshotLifecycle;

    public PutSnapshotLifecycleDescriptor(Id policyId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("policy_id", (IUrlParameter) policyId)))
    {
    }

    [SerializationConstructor]
    protected PutSnapshotLifecycleDescriptor()
    {
    }

    Id IPutSnapshotLifecycleRequest.PolicyId => this.Self.RouteValues.Get<Id>("policy_id");

    ISnapshotLifecycleConfig IPutSnapshotLifecycleRequest.Config { get; set; }

    string IPutSnapshotLifecycleRequest.Name { get; set; }

    string IPutSnapshotLifecycleRequest.Repository { get; set; }

    CronExpression IPutSnapshotLifecycleRequest.Schedule { get; set; }

    ISnapshotRetentionConfiguration IPutSnapshotLifecycleRequest.Retention { get; set; }

    public PutSnapshotLifecycleDescriptor Name(string name) => this.Assign<string>(name, (Action<IPutSnapshotLifecycleRequest, string>) ((a, v) => a.Name = v));

    public PutSnapshotLifecycleDescriptor Repository(string repository) => this.Assign<string>(repository, (Action<IPutSnapshotLifecycleRequest, string>) ((a, v) => a.Repository = v));

    public PutSnapshotLifecycleDescriptor Schedule(CronExpression schedule) => this.Assign<CronExpression>(schedule, (Action<IPutSnapshotLifecycleRequest, CronExpression>) ((a, v) => a.Schedule = v));

    public PutSnapshotLifecycleDescriptor Config(
      Func<SnapshotLifecycleConfigDescriptor, ISnapshotLifecycleConfig> selector)
    {
      return this.Assign<Func<SnapshotLifecycleConfigDescriptor, ISnapshotLifecycleConfig>>(selector, (Action<IPutSnapshotLifecycleRequest, Func<SnapshotLifecycleConfigDescriptor, ISnapshotLifecycleConfig>>) ((a, v) => a.Config = v.InvokeOrDefault<SnapshotLifecycleConfigDescriptor, ISnapshotLifecycleConfig>(new SnapshotLifecycleConfigDescriptor())));
    }

    public PutSnapshotLifecycleDescriptor Retention(
      Func<SnapshotRetentionConfigurationDescriptor, ISnapshotRetentionConfiguration> selector)
    {
      return this.Assign<Func<SnapshotRetentionConfigurationDescriptor, ISnapshotRetentionConfiguration>>(selector, (Action<IPutSnapshotLifecycleRequest, Func<SnapshotRetentionConfigurationDescriptor, ISnapshotRetentionConfiguration>>) ((a, v) => a.Retention = v.InvokeOrDefault<SnapshotRetentionConfigurationDescriptor, ISnapshotRetentionConfiguration>(new SnapshotRetentionConfigurationDescriptor())));
    }
  }
}
