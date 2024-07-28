// Decompiled with JetBrains decompiler
// Type: Nest.PutScriptDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class PutScriptDescriptor : 
    RequestDescriptorBase<PutScriptDescriptor, PutScriptRequestParameters, IPutScriptRequest>,
    IPutScriptRequest,
    IRequest<PutScriptRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespacePutScript;

    public PutScriptDescriptor(Id id)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (id), (IUrlParameter) id)))
    {
    }

    public PutScriptDescriptor(Id id, Name context)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (id), (IUrlParameter) id).Optional(nameof (context), (IUrlParameter) context)))
    {
    }

    [SerializationConstructor]
    protected PutScriptDescriptor()
    {
    }

    Id IPutScriptRequest.Id => this.Self.RouteValues.Get<Id>("id");

    Name IPutScriptRequest.Context => this.Self.RouteValues.Get<Name>("context");

    public PutScriptDescriptor Context(Name context) => this.Assign<Name>(context, (Action<IPutScriptRequest, Name>) ((a, v) => a.RouteValues.Optional(nameof (context), (IUrlParameter) v)));

    public PutScriptDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    public PutScriptDescriptor Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);

    IStoredScript IPutScriptRequest.Script { get; set; }

    public PutScriptDescriptor Script(
      Func<StoredScriptDescriptor, IStoredScript> selector)
    {
      return this.Assign<Func<StoredScriptDescriptor, IStoredScript>>(selector, (Action<IPutScriptRequest, Func<StoredScriptDescriptor, IStoredScript>>) ((a, v) => a.Script = v != null ? v(new StoredScriptDescriptor()) : (IStoredScript) null));
    }

    public PutScriptDescriptor Painless(string source) => this.Assign<PainlessScript>(new PainlessScript(source), (Action<IPutScriptRequest, PainlessScript>) ((a, v) => a.Script = (IStoredScript) v));

    public PutScriptDescriptor LuceneExpression(string source) => this.Assign<LuceneExpressionScript>(new LuceneExpressionScript(source), (Action<IPutScriptRequest, LuceneExpressionScript>) ((a, v) => a.Script = (IStoredScript) v));

    public PutScriptDescriptor Mustache(string source) => this.Assign<MustacheScript>(new MustacheScript(source), (Action<IPutScriptRequest, MustacheScript>) ((a, v) => a.Script = (IStoredScript) v));
  }
}
