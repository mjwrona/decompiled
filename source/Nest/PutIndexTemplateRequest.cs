// Decompiled with JetBrains decompiler
// Type: Nest.PutIndexTemplateRequest
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
  public class PutIndexTemplateRequest : 
    PlainRequestBase<PutIndexTemplateRequestParameters>,
    IPutIndexTemplateRequest,
    ITemplateMapping,
    IRequest<PutIndexTemplateRequestParameters>,
    IRequest
  {
    public IAliases Aliases { get; set; }

    public IReadOnlyCollection<string> IndexPatterns { get; set; }

    public ITypeMapping Mappings { get; set; }

    public int? Order { get; set; }

    public IIndexSettings Settings { get; set; }

    public int? Version { get; set; }

    protected IPutIndexTemplateRequest Self => (IPutIndexTemplateRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesPutTemplate;

    public PutIndexTemplateRequest(Name name)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (name), (IUrlParameter) name)))
    {
    }

    [SerializationConstructor]
    protected PutIndexTemplateRequest()
    {
    }

    [IgnoreDataMember]
    Name IPutIndexTemplateRequest.Name => this.Self.RouteValues.Get<Name>("name");

    public bool? Create
    {
      get => this.Q<bool?>("create");
      set => this.Q("create", (object) value);
    }

    [Obsolete("Scheduled to be removed in 8.0, Deprecated as of: 7.7.0, reason: Removed from the server as it was never a valid option")]
    public bool? FlatSettings
    {
      get => this.Q<bool?>("flat_settings");
      set => this.Q("flat_settings", (object) value);
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

    [Obsolete("Scheduled to be removed in 8.0, Deprecated as of: 7.7.0, reason: Removed from the server as it was never a valid option")]
    public Time Timeout
    {
      get => this.Q<Time>("timeout");
      set => this.Q("timeout", (object) value);
    }
  }
}
