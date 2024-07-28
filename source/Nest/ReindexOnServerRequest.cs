// Decompiled with JetBrains decompiler
// Type: Nest.ReindexOnServerRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;

namespace Nest
{
  public class ReindexOnServerRequest : 
    PlainRequestBase<ReindexOnServerRequestParameters>,
    IReindexOnServerRequest,
    IRequest<ReindexOnServerRequestParameters>,
    IRequest
  {
    public Elasticsearch.Net.Conflicts? Conflicts { get; set; }

    public IReindexDestination Destination { get; set; }

    public IScript Script { get; set; }

    [Obsolete("Deprecated. Use MaximumDocuments")]
    public long? Size { get; set; }

    public long? MaximumDocuments { get; set; }

    public IReindexSource Source { get; set; }

    protected IReindexOnServerRequest Self => (IReindexOnServerRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceReindexOnServer;

    public bool? Refresh
    {
      get => this.Q<bool?>("refresh");
      set => this.Q("refresh", (object) value);
    }

    public long? RequestsPerSecond
    {
      get => this.Q<long?>("requests_per_second");
      set => this.Q("requests_per_second", (object) value);
    }

    public Time Scroll
    {
      get => this.Q<Time>("scroll");
      set => this.Q("scroll", (object) value);
    }

    public long? Slices
    {
      get => this.Q<long?>("slices");
      set => this.Q("slices", (object) value);
    }

    public Time Timeout
    {
      get => this.Q<Time>("timeout");
      set => this.Q("timeout", (object) value);
    }

    public string WaitForActiveShards
    {
      get => this.Q<string>("wait_for_active_shards");
      set => this.Q("wait_for_active_shards", (object) value);
    }

    public bool? WaitForCompletion
    {
      get => this.Q<bool?>("wait_for_completion");
      set => this.Q("wait_for_completion", (object) value);
    }
  }
}
