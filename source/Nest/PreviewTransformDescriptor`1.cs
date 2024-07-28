// Decompiled with JetBrains decompiler
// Type: Nest.PreviewTransformDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.TransformApi;
using System;

namespace Nest
{
  public class PreviewTransformDescriptor<TDocument> : 
    RequestDescriptorBase<PreviewTransformDescriptor<TDocument>, PreviewTransformRequestParameters, IPreviewTransformRequest>,
    IPreviewTransformRequest,
    IRequest<PreviewTransformRequestParameters>,
    IRequest
    where TDocument : class
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.TransformPreview;

    public PreviewTransformDescriptor(Id transformId)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional("transform_id", (IUrlParameter) transformId)))
    {
    }

    public PreviewTransformDescriptor()
    {
    }

    Id IPreviewTransformRequest.TransformId => this.Self.RouteValues.Get<Id>("transform_id");

    public PreviewTransformDescriptor<TDocument> TransformId(Id transformId) => this.Assign<Id>(transformId, (Action<IPreviewTransformRequest, Id>) ((a, v) => a.RouteValues.Optional("transform_id", (IUrlParameter) v)));

    public PreviewTransformDescriptor<TDocument> Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);

    string IPreviewTransformRequest.Description { get; set; }

    ITransformSource IPreviewTransformRequest.Source { get; set; }

    ITransformDestination IPreviewTransformRequest.Destination { get; set; }

    Time IPreviewTransformRequest.Frequency { get; set; }

    ITransformPivot IPreviewTransformRequest.Pivot { get; set; }

    ITransformSyncContainer IPreviewTransformRequest.Sync { get; set; }

    ITransformSettings IPreviewTransformRequest.Settings { get; set; }

    public PreviewTransformDescriptor<TDocument> Description(string description) => this.Assign<string>(description, (Action<IPreviewTransformRequest, string>) ((a, v) => a.Description = v));

    public PreviewTransformDescriptor<TDocument> Source(
      Func<TransformSourceDescriptor<TDocument>, ITransformSource> selector)
    {
      return this.Assign<ITransformSource>(selector.InvokeOrDefault<TransformSourceDescriptor<TDocument>, ITransformSource>(new TransformSourceDescriptor<TDocument>()), (Action<IPreviewTransformRequest, ITransformSource>) ((a, v) => a.Source = v));
    }

    public PreviewTransformDescriptor<TDocument> Destination(
      Func<TransformDestinationDescriptor, ITransformDestination> selector)
    {
      return this.Assign<ITransformDestination>(selector.InvokeOrDefault<TransformDestinationDescriptor, ITransformDestination>(new TransformDestinationDescriptor()), (Action<IPreviewTransformRequest, ITransformDestination>) ((a, v) => a.Destination = v));
    }

    public PreviewTransformDescriptor<TDocument> Frequency(Time frequency) => this.Assign<Time>(frequency, (Action<IPreviewTransformRequest, Time>) ((a, v) => a.Frequency = v));

    public PreviewTransformDescriptor<TDocument> Pivot(
      Func<TransformPivotDescriptor<TDocument>, ITransformPivot> selector)
    {
      return this.Assign<ITransformPivot>(selector.InvokeOrDefault<TransformPivotDescriptor<TDocument>, ITransformPivot>(new TransformPivotDescriptor<TDocument>()), (Action<IPreviewTransformRequest, ITransformPivot>) ((a, v) => a.Pivot = v));
    }

    public PreviewTransformDescriptor<TDocument> Sync(
      Func<TransformSyncContainerDescriptor<TDocument>, ITransformSyncContainer> selector)
    {
      return this.Assign<ITransformSyncContainer>(selector.InvokeOrDefault<TransformSyncContainerDescriptor<TDocument>, ITransformSyncContainer>(new TransformSyncContainerDescriptor<TDocument>()), (Action<IPreviewTransformRequest, ITransformSyncContainer>) ((a, v) => a.Sync = v));
    }

    public PreviewTransformDescriptor<TDocument> Settings(
      Func<TransformSettingsDescriptor, ITransformSettings> selector)
    {
      return this.Assign<ITransformSettings>(selector != null ? selector(new TransformSettingsDescriptor()) : (ITransformSettings) null, (Action<IPreviewTransformRequest, ITransformSettings>) ((a, v) => a.Settings = v));
    }
  }
}
