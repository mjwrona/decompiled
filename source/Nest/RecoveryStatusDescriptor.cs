// Decompiled with JetBrains decompiler
// Type: Nest.RecoveryStatusDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using System;

namespace Nest
{
  public class RecoveryStatusDescriptor : 
    RequestDescriptorBase<RecoveryStatusDescriptor, RecoveryStatusRequestParameters, IRecoveryStatusRequest>,
    IRecoveryStatusRequest,
    IRequest<RecoveryStatusRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesRecoveryStatus;

    public RecoveryStatusDescriptor()
    {
    }

    public RecoveryStatusDescriptor(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    Indices IRecoveryStatusRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public RecoveryStatusDescriptor Index(Indices index) => this.Assign<Indices>(index, (Action<IRecoveryStatusRequest, Indices>) ((a, v) => a.RouteValues.Optional(nameof (index), (IUrlParameter) v)));

    public RecoveryStatusDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IRecoveryStatusRequest, Type>) ((a, v) => a.RouteValues.Optional("index", (IUrlParameter) (Indices) v)));

    public RecoveryStatusDescriptor AllIndices() => this.Index(Indices.All);

    public RecoveryStatusDescriptor ActiveOnly(bool? activeonly = true) => this.Qs("active_only", (object) activeonly);

    public RecoveryStatusDescriptor Detailed(bool? detailed = true) => this.Qs(nameof (detailed), (object) detailed);
  }
}
