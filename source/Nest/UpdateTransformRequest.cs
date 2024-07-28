// Decompiled with JetBrains decompiler
// Type: Nest.UpdateTransformRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.TransformApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class UpdateTransformRequest : 
    PlainRequestBase<UpdateTransformRequestParameters>,
    IUpdateTransformRequest,
    IRequest<UpdateTransformRequestParameters>,
    IRequest
  {
    protected IUpdateTransformRequest Self => (IUpdateTransformRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.TransformUpdate;

    public UpdateTransformRequest(Id transformId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("transform_id", (IUrlParameter) transformId)))
    {
    }

    [SerializationConstructor]
    protected UpdateTransformRequest()
    {
    }

    [IgnoreDataMember]
    Id IUpdateTransformRequest.TransformId => this.Self.RouteValues.Get<Id>("transform_id");

    public bool? DeferValidation
    {
      get => this.Q<bool?>("defer_validation");
      set => this.Q("defer_validation", (object) value);
    }

    public Time Timeout
    {
      get => this.Q<Time>("timeout");
      set => this.Q("timeout", (object) value);
    }

    public string Description { get; set; }

    public ITransformSource Source { get; set; }

    public ITransformDestination Destination { get; set; }

    public Time Frequency { get; set; }

    public ITransformSyncContainer Sync { get; set; }

    public ITransformSettings Settings { get; set; }
  }
}
