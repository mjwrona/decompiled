// Decompiled with JetBrains decompiler
// Type: Nest.CreateFollowIndexDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.CrossClusterReplicationApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class CreateFollowIndexDescriptor : 
    RequestDescriptorBase<CreateFollowIndexDescriptor, CreateFollowIndexRequestParameters, ICreateFollowIndexRequest>,
    ICreateFollowIndexRequest,
    IRequest<CreateFollowIndexRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.CrossClusterReplicationCreateFollowIndex;

    public CreateFollowIndexDescriptor(IndexName index)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index)))
    {
    }

    [SerializationConstructor]
    protected CreateFollowIndexDescriptor()
    {
    }

    IndexName ICreateFollowIndexRequest.Index => this.Self.RouteValues.Get<IndexName>("index");

    public CreateFollowIndexDescriptor Index(IndexName index) => this.Assign<IndexName>(index, (Action<ICreateFollowIndexRequest, IndexName>) ((a, v) => a.RouteValues.Required(nameof (index), (IUrlParameter) v)));

    public CreateFollowIndexDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<ICreateFollowIndexRequest, Type>) ((a, v) => a.RouteValues.Required("index", (IUrlParameter) (IndexName) v)));

    public CreateFollowIndexDescriptor WaitForActiveShards(string waitforactiveshards) => this.Qs("wait_for_active_shards", (object) waitforactiveshards);

    string ICreateFollowIndexRequest.RemoteCluster { get; set; }

    IndexName ICreateFollowIndexRequest.LeaderIndex { get; set; }

    IIndexSettings ICreateFollowIndexRequest.Settings { get; set; }

    long? ICreateFollowIndexRequest.MaxReadRequestOperationCount { get; set; }

    long? ICreateFollowIndexRequest.MaxOutstandingReadRequests { get; set; }

    string ICreateFollowIndexRequest.MaxRequestSize { get; set; }

    long? ICreateFollowIndexRequest.MaxWriteRequestOperationCount { get; set; }

    string ICreateFollowIndexRequest.MaxWriteRequestSize { get; set; }

    long? ICreateFollowIndexRequest.MaxOutstandingWriteRequests { get; set; }

    long? ICreateFollowIndexRequest.MaxWriteBufferCount { get; set; }

    string ICreateFollowIndexRequest.MaxWriteBufferSize { get; set; }

    Time ICreateFollowIndexRequest.MaxRetryDelay { get; set; }

    Time ICreateFollowIndexRequest.ReadPollTimeout { get; set; }

    public CreateFollowIndexDescriptor RemoteCluster(string remoteCluster) => this.Assign<string>(remoteCluster, (Action<ICreateFollowIndexRequest, string>) ((a, v) => a.RemoteCluster = v));

    public CreateFollowIndexDescriptor LeaderIndex(IndexName index) => this.Assign<IndexName>(index, (Action<ICreateFollowIndexRequest, IndexName>) ((a, v) => a.LeaderIndex = v));

    public CreateFollowIndexDescriptor Settings(
      Func<IndexSettingsDescriptor, IPromise<IIndexSettings>> selector)
    {
      return this.Assign<Func<IndexSettingsDescriptor, IPromise<IIndexSettings>>>(selector, (Action<ICreateFollowIndexRequest, Func<IndexSettingsDescriptor, IPromise<IIndexSettings>>>) ((a, v) => a.Settings = v != null ? v(new IndexSettingsDescriptor())?.Value : (IIndexSettings) null));
    }

    public CreateFollowIndexDescriptor MaxReadRequestOperationCount(long? max) => this.Assign<long?>(max, (Action<ICreateFollowIndexRequest, long?>) ((a, v) => a.MaxReadRequestOperationCount = v));

    public CreateFollowIndexDescriptor MaxOutstandingReadRequests(long? max) => this.Assign<long?>(max, (Action<ICreateFollowIndexRequest, long?>) ((a, v) => a.MaxOutstandingReadRequests = v));

    public CreateFollowIndexDescriptor MaxRequestSize(string maxRequestSize) => this.Assign<string>(maxRequestSize, (Action<ICreateFollowIndexRequest, string>) ((a, v) => a.MaxRequestSize = v));

    public CreateFollowIndexDescriptor MaxWriteRequestOperationCount(long? max) => this.Assign<long?>(max, (Action<ICreateFollowIndexRequest, long?>) ((a, v) => a.MaxWriteRequestOperationCount = v));

    public CreateFollowIndexDescriptor MaxWriteRequestSize(string maxSize) => this.Assign<string>(maxSize, (Action<ICreateFollowIndexRequest, string>) ((a, v) => a.MaxWriteRequestSize = v));

    public CreateFollowIndexDescriptor MaxOutstandingWriteRequests(long? max) => this.Assign<long?>(max, (Action<ICreateFollowIndexRequest, long?>) ((a, v) => a.MaxOutstandingWriteRequests = v));

    public CreateFollowIndexDescriptor MaxWriteBufferCount(long? max) => this.Assign<long?>(max, (Action<ICreateFollowIndexRequest, long?>) ((a, v) => a.MaxWriteBufferCount = v));

    public CreateFollowIndexDescriptor MaxWriteBufferSize(string max) => this.Assign<string>(max, (Action<ICreateFollowIndexRequest, string>) ((a, v) => a.MaxWriteBufferSize = v));

    public CreateFollowIndexDescriptor MaxRetryDelay(Time maxRetryDelay) => this.Assign<Time>(maxRetryDelay, (Action<ICreateFollowIndexRequest, Time>) ((a, v) => a.MaxRetryDelay = v));

    public CreateFollowIndexDescriptor ReadPollTimeout(Time readPollTimeout) => this.Assign<Time>(readPollTimeout, (Action<ICreateFollowIndexRequest, Time>) ((a, v) => a.ReadPollTimeout = v));
  }
}
