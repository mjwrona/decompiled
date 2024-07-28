// Decompiled with JetBrains decompiler
// Type: Nest.PutTransformDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.TransformApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class PutTransformDescriptor<TDocument> : 
    RequestDescriptorBase<PutTransformDescriptor<TDocument>, PutTransformRequestParameters, IPutTransformRequest>,
    IPutTransformRequest,
    IRequest<PutTransformRequestParameters>,
    IRequest
    where TDocument : class
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.TransformPut;

    public PutTransformDescriptor(Id transformId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("transform_id", (IUrlParameter) transformId)))
    {
    }

    [SerializationConstructor]
    protected PutTransformDescriptor()
    {
    }

    Id IPutTransformRequest.TransformId => this.Self.RouteValues.Get<Id>("transform_id");

    public PutTransformDescriptor<TDocument> DeferValidation(bool? defervalidation = true) => this.Qs("defer_validation", (object) defervalidation);

    public PutTransformDescriptor<TDocument> Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);

    string IPutTransformRequest.Description { get; set; }

    ITransformSource IPutTransformRequest.Source { get; set; }

    ITransformDestination IPutTransformRequest.Destination { get; set; }

    Time IPutTransformRequest.Frequency { get; set; }

    ITransformPivot IPutTransformRequest.Pivot { get; set; }

    ITransformSyncContainer IPutTransformRequest.Sync { get; set; }

    ITransformSettings IPutTransformRequest.Settings { get; set; }

    public PutTransformDescriptor<TDocument> Description(string description) => this.Assign<string>(description, (Action<IPutTransformRequest, string>) ((a, v) => a.Description = v));

    public PutTransformDescriptor<TDocument> Source(
      Func<TransformSourceDescriptor<TDocument>, ITransformSource> selector)
    {
      return this.Assign<ITransformSource>(selector.InvokeOrDefault<TransformSourceDescriptor<TDocument>, ITransformSource>(new TransformSourceDescriptor<TDocument>()), (Action<IPutTransformRequest, ITransformSource>) ((a, v) => a.Source = v));
    }

    public PutTransformDescriptor<TDocument> Destination(
      Func<TransformDestinationDescriptor, ITransformDestination> selector)
    {
      return this.Assign<ITransformDestination>(selector.InvokeOrDefault<TransformDestinationDescriptor, ITransformDestination>(new TransformDestinationDescriptor()), (Action<IPutTransformRequest, ITransformDestination>) ((a, v) => a.Destination = v));
    }

    public PutTransformDescriptor<TDocument> Frequency(Time frequency) => this.Assign<Time>(frequency, (Action<IPutTransformRequest, Time>) ((a, v) => a.Frequency = v));

    public PutTransformDescriptor<TDocument> Pivot(
      Func<TransformPivotDescriptor<TDocument>, ITransformPivot> selector)
    {
      return this.Assign<ITransformPivot>(selector.InvokeOrDefault<TransformPivotDescriptor<TDocument>, ITransformPivot>(new TransformPivotDescriptor<TDocument>()), (Action<IPutTransformRequest, ITransformPivot>) ((a, v) => a.Pivot = v));
    }

    public PutTransformDescriptor<TDocument> Sync(
      Func<TransformSyncContainerDescriptor<TDocument>, ITransformSyncContainer> selector)
    {
      return this.Assign<ITransformSyncContainer>(selector != null ? selector(new TransformSyncContainerDescriptor<TDocument>()) : (ITransformSyncContainer) null, (Action<IPutTransformRequest, ITransformSyncContainer>) ((a, v) => a.Sync = v));
    }

    public PutTransformDescriptor<TDocument> Settings(
      Func<TransformSettingsDescriptor, ITransformSettings> selector)
    {
      return this.Assign<ITransformSettings>(selector != null ? selector(new TransformSettingsDescriptor()) : (ITransformSettings) null, (Action<IPutTransformRequest, ITransformSettings>) ((a, v) => a.Settings = v));
    }
  }
}
