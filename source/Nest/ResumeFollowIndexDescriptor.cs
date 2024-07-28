// Decompiled with JetBrains decompiler
// Type: Nest.ResumeFollowIndexDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.CrossClusterReplicationApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class ResumeFollowIndexDescriptor : 
    RequestDescriptorBase<ResumeFollowIndexDescriptor, ResumeFollowIndexRequestParameters, IResumeFollowIndexRequest>,
    IResumeFollowIndexRequest,
    IRequest<ResumeFollowIndexRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.CrossClusterReplicationResumeFollowIndex;

    public ResumeFollowIndexDescriptor(IndexName index)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index)))
    {
    }

    [SerializationConstructor]
    protected ResumeFollowIndexDescriptor()
    {
    }

    IndexName IResumeFollowIndexRequest.Index => this.Self.RouteValues.Get<IndexName>("index");

    public ResumeFollowIndexDescriptor Index(IndexName index) => this.Assign<IndexName>(index, (Action<IResumeFollowIndexRequest, IndexName>) ((a, v) => a.RouteValues.Required(nameof (index), (IUrlParameter) v)));

    public ResumeFollowIndexDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IResumeFollowIndexRequest, Type>) ((a, v) => a.RouteValues.Required("index", (IUrlParameter) (IndexName) v)));

    long? IResumeFollowIndexRequest.MaxReadRequestOperationCount { get; set; }

    long? IResumeFollowIndexRequest.MaxOutstandingReadRequests { get; set; }

    string IResumeFollowIndexRequest.MaxRequestSize { get; set; }

    long? IResumeFollowIndexRequest.MaxWriteRequestOperationCount { get; set; }

    string IResumeFollowIndexRequest.MaxWriteRequestSize { get; set; }

    long? IResumeFollowIndexRequest.MaxOutstandingWriteRequests { get; set; }

    long? IResumeFollowIndexRequest.MaxWriteBufferCount { get; set; }

    string IResumeFollowIndexRequest.MaxWriteBufferSize { get; set; }

    Time IResumeFollowIndexRequest.MaxRetryDelay { get; set; }

    Time IResumeFollowIndexRequest.ReadPollTimeout { get; set; }

    public ResumeFollowIndexDescriptor MaxReadRequestOperationCount(long? max) => this.Assign<long?>(max, (Action<IResumeFollowIndexRequest, long?>) ((a, v) => a.MaxReadRequestOperationCount = v));

    public ResumeFollowIndexDescriptor MaxOutstandingReadRequests(long? max) => this.Assign<long?>(max, (Action<IResumeFollowIndexRequest, long?>) ((a, v) => a.MaxOutstandingReadRequests = v));

    public ResumeFollowIndexDescriptor MaxRequestSize(string maxRequestSize) => this.Assign<string>(maxRequestSize, (Action<IResumeFollowIndexRequest, string>) ((a, v) => a.MaxRequestSize = v));

    public ResumeFollowIndexDescriptor MaxWriteRequestOperationCount(long? max) => this.Assign<long?>(max, (Action<IResumeFollowIndexRequest, long?>) ((a, v) => a.MaxWriteRequestOperationCount = v));

    public ResumeFollowIndexDescriptor MaxWriteRequestSize(string size) => this.Assign<string>(size, (Action<IResumeFollowIndexRequest, string>) ((a, v) => a.MaxWriteRequestSize = v));

    public ResumeFollowIndexDescriptor MaxOutstandingWriteRequests(long? max) => this.Assign<long?>(max, (Action<IResumeFollowIndexRequest, long?>) ((a, v) => a.MaxOutstandingWriteRequests = v));

    public ResumeFollowIndexDescriptor MaxWriteBufferCount(long? max) => this.Assign<long?>(max, (Action<IResumeFollowIndexRequest, long?>) ((a, v) => a.MaxWriteBufferCount = v));

    public ResumeFollowIndexDescriptor MaxWriteBufferSize(string max) => this.Assign<string>(max, (Action<IResumeFollowIndexRequest, string>) ((a, v) => a.MaxWriteBufferSize = v));

    public ResumeFollowIndexDescriptor MaxRetryDelay(Time maxRetryDelay) => this.Assign<Time>(maxRetryDelay, (Action<IResumeFollowIndexRequest, Time>) ((a, v) => a.MaxRetryDelay = v));

    public ResumeFollowIndexDescriptor ReadPollTimeout(Time readPollTimeout) => this.Assign<Time>(readPollTimeout, (Action<IResumeFollowIndexRequest, Time>) ((a, v) => a.ReadPollTimeout = v));
  }
}
