// Decompiled with JetBrains decompiler
// Type: Nest.GetCategoriesDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Linq.Expressions;

namespace Nest
{
  public class GetCategoriesDescriptor : 
    RequestDescriptorBase<GetCategoriesDescriptor, GetCategoriesRequestParameters, IGetCategoriesRequest>,
    IGetCategoriesRequest,
    IRequest<GetCategoriesRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningGetCategories;

    public GetCategoriesDescriptor(Id jobId, LongId categoryId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("job_id", (IUrlParameter) jobId).Optional("category_id", (IUrlParameter) categoryId)))
    {
    }

    public GetCategoriesDescriptor(Id jobId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("job_id", (IUrlParameter) jobId)))
    {
    }

    [SerializationConstructor]
    protected GetCategoriesDescriptor()
    {
    }

    Id IGetCategoriesRequest.JobId => this.Self.RouteValues.Get<Id>("job_id");

    LongId IGetCategoriesRequest.CategoryId => this.Self.RouteValues.Get<LongId>("category_id");

    public GetCategoriesDescriptor CategoryId(LongId categoryId) => this.Assign<LongId>(categoryId, (Action<IGetCategoriesRequest, LongId>) ((a, v) => a.RouteValues.Optional("category_id", (IUrlParameter) v)));

    public GetCategoriesDescriptor PartitionFieldValue(Field partitionfieldvalue) => this.Qs("partition_field_value", (object) partitionfieldvalue);

    public GetCategoriesDescriptor PartitionFieldValue<T>(Expression<Func<T, object>> field) where T : class => this.Qs("partition_field_value", (object) (Field) (Expression) field);

    IPage IGetCategoriesRequest.Page { get; set; }

    public GetCategoriesDescriptor Page(Func<PageDescriptor, IPage> selector) => this.Assign<Func<PageDescriptor, IPage>>(selector, (Action<IGetCategoriesRequest, Func<PageDescriptor, IPage>>) ((a, v) => a.Page = v != null ? v(new PageDescriptor()) : (IPage) null));
  }
}
