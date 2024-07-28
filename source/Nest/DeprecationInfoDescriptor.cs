// Decompiled with JetBrains decompiler
// Type: Nest.DeprecationInfoDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MigrationApi;
using System;

namespace Nest
{
  public class DeprecationInfoDescriptor : 
    RequestDescriptorBase<DeprecationInfoDescriptor, DeprecationInfoRequestParameters, IDeprecationInfoRequest>,
    IDeprecationInfoRequest,
    IRequest<DeprecationInfoRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.MigrationDeprecationInfo;

    public DeprecationInfoDescriptor()
    {
    }

    public DeprecationInfoDescriptor(IndexName index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    IndexName IDeprecationInfoRequest.Index => this.Self.RouteValues.Get<IndexName>("index");

    public DeprecationInfoDescriptor Index(IndexName index) => this.Assign<IndexName>(index, (Action<IDeprecationInfoRequest, IndexName>) ((a, v) => a.RouteValues.Optional(nameof (index), (IUrlParameter) v)));

    public DeprecationInfoDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IDeprecationInfoRequest, Type>) ((a, v) => a.RouteValues.Optional("index", (IUrlParameter) (IndexName) v)));
  }
}
