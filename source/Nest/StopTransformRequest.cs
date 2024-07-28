// Decompiled with JetBrains decompiler
// Type: Nest.StopTransformRequest
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
  public class StopTransformRequest : 
    PlainRequestBase<StopTransformRequestParameters>,
    IStopTransformRequest,
    IRequest<StopTransformRequestParameters>,
    IRequest
  {
    protected IStopTransformRequest Self => (IStopTransformRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.TransformStop;

    public StopTransformRequest(Id transformId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("transform_id", (IUrlParameter) transformId)))
    {
    }

    [SerializationConstructor]
    protected StopTransformRequest()
    {
    }

    [IgnoreDataMember]
    Id IStopTransformRequest.TransformId => this.Self.RouteValues.Get<Id>("transform_id");

    public bool? AllowNoMatch
    {
      get => this.Q<bool?>("allow_no_match");
      set => this.Q("allow_no_match", (object) value);
    }

    public bool? Force
    {
      get => this.Q<bool?>("force");
      set => this.Q("force", (object) value);
    }

    public Time Timeout
    {
      get => this.Q<Time>("timeout");
      set => this.Q("timeout", (object) value);
    }

    public bool? WaitForCheckpoint
    {
      get => this.Q<bool?>("wait_for_checkpoint");
      set => this.Q("wait_for_checkpoint", (object) value);
    }

    public bool? WaitForCompletion
    {
      get => this.Q<bool?>("wait_for_completion");
      set => this.Q("wait_for_completion", (object) value);
    }
  }
}
