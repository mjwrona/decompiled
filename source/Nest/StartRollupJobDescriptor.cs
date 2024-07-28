// Decompiled with JetBrains decompiler
// Type: Nest.StartRollupJobDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.RollupApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class StartRollupJobDescriptor : 
    RequestDescriptorBase<StartRollupJobDescriptor, StartRollupJobRequestParameters, IStartRollupJobRequest>,
    IStartRollupJobRequest,
    IRequest<StartRollupJobRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.RollupStartJob;

    public StartRollupJobDescriptor(Id id)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (id), (IUrlParameter) id)))
    {
    }

    [SerializationConstructor]
    protected StartRollupJobDescriptor()
    {
    }

    Id IStartRollupJobRequest.Id => this.Self.RouteValues.Get<Id>("id");
  }
}
