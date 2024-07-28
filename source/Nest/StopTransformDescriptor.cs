// Decompiled with JetBrains decompiler
// Type: Nest.StopTransformDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.TransformApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class StopTransformDescriptor : 
    RequestDescriptorBase<StopTransformDescriptor, StopTransformRequestParameters, IStopTransformRequest>,
    IStopTransformRequest,
    IRequest<StopTransformRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.TransformStop;

    public StopTransformDescriptor(Id transformId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("transform_id", (IUrlParameter) transformId)))
    {
    }

    [SerializationConstructor]
    protected StopTransformDescriptor()
    {
    }

    Id IStopTransformRequest.TransformId => this.Self.RouteValues.Get<Id>("transform_id");

    public StopTransformDescriptor AllowNoMatch(bool? allownomatch = true) => this.Qs("allow_no_match", (object) allownomatch);

    public StopTransformDescriptor Force(bool? force = true) => this.Qs(nameof (force), (object) force);

    public StopTransformDescriptor Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);

    public StopTransformDescriptor WaitForCheckpoint(bool? waitforcheckpoint = true) => this.Qs("wait_for_checkpoint", (object) waitforcheckpoint);

    public StopTransformDescriptor WaitForCompletion(bool? waitforcompletion = true) => this.Qs("wait_for_completion", (object) waitforcompletion);
  }
}
