// Decompiled with JetBrains decompiler
// Type: Nest.DeleteRollupJobRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.RollupApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class DeleteRollupJobRequest : 
    PlainRequestBase<DeleteRollupJobRequestParameters>,
    IDeleteRollupJobRequest,
    IRequest<DeleteRollupJobRequestParameters>,
    IRequest
  {
    protected IDeleteRollupJobRequest Self => (IDeleteRollupJobRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.RollupDeleteJob;

    public DeleteRollupJobRequest(Id id)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (id), (IUrlParameter) id)))
    {
    }

    [SerializationConstructor]
    protected DeleteRollupJobRequest()
    {
    }

    [IgnoreDataMember]
    Id IDeleteRollupJobRequest.Id => this.Self.RouteValues.Get<Id>("id");
  }
}
