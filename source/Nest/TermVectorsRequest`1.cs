// Decompiled with JetBrains decompiler
// Type: Nest.TermVectorsRequest`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class TermVectorsRequest<TDocument> : 
    PlainRequestBase<TermVectorsRequestParameters>,
    ITermVectorsRequest<TDocument>,
    IRequest<TermVectorsRequestParameters>,
    IRequest
    where TDocument : class
  {
    public TDocument Document { get; set; }

    public ITermVectorFilter Filter { get; set; }

    public IPerFieldAnalyzer PerFieldAnalyzer { get; set; }

    HttpMethod IRequest.HttpMethod => (object) this.Document == null && this.Filter == null ? HttpMethod.GET : HttpMethod.POST;

    protected ITermVectorsRequest<TDocument> Self => (ITermVectorsRequest<TDocument>) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceTermVectors;

    public TermVectorsRequest(IndexName index, Id id)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index).Optional(nameof (id), (IUrlParameter) id)))
    {
    }

    public TermVectorsRequest(IndexName index)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index)))
    {
    }

    public TermVectorsRequest(Id id)
      : this((IndexName) typeof (TDocument), id)
    {
    }

    public TermVectorsRequest()
      : this((IndexName) typeof (TDocument))
    {
    }

    public TermVectorsRequest(TDocument documentWithId, IndexName index = null, Id id = null)
    {
      IndexName index1 = index;
      if ((object) index1 == null)
        index1 = (IndexName) typeof (TDocument);
      Id id1 = id;
      if ((object) id1 == null)
        id1 = Id.From<TDocument>(documentWithId);
      // ISSUE: explicit constructor call
      this.\u002Ector(index1, id1);
      this.DocumentFromPath(documentWithId);
    }

    private void DocumentFromPath(TDocument document)
    {
      this.Document = document;
      if ((object) this.Document == null)
        return;
      this.Self.RouteValues.Remove("id");
    }

    [IgnoreDataMember]
    IndexName ITermVectorsRequest<TDocument>.Index => this.Self.RouteValues.Get<IndexName>("index");

    [IgnoreDataMember]
    Id ITermVectorsRequest<TDocument>.Id => this.Self.RouteValues.Get<Id>("id");

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
