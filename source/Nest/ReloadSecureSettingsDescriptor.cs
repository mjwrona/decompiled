// Decompiled with JetBrains decompiler
// Type: Nest.ReloadSecureSettingsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.NodesApi;
using System;

namespace Nest
{
  public class ReloadSecureSettingsDescriptor : 
    RequestDescriptorBase<ReloadSecureSettingsDescriptor, ReloadSecureSettingsRequestParameters, IReloadSecureSettingsRequest>,
    IReloadSecureSettingsRequest,
    IRequest<ReloadSecureSettingsRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.NodesReloadSecureSettings;

    public ReloadSecureSettingsDescriptor()
    {
    }

    public ReloadSecureSettingsDescriptor(NodeIds nodeId)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional("node_id", (IUrlParameter) nodeId)))
    {
    }

    NodeIds IReloadSecureSettingsRequest.NodeId => this.Self.RouteValues.Get<NodeIds>("node_id");

    public ReloadSecureSettingsDescriptor NodeId(NodeIds nodeId) => this.Assign<NodeIds>(nodeId, (Action<IReloadSecureSettingsRequest, NodeIds>) ((a, v) => a.RouteValues.Optional("node_id", (IUrlParameter) v)));

    public ReloadSecureSettingsDescriptor Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);
  }
}
