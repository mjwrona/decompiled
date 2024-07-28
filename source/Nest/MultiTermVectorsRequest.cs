// Decompiled with JetBrains decompiler
// Type: Nest.MultiTermVectorsRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class MultiTermVectorsRequest : 
    PlainRequestBase<MultiTermVectorsRequestParameters>,
    IMultiTermVectorsRequest,
    IRequest<MultiTermVectorsRequestParameters>,
    IRequest
  {
    public IEnumerable<IMultiTermVectorOperation> Documents { get; set; }

    public IEnumerable<Id> Ids { get; set; }

    protected IMultiTermVectorsRequest Self => (IMultiTermVectorsRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceMultiTermVectors;

    public MultiTermVectorsRequest()
    {
    }

    public MultiTermVectorsRequest(IndexName index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    [IgnoreDataMember]
    IndexName IMultiTermVectorsRequest.Index => this.Self.RouteValues.Get<IndexName>("index");

    public bool? FieldStatistics
    {
      get => this.Q<bool?>("field_statistics");
      set => this.Q("field_statistics", (object) value);
    }

    public Fields Fields
    {
      get => this.Q<Fields>("fields");
      set => this.Q("fields", (object) value);
    }

    public bool? Offsets
    {
      get => this.Q<bool?>("offsets");
      set => this.Q("offsets", (object) value);
    }

    public bool? Payloads
    {
      get => this.Q<bool?>("payloads");
      set => this.Q("payloads", (object) value);
    }

    public bool? Positions
    {
      get => this.Q<bool?>("positions");
      set => this.Q("positions", (object) value);
    }

    public string Preference
    {
      get => this.Q<string>("preference");
      set => this.Q("preference", (object) value);
    }

    public bool? Realtime
    {
      get => this.Q<bool?>("realtime");
      set => this.Q("realtime", (object) value);
    }

    public Routing Routing
    {
      get => this.Q<Routing>("routing");
      set => this.Q("routing", (object) value);
    }

    public bool? TermStatistics
    {
      get => this.Q<bool?>("term_statistics");
      set => this.Q("term_statistics", (object) value);
    }

    public long? Version
    {
      get => this.Q<long?>("version");
      set => this.Q("version", (object) value);
    }

    public Elasticsearch.Net.VersionType? VersionType
    {
      get => this.Q<Elasticsearch.Net.VersionType?>("version_type");
      set => this.Q("version_type", (object) value);
    }
  }
}
