// Decompiled with JetBrains decompiler
// Type: Nest.GetCategoriesRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class GetCategoriesRequest : 
    PlainRequestBase<GetCategoriesRequestParameters>,
    IGetCategoriesRequest,
    IRequest<GetCategoriesRequestParameters>,
    IRequest
  {
    protected IGetCategoriesRequest Self => (IGetCategoriesRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningGetCategories;

    public GetCategoriesRequest(Id jobId, LongId categoryId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("job_id", (IUrlParameter) jobId).Optional("category_id", (IUrlParameter) categoryId)))
    {
    }

    public GetCategoriesRequest(Id jobId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("job_id", (IUrlParameter) jobId)))
    {
    }

    [SerializationConstructor]
    protected GetCategoriesRequest()
    {
    }

    [IgnoreDataMember]
    Id IGetCategoriesRequest.JobId => this.Self.RouteValues.Get<Id>("job_id");

    [IgnoreDataMember]
    LongId IGetCategoriesRequest.CategoryId => this.Self.RouteValues.Get<LongId>("category_id");

    public Field PartitionFieldValue
    {
      get => this.Q<Field>("partition_field_value");
      set => this.Q("partition_field_value", (object) value);
    }

    public IPage Page { get; set; }
  }
}
