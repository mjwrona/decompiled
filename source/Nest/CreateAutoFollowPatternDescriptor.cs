// Decompiled with JetBrains decompiler
// Type: Nest.CreateAutoFollowPatternDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.CrossClusterReplicationApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;

namespace Nest
{
  public class CreateAutoFollowPatternDescriptor : 
    RequestDescriptorBase<CreateAutoFollowPatternDescriptor, CreateAutoFollowPatternRequestParameters, ICreateAutoFollowPatternRequest>,
    ICreateAutoFollowPatternRequest,
    IRequest<CreateAutoFollowPatternRequestParameters>,
    IRequest,
    IAutoFollowPattern
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.CrossClusterReplicationCreateAutoFollowPattern;

    public CreateAutoFollowPatternDescriptor(Name name)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (name), (IUrlParameter) name)))
    {
    }

    [SerializationConstructor]
    protected CreateAutoFollowPatternDescriptor()
    {
    }

    Name ICreateAutoFollowPatternRequest.Name => this.Self.RouteValues.Get<Name>("name");

    string IAutoFollowPattern.RemoteCluster { get; set; }

    IEnumerable<string> IAutoFollowPattern.LeaderIndexPatterns { get; set; }

    IEnumerable<string> IAutoFollowPattern.LeaderIndexExclusionPatterns { get; set; }

    string IAutoFollowPattern.FollowIndexPattern { get; set; }

    IIndexSettings IAutoFollowPattern.Settings { get; set; }

    int? IAutoFollowPattern.MaxReadRequestOperationCount { get; set; }

    long? IAutoFollowPattern.MaxOutstandingReadRequests { get; set; }

    string IAutoFollowPattern.MaxReadRequestSize { get; set; }

    int? IAutoFollowPattern.MaxWriteRequestOperationCount { get; set; }

    string IAutoFollowPattern.MaxWriteRequestSize { get; set; }

    int? IAutoFollowPattern.MaxOutstandingWriteRequests { get; set; }

    int? IAutoFollowPattern.MaxWriteBufferCount { get; set; }

    string IAutoFollowPattern.MaxWriteBufferSize { get; set; }

    Time IAutoFollowPattern.MaxRetryDelay { get; set; }

    Time IAutoFollowPattern.MaxPollTimeout { get; set; }

    public CreateAutoFollowPatternDescriptor RemoteCluster(string remoteCluster) => this.Assign<string>(remoteCluster, (Action<ICreateAutoFollowPatternRequest, string>) ((a, v) => a.RemoteCluster = v));

    public CreateAutoFollowPatternDescriptor LeaderIndexPatterns(
      IEnumerable<string> leaderIndexPatterns)
    {
      return this.Assign<IEnumerable<string>>(leaderIndexPatterns, (Action<ICreateAutoFollowPatternRequest, IEnumerable<string>>) ((a, v) => a.LeaderIndexPatterns = v));
    }

    public CreateAutoFollowPatternDescriptor LeaderIndexPatterns(params string[] leaderIndexPatterns) => this.Assign<string[]>(leaderIndexPatterns, (Action<ICreateAutoFollowPatternRequest, string[]>) ((a, v) => a.LeaderIndexPatterns = (IEnumerable<string>) v));

    public CreateAutoFollowPatternDescriptor LeaderIndexExclusionPatterns(
      IEnumerable<string> leaderIndexExclusionPatterns)
    {
      return this.Assign<IEnumerable<string>>(leaderIndexExclusionPatterns, (Action<ICreateAutoFollowPatternRequest, IEnumerable<string>>) ((a, v) => a.LeaderIndexExclusionPatterns = v));
    }

    public CreateAutoFollowPatternDescriptor LeaderIndexExclusionPatterns(
      params string[] leaderIndexExclusionPatterns)
    {
      return this.Assign<string[]>(leaderIndexExclusionPatterns, (Action<ICreateAutoFollowPatternRequest, string[]>) ((a, v) => a.LeaderIndexExclusionPatterns = (IEnumerable<string>) v));
    }

    public CreateAutoFollowPatternDescriptor FollowIndexPattern(string followIndexPattern) => this.Assign<string>(followIndexPattern, (Action<ICreateAutoFollowPatternRequest, string>) ((a, v) => a.FollowIndexPattern = v));

    public CreateAutoFollowPatternDescriptor Settings(
      Func<IndexSettingsDescriptor, IPromise<IIndexSettings>> selector)
    {
      return this.Assign<Func<IndexSettingsDescriptor, IPromise<IIndexSettings>>>(selector, (Action<ICreateAutoFollowPatternRequest, Func<IndexSettingsDescriptor, IPromise<IIndexSettings>>>) ((a, v) => a.Settings = v != null ? v(new IndexSettingsDescriptor())?.Value : (IIndexSettings) null));
    }

    public CreateAutoFollowPatternDescriptor MaxReadRequestOperationCount(
      int? maxReadRequestOperationCount)
    {
      return this.Assign<int?>(maxReadRequestOperationCount, (Action<ICreateAutoFollowPatternRequest, int?>) ((a, v) => a.MaxReadRequestOperationCount = v));
    }

    public CreateAutoFollowPatternDescriptor MaxOutstandingReadRequests(
      long? maxOutstandingReadRequests)
    {
      return this.Assign<long?>(maxOutstandingReadRequests, (Action<ICreateAutoFollowPatternRequest, long?>) ((a, v) => a.MaxOutstandingReadRequests = v));
    }

    public CreateAutoFollowPatternDescriptor MaxReadRequestSize(string maxReadRequestSize) => this.Assign<string>(maxReadRequestSize, (Action<ICreateAutoFollowPatternRequest, string>) ((a, v) => a.MaxReadRequestSize = v));

    public CreateAutoFollowPatternDescriptor MaxWriteRequestOperationCount(
      int? maxWriteRequestOperationCount)
    {
      return this.Assign<int?>(maxWriteRequestOperationCount, (Action<ICreateAutoFollowPatternRequest, int?>) ((a, v) => a.MaxWriteRequestOperationCount = v));
    }

    public CreateAutoFollowPatternDescriptor MaxWriteRequestSize(string maxWriteRequestSize) => this.Assign<string>(maxWriteRequestSize, (Action<ICreateAutoFollowPatternRequest, string>) ((a, v) => a.MaxWriteRequestSize = v));

    public CreateAutoFollowPatternDescriptor MaxOutstandingWriteRequests(
      int? maxOutstandingWriteRequests)
    {
      return this.Assign<int?>(maxOutstandingWriteRequests, (Action<ICreateAutoFollowPatternRequest, int?>) ((a, v) => a.MaxOutstandingWriteRequests = v));
    }

    public CreateAutoFollowPatternDescriptor MaxWriteBufferCount(int? maxWriteBufferCount) => this.Assign<int?>(maxWriteBufferCount, (Action<ICreateAutoFollowPatternRequest, int?>) ((a, v) => a.MaxWriteBufferCount = v));

    public CreateAutoFollowPatternDescriptor MaxWriteBufferSize(string maxWriteBufferSize) => this.Assign<string>(maxWriteBufferSize, (Action<ICreateAutoFollowPatternRequest, string>) ((a, v) => a.MaxWriteBufferSize = v));

    public CreateAutoFollowPatternDescriptor MaxRetryDelay(Time maxRetryDelay) => this.Assign<Time>(maxRetryDelay, (Action<ICreateAutoFollowPatternRequest, Time>) ((a, v) => a.MaxRetryDelay = v));

    public CreateAutoFollowPatternDescriptor MaxPollTimeout(Time maxPollTimeout) => this.Assign<Time>(maxPollTimeout, (Action<ICreateAutoFollowPatternRequest, Time>) ((a, v) => a.MaxPollTimeout = v));
  }
}
