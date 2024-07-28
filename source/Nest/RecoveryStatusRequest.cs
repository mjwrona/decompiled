// Decompiled with JetBrains decompiler
// Type: Nest.RecoveryStatusRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class RecoveryStatusRequest : 
    PlainRequestBase<RecoveryStatusRequestParameters>,
    IRecoveryStatusRequest,
    IRequest<RecoveryStatusRequestParameters>,
    IRequest
  {
    protected IRecoveryStatusRequest Self => (IRecoveryStatusRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesRecoveryStatus;

    public RecoveryStatusRequest()
    {
    }

    public RecoveryStatusRequest(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    [IgnoreDataMember]
    Indices IRecoveryStatusRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public bool? ActiveOnly
    {
      get => this.Q<bool?>("active_only");
      set => this.Q("active_only", (object) value);
    }

    public bool? Detailed
    {
      get => this.Q<bool?>("detailed");
      set => this.Q("detailed", (object) value);
    }
  }
}
