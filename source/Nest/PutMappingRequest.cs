// Decompiled with JetBrains decompiler
// Type: Nest.PutMappingRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class PutMappingRequest : 
    PlainRequestBase<PutMappingRequestParameters>,
    IPutMappingRequest,
    ITypeMapping,
    IRequest<PutMappingRequestParameters>,
    IRequest
  {
    [Obsolete("The _all field is no longer supported in Elasticsearch 7.x and will be removed in the next major release. The value will not be sent in a request. An _all like field can be achieved using copy_to")]
    public IAllField AllField { get; set; }

    public bool? DateDetection { get; set; }

    public Union<bool, DynamicMapping> Dynamic { get; set; }

    public IEnumerable<string> DynamicDateFormats { get; set; }

    public IDynamicTemplateContainer DynamicTemplates { get; set; }

    public IFieldNamesField FieldNamesField { get; set; }

    [Obsolete("Configuration for the _index field is no longer supported in Elasticsearch 7.x and will be removed in the next major release.")]
    public IIndexField IndexField { get; set; }

    public IDictionary<string, object> Meta { get; set; }

    public bool? NumericDetection { get; set; }

    public IProperties Properties { get; set; }

    public IRoutingField RoutingField { get; set; }

    public IRuntimeFields RuntimeFields { get; set; }

    public ISizeField SizeField { get; set; }

    public ISourceField SourceField { get; set; }

    protected IPutMappingRequest Self => (IPutMappingRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesPutMapping;

    public PutMappingRequest(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index)))
    {
    }

    [SerializationConstructor]
    protected PutMappingRequest()
    {
    }

    [IgnoreDataMember]
    Indices IPutMappingRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public bool? AllowNoIndices
    {
      get => this.Q<bool?>("allow_no_indices");
      set => this.Q("allow_no_indices", (object) value);
    }

    public Elasticsearch.Net.ExpandWildcards? ExpandWildcards
    {
      get => this.Q<Elasticsearch.Net.ExpandWildcards?>("expand_wildcards");
      set => this.Q("expand_wildcards", (object) value);
    }

    public bool? IgnoreUnavailable
    {
      get => this.Q<bool?>("ignore_unavailable");
      set => this.Q("ignore_unavailable", (object) value);
    }

    public bool? IncludeTypeName
    {
      get => this.Q<bool?>("include_type_name");
      set => this.Q("include_type_name", (object) value);
    }

    public Time MasterTimeout
    {
      get => this.Q<Time>("master_timeout");
      set => this.Q("master_timeout", (object) value);
    }

    public Time Timeout
    {
      get => this.Q<Time>("timeout");
      set => this.Q("timeout", (object) value);
    }

    public bool? WriteIndexOnly
    {
      get => this.Q<bool?>("write_index_only");
      set => this.Q("write_index_only", (object) value);
    }
  }
}
