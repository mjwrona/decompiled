// Decompiled with JetBrains decompiler
// Type: Nest.CatAllocationRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.CatApi;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class CatAllocationRequest : 
    PlainRequestBase<CatAllocationRequestParameters>,
    ICatAllocationRequest,
    IRequest<CatAllocationRequestParameters>,
    IRequest
  {
    protected ICatAllocationRequest Self => (ICatAllocationRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.CatAllocation;

    public CatAllocationRequest()
    {
    }

    public CatAllocationRequest(NodeIds nodeId)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional("node_id", (IUrlParameter) nodeId)))
    {
    }

    [IgnoreDataMember]
    NodeIds ICatAllocationRequest.NodeId => this.Self.RouteValues.Get<NodeIds>("node_id");

    public Elasticsearch.Net.Bytes? Bytes
    {
      get => this.Q<Elasticsearch.Net.Bytes?>("bytes");
      set => this.Q("bytes", (object) value);
    }

    public string Format
    {
      get => this.Q<string>("format");
      set
      {
        this.Q("format", (object) value);
        this.SetAcceptHeader(value);
      }
    }

    public string[] Headers
    {
      get => this.Q<string[]>("h");
      set => this.Q("h", (object) value);
    }

    public bool? Help
    {
      get => this.Q<bool?>("help");
      set => this.Q("help", (object) value);
    }

    public bool? Local
    {
      get => this.Q<bool?>("local");
      set => this.Q("local", (object) value);
    }

    public Time MasterTimeout
    {
      get => this.Q<Time>("master_timeout");
      set => this.Q("master_timeout", (object) value);
    }

    public string[] SortByColumns
    {
      get => this.Q<string[]>("s");
      set => this.Q("s", (object) value);
    }

    public bool? Verbose
    {
      get => this.Q<bool?>("v");
      set => this.Q("v", (object) value);
    }
  }
}
