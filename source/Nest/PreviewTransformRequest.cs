// Decompiled with JetBrains decompiler
// Type: Nest.PreviewTransformRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.TransformApi;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class PreviewTransformRequest : 
    PlainRequestBase<PreviewTransformRequestParameters>,
    IPreviewTransformRequest,
    IRequest<PreviewTransformRequestParameters>,
    IRequest
  {
    protected IPreviewTransformRequest Self => (IPreviewTransformRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.TransformPreview;

    public PreviewTransformRequest(Id transformId)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional("transform_id", (IUrlParameter) transformId)))
    {
    }

    public PreviewTransformRequest()
    {
    }

    [IgnoreDataMember]
    Id IPreviewTransformRequest.TransformId => this.Self.RouteValues.Get<Id>("transform_id");

    public Time Timeout
    {
      get => this.Q<Time>("timeout");
      set => this.Q("timeout", (object) value);
    }

    public string Description { get; set; }

    public ITransformSource Source { get; set; }

    public ITransformDestination Destination { get; set; }

    public Time Frequency { get; set; }

    public ITransformPivot Pivot { get; set; }

    public ITransformSyncContainer Sync { get; set; }

    [DataMember(Name = "settings")]
    public ITransformSettings Settings { get; set; }
  }
}
