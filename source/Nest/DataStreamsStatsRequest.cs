// Decompiled with JetBrains decompiler
// Type: Nest.DataStreamsStatsRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class DataStreamsStatsRequest : 
    PlainRequestBase<DataStreamsStatsRequestParameters>,
    IDataStreamsStatsRequest,
    IRequest<DataStreamsStatsRequestParameters>,
    IRequest
  {
    protected IDataStreamsStatsRequest Self => (IDataStreamsStatsRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesDataStreamsStats;

    public DataStreamsStatsRequest()
    {
    }

    public DataStreamsStatsRequest(Names name)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (name), (IUrlParameter) name)))
    {
    }

    [IgnoreDataMember]
    Names IDataStreamsStatsRequest.Name => this.Self.RouteValues.Get<Names>("name");
  }
}
