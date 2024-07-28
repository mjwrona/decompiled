// Decompiled with JetBrains decompiler
// Type: Nest.ReloadSecureSettingsRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.NodesApi;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class ReloadSecureSettingsRequest : 
    PlainRequestBase<ReloadSecureSettingsRequestParameters>,
    IReloadSecureSettingsRequest,
    IRequest<ReloadSecureSettingsRequestParameters>,
    IRequest
  {
    protected IReloadSecureSettingsRequest Self => (IReloadSecureSettingsRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.NodesReloadSecureSettings;

    public ReloadSecureSettingsRequest()
    {
    }

    public ReloadSecureSettingsRequest(NodeIds nodeId)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional("node_id", (IUrlParameter) nodeId)))
    {
    }

    [IgnoreDataMember]
    NodeIds IReloadSecureSettingsRequest.NodeId => this.Self.RouteValues.Get<NodeIds>("node_id");

    public Time Timeout
    {
      get => this.Q<Time>("timeout");
      set => this.Q("timeout", (object) value);
    }
  }
}
