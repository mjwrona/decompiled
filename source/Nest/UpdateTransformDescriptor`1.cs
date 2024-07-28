// Decompiled with JetBrains decompiler
// Type: Nest.UpdateTransformDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.TransformApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class UpdateTransformDescriptor<TDocument> : 
    RequestDescriptorBase<UpdateTransformDescriptor<TDocument>, UpdateTransformRequestParameters, IUpdateTransformRequest>,
    IUpdateTransformRequest,
    IRequest<UpdateTransformRequestParameters>,
    IRequest
    where TDocument : class
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.TransformUpdate;

    public UpdateTransformDescriptor(Id transformId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("transform_id", (IUrlParameter) transformId)))
    {
    }

    [SerializationConstructor]
    protected UpdateTransformDescriptor()
    {
    }

    Id IUpdateTransformRequest.TransformId => this.Self.RouteValues.Get<Id>("transform_id");

    public UpdateTransformDescriptor<TDocument> DeferValidation(bool? defervalidation = true) => this.Qs("defer_validation", (object) defervalidation);

    public UpdateTransformDescriptor<TDocument> Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);

    string IUpdateTransformRequest.Description { get; set; }

    ITransformSource IUpdateTransformRequest.Source { get; set; }

    ITransformDestination IUpdateTransformRequest.Destination { get; set; }

    Time IUpdateTransformRequest.Frequency { get; set; }

    ITransformSyncContainer IUpdateTransformRequest.Sync { get; set; }

    ITransformSettings IUpdateTransformRequest.Settings { get; set; }

    public UpdateTransformDescriptor<TDocument> Description(string description) => this.Assign<string>(description, (Action<IUpdateTransformRequest, string>) ((a, v) => a.Description = v));

    public UpdateTransformDescriptor<TDocument> Source(
      Func<TransformSourceDescriptor<TDocument>, ITransformSource> selector)
    {
      return this.Assign<ITransformSource>(selector.InvokeOrDefault<TransformSourceDescriptor<TDocument>, ITransformSource>(new TransformSourceDescriptor<TDocument>()), (Action<IUpdateTransformRequest, ITransformSource>) ((a, v) => a.Source = v));
    }

    public UpdateTransformDescriptor<TDocument> Destination(
      Func<TransformDestinationDescriptor, ITransformDestination> selector)
    {
      return this.Assign<ITransformDestination>(selector.InvokeOrDefault<TransformDestinationDescriptor, ITransformDestination>(new TransformDestinationDescriptor()), (Action<IUpdateTransformRequest, ITransformDestination>) ((a, v) => a.Destination = v));
    }

    public UpdateTransformDescriptor<TDocument> Frequency(Time frequency) => this.Assign<Time>(frequency, (Action<IUpdateTransformRequest, Time>) ((a, v) => a.Frequency = v));

    public UpdateTransformDescriptor<TDocument> Sync(
      Func<TransformSyncContainerDescriptor<TDocument>, ITransformSyncContainer> selector)
    {
      return this.Assign<ITransformSyncContainer>(selector != null ? selector(new TransformSyncContainerDescriptor<TDocument>()) : (ITransformSyncContainer) null, (Action<IUpdateTransformRequest, ITransformSyncContainer>) ((a, v) => a.Sync = v));
    }

    public UpdateTransformDescriptor<TDocument> Settings(
      Func<TransformSettingsDescriptor, ITransformSettings> selector)
    {
      return this.Assign<ITransformSettings>(selector != null ? selector(new TransformSettingsDescriptor()) : (ITransformSettings) null, (Action<IUpdateTransformRequest, ITransformSettings>) ((a, v) => a.Settings = v));
    }
  }
}
